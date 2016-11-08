﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Common.Core;
using Microsoft.Common.Core.Disposables;
using Microsoft.Common.Core.Events;
using Microsoft.R.Components.InteractiveWorkflow;
using Microsoft.R.Components.PackageManager.Model;
using Microsoft.R.Components.Settings;
using Microsoft.R.Host.Client;
using Microsoft.R.Host.Client.Host;
using Microsoft.R.Host.Client.Session;
using Newtonsoft.Json.Linq;
using static System.FormattableString;

namespace Microsoft.R.Components.PackageManager.Implementation {
    internal class RPackageManager : IRPackageManager {
        private readonly IRSessionProvider _sessionProvider;
        private readonly IRSettings _settings;
        private readonly IRInteractiveWorkflow _interactiveWorkflow;
        private readonly DisposableBag _disposableBag;
        private readonly DirtyEventSource _loadedPackagesEvent;
        private readonly DirtyEventSource _installedPackagesEvent;
        private readonly DirtyEventSource _availablePackagesEvent;

        public event EventHandler LoadedPackagesInvalidated {
            add { _loadedPackagesEvent.Event += value; }
            remove { _loadedPackagesEvent.Event -= value; }
        }

        public event EventHandler InstalledPackagesInvalidated {
            add { _installedPackagesEvent.Event += value; }
            remove { _installedPackagesEvent.Event -= value; }
        }

        public event EventHandler AvailablePackagesInvalidated {
            add { _availablePackagesEvent.Event += value; }
            remove { _availablePackagesEvent.Event -= value; }
        }

        public IRPackageManagerVisualComponent VisualComponent { get; private set; }

        public RPackageManager(IRSettings settings, IRInteractiveWorkflow interactiveWorkflow, Action dispose) {
            _sessionProvider = interactiveWorkflow.RSessions;
            _settings = settings;
            _interactiveWorkflow = interactiveWorkflow;
            _loadedPackagesEvent = new DirtyEventSource(this);
            _installedPackagesEvent = new DirtyEventSource(this);
            _availablePackagesEvent = new DirtyEventSource(this);

            _disposableBag = DisposableBag.Create<RPackageManager>(dispose)
                .Add(() => _interactiveWorkflow.RSessions.BrokerChanged -= BrokerChanged)
                .Add(() => _interactiveWorkflow.RSession.Mutated -= RSessionMutated)
                .Add(() => _interactiveWorkflow.RSession.PackagesInstalled -= PackagesInstalled)
                .Add(() => _interactiveWorkflow.RSession.PackagesRemoved -= PackagesRemoved);

            _interactiveWorkflow.RSessions.BrokerChanged += BrokerChanged;
            _interactiveWorkflow.RSession.Mutated += RSessionMutated;
            _interactiveWorkflow.RSession.PackagesInstalled += PackagesInstalled;
            _interactiveWorkflow.RSession.PackagesRemoved += PackagesRemoved;
        }

        public IRPackageManagerVisualComponent GetOrCreateVisualComponent(IRPackageManagerVisualComponentContainerFactory visualComponentContainerFactory, int instanceId = 0) {
            if (VisualComponent != null) {
                return VisualComponent;
            }

            VisualComponent = visualComponentContainerFactory.GetOrCreate(this, instanceId).Component;
            return VisualComponent;
        }

        public async Task<IReadOnlyList<RPackage>> GetInstalledPackagesAsync() {
            _installedPackagesEvent.Reset();
            return await GetPackagesAsync(async eval => await eval.InstalledPackagesAsync());
        }

        public async Task<IReadOnlyList<RPackage>> GetAvailablePackagesAsync() {
            _availablePackagesEvent.Reset();
            return await GetPackagesAsync(async eval => await eval.AvailablePackagesAsync());
        }

        public async Task InstallPackageAsync(string name, string libraryPath) {
            using (var request = await _interactiveWorkflow.RSession.BeginInteractionAsync()) {
                if (string.IsNullOrEmpty(libraryPath)) {
                    await request.InstallPackageAsync(name);
                } else {
                    await request.InstallPackageAsync(name, libraryPath);
                }
            }
        }

        public Task<PackageLockState> UninstallPackageAsync(string name, string libraryPath) => 
            _interactiveWorkflow.RSession.EvaluateAsync<PackageLockState>(
                Invariant($"rtvs:::package_uninstall({name.ToRStringLiteral()}, {libraryPath.ToRStringLiteral()})"), REvaluationKind.Normal);

        public Task<PackageLockState> UpdatePackageAsync(string name, string libraryPath) => 
            _interactiveWorkflow.RSession.EvaluateAsync<PackageLockState>(
                Invariant($"rtvs:::package_update({name.ToRStringLiteral()}, {libraryPath.ToRStringLiteral()})"), REvaluationKind.Normal);

        public async Task LoadPackageAsync(string name, string libraryPath) {
            using (var request = await _interactiveWorkflow.RSession.BeginInteractionAsync()) {
                if (string.IsNullOrEmpty(libraryPath)) {
                    await request.LoadPackageAsync(name);
                } else {
                    await request.LoadPackageAsync(name, libraryPath);
                }
            }
        }

        public async Task UnloadPackageAsync(string name) {
            using (var request = await _interactiveWorkflow.RSession.BeginInteractionAsync()) {
                await request.UnloadPackageAsync(name);
            }
        }

        public async Task<string[]> GetLoadedPackagesAsync() {
            _loadedPackagesEvent.Reset();
            var result = await WrapRException(_interactiveWorkflow.RSession.LoadedPackagesAsync());
            return result.Select(p => (string)((JValue)p).Value).ToArray();
        }

        public async Task<string> GetLibraryPathAsync() {
            var result = await WrapRException(_interactiveWorkflow.RSession.LibraryPathsAsync());
            return result.Select(p => p.ToRPath()).FirstOrDefault();
        }

        public Task<PackageLockState> GetPackageLockStateAsync(string name, string libraryPath) 
            => _interactiveWorkflow.RSession.EvaluateAsync<PackageLockState>(
                 Invariant($"rtvs:::package_lock_state({name.ToRStringLiteral()}, {libraryPath.ToRStringLiteral()})"), REvaluationKind.Normal);

        private async Task<IReadOnlyList<RPackage>> GetPackagesAsync(Func<IRExpressionEvaluator, Task<JArray>> queryFunc) {
            // Fetching of installed and available packages is done in a
            // separate package query session to avoid freezing the REPL.
            try {
                var startupInfo = new RHostStartupInfo {
                    Name = "PackageManager",
                    CranMirrorName = _settings.CranMirror,
                    CodePage = _settings.RCodePage
                };

                using (var eval = await _sessionProvider.BeginEvaluationAsync(startupInfo)) {
                    // Get the repos and libpaths from the REPL session and set them
                    // in the package query session
                    var repositories = (await DeparseRepositoriesAsync());
                    if (repositories != null) {
                        await WrapRException(eval.ExecuteAsync($"options(repos=eval(parse(text={repositories.ToRStringLiteral()})))"));
                    }
                    var libraries = (await DeparseLibrariesAsync());
                    if (libraries != null) { 
                        await WrapRException(eval.ExecuteAsync($".libPaths(eval(parse(text={libraries.ToRStringLiteral()})))"));
                    }

                    var result = await WrapRException(queryFunc(eval));
                    return result.Select(p => p.ToObject<RPackage>()).ToList().AsReadOnly();
                }
            } catch (RHostDisconnectedException ex) {
                throw new RPackageManagerException(Resources.PackageManager_TransportError, ex);
            }
        }

        private async Task<string> DeparseRepositoriesAsync() {
            try {
                return await WrapRException(_interactiveWorkflow.RSession.EvaluateAsync<string>("rtvs:::deparse_str(getOption('repos'))", REvaluationKind.Normal));
            } catch(RHostDisconnectedException) {
                return null;
            }
        }

        private async Task<string> DeparseLibrariesAsync() {
            try {
                return await WrapRException(_interactiveWorkflow.RSession.EvaluateAsync<string>("rtvs:::deparse_str(.libPaths())", REvaluationKind.Normal));
            } catch (RHostDisconnectedException) {
                return null;
            }
        }

        public void Dispose() {
            if (_disposableBag.TryDispose()) {
                VisualComponent?.Dispose();
            }
        }

        private async Task WrapRException(Task task) {
            try {
                await task;
            } catch (RException ex) {
                throw new RPackageManagerException(string.Format(CultureInfo.InvariantCulture, Resources.PackageManager_EvalError, ex.Message), ex);
            }
        }

        private async Task<T> WrapRException<T>(Task<T> task) {
            try {
                return await task;
            } catch (RException ex) {
                throw new RPackageManagerException(string.Format(CultureInfo.InvariantCulture, Resources.PackageManager_EvalError, ex.Message), ex);
            }
        }

        private void BrokerChanged(object sender, EventArgs e) {
            _availablePackagesEvent.FireOnce();
            _installedPackagesEvent.FireOnce();
            _loadedPackagesEvent.FireOnce();
        }

        private void PackagesInstalled(object sender, EventArgs e) {
            _installedPackagesEvent.FireOnce();
            _loadedPackagesEvent.FireOnce();
        }

        private void PackagesRemoved(object sender, EventArgs e) {
            _installedPackagesEvent.FireOnce();
            _loadedPackagesEvent.FireOnce();
        }
        
        private void RSessionMutated(object sender, EventArgs e) {
            _loadedPackagesEvent.FireOnce();
        }
    }
}
