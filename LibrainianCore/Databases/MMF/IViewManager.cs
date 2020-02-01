// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "IViewManager.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "IViewManager.cs" was last formatted by Protiguous on 2020/01/31 at 12:24 AM.

namespace LibrainianCore.Databases.MMF {

    using System;
    using System.IO;

    public interface IViewManager {

        /// <summary>Keep file on exit</summary>
        Boolean KeepFile { get; set; }

        /// <summary>Number of items in the file</summary>
        Int64 Length { get; }

        /// <summary>Remove the backing file</summary>
        void CleanUp();

        /// <summary>Verify that the persisting file is large enough for the data written</summary>
        /// <param name="position">   Position to start writing</param>
        /// <param name="writeLength">Number of bytes to write</param>
        /// <returns></returns>
        Boolean EnoughBackingCapacity( Int64 position, Int64 writeLength );

        /// <summary>Get a working view for the current thread</summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        Stream GetView( Int32 threadId );

        /// <summary>Grow file</summary>
        /// <param name="sizeToGrowFrom">Size to grow from. Could be max size or an offset larger than the file</param>
        void Grow( Int64 sizeToGrowFrom );

        /// <summary>Initialize the backing file</summary>
        /// <param name="fileName">Filename to store the data</param>
        /// <param name="capacity">Number of items to allocate</param>
        /// <param name="dataSize">Size of datastructure</param>
        void Initialize( String fileName, Int64 capacity, Int32 dataSize );
    }
}