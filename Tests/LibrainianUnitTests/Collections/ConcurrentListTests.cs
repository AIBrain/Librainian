// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "ConcurrentListTests.cs" last touched on 2021-12-29 at 5:28 AM by Protiguous.

namespace LibrainianUnitTests.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Librainian.Collections.Lists;
using Librainian.Maths;
using NUnit.Framework;

//[TestFixture]
//[Parallelizable( ParallelScope.All )]
public static class ConcurrentListTests {

	/// <summary>Environment.ProcessorCount * 10240 = 163840</summary>
	private static Int32 Threads { get; } = Environment.ProcessorCount;

	private static Int32 AddManyProducts( ICollection<String> list ) {
		if ( list is null ) {
			throw new ArgumentNullException( nameof( list ) );
		}

		Parallel.For( 0, Threads, x => list.Add( $"name {x}" ) );

		return list.Count;
	}

	private static (Int32 added, Int32 removed) AddManyProductsRemoveRandom( ICollection<String> list ) {
		if ( list is null ) {
			throw new ArgumentNullException( nameof( list ) );
		}

		var added = AddManyProducts( list );
		var removed = 0;

		Parallel.For( 0, added, x => {
			if ( Randem.NextBoolean() ) {
				list.Remove( $"name {x}" );
				Interlocked.Increment( ref removed );
			}
		} );

		return ( added, removed );
	}

	/*
    [Test]
    public static void AddNameThreadSafetyTest() {
        var list = new ConcurrentList<String>();

        var threads = new List<Thread>( Threads );

        foreach ( var thread in 1.To( Threads )
            .Select( selector: i => new Thread( start: () => AddManyProducts( list: list ) ) ) ) {
            threads.Add( thread );
            thread.Start();
        }

        Parallel.ForEach( threads.AsParallel(), thread => thread?.Join() );

        //Assert.AreEqual( expected: Threads * Threads, actual: list.Count );
    }
    */

	/*
    [Test]
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

	//[Test]
	//[Parallelizable( ParallelScope.All )]
	public static void ThreadSafetyTestAddAndRemoves() {
		var list1 = new ConcurrentList<String>();

		using var threads = new ConcurrentList<Thread>( Threads );

		Parallel.ForEach( threads.AsParallel(), thread => {
			AddManyProductsRemoveRandom( list1 );

			thread.Start();
		} );

		Parallel.ForEach( threads.AsParallel(), thread => thread.Join() );

		Assert.AreNotEqual( Threads, list1.Count );
	}

	//[Test]
	//[Parallelizable( ParallelScope.All )]
	public static void ThreadSafetyTestEquals() {
		var list1 = new ConcurrentList<String>();
		var list2 = new ConcurrentList<String>();

		var thread1 = new Thread( () => AddManyProducts( list1 ) );
		var thread2 = new Thread( () => AddManyProducts( list2 ) );

		thread1.Start();
		thread2.Start();

		thread1.Join();
		thread2.Join();

		Assert.AreEqual( list1.Count, list2.Count );
	}

	//[Test]
	//[Parallelizable( ParallelScope.All )]
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

}