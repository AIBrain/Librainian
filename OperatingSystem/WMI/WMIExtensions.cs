﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "WMIExtensions.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/WMIExtensions.cs" was last formatted by Protiguous on 2018/05/21 at 11:17 PM.

namespace Librainian.OperatingSystem.WMI {

    using System;
    using System.Management;

    public static class WMIExtensions {

        public static String Identifier( String wmiClass, String wmiProperty, String wmiMustBeTrue ) {
            using ( var managementClass = new ManagementClass( wmiClass ) ) {
                var instances = managementClass.GetInstances();

                foreach ( var baseObject in instances ) {
                    if ( !( baseObject is ManagementObject managementObject ) || managementObject[wmiMustBeTrue].ToString() != "True" ) { continue; }

                    try { return managementObject[wmiProperty].ToString(); }
                    catch {

                        // ignored
                    }
                }
            }

            return String.Empty;
        }

        public static String Identifier( String wmiClass, String wmiProperty ) {
            using ( var managementClass = new ManagementClass( wmiClass ) ) {
                var instances = managementClass.GetInstances();

                foreach ( var baseObject in instances ) {
                    try {
                        if ( baseObject is ManagementObject managementObject ) { return managementObject[wmiProperty].ToString(); }
                    }
                    catch {

                        // ignored
                    }
                }
            }

            return String.Empty;
        }

        public static ManagementObjectCollection QueryWMI( String machineName, String scope, String query ) {
            var conn = new ConnectionOptions();
            var nameSpace = @"\\";
            nameSpace += machineName != String.Empty ? machineName : ".";
            nameSpace += @"\root\" + scope;
            var managementScope = new ManagementScope( nameSpace, conn );
            var wmiQuery = new ObjectQuery( query );
            var moSearcher = new ManagementObjectSearcher( managementScope, wmiQuery );

            return moSearcher.Get();
        }

        public static ManagementObjectCollection WmiQuery( String query ) {
            var oQuery = new ObjectQuery( query );

            using ( var oSearcher = new ManagementObjectSearcher( oQuery ) ) { return oSearcher.Get(); }
        }
    }
}