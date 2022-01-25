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
// File "Frame.cs" last formatted on 2022-12-22 at 5:16 PM by Protiguous.

namespace Librainian.Graphics.Imaging;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using Utilities;

[JsonObject]
public class Frame : IEquatable<Frame> {

	public static readonly String DefaultHeader = "EFGFrame";

	/// <summary>Checksum of the page (guard against corruption).</summary>
	/// <remarks>Should include the <see cref="LineCount" /> and <see cref="Delay" /> to prevent buffer overflows and timeouts.</remarks>
	[JsonProperty]
	public UInt64 Checksum { get; }

	/// <summary>How many milliseconds to display this frame?</summary>
	[JsonProperty]
	public UInt64 Delay { get; }

	[JsonProperty]
	public UInt64 Identity { get; }

	/// <summary>How many lines should be in this frame?</summary>
	[JsonProperty]
	public UInt64 LineCount { get; }

	/// <summary>An array of <see cref="Line" />.</summary>
	[JsonProperty]
	public Line[]? Lines { get; }

	/// <summary>static comparision</summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	[NeedsTesting]
	public static Boolean Equals( Frame? left, Frame? right ) {
		if ( ReferenceEquals( left, right ) ) {
			return true;
		}

		if ( left is null || right is null ) {
			return false;
		}

		if ( left.Lines is null || right.Lines is null ) {
			return false;
		}

		if ( left.Checksum != right.Checksum ) {
			return false;
		}

		if ( left.LineCount != right.LineCount ) {
			return false;
		}

		if ( left.Lines.LongLength != right.Lines.LongLength ) {
			return false;
		}

		return left.Lines.SequenceEqual( right.Lines );
	}

	/// <summary>
	///     Returns a value that indicates whether two <see cref="T:Librainian.Graphics.Imaging.Frame" /> objects have
	///     different values.
	/// </summary>
	/// <param name="left">The first value to compare.</param>
	/// <param name="right">The second value to compare.</param>
	/// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
	public static Boolean operator !=( Frame? left, Frame? right ) => !left.Equals( right );

	/// <summary>
	///     Returns a value that indicates whether the values of two <see cref="T:Librainian.Graphics.Imaging.Frame" />
	///     objects are equal.
	/// </summary>
	/// <param name="left">The first value to compare.</param>
	/// <param name="right">The second value to compare.</param>
	/// <returns>
	///     true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise,
	///     false.
	/// </returns>
	public static Boolean operator ==( Frame? left, Frame? right ) => left.Equals( right );

	/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
	/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
	/// <param name="other">An object to compare with this object.</param>
	public Boolean Equals( Frame? other ) => Equals( this, other );

	/// <summary>Indicates whether this instance and a specified object are equal.</summary>
	/// <param name="obj">The object to compare with the current instance.</param>
	/// <returns>
	///     <see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same
	///     value; otherwise, <see langword="false" />.
	/// </returns>
	public override Boolean Equals( Object? obj ) => Object.Equals( this, obj is Frame frame ? frame : default( Frame ) );

	/// <summary>Returns the hash code for this instance.</summary>
	/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
	[SuppressMessage( "ReSharper", "NonReadonlyMemberInGetHashCode" )]
	public override Int32 GetHashCode() => (this.Checksum, this.Identity, this.LineCount).GetHashCode();
}