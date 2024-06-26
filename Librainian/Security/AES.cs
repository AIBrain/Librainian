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
// File "AES.cs" last formatted on 2021-11-30 at 7:22 PM by Protiguous.

namespace Librainian.Security;

using System;
using System.Runtime.InteropServices;

public class AES {

	private IntPtr _algHandle;

	private IntPtr _keyHandle;

	[DllImport( "Bcrypt.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern UInt32 BCryptCloseAlgorithmProvider( [In] IntPtr phAlgorithm, [In] Int32 dwFlags );

	[DllImport( "Bcrypt.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern UInt32 BCryptDecrypt(
		[In][Out] IntPtr hKey,
		[In] Byte[] pbInput,
		[In] Int32 cbInput,
		[In] IntPtr pPaddingInfo,
		[In] Byte[] pbIV,
		[In] Int32 cbIV,
		[Out] Byte[] pbOutput,
		[In] Int32 cbOutput,
		[In][Out] ref Int32 pcbResult,
		[In] Int32 dwFlags
	);

	[DllImport( "Bcrypt.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern UInt32 BCryptEncrypt(
		[In][Out] IntPtr hKey,
		[In] Byte[] pbInput,
		[In] Int32 cbInput,
		[In] IntPtr pPaddingInfo,
		[In] Byte[] pbIV,
		[In] Int32 cbIV,
		[Out] Byte[] pbOutput,
		[In] Int32 cbOutput,
		[In][Out] ref Int32 pcbResult,
		[In] Int32 dwFlags
	);

	[DllImport( "Bcrypt.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern UInt32 BCryptGenerateSymmetricKey(
		[In] IntPtr hAlgorithm,
		[In][Out] ref IntPtr phKey,
		[Out] Byte[] pbKeyObject,
		[In] Int32 cbKeyObject,
		[In] Byte[] pbSecret,
		[In] Int32 cbSecret,
		[In] Int32 dwFlags
	);

	[DllImport( "Bcrypt.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern UInt32 BCryptGetProperty(
		[In] IntPtr hObject,
		[In] String pszProperty,
		[Out] Byte[] pbOutput,
		[In] Int32 cbOutput,
		[In][Out] ref Int32 pcbResult,
		[In] Int32 dwFlags
	);

	[DllImport( "Bcrypt.dll", CharSet = CharSet.Unicode, SetLastError = true )]
	public static extern UInt32 BCryptOpenAlgorithmProvider( [In][Out] ref IntPtr phAlgorithm, [In] String pszAlgId, [In] String pszImplementation, [In] Int32 dwFlags );

	public UInt32 Close() {
		var status = BCryptCloseAlgorithmProvider( this._algHandle, 0 );

		return status;
	}

	public UInt32 Decrypt( Int32 pcbCipherText, Byte[]? pbCipherText ) {

		//Initialize Initialization Vector
		Byte[] pbIV2 = {
			0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
			0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
		};

		//Initialize Plain Text Byte Count
		var pcbPlainText = 0;

		//Get Plain Text Byte Count
		BCryptDecrypt( this._keyHandle, pbCipherText, pcbCipherText, IntPtr.Zero, pbIV2, pbIV2.Length, null, 0, ref pcbPlainText, 0 );

		//Allocate Plain Text Buffer
		var pbPlainText = new Byte[ pcbPlainText ];

		//Decrypt The Data
		var status = BCryptDecrypt( this._keyHandle, pbCipherText, pcbCipherText, IntPtr.Zero, pbIV2, pbIV2.Length, pbPlainText, pbPlainText.Length, ref pcbPlainText, 0 );

		return status;
	}

	public UInt32 Encrypt( Byte[] pbData ) {

		//Initialize Data To Encrypt
		//Byte[] pbData = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

		//Initialize Initialization Vector
		Byte[] pbIV = {
			0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
			0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
		}; //16 bytes.

		//Initialize PaddingInfo

		//Initialize Cipher Text Byte Count
		var pcbCipherText = 0;

		//Get Cipher Text Byte Count
		BCryptEncrypt( this._keyHandle, pbData, pbData.Length, IntPtr.Zero, pbIV, pbIV.Length, null, 0, ref pcbCipherText, 0 );

		//Allocate Cipher Text Buffer
		var pbCipherText = new Byte[ pcbCipherText ];

		//Encrypt The Data
		var status = BCryptEncrypt( this._keyHandle, pbData, pbData.Length, IntPtr.Zero, pbIV, pbIV.Length, pbCipherText, pcbCipherText, ref pcbCipherText, 0 );

		return status;
	}

	public UInt32 Open() {

		//Open the Algorithm Provider

		//Initialize AlgHandle
		this._algHandle = IntPtr.Zero;

		//Initialize Status
		BCryptOpenAlgorithmProvider( ref this._algHandle, "AES", "Microsoft Primitive Provider", 0 );

		//Allocate DWORD for ObjectLength
		var pbObjectLength = new Byte[ 4 ];

		//Initialize ObjectLength Byte Count
		var pcbObjectLength = 0;

		//Get Algorithm Properties(BCRYPT_OBJECT_LENGTH)
		BCryptGetProperty( this._algHandle, "ObjectLength", pbObjectLength, pbObjectLength.Length, ref pcbObjectLength, 0 );

		//Initialize KeyHandle
		this._keyHandle = IntPtr.Zero;

		//Initialize Key Object Size with ObjectLength
		var keyObjectSize = ( pbObjectLength[ 3 ] << 24 ) | ( pbObjectLength[ 2 ] << 16 ) | ( pbObjectLength[ 1 ] << 8 ) | pbObjectLength[ 0 ];

		//Initialize AES Key
		Byte[] pbKey = {
			0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
			0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
		};

		//Allocate KeyObject With Key Object Size
		var pbKeyObject = new Byte[ keyObjectSize ];

		//Generate Symmetric Key Object
		var status = BCryptGenerateSymmetricKey( this._algHandle, ref this._keyHandle, pbKeyObject, keyObjectSize, pbKey, pbKey.Length, 0 );

		return status;
	}
}