// Copyright 2016 Protiguous.
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
// "Librainian/Polygon.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics.DD {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public struct Polygon : IEnumerable<PointF> {

        public Polygon( PointF[] points ) : this() => this.Points = points;

	    public Int32 Length => this.Points.Length;

        public PointF[] Points {
            get; set;
        }

        public PointF this[ Int32 index ] {
            get => this.Points[ index ];

	        set => this.Points[ index ] = value;
        }

        public static implicit operator PointF[] ( Polygon polygon ) => polygon.Points;

        public static implicit operator Polygon( PointF[] points ) => new Polygon( points );

        public IEnumerator GetEnumerator() => this.Points.GetEnumerator();

        IEnumerator<PointF> IEnumerable<PointF>.GetEnumerator() => ( IEnumerator<PointF> )this.Points.GetEnumerator();
    }
}