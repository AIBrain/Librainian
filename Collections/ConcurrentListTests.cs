// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConcurrentListTests.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/ConcurrentListTests.cs" was last formatted by Protiguous on 2018/05/24 at 6:58 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Maths;
    using NUnit.Framework;
    using Persistence;

    [TestFixture]
    public static class ConcurrentListTests {

        private const Int32 NamesToAdd = 1024;

        private const Int32 ScaleCPUUsed = 1;

        private static void AddManyProducts( ICollection<String> list ) {
            for ( var x = 0; x < NamesToAdd; x++ ) { list.Add( item: $"name {x}" ); }
        }

        private static void AddManyProductsRemoveOdds( ICollection<String> list ) {
            AddManyProducts( list: list );

            for ( var x = 0; x < NamesToAdd; x++ ) {
                if ( x.IsOdd() ) { list.Remove( item: $"name {x}" ); }
            }
        }

        [Test]
        public static void AddNameThreadSafetyTest() {
            var list = new ConcurrentList<String>();

            //var threads = new List<Thread>();

            foreach ( var thread in 1.To( end: Environment.ProcessorCount * ScaleCPUUsed ).Select( selector: i => new Thread( start: () => AddManyProducts( list: list ) ) ) ) {

                //threads.Add( thread );
                thread.Start();
                thread.Join();
            }

            Assert.AreEqual( expected: NamesToAdd * Environment.ProcessorCount * ScaleCPUUsed, actual: list.Count );
        }

        [Test]
        public static void ThreadSafetyTestAddAndRemoves() {
            var list1 = new ConcurrentList<String>();

            //var threads = new List<Thread>();

            foreach ( var thread in 1.To( end: Environment.ProcessorCount * ScaleCPUUsed ).Select( selector: i => new Thread( start: () => AddManyProductsRemoveOdds( list: list1 ) ) ) ) {

                //threads.Add( thread );
                thread.Start();
                thread.Join();
            }

            Assert.AreEqual( expected: NamesToAdd / 2 * Environment.ProcessorCount * ScaleCPUUsed, actual: list1.Count );
        }

        [Test]
        public static void ThreadSafetyTestEquals() {
            var list1 = new ConcurrentList<String>();
            var list2 = new ConcurrentList<String>();

            var thread1 = new Thread( start: () => AddManyProducts( list: list1 ) );
            var thread2 = new Thread( start: () => AddManyProducts( list: list2 ) );

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.AreEqual( expected: list1, actual: list2 );
        }

        [Test]
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

            Assert.AreNotEqual( expected: list1, actual: list2 );
        }

        [Test]
        public static void ThreadSafetyTestSerialize() {
            var list1 = new ConcurrentList<String>();
            var list2 = new ConcurrentList<String>();

            var mainthread = new Thread( start: () => AddManyProducts( list: list1 ) );
            mainthread.Start();
            mainthread.Join();

            String mainString = null;

            var thread1 = new Thread( start: () => mainString = list1.ToJSON() );
            var thread2 = new Thread( start: () => list2.AddRange( items: mainString.FromJSON<ConcurrentList<String>>() ) );

            thread1.Start();
            thread1.Join();

            thread2.Start();
            thread2.Join();

            Assert.AreEqual( expected: list1, actual: list2 );
        }
    }
}