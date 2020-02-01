// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "VotallyI.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "VotallyI.cs" was last formatted by Protiguous on 2020/01/31 at 12:26 AM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using JetBrains.Annotations;
    using Logging;
    using Newtonsoft.Json;
    using Rationals;

    /// <summary>
    ///     <para>threadsafe, keep integer count of Yes or No votes.</para>
    /// </summary>
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class VotallyI {

        /// <summary>ONLY used in the getter and setter.</summary>
        private UInt64 _votesNo;

        /// <summary>ONLY used in the getter and setter.</summary>
        private UInt64 _votesYes;

        public UInt64 No {
            get => Thread.VolatileRead( ref this._votesNo );

            private set => Thread.VolatileWrite( ref this._votesNo, value );
        }

        public UInt64 Votes => this.Yes + this.No;

        public UInt64 Yes {
            get => Thread.VolatileRead( ref this._votesYes );

            private set => Thread.VolatileWrite( ref this._votesYes, value );
        }

        /// <summary>No vote for either.</summary>
        public static readonly VotallyI Zero = new VotallyI( votesYes: 0, votesNo: 0 );

        public VotallyI( UInt64 votesYes = 0, UInt64 votesNo = 0 ) {
            this.Yes = votesYes;
            this.No = votesNo;
        }

        [NotNull]
        public static VotallyI Combine( [NotNull] VotallyI left, [NotNull] VotallyI right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            var result = left;
            result.VoteYes( right.Yes );
            result.VoteNo( right.No );

            return result;
        }

        /// <summary>Add in the votes from another <see cref="VotallyI" />.</summary>
        /// <param name="right"></param>
        public void Add( [NotNull] VotallyI right ) {
            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            this.VoteYes( right.Yes );
            this.VoteNo( right.No );
        }

        public Double ChanceNo() {
            try {
                var votes = this.Votes;

                if ( !votes.Near( 0 ) ) {
                    var result = new Rational( this.No, votes );

                    return ( Double ) result;
                }
            }
            catch ( DivideByZeroException exception ) {
                exception.Log();
            }

            return 0;
        }

        public Double ChanceYes() {
            try {
                var votes = this.Votes;

                if ( votes.Near( 0 ) ) {
                    return 0;
                }

                var chance = new Rational( this.Yes, votes );

                return ( Double ) chance;
            }
            catch ( DivideByZeroException exception ) {
                exception.Log();

                return 0;
            }
        }

        [NotNull]
        public VotallyI Clone() => new VotallyI( votesYes: this.Yes, votesNo: this.No );

        public UInt64 HalfOfVotes() => this.Votes / 2;

        public Boolean IsLandslideNo() => this.IsNoWinning() && this.No > this.HalfOfVotes();

        public Boolean IsLandslideYes() => this.IsYesWinning() && this.Yes > this.HalfOfVotes();

        public Boolean IsNoWinning() => this.No > this.Yes && this.Yes > 1 && this.No > 1;

        public Boolean IsProtiguous() => this.IsTied() && this.Votes >= 2;

        public Boolean IsTied() => this.Yes == this.No;

        public Boolean IsYesWinning() => this.Yes > this.No && this.Yes > 1 && this.No > 1;

        public override String ToString() => $"{this.ChanceYes():P1} yes vs {this.ChanceNo():p1} no of {this.Votes} votes.";

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="No" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void VoteNo( UInt64 votes = 1 ) => this.No += votes;

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="Yes" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void VoteYes( UInt64 votes = 1 ) => this.Yes += votes;

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="No" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawNoVote( UInt64 votes = 1 ) => this.No -= votes;

        /// <summary>
        ///     <para>Increments the votes for candidate <see cref="Yes" /> by <paramref name="votes" />.</para>
        /// </summary>
        public void WithdrawYesVote( UInt64 votes = 1 ) => this.Yes -= votes;

    }

}