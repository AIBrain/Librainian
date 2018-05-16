// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "DocumentExtensions.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/DocumentExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem {

    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Measurement.Time;
    using NUnit.Framework;
    using Parsing;

    public static class DocumentExtensions {

        /// <summary>
        ///     The characters not allowed in file names.
        /// </summary>
        [NotNull]
        public static readonly Char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

        /// <summary>
        ///     I hope this works the way I need it to: allocate one per thread and reuse it many times.
        /// </summary>
        private static ThreadLocal<Char[]> Buffers { get; } = new ThreadLocal<Char[]>( () => new Char[BufferSize], true );

        public static UInt32 BufferSize { get; } = 0x1000000;

        private static async Task InternalCpyWithProgress( Document source, Document destination, IProgress<Single> progress, IProgress<TimeSpan> eta, Char[] buffer, Single bytesToBeCopied, StopWatch begin ) {
            using ( var reader = new StreamReader( source.FullPathWithFileName ) ) {
                using ( var writer = new StreamWriter( destination.FullPathWithFileName, false ) ) {
                    Int32 numRead;

                    while ( ( numRead = await reader.ReadAsync( buffer, 0, buffer.Length ).ConfigureAwait( false ) ) != 0 ) {
                        await writer.WriteAsync( buffer, 0, numRead ).ConfigureAwait( false );
                        var bytesCopied = ( UInt64 )numRead;

                        var percent = bytesCopied / bytesToBeCopied;

                        progress?.Report( percent );
                        eta?.Report( begin.Elapsed.EstimateTimeRemaining( percent ) );
                    }
                }
            }
        }

        /// <summary>
        ///     Returns the <paramref name="filename" /> with any invalid chars removed.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [NotNull]
        public static String CleanupForFileName( this String filename ) {
            filename = filename ?? String.Empty;

            var sb = new StringBuilder( filename.Length );

            foreach ( var c in filename.Where( c => !InvalidFileNameChars.Contains( c ) ) ) { sb.Append( c ); }

            return sb.ToString();
        }

        /// <summary>
        ///     Any result less than 1 is an error of some sort.
        /// </summary>
        /// <param name="source">              </param>
        /// <param name="destination">         </param>
        /// <param name="overwriteDestination"></param>
        /// <param name="deleteSource">        </param>
        /// <param name="progress">            </param>
        /// <param name="eta">                 </param>
        /// <returns></returns>
        public static async Task<ResultCode> CloneAsync( [NotNull] this Document source, [NotNull] Document destination, Boolean overwriteDestination, Boolean deleteSource, IProgress<Single> progress = null,
            IProgress<TimeSpan> eta = null ) {
            if ( source is null ) { throw new ArgumentNullException( nameof( source ) ); }

            if ( destination is null ) { throw new ArgumentNullException( nameof( destination ) ); }

            try {
                var begin = StopWatch.StartNew();

                if ( !source.Exists() ) { return ResultCode.FailureSourceDoesNotExist; }

                if ( overwriteDestination && destination.Exists() ) { destination.Delete(); }

                var sourceInfo = source.Info;
                var sourceLength = source.Size();

                if ( !sourceLength.HasValue ) { return ResultCode.FailureSourceIsEmpty; }

                var bytesToBeCopied = ( Single )sourceLength.Value;
                UInt64 bytesCopied = 0;

                var buffer = Buffers.Value;

                try { await InternalCopyWithProgress( source, destination, progress, eta, buffer, bytesToBeCopied, begin ).ConfigureAwait( false ); }
                catch ( Exception exception ) {
                    exception.More();

                    return ResultCode.FailureOnCopy;
                }

                if ( !destination.Exists() ) { return ResultCode.FailureDestinationDoesNotExist; }

                if ( bytesCopied != ( UInt64 )bytesToBeCopied ) { return ResultCode.FailureDestinationSizeIsDifferent; }

                try { File.SetAttributes( destination.FullPathWithFileName, sourceInfo.Attributes ); }
                catch ( Exception exception ) {
                    exception.More();

                    return ResultCode.FailureUnableToSetFileAttributes;
                }

                try { File.SetCreationTimeUtc( destination.FullPathWithFileName, sourceInfo.CreationTimeUtc ); }
                catch ( Exception exception ) {
                    exception.More();

                    return ResultCode.FailureUnableToSetFileCreationTime;
                }

                try { File.SetLastWriteTimeUtc( destination.FullPathWithFileName, sourceInfo.LastWriteTimeUtc ); }
                catch ( Exception exception ) {
                    exception.More();

                    return ResultCode.FailureUnableToSetLastWriteTime;
                }

                try { File.SetLastAccessTimeUtc( destination.FullPathWithFileName, sourceInfo.LastAccessTimeUtc ); }
                catch ( Exception exception ) {
                    exception.More();

                    return ResultCode.FailureUnableToSetLastAccessTime;
                }

                if ( !deleteSource ) { return ResultCode.Success; }

                try { File.Delete( source.FullPathWithFileName ); }
                catch ( Exception exception ) {
                    exception.More();

                    return ResultCode.FailureUnableToDeleteSourceDocument;
                }

                return ResultCode.Success;
            }
            catch ( Exception exception ) {
                exception.More();

                return ResultCode.FailureUnknown;
            }
        }

        public static async Task<ResultCode> MoveAsync( [NotNull] this Document source, [NotNull] Document destination, Boolean overwriteDestination, IProgress<Single> progress = null, IProgress<TimeSpan> eta = null ) =>
            await source.CloneAsync( destination, overwriteDestination, true, progress, eta ).ConfigureAwait( false );
    }

    [TestFixture]
    public static class TestAsyncCopyAndMoves {

        [Test]
        public static async Task TestCopyAsync() {
            var source = Document.GetTempDocument();
            source.AppendText( "0123456789,\r\n".Repeat( UInt16.MaxValue, String.Empty ) );
            var sourceSize = source.Size();

            var destination = Document.GetTempDocument();
            var eta = new Progress<TimeSpan>();
            await source.MoveAsync( destination, false, null, eta );

            Console.WriteLine( destination.Size() );

            //TODO
            //destination.Size().Should( ).Should().Be( sourceSize );

            source.Delete();

            destination.Delete();
        }
    }
}