// Copyright 2017 Rick@AIBrain.org.
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
// "Librainian/SelfDeletingFile.cs" was last cleaned by Rick on 2017/08/16 at 3:20 AM

namespace Librainian.FileSystem {

    using System;
    using JetBrains.Annotations;
    using Magic;

    /// <summary>
    /// Deletes the file after using.
    /// </summary>
    public class SelfDeletingFile : ABetterClassDispose {

        private readonly Document _document;

        public SelfDeletingFile( [ NotNull ] String filename ) {
            if ( filename is null ) {
                throw new ArgumentNullException( paramName: nameof(filename) );
            }
            this._document = new Document( filename );
        }

        public SelfDeletingFile( [ NotNull ] Document filename ) {
            if ( filename is null ) {
                throw new ArgumentNullException( paramName: nameof(filename) );
            }
            this._document = new Document( filename.FullPathWithFileName );
        }

	    protected override void DisposeManaged() {
		    try
		    {
			    this._document.Delete();
		    }
		    catch
		    {
			    // ignore
		    }
	    }
	}

}
