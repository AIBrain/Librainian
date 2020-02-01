// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "PronounceablePasswordCreator.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "PronounceablePasswordCreator.cs" was last formatted by Protiguous on 2020/01/31 at 12:31 AM.

namespace Librainian.Security {

    using System;
    using System.Collections.Generic;
    using System.Text;
    using JetBrains.Annotations;
    using Maths;

    //  This password generator gives you a list of "pronounceable" passwords. It is modeled after Morrie Gasser's original generator described in
    //           Gasser, M., A Random Word Generator for Pronouncable Passwords, MTR-3006,
    //            The MITRE Corporation, Bedford, MA 01730, ESD-TR-75-97, HQ Electronic Systems Division, Hanscom AFB, MA 01731. NTIS AD A 017676.
    //  except that Morrie's used a second-order approximation to English and this generator uses a third-order approximation.
    // A descendant of Gasser's generator was added to the Multics operating system by Project Guardian in the mid 70s, and I believe Digital's VMS
    // added a similar feature in the 80s.
    // FIPS Standard 181 describes a similar digraph-based generator, derived from Gasser's.
    // The first digraph-based password generator I know of was written by Daniel J. Edwards about 1965 for MIT's CTSS
    // timesharing system. Over the years I have implemented versions in Multics PL/I, Tandem TAL, C++, Java, and JavaScript.
    // C# version by Richard Hazrrison : http://chateau-logic.com http://zaretto.com
    /// <summary>Random Password Generator, see http://www.multicians.org/thvv/gpw.html</summary>
    public static class PronounceablePasswordCreator {

        [NotNull]
        public static String EnglishAlphabetLowercase { get; } = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>create a prounouncable word of the required length using third-order approximation.</summary>
        /// <param name="requiredLength"></param>
        /// <returns></returns>
        [NotNull]
        public static String Generate( Int32 requiredLength ) {
            Int32 c1;
            Int32 c2;
            Int32 c3;

            var password = new StringBuilder( capacity: requiredLength );
            var weightedRandom = ( Int64 ) ( Randem.NextDouble() * GpwData.Sigma );
            Int64 sum = 0;

            var finished = false;

            for ( c1 = 0; c1 < 26 && !finished; c1++ ) {
                for ( c2 = 0; c2 < 26 && !finished; c2++ ) {
                    for ( c3 = 0; c3 < 26 && !finished; c3++ ) {
                        sum += GpwData.Get( i1: c1, i2: c2, i3: c3 );

                        if ( sum <= weightedRandom ) {
                            continue;
                        }

                        password.Append( EnglishAlphabetLowercase[ index: c1 ] );
                        password.Append( EnglishAlphabetLowercase[ index: c2 ] );
                        password.Append( EnglishAlphabetLowercase[ index: c3 ] );
                        finished = true;
                    }
                }
            }

            // Now do a random walk - starting at the 4th position as just done 3 above.
            var nchar = 3;

            while ( nchar < requiredLength ) {
                c1 = EnglishAlphabetLowercase.IndexOf( password[ index: nchar - 2 ] );
                c2 = EnglishAlphabetLowercase.IndexOf( password[ index: nchar - 1 ] );

                sum = 0;

                for ( c3 = 0; c3 < 26; c3++ ) {
                    sum += GpwData.Get( i1: c1, i2: c2, i3: c3 );
                }

                if ( sum == 0 ) {
                    break;
                }

                weightedRandom = ( Int64 ) ( Randem.NextDouble() * sum );

                sum = 0;

                for ( c3 = 0; c3 < 26; c3++ ) {
                    sum += GpwData.Get( i1: c1, i2: c2, i3: c3 );

                    if ( sum <= weightedRandom ) {
                        continue;
                    }

                    password.Append( EnglishAlphabetLowercase[ index: c3 ] );

                    break;
                }

                nchar++;
            }

            return password.ToString();
        }

        /// <summary>generate a pass phrase built from pronouncable words</summary>
        /// <param name="minLength"></param>
        /// <param name="minWordLength"></param>
        /// <param name="maxWordLength"></param>
        /// <returns></returns>
        [NotNull]
        public static String GeneratePhrase( Int32 minLength, Int32 minWordLength = 3, Int32 maxWordLength = 6 ) {
            var words = new List<String>();
            var passwordLength = 0;

            while ( passwordLength < minLength ) {
                var length = ( maxWordLength - minWordLength + 1 ).Next() + minWordLength;
                var word = Generate( requiredLength: length );
                passwordLength += word.Length;
                words.Add( item: word );
            }

            return String.Join( separator: " ", words.ToArray() );
        }

    }

}