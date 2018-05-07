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
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/EAPCommon.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

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
                if ( e.UserState != tcs ) {
                    return;
                }
                if ( e.Cancelled ) {
                    tcs.TrySetCanceled();
                }
                else if ( e.Error != null ) {
                    tcs.TrySetException( e.Error );
                }
                else {
                    tcs.TrySetResult( getResult() );
                }
            }
            finally {
                unregisterHandler();
            }
        }
    }
}