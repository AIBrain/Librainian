#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Votally.cs" was last cleaned by Rick on 2014/08/12 at 8:20 AM

#endregion License & Information

namespace Librainian.Maths {

    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Threading;
    using Annotations;
    using Numerics;
    using Threading;

    /// <summary>
    /// <para>Keep count of Yes or No votes.</para>
    /// </summary>
    [DataContract( IsReference = true )]
    [Serializable]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public class VotallyI {

        /// <summary>
        /// No vote for either.
        /// </summary>
        public static readonly VotallyI Zero = new VotallyI( votesYes: 0, votesNo: 0 );

        /// <summary>
        /// ONLY used in the getter and setter.
        /// </summary>
        [DataMember]
        [OptionalField]
        private UInt64 _votesNo;

        /// <summary>
        /// ONLY used in the getter and setter.
        /// </summary>
        [DataMember]
        [OptionalField]
        private UInt64 _votesYes;

        public VotallyI( UInt64 votesYes = 0, UInt64 votesNo = 0 ) {
            this.Yes = votesYes;
            this.No = votesNo;
        }

        public Double ChanceNo() {
            try {
                var votes = this.Votes;
                if ( !votes.Near( 0 ) ) {
                    var result = new BigRational( this.No, votes );
                    return ( Double ) result;
                }
            }
            catch ( DivideByZeroException exception ) {
                exception.More();
            }
            return 0;
        }

        public Double ChanceYes() {
            try {
                var votes = this.Votes;
                if ( votes.Near( 0 ) ) {
                    return 0;
                }
                var chance = new BigRational( this.Yes, votes );
                return ( Double ) chance;
            }
            catch ( DivideByZeroException exception ) {
                exception.More();
                return 0;
            }
        }

        public bool IsLandslideNo() => this.IsNoWinning() && this.No > this.HalfOfVotes();

        public bool IsLandslideYes() => this.IsYesWinning() && this.Yes > this.HalfOfVotes();

        public bool IsNoWinning() => this.No > this.Yes && this.Yes > 1 && this.No > 1;

        public bool IsProtiguous() => this.IsTied() && this.Votes >= 2;

        public bool IsYesWinning() => this.Yes > this.No && this.Yes > 1 && this.No > 1;

        public UInt64 No {
            get {
                return Thread.VolatileRead( ref this._votesNo );
            }

            private set {
                Thread.VolatileWrite( ref this._votesNo, value );
            }
        }

        public UInt64 Votes {
            get {
                return this.Yes + this.No;
            }
        }

        public UInt64 Yes {
            get {
                return Thread.VolatileRead( ref this._votesYes );
            }

            private set {
                Thread.VolatileWrite( ref this._votesYes, value );
            }
        }

        [UsedImplicitly]
        private String DebuggerDisplay {
            get {
                return this.ToString();
            }
        }

        public static VotallyI Combine( [NotNull] VotallyI left, [NotNull] VotallyI right ) {
            if ( left == null ) {
                throw new ArgumentNullException( "left" );
            }
            if ( right == null ) {
                throw new ArgumentNullException( "right" );
            }
            var result = left;
            result.VoteYes( right.Yes );
            result.VoteNo( right.No );
            return result;
        }

        public VotallyI Clone() => new VotallyI( votesYes: this.Yes, votesNo: this.No );

        public UInt64 HalfOfVotes() => this.Votes / 2;

        public Boolean IsTied() => this.Yes == this.No;

        public override String ToString() => String.Format( "{0:P1} yes vs {1:p1} no of {2} votes.", this.ChanceYes(), this.ChanceNo(), this.Votes );

        /// <summary>
        /// <para>Increments the votes for candidate <see cref="No" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void VoteNo( UInt64 votes = 1 ) => this.No += votes;

        /// <summary>
        /// <para>Increments the votes for candidate <see cref="Yes" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void VoteYes( UInt64 votes = 1 ) => this.Yes += votes;

        /// <summary>
        /// <para>Increments the votes for candidate <see cref="No" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawNoVote( UInt64 votes = 1 ) => this.No -= votes;

        /// <summary>
        /// <para>Increments the votes for candidate <see cref="Yes" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawYesVote( UInt64 votes = 1 ) => this.Yes -= votes;
    }
}