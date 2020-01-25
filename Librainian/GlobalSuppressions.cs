// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "GlobalSuppressions.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
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
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "GlobalSuppressions.cs" was last formatted by Protiguous on 2020/01/21 at 10:34 AM.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage( "Globalization", "CA1303:Do not pass literals as localized parameters" )]
[assembly: SuppressMessage( "Globalization", "CA1305:Specify IFormatProvider" )]
[assembly: SuppressMessage( "Design", "CA1031:Do not catch general exception types" )]
[assembly: SuppressMessage( "Design", "CA1065:Do not raise exceptions in unexpected locations" )]
[assembly: SuppressMessage( "Design", "CA1036:Override methods on comparable types" )]
[assembly: SuppressMessage( "Design", "CA1040:Avoid empty interfaces" )]
[assembly: SuppressMessage( "Design", "CA1032:Implement standard exception constructors" )]
[assembly: SuppressMessage( "Design", "CA1062:Validate arguments of public methods" )]
[assembly: SuppressMessage( "Naming", "CA1710:Identifiers should have correct suffix" )]
[assembly: SuppressMessage( "Performance", "CA1819:Properties should not return arrays", Justification = "What a dumb rule for dumb programmers." )]
[assembly: SuppressMessage( "Design", "CA1000:Do not declare static members on generic types", Justification = "Stupid rule." )]
[assembly: SuppressMessage( "Design", "CA1008:Enums should have zero value" )]
[assembly: SuppressMessage( "Design", "CA1034:Nested types should not be visible", Justification = "Who comes up with some of these stupid 'rules'?" )]
[assembly: SuppressMessage( "Performance", "CA1814:Prefer jagged arrays over multidimensional" )]
[assembly: SuppressMessage( "Design", "CA1028:Enum Storage should be Int32", Justification = "Stupid rule. Not everything needs to be an Int32!" )]
[assembly: SuppressMessage( "Design", "CA1051:Do not declare visible instance fields", Justification = "Stupid rule for structs." )]
[assembly: SuppressMessage( "Design", "CA1060:Move pinvokes to native methods class", Justification = "Another stupid rule." )]
[assembly: SuppressMessage( "Design", "CA1063:Implement IDisposable Correctly", Justification = "I am so fucking tired of stupid design rules. Analyze better!" )]
[assembly: SuppressMessage( "Interoperability", "CA1401:P/Invokes should not be visible", Justification = "Why?" )]