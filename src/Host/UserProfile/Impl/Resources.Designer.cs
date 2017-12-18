﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.R.Host.UserProfile {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.R.Host.UserProfile.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Client took too long to respond..
        /// </summary>
        internal static string Error_ClientTimedOut {
            get {
                return ResourceManager.GetString("Error_ClientTimedOut", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to delete user profile for user {0}\{1}, with Win32 error {2}..
        /// </summary>
        internal static string Error_DeleteUserProfileFailed {
            get {
                return ResourceManager.GetString("Error_DeleteUserProfileFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to logon user {0}\{1}, with Win32 error {2}..
        /// </summary>
        internal static string Error_UserLogonFailed {
            get {
                return ResourceManager.GetString("Error_UserLogonFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to create user profile for user {0}\{1}, with Win32 error {2}..
        /// </summary>
        internal static string Error_UserProfileCreateFailed {
            get {
                return ResourceManager.GetString("Error_UserProfileCreateFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User profile directory expected but not found for user {0}\{1}, Win32 error {0}.
        /// </summary>
        internal static string Error_UserProfileDirectoryWasNotFound {
            get {
                return ResourceManager.GetString("Error_UserProfileDirectoryWasNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User profile service failed with exception: {0}..
        /// </summary>
        internal static string Error_UserProfileServiceError {
            get {
                return ResourceManager.GetString("Error_UserProfileServiceError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User profile deleted for user {0}\{1}..
        /// </summary>
        internal static string Info_DeletedUserProfile {
            get {
                return ResourceManager.GetString("Info_DeletedUserProfile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deleting user profile for user {0}\{1}..
        /// </summary>
        internal static string Info_DeletingUserProfile {
            get {
                return ResourceManager.GetString("Info_DeletingUserProfile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User profile already exists for user  {0}\{1}..
        /// </summary>
        internal static string Info_UserProfileAlreadyExists {
            get {
                return ResourceManager.GetString("Info_UserProfileAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Created user profile for user {0}\{1}..
        /// </summary>
        internal static string Info_UserProfileCreated {
            get {
                return ResourceManager.GetString("Info_UserProfileCreated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User profile directory was found for user {0}\{1}, path: {2}.
        /// </summary>
        internal static string Info_UserProfileDirectoryFound {
            get {
                return ResourceManager.GetString("Info_UserProfileDirectoryFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creates Windows User profile for users connecting via R Host Broker.
        /// </summary>
        internal static string Text_ServiceDescription {
            get {
                return ResourceManager.GetString("Text_ServiceDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to R User Profile Service.
        /// </summary>
        internal static string Text_ServiceDisplayName {
            get {
                return ResourceManager.GetString("Text_ServiceDisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RUserProfileService.
        /// </summary>
        internal static string Text_ServiceName {
            get {
                return ResourceManager.GetString("Text_ServiceName", resourceCulture);
            }
        }
    }
}
