// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/ERG.cs" was last cleaned by Rick on 2015/06/12 at 2:55 PM

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Collections.Concurrent;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Collections;
    using JetBrains.Annotations;
    using Maths;
    using Measurement.Time;
    using OperatingSystem.FileSystem;
    using Threading;

    /// <summary> Experimental Resilient Graphics </summary> <remarks> Just for fun & learning.
    /// </remarks> <remarks> Prefer native file system compression over encoding/compression speed
    /// (assuming local cpu will be 'faster' than network transfer
    /// speed) . Allow 'pages' of animation, each with their own delay. Default should be page 0 = 0
    ///        delay. Checksums are used on each pixel to guard against (detect but not fix) corruption.
    /// </remarks> <remarks> 60 frames per second allows 16.67 milliseconds per frame.</remarks>
    /// <remarks> 1920x1080 pixels = 2,052,000 possible pixels ...so about 8 nanoseconds per pixel? </remarks>
    [DataContract]
    [Serializable]
    public class Erg {
        public static readonly String Extension = ".erg";

        /// <summary>Human readable file header.</summary>
        public static readonly String Header = "ERG1";

        /// <summary>EXIF metadatas</summary>
        [DataMember]
        public readonly ConcurrentDictionary<String, String> Exifs = new ConcurrentDictionary<String, String>();

        /// <summary>Checksum of all pages</summary>
        [DataMember]
        public UInt64 Checksum {
            get; private set;
        }

        public UInt32 Height {
            get; private set;
        }

        [DataMember]
        public ConcurrentSet<Pixel> Pixels {
            get;
        }
        = new ConcurrentSet<Pixel>();

        public ConcurrentSet<Int32> PropertyIdList {
            get; private set;
        }

        public ConcurrentSet<PropertyItem> PropertyItems {
            get; private set;
        }

        public UInt32 Width {
            get; private set;
        }

        public Erg() {
            this.Checksum = UInt64.MaxValue; //an unlikely hash
        }

        public async Task<UInt64> CalculateChecksumAsync() => await Task.Run( () => {
            unchecked {
                return ( UInt64 )MathHashing.GetHashCodes( this.Pixels );
            }
        } );

        public async Task<Boolean> TryAdd(Document document, Span delay, SimpleCancel simpleCancel) => await this.TryAdd( new Bitmap( document.FullPathWithFileName ), delay, simpleCancel );

        public async Task<Boolean> TryAdd([CanBeNull] Bitmap bitmap, Span timeout, SimpleCancel simpleCancel) {
            if ( bitmap == null ) {
                return false;
            }
            var stopwatch = StopWatch.StartNew();
            return await Task.Run( () => {
                var width = bitmap.Width;
                if ( width < UInt32.MinValue ) {
                    return false;
                }

                var height = bitmap.Height;
                if ( height < UInt32.MinValue ) {
                    return false;
                }

                this.PropertyIdList = new ConcurrentSet<Int32>( bitmap.PropertyIdList );
                this.PropertyItems = new ConcurrentSet<PropertyItem>( bitmap.PropertyItems );

                this.Width = ( UInt32 )bitmap.Width;
                this.Height = ( UInt32 )bitmap.Height;

                var rect = new Rectangle( 0, 0, bitmap.Width, bitmap.Height );

                var data = bitmap.LockBits( rect, ImageLockMode.ReadOnly, bitmap.PixelFormat );

                Parallel.For( 0, this.Height, y => {
                    if ( stopwatch.Elapsed > timeout ) {
                        return;
                    }
                    if ( simpleCancel.HaveAnyCancellationsBeenRequested() ) {
                        return;
                    }
                    for ( UInt32 x = 0; x < bitmap.Width; x++ ) {
                        var color = bitmap.GetPixel( ( Int32 )x, ( Int32 )y );
                        var pixel = new Pixel( color, x, ( UInt32 )y );
                        this.Pixels.TryAdd( pixel );
                    }
                } );

                bitmap.UnlockBits( data );

                //TODO animated gif RE: image.FrameDimensionsList;

                //image.Palette?

                return false; //TODO add frame
            } );
        }
    }
}