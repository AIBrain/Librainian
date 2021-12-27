// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "Hash.cs" last formatted on 2020-08-14 at 8:36 PM.

namespace Librainian.Financial.Currency.BTC;

using System;
using System.Linq;

	
/// <see cref="http://github.com/mb300sd/Bitcoin-Tool" />
public class Hash {

	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>" )]
	public readonly Byte[] HashBytes;

	public Byte this[Int32 i] {
		get => this.HashBytes[i];

		set => this.HashBytes[i] = value;
	}

	public Hash( Byte[] b ) => this.HashBytes = b;

	public static implicit operator Byte[]( Hash hash ) => hash.HashBytes;

	public static implicit operator Hash( Byte[] bytes ) => new( bytes );

	public override Boolean Equals( Object? obj ) => obj is Hash hash1 && this.HashBytes.SequenceEqual( hash1.HashBytes );

	public override Int32 GetHashCode() {
		if ( this.HashBytes.Length >= 4 ) {
			return ( this.HashBytes[0] << 24 ) | ( this.HashBytes[1] << 16 ) | ( this.HashBytes[2] << 8 ) | ( this.HashBytes[3] << 0 );
		}

		return this.HashBytes.GetHashCode();
	}
}