// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "VotallyD.cs" last formatted on 2021-11-30 at 7:19 PM by Protiguous.

namespace Librainian.Maths.Numbers;

using System;
using System.Diagnostics;
using System.Threading;
using Exceptions;
using Newtonsoft.Json;

/// <summary>
/// <para>Keep track of votes for candidate A and candidate B.</para>
/// </summary>
[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
public class VotallyD : ICloneable {

	/// <summary>ONLY used in the getter and setter.</summary>
	[JsonProperty]
	private Double _aVotes;

	/// <summary>ONLY used in the getter and setter.</summary>
	[JsonProperty]
	private Double _bVotes;

	/// <summary>No vote for either.</summary>
	public static readonly VotallyD Zero = new();

	public VotallyD( Double votesForA = 0, Double votesForB = 0 ) {
		this.A = votesForA;
		this.B = votesForB;
	}

	public Double A {
		get => Thread.VolatileRead( ref this._aVotes );

		private set => Thread.VolatileWrite( ref this._aVotes, value );
	}

	public Double B {
		get => Thread.VolatileRead( ref this._bVotes );

		private set => Thread.VolatileWrite( ref this._bVotes, value );
	}

	public Double ChanceB {
		get {
			var votes = this.Votes;

			return votes.Near( 0 ) ? 0 : this.B / votes;
		}
	}

	public Boolean IsAWinning => this.A > this.B;

	public Boolean IsBWinning => this.B > this.A;

	public Boolean IsLandslideA => this.IsAWinning && this.A > this.HalfOfVotes();

	public Boolean IsProtiguous => this.IsTied() && this.Votes > 1;

	/// <summary><see cref="A" /> + <see cref="B" /></summary>
	public Double Votes => this.A + this.B;

	public static VotallyD Combine( VotallyD left, VotallyD right ) {
		if ( left is null ) {
			throw new NullException( nameof( left ) );
		}

		if ( right is null ) {
			throw new NullException( nameof( right ) );
		}

		var result = left;
		result.ForA( right.A );
		result.ForB( right.B );

		return result;
	}

	public Double ChanceA() {
		var votes = this.Votes;

		return votes.Near( 0 ) ? 0 : this.A / votes;
	}

	Object ICloneable.Clone() => this.Clone();

	public VotallyD Clone() => new( this.A, this.B );

	/// <summary>
	/// <para>Increments the votes for candidate <see cref="A" /> by <paramref name="votes" />.</para>
	/// </summary>
	public Double ForA( Double votes = 1 ) {
		this.A += votes;

		if ( this.A <= 0 ) {
			this.A = 0;
		}

		return votes;
	}

	/// <summary>
	/// <para>Increments the votes for candidate <see cref="B" /> by <paramref name="votes" />.</para>
	/// </summary>
	public Double ForB( Double votes = 1 ) {
		this.B += votes;

		if ( this.B <= 0 ) {
			this.B = 0;
		}

		return votes;
	}

	public Double HalfOfVotes() => this.Votes / 2;

	public Boolean IsTied() => this.A.Near( this.B );

	public override String ToString() => $"A has {this.ChanceA():P1} and B has {this.ChanceB:P1} of {this.Votes:F1} votes.";

	/// <summary>
	/// <para>Increments the votes for candidate <see cref="A" /> by <paramref name="votes" />.</para>
	/// </summary>
	public void WithdrawVoteForA( Double votes = 1 ) {
		this.A -= votes;

		if ( this.A <= 0 ) {
			this.A = 0;
		}
	}

	/// <summary>
	/// <para>Increments the votes for candidate <see cref="B" /> by <paramref name="votes" />.</para>
	/// </summary>
	public void WithdrawVoteForB( Double votes = 1 ) {
		this.B -= votes;

		if ( this.B <= 0 ) {
			this.B = 0;
		}
	}
}