// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "IViewManager.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license has
// been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/IViewManager.cs" was last cleaned by Protiguous on 2018/05/15 at 1:34 AM.

namespace Librainian.Database.MMF {

    using System;
    using System.IO;

    public interface IViewManager {

        /// <summary>
        /// Keep file on exit
        /// </summary>
        Boolean KeepFile { get; set; }

        /// <summary>
        /// Number of items in the file
        /// </summary>
        Int64 Length { get; }

        /// <summary>
        /// Remove the backing file
        /// </summary>
        void CleanUp();

        /// <summary>
        /// Verify that the persisting file is large enough for the data written
        /// </summary>
        /// <param name="position">   Position to start writing</param>
        /// <param name="writeLength">Number of bytes to write</param>
        /// <returns></returns>
        Boolean EnoughBackingCapacity( Int64 position, Int64 writeLength );

        /// <summary>
        /// Get a working view for the current thread
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        Stream GetView( Int32 threadId );

        /// <summary>
        /// Grow file
        /// </summary>
        /// <param name="sizeToGrowFrom">Size to grow from. Could be max size or an offset larger than the file</param>
        void Grow( Int64 sizeToGrowFrom );

        /// <summary>
        /// Initialize the backing file
        /// </summary>
        /// <param name="fileName">Filename to store the data</param>
        /// <param name="capacity">Number of items to allocate</param>
        /// <param name="dataSize">Size of datastructure</param>
        void Initialize( String fileName, Int64 capacity, Int32 dataSize );
    }
}