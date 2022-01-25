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
// File "Utility.cs" last formatted on 2022-12-22 at 5:15 PM by Protiguous.

#nullable enable

namespace Librainian.Extensions;

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using Logging;

public static class Utility {

	private static ReaderWriterLockSlim ConsoleOutputSynch { get; } = new( LockRecursionPolicy.SupportsRecursion );

	private static DummyXMLResolver DummyXMLResolver { get; } = new();

	/// <summary>Output the <paramref name="text" /> at the end of the current <see cref="Console" /> line.</summary>
	/// <param name="text">   </param>
	/// <param name="yOffset"></param>
	public static void AtEndOfLine( this String? text, Int32 yOffset = 0 ) {
		if ( String.IsNullOrEmpty( text ) ) {
			return;
		}

		try {
			ConsoleOutputSynch.EnterUpgradeableReadLock();
			var oldTop = Console.CursorTop;
			var oldLeft = Console.CursorLeft;

			try {
				ConsoleOutputSynch.EnterWriteLock();
				Console.CursorVisible = false;
				yOffset = oldTop + yOffset;

				while ( yOffset < 0 ) {
					yOffset++;
				}

				while ( yOffset >= Console.WindowHeight ) {
					yOffset--;
				}

				Console.SetCursorPosition( Console.WindowWidth - ( text.Length + 2 ), yOffset );
				Console.Write( text );
				Console.SetCursorPosition( oldLeft, oldTop );
				Console.CursorVisible = true;
			}
			catch ( ArgumentOutOfRangeException exception ) {
				exception.Log();
			}
			catch ( IOException exception ) {
				exception.Log();
			}
			catch ( SecurityException exception ) {
				exception.Log();
			}
			finally {
				ConsoleOutputSynch.ExitWriteLock();
			}
		}
		finally {
			ConsoleOutputSynch.ExitUpgradeableReadLock();
		}
	}

	/// <summary>
	///     Copy from one stream to another.
	///     Example:
	///     using(var stream = response.GetResponseStream())
	///     using(var ms = new RecyclableMemoryStream())
	///     {
	///     stream.CopyTo(ms);
	///     // Do something with copied data
	///     }
	/// </summary>
	/// <param name="fromStream">From stream.</param>
	/// <param name="toStream">To stream.</param>
	public static void CopyTo( this Stream fromStream, Stream toStream ) {
		if ( fromStream == null ) {
			throw new ArgumentNullException( nameof( fromStream ) );
		}

		if ( toStream == null ) {
			throw new ArgumentNullException( nameof( toStream ) );
		}

		var bytes = new Byte[ 8192 ];
		Int32 dataRead;
		while ( ( dataRead = fromStream.Read( bytes, 0, bytes.Length ) ) > 0 ) {
			toStream.Write( bytes, 0, dataRead );
		}
	}

	public static void OnSet<T>( this EventHandler<T> @event, Object sender, T e ) where T : EventArgs => throw new NotImplementedException();

	public static void Spin( String? text ) {
		var oldTop = Console.CursorTop;
		var oldLeft = Console.CursorLeft;
		Console.Write( text );
		Console.SetCursorPosition( oldLeft, oldTop );
	}

	public static void TopRight( String? text ) {
		if ( String.IsNullOrEmpty( text ) ) {
			return;
		}

		try {
			ConsoleOutputSynch.EnterUpgradeableReadLock();
			var oldTop = Console.CursorTop;
			var oldLeft = Console.CursorLeft;

			try {
				ConsoleOutputSynch.EnterWriteLock();
				Console.CursorVisible = false;
				Console.SetCursorPosition( Console.WindowWidth - ( text.Length + 2 ), 0 );
				Console.Write( text );
				Console.SetCursorPosition( oldLeft, oldTop );
				Console.CursorVisible = true;
			}
			finally {
				ConsoleOutputSynch.ExitWriteLock();
			}
		}
		finally {
			ConsoleOutputSynch.ExitUpgradeableReadLock();
		}
	}

	public static void WriteColor( this String? text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black, params Object[]? parms ) {
		lock ( ConsoleOutputSynch ) {
			if ( parms?.Any() != true ) {

				//text.Info();
				var oldFore = Console.ForegroundColor;
				var oldBack = Console.BackgroundColor;
				Console.ForegroundColor = foreColor; //TODO d.r.y.
				Console.BackgroundColor = backColor; //TODO d.r.y.
				Console.Write( text );
				Console.BackgroundColor = oldBack;
				Console.ForegroundColor = oldFore;
			}
			else {

				//String.Format( text, parms ).Info();
				var oldFore = Console.ForegroundColor;
				var oldBack = Console.BackgroundColor;
				Console.ForegroundColor = foreColor;
				Console.BackgroundColor = backColor;
				Console.Write( text ?? String.Empty, parms );
				Console.BackgroundColor = oldBack;
				Console.ForegroundColor = oldFore;
			}
		}
	}

	public static void WriteLineColor( this String? text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black, params Object[]? parms ) {
		lock ( ConsoleOutputSynch ) {
			if ( parms?.Any() != true ) {

				//text.Info();
				var oldFore = Console.ForegroundColor;
				var oldBack = Console.BackgroundColor;
				Console.ForegroundColor = foreColor;
				Console.BackgroundColor = backColor;
				Console.WriteLine( text );
				Console.BackgroundColor = oldBack;
				Console.ForegroundColor = oldFore;
			}
			else {

				//String.Format( text, parms ).Info();
				var oldFore = Console.ForegroundColor;
				var oldBack = Console.BackgroundColor;
				Console.ForegroundColor = foreColor;
				Console.BackgroundColor = backColor;
				Console.WriteLine( text ?? String.Empty, parms );
				Console.BackgroundColor = oldBack;
				Console.ForegroundColor = oldFore;
			}
		}
	}
}