// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "CurrentUser.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "CurrentUser.cs" was last formatted by Protiguous on 2020/01/31 at 12:29 AM.

namespace Librainian.Persistence {

    using System;
    using System.Windows.Forms;
    using JetBrains.Annotations;
    using Logging;
    using Microsoft.Win32;

    public static class CurrentUser {

        /// <summary>
        ///     <para>The current executable's name under the <see cref="Protiguous" /> key.</para>
        ///     <para>Use this one for setting and getting.</para>
        /// </summary>
        /// <value></value>
        [NotNull]
        internal static RegistryKey App => Protiguous.CreateSubKey( Application.ProductName, true );

        /// <summary>Current user.</summary>
        [NotNull]
        public static RegistryKey HKCU => Registry.CurrentUser;

        /// <summary>The <see cref="Protiguous" /> key under the <see cref="Software" /> key.</summary>
        [NotNull]
        public static RegistryKey Protiguous => Software.CreateSubKey( nameof( Protiguous ), true );

        /// <summary>The <see cref="Software" /> key under the <see cref="HKCU" /> key.</summary>
        [NotNull]
        public static RegistryKey Software => HKCU.CreateSubKey( nameof( Software ), true );

        /// <summary>By default, this retrieves the registry key under HKCU\Software\Protiguous\ProcessName.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        [CanBeNull]
        public static T Get<T>( [NotNull] this String key ) {

            if ( App is null ) {
                throw new ArgumentNullException( nameof( App ) );
            }

            if ( String.IsNullOrWhiteSpace( value: key ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( key ) );
            }

            var value = App.GetValue( key );

            if ( value is T result ) {
                return result;
            }

            return default;
        }

        /// <summary>By default, this stores the key under HKCU\Software\Protiguous\ProcessName.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean Set<T>( [NotNull] this String key, [CanBeNull] T value ) {

            if ( App is null ) {
                throw new ArgumentNullException( nameof( App ) );
            }

            if ( String.IsNullOrWhiteSpace( value: key ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( key ) );
            }

            try {
                switch ( value ) {
                    case String[] _: {
                            App.SetValue( key, value, RegistryValueKind.MultiString );

                            return true;
                        }

                    case String _:
                        App.SetValue( key, value, RegistryValueKind.String );

                        return true;

                    case UInt64 _:
                    case Int64 _:
                        App.SetValue( key, value, RegistryValueKind.QWord );

                        return true;

                    case UInt32 _:
                    case Int32 _:
                    case UInt16 _:
                    case Int16 _:
                    case SByte _:
                    case Byte _:
                        App.SetValue( key, value, RegistryValueKind.DWord );

                        return true;

                    case Byte[] _:
                        App.SetValue( key, value, RegistryValueKind.Binary );

                        return true;

                    default: {

                            // ReSharper disable once AssignNullToNotNullAttribute
                            App.SetValue( key, value, RegistryValueKind.Unknown );

                            return true;
                        }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }
    }
}