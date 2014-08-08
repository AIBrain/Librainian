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
// "Librainian2/ObservableDrainExtensions.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM

#endregion License & Information

namespace Librainian.Extensions {

    //using System;
    //using System.Reactive;
    //using System.Reactive.Linq;
    //using System.Reactive.Subjects;

    public static class ObservableDrainExtensions {

        //public static IObservable<TOut> Drain<TSource, TOut>( this IObservable<TSource> source, Func<TSource, IObservable<TOut>> selector ) {
        //    return Observable.Defer( () => {
        //        var queue = new BehaviorSubject<Unit>( new Unit() );

        //        return source
        //            .Zip( queue, ( source1, unit ) => source1 )
        //            .SelectMany( source1 => selector( source1 )
        //                             .Do( _ => { }, () => queue.OnNext( new Unit() ) )
        //            );
        //    } );
        //}
    }
}