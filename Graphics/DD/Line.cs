// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Line.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Line.cs" was last cleaned by Protiguous on 2018/05/15 at 10:42 PM.

namespace Librainian.Graphics.DD {

    using System;
    using System.Drawing;

    public struct Line {

        public static Line Empty;

        public PointF P1;

        public PointF P2;

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

        public Line( PointF p1, PointF p2 ) : this() {
            this.P1 = p1;
            this.P2 = p2;
        }
    }
}