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
// "Librainian/IntegerGenerator.cs" was last cleaned by Protiguous on 2016/06/18 at 10:57 PM

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Librainian.Internet;
using Librainian.Parsing;

namespace Librainian.Threading.RandomOrg {

    /// <summary>
    /// </summary>
    /// <seealso cref="http://github.com/OrigamiTech/Random.org/blob/master/Random.org/IntegerGenerator.cs"/>
    public class IntegerGenerator {
        private const Int32 BaseDefault = 10;
        private const Int32 ColDefault = 1;
        private const Int32 ColMax = 1000000000;
        private const Int32 Max = 1000000000;
        private const Int32 Min = -1000000000;
        private const Int32 NumMax = 10000;
        private const Int32 NumMin = 1;
        private readonly List<Int32> _ints = new List<Int32>();
        private Int32 _index;

        public IntegerGenerator() {
            this.Init( NumMax, Min, Max, ColDefault, BaseDefault );
        }

        public IntegerGenerator( Int32 num ) {
            this.Init( num, Min, Max, ColDefault, BaseDefault );
        }

        public IntegerGenerator( Int32 num, Int32 min ) {
            this.Init( num, min, Max, ColDefault, BaseDefault );
        }

        public IntegerGenerator( Int32 num, Int32 min, Int32 max ) {
            this.Init( num, min, max, ColDefault, BaseDefault );
        }

        public IntegerGenerator( Int32 num, Int32 min, Int32 max, Int32 col ) {
            this.Init( num, min, max, col, BaseDefault );
        }

        public IntegerGenerator( Int32 num, Int32 min, Int32 max, Int32 col, Int32 inbase ) {
            this.Init( num, min, max, col, inbase );
        }

        private void Init( Int32 num, Int32 min, Int32 max, Int32 col, Int32 inbase ) {
            if ( num >= NumMin && num <= NumMax ) {
                if ( min >= Min ) {
                    if ( max <= Max ) {
                        if ( max > min ) {
                            if ( col > 0 && col <= ColMax ) {
                                if ( inbase == 2 || inbase == 8 || inbase == 10 || inbase == 16 ) {
                                    var toParse =
                                        $"http://www.random.org/integers/?num={num}&min={min}&max={max}&col={col}&base={inbase}&format=plain&rnd=new"
                                            .GetWebPage();
                                    if ( toParse is null ) {
                                        return;
                                    }

                                    foreach ( var s in Regex.Split( toParse, @"\D" ) ) {
                                        try {
                                            if ( !s.IsNullOrWhiteSpace() ) {
                                                this._ints.Add( Convert.ToInt32( s, inbase ) );
                                            }
                                        }
                                        catch { }
                                    }
                                }
                                else {
                                    throw new ArgumentOutOfRangeException( nameof( inbase ),
                                        "The base must be 2, 8, 10, or 16." );
                                }
                            }
                            else {
                                throw new ArgumentOutOfRangeException( nameof( col ),
                                    "The column count must be between 1 and 1000000000." );
                            }
                        }
                        else {
                            throw new ArgumentOutOfRangeException( nameof( min ),
                                "The random number upper bound must be greater than the lower bound." );
                        }
                    }
                    else {
                        throw new ArgumentOutOfRangeException( nameof( max ),
                            "The random number upper bound must be between -1000000000 and 1000000000." );
                    }
                }
                else {
                    throw new ArgumentOutOfRangeException( nameof( min ),
                        "The random number lower bound must be between -1000000000 and 1000000000." );
                }
            }
            else {
                throw new ArgumentOutOfRangeException( nameof( num ),
                    "The number of random numbers to generate must be between 1 and 10000." );
            }
        }

        public Int32 Get() {
            this._index++;
            this._index %= this._ints.Count;
            return this._ints[this._index];
        }
    }
}