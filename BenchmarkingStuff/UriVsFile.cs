// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "UriVsFile.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
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
// Project: "BenchmarkingStuff", "UriVsFile.cs" was last formatted by Protiguous on 2019/02/09 at 9:35 AM.

namespace BenchmarkingStuff {

    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Mathematics;
    using JetBrains.Annotations;
    using Librainian.Logging;
    using Librainian.Maths;
    using NUnit.Framework;

    //[CoreJob]
    //[MonoJob]
    //[CoreRtJob]
    //[RPlotExporter]
    [RankColumn( NumeralSystem.Arabic )]
    [EvaluateOverhead]
    [ClrJob( baseline: true )]
    [TestFixture]
    public class UriVsFile {

        private const String Contents = "\r\nHello World\r\n";

        private const Int32 MaxLength = Int16.MaxValue;

        private const String pathFooter = @"\t.$";

        private const String pathHeader = @"\\?\T:";

        private const Int32 SegmentSize = 248 - 1;

        private static readonly String ContentsTrimmed = Contents.Trim();

        [Params( MaxLength / 4, MaxLength / 2, MaxLength )]
        public Int32 N;

        private ConcurrentStack<String> PathsToDelete { get; } = new ConcurrentStack<String>();

        public String Filename { get; set; }

        [NotNull]
        private static String CreateSegment( Int32 length ) {
            var segment = $@"\{Randem.RandomString( length - 1, lowerCase: true, upperCase: false, numbers: false, symbols: false )}";

            return segment;
        }

        [Test( ExpectedResult = true )]
        [Benchmark]
        public async Task<Boolean> AsFile() {
            using ( var reader = File.OpenText( this.Filename ) ) {

                var text = await reader.ReadToEndAsync().ConfigureAwait( false );
                text = text.Trim();

                return text.Equals( ContentsTrimmed, StringComparison.Ordinal );
            }
        }

        [OneTimeTearDown]
        public void Done() {
            File.Delete( this.Filename );

            while ( this.PathsToDelete.TryPop( out var path ) ) {
                if ( path != null ) {
                    try {
                        Directory.Delete( path );
                    }
                    catch ( Exception exception ) {
                        exception.Log( breakinto: true );
                    }
                }
            }
        }

        [OneTimeSetUp]
        [GlobalSetup]
        public void Setup() {
            var totalNeeded = MaxLength - ( pathHeader.Length + pathFooter.Length );

            var segments = totalNeeded / SegmentSize;
            var remainder = totalNeeded - segments * SegmentSize;

            var path = String.Empty;
            String dir;

            foreach ( var i in 1.To( segments ) ) {
                path += CreateSegment( SegmentSize );
                dir = $"{pathHeader}{path}";
                Directory.CreateDirectory( dir );
                this.PathsToDelete.Push( dir );
            }

            if ( remainder.Any() ) {
                path += CreateSegment( remainder );
                dir = $"{pathHeader}{path}";
                Directory.CreateDirectory( dir );
                this.PathsToDelete.Push( dir );
            }

            this.Filename = $@"{pathHeader}{path}{pathFooter}";

            //if ( File.Exists( this.filename ) ) { File.Delete( this.filename ); }	//the file gets overridden by the File.WriteAllText

            File.WriteAllText( this.Filename, Contents, Encoding.Unicode );
        }
    }
}