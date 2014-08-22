#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Types.cs" was last cleaned by Rick on 2014/08/15 at 2:22 PM
#endregion

namespace Librainian.Extensions {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Annotations;
    using Threading;

    public static class Types {
        public static void CopyField<TSource>( this TSource source, TSource destination, [NotNull] FieldInfo field, Boolean mergeDictionaries = true ) {
            if ( field == null ) {
                throw new ArgumentNullException( "field" );
            }
            try {
                var sourceValue = field.GetValue( source );

                if ( mergeDictionaries && sourceValue is IDictionary && MergeDictions( sourceValue as IDictionary, field, destination ) ) {
                    return;
                }

                field.SetValue( destination, sourceValue );
            }
            catch ( TargetException exception ) {
                exception.Error();
            }
            catch ( NotSupportedException exception ) {
                exception.Error();
            }
            catch ( FieldAccessException exception ) {
                exception.Error();
            }
            catch ( ArgumentException exception ) {
                exception.Error();
            }
        }

        public static Boolean MergeDictions<TSource>( IDictionary sourceValue, FieldInfo field, TSource destination ) {
            if ( null == sourceValue ) {
                return false;
            }
            var destAsDictionary = field.GetValue( destination ) as IDictionary;
            if ( null == destAsDictionary ) {
                return false;
            }
            foreach ( var key in sourceValue.Keys ) {
                try {
                    destAsDictionary[ key ] = sourceValue[ key ];
                }
                catch ( Exception exception ) {
                    exception.Error();
                }
            }
            return false;
        }

        /// <summary>
        ///     Copy the value of each field of the <paramref name="source" /> to the matching field in
        ///     the <paramref name="destination" /> .
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static Boolean CopyFields<TSource>( this TSource source, TSource destination ) {
            try {
                var sourceFields = source.GetType().GetAllFields();
                var destFields = destination.GetType().GetAllFields();

                foreach ( var field in sourceFields.Where( destFields.Contains ) ) {
                    CopyField( source: source, destination: destination, field: field );
                }
                return true;
            }
            catch ( Exception ) {
                return false;
            }
        }

        /// <summary>
        ///     Copy the value of each get property of the <paramref name="source" /> to each set
        ///     property of the <paramref name="destination" /> .
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static Boolean CopyProperties<TSource>( this TSource source, TSource destination ) {
            try {
                var sourceProps = source.GetType().GetAllProperties().Where( prop => prop.CanRead );
                var destProps = destination.GetType().GetAllProperties().Where( prop => prop.CanWrite );

                foreach ( var prop in sourceProps.Where( destProps.Contains ) ) {
                    CopyProperty( source: source, destination: destination, prop: prop );
                }
                return true;
            }
            catch ( Exception ) {
                return false;
            }
        }

        public static void CopyProperty<TSource>( this TSource source, TSource destination, [NotNull] PropertyInfo prop ) {
            if ( prop == null ) {
                throw new ArgumentNullException( "prop" );
            }
            try {
                var sourceValue = prop.GetValue( source, null );
                prop.SetValue( destination, sourceValue, null );
            }
            catch ( TargetParameterCountException exception ) {
                exception.Error();
            }
            catch ( TargetException exception ) {
                exception.Error();
            }
            catch ( NotSupportedException exception ) {
                exception.Error();
            }
            catch ( FieldAccessException exception ) {
                exception.Error();
            }
            catch ( ArgumentException exception ) {
                exception.Error();
            }
        }

        /// <summary>
        ///     <para>
        ///         Copy each field in the <paramref name="source" /> to the matching field in the
        ///         <paramref name="destination" />.
        ///     </para>
        ///     <para>
        ///         then Copy each property in the
        ///         <paramref
        ///             name="source" />
        ///         to the matching property in the <paramref name="destination" />.
        ///     </para>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static Boolean DeepClone<TSource>( this TSource source, TSource destination ) {
            if ( ReferenceEquals( source, destination ) ) {
                return false;
            }
            if ( Equals( source, default( TSource ) ) ) {
                return false;
            }
            if ( Equals( destination, default( TSource ) ) ) {
                return false;
            }

            //copy all settable fields
            // then
            //copy all settable properties (going on the assumption that properties should be modifiying their private fields).
            return CopyFields( source: source, destination: destination ) && CopyProperties( source: source, destination: destination );
        }

        /// <summary>
        ///     Enumerate all fields of the <paramref name="type" />
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetAllFields( [CanBeNull] this Type type ) {
            if ( null == type ) {
                return Enumerable.Empty<FieldInfo>();
            }

            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return type.GetFields( flags ).Union( GetAllFields( type.BaseType ) );
        }

        /// <summary>
        ///     Enumerate all properties of the <paramref name="type" />
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetAllProperties( [CanBeNull] this Type type ) {
            if ( null == type ) {
                return Enumerable.Empty<PropertyInfo>();
            }

            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return type.GetProperties( flags ).Union( GetAllProperties( type.BaseType ) );
        }

/*
        public static string GetName<T>( [CanBeNull] this Expression<Func<T>> propertyExpression ) {
            if ( null == propertyExpression ) {
                return String.Empty;
            }
            var memberExpression = propertyExpression.Body as MemberExpression;
            return memberExpression != null ? memberExpression.Member.Name : String.Empty;
        }
*/

        public static string GetPropertyName<T>( [CanBeNull] this Expression<Func<T>> propertyExpression ) {
            if ( propertyExpression == null ) {
                throw new ArgumentNullException( "propertyExpression" );
            }
            var memberExpression = propertyExpression.Body as MemberExpression;
            return memberExpression != null ? memberExpression.Member.Name : String.Empty;
        }

/*
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getMethod"></param>
        /// <returns></returns>
        /// <seealso cref="http://stackoverflow.com/a/557711"/>
        public static T GetProperty<T>( MethodBase getMethod ) {
            if ( !getMethod.Name.StartsWith( "get_" ) ) {
                throw new ArgumentException(
                    "GetProperty must be called from a property" );
            }
            return GetValue<T>( getMethod.Name.Substring( 4 ) );
        }
*/

        /// <summary>
        ///     Get all <see cref="Type" /> from <see cref="AppDomain.CurrentDomain" /> that should be
        ///     able to be created via
        ///     <see cref="Activator.CreateInstance(Type,BindingFlags,Binder,object[],CultureInfo) " />.
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypesDerivedFrom( [CanBeNull] this Type baseType ) {
            if ( baseType == null ) {
                throw new ArgumentNullException( "baseType" );
            }
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany( assembly => assembly.GetTypes(), ( assembly, type ) => type ).Where( arg => baseType.IsAssignableFrom( arg ) && arg.IsClass && !arg.IsAbstract );
        }

        public static Func<object> NewInstanceByLambda( [NotNull] this Type type ) {
            if ( type == null ) {
                throw new ArgumentNullException( "type" );
            }
            return Expression.Lambda<Func<object>>( Expression.New( type ) ).Compile();
        }

        public static Func<object> NewInstanceByCreate( [NotNull] this Type type ) {
            if ( type == null ) {
                throw new ArgumentNullException( "type" );
            }
            var localType = type; // create a local copy to prevent adverse effects of closure
            Func<object> func = ( () => Activator.CreateInstance( localType ) ); // curry the localType
            return func;
        }
    }
}
