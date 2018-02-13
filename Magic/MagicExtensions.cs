// Copyright 2017 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/MagicExtensions.cs" was last cleaned by Rick on 2017/10/20 at 2:06 AM

namespace Librainian.Magic {

	using System;
	using System.Reactive;
	using System.Reactive.Disposables;
	using System.Reactive.Linq;
	using System.Reactive.Subjects;
	using JetBrains.Annotations;

	/// <summary>
	///     <para>Any sufficiently advanced technology is indistinguishable from magic.</para>
	/// </summary>
	/// <seealso cref="http://wikipedia.org/wiki/Clarke's_three_laws" />
	public static class MagicExtensions {

		/// <summary></summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="observable"></param>
		/// <returns></returns>
		/// <seealso cref="http://haacked.com/archive/2012/10/08/writing-a-continueafter-method-for-rx.aspx/" />
		public static IObservable<Unit> AsCompletion<T>( this IObservable<T> observable ) => Observable.Create<Unit>( observer => {
			return observable.Subscribe( _ => { }, onError: observer.OnError, onCompleted: () => {
				observer.OnNext( value: Unit.Default );
				observer.OnCompleted();
			} );
		} );

		public static IObservable<TRet> ContinueAfter<T, TRet>( this IObservable<T> observable, Func<IObservable<TRet>> selector ) => observable.AsCompletion().SelectMany( _ => selector() );

		/// <summary>
		///     http://stackoverflow.com/a/7642198
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="pauser"></param>
		/// <returns></returns>
		public static IObservable<T> Pausable<T>( this IObservable<T> source, IObservable<Boolean> pauser ) => Observable.Create<T>( o => {
			var paused = new SerialDisposable();
			var subscription = source.Publish( ps => {
				var values = new ReplaySubject<T>();

				IObservable<T> Switcher( Boolean b ) {
					if ( !b ) {
						return values.Concat( second: ps );
					}

					values.Dispose();
					values = new ReplaySubject<T>();
					paused.Disposable = ps.Subscribe( observer: values );
					return Observable.Empty<T>();
				}

				return pauser.StartWith( false ).DistinctUntilChanged().Select( selector: Switcher ).Switch();
			} ).Subscribe( observer: o );
			return new CompositeDisposable( subscription, paused );
		} );

		/// <summary>
		///     If <paramref name="b" /> is <see cref="Boolean.True" /> then perform the <paramref name="action" />.
		/// </summary>
		/// <param name="b"></param>
		/// <param name="action"></param>
		public static void Then( this Boolean b, Action action ) {
			if ( b ) {
				action?.Invoke();
			}
		}

		/// <summary></summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="obj"></param>
		// ReSharper disable once UnusedParameter.Local
		public static void ThrowIfNull<TKey>( [CanBeNull] this TKey obj ) {
			if ( null == obj ) {
				throw new ArgumentNullException();
			}
		}

	}

}
