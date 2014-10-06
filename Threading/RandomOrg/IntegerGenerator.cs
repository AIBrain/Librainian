#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/IntegerGenerator.cs" was last cleaned by Rick on 2014/10/06 at 2:20 PM
#endregion

namespace Librainian.Threading.RandomOrg {
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="http://github.com/OrigamiTech/Random.org/blob/master/Random.org/IntegerGenerator.cs"/>
    public class IntegerGenerator {
        private const int BASE_DEFAULT = 10;

        private const int COL_DEFAULT = 1;

        private const int COL_MAX = 1000000000;

        private const int MAX = 1000000000;

        private const int MIN = -1000000000;

        private const int NUM_MAX = 10000;

        private const int NUM_MIN = 1;

        private readonly List< int > _ints = new List< int >();
        private int _index;

        public IntegerGenerator() {
            this.Init( NUM_MAX, MIN, MAX, COL_DEFAULT, BASE_DEFAULT );
        }

        public IntegerGenerator( int num ) {
            this.Init( num, MIN, MAX, COL_DEFAULT, BASE_DEFAULT );
        }

        public IntegerGenerator( int num, int min ) {
            this.Init( num, min, MAX, COL_DEFAULT, BASE_DEFAULT );
        }

        public IntegerGenerator( int num, int min, int max ) {
            this.Init( num, min, max, COL_DEFAULT, BASE_DEFAULT );
        }

        public IntegerGenerator( int num, int min, int max, int col ) {
            this.Init( num, min, max, col, BASE_DEFAULT );
        }

        public IntegerGenerator( int num, int min, int max, int col, int inbase ) {
            this.Init( num, min, max, col, inbase );
        }

        public int Get() {
            this._index++;
            this._index %= this._ints.Count;
            return this._ints[ this._index ];
        }

        private void Init( int num, int min, int max, int col, int inbase ) {
            if ( num >= NUM_MIN && num <= NUM_MAX ) {
                if ( min >= MIN ) {
                    if ( max <= MAX ) {
                        if ( max > min ) {
                            if ( col > 0 && col <= COL_MAX ) {
                                if ( inbase == 2 || inbase == 8 || inbase == 10 || inbase == 16 ) {
                                    var toParse = WebInterface.GetWebPage( string.Format( "http://www.random.org/integers/?num={0}&min={1}&max={2}&col={3}&base={4}&format=plain&rnd=new", num, min, max, col, inbase ) );
                                    foreach ( var s in Regex.Split( toParse, @"\D" ) ) {
                                        try {
                                            this._ints.Add( Convert.ToInt32( s, inbase ) );
                                        }
                                        catch { }
                                    }
                                }
                                else {
                                    throw new Exception( "The base must be 2, 8, 10, or 16." );
                                }
                            }
                            else {
                                throw new Exception( "The column count must be between 1 and 1000000000." );
                            }
                        }
                        else {
                            throw new Exception( "The random number upper bound must be greater than the lower bound." );
                        }
                    }
                    else {
                        throw new Exception( "The random number upper bound must be between -1000000000 and 1000000000." );
                    }
                }
                else {
                    throw new Exception( "The random number lower bound must be between -1000000000 and 1000000000." );
                }
            }
            else {
                throw new Exception( "The number of random numbers to generate must be between 1 and 10000." );
            }
        }
    }
}
