// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "TheInternetTests.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", File: "TheInternetTests.cs" was last formatted by Protiguous on 2020/03/16 at 4:48 PM.

namespace Librainian.Internet {

    using System;
    using System.IO;
    using System.Media;
    using System.Net;
    using JetBrains.Annotations;
    using Logging;
    using Measurement.Time;
    using Threading;

    public static class TheInternetTests {

        public static SoundPlayer Player { get; } = new SoundPlayer();

        private static void OnWebException( [CanBeNull] Uri uri, WebExceptionStatus webExceptionStatus ) {
            Console.WriteLine( value: uri );
            Console.WriteLine( value: webExceptionStatus );
        }

        public static void Test1() {
            var inprogress = new VolatileBoolean();
            var creds = new NetworkCredential( userName: "AIBrain", password: @"hP&Y@bYsM5qT0tr" );

            var bob = TheInternet.DownloadAsync( address: new Uri( uriString: "https://www.freesound.org/people/BDWRekordings.com/sounds/98104/" ), timeOut: Seconds.Ten,
                reportProgress: null, inProgress: inprogress, credentials: creds, onWebException: OnWebException ).Result;

            if ( null != bob ) {
                Player.Stream = File.OpenRead( path: bob.FullPath );

                try {
                    Player.PlaySync();
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            }
        }

    }

}