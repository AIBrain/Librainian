// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Chunk.cs" was last cleaned by Protiguous on 2018/05/11 at 8:45 PM

namespace Librainian.FileSystem {

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using JetBrains.Annotations;
    using Magic;

    /// <summary>
    /// For copying a <see cref="Chunk"/> of a <see cref="Document"/> into another <see cref="Document"/>.
    /// </summary>
    public class Chunk : ABetterClassDispose {
        private Int64 _offsetBegin;

        private Int64 _offsetEnd;

        public Chunk( [NotNull] Document document ) => this.Document = document ?? throw new ArgumentNullException( nameof( document ) );

        [CanBeNull]
        public Byte[] Buffer { get; private set; }

        [NotNull]
        public Document Document { get; }

        /// <summary>
        /// Starting position in file. Limited to <see cref="Int32.MaxValue"/> (even though this is a <see cref="Int64"/>).
        /// </summary>
        public Int64 OffsetBegin {
            get => this._offsetBegin;

            set {
                if ( value > this.OffsetEnd ) {
                    throw new ArgumentOutOfRangeException( nameof( this.OffsetBegin ), $"Offset {value:N0} is greater than {nameof( Int64.MaxValue )}." );
                }

                this._offsetBegin = value;
            }
        }

        /// <summary>
        /// Ending position in file.
        /// </summary>
        public Int64 OffsetEnd {
            get => this._offsetEnd;

            set {
                if ( value < this.OffsetBegin ) {
                    throw new ArgumentOutOfRangeException( nameof( this.OffsetEnd ) );
                }

                this._offsetEnd = value;
            }
        }

        public Boolean CopyTo( [NotNull] Document destination ) {
            if ( destination is null ) {
                throw new ArgumentNullException( paramName: nameof( destination ) );
            }

            if ( !destination.Exists() ) {
                using ( var mmfOut = MemoryMappedFile.CreateNew( destination.JustName(), ( Int64 )fileSize, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.DelayAllocatePages,
                    HandleInheritability.Inheritable ) ) {
                    using ( var stream = mmfOut.CreateViewStream() ) {
                        stream.Seek( this.OffsetBegin, SeekOrigin.Begin );
                        using ( var writer = new BinaryWriter( stream ) ) {
                            writer.Write( this.Buffer, 0, this.BufferSize() );
                        }
                    }
                }
            }

            /// <summary>
            /// Returns true if the buffer has been allocated.
            /// </summary>
            /// <returns></returns>
            Boolean BufferCreated() => this.Buffer != null && this.Buffer.Length > 0;

            Int64 BufferSize() {
                if ( !this.BufferCreated() ) {
                    this.CreateBuffer();
                }

                var buffer = this.Buffer;
                if ( buffer != null ) {
                    return buffer.LongLength;
                }

                throw new InvalidOperationException( $"Could not allocate a {this.BufferSize()} buffer" );
            }

            Int64 ChunksNeeded() {
                var needed = Math.Ceiling( this.Size() / ( Double )this.BufferSize() );
                if ( needed > Int64.MaxValue ) {
                    throw new ArgumentOutOfRangeException( nameof( this.ChunksNeeded ) ); //should never happen..
                }

                return ( Int64 )needed;
            }

            Boolean CreateBuffer() {
                this.Buffer = new Byte[this.OptimalBufferSize()];
                if ( Debugger.IsAttached ) {
                    Debug.WriteLine( $"Created {this.BufferSize()} byte buffer for {this.Document.FullPathWithFileName}." );
                }

                return this.BufferCreated();
            }


        /// <summary>
        /// Dispose any disposable managed fields or properties.
        /// </summary>
        public override void DisposeManaged() {
            this.Buffer = null;
            base.DisposeManaged();
        }

        /// <summary>
        /// Not really 'optimal'. Just sounds better lol.
        /// </summary>
        /// <returns></returns>
        public Int64 OptimalBufferSize() {
            var size = this.Size();
            if ( size <= UInt16.MaxValue ) {
                return size;
            }

            return UInt16.MaxValue * 1048576UL; //64mb buffer
        }

        /// <summary>
        /// Return the difference between <see cref="OffsetEnd"/> and <see cref="OffsetBegin"/>.
        /// </summary>
        /// <returns></returns>
        public Int64 Size() => this.OffsetEnd - this.OffsetBegin;
    }
}