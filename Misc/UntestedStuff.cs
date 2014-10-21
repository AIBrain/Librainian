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
// "Librainian/UntestedStuff.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Misc {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Threading;

    internal class UntestedStuff {
        private static List< PerformanceCounter > utilizationCounters;

        //private int _partitions = Math.Min( data.Count, ( int ) Math.Max( 1.0f, ( float ) GetFreeProcessors() / ( 1 - blocking ) ) );

        private static void InitCounters() {
            // Initialize the list to a counter-per-processor:
            utilizationCounters = new List< PerformanceCounter >();
            for ( var i = 0; i < Environment.ProcessorCount; i++ ) {
                utilizationCounters.Add( new PerformanceCounter( "Processor", "% Processor Time", i.ToString() ) );
            }
        }

        private static int GetFreeProcessors() {
            return utilizationCounters.Count( pc => pc.NextValue() < 0.80f );
        }

        // ForAll<T> change...
    }
}
