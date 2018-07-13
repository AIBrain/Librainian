// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "NetworkAdapter.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "NetworkAdapter.cs" was last formatted by Protiguous on 2018/07/10 at 8:55 PM.

namespace Librainian.ComputerSystem.FileSystem {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Management;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Measurement.Time;
	using OperatingSystem.WMI;

	/// <summary>
	///     Module Name: NetworkAdapter.cs
	///     Project: CSWMIEnableDisableNetworkAdapter Copyright (c) Microsoft Corporation.
	/// </summary>
	public class NetworkAdapter {

		/// <summary>
		///     The DeviceID of the NetworkAdapter
		/// </summary>
		public Int32 DeviceId { get; }

		/// <summary>
		///     The ProductName of the NetworkAdapter
		/// </summary>
		public String Name { get; }

		/// <summary>
		///     The Net Connection Status Value
		/// </summary>
		public Int32 NetConnectionStatus { get; }

		/// <summary>
		///     The NetEnabled status of the NetworkAdapter
		/// </summary>
		public Int32 NetEnabled { get; }

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

				if ( null == crtNetworkAdapter ) { return; }

				this.DeviceId = deviceId;

				this.Name = crtNetworkAdapter[ "ProductName" ].ToString();

				this.NetEnabled = Convert.ToBoolean( crtNetworkAdapter[ "NetEnabled" ].ToString() ) ? ( Int32 ) EnumNetEnabledStatus.Enabled : ( Int32 ) EnumNetEnabledStatus.Disabled;

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
		///     List all the NetworkAdapters
		/// </summary>
		/// <returns>The list of all NetworkAdapter of the machine</returns>
		[NotNull]
		public static IEnumerable<NetworkAdapter> GetAllNetworkAdapters() {

			//var allNetworkAdapter = new List<NetworkAdapter>();

			// Manufacturer <> 'Microsoft'to get all nonvirtual devices. Because the AdapterType
			// property will be null if the NetworkAdapter is disabled, so we do not use
			// NetworkAdapter = 'Ethernet 802.3' or NetworkAdapter = 'Wireless’

			var networkAdapters = WMIExtensions.WmiQuery( "SELECT DeviceID, ProductName, NetEnabled, NetConnectionStatus FROM Win32_NetworkAdapter WHERE Manufacturer <> \'Microsoft\'" );

			return networkAdapters.Cast<ManagementBaseObject>().Select( o => o as ManagementObject ).Select( moNetworkAdapter => new NetworkAdapter( Convert.ToInt32( moNetworkAdapter[ "DeviceID" ].ToString() ),
				moNetworkAdapter[ "ProductName" ].ToString(), Convert.ToBoolean( moNetworkAdapter[ "NetEnabled" ].ToString() ) ? ( Int32 ) EnumNetEnabledStatus.Enabled : ( Int32 ) EnumNetEnabledStatus.Disabled,
				Convert.ToInt32( moNetworkAdapter[ "NetConnectionStatus" ].ToString() ) ) );

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

					foreach ( var networkAdapter in from ManagementBaseObject o in networkAdapters select o as ManagementObject ) { crtNetworkAdapter = networkAdapter; }

					crtNetworkAdapter?.InvokeMethod( strOperation, null );

					Task.Delay( Milliseconds.OneHundred ).Wait();

					while ( this.GetNetEnabled() != ( strOperation.Equals( "Enable", StringComparison.OrdinalIgnoreCase ) ? ( Int32 ) EnumNetEnabledStatus.Enabled : ( Int32 ) EnumNetEnabledStatus.Disabled ) ) {
						Task.Delay( Milliseconds.OneHundred ).Wait();
					}

					resultEnableDisableNetworkAdapter = ( Int32 ) EnumEnableDisableResult.Success;
				}
				catch ( NullReferenceException ) {

					// If there is a NullReferenceException, the result of the enable or disable network
					// adapter operation will be fail
					resultEnableDisableNetworkAdapter = ( Int32 ) EnumEnableDisableResult.Fail;
				}
			}

			return resultEnableDisableNetworkAdapter;
		}

		/// <summary>
		///     Get the NetworkAdapter NetEnabled Property
		/// </summary>
		/// <returns>Whether the NetworkAdapter is enabled</returns>
		public Int32 GetNetEnabled() {
			var netEnabled = ( Int32 ) EnumNetEnabledStatus.Unknown;

			try {
				var networkAdapters = WMIExtensions.WmiQuery( $"SELECT NetEnabled FROM Win32_NetworkAdapter WHERE DeviceID = {this.DeviceId}" );

				foreach ( var networkAdapter in networkAdapters.Cast<ManagementBaseObject>().Select( o => o as ManagementObject ) ) {
					if ( Convert.ToBoolean( networkAdapter[ "NetEnabled" ].ToString() ) ) { netEnabled = ( Int32 ) EnumNetEnabledStatus.Enabled; }
					else { netEnabled = ( Int32 ) EnumNetEnabledStatus.Disabled; }
				}
			}
			catch ( NullReferenceException ) { }

			return netEnabled;
		}
	}
}