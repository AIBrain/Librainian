// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Pixelyx.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Pixelyx.cs" was last formatted by Protiguous on 2018/07/10 at 9:08 PM.

namespace Librainian.Graphics.Moving {

	using System;
	using System.Runtime.InteropServices;
	using JetBrains.Annotations;
	using Maths.Hashings;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>
	///         A pixel (14 bytes?) with <see cref="Red" />, <see cref="Green" />, and <see cref="Blue" />
	///         values, checksum, and X/Y coordinates.
	///     </para>
	///     <para>
	///         At one screencap of 1920*1080, that's about ~24883200 (23MB) bytes of data for just one frame.
	///     </para>
	///     <para>At 60 fps, that is ~1492992000 bytes of data per second (1423MB/s)!</para>
	/// </summary>
	[JsonObject]
	[StructLayout( LayoutKind.Sequential )]
	public class Pixelyx : IEquatable<Pixelyx> {

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		///     true if the current object is equal to the <paramref name="other" /> parameter;
		///     otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public Boolean Equals( Pixelyx other ) => Equal( this, other );

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
			this.Checksum = HashingExtensions.GetHashCodes( this.Blue, this.Red, this.Alpha, this.Timestamp, this.X, this.Y );
		}

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
		///     <para>Static comparison type.</para>
		///     <para>Compares: Alpha, Red, Green, Blue, X, and Y</para>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equal( [NotNull] Pixelyx left, [NotNull] Pixelyx right ) =>
			left.Alpha == right.Alpha && left.Red == right.Red && left.Green == right.Green && left.Blue == right.Blue && left.X == right.X && left.Y == right.Y;

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		public override Int32 GetHashCode() => this.Checksum;
	}
}