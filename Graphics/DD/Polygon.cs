// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Polygon.cs",
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
// "Librainian/Librainian/Polygon.cs" was last cleaned by Protiguous on 2018/05/15 at 10:42 PM.

namespace Librainian.Graphics.DD {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public struct Polygon : IEnumerable<PointF> {

        public Polygon( PointF[] points ) : this() => this.Points = points;

        public PointF[] Points { get; set; }

        public Int32 Length => this.Points.Length;

        public PointF this[Int32 index] {
            get => this.Points[index];

            set => this.Points[index] = value;
        }

        public static implicit operator PointF[] ( Polygon polygon ) => polygon.Points;

        public static implicit operator Polygon( PointF[] points ) => new Polygon( points );

        public IEnumerator GetEnumerator() => this.Points.GetEnumerator();

        IEnumerator<PointF> IEnumerable<PointF>.GetEnumerator() => ( IEnumerator<PointF> )this.Points.GetEnumerator();
    }
}