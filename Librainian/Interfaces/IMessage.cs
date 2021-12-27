// Copyright © Protiguous. All Rights Reserved.
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
// File "IMessage.cs" last touched on 2021-09-08 at 1:53 PM by Protiguous.

namespace Librainian.Interfaces;

using System;
using Parsing;

public interface IMessage : IColored, IDisposable {

	/// <summary>The data for this message. Usually a string.</summary>
	String Data { get; init; }

	/// <summary>The UTC when this message was created.</summary>
	DateTime Date { get; }

	/// <summary>
	///     The message's source identifier.. (like the user's name)
	/// </summary>
	String? Description { get; set; }

	/// <summary>Guid assigned on message creation.</summary>
	Guid ID { get; }

	Boolean Processed { get; set; }

	DateTime? ProcessingEnded { get; set; }

	DateTime? ProcessingStarted { get; set; }

	/// <summary>This message is in reference to.</summary>
	Guid ReferenceMessageID { get; set; }

	SourceRecord Source { get; set; }

	TimeSpan? ProcessingTime() {
		if ( this.ProcessingStarted is null || this.ProcessingEnded is null ) {
			return default( TimeSpan? );
		}

		return this.ProcessingEnded.Value - this.ProcessingStarted.Value;
	}

	/// <summary>
	///     Default layout for <see cref="IMessage" /> interface.
	///     <para>"override" in derived class to change what is shown.</para>
	/// </summary>
	String ToString() => $"{nameof( Message )} {this.ID:D} from {this.Source:G} ({this.Description}){Environment.NewLine}{Symbols.VerticalEllipsis}{this.Data}";

}