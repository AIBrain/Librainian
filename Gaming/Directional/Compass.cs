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
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Compass.cs" was last cleaned by Rick on 2014/09/29 at 12:30 PM
#endregion

namespace Librainian.Gaming.Directional {
    using System;
    using System.Runtime.Serialization;
    using AForge.Math;
    

    public static class CompassExtensions {

        /// <summary>
        /// When you have an angle in degrees, that you want to convert in the range of 0-360
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Single To360Angle( this Single angle ) {
            while ( angle < 0.0f ) {
                angle += 360.0f;
            }
            while ( angle >= 360.0f ) {
                angle -= 360.0f;
            }
            return angle;
        }

        /// <summary>
        /// To do the same for a vector of 3 angles
        /// </summary>
        /// <param name="angles"></param>
        /// <returns></returns>
        public static Vector3 To360Angle( this Vector3 angles ) {
            angles.X = To360Angle( angles.X );
            angles.Y = To360Angle( angles.Y );
            angles.Z = To360Angle( angles.Z );
            return angles;
        }

        /// <summary>
        /// Convert angle between -180 and 180 degrees
        /// If you want the angle to be between -180 and 180
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Single To180Angle( this Single angle ) {
            while ( angle < -180.0f )
                angle += 360.0f;
            while ( angle >= 180.0f )
                angle -= 360.0f;
            return angle;
        }

        /// <summary>
        /// And for a Vector with 3 angles
        /// </summary>
        /// <param name="angles"></param>
        /// <returns></returns>
        public static Vector3 To180Angle( Vector3 angles ) {
            angles.X = To180Angle( angles.X );
            angles.Y = To180Angle( angles.Y );
            angles.Z = To180Angle( angles.Z );
            return angles;
        }

        /// <summary>
        /// Compass angles are slightly different from mathematical angles, because they start at the top (north and go clockwise, whereas mathematical angles start at the x-axis (east) and go counter-clockwise.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Single MathAngleToCompassAngle( Single angle ) {
            angle = 90.0f - angle;
            return To360Angle( angle );
        }

        public static T Clamp<T>( this T val, T min, T max ) where T : IComparable<T> {
            return val.CompareTo( min ) < 0 ? min : ( val.CompareTo( max ) > 0 ? max : val );
        }

        public static Single Clamp01( this Single value  ) {
            return Clamp( value, 0.0f, 1.0f );
        }

        /// <summary>
        /// Lerp function for compass angles
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="portion"></param>
        /// <remarks>When you gradually want to steer towards a target heading, you need a Lerp function.
        /// But to slide from 350 degrees to 10 degrees should work like 350, 351, 352, ....359, 0, 1, 2, 3....10.
        /// And not the other way around going 350, 349, 348.....200...1000, 12, 11, 10.
        /// </remarks>
        /// <returns></returns>
        public static Single CompassAngleLerp( this Single from, Single to, Single portion ) {
            var dif = To180Angle( to - from );
            dif *= Clamp01( portion );
            return To360Angle( from + dif );
        }
    }

    [ DataContract( IsReference = true ) ]
    public class Compass {
        public const Single Minimum = Single.Epsilon;
        public const Single Maximum = 360.0f - Single.Epsilon;

        public Single Value { get; set; }

        /// <summary>
        /// Clockwise from a top-down view.
        /// </summary>
        /// <param name="byAmount"></param>
        /// <returns></returns>
        public Boolean RotateRight( Single byAmount = 1 ) {
            if ( Single.IsNaN( byAmount ) ) {
                return false;
            }
            if ( Single.IsInfinity( byAmount ) ) {
                return false;
            }
            if ( byAmount < Minimum ) {
                byAmount = Minimum;
            }
            else if ( byAmount > Maximum ) {
                byAmount = Maximum;
            }
        }


        public Boolean RotateLeft( Single byAmount = 1 ) {
            if ( Single.IsNaN( byAmount ) ) {
                return false;
            }
            if ( Single.IsInfinity( byAmount ) ) {
                return false;
            }
            if ( byAmount < Minimum ) {
                byAmount = Minimum;
            }
            
            this.Value -= byAmount;

            this.Value = this.Value.To360Angle();

            
            //TODO
            if ( this.Value < CardinalDirections.North ) { }

            return false;
        }
    }
}
