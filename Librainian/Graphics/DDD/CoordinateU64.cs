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
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "CoordinateU64.cs" last touched on 2021-07-31 at 7:34 AM by Protiguous.

#nullable enable

namespace Librainian.Graphics.DDD;

using System;
using System.Diagnostics;
using System.Drawing;
using Extensions;
using Maths;
using Maths.Ranges;
using Newtonsoft.Json;
using Utilities;

/// <summary>A 3D point, with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /> .</summary>
/// <remarks>Coded towards speed.</remarks>
[Immutable]
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[JsonObject]
public record CoordinateU64 {

	/// <summary>The smallest value a <see cref="CoordinateU64" /> will hold.</summary>
	public const UInt64 Minimum = 1 + UInt64.MinValue; //TODO why is this not Zero ?

	/// <summary>The largest value a <see cref="CoordinateU64" /> will hold.</summary>
	/// <remarks>the cuberoot of <see cref="UInt64.MaxValue" /> split into x*y*z.</remarks>
	public static readonly UInt64 Maximum = ( UInt64 )Math.Pow( UInt64.MaxValue, 1.0 / 3.0 );

	public static readonly CoordinateU64 MaxValue = new( UInt64.MaxValue, UInt64.MaxValue, UInt64.MaxValue );

	public static readonly CoordinateU64 MinValue = new( Minimum, Minimum, Minimum );

	///// <summary>
	/////   Initialize with a random point.
	///// </summary>
	//public Coordinate() : this( x: Randem.NextFloat(), y: Randem.NextFloat(), z: Randem.NextFloat() ) { }

	public CoordinateU64( UInt64Range x ) : this( x.Minimum.Next( x.Maximum ), Minimum.Next( Maximum ), Minimum.Next( Maximum ) ) { }

	public CoordinateU64( UInt64Range x, UInt64Range y, UInt64Range z ) : this( x.Minimum.Next( x.Maximum ), y.Minimum.Next( y.Maximum ), z.Minimum.Next( z.Maximum ) ) { }

		
	/// <summary>
	/// 
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	public CoordinateU64( UInt64 x, UInt64 y, UInt64 z ) {
		this.X = Math.Max( Minimum, Math.Min( Maximum, x ) );
		this.Y = Math.Max( Minimum, Math.Min( Maximum, y ) );
		this.Z = Math.Max( Minimum, Math.Min( Maximum, z ) );
		this.SquareLength = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
	}

	/// <summary>Maximum - Minimum</summary>
	public static UInt64 Range { get; } = Maximum - Minimum;

	[JsonProperty]
	public UInt64 SquareLength { get; init; }

	[JsonProperty]
	public UInt64 X { get; init; }

	[JsonProperty]
	public UInt64 Y { get; init; }

	[JsonProperty]
	public UInt64 Z { get; init; }

	/// <summary>Calculates the distance between two <see cref="CoordinateU64" />.</summary>
	public static UInt64 Distance( CoordinateU64 left, CoordinateU64 right ) {
		var num1 = left.X - right.X;
		var num2 = left.Y - right.Y;
		var num3 = left.Z - right.Z;

		return ( UInt64 )Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
	}

	/// <summary>static comparison.</summary>
	/// <param name="left"></param>
	/// <param name="right"> </param>
	public static Boolean Equals( CoordinateU64 left, CoordinateU64 right ) => left.X == right.X && left.Y == right.Y && left.Z == right.Z;

	public static implicit operator Point( CoordinateU64 coordinate ) => new( ( Int32 )coordinate.X, ( Int32 )coordinate.Y );

	public static implicit operator PointF( CoordinateU64 coordinate ) => new( coordinate.X, coordinate.Y );

	/// <summary>
	///     <para>Returns a new Coordinate as a unit <see cref="CoordinateU64" />.</para>
	///     <para>The result is a Coordinate one unit in length pointing in the same direction as the original Coordinate.</para>
	/// </summary>
	public static CoordinateU64 Normalize( CoordinateU64 coordinate ) {
		var num = 1.0D / coordinate.SquareLength;

		return new CoordinateU64( ( UInt64 )( coordinate.X * num ), ( UInt64 )( coordinate.Y * num ), ( UInt64 )( coordinate.Z * num ) );
	}

	public static CoordinateU64 operator -( CoordinateU64 v1, CoordinateU64 v2 ) => new( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );

	/// <summary>Compares the current <see cref="CoordinateU64" /> with another <see cref="CoordinateU64" />.</summary>
	/// <returns>
	///     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the
	///     following meanings: Value Meaning Less than zero: This
	///     object is less than the <paramref name="other" /> parameter. Zero: This object is equal to
	///     <paramref name="other" /> . Greater than zero This object is greater than
	///     <paramref name="other" /> .
	/// </returns>
	/// <param name="other">An object to compare with this object.</param>
	[NeedsTesting]
	public Int32 CompareTo( CoordinateU64 other ) => this.SquareLength.CompareTo( other.SquareLength );

	/// <summary>Calculates the distance between this <see cref="CoordinateU64" /> and another <see cref="CoordinateU64" />.</summary>
	public UInt64 Distance( CoordinateU64 right ) {
		var num1 = this.X - right.X;
		var num2 = this.Y - right.Y;
		var num3 = this.Z - right.Z;

		return ( UInt64 )Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
	}

	public override String ToString() => $"{this.X}, {this.Y}, {this.Z}";
}