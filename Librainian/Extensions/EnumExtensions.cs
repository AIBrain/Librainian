﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "EnumExtensions.cs" last formatted on 2021-11-30 at 7:17 PM by Protiguous.

#nullable enable

namespace Librainian.Extensions;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Exceptions;

/// <summary>Pulled from <see cref="http://stackoverflow.com/a/944352/956364" /></summary>
public static class EnumExtensions {

	/// <summary>Returns the text of the [Description("text")] attribute on an enum. Or null if not found.</summary>
	/// <param name="element"></param>
	public static String? Description( this Enum element ) {
		var type = element.GetType();

		var memberInfo = type.GetMember( element.ToString() );

		if ( !memberInfo.Any() ) {
			return default( String? ); //element.ToString();
		}

		var attributes = memberInfo[ 0 ].GetCustomAttributes( typeof( DescriptionAttribute ), false );

		if ( attributes.Any() ) {
			return ( attributes[ 0 ] as DescriptionAttribute )?.Description;
		}

		return null;
	}

	/// <summary>Gets all combined items from an enum value.</summary>
	public static IEnumerable<T> GetAllSelectedItems<T>( this Enum? value ) {
		var valueAsInt = Convert.ToInt32( value );

		return from Object item in Enum.GetValues( typeof( T ) ) let itemAsInt = Convert.ToInt32( item ) where itemAsInt == ( valueAsInt & itemAsInt ) select ( T )item;
	}

	/// <summary>Gets all items for an enum value.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="value">The value.</param>
	public static IEnumerable<T> GetAllValues<T>( this Enum value ) {
		if ( value is null ) {
			throw new NullException( nameof( value ) );
		}

		return Enum.GetValues( value.GetType() ).Cast<Object>().Select( item => ( T )item );
	}

	/// <summary>Gets all values for an enum type.</summary>
	/// <typeparam name="T"></typeparam>
	public static IEnumerable<T> GetAllValues<T>() where T : struct => Enum.GetValues( typeof( T ) ).Cast<T>();

	// This extension method is broken out so you can use a similar pattern with other MetaData elements in the future. This is
	// your base method for each.
	public static T GetAttribute<T>( this Enum value ) where T : Attribute {
		var type = value.GetType();
		var memberInfo = type.GetMember( value.ToString() );
		var attributes = memberInfo[ 0 ].GetCustomAttributes( typeof( T ), false );

		return ( T )attributes[ 0 ];
	}

	public static String? GetDescription<T>( this T? e ) where T : IConvertible {
		if ( e is not Enum ) {
			return default( String? );
		}

		var type = e.GetType();

		foreach ( Int32 val in Enum.GetValues( type ) ) {
			if ( val != e.ToInt32( CultureInfo.InvariantCulture ) ) {
				continue;
			}

			var ename = type.GetEnumName( val );

			if ( ename is null ) {
				continue;
			}

			var memInfo = type.GetMember( ename );

			if ( memInfo[ 0 ].GetCustomAttributes( typeof( DescriptionAttribute ), false ).FirstOrDefault() is DescriptionAttribute descriptionAttribute ) {
				return descriptionAttribute.Description;
			}
		}

		return default( String? );
	}

	public static IEnumerable<T> GetEnums<T>( this T? _ ) => Enum.GetValues( typeof( T ) ).Cast<T>();
}