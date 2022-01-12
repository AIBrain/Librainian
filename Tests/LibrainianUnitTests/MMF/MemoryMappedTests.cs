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
// File "MemoryMappedTests.cs" last touched on 2021-03-07 at 3:20 PM by Protiguous.

namespace LibrainianUnitTests.MMF;

using System;
using System.IO.MemoryMappedFiles;
using System.Text;
using NUnit.Framework;

//[TestFixture]
public class MemoryMappedTests {

	//TODO any tests?

	//private var localFilePath = "complete_path_to_large_file";

	public String GetContent( MemoryMappedFile memoryMappedFile, Int64 beginningByteLocation, Int64 bytesToReadIn ) {
		using var memoryMappedViewStream = memoryMappedFile.CreateViewStream( beginningByteLocation, bytesToReadIn, MemoryMappedFileAccess.Read );
		var contentArray = new Byte[ bytesToReadIn ];
		memoryMappedViewStream.Read( contentArray, 0, contentArray.Length );
		var content = Encoding.UTF8.GetString( contentArray );

		return content;
	}

	public void Test() {
		const Int64 size32 = sizeof( UInt32 );
		const Int64 multiplier = UInt32.MaxValue;
		const Int64 biteSize = size32 * multiplier; //that's a 17.18 GB !!

		using var bob = MemoryMappedFile.CreateOrOpen( "test.$$$", biteSize, MemoryMappedFileAccess.ReadWrite );
		//bob.CreateViewAccessor
	}

	//using (var memoryMappedFile = MemoryMappedFile.CreateFromFile(localFilePath, FileMode.Open)){}

}