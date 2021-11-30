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
// File "JSONFileTests.cs" last touched on 2021-03-07 at 3:20 PM by Protiguous.

#nullable enable

namespace LibrainianUnitTests.Persistence;

using System;
using System.Threading;
using System.Threading.Tasks;
using Librainian.FileSystem;
using Librainian.Measurement.Time;
using Librainian.Persistence;
using Librainian.Persistence.InIFiles;
using NUnit.Framework;


[TestFixture]
public class JSONFileTests {

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

	public IniFile Ini { get; } = new();

	public JSONFile Json { get; } = new();

	private CancellationTokenSource? CancellationTokenSource { get; set; }

	public CancellationTokenSource CreateCancelToken() {
		this.CancellationTokenSource ??= new CancellationTokenSource( Minutes.Ten );

		Assert.NotNull( this.CancellationTokenSource );
		return this.CancellationTokenSource;
	}

	private CancellationToken GetCancellationToken() => this.CreateCancelToken().Token;

	[Test]
	public void Test_load_from_string() {
		this.Ini.Add( IniTestData );
		this.Ini.Add( IniTestData );
		this.Ini.Add( IniTestData );
		this.Ini.Add( IniTestData );
		this.Ini.Add( IniTestData );
	}

	[Test]
	public async Task Test_load_from_string_and_save_temp() {
		this.Test_load_from_string();

		this.Json.Document = await this.CreateTempDocument().ConfigureAwait( false );
		Assert.True( !await this.Json.Document.Exists( this.GetCancellationToken() ).ConfigureAwait( false ) );

		if ( await this.Json.Write( this.GetCancellationToken() ).ConfigureAwait( false ) ) {
			Assert.True( await this.Json.Document.Exists( this.GetCancellationToken() ).ConfigureAwait( false ) );
		}

		await this.Json.Document.Delete( this.GetCancellationToken() ).ConfigureAwait( false );
		Assert.True( !await this.Json.Document.Exists( this.GetCancellationToken() ).ConfigureAwait( false ) );
	}

	public async Task<Document> CreateTempDocument() {
		var temp = Document.GetTempDocument( "config" );
		if ( await temp.Exists( this.GetCancellationToken() ).ConfigureAwait( false ) ) {
			throw new InvalidOperationException();
		}

		return ( Document )temp;
	}

}