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
    using System.Runtime.InteropServices;

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

    [StructLayout( LayoutKind.Sequential )]
    public class NetResource {
        public String Comment;
        public ResourceDisplaytype DisplayType;
        public String LocalName;
        public String Provider;
        public String RemoteName;
        public ResourceType ResourceType;
        public ResourceScope Scope;
        public Int32 Usage;
    }

    public class NetworkConnection : IDisposable {

        public NetworkConnection( String networkName, NetworkCredential credentials ) {
            this.NetworkName = networkName;

            var netResource = new NetResource { Scope = ResourceScope.GlobalNetwork, ResourceType = ResourceType.Disk, DisplayType = ResourceDisplaytype.Share, RemoteName = networkName };

            var userName = String.IsNullOrEmpty( credentials.Domain ) ? credentials.UserName : $@"{credentials.Domain}\{credentials.UserName}";

            var result = WNetAddConnection2( netResource, credentials.Password, userName, 0 );

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
            WNetCancelConnection2( this.NetworkName, 0, true );
        }

        [DllImport( "mpr.dll" )]
        private static extern Int32 WNetAddConnection2( NetResource netResource, String password, String username, Int32 flags );

        [DllImport( "mpr.dll" )]
        private static extern Int32 WNetCancelConnection2( String name, Int32 flags, Boolean force );
    }
}