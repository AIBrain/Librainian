// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "EnumExtensions.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "EnumExtensions.cs" was last formatted by Protiguous on 2019/08/08 at 7:12 AM.

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>
    ///     Pulled from <see cref="http://stackoverflow.com/a/944352/956364" />
    /// </summary>
    public static class EnumExtensions {

        /// <summary>
        ///     Determines whether the enum value contains a specific value. The enum itself must be decorated with the
        ///     FlagsAttribute.
        /// </summary>
        /// <param name="value">  The value.</param>
        /// <param name="request">The request.</param>
        /// <returns><c>true</c> if value contains the specified value; otherwise, <c>false</c>.</returns>
        /// <example>
        ///     <code>
        /// EnumExample dummy = EnumExample.Combi;
        /// if (dummy.Contains/\EnumExample/\(EnumExample.ValueA))
        /// {
        ///     Console.WriteLine("dummy contains EnumExample.ValueA");
        /// }
        /// </code>
        /// </example>
        public static Boolean Contains<T>( this Enum value, T request ) {
            var valueAsInt = Convert.ToInt32( value );
            var requestAsInt = Convert.ToInt32( request );

            return requestAsInt == ( valueAsInt & requestAsInt ); //TODO what??
        }

        /// <summary>
        ///     Returns the text of the [Description("text")] attribute on an enum. Or null if not found.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [CanBeNull]
        public static String Description( [NotNull] this Enum element ) {
            var type = element.GetType();

            var memberInfo = type.GetMember( element.ToString() );

            if ( !memberInfo.Any() ) {
                return null; //element.ToString();
            }

            var attributes = memberInfo[ 0 ].GetCustomAttributes( typeof( DescriptionAttribute ), false );

            return attributes.Any() ? ( attributes[ 0 ] as DescriptionAttribute )?.Description : null; //element.ToString();
        }

        /// <summary>
        ///     Gets all combined items from an enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <example>
        ///     Displays ValueA and ValueB.
        ///     <code>
        /// EnumExample dummy = EnumExample.Combi;
        /// foreach (var item in dummy.GetAllSelectedItems /\EnumExample/\())
        /// {
        ///    Console.WriteLine(item);
        /// }
        /// </code>
        /// </example>
        [NotNull]
        public static IEnumerable<T> GetAllSelectedItems<T>( this Enum value ) {
            var valueAsInt = Convert.ToInt32( value );

            return from Object item in Enum.GetValues( typeof( T ) ) let itemAsInt = Convert.ToInt32( item ) where itemAsInt == ( valueAsInt & itemAsInt ) select ( T ) item;
        }

        /// <summary>
        ///     Gets all items for an enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [NotNull]
        public static IEnumerable<T> GetAllValues<T>( [NotNull] this Enum value ) {
            if ( value == null ) {
                throw new ArgumentNullException( paramName: nameof( value ) );
            }

            return Enum.GetValues( value.GetType() ).Cast<Object>().Select( item => ( T ) item );
        }

        /// <summary>
        ///     Gets all values for an enum type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [NotNull]
        public static IEnumerable<T> GetAllValues<T>() where T : struct => Enum.GetValues( typeof( T ) ).Cast<T>();

        // This extension method is broken out so you can use a similar pattern with other MetaData elements in the future. This is your base method for each.
        public static T GetAttribute<T>( [NotNull] this Enum value ) where T : Attribute {
            var type = value.GetType();
            var memberInfo = type.GetMember( value.ToString() );
            var attributes = memberInfo[ 0 ].GetCustomAttributes( typeof( T ), false );

            return ( T ) attributes[ 0 ];
        }

        [NotNull]
        public static IEnumerable<T> GetEnums<T>( this T _ ) => Enum.GetValues( typeof( T ) ).Cast<T>();
    }
}