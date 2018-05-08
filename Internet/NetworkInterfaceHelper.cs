// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/NetworkInterfaceHelper.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using OperatingSystem;

    /// <summary>
    ///     http://pastebin.com/u9159Ys8
    /// </summary>
    public static class NetworkInterfaceHelper {

		static NetworkInterfaceHelper() => NetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces().ToDictionary( o => o.Id );

		public static IDictionary<String, NetworkInterface> NetworkInterfaces {
            get;
        }

        public static NetworkInterface GetBestInterface( this IPAddress address ) {
            var byteArray1 = address.GetAddressBytes();

            var ipaddr = BitConverter.ToUInt32( byteArray1, 0 );
			var error = NativeMethods.GetBestInterface( ipaddr, out var interfaceIndex );

			if ( error != 0 ) {
                throw new InvalidOperationException( $"Error while calling GetBestInterface(). Error={error}" );
            }

            var indexedIpAdapterInfo = AdapterInfo.IndexedIpAdapterInfos[ interfaceIndex ];

            return NetworkInterfaces[ indexedIpAdapterInfo.AdapterName ];
        }
    }
}