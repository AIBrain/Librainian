#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/Search.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Extensions {
    using System;
    using System.IO;

    public class Search {
        /// <summary>
        ///     Action to perform when we find a file matching the <see cref="Pattern" />.
        /// </summary>
        public Action< FileInfo > OnFindFile { get; set; }

        /// <summary>
        ///     (optional) Action to perform when we find a folder.
        /// </summary>
        public Action< DirectoryInfo > OnFindFolder { get; set; }

        /// <summary>
        ///     The path we are to look inside.
        /// </summary>
        public String Path { get; set; }

        /// <summary>
        ///     The file pattern we are looking for.
        /// </summary>
        public String Pattern { get; set; }
    }
}
