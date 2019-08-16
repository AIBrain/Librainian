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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "DocumentInfo.cs" was last formatted by Protiguous on 2019/08/08 at 9:28 AM.

namespace Librainian.Persistence {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Logging;
    using Newtonsoft.Json;
    using OperatingSystem.FileSystem;

    /// <summary>
    ///     <para>Computes the various hashes of the given <see cref="AbsolutePath" />.</para>
    /// </summary>
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Serializable]
    [JsonObject]
    public class DocumentInfo : IEquatable<DocumentInfo> {

        public Boolean Equals( [CanBeNull] DocumentInfo other ) => Equals( this, other );

        [JsonIgnore]
        private UInt64? _length;

        /// <summary>
        ///     "drive:\folder\file.ext"
        /// </summary>
        [NotNull]
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
        public UInt64? Length {
            get => this._length;

            private set => this._length = value;
        }

        public DocumentInfo( [NotNull] Document document ) {
            if ( document == null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            this.Reset();

            this.AbsolutePath = document.FullPath;

            this.Length = document.Length;
            this.CreationTimeUtc = document.CreationTimeUtc;
            this.LastWriteTimeUtc = document.LastWriteTimeUtc;

            this.LastScanned = null;
        }

        public static Boolean? AreEitherDifferent( [NotNull] DocumentInfo left, [NotNull] DocumentInfo right ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right == null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            if ( !left.Length.HasValue || !right.Length.HasValue ) {
                return null;
            }

            if ( !left.CreationTimeUtc.HasValue || !right.CreationTimeUtc.HasValue || !left.LastWriteTimeUtc.HasValue || !right.LastWriteTimeUtc.HasValue ) {
                return null;
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

            return false;
        }

        [NotNull]
        public static Task<Int32> CalcHashInt32Async( [NotNull] Document document, CancellationToken token ) => Task.Run( () => document.CalcHashInt32(), token );

        /// <summary>
        ///     <para>Static comparison test. Compares file lengths and hashes.</para>
        ///     <para>
        ///         If the hashes have not been computed yet on either file, the
        ///         <see cref="Equals(Librainian.Persistence.DocumentInfo,Librainian.Persistence.DocumentInfo)" /> is false.
        ///     </para>
        ///     <para>Unless <paramref name="left" /> is the same object as <paramref name="right" />.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( DocumentInfo left, DocumentInfo right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true; //this is true for null==null, right?
            }

            if ( left == null || right == null ) {
                return false;
            }

            if ( left.LastScanned == null || right.LastScanned == null ) {
                return false; //the files need to be ran through Update() before we can compare them.
            }

            if ( !left.Length.HasValue || !right.Length.HasValue || left.Length.Value != right.Length.Value ) {
                return false;
            }

            if ( !left.AddHash.HasValue || !right.AddHash.HasValue || left.AddHash.Value != right.AddHash.Value ) {
                return false;
            }

            if ( !left.CRC32.HasValue || !right.CRC32.HasValue || left.CRC32.Value != right.CRC32.Value ) {
                return false;
            }

            //Okay, we've compared by 3 different hashes. File should be unique by now.
            //The chances of 3 collisions is so low.. I won't even bother worrying about it happening in my lifetime.
            return left.CRC64.HasValue && right.CRC64.HasValue && left.CRC64.Value == right.CRC64.Value;
        }

        public static Boolean operator !=( [CanBeNull] DocumentInfo left, [CanBeNull] DocumentInfo right ) => !Equals( left, right );

        public static Boolean operator ==( [CanBeNull] DocumentInfo left, [CanBeNull] DocumentInfo right ) => Equals( left, right );

        public override Boolean Equals( Object obj ) => Equals( this, obj as DocumentInfo );

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override Int32 GetHashCode() => this.Length.GetHashCode();

        /// <summary>
        ///     Attempt to read all hashes at the same time (and thereby efficiently use the disk caching?)
        /// </summary>
        /// <param name="document"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task GetHashesAsync( [NotNull] Document document, CancellationToken token ) {
            if ( document == null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            Debug.Write( $"[{Thread.CurrentThread.ManagedThreadId}] Started hashings on {this.AbsolutePath}..." );

            var addHash = Task.Run( document.CalculateHarkerHashInt32, this.CancellationToken );
            var crc32 = document.CRC32Async( this.CancellationToken );
            var crc64 = document.CRC64Async( this.CancellationToken );

            await Task.WhenAll( crc32, crc64, addHash ).ConfigureAwait( false );

            if ( addHash.IsCompleted ) {
                this.AddHash = addHash.Result;
            }

            if ( crc32.IsCompleted ) {
                this.CRC32 = crc32.Result;
            }

            if ( crc64.IsCompleted ) {
                this.CRC64 = crc64.Result;
            }

            Debug.Write( $"[{Thread.CurrentThread.ManagedThreadId}] Completed hashings on {this.AbsolutePath}..." );
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
        /// <returns></returns>
        public async Task ScanAsync( CancellationToken token ) {

            try {
                var needScanned = false;

                if ( MasterDocumentTable.DocumentInfos[ this.AbsolutePath ] is DocumentInfo record ) {
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

                    MasterDocumentTable.DocumentInfos[ this.AbsolutePath ] = copy;
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

        public override String ToString() => $"{this.AbsolutePath}={this.Length?.ToString() ?? "toscan"} bytes";
    }
}