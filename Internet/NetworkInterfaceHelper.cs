// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "NetworkInterfaceHelper.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/NetworkInterfaceHelper.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

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

        public static IDictionary<String, NetworkInterface> NetworkInterfaces { get; }

        static NetworkInterfaceHelper() => NetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces().ToDictionary( o => o.Id );

        public static NetworkInterface GetBestInterface( this IPAddress address ) {
            var byteArray1 = address.GetAddressBytes();

            var ipaddr = BitConverter.ToUInt32( byteArray1, 0 );
            var error = NativeMethods.GetBestInterface( ipaddr, out var interfaceIndex );

            if ( error != 0 ) { throw new InvalidOperationException( $"Error while calling GetBestInterface(). Error={error}" ); }

            var indexedIpAdapterInfo = AdapterInfo.IndexedIpAdapterInfos[interfaceIndex];

            return NetworkInterfaces[indexedIpAdapterInfo.AdapterName];
        }
    }
}