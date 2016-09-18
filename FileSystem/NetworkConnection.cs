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
// "Librainian/NetworkConnection.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Measurement.Time;
    using OperatingSystem;

    public enum ResourceDisplaytype {
        Generic = 0x0,
        Domain = 0x01,
        Server = 0x02,
        Share = 0x03,
        File = 0x04,
        Group = 0x05,
        Network = 0x06,
        Root = 0x07,
        Shareadmin = 0x08,
        Directory = 0x09,
        Tree = 0x0a,
        Ndscontainer = 0x0b
    }

    public enum ResourceScope {
        Connected = 1,
        GlobalNetwork,
        Remembered,
        Recent,
        Context
    };

    public enum ResourceType {
        Any = 0,
        Disk = 1,
        Print = 2,
        Reserved = 8
    }

    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class NetResource {
        [MarshalAs( UnmanagedType.LPWStr )]
        public String Comment;

        public ResourceDisplaytype DisplayType;

        [MarshalAs( UnmanagedType.LPWStr )]
        public String LocalName;

        [MarshalAs( UnmanagedType.LPWStr )]
        public String Provider;

        [MarshalAs( UnmanagedType.LPWStr )]
        public String RemoteName;

        public ResourceType ResourceType;
        public ResourceScope Scope;
        public Int32 Usage;
    }

    public enum ResourceDisplayType {
        RESOURCEDISPLAYTYPE_GENERIC,
        RESOURCEDISPLAYTYPE_DOMAIN,
        RESOURCEDISPLAYTYPE_SERVER,
        RESOURCEDISPLAYTYPE_SHARE,
        RESOURCEDISPLAYTYPE_FILE,
        RESOURCEDISPLAYTYPE_GROUP,
        RESOURCEDISPLAYTYPE_NETWORK,
        RESOURCEDISPLAYTYPE_ROOT,
        RESOURCEDISPLAYTYPE_SHAREADMIN,
        RESOURCEDISPLAYTYPE_DIRECTORY,
        RESOURCEDISPLAYTYPE_TREE,
        RESOURCEDISPLAYTYPE_NDSCONTAINER
    };

    public enum ResourceUsage {
        RESOURCEUSAGE_CONNECTABLE = 0x00000001,
        RESOURCEUSAGE_CONTAINER = 0x00000002,
        RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
        RESOURCEUSAGE_SIBLING = 0x00000008,
        RESOURCEUSAGE_ATTACHED = 0x00000010,
        RESOURCEUSAGE_ALL = RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED,
       };


    [StructLayout( LayoutKind.Sequential )]
    public class NETRESOURCE {
        public ResourceScope dwScope = 0;
        public ResourceType dwType = 0;
        public ResourceDisplayType dwDisplayType = 0;
        public ResourceUsage dwUsage = 0;
        public String lpLocalName = null;
        public String lpRemoteName = null;
        public String lpComment = null;
        public String lpProvider = null;
    };



    public class NetworkConnection : IDisposable {

        public NetworkConnection( String networkName, NetworkCredential credentials ) {
            this.NetworkName = networkName;

            var netResource = new NetResource { Scope = ResourceScope.GlobalNetwork, ResourceType = ResourceType.Disk, DisplayType = ResourceDisplaytype.Share, RemoteName = networkName };

            var userName = String.IsNullOrEmpty( credentials.Domain ) ? credentials.UserName : $@"{credentials.Domain}\{credentials.UserName}";

            var result = NativeMethods.WNetAddConnection2( ref netResource, credentials.Password, userName, 0 );

            if ( result != 0 ) {
                throw new Win32Exception( result, "Error connecting to remote share" );
            }
        }

        ~NetworkConnection() {
            this.Dispose( false );
        }

        private String NetworkName {
            get;
        }

        public void Dispose() {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( Boolean disposing ) {
            NativeMethods.WNetCancelConnection2( this.NetworkName, 0, true );
        }

        public static Boolean IsNetworkConnected( Int32 retries = 3 ) {
            var counter = retries;
            while ( !NetworkInterface.GetIsNetworkAvailable() && counter > 0 ) {
                --counter;
                $"Network disconnected. Waiting {Seconds.One}. {counter} retries left...".WriteLine();
                Thread.Sleep( Seconds.One );
            }
            return NetworkInterface.GetIsNetworkAvailable();
        }

    }
}