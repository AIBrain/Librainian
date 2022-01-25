// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "ImmutableAttribute.cs" last formatted on 2022-12-22 at 5:15 PM by Protiguous.

namespace Librainian.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exceptions;
using Newtonsoft.Json;

/// <summary>http://blogs.msdn.com/b/kevinpilchbisson/archive/2007/11/20/enforcing-immutability-in-code.aspx</summary>
[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct )]
[JsonObject]
public sealed class ImmutableAttribute : Attribute {

	public Boolean OnFaith { get; set; }

	private static Boolean IsMarkedImmutable( Type type ) {
		if ( type is null ) {
			throw new ArgumentEmptyException( nameof( type ) );
		}

		return type.TypeHasAttribute<ImmutableAttribute>();
	}

	private static Boolean IsWhiteListed( Type type ) {
		if ( type is null ) {
			throw new ArgumentEmptyException( nameof( type ) );
		}

		// Boolean, int, etc.
		if ( type.IsPrimitive ) {
			return true;
		}

		if ( type == typeof( Object ) ) {
			return true;
		}

		if ( type == typeof( String ) ) {
			return true;
		}

		if ( type == typeof( Guid ) ) {
			return true;
		}

		if ( type.IsEnum ) {
			return true;
		}

		// override all checks on this type if [ImmutableAttribute(OnFaith=true)] is set
		var immutableAttribute = type.GetCustomAttribute<ImmutableAttribute>();

		return immutableAttribute is { OnFaith: true };
	}

	/// <summary>Ensures that 'type' follows the rules for immutability</summary>
	/// <exception cref="ImmutableFailureException">Thrown if a mutability issue appears.</exception>
	public static void VerifyTypeIsImmutable( Type type, IEnumerable<Type> whiteList ) {
		if ( type is null ) {
			throw new ArgumentEmptyException( nameof( type ) );
		}

		if ( type.BaseType is null ) {
			throw new ArgumentEmptyException( nameof( type ) );
		}

		if ( whiteList is null ) {
			throw new ArgumentEmptyException( nameof( whiteList ) );
		}

		var list = whiteList as IList<Type> ?? whiteList.ToList();

		if ( list.Contains( type ) ) {
			return;
		}

		if ( IsWhiteListed( type ) ) {
			return;
		}

		try {
			VerifyTypeIsImmutable( type.BaseType, list );
		}
		catch ( ImmutableFailureException ex ) {
			throw new MutableBaseException( type, ex );
		}

		foreach ( var fieldInfo in type.GetAllDeclaredInstanceFields() ) {
			if ( ( fieldInfo.Attributes & FieldAttributes.InitOnly ) == 0 ) {
				throw new WritableFieldException( fieldInfo );
			}

			// if it's marked with [Extensions.Immutable], that's good enough, as we can be sure that these tests will all be applied to
			// this type
			if ( IsMarkedImmutable( fieldInfo.FieldType ) ) {
				continue;
			}

			try {
				VerifyTypeIsImmutable( fieldInfo.FieldType, list );
			}
			catch ( ImmutableFailureException ex ) {
				throw new MutableFieldException( fieldInfo, ex );
			}
		}
	}

	/// <summary>
	///     Ensures that all types in 'assemblies' that are marked [Extensions.Immutable] follow the rules for
	///     immutability.
	/// </summary>
	/// <exception cref="ImmutableFailureException">Thrown if a mutability issue appears.</exception>
	public static void VerifyTypesAreImmutable( IEnumerable<Assembly> assemblies, params Type[] whiteList ) {
		if ( assemblies is null ) {
			throw new ArgumentEmptyException( nameof( assemblies ) );
		}

		if ( whiteList == null ) {
			throw new ArgumentEmptyException( nameof( whiteList ) );
		}

		var typesMarkedImmutable = from type in assemblies.GetTypes() where IsMarkedImmutable( type ) select type;

		foreach ( var type in typesMarkedImmutable ) {
			VerifyTypeIsImmutable( type, whiteList );
		}
	}
}