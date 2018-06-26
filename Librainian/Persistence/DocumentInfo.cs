// Copyright © 1995-2017 to Rick@AIBrain.org and 2018-2018 to Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DocumentInfo.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "DocumentInfo.cs" was last formatted by Protiguous on 2018/06/22 at 4:12 PM.

namespace Librainian.Persistence {

	using System;
	using System.Diagnostics;
	using System.Threading;
	using System.Threading.Tasks;
	using ComputerSystems.FileSystem;
	using JetBrains.Annotations;
	using Maths.Hashings;
	using Newtonsoft.Json;
	using Threading;

	/// <summary>
	/// </summary>
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Serializable]
	[JsonObject]
	public class DocumentInfo : IEquatable<DocumentInfo> {

		private Int64? _length;

		/// <summary>
		///     "drive:/folder/file.ext"
		/// </summary>
		[NotNull]
		[JsonProperty]
		public String AbsolutePath { get; private set; }

		[JsonProperty]
		public Int32? AddHash { get; private set; }

		public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

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
		public Int64? Length {
			get => this._length;

			private set {
				this._length = value;
				this.Reset();
			}
		}

		public Task ScanningTask { get; private set; }

		public DocumentInfo( [NotNull] Document document ) {
			if ( document is null ) {
				throw new ArgumentNullException( paramName: nameof( document ) );
			}

			this.AbsolutePath = document.FullPathWithFileName;

			this.Length = document.Length;
			this.CreationTimeUtc = document.Info.CreationTimeUtc;
			this.LastWriteTimeUtc = document.Info.LastWriteTimeUtc;

			this.LastScanned = null;

			//attempt to read all hashes at the same time (and thereby efficiently use the disk caching?)
			this.ScanningTask = new Task( () => Parallel.Invoke( new ParallelOptions {
				CancellationToken = this.CancellationTokenSource.Token,
				MaxDegreeOfParallelism = 3
			}, async () => this.CRC32 = await document.CRC32Async( this.CancellationTokenSource.Token ).NoUI(),
				async () => this.CRC64 = await document.CRC64Async( this.CancellationTokenSource.Token ).NoUI(),
				async () => this.AddHash = await document.CalcHashInt32Async( this.CancellationTokenSource.Token ).NoUI() ), this.CancellationTokenSource.Token );
		}

		/// <summary>
		///     Static comparison test. Compares file lengths and hashes.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( DocumentInfo left, DocumentInfo right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true; //this is true for null==null, right?
			}

			if ( left is null || right is null ) {
				return false;
			}

			if ( left.LastScanned is null || right.LastScanned is null ) {
				return false; //the files need to be ran through Update() before we can compare them.
			}

			if ( left.Length.HasValue && right.Length.HasValue && left.Length.Value == right.Length.Value ) {
				if ( left.AddHash.HasValue && right.AddHash.HasValue && left.AddHash.Value == right.AddHash.Value ) {
					if ( left.CRC32.HasValue && right.CRC32.HasValue && left.CRC32.Value == right.CRC32.Value ) {
						if ( left.CRC64.HasValue && right.CRC64.HasValue && left.CRC64.Value == right.CRC64.Value ) {

							//Okay, we've compared by 3 different hashes. File should be unique by now.
							//the chances of collisions are so low, I won't even bother worrying about it happening in my lifetime.
							return true;
						}
					}
				}
			}

			return false;
		}

		public static Boolean operator !=( [CanBeNull] DocumentInfo left, [CanBeNull] DocumentInfo right ) => !Equals( left, right );

		public static Boolean operator ==( [CanBeNull] DocumentInfo left, [CanBeNull] DocumentInfo right ) => Equals( left, right );

		public Boolean Equals( [CanBeNull] DocumentInfo other ) => Equals( this, other );

		public override Boolean Equals( Object obj ) => Equals( this, obj as DocumentInfo );

		// ReSharper disable once NonReadonlyMemberInGetHashCode
		public override Int32 GetHashCode() => this.Length.GetHashCode();

		/// <summary>
		///     <para>A change in <see cref="Length" /> basically means it's a new document.</para>
		///     <para><see cref="Scan" /> needs to be called to repopulate these values.</para>
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
		/// <returns></returns>
		public async Task<Boolean> Scan( CancellationToken token ) {

			try {
				Debug.Write( $"Starting scan on {this.AbsolutePath}..." );
				var document = new Document( this.AbsolutePath );

				this.Length = document.Length;
				this.CreationTimeUtc = document.Info.CreationTimeUtc;
				this.LastWriteTimeUtc = document.Info.LastWriteTimeUtc;

				await this.ScanningTask.NoUI();

				this.LastScanned = DateTime.UtcNow;

				Data.ScannedDocuments[this.AbsolutePath] = this;

				return true;
			}
			catch ( Exception exception ) {
				exception.More();
			}
			finally {
				Debug.WriteLine( "done." );
			}

			return false;
		}

		public override String ToString() => $"{this.AbsolutePath}={this.Length} bytes";
	}
}