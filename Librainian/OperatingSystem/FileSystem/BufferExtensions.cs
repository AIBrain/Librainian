// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "BufferExtensions.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "BufferExtensions.cs" was last formatted by Protiguous on 2019/04/13 at 10:03 PM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime;
    using ComputerSystem;
    using JetBrains.Annotations;
    using Logging;
    using Maths;

    /// <summary>
    ///     For copying or moving a <see cref="Document" /> using largest possible buffer.
    /// </summary>
    public static class BufferExtensions {

        /// <summary>
        ///     Largest 2^power this will try.
        /// </summary>
        private const Byte SizeHigh = 32;

        /// <summary>
        ///     Smallest 2^power this will try.
        /// </summary>
        private const Byte SizeLow = 0;

        /// <summary>
        ///     Just some common buffer sizes we might use.
        /// </summary>
        private static readonly HashSet<Int32> BufferSizes = new HashSet<Int32>( SizeLow.To( SizeHigh ).Select( b => ( Int32 ) Math.Pow( 2, b ) ) );

        public static Int32 OptimalBufferSize( [NotNull] this Document document ) {
            if ( document == null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            var size = document.Size();

            if ( !size.HasValue || !size.Any() ) {
                return DefaultBufferSize;
            }

            if ( size > Int64.MaxValue ) {
                size = Int64.MaxValue;
            }

            var fileSize = ( Int64 ) size.Value;

            return fileSize.OptimalBufferSize();
        }

        public const Int32 DefaultBufferSize = 4096;

        /// <summary>
        ///     Gets the largest buffer we can allocate. Up to 2^32 bytes. Defaults to 4096 bytes.
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public static Int32 OptimalBufferSize( this Int64 fileSize ) {
            try {
                if ( !fileSize.Any() ) {
                    return DefaultBufferSize;
                }

                var ram = ( Int64 ) new Computer().GetAvailableMemeory();

                foreach ( var ul in BufferSizes.Where( value => value <= fileSize && value <= ram ).OrderByDescending( value => value ).Select( value => value ) ) {
                    try {
                        using ( new MemoryFailPoint( ul / MathConstants.Sizes.OneMegaByte ) ) {
                            return ul;
                        }
                    }
                    catch ( ArgumentOutOfRangeException ) { }
                    catch ( InsufficientMemoryException ) { }
                    catch ( OutOfMemoryException ) { }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return DefaultBufferSize;
        }

    }

}