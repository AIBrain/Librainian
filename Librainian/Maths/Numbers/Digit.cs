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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Digit.cs" last formatted on 2022-12-22 at 4:22 AM by Protiguous.

#nullable enable

namespace Librainian.Maths.Numbers;

using System;
using System.Runtime.CompilerServices;
using Extensions;

/// <summary>
///     Valid numbers are 0, 1, 2, 3, 4, 5, 6, 7, 8, 9.
/// </summary>
[Immutable]
public record Digit : IComparable<Digit> {

	public const Byte MaximumValue = 9;

	public const Byte MinimumValue = 0;

	public Digit( Byte value ) {
		if ( value > MaximumValue ) {
			throw new ArgumentOutOfRangeException( nameof( value ), "Out of range" );
		}

		this.Value = value;
	}

	public static Digit[] AllDigits { get; } = {
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9
	};

	public static Digit Eight { get; } = new(8);

	public static Digit Five { get; } = new(5);

	public static Digit Four { get; } = new(4);

	public static Digit Nine { get; } = new(9);

	public static Digit One { get; } = new(1);

	public static Digit Seven { get; } = new(7);

	public static Digit Six { get; } = new(6);

	public static Digit Three { get; } = new(3);

	public static Digit Two { get; } = new(2);

	public static Digit Zero { get; } = new(0);

	public Byte Value { get; init; }

	public Int32 CompareTo( Digit? other ) => this.Value.CompareTo( other?.Value );

	public virtual Boolean Equals( Digit? other ) {
		if ( other is null ) {
			return false;
		}

		if ( ReferenceEquals( this, other ) ) {
			return true;
		}

		return this.Value == other.Value;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Boolean Equals( Digit left, Digit right ) => left.Value == right.Value;

	public static explicit operator Byte( Digit digit ) => digit.Value;

	public static implicit operator Digit( Byte value ) => new(value);

	public static Boolean operator <( Digit left, Digit right ) => left.Value < right.Value;

	public static Boolean operator <( Digit left, SByte right ) => left.Value < right;

	public static Boolean operator <( Digit left, Byte right ) => left.Value < right;

	public static Boolean operator <( Byte left, Digit right ) => left < right.Value;

	public static Boolean operator <( SByte left, Digit right ) => left < right.Value;

	public static Boolean operator <=( Digit left, Digit right ) => left.Value <= right.Value;

	public static Boolean operator <=( Digit left, SByte right ) => left.Value <= right;

	public static Boolean operator <=( Digit left, Byte right ) => left.Value <= right;

	public static Boolean operator <=( Byte left, Digit right ) => left <= right.Value;

	public static Boolean operator <=( SByte left, Digit right ) => left <= right.Value;

	public static Boolean operator >( Digit left, Digit right ) => left.Value > right.Value;

	public static Boolean operator >( Digit left, SByte right ) => left.Value > right;

	public static Boolean operator >( Digit left, Byte right ) => left.Value > right;

	public static Boolean operator >( Byte left, Digit right ) => left > right.Value;

	public static Boolean operator >( SByte left, Digit right ) => left > right.Value;

	public static Boolean operator >=( Digit left, Digit right ) => left.Value >= right.Value;

	public static Boolean operator >=( Digit left, SByte right ) => left.Value >= right;

	public static Boolean operator >=( Digit left, Byte right ) => left.Value >= right;

	public static Boolean operator >=( Byte left, Digit right ) => left >= right.Value;

	public static Boolean operator >=( SByte left, Digit right ) => left >= right.Value;

	public Boolean Equals( Byte other ) => this.Value.Equals( other );

	public String Number() => this.Value.ToString();

	public override String ToString() {
		return this.Value switch {
			0 => nameof( Zero ),
			1 => nameof( One ),
			2 => nameof( Two ),
			3 => nameof( Three ),
			4 => nameof( Four ),
			5 => nameof( Five ),
			6 => nameof( Six ),
			7 => nameof( Seven ),
			8 => nameof( Eight ),
			9 => nameof( Nine ),
			var _ => throw new ArgumentOutOfRangeException( nameof( this.Value ), "Invalid internal value!" )
		};
	}

	public override Int32 GetHashCode() => this.Value.GetHashCode();

}