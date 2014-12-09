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
// "Librainian/Polygon.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Graphics.DD {
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public struct Polygon : IEnumerable< PointF > {
        public Polygon( PointF[] points ) : this() {
            this.Points = points;
        }

        public PointF[] Points { get; set; }

        public int Length { get { return this.Points.Length; } }

        public PointF this[ int index ] { get { return this.Points[ index ]; } set { this.Points[ index ] = value; } }

        IEnumerator< PointF > IEnumerable< PointF >.GetEnumerator() => ( IEnumerator< PointF > ) this.Points.GetEnumerator();

        public IEnumerator GetEnumerator() => this.Points.GetEnumerator();

        public static implicit operator PointF[]( Polygon polygon ) => polygon.Points;

        public static implicit operator Polygon( PointF[] points ) => new Polygon( points );
    }
}
