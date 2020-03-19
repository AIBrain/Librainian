// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Hours.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "Hours.cs" was last formatted by Protiguous on 2020/03/16 at 3:08 PM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Extensions;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;
    using Rationals;

    [JsonObject]
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct Hours : IComparable<Hours>, IQuantityOfTime {

        /// <summary>24</summary>
        public const SByte InOneDay = 24;

        /// <summary>Eight <see cref="Hours" /> .</summary>
        public static readonly Hours Eight = new Hours( value: 8 );

        /// <summary>One <see cref="Hours" /> .</summary>
        public static readonly Hours One = new Hours( value: 1 );

        /// <summary></summary>
        public static readonly Hours Ten = new Hours( value: 10 );

        /// <summary></summary>
        public static readonly Hours Thousand = new Hours( value: 1000 );

        /// <summary>Zero <see cref="Hours" /></summary>
        public static readonly Hours Zero = new Hours( value: 0 );

        [JsonProperty]
        public Rational Value { get; }

        public Hours( Decimal value ) => this.Value = ( Rational ) value;

        public Hours( Rational value ) => this.Value = value;

        public Hours( Int64 value ) => this.Value = value;

        public Hours( BigInteger value ) => this.Value = value;

        public static Hours Combine( Hours left, Hours right ) => Combine( left: left, hours: right.Value );

        public static Hours Combine( Hours left, Rational hours ) => new Hours( value: left.Value + hours );

        public static Hours Combine( Hours left, BigInteger hours ) => new Hours( value: left.Value + hours );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Hours left, Hours right ) => left.Value == right.Value;

        /// <summary>Implicitly convert the number of <paramref name="hours" /> to <see cref="Days" />.</summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static implicit operator Days( Hours hours ) => hours.ToDays();

        /// <summary>Implicitly convert the number of <paramref name="hours" /> to <see cref="Minutes" />.</summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static implicit operator Minutes( Hours hours ) => hours.ToMinutes();

        public static implicit operator SpanOfTime( Hours hours ) => new SpanOfTime( timeSpan: hours );

        public static implicit operator TimeSpan( Hours hours ) => TimeSpan.FromHours( value: ( Double ) hours.Value );

        public static Hours operator -( Hours hours ) => new Hours( value: hours.Value * -1 );

        public static Hours operator -( Hours left, Hours right ) => Combine( left: left, right: -right );

        public static Hours operator -( Hours left, Decimal hours ) => Combine( left: left, hours: ( Rational ) ( -hours ) );

        public static Boolean operator !=( Hours left, Hours right ) => !Equals( left: left, right: right );

        public static Hours operator +( Hours left, Hours right ) => Combine( left: left, right: right );

        public static Hours operator +( Hours left, Decimal hours ) => Combine( left: left, hours: ( Rational ) hours );

        public static Hours operator +( Hours left, BigInteger hours ) => Combine( left: left, hours: hours );

        public static Boolean operator <( Hours left, Hours right ) => left.Value < right.Value;

        public static Boolean operator <( Hours left, Minutes right ) => left < ( Hours ) right;

        public static Boolean operator ==( Hours left, Hours right ) => Equals( left: left, right: right );

        public static Boolean operator >( Hours left, Minutes right ) => left > ( Hours ) right;

        public static Boolean operator >( Hours left, Hours right ) => left.Value > right.Value;

        public Int32 CompareTo( Hours other ) => this.Value.CompareTo( other: other.Value );

        public Boolean Equals( Hours other ) => Equals( left: this, right: other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return default;
            }

            return obj is Hours hours && this.Equals( other: hours );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public Days ToDays() => new Days( value: this.Value / InOneDay );

        public Minutes ToMinutes() => new Minutes( value: this.Value * Minutes.InOneHour );

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( value: this.Value.WholePart * ( BigInteger ) PlanckTimes.InOneHour );

        public Seconds ToSeconds() => new Seconds( value: this.Value / Seconds.InOneHour );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( singular: "hour" )}";
            }

            var dec = ( Decimal ) this.Value;

            return $"{dec} {dec.PluralOf( singular: "hour" )}";
        }

        public TimeSpan ToTimeSpan() => this.ToSeconds();

        /*
		 * Add this? months aren't always 30 days..

		/// <summary>
		///     730 <see cref="Hours" /> in one month, according to WolframAlpha.
		/// </summary>
		/// <see cref="http://www.wolframalpha.com/input/?i=converts+1+month+to+hours" />
		public static BigInteger InOneMonth = 730;
		*/

    }

}