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
// File "$FILENAME$" last touched on $CURRENT_YEAR$-$CURRENT_MONTH$-$CURRENT_DAY$ at $CURRENT_TIME$ by Protiguous.

namespace LibrainianUnitTests.Persistence;

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Librainian.FileSystem;
using Librainian.Measurement.Time;
using Librainian.Persistence.InIFiles;
using NUnit.Framework;

[TestFixture]
public class IniFileTests {

	public const String IniTestData = @"
[ Section 1  ]
;This is a comment
data1=value1
data2 =value2
data3= value3
data4 = value4
data5   =   value5

[ Section 2  ]

//This is also a comment
data11=value11
data22 = value22
data33   =   value33
data44 =value44
data55= value55

[ Section 2  ]

//This is also a comment
data11=value11b
data22 = value22b
data33   =   value33b

[ Section 3  ]

//This is also a comment
data11=1
data22 = 2
data33   =   3

";

	private static IniFile? Ini1;

	private static IniFile? Ini2;

	private static IniFile? Ini3;

	[Test]
	public async Task test_load_from_file() {
		var cancellationTokenSource = new CancellationTokenSource( Minutes.One );

		//prepare file
		var config = Document.GetTempDocument( "config" );
		var doc = await config.AppendText( IniTestData, cancellationTokenSource.Token ).ConfigureAwait( false );

		doc.Should()?.Be( config );

		Ini2 = new IniFile {
			["Greetings", "Hello"] = "world",
			["Greeting", "Hello"] = "world",
			["Greetings", "Hello"] = "World!"
		};

		var test = Ini2["Greetings", "Hello"];

		test.Should()?.Be( "World!" );

		Ini3 = new IniFile( doc );

		Console.WriteLine( Ini3 );
	}

	[Test]
	public static async Task test_save_from_string() {
		var cancellationTokenSource = new CancellationTokenSource( Minutes.One );

		Ini1 = new IniFile( IniTestData, cancellationTokenSource.Token );
		var temp = Document.GetTempDocument( "config" );
		var saved = await Ini1.Save( temp, cancellationTokenSource.Token ).ConfigureAwait( false );
		if ( saved ) {
			Console.Write( "File saved to: " );
			Console.WriteLine( temp.FullPath );
		}
		else {
			Console.WriteLine( "File not saved." );
		}

	}

}