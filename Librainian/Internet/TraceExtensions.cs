// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "TraceExtensions.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "TraceExtensions.cs" was last formatted by Protiguous on 2020/01/31 at 12:25 AM.

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using JetBrains.Annotations;

    public class TraceExtensions {

        /// <summary>Traces the route which data have to travel through in order to reach an IP address.</summary>
        /// <param name="ipAddress">The IP address of the destination.</param>
        /// <param name="maxHops">Max hops to be returned.</param>
        /// <param name="timeout"></param>
        [ItemNotNull]
        public IEnumerable<TracertEntry> Tracert( [NotNull] String ipAddress, Int32 maxHops, Int32 timeout ) {

            // Ensure that the argument address is valid.
            if ( !IPAddress.TryParse( ipAddress, out var address ) ) {
                throw new ArgumentException( $"{ipAddress} is not a valid IP address." );
            }

            // Max hops should be at least one or else there won't be any data to return.
            if ( maxHops < 1 ) {
                throw new ArgumentException( "Max hops can't be lower than 1." );
            }

            // Ensure that the timeout is not set to 0 or a negative number.
            if ( timeout < 1 ) {
                throw new ArgumentException( "Timeout value must be higher than 0." );
            }

            var ping = new Ping();
            var pingOptions = new PingOptions( 1, true );
            var replyTime = new Stopwatch();
            PingReply pingReply;

            do {
                replyTime.Start();

                pingReply = ping.Send( address: address, timeout: timeout, buffer: new Byte[] {
                    0
                }, options: pingOptions );

                replyTime.Stop();

                var hostname = String.Empty;

                if ( pingReply?.Address != null ) {
                    try {

                        //hostname = Dns.GetHostByAddress( reply.Address ).HostName; // Retrieve the hostname for the replied address.
                        hostname = Dns.GetHostEntry( pingReply.Address ).HostName;
                    }
                    catch ( SocketException ) {
                        /* No host available for that address. */
                    }
                }

                // Return out TracertEntry object with all the information about the hop.
                if ( pingReply != null ) {
                    yield return new TracertEntry {
                        HopID = pingOptions.Ttl,
                        Address = pingReply.Address?.ToString() ?? "N/A",
                        Hostname = hostname,
                        ReplyTime = replyTime.ElapsedMilliseconds,
                        ReplyStatus = pingReply.Status
                    };
                }

                pingOptions.Ttl++;

                //replyTime.Reset();
            } while ( pingReply != null && pingReply.Status != IPStatus.Success && pingOptions.Ttl <= maxHops );
        }

        public sealed class TracertEntry {

            /// <summary>The IP address.</summary>
            public String Address { get; set; }

            /// <summary>The hop id. Represents the number of the hop.</summary>
            public Int32 HopID { get; set; }

            /// <summary>The hostname</summary>
            public String Hostname { get; set; }

            /// <summary>The reply status of the request.</summary>
            public IPStatus ReplyStatus { get; set; }

            /// <summary>The reply time it took for the host to receive and reply to the request in milliseconds.</summary>
            public Int64 ReplyTime { get; set; }

            public override String ToString() =>
                $"{this.HopID} | {( String.IsNullOrEmpty( this.Hostname ) ? this.Address : this.Hostname + "[" + this.Address + "]" )} | {( this.ReplyStatus == IPStatus.TimedOut ? "Request Timed Out." : this.ReplyTime + " ms" )}";

        }

    }

}