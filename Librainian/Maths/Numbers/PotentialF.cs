// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "PotentialF.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "PotentialF.cs" was last formatted by Protiguous on 2018/06/04 at 4:06 PM.

namespace Librainian.Maths.Numbers {

	using System;
	using System.Threading;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>Restricts the value to between 0.0 and 1.0.</para>
	/// </summary>
	/// <remarks>
	///     <para>Just wanted a threadsafe wrapper for Min and Max.</para>
	/// </remarks>
	[JsonObject]
	public sealed class PotentialF {

		/// <summary>
		///     <para>Thread-safe getter and setter.</para>
		/// </summary>
		/// <remarks>
		///     <para>
		///         Constrains the value to stay between <see cref="MinValue" /> and <see cref="MaxValue" /> .
		///     </para>
		/// </remarks>
		public Single Value {
			get => Thread.VolatileRead( ref this._value );

			private set => Thread.VolatileWrite( ref this._value, value >= MaxValue ? MaxValue : ( value <= MinValue ? MinValue : value ) );
		}

		/// <summary></summary>
		/// <remarks>ONLY used in the getter and setter.</remarks>
		[JsonProperty]
		private Single _value = MinValue;

		public static implicit operator Single( [NotNull] PotentialF special ) => special.Value;

		[NotNull]
		public static PotentialF Parse( [NotNull] String value ) => new PotentialF( Single.Parse( value ) );

		public void Add( Single amount ) => this.Value += amount;

		public void Divide( Single amount ) => this.Value /= amount;

		public override Int32 GetHashCode() => this.Value.GetHashCode();

		public void Multiply( Single amount ) => this.Value *= amount;

		public override String ToString() => $"{this.Value:P3}";

		/// <summary>1</summary>
		public const Single MaxValue = 1.0f;

		/// <summary>
		///     <para>0.000000000000000000000000000000000000000000001401298</para>
		///     <para>"1.401298E-45"</para>
		/// </summary>
		public const Single MinValue = 0.0f;

		/// <summary>Initializes a random number between <see cref="MinValue" /> and <see cref="MaxValue" /></summary>
		public PotentialF( Boolean randomValue ) {
			if ( randomValue ) { this.Value = Randem.NextFloat( MinValue, MaxValue ); }
		}

		/// <summary>Initializes with <paramref name="initialValue" />.</summary>
		/// <param name="initialValue"></param>
		public PotentialF( Single initialValue ) => this.Value = initialValue;

		public PotentialF( Single min, Single max ) : this( Randem.NextFloat( min: min, max: max ) ) { }

	}

}