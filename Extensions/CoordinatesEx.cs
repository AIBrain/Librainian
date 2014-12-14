#region License & Information

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
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// "Librainian/CoordinatesEx.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM

#endregion License & Information

namespace Librainian.Extensions {

    using System;
    using Graphics.DDD;

    public static class CoordinatesEx {

        public static Double DistanceTo( this Coordinate from, Coordinate to ) {
            if ( from == default(Coordinate) ) {
                return 0;
            }
            if ( to == default(Coordinate) ) {
                return 0;
            }
            var dx = @from.X - to.X;
            var dy = @from.Y - to.Y;
            var dz = @from.Z - to.Z;
            return Math.Sqrt( dx * dx + dy * dy + dz * dz );
        }

#if XNA

        public static Double DistanceTo( this Coordinate from, Vector3 to ) {
            if ( from == default(Coordinate) ) {
                return 0;
            }
            if ( to == default( Vector3 ) ) {
                return 0;
            }
            var dx = from.x - to.X;
            var dy = @from.Y - to.Y;
            var dz = @from.Z - to.Z;
            return Math.Sqrt( dx * dx + dy * dy + dz * dz );
        }

        public static Double DistanceTo( this Vector3 from, Coordinate to ) {
            if ( from == default( Vector3 ) ) {
                return 0;
            }
            if ( to == default( Coordinate) ) {
                return 0;
            }
            var dx = from.X - to.x;
            var dy = from.Y - to.Y;
            var dz = from.Z - to.Z;
            return Math.Sqrt( dx * dx + dy * dy + dz * dz );
        }

        public static Double DistanceTo( this Vector3 from, Vector3 to ) {
            if ( from == default( Vector3 ) ) {
                return 0;
            }
            if ( to == default( Vector3 ) ) {
                return 0;
            }
            var dx = from.X - to.X;
            var dy = from.Y - to.Y;
            var dz = from.Z - to.Z;
            return Math.Sqrt( dx * dx + dy * dy + dz * dz );
        }
#endif
    }
}