// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DocumentInfo.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "DocumentInfo.cs" was last formatted by Protiguous on 2018/07/10 at 6:17 PM.

namespace Librainian.Persistence {

	using System;
	using System.Diagnostics;
	using System.Threading;
	using System.Threading.Tasks;
	using ComputerSystem.FileSystem;
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

		public Boolean Equals( [CanBeNull] DocumentInfo other ) => Equals( this, other );

		private Int64? _length;

		/// <summary>
		///     "drive:/folder/file.ext"
		/// </summary>
		[NotNull]
		[JsonProperty]
		public String AbsolutePath { get; private set; }

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
		public Int64? Length {
			get => this._length;

			private set {
				this._length = value;
				this.Reset();
			}
		}

		public Task ScanningTask { get; private set; }

		public DocumentInfo( [NotNull] Document document ) {
			if ( document is null ) { throw new ArgumentNullException( paramName: nameof( document ) ); }

			this.AbsolutePath = document.FullPathWithFileName;

			this.Length = document.Length;
			this.CreationTimeUtc = document.Info.CreationTimeUtc;
			this.LastWriteTimeUtc = document.Info.LastWriteTimeUtc;

			this.LastScanned = null;

			//attempt to read all hashes at the same time (and thereby efficiently use the disk caching?)
			this.ScanningTask = new Task( () => Parallel.Invoke( new ParallelOptions {
					CancellationToken = this.CancellationToken,
					MaxDegreeOfParallelism = 3
				}, async () => this.CRC32 = await document.CRC32Async( this.CancellationToken ).NoUI(), async () => this.CRC64 = await document.CRC64Async( this.CancellationToken ).NoUI(),
				async () => this.AddHash = await document.CalcHashInt32Async( this.CancellationToken ).NoUI() ), this.CancellationToken );
		}

		public static Boolean? AreEitherDifferent( [NotNull] DocumentInfo left, [NotNull] DocumentInfo right ) {
			if ( left == null ) { throw new ArgumentNullException( paramName: nameof( left ) ); }

			if ( right == null ) { throw new ArgumentNullException( paramName: nameof( right ) ); }

			if ( !left.Length.HasValue || !right.Length.HasValue || !left.CreationTimeUtc.HasValue || !right.CreationTimeUtc.HasValue || !left.LastWriteTimeUtc.HasValue || !right.LastWriteTimeUtc.HasValue ) {
				return null;
			}

			if ( left.Length.Value != right.Length.Value || left.CreationTimeUtc.Value != right.CreationTimeUtc.Value || left.LastWriteTimeUtc.Value != right.LastWriteTimeUtc.Value ) { return true; }

			if ( !left.AddHash.HasValue || !right.AddHash.HasValue || !left.CRC32.HasValue || !right.CRC32.HasValue || !left.CRC64.HasValue || !right.CRC64.HasValue ) { return true; }

			if ( left.AddHash.Value != right.AddHash.Value || left.CRC32.Value != right.CRC32.Value || left.CRC64.Value != right.CRC64.Value ) { return true; }

			return false;
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

			if ( left is null || right is null ) { return false; }

			if ( left.LastScanned is null || right.LastScanned is null ) {
				return false; //the files need to be ran through Update() before we can compare them.
			}

			if ( left.Length.HasValue && right.Length.HasValue && left.Length.Value == right.Length.Value ) {
				if ( left.AddHash.HasValue && right.AddHash.HasValue && left.AddHash.Value == right.AddHash.Value ) {
					if ( left.CRC32.HasValue && right.CRC32.HasValue && left.CRC32.Value == right.CRC32.Value ) {
						if ( left.CRC64.HasValue && right.CRC64.HasValue && left.CRC64.Value == right.CRC64.Value ) {

							//Okay, we've compared by 3 different hashes. File should be unique by now.
							//The chances of 3 collisions is so low.. I won't even bother worrying about it happening in my lifetime.
							return true;
						}
					}
				}
			}

			return false;
		}

		public static Boolean operator !=( [CanBeNull] DocumentInfo left, [CanBeNull] DocumentInfo right ) => !Equals( left, right );

		public static Boolean operator ==( [CanBeNull] DocumentInfo left, [CanBeNull] DocumentInfo right ) => Equals( left, right );

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

				var record = Data.ScannedDocuments[ this.AbsolutePath ];

				var needScanned = false;

				if ( record is null ) {
					needScanned = true;

					goto TheTask;
				}

				if ( AreEitherDifferent( this, record ) == true ) { needScanned = true; }

				TheTask:

				if ( needScanned ) {
					if ( token.IsCancellationRequested ) { return false; }

					await this.ScanningTask.NoUI();
					this.LastScanned = DateTime.UtcNow;
				}

				if ( record is null ) { Data.ScannedDocuments[ this.AbsolutePath ] = this; }

				return true;
			}
			catch ( Exception exception ) { exception.More(); }
			finally { Debug.WriteLine( "done." ); }

			return false;
		}

		public override String ToString() => $"{this.AbsolutePath}={this.Length ?? -1} bytes";

	}

}