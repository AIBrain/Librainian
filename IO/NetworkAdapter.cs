#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/NetworkAdapter.cs" was last cleaned by Rick on 2014/09/07 at 3:23 AM

#endregion License & Information

namespace Librainian.IO {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using System.Threading;

    /// <summary>
    ///     Module Name: NetworkAdapter.cs
    ///     Project: CSWMIEnableDisableNetworkAdapter
    ///     Copyright (c) Microsoft Corporation.
    /// </summary>
    public class NetworkAdapter {

        public NetworkAdapter( int deviceId, String name, int netEnabled, int netConnectionStatus ) {
            this.DeviceId = deviceId;
            this.Name = name;
            this.NetEnabled = netEnabled;
            this.NetConnectionStatus = netConnectionStatus;
        }

        public NetworkAdapter( int deviceId ) {
            var strWQuery = String.Format( "SELECT DeviceID, ProductName, NetEnabled, NetConnectionStatus FROM Win32_NetworkAdapter WHERE DeviceID = {0}", deviceId );
            try {
                var networkAdapters = WMIOperation.WMIQuery( strWQuery );

                var crtNetworkAdapter = networkAdapters.Cast<ManagementBaseObject>().Select( o => o as ManagementObject ).FirstOrDefault();
                if ( null == crtNetworkAdapter ) {
                    return;
                }

                this.DeviceId = deviceId;

                this.Name = crtNetworkAdapter[ "ProductName" ].ToString();

                if ( Convert.ToBoolean( crtNetworkAdapter[ "NetEnabled" ].ToString() ) ) {
                    this.NetEnabled = ( int )EnumNetEnabledStatus.Enabled;
                }
                else {
                    this.NetEnabled = ( int )EnumNetEnabledStatus.Disabled;
                }

                this.NetConnectionStatus = Convert.ToInt32( crtNetworkAdapter[ "NetConnectionStatus" ].ToString() );
            }
            catch ( NullReferenceException ) {

                // If there is no a network adapter which deviceid equates to the argument
                // "deviceId" just to construct a none exists network adapter
                this.DeviceId = -1;
                this.Name = String.Empty;
                this.NetEnabled = 0;
                this.NetConnectionStatus = -1;
            }
        }

        /// <summary>
        ///     Enum the Operation result of Enable and Disable  Network Adapter
        /// </summary>
        private enum EnumEnableDisableResult {
            Fail = -1,
            Unknow,
            Success
        }

        ///// <summary>
        ///// The Net Connection Status Description
        ///// </summary>
        //public static String[] SaNetConnectionStatus =
        //{
        //    Resources.NetConnectionStatus0,
        //    Resources.NetConnectionStatus1,
        //    Resources.NetConnectionStatus2,
        //    Resources.NetConnectionStatus3,
        //    Resources.NetConnectionStatus4,
        //    Resources.NetConnectionStatus5,
        //    Resources.NetConnectionStatus6,
        //    Resources.NetConnectionStatus7,
        //    Resources.NetConnectionStatus8,
        //    Resources.NetConnectionStatus9,
        //    Resources.NetConnectionStatus10,
        //    Resources.NetConnectionStatus11,
        //    Resources.NetConnectionStatus12
        //};
        /// <summary>
        ///     Enum the NetEnabled Status
        /// </summary>
        private enum EnumNetEnabledStatus {
            Disabled = -1,
            Unknown,
            Enabled
        }

        /// <summary>
        ///     The DeviceID of the NetworkAdapter
        /// </summary>
        public int DeviceId {
            get;
            private set;
        }

        /// <summary>
        ///     The ProductName of the NetworkAdapter
        /// </summary>
        public String Name {
            get;
            private set;
        }

        /// <summary>
        ///     The Net Connection Status Value
        /// </summary>
        public int NetConnectionStatus {
            get;
            private set;
        }

        /// <summary>
        ///     The NetEnabled status of the NetworkAdapter
        /// </summary>
        public int NetEnabled {
            get;
            private set;
        }

        /// <summary>
        ///     List all the NetworkAdapters
        /// </summary>
        /// <returns>The list of all NetworkAdapter of the machine</returns>
        public static IEnumerable<NetworkAdapter> GetAllNetworkAdapters() {
            //var allNetworkAdapter = new List<NetworkAdapter>();

            // Manufacturer <> 'Microsoft'to get all nonvirtual devices.
            // Because the AdapterType property will be null if the NetworkAdapter is
            // disabled, so we do not use NetworkAdapter = 'Ethernet 802.3' or
            // NetworkAdapter = 'Wireless’

            var networkAdapters = WMIOperation.WMIQuery( "SELECT DeviceID, ProductName, NetEnabled, NetConnectionStatus FROM Win32_NetworkAdapter WHERE Manufacturer <> \'Microsoft\'" );
            return from ManagementBaseObject o in networkAdapters
                   select o as ManagementObject
                   into moNetworkAdapter
                   select new NetworkAdapter( Convert.ToInt32( moNetworkAdapter[ "DeviceID" ].ToString() ), moNetworkAdapter[ "ProductName" ].ToString(), ( Convert.ToBoolean( moNetworkAdapter[ "NetEnabled" ].ToString() ) ) ? ( int ) EnumNetEnabledStatus.Enabled : ( int ) EnumNetEnabledStatus.Disabled, Convert.ToInt32( moNetworkAdapter[ "NetConnectionStatus" ].ToString() ) );

            //return allNetworkAdapter;
        }

        /// <summary>
        ///     Enable Or Disable The NetworkAdapter
        /// </summary>
        /// <returns>
        ///     Whether the NetworkAdapter was enabled or disabled successfully
        /// </returns>
        public int EnableOrDisableNetworkAdapter( String strOperation ) {
            strOperation = strOperation.Trim();

            int resultEnableDisableNetworkAdapter;
            var crtNetworkAdapter = new ManagementObject();

            try {
                var networkAdapters = WMIOperation.WMIQuery( String.Format( "SELECT DeviceID, ProductName, NetEnabled, NetConnectionStatus FROM Win32_NetworkAdapter WHERE DeviceID = {0}", this.DeviceId ) );
                foreach ( var o in networkAdapters ) {
                    var networkAdapter = o as ManagementObject;
                    crtNetworkAdapter = networkAdapter;
                }

                if ( crtNetworkAdapter != null ) {
                    crtNetworkAdapter.InvokeMethod( strOperation, null );
                }

                Thread.Sleep( 100 );
                while ( this.GetNetEnabled() != ( strOperation.Equals( "Enable", StringComparison.OrdinalIgnoreCase ) ? ( int )EnumNetEnabledStatus.Enabled : ( int )EnumNetEnabledStatus.Disabled ) ) {
                    Thread.Sleep( 100 );
                }

                resultEnableDisableNetworkAdapter = ( int )EnumEnableDisableResult.Success;
            }
            catch ( NullReferenceException ) {

                // If there is a NullReferenceException, the result of the enable or
                // disable network adapter operation will be fail
                resultEnableDisableNetworkAdapter = ( int )EnumEnableDisableResult.Fail;
            }

            if ( crtNetworkAdapter != null ) {
                crtNetworkAdapter.Dispose();
            }

            return resultEnableDisableNetworkAdapter;
        }

        /// <summary>
        ///     Get the NetworkAdapter NetEnabled Property
        /// </summary>
        /// <returns>Whether the NetworkAdapter is enabled</returns>
        public int GetNetEnabled() {
            var netEnabled = ( int )EnumNetEnabledStatus.Unknown;
            try {
                var networkAdapters = WMIOperation.WMIQuery( String.Format( "SELECT NetEnabled FROM Win32_NetworkAdapter WHERE DeviceID = {0}", this.DeviceId ) );
                foreach ( var networkAdapter in networkAdapters.Cast<ManagementBaseObject>().Select( o => o as ManagementObject ) ) {
                    if ( Convert.ToBoolean( networkAdapter[ "NetEnabled" ].ToString() ) ) {
                        netEnabled = ( int )EnumNetEnabledStatus.Enabled;
                    }
                    else {
                        netEnabled = ( int )EnumNetEnabledStatus.Disabled;
                    }
                }
            }
            catch ( NullReferenceException ) {
            }
            return netEnabled;
        }
    }
}