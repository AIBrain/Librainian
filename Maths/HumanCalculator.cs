// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "HumanCalculator.cs",
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
// "Librainian/Librainian/HumanCalculator.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Maths {

    using System;
    using System.Linq;
    using System.Numerics;

    /// <summary>
    ///     Challenge: Do math the way we were taught in school.
    /// </summary>
    public static class HumanCalculator {

        public enum Operation {

            /// <summary>
            ///     https://en.wikipedia.org/wiki/Addition
            /// </summary>
            /// <see cref="http://wikipedia.org/wiki/Addition" />
            Addition,

            /// <summary>
            /// </summary>
            Subtraction,

            /// <summary>
            /// </summary>
            Multiplication,

            /// <summary>
            /// </summary>
            Division
        }

        /// <summary>
        ///     Add classroom-style (the challenge: avoid using BigInteger+BigInteger operation or reversing the strings).
        /// </summary>
        /// <param name="whom"></param>
        /// <param name="nombre"></param>
        /// <returns></returns>
        /// <see cref="http://wikipedia.org/wiki/Addition#Notation_and_terminology" />
        public static BigInteger Add( this BigInteger whom, BigInteger nombre ) {
            var resultant = BigInteger.Zero;

            //TODO
            return resultant;
        }

        /// <summary>
        ///     Add classroom-style (the challenge: avoid using BigInteger+BigInteger operation or reversing the strings).
        /// </summary>
        /// <param name="terms"></param>
        /// <returns></returns>
        public static BigInteger Add( params BigInteger[] terms ) {
            var total = BigInteger.Zero;

            foreach ( var local in terms.Select( term => term.ToString() ) ) {
                var term = local;

                // total
                //+ term
                //______
                //result

                var s = total.ToString();
                var result = String.Empty;

                if ( s.Length < term.Length ) { s = s.PadLeft( term.Length, '0' ); }
                else if ( term.Length < s.Length ) { term = term.PadLeft( s.Length, '0' ); }

                while ( term.Any() ) {
                    var l = Byte.Parse( s.Last().ToString() );
                    s = s.Substring( 0, s.Length - 1 );

                    var m = Byte.Parse( term.Last().ToString() );
                    term = term.Substring( 0, term.Length - 1 );

                    var t = ( l + m ).ToString();
                    var c = Byte.Parse( t.Last().ToString() );

                    if ( 2 == t.Length ) { result = "1" + c; }
                    else { result += c; }
                }

                total += BigInteger.Parse( result );
            }

            return total;
        }

        public static BigInteger Divide( BigInteger[] terms ) => throw new NotImplementedException();

        public static BigInteger Multiply( BigInteger[] terms ) => throw new NotImplementedException();

        public static BigInteger Operate( Operation operation, params BigInteger[] terms ) {
            switch ( operation ) {
                case Operation.Addition: return Add( terms );

                case Operation.Subtraction: return Subtract( terms );

                case Operation.Multiplication: return Multiply( terms );

                case Operation.Division: return Divide( terms );

                default: throw new ArgumentOutOfRangeException( nameof( operation ), operation, $"Unknown operation {operation}" );
            }
        }

        public static BigInteger Subtract( BigInteger[] terms ) => throw new NotImplementedException();
    }
}