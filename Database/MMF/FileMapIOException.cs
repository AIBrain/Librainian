// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "FileMapIOException.cs" belongs to Rick@AIBrain.org and
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
// File "FileMapIOException.cs" was last formatted by Protiguous on 2018/06/04 at 3:50 PM.

namespace Librainian.Database.MMF {

	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using Newtonsoft.Json;

	/// <summary>
	///     An exception occured as a result of an invalid IO operation on any of the File mapping classes. It wraps the error
	///     message and the underlying Win32 error code that caused the error.
	/// </summary>
	[JsonObject]
	[Serializable]
	public class FileMapIOException : IOException {

		public override String Message {
			get {
				if ( this.Win32ErrorCode == 0 ) { return base.Message; }

				//if ( this.Win32ErrorCode == 0x80070008 ) {
				//    return base.Message + " Not enough address space available (" + this.Win32ErrorCode + ")";
				//}
				return base.Message + " (" + this.Win32ErrorCode.ToString( "X" ) + ")";
			}
		}

		public Int32 Win32ErrorCode { get; }

		protected FileMapIOException( SerializationInfo info, StreamingContext context ) : base( info, context ) { }

		// construction
		public FileMapIOException( Int32 error ) => this.Win32ErrorCode = error;

		public FileMapIOException( String message ) : base( message ) { }

		public FileMapIOException( String message, Exception innerException ) : base( message, innerException ) { }

	}

}