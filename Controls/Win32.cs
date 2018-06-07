// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Win32.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
//
// ***  Project "Librainian"  ***
// File "Win32.cs" was last formatted by Protiguous on 2018/06/04 at 3:49 PM.

namespace Librainian.Controls {

	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;
	using System.Threading.Tasks;
	using System.Windows.Forms;
	using JetBrains.Annotations;
	using Maths;
	using Measurement.Frequency;

	public enum Speed {

		Slow,

		Regular,

		Fast
	}

	public static class Win32 {

		[StructLayout( LayoutKind.Sequential )]
		public struct POINT {

			public readonly Int32 x;

			public readonly Int32 y;
		}

		// ReSharper disable once InconsistentNaming
		public const Int32 MF_DISABLED = 0x00000002;

		// ReSharper disable InconsistentNaming
		public const Int32 MF_ENABLED = 0x00000000;

		public const Int32 MF_GRAYED = 0x1;

		public const Int32 SC_CLOSE = 0xF060;

		public const Int32 SC_MAXIMIZE = 0xF030;

		//disabled button status
		public const Int32 SC_MINIMIZE = 0xF020;

		public static Task MoveCursor( [NotNull] this Form form, Int32 x, Int32 y, TimeSpan speed ) {
			if ( form is null ) { throw new ArgumentNullException( nameof( form ) ); }

			return Task.Run( () => {

				// Set the Current cursor, move the cursor's Position, and set its clipping rectangle to the form.
				var cx = Cursor.Position.X;
				var cy = Cursor.Position.Y;

				while ( true ) {
					if ( Cursor.Position.X == x && Cursor.Position.Y == y ) { break; }

					if ( Randem.NextBoolean() ) {
						if ( cx < x ) {
							var step = ( x - cx ) / 10.0f;

							if ( step < 1 ) { step = 1; }

							cx -= ( Int32 )step;
						}
						else {
							var step = ( cx - x ) / 10.0f;

							if ( step < 1 ) { step = 1; }

							cx += ( Int32 )step;
						}
					}
					else {
						if ( cy < y ) {
							var step = ( y - cy ) / 10.0f;

							if ( step < 1 ) { step = 1; }

							cy -= ( Int32 )step;
						}
						else {
							var step = ( cy - y ) / 10.0f;

							if ( step < 1 ) { step = 1; }

							cy += ( Int32 )step;
						}
					}

					Cursor.Position = new Point( cx, cy );
					Task.Delay( Hertz.Sixty ).Wait();
				}
			} );
		}

		//close button's code in Windows API

		//enabled button status

		//disabled button status (enabled = false)

		//for minimize button on forms

		//for maximize button on forms

		// ReSharper restore InconsistentNaming
	}
}