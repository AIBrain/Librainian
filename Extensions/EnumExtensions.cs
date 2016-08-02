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
// "Librainian/EnumExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

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
        /// <param name="value">The value.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     <c>true</c> if value contains the specified value; otherwise, <c>false</c>.
        /// </returns>
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
        ///     Gets all items for an enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllItems<T>( this Enum value ) {
            return from Object item in Enum.GetValues( typeof( T ) ) select ( T )item;
        }

        /// <summary>
        ///     Gets all items for an enum type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAllItems<T>() where T : struct {
            return Enum.GetValues( typeof( T ) ).Cast<T>();
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
        public static IEnumerable<T> GetAllSelectedItems<T>( this Enum value ) {
            var valueAsInt = Convert.ToInt32( value, CultureInfo.InvariantCulture );

            return from Object item in Enum.GetValues( typeof( T ) ) let itemAsInt = Convert.ToInt32( item, CultureInfo.InvariantCulture ) where itemAsInt == ( valueAsInt & itemAsInt ) select ( T )item;
        }

        // This extension method is broken out so you can use a similar pattern with
        // other MetaData elements in the future. This is your base method for each.
        public static T GetAttribute<T>( this Enum value ) where T : Attribute {
            var type = value.GetType();
            var memberInfo = type.GetMember( value.ToString() );
            var attributes = memberInfo[ 0 ].GetCustomAttributes( typeof( T ), false );
            return ( T )attributes[ 0 ];
        }

        public static IEnumerable<T> GetEnums<T>( this T hmm ) => Enum.GetValues( typeof( T ) ).Cast<T>();

        /// <summary>
        /// Returns the text of the [Description("text")] attribute on an enum. Or null if not found.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [ CanBeNull ]
        public static String Description( this Enum element ) {
            var type = element.GetType();

            var memberInfo = type.GetMember( element.ToString() );

            if ( !memberInfo.Any() ) {
                return null; //element.ToString();
            }
            var attributes = memberInfo[ 0 ].GetCustomAttributes( typeof( DescriptionAttribute ), false );

            return attributes.Any() ? ( attributes[ 0 ] as DescriptionAttribute )?.Description : null; //element.ToString();
        }
    }
}