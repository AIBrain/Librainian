// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Hours.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Hours.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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
    public struct Hours : IComparable<Hours>, IQuantityOfTime {

        /// <summary>
        ///     24
        /// </summary>
        public const Byte InOneDay = 24;

        /// <summary>
        ///     Eight <see cref="Hours" /> .
        /// </summary>
        public static readonly Hours Eight = new Hours( 8 );

        /// <summary>
        ///     One <see cref="Hours" /> .
        /// </summary>
        public static readonly Hours One = new Hours( 1 );

        /// <summary>
        /// </summary>
        public static readonly Hours Ten = new Hours( 10 );

        /// <summary>
        /// </summary>
        public static readonly Hours Thousand = new Hours( 1000 );

        /// <summary>
        ///     Zero <see cref="Hours" />
        /// </summary>
        public static readonly Hours Zero = new Hours( 0 );

        /// <summary>
        ///     730 <see cref="Hours" /> in one month, according to WolframAlpha.
        /// </summary>
        /// <see cref="http://www.wolframalpha.com/input/?i=converts+1+month+to+hours" />
        public static BigInteger InOneMonth = 730;

        public Hours( Decimal value ) => this.Value = value;

        public Hours( BigRational value ) => this.Value = value;

        public Hours( Int64 value ) => this.Value = value;

        public Hours( BigInteger value ) => this.Value = value;

        [JsonProperty]
        public BigRational Value { get; }

        public static Hours Combine( Hours left, Hours right ) => Combine( left, right.Value );

        public static Hours Combine( Hours left, BigRational hours ) => new Hours( left.Value + hours );

        public static Hours Combine( Hours left, BigInteger hours ) => new Hours( ( BigInteger )left.Value + hours );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Hours left, Hours right ) => left.Value == right.Value;

        /// <summary>
        ///     Implicitly convert the number of <paramref name="hours" /> to <see cref="Days" />.
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static implicit operator Days( Hours hours ) => hours.ToDays();

        /// <summary>
        ///     Implicitly convert the number of <paramref name="hours" /> to <see cref="Minutes" />.
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static implicit operator Minutes( Hours hours ) => hours.ToMinutes();

        public static implicit operator Span( Hours hours ) => new Span( hours );

        public static implicit operator TimeSpan( Hours hours ) => TimeSpan.FromHours( ( Double )hours.Value );

        public static Hours operator -( Hours hours ) => new Hours( hours.Value * -1 );

        public static Hours operator -( Hours left, Hours right ) => Combine( left: left, right: -right );

        public static Hours operator -( Hours left, Decimal hours ) => Combine( left, -hours );

        public static Boolean operator !=( Hours left, Hours right ) => !Equals( left, right );

        public static Hours operator +( Hours left, Hours right ) => Combine( left, right );

        public static Hours operator +( Hours left, Decimal hours ) => Combine( left, hours );

        public static Hours operator +( Hours left, BigInteger hours ) => Combine( left, hours );

        public static Boolean operator <( Hours left, Hours right ) => left.Value < right.Value;

        public static Boolean operator <( Hours left, Minutes right ) => left < ( Hours )right;

        public static Boolean operator ==( Hours left, Hours right ) => Equals( left, right );

        public static Boolean operator >( Hours left, Minutes right ) => left > ( Hours )right;

        public static Boolean operator >( Hours left, Hours right ) => left.Value > right.Value;

        public Int32 CompareTo( Hours other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Hours other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) { return false; }

            return obj is Hours hours && this.Equals( hours );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public Days ToDays() => new Days( this.Value / InOneDay );

        [Pure]
        public Minutes ToMinutes() => new Minutes( this.Value * Minutes.InOneHour );

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneHour * this.Value );

        [Pure]
        public override String ToString() {
            if ( this.Value > Constants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.GetWholePart();

                return $"{whole} {whole.PluralOf( "hour" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( "hour" )}";
        }
    }
}