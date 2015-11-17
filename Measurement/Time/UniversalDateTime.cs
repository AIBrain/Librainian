// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/UniversalDateTime.cs" was last cleaned by Rick on 2015/10/07 at 8:57 PM

namespace Librainian.Measurement.Time {

    using System;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Extensions;

    /// <summary>
    ///     <para>Absolute universal date and time.</para>
    ///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Lol" />
    [Immutable]
    [DataContract]
    public struct UniversalDateTime : IComparable< UniversalDateTime > {

        /// <summary>
        ///     <para>1 planck times</para>
        /// </summary>
        public static UniversalDateTime One { get; } = new UniversalDateTime( BigInteger.One );

        /// <summary>
        ///     <para>The value of this constant is equivalent to 00:00:00.0000000, January 1, 0001.</para>
        ///     <para>430,000,000,000,000,000 seconds</para>
        /// </summary>
        public static PlanckTimes PlancksUpToMinDateTime { get; } = new PlanckTimes( new Seconds( 4.3E17m ) );

        /// <summary>
        ///     <para>0 planck times</para>
        /// </summary>
        public static UniversalDateTime TheBeginning { get; } = new UniversalDateTime( BigInteger.Zero );

        public static UniversalDateTime Unix { get; } = new UniversalDateTime( Epochs.Unix );

        /// <summary>
        /// </summary>
        [DataMember]
        public Date Date { get; }

        /// <summary>
        /// </summary>
        [DataMember]
        public Time Time { get; }

        /// <summary>
        ///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
        /// </summary>
        [DataMember]
        public BigInteger Value { get; }

        public UniversalDateTime( BigInteger planckTimesSinceBigBang ) {
            this.Value = planckTimesSinceBigBang;
            var span = new Span( planckTimes: this.Value );

            //TODO
            this.Date = new Date( span );
            this.Time = new Time( span );
        }

        public UniversalDateTime( DateTime dateTime ) {
            var span = CalcSpanSince( dateTime );

            this.Value = span.TotalPlanckTimes.Value;
            this.Date = new Date( span ); //we can use span here because the values have been normalized. Should()Have()Been()?
            this.Time = new Time( span );

            //this.Time = new Time();
        }

        public static UniversalDateTime Now => new UniversalDateTime( DateTime.UtcNow );

        /// <summary>
        ///     Given a <see cref="DateTime" />, calculate the <see cref="Span" />.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static Span CalcSpanSince( DateTime dateTime ) {
            var sinceThen = new Span( dateTime - DateTime.MinValue );
            var plancksSinceThen = sinceThen.TotalPlanckTimes;
            var span = new Span( planckTimes: PlancksUpToMinDateTime.Value + plancksSinceThen.Value );
            return span;
        }

        public static UniversalDateTime operator -( UniversalDateTime left, UniversalDateTime right ) => Combine( left, -right );

        public static UniversalDateTime operator -( UniversalDateTime universalDateTime ) => new UniversalDateTime( universalDateTime.Value * -1 );

        public static Boolean operator <( UniversalDateTime left, UniversalDateTime right ) => left.Value < right.Value;

        public static Boolean operator >( UniversalDateTime left, UniversalDateTime right ) => left.Value > right.Value;

        public Int32 CompareTo( UniversalDateTime other ) => this.Value.CompareTo( other.Value );

        private static UniversalDateTime Combine( UniversalDateTime left, BigInteger value ) => new UniversalDateTime( left.Value + value );

        private static UniversalDateTime Combine( UniversalDateTime left, UniversalDateTime right ) => Combine( left, right.Value );

    }

}
