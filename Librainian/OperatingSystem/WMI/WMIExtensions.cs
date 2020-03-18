// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "WMIExtensions.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", File: "WMIExtensions.cs" was last formatted by Protiguous on 2020/03/16 at 5:07 PM.

namespace Librainian.OperatingSystem.WMI {

    using System;
    using System.Management;
    using Converters;
    using JetBrains.Annotations;
    using Parsing;

    public static class WMIExtensions {

        [NotNull]
        public static String Identifier( [NotNull] String wmiClass, [NotNull] String wmiProperty, [NotNull] String wmiMustBeTrue ) {
            if ( String.IsNullOrWhiteSpace( value: wmiClass ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( wmiClass ) );
            }

            if ( String.IsNullOrWhiteSpace( value: wmiProperty ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( wmiProperty ) );
            }

            if ( String.IsNullOrWhiteSpace( value: wmiMustBeTrue ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( wmiMustBeTrue ) );
            }

            using ( var managementClass = new ManagementClass( path: wmiClass ) ) {
                var instances = managementClass.GetInstances();

                foreach ( var baseObject in instances ) {
                    if ( !( baseObject is ManagementObject managementObject ) || !managementObject[ propertyName: wmiMustBeTrue ].ToBoolean() ) {
                        continue;
                    }

                    try {
                        return managementObject[ propertyName: wmiProperty ].ToString();
                    }
                    catch {

                        // ignored
                    }
                }
            }

            return String.Empty;
        }

        [NotNull]
        public static String Identifier( [NotNull] String wmiClass, [NotNull] String wmiProperty ) {
            if ( String.IsNullOrWhiteSpace( value: wmiClass ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( wmiClass ) );
            }

            if ( String.IsNullOrWhiteSpace( value: wmiProperty ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( wmiProperty ) );
            }

            using ( var managementClass = new ManagementClass( path: wmiClass ) ) {
                var instances = managementClass.GetInstances();

                foreach ( var baseObject in instances ) {
                    try {
                        if ( baseObject is ManagementObject managementObject ) {
                            return managementObject[ propertyName: wmiProperty ].ToString();
                        }
                    }
                    catch {

                        // ignored
                    }
                }
            }

            return String.Empty;
        }

        [NotNull]
        public static ManagementObjectCollection QueryWMI( [CanBeNull] String? machineName, [NotNull] String scope, [NotNull] String query ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            if ( String.IsNullOrWhiteSpace( value: scope ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( scope ) );
            }

            var conn = new ConnectionOptions();
            var nameSpace = @"\\";
            machineName = machineName.Trimmed();
            nameSpace += machineName != String.Empty ? machineName : ".";
            nameSpace += $@"\root\{scope}";
            var managementScope = new ManagementScope( path: nameSpace, options: conn );
            var wmiQuery = new ObjectQuery( query: query );

            using var moSearcher = new ManagementObjectSearcher( scope: managementScope, query: wmiQuery );

            return moSearcher.Get();
        }

        [NotNull]
        public static ManagementObjectCollection WmiQuery( [NotNull] String query ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            var oQuery = new ObjectQuery( query: query );

            using var oSearcher = new ManagementObjectSearcher( query: oQuery );

            return oSearcher.Get();
        }
    }
}