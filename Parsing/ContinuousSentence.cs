#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/ContinuousSentence.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Parsing {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using Collections;
    using JetBrains.Annotations;

    /// <summary>
    ///     A thread-safe object to contain a moving target of sentences.
    ///     I'd like to make this act like a <see cref="Stream" /> if possible?
    /// </summary>
    [DataContract( IsReference = true )]
    public class ContinuousSentence {
        public static IEnumerable< string > EndOfSentencesEnglishUS { get; } = new[] { ".", "?", "!", "lol", ":)", ";)", ":P" };

        [DataMember]  private readonly ReaderWriterLockSlim _access = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        [DataMember]  private String _currentSentence = String.Empty;

        public ContinuousSentence( [NotNull] String paragraph = "" ) {
            if ( paragraph == null ) {
                throw new ArgumentNullException( "paragraph" );
            }
            this.CurrentSentence = paragraph;
        }

        public String CurrentSentence {
            get {
                try {
                    this._access.EnterReadLock();
                    return this._currentSentence;
                }
                finally {
                    this._access.ExitReadLock();
                }
            }
            set {
                try {
                    this._access.EnterWriteLock();
                    this._currentSentence = value;
                }
                finally {
                    this._access.ExitWriteLock();
                }
            }
        }

        /// <summary>
        ///     Append the <paramref name="text" /> to the current sentence buffer.
        /// </summary>
        /// <returns></returns>
        public ContinuousSentence Add( [NotNull] String text ) {
            if ( text == null ) {
                throw new ArgumentNullException( "text" );
            }
            var temp = new List< String >();
            if ( !String.IsNullOrEmpty( this.CurrentSentence ) ) {
                temp.Add( this.CurrentSentence );
            }
            if ( !String.IsNullOrEmpty( text ) ) {
                temp.Add( text );
            }
            this.CurrentSentence = temp.ToStrings( " " );
            return this;
        }

        public String PeekNextChar() => new String( new[] { this.CurrentSentence.FirstOrDefault() } );

        /// <summary>
        ///     for now, find the next .?!
        /// </summary>
        /// <returns></returns>
        public String PeekNextSentence( int noMoreThanXWords = 10 ) {
            try {
                this._access.EnterReadLock();

                var sentence = this._currentSentence.FirstSentence();

                if ( String.IsNullOrEmpty( sentence ) ) {
                    if ( this._currentSentence.WordCount() >= noMoreThanXWords ) {
                        sentence = this._currentSentence;
                        return sentence;
                    }
                }
                else {
                    if ( EndOfSentencesEnglishUS.Any( sentence.EndsWith ) || this._currentSentence.WordCount() >= noMoreThanXWords ) {
                        return sentence;
                    }
                }
                return String.Empty;
            }
            finally {
                this._access.ExitReadLock();
            }
        }

        public String PeekNextWord() {
            var word = this.CurrentSentence.FirstWord();
            return String.IsNullOrEmpty( word ) ? String.Empty : word;
        }

        public String PullNextChar() {
            try {
                this._access.EnterWriteLock();
                if ( String.IsNullOrEmpty( this._currentSentence ) ) {
                    return String.Empty;
                }

                var result = new String( new[] { this._currentSentence.FirstOrDefault() } );
                if ( !String.IsNullOrEmpty( result ) ) {
                    this._currentSentence = this._currentSentence.Remove( 0, 1 );
                }
                return result;
            }
            finally {
                this._access.ExitWriteLock();
            }
        }

        /// <summary>
        ///     for now, find the next .?!
        /// </summary>
        /// <returns></returns>
        public String PullNextSentence( int noMoreThanXWords = 10 ) {
            try {
                this._access.EnterUpgradeableReadLock();

                var sentence = this.PeekNextSentence( noMoreThanXWords: noMoreThanXWords );

                if ( !String.IsNullOrEmpty( sentence ) ) {
                    var position = this._currentSentence.IndexOf( sentence, StringComparison.Ordinal );
                    this.CurrentSentence = this._currentSentence.Substring( position + sentence.Length );
                    return sentence;
                }
                return String.Empty;
            }
            finally {
                this._access.ExitUpgradeableReadLock();
            }
        }

        public String PullNextWord() {
            var sentence = this.CurrentSentence; //grab a copy of the String at this moment in time

            var word = sentence.FirstWord();

            if ( String.IsNullOrEmpty( word ) ) {
                return String.Empty;
            }

            var position = sentence.IndexOf( word, StringComparison.Ordinal );
            if ( position >= 0 ) {
                this.CurrentSentence = this.CurrentSentence.Remove( 0, word.Length ).TrimStart();
            }

            return word;
        }
    }
}
