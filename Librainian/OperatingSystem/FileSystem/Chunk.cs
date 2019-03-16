// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Chunk.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Chunk.cs" was last formatted by Protiguous on 2018/07/10 at 8:53 PM.

namespace Librainian.OperatingSystem.FileSystem
{

    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using ComputerSystem;
    using Exceptions;
    using JetBrains.Annotations;
    using Logging;
    using Magic;
    using Maths;

    /// <summary>
    ///     For copying a <see cref="Chunk" /> of a <see cref="Document" /> into another <see cref="Document" />.
    /// </summary>
    public class Chunk : ABetterClassDispose {

        private UInt64 _offsetBegin;

        private UInt64 _offsetEnd;

        [NotNull]
        public Document Document { get; }

        /// <summary>
        ///     Starting position in file. Limited to <see cref="Int32.MaxValue" /> (even though this is a <see cref="UInt64" />).
        /// </summary>
        public UInt64 OffsetBegin {
            get => this._offsetBegin;

            set {
                if (value == this.OffsetEnd) { throw new ArgumentOutOfRangeException(nameof(this.OffsetBegin), $"{this.OffsetBegin} cannot be equal to {nameof(this.OffsetEnd)}."); }

                if (value > this.OffsetEnd) { throw new ArgumentOutOfRangeException(nameof(this.OffsetBegin), $"Offset {value:N0} is greater than {nameof(UInt64.MaxValue)}."); }

                this._offsetBegin = value;
            }
        }

        /// <summary>
        ///     Ending position in file.
        /// </summary>
        public UInt64 OffsetEnd {
            get => this._offsetEnd;

            set {
                if (value == this.OffsetBegin) { throw new ArgumentOutOfRangeException(nameof(this.OffsetBegin), $"{this.OffsetEnd} cannot be equal to {nameof(this.OffsetBegin)}."); }

                if (value < this.OffsetBegin) { throw new ValueTooHighException($"{nameof(this.OffsetBegin)} {value:N0} is greater than {nameof(this.OffsetEnd)}."); }

                this._offsetEnd = value;
            }
        }

        [CanBeNull]
        public Byte[] ReadWriteBuffer { get; private set; }

        /// <summary>
        ///     Just some common buffer sizes we might use.
        /// </summary>
        private static ConcurrentDictionary<Byte, UInt64> GoodBufferSizes { get; } = new ConcurrentDictionary<Byte, UInt64>();

        private const Byte High = 32;

        private const Byte Low = 11;

        static Chunk()
        {
            foreach (var l in Low.To(High)) { GoodBufferSizes[l] = (UInt64)Math.Pow(2, l); }
        }

        public Chunk([NotNull] Document document)
        {
            this.Document = document ?? throw new ArgumentNullException(nameof(document));

            this.CreateOptimalBufferSize();
        }

        private UInt64 BufferSize(UInt64? size)
        {
            if (!this.IsBufferCreated()) { this.CreateOptimalBufferSize(); }

            if (this.ReadWriteBuffer != null) { return ( UInt64 ) this.ReadWriteBuffer.LongLength; }

            throw new InvalidOperationException($"Could not allocate a {size} buffer");
        }

        private UInt64 ChunksNeeded()
        {
            var size = this.Size();
            var needed = Math.Ceiling(size / (Double)this.BufferSize(size));

            if (needed > UInt64.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(this.ChunksNeeded)); //should never happen..
            }

            return (UInt64)needed;
        }

        /// <summary>
        ///     Not really 'optimal'.. Gets the largest buffer we can allocate. Up to 2^32 down to 4096 bytes.
        /// </summary>
        /// <returns></returns>
        private void CreateOptimalBufferSize()
        {

            this.ReadWriteBuffer = null;

            try
            {
                var computer = new Computer();
                var ram = computer.GetAvailableMemeory();

                foreach (var l in GoodBufferSizes.Where(pair => pair.Value <= ram).OrderByDescending(pair => pair.Value).Select(pair => pair.Value))
                {
                    try {
                        GC.Collect( 2, GCCollectionMode.Forced, true );
                        this.ReadWriteBuffer = new Byte[l];

                        return;
                    }
                    catch (OutOfMemoryException) { this.ReadWriteBuffer = null; }
                    finally
                    {
                        //this.ReadWriteBuffer.Should().NotBeNull();

                        $"Created {l:N0} byte buffer for {this.Document.FullPath}.".Log();
                    }
                }
            }
            catch (Exception exception) { exception.Log(); }

            this.ReadWriteBuffer = new Byte[4096]; //default. If we can't allocate this few of bytes, then we're in another bigger issue.
        }

        private Boolean IsBufferCreated() => this.ReadWriteBuffer?.LongLength.Any() == true;

        /*
        public Boolean CopyTo( [NotNull] Document destination ) {
            if ( destination == null ) { throw new ArgumentNullException( paramName: nameof( destination ) ); }

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
        */

        /// <summary>
        ///     Dispose any disposable managed fields or properties.
        /// </summary>
        public override void DisposeManaged()
        {
            this.ReadWriteBuffer = null;
            base.DisposeManaged();
        }

        /// <summary>
        ///     Return the difference between <see cref="OffsetEnd" /> and <see cref="OffsetBegin" />.
        /// </summary>
        /// <returns></returns>
        public UInt64 Size() => this.OffsetEnd - this.OffsetBegin;
    }
}