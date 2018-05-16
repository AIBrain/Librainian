// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "DocumentCopyStatistics.cs",
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
// "Librainian/Librainian/DocumentCopyStatistics.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem {

    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;

    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public class DocumentCopyStatistics {

        [JsonProperty]
        public UInt64 BytesCopied { get; set; }

        [JsonProperty]
        [CanBeNull]
        public Document DestinationDocument { get; set; }

        [JsonProperty]
        [CanBeNull]
        public String DestinationDocumentCRC64 { get; set; }

        [JsonProperty]
        [CanBeNull]
        public Document SourceDocument { get; set; }

        [JsonProperty]
        [CanBeNull]
        public String SourceDocumentCRC64 { get; set; }

        [JsonProperty]
        public DateTime TimeStarted { get; set; }

        [JsonProperty]
        public TimeSpan TimeTaken { get; set; }

        public Double BytesPerMillisecond() {
            if ( Math.Abs( this.TimeTaken.TotalMilliseconds ) < Double.Epsilon ) { return 0; }

            return this.BytesCopied / this.TimeTaken.TotalMilliseconds;
        }

        public Double MegabytesPerSecond() {
            if ( Math.Abs( this.TimeTaken.TotalSeconds ) < Double.Epsilon ) { return 0; }

            var mb = this.BytesCopied / ( Double )Constants.Sizes.OneMegaByte;

            return mb / this.TimeTaken.TotalSeconds;
        }

        public Double MillisecondsPerByte() {
            if ( this.BytesCopied <= 0 ) { return 0; }

            return this.TimeTaken.TotalMilliseconds / this.BytesCopied;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => $"{this.SourceDocument?.FileName()} copied to {this.DestinationDocument.Folder} @ {this.MegabytesPerSecond()}MB/s";
    }
}