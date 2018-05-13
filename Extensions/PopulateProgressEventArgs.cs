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
// "Librainian/PopulateProgressEventArgs.cs" was last cleaned by Protiguous on 2018/05/12 at 1:23 AM

namespace Librainian.Extensions {

    using System;

    /// <summary>
    /// Simple EventArg for the two progress events
    /// NOTE: There will typically be some errors which is fine as some parts of the Registry are not accessible with standard security
    /// </summary>
    public class PopulateProgressEventArgs : EventArgs {

        public PopulateProgressEventArgs( Int32 itemCount, String keyName = null ) {
            this.ItemCount = itemCount;
            this.KeyName = keyName;
        }

        public PopulateProgressEventArgs() : this( -1 ) { }

        public Int32 ItemCount { get; internal set; }

        public String KeyName { get; }
    }
}