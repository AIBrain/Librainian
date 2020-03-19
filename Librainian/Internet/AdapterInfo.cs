// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "AdapterInfo.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "AdapterInfo.cs" was last formatted by Protiguous on 2020/03/18 at 10:24 AM.

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;
    using OperatingSystem;

    /// <summary>http://pastebin.com/u9159Ys8</summary>
    public static class AdapterInfo {

        public static Dictionary<UInt32, IPHelperInvoke.IPAdapterInfo> IndexedIpAdapterInfos { get; }

        public static IEnumerable<IPHelperInvoke.IPAdapterInfo> IpAdapterInfos { get; }

        static AdapterInfo() {
            IpAdapterInfos = RetrieveAdapters();
            IndexedIpAdapterInfos = IpAdapterInfos.ToDictionary( o => ( UInt32 ) o.Index );
        }

        [NotNull]
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
                    var entry = ( IPHelperInvoke.IPAdapterInfo ) Marshal.PtrToStructure( pEntry, typeof( IPHelperInvoke.IPAdapterInfo ) );

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