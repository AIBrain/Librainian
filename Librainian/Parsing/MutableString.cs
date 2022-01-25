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
// File "MutableString.cs" last formatted on 2022-12-22 at 5:20 PM by Protiguous.

namespace Librainian.Parsing;

using System;
using System.Text;

/// <summary>
///     Mutable extended string type.
/// </summary>
public class MutableString {

	private const Int32 DefaultExtraBufferSize = 2048;

	private readonly StringBuilder _value;

	public MutableString() => this._value = new StringBuilder();

	public MutableString( Int32 capacity ) => this._value = new StringBuilder( capacity );

	public MutableString( Int32 capacity, Int32 maxCapacity ) => this._value = new StringBuilder( capacity, maxCapacity );

	public MutableString( StringBuilder value, Int32? bufferCapacity = DefaultExtraBufferSize ) : this() {
		this.Append( value.ToString() );
		try {
			this._value.Capacity += bufferCapacity ?? DefaultExtraBufferSize;
		}
		catch ( ArgumentOutOfRangeException ) { }
	}

	public MutableString( String value, Int32? bufferCapacity = DefaultExtraBufferSize ) : this() {
		this.Append( value );
		try {
			this._value.Capacity += bufferCapacity ?? DefaultExtraBufferSize;
		}
		catch ( ArgumentOutOfRangeException ) { }
	}

	public Int32 Length => this._value.Length;

	public String Value {
		get => this._value.ToString();

		set {
			this.Clear();
			this.Append( value );
		}
	}

	public Char this[ Int32 i ] {
		get => this._value[ i ];

		set => this._value[ i ] = value;
	}

	public MutableString this[ Int32 startIndex, Int32 length ] => this.Value.Substring( startIndex, length );

	public Char this[ Index i ] {
		get => this._value[ i ];

		set => this._value[ i ] = value;
	}

	public MutableString this[ Range r ] => this.Value[ r ];

	public static implicit operator MutableString( String value ) => new( value );

	public static implicit operator String( MutableString value ) => value.Value;

	public static MutableString operator +( MutableString a, MutableString b ) {
		a.Append( b );
		return a;
	}

	public void Append( String value ) => this._value.Append( value );

	public void Append( MutableString value ) => this._value.Append( value );

	public void Clear() => this._value.Clear();

	public MutableString Copy() => new( this._value );

	public void Remove( Int32 startIndex, Int32 length ) => this._value.Remove( startIndex, length );

	public void Replace( MutableString oldValue, MutableString newValue ) => this._value.Replace( oldValue, newValue );

	public override String ToString() => this.Value;
}