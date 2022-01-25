// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Inet.cs" last formatted on 2022-12-22 at 5:17 PM by Protiguous.

namespace Librainian.Internet;

using System;
using System.Net;
using System.Threading.Tasks;
using Maths;
using Microsoft.IO;
using Utilities;

[NeedsTesting]
public static class Inet {

	[NeedsTesting]
	private static RecyclableMemoryStreamManager MemoryStreamManager { get; } = new( MathConstants.Sizes.OneMegaByte, MathConstants.Sizes.OneGigaByte );

	[NeedsTesting]
	public static async Task<Byte[]> GetUrlContentsAsync( this Uri url ) {

		// The downloaded resource ends up in the variable named content.

		await using var content = MemoryStreamManager.GetStream();

		// Send the request to the Internet resource and wait for the response.

		if ( WebRequest.Create( url ) is HttpWebRequest webReq ) {
			using var response = await webReq.GetResponseAsync().ConfigureAwait( false );

			// Get the data stream that is associated with the specified url.

			await using var responseStream = response.GetResponseStream();

			// Read the bytes in responseStream and copy them to content.
			await responseStream.CopyToAsync( content ).ConfigureAwait( false );

			// The previous statement abbreviates the following two statements.

			// CopyToAsync returns a Task, not a Task<T>.
			//Task copyTask = responseStream.CopyToAsync(content);

			// When copyTask is completed, content contains a copy of
			// responseStream.
			//await copyTask;
		}

		return content.ToArray();
	}
}