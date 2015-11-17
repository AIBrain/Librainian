// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/Computer.cs" was last cleaned by Rick on 2015/11/13 at 11:30 PM

namespace Librainian.OperatingSystem {

    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public static class Computer {

        public static void AbortShutdown() {
            Process.Start( "shutdown", "/a" );
        }

        public static void Hibernate( TimeSpan? delay = null ) {
            Process.Start( "shutdown", !delay.HasValue ? "/h" : $"/h /t {( Int32 ) delay.Value.TotalSeconds}" );
        }

        [DllImport( "user32" )]
        public static extern void LockWorkStation();

        public static void Logoff( TimeSpan? delay = null ) {
            Process.Start( "shutdown", !delay.HasValue ? "/l" : $"/l /t {( Int32 ) delay.Value.TotalSeconds}" );
        }

        /// <summary>
        ///     Send a reboot request.
        /// </summary>
        public static void Restart( TimeSpan? delay = null ) {
            Process.Start( "shutdown", !delay.HasValue ? "/r" : $"/r /t {( Int32 ) delay.Value.TotalSeconds}" );
        }

        public static void RestartFast( TimeSpan? delay = null ) {
            Process.Start( "shutdown", !delay.HasValue ? "/hybrid /s" : $"/hybrid /s /t {( Int32 ) delay.Value.TotalSeconds}" );
        }

        public static void Shutdown( TimeSpan? delay = null ) {
            Process.Start( "shutdown", !delay.HasValue ? "/s" : $"/s /t {( Int32 ) delay.Value.TotalSeconds}" );
        }

        /// <summary>
        ///     Turn off the local computer with no time-out or warning.
        /// </summary>
        public static void ShutdownNow() {
            Process.Start( "shutdown", "/p" );
        }

    }

}
