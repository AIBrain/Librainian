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
// Usage of the source code or compiled binaries is AS-IS.I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/Requests.cs" was last cleaned by Rick on 2015/09/15 at 2:27 PM

namespace Librainian.Threading {

    using System;
    using System.Threading;

    /// <summary>
    /// Just a wrapper around <see cref="Interlocked"/> for counting requests.
    /// </summary>
    public class Requests {

        private Int64 _requests;

        public Int64 MakeRequest( Int64 amount = 1 ) {
            return Interlocked.Add( ref _requests, amount );
        }

        public Int64 UndoRequest( Int64 amount = 1 ) {
            return Interlocked.Add( ref _requests, -amount );
        }

        public Int64 Count() {
            return Interlocked.Read( ref _requests );
        }

        public Boolean Any() {
            return Interlocked.Read( ref _requests ) > 0;
        }

    }

}
