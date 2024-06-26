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
// File "Unique.cs" last formatted on 2021-11-30 at 7:17 PM by Protiguous.

namespace Librainian.FileSystem;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using Internet;
using Logging;
using Newtonsoft.Json;
using Parsing;

/// <summary>
/// <para>A custom class for the location of a file, directory, network location, or internet address/location.</para>
/// <para>The idea centers around a <see cref="Uri" />, which points to a single location.</para>
/// <para>A string is stored instead of the Uri itself, a tradeoff of memory vs computational time.</para>
/// <para>Locations should be case-sensitive ( <see cref="Equals(Object)" />).</para>
/// <para>It's... <see cref="Unique" />!</para>
/// <see cref="Location" />
/// </summary>
[Serializable]
public class Unique : IEquatable<Unique> {

	private const Int32 EOFMarker = -1;

	[JsonProperty]
	private readonly Uri? u;

	/// <summary>A <see cref="Unique" /> that points to nowhere.</summary>
	public static readonly Unique Empty = new();

	/// <summary>What effect will this have down the road?</summary>
	private Unique() => Uri.TryCreate( String.Empty, UriKind.RelativeOrAbsolute, out this.u );

	/// <param name="location"></param>
	/// <exception cref="NullException">When <paramref name="location" /> was parsed down to nothing.</exception>
	/// <exception cref="UriFormatException">When <paramref name="location" /> could not be parsed.</exception>
	protected Unique( TrimmedString location ) {
		if ( location.IsEmpty() ) {
			throw new NullException( "Location cannot be null or whitespace." );
		}

		if ( Uri.TryCreate( location, UriKind.Absolute, out var uri ) ) {
			this.u = uri;
		}
		else {
			throw new UriFormatException( $"Unable to parse the String `{location}` into a Uri" );
		}
	}

	//TODO What needs to happen if a uri cannot be parsed? throw exception? Maybe.

	/// <summary>Just an easier to use mnemonic.</summary>
	[JsonIgnore]
	public String? AbsolutePath => this.U?.AbsolutePath;

	/// <summary>
	/// The location/directory/path/file/name/whatever.ext
	/// <para>Has been filtered through Uri.AbsoluteUri already.</para>
	/// </summary>
	[JsonIgnore]
	public Uri? U => this.u;

	/// <summary>Static (Ordinal) comparison.</summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Boolean Equals( Unique? left, Unique? right ) {
		if ( ReferenceEquals( left, right ) ) {
			return true;
		}

		if ( left is null || right is null ) {
			return false;
		}

		return String.Equals( left.AbsolutePath, right.AbsolutePath, StringComparison.Ordinal );
	}

	public static Boolean operator !=( Unique? left, Unique? right ) => !Equals( left, right );

	public static Boolean operator ==( Unique? left, Unique? right ) => Equals( left, right );

	public static Boolean TryCreate( TrimmedString location, out Unique unique ) {
		if ( !location.IsEmpty() ) {
			try {
				unique = new Unique( location );

				return true;
			}
			catch ( NullException ) { }
			catch ( UriFormatException ) { }
		}

		unique = Empty;

		return false;
	}

	/// <summary>If the <paramref name="uri" /> is parsed, then <paramref name="unique" /> will never be null.</summary>
	/// <param name="uri"></param>
	/// <param name="unique"></param>
	public static Boolean TryCreate( Uri? uri, out Unique unique ) {
		if ( uri is null ) {
			unique = Empty;

			return false;
		}

		if ( uri.IsAbsoluteUri ) {
			unique = new Unique( uri.AbsoluteUri );

			return true;
		}

		unique = Empty;

		return false;
	}

	/// <summary>Enumerates the <see cref="Document" /> as a sequence of <see cref="Byte" />.</summary>
	public IEnumerable<Byte> AsBytes( TimeSpan timeout, CancellationToken cancellationToken ) {
		using var client = new WebClient().SetTimeoutAndCancel( timeout, cancellationToken );

		using var stream = client.OpenRead( this.U );

		while ( stream.CanRead ) {
			var a = stream.ReadByte();

			if ( a == EOFMarker ) {
				yield break;
			}

			yield return ( Byte )a;
		}
	}

	/// <summary>Enumerates the <see cref="Document" /> as a sequence of <see cref="Int16" />.</summary>
	public IEnumerable<Int32> AsInt16( TimeSpan timeout, CancellationToken cancellationToken ) {
		using var client = new WebClient().SetTimeoutAndCancel( timeout, cancellationToken );

		using var stream = client.OpenRead( this.U );

		while ( stream.CanRead ) {
			var a = stream.ReadByte();

			if ( a == EOFMarker ) {
				yield break;
			}

			var b = stream.ReadByte();

			if ( b == EOFMarker ) {
				yield return BitConverter.ToInt16( new[] {
					( Byte ) a
				}, 0 );

				yield break;
			}

			yield return BitConverter.ToInt16( new[] {
				( Byte ) a, ( Byte ) b
			}, 0 );
		}
	}

	/// <summary>Enumerates the <see cref="Document" /> as a sequence of <see cref="Int32" />.</summary>
	public IEnumerable<Int32> AsInt32( TimeSpan timeout, CancellationToken cancellationToken ) {
		using var client = new WebClient().SetTimeoutAndCancel( timeout, cancellationToken );

		using var stream = client.OpenRead( this.U );

		if ( !stream.CanRead ) {
			yield break;
		}

		while ( true ) {
			var a = stream.ReadByte();

			if ( a == EOFMarker ) {
				yield break;
			}

			var b = stream.ReadByte();

			if ( b == EOFMarker ) {
				yield return BitConverter.ToInt32( new[] {
					( Byte ) a
				}, 0 );

				yield break;
			}

			var c = stream.ReadByte();

			if ( c == EOFMarker ) {
				yield return BitConverter.ToInt32( new[] {
					( Byte ) a, ( Byte ) b
				}, 0 );

				yield break;
			}

			var d = stream.ReadByte();

			if ( d == EOFMarker ) {
				yield return BitConverter.ToInt32( new[] {
					( Byte ) a, ( Byte ) b, ( Byte ) c
				}, 0 );

				yield break;
			}

			yield return BitConverter.ToInt32( new[] {
				( Byte ) a, ( Byte ) b, ( Byte ) c, ( Byte ) d
			}, 0 );
		}
	}

	public Boolean Equals( Unique? other ) => Equals( this, other );

	public override Boolean Equals( Object? obj ) => Equals( this, obj as Unique );

	public override Int32 GetHashCode() => this.U.GetHashCode();

	/// <summary>Legacy name for a windows folder.</summary>
	public Boolean IsDirectory() => this.ToDirectoryInfo()?.Attributes.HasFlag( FileAttributes.Directory ) ?? false;

	public Boolean IsFile() => !this.ToFileInfo()?.Attributes.HasFlag( FileAttributes.Directory ) ?? false;

	/// <summary>Is this a windows folder (directory)?</summary>
	public Boolean IsFolder() => this.IsDirectory();

	/// <summary>
	/// <para>Gets the size in bytes of the location.</para>
	/// <para>A value of -1 indicates an error, timeout, or exception.</para>
	/// </summary>
	/// <param name="timeout"></param>
	/// <param name="cancellationToken"></param>
	public async Task<Int64> Length( TimeSpan timeout, CancellationToken cancellationToken ) {
		try {
			using var client = new WebClient().SetTimeoutAndCancel( timeout, cancellationToken );

			try {
				await client.OpenReadTaskAsync( this.U ).ConfigureAwait( false );

				var header = client.ResponseHeaders[ "Content-Length" ];

				if ( Int64.TryParse( header, out var result ) ) {
					return result;
				}
			}
			catch ( WebException exception ) {
				exception.Log();
			}
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		return -1;
	}

	public DirectoryInfo? ToDirectoryInfo() {
		try {
			if ( this.U.IsFile ) {
				return new DirectoryInfo( this.AbsolutePath );
			}
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		return default( DirectoryInfo? );
	}

	public FileInfo? ToFileInfo() {
		try {
			if ( this.U.IsFile ) {
				return new FileInfo( this.AbsolutePath );
			}
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		return default( FileInfo? );
	}

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override String ToString() => $"{this.AbsolutePath}";
}