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
// "Librainian/NetworkAdapter.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using System.Threading.Tasks;
    using Measurement.Time;
    using OperatingSystem.WMI;

    /// <summary>
    ///     Module Name: NetworkAdapter.cs
    ///     Project: CSWMIEnableDisableNetworkAdapter Copyright (c) Microsoft Corporation.
    /// </summary>
    public class NetworkAdapter {

        public NetworkAdapter( Int32 deviceId, String name, Int32 netEnabled, Int32 netConnectionStatus ) {
            this.DeviceId = deviceId;
            this.Name = name;
            this.NetEnabled = netEnabled;
            this.NetConnectionStatus = netConnectionStatus;
        }

        public NetworkAdapter( Int32 deviceId ) {
            var strWQuery = $"SELECT DeviceID, ProductName, NetEnabled, NetConnectionStatus FROM Win32_NetworkAdapter WHERE DeviceID = {deviceId}";
            try {
                var networkAdapters = WMIExtensions.WmiQuery( strWQuery );

                var crtNetworkAdapter = networkAdapters.Cast<ManagementBaseObject>().Select( o => o as ManagementObject ).FirstOrDefault();
                if ( null == crtNetworkAdapter ) {
                    return;
                }

                this.DeviceId = deviceId;

                this.Name = crtNetworkAdapter[ "ProductName" ].ToString();

                this.NetEnabled = Convert.ToBoolean( crtNetworkAdapter[ "NetEnabled" ].ToString() ) ? ( Int32 )EnumNetEnabledStatus.Enabled : ( Int32 )EnumNetEnabledStatus.Disabled;

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
        ///     Enum the Operation result of Enable and Disable Network Adapter
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
        public Int32 DeviceId {
            get;
        }

        /// <summary>
        ///     The ProductName of the NetworkAdapter
        /// </summary>
        public String Name {
            get;
        }

        /// <summary>
        ///     The Net Connection Status Value
        /// </summary>
        public Int32 NetConnectionStatus {
            get;
        }

        /// <summary>
        ///     The NetEnabled status of the NetworkAdapter
        /// </summary>
        public Int32 NetEnabled {
            get;
        }

        /// <summary>
        ///     List all the NetworkAdapters
        /// </summary>
        /// <returns>The list of all NetworkAdapter of the machine</returns>
        public static IEnumerable<NetworkAdapter> GetAllNetworkAdapters() {

            //var allNetworkAdapter = new List<NetworkAdapter>();

            // Manufacturer <> 'Microsoft'to get all nonvirtual devices. Because the AdapterType
            // property will be null if the NetworkAdapter is disabled, so we do not use
            // NetworkAdapter = 'Ethernet 802.3' or NetworkAdapter = 'Wireless’

            var networkAdapters = WMIExtensions.WmiQuery( "SELECT DeviceID, ProductName, NetEnabled, NetConnectionStatus FROM Win32_NetworkAdapter WHERE Manufacturer <> \'Microsoft\'" );
            return networkAdapters.Cast<ManagementBaseObject>().Select( o => o as ManagementObject ).Select( moNetworkAdapter => new NetworkAdapter( Convert.ToInt32( moNetworkAdapter[ "DeviceID" ].ToString() ), moNetworkAdapter[ "ProductName" ].ToString(), Convert.ToBoolean( moNetworkAdapter[ "NetEnabled" ].ToString() ) ? ( Int32 )EnumNetEnabledStatus.Enabled : ( Int32 )EnumNetEnabledStatus.Disabled, Convert.ToInt32( moNetworkAdapter[ "NetConnectionStatus" ].ToString() ) ) );

            //return allNetworkAdapter;
        }

        /// <summary>
        ///     Enable Or Disable The NetworkAdapter
        /// </summary>
        /// <returns>Whether the NetworkAdapter was enabled or disabled successfully</returns>
        public Int32 EnableOrDisableNetworkAdapter( String strOperation ) {
            strOperation = strOperation.Trim();

            Int32 resultEnableDisableNetworkAdapter;
            ManagementObject crtNetworkAdapter;

            using ( crtNetworkAdapter = new ManagementObject() ) {

                try {
                    var networkAdapters = WMIExtensions.WmiQuery( $"SELECT DeviceID, ProductName, NetEnabled, NetConnectionStatus FROM Win32_NetworkAdapter WHERE DeviceID = {this.DeviceId}" );
                    foreach ( var networkAdapter in from ManagementBaseObject o in networkAdapters select o as ManagementObject ) {
                        crtNetworkAdapter = networkAdapter;
                    }

                    crtNetworkAdapter?.InvokeMethod( strOperation, null );

                    Task.Delay( Milliseconds.OneHundred ).Wait();
                    while ( this.GetNetEnabled() != ( strOperation.Equals( "Enable", StringComparison.OrdinalIgnoreCase ) ? ( Int32 )EnumNetEnabledStatus.Enabled : ( Int32 )EnumNetEnabledStatus.Disabled ) ) {
                        Task.Delay( Milliseconds.OneHundred ).Wait();
                    }

                    resultEnableDisableNetworkAdapter = ( Int32 )EnumEnableDisableResult.Success;
                }
                catch ( NullReferenceException ) {

                    // If there is a NullReferenceException, the result of the enable or disable network
                    // adapter operation will be fail
                    resultEnableDisableNetworkAdapter = ( Int32 )EnumEnableDisableResult.Fail;
                }

            }

            return resultEnableDisableNetworkAdapter;
        }

        /// <summary>
        ///     Get the NetworkAdapter NetEnabled Property
        /// </summary>
        /// <returns>Whether the NetworkAdapter is enabled</returns>
        public Int32 GetNetEnabled() {
            var netEnabled = ( Int32 )EnumNetEnabledStatus.Unknown;
            try {
                var networkAdapters = WMIExtensions.WmiQuery( $"SELECT NetEnabled FROM Win32_NetworkAdapter WHERE DeviceID = {this.DeviceId}" );
                foreach ( var networkAdapter in networkAdapters.Cast<ManagementBaseObject>().Select( o => o as ManagementObject ) ) {
                    if ( Convert.ToBoolean( networkAdapter[ "NetEnabled" ].ToString() ) ) {
                        netEnabled = ( Int32 )EnumNetEnabledStatus.Enabled;
                    }
                    else {
                        netEnabled = ( Int32 )EnumNetEnabledStatus.Disabled;
                    }
                }
            }
            catch ( NullReferenceException ) { }
            return netEnabled;
        }
    }
}