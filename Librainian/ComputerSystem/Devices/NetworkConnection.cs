// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "NetworkConnection.cs" last formatted on 2022-12-22 at 5:14 PM by Protiguous.

namespace Librainian.ComputerSystem.Devices;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using Logging;
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

public enum ResourceDisplayType {

	GENERIC,

	DOMAIN,

	SERVER,

	SHARE,

	FILE,

	GROUP,

	NETWORK,

	ROOT,

	SHAREADMIN,

	DIRECTORY,

	TREE,

	NDSCONTAINER

}

public enum ResourceScope {

	Connected = 1,

	GlobalNetwork,

	Remembered,

	Recent,

	Context

}

public enum ResourceType {

	Any = 0,

	Disk = 1,

	Print = 2,

	Reserved = 8

}

public enum ResourceUsage {

	CONNECTABLE = 0x00000001,

	CONTAINER = 0x00000002,

	NOLOCALDEVICE = 0x00000004,

	SIBLING = 0x00000008,

	ATTACHED = 0x00000010,

	ALL = CONNECTABLE | CONTAINER | ATTACHED

}

[StructLayout( LayoutKind.Sequential )]
[SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Global" )]
public struct NETRESOURCE {

	public ResourceDisplayType dwDisplayType;

	public ResourceScope dwScope;

	public ResourceType dwType;

	public ResourceUsage dwUsage;

	public String? lpComment;

	public String? lpLocalName;

	public String? lpProvider;

	public String? lpRemoteName;

}

[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
public class NetResource {

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

	[field: MarshalAs( UnmanagedType.LPWStr )]
	public String Comment { get; set; }

}

public class NetworkConnection : IDisposable {

	public NetworkConnection( String? networkName, NetworkCredential credentials ) {
		this.NetworkName = networkName;

		var netResource = new NetResource {
			Scope = ResourceScope.GlobalNetwork,
			ResourceType = ResourceType.Disk,
			DisplayType = ResourceDisplaytype.Share,
			RemoteName = networkName
		};

		var userName = String.IsNullOrEmpty( credentials.Domain ) ? credentials.UserName : $@"{credentials.Domain}\{credentials.UserName}";

		var result = NativeMethods.WNetAddConnection2( ref netResource, credentials.Password, userName, 0 );

		if ( result != 0 ) {
			throw new Win32Exception( result, "Error connecting to remote share" );
		}
	}

	private String? NetworkName { get; }

	public void Dispose() {
		this.Dispose( true );
		GC.SuppressFinalize( this );
	}

	~NetworkConnection() => this.Dispose( false );

	protected virtual void Dispose( Boolean disposing ) => NativeMethods.WNetCancelConnection2( this.NetworkName, 0, true );

	public static Boolean IsNetworkConnected( Int32 retries = 3 ) {
		var counter = retries;

		while ( !NetworkInterface.GetIsNetworkAvailable() && counter > 0 ) {
			--counter;
			$"Network disconnected. Waiting {Seconds.One}. {counter} retries left...".Info();
			Thread.Sleep( Seconds.One );
		}

		return NetworkInterface.GetIsNetworkAvailable();
	}

}