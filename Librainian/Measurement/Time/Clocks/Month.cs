// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>A simple struct for a <see cref="Month" />.</summary>
	[JsonObject]
	[Immutable]
	public record Month : IComparable<Month>, IClockPart {
		public const Byte MaximumValue = 12;

		public const Byte MinimumValue = 0;	//TODO um... not 1??

		public Month( Byte value ) {
			if ( value is < MinimumValue or > MaximumValue ) {
				throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
			}

			this.Value = value;
		}

		/// <summary>12</summary>
		public static Month Maximum { get; } = new( MaximumValue );

		/// <summary>1</summary>
		public static Month Minimum { get; } = new( MinimumValue );

		[JsonProperty]
		public Byte Value { get; init; }

		public Int32 CompareTo( [NotNull] Month? other ) {
			if ( other == null ) {
				throw new ArgumentNullException( nameof( other ) );
			}

			return this.Value.CompareTo( other.Value );
		}

		public static implicit operator Byte( Month value ) => value.Value;

		public static implicit operator Month( Byte value ) => new( value );

		/// <summary>Provide the next <see cref="Month" />.</summary>
		/// <param name="tocked"></param>
		/// <returns></returns>
		public Month Next( out Boolean tocked ) {
			var next = this.Value + 1;

			if ( next > Maximum.Value ) {
				next = Minimum.Value;
				tocked = true;
			}
			else {
				tocked = false;
			}

			return ( Month )next;
		}

		/// <summary>Provide the previous <see cref="Month" />.</summary>
		public Month Previous( out Boolean tocked ) {
			var next = this.Value - 1;

			if ( next < Minimum.Value ) {
				next = Maximum.Value;
				tocked = true;
			}
			else {
				tocked = false;
			}

			return ( Month )next;
		}
	}
}