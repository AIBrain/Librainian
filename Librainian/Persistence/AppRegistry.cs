// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "AppRegistry.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "AppRegistry.cs" was last formatted by Protiguous on 2019/08/08 at 9:27 AM.

namespace Librainian.Persistence {

    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using Collections.Extensions;
    using Converters;
    using Exceptions;
    using JetBrains.Annotations;
    using Logging;
    using Microsoft.Win32;
    using Parsing;

    public static class AppRegistry {

        /// <summary>
        ///     Registry key for the user's software.
        /// </summary>
        [NotNull]
        public static RegistryKey Software { get; }

        /// <summary>
        ///     Registry key for the application.
        /// </summary>
        [NotNull]
        public static RegistryKey TheApplication { get; }

        /// <summary>
        ///     Registry key for the product's company.
        /// </summary>
        [NotNull]
        public static RegistryKey TheCompany { get; }

        /// <summary>
        ///     Registry key for the current user;
        /// </summary>
        [NotNull]
        public static RegistryKey TheUser { get; } = Registry.CurrentUser ?? throw new InvalidOperationException();

        static AppRegistry() {
            if ( TheUser is null ) {
                throw new ArgumentEmptyException( $"Registry folder {nameof( Registry.CurrentUser )} is null!" );
            }

            Software = TheUser.CreateSubKey( nameof( Software ), true ) ?? throw new InvalidOperationException();

            if ( Software is null ) {
                throw new ArgumentEmptyException( $"Application {nameof( AppRegistry )} folder {nameof( Software )} is null!" );
            }

            var compName = Application.CompanyName.Trimmed();
            if ( String.IsNullOrEmpty( compName ) ) {
                throw new InvalidOperationException();
            }
            TheCompany = Software.CreateSubKey( compName.Replace( "&", Symbols.Singlespace ).Trim(), true ) ?? throw new InvalidOperationException();

            if ( TheCompany is null ) {
                throw new ArgumentEmptyException( $"Application {nameof( AppRegistry )} folder {nameof( Application.CompanyName )} is null!" );
            }

            var product = Application.ProductName.Trimmed();

            if ( String.IsNullOrEmpty( product ) ) {
                throw new InvalidOperationException();
            }
            TheApplication = TheCompany.CreateSubKey( product, true ) ?? throw new InvalidOperationException();

            if ( TheApplication is null ) {
                throw new ArgumentEmptyException( $"Application {nameof( AppRegistry )} folder {nameof( Application.ProductName )} is null!" );
            }
        }

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        /// <returns></returns>
        [CanBeNull]
        public static Object Get( [CanBeNull] String folder, [CanBeNull] String key ) {
            if ( folder.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( folder ) );
            }

            if ( key.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( key ) );
            }

            try {
                using ( var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {
                    return registryKey?.GetValue( key );
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return null;
        }

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        /// <param name="subkey"></param>
        /// <returns></returns>
        [CanBeNull]
        public static Object Get( [CanBeNull] String folder, [CanBeNull] String key, [CanBeNull] String subkey ) {
            if ( folder.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( folder ) );
            }

            if ( key.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( key ) );
            }

            if ( subkey.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( subkey ) );
            }

            try {
                using ( var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {
                    using ( var subKey = registryKey?.OpenSubKey( subkey, RegistryKeyPermissionCheck.ReadSubTree ) ) {
                        return subKey?.GetValue( key );
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return null;
        }

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        [Pure]
        [CanBeNull]
        public static Boolean? GetBoolean( [CanBeNull] String folder, [CanBeNull] String key ) {
            if ( folder.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( folder ) );
            }

            if ( key.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( key ) );
            }

            try {
                using ( var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {

                    return registryKey?.GetValue( key )?.ToBooleanOrNull();
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        /// <param name="subkey"></param>
        [Pure]
        [CanBeNull]
        public static Boolean? GetBoolean( [CanBeNull] String folder, [CanBeNull] String key, [CanBeNull] String subkey ) {
            if ( folder.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( folder ) );
            }

            if ( key.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( key ) );
            }

            if ( subkey.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( subkey ) );
            }

            try {
                using ( var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {
                    using ( var subKey = registryKey?.OpenSubKey( subkey, RegistryKeyPermissionCheck.ReadSubTree ) ) {
                        return subKey?.GetValue( key )?.ToBooleanOrNull();
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        [Pure]
        [CanBeNull]
        public static Byte? GetByte( [CanBeNull] String folder, [CanBeNull] String key ) {
            if ( folder.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( folder ) );
            }

            if ( key.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( key ) );
            }

            try {
                using ( var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {

                    return registryKey?.GetValue( key )?.ToByteOrNull();
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        /// <param name="subkey"></param>
        [Pure]
        [CanBeNull]
        public static Byte? GetByte( [CanBeNull] String folder, [CanBeNull] String key, [CanBeNull] String subkey ) {
            if ( folder.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( folder ) );
            }

            if ( key.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( key ) );
            }

            if ( subkey.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( subkey ) );
            }

            try {
                using ( var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {
                    using ( var subKey = registryKey?.OpenSubKey( subkey, RegistryKeyPermissionCheck.ReadSubTree ) ) {
                        return subKey?.GetValue( key )?.ToByteOrNull();
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        [Pure]
        [CanBeNull]
        public static Int32? GetInt32( [CanBeNull] String folder, [CanBeNull] String key ) {
            if ( folder.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( folder ) );
            }

            if ( key.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( key ) );
            }

            try {
                using ( var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {

                    return registryKey?.GetValue( key )?.ToIntOrNull();
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        /// <param name="subkey"></param>
        [Pure]
        [CanBeNull]
        public static Int32? GetInt32( [CanBeNull] String folder, [CanBeNull] String key, [CanBeNull] String subkey ) {
            if ( folder.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( folder ) );
            }

            if ( key.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( key ) );
            }

            if ( subkey.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( subkey ) );
            }

            try {
                using ( var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {
                    using ( var subKey = registryKey?.OpenSubKey( subkey, RegistryKeyPermissionCheck.ReadSubTree ) ) {
                        return subKey?.GetValue( key )?.ToIntOrNull();
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        [Pure]
        [CanBeNull]
        public static Int64? GetInt64( [CanBeNull] String folder, [CanBeNull] String key ) {
            if ( folder.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( folder ) );
            }

            if ( key.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( key ) );
            }

            try {
                using ( var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {

                    if ( Int64.TryParse( registryKey?.GetValue( key )?.ToString(), out var result ) ) {
                        return result;
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        /// <param name="subkey"></param>
        [Pure]
        [CanBeNull]
        public static Int64? GetInt64( [CanBeNull] String folder, [CanBeNull] String key, [CanBeNull] String subkey ) {
            if ( folder.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( folder ) );
            }

            if ( key.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( key ) );
            }

            if ( subkey.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( subkey ) );
            }

            try {
                using ( var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {
                    using ( var subKey = registryKey?.OpenSubKey( subkey, RegistryKeyPermissionCheck.ReadSubTree ) ) {

                        if ( Int64.TryParse( subKey?.GetValue( key )?.ToString(), out var result ) ) {
                            return result;
                        }
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        [Pure]
        [CanBeNull]
        [DebuggerStepThrough]
        public static String GetString( [CanBeNull] String folder, [CanBeNull] String key ) {
            if ( folder.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( folder ) );
            }

            if ( key.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( key ) );
            }

            try {
                using ( var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {

                    return registryKey?.GetValue( key )?.ToStringOrNull();
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        /// <param name="subkey"></param>
        [Pure]
        [CanBeNull]
        public static String GetString( [CanBeNull] String folder, [CanBeNull] String key, [CanBeNull] String subkey ) {
            if ( folder.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( folder ) );
            }

            if ( key.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( key ) );
            }

            if ( subkey.IsEmpty() ) {
                throw new ArgumentEmptyException( nameof( subkey ) );
            }

            try {
                using ( var registryKey = TheApplication.OpenSubKey( folder, RegistryKeyPermissionCheck.ReadSubTree ) ) {
                    using ( var subKey = registryKey?.OpenSubKey( subkey, RegistryKeyPermissionCheck.ReadSubTree ) ) {
                        return subKey?.GetValue( key )?.ToStringOrNull();
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        /// <param name="value"> </param>
        /// <param name="kind">  </param>
        /// <returns></returns>
        public static Boolean Set( [NotNull] String folder, [CanBeNull] String key, [CanBeNull] Object value, RegistryValueKind kind ) {
            if ( folder.IsEmpty() ) {
                throw new ArgumentException( "Value cannot be null or whitespace.",  nameof( folder ) );
            }

            if ( key.IsEmpty() ) {
                throw new ArgumentException( "Value cannot be null or whitespace.",  nameof( key ) );
            }

            using ( var regFolder = TheApplication.CreateSubKey( folder, RegistryKeyPermissionCheck.ReadWriteSubTree ) ) {
                if ( regFolder is null ) {
                    $"Error creating subkey {folder}".Break();

                    return false;
                }

                try {
                    if ( value is null ) {
                        regFolder.DeleteValue( key );
                    }
                    else {
                        regFolder.SetValue( key, value, kind );
                    }

                    return true;
                }
                catch ( Exception exception ) {
                    exception.Break();
                }

                return false;
            }
        }

        /// <summary>
        ///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        /// <param name="subkey"></param>
        /// <param name="value"> </param>
        /// <param name="kind">  </param>
        /// <returns></returns>
        public static Boolean Set<T>( [NotNull] String folder, [NotNull] String key, [NotNull] String subkey, [CanBeNull] T value, RegistryValueKind kind ) {

            if ( String.IsNullOrWhiteSpace( value: folder ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.",  nameof( folder ) );
            }

            if ( String.IsNullOrWhiteSpace( value: key ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.",  nameof( key ) );
            }

            if ( String.IsNullOrWhiteSpace( value: subkey ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.",  nameof( subkey ) );
            }

            using ( var registryKey = TheApplication.CreateSubKey( folder, RegistryKeyPermissionCheck.ReadWriteSubTree ) ) {
                if ( registryKey is null ) {
                    return false;
                }

                using ( var subKey = registryKey.CreateSubKey( subkey, RegistryKeyPermissionCheck.ReadWriteSubTree ) ) {
                    if ( subKey is null ) {
                        $"Error creating subkey {folder}".Break();

                        return false;
                    }

                    try {
                        if ( value is null ) {
                            subKey.DeleteValue( key );
                        }
                        else {
                            subKey.SetValue( key, value, kind );
                        }

                        return true;
                    }
                    catch ( Exception exception ) {
                        exception.Break();
                    }
                }

                return false;
            }
        }

        /// <summary>
        ///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        /// <param name="value"> </param>
        public static void Set( [NotNull] String folder, [CanBeNull] String key, [CanBeNull] String value ) => Set( folder, key, value, RegistryValueKind.String );

        /// <summary>
        ///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        /// <param name="value"> </param>
        public static void Set( [NotNull] String folder, [CanBeNull] String key, Int32 value ) => Set( folder, key, value, RegistryValueKind.DWord );

        /// <summary>
        ///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        /// <param name="value"> </param>
        public static void Set( [NotNull] String folder, [CanBeNull] String key, Int64 value ) => Set( folder, key, value, RegistryValueKind.QWord );
    }
}