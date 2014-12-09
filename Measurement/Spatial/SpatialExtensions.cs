namespace Librainian.Measurement.Spatial {

    using System;
    using System.Windows;
    using System.Windows.Media.Media3D;
    using AForge.Math;

    public static class SpatialExtensions {

        public const Single TwoPi = ( Single )( Math.PI * 2 );

        public const Single TwoPI = ( Single )( Math.PI * 2 );

        public static T Clamp<T>( this T val, T min, T max ) where T : IComparable<T> => val.CompareTo( min ) < 0 ? min : ( val.CompareTo( max ) > 0 ? max : val );

        public static Double Clamp01( this Double value ) => Clamp( value, 0.0f, 1.0f );

        /// <summary>
        /// Lerp function for compass angles
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="portion"></param>
        /// <remarks>
        /// When you gradually want to steer towards a target heading, you need a Lerp function. But
        /// to slide from 350 degrees to 10 degrees should work like 350, 351, 352, ....359, 0, 1,
        /// 2, 3....10. And not the other way around going 350, 349, 348.....200...1000, 12, 11, 10.
        /// </remarks>
        /// <returns></returns>
        public static Double CompassAngleLerp( this Double from, Double to, Double portion ) {
            var dif = To180Angle( to - @from );
            dif *= Clamp01( portion );
            return To360Angle( @from + dif );
        }

        public static Double DegreesToRadians( Double degrees ) => degrees * Degrees.DegreesToRadiansFactor;

        public static Double FindAngle( this Point here, Point there ) {

            var dx = there.X - here.X;
            var dy = there.Y - here.Y;
            var angle = RadiansToDegrees( Math.Atan2( dy, dx ) );
            if ( angle < 0 ) {
                angle += 360; //This is simular to doing 360 Math.Atan2(y1 - y2, x1 - x2) * (180 / Math.PI)
            }
            return angle;
        }

        //public const Single RadiansToDegrees = ( float ) ( 180.0 / Math.PI );
        //public const Single DegreesToRadians = ( float )( Math.PI / 180.0 );
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

        /// <summary>
        /// Compass angles are slightly different from mathematical angles, because they start at
        /// the top (north and go clockwise, whereas mathematical angles start at the x-axis (east)
        /// and go counter-clockwise.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Double MathAngleToCompassAngle( Double angle ) {
            angle = 90.0f - angle;
            return To360Angle( angle );
        }

        public static Double RadiansToDegrees( Double radians ) => radians * Radians.RadiansToDegreesFactor;

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
        /// Convert angle between -180 and 180 degrees If you want the angle to be between -180 and 180
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
            angles.X = ( float )To180Angle( angles.X );
            angles.Y = ( float )To180Angle( angles.Y );
            angles.Z = ( float )To180Angle( angles.Z );
            return angles;
        }

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

        public static Degrees TurnLeft( this Degrees degrees, Single angle ) => new Degrees( degrees.Value += DegreesToRadians( angle ) );

        public static Degrees TurnRight( this Degrees degrees, Single angle ) => new Degrees( degrees.Value -= DegreesToRadians( angle ) );

/*
        /// <summary>
        /// Calculates the angle that an object should face, given its position, its target's position, its current angle, and its maximum turning speed.
        /// </summary>
        public static float TurnToFace( this Vector3 position, Vector3 faceThis, float currentAngle, float turnSpeed ) {
            return TurnToFace( new Vector( position.X,position.Y), new Vector( faceThis.X, faceThis.Y), currentAngle, turnSpeed );
        }
*/
        
        /// <summary>
        /// Calculates the angle that an object should face, given its position, its target's position, its current angle, and its maximum turning speed.
        /// </summary>
        public static float TurnToFace( this Point3D position, Point3D faceThis, float currentAngle, float turnSpeed ) => TurnToFace( new Vector( position.X,position.Y), new Vector( faceThis.X, faceThis.Y), currentAngle, turnSpeed );

        /// <summary>
        /// Calculates the angle that an object should face, given its position, its target's position, its current angle, and its maximum turning speed.
        /// </summary>
        public static float TurnToFace( this Vector position, Vector faceThis, float currentAngle, float turnSpeed ) {

            // consider this diagram: C /| / | / | y / o | S-------- x where S is the position of
            // the spot light, C is the position of the cat, and "o" is the angle that the spot
            // light should be facing in order to point at the cat. we need to know what o is. using
            // trig, we know that tan(theta) = opposite / adjacent tan(o) = y / x if we take the
            // arctan of both sides of this equation... arctan( tan(o) ) = arctan( y / x ) o =
            // arctan( y / x ) so, we can use x and y to find o, our "desiredAngle." x and y are
            // just the differences in position between the two objects.
            var x = faceThis.X - position.X;
            var y = faceThis.Y - position.Y;

            // we'll use the Atan2 function. Atan will calculates the arc tangent of y / x for us,
            // and has the added benefit that it will use the signs of x and y to determine what
            // cartesian quadrant to put the result in.
            // http: //msdn2.microsoft.com/en-us/library/system.math.atan2.aspx
            var desiredAngle = new Radians( Math.Atan2( y, x ) );

            // so now we know where we WANT to be facing, and where we ARE facing... if we weren't
            // constrained by turnSpeed, this would be easy: we'd just return desiredAngle. instead,
            // we have to calculate how much we WANT to turn, and then make sure that's not more
            // than turnSpeed.

            // first, figure out how much we want to turn, using WrapAngle to get our result from
            // - Pi to Pi ( -180 degrees to 180 degrees )
            //var difference = WrapAngle( desiredAngle - currentAngle );
            var difference = To360Angle( desiredAngle - currentAngle);

            // clamp that between -turnSpeed and turnSpeed.
            difference = Clamp( difference, -turnSpeed, turnSpeed );

            // so, the closest we can get to our target is currentAngle + difference. return that,
            // using WrapAngle again.
            //return WrapAngle( currentAngle + difference );
            return To360Angle( currentAngle + difference );
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// </summary>
        private static float WrapAngle( float radians ) {
            while ( radians < -Math.PI ) {
                radians += TwoPI;
            }
            while ( radians > Math.PI ) {
                radians -= TwoPi;
            }
            return radians;
        }
    }
}