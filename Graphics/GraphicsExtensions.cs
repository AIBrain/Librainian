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
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/GraphicsExtensions.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics {

    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using FileSystem;
    using Imaging;
    using Maths;
    using Moving;
    using OperatingSystem;

    public static class GraphicsExtensions {
        public static RAMP DefaultRamp;

        private static Boolean _gotGamma;

        static GraphicsExtensions() => DefaultRamp = GetGamma();

	    public static Stream EfvToStream() {
            var ms = new MemoryStream();

            //TODO
            return ms;
        }

        public static RAMP GetGamma() {
            var ramp = new RAMP();
            if ( NativeMethods.GetDeviceGammaRamp( NativeMethods.GetDC( IntPtr.Zero ), ref ramp ) ) {
                GraphicsExtensions._gotGamma = true;
                return ramp;
            }
            throw new InvalidOperationException( "Unable to obtain Gamma setting." );
        }

        /// <summary>
        ///     Resets the gamma to when it was first called.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static void ResetGamma() {
            if ( GraphicsExtensions._gotGamma ) {
                NativeMethods.SetDeviceGammaRamp( NativeMethods.GetDC( IntPtr.Zero ), ref DefaultRamp );
            }
            else {
                throw new InvalidOperationException( "Unable to obtain Gamma setting on this device." );
            }
        }

        public static void SetGamma( Int32 gamma ) {
            if ( gamma.Between( 1, 256 ) ) {
                var ramp = new RAMP { Red = new UInt16[ 256 ], Green = new UInt16[ 256 ], Blue = new UInt16[ 256 ] };
                for ( var i = 1; i < 256; i++ ) {
                    var iArrayValue = i * ( gamma + 128 );

                    if ( iArrayValue > 65535 ) {
                        iArrayValue = 65535;
                    }
                    ramp.Red[ i ] = ramp.Blue[ i ] = ramp.Green[ i ] = ( UInt16 )iArrayValue;
                }

                NativeMethods.SetDeviceGammaRamp( NativeMethods.GetDC( IntPtr.Zero ), ref ramp );
            }
        }

        public static async Task<Erg> TryConvertToERG( this Document document, CancellationToken token ) => await Task.Run( () => {
            var erg = new Erg();

            //TODO recalc the checksums
            //load file, checking checksums along the way.. (skip frames/lines with bad checksums?)
            //erg.TryAdd( document, Span.Zero,
            return erg;
        }, token );

        public static async Task<Boolean> TrySave( this Erg erg, Document document, CancellationToken token ) => await Task.Run( () => {

            //TODO recalc the checksums
            //write out to file
            // ReSharper disable once ConvertToLambdaExpression
            return false;
        }, token );

        public static async Task<Boolean> TrySave( this Efv efv, Document document, CancellationToken token ) => await Task.Run( () => {

            //TODO recalc the checksums
            //write out to file
            // ReSharper disable once ConvertToLambdaExpression
            var bob = new BinaryFormatter();

            //bob.Serialize(
            return false;
        }, token );

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
        public struct RAMP {

            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
            public UInt16[] Red;

            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
            public UInt16[] Green;

            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
            public UInt16[] Blue;
        }
    }
}