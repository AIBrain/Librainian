// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "GraphicsExtensions.cs",
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
// "Librainian/Librainian/GraphicsExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:42 PM.

namespace Librainian.Graphics {

    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using ComputerSystems.FileSystem;
    using Extensions;
    using Imaging;
    using Moving;
    using OperatingSystem;

    public static class GraphicsExtensions {

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
        public struct RAMP {

            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
            public UInt16[] Red;

            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
            public UInt16[] Green;

            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
            public UInt16[] Blue;
        }

        private static Boolean _gotGamma;

        public static RAMP DefaultRamp;

        static GraphicsExtensions() => DefaultRamp = GetGamma();

        public static Stream EfvToStream() {
            var ms = new MemoryStream();

            //TODO
            return ms;
        }

        public static RAMP GetGamma() {
            var ramp = new RAMP();

            if ( NativeMethods.GetDeviceGammaRamp( NativeMethods.GetDC( IntPtr.Zero ), ref ramp ) ) {
                _gotGamma = true;

                return ramp;
            }

            throw new InvalidOperationException( "Unable to obtain Gamma setting." );
        }

        /// <summary>
        ///     Resets the gamma to when it was first called.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static void ResetGamma() {
            if ( _gotGamma ) { NativeMethods.SetDeviceGammaRamp( NativeMethods.GetDC( IntPtr.Zero ), ref DefaultRamp ); }
            else { throw new InvalidOperationException( "Unable to obtain Gamma setting on this device." ); }
        }

        public static void SetGamma( Int32 gamma ) {
            if ( gamma.Between( 1, 256 ) ) {
                var ramp = new RAMP { Red = new UInt16[256], Green = new UInt16[256], Blue = new UInt16[256] };

                for ( var i = 1; i < 256; i++ ) {
                    var iArrayValue = i * ( gamma + 128 );

                    if ( iArrayValue > 65535 ) { iArrayValue = 65535; }

                    ramp.Red[i] = ramp.Blue[i] = ramp.Green[i] = ( UInt16 )iArrayValue;
                }

                NativeMethods.SetDeviceGammaRamp( NativeMethods.GetDC( IntPtr.Zero ), ref ramp );
            }
        }

        public static async Task<Erg> TryConvertToERG( this Document document, CancellationToken token ) =>
            await Task.Run( () => {
                var erg = new Erg();

                //TODO recalc the checksums
                //load file, checking checksums along the way.. (skip frames/lines with bad checksums?)
                //erg.TryAdd( document, Span.Zero,
                return erg;
            }, token );

        public static async Task<Boolean> TrySave( this Erg erg, Document document, CancellationToken token ) =>
            await Task.Run( () => {

                //TODO recalc the checksums
                //write out to file
                // ReSharper disable once ConvertToLambdaExpression
                return false;
            }, token );

        public static async Task<Boolean> TrySave( this Efv efv, Document document, CancellationToken token ) =>
            await Task.Run( () => {

                //TODO recalc the checksums
                //write out to file
                // ReSharper disable once ConvertToLambdaExpression
                var bob = new BinaryFormatter();

                //bob.Serialize(
                return false;
            }, token );
    }
}