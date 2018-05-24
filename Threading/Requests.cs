// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Requests.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/Requests.cs" was last formatted by Protiguous on 2018/05/21 at 10:56 PM.

namespace Librainian.Threading {

    using System;
    using System.Threading;

    /// <summary>
    ///     Just a wrapper around <see cref="Interlocked" /> for counting requests.
    /// </summary>
    public class Requests {

        private Int64 _requests;

        public Boolean Any() => Interlocked.Read( ref this._requests ) > 0;

        public Int64 Count() => Interlocked.Read( ref this._requests );

        public Int64 MakeRequest( Int64 amount = 1 ) => Interlocked.Add( ref this._requests, amount );

        public Int64 UndoRequest( Int64 amount = 1 ) => Interlocked.Add( ref this._requests, -amount );
    }
}