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
// File "DocumentInfo.cs" last touched on 2021-10-13 at 4:31 PM by Protiguous.

namespace Librainian.Persistence;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using FileSystem;
using Logging;
using Newtonsoft.Json;

/// <summary>
///     <para>Computes the various hashes of the given <see cref="AbsolutePath" />.</para>
/// </summary>
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Serializable]
[JsonObject]
public class DocumentInfo : IEquatable<DocumentInfo> {

	public DocumentInfo( Document document ) {
		if ( document is null ) {
			throw new NullException( nameof( document ) );
		}

		this.Reset();

		this.AbsolutePath = document.FullPath;

		this.Length = document.GetLength();
		this.CreationTimeUtc = document.CreationTimeUtc;
		this.LastWriteTimeUtc = document.LastWriteTimeUtc;

		this.LastScanned = null;
	}

	/// <summary>
	///     "drive:\folder\file.ext"
	/// </summary>
	[JsonProperty]
	public String AbsolutePath { get; private set; }

	/// <summary>
	///     The result of the Add-Hashing function.
	/// </summary>
	[JsonProperty]
	public Int32? AddHash { get; private set; }

	[JsonIgnore]
	public CancellationToken CancellationToken { get; set; }

	[JsonProperty]
	public Int32? CRC32 { get; private set; }

	[JsonProperty]
	public Int64? CRC64 { get; private set; }

	[JsonProperty]
	public DateTime? CreationTimeUtc { get; private set; }

	/// <summary>
	///     The most recent UTC datetime this info was updated.
	/// </summary>
	[JsonProperty]
	public DateTime? LastScanned { get; private set; }

	[JsonProperty]
	public DateTime? LastWriteTimeUtc { get; private set; }

	/// <summary>
	/// </summary>
	[JsonProperty]
	public UInt64? Length { get; private set; }

	public Boolean Equals( DocumentInfo? other ) => Equals( this, other );

	public static Boolean? AreEitherDifferent( DocumentInfo left, DocumentInfo right ) {
		if ( left.Length != right.Length ) {
			return true;
		}

		if ( left.CreationTimeUtc != right.CreationTimeUtc || left.LastWriteTimeUtc != right.LastWriteTimeUtc ) {
			return true;
		}

		if ( left.AddHash != right.AddHash ) {
			return true;
		}

		if ( left.CRC32 != right.CRC32 ) {
			return true;
		}

		if ( left.CRC64 != right.CRC64 ) {
			return true;
		}

		return false;
	}

	/// <summary>
	///     <para>Static comparison test. Compares file lengths and hashes.</para>
	///     <para>
	///         If the hashes have not been computed yet on either file, the <see cref="Equals(DocumentInfo,DocumentInfo)" />
	///         is false.
	///     </para>
	///     <para>Unless <paramref name="left" /> is the same object as <paramref name="right" />.</para>
	/// </summary>
	/// <param name="left"> </param>
	/// <param name="right"></param>
	public static Boolean Equals( DocumentInfo? left, DocumentInfo? right ) {
		if ( left is null || right is null ) {
			return false;
		}

		if ( ReferenceEquals( left, right ) ) {
			return true;
		}

		if ( left.LastScanned is null || right.LastScanned is null ) {
			return false; //the files need to be ran through Update() before we can compare them.
		}

		if ( left.Length != right.Length ) {
			return false;
		}

		if ( left.AddHash != right.AddHash ) {
			return false;
		}

		if ( left.CRC32 != right.CRC32 ) {
			return false;
		}

		//The chance of 3 collisions is so low.. I won't even bother worrying about it happening.
		return left.CRC64 == right.CRC64; //Okay, we've compared by 3 different hashes. File should be unique by now.
	}

	public static Boolean operator !=( DocumentInfo? left, DocumentInfo? right ) => !Equals( left, right );

	public static Boolean operator ==( DocumentInfo? left, DocumentInfo? right ) => Equals( left, right );

	public override Boolean Equals( Object? obj ) => Equals( this, obj as DocumentInfo );

	/// <summary>
	/// </summary>
	/// <returns></returns>
	// ReSharper disable once NonReadonlyMemberInGetHashCode
	public override Int32 GetHashCode() => this.Length?.GetHashCode() ?? 0;

	/// <summary>
	///     Attempt to read all hashes at the same time (and thereby efficiently use the disk caching?)
	/// </summary>
	/// <param name="document"></param>
	/// <param name="cancellationToken">   </param>
	public async Task GetHashesAsync( Document document, CancellationToken cancellationToken ) {
		if ( document is null ) {
			throw new NullException( nameof( document ) );
		}

		var watch = Stopwatch.StartNew();
		$"[{Environment.CurrentManagedThreadId}] Started hashings on {this.AbsolutePath}...".Verbose();

		var addHash = document.HarkerHash32( cancellationToken ).AsValueTask().AsTask();
		var crc32 = document.CRC32( cancellationToken ).AsValueTask().AsTask();
		var crc64 = document.CRC64( cancellationToken ).AsValueTask().AsTask();

		await Task.WhenAll( addHash, crc32, crc64 ).ConfigureAwait( false );

		this.AddHash = addHash.Result;

		this.CRC32 = crc32.Result;

		this.CRC64 = crc64.Result;

		watch.Stop();

		$"[{Environment.CurrentManagedThreadId}] Completed hashings on {this.AbsolutePath}...at {3 * await document.Size( cancellationToken ).ConfigureAwait( false ) / ( Decimal )watch.ElapsedMilliseconds:F} bytes per millisecond."
			.Verbose();
	}

	/// <summary>
	///     <para>Resets the results of the hashes to null.</para>
	///     <para>A change in <see cref="Length" /> basically means it's a new document.</para>
	///     <para><see cref="ScanAsync" /> needs to be called to repopulate these values.</para>
	/// </summary>
	public void Reset() {
		this.LastScanned = null;
		this.CreationTimeUtc = null;
		this.LastWriteTimeUtc = null;
		this.LastScanned = null;
		this.AddHash = null;
		this.CRC32 = null;
		this.CRC64 = null;
	}

	/// <summary>
	///     Looks at the entire document.
	/// </summary>
	public async Task ScanAsync( CancellationToken cancellationToken ) {
		try {
			var record = MainDocumentTable.DocumentInfos[ this.AbsolutePath ];

			var needScanned = AreEitherDifferent( this, record );

			if ( needScanned == true ) {
				var document = new Document( this.AbsolutePath );

				this.Length = await document.Length( cancellationToken ).ConfigureAwait( false );
				this.CreationTimeUtc = document.CreationTimeUtc;
				this.LastWriteTimeUtc = document.LastWriteTimeUtc;

				await this.GetHashesAsync( document, cancellationToken ).ConfigureAwait( false );

				this.LastScanned = DateTime.UtcNow;

				var documentInfo = new DocumentInfo( document ) {
					LastScanned = this.LastScanned, CRC32 = this.CRC32, CRC64 = this.CRC64, AddHash = this.AddHash
				};

				MainDocumentTable.DocumentInfos[ this.AbsolutePath ] = documentInfo;
			}
		}
		catch ( Exception exception ) {
			exception.Log();
		}
	}

	public override String ToString() => $"{this.AbsolutePath}={this.Length?.ToString() ?? "toscan"} bytes";

}