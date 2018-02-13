// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/HumanCalculator.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

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

                if ( s.Length < term.Length ) {
                    s = s.PadLeft( term.Length, '0' );
                }
                else if ( term.Length < s.Length ) {
                    term = term.PadLeft( s.Length, '0' );
                }

                while ( term.Any() ) {
                    var l = Byte.Parse( s.Last().ToString() );
                    s = s.Substring( 0, s.Length - 1 );

                    var m = Byte.Parse( term.Last().ToString() );
                    term = term.Substring( 0, term.Length - 1 );

                    var t = ( l + m ).ToString();
                    var c = Byte.Parse( t.Last().ToString() );
                    if ( 2 == t.Length ) {
                        result = "1" + c;
                    }
                    else {
                        result += c;
                    }
                }

                total += BigInteger.Parse( result );
            }

            return total;
        }

        public static BigInteger Divide( BigInteger[] terms ) => throw new NotImplementedException();

	    public static BigInteger Multiply( BigInteger[] terms ) => throw new NotImplementedException();

	    public static BigInteger Operate( Operation operation, params BigInteger[] terms ) {
            switch ( operation ) {
                case Operation.Addition:
                    return Add( terms );

                case Operation.Subtraction:
                    return Subtract( terms );

                case Operation.Multiplication:
                    return Multiply( terms );

                case Operation.Division:
                    return Divide( terms );

                default:
                    throw new ArgumentOutOfRangeException( nameof( operation ), operation, $"Unknown operation {operation}" );
            }
        }

        public static BigInteger Subtract( BigInteger[] terms ) => throw new NotImplementedException();

    }
}