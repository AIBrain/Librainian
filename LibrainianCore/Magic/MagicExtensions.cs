// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "MagicExtensions.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "MagicExtensions.cs" was last formatted by Protiguous on 2020/01/31 at 12:31 AM.

namespace LibrainianCore.Magic {

    using System;
    using JetBrains.Annotations;

    /// <summary>
    ///     <para>Any sufficiently advanced technology is indistinguishable from magic.</para>
    /// </summary>
    /// <see cref="http://wikipedia.org/wiki/Clarke's_three_laws" />
    public static class MagicExtensions {

        /*

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observable"></param>
        /// <returns></returns>
        /// <see cref="http://haacked.com/archive/2012/10/08/writing-a-continueafter-method-for-rx.aspx/" />
        [NotNull]
        public static IObservable<Unit> AsCompletion<T>( this IObservable<T> observable ) =>
            Observable.Create<Unit>( observer => observable.Subscribe( _ => { }, onError: observer.OnError, onCompleted: () => {
                observer.OnNext( Unit.Default );
                observer.OnCompleted();
            } ) );
        */

        //[NotNull]public static IObservable<TRet> ContinueAfter<T, TRet>( this IObservable<T> observable, Func<IObservable<TRet>> selector ) => observable.AsCompletion().SelectMany( _ => selector() );

        /*

        /// <summary>
        ///     http://stackoverflow.com/a/7642198
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pauser"></param>
        /// <returns></returns>
        [NotNull]
        public static IObservable<T> Pausable<T>( this IObservable<T> source, IObservable<Boolean> pauser ) =>
            Observable.Create<T>( o => {
                var paused = new SerialDisposable();

                var subscription = source.Publish( ps => {
                    var values = new ReplaySubject<T>();

                    IObservable<T> Switcher( Boolean b ) {
                        if ( !b ) { return values.Concat( second: ps ); }

                        values.Dispose();
                        values = new ReplaySubject<T>();
                        paused.Disposable = ps.Subscribe( observer: values );

                        return Observable.Empty<T>();
                    }

                    return pauser.StartWith( false ).DistinctUntilChanged().Select( selector: Switcher ).Switch();
                } ).Subscribe( observer: o );

                return new CompositeDisposable( subscription, paused );
            } );
        */

        /// <summary>If <paramref name="b" /> is true then perform the <paramref name="action" />.</summary>
        /// <param name="b"></param>
        /// <param name="action"></param>
        public static void Then( this Boolean b, [NotNull] Action action ) {
            if ( action is null ) {
                throw new ArgumentNullException( nameof( action ) );
            }

            if ( b ) {
                action.Invoke();
            }
        }

        /// <summary></summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>

        // ReSharper disable once UnusedParameter.Local
        public static void ThrowIfNull<TKey>( [CanBeNull] this TKey obj ) {
            if ( obj is null ) {
                throw new ArgumentNullException();
            }
        }
    }
}