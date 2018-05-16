// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ContainerStream.cs",
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
// "Librainian/Librainian/ContainerStream.cs" was last cleaned by Protiguous on 2018/05/15 at 10:48 PM.

namespace Librainian.OperatingSystem.Streams {

    using System;
    using System.IO;

    public abstract class ContainerStream : Stream {

        protected readonly Stream Stream;

        protected ContainerStream( Stream stream ) => this.Stream = stream ?? throw new ArgumentNullException( nameof( stream ) );

        protected Stream ContainedStream => this.Stream;

        public override Int64 Position {
            get => this.Stream.Position;

            set => this.Stream.Position = value;
        }

        public override Boolean CanRead => this.Stream.CanRead;

        public override Boolean CanSeek => this.Stream.CanSeek;

        public override Boolean CanWrite => this.Stream.CanWrite;

        public override Int64 Length => this.Stream.Length;

        public override void Flush() => this.Stream.Flush();

        public override Int32 Read( Byte[] buffer, Int32 offset, Int32 count ) => this.Stream.Read( buffer, offset, count );

        public override void Write( Byte[] buffer, Int32 offset, Int32 count ) => this.Stream.Write( buffer, offset, count );
    }
}