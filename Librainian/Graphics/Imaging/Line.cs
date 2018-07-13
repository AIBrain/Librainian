// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Line.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Line.cs" was last formatted by Protiguous on 2018/07/10 at 9:07 PM.

namespace Librainian.Graphics.Imaging {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     A horizontal line of <see cref="Pixel" />.
	/// </summary>
	[JsonObject]
	[StructLayout( LayoutKind.Sequential )]
	public class Line : IEquatable<Line>, IEnumerable<Pixel>, IEqualityComparer<Line> {

		/// <summary>
		///     Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the
		///     collection.
		/// </returns>
		public IEnumerator<Pixel> GetEnumerator() => this.Pixels.AsEnumerable().GetEnumerator();

		/// <summary>
		///     Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		/// <summary>
		///     Determines whether the specified objects are equal.
		/// </summary>
		/// <returns>true if the specified objects are equal.</returns>
		public Boolean Equals( Line x, Line y ) => Equal( x, y );

		/// <summary>
		///     Returns a hash code for the specified object.
		/// </summary>
		/// <returns>A hash code for the specified object.</returns>
		/// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///     The type of <paramref name="obj" /> is a reference type and
		///     <paramref name="obj" /> is null.
		/// </exception>
		public Int32 GetHashCode( Line obj ) => this.Pixels.GetHashCode();

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public Boolean Equals( Line other ) => Equal( this, other );

		/// <summary>
		///     Checksum of the pixels (to guard against corruption).
		/// </summary>
		/// <remarks>Should include the <see cref="Count" /> to prevent buffer overflows.</remarks>
		[JsonProperty]

		//[FieldOffset( 0 )]
		public UInt64 Checksum;

		/// <summary>
		///     How many pixels should be in this line?
		/// </summary>
		[JsonProperty]

		//[FieldOffset( sizeof( UInt64 ) * 1 )]
		public UInt64 Count;

		/// <summary>
		///     An array of pixels
		/// </summary>
		/// <remarks>I'd prefer a list instead of an array.</remarks>
		[JsonProperty]

		//[FieldOffset( sizeof( UInt64 ) * 2 )]
		[NotNull]
		public Pixel[] Pixels;

		/// <summary>
		///     Returns the zero-based <see cref="Pixel" /> or null if not found.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Pixel? this[ UInt64 index ] {
			get {
				var pixels = this.Pixels;

				if ( index <= this.Count ) { return pixels[ index ]; }

				return null;
			}

			set {
				var pixels = this.Pixels;

				if ( value.HasValue && index <= this.Count ) { pixels[ index ] = value.Value; }
			}
		}

		/// <summary>
		///     Construct a <see cref="Line" /> from an array of <see cref="Pixel" />.
		/// </summary>
		/// <param name="pixels"></param>
		public Line( [NotNull] Pixel[] pixels ) {
			this.Pixels = pixels.ToArray();
			this.Count = ( UInt64 ) this.Pixels.LongLength;
			this.Checksum = CalculateChecksumAsync( this.Pixels ).Result;
		}

		public static async Task<UInt64> CalculateChecksumAsync( IEnumerable<Pixel> pixels ) =>
			await Task.Run( () => {
				var checksum = UInt64.MinValue;

				foreach ( var pixel in pixels ) {
					unchecked { checksum = ( checksum + ( UInt64 ) pixel.GetHashCode() ) / 2; }
				}

				return checksum;
			} );

		/// <summary>
		///     Static comparison type.
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equal( [CanBeNull] Line left, [CanBeNull] Line right ) {
			if ( left is null || right is null ) { return false; }

			if ( left.Checksum != right.Checksum ) { return false; }

			if ( left.Count != right.Count ) { return false; }

			return left.Pixels.SequenceEqual( right.Pixels );
		}
	}
}