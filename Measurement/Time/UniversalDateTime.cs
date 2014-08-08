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
// "Librainian2/UniversalDateTime.cs" was last cleaned by Rick on 2014/08/08 at 2:30 PM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Numerics;

    /// <summary>
    ///     <para>Absolute universal date and time.</para>
    ///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Lol" />
    public struct UniversalDateTime : IComparable< UniversalDateTime > {
        public static readonly PlanckTimes PlancksUpTo1927 = new PlanckTimes( new Years( 13700000000m ).ToPlanckTimes() );

        public static readonly DateTime BigBangProposedAtDate = new DateTime( year: 1927, month: 1, day: 1 );

        public static readonly UniversalDateTime UniversalDateTimeIn1927 = new UniversalDateTime( PlancksUpTo1927.Value );

        public static readonly UniversalDateTime UniversalDateTimeInDateTimeMinValue = new UniversalDateTime( DateTime.MinValue );

        public static readonly UniversalDateTime One = new UniversalDateTime( BigInteger.One );

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

        //public UniversalDateTime( int i )
        //    : this( ( BigInteger )i ) {
        //}

        //public When( DateTime dateTime ) {
        //    // big bang was 13700000000 years ago in 1927
        //    var

        //    //var yearsAgo = new Years( 13700000000 );
        //    //yearsAgo
        //}

        public UniversalDateTime( BigInteger planckTimesSinceBigBang ) {
            this.Value = planckTimesSinceBigBang;

            var span = new Span( planckTimesSinceBigBang );

            //TODO we need a 1927 offset added in here.
            this.Date = new Date( span ); //we can use span here because the values have been normalized.
            this.Time = new Time( span ); //we can use span here because the values have been normalized.
        }

        public UniversalDateTime( DateTime dateTime ) {
            //TODO we need a 1927 offset added in here.

            DateTime? ssadaff;
            //if ( BigBangProposedAtDate.TryConvertToDateTime( out ssadaff ) ) {}
            //UniversalDateTimeIn1927
            //var timePassedSince1927 = 

            //TODO
            var timeSpan = Date.Now - BigBangProposedAtDate;
            this.Value = new BigInteger();
            this.Date = new Date();
            this.Time = new Time();
        }

        public int CompareTo( UniversalDateTime other ) {
            return this.Value.CompareTo( other.Value );
        }

        public static UniversalDateTime operator -( UniversalDateTime left, UniversalDateTime right ) {
            return Combine( left, -right );
        }

        public static UniversalDateTime operator -( UniversalDateTime universalDateTime ) {
            return new UniversalDateTime( universalDateTime.Value*-1 );
        }

        public static Boolean operator <( UniversalDateTime left, UniversalDateTime right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator >( UniversalDateTime left, UniversalDateTime right ) {
            return left.Value > right.Value;
        }

        private static UniversalDateTime Combine( UniversalDateTime left, BigInteger value ) {
            return new UniversalDateTime( left.Value + value );
        }

        private static UniversalDateTime Combine( UniversalDateTime left, UniversalDateTime right ) {
            return Combine( left, right.Value );
        }
    }
}
