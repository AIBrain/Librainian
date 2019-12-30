﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "RegistryContext.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "RegistryContext.cs" was last formatted by Protiguous on 2019/08/08 at 9:21 AM.

namespace LibrainianCore.OperatingSystem {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Mime;
    using Logging;

    public static class RegistryContext {

        /// <summary>
        ///     <para>Example <paramref name="menuName" />: "Copy Folder"</para>
        ///     <para>Example <paramref name="menuPath" />: "Folder\shell\Copy"</para>
        ///     <para>Example <paramref name="command" />: "command"</para>
        ///     <para>Example <paramref name="action" />: $"{Application.ExecutablePath} \"%1\""</para>
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="menuPath"></param>
        /// <param name="command"></param>
        /// <param name="action"></param>
        public static Boolean AddRegistryContext( [NotNull] String menuName, [NotNull] String menuPath, [CanBeNull] String command = null, [CanBeNull] String action = null ) {
            if ( String.IsNullOrWhiteSpace( value: menuPath ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( menuPath ) );
            }

            if ( String.IsNullOrWhiteSpace( value: menuName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( menuName ) );
            }

            try {
                using ( var registryKey = Registry.ClassesRoot.CreateSubKey( menuPath ) ) {
                    if ( registryKey != default ) {
                        registryKey.SetValue( String.Empty, menuName, RegistryValueKind.String );

                        if ( String.IsNullOrEmpty( command ) ) {
                            command = "command";
                        }

                        if ( String.IsNullOrEmpty( action ) ) {
                            action = $"{MediaTypeNames.Application.ExecutablePath} \"%1\" \"%2\" \"%3\" \"%4\"";
                        }

                        using ( var commandKey = registryKey.CreateSubKey( command ) ) {

                            if ( commandKey != default ) {
                                commandKey.SetValue( String.Empty, action, RegistryValueKind.String );
                            }
                            else {
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return false;
        }

        public static void RemoveRegistryContext( String menuPath, String registryCommand ) {

            try {
                try {
                    using ( var reg = Registry.ClassesRoot.OpenSubKey( registryCommand ) ) {
                        reg?.Close();
                    }
                }
                finally {
                    Registry.ClassesRoot.DeleteSubKey( registryCommand );
                }

                try {
                    using ( var reg2 = Registry.ClassesRoot.OpenSubKey( menuPath ) ) {
                        reg2?.Close();
                    }
                }
                finally {
                    Registry.ClassesRoot.DeleteSubKey( menuPath );
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }
    }
}