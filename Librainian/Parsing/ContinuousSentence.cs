﻿// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "ContinuousSentence.cs" last formatted on 2020-08-14 at 8:41 PM.

#nullable enable

namespace Librainian.Parsing {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Utilities;

	/// <summary>
	///     A thread-safe object to contain a moving target of sentences.
	/// </summary>
	[JsonObject]
	public class ContinuousSentence : ABetterClassDispose {

		//TODO this class *really* needs updated

		[JsonProperty]
		[NotNull]
		private String _inputBuffer = String.Empty;

		[JsonIgnore]
		[NotNull]
		private ReaderWriterLockSlim AccessInputBuffer { get; } = new( LockRecursionPolicy.SupportsRecursion );

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
			text ??= String.Empty;

			this.CurrentBuffer += text;

			return this;
		}

		/// <summary>Dispose any disposable members.</summary>
		public override void DisposeManaged() {
			using ( this.AccessInputBuffer ) { }
		}

		[NotNull]
		public String PeekNextChar() =>
			new( new[] {
				this.CurrentBuffer.FirstOrDefault()
			} );

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

		[NotNull]
		public String PeekNextWord() {
			var word = this.CurrentBuffer.ToWords().FirstOrDefault();

			return word ?? String.Empty;
		}

		[NotNull]
		public String PullNextChar() {
			try {
				this.AccessInputBuffer.EnterWriteLock();

				if ( String.IsNullOrEmpty( this._inputBuffer ) ) {
					return String.Empty;
				}

				var result = new String( new[] {
					this._inputBuffer.FirstOrDefault()
				} );

				if ( !String.IsNullOrEmpty( result ) ) {
					this._inputBuffer = this._inputBuffer.Remove( 0, 1 );
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

				if ( !String.IsNullOrWhiteSpace( sentence ) ) {
					var position = this._inputBuffer.IndexOf( sentence, StringComparison.Ordinal );
					this.CurrentBuffer = this._inputBuffer[ ( position + sentence.Length ).. ];

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