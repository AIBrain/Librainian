// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "IniFileTests.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're still looking into other solutions! Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "LibrainianTests", "IniFileTests.cs" was last formatted by Protiguous on 2019/03/17 at 8:36 PM.

namespace LibrainianTests.Persistence {

    using System;
    using FluentAssertions;
    using Librainian.OperatingSystem.FileSystem;
    using Librainian.Persistence.InIFiles;
    using Xunit;

    public static class IniFileTests {

        public const String ini_test_data = @"
[ Section 1  ]
;This is a comment for section 1
data.1=value1
data.2 =value2
data.3= value3
data.4 = value4
data5   =   !value5

[ Section 2  ]
;This is also a comment for section 2
data11=value11
data22 = value22
data33   =   value33
data44 =value44
data55= $55.00

[ Section 2  ]
;This is also another comment for section 2
data11=value11b
data22 = value22b
data33   =   value33b

[ Section 3  ]
;This is also a comment for section 3
data3.1 =  1
data3.2 =    2
data3.3   =   3

";

        public static IniFile Ini1;

        public static IniFile Ini2;

        public static IniFile Ini3;

        [Fact]
        public static void test1() {

            //prepare file
            var config = Document.GetTempDocument( "config" ).AppendText( ini_test_data );
            config.DeleteAfterClose = false;

            Console.WriteLine( config.CalculateHarkerHashInt32() );

            Ini2 = new IniFile( config );
            Ini2.Add( "Greetings", "Hello", "world1!" );
            Ini2.Add( "Greetings", "Hello", "world2!" );
            Ini2[ "Greetings", "Hello" ].Should().Be( "world2!" );

            if ( Ini2.Save( config ) ) {
                Console.WriteLine( $"Saved to {config.FullPath}" );
            }
        }
    }
}