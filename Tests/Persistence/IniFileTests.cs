// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
//
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/INIFile_Tests.cs" was last cleaned by Protiguous on 2016/06/18 at 10:56 PM

namespace LibrainianTests.Persistence {

    using System;
    using FluentAssertions;
    using Librainian.FileSystem;
    using Librainian.Persistence;
    using NUnit.Framework;

    [TestFixture]
    public static class IniFileTests {

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

        public static IniFile Ini1;

        public static IniFile Ini2;

        public static IniFile Ini3;

        //[OneTimeSetUp]
        public static void Setup() {
        }

        [Test]
        public static void test_load_from_file() {

            //prepare file
            var config = Document.GetTempDocument( "config" );
            config.AppendText( ini_test_data );

            Ini2 = new IniFile( config ) { ["Greetings", "Hello"] = "world1!", ["Greetings", "Hello"] = "world2!" };
            Ini2["Greetings", "Hello"].Should().Be( "world2!" );
        }

        [Test]
        public static void test_load_from_string() {
            Ini1 = new IniFile( ini_test_data );
            Ini1.Save( Document.GetTempDocument( "config" ) );
        }
    }
}