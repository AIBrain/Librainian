// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ParallelAlgorithms.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "ParallelAlgorithms.cs" was last formatted by Protiguous on 2020/01/31 at 12:31 AM.

namespace Librainian.Threading {

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>Copyright (c) Microsoft Corporation. All rights reserved. File: ParallelAlgorithms_Wavefront.cs</summary>
    public static class ParallelAlgorithms {

        /// <summary>Executes a function for each value in a range, returning the first result achieved and ceasing execution.</summary>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="fromInclusive">The start of the range, inclusive.</param>
        /// <param name="toExclusive">  The end of the range, exclusive.</param>
        /// <param name="body">         The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        [CanBeNull]
        public static TResult SpeculativeFor<TResult>( this Int32 fromInclusive, Int32 toExclusive, [NotNull] Func<Int32, TResult> body ) =>
            fromInclusive.SpeculativeFor( toExclusive, CPU.AllCPUExceptOne, body );

        /// <summary>Executes a function for each value in a range, returning the first result achieved and ceasing execution.</summary>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="fromInclusive">The start of the range, inclusive.</param>
        /// <param name="toExclusive">  The end of the range, exclusive.</param>
        /// <param name="options">      The options to use for processing the loop.</param>
        /// <param name="body">         The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeFor<TResult>( this Int32 fromInclusive, Int32 toExclusive, [NotNull] ParallelOptions options, [NotNull] Func<Int32, TResult> body ) {

            // Validate parameters; the Parallel.For we delegate to will validate the rest
            if ( body is null ) {
                throw new ArgumentNullException( nameof( body ) );
            }

            // Store one result. We box it if it's a value type to avoid torn writes and enable CompareExchange even for value types.
            Object result = null;

            // Run all bodies in parallel, stopping as soon as one has completed.
            Parallel.For( fromInclusive, toExclusive, options, ( i, loopState ) => {

                // Run an iteration. When it completes, store (box) the result, and cancel the rest
                Interlocked.CompareExchange( ref result, body( i ), null );
                loopState.Stop();
            } );

            // Return the computed result
            return ( TResult ) result;
        }

        /// <summary>Executes a function for each element in a source, returning the first result achieved and ceasing execution.</summary>
        /// <typeparam name="TSource">The type of the data in the source.</typeparam>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="source">The input elements to be processed.</param>
        /// <param name="body">  The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        [CanBeNull]
        public static TResult SpeculativeForEach<TSource, TResult>( [NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, TResult> body ) =>
            source.SpeculativeForEach( CPU.AllCPUExceptOne, body );

        /// <summary>Executes a function for each element in a source, returning the first result achieved and ceasing execution.</summary>
        /// <typeparam name="TSource">The type of the data in the source.</typeparam>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="source"> The input elements to be processed.</param>
        /// <param name="options">The options to use for processing the loop.</param>
        /// <param name="body">   The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeForEach<TSource, TResult>( [NotNull] this IEnumerable<TSource> source, [NotNull] ParallelOptions options,
            [NotNull] Func<TSource, TResult> body ) {

            // Validate parameters; the Parallel.ForEach we delegate to will validate the rest
            if ( body is null ) {
                throw new ArgumentNullException( nameof( body ) );
            }

            // Store one result. We box it if it's a value type to avoid torn writes and enable CompareExchange even for value types.
            Object result = null;

            // Run all bodies in parallel, stopping as soon as one has completed.
            Parallel.ForEach( source, options, ( item, loopState ) => {

                // Run an iteration. When it completes, store (box) the result, and cancel the rest
                Interlocked.CompareExchange( ref result, body( item ), null );
                loopState.Stop();
            } );

            // Return the computed result
            return ( TResult ) result;
        }

        /// <summary>Invokes the specified functions, potentially in parallel, canceling outstanding invocations once ONE completes.</summary>
        /// <typeparam name="T">Specifies the type of data returned by the functions.</typeparam>
        /// <param name="functions">The functions to be executed.</param>
        /// <returns>A result from executing one of the functions.</returns>
        [CanBeNull]
        public static T SpeculativeInvoke<T>( [NotNull] params Func<T>[] functions ) => CPU.AllCPUExceptOne.SpeculativeInvoke( functions );

        /// <summary>Invokes the specified functions, potentially in parallel, canceling outstanding invocations once ONE completes.</summary>
        /// <typeparam name="T">Specifies the type of data returned by the functions.</typeparam>
        /// <param name="options">  The options to use for the execution.</param>
        /// <param name="functions">The functions to be executed.</param>
        /// <returns>A result from executing one of the functions.</returns>
        [CanBeNull]
        public static T SpeculativeInvoke<T>( [NotNull] this ParallelOptions options, [NotNull] params Func<T>[] functions ) {

            // Validate parameters
            if ( options is null ) {
                throw new ArgumentNullException( nameof( options ) );
            }

            if ( functions is null ) {
                throw new ArgumentNullException( nameof( functions ) );
            }

            // Speculatively invoke each function
            return functions.SpeculativeForEach( options, function => function() );
        }

        /// <summary>Process in parallel a matrix where every cell has a dependency on the cell above it and to its left.</summary>
        /// <param name="processBlock">The action to invoke for every block, supplied with the start and end indices of the rows and columns.</param>
        /// <param name="numRows">           The number of rows in the matrix.</param>
        /// <param name="numColumns">        The number of columns in the matrix.</param>
        /// <param name="numBlocksPerRow">   Partition the matrix into this number of blocks along the rows.</param>
        /// <param name="numBlocksPerColumn">Partition the matrix into this number of blocks along the columns.</param>
        public static void Wavefront( [NotNull] this Action<Int32, Int32, Int32, Int32> processBlock, Int32 numRows, Int32 numColumns, Int32 numBlocksPerRow,
            Int32 numBlocksPerColumn ) {

            // Validate parameters
            if ( numRows <= 0 ) {
                throw new ArgumentOutOfRangeException( nameof( numRows ) );
            }

            if ( numColumns <= 0 ) {
                throw new ArgumentOutOfRangeException( nameof( numColumns ) );
            }

            if ( numBlocksPerRow <= 0 || numBlocksPerRow > numRows ) {
                throw new ArgumentOutOfRangeException( nameof( numBlocksPerRow ) );
            }

            if ( numBlocksPerColumn <= 0 || numBlocksPerColumn > numColumns ) {
                throw new ArgumentOutOfRangeException( nameof( numBlocksPerColumn ) );
            }

            if ( processBlock is null ) {
                throw new ArgumentNullException( nameof( processBlock ) );
            }

            // Compute the size of each block
            var rowBlockSize = numRows / numBlocksPerRow;
            var columnBlockSize = numColumns / numBlocksPerColumn;

            Wavefront( ( row, column ) => {
                var startI = row * rowBlockSize;
                var endI = row < numBlocksPerRow - 1 ? startI + rowBlockSize : numRows;

                var startJ = column * columnBlockSize;
                var endJ = column < numBlocksPerColumn - 1 ? startJ + columnBlockSize : numColumns;

                processBlock( startI, endI, startJ, endJ );
            }, numBlocksPerRow, numBlocksPerColumn );
        }

        /// <summary>Process in parallel a matrix where every cell has a dependency on the cell above it and to its left.</summary>
        /// <param name="processRowColumnCell">The action to invoke for every cell, supplied with the row and column indices.</param>
        /// <param name="numRows">             The number of rows in the matrix.</param>
        /// <param name="numColumns">          The number of columns in the matrix.</param>
        public static void Wavefront( [NotNull] this Action<Int32, Int32> processRowColumnCell, Int32 numRows, Int32 numColumns ) {

            // Validate parameters
            if ( numRows <= 0 ) {
                throw new ArgumentOutOfRangeException( nameof( numRows ) );
            }

            if ( numColumns <= 0 ) {
                throw new ArgumentOutOfRangeException( nameof( numColumns ) );
            }

            if ( processRowColumnCell is null ) {
                throw new ArgumentNullException( nameof( processRowColumnCell ) );
            }

            // Store the previous row of tasks as well as the previous task in the current row
            var prevTaskRow = new Task[ numColumns ];
            Task prevTaskInCurrentRow = null;
            var dependencies = new Task[ 2 ];

            // Create a task for each cell
            for ( var row = 0; row < numRows; row++ ) {
                prevTaskInCurrentRow = null;

                for ( var column = 0; column < numColumns; column++ ) {

                    // In-scope locals for being captured in the task closures
                    Int32 j = row, i = column;

                    // Create a task with the appropriate dependencies.
                    Task curTask;

                    if ( row == 0 && column == 0 ) {

                        // Upper-left task kicks everything off, having no dependencies
                        curTask = Task.Run( () => processRowColumnCell( j, i ) );
                    }
                    else if ( row == 0 || column == 0 ) {

                        // Tasks in the left-most column depend only on the task above them, and tasks in the top row depend only on the task to their left
                        var antecedent = column == 0 ? prevTaskRow[ 0 ] : prevTaskInCurrentRow;

                        curTask = antecedent?.ContinueWith( p => {
                            p.Wait(); // Necessary only to propagate exceptions
                            processRowColumnCell( j, i );
                        } );
                    }
                    else // row > 0 && column > 0
                    {
                        // All other tasks depend on both the tasks above and to the left
                        dependencies[ 0 ] = prevTaskInCurrentRow;
                        dependencies[ 1 ] = prevTaskRow[ column ];

                        curTask = Task.Factory.ContinueWhenAll( dependencies, ps => {
                            Task.WaitAll( ps ); // Necessary only to propagate exceptions
                            processRowColumnCell( j, i );
                        } );
                    }

                    // Keep track of the task just created for future iterations
                    prevTaskRow[ column ] = prevTaskInCurrentRow = curTask;
                }
            }

            // Wait for the last task to be done.
            prevTaskInCurrentRow?.Wait();
        }

    }

}