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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "DocumentExtensions.cs" last formatted on 2022-12-22 at 5:15 PM by Protiguous.

namespace Librainian.FileSystem;

using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Maths;
using Measurement.Time;
using Utilities;

public static class DocumentExtensions {

	private static async Task InternalCopyWithProgress(
		IDocument source,
		IDocument destination,
		IProgress<Decimal>? progress,
		IProgress<TimeSpan>? eta,
		Char[] buffer,
		Decimal bytesToBeCopied,
		Stopwatch? begin
	) {
		using var reader = new StreamReader( source.FullPath );

#if NET5_0_OR_GREATER
#endif
		var writer = new StreamWriter( destination.FullPath, false );
		await using var _ = writer.ConfigureAwait( false );

		Int32 numRead;

		while ( ( numRead = await reader.ReadAsync( buffer, 0, buffer.Length ).ConfigureAwait( false ) ).Any() ) {
			await writer.WriteAsync( buffer, 0, numRead ).ConfigureAwait( false );
			var bytesCopied = ( UInt64 ) numRead;

			var percent = bytesCopied / bytesToBeCopied;

			progress?.Report( percent );
			eta?.Report( begin.Elapsed.EstimateTimeRemaining( percent ) );
		}
	}

	[NeedsTesting]
	public static Boolean BadlyNamedFile( this Document document, out BadlyNamedReason badlyNamedReason ) {
		//TODO This actually needs fleshed out with a whole host of options to decide what constitutes a "bad" file name

		var currentExtension = Path.GetExtension( document.FullPath );
		if ( !String.IsNullOrWhiteSpace( currentExtension ) ) {
			badlyNamedReason = BadlyNamedReason.MissingExtension;
			return true;
		}

		var withoutExtension = Path.GetFileNameWithoutExtension( document.FullPath );
		var anotherExtension = Path.GetExtension( withoutExtension );
		if ( !String.IsNullOrWhiteSpace( anotherExtension ) ) {
			badlyNamedReason = BadlyNamedReason.MultipleExtensions;
			return true;
		}

		var justName = Path.GetFileName( document.FileName );

		var regex = new Regex( "", RegexOptions.Compiled );
		if ( regex.IsMatch( justName ) ) { }

		badlyNamedReason = BadlyNamedReason.NotNamedBadly;
		return false;
	}

	/*

    /// <summary>
    /// Returns the <paramref name="filename" /> with any invalid chars removed.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    [NeedsTesting]
    public static String CleanupForFileName(this String filename) {
        filename = filename ?? String.Empty;

        var sb = new StringBuilder(filename.Length);

        foreach (var c in filename.Where(c => !InvalidFileNameChars.Contains(c))) { sb.Append(c); }

        return sb.ToString();
    }
    */

	/*

    /// <summary>
    /// Any result less than 1 is an error of some sort.
    /// </summary>
    /// <param name="source">              </param>
    /// <param name="destination">         </param>
    /// <param name="overwriteDestination"></param>
    /// <param name="deleteSource">        </param>
    /// <param name="progress">            </param>
    /// <param name="eta">                 </param>
    /// <returns></returns>
    public static async Task<ResultCode> CloneAsync( [NeedsTesting] this Document source, [NeedsTesting] Document destination, Boolean overwriteDestination, Boolean deleteSource, IProgress<Single> progress = null,
        IProgress<TimeSpan> eta = null ) {
        if ( source is null ) { throw new ArgumentEmptyException( nameof( source ) ); }

        if ( destination is null ) { throw new ArgumentEmptyException( nameof( destination ) ); }

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
    public static async Task<ResultCode> MoveAsync( [NeedsTesting] this Document source, [NeedsTesting] Document destination, Boolean overwriteDestination, IProgress<Single> progress = null, IProgress<TimeSpan> eta = null ) =>
        await source.CloneAsync( destination, overwriteDestination, true, progress, eta ).NoUI();
    */

}