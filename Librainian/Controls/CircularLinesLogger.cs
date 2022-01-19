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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "CircularLinesLogger.cs" last formatted on 2022-12-22 at 5:14 PM by Protiguous.

namespace Librainian.Controls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Maths;

/// <summary>
///     A circular buffer style logging class which stores N items for display in a Rich Text Box.
/// </summary>
/// <see cref="http://stackoverflow.com/a/55540909/956364" />
public class CircularLinesLogger : ICircularLinesLogger {

	private readonly Color _defaultColor = Color.White;

	private readonly Queue<LogEntry> _lines;

	private readonly Object _logLock = new();

	private readonly UInt32 _maxEntries;

	private UInt32 _entryNumber;

	/// <summary>
	///     Create an instance of the Logger class which stores <paramref name="maximumEntries" /> log entries.
	/// </summary>
	public CircularLinesLogger( UInt32 maximumEntries = 128 ) {
		this._lines = new Queue<LogEntry>();
		this._maxEntries = maximumEntries;
	}

	private static String ColorToRichColorString( Color c ) => $"\\red{c.R}\\green{c.G}\\blue{c.B};";

	private Dictionary<Color, ColorTableItem> BuildRichTextColorTable() {
		var uniqueColors = new Dictionary<Color, ColorTableItem>();
		var index = 0u;

		uniqueColors.Add( this._defaultColor, new ColorTableItem( index++, ColorToRichColorString( this._defaultColor ) ) );

		foreach ( var c in this._lines.Select( l => l.EntryColor ).Distinct().Where( c => c != this._defaultColor ) ) {
			uniqueColors.Add( c, new ColorTableItem( index++, ColorToRichColorString( c ) ) );
		}

		return uniqueColors;
	}

	/// <summary>
	///     Adds <paramref name="text" /> as a log entry.
	/// </summary>
	public void AddToLog( String text ) => this.AddToLog( text, this._defaultColor );

	/// <summary>
	///     Adds <paramref name="text" /> as a log entry, and specifies a color to display it in.
	/// </summary>
	public void AddToLog( String text, Color entryColor ) {
		lock ( this._logLock ) {
			if ( this._entryNumber >= UInt32.MaxValue ) {
				this._entryNumber = 0;
			}
			else {
				++this._entryNumber;
			}

			var logEntry = new LogEntry {
				EntryId = this._entryNumber,
				EntryTimeStamp = DateTime.Now,
				EntryText = text,
				EntryColor = entryColor
			};
			this._lines.Enqueue( logEntry );

			while ( this._lines.Count > this._maxEntries ) {
				this._lines.Dequeue();
			}
		}
	}

	public void AppendToLog( String text ) {
		lock ( this._logLock ) {
			if ( !this._lines.Any() ) {
				this.AddToLog( text );
				return;
			}

			var entry = this._lines.Last();
			entry.EntryText += $" {text}";
			if ( entry.EntryText.Length > 65536 ) {
				entry.EntryText = entry.EntryText[ entry.EntryText.Length.Half().. ];
			}
		}
	}

	public String AsText() {
		StringBuilder sb = new();
		lock ( this._logLock ) {
			foreach ( var logEntry in this._lines ) {
				sb.AppendLine( $"{logEntry.EntryText}" );
			}
		}

		return sb.ToString();
	}

	/// <summary>
	///     Clears the entire log.
	/// </summary>
	public void Clear() {
		lock ( this._logLock ) {
			this._lines.Clear();
		}
	}

	/// <summary>
	///     Retrieve the contents of the log as rich text, suitable for populating a
	///     <see cref="System.Windows.Forms.RichTextBox.Rtf" /> property.
	/// </summary>
	/// <param name="includeEntryNumbers">Option to prepend line numbers to each entry.</param>
	[Obsolete( "Does not work" )]
	public String GetLogAsRichText( Boolean includeEntryNumbers ) {
		lock ( this._logLock ) {
			var sb = new StringBuilder();

			var uniqueColors = this.BuildRichTextColorTable();
			sb.AppendLine( $@"{{\rtf1{{\colortbl;{String.Concat( uniqueColors.Select( d => d.Value.RichColor ) )}}}" );

			foreach ( var entry in this._lines ) {
				if ( includeEntryNumbers ) {
					sb.Append( $"\\cf1 {entry.EntryId}. " );
				}

				sb.Append( $"\\cf1 {entry.EntryTimeStamp.ToShortDateString()} {entry.EntryTimeStamp.ToShortTimeString()}: " );

				var richColor = $"\\cf{uniqueColors[ entry.EntryColor ].Index + 1}";
				sb.Append( $"{richColor} {entry.EntryText}\\par" ).AppendLine();
			}

			return sb.ToString();
		}
	}

	private record LogEntry {

		public Color EntryColor;

		public UInt32 EntryId;

		public String? EntryText;

		public DateTime EntryTimeStamp;

	}

	private record ColorTableItem( UInt32 Index, String RichColor );

}

public interface ICircularLinesLogger { }