// Copyright � Protiguous. All Rights Reserved.
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
// File "ConsoleCursorExtensions.cs" last formatted on 2021-11-30 at 7:16 PM by Protiguous.

namespace Librainian.Controls;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Exceptions;
using Maths;
using Measurement.Frequency;

public enum Speed {

	Slow,

	Regular,

	Fast
}

[SuppressMessage( "ReSharper", "InconsistentNaming" )]
public static class ConsoleCursorExtensions {

	public const Int32 MF_DISABLED = 0x00000002;

	public const Int32 MF_ENABLED = 0x00000000;

	public const Int32 MF_GRAYED = 0x1;

	public const Int32 SC_CLOSE = 0xF060;

	public const Int32 SC_MAXIMIZE = 0xF030;

	//disabled button status
	public const Int32 SC_MINIMIZE = 0xF020;

	public static Task MoveCursor( this Form form, Int32 x, Int32 y, TimeSpan speed ) {
		if ( form is null ) {
			throw new NullException( nameof( form ) );
		}

		return Task.Run( async () => {

			// Set the Current cursor, move the cursor's Position, and set its clipping rectangle to the form.
			var cx = Cursor.Position.X;
			var cy = Cursor.Position.Y;

			while ( true ) {
				if ( Cursor.Position.X == x && Cursor.Position.Y == y ) {
					break;
				}

				if ( Randem.NextBoolean() ) {
					if ( cx < x ) {
						var step = ( x - cx ) / 10.0f;

						if ( step < 1 ) {
							step = 1;
						}

						cx -= ( Int32 )step;
					}
					else {
						var step = ( cx - x ) / 10.0f;

						if ( step < 1 ) {
							step = 1;
						}

						cx += ( Int32 )step;
					}
				}
				else {
					if ( cy < y ) {
						var step = ( y - cy ) / 10.0f;

						if ( step < 1 ) {
							step = 1;
						}

						cy -= ( Int32 )step;
					}
					else {
						var step = ( cy - y ) / 10.0f;

						if ( step < 1 ) {
							step = 1;
						}

						cy += ( Int32 )step;
					}
				}

				Cursor.Position = new Point( cx, cy );

				await Task.Delay( Hertz.Sixty ).ConfigureAwait( false );
			}
		} );
	}
}