// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "TraceExtensions.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

#nullable enable

namespace Librainian.Internet;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Exceptions;

public static class TraceExtensions {

	/// <summary>Traces the route which data have to travel through in order to reach an IP address.</summary>
	/// <param name="address">The IP address of the destination.</param>
	/// <param name="maxHops">Max hops to be returned.</param>
	/// <param name="timeout"></param>
	public static IEnumerable<TracertEntry> TraceRoute( this IPAddress address, Int32 maxHops, Int32 timeout ) {

		// Max hops should be at least one or else there won't be any data to return.
		if ( maxHops < 1 ) {
			maxHops = 1;
		}

		// Ensure that the timeout is not set to 0 or a negative number.
		if ( timeout < 1 ) {
			throw new NullException( "Timeout value must be higher than 0." );
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

		public override String ToString() =>
			$"{this.HopID} | {( String.IsNullOrEmpty( this.Hostname ) ? this.Address : this.Hostname + "[" + this.Address + "]" )} | {( this.ReplyStatus == IPStatus.TimedOut ? "Request Timed Out." : this.ReplyTime + " ms" )}";
	}
}