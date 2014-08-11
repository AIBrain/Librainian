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
// "Librainian/Votally.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using System.Threading;
    using Annotations;
    using Threading;

    /// <summary>
    ///     <para>Keep track of votes for candidate A and candidate B.</para>
    /// </summary>
    [DataContract( IsReference = true )]
    [Serializable]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public class Votally {
        /// <summary>
        ///     No vote for either.
        /// </summary>
        public static readonly Votally Zero = new Votally( votesForA: 0, votesForB: 0 );

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [DataMember] [OptionalField] private UInt64 _aVotes;

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [DataMember] [OptionalField] private UInt64 _bVotes;

        public Votally( UInt64 votesForA = 0, UInt64 votesForB = 0 ) {
            this.A = votesForA;
            this.B = votesForB;
        }

        public Boolean IsLandslideA { get { return this.IsAWinning && this.A > this.HalfOfVotes(); } }

        public Boolean IsLandslideB { get { return this.IsBWinning && this.B > this.HalfOfVotes(); } }

        public Boolean IsProtiguous { get { return this.IsTied() && this.Votes > UBigInteger.One; } }

        [UsedImplicitly]
        public Boolean IsBWinning { get { return this.B > this.A && this.A > BigInteger.One && this.B > BigInteger.One; } }

        public UInt64 A { get { return Thread.VolatileRead( ref this._aVotes ); } private set { Thread.VolatileWrite( ref this._aVotes, value ); } }

        public UInt64 B { get { return Thread.VolatileRead( ref this._bVotes ); } private set { Thread.VolatileWrite( ref this._bVotes, value ); } }

        public double ChanceA {
            get {
                try {
                    var votes = this.Votes;
                    if ( !votes.Near( 0 ) ) {
                        return this.A/votes;
                    }
                }
                catch ( DivideByZeroException exception ) {
                    exception.Log();
                }
                return 0;
            }
        }

        public double ChanceB {
            get {
                try {
                    var votes = this.Votes;
                    if ( !votes.Near( 0 ) ) {
                        return this.B/votes;
                    }
                }
                catch ( DivideByZeroException exception ) {
                    exception.Log();
                }
                return 0;
            }
        }

        [UsedImplicitly]
        public Boolean IsAWinning { get { return this.A > this.B && this.A > BigInteger.One && this.B > BigInteger.One; } }

        public UBigInteger Votes { get { return new UBigInteger( this.A ) + new UBigInteger( this.B ); } }

        [UsedImplicitly]
        private string DebuggerDisplay { get { return this.ToString(); } }

        public UBigInteger HalfOfVotes() {
            return this.Votes/UBigInteger.Two;
        }

        public Boolean IsTied() {
            return this.A == this.B;
        }

        public override String ToString() {
            return String.Format( "A has {0:f1} and B has {1:f1} of {2} votes.", this.ChanceA, this.ChanceB, this.Votes );
        }

        public static Votally Combine( [NotNull] Votally left, [NotNull] Votally right ) {
            if ( left == null ) {
                throw new ArgumentNullException( "left" );
            }
            if ( right == null ) {
                throw new ArgumentNullException( "right" );
            }
            var result = left;
            result.VoteForA( right.A );
            result.VoteForB( right.B );
            return result;
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="A" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void VoteForA( UInt64 votes = 1 ) {
            this.A.AddWithoutOverFlow( votes );
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="B" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void VoteForB( UInt64 votes = 1 ) {
            this.B.AddWithoutOverFlow( votes );
        }

        public Votally Clone() {
            return new Votally( votesForA: this.A, votesForB: this.B );
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="A" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawVoteForA( UInt64 votes = 1 ) {
            this.A.SubtractWithoutUnderFlow( votes );
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="A" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawVoteForA( long votes = 1 ) {
            if ( votes >= 0 ) {
                this.A.SubtractWithoutUnderFlow( ( UInt64 ) votes );
            }
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="B" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawVoteForB( UInt64 votes = 1 ) {
            this.B.SubtractWithoutUnderFlow( votes );
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="B" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawVoteForB( long votes = 1 ) {
            if ( votes >= 0 ) {
                this.B.SubtractWithoutUnderFlow( ( UInt64 ) votes );
            }
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="A" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void VoteForA( long votes ) {
            if ( votes >= 0 ) {
                this.VoteForA( ( UInt64 ) votes );
            }
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="B" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void VoteForB( long votes ) {
            if ( votes >= 0 ) {
                this.VoteForB( ( UInt64 ) votes );
            }
        }
    }
}
