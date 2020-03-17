// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "ERG.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", File: "ERG.cs" was last formatted by Protiguous on 2020/03/16 at 3:00 PM.

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections.Sets;
    using JetBrains.Annotations;
    using Logging;
    using Maths.Hashings;
    using Newtonsoft.Json;
    using OperatingSystem.FileSystem;

    /// <summary> Experimental Resilient Graphics </summary>
    /// <remarks>Just for fun & learning.</remarks>
    /// <remarks>Prefer native file system compression over encoding/compression speed (assuming local cpu will be 'faster' than network transfer speed).
    /// <para>Allow 'pages' of animation, each with their own delay. Default should be page 0 = 0 delay.</para>
    /// <para>Checksums are used on each pixel to guard against (detect but not fix) corruption.</para>
    /// </remarks>
    /// <remarks> 60 frames per second allows 16.67 milliseconds per frame.</remarks>
    /// <remarks> 1920x1080 pixels = 2,052,000 possible pixels ...so about 8 nanoseconds per pixel? </remarks>
    [JsonObject]
    public class Erg {

        public static readonly String Extension = ".erg";

        /// <summary>Human readable file header.</summary>
        public static readonly String Header = "ERG0.1";

        /// <summary>EXIF metadata</summary>
        [JsonProperty]
        public readonly ConcurrentDictionary<String, String> Exifs = new ConcurrentDictionary<String, String>();

        /// <summary>Checksum of all pages</summary>
        [JsonProperty]
        public UInt64 Checksum { get; private set; }

        public UInt32 Height { get; private set; }

        [JsonProperty]
        public ConcurrentSet<Pixel> Pixels { get; } = new ConcurrentSet<Pixel>();

        [JsonProperty]
        public ConcurrentSet<Int32> PropertyIdList { get; } = new ConcurrentSet<Int32>();

        [JsonProperty]
        public ConcurrentSet<PropertyItem> PropertyItems { get; } = new ConcurrentSet<PropertyItem>();

        public UInt32 Width { get; private set; }

        public Erg() => this.Checksum = UInt64.MaxValue;

        [NotNull]
        public Task<UInt64> CalculateChecksumAsync() =>
            Task.Run( function: () => {
                unchecked {
                    return ( UInt64 )HashingExtensions.GetHashCodes( this.Pixels );
                }
            } );

        public async Task<Boolean> TryAdd( [CanBeNull] Document document, TimeSpan delay, CancellationToken cancellationToken ) {
            try {
                return await this.TryAddAsync( bitmap: new Bitmap( filename: document.FullPath ), timeout: delay, cancellationToken: cancellationToken )
                                 .ConfigureAwait( continueOnCapturedContext: false );
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        public async Task<Boolean> TryAddAsync( [CanBeNull] Bitmap bitmap, TimeSpan timeout, CancellationToken cancellationToken ) {
            if ( bitmap is null ) {
                return default;
            }

            var stopwatch = Stopwatch.StartNew();

            return await Task.Run( function: () => {
                var width = bitmap.Width;

                if ( width < UInt32.MinValue ) {
                    return default;
                }

                var height = bitmap.Height;

                if ( height < UInt32.MinValue ) {
                    return default;
                }

                this.PropertyIdList.UnionWith( other: bitmap.PropertyIdList );

                this.PropertyItems.UnionWith( other: bitmap.PropertyItems.Select( selector: item => new PropertyItem {
                    Id = item.Id,
                    Len = item.Len,
                    Type = item.Type,
                    Value = item.Value
                } ) );

                this.Width = ( UInt32 )bitmap.Width;
                this.Height = ( UInt32 )bitmap.Height;

                var rect = new Rectangle( x: 0, y: 0, width: bitmap.Width, height: bitmap.Height );

                var data = bitmap.LockBits( rect: rect, flags: ImageLockMode.ReadOnly, format: bitmap.PixelFormat );

                Parallel.For( fromInclusive: 0, toExclusive: this.Height, body: y => {
                    if ( stopwatch.Elapsed > timeout ) {
                        return;
                    }

                    if ( cancellationToken.IsCancellationRequested ) {
                        return;
                    }

                    for ( UInt32 x = 0; x < bitmap.Width; x++ ) {
                        var color = bitmap.GetPixel( x: ( Int32 )x, y: ( Int32 )y );
                        var pixel = new Pixel( color: color, x: x, y: ( UInt32 )y );
                        this.Pixels.TryAdd( item: pixel );
                    }
                } );

                bitmap.UnlockBits( bitmapdata: data );

                //TODO animated gif RE: image.FrameDimensionsList;

                //image.Palette?

                return false; //TODO add frame
            }, cancellationToken: cancellationToken ).ConfigureAwait( continueOnCapturedContext: false );
        }
    }
}