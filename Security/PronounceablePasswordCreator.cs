// Copyright 2018 Protiguous
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/PronounceablePasswordCreator.cs" was last cleaned by Protiguous on 2018/05/06 at 2:22 PM

namespace Librainian.Security {

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Maths;
    using Parsing;

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
    /// <summary>
    ///     Random Password Generator, see http://www.multicians.org/thvv/gpw.html
    /// </summary>
    public class PronounceablePasswordCreator {

        /// <summary>
        ///     create a prounouncable word of the required length using third-order approximation.
        /// </summary>
        /// <param name="requiredLength"></param>
        /// <returns></returns>
        public static String Generate( Int32 requiredLength ) {
            Int32 c1;
            Int32 c2;
            Int32 c3;

            var password = new StringBuilder( capacity: requiredLength );
            var weightedRandom = ( Int64 )( Randem.NextDouble() * GpwData.Sigma );
            Int64 sum = 0;

            var finished = false;

            for ( c1 = 0; c1 < 26 && !finished; c1++ ) {
                for ( c2 = 0; c2 < 26 && !finished; c2++ ) {
                    for ( c3 = 0; c3 < 26 && !finished; c3++ ) {
                        sum += GpwData.Get( i1: c1, i2: c2, i3: c3 );
                        if ( sum <= weightedRandom ) {
                            continue;
                        }

                        password.Append( value: ParsingExtensions.EnglishAlphabetLowercase[index: c1] );
                        password.Append( value: ParsingExtensions.EnglishAlphabetLowercase[index: c2] );
                        password.Append( value: ParsingExtensions.EnglishAlphabetLowercase[index: c3] );
                        finished = true;
                    }
                }
            }

            // Now do a random walk - starting at the 4th position as just done 3 above.
            var nchar = 3;
            while ( nchar < requiredLength ) {
                c1 = ParsingExtensions.EnglishAlphabetLowercase.IndexOf( value: password[index: nchar - 2] );
                c2 = ParsingExtensions.EnglishAlphabetLowercase.IndexOf( value: password[index: nchar - 1] );

                sum = 0;

                for ( c3 = 0; c3 < 26; c3++ ) {
                    sum += GpwData.Get( i1: c1, i2: c2, i3: c3 );
                }

                if ( sum == 0 ) {
                    break;
                }

                weightedRandom = ( Int64 )( Randem.NextDouble() * sum );

                sum = 0;

                for ( c3 = 0; c3 < 26; c3++ ) {
                    sum += GpwData.Get( i1: c1, i2: c2, i3: c3 );
                    if ( sum <= weightedRandom ) {
                        continue;
                    }

                    password.Append( value: ParsingExtensions.EnglishAlphabetLowercase[index: c3] );
                    break;
                }

                nchar++;
            }

            return password.ToString();
        }

        /// <summary>
        ///     generate a pass phrase built from pronouncable words
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="minWordLength"></param>
        /// <param name="maxWordLength"></param>
        /// <returns></returns>
        public static String GeneratePhrase( Int32 minLength, Int32 minWordLength = 3, Int32 maxWordLength = 6 ) {
            var words = new List<String>();
            var passwordLength = 0;

            while ( passwordLength < minLength ) {
                var length = ( maxWordLength - minWordLength + 1 ).Next() + minWordLength;
                var word = Generate( requiredLength: length );
                passwordLength += word.Length;
                words.Add( item: word );
            }

            return String.Join( separator: " ", value: words.ToArray() );
        }
    }
}