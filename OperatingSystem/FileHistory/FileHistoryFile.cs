// Copyright 2018 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/FileHistoryFile.cs" was last cleaned by Rick on 2018/02/01 at 9:49 PM

namespace Librainian.OperatingSystem.FileHistory {
    using System;
    using FileSystem;

    public class FileHistoryFile {
        private readonly String _filename;

        private readonly Folder _folder;

        private readonly DateTime? _when;

        public FileHistoryFile( Document biglongpath ) {
            this.OriginalPath = biglongpath;
            this.IsFHF = biglongpath.TryParse( folder: out this._folder, filename: out this._filename, when: out this._when );
        }

        /// <summary>
        ///     (includes the extension)
        /// </summary>
        public String FileName => this._filename;

        public Folder Folder => this._folder;

        public Document FullPathAndName => new Document( folder: this.Folder, filename: this.FileName );

        public Boolean IsFHF { get; }

        public Document OriginalPath { get; }

        public DateTime? When => this._when;
    }
}