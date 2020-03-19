// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Computer.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "Computer.cs" was last formatted by Protiguous on 2020/03/16 at 3:10 PM.

namespace Librainian.OperatingSystem {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Numerics;
    using System.Runtime;
    using JetBrains.Annotations;
    using Maths;
    using Measurement.Time;

    public static class Computer {

        public static void AbortShutdown() => Process.Start( "shutdown", "/a" );

        public static Boolean CanAllocateMemory( this BigInteger bytes ) {
            try {
                var megabytes = bytes / MathConstants.Sizes.OneMegaByte;

                if ( megabytes <= BigInteger.Zero ) {
                    return true;
                }

                if ( megabytes > Int32.MaxValue ) {
                    megabytes = Int32.MaxValue;
                }

                // ReSharper disable once UnusedVariable
                using ( var memoryFailPoint = new MemoryFailPoint( ( Int32 ) megabytes ) ) {
                    return true;
                }
            }
            catch ( ArgumentOutOfRangeException ) {
                return false;
            }
            catch ( InsufficientMemoryException ) {
                return false;
            }
            catch ( OutOfMemoryException ) {
                return false;
            }
        }

        public static Boolean CanAllocateMemory( this UInt64 bytes ) => ( ( BigInteger ) bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this Int64 bytes ) => ( ( BigInteger ) bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this Int32 bytes ) => ( ( BigInteger ) bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this UInt32 bytes ) => ( ( BigInteger ) bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this Int16 bytes ) => ( ( BigInteger ) bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this UInt16 bytes ) => ( ( BigInteger ) bytes ).CanAllocateMemory();

        public static Boolean CanAllocateMemory( this Byte bytes ) => ( ( BigInteger ) bytes ).CanAllocateMemory();

        [NotNull]
        public static IEnumerable<String> GetVersions() =>
            AppDomain.CurrentDomain.GetAssemblies().Select( assembly => $"Assembly: {assembly.GetName().Name}, {assembly.GetName().Version}" );

        [NotNull]
        public static IEnumerable<String> GetWorkingMacAddresses() =>
            from nic in NetworkInterface.GetAllNetworkInterfaces() where nic.OperationalStatus == OperationalStatus.Up select nic.GetPhysicalAddress().ToString();

        public static void Hibernate( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/h" : $"/h /t {( Int32 ) delay.Value.TotalSeconds}" );

        public static void Logoff( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/l" : $"/l /t {( Int32 ) delay.Value.TotalSeconds}" );

        /// <summary>Send a reboot request.</summary>
        public static void Restart( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/r" : $"/r /t {( Int32 ) delay.Value.TotalSeconds}" );

        public static void RestartFast( TimeSpan? delay = null ) =>
            Process.Start( "shutdown", !delay.HasValue ? "/hybrid /s" : $"/hybrid /s /t {( Int32 ) delay.Value.TotalSeconds}" );

        public static void Shutdown( TimeSpan? delay = null ) => Process.Start( "shutdown", !delay.HasValue ? "/s" : $"/s /t {( Int32 ) delay.Value.TotalSeconds}" );

        /// <summary>Turn off the local computer with no time-out or warning.</summary>
        public static void ShutdownNow() => Process.Start( "shutdown", "/p" );

        public static class Beep {

            public static void At( Int32 frequency, TimeSpan duration ) => Console.Beep( frequency, ( Int32 ) duration.TotalMilliseconds );

            public static void High() => At( 14917, TimeExtensions.GetTimePrecision() );

            public static void Low() => At( 440, TimeExtensions.GetTimePrecision() );

        }

    }

}