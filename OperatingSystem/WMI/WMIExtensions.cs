// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/WMIExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:55 PM

namespace Librainian.OperatingSystem.WMI {

    using System;
    using System.Management;

    public static class WMIExtensions {

        public static String Identifier( String wmiClass, String wmiProperty, String wmiMustBeTrue ) {
            using ( var managementClass = new ManagementClass( wmiClass ) ) {
                var instances = managementClass.GetInstances();
                foreach ( var baseObject in instances ) {
                    var managementObject = baseObject as ManagementObject;
                    if ( managementObject == null || ( managementObject[ wmiMustBeTrue ].ToString() != "True" ) ) {
                        continue;
                    }
                    try {
                        return managementObject[ wmiProperty ].ToString();
                    }
                    catch { }
                }
            }
            return String.Empty;
        }

        public static String Identifier( String wmiClass, String wmiProperty ) {
            using ( var managementClass = new ManagementClass( wmiClass ) ) {
                var instances = managementClass.GetInstances();
                foreach ( var baseObject in instances ) {
                    try {
                        var managementObject = baseObject as ManagementObject;
                        if ( managementObject != null ) {
                            return managementObject[ wmiProperty ].ToString();
                        }
                        break;
                    }
                    catch { }
                }
            }
            return String.Empty;
        }

        public static ManagementObjectCollection WmiQuery( String query ) {
            var oQuery = new ObjectQuery( query );
            using ( var oSearcher = new ManagementObjectSearcher( oQuery ) ) {
                return oSearcher.Get();
            }
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

    }
}