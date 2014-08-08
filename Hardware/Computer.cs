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
// "Librainian2/Computer.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Hardware {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using System.Text;
    using Threading;

    public static class Computer {
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
                            var mem = ( ulong ) result[ "Capacity" ];
                            total += mem;
                            sb.AppendFormat( "{0}:{1}MB, ", result[ "DeviceLocator" ], mem/1024/1024 );
                        }
                        sb.AppendFormat( " Total: {0:0,000}MB", total/1024/1024 );
                        return sb.ToString();
                    }
                }
                catch ( Exception ex ) {
                    return String.Format( "WMI Error: {0}", ex.Message );
                }
            }
        }

        /// <summary>
        ///     Use WMI (System.Managment) to get the CPU type
        /// </summary>
        /// <remarks>
        ///     http://msdn2.microsoft.com/en-us/library/aa394373(VS.85).aspx
        /// </remarks>
        /// <returns></returns>
        public static string GetCPUDescription() {
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

        /// <summary>
        /// </summary>
        /// <remarks>
        ///     http://msdn.microsoft.com/en-us/library/aa394347(VS.85).aspx
        /// </remarks>
        /// <returns></returns>
        public static ulong GetRAM() {
            try {
                using ( var searcher = new ManagementObjectSearcher( "Select * from Win32_PhysicalMemory" ) ) {
                    var total = searcher.Get().Cast< ManagementBaseObject >().Select( baseObject => ( ulong ) baseObject[ "Capacity" ] ).Aggregate< ulong, ulong >( 0, ( current, mem ) => current + mem );
                    return total;
                }
            }
            catch ( Exception exception ) {
                exception.Log();
                return 0;
            }
        }

        public static IEnumerable< String > GetVersions() {
            return AppDomain.CurrentDomain.GetAssemblies().Select( assembly => String.Format( "Assembly: {0}, {1}", assembly.GetName().Name, assembly.GetName().Version ) );
        }
    }
}
