namespace Librainian.Internet {
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public class EAPCommon {
        internal static void HandleCompletion< T >( TaskCompletionSource< T > tcs, AsyncCompletedEventArgs e, Func< T > getResult, Action unregisterHandler ) {
            // Transfers the results from the AsyncCompletedEventArgs and getResult() to the
            // TaskCompletionSource, but only AsyncCompletedEventArg's UserState matches the TCS
            // (this check is important if the same WebClient is used for multiple, asynchronous
            // operations concurrently).  Also unregisters the handler to avoid a leak.
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
            unregisterHandler();
        }
    }
}