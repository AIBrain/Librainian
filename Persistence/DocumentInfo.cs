// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/DocumentInfo.cs" was last formatted by Protiguous on 2018/05/24 at 7:31 PM.

namespace Librainian.Persistence {

    using System;
    using System.Threading.Tasks;
    using ComputerSystems.FileSystem;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Threading;

    /// <summary>
    /// </summary>
    [Serializable]
    [JsonObject]
    public class DocumentInfo {

        [JsonProperty]
        public UInt32? CRC32 { get; set; }

        [JsonProperty]
        public UInt64? CRC64 { get; set; }

        public DateTime? CreationTimeUtc { get; set; }

        /// <summary>
        ///     "drive:/folder/file.ext"
        /// </summary>
        [NotNull]
        [JsonProperty]
        public String FullPath { get; set; }

        public DateTime? LastWriteTimeUtc { get; set; }

        [JsonProperty]
        public Int64? Length { get; set; }

        [JsonProperty]
        public DateTime? Updated { get; set; }

        public DocumentInfo( [NotNull] Document document ) {
            if ( document is null ) { throw new ArgumentNullException( paramName: nameof( document ) ); }

            this.FullPath = document.FullPathWithFileName;
            this.CreationTimeUtc = document.Info.CreationTimeUtc;
            this.LastWriteTimeUtc = document.Info.LastWriteTimeUtc;
            this.Length = document.Length;
        }

        public async Task<Boolean> Update() {

            var document = new Document( this.FullPath );

            //attempt to read both files at the same time (and thereby use the OS/disk caching)
            await Task.Run( () => Parallel.Invoke( () => this.CRC32 = document.CRC32(), () => this.CRC64 = document.CRC64() ) ).NoUI();
            this.Updated = DateTime.UtcNow;

            return true;
        }
    }
}