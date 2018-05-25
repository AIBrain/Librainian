// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "AdapterInfo.cs",
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
// "Librainian/Librainian/AdapterInfo.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using OperatingSystem;

    /// <summary>
    ///     http://pastebin.com/u9159Ys8
    /// </summary>
    public class AdapterInfo {

        public static Dictionary<UInt32, IPHelperInvoke.IPAdapterInfo> IndexedIpAdapterInfos { get; }

        public static IEnumerable<IPHelperInvoke.IPAdapterInfo> IpAdapterInfos { get; }

        static AdapterInfo() {
            IpAdapterInfos = RetrieveAdapters();
            IndexedIpAdapterInfos = IpAdapterInfos.ToDictionary( o => ( UInt32 )o.Index );
        }

        private static IEnumerable<IPHelperInvoke.IPAdapterInfo> RetrieveAdapters() {
            Int64 structSize = Marshal.SizeOf( typeof( IPHelperInvoke.IPAdapterInfo ) );
            var pArray = Marshal.AllocHGlobal( new IntPtr( structSize ) );

            var ret = NativeMethods.GetAdaptersInfo( pArray, ref structSize );

            var pEntry = pArray;

            if ( ret == IPHelperInvoke.ErrorBufferOverflow ) {

                // Buffer was too small, reallocate the correct size for the buffer.
                pArray = Marshal.ReAllocHGlobal( pArray, new IntPtr( structSize ) ); //BUG memory leak? or does realloc take into account?
                ret = NativeMethods.GetAdaptersInfo( pArray, ref structSize );
            }

            var result = new List<IPHelperInvoke.IPAdapterInfo>();

            if ( ret == 0 ) {
                do {

                    // Retrieve the adapter info from the memory address
                    var entry = ( IPHelperInvoke.IPAdapterInfo )Marshal.PtrToStructure( pEntry, typeof( IPHelperInvoke.IPAdapterInfo ) );

                    result.Add( entry );

                    // Get next adapter (if any)
                    pEntry = entry.Next;
                } while ( pEntry != IntPtr.Zero );
            }

            Marshal.FreeHGlobal( pArray );

            return result;
        }
    }
}