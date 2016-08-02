// Copyright 2016 Rick@AIBrain.org.
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
// "Librainian/IViewManager.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Database.MMF {

    using System;
    using System.IO;

    public interface IViewManager {

        /// <summary>Keep file on exit</summary>
        Boolean KeepFile {
            get; set;
        }

        /// <summary>Number of items in the file</summary>
        Int64 Length {
            get;
        }

        /// <summary>Remove the backing file</summary>
        void CleanUp();

        /// <summary>Verify that the persisting file is large enough for the data written</summary>
        /// <param name="position">Position to start writing</param>
        /// <param name="writeLength">Number of bytes to write</param>
        /// <returns></returns>
        Boolean EnoughBackingCapacity( Int64 position, Int64 writeLength );

        /// <summary>Get a working view for the current thread</summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        Stream GetView( Int32 threadId );

        /// <summary>Grow file</summary>
        /// <param name="sizeToGrowFrom">
        ///     Size to grow from. Could be max size or an offset larger than the file
        /// </param>
        void Grow( Int64 sizeToGrowFrom );

        /// <summary>Initialize the backing file</summary>
        /// <param name="fileName">Filename to store the data</param>
        /// <param name="capacity">Number of items to allocate</param>
        /// <param name="dataSize">Size of datastructure</param>
        void Initialize( String fileName, Int64 capacity, Int32 dataSize );
    }
}