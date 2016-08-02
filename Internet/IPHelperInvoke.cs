// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/IPHelperInvoke.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

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

        [DllImport( "iphlpapi.dll", CharSet = CharSet.Ansi )]
        public static extern Int32 GetAdaptersInfo( IntPtr pAdapterInfo, ref Int64 pBufOutLen );

        [DllImport( "iphlpapi.dll", SetLastError = true )]
        public static extern Int32 GetBestInterface( UInt32 destAddr, out UInt32 bestIfIndex );

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
        public struct IPAdapterInfo {
            public IntPtr Next;
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
            public IntPtr CurrentIpAddress;
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
            public IntPtr Next;
            public IPAddressString IpAddress;
            public IPAddressString IpMask;
            public readonly Int32 Context;
        }
    }
}