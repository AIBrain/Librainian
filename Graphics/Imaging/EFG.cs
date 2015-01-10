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
// "Librainian/Class2.cs" was last cleaned by Rick on 2014/11/16 at 2:43 PM
#endregion

namespace Librainian.Graphics.Imaging {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Maths;

    /// <summary>
    ///     Experimental Flexible Graphics
    /// </summary>
    /// <remarks>
    /// Just for fun & learning.
    /// Prefer compression over [decoding/display] speed (assuming local cpu will be 'faster' than network transfer speed).
    /// Compressions must be lossless.
    /// Allow 'pages' of animation, each with their own delay. Default should be page 0 = 0 delay.
    /// Checksums are used on each line of each page to guard against (detect but not fix) corruption.
    /// </remarks>
    /// <remarks>
    /// 60 frames per second allows 16.67 milliseconds per frame.
    /// 1920x1080 pixels = 2,052,000 possible pixels
    /// ...so about 8 nanoseconds per pixel?
    /// </remarks>
    [DataContract]
    [Serializable]
    //[StructLayout( LayoutKind.Explicit )]
    public class EFG {

        /// <summary>
        /// Human readable file header.
        /// </summary>
        public static readonly String Header = "EFG1";
        public static readonly String Extension = ".efg";

        /*
                /// <summary>
                /// only here for reference
                /// </summary>
                [Obsolete]
                private Bitmap Bitmap {
                    get;
                    set;
                }
        */

        /// <summary>
        /// Checksum of all pages
        /// </summary>
        [DataMember]
        public UInt64 Checksum {
            get;
            set;
        }

        /// <summary>
        /// EXIF metadatas
        /// </summary>
        [DataMember]
        public readonly ConcurrentDictionary<String, String> Exifs = new ConcurrentDictionary<String, String>();

        [DataMember]
        public readonly ConcurrentQueue<Pixel> Pixels = new ConcurrentQueue<Pixel>();


        public async Task<UInt64> CalculateChecksumAsync() => ( ulong )await Task.Run( () => {
            unchecked {
                return MathExtensions.GetBigHash( this.Pixels.ToArray() );
            }
        } );

        public EFG() {
            this.Checksum = UInt64.MaxValue;    //an unlikely hash
        }

        public async Task<Boolean> TryAdd( UInt64 index, Frame frame, CancellationToken token ) => await Task.Run( () => {
            // ReSharper disable once ConvertToLambdaExpression
            return false;   //TODO add frame
        }, token );
    }
}
