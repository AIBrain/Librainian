// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "IntegerGenerator.cs" belongs to Rick@AIBrain.org and
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/IntegerGenerator.cs" was last formatted by Protiguous on 2018/05/24 at 7:34 PM.

namespace Librainian.Threading.RandomOrg {

    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Internet;
    using Parsing;

    /// <summary>
    /// </summary>
    /// <seealso cref="http://github.com/OrigamiTech/Random.org/blob/master/Random.org/IntegerGenerator.cs" />
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

        public IntegerGenerator() { this.Init( NumMax, Min, Max, ColDefault, BaseDefault ); }

        public IntegerGenerator( Int32 num ) { this.Init( num, Min, Max, ColDefault, BaseDefault ); }

        public IntegerGenerator( Int32 num, Int32 min ) { this.Init( num, min, Max, ColDefault, BaseDefault ); }

        public IntegerGenerator( Int32 num, Int32 min, Int32 max ) { this.Init( num, min, max, ColDefault, BaseDefault ); }

        public IntegerGenerator( Int32 num, Int32 min, Int32 max, Int32 col ) { this.Init( num, min, max, col, BaseDefault ); }

        public IntegerGenerator( Int32 num, Int32 min, Int32 max, Int32 col, Int32 inbase ) { this.Init( num, min, max, col, inbase ); }

        private void Init( Int32 num, Int32 min, Int32 max, Int32 col, Int32 inbase ) {
            if ( num >= NumMin && num <= NumMax ) {
                if ( min >= Min ) {
                    if ( max <= Max ) {
                        if ( max > min ) {
                            if ( col > 0 && col <= ColMax ) {
                                if ( inbase == 2 || inbase == 8 || inbase == 10 || inbase == 16 ) {
                                    var toParse = $"http://www.random.org/integers/?num={num}&min={min}&max={max}&col={col}&base={inbase}&format=plain&rnd=new".GetWebPage();

                                    if ( toParse is null ) { return; }

                                    foreach ( var s in Regex.Split( toParse, @"\D" ) ) {
                                        try {
                                            if ( !s.IsNullOrWhiteSpace() ) { this.Ints.Add( Convert.ToInt32( s, inbase ) ); }
                                        }
                                        catch { }
                                    }
                                }
                                else { throw new ArgumentOutOfRangeException( nameof( inbase ), "The base must be 2, 8, 10, or 16." ); }
                            }
                            else { throw new ArgumentOutOfRangeException( nameof( col ), "The column count must be between 1 and 1000000000." ); }
                        }
                        else { throw new ArgumentOutOfRangeException( nameof( min ), "The random number upper bound must be greater than the lower bound." ); }
                    }
                    else { throw new ArgumentOutOfRangeException( nameof( max ), "The random number upper bound must be between -1000000000 and 1000000000." ); }
                }
                else { throw new ArgumentOutOfRangeException( nameof( min ), "The random number lower bound must be between -1000000000 and 1000000000." ); }
            }
            else { throw new ArgumentOutOfRangeException( nameof( num ), "The number of random numbers to generate must be between 1 and 10000." ); }
        }

        public Int32 Get() {
            this._index++;
            this._index %= this.Ints.Count;

            return this.Ints[this._index];
        }
    }
}