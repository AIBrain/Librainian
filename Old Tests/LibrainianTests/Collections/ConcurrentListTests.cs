// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConcurrentListTests.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "LibrainianTests", "ConcurrentListTests.cs" was last formatted by Protiguous on 2019/11/20 at 6:33 AM.

namespace LibrainianTests.Collections {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Librainian.Collections.Lists;
    using Librainian.Maths;
    using Xunit;

    public static class ConcurrentListTests {

        /// <summary>Environment.ProcessorCount * 10240 = 163840</summary>
        private static Int32 Threads { get; } = Environment.ProcessorCount * 10240;

        private static Int32 AddManyProducts( [NotNull] ICollection<String> list ) {
            if ( list is null ) {
                throw new ArgumentNullException( paramName: nameof( list ) );
            }

            Parallel.For( 0, Threads, x => list.Add( item: $"name {x}" ) );

            return list.Count;
        }

        private static (Int32 added, Int32 removed) AddManyProductsRemoveRandom( [NotNull] ICollection<String> list ) {
            if ( list is null ) {
                throw new ArgumentNullException( paramName: nameof( list ) );
            }

            var added = AddManyProducts( list: list );
            var removed = 0;

            Parallel.For( 0, added, x => {
                if ( Randem.NextBooleanFast() ) {
                    list.Remove( item: $"name {x}" );
                    Interlocked.Increment( ref removed );
                }
            } );

            return (added, removed);
        }

        /*
        [Fact]
        public static void AddNameThreadSafetyTest() {
            var list = new ConcurrentList<String>();

            var threads = new List<Thread>( Threads );

            foreach ( var thread in 1.To( Threads )
                .Select( selector: i => new Thread( start: () => AddManyProducts( list: list ) ) ) ) {

                threads.Add( thread );
                thread.Start();
            }

            Parallel.ForEach( threads.AsParallel(), thread => thread?.Join() );

            //Assert.Equal( expected: Threads * Threads, actual: list.Count );
        }
        */

        /*
        [Fact]
        public static async Task TestAFew() {
            var cancel = new CancellationTokenSource( Minutes.One );
            var token = cancel.Token;

            var numbers = new ConcurrentList<Int32>();
            var create =  * 10240;
            var workers = new List<Task>( create );

            Debug.WriteLine( $"Creating {create} workers..." );

            foreach ( var i in 1.To( create ) ) {
                var task = Task.Run( async () => {
                    var rnd = Randem.NextTimeSpan( Milliseconds.One, Milliseconds.NinetySeven );

                    if ( i % 2 == 1 ) {
                        workers.Add( numbers.AddAsync( ( Int32 ) rnd.TotalMilliseconds ) );
                    }
                    else {
                        var total = workers.Count;
                        var b = Randem.Next( total );

                        if ( numbers.Remove( b ) ) {
                            workers.Add( b );
                        }
                    }

                    await rnd.Delay().ConfigureAwait( false );
                }, token );

                workers.Add( task );
            }

            Debug.WriteLine( $"Waiting for {workers.Count} workers to complete..." );

            while ( workers.Any( worker => worker?.IsDone() == false ) ) {
                await Task.Delay( 100, token ).ConfigureAwait( false );
            }

            Debug.WriteLine( $"We now have {numbers.Count} numbers." );

            numbers.Nop();
        }
        */

        [Fact]
        public static void ThreadSafetyTestAddAndRemoves() {
            using var list1 = new ConcurrentList<String>();

            using var threads = new ConcurrentList<Thread>( Threads );

            Parallel.ForEach( threads.AsParallel(), thread => {
                AddManyProductsRemoveRandom( list: list1 );

                thread?.Start();
            } );

            Parallel.ForEach( threads.AsParallel(), thread => thread?.Join() );

            Assert.NotEqual( expected: Threads, actual: list1.Count );
        }

        [Fact]
        public static void ThreadSafetyTestEquals() {
            using var list1 = new ConcurrentList<String>();
            using var list2 = new ConcurrentList<String>();

            var thread1 = new Thread( start: () => AddManyProducts( list: list1 ) );
            var thread2 = new Thread( start: () => AddManyProducts( list: list2 ) );

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.Equal( expected: list1.Count, actual: list2.Count );
        }

        [Fact]
        public static void ThreadSafetyTestNotEquals() {
            using var list1 = new ConcurrentList<String>();
            using var list2 = new ConcurrentList<String>();

            var thread1 = new Thread( start: () => AddManyProducts( list: list1 ) );
            var thread2 = new Thread( start: () => AddManyProducts( list: list2 ) );
            var thread3 = new Thread( start: () => AddManyProducts( list: list2 ) );

            thread1.Start();
            thread2.Start();
            thread3.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();

            Assert.NotEqual( expected: list1, actual: list2 );
        }
    }
}