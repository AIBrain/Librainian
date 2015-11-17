// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/EnumExtensions.cs" was last cleaned by Rick on 2015/10/03 at 2:15 AM

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Pulled from <see cref="http://stackoverflow.com/a/944352/956364"/>
    /// </summary>
    public static class EnumExtensions {

        /// <summary>
        /// Gets all items for an enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IEnumerable< T > GetAllItems< T >( this Enum value ) {
            return Enum.GetValues( typeof ( T ) )
                       .Cast< Object >()
                       .Select( item => ( T ) item );
        }

        /// <summary>
        /// Gets all items for an enum type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable< T > GetAllItems< T >() where T : struct {
            return Enum.GetValues( typeof ( T ) )
                       .Cast< T >();
        }

        /// <summary>
        /// Gets all combined items from an enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <example>
        /// Displays ValueA and ValueB.
        /// <code>
        /// EnumExample dummy = EnumExample.Combi;
        /// foreach (var item in dummy.GetAllSelectedItems /\EnumExample/\())
        /// {
        ///    Console.WriteLine(item);
        /// }
        /// </code>
        /// </example>
        public static IEnumerable< T > GetAllSelectedItems< T >( this Enum value ) {
            var valueAsInt = Convert.ToInt32( value, CultureInfo.InvariantCulture );

            return Enum.GetValues( typeof ( T ) )
                       .Cast< Object >()
                       .Select( item => new {item, itemAsInt = Convert.ToInt32( item, CultureInfo.InvariantCulture )} )
                       .Where( @t => @t.itemAsInt == ( valueAsInt & @t.itemAsInt ) )
                       .Select( @t => ( T ) @t.item );
        }

        /// <summary>
        /// Determines whether the enum value contains a specific value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     <c>true</c> if value contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// <code>
        /// EnumExample dummy = EnumExample.Combi;
        /// if (dummy.Contains/\EnumExample/\(EnumExample.ValueA))
        /// {
        ///     Console.WriteLine("dummy contains EnumExample.ValueA");
        /// }
        /// </code>
        /// </example>
        public static Boolean Contains< T >( this Enum value, T request ) {
            var valueAsInt = Convert.ToInt32( value, CultureInfo.InvariantCulture );
            var requestAsInt = Convert.ToInt32( request, CultureInfo.InvariantCulture );

            return requestAsInt == ( valueAsInt & requestAsInt );
        }

    }

}
