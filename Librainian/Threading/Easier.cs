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
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Easier.cs" last touched on 2021-10-13 at 4:31 PM by Protiguous.

namespace Librainian.Threading;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

/// <summary>
///     Provides lazy initialization routines.
/// </summary>
/// <remarks>
///     These routines avoid needing to allocate a dedicated, lazy-initialization instance, instead using
///     references to ensure targets have been initialized as they are accessed.
/// </remarks>
public static class Easier {

	/// <summary>
	///     Initializes a target reference type with the type's default constructor (slow path)
	/// </summary>
	/// <typeparam name="T">The reference type of the reference to be initialized.</typeparam>
	/// <param name="target">The variable that need to be initialized</param>
	/// <returns>The initialized variable</returns>
	private static T EnsureInitializedCore<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicParameterlessConstructor )] T>( [NotNull] ref T? target )
		where T : class {
		Interlocked.CompareExchange( ref target, Activator.CreateInstance<T>(), null );

		return target;
	}

	/// <summary>
	///     Initialize the target using the given delegate (slow path).
	/// </summary>
	/// <typeparam name="T">The reference type of the reference to be initialized.</typeparam>
	/// <param name="target">The variable that need to be initialized</param>
	/// <param name="valueFactory">The delegate that will be executed to initialize the target</param>
	/// <returns>The initialized variable</returns>
	private static T EnsureInitializedCore<T>( [NotNull] ref T? target, Func<T> valueFactory ) where T : class {
		var value = valueFactory();
		if ( value == null ) {
			throw new InvalidOperationException();
		}

		Interlocked.CompareExchange( ref target, value, null );
		return target;
	}

	/// <summary>
	///     Ensure the target is initialized and return the value (slow path). This overload permits nulls
	///     and also works for value type targets. Uses the type's default constructor to create the value.
	/// </summary>
	/// <typeparam name="T">The type of target.</typeparam>
	/// <param name="target">A reference to the target to be initialized.</param>
	/// <param name="initialized">A reference to a location tracking whether the target has been initialized.</param>
	/// <param name="syncLock">
	///     A reference to a location containing a mutual exclusive lock. If <paramref name="syncLock" /> is null,
	///     a new object will be instantiated.
	/// </param>
	/// <returns>The initialized object.</returns>
	private static T? EnsureInitializedCore<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicParameterlessConstructor )] T>(
		ref T? target,
		ref Boolean initialized,
		[NotNull] ref Object? syncLock
	) {
		// Lazily initialize the lock if necessary and then double check if initialization is still required.
		lock ( EnsureLockInitialized( ref syncLock ) ) {
			if ( !Volatile.Read( ref initialized ) ) {
				try {
					target = Activator.CreateInstance<T>();
				}
				catch ( MissingMethodException ) {
					throw new MissingMemberException();
				}

				Volatile.Write( ref initialized, true );
			}
		}

		return target;
	}

	/// <summary>
	///     Ensure the target is initialized and return the value (slow path). This overload permits nulls
	///     and also works for value type targets. Uses the supplied function to create the value.
	/// </summary>
	/// <typeparam name="T">The type of target.</typeparam>
	/// <param name="target">A reference to the target to be initialized.</param>
	/// <param name="initialized">A reference to a location tracking whether the target has been initialized.</param>
	/// <param name="syncLock">
	///     A reference to a location containing a mutual exclusive lock. If <paramref name="syncLock" /> is null,
	///     a new object will be instantiated.
	/// </param>
	/// <param name="valueFactory">
	///     The <see cref="System.Func{T}" /> to invoke in order to produce the lazily-initialized value.
	/// </param>
	/// <returns>The initialized object.</returns>
	private static T? EnsureInitializedCore<T>( [AllowNull] ref T target, ref Boolean initialized, [NotNull] ref Object? syncLock, Func<T> valueFactory ) {
		// Lazily initialize the lock if necessary and then double check if initialization is still required.
		lock ( EnsureLockInitialized( ref syncLock ) ) {
			if ( !Volatile.Read( ref initialized ) ) {
				target = valueFactory();
				Volatile.Write( ref initialized, true );
			}
		}

		return target;
	}

	/// <summary>
	///     Ensure the target is initialized and return the value (slow path). This overload works only for reference type
	///     targets.
	///     Uses the supplied function to create the value.
	/// </summary>
	/// <typeparam name="T">The type of target. Has to be reference type.</typeparam>
	/// <param name="target">A reference to the target to be initialized.</param>
	/// <param name="syncLock">
	///     A reference to a location containing a mutual exclusive lock. If <paramref name="syncLock" /> is null,
	///     a new object will be instantiated.
	/// </param>
	/// <param name="valueFactory">
	///     The <see cref="System.Func{T}" /> to invoke in order to produce the lazily-initialized value.
	/// </param>
	/// <returns>The initialized object.</returns>
	private static T EnsureInitializedCore<T>( [NotNull] ref T? target, [NotNull] ref Object? syncLock, Func<T> valueFactory ) where T : class {
		// Lazily initialize the lock if necessary and then double check if initialization is still required.
		lock ( EnsureLockInitialized( ref syncLock ) ) {
			if ( Volatile.Read( ref target ) == null ) {
				Volatile.Write( ref target, valueFactory() );
				if ( target == null ) {
					throw new InvalidOperationException();
				}
			}
		}

		Debug.Assert( target != null );
		return target;
	}

	/// <summary>
	///     Ensure the lock object is initialized.
	/// </summary>
	/// <param name="syncLock">
	///     A reference to a location containing a mutual exclusive lock. If <paramref name="syncLock" /> is null,
	///     a new object will be instantiated.
	/// </param>
	/// <returns>Initialized lock object.</returns>
	private static Object EnsureLockInitialized( [NotNull] ref Object? syncLock ) => syncLock ?? Interlocked.CompareExchange( ref syncLock, new Object(), null ) ?? syncLock;

	/// <summary>
	///     Initializes a target reference type with the type's default constructor if the target has not
	///     already been initialized.
	/// </summary>
	/// <typeparam name="T">The reference type of the reference to be initialized.</typeparam>
	/// <param name="target">
	///     A reference of type <typeparamref name="T" /> to initialize if it has not
	///     already been initialized.
	/// </param>
	/// <returns>The initialized reference of type <typeparamref name="T" />.</returns>
	/// <exception cref="System.MissingMemberException">
	///     Type <typeparamref name="T" /> does not have a default
	///     constructor.
	/// </exception>
	/// <exception cref="System.MemberAccessException">
	///     Permissions to access the constructor of type <typeparamref name="T" /> were missing.
	/// </exception>
	/// <remarks>
	///     <para>
	///         This method may only be used on reference types. To ensure initialization of value
	///         types, see other overloads of EnsureInitialized.
	///     </para>
	///     <para>
	///         This method may be used concurrently by multiple threads to initialize <paramref name="target" />.
	///         In the event that multiple threads access this method concurrently, multiple instances of
	///         <typeparamref name="T" />
	///         may be created, but only one will be stored into <paramref name="target" />. In such an occurrence, this method
	///         will not dispose of the
	///         objects that were not stored.  If such objects must be disposed, it is up to the caller to determine
	///         if an object was not used and to then dispose of the object appropriately.
	///     </para>
	/// </remarks>
	public static T EnsureInitialized<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicParameterlessConstructor )] T>( [NotNull] ref T? target )
		where T : class {
		if ( target is null ) {
			return EnsureInitializedCore( ref target );
		}

		return Volatile.Read( ref target );
	}

	/// <summary>
	///     Initializes a target reference type using the specified function if it has not already been
	///     initialized.
	/// </summary>
	/// <typeparam name="T">The reference type of the reference to be initialized.</typeparam>
	/// <param name="target">
	///     The reference of type <typeparamref name="T" /> to initialize if it has not
	///     already been initialized.
	/// </param>
	/// <param name="valueFactory">
	///     The <see cref="System.Func{T}" /> invoked to initialize the
	///     reference.
	/// </param>
	/// <returns>The initialized reference of type <typeparamref name="T" />.</returns>
	/// <exception cref="System.MissingMemberException">
	///     Type <typeparamref name="T" /> does not have a
	///     default constructor.
	/// </exception>
	/// <exception cref="System.InvalidOperationException">
	///     <paramref name="valueFactory" /> returned
	///     null.
	/// </exception>
	/// <remarks>
	///     <para>
	///         This method may only be used on reference types, and <paramref name="valueFactory" /> may
	///         not return a null reference (Nothing in Visual Basic). To ensure initialization of value types or
	///         to allow null reference types, see other overloads of EnsureInitialized.
	///     </para>
	///     <para>
	///         This method may be used concurrently by multiple threads to initialize <paramref name="target" />.
	///         In the event that multiple threads access this method concurrently, multiple instances of
	///         <typeparamref name="T" />
	///         may be created, but only one will be stored into <paramref name="target" />. In such an occurrence, this method
	///         will not dispose of the
	///         objects that were not stored.  If such objects must be disposed, it is up to the caller to determine
	///         if an object was not used and to then dispose of the object appropriately.
	///     </para>
	/// </remarks>
	public static T EnsureInitialized<T>( [NotNull] ref T? target, Func<T> valueFactory ) where T : class {
		if ( target is null ) {
			return EnsureInitializedCore( ref target, valueFactory );
		}

		return Volatile.Read( ref target );
	}

	/// <summary>
	///     Initializes a target reference or value type with its default constructor if it has not already
	///     been initialized.
	/// </summary>
	/// <typeparam name="T">The type of the reference to be initialized.</typeparam>
	/// <param name="target">
	///     A reference or value of type <typeparamref name="T" /> to initialize if it
	///     has not already been initialized.
	/// </param>
	/// <param name="initialized">
	///     A reference to a boolean that determines whether the target has already
	///     been initialized.
	/// </param>
	/// <param name="syncLock">
	///     A reference to an object used as the mutually exclusive lock for initializing
	///     <paramref name="target" />. If <paramref name="syncLock" /> is null, and if the target hasn't already
	///     been initialized, a new object will be instantiated.
	/// </param>
	/// <returns>The initialized value of type <typeparamref name="T" />.</returns>
	public static T? EnsureInitialized<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicParameterlessConstructor )] T>(
		ref T? target,
		ref Boolean initialized,
		[NotNullIfNotNull( "syncLock" )] ref Object? syncLock
	) {
		// Fast path.
		if ( Volatile.Read( ref initialized ) ) {
			return target;
		}

		return EnsureInitializedCore( ref target, ref initialized, ref syncLock );
	}

	/// <summary>
	///     Initializes a target reference or value type with a specified function if it has not already been
	///     initialized.
	/// </summary>
	/// <typeparam name="T">The type of the reference to be initialized.</typeparam>
	/// <param name="target">
	///     A reference or value of type <typeparamref name="T" /> to initialize if it
	///     has not already been initialized.
	/// </param>
	/// <param name="initialized">
	///     A reference to a boolean that determines whether the target has already
	///     been initialized.
	/// </param>
	/// <param name="syncLock">
	///     A reference to an object used as the mutually exclusive lock for initializing
	///     <paramref name="target" />. If <paramref name="syncLock" /> is null, and if the target hasn't already
	///     been initialized, a new object will be instantiated.
	/// </param>
	/// <param name="valueFactory">
	///     The <see cref="System.Func{T}" /> invoked to initialize the
	///     reference or value.
	/// </param>
	/// <returns>The initialized value of type <typeparamref name="T" />.</returns>
	public static T? EnsureInitialized<T>( [AllowNull] ref T target, ref Boolean initialized, [NotNullIfNotNull( "syncLock" )] ref Object? syncLock, Func<T> valueFactory ) {
		// Fast path.
		if ( Volatile.Read( ref initialized ) ) {
			return target;
		}

		return EnsureInitializedCore( ref target, ref initialized, ref syncLock, valueFactory );
	}

	/// <summary>
	///     Initializes a target reference type with a specified function if it has not already been initialized.
	/// </summary>
	/// <typeparam name="T">The type of the reference to be initialized. Has to be reference type.</typeparam>
	/// <param name="target">
	///     A reference of type <typeparamref name="T" /> to initialize if it has not already been
	///     initialized.
	/// </param>
	/// <param name="syncLock">
	///     A reference to an object used as the mutually exclusive lock for initializing
	///     <paramref name="target" />. If <paramref name="syncLock" /> is null, and if the target hasn't already
	///     been initialized, a new object will be instantiated.
	/// </param>
	/// <param name="valueFactory">The <see cref="System.Func{T}" /> invoked to initialize the reference.</param>
	/// <returns>The initialized value of type <typeparamref name="T" />.</returns>
	public static T EnsureInitialized<T>( [NotNull] ref T? target, [NotNullIfNotNull( "syncLock" )] ref Object? syncLock, Func<T> valueFactory ) where T : class {
		var read = Volatile.Read( ref target );
		if ( read is null ) {
			return EnsureInitializedCore( ref target, ref syncLock, valueFactory );
		}

		return read;
	}

}