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
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "ScrollingRichTextBox.cs" last formatted on 2022-12-22 at 5:15 PM by Protiguous.

namespace Librainian.Controls;

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Exceptions;
using OperatingSystem;

/// <summary></summary>
/// <see cref="http://stackoverflow.com/a/55540909/956364" />
public class ScrollingRichTextBox : RichTextBox {

	private const Int32 _SB_BOTTOM = 7;

	private const Int32 _WM_VSCROLL = 277;

	public enum NewLineOrNot {

		Neither = 0,

		Append,

		NewLine
	}

	public CircularLinesLogger? CircularLogger { get; set; }

	[DllImport( DLL.User32, CharSet = CharSet.Auto )]
	private static extern IntPtr SendMessage( IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam );

	public Boolean AttachCircularLinesLogger( ICircularLinesLogger circularLinesLogger ) {
		this.CircularLogger = circularLinesLogger as CircularLinesLogger ?? throw new ArgumentNullException( nameof( circularLinesLogger ) );
		return this.CircularLogger is not null;
	}

	/// <summary>Scrolls to the bottom of the RichTextBox.</summary>
	public void ScrollToBottom() => SendMessage( this.Handle, _WM_VSCROLL, new IntPtr( _SB_BOTTOM ), new IntPtr( 0 ) );

	public void Write( String message, NewLineOrNot newline = NewLineOrNot.Append ) {
		if ( this.CircularLogger is null ) {
			throw new NullException( nameof( this.CircularLogger ) );
		}

		this.CircularLogger.AppendToLog( message );

		this.InvokeAction( () => {
			var newText = this.CircularLogger.AsText();

			//if ( newText.Length > 65536 ) {
			//	newText = newText[ newText.Length.Half().. ];
			//}
			if ( newline == NewLineOrNot.NewLine ) {
				newText += Environment.NewLine;
			}

			this.SuspendLayout();
			this.Text( newText, RefreshOrInvalidate.Neither );
			this.ResumeLayout();
			this.ScrollToBottom();
		}, RefreshOrInvalidate.Invalidate );
	}
}