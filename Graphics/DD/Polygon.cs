#region License & Information

// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Polygon.cs" was last cleaned by Rick on 2015/06/12 at 2:55 PM
#endregion License & Information

namespace Librainian.Graphics.DD {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public struct Polygon : IEnumerable<PointF> {
        public Polygon(PointF[] points) : this() {
            this.Points = points;
        }

        public PointF[] Points {
            get; set;
        }

        public Int32 Length => this.Points.Length;

        public PointF this[ Int32 index ] {
            get {
                return this.Points[ index ];
            }
            set {
                this.Points[ index ] = value;
            }
        }

        IEnumerator<PointF> IEnumerable<PointF>.GetEnumerator() => ( IEnumerator<PointF> )this.Points.GetEnumerator();

        public IEnumerator GetEnumerator() => this.Points.GetEnumerator();

        public static implicit operator PointF[] (Polygon polygon) => polygon.Points;

        public static implicit operator Polygon(PointF[] points) => new Polygon( points );
    }
}