// Copyright � Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "AdapterInfo.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

namespace Librainian.Internet;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OperatingSystem;

/// <summary>http://pastebin.com/u9159Ys8</summary>
public static class AdapterInfo {

	static AdapterInfo() {
		IpAdapterInfos = RetrieveAdapters();
		IndexedIpAdapterInfos = IpAdapterInfos.ToDictionary( o => ( UInt32 )o.Index );
	}

	public static Dictionary<UInt32, IPHelperInvoke.IPAdapterInfo> IndexedIpAdapterInfos { get; }

	public static IEnumerable<IPHelperInvoke.IPAdapterInfo> IpAdapterInfos { get; }

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
				var toStructure = Marshal.PtrToStructure( pEntry, typeof( IPHelperInvoke.IPAdapterInfo ) );
				if ( toStructure != null ) {
					var entry = ( IPHelperInvoke.IPAdapterInfo )toStructure;

					result.Add( entry );

					// Get next adapter (if any)
					pEntry = entry.Next;
				}
			} while ( pEntry != IntPtr.Zero );
		}

		Marshal.FreeHGlobal( pArray );

		return result;
	}
}