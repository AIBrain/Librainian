// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "IntegerGenerator.cs" belongs to Protiguous@Protiguous.com
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
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "IntegerGenerator.cs" was last formatted by Protiguous on 2020/01/31 at 12:25 AM.

namespace Librainian.Internet.RandomOrg {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;
    using Measurement.Time;
    using Parsing;

    public static class RandomDotOrg {

        internal static Lazy<IntegerGenerator> Generator { get; } = new Lazy<IntegerGenerator>( valueFactory: () => new IntegerGenerator( 1, CancellationTokenSource.Token ) );

        public static CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

        [ItemNotNull]
        public static async Task<IEnumerable<Int32>> SequenceGenerator( this Int32 minValue, Int32 maxValue ) {

            if ( maxValue < minValue ) {
                CommonExtensions.Swap( ref minValue, ref maxValue );
            }

            if ( maxValue - minValue + 1 > Math.Pow( x: 10, y: 3 ) ) {
                throw new ArgumentException( "Range requested cannot be larger than 10,000" );
            }

            if ( minValue < -Math.Pow( x: 10, y: 8 ) || minValue > Math.Pow( x: 10, y: 8 ) ) {
                throw new ArgumentException( "Value of min must be between -1e9 and 1e9", nameof( minValue ) );
            }

            if ( maxValue < -Math.Pow( x: 10, y: 8 ) || maxValue > Math.Pow( x: 10, y: 8 ) ) {
                throw new ArgumentException( "Value of max must be between -1e9 and 1e9", nameof( maxValue ) );
            }

            var url = new Uri( "https" + "://random.org/sequences/?min=" + minValue + "&max=" + maxValue + "&col=1&base=10&format=plain&rnd=new", UriKind.Absolute );

            var task = url.GetWebPageAsync( Seconds.Seven );

            if ( task is null ) {
                throw new InvalidOperationException( "Unable to pull any data from random.org." );
            }

            var responseFromServer = await task.ConfigureAwait( false );

            return responseFromServer.Split( '\n' ).Where( s => s.Any() ).Select( Int32.Parse );
        }

        /// <summary></summary>
        /// <see cref="http://github.com/OrigamiTech/Random.org/blob/master/Random.org/IntegerGenerator.cs" />
        public class IntegerGenerator {

            private const Int32 BaseDefault = 10;
            private const Int32 ColDefault = 1;
            private const Int32 ColMax = 1000000000;
            private const Int32 Max = 1000000000;
            private const Int32 Min = -1000000000;
            private const Int32 NumMax = 10000;
            private const Int32 NumMin = 1;
            private Int32 _index;

            private List<Int32> Ints { get; } = new List<Int32>();

            public IntegerGenerator( CancellationToken token ) {
                this.Init( NumMax, Min, Max, ColDefault, BaseDefault, token ).Wait( token );
            }

            public IntegerGenerator( Int32 num, CancellationToken token ) => this.Init( num, Min, Max, ColDefault, BaseDefault, token ).Wait( token );

            public IntegerGenerator( Int32 num, Int32 min, CancellationToken token ) => this.Init( num, min, Max, ColDefault, BaseDefault, token ).Wait( token );

            public IntegerGenerator( Int32 num, Int32 min, Int32 max, CancellationToken token ) => this.Init( num, min, max, ColDefault, BaseDefault, token ).Wait( token );

            public IntegerGenerator( Int32 num, Int32 min, Int32 max, Int32 col, CancellationToken token ) =>
                this.Init( num, min, max, col, BaseDefault, token ).Wait( token );

            public IntegerGenerator( Int32 num, Int32 min, Int32 max, Int32 col, Int32 inbase, CancellationToken token ) =>
                this.Init( num, min, max, col, inbase, token ).Wait( token );

            private async Task Init( Int32 num, Int32 min, Int32 max, Int32 col, Int32 inbase, CancellationToken token ) {
                if ( num < NumMin || num > NumMax ) {
                    throw new ArgumentOutOfRangeException( nameof( num ), "The number of random numbers to generate must be between 1 and 10000." );
                }

                if ( min < Min ) {
                    throw new ArgumentOutOfRangeException( nameof( min ), "The random number lower bound must be between -1000000000 and 1000000000." );
                }

                if ( max > Max ) {
                    throw new ArgumentOutOfRangeException( nameof( max ), "The random number upper bound must be between -1000000000 and 1000000000." );
                }

                if ( max <= min ) {
                    throw new ArgumentOutOfRangeException( nameof( min ), "The random number upper bound must be greater than the lower bound." );
                }

                if ( col <= 0 || col > ColMax ) {
                    throw new ArgumentOutOfRangeException( nameof( col ), "The column count must be between 1 and 1000000000." );
                }

                if ( inbase != 2 && inbase != 8 && inbase != 10 && inbase != 16 ) {
                    throw new ArgumentOutOfRangeException( nameof( inbase ), "The base must be 2, 8, 10, or 16." );
                }

                var job = $"http://www.random.org/integers/?num={num}&min={min}&max={max}&col={col}&base={inbase}&format=plain&rnd=new".GetWebPageAsync( Minutes.One );

                if ( job == null ) {
                    throw new InvalidOperationException( "Unable to pull random numbers from Random.Org." );
                }

                var toParse = await job.ConfigureAwait( false );

                if ( toParse is null ) {
                    return;
                }

                foreach ( var s in Regex.Split( toParse, @"\D" ) ) {
                    try {
                        if ( !s.IsNullOrWhiteSpace() ) {
                            this.Ints.Add( Convert.ToInt32( s, inbase ) );
                        }
                    }
                    catch { }
                }
            }

            public Int32 Get() {
                this._index++;
                this._index %= this.Ints.Count;

                return this.Ints[ this._index ];
            }
        }
    }
}