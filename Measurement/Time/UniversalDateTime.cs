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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/UniversalDateTime.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time {

    using System;
    using System.Numerics;
    using Annotations;

    /// <summary>
    ///     <para>Absolute universal date and time.</para>
    ///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Lol" />
    public struct UniversalDateTime : IComparable<UniversalDateTime> {

        /// <summary>
        ///     <para>1 planck times</para>
        /// </summary>
        public static readonly UniversalDateTime One = new UniversalDateTime( BigInteger.One );

        /// <summary>
        ///     <para>The value of this constant is equivalent to 00:00:00.0000000, January 1, 0001.</para>
        ///     <para>430,000,000,000,000,000 seconds</para>
        /// </summary>
        public static readonly PlanckTimes PlancksUpToMinDateTime = new PlanckTimes( new Seconds( 4.3E17m ) );

        /// <summary>
        ///     <para>0 planck times</para>
        /// </summary>
        public static readonly UniversalDateTime TheBeginning = new UniversalDateTime( BigInteger.Zero );

        public static readonly UniversalDateTime UNIX = new UniversalDateTime( Epochs.UNIX );

        /// <summary>
        /// </summary>
        public readonly Date Date;

        /// <summary>
        /// </summary>
        public readonly Time Time;

        /// <summary>
        ///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
        /// </summary>
        public readonly BigInteger Value;

        public UniversalDateTime( BigInteger planckTimesSinceBigBang ) {
            this.Value = planckTimesSinceBigBang;
            var span = new Span( planckTimes: this.Value );

            //TODO
            //this.Date = new Date( span );
            //this.Time = new Time( span );
            this.Date = new Date();
            this.Time = new Time();
        }

        public UniversalDateTime( DateTime dateTime ) {
            var span = CalcSpanSince( dateTime );

            this.Value = span.TotalPlanckTimes;
            this.Date = new Date( span ); //we can use span here because the values have been normalized. Should()
            this.Time = new Time( span ); //we can use span here because the values have been normalized. Should()
        }

        public static UniversalDateTime Now {
            get {
                return new UniversalDateTime( DateTime.UtcNow );
            }
        }

        /// <summary>
        ///     Given a <see cref="DateTime" />, calculate the <see cref="Span" />.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static Span CalcSpanSince( DateTime dateTime ) {
            var sinceThen = new Span( dateTime - DateTime.MinValue );
            var plancksSinceThen = sinceThen.TotalPlanckTimes;
            var span = new Span( planckTimes: PlancksUpToMinDateTime.Value + plancksSinceThen );
            return span;
        }

        public static UniversalDateTime operator -( UniversalDateTime left, UniversalDateTime right ) {
            return Combine( left, -right );
        }

        public static UniversalDateTime operator -( UniversalDateTime universalDateTime ) {
            return new UniversalDateTime( universalDateTime.Value * -1 );
        }

        public static Boolean operator <( UniversalDateTime left, UniversalDateTime right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator >( UniversalDateTime left, UniversalDateTime right ) {
            return left.Value > right.Value;
        }

        [Pure]
        public int CompareTo( UniversalDateTime other ) {
            return this.Value.CompareTo( other.Value );
        }

        private static UniversalDateTime Combine( UniversalDateTime left, BigInteger value ) {
            return new UniversalDateTime( left.Value + value );
        }

        private static UniversalDateTime Combine( UniversalDateTime left, UniversalDateTime right ) {
            return Combine( left, right.Value );
        }
    }
}