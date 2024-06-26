﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "Message.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

namespace Librainian.Interfaces;

using System;
using System.Drawing;
using Utilities.Disposables;

public record Message : ABetterRecordDispose, IMessage {
	public Message( SourceRecord source, String data ) {
		this.ID = Guid.NewGuid();
		this.Date = DateTime.UtcNow;
		this.Source = source;
		this.Data = data;
	}

	/// <summary>Optional color to be used.</summary>
	public Color? BackgroundColor { get; set; }

	/// <summary>The data for this message. Usually a string.</summary>
	public String Data { get; init; }

	/// <summary>The message's source id.. (could be the user's name)</summary>
	public String? Description { get; set; }

	/// <summary>Optional color to be used.</summary>
	public Color? ForegroundColor { get; set; }

	/// <summary>Guid assigned on message creation.</summary>
	public Guid ID { get; }

	public Boolean Processed { get; set; }

	public DateTime? ProcessingEnded { get; set; }

	public DateTime? ProcessingStarted { get; set; }

	/// <summary>This message is in reference to.</summary>
	public Guid ReferenceMessageID { get; set; }

	public SourceRecord Source { get; set; }

	public DateTimeOffset Date { get; }
}