// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/DocumentInfo.cs" was last cleaned by Protiguous on 2018/05/13 at 8:51 PM

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
            if ( String.IsNullOrWhiteSpace( value: fullPathFilenameExtension ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( fullPathFilenameExtension ) );
            }
            this.FullPath = fullPathFilenameExtension;
        }

        [JsonProperty]
        public UInt32? CRC32 { get; set; }

        [JsonProperty]
        public UInt64? CRC64 { get; set; }

        /// <summary>
        /// "drive:/folder/file.ext"
        /// </summary>
        [JsonProperty]
        public String FullPath { get; set; }

        [JsonProperty]
        public UInt64? Length { get; set; }

        [JsonProperty]
        public DateTime? Updated { get; set; }

        public async Task<Boolean> Update() {
            if ( this.FullPath.IsNullOrWhiteSpace() ) {
                return false;
            }

            var doc = new Document( this.FullPath );

            this.Length = doc.Length;

            this.CRC32 = await doc.CRC32().ConfigureAwait( false );

            this.CRC64 = await doc.CRC64().ConfigureAwait( false ); //TODO it would be nice to have a stream ?duplicator? so we only have to read over the file once.

            this.Updated = DateTime.UtcNow;

            return true;
        }
    }
}