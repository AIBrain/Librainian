// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "SpatialExtensions.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "SpatialExtensions.cs" was last formatted by Protiguous on 2018/06/04 at 4:11 PM.

namespace Librainian.Measurement.Spatial {

	using System;
	using System.Numerics;
	using System.Windows;
	using System.Windows.Media.Media3D;
	using JetBrains.Annotations;
	using Vector = System.Windows.Vector;

	public static class SpatialExtensions {

		/// <summary>Returns the angle expressed in radians between -Pi and Pi.</summary>
		[UsedImplicitly]
		private static Single WrapAngle( Single radians ) {
			while ( radians < -Math.PI ) { radians += TwoPi; }

			while ( radians > Math.PI ) { radians -= TwoPi; }

			return radians;
		}

		public static T Clamp<T>( [NotNull] this T val, T min, T max ) where T : IComparable<T> => val.CompareTo( min ) < 0 ? min : ( val.CompareTo( max ) > 0 ? max : val );

		public static Single Clamp01( this Single value ) => Clamp( value, 0.0f, 1.0f );

		/// <summary>Lerp function for compass angles</summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="portion"></param>
		/// <remarks>
		///     When you gradually want to steer towards a target heading, you need a Lerp function. But
		///     to slide from 350 degrees to 10 degrees should work like 350, 351, 352, ....359, 0, 1,
		///     2, 3....10. And not the other way around going 350, 349, 348.....200...1000, 12, 11, 10.
		/// </remarks>
		/// <returns></returns>
		public static Single CompassAngleLerp( this Single from, Single to, Single portion ) {
			var dif = To180Angle( to - from );
			dif *= Clamp01( portion );

			return To360Angle( from + dif );
		}

		public static Single DegreesToRadians( Single degrees ) => degrees * Degrees.DegreesToRadiansFactor;

		public static Double FindAngle( this Point here, Point there ) {
			var dx = there.X - here.X;
			var dy = there.Y - here.Y;
			var angle = RadiansToDegrees( Math.Atan2( dy, dx ) );

			if ( angle < 0 ) {
				angle += 360; //This is simular to doing 360 Math.Atan2(y1 - y2, x1 - x2) * (180 / Math.PI)
			}

			return angle;
		}

		public static GeoLocation FindPointAtDistanceFrom( this GeoLocationF startPoint, Double initialBearingRadians, Double distanceKilometres ) {
			const Double radiusEarthKilometres = 6371.01;
			var distRatio = distanceKilometres / radiusEarthKilometres;
			var distRatioSine = Math.Sin( distRatio );
			var distRatioCosine = Math.Cos( distRatio );

			var startLatRad = DegreesToRadians( startPoint.Latitude );
			var startLonRad = DegreesToRadians( startPoint.Longitude );

			var startLatCos = Math.Cos( startLatRad );
			var startLatSin = Math.Sin( startLatRad );

			var endLatRads = Math.Asin( startLatSin * distRatioCosine + startLatCos * distRatioSine * Math.Cos( initialBearingRadians ) );

			var endLonRads = startLonRad + Math.Atan2( Math.Sin( initialBearingRadians ) * distRatioSine * startLatCos, distRatioCosine - startLatSin * Math.Sin( endLatRads ) );

			return new GeoLocation { Latitude = RadiansToDegrees( endLatRads ), Longitude = RadiansToDegrees( endLonRads ) };
		}

		/// <summary>
		///     Compass angles are slightly different from mathematical angles, because they start at
		///     the top (north and go clockwise, whereas mathematical angles start at the x-axis (east)
		///     and go counter-clockwise.
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static Double MathAngleToCompassAngle( Double angle ) {
			angle = 90.0f - angle;

			return To360Angle( angle );
		}

		public static Double RadiansToDegrees( Double radians ) => radians * Radians.RadiansToDegreesFactor;

		/// <summary>Clockwise from a top-down view.</summary>
		/// <param name="degrees"></param>
		/// <param name="byAmount"></param>
		/// <returns></returns>
		public static Degrees RotateLeft( this Degrees degrees, Single byAmount = 1 ) {
			if ( Single.IsNaN( byAmount ) ) { return degrees; }

			if ( Single.IsInfinity( byAmount ) ) { return degrees; }

			return degrees - byAmount;
		}

		/// <summary>Clockwise from a top-down view.</summary>
		/// <param name="degrees"></param>
		/// <param name="byAmount"></param>
		/// <returns></returns>
		public static Degrees RotateRight( this Degrees degrees, Single byAmount = 1 ) {
			if ( Single.IsNaN( byAmount ) ) { return degrees; }

			if ( Single.IsInfinity( byAmount ) ) { return degrees; }

			return degrees + byAmount;
		}

		/// <summary>
		///     Convert angle between -180 and 180 degrees If you want the angle to be between -180 and 180
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static Single To180Angle( this Single angle ) {
			while ( angle < -180.0f ) { angle += 360.0f; }

			while ( angle >= 180.0f ) { angle -= 360.0f; }

			return angle;
		}

		/// <summary>And for a Vector with 3 angles</summary>
		/// <param name="angles"></param>
		/// <returns></returns>
		public static Vector3 To180Angle( Vector3 angles ) {
			angles.X = To180Angle( angles.X );
			angles.Y = To180Angle( angles.Y );
			angles.Z = To180Angle( angles.Z );

			return angles;
		}

		/// <summary>
		///     When you have an angle in degrees, that you want to convert in the range of 0-360
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static Double To360Angle( this Double angle ) {
			while ( angle < 0.0 ) { angle += 360.0; }

			while ( angle >= 360.0 ) { angle -= 360.0; }

			return angle;
		}

		/// <summary>
		///     When you have an angle in degrees, that you want to convert in the range of 0-360
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static Single To360Angle( this Single angle ) {
			while ( angle < 0.0f ) { angle += 360.0f; }

			while ( angle >= 360.0f ) { angle -= 360.0f; }

			return angle;
		}

		/// <summary>To do the same for a vector of 3 angles</summary>
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
                /// Calculates the angle that an object should face, given its position, its
                /// target's position, its current angle, and its maximum turning speed.
                /// </summary>
                public static float TurnToFace( this Vector3 position, Vector3 faceThis, float currentAngle, float turnSpeed ) {
                    return TurnToFace( new Vector( position.X,position.Y), new Vector( faceThis.X, faceThis.Y), currentAngle, turnSpeed );
                }
        */

		/// <summary>
		///     Calculates the angle that an object should face, given its position, its target's
		///     position, its current angle, and its maximum turning speed.
		/// </summary>
		public static Single TurnToFace( this Point3D position, Point3D faceThis, Single currentAngle, Single turnSpeed ) =>
			TurnToFace( new Vector( position.X, position.Y ), new Vector( faceThis.X, faceThis.Y ), currentAngle, turnSpeed );

		/// <summary>
		///     Calculates the angle that an object should face, given its position, its target's
		///     position, its current angle, and its maximum turning speed.
		/// </summary>
		public static Single TurnToFace( this Vector position, Vector faceThis, Single currentAngle, Single turnSpeed ) {

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
			// http: //msdn2.microsoft.com/en-us/Library/system.math.atan2.aspx
			var desiredAngle = new Radians( ( Single ) Math.Atan2( y, x ) );

			// so now we know where we WANT to be facing, and where we ARE facing... if we weren't
			// constrained by turnSpeed, this would be easy: we'd just return desiredAngle. instead,
			// we have to calculate how much we WANT to turn, and then make sure that's not more
			// than turnSpeed.

			// first, figure out how much we want to turn, using WrapAngle to get our result from
			// - Pi to Pi ( -180 degrees to 180 degrees )
			//var difference = WrapAngle( desiredAngle - currentAngle );
			var difference = To360Angle( desiredAngle - currentAngle );

			// clamp that between -turnSpeed and turnSpeed.
			difference = Clamp( difference, -turnSpeed, turnSpeed );

			// so, the closest we can get to our target is currentAngle + difference. return that,
			// using WrapAngle again.
			//return WrapAngle( currentAngle + difference );
			return To360Angle( currentAngle + difference );
		}

		public const Single TwoPi = ( Single ) ( Math.PI * 2 );

	}

}