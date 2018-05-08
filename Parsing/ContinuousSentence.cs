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
// "Librainian/ContinuousSentence.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

namespace Librainian.Parsing {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using JetBrains.Annotations;
    using Magic;
    using Newtonsoft.Json;

    /// <summary>
    ///     A thread-safe object to contain a moving target of sentences. I'd like to make this act like
    ///     a <see cref="Stream" /> if possible?
    /// </summary>
    [JsonObject]
    public class ContinuousSentence : ABetterClassDispose {

        [JsonProperty]
        private ReaderWriterLockSlim AccessInputBuffer { get; } = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        [JsonProperty]
        private String _inputBuffer = String.Empty;

        public ContinuousSentence( [CanBeNull] String startingInput = null ) => this.CurrentBuffer = startingInput ?? String.Empty;

	    public static IEnumerable<String> EndOfUSEnglishSentences { get; } = new[] { ".", "?", "!" };

        public String CurrentBuffer {
            get {
                try {
                    this.AccessInputBuffer.EnterReadLock();
                    return this._inputBuffer;
                }
                finally {
                    this.AccessInputBuffer.ExitReadLock();
                }
            }

            set {
                try {
                    this.AccessInputBuffer.EnterWriteLock();
                    this._inputBuffer = value;
                }
                finally {
                    this.AccessInputBuffer.ExitWriteLock();
                }
            }
        }

        /// <summary>Append the <paramref name="text" /> to the current sentence buffer.</summary>
        /// <returns></returns>
        public ContinuousSentence Add( [CanBeNull] String text ) {
            if ( text is null ) {
                text = String.Empty;
            }
            this.CurrentBuffer += text;
            return this;
        }

        public String PeekNextChar() => new String( new[] { this.CurrentBuffer.FirstOrDefault() } );

        [NotNull]
        public String PeekNextSentence() {
            try {
                this.AccessInputBuffer.EnterReadLock();

                var sentence = this.CurrentBuffer.FirstSentence();

                return String.IsNullOrEmpty( sentence ) ? String.Empty : sentence;
            }
            finally {
                this.AccessInputBuffer.ExitReadLock();
            }
        }

        public String PeekNextWord() {
            var word = this.CurrentBuffer.FirstWord();
            return String.IsNullOrEmpty( word ) ? String.Empty : word;
        }

        public String PullNextChar() {
            try {
                this.AccessInputBuffer.EnterWriteLock();
                if ( String.IsNullOrEmpty( this._inputBuffer ) ) {
                    return String.Empty;
                }

                var result = new String( new[] { this._inputBuffer.FirstOrDefault() } );
                if ( !String.IsNullOrEmpty( result ) ) {
                    this._inputBuffer = this._inputBuffer.Remove( 0, 1 );
                }
                return result;
            }
            finally {
                this.AccessInputBuffer.ExitWriteLock();
            }
        }

        public String PullNextSentence() {
            try {
                this.AccessInputBuffer.EnterUpgradeableReadLock();

                var sentence = this.PeekNextSentence();

                if ( !String.IsNullOrWhiteSpace( sentence ) ) {
                    var position = this._inputBuffer.IndexOf( sentence, StringComparison.Ordinal );
                    this.CurrentBuffer = this._inputBuffer.Substring( position + sentence.Length );
                    return sentence;
                }

                return String.Empty;
            }
            finally {
                this.AccessInputBuffer.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        protected override void DisposeManaged() {
            using ( this.AccessInputBuffer ) {
            }
        }

    }
}