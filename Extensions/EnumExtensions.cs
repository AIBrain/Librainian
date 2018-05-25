// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "EnumExtensions.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/EnumExtensions.cs" was last formatted by Protiguous on 2018/05/24 at 7:07 PM.

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
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
            var valueAsInt = Convert.ToInt32( value, CultureInfo.InvariantCulture );
            var requestAsInt = Convert.ToInt32( request, CultureInfo.InvariantCulture );

            return requestAsInt == ( valueAsInt & requestAsInt );
        }

        /// <summary>
        ///     Returns the text of the [Description("text")] attribute on an enum. Or null if not found.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [CanBeNull]
        public static String Description( this Enum element ) {
            var type = element.GetType();

            var memberInfo = type.GetMember( element.ToString() );

            if ( !memberInfo.Any() ) {
                return null; //element.ToString();
            }

            var attributes = memberInfo[0].GetCustomAttributes( typeof( DescriptionAttribute ), false );

            return attributes.Any() ? ( attributes[0] as DescriptionAttribute )?.Description : null; //element.ToString();
        }

        /// <summary>
        ///     Gets all items for an enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllItems<T>( this Enum value ) => from Object item in Enum.GetValues( typeof( T ) ) select ( T )item;

        /// <summary>
        ///     Gets all items for an enum type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAllItems<T>() where T : struct => Enum.GetValues( typeof( T ) ).Cast<T>();

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
        public static IEnumerable<T> GetAllSelectedItems<T>( this Enum value ) {
            var valueAsInt = Convert.ToInt32( value, CultureInfo.InvariantCulture );

            return from Object item in Enum.GetValues( typeof( T ) ) let itemAsInt = Convert.ToInt32( item, CultureInfo.InvariantCulture ) where itemAsInt == ( valueAsInt & itemAsInt ) select ( T )item;
        }

        // This extension method is broken out so you can use a similar pattern with other MetaData elements in the future. This is your base method for each.
        public static T GetAttribute<T>( this Enum value ) where T : Attribute {
            var type = value.GetType();
            var memberInfo = type.GetMember( value.ToString() );
            var attributes = memberInfo[0].GetCustomAttributes( typeof( T ), false );

            return ( T )attributes[0];
        }

        public static IEnumerable<T> GetEnums<T>( this T hmm ) => Enum.GetValues( typeof( T ) ).Cast<T>();
    }
}