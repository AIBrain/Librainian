// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Computer.cs",
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
// "Librainian/Librainian/Computer.cs" was last cleaned by Protiguous on 2018/05/15 at 10:48 PM.

namespace Librainian.OperatingSystem {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Management;
    using System.Net.NetworkInformation;
    using System.Numerics;
    using System.Runtime;
    using System.Text;
    using JetBrains.Annotations;
    using Maths;
    using Measurement.Time;
    using Microsoft.VisualBasic.Devices;
    using NUnit.Framework;

    public static class Computer {

        private static List<PerformanceCounter> _utilizationCounters;

        static Computer() => InitCounters();

        private static ManagementObjectSearcher PerfFormattedDataManagementObjectSearcher { get; } = new ManagementObjectSearcher( "select * from Win32_PerfFormattedData_PerfOS_Processor" );

        /// <summary>
        /// </summary>
        /// <remarks>http: //msdn.microsoft.com/en-us/Library/aa394347(VS.85).aspx</remarks>
        /// <returns></returns>
        public static UInt64 RAM {
            get {
                try {
                    using ( var searcher = new ManagementObjectSearcher( "Select * from Win32_PhysicalMemory" ) ) {
                        UInt64 total = 0;

                        foreach ( var result in searcher.Get() ) {
                            var mem = Convert.ToUInt64( result["Capacity"] );
                            total += mem;
                        }

                        return total;
                    }
                }
                catch ( Exception exception ) { exception.More(); }

                return 0;
            }
        }

        /// <summary>
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/Library/aa394347(VS.85).aspx" />
        public static UInt64 TotalPhysicalMemory {
            get {
                try { return Info().TotalPhysicalMemory; }
                catch ( Exception exception ) {
                    exception.More();

                    return 0;
                }
            }
        }

        private static Int32 GetFreeProcessors() => _utilizationCounters.Count( pc => pc.NextValue() <= 0.50f );

        private static void InitCounters() {

            // Initialize the list to a counter-per-processor:
            _utilizationCounters = new List<PerformanceCounter>();

            for ( var i = 0; i < Environment.ProcessorCount; i++ ) { _utilizationCounters.Add( new PerformanceCounter( "Processor", "% Processor Time", i.ToString() ) ); }
        }

        public static void AbortShutdown() => Process.Start( "shutdown", "/a" );

        public static Boolean CanAllocateMemory( this BigInteger bytes ) {
            try {
                var megabytes = bytes / Constants.Sizes.OneMegaByteBi;

                if ( megabytes <= BigInteger.Zero ) { return true; }

                if ( megabytes > Int32.MaxValue ) { megabytes = Int32.MaxValue; }

                // ReSharper disable once UnusedVariable
                using ( var memoryFailPoint = new MemoryFailPoint( ( Int32 )megabytes ) ) { return true; }
            }
            catch ( ArgumentOutOfRangeException ) { return false; }
            catch ( InsufficientMemoryException ) { return false; }
            catch ( OutOfMemoryException ) { return false; }
        }

        public static Boolean CanAllocateMemory( this UInt64 bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this Int64 bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this Int32 bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this UInt32 bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this Int16 bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this UInt16 bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this Byte bytes ) => ( ( BigInteger )bytes ).CanAllocateMemory();

        /// <summary>
        ///     //TODO description. Bytes? Which one does .NET allocate objects in..? Sum, or smaller of the two? Is this real
        ///     time? How fast/slow is this method?
        /// </summary>
        /// <returns></returns>
        public static UInt64 GetAvailableMemeory() {
            var info = Info();

            return Math.Min( info.AvailablePhysicalMemory, info.AvailableVirtualMemory );
        }

        /// <summary>
        /// </summary>
        /// <remarks>http: //msdn.microsoft.com/en-us/Library/aa394347(VS.85).aspx</remarks>
        /// <returns></returns>
        public static UInt64 GetAvailableVirtualMemory() {
            try {
                using ( var searcher = new ManagementObjectSearcher( "Select * from Win32_LogicalMemoryConfiguration" ) ) {
                    var total = searcher.Get().Cast<ManagementBaseObject>().Select( baseObject => ( UInt64 )baseObject["AvailableVirtualMemory"] ).Aggregate( 0UL, ( current, mem ) => current + mem );

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

                    foreach ( var result in searcher.Get() ) { sb.Append( $"{result["Name"]} with {result["NumberOfCores"]} cores" ); }

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
        public static Single GetCPUUsage() {
            var cpuTimes = PerfFormattedDataManagementObjectSearcher.Get().Cast<ManagementObject>()
                .Select( managementObject => new { Name = managementObject["Name"], Usage = Convert.ToSingle( managementObject["PercentProcessorTime"] ) / 100.0f } ); //.ToList();

            //The '_Total' value represents the average usage across all cores, and is the best representation of overall CPU usage
            var cpuUsage = cpuTimes.Where( x => x.Name.ToString() == "_Total" ).Select( x => x.Usage ).Single();
            cpuUsage.Should().BeGreaterOrEqualTo( 0 );
            cpuUsage.Should().BeLessOrEqualTo( 1 );

            return cpuUsage;
        }

        public static IEnumerable<String> GetVersions() => AppDomain.CurrentDomain.GetAssemblies().Select( assembly => $"Assembly: {assembly.GetName().Name}, {assembly.GetName().Version}" );

        public static IEnumerable<String> GetWorkingMacAddresses() => from nic in NetworkInterface.GetAllNetworkInterfaces() where nic.OperationalStatus == OperationalStatus.Up select nic.GetPhysicalAddress().ToString();

        public static void Hibernate( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/h" : $"/h /t {( Int32 )delay.Value.TotalSeconds}" );

        /// <summary>
        ///     Provides properties for getting information about the computer's memory, loaded assemblies, name, and operating
        ///     system.
        /// </summary>
        // TODO how slow is this class? The code behind it doesn't *look* slow..
        [NotNull]
        public static ComputerInfo Info() => new ComputerInfo();

        public static void Logoff( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/l" : $"/l /t {( Int32 )delay.Value.TotalSeconds}" );

        /// <summary>
        ///     Send a reboot request.
        /// </summary>
        public static void Restart( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/r" : $"/r /t {( Int32 )delay.Value.TotalSeconds}" );

        public static void RestartFast( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/hybrid /s" : $"/hybrid /s /t {( Int32 )delay.Value.TotalSeconds}" );

        public static void Shutdown( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/s" : $"/s /t {( Int32 )delay.Value.TotalSeconds}" );

        /// <summary>
        ///     Turn off the local computer with no time-out or warning.
        /// </summary>
        public static void ShutdownNow() => Process.Start( "shutdown", "/p" );

        public static class Beep {

            public static void At( Int32 frequency, TimeSpan duration ) => Console.Beep( frequency: frequency, duration: ( Int32 )duration.TotalMilliseconds );

            public static void High() => At( 14917, TimeExtensions.GetAverageDateTimePrecision() );

            public static void Low() => At( 440, TimeExtensions.GetAverageDateTimePrecision() );
        }

        public static class SATScores {

            public static Lazy<ManagementObjectSearcher> Searcher { get; } = new Lazy<ManagementObjectSearcher>( () => new ManagementObjectSearcher( "root\\CIMV2", "SELECT * FROM Win32_WinSAT" ) );

            public static Single? CPU() {
                try {
                    foreach ( var queryObj in Searcher.Value.Get().OfType<ManagementObject>() ) {
                        if ( Single.TryParse( queryObj["CPUScore"].ToString(), out var result ) ) { return result; }
                    }
                }
                catch ( ManagementException exception ) { exception.More(); }

                return null;
            }

            public static Single? D3D() {
                try {
                    foreach ( var queryObj in Searcher.Value.Get().OfType<ManagementObject>() ) {
                        if ( Single.TryParse( queryObj["D3DScore"].ToString(), out var result ) ) { return result; }
                    }
                }
                catch ( ManagementException exception ) { exception.More(); }

                return null;
            }

            public static Single? Disk() {
                try {
                    foreach ( var queryObj in Searcher.Value.Get().OfType<ManagementObject>() ) {
                        if ( Single.TryParse( queryObj["DiskScore"].ToString(), out var result ) ) { return result; }
                    }
                }
                catch ( ManagementException exception ) { exception.More(); }

                return null;
            }

            public static Single? Graphics() {
                try {
                    foreach ( var queryObj in Searcher.Value.Get().OfType<ManagementObject>() ) {
                        if ( Single.TryParse( queryObj["GraphicsScore"].ToString(), out var result ) ) { return result; }
                    }
                }
                catch ( ManagementException exception ) { exception.More(); }

                return null;
            }

            public static Single? Memory() {
                try {
                    foreach ( var queryObj in Searcher.Value.Get().OfType<ManagementObject>() ) {
                        if ( Single.TryParse( queryObj["MemoryScore"].ToString(), out var result ) ) { return result; }
                    }
                }
                catch ( ManagementException exception ) { exception.More(); }

                return null;
            }

            public static Object TimeTaken() {
                try {
                    foreach ( var queryObj in Searcher.Value.Get().OfType<ManagementObject>() ) { return queryObj["TimeTaken"]; }
                }
                catch ( ManagementException exception ) { exception.More(); }

                return null;
            }

            public static Int32? WinSAT_AssessmentState() {
                try {
                    foreach ( var queryObj in Searcher.Value.Get().OfType<ManagementObject>() ) {
                        if ( Int32.TryParse( queryObj["WinSATAssessmentState"].ToString(), out var result ) ) { return result; }
                    }
                }
                catch ( ManagementException exception ) { exception.More(); }

                return null;
            }

            public static Single? WinSPRLevel() {
                try {
                    foreach ( var queryObj in Searcher.Value.Get().OfType<ManagementObject>() ) {
                        if ( Single.TryParse( queryObj["WinSPRLevel"].ToString(), out var result ) ) { return result; }
                    }
                }
                catch ( ManagementException exception ) { exception.More(); }

                return null;
            }
        }
    }

    [TestFixture]
    public static class ComputerTests {

        [Test]
        public static void TestScores() {
            Debug.WriteLine( Computer.SATScores.CPU() );
            Debug.WriteLine( Computer.SATScores.D3D() );
            Debug.WriteLine( Computer.SATScores.Disk() );
            Debug.WriteLine( Computer.SATScores.Graphics() );
            Debug.WriteLine( Computer.SATScores.Memory() );
            Debug.WriteLine( Computer.SATScores.TimeTaken() );
            Debug.WriteLine( Computer.SATScores.WinSAT_AssessmentState() );
            Debug.WriteLine( Computer.SATScores.WinSPRLevel() );
        }
    }
}