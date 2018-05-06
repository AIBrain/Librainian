// Copyright 2018 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/SafeSearchHandle.cs" was last cleaned by Rick on 2018/03/14 at 6:59 PM

namespace Librainian.Extensions {
	using System;
	using Microsoft.Win32.SafeHandles;
	using OperatingSystem;

	/// <summary>
	///     Class to encapsulate a seach handle returned from FindFirstFile. Using a wrapper like this
	///     ensures that the handle is properly cleaned up with FindClose.
	/// </summary>
	public class SafeSearchHandle : SafeHandleZeroOrMinusOneIsInvalid {
		public SafeSearchHandle() : base( ownsHandle: true ) { }

		protected override Boolean ReleaseHandle() => NativeMethods.FindClose( hFindFile: this.handle );
	}
}