// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "IPHelperInvoke.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "IPHelperInvoke.cs" was last formatted by Protiguous on 2020/03/16 at 2:55 PM.

namespace Librainian.Internet {

    using System;
    using System.Runtime.InteropServices;

    /// <summary>http://pastebin.com/u9159Ys8</summary>
    public static class IPHelperInvoke {

        public const Int32 ErrorBufferOverflow = 111;

        public const Int32 MaxAdapterAddressLength = 8;

        public const Int32 MaxAdapterDescriptionLength = 128;

        public const Int32 MaxAdapterNameLength = 256;

        [StructLayout( layoutKind: LayoutKind.Sequential, CharSet = CharSet.Ansi )]
        public struct IPAdapterInfo {

            internal IntPtr Next;

            public readonly Int32 ComboIndex;

            [MarshalAs( unmanagedType: UnmanagedType.ByValTStr, SizeConst = MaxAdapterNameLength + 4 )]
            public readonly String AdapterName;

            [MarshalAs( unmanagedType: UnmanagedType.ByValTStr, SizeConst = MaxAdapterDescriptionLength + 4 )]
            public readonly String AdapterDescription;

            public UInt32 AddressLength;

            [MarshalAs( unmanagedType: UnmanagedType.ByValArray, SizeConst = MaxAdapterAddressLength )]
            public Byte[] Address;

            public Int32 Index;

            public UInt32 Type;

            public UInt32 DhcpEnabled;

            private readonly IntPtr CurrentIpAddress;

            public IPAddrString IpAddressList;

            public IPAddrString GatewayList;

            public IPAddrString DhcpServer;

            public readonly Boolean HaveWins;

            public IPAddrString PrimaryWinsServer;

            public IPAddrString SecondaryWinsServer;

            public Int32 LeaseObtained;

            public Int32 LeaseExpires;
        }

        [StructLayout( layoutKind: LayoutKind.Sequential, CharSet = CharSet.Ansi )]
        public struct IPAddressString {

            [MarshalAs( unmanagedType: UnmanagedType.ByValTStr, SizeConst = 16 )]
            public readonly String Address;
        }

        [StructLayout( layoutKind: LayoutKind.Sequential, CharSet = CharSet.Ansi )]
        public struct IPAddrString {

            private readonly IntPtr Next;

            public IPAddressString IpAddress;

            public IPAddressString IpMask;

            public readonly Int32 Context;
        }
    }
}