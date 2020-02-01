// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DocumentExtensions.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "DocumentExtensions.cs" was last formatted by Protiguous on 2020/01/31 at 12:27 AM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Measurement.Time;

    public static class DocumentExtensions {

        /// <summary>16mb</summary>
        public static UInt32 BufferSize { get; } = 0x1000000;

        /*

        /// <summary>
        ///     The characters not allowed in file names.
        /// </summary>
        [NotNull]
        public static Char[] InvalidFileNameChars { get; } = Path.GetInvalidFileNameChars();
        */

        private static async Task InternalCopyWithProgress( [NotNull] Document source, [NotNull] Document destination, [CanBeNull] IProgress<Single> progress,
            [CanBeNull] IProgress<TimeSpan> eta, [NotNull] Char[] buffer, Single bytesToBeCopied, [CanBeNull] Stopwatch begin ) {
            using ( var reader = new StreamReader( source.FullPath ) ) {
                using ( var writer = new StreamWriter( destination.FullPath, false ) ) {
                    Int32 numRead;

                    while ( ( numRead = await reader.ReadAsync( buffer, 0, buffer.Length ).ConfigureAwait( false ) ).Any() ) {
                        await writer.WriteAsync( buffer, 0, numRead ).ConfigureAwait( false );
                        var bytesCopied = ( UInt64 ) numRead;

                        var percent = bytesCopied / bytesToBeCopied;

                        progress?.Report( percent );
                        eta?.Report( begin.Elapsed.EstimateTimeRemaining( percent ) );
                    }
                }
            }
        }

        /*

        /// <summary>
        ///     Returns the <paramref name="filename" /> with any invalid chars removed.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [NotNull]
        public static String CleanupForFileName(this String filename) {
            filename = filename ?? String.Empty;

            var sb = new StringBuilder(filename.Length);

            foreach (var c in filename.Where(c => !InvalidFileNameChars.Contains(c))) { sb.Append(c); }

            return sb.ToString();
        }
        */

        /*

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
                var begin = Stopwatch.StartNew();

                if ( !source.Exists() ) { return ResultCode.FailureSourceDoesNotExist; }

                if ( overwriteDestination && destination.Exists() ) { destination.Delete(); }

                var sourceInfo = source.Info;
                var sourceLength = source.Size();

                if ( !sourceLength.Any() ) { return ResultCode.FailureSourceIsEmpty; }

                var bytesToBeCopied = ( Single )sourceLength;
                UInt64 bytesCopied = 0;

                var buffer = Buffers.Value;

                try { await InternalCopyWithProgress( source, destination, progress, eta, buffer, bytesToBeCopied, begin ).NoUI(); }
                catch ( Exception exception ) {
                    exception.Log();

                    return ResultCode.FailureOnCopy;
                }

                if ( !destination.Exists() ) { return ResultCode.FailureDestinationDoesNotExist; }

                if ( bytesCopied != ( UInt64 )bytesToBeCopied ) { return ResultCode.FailureDestinationSizeIsDifferent; }

                try { File.SetAttributes( destination.FullPathWithFileName, sourceInfo.Attributes ); }
                catch ( Exception exception ) {
                    exception.Log();

                    return ResultCode.FailureUnableToSetFileAttributes;
                }

                try { File.SetCreationTimeUtc( destination.FullPathWithFileName, sourceInfo.CreationTimeUtc ); }
                catch ( Exception exception ) {
                    exception.Log();

                    return ResultCode.FailureUnableToSetFileCreationTime;
                }

                try { File.SetLastWriteTimeUtc( destination.FullPathWithFileName, sourceInfo.LastWriteTimeUtc ); }
                catch ( Exception exception ) {
                    exception.Log();

                    return ResultCode.FailureUnableToSetLastWriteTime;
                }

                try { File.SetLastAccessTimeUtc( destination.FullPathWithFileName, sourceInfo.LastAccessTimeUtc ); }
                catch ( Exception exception ) {
                    exception.Log();

                    return ResultCode.FailureUnableToSetLastAccessTime;
                }

                if ( !deleteSource ) { return ResultCode.Success; }

                try { File.Delete( source.FullPathWithFileName ); }
                catch ( Exception exception ) {
                    exception.Log();

                    return ResultCode.FailureUnableToDeleteSourceDocument;
                }

                return ResultCode.Success;
            }
            catch ( Exception exception ) {
                exception.Log();

                return ResultCode.FailureUnknown;
            }
        }
        */

        /*
        public static async Task<ResultCode> MoveAsync( [NotNull] this Document source, [NotNull] Document destination, Boolean overwriteDestination, IProgress<Single> progress = null, IProgress<TimeSpan> eta = null ) =>
            await source.CloneAsync( destination, overwriteDestination, true, progress, eta ).NoUI();
        */

        [NotNull]
        public static async Task<Boolean> IsAll( [NotNull] Document document, Byte number ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( !document.Exists() ) {
                return default;
            }

            using ( var stream = new FileStream( path: document.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                bufferSize: MathConstants.Sizes.OneGigaByte, options: FileOptions.SequentialScan ) ) {

                if ( !stream.CanRead ) {
                    throw new NotSupportedException( message: $"Cannot read from file stream on {document.FullPath}" );
                }

                var buffer = new Byte[ MathConstants.Sizes.OneGigaByte ];

                using var buffered = new BufferedStream( stream: stream );

                var bytesRead = 0;

                do {
                    var readTask = buffered.ReadAsync( buffer, offset: 0, count: buffer.Length );

                    if ( readTask != null ) {
                        bytesRead = await readTask.ConfigureAwait( false );

                        if ( !bytesRead.Any() || buffer.Any( b => b != number ) ) {
                            return default;
                        }
                    }
                } while ( bytesRead.Any() );

                return true;
            }
        }

        public static void TryPlayFile( [NotNull] this Document document ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            try {
                using ( var player = new SoundPlayer() ) {
                    player.SoundLocation = document.FullPath;
                    player.Load();
                    player.Play();
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

    }

    /*
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
    */

}