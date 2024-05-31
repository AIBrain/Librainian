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
// File "Pixelyx.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

namespace Librainian.Graphics.Moving;

using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

/// <summary>
/// <para>
/// A pixel (14 bytes?) with <see cref="Red" />, <see cref="Green" />, and <see cref="Blue" /> values, checksum, and X/Y coordinates.
/// </para>
/// <para>At one screencap of 1920*1080, that's about ~24883200 (23MB) bytes of data for just one frame.</para>
/// <para>At 60 fps, that is ~1492992000 bytes of data per second (1423MB/s)!</para>
/// </summary>
[JsonObject]
[StructLayout( LayoutKind.Sequential )]
public class Pixelyx : IEquatable<Pixelyx> {

	[JsonProperty]
	public readonly Byte Alpha;

	[JsonProperty]
	public readonly Byte Blue;

	[JsonProperty]
	public readonly Int32 Checksum;

	[JsonProperty]
	public readonly Byte Green;

	[JsonProperty]
	public readonly Byte Red;

	[JsonProperty]
	public readonly UInt64 Timestamp;

	[JsonProperty]
	public readonly UInt16 X;

	[JsonProperty]
	public readonly UInt16 Y;

	private Pixelyx( Byte alpha, Byte red, Byte green, Byte blue, UInt16 x, UInt16 y, UInt64 timestamp ) {
		this.Alpha = alpha;
		this.Red = red;
		this.Green = green;
		this.Blue = blue;
		this.X = x;
		this.Y = y;
		this.Timestamp = timestamp;
		this.Checksum = (this.Blue, this.Red, this.Alpha, this.Timestamp, this.X, this.Y).GetHashCode();
	}

	public virtual Boolean Equals( Pixelyx? other ) {
		if ( other is null ) {
			return false;
		}

		if ( ReferenceEquals( this, other ) ) {
			return true;
		}

		return this.Alpha == other.Alpha && this.Blue == other.Blue && this.Checksum == other.Checksum && this.Green == other.Green && this.Red == other.Red &&
			   this.Timestamp == other.Timestamp && this.X == other.X && this.Y == other.Y;
	}

	/*

    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
    /// <param name="other">An object to compare with this object.</param>
    public Boolean Equals( Pixelyx? other ) => Equal( this, other );
    */

	//public static explicit operator Pixelyx( Color pixel ) {
	//    return new Pixelyx( pixel.A, pixel.R, pixel.G, pixel.B, 0, 0, 0 );
	//}

	//public static implicit operator Color( Pixelyx pixel ) {
	//    return Color.FromArgb( pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue );
	//}

	//public static explicit operator UInt16[]( Pixelyx pixel ) {
	//    return new[] { pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue, pixel.X, pixel.Y };
	//}

	/// <summary>
	/// <para>Static comparison type.</para>
	/// <para>Compares: Alpha, Red, Green, Blue, X, and Y</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Boolean Equal( Pixelyx? left, Pixelyx? right ) {
		if ( ReferenceEquals( left, right ) ) {
			return true;
		}

		if ( left is null || right is null ) {
			return false;
		}

		return left.Alpha == right.Alpha && left.Red == right.Red && left.Green == right.Green && left.Blue == right.Blue && left.X == right.X && left.Y == right.Y;
	}

	/// <summary>Returns the hash code for this instance.</summary>
	/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
	public override Int32 GetHashCode() => this.Checksum;

	//public override Int32 GetHashCode() => HashCode.Combine( this.Alpha, this.Blue, this.Checksum, this.Green, this.Red, this.Timestamp, this.X, this.Y );
}