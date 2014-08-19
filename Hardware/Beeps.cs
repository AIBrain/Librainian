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
// "Librainian/Beeps.cs" was last cleaned by Rick on 2014/08/19 at 1:26 PM
#endregion

namespace Librainian.Hardware {
    using System;
    using Threading;

    public static class Beeps {
        public static void Low() {
            Console.Beep( frequency: 440, duration: ( int ) Threads.GetSlicingAverage().TotalMilliseconds );
        }

        public static void High() {
            Console.Beep( frequency: 14917, duration: ( int ) Threads.GetSlicingAverage().TotalMilliseconds );
        }
    }
}
