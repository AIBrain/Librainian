// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Minutes.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Numerics;
    using Parsing;

    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct Minutes : IComparable<Minutes>, IQuantityOfTime {

        /// <summary>
        ///     60
        /// </summary>
        public const Byte InOneHour = 60;

        /// <summary>
        ///     15
        /// </summary>
        public static readonly Minutes Fifteen = new Minutes( value: 15 );

        /// <summary>
        ///     One <see cref="Minutes" /> .
        /// </summary>
        public static readonly Minutes One = new Minutes( value: 1 );

        /// <summary>
        ///     10
        /// </summary>
        public static readonly Minutes Ten = new Minutes( value: 10 );

        /// <summary>
        ///     30
        /// </summary>
        public static readonly Minutes Thirty = new Minutes( value: 30 );

        /// <summary>
        /// </summary>
        public static readonly Minutes Thousand = new Minutes( value: 1000 );

        /// <summary>
        ///     Zero <see cref="Minutes" />
        /// </summary>
        public static readonly Minutes Zero = new Minutes( value: 0 );

        public Minutes( Decimal value ) => this.Value = value;

	    public Minutes( BigRational value ) => this.Value = value;

	    public Minutes( Int64 value ) => this.Value = value;

	    public Minutes( BigInteger value ) => this.Value = value;

	    [JsonProperty]
        public BigRational Value {
            get;
        }

        public static Minutes Combine( Minutes left, Minutes right ) => Combine( left, right.Value );

        public static Minutes Combine( Minutes left, BigRational minutes ) => new Minutes( left.Value + minutes );

        public static Minutes Combine( Minutes left, BigInteger minutes ) => new Minutes( ( BigInteger )left.Value + minutes );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Minutes left, Minutes right ) => left.Value == right.Value;

        /// <summary>
        ///     Implicitly convert the number of <paramref name="minutes" /> to <see cref="Hours" />.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static implicit operator Hours( Minutes minutes ) => minutes.ToHours();

        /// <summary>
        ///     Implicitly convert the number of <paramref name="minutes" /> to <see cref="Seconds" />.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static implicit operator Seconds( Minutes minutes ) => minutes.ToSeconds();

        /// <summary>
        ///     Implicitly convert the number of <paramref name="minutes" /> to a <see cref="Span" />.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static implicit operator Span( Minutes minutes ) => new Span( minutes );

        public static implicit operator TimeSpan( Minutes minutes ) => TimeSpan.FromMinutes( ( Double )minutes.Value );

        public static Minutes operator -( Minutes minutes ) => new Minutes( minutes.Value * -1 );

        public static Minutes operator -( Minutes left, Minutes right ) => Combine( left: left, right: -right );

        public static Minutes operator -( Minutes left, Decimal minutes ) => Combine( left, -minutes );

        public static Boolean operator !=( Minutes left, Minutes right ) => !Equals( left, right );

        public static Minutes operator +( Minutes left, Minutes right ) => Combine( left, right );

        public static Minutes operator +( Minutes left, Decimal minutes ) => Combine( left, minutes );

        public static Minutes operator +( Minutes left, BigInteger minutes ) => Combine( left, minutes );

        public static Boolean operator <( Minutes left, Minutes right ) => left.Value < right.Value;

        public static Boolean operator <( Minutes left, Hours right ) => ( Hours )left < right;

        public static Boolean operator <( Minutes left, Seconds right ) => left < ( Minutes )right;

        public static Boolean operator ==( Minutes left, Minutes right ) => Equals( left, right );

        public static Boolean operator >( Minutes left, Hours right ) => ( Hours )left > right;

        public static Boolean operator >( Minutes left, Minutes right ) => left.Value > right.Value;

        public static Boolean operator >( Minutes left, Seconds right ) => left > ( Minutes )right;

        public Int32 CompareTo( Minutes other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Minutes other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return false;
            }
            return obj is Minutes minutes && this.Equals( minutes );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public Hours ToHours() => new Hours( this.Value / InOneHour );

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneMinute * this.Value );

        [Pure]
        public Seconds ToSeconds() => new Seconds( this.Value * Seconds.InOneMinute );

        [Pure]
        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.GetWholePart();
                return $"{whole} {whole.PluralOf( "minute" )}";
            }
            var dec = ( Decimal )this.Value;
            return $"{dec} {dec.PluralOf( "minute" )}";
        }
    }
}