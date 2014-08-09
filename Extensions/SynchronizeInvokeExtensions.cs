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
// "Librainian2/SynchronizeInvokeExtensions.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM

#endregion License & Information

namespace Librainian.Extensions {

    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Threading;

    public static class SynchronizeInvokeExtensions {

        public static void InvokeA<T>( this T invokable, Action<T> action, T argument = default( T ) ) where T : ISynchronizeInvoke {
            try {
                if ( Equals( invokable, default( T ) ) ) {
                    return;
                }
                if ( invokable is Control && ( invokable as Control ).IsDisposed ) {
                    return;
                }
                if ( invokable.InvokeRequired ) {
                    invokable.Invoke( action, new object[] { argument } );
                }
                else {
                    action( argument );
                }
            }
            catch ( ObjectDisposedException exception ) {
                exception.Log();
            }
        }

        public static T InvokeF<T>( this T invokable, Func<T> function, T argument = default( T ) ) where T : class, ISynchronizeInvoke {
            if ( invokable.InvokeRequired ) {
                if ( invokable is Control && ( invokable as Control ).IsDisposed ) { }
                else {
                    return invokable.Invoke( function, new object[] { argument } ) as T;
                }
            }
            return function();
        }
    }
}