// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/IWaitQueue.cs" was last cleaned by Protiguous on 2018/05/12 at 1:19 AM

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Threading;

    /// <summary>
    /// Interface for internal queue classes for semaphores, etc. Relies on implementations to actually implement queue mechanics.
    /// NOTE: this interface is NOT present in java.util.concurrent.
    /// </summary>
    /// <author>Dawid Kurzyniec</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <changes>
    /// <list>
    /// <item>Renamed Insert to Enqueue</item>
    /// <item>Renamed Extract to Dequeue</item>
    /// </list>
    /// </changes>
    public interface IWaitQueue {

        Boolean HasNodes { get; }

        Int32 Length { get; }

        ICollection<Thread> WaitingThreads { get; }

        WaitNode Dequeue();

        void Enqueue( WaitNode w );

        Boolean IsWaiting( Thread thread );
    }
}