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
// "Librainian 2015/Class1.cs" was last cleaned by aibra_000 on 2015/03/29 at 1:09 PM
#endregion

namespace Librainian.Internet {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    public class TraceExtensions {
        /// <summary>
        ///     Traces the route which data have to travel through in order to reach an IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address of the destination.</param>
        /// <param name="maxHops">Max hops to be returned.</param>
        /// <param name="timeout"></param>
        public IEnumerable< TracertEntry > Tracert( string ipAddress, int maxHops, int timeout ) {
            IPAddress address;

            // Ensure that the argument address is valid.
            if ( !IPAddress.TryParse( ipAddress, out address ) ) {
                throw new ArgumentException( string.Format( "{0} is not a valid IP address.", ipAddress ) );
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
                pingReply = ping.Send( address: address, timeout: timeout, buffer: new byte[] { 0 }, options: pingOptions );
                replyTime.Stop();

                var hostname = string.Empty;
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
            } while ( pingReply.Status != IPStatus.Success && pingOptions.Ttl <= maxHops );
        }

        public sealed class TracertEntry {
            /// <summary>
            ///     The hop id. Represents the number of the hop.
            /// </summary>
            public int HopID { get; set; }

            /// <summary>
            ///     The IP address.
            /// </summary>
            public string Address { get; set; }

            /// <summary>
            ///     The hostname
            /// </summary>
            public string Hostname { get; set; }

            /// <summary>
            ///     The reply time it took for the host to receive and reply to the request in milliseconds.
            /// </summary>
            public long ReplyTime { get; set; }

            /// <summary>
            ///     The reply status of the request.
            /// </summary>
            public IPStatus ReplyStatus { get; set; }

            public override string ToString() => string.Format( "{0} | {1} | {2}", this.HopID, string.IsNullOrEmpty( this.Hostname ) ? this.Address : this.Hostname + "[" + this.Address + "]", this.ReplyStatus == IPStatus.TimedOut ? "Request Timed Out." : this.ReplyTime + " ms" );
        }
    }
}
