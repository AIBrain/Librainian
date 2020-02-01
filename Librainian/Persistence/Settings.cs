// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Settings.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "Settings.cs" was last formatted by Protiguous on 2020/01/31 at 12:29 AM.

namespace Librainian.Persistence {

    using System;
    using Extensions;
    using JetBrains.Annotations;
    using Logging;
    using Microsoft.Win32;
    using Parsing;

    /// <summary>Store and retrieve values from the registry.</summary>
    public static class Settings {

        public static Boolean? GetBoolean( TrimmedString location, TrimmedString key ) {
            var result = GetObject( location, key );

            return result is null ? ( Boolean? ) null : Convert.ToBoolean( result );
        }

        public static Byte? GetByte( TrimmedString location, TrimmedString key ) {
            var result = GetObject( location, key );

            return result is null ? ( Byte? ) null : Convert.ToByte( result );
        }

        public static Int32? GetInt32( TrimmedString location, TrimmedString key ) {
            var result = GetObject( location, key );

            return result is null ? ( Int32? ) null : Convert.ToInt32( result );
        }

        public static Int64? GetInt64( TrimmedString location, TrimmedString key ) {
            var result = GetObject( location, key );

            return result is null ? ( Int64? ) null : Convert.ToInt64( result );
        }

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="location"></param>
        /// <param name="key">     </param>
        /// <returns></returns>
        [CanBeNull]
        public static Object GetObject( TrimmedString location, TrimmedString key ) => AppRegistry.Get( location, key );

        /// <summary>
        ///     <para>Gets the value of the current user's software's company's application's folder's key's subkey.</para>
        /// </summary>
        /// <param name="location"></param>
        /// <param name="key">     </param>
        /// <param name="subkey"></param>
        /// <returns></returns>
        [CanBeNull]
        public static Object GetObject( TrimmedString location, TrimmedString key, TrimmedString subkey ) => AppRegistry.Get( location, key, subkey );

        [CanBeNull]
        public static String GetString( TrimmedString location, TrimmedString key ) => Convert.ToString( GetObject( location, key ) ).NullIf( String.Empty );

        /// <summary>
        ///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key">   </param>
        /// <param name="value"> </param>
        public static void Set( TrimmedString folder, TrimmedString key, [CanBeNull] Object value ) {
            try {
                if ( folder.IsEmpty() ) {
                    throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( folder ) );
                }

                if ( key.IsEmpty() ) {
                    throw new ArgumentException( message: "Value cannot be null or whitespace.", nameof( key ) );
                }

                if ( value is null ) {
                    AppRegistry.Set( folder, key, null, RegistryValueKind.DWord );

                    return;
                }

                switch ( value ) {
                    case String s: {
                        AppRegistry.Set( folder, key, s );

                        break;
                    }

                    case UInt64 u64: {
                        AppRegistry.Set( folder, key, u64, RegistryValueKind.QWord );

                        break;
                    }

                    case Int64 i64: {
                        AppRegistry.Set( folder, key, i64, RegistryValueKind.QWord );

                        break;
                    }

                    case UInt32 u32: {
                        AppRegistry.Set( folder, key, u32, RegistryValueKind.DWord );

                        break;
                    }

                    case Int32 i32: {
                        AppRegistry.Set( folder, key, i32, RegistryValueKind.DWord );

                        break;
                    }

                    case Boolean b: {
                        AppRegistry.Set( folder, key, b ? 1 : 0, RegistryValueKind.DWord );

                        break;
                    }

                    case Enum e: {
                        AppRegistry.Set( folder, key, e, RegistryValueKind.DWord );

                        break;
                    }

                    default: {
                        $"Registry: unknown type {value}.".Log( breakinto: true );
                        AppRegistry.Set( folder, key, value, RegistryValueKind.Unknown );

                        break;
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

    }

}