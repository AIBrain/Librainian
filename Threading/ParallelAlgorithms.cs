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
// "Librainian/ParallelAlgorithms.cs" was last cleaned by Rick on 2014/08/31 at 2:33 PM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Copyright (c) Microsoft Corporation.  All rights reserved.
    ///     File: ParallelAlgorithms_Wavefront.cs
    /// </summary>
    public static class ParallelAlgorithms {
        /// <summary>Executes a function for each element in a source, returning the first result achieved and ceasing execution.</summary>
        /// <typeparam name="TSource">The type of the data in the source.</typeparam>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="source">The input elements to be processed.</param>
        /// <param name="body">The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeForEach< TSource, TResult >( this IEnumerable< TSource > source, Func< TSource, TResult > body ) => source.SpeculativeForEach( ThreadingExtensions.Parallelism, body );

        /// <summary>Executes a function for each element in a source, returning the first result achieved and ceasing execution.</summary>
        /// <typeparam name="TSource">The type of the data in the source.</typeparam>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="source">The input elements to be processed.</param>
        /// <param name="options">The options to use for processing the loop.</param>
        /// <param name="body">The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeForEach< TSource, TResult >( this IEnumerable< TSource > source, ParallelOptions options, Func< TSource, TResult > body ) {
            // Validate parameters; the Parallel.ForEach we delegate to will validate the rest
            if ( body == null ) {
                throw new ArgumentNullException( "body" );
            }

            // Store one result.  We box it if it's a value type to avoid torn writes and enable
            // CompareExchange even for value types.
            object result = null;

            // Run all bodies in parallel, stopping as soon as one has completed.
            Parallel.ForEach( source, options, ( item, loopState ) => {
                                                   // Run an iteration.  When it completes, store (box) 
                                                   // the result, and cancel the rest
                                                   Interlocked.CompareExchange( ref result, body( item ), null );
                                                   loopState.Stop();
                                               } );

            // Return the computed result
            return ( TResult ) result;
        }

        /// <summary>Executes a function for each value in a range, returning the first result achieved and ceasing execution.</summary>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="fromInclusive">The start of the range, inclusive.</param>
        /// <param name="toExclusive">The end of the range, exclusive.</param>
        /// <param name="options">The options to use for processing the loop.</param>
        /// <param name="body">The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeFor< TResult >( this int fromInclusive, int toExclusive, Func< int, TResult > body ) => fromInclusive.SpeculativeFor( toExclusive, ThreadingExtensions.Parallelism, body );

        /// <summary>Executes a function for each value in a range, returning the first result achieved and ceasing execution.</summary>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="fromInclusive">The start of the range, inclusive.</param>
        /// <param name="toExclusive">The end of the range, exclusive.</param>
        /// <param name="options">The options to use for processing the loop.</param>
        /// <param name="body">The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeFor< TResult >( this int fromInclusive, int toExclusive, ParallelOptions options, Func< int, TResult > body ) {
            // Validate parameters; the Parallel.For we delegate to will validate the rest
            if ( body == null ) {
                throw new ArgumentNullException( "body" );
            }

            // Store one result.  We box it if it's a value type to avoid torn writes and enable
            // CompareExchange even for value types.
            object result = null;

            // Run all bodies in parallel, stopping as soon as one has completed.
            Parallel.For( fromInclusive, toExclusive, options, ( i, loopState ) => {
                                                                   // Run an iteration.  When it completes, store (box)
                                                                   // the result, and cancel the rest
                                                                   Interlocked.CompareExchange( ref result, body( i ), null );
                                                                   loopState.Stop();
                                                               } );

            // Return the computed result
            return ( TResult ) result;
        }

        /// <summary>Process in parallel a matrix where every cell has a dependency on the cell above it and to its left.</summary>
        /// <param name="processBlock">
        ///     The action to invoke for every block, supplied with the start and end indices of the rows
        ///     and columns.
        /// </param>
        /// <param name="numRows">The number of rows in the matrix.</param>
        /// <param name="numColumns">The number of columns in the matrix.</param>
        /// <param name="numBlocksPerRow">Partition the matrix into this number of blocks along the rows.</param>
        /// <param name="numBlocksPerColumn">Partition the matrix into this number of blocks along the columns.</param>
        public static void Wavefront( this Action< int, int, int, int > processBlock, int numRows, int numColumns, int numBlocksPerRow, int numBlocksPerColumn ) {
            // Validate parameters
            if ( numRows <= 0 ) {
                throw new ArgumentOutOfRangeException( "numRows" );
            }
            if ( numColumns <= 0 ) {
                throw new ArgumentOutOfRangeException( "numColumns" );
            }
            if ( numBlocksPerRow <= 0 || numBlocksPerRow > numRows ) {
                throw new ArgumentOutOfRangeException( "numBlocksPerRow" );
            }
            if ( numBlocksPerColumn <= 0 || numBlocksPerColumn > numColumns ) {
                throw new ArgumentOutOfRangeException( "numBlocksPerColumn" );
            }
            if ( processBlock == null ) {
                throw new ArgumentNullException( "processRowColumnCell" );
            }

            // Compute the size of each block
            var rowBlockSize = numRows / numBlocksPerRow;
            var columnBlockSize = numColumns / numBlocksPerColumn;

            Wavefront( ( ( row, column ) => {
                             var start_i = row * rowBlockSize;
                             var end_i = row < numBlocksPerRow - 1 ? start_i + rowBlockSize : numRows;

                             var start_j = column * columnBlockSize;
                             var end_j = column < numBlocksPerColumn - 1 ? start_j + columnBlockSize : numColumns;

                             processBlock( start_i, end_i, start_j, end_j );
                         } ), numBlocksPerRow, numBlocksPerColumn );
        }

        /// <summary>Process in parallel a matrix where every cell has a dependency on the cell above it and to its left.</summary>
        /// <param name="processRowColumnCell">The action to invoke for every cell, supplied with the row and column indices.</param>
        /// <param name="numRows">The number of rows in the matrix.</param>
        /// <param name="numColumns">The number of columns in the matrix.</param>
        public static void Wavefront( this Action< int, int > processRowColumnCell, int numRows, int numColumns ) {
            // Validate parameters
            if ( numRows <= 0 ) {
                throw new ArgumentOutOfRangeException( "numRows" );
            }
            if ( numColumns <= 0 ) {
                throw new ArgumentOutOfRangeException( "numColumns" );
            }
            if ( processRowColumnCell == null ) {
                throw new ArgumentNullException( "processRowColumnCell" );
            }

            // Store the previous row of tasks as well as the previous task in the current row
            var prevTaskRow = new Task[numColumns];
            Task prevTaskInCurrentRow = null;
            var dependencies = new Task[2];

            // Create a task for each cell
            for ( var row = 0; row < numRows; row++ ) {
                prevTaskInCurrentRow = null;
                for ( var column = 0; column < numColumns; column++ ) {
                    // In-scope locals for being captured in the task closures
                    int j = row, i = column;

                    // Create a task with the appropriate dependencies.
                    Task curTask;
                    if ( row == 0 && column == 0 ) {
                        // Upper-left task kicks everything off, having no dependencies
                        curTask = Task.Factory.StartNew( () => processRowColumnCell( j, i ) );
                    }
                    else if ( row == 0 || column == 0 ) {
                        // Tasks in the left-most column depend only on the task above them, and
                        // tasks in the top row depend only on the task to their left
                        var antecedent = column == 0 ? prevTaskRow[ 0 ] : prevTaskInCurrentRow;
// ReSharper disable once PossibleNullReferenceException
                        curTask = antecedent.ContinueWith( p => {
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
// ReSharper disable once PossibleNullReferenceException
            prevTaskInCurrentRow.Wait();
        }

        /// <summary>
        ///     Invokes the specified functions, potentially in parallel, canceling outstanding invocations once ONE
        ///     completes.
        /// </summary>
        /// <typeparam name="T">Specifies the type of data returned by the functions.</typeparam>
        /// <param name="functions">The functions to be executed.</param>
        /// <returns>A result from executing one of the functions.</returns>
        public static T SpeculativeInvoke< T >( params Func< T >[] functions ) => ThreadingExtensions.Parallelism.SpeculativeInvoke( functions );

        /// <summary>
        ///     Invokes the specified functions, potentially in parallel, canceling outstanding invocations once ONE
        ///     completes.
        /// </summary>
        /// <typeparam name="T">Specifies the type of data returned by the functions.</typeparam>
        /// <param name="options">The options to use for the execution.</param>
        /// <param name="functions">The functions to be executed.</param>
        /// <returns>A result from executing one of the functions.</returns>
        public static T SpeculativeInvoke< T >( this ParallelOptions options, params Func< T >[] functions ) {
            // Validate parameters
            if ( options == null ) {
                throw new ArgumentNullException( "options" );
            }
            if ( functions == null ) {
                throw new ArgumentNullException( "functions" );
            }

            // Speculatively invoke each function
            return functions.SpeculativeForEach( options, function => function() );
        }
    }
}
