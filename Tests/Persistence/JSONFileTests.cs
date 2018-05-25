// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "JSONFileTests.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/LibrainianTests/JSONFileTests.cs" was last formatted by Protiguous on 2018/05/22 at 4:40 PM.

namespace LibrainianTests.Persistence {

    using System;
    using System.Threading.Tasks;
    using Librainian.ComputerSystems.FileSystem;
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

            if ( await Json.Write() ) { Json.Document?.Delete(); }
        }
    }
}