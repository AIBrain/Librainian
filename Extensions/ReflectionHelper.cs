// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ReflectionHelper.cs" belongs to Rick@AIBrain.org and
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
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "ReflectionHelper.cs" was last formatted by Protiguous on 2018/06/04 at 3:53 PM.

namespace Librainian.Extensions {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using JetBrains.Annotations;

	public static class ReflectionHelper {

		/// <summary>
		///     Find all types in 'assembly' that derive from 'baseType'
		/// </summary>
		/// <owner>jayBaz</owner>
		public static IEnumerable<Type> FindAllTypesThatDeriveFrom<TBase>( [NotNull] this Assembly assembly ) {
			if ( assembly == null ) { throw new ArgumentNullException( paramName: nameof( assembly ) ); }

			return assembly.GetTypes().Where( type => type.IsSubclassOf( typeof( TBase ) ) );
		}

		public static IEnumerable<FieldInfo> GetAllDeclaredInstanceFields( [NotNull] this Type type ) {
			if ( type == null ) { throw new ArgumentNullException( paramName: nameof( type ) ); }

			return type.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly );
		}

		/// <summary>
		///     A typesafe wrapper for Attribute.GetCustomAttribute
		/// </summary>
		/// <remarks>TODO: add overloads for Assembly, Module, and ParameterInfo</remarks>
		public static TAttribute GetCustomAttribute<TAttribute>( [NotNull] this MemberInfo element ) where TAttribute : Attribute {
			if ( element == null ) { throw new ArgumentNullException( paramName: nameof( element ) ); }

			return Attribute.GetCustomAttribute( element, typeof( TAttribute ) ) as TAttribute;
		}

		/// <summary>
		///     All types across multiple assemblies
		/// </summary>
		public static IEnumerable<Type> GetTypes( [NotNull] this IEnumerable<Assembly> assemblies ) {
			if ( assemblies == null ) { throw new ArgumentNullException( paramName: nameof( assemblies ) ); }

			return assemblies.SelectMany( assembly => assembly.GetTypes() );
		}

		/// <summary>
		///     Check if the given type has the given attribute on it. Don't look at base classes.
		/// </summary>
		/// <owner>jayBaz</owner>
		public static Boolean TypeHasAttribute<TAttribute>( [NotNull] this Type type ) where TAttribute : Attribute {
			if ( type == null ) { throw new ArgumentNullException( paramName: nameof( type ) ); }

			return Attribute.IsDefined( type, typeof( TAttribute ) );
		}

	}

}