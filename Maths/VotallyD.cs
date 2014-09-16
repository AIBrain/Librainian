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
// "Librainian/VotallyD.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM

#endregion License & Information

namespace Librainian.Maths {

    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Threading;
    using Annotations;

    /// <summary>
    ///     <para>Keep track of votes for candidate A and candidate B.</para>
    /// </summary>
    [DataContract( IsReference = true )]
    [Serializable]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public class VotallyD {

        /// <summary>
        ///     No vote for either.
        /// </summary>
        public static readonly VotallyD Zero = new VotallyD( votesForA: 0, votesForB: 0 );

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [DataMember]
        [OptionalField]
        private Double _aVotes;

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [DataMember]
        [OptionalField]
        private Double _bVotes;

        public VotallyD( Double votesForA = 0, Double votesForB = 0 ) {
            this.A = votesForA;
            this.B = votesForB;
        }

        public Double A {
            get {
                return Thread.VolatileRead( ref this._aVotes );
            }

            private set {
                Thread.VolatileWrite( ref this._aVotes, value );
            }
        }

        public Double B {
            get {
                return Thread.VolatileRead( ref this._bVotes );
            }

            private set {
                Thread.VolatileWrite( ref this._bVotes, value );
            }
        }

        public Double ChanceA() {
            try {
                var votes = this.Votes;
                if ( !votes.Near( 0 ) ) {
                    return this.A / votes;
                }
            }
            catch ( DivideByZeroException exception ) {
                exception.Error();
            }
            return 0;
        }

        public Double ChanceB {
            get {
                try {
                    var votes = this.Votes;
                    if ( !votes.Near( 0 ) ) {
                        return this.B / votes;
                    }
                }
                catch ( DivideByZeroException exception ) {
                    exception.Error();
                }
                return 0;
            }
        }

        public Boolean IsAWinning {
            get {
                return this.A > this.B;
            }
        }

        public Boolean IsBWinning {
            get {
                return this.B > this.A;
            }
        }

        public Boolean IsLandslideA {
            get {
                return this.IsAWinning && this.A > this.HalfOfVotes();
            }
        }

        public Boolean IsProtiguous {
            get {
                return this.IsTied() && this.Votes > 1;
            }
        }

        public Double Votes {
            get {
                return this.A + this.B;
            }
        }

        [UsedImplicitly]
        private String DebuggerDisplay {
            get {
                return this.ToString();
            }
        }

        public static VotallyD Combine( [NotNull] VotallyD left, [NotNull] VotallyD right ) {
            if ( left == null ) {
                throw new ArgumentNullException( "left" );
            }
            if ( right == null ) {
                throw new ArgumentNullException( "right" );
            }
            var result = left;
            result.ForA( right.A );
            result.ForB( right.B );
            return result;
        }

        public VotallyD Clone() {
            return new VotallyD( votesForA: this.A, votesForB: this.B );
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="A" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void ForA( Double votes = 1 ) {
            this.A += votes;
            if ( this.A <= 0 ) {
                this.A = 0;
            }
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="B" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void ForB( Double votes = 1 ) {
            this.B += votes;
            if ( this.B <= 0 ) {
                this.B = 0;
            }
        }

        public Double HalfOfVotes() {
            return this.Votes / 2;
        }

        public Boolean IsTied() {
            return this.A.Near( this.B );
        }

        public override String ToString() {
            return String.Format( "A has {0:P1} and B has {1:P1} of {2:F1} votes.", this.ChanceA(), this.ChanceB, this.Votes );
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="A" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawVoteForA( Double votes = 1 ) {
            this.A -= votes;
            if ( this.A <= 0 ) {
                this.A = 0;
            }
        }

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="B" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawVoteForB( Double votes = 1 ) {
            this.B -= votes;
            if ( this.B <= 0 ) {
                this.B = 0;
            }
        }
    }
}