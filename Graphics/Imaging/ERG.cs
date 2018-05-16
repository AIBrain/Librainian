// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ERG.cs",
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
// "Librainian/Librainian/ERG.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Collections.Concurrent;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using FileSystem;
    using JetBrains.Annotations;
    using Maths;
    using Measurement.Time;
    using Newtonsoft.Json;

    /// <summary> Experimental Resilient Graphics </summary>
    /// <remarks>
    ///     Just for fun & learning.
    /// </remarks>
    /// <remarks>
    ///     Prefer native file system compression over encoding/compression speed
    ///     (assuming local cpu will be 'faster' than network transfer speed).
    ///     <para>Allow 'pages' of animation, each with their own delay. Default should be page 0 = 0 delay.</para>
    ///     <para>Checksums are used on each pixel to guard against (detect but not fix) corruption.</para>
    /// </remarks>
    /// <remarks> 60 frames per second allows 16.67 milliseconds per frame.</remarks>
    /// <remarks> 1920x1080 pixels = 2,052,000 possible pixels ...so about 8 nanoseconds per pixel? </remarks>
    [JsonObject]
    public class Erg {

        public static readonly String Extension = ".erg";

        /// <summary>
        ///     Human readable file header.
        /// </summary>
        public static readonly String Header = "ERG0.1";

        /// <summary>
        ///     EXIF metadata
        /// </summary>
        [JsonProperty]
        public readonly ConcurrentDictionary<String, String> Exifs = new ConcurrentDictionary<String, String>();

        public Erg() => this.Checksum = UInt64.MaxValue;

        /// <summary>
        ///     Checksum of all pages
        /// </summary>
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

        public async Task<UInt64> CalculateChecksumAsync() =>
            await Task.Run( () => {
                unchecked { return ( UInt64 )Hashing.GetHashCodes( this.Pixels ); }
            } );

        public async Task<Boolean> TryAdd( Document document, TimeSpan delay, CancellationToken cancellationToken ) {
            try { return await this.TryAdd( new Bitmap( document.FullPathWithFileName ), delay, cancellationToken ).ConfigureAwait( false ); }
            catch ( Exception exception ) { exception.More(); }

            return false;
        }

        public async Task<Boolean> TryAdd( [CanBeNull] Bitmap bitmap, TimeSpan timeout, CancellationToken cancellationToken ) {
            if ( bitmap is null ) { return false; }

            var stopwatch = StopWatch.StartNew();

            return await Task.Run( () => {
                var width = bitmap.Width;

                if ( width < UInt32.MinValue ) { return false; }

                var height = bitmap.Height;

                if ( height < UInt32.MinValue ) { return false; }

                this.PropertyIdList.UnionWith( bitmap.PropertyIdList );
                this.PropertyItems.UnionWith( bitmap.PropertyItems.Select( item => new PropertyItem { Id = item.Id, Len = item.Len, Type = item.Type, Value = item.Value } ) );

                this.Width = ( UInt32 )bitmap.Width;
                this.Height = ( UInt32 )bitmap.Height;

                var rect = new Rectangle( 0, 0, bitmap.Width, bitmap.Height );

                var data = bitmap.LockBits( rect, ImageLockMode.ReadOnly, bitmap.PixelFormat );

                Parallel.For( 0, this.Height, y => {
                    if ( stopwatch.Elapsed > timeout ) { return; }

                    if ( cancellationToken.IsCancellationRequested ) { return; }

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
            }, cancellationToken );
        }
    }
}