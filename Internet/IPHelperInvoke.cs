// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "IPHelperInvoke.cs",
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
// "Librainian/Librainian/IPHelperInvoke.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Internet {

    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     http://pastebin.com/u9159Ys8
    /// </summary>
    public class IPHelperInvoke {

        public const Int32 ErrorBufferOverflow = 111;

        public const Int32 MaxAdapterAddressLength = 8;

        public const Int32 MaxAdapterDescriptionLength = 128;

        public const Int32 MaxAdapterNameLength = 256;

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
        public struct IPAdapterInfo {

            internal IntPtr Next;

            public readonly Int32 ComboIndex;

            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = MaxAdapterNameLength + 4 )]
            public readonly String AdapterName;

            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = MaxAdapterDescriptionLength + 4 )]
            public readonly String AdapterDescription;

            public UInt32 AddressLength;

            [MarshalAs( UnmanagedType.ByValArray, SizeConst = MaxAdapterAddressLength )]
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

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
        public struct IPAddressString {

            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 16 )]
            public readonly String Address;
        }

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
        public struct IPAddrString {

            private readonly IntPtr Next;

            public IPAddressString IpAddress;

            public IPAddressString IpMask;

            public readonly Int32 Context;
        }
    }
}