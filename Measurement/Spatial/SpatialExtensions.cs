namespace Librainian.Measurement.Spatial {
    using System;
    using System.Windows;
    using AForge.Math;

    public static class SpatialExtensions {

        /// <summary>
        /// When you have an angle in degrees, that you want to convert in the range of 0-360
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Double To360Angle( this Double angle ) {
            while ( angle < 0.0 ) {
                angle += 360.0;
            }
            while ( angle >= 360.0 ) {
                angle -= 360.0;
            }
            return angle;
        }

        /// <summary>
        /// To do the same for a vector of 3 angles
        /// </summary>
        /// <param name="angles"></param>
        /// <returns></returns>
        public static Vector3 To360Angle( this Vector3 angles ) {
            angles.X = ( float ) To360Angle( angles.X );
            angles.Y = ( float ) To360Angle( angles.Y );
            angles.Z = ( float ) To360Angle( angles.Z );
            return angles;
        }

        /// <summary>
        /// Convert angle between -180 and 180 degrees
        /// If you want the angle to be between -180 and 180
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Double To180Angle( this Double angle ) {
            while ( angle < -180.0 )
                angle += 360.0f;
            while ( angle >= 180.0 )
                angle -= 360.0f;
            return angle;
        }

        /// <summary>
        /// And for a Vector with 3 angles
        /// </summary>
        /// <param name="angles"></param>
        /// <returns></returns>
        public static Vector3 To180Angle( Vector3 angles ) {
            angles.X = ( float ) To180Angle( angles.X );
            angles.Y = ( float ) To180Angle( angles.Y );
            angles.Z = ( float ) To180Angle( angles.Z );
            return angles;
        }

        /// <summary>
        /// Compass angles are slightly different from mathematical angles, because they start at the top (north and go clockwise, whereas mathematical angles start at the x-axis (east) and go counter-clockwise.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Double MathAngleToCompassAngle( Double angle ) {
            angle = 90.0f - angle;
            return To360Angle( angle );
        }

        public static T Clamp<T>( this T val, T min, T max ) where T : IComparable<T> {
            return val.CompareTo( min ) < 0 ? min : ( val.CompareTo( max ) > 0 ? max : val );
        }

        public static Double Clamp01( this Double value ) {
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
        public static Double CompassAngleLerp( this Double from, Double to, Double portion ) {
            var dif = To180Angle( to - @from );
            dif *= Clamp01( portion );
            return To360Angle( @from + dif );
        }

        /// <summary>
        /// Clockwise from a top-down view.
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="byAmount"></param>
        /// <returns></returns>
        public static Degrees RotateRight( this Degrees degrees, Single byAmount = 1 ) {
            if ( Single.IsNaN( byAmount ) ) {
                return degrees;
            }
            if ( Single.IsInfinity( byAmount ) ) {
                return degrees;
            }

            return degrees + byAmount;
        }

        /// <summary>
        /// Clockwise from a top-down view.
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="byAmount"></param>
        /// <returns></returns>
        public static Degrees RotateLeft( this Degrees degrees, Single byAmount = 1 ) {
            if ( Single.IsNaN( byAmount ) ) {
                return degrees;
            }
            if ( Single.IsInfinity( byAmount ) ) {
                return degrees;
            }

            return degrees - byAmount;
        }


        public static Degrees TurnLeft( this Degrees degrees, Single angle ) {
            return new Degrees( degrees.Value += DegreesToRadians( angle ) );
        }

        public static Degrees TurnRight( this Degrees degrees, Single angle ) {
            return new Degrees( degrees.Value -= DegreesToRadians( angle ) );
        }

        //public const Single RadiansToDegrees = ( float ) ( 180.0 / Math.PI );
        //public const Single DegreesToRadians = ( float )( Math.PI / 180.0 );

        public static Double FindAngle( this Point here, Point there ) {

            var dx = there.X - here.X;
            var dy = there.Y - here.Y;
            var angle = RadiansToDegrees( Math.Atan2( dy, dx ) );
            if ( angle < 0 ) {
                angle += 360; //This is simular to doing 360 Math.Atan2(y1 - y2, x1 - x2) * (180 / Math.PI)
            }
            return angle;
        }

        public static GeoLocation FindPointAtDistanceFrom( this GeoLocation startPoint, Double initialBearingRadians, Double distanceKilometres ) {
            const double radiusEarthKilometres = 6371.01;
            var distRatio = distanceKilometres / radiusEarthKilometres;
            var distRatioSine = Math.Sin( distRatio );
            var distRatioCosine = Math.Cos( distRatio );

            var startLatRad = DegreesToRadians( startPoint.Latitude );
            var startLonRad = DegreesToRadians( startPoint.Longitude );

            var startLatCos = Math.Cos( startLatRad );
            var startLatSin = Math.Sin( startLatRad );

            var endLatRads = Math.Asin( ( startLatSin * distRatioCosine ) + ( startLatCos * distRatioSine * Math.Cos( initialBearingRadians ) ) );

            var endLonRads = startLonRad + Math.Atan2( Math.Sin( initialBearingRadians ) * distRatioSine * startLatCos, distRatioCosine - startLatSin * Math.Sin( endLatRads ) );

            return new GeoLocation {
                Latitude = RadiansToDegrees( endLatRads ),
                Longitude = RadiansToDegrees( endLonRads )
            };
        }

        //public static Single DegreesToRadians( Single degrees ) {
        //    const Single degToRadFactor = ( float )( Math.PI / 180.0f );
        //    return degrees * degToRadFactor;
        //}

        //public static Single RadiansToDegrees( Single radians ) {
        //    const Single radToDegFactor = ( float )( 180.0f / Math.PI );
        //    return radians * radToDegFactor;
        //}

        public static Double DegreesToRadians( Double degrees ) {
            const double degToRadFactor = Math.PI / 180.0d;
            return degrees * degToRadFactor;
        }

        public static Double RadiansToDegrees( Double radians ) {
            const double radToDegFactor = 180.0d / Math.PI;
            return radians * radToDegFactor;
        }
    }
}