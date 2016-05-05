﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using static System.FormattableString;

namespace Microsoft.R.Debugger {
    public class DebugErrorEvaluationResult : DebugEvaluationResult {
        public string ErrorText { get; }

        internal DebugErrorEvaluationResult(DebugSession session, string environmentExpression, string expression, string name, string errorText)
            : base(session, environmentExpression, expression, name) {
            ErrorText = errorText;
        }

        public override string ToString() {
            return Invariant($"ERROR: {ErrorText}");
        }
    }
}