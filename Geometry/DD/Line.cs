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
// "Librainian2/Line.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Geometry.DD {
    using System;
    using System.Drawing;

    [Obsolete]
    public struct Line {
        public static Line Empty;

        public PointF P1;

        public PointF P2;

        public Line( PointF p1, PointF p2 ) : this() {
            this.P1 = p1;
            this.P2 = p2;
        }

        public Single X1 { get { return this.P1.X; } set { this.P1.X = value; } }

        public Single X2 { get { return this.P2.X; } set { this.P2.X = value; } }

        public Single Y1 { get { return this.P1.Y; } set { this.P1.Y = value; } }

        public Single Y2 { get { return this.P2.Y; } set { this.P2.Y = value; } }
    }
}
