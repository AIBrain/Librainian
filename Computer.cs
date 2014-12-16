#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Computer.cs" was last cleaned by Rick on 2014/08/19 at 1:26 PM
#endregion

namespace Librainian {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Numerics;
    using System.Runtime;
    using JetBrains.Annotations;
    using Maths;
    using Microsoft.VisualBasic.Devices;
    using Threading;

    public static class Computer {

        public static class Beep {
            public static void Low() => At( 440, Threads.GetSlicingAverage() );

            public static void At( int frequency, TimeSpan duration ) => Console.Beep( frequency: frequency, duration: ( int )duration.TotalMilliseconds );

            public static void High() => At( 14917, Threads.GetSlicingAverage() );
        }

        /// <summary>
        ///     Provides properties for getting information about the computer's memory, loaded assemblies, name, and operating
        ///     system.
        /// </summary>
        // TODO how slow is this class?
        [NotNull]
        public static ComputerInfo Info => new ComputerInfo();

        /// <summary>
        ///     //TODO description. Bytes?
        ///     Which one does .NET allocate objects in..? Sum, or smaller of the two?
        ///     Is this real time?
        ///     How fast/slow is this method?
        /// </summary>
        /// <returns></returns>
        public static BigInteger GetAvailableMemeory() {
            var info = Info;
            var physical = info.AvailablePhysicalMemory;
            var virtl = info.AvailableVirtualMemory;
            return physical <= virtl ? physical : virtl;
        }

        /*
                /// <summary>
                /// </summary>
                /// <remarks>
                ///     http://msdn.microsoft.com/en-us/library/aa394347(VS.85).aspx
                /// </remarks>
                /// <returns></returns>
                public static String RAM {
                    get {
                        try {
                            using ( var searcher = new ManagementObjectSearcher( "Select * from Win32_PhysicalMemory" ) ) {
                                ulong total = 0;

                                var sb = new StringBuilder();
                                foreach ( var result in searcher.Get() ) {
                                    var mem = ( ulong )result[ "Capacity" ];
                                    total += mem;
                                    sb.AppendFormat( "{0}:{1}MB, ", result[ "DeviceLocator" ], mem / 1024 / 1024 );
                                }
                                sb.AppendFormat( " Total: {0:0,000}MB", total / 1024 / 1024 );
                                return sb.ToString();
                            }
                        }
                        catch ( Exception ex ) {
                            return String.Format( "WMI Error: {0}", ex.Message );
                        }
                    }
                }
        */

        /*
                /// <summary>
                ///     Use WMI (System.Managment) to get the CPU type
                /// </summary>
                /// <remarks>
                ///     http://msdn2.microsoft.com/en-us/library/aa394373(VS.85).aspx
                /// </remarks>
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
                        return String.Format( "WMI Error: {0}", ex.Message );
                    }
                }
        */

        /*
                /// <summary>
                /// </summary>
                /// <remarks>
                ///     http://msdn.microsoft.com/en-us/library/aa394347(VS.85).aspx
                /// </remarks>
                /// <value></value>
                public static ulong TotalPhysicalMemory {
                    get {
                        try {
                            return ComputerInfo.TotalPhysicalMemory;
                            //using ( var searcher = new ManagementObjectSearcher( "Select * from Win32_PhysicalMemory" ) ) {
                            //    var total = searcher.Get().Cast< ManagementBaseObject >().Select( baseObject => ( ulong ) baseObject[ "Capacity" ] ).Aggregate( 0UL, ( current, mem ) => current + mem );
                            //    return total;
                            //}
                        }
                        catch ( Exception exception ) {
                            exception.Error();
                            return 0;
                        }
                    }
                }
        */

        /*
                /// <summary>
                /// </summary>
                /// <remarks>
                ///     http://msdn.microsoft.com/en-us/library/aa394347(VS.85).aspx
                /// </remarks>
                /// <returns></returns>
                public static UInt64 GetAvailableVirtualMemory() {
                    try {
                        using ( var searcher = new ManagementObjectSearcher( "Select * from Win32_LogicalMemoryConfiguration" ) ) {
                            var total = searcher.Get().Cast<ManagementBaseObject>().Select( baseObject => ( UInt64 )baseObject[ "AvailableVirtualMemory" ] ).Aggregate( 0UL, ( current, mem ) => current + mem );
                            return total;
                        }
                    }
                    catch ( Exception exception ) {
                        exception.Error();
                        return 0;
                    }
                }
        */

        public static IEnumerable< string > GetVersions() => AppDomain.CurrentDomain.GetAssemblies().Select( assembly => String.Format( "Assembly: {0}, {1}", assembly.GetName().Name, assembly.GetName().Version ) );

        public static Boolean CanAllocateMemory( this BigInteger bytes ) {
            try {
                if ( bytes <= 1 ) {
                    return true;
                }
                var megabytes = bytes / MathExtensions.OneMegaByteBI;
                if ( megabytes <= BigInteger.Zero ) {
                    return true;
                }
                if ( megabytes > Int32.MaxValue ) {
                    megabytes = Int32.MaxValue;
                }
                if ( bytes > GetAvailableMemeory() ) {
                    GC.Collect();
                }
                using ( new MemoryFailPoint( ( int )megabytes ) ) {
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

        public static IEnumerable< string > GetWorkingMACAddresses() => from nic in NetworkInterface.GetAllNetworkInterfaces()
                                                                        where nic.OperationalStatus == OperationalStatus.Up
                                                                        select nic.GetPhysicalAddress().ToString();

        private static List< PerformanceCounter > utilizationCounters;

        private static void InitCounters() {
            // Initialize the list to a counter-per-processor:
            utilizationCounters = new List< PerformanceCounter >();
            for ( var i = 0; i < Environment.ProcessorCount; i++ ) {
                utilizationCounters.Add( new PerformanceCounter( "Processor", "% Processor Time", i.ToString() ) );
            }
        }

        private static int GetFreeProcessors() => utilizationCounters.Count( pc => pc.NextValue() < 0.80f );
    }
}
