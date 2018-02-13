// Copyright 2017 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks goes
// to the Authors.
//
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ResultCode.cs" was last cleaned by Rick on 2017/04/23 at 7:31 AM

namespace Librainian.FileSystem {

	/// <summary>
	/// Any result less than 1 is an error of some sort.
	/// </summary>
	public enum ResultCode {
		FailureUnknown = -11,

		FailureUnableToSetLastWriteTime = -10,

		FailureUnableToSetLastAccessTime = -9,

		FailureUnableToDeleteSourceDocument = -8,

		FailureSourceDoesNotExist = -7,

		FailureSourceIsEmpty = -6,

		FailureOnCopy = -5,

		FailureDestinationDoesNotExist = -4,

		FailureDestinationSizeIsDifferent = -3,

		FailureUnableToSetFileAttributes = -2,

		FailureUnableToSetFileCreationTime = -1,

		Success = 1
	}
}
