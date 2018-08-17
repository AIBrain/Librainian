// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Computer.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Computer.cs" was last formatted by Protiguous on 2018/07/10 at 8:52 PM.

namespace Librainian.ComputerSystem {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Management;
	using System.Net.NetworkInformation;
	using System.Text;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Measurement.Time;
	using Microsoft.VisualBasic.Devices;

	public class Beep {

		public void At( Int32 frequency, TimeSpan duration ) => Console.Beep( frequency: frequency, duration: ( Int32 ) duration.TotalMilliseconds );

		public void High() => this.At( 14917, TimeExtensions.GetAverageDateTimePrecision() );

		public void Low() => this.At( 440, TimeExtensions.GetAverageDateTimePrecision() );
	}

	public class Computer {

		private ManagementObjectSearcher PerfFormattedDataManagementObjectSearcher { get; } = new ManagementObjectSearcher( "select * from Win32_PerfFormattedData_PerfOS_Processor" );

		private HashSet<PerformanceCounter> UtilizationCounters { get; }

		[NotNull]
		public ComputerInfo Info { get; }

		/// <summary>
		/// </summary>
		/// <remarks>http: //msdn.microsoft.com/en-us/Library/aa394347(VS.85).aspx</remarks>
		/// <returns></returns>
		public UInt64 RAM {
			get {
				try {
					using ( var searcher = new ManagementObjectSearcher( "Select * from Win32_PhysicalMemory" ) ) {
						UInt64 total = 0;

						foreach ( var result in searcher.Get() ) {
							var mem = Convert.ToUInt64( result[ "Capacity" ] );
							total += mem;
						}

						return total;
					}
				}
				catch ( Exception exception ) { exception.Log(); }

				return 0;
			}
		}

		/// <summary>
		/// </summary>
		/// <see cref="http://msdn.microsoft.com/en-us/Library/aa394347(VS.85).aspx" />
		public UInt64 TotalPhysicalMemory {
			get {
				try { return this.Info.TotalPhysicalMemory; }
				catch ( Exception exception ) {
					exception.Log();

					return 0;
				}
			}
		}

		public Computer() {

			// Initialize the list to a counter-per-processor:
			this.UtilizationCounters = new HashSet<PerformanceCounter>( Environment.ProcessorCount );

			for ( var i = 0; i < Environment.ProcessorCount; i++ ) { this.UtilizationCounters.Add( new PerformanceCounter( "Processor", "% Processor Time", i.ToString() ) ); }

			this.Info = new ComputerInfo(); // TODO how slow is this class? The code behind it doesn't *look* slow..
		}

		private Int32 GetFreeProcessors() => this.UtilizationCounters.Count( pc => pc.NextValue() <= 0.50f );

		public void AbortShutdown() => Process.Start( "shutdown", "/a" );

		/// <summary>
		///     //TODO description. Bytes? Which one does .NET allocate objects in..? Sum, or smaller of the two? Is this real
		///     time? How fast/slow is this method?
		/// </summary>
		/// <returns></returns>
		public UInt64 GetAvailableMemeory() {
			var info = this.Info;

			return Math.Min( info.AvailablePhysicalMemory, info.AvailableVirtualMemory );
		}

		/// <summary>
		/// </summary>
		/// <remarks>http: //msdn.microsoft.com/en-us/Library/aa394347(VS.85).aspx</remarks>
		/// <returns></returns>
		public UInt64 GetAvailableVirtualMemory() {
			try {
				using ( var searcher = new ManagementObjectSearcher( "Select * from Win32_LogicalMemoryConfiguration" ) ) {
					var total = searcher.Get().Cast<ManagementBaseObject>().Select( baseObject => ( UInt64 ) baseObject[ "AvailableVirtualMemory" ] ).Aggregate( 0UL, ( current, mem ) => current + mem );

					return total;
				}
			}
			catch ( Exception exception ) {
				exception.Log();

				return 0;
			}
		}

		/// <summary>
		///     Use WMI (System.Managment) to get the CPU type
		/// </summary>
		/// <remarks>http: //msdn2.microsoft.com/en-us/Library/aa394373(VS.85).aspx</remarks>
		/// <returns></returns>
		[NotNull]
		public String GetCPUDescription() {
			try {
				using ( var searcher = new ManagementObjectSearcher( "Select * from Win32_Processor" ) ) {
					var sb = new StringBuilder();

					foreach ( var result in searcher.Get() ) { sb.Append( $"{result[ "Name" ]} with {result[ "NumberOfCores" ]} cores" ); }

					return sb.ToString();
				}
			}
			catch ( Exception ex ) { return $"WMI Error: {ex.Message}"; }
		}

		/// <summary>
		///     Get the average percent of all cpu cores being used.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public Single GetCPUUsage() {
			var cpuTimes = this.PerfFormattedDataManagementObjectSearcher.Get().Cast<ManagementObject>().Select( managementObject => new {
				Name = managementObject[ "Name" ],
				Usage = Convert.ToSingle( managementObject[ "PercentProcessorTime" ] ) / 100.0f
			} ); //.ToList();

			//The '_Total' value represents the average usage across all cores, and is the best representation of overall CPU usage
			var cpuUsage = cpuTimes.Where( x => x.Name.ToString() == "_Total" ).Select( x => x.Usage ).Single();
			cpuUsage.Should().BeGreaterOrEqualTo( 0 );
			cpuUsage.Should().BeLessOrEqualTo( 1 );

			return cpuUsage;
		}

		[NotNull]
		public IEnumerable<String> GetVersions() => AppDomain.CurrentDomain.GetAssemblies().Select( assembly => $"Assembly: {assembly.GetName().Name}, {assembly.GetName().Version}" );

		[NotNull]
		public IEnumerable<String> GetWorkingMacAddresses() => from nic in NetworkInterface.GetAllNetworkInterfaces() where nic.OperationalStatus == OperationalStatus.Up select nic.GetPhysicalAddress().ToString();

		public void Hibernate( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/h" : $"/h /t {( Int32 ) delay.Value.TotalSeconds}" );

		/// <summary>
		///     Provides properties for getting information about the computer's memory, loaded assemblies, name, and operating
		///     system. (Uses the VisualBasic library)
		/// </summary>
		public void Logoff( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/l" : $"/l /t {( Int32 ) delay.Value.TotalSeconds}" );

		/// <summary>
		///     Send a reboot request.
		/// </summary>
		public void Restart( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/r" : $"/r /t {( Int32 ) delay.Value.TotalSeconds}" );

		public void RestartFast( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/hybrid /s" : $"/hybrid /s /t {( Int32 ) delay.Value.TotalSeconds}" );

		public void Shutdown( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/s" : $"/s /t {( Int32 ) delay.Value.TotalSeconds}" );

		/// <summary>
		///     Turn off the local computer with no time-out or warning.
		/// </summary>
		public void ShutdownNow() => Process.Start( "shutdown", "/p" );

		public class SATScores {

			public Lazy<ManagementObjectSearcher> Searcher { get; } = new Lazy<ManagementObjectSearcher>( () => new ManagementObjectSearcher( "root\\CIMV2", "SELECT * FROM Win32_WinSAT" ) );

			public Single? CPU() {
				try {
					foreach ( var queryObj in this.Searcher.Value.Get().OfType<ManagementObject>() ) {
						if ( Single.TryParse( queryObj[ "CPUScore" ].ToString(), out var result ) ) { return result; }
					}
				}
				catch ( ManagementException exception ) { exception.Log(); }

				return null;
			}

			public Single? D3D() {
				try {
					foreach ( var queryObj in this.Searcher.Value.Get().OfType<ManagementObject>() ) {
						if ( Single.TryParse( queryObj[ "D3DScore" ].ToString(), out var result ) ) { return result; }
					}
				}
				catch ( ManagementException exception ) { exception.Log(); }

				return null;
			}

			public Single? Disk() {
				try {
					foreach ( var queryObj in this.Searcher.Value.Get().OfType<ManagementObject>() ) {
						if ( Single.TryParse( queryObj[ "DiskScore" ].ToString(), out var result ) ) { return result; }
					}
				}
				catch ( ManagementException exception ) { exception.Log(); }

				return null;
			}

			public Single? Graphics() {
				try {
					foreach ( var queryObj in this.Searcher.Value.Get().OfType<ManagementObject>() ) {
						if ( Single.TryParse( queryObj[ "GraphicsScore" ].ToString(), out var result ) ) { return result; }
					}
				}
				catch ( ManagementException exception ) { exception.Log(); }

				return null;
			}

			public Single? Memory() {
				try {
					foreach ( var queryObj in this.Searcher.Value.Get().OfType<ManagementObject>() ) {
						if ( Single.TryParse( queryObj[ "MemoryScore" ].ToString(), out var result ) ) { return result; }
					}
				}
				catch ( ManagementException exception ) { exception.Log(); }

				return null;
			}

			public Object TimeTaken() {
				try {
					foreach ( var queryObj in this.Searcher.Value.Get().OfType<ManagementObject>() ) { return queryObj[ "TimeTaken" ]; }
				}
				catch ( ManagementException exception ) { exception.Log(); }

				return null;
			}

			public Int32? WinSAT_AssessmentState() {
				try {
					foreach ( var queryObj in this.Searcher.Value.Get().OfType<ManagementObject>() ) {
						if ( Int32.TryParse( queryObj[ "WinSATAssessmentState" ].ToString(), out var result ) ) { return result; }
					}
				}
				catch ( ManagementException exception ) { exception.Log(); }

				return null;
			}

			public Single? WinSPRLevel() {
				try {
					foreach ( var queryObj in this.Searcher.Value.Get().OfType<ManagementObject>() ) {
						if ( Single.TryParse( queryObj[ "WinSPRLevel" ].ToString(), out var result ) ) { return result; }
					}
				}
				catch ( ManagementException exception ) { exception.Log(); }

				return null;
			}
		}
	}
}