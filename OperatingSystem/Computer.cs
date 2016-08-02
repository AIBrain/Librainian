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
// "Librainian/Computer.cs" was last cleaned by Rick on 2016/06/18 at 10:55 PM

namespace Librainian.OperatingSystem {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Management;
    using System.Net.NetworkInformation;
    using System.Numerics;
    using System.Runtime;
    using System.Runtime.InteropServices;
    using System.Text;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Maths;
    using Measurement.Time;
    using Microsoft.VisualBasic.Devices;

    public static class Computer {
        private static List<PerformanceCounter> _utilizationCounters;

        static Computer() {
            InitCounters();
        }

        /// <summary>
        /// </summary>
        /// <remarks>http: //msdn.microsoft.com/en-us/Library/aa394347(VS.85).aspx</remarks>
        /// <returns></returns>
        public static String RAM {
            get {
                try {
                    using ( var searcher = new ManagementObjectSearcher( "Select * from Win32_PhysicalMemory" ) ) {
                        UInt64 total = 0;

                        var sb = new StringBuilder();
                        foreach ( var result in searcher.Get() ) {
                            var mem = ( UInt64 )result[ "Capacity" ];
                            total += mem;
                            sb.AppendFormat( "{0}:{1}MB, ", result[ "DeviceLocator" ], mem / 1024 / 1024 );
                        }
                        sb.AppendFormat( " Total: {0:0,000}MB", total / 1024 / 1024 );
                        return sb.ToString();
                    }
                }
                catch ( Exception ex ) {
                    return $"WMI Error: {ex.Message}";
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/Library/aa394347(VS.85).aspx" />
        public static UInt64 TotalPhysicalMemory {
            get {
                try {
                    return Info.TotalPhysicalMemory;
                }
                catch ( Exception exception ) {
                    exception.More();
                    return 0;
                }
            }
        }

        /// <summary>
        ///     Provides properties for getting information about the computer's memory, loaded
        ///     assemblies, name, and operating system.
        /// </summary>
        // BUG how slow is this class?
        [NotNull]
        public static ComputerInfo Info => new ComputerInfo();

        private static ManagementObjectSearcher PerfFormattedDataManagementObjectSearcher { get; } = new ManagementObjectSearcher( "select * from Win32_PerfFormattedData_PerfOS_Processor" );

        public static void AbortShutdown() {
            Process.Start( "shutdown", "/a" );
        }

        public static Boolean CanAllocateMemory( this BigInteger bytes ) {
            try {
                if ( bytes <= 1 ) {
                    return true;
                }
                var megabytes = bytes / MathConstants.OneMegaByteBi;

                if ( megabytes <= BigInteger.Zero ) {
                    return true;
                }

                if ( megabytes > Int32.MaxValue ) {
                    megabytes = Int32.MaxValue;
                }

                // ReSharper disable once UnusedVariable
                using ( var memoryFailPoint = new MemoryFailPoint( ( Int32 )megabytes ) ) {
                    return true;
                }
            }
            catch ( ArgumentOutOfRangeException ) {
                return false;
            }
            catch ( OutOfMemoryException ) {
                return false;
            }
        }

        public static Boolean CanAllocateMemory( this UInt64 bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this Int64 bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this Int32 bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this UInt32 bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this Int16 bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this UInt16 bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this Byte bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        /// <summary>
        ///     //TODO description. Bytes? Which one does .NET allocate objects in..? Sum, or smaller of
        ///     the two? Is this real time? How fast/slow is this method?
        /// </summary>
        /// <returns></returns>
        public static BigInteger GetAvailableMemeory() {
            var info = Info;
            var physical = info.AvailablePhysicalMemory;
            var virtl = info.AvailableVirtualMemory;
            return Math.Min( physical, virtl );
        }

        /// <summary>
        /// </summary>
        /// <remarks>http: //msdn.microsoft.com/en-us/Library/aa394347(VS.85).aspx</remarks>
        /// <returns></returns>
        public static UInt64 GetAvailableVirtualMemory() {
            try {
                using ( var searcher = new ManagementObjectSearcher( "Select * from Win32_LogicalMemoryConfiguration" ) ) {
                    var total = searcher.Get().Cast<ManagementBaseObject>().Select( baseObject => ( UInt64 )baseObject[ "AvailableVirtualMemory" ] ).Aggregate( 0UL, ( current, mem ) => current + mem );
                    return total;
                }
            }
            catch ( Exception exception ) {
                exception.More();
                return 0;
            }
        }

        /// <summary>
        ///     Use WMI (System.Managment) to get the CPU type
        /// </summary>
        /// <remarks>http: //msdn2.microsoft.com/en-us/Library/aa394373(VS.85).aspx</remarks>
        /// <returns></returns>
        public static String GetCPUDescription() {
            try {
                using ( var searcher = new ManagementObjectSearcher( "Select * from Win32_Processor" ) ) {
                    var sb = new StringBuilder();
                    foreach ( var result in searcher.Get() ) {
                        sb.AppendFormat( "{0} with {1} cores", result[ "Name" ], result[ "NumberOfCores" ] );
                    }
                    return sb.ToString();
                }
            }
            catch ( Exception ex ) {
                return $"WMI Error: {ex.Message}";
            }
        }

        /// <summary>
        ///     Get the average percent of all cpu cores being used.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Single GetCPUUsage() {
            var cpuTimes = PerfFormattedDataManagementObjectSearcher.Get().Cast<ManagementObject>().Select( managementObject => new {
                Name = managementObject[ "Name" ],
                Usage = Convert.ToSingle( managementObject[ "PercentProcessorTime" ] ) / 100.0f
            } ); //.ToList();

            //The '_Total' value represents the average usage across all cores, and is the best representation of overall CPU usage
            var cpuUsage = cpuTimes.Where( x => x.Name.ToString() == "_Total" ).Select( x => x.Usage ).Single();
            cpuUsage.Should().BeGreaterOrEqualTo( 0 );
            cpuUsage.Should().BeLessOrEqualTo( 1 );
            return cpuUsage;
        }

        public static IEnumerable<String> GetVersions() => AppDomain.CurrentDomain.GetAssemblies().Select( assembly => $"Assembly: {assembly.GetName().Name}, {assembly.GetName().Version}" );

        public static IEnumerable<String> GetWorkingMacAddresses() => from nic in NetworkInterface.GetAllNetworkInterfaces() where nic.OperationalStatus == OperationalStatus.Up select nic.GetPhysicalAddress().ToString();

        public static void Hibernate( TimeSpan? delay = null ) {
            Process.Start( "shutdown", !delay.HasValue ? "/h" : $"/h /t {( Int32 )delay.Value.TotalSeconds}" );
        }

        [DllImport( "user32" )]
        public static extern void LockWorkStation();

        public static void Logoff( TimeSpan? delay = null ) {
            Process.Start( "shutdown", !delay.HasValue ? "/l" : $"/l /t {( Int32 )delay.Value.TotalSeconds}" );
        }

        /// <summary>
        ///     Send a reboot request.
        /// </summary>
        public static void Restart( TimeSpan? delay = null ) {
            Process.Start( "shutdown", !delay.HasValue ? "/r" : $"/r /t {( Int32 )delay.Value.TotalSeconds}" );
        }

        public static void RestartFast( TimeSpan? delay = null ) {
            Process.Start( "shutdown", !delay.HasValue ? "/hybrid /s" : $"/hybrid /s /t {( Int32 )delay.Value.TotalSeconds}" );
        }

        public static void Shutdown( TimeSpan? delay = null ) {
            Process.Start( "shutdown", !delay.HasValue ? "/s" : $"/s /t {( Int32 )delay.Value.TotalSeconds}" );
        }

        /// <summary>
        ///     Turn off the local computer with no time-out or warning.
        /// </summary>
        public static void ShutdownNow() {
            Process.Start( "shutdown", "/p" );
        }

        private static Int32 GetFreeProcessors() => _utilizationCounters.Count( pc => pc.NextValue() <= 0.50f );

        private static void InitCounters() {

            // Initialize the list to a counter-per-processor:
            _utilizationCounters = new List<PerformanceCounter>();
            for ( var i = 0; i < Environment.ProcessorCount; i++ ) {
                _utilizationCounters.Add( new PerformanceCounter( "Processor", "% Processor Time", i.ToString() ) );
            }
        }

        public static class Beep {

            public static void At( Int32 frequency, TimeSpan duration ) => Console.Beep( frequency: frequency, duration: ( Int32 )duration.TotalMilliseconds );

            public static void High() => At( 14917, TimeExtensions.GetAverageDateTimePrecision() );

            public static void Low() => At( 440, TimeExtensions.GetAverageDateTimePrecision() );
        }
    }
}