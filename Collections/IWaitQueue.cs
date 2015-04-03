#region License & Information

// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// "Librainian/IWaitQueue.cs" was last cleaned by Rick on 2014/08/11 at 12:36 AM

#endregion License & Information

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Threading;

    /// <summary>
    /// Interface for internal queue classes for semaphores, etc. Relies on implementations to
    /// actually implement queue mechanics.
    /// NOTE: this interface is NOT present in java.util.concurrent.
    /// </summary>
    /// <author>Dawid Kurzyniec</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <changes><list><item>Renamed Insert to Enqueue</item><item>Renamed Extract to Dequeue</item></list></changes>
    public interface IWaitQueue {

        Boolean HasNodes { get; }

        int Length { get; }

        ICollection<Thread> WaitingThreads { get; }

        WaitNode Dequeue( );

        void Enqueue( WaitNode w );

        Boolean IsWaiting( Thread thread );

        // assumed not to block

        // should return null if empty
        // In backport 3.1 but not used.
        //void PutBack(WaitNode w);
    }
}