// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ProgressStream.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ProgressStream.cs" was last formatted by Protiguous on 2018/07/13 at 1:33 AM.

namespace Librainian.OperatingSystem.Streams
{

    using JetBrains.Annotations;
    using Measurement.Frequency;
    using System;
    using System.ComponentModel;
    using System.IO;
    using Logging;

    public sealed class ProgressStream : ContainerStream
    {

        private Int32 _lastProgress;

        private DateTime _lastProgressUpdate = DateTime.UtcNow.AddSeconds(-1);

        public ProgressChangedEventHandler ProgressChanged { get; set; }

        public ProgressStream([NotNull] Stream stream) : base(stream: stream)
        {
            if (stream.CanRead && stream.CanSeek && stream.Length > 0) { return; }

            stream.Break();

            throw new ArgumentException("stream");
        }

        public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
        {
            var amountRead = this.Stream.Read(buffer, offset, count);

            var newProgress = (Int32)(1024.0 * (this.Position / (Double)this.Length));

            if (newProgress <= this._lastProgress || DateTime.UtcNow - this._lastProgressUpdate < Hertz.Sixty) { return amountRead; }

            this._lastProgressUpdate = DateTime.UtcNow;
            this._lastProgress = newProgress;
            var progressChanged = this.ProgressChanged;
            progressChanged?.Invoke(this, new ProgressChangedEventArgs(this._lastProgress, null));

            return amountRead;
        }

        /// <summary>
        ///     When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <returns>The new position within the current stream.</returns>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">
        ///     A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to
        ///     obtain the new position.
        /// </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The stream does not support seeking, such as if the stream is
        ///     constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        /// <filterpriority>1</filterpriority>
        public override Int64 Seek(Int64 offset, SeekOrigin origin) => this.Stream.Seek(offset: offset, origin: origin);

        /// <summary>
        ///     When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The stream does not support both writing and seeking, such as if the
        ///     stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        /// <filterpriority>2</filterpriority>
        public override void SetLength(Int64 value) => this.Stream.SetLength(value);

        /// <summary>
        ///     When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current
        ///     position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        ///     An array of bytes. This method copies <paramref name="count" /> bytes from
        ///     <paramref name="buffer" /> to the current stream.
        /// </param>
        /// <param name="offset">
        ///     The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the
        ///     current stream.
        /// </param>
        /// <param name="count"> The number of bytes to be written to the current stream.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is
        ///     greater than the buffer length.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> == null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="offset" /> or <paramref name="count" /> is
        ///     negative.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        /// <filterpriority>1</filterpriority>
        public override void Write(Byte[] buffer, Int32 offset, Int32 count) => this.Stream.Write(buffer: buffer, offset: offset, count: count);
    }
}