// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Years.cs",
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
// "Librainian/Librainian/Years.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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
    public struct Years : IComparable<Years>, IQuantityOfTime {

        /// <summary>
        ///     One <see cref="Years" /> .
        /// </summary>
        public static readonly Years One = new Years( 1 );

        /// <summary>
        /// </summary>
        public static readonly Years Ten = new Years( 10 );

        /// <summary>
        /// </summary>
        public static readonly Years Thousand = new Years( 1000 );

        /// <summary>
        ///     Zero <see cref="Years" />
        /// </summary>
        public static readonly Years Zero = new Years( 0 );

        [JsonProperty]
        public BigRational Value { get; }

        public Years( Decimal value ) => this.Value = value;

        public Years( BigRational value ) => this.Value = value;

        public Years( Int64 value ) => this.Value = value;

        public Years( BigInteger value ) => this.Value = value;

        public static Years Combine( Years left, Years right ) => Combine( left, right.Value );

        public static Years Combine( Years left, Decimal years ) => new Years( left.Value + years );

        public static Years Combine( Years left, BigRational years ) => new Years( left.Value + years );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Years left, Years right ) => left.Value == right.Value;

        public static implicit operator Months( Years years ) => years.ToMonths();

        public static implicit operator Span( Years years ) => new Span( years: years.Value );

        public static Years operator -( Years days ) => new Years( days.Value * -1 );

        public static Years operator -( Years left, Years right ) => Combine( left: left, right: -right );

        public static Years operator -( Years left, Decimal years ) => Combine( left, -years );

        public static Boolean operator !=( Years left, Years right ) => !Equals( left, right );

        public static Years operator +( Years left, Years right ) => Combine( left, right );

        public static Years operator +( Years left, Decimal years ) => Combine( left, years );

        public static Years operator +( Years left, BigInteger years ) => Combine( left, years );

        public static Boolean operator <( Years left, Years right ) => left.Value < right.Value;

        public static Boolean operator ==( Years left, Years right ) => Equals( left, right );

        public static Boolean operator >( Years left, Years right ) => left.Value > right.Value;

        public Int32 CompareTo( Years other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Years other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) { return false; }

            return obj is Years years && this.Equals( years );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public Days ToDays() => new Days( this.Value * Days.InOneCommonYear );

        [Pure]
        public Months ToMonths() => new Months( this.Value * Months.InOneCommonYear );

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneYear * this.Value );

        [Pure]
        public Seconds ToSeconds() => new Seconds( this.Value * Seconds.InOneCommonYear );

        [Pure]
        public override String ToString() {
            if ( this.Value > Constants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.GetWholePart();

                return $"{whole} {whole.PluralOf( "year" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( "year" )}";
        }

        [Pure]
        public Weeks ToWeeks() => new Weeks( this.Value * Weeks.InOneCommonYear );
    }
}