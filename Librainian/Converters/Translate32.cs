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
// File "Translate32.cs" last formatted on 2022-12-22 at 5:15 PM by Protiguous.

namespace Librainian.Converters;

using System;
using System.Runtime.InteropServices;

/// <summary>
///     Struct for combining two <see cref="UInt16" /> (or <see cref="Int16" />) to and from a <see cref="UInt32" /> (or
///     <see cref="Int32" />) as easily as possible.
/// </summary>
[StructLayout( LayoutKind.Explicit, Pack = 0 )]
public struct Translate32 {

	[field: FieldOffset( 0 )]
	public UInt32 UnsignedValue { get; set; }

	[field: FieldOffset( 0 )]
	public Int32 SignedValue { get; set; }

	[field: FieldOffset( 0 )]
	public Int16 SignedLow { get; set; }

	[field: FieldOffset( 0 )]
	public UInt16 UnsignedLow { get; set; }

	[field: FieldOffset( sizeof( UInt16 ) )]
	public UInt16 UnsignedHigh { get; set; }

	[field: FieldOffset( sizeof( Int16 ) )]
	public Int16 SignedHigh { get; set; }

	public Translate32( Byte a, Byte b, Byte c, Byte d ) : this( BitConverter.ToInt32( new[] {
		a, b, c, d
	}, 0 ) ) { }

	public Translate32( Int16 signedHigh, Int16 signedLow ) : this() {
		this.SignedHigh = signedHigh;
		this.SignedLow = signedLow;
	}

	public Translate32( UInt32 unsignedValue ) : this() => this.UnsignedValue = unsignedValue;

	public Translate32( Int32 signedValue ) : this() => this.SignedValue = signedValue;

	public Translate32( UInt16 unsignedLow, UInt16 unsignedHigh ) : this() {
		this.UnsignedLow = unsignedLow;
		this.UnsignedHigh = unsignedHigh;
	}
}