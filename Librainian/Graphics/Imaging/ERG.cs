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
// File "ERG.cs" last formatted on 2022-12-22 at 5:16 PM by Protiguous.

namespace Librainian.Graphics.Imaging;

using System;
using System.Collections.Concurrent;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Collections.Sets;
using Newtonsoft.Json;

/// <summary> Experimental Resilient Graphics </summary>
/// <remarks>Just for fun & learning.</remarks>
/// <remarks>
///     Prefer native file system compression over encoding/compression speed (assuming local cpu will be 'faster' than
///     network transfer speed).
///     <para>Allow 'pages' of animation, each with their own delay. Default should be page 0 = 0 delay.</para>
///     <para>Checksums are used on each pixel to guard against (detect but not fix) corruption.</para>
/// </remarks>
/// <remarks> 60 frames per second allows 16.67 milliseconds per frame.</remarks>
/// <remarks> 1920x1080 pixels = 2,052,000 possible pixels ...so about 8 nanoseconds per pixel? </remarks>
[JsonObject]
public class Erg {

	public static readonly String Extension = ".erg";

	/// <summary>Human readable file header.</summary>
	public static readonly String Header = "ERG0.1";

	public Erg() => this.Checksum = UInt64.MaxValue;

	/// <summary>Checksum of all pages</summary>
	[JsonProperty]
	public UInt64 Checksum { get; private set; }

	/// <summary>EXIF metadata</summary>
	[JsonProperty]
	public ConcurrentDictionary<String, String> Exifs { get; } = new();

	public UInt32 Height { get; private set; }

	[JsonProperty]
	public ConcurrentSet<Pixel> Pixels { get; } = new();

	[JsonProperty]
	public ConcurrentSet<Int32> PropertyIdList { get; } = new();

	[JsonProperty]
	public ConcurrentSet<PropertyItem> PropertyItems { get; } = new();

	public UInt32 Width { get; private set; }

	public Task<UInt64> CalculateChecksumAsync() =>
		Task.Run( () => {
			unchecked {
				return ( UInt64 )this.Pixels.GetHashCode();
			}
		} );
}