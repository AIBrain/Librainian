// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Line.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics.DD {

    using System;
    using System.Drawing;

    public struct Line {
        public static Line Empty;
        public PointF P1;
        public PointF P2;

        public Line( PointF p1, PointF p2 ) : this() {
            this.P1 = p1;
            this.P2 = p2;
        }

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