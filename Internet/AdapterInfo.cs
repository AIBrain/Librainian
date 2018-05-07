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
// "Librainian/AdapterInfo.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

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

        static AdapterInfo() {
            IpAdapterInfos = RetrieveAdapters();
            IndexedIpAdapterInfos = IpAdapterInfos.ToDictionary( o => ( UInt32 )o.Index );
        }

        public static Dictionary<UInt32, IPHelperInvoke.IPAdapterInfo> IndexedIpAdapterInfos {
            get;
        }

        public static IEnumerable<IPHelperInvoke.IPAdapterInfo> IpAdapterInfos {
            get;
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