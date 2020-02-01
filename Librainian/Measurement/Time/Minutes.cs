// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Minutes.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "Minutes.cs" was last formatted by Protiguous on 2020/01/31 at 12:27 AM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;
    using Rationals;

    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public class Minutes : IComparable<Minutes>, IQuantityOfTime {

        public Int32 CompareTo( [NotNull] Minutes other ) {
            if ( other is null ) {
                throw new ArgumentNullException( nameof( other ) );
            }

            return this.Value.CompareTo( other.Value );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( ( Rational ) PlanckTimes.InOneMinute * this.Value );

        [NotNull]
        [Pure]
        public Seconds ToSeconds() => new Seconds( this.Value * Seconds.InOneMinute );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( "minute" )}";
            }

            var dec = ( Decimal ) this.Value;

            return $"{dec} {dec.PluralOf( "minute" )}";
        }

        public TimeSpan ToTimeSpan() => this.ToSeconds();

        [JsonProperty]
        public Rational Value { get; }

        /// <summary>60</summary>
        public const Byte InOneHour = 60;

        /// <summary>15</summary>
        public static Minutes Fifteen = new Minutes( 15 );

        /// <summary>One <see cref="Minutes" /> .</summary>
        public static Minutes One = new Minutes( 1 );

        /// <summary>10</summary>
        public static Minutes Ten = new Minutes( 10 );

        /// <summary>30</summary>
        public static Minutes Thirty = new Minutes( 30 );

        /// <summary></summary>
        public static Minutes Thousand = new Minutes( 1000 );

        /// <summary>Zero <see cref="Minutes" /></summary>
        public static Minutes Zero = new Minutes( 0 );

        public Minutes( Decimal value ) => this.Value = ( Rational ) value;

        public Minutes( Rational value ) => this.Value = value;

        public Minutes( Int64 value ) => this.Value = value;

        public Minutes( BigInteger value ) => this.Value = value;

        [NotNull]
        public static Minutes Combine( [NotNull] Minutes left, [NotNull] Minutes right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            return Combine( left, right.Value );
        }

        [NotNull]
        public static Minutes Combine( [NotNull] Minutes left, Rational minutes ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            return new Minutes( left.Value + minutes );
        }

        [NotNull]
        public static Minutes Combine( [NotNull] Minutes left, BigInteger minutes ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            return new Minutes( left.Value + minutes );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Minutes left, [CanBeNull] Minutes right ) => left?.Value == right?.Value;

        /// <summary>Implicitly convert the number of <paramref name="minutes" /> to <see cref="Hours" />.</summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        [CanBeNull]
        public static implicit operator Hours( [NotNull] Minutes minutes ) {
            if ( minutes is null ) {
                throw new ArgumentNullException( nameof( minutes ) );
            }

            return minutes.ToHours();
        }

        /// <summary>Implicitly convert the number of <paramref name="minutes" /> to <see cref="Seconds" />.</summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        [NotNull]
        public static implicit operator Seconds( [NotNull] Minutes minutes ) {
            if ( minutes is null ) {
                throw new ArgumentNullException( nameof( minutes ) );
            }

            return minutes.ToSeconds();
        }

        /// <summary>Implicitly convert the number of <paramref name="minutes" /> to a <see cref="SpanOfTime" />.</summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        [NotNull]
        public static implicit operator SpanOfTime( [NotNull] Minutes minutes ) {
            if ( minutes is null ) {
                throw new ArgumentNullException( nameof( minutes ) );
            }

            return new SpanOfTime( minutes );
        }

        public static implicit operator TimeSpan( [NotNull] Minutes minutes ) {
            if ( minutes is null ) {
                throw new ArgumentNullException( nameof( minutes ) );
            }

            return TimeSpan.FromMinutes( ( Double ) minutes.Value );
        }

        [NotNull]
        public static Minutes operator -( [NotNull] Minutes minutes ) {
            if ( minutes is null ) {
                throw new ArgumentNullException( nameof( minutes ) );
            }

            return new Minutes( minutes.Value * -1 );
        }

        [NotNull]
        public static Minutes operator -( [NotNull] Minutes left, [NotNull] Minutes right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            return Combine( left: left, right: -right );
        }

        [NotNull]
        public static Minutes operator -( [NotNull] Minutes left, Decimal minutes ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            return Combine( left, ( Rational ) ( -minutes ) );
        }

        public static Boolean operator !=( [NotNull] Minutes left, [NotNull] Minutes right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            return !Equals( left, right );
        }

        [NotNull]
        public static Minutes operator +( [NotNull] Minutes left, [NotNull] Minutes right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            return Combine( left, right );
        }

        [NotNull]
        public static Minutes operator +( [NotNull] Minutes left, Decimal minutes ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            return Combine( left, ( Rational ) minutes );
        }

        [NotNull]
        public static Minutes operator +( [NotNull] Minutes left, BigInteger minutes ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            return Combine( left, minutes );
        }

        public static Boolean operator <( [NotNull] Minutes left, [NotNull] Minutes right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            return left.Value < right.Value;
        }

        public static Boolean operator <( [NotNull] Minutes left, [CanBeNull] Hours right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            return ( Hours ) left < right;
        }

        public static Boolean operator <( [NotNull] Minutes left, [NotNull] Seconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            return left < ( Minutes ) right;
        }

        public static Boolean operator ==( [NotNull] Minutes left, [NotNull] Minutes right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            return Equals( left, right );
        }

        public static Boolean operator >( [CanBeNull] Minutes left, [CanBeNull] Hours right ) => ( Hours ) left > right;

        public static Boolean operator >( [NotNull] Minutes left, [NotNull] Minutes right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            return left.Value > right.Value;
        }

        public static Boolean operator >( [NotNull] Minutes left, [NotNull] Seconds right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            return left > ( Minutes ) right;
        }

        public Boolean Equals( [NotNull] Minutes other ) {
            if ( other is null ) {
                throw new ArgumentNullException( nameof( other ) );
            }

            return Equals( this, other );
        }

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return default;
            }

            return obj is Minutes minutes && this.Equals( minutes );
        }

        [NotNull]
        public Hours ToHours() => new Hours( this.Value / InOneHour );

    }

}