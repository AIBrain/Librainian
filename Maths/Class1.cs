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
// "Librainian/Class1.cs" was last cleaned by Rick on 2014/08/13 at 9:48 AM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public enum RomanNumber : short {
        I = 1,
        IV = 4,
        V = 5,
        IX = 9,
        X = 10,
        XL = 40,
        L = 50,
        XC = 90,
        C = 100,
        CD = 400,
        D = 500,
        CM = 900,
        M = 1000,
    }

    /// <summary>
    /// </summary>
    /// <seealso cref="http://github.com/lavz24/DecimalToRoman/blob/master/DecimalToRoman/Converter.cs" />
    public static class Converter {
        public static readonly RomanNumber[] RomanValue = {
                                                              RomanNumber.I, RomanNumber.IV, RomanNumber.V, RomanNumber.IX, RomanNumber.X, RomanNumber.XL, RomanNumber.L, RomanNumber.XC, RomanNumber.C, RomanNumber.CD, RomanNumber.D, RomanNumber.CM, RomanNumber.M };

        public static string ToRoman( this short number ) {
            if ( number <= 0 ) {
                return String.Empty;
            }
            if ( number >= 4000 ) {
                return String.Empty;
            }

            List< RomanNumber > listResult = new List<RomanNumber>();

            short currentRoman = ( short ) ( RomanValue.Length - 1 );

            for ( short i = number; i > (short)0; ) {
                if ( i < RomanValue[ currentRoman ] ) {
                    --currentRoman;
                } else {
                    listResult.Add( RomanValue[ currentRoman ] );
                    i -= RomanValue[ currentRoman ];
                }
            }

            return listResult.Aggregate( String.Empty, ( current, romanNumber ) => current + romanNumber.ToString() );
        }
    }
}
