// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Memory.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", "Memory.cs" was last formatted by Protiguous on 2019/11/30 at 7:20 AM.

namespace Librainian.ComputerSystem {

    using System;
    using System.Diagnostics;
    using System.Runtime;
    using Maths;

    public static class Memory {

        [DebuggerStepThrough]
        public static Boolean CanAllocateMemory( this Int32 bytesOfRAM, Boolean gc = true ) {
            try {
                var megabytes = bytesOfRAM / MathConstants.Sizes.OneMegaByte;

                if ( !megabytes.Any() ) {
                    return true; /*zero mb? sure!*/
                }

                if ( gc ) {
                    GC.Collect( 2, GCCollectionMode.Optimized, true, true );
                }

                using var _ = new MemoryFailPoint( megabytes );

                return true;
            }
            catch ( ArgumentOutOfRangeException ) {
                return false;
            }
            catch ( InsufficientMemoryException ) {
                return false;
            }
            catch ( OutOfMemoryException ) {
                return false;
            }
        }

        public static Int32 LargestCanAllocate( UInt64 maxNeeded = Int32.MaxValue ) {
            var size = ( Int32 ) Math.Min( maxNeeded, Int32.MaxValue );

            do {
                if ( size.CanAllocateMemory() ) {
                    return size;
                }

                size -= size / 10; //shave off 1/10 of the value

                if ( size <= 4096 ) {
                    return 4096;
                }
            } while ( true );
        }

    }

}