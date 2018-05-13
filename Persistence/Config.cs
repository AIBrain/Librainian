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
// "Librainian/Config.cs" was last cleaned by Protiguous on 2018/05/13 at 12:56 AM

namespace Librainian.Persistence {

    using System;
    using FileSystem;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    /// </summary>
    [Serializable]
    [JsonObject]
    public struct DocumentInfo {

        [JsonProperty]
        public UInt32? CRC32 { get; set; }

        [JsonProperty]
        public UInt64? CRC64 { get; set; }

        [JsonProperty]
        public String Document { get; set; }

        [JsonProperty]
        public UInt64 Length { get; set; }

        [JsonProperty]
        public DateTime Updated { get; set; }
    }

    public static class Config {

        public static PersistTable<String, DocumentInfo> DocumentRepository { get; } = new PersistTable<String, DocumentInfo>( Environment.SpecialFolder.CommonApplicationData, nameof( DocumentInfos ) );

        public static PersistTable<String, Folder> StorageLocations { get; } = new PersistTable<String, Folder>( Environment.SpecialFolder.CommonApplicationData, nameof( StorageLocations ) );

        public static void Record( [NotNull] this Document document, DocumentInfo info ) {
            if ( document == null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            info.Document.Should().BeEquivalentTo( document.FullPathWithFileName );

            DocumentRepository[document.FullPathWithFileName] = info;
        }
    }
}