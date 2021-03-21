// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting.
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
// 
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "JSONFile_Tests.cs" last formatted on 2021-02-08 at 2:16 AM.

#nullable enable

namespace LibrainianUnitTests.Persistence {
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Librainian.FileSystem;
	using Librainian.Measurement.Time;
	using Librainian.Persistence;
	using Librainian.Persistence.InIFiles;
	using Xunit;

	public class JSONFileTests {

		public const String ini_test_data = @"
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

		[NotNull]
		public IniFile Ini { get; } = new();

		[NotNull]
		public JSONFile Json { get; } = new();

		[Fact]
		public async Task test_load_from_string() {

			var cancellationTokenSource = new CancellationTokenSource( Seconds.Thirty );

			this.Ini.Add( ini_test_data );

			//TODO needs testing
			//this.Json.Add( this.Ini );
			//this.Json.Add( this.Ini );
			//this.Json.Add( this.Ini );

			this.Json.Document = ( Document )Document.GetTempDocument( "config" );

			if ( await this.Json.Write( cancellationTokenSource.Token ).ConfigureAwait( false ) ) {
				await this.Json.Document.Delete(cancellationTokenSource.Token ).ConfigureAwait( false );
			}
		}

	}
}