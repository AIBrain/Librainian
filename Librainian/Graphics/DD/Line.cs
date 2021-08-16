// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting.
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Line.cs" last formatted on 2021-02-08 at 1:36 AM.

namespace Librainian.Graphics.DD {

	using System;
	using System.Drawing;

	public record Line {
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>" )]
		public PointF P1;

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>" )]
		public PointF P2;

		public Line( PointF p1, PointF p2 ) {
			this.P1 = p1;
			this.P2 = p2;
		}

		public static Line Empty { get; } = new( PointF.Empty, PointF.Empty );

		public Single X1 {
			get => this.P1.X;

			set => this.P1.X = value;
		}

		public Single X2 {
			get => this.P2.X;

			set => this.P2.X = value;
		}

		public Single Y1 {
			get => this.P1.Y;

			set => this.P1.Y = value;
		}

		public Single Y2 {
			get => this.P2.Y;

			set => this.P2.Y = value;
		}
	}
}