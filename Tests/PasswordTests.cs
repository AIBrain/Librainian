// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "PasswordTests.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/LibrainianTests/PasswordTests.cs" was last cleaned by Protiguous on 2018/05/15 at 10:51 PM.

namespace LibrainianTests {

    using System;
    using System.Globalization;
    using Librainian.Maths;
    using Librainian.Parsing;
    using Librainian.Security;
    using NUnit.Framework;

    [TestFixture]
    public static class PasswordTests {

        [Test]
        public static void TestAFew() {
            foreach ( var variable in 1.To( end: 25 ) ) {
                Console.WriteLine( value: PronounceablePasswordCreator.Generate( requiredLength: 3.Next( maxValue: 15 ) ).ToPascalCase( culture: CultureInfo.CurrentUICulture ) );
            }
        }
    }
}