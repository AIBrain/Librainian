// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: Protiguous@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/JSONFile_Tests.cs" was last cleaned by Rick on 2016/06/18 at 10:56 PM

namespace LibrainianCore.Persistence {

    using System;

    [TestFixture]
    public static class JSONFile_Tests {

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

        public static INIFile Ini;
        public static JSONFile Json;

        [OneTimeSetUp]
        public static void Setup() {
            Json = IoC.Container.Get<JSONFile>();
            Ini = IoC.Container.Get<INIFile>();
        }

        [Test]
        public static async void test_load_from_string() {
            Ini.Add( ini_test_data );

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