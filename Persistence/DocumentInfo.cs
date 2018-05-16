// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "DocumentInfo.cs",
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
// "Librainian/Librainian/DocumentInfo.cs" was last cleaned by Protiguous on 2018/05/15 at 10:49 PM.

namespace Librainian.Persistence {

    using System;
    using System.Threading.Tasks;
    using FileSystem;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Parsing;

    /// <summary>
    /// </summary>
    [Serializable]
    [JsonObject]
    public class DocumentInfo {

        public DocumentInfo( [NotNull] String fullPathFilenameExtension ) {
            if ( String.IsNullOrWhiteSpace( fullPathFilenameExtension ) ) { throw new ArgumentException( "Value cannot be null or whitespace.", paramName: nameof( fullPathFilenameExtension ) ); }

            this.FullPath = fullPathFilenameExtension;
        }

        [JsonProperty]
        public UInt32? CRC32 { get; set; }

        [JsonProperty]
        public UInt64? CRC64 { get; set; }

        /// <summary>
        ///     "drive:/folder/file.ext"
        /// </summary>
        [JsonProperty]
        public String FullPath { get; set; }

        [JsonProperty]
        public UInt64? Length { get; set; }

        [JsonProperty]
        public DateTime? Updated { get; set; }

        public async Task<Boolean> Update() {
            if ( this.FullPath.IsNullOrWhiteSpace() ) { return false; }

            var doc = new Document( this.FullPath );

            this.Length = doc.Length;

            this.CRC32 = await doc.CRC32().ConfigureAwait( false );

            this.CRC64 = await doc.CRC64().ConfigureAwait( false ); //TODO it would be nice to have a stream ?duplicator? so we only have to read over the file once.

            this.Updated = DateTime.UtcNow;

            return true;
        }
    }
}