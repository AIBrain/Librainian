// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "MathUtils.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "MathUtils.cs" was last formatted by Protiguous on 2020/01/31 at 12:29 AM.

namespace Librainian.Graphics {

    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using JetBrains.Annotations;
    using Maths;

    public static class MathUtils {

        public static readonly Vector3D XAxis = new Vector3D( 1, 0, 0 );

        //---------------------------------------------------------------------------
        //
        // (c) Copyright Microsoft Corporation. This source is subject to the Microsoft Limited Permissive
        // License. See
        // http://www.microsoft.com/resources/sharedsource/licensingbasics/limitedpermissivelicense.mspx All
        // other rights reserved.
        //
        // This file is part of the 3D Tools for Windows Presentation Foundation project. For more
        // information, see:
        //
        // http: //CodePlex.com/Wiki/View.aspx?ProjectName=3DTools
        //
        //---------------------------------------------------------------------------
        public static readonly Vector3D YAxis = new Vector3D( 0, 1, 0 );

        public static readonly Vector3D ZAxis = new Vector3D( 0, 0, 1 );

        public static readonly Matrix3D ZeroMatrix = new Matrix3D( 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );

        private static Matrix3D GetHomogeneousToViewportTransform( Rect viewport ) {
            var scaleX = viewport.Width / 2;
            var scaleY = viewport.Height / 2;
            var offsetX = viewport.X + scaleX;
            var offsetY = viewport.Y + scaleY;

            return new Matrix3D( scaleX, 0, 0, 0, 0, -scaleY, 0, 0, 0, 0, 1, 0, offsetX, offsetY, 0, 1 );
        }

        private static Matrix3D GetProjectionMatrix( [NotNull] OrthographicCamera camera, Double aspectRatio ) {
            Debug.Assert( camera != null, "Caller needs to ensure camera is non-null." );

            // This math is identical to what you find documented for D3DXMatrixOrthoRH with the
            // exception that in WPF only the camera's width is specified. Height is calculated from
            // width and the aspect ratio.

            var w = camera.Width;
            var h = w / aspectRatio;
            var zn = camera.NearPlaneDistance;
            var zf = camera.FarPlaneDistance;

            var m33 = 1 / ( zn - zf );
            var m43 = zn * m33;

            return new Matrix3D( 2 / w, 0, 0, 0, 0, 2 / h, 0, 0, 0, 0, m33, 0, 0, 0, m43, 1 );
        }

        private static Matrix3D GetProjectionMatrix( [NotNull] PerspectiveCamera camera, Double aspectRatio ) {
            Debug.Assert( camera != null, "Caller needs to ensure camera is non-null." );

            // This math is identical to what you find documented for D3DXMatrixPerspectiveFovRH
            // with the exception that in WPF the camera's horizontal rather the vertical
            // field-of-view is specified.

            var hFoV = DegreesToRadians( camera.FieldOfView );
            var zn = camera.NearPlaneDistance;
            var zf = camera.FarPlaneDistance;

            var xScale = 1 / Math.Tan( hFoV / 2 );
            var yScale = aspectRatio * xScale;
            var m33 = Double.IsPositiveInfinity( zf ) ? -1 : zf / ( zn - zf );
            var m43 = zn * m33;

            return new Matrix3D( xScale, 0, 0, 0, 0, yScale, 0, 0, 0, 0, m33, -1, 0, 0, m43, 0 );
        }

        private static Matrix3D GetViewMatrix( [NotNull] ProjectionCamera camera ) {
            Debug.Assert( camera != null, "Caller needs to ensure camera is non-null." );

            // This math is identical to what you find documented for D3DXMatrixLookAtRH with the
            // exception that WPF uses a LookDirection vector rather than a LookAt point.

            var zAxis = -camera.LookDirection;
            zAxis.Normalize();

            var xAxis = Vector3D.CrossProduct( camera.UpDirection, zAxis );
            xAxis.Normalize();

            var yAxis = Vector3D.CrossProduct( zAxis, xAxis );

            var position = ( Vector3D )camera.Position;
            var offsetX = -Vector3D.DotProduct( xAxis, position );
            var offsetY = -Vector3D.DotProduct( yAxis, position );
            var offsetZ = -Vector3D.DotProduct( zAxis, position );

            return new Matrix3D( xAxis.X, yAxis.X, zAxis.X, 0, xAxis.Y, yAxis.Y, zAxis.Y, 0, xAxis.Z, yAxis.Z, zAxis.Z, 0, offsetX, offsetY, offsetZ, 1 );
        }

        /// <summary>Gets the object space to world space transformation for the given DependencyObject</summary>
        /// <param name="visual">The visual whose world space transform should be found</param>
        /// <param name="viewport">The Viewport3DVisual the Visual is contained within</param>
        /// <returns>The world space transformation</returns>
        private static Matrix3D GetWorldTransformationMatrix( DependencyObject visual, [CanBeNull] out Viewport3DVisual viewport ) {
            var worldTransform = Matrix3D.Identity;
            viewport = null;

            if ( !( visual is Visual3D ) ) {
                throw new ArgumentException( "Must be of type Visual3D.", nameof( visual ) );
            }

            while ( visual != null ) {
                if ( !( visual is ModelVisual3D ) ) {
                    break;
                }

                var transform = ( Transform3D )visual.GetValue( ModelVisual3D.TransformProperty );

                if ( transform != null ) {
                    worldTransform.Append( transform.Value );
                }

                visual = VisualTreeHelper.GetParent( visual );
            }

            viewport = visual as Viewport3DVisual;

            if ( viewport is null ) {
                if ( visual != null ) {

                    // In WPF 3D v1 the only possible configuration is a chain of ModelVisual3Ds
                    // leading up to a Viewport3DVisual.

                    throw new ApplicationException( $"Unsupported type: '{visual.GetType().FullName}'.  Expected tree of ModelVisual3Ds leading up to a Viewport3DVisual." );
                }

                return ZeroMatrix;
            }

            return worldTransform;
        }

        public static Double AngleBetweenVectors( Vector3D a, Vector3D b ) => TranslateRadianToAngle( RadiansBetweenVectors( a, b ) );

        //===========================================================================================================
        public static Point3D Convert2DPoint( Point pointToConvert, [CanBeNull] Visual3D sphere, [CanBeNull] TranslateTransform3D cameraPosition ) // transform world matrix
        {
            var screenTransform = TryTransformTo2DAncestor( sphere, out var viewport, out var success );

            var pointInWorld = new Point3D();

            if ( screenTransform.HasInverse ) {

                //Matrix3D reverseTransform = screenTransform;
                //reverseTransform.Invert();

                screenTransform.Invert();

                var pointOnScreen = new Point3D( pointToConvert.X, pointToConvert.Y, 1 );

                // pointInWorld = reverseTransform.Transform(pointOnScreen);
                pointInWorld = screenTransform.Transform( pointOnScreen );

                //pointInWorld = new Point3D(((pointInWorld.X + cameraPosition.OffsetX) / 4),
                //                            ((pointInWorld.Y + cameraPosition.OffsetY) / 4),
                //                            ((pointInWorld.Z + cameraPosition.OffsetZ) / 4));
            }

            return pointInWorld;
        }

        //FIXME: Should be replaced with method below
        public static Point Convert3DPoint( Point3D p3D, [NotNull] Viewport3D vp ) {
            var vp3Dv = VisualTreeHelper.GetParent( vp.Children[ 0 ] ) as Viewport3DVisual;
            var m = TryWorldToViewportTransform( vp3Dv, out var transformationResultOk );

            if ( !transformationResultOk ) {
                return new Point( 0, 0 );
            }

            var pb = m.Transform( p3D );
            var p2D = new Point( pb.X, pb.Y );

            return p2D;
        }

        public static Point Convert3DPoint( Point3D p3D, [NotNull] DependencyObject dependencyObject ) {
            var vp3Dv = VisualTreeHelper.GetParent( dependencyObject ) as Viewport3DVisual;
            var m = TryWorldToViewportTransform( vp3Dv, out var transformationResultOk );

            if ( !transformationResultOk ) {
                return new Point( 0, 0 );
            }

            var pb = m.Transform( p3D );
            var p2D = new Point( pb.X, pb.Y );

            return p2D;
        }

        public static Double DegreesToRadians( Double degrees ) => degrees * ( Math.PI / 180.0 );

        public static Double GetAspectRatio( Size size ) => size.Width / size.Height;

        /// <summary>Computes the center of 'box'</summary>
        /// <param name="box">The Rect3D we want the center of</param>
        /// <returns>The center point</returns>
        public static Point3D GetCenter( Rect3D box ) => new Point3D( box.X + box.SizeX / 2, box.Y + box.SizeY / 2, box.Z + box.SizeZ / 2 );

        public static Point3D GetCirclePoint( Double angle, Double radius, Point3D orientation = new Point3D() ) {
            var x = radius * Math.Cos( TranslateAngleToRadian( angle ) );
            var y = radius * Math.Sin( TranslateAngleToRadian( angle ) );

            // TODO: Try to find the best way to calculate circle point
            if ( orientation.Equals( new Point3D() ) ) {
                orientation = new Point3D( 1, 1, 0 );
            }

            if ( orientation.X.Near( 0 ) ) {
                return new Point3D( x * orientation.X, x * orientation.Y, y * orientation.Z );
            }

            return new Point3D( x * orientation.X, y * orientation.Y, y * orientation.Z );
        }

        [NotNull]
        public static Point3D[] GetCirclePoints( Int32 quantity, Point3D orientation = new Point3D(), Double radius = 70 ) {
            var circlePoints = new Point3D[ quantity ];

            var step = 360.0 / quantity;
            Double angle = 0;

            for ( var i = 0; i < quantity; i++, angle += step ) {
                circlePoints[ i ] = GetCirclePoint( angle, radius, orientation );
            }

            return circlePoints;
        }

        /// <summary>Computes the effective projection matrix for the given camera.</summary>
        public static Matrix3D GetProjectionMatrix( [NotNull] Camera camera, Double aspectRatio ) {
            switch ( camera ) {
                case PerspectiveCamera perspectiveCamera: return GetProjectionMatrix( perspectiveCamera, aspectRatio );
                case OrthographicCamera orthographicCamera: return GetProjectionMatrix( orthographicCamera, aspectRatio );
                case MatrixCamera matrixCamera: return matrixCamera.ProjectionMatrix;
            }

            throw new ArgumentException( $"Unsupported camera type '{camera.GetType().FullName}'.", nameof( camera ) );
        }

        [NotNull]
        public static Point3D[] GetSectorPoints( Int32 resolution, Double startAngle, Double endAngle, Point3D orientation = new Point3D(), Double radius = 70 ) {
            var circlePoints = new Point3D[ resolution + 1 ];

            var step = endAngle / resolution;
            var angle = startAngle;

            for ( var i = 0; i < resolution + 1; i++, angle += step ) {
                circlePoints[ i ] = GetCirclePoint( angle, radius, orientation );
            }

            return circlePoints;
        }

        /// <summary>Computes the effective view matrix for the given camera.</summary>
        public static Matrix3D GetViewMatrix( [NotNull] Camera camera ) {
            switch ( camera ) {

                //case null: throw new ArgumentNullException( nameof( camera ) );
                case ProjectionCamera projectionCamera: return GetViewMatrix( projectionCamera );
                case MatrixCamera matrixCamera: return matrixCamera.ViewMatrix;
            }

            throw new ArgumentException( $"Unsupported camera type '{camera.GetType().FullName}'.", nameof( camera ) );
        }

        public static Point3D MultiplyPoints( Point3D point1, Point3D point2 ) => new Point3D( point1.X * point2.X, point1.Y * point2.Y, point1.Z * point2.Z );

        /// <summary>
        /// Takes a 3D point and returns the corresponding 2D point (X,Y) within the viewport. Requires the 3DUtils project available at
        /// http://www.codeplex.com/Wiki/View.aspx?ProjectName=3DTools
        /// </summary>
        /// <param name="point3D">A point in 3D space</param>
        /// <param name="viewPort">An instance of Viewport3D</param>
        /// <returns>The corresponding 2D point or null if it could not be calculated</returns>
        public static Point Point3DToScreen2D( Point3D point3D, [NotNull] Viewport3D viewPort ) {

            // We need a Viewport3DVisual but we only have a Viewport3D.
            var vpv = VisualTreeHelper.GetParent( viewPort.Children[ 0 ] ) as Viewport3DVisual;

            // Get the world to viewport transform matrix
            var m = TryWorldToViewportTransform( vpv, out var bOk );

            if ( bOk ) {

                // Transform the 3D point to 2D
                var transformedPoint = m.Transform( point3D );

                var screen2DPoint = new Point( transformedPoint.X, transformedPoint.Y );

                return screen2DPoint;
            }

            return new Point();
        }

        public static Double RadiansBetweenVectors( Vector3D a, Vector3D b ) => Math.Acos( VectorMultiplication( a, b ) / ( VectorLength( a ) * VectorLength( b ) ) );

        public static Point3D RotatePoint3D( Double angle, Point3D point, Point3D center = new Point3D() ) {
            var radians = TranslateAngleToRadian( angle );
            var x = center.X + ( point.X - center.X ) * Math.Cos( radians ) + ( center.Y - point.Y ) * Math.Sin( radians );
            var y = center.Y + ( point.X - center.X ) * Math.Sin( radians ) + ( point.Y - center.Y ) * Math.Cos( radians );

            return new Point3D( x, y, point.Z );
        }

        /// <summary>Transforms the axis-aligned bounding box 'bounds' by 'transform'</summary>
        /// <param name="bounds">The AABB to transform</param>
        /// <param name="transform"></param>
        /// <returns>Transformed AABB</returns>
        public static Rect3D TransformBounds( Rect3D bounds, Matrix3D transform ) {
            var x1 = bounds.X;
            var y1 = bounds.Y;
            var z1 = bounds.Z;
            var x2 = bounds.X + bounds.SizeX;
            var y2 = bounds.Y + bounds.SizeY;
            var z2 = bounds.Z + bounds.SizeZ;

            Point3D[] points = {
                new Point3D( x1, y1, z1 ), new Point3D( x1, y1, z2 ), new Point3D( x1, y2, z1 ), new Point3D( x1, y2, z2 ), new Point3D( x2, y1, z1 ),
                new Point3D( x2, y1, z2 ), new Point3D( x2, y2, z1 ), new Point3D( x2, y2, z2 )
            };

            transform.Transform( points );

            // reuse the 1 and 2 variables to stand for smallest and largest
            var p = points[ 0 ];
            x1 = x2 = p.X;
            y1 = y2 = p.Y;
            z1 = z2 = p.Z;

            for ( var i = 1; i < points.Length; i++ ) {
                p = points[ i ];

                x1 = Math.Min( x1, p.X );
                y1 = Math.Min( y1, p.Y );
                z1 = Math.Min( z1, p.Z );
                x2 = Math.Max( x2, p.X );
                y2 = Math.Max( y2, p.Y );
                z2 = Math.Max( z2, p.Z );
            }

            return new Rect3D( x1, y1, z1, x2 - x1, y2 - y1, z2 - z1 );
        }

        public static Double TranslateAngleToRadian( Double angle ) => angle * Math.PI / 180;

        public static Double TranslateRadianToAngle( Double radian ) => radian * ( 180 / Math.PI );

        /// <summary>
        /// Normalizes v if |v| &gt; 0. This normalization is slightly different from Vector3D.Normalize. Here we just divide by the length but Vector3D.Normalize tries to avoid
        /// overflow when finding the length.
        /// </summary>
        /// <param name="v">The vector to normalize</param>
        /// <returns>'true' if v was normalized</returns>
        public static Boolean TryNormalize( ref Vector3D v ) {
            var length = v.Length;

            if ( length.Near( 0 ) ) {
                return default;
            }

            v /= length;

            return true;
        }

        /// <summary>
        /// Computes the transform from the inner space of the given Visual3D to the 2D space of the Viewport3DVisual which contains it. The result will contain the transform of the
        /// given visual. This method can fail if Camera.Transform is non-invertable in which case the camera clip planes will be coincident and nothing will render. In this case success will
        /// be false.
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="viewport"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static Matrix3D TryTransformTo2DAncestor( [CanBeNull] DependencyObject visual, [CanBeNull] out Viewport3DVisual viewport, out Boolean success ) {
            var to2D = GetWorldTransformationMatrix( visual, out viewport );
            to2D.Append( TryWorldToViewportTransform( viewport, out success ) );

            if ( !success ) {
                return ZeroMatrix;
            }

            return to2D;
        }

        /// <summary>
        /// Computes the transform from the inner space of the given Visual3D to the camera coordinate space The result will contain the transform of the given visual. This method
        /// can fail if Camera.Transform is non-invertable in which case the camera clip planes will be coincident and nothing will render. In this case success will be false.
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="viewport"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static Matrix3D TryTransformToCameraSpace( [CanBeNull] DependencyObject visual, [CanBeNull] out Viewport3DVisual viewport, out Boolean success ) {
            var toViewSpace = GetWorldTransformationMatrix( visual, out viewport );
            toViewSpace.Append( TryWorldToCameraTransform( viewport, out success ) );

            if ( !success ) {
                return ZeroMatrix;
            }

            return toViewSpace;
        }

        /// <summary>
        /// Computes the transform from world space to camera space This method can fail if Camera.Transform is non-invertable in which case the camera clip planes will be coincident
        /// and nothing will render. In this case success will be false.
        /// </summary>
        public static Matrix3D TryWorldToCameraTransform( [CanBeNull] Viewport3DVisual visual, out Boolean success ) {
            success = false;

            if ( visual != null ) {
                var result = Matrix3D.Identity;
                var camera = visual.Camera;

                if ( camera is null ) {
                    return ZeroMatrix;
                }

                var viewport = visual.Viewport;

                if ( viewport == Rect.Empty ) {
                    return ZeroMatrix;
                }

                var cameraTransform = camera.Transform;

                if ( cameraTransform != null ) {
                    var m = cameraTransform.Value;

                    if ( !m.HasInverse ) {
                        return ZeroMatrix;
                    }

                    m.Invert();
                    result.Append( m );
                }

                result.Append( GetViewMatrix( camera ) );

                success = true;

                return result;
            }

            return new Matrix3D();
        }

        /// <summary>
        /// Computes the transform from world space to the Viewport3DVisual's inner 2D space. This method can fail if Camera.Transform is non-invertable in which case the camera clip
        /// planes will be coincident and nothing will render. In this case success will be false.
        /// </summary>
        public static Matrix3D TryWorldToViewportTransform( [CanBeNull] Viewport3DVisual visual, out Boolean success ) {
            var result = TryWorldToCameraTransform( visual, out success );

            if ( success ) {
                result.Append( GetProjectionMatrix( visual.Camera, GetAspectRatio( visual.Viewport.Size ) ) );
                result.Append( GetHomogeneousToViewportTransform( visual.Viewport ) );
            }

            return result;
        }

        public static Double VectorLength( Vector3D a ) => Math.Sqrt( a.X * a.X + a.Y * a.Y + a.Z * a.Z );

        public static Double VectorLength( Vector a ) => Math.Sqrt( a.X * a.X + a.Y * a.Y );

        public static Double VectorMultiplication( Vector3D a, Vector3D b ) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        public static Vector VectorOnPlaneXoYrojection( Vector3D a ) => new Vector( a.X, a.Y );

        public static Vector VectorOnPlaneYozProjection( Vector3D a ) => new Vector( a.Y, a.Z );

        public static Vector VectorProjectionOnPlane( Vector3D a, Vector3D plane ) {
            if ( plane.X.Near( 0 ) ) {
                return new Vector( a.Y * plane.Y, a.Z * plane.Z );
            }

            if ( plane.Y.Near( 0 ) ) {
                return new Vector( a.X * plane.X, a.Z * plane.Z );
            }

            if ( plane.Z.Near( 0 ) ) {
                return new Vector( a.X * plane.X, a.Y * plane.Y );
            }

            throw new ArgumentException( "The vector 'plane' doesn't contain at least one coordinate equals 0" );

            //return new Vector();
        }

        public static Double VectorProjectionOnVector( Vector3D a, Vector3D b ) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }
}