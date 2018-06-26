// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "WeightObsolete.cs" belongs to Rick@AIBrain.org and
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
// File "WeightObsolete.cs" was last formatted by Protiguous on 2018/06/04 at 4:08 PM.

namespace Librainian.Maths {

	using System;
	using System.Threading;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>A Double number, constrained between <see cref="MinValue" /> and <see cref="MaxValue" />.</para>
	///     <para>thread-safe by Interlocked</para>
	/// </summary>
	[Obsolete]
	[JsonObject]
	public class WeightObsolete {

		public Double Value {
			get => Interlocked.Exchange( ref this._value, this._value );

			set {
				var correctedvalue = value;

				if ( value >= MaxValue ) { correctedvalue = MaxValue; }
				else if ( value <= MinValue ) { correctedvalue = MinValue; }

				Interlocked.Exchange( ref this._value, correctedvalue );
			}
		}

		/// <summary>ONLY used in the getter and setter.</summary>
		[JsonProperty]
		private Double _value;

		public static Double Combine( Double value1, Double value2 ) => ( value1 + value2 ) / 2D;

		public static implicit operator Double( [NotNull] WeightObsolete special ) => special.Value;

		[NotNull]
		public static WeightObsolete Parse( [NotNull] String value ) {
			if ( value is null ) { throw new ArgumentNullException( nameof( value ) ); }

			return new WeightObsolete( Double.Parse( value ) );
		}

		public void AdjustTowardsMax() => this.Value = ( this.Value + MaxValue ) / 2D;

		public void AdjustTowardsMin() => this.Value = ( this.Value + MinValue ) / 2D;

		public Boolean IsAgainst() => this.Value < 0.0D - Double.Epsilon;

		public Boolean IsFor() => this.Value > 0.0D + Double.Epsilon;

		public Boolean IsNeither() => !this.IsFor() && !this.IsAgainst();

		public override String ToString() => $"{this.Value:R}";

		//public object Clone() { return new Weight( this ); }

		/// <summary>1 <see cref="MaxValue" /></summary>
		public const Double MaxValue = +1D;

		/// <summary>- 1 <see cref="MinValue" /></summary>
		public const Double MinValue = -1D;

		/// <summary>Initializes to a random number between 0.0 and 0.50D</summary>
		public WeightObsolete() => this.Value = Randem.NextDouble() * 0.25 + Randem.NextDouble() * 0.25;

		/// <summary>A Double number, constrained between <see cref="MinValue" /> and <see cref="MaxValue" />.</summary>
		/// <param name="value"></param>
		public WeightObsolete( Double value ) => this.Value = value;

	}

}