// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "IWaitQueue.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/IWaitQueue.cs" was last formatted by Protiguous on 2018/05/21 at 10:50 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Threading;

    /// <summary>
    ///     Interface for internal queue classes for semaphores, etc. Relies on implementations to actually implement queue
    ///     mechanics.
    ///     NOTE: this interface is NOT present in java.util.concurrent.
    /// </summary>
    /// <author>Dawid Kurzyniec</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <changes>
    ///     <list>
    ///         <item>Renamed Insert to Enqueue</item>
    ///         <item>Renamed Extract to Dequeue</item>
    ///     </list>
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