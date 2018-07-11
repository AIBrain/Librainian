// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "Chunk.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "Chunk.cs" was last formatted by Protiguous on 2018/06/26 at 12:54 AM.

namespace Librainian.ComputerSystem.FileSystem {

	using System;
	using System.Collections.Concurrent;
	using System.Diagnostics;
	using System.Linq;
	using Exceptions;
	using Extensions;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Magic;
	using Maths;

	/// <summary>
	///     For copying a <see cref="Chunk" /> of a <see cref="Document" /> into another <see cref="Document" />.
	/// </summary>
	public class Chunk : ABetterClassDispose {

		[NotNull]
		public Document Document { get; }

#pragma warning disable IDE0015 // Use framework type

		/// <summary>
		///     Starting position in file. Limited to <see cref="int.MaxValue" /> (even though this is a <see cref="long" />).
		/// </summary>
		public Int64 OffsetBegin {
#pragma warning restore IDE0015 // Use framework type
			get => this._offsetBegin;

			set {
				if ( value == this.OffsetEnd ) {
					throw new ArgumentOutOfRangeException( nameof( this.OffsetBegin ), $"{this.OffsetBegin} cannot be equal to {nameof( this.OffsetEnd )}." );
				}

				if ( value > this.OffsetEnd ) {
					throw new ArgumentOutOfRangeException( nameof( this.OffsetBegin ), $"Offset {value:N0} is greater than {nameof( Int64.MaxValue )}." );
				}

				this._offsetBegin = value;
			}
		}

		/// <summary>
		///     Ending position in file.
		/// </summary>
		public Int64 OffsetEnd {
			get => this._offsetEnd;

			set {
				if ( value == this.OffsetBegin ) {
					throw new ArgumentOutOfRangeException( nameof( this.OffsetBegin ), $"{this.OffsetEnd} cannot be equal to {nameof( this.OffsetBegin )}." );
				}

				if ( value < this.OffsetBegin ) {
					throw new ValueTooHighException( $"{nameof( this.OffsetBegin )} {value:N0} is greater than {nameof( this.OffsetEnd )}." );
				}

				this._offsetEnd = value;
			}
		}

		[CanBeNull]
		public Byte[] ReadWriteBuffer { get; private set; }

		/// <summary>
		///     Just some common buffer sizes we might use.
		/// </summary>
		private static ConcurrentDictionary<Byte, Int64> GoodBufferSizes { get; } = new ConcurrentDictionary<Byte, Int64>();

		private Int64 _offsetBegin;

		private Int64 _offsetEnd = 1;

		private Int64 BufferSize( Int64? size ) {
			if ( !this.IsBufferCreated() ) {
				this.CreateOptimalBufferSize();
			}

			if ( this.ReadWriteBuffer != null ) {
				return this.ReadWriteBuffer.LongLength;
			}

			throw new InvalidOperationException( $"Could not allocate a {this.BufferSize( null )} buffer" );
		}

		private Int64 ChunksNeeded() {
			var size = this.Size();
			var needed = Math.Ceiling( size / ( Double ) this.BufferSize( size ) );

			if ( needed > Int64.MaxValue ) {
				throw new ArgumentOutOfRangeException( nameof( this.ChunksNeeded ) ); //should never happen..
			}

			return ( Int64 ) needed;
		}

		/// <summary>
		///     Not really 'optimal'.. Gets the largest buffer we can allocate. Up to 2^32 down to 4096 bytes.
		/// </summary>
		/// <returns></returns>
		private void CreateOptimalBufferSize() {

			this.ReadWriteBuffer = null;

			try {
				var computer = new Computer();
				var ram = computer.GetAvailableMemeory();

				foreach ( var l in GoodBufferSizes.OrderByDescending( pair => pair.Value ).Select( pair => pair.Value ) ) {
					try {
						Logging.Garbage();
						this.ReadWriteBuffer = new Byte[ l ];

						return;
					}
					catch ( OutOfMemoryException ) {
						this.ReadWriteBuffer = null;
					}
					finally {
						this.ReadWriteBuffer.Should().NotBeNull();

						if ( Debugger.IsAttached ) {
							Debug.WriteLine( $"Created {l:N0} byte buffer for {this.Document.FullPathWithFileName}." );
						}
					}
				}
			}
			catch ( Exception exception ) {
				exception.More();
			}

			this.ReadWriteBuffer = new Byte[ 4096 ]; //default. If we can't allocate this few of bytes, then we're in another bigger issue.
		}

		private Boolean IsBufferCreated() => this.ReadWriteBuffer?.Length.Any() == true;

		/*
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
        */

		/// <summary>
		///     Dispose any disposable managed fields or properties.
		/// </summary>
		public override void DisposeManaged() {
			this.ReadWriteBuffer = null;
			base.DisposeManaged();
		}

		/// <summary>
		///     Return the difference between <see cref="OffsetEnd" /> and <see cref="OffsetBegin" />.
		/// </summary>
		/// <returns></returns>
		public Int64 Size() => this.OffsetEnd - this.OffsetBegin;

		private const Byte High = 32;

		private const Byte Low = 11;

		static Chunk() {
			foreach ( var l in Low.To( High ) ) {
				GoodBufferSizes[ l ] = ( Int64 ) Math.Pow( 2, l );
			}
		}

		public Chunk( [NotNull] Document document ) {
			this.Document = document ?? throw new ArgumentNullException( nameof( document ) );

			this.CreateOptimalBufferSize();
		}
	}
}