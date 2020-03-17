// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "ContinuousSentence.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", File: "ContinuousSentence.cs" was last formatted by Protiguous on 2020/03/16 at 2:59 PM.

namespace Librainian.Parsing {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Utilities;

    /// <summary>A thread-safe object to contain a moving target of sentences. I'd like to make this act like a <see cref="Stream" /> if possible?</summary>
    [JsonObject]
    public class ContinuousSentence : ABetterClassDispose {

        //TODO this class *really* needs updated

        [JsonProperty]
        [NotNull]
        private String _inputBuffer = String.Empty;

        [JsonIgnore]
        [NotNull]
        private ReaderWriterLockSlim AccessInputBuffer { get; } = new ReaderWriterLockSlim( recursionPolicy: LockRecursionPolicy.SupportsRecursion );

        public static IEnumerable<String> EndOfUSEnglishSentences { get; } = new[] {
            ".", "?", "!"
        };

        [NotNull]
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

        public ContinuousSentence( [CanBeNull] String? startingInput = null ) => this.CurrentBuffer = startingInput ?? String.Empty;

        /// <summary>Append the <paramref name="text" /> to the current sentence buffer.</summary>
        /// <returns></returns>
        [NotNull]
        public ContinuousSentence Add( [CanBeNull] String? text ) {
            if ( text is null ) {
                text = String.Empty;
            }

            this.CurrentBuffer += text;

            return this;
        }

        /// <summary>Dispose any disposable members.</summary>
        public override void DisposeManaged() {
            using ( this.AccessInputBuffer ) { }
        }

        [NotNull]
        public String PeekNextChar() =>
            new String( value: new[] {
                this.CurrentBuffer.FirstOrDefault()
            } );

        [NotNull]
        public String PeekNextSentence() {
            try {
                this.AccessInputBuffer.EnterReadLock();

                var sentence = this.CurrentBuffer.FirstSentence();

                return String.IsNullOrEmpty( value: sentence ) ? String.Empty : sentence;
            }
            finally {
                this.AccessInputBuffer.ExitReadLock();
            }
        }

        [NotNull]
        public String PeekNextWord() {
            var word = this.CurrentBuffer.FirstWord();

            return String.IsNullOrEmpty( value: word ) ? String.Empty : word;
        }

        [NotNull]
        public String PullNextChar() {
            try {
                this.AccessInputBuffer.EnterWriteLock();

                if ( String.IsNullOrEmpty( value: this._inputBuffer ) ) {
                    return String.Empty;
                }

                var result = new String( value: new[] {
                    this._inputBuffer.FirstOrDefault()
                } );

                if ( !String.IsNullOrEmpty( value: result ) ) {
                    this._inputBuffer = this._inputBuffer.Remove( startIndex: 0, count: 1 );
                }

                return result;
            }
            finally {
                this.AccessInputBuffer.ExitWriteLock();
            }
        }

        [NotNull]
        public String PullNextSentence() {
            try {
                this.AccessInputBuffer.EnterUpgradeableReadLock();

                var sentence = this.PeekNextSentence();

                if ( !String.IsNullOrWhiteSpace( value: sentence ) ) {
                    var position = this._inputBuffer.IndexOf( value: sentence, comparisonType: StringComparison.Ordinal );
                    this.CurrentBuffer = this._inputBuffer.Substring( startIndex: position + sentence.Length );

                    return sentence;
                }

                return String.Empty;
            }
            finally {
                this.AccessInputBuffer.ExitUpgradeableReadLock();
            }
        }
    }
}