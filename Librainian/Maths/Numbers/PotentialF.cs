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
// File "PotentialF.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

namespace Librainian.Maths.Numbers;

using System;
using System.Threading;
using Newtonsoft.Json;

/// <summary>
/// <para>Restricts the value to between 0.0 and 1.0.</para>
/// </summary>
/// <remarks>
/// <para>Just wanted a threadsafe wrapper.</para>
/// </remarks>
[JsonObject]
public class PotentialF {

	/// <remarks>ONLY used in the getter and setter.</remarks>
	[JsonProperty]
	private Single _value = MinValue;

	public const Single MaxValue = 1.0f;

	public const Single MinValue = 0.0f;

	/// <summary>Initializes a random number between <see cref="MinValue" /> and <see cref="MaxValue" /></summary>
	public PotentialF( Boolean randomValue ) {
		if ( randomValue ) {
			this.Value = Randem.NextFloat();
		}
	}

	/// <summary>Initializes with <paramref name="initialValue" />.</summary>
	/// <param name="initialValue"></param>
	public PotentialF( Single initialValue ) => this.Value = initialValue;

	/// <summary>
	/// Sets the internal value to a random number between <paramref name="min" /> and <paramref name="max" />, and between 0
	/// and 1.
	/// </summary>
	/// <param name="min"></param>
	/// <param name="max"></param>
	public PotentialF( Single min, Single max ) : this( Randem.NextFloat( min, max ) ) { }

	/// <summary>
	/// <para>Thread-safe getter and setter.</para>
	/// </summary>
	/// <remarks>
	/// <para>Constrains the value to stay between <see cref="MinValue" /> and <see cref="MaxValue" /> .</para>
	/// </remarks>
	public Single Value {
		get => Thread.VolatileRead( ref this._value );

		private set =>
			Thread.VolatileWrite( ref this._value, value >= MaxValue ? MaxValue :
				value <= MinValue ? MinValue : value );
	}

	public static implicit operator Single( PotentialF special ) => special.Value;

	public static PotentialF Parse( String value ) => new( Single.Parse( value ) );

	public void Add( Single amount ) => this.Value += amount;

	public void Divide( Single amount ) => this.Value /= amount;

	public override Int32 GetHashCode() => this.Value.GetHashCode();

	public void Multiply( Single amount ) => this.Value *= amount;

	public override String ToString() => $"{this.Value:P3}";
}