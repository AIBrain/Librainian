// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Computer.cs" last formatted on 2020-08-14 at 8:31 PM.

namespace Librainian.ComputerSystem {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Net.NetworkInformation;
	using JetBrains.Annotations;
	using Parsing;

	public class Computer {

		public void AbortShutdown() => Process.Start( "shutdown", "/a" );

		[NotNull]
		public IEnumerable<String> GetVersions() =>
			AppDomain.CurrentDomain.GetAssemblies()
					 .Select( assembly => $"Assembly: {assembly.GetName().Name ?? Symbols.Null}, {assembly.GetName().Version?.ToString() ?? Symbols.Null}" );

		[NotNull]
		public IEnumerable<String> GetWorkingMacAddresses() =>
			from nic in NetworkInterface.GetAllNetworkInterfaces()
			where nic.OperationalStatus == OperationalStatus.Up
			select nic.GetPhysicalAddress().ToString();

		public void Hibernate( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/h" : $"/h /t {( Int32 )delay.Value.TotalSeconds}" );

		/// <summary>
		///     Provides properties for getting information about the computer's memory, loaded assemblies, name, and
		///     operating system. (Uses the VisualBasic library)
		/// </summary>
		public void Logoff( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/l" : $"/l /t {( Int32 )delay.Value.TotalSeconds}" );

		/// <summary>Send a reboot request.</summary>
		public void Restart( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/r" : $"/r /t {( Int32 )delay.Value.TotalSeconds}" );

		public void RestartFast( TimeSpan? delay = null ) =>
			Process.Start( "shutdown", !delay.HasValue ? "/hybrid /s" : $"/hybrid /s /t {( Int32 )delay.Value.TotalSeconds}" );

		public void Shutdown( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/s" : $"/s /t {( Int32 )delay.Value.TotalSeconds}" );

		/// <summary>Turn off the local computer with no time-out or warning.</summary>
		public void ShutdownNow() => Process.Start( "shutdown", "/p" );

	}

}