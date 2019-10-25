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
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "LibrainianTests", "ConcurrentListTests.cs" was last formatted by Protiguous on 2019/03/17 at 11:05 AM.

namespace LibrainianTests.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Librainian.Collections.Extensions;
    using Librainian.Collections.Lists;
    using Librainian.Extensions;
    using Librainian.Maths;
    using Librainian.Measurement.Time;
    using Librainian.Threading;
    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public static class ConcurrentListTests {

        private const Int32 NamesToAdd = 1024;

        private const Int32 ScaleCPUUsed = 1;

        private static void AddManyProducts( ICollection<String> list ) {
            for ( var x = 0; x < NamesToAdd; x++ ) {
                list.Add( item: $"name {x}" );
            }
        }

        private static void AddManyProductsRemoveOdds( ICollection<String> list ) {
            AddManyProducts( list: list );

            for ( var x = 0; x < NamesToAdd; x++ ) {
                if ( x.IsOdd() ) {
                    list.Remove( item: $"name {x}" );
                }
            }
        }

        [Fact]
        public static void AddNameThreadSafetyTest() {
            var list = new ConcurrentList<String>();

            //var threads = new List<Thread>();

            foreach ( var thread in 1.To( end: Environment.ProcessorCount * ScaleCPUUsed )
                .Select( selector: i => new Thread( start: () => AddManyProducts( list: list ) ) ) ) {

                //threads.Add( thread );
                thread.Start();
                thread.Join();
            }

            Assert.Equal( expected: NamesToAdd * Environment.ProcessorCount * ScaleCPUUsed, actual: list.Count );
        }

        [Fact]
        public static void ThreadSafetyTestAddAndRemoves() {
            var list1 = new ConcurrentList<String>();

            //var threads = new List<Thread>();

            foreach ( var thread in 1.To( end: Environment.ProcessorCount * ScaleCPUUsed )
                .Select( selector: i => new Thread( start: () => AddManyProductsRemoveOdds( list: list1 ) ) ) ) {

                //threads.Add( thread );
                thread.Start();
                thread.Join();
            }

            Assert.Equal( expected: NamesToAdd / 2 * Environment.ProcessorCount * ScaleCPUUsed, actual: list1.Count );
        }

        [Fact]
        public static void ThreadSafetyTestEquals() {
            var list1 = new ConcurrentList<String>();
            var list2 = new ConcurrentList<String>();

            var thread1 = new Thread( start: () => AddManyProducts( list: list1 ) );
            var thread2 = new Thread( start: () => AddManyProducts( list: list2 ) );

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.Equal( expected: list1, actual: list2 );
        }

        [Fact]
        public static void ThreadSafetyTestNotEquals() {
            var list1 = new ConcurrentList<String>();
            var list2 = new ConcurrentList<String>();

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

        [Fact]
        public static async Task TestAFew() {
            var cancel = new CancellationTokenSource( Minutes.One );
            var token = cancel.Token;

            var numbers = new ConcurrentList<Int32>();
            var create = Environment.ProcessorCount * 1024;
            var workers = new List<Task>();


            Debug.WriteLine( $"Creating {create} workers..." );

            foreach ( var i in 1.To( create ) ) {
                var task = Task.Run( () => {
                    var rnd = Randem.NextTimeSpan( Milliseconds.One, Milliseconds.NinetySeven );
                    rnd.Delay();
                    workers.Add( numbers.AddAsync( ( Int32 ) rnd.TotalMilliseconds ) );
                }, token );
                workers.Add( task );
            }

            Debug.WriteLine( $"Waiting for {workers.Count} workers to complete..." );

            while ( workers.Any( worker => worker?.IsDone() == false ) ) {
                Task.Delay( 100, token ).Wait( token );
            }

            Debug.WriteLine( $"We now have {numbers.Count} numbers." );

            numbers.Nop();
        }
    }
}