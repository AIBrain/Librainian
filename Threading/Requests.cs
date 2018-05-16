// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "Requests.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license has
// been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Requests.cs" was last cleaned by Protiguous on 2018/05/15 at 4:23 AM.

namespace Librainian.Threading {

    using System;
    using System.Threading;

    /// <summary>
    /// Just a wrapper around <see cref="Interlocked"/> for counting requests.
    /// </summary>
    public class Requests {

        private Int64 _requests;

        public Boolean Any() => Interlocked.Read( ref this._requests ) > 0;

        public Int64 Count() => Interlocked.Read( ref this._requests );

        public Int64 MakeRequest( Int64 amount = 1 ) => Interlocked.Add( ref this._requests, amount );

        public Int64 UndoRequest( Int64 amount = 1 ) => Interlocked.Add( ref this._requests, -amount );
    }
}