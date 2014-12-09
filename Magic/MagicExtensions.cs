#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/MagicExtensions.cs" was last cleaned by Rick on 2014/10/21 at 10:36 PM
#endregion

namespace Librainian.Magic {
    using System;
    using System.Reactive;
    using System.Reactive.Linq;

    /// <summary>
    ///     <para>Any sufficiently advanced technology is indistinguishable from magic.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Clarke's_three_laws" />
    public static class MagicExtensions {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observable"></param>
        /// <returns></returns>
        /// <seealso cref="http://haacked.com/archive/2012/10/08/writing-a-continueafter-method-for-rx.aspx/"/>
        public static IObservable< Unit > AsCompletion< T >( this IObservable< T > observable ) => Observable.Create< Unit >( observer => {
                                                                                                                                  Action onCompleted = () => {
                                                                                                                                                           observer.OnNext( Unit.Default );
                                                                                                                                                           observer.OnCompleted();
                                                                                                                                                       };
                                                                                                                                  return observable.Subscribe( _ => { }, observer.OnError, onCompleted );
                                                                                                                              } );

        public static IObservable< TRet > ContinueAfter< T, TRet >( this IObservable< T > observable, Func< IObservable< TRet > > selector ) => observable.AsCompletion().SelectMany( _ => selector() );
    }
}
