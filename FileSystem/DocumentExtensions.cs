// Copyright 2016 Rick@AIBrain.org.
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
// "Librainian/DocumentExtensions.cs" was last cleaned by Rick on 2016/08/13 at 4:01 PM

namespace Librainian.FileSystem {

    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
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

        public static UInt32 BufferSize { get; set; } = 0x10000;

        /// <summary>
        ///     I hope this works the way I need it to: allocate one per thread and reuse it many times.
        /// </summary>
        private static ThreadLocal<Char[]> Buffers { get; } = new ThreadLocal<Char[]>( () => new Char[ BufferSize ], false );

        /// <summary>
        ///     Returns the <paramref name="filename" /> with any invalid chars removed.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static String CleanupForFileName( this String filename ) {
            filename = filename ?? String.Empty;

            var sb = new StringBuilder( filename.Length );

            foreach ( var c in filename.Where( c => !InvalidFileNameChars.Contains( c ) ) ) {
                sb.Append( c );
            }

            return sb.ToString();
        }

        public static async Task<ResultCode> CloneAsync( [NotNull] this Document source, [NotNull] Document destination, Boolean overwriteDestination, Boolean deleteSource, IProgress<Single> progress = null, IProgress<TimeSpan> eta = null ) {
            if ( source == null ) {
                throw new ArgumentNullException( nameof( source ) );
            }
            if ( destination == null ) {
                throw new ArgumentNullException( nameof( destination ) );
            }

            try {
                var begin = StopWatch.StartNew();

                if ( !source.Exists() ) {
                    return ResultCode.FailureSourceDoesNotExist;
                }

                if ( overwriteDestination && destination.Exists() ) {
                    destination.Delete();
                }

                var sourceInfo = source.Info();
                var sourceLength = source.Size();

                if ( !sourceLength.HasValue ) {
                    return ResultCode.FailureSourceIsEmpty;
                }

                var bytesToBeCopied = ( Single )sourceLength.Value;
                UInt64 bytesCopied = 0;

                var buffer = Buffers.Value;

                try {
                    using ( var reader = new StreamReader( source.FullPathWithFileName ) ) {
                        using ( var writer = new StreamWriter( destination.FullPathWithFileName, false ) ) {
                            Int32 numRead;
                            while ( ( numRead = await reader.ReadAsync( buffer, 0, buffer.Length ).ConfigureAwait( false ) ) != 0 ) {
                                await writer.WriteAsync( buffer, 0, numRead ).ConfigureAwait( false );
                                bytesCopied += ( UInt64 )numRead;

                                var percent = bytesCopied / bytesToBeCopied;

                                progress?.Report( percent );
                                eta?.Report( begin.Elapsed.EstimateTimeRemaining( percent ) );
                            }
                        }
                    }
                }
                catch ( Exception exception ) {
                    exception.More();
                    return ResultCode.FailureOnCopy;
                }

                if ( !destination.Exists() ) {
                    return ResultCode.FailureDestinationDoesNotExist;
                }

                if ( bytesCopied != ( UInt64 )bytesToBeCopied ) {
                    return ResultCode.FailureDestinationSizeIsDifferent;
                }

                try {
                    File.SetAttributes( destination.FullPathWithFileName, sourceInfo.Attributes );
                }
                catch ( Exception exception ) {
                    exception.More();
                    return ResultCode.FailureUnableToSetFileAttributes;
                }

                try {
                    File.SetCreationTimeUtc( destination.FullPathWithFileName, sourceInfo.CreationTimeUtc );
                }
                catch ( Exception exception ) {
                    exception.More();
                    return ResultCode.FailureUnableToSetFileCreationTime;
                }

                try {
                    File.SetLastWriteTimeUtc( destination.FullPathWithFileName, sourceInfo.LastWriteTimeUtc );
                }
                catch ( Exception exception ) {
                    exception.More();
                    return ResultCode.FailureUnableToSetLastWriteTime;
                }

                try {
                    File.SetLastAccessTimeUtc( destination.FullPathWithFileName, sourceInfo.LastAccessTimeUtc );
                }
                catch ( Exception exception ) {
                    exception.More();
                    return ResultCode.FailureUnableToSetLastAccessTime;
                }

                if ( deleteSource ) {
                    try {
                        File.Delete( source.FullPathWithFileName );
                    }
                    catch ( Exception exception ) {
                        exception.More();
                        return ResultCode.FailureUnableToDeleteSourceDocument;
                    }
                }

                return ResultCode.Success;
            }
            catch ( Exception exception ) {
                exception.More();
                return ResultCode.FailureUnknown;
            }
        }

        public static async Task<ResultCode> MoveAsync( [NotNull] this Document source, [NotNull] Document destination, Boolean overwriteDestination, IProgress<Single> progress = null, IProgress<TimeSpan> eta = null ) {
            return await CloneAsync( source, destination, overwriteDestination, true, progress, eta ).ConfigureAwait( false );
        }
    }

    [TestFixture]
    public static class TestAsyncCopyAndMoves {

        [Test]
        public static async void TestCopy() {

            var source = Document.GetTempDocument();
            source.AppendText( "0123456789,\r\n".Repeat( UInt16.MaxValue, String.Empty ) );
            var sourceSize = source.Size();

            var destination = Document.GetTempDocument();
            var eta = new Progress<TimeSpan>();
            await source.MoveAsync( destination, false, null, eta );

            Console.WriteLine( destination.Size() );

            destination.Size().Should().Be( sourceSize );

            source.Delete();

            destination.Delete();

        }

    }

}
