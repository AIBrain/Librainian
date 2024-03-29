﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
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
// File "Message.cs" last touched on 2021-02-10 at 3:06 PM by Protiguous.

namespace Librainian.Interfaces {

	using System;
	using System.Drawing;
	using JetBrains.Annotations;
	using Utilities;

	public class Message : ABetterClassDispose, IMessage {

		/// <summary>Optional color to be used.</summary>
		public Color? BackgroundColor { get; set; }

		/// <summary>The data for this message. Usually a string.</summary>
		[CanBeNull]
		public String? Data { get; set; }

		/// <summary>The UTC when this message was created.</summary>
		public DateTime Date { get; }

		/// <summary>The message's source id.. (like the user's name)</summary>
		[CanBeNull]
		public String? Description { get; set; }

		/// <summary>Optional color to be used.</summary>
		public Color? ForegroundColor { get; set; }

		/// <summary>Guid assigned on message creation.</summary>
		public Guid ID { get; }

		[CanBeNull]
		public String? Key { get; set; }

		public Boolean Processed { get; set; }

		[CanBeNull]
		public DateTime? ProcessingEnded { get; set; }

		[CanBeNull]
		public DateTime? ProcessingStarted { get; set; }

		/// <summary>This message is in reference to.</summary>
		public Guid ReferenceMessageID { get; set; }

		public SourceRecord Source { get; set; }

		public Message( SourceRecord source, [CanBeNull] String? sourceID = null ) {
			this.ID = Guid.NewGuid();
			this.Date = DateTime.UtcNow;
			this.Description = sourceID;
			this.Source = source;
		}

		[CanBeNull]
		public TimeSpan? ProcessingTime() => this.ProcessingEnded - this.ProcessingStarted;

		/// <summary>
		/// Default layout for <see cref="Message"/> object.
		/// </summary>
		/// <returns></returns>
		public override String ToString() =>
			this.Data is null ?
				$"{nameof( Message )} {this.ID:D} from {this.Source:G} ({this.Description})" :
				$"{nameof( Message )} {this.ID:D} from {this.Source:G} ({this.Description}){Environment.NewLine} {this.Data}";
	}
}