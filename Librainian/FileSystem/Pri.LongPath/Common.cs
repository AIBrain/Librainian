// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Common.cs" last formatted on 2020-08-14 at 8:39 PM.

#nullable enable
namespace Librainian.FileSystem.Pri.LongPath {

	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;
	using System.Security;
	using System.Text;
	using JetBrains.Annotations;

	public static class Common {

		private const UInt32 ProtectedDiscretionaryAcl = 0x80000000;

		private const UInt32 ProtectedSystemAcl = 0x40000000;

		private const UInt32 UnprotectedDiscretionaryAcl = 0x20000000;

		private const UInt32 UnprotectedSystemAcl = 0x10000000;

		public const Int32 DefaultBufferSize = 16384;

		public const Int32 INVALID = -1;

		public const Int32 SUCCESS = 0;

		[NotNull]
		private static String GetMessageFromErrorCode( PriNativeMethods.ERROR errorCode ) {
			var buffer = new StringBuilder( 1024 );

			var _ = PriNativeMethods.FormatMessage( PriNativeMethods.FORMAT_MESSAGE_IGNORE_INSERTS | PriNativeMethods.FORMAT_MESSAGE_FROM_SYSTEM | PriNativeMethods.FORMAT_MESSAGE_ARGUMENT_ARRAY,
										 IntPtr.Zero, ( Int32 )errorCode, 0, buffer, buffer.Capacity, IntPtr.Zero );

			return buffer.ToString();
		}

		public static Boolean EndsWith( [CanBeNull] this String? text, Char value ) => !String.IsNullOrEmpty( text ) && text[^1] == value;

		public static Boolean Exists( [NotNull] this String path, out Boolean isDirectory ) {
			path = path.ThrowIfBlank();

			if ( path.TryNormalizeLongPath( out var normalizedPath ) || path.IsPathUnc() ) {
				if ( !String.IsNullOrWhiteSpace( normalizedPath ) ) {
					var errorCode = TryGetFileAttributes( normalizedPath, out var attributes );

					if ( errorCode == 0 && ( Int32 )attributes != PriNativeMethods.INVALID_FILE_ATTRIBUTES ) {
						isDirectory = attributes.IsDirectory();

						return true;
					}
				}
			}

			isDirectory = false;

			return false;
		}

		public static FileAttributes GetAttributes( [NotNull] this String path ) {
			var normalizedPath = path.NormalizeLongPath();

			var errorCode = normalizedPath.TryGetDirectoryAttributes( out var fileAttributes );

			if ( errorCode != ( Int32 )PriNativeMethods.ERROR.ERROR_SUCCESS ) {
				throw GetExceptionFromWin32Error( errorCode );
			}

			return fileAttributes;
		}

		public static FileAttributes GetAttributes( [NotNull] this String path, out PriNativeMethods.ERROR errorCode ) {
			var normalizedPath = path.NormalizeLongPath();

			errorCode = normalizedPath.TryGetDirectoryAttributes( out var fileAttributes );

			return fileAttributes;
		}

		[NotNull]
		public static Exception GetExceptionFromLastWin32Error() => GetExceptionFromLastWin32Error( "path" );

		[NotNull]
		public static Exception GetExceptionFromLastWin32Error( [NotNull] String parameterName ) => GetExceptionFromWin32Error( ( PriNativeMethods.ERROR )Marshal.GetLastWin32Error(), parameterName );

		[NotNull]
		public static Exception GetExceptionFromWin32Error( PriNativeMethods.ERROR errorCode ) => GetExceptionFromWin32Error( errorCode, "path" );

		[NotNull]
		public static Exception GetExceptionFromWin32Error( PriNativeMethods.ERROR errorCode, [NotNull] String parameterName ) {
			var message = GetMessageFromErrorCode( errorCode );

			return errorCode switch {
				PriNativeMethods.ERROR.ERROR_FILE_NOT_FOUND => new FileNotFoundException( message ),
				PriNativeMethods.ERROR.ERROR_PATH_NOT_FOUND => new DirectoryNotFoundException( message ),
				PriNativeMethods.ERROR.ERROR_ACCESS_DENIED => new UnauthorizedAccessException( message ),
				PriNativeMethods.ERROR.ERROR_FILENAME_EXCED_RANGE => new PathTooLongException( message ),
				PriNativeMethods.ERROR.ERROR_INVALID_DRIVE => new DriveNotFoundException( message ),
				PriNativeMethods.ERROR.ERROR_OPERATION_ABORTED => new OperationCanceledException( message ),
				PriNativeMethods.ERROR.ERROR_INVALID_NAME => new ArgumentException( message, parameterName ),
				_ => new IOException( message, PriNativeMethods.MakeHRFromErrorCode( errorCode ) )
			};
		}

		public static FileAttributes GetFileAttributes( [NotNull] this String path ) {
			var normalizedPath = path.NormalizeLongPath();

			var errorCode = TryGetFileAttributes( normalizedPath, out var fileAttributes );

			if ( errorCode != ( Int32 )PriNativeMethods.ERROR.ERROR_SUCCESS ) {
				throw GetExceptionFromWin32Error( errorCode );
			}

			return fileAttributes;
		}

		public static Boolean IsPathDots( [NotNull] this String path ) {
			path = path.ThrowIfBlank();

			return path is "." or "..";
		}

		public static Boolean IsPathUnc( [NotNull] this String path ) {
			path = path.ThrowIfBlank();

			if ( path.StartsWith( Path.UNCLongPathPrefix, StringComparison.Ordinal ) ) {
				return true;
			}

			return Uri.TryCreate( path, UriKind.Absolute, out var uri ) && uri.IsUnc;
		}

		/// <summary>Capture the <see cref="Uri" /> from <paramref name="path" /></summary>
		/// <param name="path"></param>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static Boolean IsPathUnc( [NotNull] this String path, [CanBeNull] out Uri? uri ) {
			path = path.ThrowIfBlank();

			if ( path.StartsWith( Path.UNCLongPathPrefix, StringComparison.Ordinal ) ) {
				uri = null;

				return true;
			}

			return Uri.TryCreate( path, UriKind.Absolute, out uri ) && uri.IsUnc;
		}

		[NotNull]
		public static String NormalizeSearchPattern( [NotNull] this String searchPattern ) =>
			String.IsNullOrEmpty( searchPattern ) || searchPattern == "." ? "*" : searchPattern;

		public static void SetAttributes( [NotNull] this String path, FileAttributes fileAttributes ) {
			var normalizedPath = path.NormalizeLongPath();

			if ( !PriNativeMethods.SetFileAttributes( normalizedPath, fileAttributes ) ) {
				throw GetExceptionFromLastWin32Error();
			}
		}

		/*
		public static Int32 SetSecurityInfo(
			ResourceType type,
			[NotNull] String name,
			SecurityInfos securityInformation,
			[CanBeNull] SecurityIdentifier owner,
			[CanBeNull] SecurityIdentifier group,
			[CanBeNull] GenericAcl sacl,
			[CanBeNull] GenericAcl dacl
		) {
			name = name.ThrowIfBlank();

			if ( !Enum.IsDefined( typeof( ResourceType ), type ) ) {
				throw new InvalidEnumArgumentException( nameof( type ), ( Int32 )type, typeof( ResourceType ) );
			}

			if ( !Enum.IsDefined( typeof( SecurityInfos ), securityInformation ) ) {
				throw new InvalidEnumArgumentException( nameof( securityInformation ), ( Int32 )securityInformation, typeof( SecurityInfos ) );
			}

			Int32 errorCode;
			Int32 Length;
			Byte[] OwnerBinary = null, GroupBinary = null, SaclBinary = null, DaclBinary = null;
			Privilege securityPrivilege = null;

			// Demand unmanaged code permission
			// The integrator layer is free to assert this permission and, in turn, demand another permission of its caller

			//new SecurityPermission( SecurityPermissionFlag.UnmanagedCode ).Demand();

			if ( owner != null ) {
				Length = owner.BinaryLength;
				OwnerBinary = new Byte[Length];
				owner.GetBinaryForm( OwnerBinary, 0 );
			}

			if ( group != null ) {
				Length = group.BinaryLength;
				GroupBinary = new Byte[Length];
				group.GetBinaryForm( GroupBinary, 0 );
			}

			if ( dacl != null ) {
				Length = dacl.BinaryLength;
				DaclBinary = new Byte[Length];
				dacl.GetBinaryForm( DaclBinary, 0 );
			}

			if ( sacl != null ) {
				Length = sacl.BinaryLength;
				SaclBinary = new Byte[Length];
				sacl.GetBinaryForm( SaclBinary, 0 );
			}

			if ( ( securityInformation & SecurityInfos.SystemAcl ) != 0 ) {
				// Enable security privilege if trying to set a SACL.
				// Note: even setting it by handle needs this privilege enabled!

				securityPrivilege = new Privilege( Privilege.Security );
			}

			// Ensure that the finally block will execute
			RuntimeHelpers.PrepareConstrainedRegions();

			try {
				if ( securityPrivilege != null ) {
					try {
						securityPrivilege.Enable();
					}
					catch ( PrivilegeNotHeldException ) {
						// we will ignore this exception and press on just in case this is a remote resource
					}
				}

				errorCode = ( Int32 )PriNativeMethods.SetSecurityInfoByName( name, ( UInt32 )type, ( UInt32 )securityInformation, OwnerBinary, GroupBinary, DaclBinary,
																		  SaclBinary );

				switch ( errorCode ) {
					case PriNativeMethods.ERROR_NOT_ALL_ASSIGNED:
					case PriNativeMethods.ERROR_PRIVILEGE_NOT_HELD:
						throw new PrivilegeNotHeldException( Privilege.Security );

					case PriNativeMethods.ERROR_ACCESS_DENIED:
					case PriNativeMethods.ERROR_CANT_OPEN_ANONYMOUS:
						throw new UnauthorizedAccessException();
				}

				if ( errorCode != PriNativeMethods.ERROR_SUCCESS ) {
					goto Error;
				}
			}
			catch {
				// protection against exception filter-based luring attacks
				securityPrivilege?.Revert();

				throw;
			}
			finally {
				securityPrivilege?.Revert();
			}

			return 0;

			Error:

			if ( errorCode == PriNativeMethods.ERROR_NOT_ENOUGH_MEMORY ) {
				throw new OutOfMemoryException();
			}

			return errorCode;
		}
		*/

		/// <summary>Returns the trimmed string or throws <see cref="ArgumentNullException" /> if null, whitespace, or empty.</summary>
		/// <param name="path"></param>
		/// <exception cref="ArgumentNullException">Gets thrown if the <paramref name="path" /> is null, whitespace, or empty.</exception>
		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "path" )]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[NotNull]
		[Pure]
		[DebuggerStepThrough]
		public static String ThrowIfBlank( [CanBeNull] this String? path ) {
			if ( String.IsNullOrEmpty( path = path?.Trim() ) ) {
				throw new ArgumentNullException( nameof( path ), "Value cannot be null or whitespace." );
			}

			return path;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		public static void ThrowIfBlank( [CanBeNull] ref String? path ) {
			if ( String.IsNullOrEmpty( path?.Trim() ) ) {
				throw new ArgumentNullException( nameof( path ), "Value cannot be null or whitespace." );
			}

		}

		public static void ThrowIfError( PriNativeMethods.ERROR errorCode, IntPtr byteArray ) {
			if ( errorCode == PriNativeMethods.ERROR.ERROR_SUCCESS ) {
				if ( IntPtr.Zero.Equals( byteArray ) ) {
					//
					// This means that the object doesn't have a security descriptor. And thus we throw
					// a specific exception for the caller to catch and handle properly.
					//
					throw new InvalidOperationException( "Object does not have security descriptor," );
				}
			}
			else {
				switch ( errorCode ) {
					case PriNativeMethods.ERROR.ERROR_NOT_ALL_ASSIGNED:
					case PriNativeMethods.ERROR.ERROR_PRIVILEGE_NOT_HELD:
						throw new SecurityException( "PrivilegeNotHeldException.SeSecurityPrivilege" );

					case PriNativeMethods.ERROR.ERROR_ACCESS_DENIED:
					case PriNativeMethods.ERROR.ERROR_CANT_OPEN_ANONYMOUS:
					case PriNativeMethods.ERROR.ERROR_LOGON_FAILURE:
						throw new UnauthorizedAccessException();

					case PriNativeMethods.ERROR.ERROR_BAD_NETPATH:
						throw new IOException( PriNativeMethods.GetMessage( errorCode ), PriNativeMethods.MakeHRFromErrorCode( errorCode ) );
					case PriNativeMethods.ERROR.ERROR_NETNAME_DELETED:
						throw new IOException( PriNativeMethods.GetMessage( errorCode ), PriNativeMethods.MakeHRFromErrorCode( errorCode ) );

					case PriNativeMethods.ERROR.ERROR_NOT_ENOUGH_MEMORY:
						throw new OutOfMemoryException();

					default:
						throw new IOException( PriNativeMethods.GetMessage( errorCode ), PriNativeMethods.MakeHRFromErrorCode( errorCode ) );
				}
			}
		}

		public static void ThrowIOError( PriNativeMethods.ERROR errorCode, [NotNull] String maybeFullPath ) {
			if ( String.IsNullOrWhiteSpace( maybeFullPath ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( maybeFullPath ) );
			}

			// This doesn't have to be perfect, but is a perf optimization.
			var isInvalidPath = errorCode is PriNativeMethods.ERROR.ERROR_INVALID_NAME or PriNativeMethods.ERROR.ERROR_BAD_PATHNAME;
			var str = isInvalidPath ? maybeFullPath.GetFileName() : maybeFullPath;

			switch ( errorCode ) {
				case PriNativeMethods.ERROR.ERROR_FILE_NOT_FOUND:

					if ( str.Length == 0 ) {
						throw new FileNotFoundException( "Empty filename" );
					}
					else {
						throw new FileNotFoundException( $"File {str} not found", str );
					}

				case PriNativeMethods.ERROR.ERROR_PATH_NOT_FOUND:

					if ( str.Length == 0 ) {
						throw new DirectoryNotFoundException( "Empty directory" );
					}
					else {
						throw new DirectoryNotFoundException( $"Directory {str} not found" );
					}

				case PriNativeMethods.ERROR.ERROR_ACCESS_DENIED:

					if ( str.Length == 0 ) {
						throw new UnauthorizedAccessException( "Empty path" );
					}
					else {
						throw new UnauthorizedAccessException( $"Access denied accessing {str}" );
					}

				case PriNativeMethods.ERROR.ERROR_ALREADY_EXISTS:

					if ( str.Length == 0 ) {
						goto default;
					}

					throw new IOException( $"File {str}", PriNativeMethods.MakeHRFromErrorCode( errorCode ) );

				case PriNativeMethods.ERROR.ERROR_FILENAME_EXCED_RANGE: throw new PathTooLongException( "Path too long" );

				case PriNativeMethods.ERROR.ERROR_INVALID_DRIVE: throw new DriveNotFoundException( $"Drive {str} not found" );

				case PriNativeMethods.ERROR.ERROR_INVALID_PARAMETER: throw new IOException( PriNativeMethods.GetMessage( errorCode ), PriNativeMethods.MakeHRFromErrorCode( errorCode ) );

				case PriNativeMethods.ERROR.ERROR_SHARING_VIOLATION:

					if ( str.Length == 0 ) {
						throw new IOException( "Sharing violation with empty filename", PriNativeMethods.MakeHRFromErrorCode( errorCode ) );
					}
					else {
						throw new IOException( $"Sharing violation: {str}", PriNativeMethods.MakeHRFromErrorCode( errorCode ) );
					}

				case PriNativeMethods.ERROR.ERROR_FILE_EXISTS:

					if ( str.Length == 0 ) {
						goto default;
					}

					throw new IOException( $"File exists {str}", PriNativeMethods.MakeHRFromErrorCode( errorCode ) );

				case PriNativeMethods.ERROR.ERROR_OPERATION_ABORTED: throw new OperationCanceledException();

				default: throw new IOException( PriNativeMethods.GetMessage( errorCode ), PriNativeMethods.MakeHRFromErrorCode( errorCode ) );
			}
		}

		/*
		public static SecurityInfos ToSecurityInfos( this AccessControlSections accessControlSections ) {
			SecurityInfos securityInfos = 0;

			if ( ( accessControlSections & AccessControlSections.Owner ) != 0 ) {
				securityInfos |= SecurityInfos.Owner;
			}

			if ( ( accessControlSections & AccessControlSections.Group ) != 0 ) {
				securityInfos |= SecurityInfos.Group;
			}

			if ( ( accessControlSections & AccessControlSections.Access ) != 0 ) {
				securityInfos |= SecurityInfos.DiscretionaryAcl;
			}

			if ( ( accessControlSections & AccessControlSections.Audit ) != 0 ) {
				securityInfos |= SecurityInfos.SystemAcl;
			}

			return securityInfos;
		}
		*/

		public static PriNativeMethods.ERROR TryGetDirectoryAttributes( [NotNull] this String normalizedPath, out FileAttributes attributes ) =>
			TryGetFileAttributes( normalizedPath.ThrowIfBlank(), out attributes );

		public static PriNativeMethods.ERROR TryGetFileAttributes( [NotNull] String normalizedPath, out FileAttributes attributes ) {
			var data = new WIN32_FILE_ATTRIBUTE_DATA();

			var errorMode = PriNativeMethods.SetErrorMode( 1 );

			try {
				if ( PriNativeMethods.GetFileAttributesEx( normalizedPath.ThrowIfBlank(), 0, ref data ) ) {
					attributes = data.fileAttributes;

					return SUCCESS;
				}
			}
			finally {
				PriNativeMethods.SetErrorMode( errorMode );
			}

			attributes = ( FileAttributes )PriNativeMethods.INVALID_FILE_ATTRIBUTES;

			return ( PriNativeMethods.ERROR )Marshal.GetLastWin32Error();
		}

	}

}