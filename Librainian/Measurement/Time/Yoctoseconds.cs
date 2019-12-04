// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Yoctoseconds.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", "Yoctoseconds.cs" was last formatted by Protiguous on 2019/11/25 at 4:19 PM.

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

    /// <summary></summary>
    /// <see cref="http://wikipedia.org/wiki/Yoctosecond" />
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public class Yoctoseconds : IComparable<Yoctoseconds>, IQuantityOfTime {

        public Int32 CompareTo( [NotNull] Yoctoseconds other ) {
            if ( other == null ) {
                throw new ArgumentNullException( paramName: nameof( other ) );
            }

            return this.Value.CompareTo( other.Value );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( this.Value * ( Rational ) PlanckTimes.InOneYoctosecond );

        [NotNull]
        public Seconds ToSeconds() => new Seconds( this.Value * InOneSecond );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( "ys" )}";
            }

            var dec = ( Decimal ) this.Value;

            return $"{dec} {dec.PluralOf( "ys" )}";
        }

        public TimeSpan ToTimeSpan() => this.ToSeconds();

        public static Rational InOneSecond { get; } = new BigInteger( 10E24 );

        [JsonProperty]
        public Rational Value { get; }

        /// <summary>1000</summary>
        public const UInt16 InOneZeptosecond = 1000;

        /// <summary><see cref="Five" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Five = new Yoctoseconds( 5 );

        /// <summary><see cref="One" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds One = new Yoctoseconds( 1 );

        /// <summary><see cref="Seven" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Seven = new Yoctoseconds( 7 );

        /// <summary><see cref="Ten" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Ten = new Yoctoseconds( 10 );

        /// <summary><see cref="Thirteen" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Thirteen = new Yoctoseconds( 13 );

        /// <summary><see cref="Thirty" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Thirty = new Yoctoseconds( 30 );

        /// <summary><see cref="Three" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Three = new Yoctoseconds( 3 );

        /// <summary><see cref="Two" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Two = new Yoctoseconds( 2 );

        /// <summary></summary>
        public static Yoctoseconds Zero = new Yoctoseconds( 0 );

        public Yoctoseconds( Decimal value ) => this.Value = ( Rational ) value;

        public Yoctoseconds( Rational value ) => this.Value = value;

        public Yoctoseconds( Int64 value ) => this.Value = value;

        public Yoctoseconds( BigInteger value ) => this.Value = value;

        [CanBeNull]
        public static Yoctoseconds Combine( [NotNull] Yoctoseconds left, [NotNull] Yoctoseconds right ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right == null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return Combine( left, right.Value );
        }

        [NotNull]
        public static Yoctoseconds Combine( [NotNull] Yoctoseconds left, Rational yoctoseconds ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return new Yoctoseconds( left.Value + yoctoseconds );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Yoctoseconds left, [CanBeNull] Yoctoseconds right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return false;
            }

            return left.Value == right.Value;
        }

        /// <summary>Implicitly convert the number of <paramref name="yoctoseconds" /> to <see cref="PlanckTimes" />.</summary>
        /// <param name="yoctoseconds"></param>
        /// <returns></returns>
        [CanBeNull]
        public static implicit operator PlanckTimes( [NotNull] Yoctoseconds yoctoseconds ) {
            if ( yoctoseconds == null ) {
                throw new ArgumentNullException( paramName: nameof( yoctoseconds ) );
            }

            return ToPlanckTimes( yoctoseconds );
        }

        [NotNull]
        public static implicit operator SpanOfTime( [NotNull] Yoctoseconds yoctoseconds ) {
            if ( yoctoseconds == null ) {
                throw new ArgumentNullException( paramName: nameof( yoctoseconds ) );
            }

            return new SpanOfTime( yoctoseconds: yoctoseconds );
        }

        /// <summary>Implicitly convert the number of <paramref name="yoctoseconds" /> to <see cref="Zeptoseconds" />.</summary>
        /// <param name="yoctoseconds"></param>
        /// <returns></returns>
        public static implicit operator Zeptoseconds( [NotNull] Yoctoseconds yoctoseconds ) {
            if ( yoctoseconds == null ) {
                throw new ArgumentNullException( paramName: nameof( yoctoseconds ) );
            }

            return yoctoseconds.ToZeptoseconds();
        }

        [NotNull]
        public static Yoctoseconds operator -( [NotNull] Yoctoseconds yoctoseconds ) {
            if ( yoctoseconds == null ) {
                throw new ArgumentNullException( paramName: nameof( yoctoseconds ) );
            }

            return new Yoctoseconds( yoctoseconds.Value * -1 );
        }

        [CanBeNull]
        public static Yoctoseconds operator -( [NotNull] Yoctoseconds left, [NotNull] Yoctoseconds right ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right == null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return Combine( left: left, right: -right );
        }

        [NotNull]
        public static Yoctoseconds operator -( [NotNull] Yoctoseconds left, Decimal seconds ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return Combine( left, ( Rational ) ( -seconds ) );
        }

        public static Boolean operator !=( [NotNull] Yoctoseconds left, [NotNull] Yoctoseconds right ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right == null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return !Equals( left, right );
        }

        [CanBeNull]
        public static Yoctoseconds operator +( [NotNull] Yoctoseconds left, [NotNull] Yoctoseconds right ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right == null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return Combine( left, right );
        }

        [NotNull]
        public static Yoctoseconds operator +( [NotNull] Yoctoseconds left, Decimal yoctoseconds ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return Combine( left, ( Rational ) yoctoseconds );
        }

        public static Boolean operator <( [NotNull] Yoctoseconds left, [NotNull] Yoctoseconds right ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right == null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return left.Value < right.Value;
        }

        public static Boolean operator ==( [CanBeNull] Yoctoseconds left, [CanBeNull] Yoctoseconds right ) => Equals( left, right );

        public static Boolean operator >( [NotNull] Yoctoseconds left, [NotNull] Yoctoseconds right ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right == null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return left.Value > right.Value;
        }

        [NotNull]
        public static PlanckTimes ToPlanckTimes( [NotNull] Yoctoseconds yoctoseconds ) {
            if ( yoctoseconds == null ) {
                throw new ArgumentNullException( paramName: nameof( yoctoseconds ) );
            }

            return new PlanckTimes( yoctoseconds.Value * ( Rational ) PlanckTimes.InOneYoctosecond );
        }

        public Boolean Equals( [CanBeNull] Yoctoseconds other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return false;
            }

            return obj is Yoctoseconds yoctoseconds && this.Equals( yoctoseconds );
        }

        public Zeptoseconds ToZeptoseconds() => new Zeptoseconds( this.Value / InOneZeptosecond );

    }

}