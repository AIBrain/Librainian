#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/PlanckTimes.cs" was last cleaned by Rick on 2014/08/09 at 2:16 PM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;
    using NUnit.Framework;

    /// <summary>
    ///     <para>In physics, the Planck time (tP) is the unit of time in the system of natural units known as Planck units.</para>
    ///     <para>It is the time required for light to travel, in a vacuum, a distance of 1 Planck length.</para>
    ///     <para>The Planck time is defined as:[2]</para>
    ///     <para>t_P \equiv \sqrt{\frac{\hbar G}{c^5}} ≈ 5.39106(32) × 10−44 s</para>
    ///     <para>
    ///         where:
    ///         \hbar = h / 2 \pi is the reduced Planck constant (sometimes h is used instead of \hbar in the definition[1])
    ///         G = gravitational constant
    ///         c = speed of light in a vacuum
    ///         s is the SI unit of time, the second.
    ///         The two digits between parentheses denote the standard error of the estimated value.
    ///     </para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Planck_time" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    public struct PlanckTimes : IComparable< PlanckTimes >, IQuantityOfTime {
        /// <summary>
        ///     18548608483392000000
        /// </summary>
        public static readonly BigInteger InOneYoctosecond = new BigInteger( 18548608483392000000m ); //where did I get this number???

        public static readonly BigInteger InOneZeptosecond = BigInteger.Multiply( InOneYoctosecond, Yoctoseconds.InOneZeptosecond );
        public static readonly BigInteger InOneAttosecond = BigInteger.Multiply( InOneZeptosecond, Zeptoseconds.InOneAttosecond );
        public static readonly BigInteger InOneFemtosecond = BigInteger.Multiply( InOneAttosecond, Attoseconds.InOneFemtosecond );
        public static readonly BigInteger InOnePicosecond = BigInteger.Multiply( InOneFemtosecond, Femtoseconds.InOnePicosecond );
        public static readonly BigInteger InOneNanosecond = BigInteger.Multiply( InOnePicosecond, Picoseconds.InOneNanosecond );
        public static readonly BigInteger InOneMicrosecond = BigInteger.Multiply( InOneNanosecond, Nanoseconds.InOneMicrosecond );
        public static readonly BigInteger InOneMillisecond = BigInteger.Multiply( InOneMicrosecond, Microseconds.InOneMillisecond );
        public static readonly BigInteger InOneSecond = BigInteger.Multiply( InOneMillisecond, Milliseconds.InOneSecond );
        public static readonly BigInteger InOneMinute = BigInteger.Multiply( InOneSecond, Seconds.InOneMinute );
        public static readonly BigInteger InOneHour = BigInteger.Multiply( InOneMinute, Minutes.InOneHour );
        public static readonly BigInteger InOneDay = BigInteger.Multiply( InOneHour, Hours.InOneDay );
        public static readonly BigInteger InOneWeek = BigInteger.Multiply( InOneDay, Days.InOneWeek );
        public static readonly BigInteger InOneMonth = BigInteger.Multiply( InOneHour, Hours.InOneMonth );
        public static readonly BigInteger InOneYear = BigInteger.Multiply( InOneMonth, Months.InOneYear );

        //public static readonly BigInteger InOneCentury = BigInteger.Multiply( InOneYear, Years.InOneCentury );
        //public static readonly BigInteger InOneMillenium = BigInteger.Multiply( InOneCentury, Centuries.InOneMillenium );
        //public static readonly BigInteger InOneBillionYear = BigInteger.Multiply( InOneMillenium, Milleniums.InOneBillionYears );

        /// <summary>
        ///     One <see cref="PlanckTimes" />.
        /// </summary>
        public static readonly PlanckTimes One = new PlanckTimes( value: 1 );

        /// <summary>
        ///     Two <see cref="PlanckTimes" />.
        /// </summary>
        public static readonly PlanckTimes Two = new PlanckTimes( value: 2 );

        /// <summary>
        ///     Zero <see cref="PlanckTimes" />.
        /// </summary>
        public static readonly PlanckTimes Zero = new PlanckTimes( value: 0 );

        [DataMember] public readonly BigInteger Value; //TODO Shouldn't this be a BigInteger ? according to wikipedia planck units by definition cannot be spilt into smaller units.

        static PlanckTimes() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeLessThan( Yoctoseconds.One );
        }

        public PlanckTimes( long value ) {
            this.Value = value;
        }

        //public PlanckTimes( Decimal value ) {
        //    this.Value = new BigInteger( value );
        //}

        public PlanckTimes( BigInteger value ) {
            //value.Should().BeInRange( Constants.MinimumUsefulValue, Constants.MaximumUsefulValue );

            //if ( value < Constants.MinimumUsefulValue ) {
            //    throw new OverflowException( Constants.ValueIsTooLow );
            //}

            //if ( value > Constants.MaximumUsefulValue ) {
            //    throw new OverflowException( Constants.ValueIsTooHigh );
            //}
            //this.Value = ( Decimal )value;
            this.Value = value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay { get { return this.ToString(); } }

        public int CompareTo( PlanckTimes other ) {
            return this.Value.CompareTo( other.Value );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return this.Value;
        }

        public override String ToString() {
            return String.Format( "{0:R} tP", this.Value );
        }

        public Boolean Equals( PlanckTimes other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is PlanckTimes && this.Equals( ( PlanckTimes ) obj );
        }

        public static PlanckTimes Combine( PlanckTimes left, PlanckTimes right ) {
            return new PlanckTimes( left.Value + right.Value );
        }

        public static PlanckTimes Combine( PlanckTimes left, BigInteger planckTimes ) {
            return new PlanckTimes( left.Value + planckTimes );
        }

        public static implicit operator Span( PlanckTimes planckTimes ) {
            return new Span( planckTimes );
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="planckTimes" /> to <see cref="Yoctoseconds" />.
        /// </summary>
        /// <param name="planckTimes"></param>
        /// <returns></returns>
        public static implicit operator Yoctoseconds( PlanckTimes planckTimes ) {
            return ToYoctoseconds( planckTimes );
        }

        public static PlanckTimes operator -( PlanckTimes left, BigInteger planckTimes ) {
            return Combine( left, -planckTimes );
        }

        public static Boolean operator <( PlanckTimes left, PlanckTimes right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator >( PlanckTimes left, PlanckTimes right ) {
            return left.Value > right.Value;
        }

        [Test]
        public static void TestConstants() {
            //InOneMillenium.Should().BeGreaterThan( InOneCentury );
            //InOneCentury.Should().BeGreaterThan( InOneYear );
            InOneYear.Should().BeGreaterThan( InOneMonth );
            InOneMonth.Should().BeGreaterThan( InOneWeek );
            InOneWeek.Should().BeGreaterThan( InOneDay );
            InOneDay.Should().BeGreaterThan( InOneHour );
            InOneHour.Should().BeGreaterThan( InOneMinute );
            InOneMinute.Should().BeGreaterThan( InOneSecond );
            InOneSecond.Should().BeGreaterThan( InOneMillisecond );
            InOneMillisecond.Should().BeGreaterThan( InOneMicrosecond );
            InOneMicrosecond.Should().BeGreaterThan( InOneNanosecond );
            InOneNanosecond.Should().BeGreaterThan( InOnePicosecond );
            InOnePicosecond.Should().BeGreaterThan( InOneFemtosecond );
            InOneFemtosecond.Should().BeGreaterThan( InOneAttosecond );
            InOneAttosecond.Should().BeGreaterThan( InOneZeptosecond );
            InOneZeptosecond.Should().BeGreaterThan( InOneYoctosecond );
        }

        /// <summary>
        ///     <para>Convert to a larger unit.</para>
        /// </summary>
        /// <param name="planckTimes"></param>
        /// <returns></returns>
        public static Yoctoseconds ToYoctoseconds( PlanckTimes planckTimes ) {
            return new Yoctoseconds( planckTimes.Value/InOneYoctosecond );
        }

        public static PlanckTimes operator +( PlanckTimes left, PlanckTimes right ) {
            return Combine( left, right );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( PlanckTimes left, PlanckTimes right ) {
            return left.Value == right.Value;
        }

        public static Boolean operator ==( PlanckTimes left, PlanckTimes right ) {
            return Equals( left, right );
        }

        public static Boolean operator !=( PlanckTimes left, PlanckTimes right ) {
            return !Equals( left, right );
        }

        public static PlanckTimes operator +( PlanckTimes left, BigInteger planckTimes ) {
            return Combine( left, planckTimes );
        }
    }
}
