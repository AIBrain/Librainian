// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "CommonExtensions.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "CommonExtensions.cs" was last formatted by Protiguous on 2020/01/31 at 12:25 AM.

namespace LibrainianCore {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public static class Common {

        /// <summary>Return true if an <see cref="IComparable" /> value is <see cref="Between{T}" /> two inclusive values.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">        </param>
        /// <param name="startInclusive"></param>
        /// <param name="endInclusive">  </param>
        /// <returns></returns>
        /// <example>5. Between(1, 10)</example>
        /// <example>5. Between(10, 1)</example>
        /// <example>5. Between(10, 6) == false</example>
        /// <example>5. Between(5, 5))</example>
        public static Boolean Between<T>( [NotNull] this T target, [NotNull] T startInclusive, [NotNull] T endInclusive ) where T : IComparable {
            if ( target is null ) {
                throw new ArgumentNullException( nameof( target ) );
            }

            if ( startInclusive is null ) {
                throw new ArgumentNullException( nameof( startInclusive ) );
            }

            if ( endInclusive is null ) {
                throw new ArgumentNullException( nameof( endInclusive ) );
            }

            return target.CompareTo( startInclusive ) >= 0 && target.CompareTo( endInclusive ) <= 0;
        }

        /// <summary>Returns a new <typeparamref name="T" /> that is the value of <paramref name="self" />, constrained between <paramref name="min" /> and <paramref name="max" />.</summary>
        /// <param name="self">The extended T.</param>
        /// <param name="min"> The minimum value of the <typeparamref name="T" /> that can be returned.</param>
        /// <param name="max"> The maximum value of the <typeparamref name="T" /> that can be returned.</param>
        /// <returns>The equivalent to: <c>this &lt; min ? min : this &gt; max ? max : this</c>.</returns>
        [NotNull]
        public static T Clamp<T>( [NotNull] this T self, [NotNull] T min, [NotNull] T max ) where T : IComparable<T> {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ) );
            }

            if ( min is null ) {
                throw new ArgumentNullException( nameof( min ) );
            }

            if ( max is null ) {
                throw new ArgumentNullException( nameof( max ) );
            }

            if ( self.CompareTo( min ) < 0 ) {
                return min;
            }

            return self.CompareTo( max ) > 0 ? max : self;
        }

        [ItemCanBeNull]
        public static IEnumerable<T> Concat<T>( [NotNull] this IEnumerable<T> first, [CanBeNull] T second ) {
            foreach ( var item in first ) {
                yield return item;
            }

            yield return second;
        }

        /// <summary>Just a no-op for setting a breakpoint on.</summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        [Conditional( "DEBUG" )]
        public static void Nop<T>( [CanBeNull] this T obj ) { }

        /// <summary>
        ///     <para>Works like the SQL "nullif" function.</para>
        ///     <para>If <paramref name="left" /> is equal to <paramref name="right" /> then return null (or the default value for value types).</para>
        ///     <para>Otherwise return <paramref name="left" />.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        [CanBeNull]
        [DebuggerStepThrough]
        public static T NullIf<T>( [NotNull] this T left, [CanBeNull] T right ) => Comparer<T>.Default.Compare( left, right ) == 0 ? default : left;

        [CanBeNull]
        public static String OnlyDigits( [CanBeNull] this String input ) => String.IsNullOrWhiteSpace( input ) ? null : new String( input.Where( Char.IsDigit ).ToArray() );

        [CanBeNull]
        public static String OnlyLetters( [CanBeNull] String input ) => String.IsNullOrWhiteSpace( input ) ? null : new String( input.Where( Char.IsLetter ).ToArray() );

        [CanBeNull]
        public static String OnlyLettersAndNumbers( [CanBeNull] String input ) =>
            String.IsNullOrWhiteSpace( input ) ? null : new String( input.Where( c => Char.IsDigit( c ) || Char.IsLetter( c ) ).ToArray() );

        /// <summary>Swap <paramref name="left" /> with <paramref name="right" />.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void Swap<T>( [CanBeNull] ref T left, [CanBeNull] ref T right ) {
            var temp = left;
            left = right;
            right = temp;
        }

        /// <summary>Given (T left, T right), Return (T right, T left).</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static (T right, T left) Swap<T>( [CanBeNull] this T left, [CanBeNull] T right ) => (right, left);
    }
}