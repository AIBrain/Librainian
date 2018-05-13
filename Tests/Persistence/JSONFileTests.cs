// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "LibrainianTests/JSONFile_Tests.cs" was last cleaned by Protiguous on 2018/05/11 at 8:52 PM

namespace LibrainianTests.Persistence {

    using System;
    using System.Threading.Tasks;
    using Librainian.FileSystem;
    using Librainian.Magic;
    using Librainian.Persistence;
    using NUnit.Framework;

    [TestFixture]
    public static class JSONFileTests {

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

        public static IniFile Ini;
        public static Ini Json;

        [OneTimeSetUp]
        public static void Setup() {
            Json = IoC.Container.Get<Ini>();
            Ini = IoC.Container.Get<IniFile>();
        }

        [Test]
        public static async Task test_load_from_string() {
            Ini.Add( IniTestData );

            Json.Add( Ini );
            Json.Add( Ini );
            Json.Add( Ini );

            Json.Document = Document.GetTempDocument( "config" );
            if ( await Json.Write() ) {
                Json.Document?.Delete();
            }
        }
    }
}