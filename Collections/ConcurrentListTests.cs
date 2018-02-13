
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
        private const Int32 ScaleCPUUsed = 10;

        [Test]
        public static void AddNameThreadSafetyTest() {

            var list = new ConcurrentList<String>();
            //var threads = new List<Thread>();

            foreach ( var thread in 1.To( Environment.ProcessorCount * ScaleCPUUsed ).Select( i => new Thread( () => AddManyProducts( list ) ) ) ) {
                //threads.Add( thread );
                thread.Start();
                thread.Join();
            }

            Assert.AreEqual( NamesToAdd * Environment.ProcessorCount * ScaleCPUUsed, list.Count );
        }

        [Test]
        public static void ThreadSafetyTestEquals() {

            var list1 = new ConcurrentList<String>();
            var list2 = new ConcurrentList<String>();

            var thread1 = new Thread( () => AddManyProducts( list1 ) );
            var thread2 = new Thread( () => AddManyProducts( list2 ) );

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.AreEqual( list1, list2 );
        }

        [Test]
        public static void ThreadSafetyTestSerialize() {

            var list1 = new ConcurrentList<String>();
            var list2 = new ConcurrentList<String>();

            var mainthread = new Thread( () => AddManyProducts( list1 ) );
            mainthread.Start();
            mainthread.Join();

            String mainString = null;

            var thread1 = new Thread( () => mainString = list1.ToJSON() );
            var thread2 = new Thread( () => list2.AddRange(mainString.FromJSON<ConcurrentList<String> >()) );

            thread1.Start();
            thread1.Join();

            thread2.Start();
            thread2.Join();

            Assert.AreEqual( list1, list2 );
        }

        [Test]
        public static void ThreadSafetyTestNotEquals() {

            var list1 = new ConcurrentList<String>();
            var list2 = new ConcurrentList<String>();

            var thread1 = new Thread( () => AddManyProducts( list1 ) );
            var thread2 = new Thread( () => AddManyProducts( list2 ) );
            var thread3 = new Thread( () => AddManyProducts( list2 ) );

            thread1.Start();
            thread2.Start();
            thread3.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();

            Assert.AreNotEqual( list1, list2 );
        }

        [Test]
        public static void ThreadSafetyTestAddAndRemoves() {

            var list1 = new ConcurrentList<String>();
            //var threads = new List<Thread>();

            foreach ( var thread in 1.To( Environment.ProcessorCount * ScaleCPUUsed ).Select( i => new Thread( () => AddManyProductsRemoveOdds( list1 ) ) ) ) {
                //threads.Add( thread );
                thread.Start();
                thread.Join();
            }


            Assert.AreEqual( NamesToAdd / 2 * Environment.ProcessorCount * ScaleCPUUsed, list1.Count );
        }

        private static void AddManyProducts( ICollection<String> list ) {
            for ( var x = 0; x < NamesToAdd; x++ ) {
                list.Add( $"name {x}" );
            }
        }

        private static void AddManyProductsRemoveOdds( ICollection<String> list ) {
            AddManyProducts( list );

            for ( var x = 0; x < NamesToAdd; x++ ) {
                if ( x.IsOdd() ) {
                    list.Remove( $"name {x}" );
                }
            }
        }


    }

}
