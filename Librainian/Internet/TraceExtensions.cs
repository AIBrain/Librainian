#nullable enable

namespace Librainian.Internet {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Net;
	using System.Net.NetworkInformation;
	using System.Net.Sockets;
	using JetBrains.Annotations;

	public static class TraceExtensions {

		/// <summary>Traces the route which data have to travel through in order to reach an IP address.</summary>
		/// <param name="address">The IP address of the destination.</param>
		/// <param name="maxHops">Max hops to be returned.</param>
		/// <param name="timeout"></param>
		[ItemNotNull]
		public static IEnumerable<TracertEntry> TraceRoute( [NotNull] this IPAddress address, Int32 maxHops, Int32 timeout ) {

			// Max hops should be at least one or else there won't be any data to return.
			if ( maxHops < 1 ) {
				maxHops = 1;
			}

			// Ensure that the timeout is not set to 0 or a negative number.
			if ( timeout < 1 ) {
				throw new ArgumentException( "Timeout value must be higher than 0." );
			}

			using var ping = new Ping();
			var pingOptions = new PingOptions( 1, true );
			var replyTime = new Stopwatch();
			PingReply pingReply;

			do {
				replyTime.Start();

				pingReply = ping.Send( address, timeout, new Byte[] {
					0
				}, pingOptions );

				replyTime.Stop();

				var hostname = String.Empty;

				try {
					//hostname = Dns.GetHostByAddress( reply.Address ).HostName; // Retrieve the hostname for the replied address.
					hostname = Dns.GetHostEntry( pingReply.Address ).HostName;
				}
				catch ( SocketException ) {
					/* No host available for that address. */
				}

				// Return out TracertEntry object with all the information about the hop.
				yield return new TracertEntry {
					HopID = pingOptions.Ttl,
					Address = pingReply.Address.ToString(),
					Hostname = hostname,
					ReplyTime = replyTime.ElapsedMilliseconds,
					ReplyStatus = pingReply.Status
				};

				pingOptions.Ttl++;

				//replyTime.Reset();
			} while ( pingReply.Status != IPStatus.Success && pingOptions.Ttl <= maxHops );
		}

		public sealed class TracertEntry {

			/// <summary>The IP address.</summary>
			public String? Address { get; set; }

			/// <summary>The hop id. Represents the number of the hop.</summary>
			public Int32 HopID { get; set; }

			/// <summary>The hostname</summary>
			public String? Hostname { get; set; }

			/// <summary>The reply status of the request.</summary>
			public IPStatus ReplyStatus { get; set; }

			/// <summary>The reply time it took for the host to receive and reply to the request in milliseconds.</summary>
			public Int64 ReplyTime { get; set; }

			[NotNull]
			public override String ToString() =>
				$"{this.HopID} | {( String.IsNullOrEmpty( this.Hostname ) ? this.Address : this.Hostname + "[" + this.Address + "]" )} | {( this.ReplyStatus == IPStatus.TimedOut ? "Request Timed Out." : this.ReplyTime + " ms" )}";

		}

	}

}