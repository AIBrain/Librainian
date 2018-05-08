// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ContainerStream.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

namespace Librainian.OperatingSystem.Streams {

    using System;
    using System.IO;

    public abstract class ContainerStream : Stream {
        protected readonly Stream Stream;

        protected ContainerStream( Stream stream ) => this.Stream = stream ?? throw new ArgumentNullException( nameof( stream ) );

	    public override Boolean CanRead => this.Stream.CanRead;

        public override Boolean CanSeek => this.Stream.CanSeek;

        public override Boolean CanWrite => this.Stream.CanWrite;

        public override Int64 Length => this.Stream.Length;

        public override Int64 Position {
            get => this.Stream.Position;

	        set => this.Stream.Position = value;
        }

        protected Stream ContainedStream => this.Stream;

        public override void Flush() => this.Stream.Flush();

        public override Int32 Read( Byte[] buffer, Int32 offset, Int32 count ) => this.Stream.Read( buffer, offset, count );

        public override void Write( Byte[] buffer, Int32 offset, Int32 count ) => this.Stream.Write( buffer, offset, count );
    }
}