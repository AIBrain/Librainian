#nullable enable
namespace Librainian.Tests {

	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using FileSystem;
	using JetBrains.Annotations;
	using Persistence;
	using Persistence.InIFiles;
	using Xunit;

	public class JSONFile_Tests {

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
		public IniFile Ini { get; } = new IniFile();

		[NotNull]
		public JSONFile Json { get; } = new JSONFile();

		[Fact]
		public async Task test_load_from_string() {
			this.Ini.Add( ini_test_data );

			//TODO needs testing
			//this.Json.Add( this.Ini );
			//this.Json.Add( this.Ini );
			//this.Json.Add( this.Ini );

			this.Json.Document = ( Document )Document.GetTempDocument( "config" );

			if ( await this.Json.Write(CancellationToken.None).ConfigureAwait(false) ) {
				this.Json.Document?.Delete();
			}
		}

	}

}