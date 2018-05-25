// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "EAPCommon.cs" belongs to Rick@AIBrain.org and
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/EAPCommon.cs" was last formatted by Protiguous on 2018/05/24 at 7:14 PM.

namespace Librainian.Internet {

    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public class EapCommon {

        internal static void HandleCompletion<T>( TaskCompletionSource<T> tcs, AsyncCompletedEventArgs e, Func<T> getResult, Action unregisterHandler ) {

            // Transfers the results from the AsyncCompletedEventArgs and getResult() to the
            // TaskCompletionSource, but only AsyncCompletedEventArg's UserState matches the TCS
            // (this check is important if the same WebClient is used for multiple, asynchronous
            // operations concurrently). Also unregisters the handler to avoid a leak.
            try {
                if ( e.UserState != tcs ) { return; }

                if ( e.Cancelled ) { tcs.TrySetCanceled(); }
                else if ( e.Error != null ) { tcs.TrySetException( e.Error ); }
                else { tcs.TrySetResult( getResult() ); }
            }
            finally { unregisterHandler(); }
        }
    }
}