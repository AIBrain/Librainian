// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "DocumentInfo.cs" last formatted on 2020-10-09 at 9:45 AM.

namespace Librainian.Persistence {
	using System;
	using System.Diagnostics;
	using System.Threading;
	using System.Threading.Tasks;
	using FileSystem;
	using JetBrains.Annotations;
	using Logging;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>Computes the various hashes of the given <see cref="AbsolutePath" />.</para>
	/// </summary>
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Serializable]
	[JsonObject]
	public class DocumentInfo : IEquatable<DocumentInfo> {

		[JsonIgnore]
		private UInt64? _length;

		public DocumentInfo( [NotNull] Document document ) {
			if ( document is null ) {
				throw new ArgumentNullException( nameof( document ) );
			}

			this.Reset();

			this.AbsolutePath = document.FullPath;

			this.Length = document.Length;
			this.CreationTimeUtc = document.CreationTimeUtc;
			this.LastWriteTimeUtc = document.LastWriteTimeUtc;

			this.LastScanned = null;
		}

		/// <summary>"drive:\folder\file.ext"</summary>
		[NotNull]
		[JsonProperty]
		public String AbsolutePath { get; private set; }

		/// <summary>The result of the Add-Hashing function.</summary>
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

		/// <summary>The most recent UTC datetime this info was updated.</summary>
		[JsonProperty]
		public DateTime? LastScanned { get; private set; }

		[JsonProperty]
		public DateTime? LastWriteTimeUtc { get; private set; }

		/// <summary></summary>
		[JsonProperty]
		public UInt64? Length {
			get => this._length;

			private set => this._length = value;
		}

		public Boolean Equals( [CanBeNull] DocumentInfo other ) => Equals( this, other );

		public static Boolean? AreEitherDifferent( [NotNull] DocumentInfo left, [NotNull] DocumentInfo right ) {
			if ( left is null ) {
				throw new ArgumentNullException( nameof( left ) );
			}

			if ( right is null ) {
				throw new ArgumentNullException( nameof( right ) );
			}

			if ( !left.Length.HasValue || !right.Length.HasValue ) {
				return default;
			}

			if ( !left.CreationTimeUtc.HasValue || !right.CreationTimeUtc.HasValue || !left.LastWriteTimeUtc.HasValue || !right.LastWriteTimeUtc.HasValue ) {
				return default;
			}

			if ( left.Length.Value != right.Length.Value ) {
				return true;
			}

			if ( left.CreationTimeUtc.Value != right.CreationTimeUtc.Value || left.LastWriteTimeUtc.Value != right.LastWriteTimeUtc.Value ) {
				return true;
			}

			if ( !left.AddHash.HasValue || !right.AddHash.HasValue || !left.CRC32.HasValue || !right.CRC32.HasValue || !left.CRC64.HasValue || !right.CRC64.HasValue ) {
				return true;
			}

			if ( left.AddHash.Value != right.AddHash.Value || left.CRC32.Value != right.CRC32.Value || left.CRC64.Value != right.CRC64.Value ) {
				return true;
			}

			return default;
		}

		[NotNull]
		public static Task<Int32> CalcHarkerHashInt32Async( [NotNull] Document document, CancellationToken token ) => Task.Run( () => document.CalcHashInt32(), token );

		/// <summary>
		///     <para>Static comparison test. Compares file lengths and hashes.</para>
		///     <para>
		///         If the hashes have not been computed yet on either file, the <see cref="Equals(DocumentInfo,DocumentInfo)" />
		///         is false.
		///     </para>
		///     <para>Unless <paramref name="left" /> is the same object as <paramref name="right" />.</para>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] DocumentInfo left, [CanBeNull] DocumentInfo right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true; //this is true for null==null, right?
			}

			if ( left is null || right is null ) {
				return default;
			}

			if ( left.LastScanned is null || right.LastScanned is null ) {
				return default; //the files need to be ran through Update() before we can compare them.
			}

			if ( !left.Length.HasValue || !right.Length.HasValue || left.Length.Value != right.Length.Value ) {
				return default;
			}

			if ( !left.AddHash.HasValue || !right.AddHash.HasValue || left.AddHash.Value != right.AddHash.Value ) {
				return default;
			}

			if ( !left.CRC32.HasValue || !right.CRC32.HasValue || left.CRC32.Value != right.CRC32.Value ) {
				return default;
			}

			//Okay, we've compared by 3 different hashes. File should be unique by now.
			//The chances of 3 collisions is so low.. I won't even bother worrying about it happening in my lifetime.
			return left.CRC64.HasValue && right.CRC64.HasValue && left.CRC64.Value == right.CRC64.Value;
		}

		public static Boolean operator !=( [CanBeNull] DocumentInfo left, [CanBeNull] DocumentInfo right ) => !Equals( left, right );

		public static Boolean operator ==( [CanBeNull] DocumentInfo left, [CanBeNull] DocumentInfo right ) => Equals( left, right );

		public override Boolean Equals( Object obj ) => Equals( this, obj as DocumentInfo );

		public override Int32 GetHashCode() => this.Length.GetHashCode();

		/// <summary>Attempt to read all hashes at the same time (and thereby efficiently use the disk caching?)</summary>
		/// <param name="document"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task GetHashesAsync( [NotNull] Document document, CancellationToken token ) {
			if ( document is null ) {
				throw new ArgumentNullException( nameof( document ) );
			}

			$"[{Thread.CurrentThread.ManagedThreadId}] Started hashings on {this.AbsolutePath}...".Verbose();

			var addHash = Task.Run( document.CalculateHarkerHashInt32, this.CancellationToken );
			var crc32 = document.CRC32( this.CancellationToken ).AsValueTask();
			var crc64 = document.CRC64( this.CancellationToken ).AsValueTask();

			await Task.WhenAll( crc32.AsTask(), crc64.AsTask(), addHash ).ConfigureAwait( false );


			this.AddHash = addHash.Result;


			this.CRC32 = crc32.Result;


			this.CRC64 = crc64.Result;


			$"[{Thread.CurrentThread.ManagedThreadId}] Completed hashings on {this.AbsolutePath}...".Verbose();
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

		/// <summary>Looks at the entire document.</summary>
		/// <returns></returns>
		public async Task ScanAsync( CancellationToken token ) {
			try {
				var needScanned = false;

				var record = MasterDocumentTable.DocumentInfos[this.AbsolutePath];

				if ( record != null ) {
					if ( AreEitherDifferent( this, record ) == true ) {
						needScanned = true;
					}
				}

				if ( needScanned ) {
					var document = new Document( this.AbsolutePath );

					this.Length = document.Length;
					this.CreationTimeUtc = document.CreationTimeUtc;
					this.LastWriteTimeUtc = document.LastWriteTimeUtc;

					await this.GetHashesAsync( document, token ).ConfigureAwait( false );

					this.LastScanned = DateTime.UtcNow;

					var copy = new DocumentInfo( document ) {
						LastScanned = this.LastScanned, CRC32 = this.CRC32, CRC64 = this.CRC64, AddHash = this.AddHash
					};

					MasterDocumentTable.DocumentInfos[this.AbsolutePath] = copy;
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}
		}

		[NotNull]
		public override String ToString() => $"{this.AbsolutePath}={this.Length?.ToString() ?? "toscan"} bytes";

	}
}