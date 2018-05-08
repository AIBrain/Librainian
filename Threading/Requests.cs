// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Requests.cs" was last cleaned by Protiguous on 2016/06/18 at 10:57 PM

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