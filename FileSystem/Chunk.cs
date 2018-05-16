// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Chunk.cs",
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
// "Librainian/Librainian/Chunk.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem {

    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using JetBrains.Annotations;
    using Magic;
    using OperatingSystem;

    /// <summary>
    ///     For copying a <see cref="Chunk" /> of a <see cref="Document" /> into another <see cref="Document" />.
    /// </summary>
    public class Chunk : ABetterClassDispose {

        private const Byte High = 32;

        private const Byte Low = 12;

        private Int64 _offsetBegin;

        private Int64 _offsetEnd = 1;

        static Chunk() {
            foreach ( var l in Low.To( High ) ) { BufferSizes[l] = ( Int64 )Math.Pow( 2, l ); }
        }

        public Chunk( [NotNull] Document document ) => this.Document = document ?? throw new ArgumentNullException( nameof( document ) );

        /// <summary>
        ///     Just some common buffer sizes we might use.
        /// </summary>
        private static ConcurrentDictionary<Byte, Int64> BufferSizes { get; } = new ConcurrentDictionary<Byte, Int64>();

        [CanBeNull]
        public Byte[] Buffer { get; private set; }

        [NotNull]
        public Document Document { get; }

        /// <summary>
        ///     Starting position in file. Limited to <see cref="Int32.MaxValue" /> (even though this is a <see cref="Int64" />).
        /// </summary>
        public Int64 OffsetBegin {
            get => this._offsetBegin;

            set {
                if ( value == this.OffsetEnd ) { throw new ArgumentOutOfRangeException( nameof( this.OffsetBegin ), $"{this.OffsetBegin} cannot be equal to {nameof( this.OffsetEnd )}." ); }

                if ( value > this.OffsetEnd ) { throw new ArgumentOutOfRangeException( nameof( this.OffsetBegin ), $"Offset {value:N0} is greater than {nameof( Int64.MaxValue )}." ); }

                this._offsetBegin = value;
            }
        }

        /// <summary>
        ///     Ending position in file.
        /// </summary>
        public Int64 OffsetEnd {
            get => this._offsetEnd;

            set {
                if ( value == this.OffsetBegin ) { throw new ArgumentOutOfRangeException( nameof( this.OffsetBegin ), $"{this.OffsetEnd} cannot be equal to {nameof( this.OffsetBegin )}." ); }

                if ( value < this.OffsetBegin ) { throw new ArgumentOutOfRangeException( nameof( this.OffsetBegin ), $"Offset {value:N0} is greater than {nameof( Int64.MaxValue )}." ); }

                this._offsetEnd = value;
            }
        }

        private Boolean BufferCreated() => this.Buffer != null && this.Buffer.Length > 0;

        private Int64 BufferSize() {
            if ( !this.BufferCreated() ) { this.CreateBuffer(); }

            var buffer = this.Buffer;

            if ( buffer != null ) { return buffer.LongLength; }

            throw new InvalidOperationException( $"Could not allocate a {this.BufferSize()} buffer" );
        }

        private Int64 ChunksNeeded() {
            var needed = Math.Ceiling( this.Size() / ( Double )this.BufferSize() );

            if ( needed > Int64.MaxValue ) {
                throw new ArgumentOutOfRangeException( nameof( this.ChunksNeeded ) ); //should never happen..
            }

            return ( Int64 )needed;
        }

        private Boolean CreateBuffer() {
            var l = this.CreateOptimalBufferSize();

            return this.BufferCreated();
        }

        /// <summary>
        ///     Not really 'optimal'.. Gets the largest buffer we can allocate. Up to 2^32 down to 4096 bytes.
        /// </summary>
        /// <returns></returns>
        private Int64 CreateOptimalBufferSize() {

            var ram = Computer.GetAvailableMemeory();

            this.Buffer = null;

            foreach ( var l in BufferSizes.OrderByDescending( pair => pair.Value ).Select( pair => pair.Value ) ) {
                try {
                    Logging.Garbage();
                    this.Buffer = new Byte[l];

                    return l;
                }
                catch ( OutOfMemoryException ) { this.Buffer = null; }
                finally {
                    this.Buffer.Should().NotBeNull();

                    if ( Debugger.IsAttached ) { Debug.WriteLine( $"Created {l:N0} byte buffer for {this.Document.FullPathWithFileName}." ); }
                }
            }

            return 4096; //default
        }

        public Boolean CopyTo( [NotNull] Document destination ) {
            if ( destination is null ) { throw new ArgumentNullException( paramName: nameof( destination ) ); }

            if ( !destination.Exists() ) {
                using ( var mmfOut = MemoryMappedFile.CreateNew( destination.JustName(), this.Document.Length, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.DelayAllocatePages,
                    HandleInheritability.Inheritable ) ) {
                    using ( var stream = mmfOut.CreateViewStream() ) {
                        stream.Seek( this.OffsetBegin, SeekOrigin.Begin );

                        using ( var writer = new BinaryWriter( stream ) ) { writer.Write( this.Buffer, 0, this.BufferSize() ); }
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     Dispose any disposable managed fields or properties.
        /// </summary>
        public override void DisposeManaged() {
            this.Buffer = null;
            base.DisposeManaged();
        }

        /// <summary>
        ///     Return the difference between <see cref="OffsetEnd" /> and <see cref="OffsetBegin" />.
        /// </summary>
        /// <returns></returns>
        public Int64 Size() => this.OffsetEnd - this.OffsetBegin;
    }
}