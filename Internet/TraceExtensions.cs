// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "TraceExtensions.cs",
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
// "Librainian/Librainian/TraceExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using Measurement.Time;

    public class TraceExtensions {

        /// <summary>
        ///     Traces the route which data have to travel through in order to reach an IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address of the destination.</param>
        /// <param name="maxHops">Max hops to be returned.</param>
        /// <param name="timeout"></param>
        public IEnumerable<TracertEntry> Tracert( String ipAddress, Int32 maxHops, Int32 timeout ) {

            // Ensure that the argument address is valid.
            if ( !IPAddress.TryParse( ipAddress, out var address ) ) { throw new ArgumentException( $"{ipAddress} is not a valid IP address." ); }

            // Max hops should be at least one or else there won't be any data to return.
            if ( maxHops < 1 ) { throw new ArgumentException( "Max hops can't be lower than 1." ); }

            // Ensure that the timeout is not set to 0 or a negative number.
            if ( timeout < 1 ) { throw new ArgumentException( "Timeout value must be higher than 0." ); }

            var ping = new Ping();
            var pingOptions = new PingOptions( 1, true );
            var replyTime = new StopWatch();
            PingReply pingReply;

            do {
                replyTime.Start();
                pingReply = ping.Send( address: address, timeout: timeout, buffer: new Byte[] { 0 }, options: pingOptions );
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

            /// <summary>
            ///     The reply time it took for the host to receive and reply to the request in milliseconds.
            /// </summary>
            public Int64 ReplyTime { get; set; }

            public override String ToString() =>
                $"{this.HopID} | {( String.IsNullOrEmpty( this.Hostname ) ? this.Address : this.Hostname + "[" + this.Address + "]" )} | {( this.ReplyStatus == IPStatus.TimedOut ? "Request Timed Out." : this.ReplyTime + " ms" )}";
        }
    }
}