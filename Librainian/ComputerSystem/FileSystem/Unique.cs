// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "Unique.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "Unique.cs" was last formatted by Protiguous on 2018/06/26 at 12:56 AM.

namespace Librainian.ComputerSystem.FileSystem {

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;
	using Exceptions;
	using Internet;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Parsing;
	using Threading;

	/// <summary>
	///     <para>A custom class for the location of a file, directory, network location, or internet address/location.</para>
	///     <para>The idea centers around a <see cref="Uri" />, which points to a single location.</para>
	///     <para>A string is stored instead of the Uri itself, a tradeoff of memory vs computational time.</para>
	///     <para>It's...<see cref="Unique" />!</para>
	/// </summary>
	[Serializable]
	public class Unique : IEquatable<Unique> {

		private const Int32 EOFMarker = -1;

		/// <summary>
		///     A <see cref="Unique" /> that points to nowhere.
		/// </summary>
		public static readonly Unique Empty = new Unique();

		/// <summary>
		///     Just an easier to use mnemonic.
		/// </summary>
		[JsonIgnore]
		public TrimmedString Location => this.U;

		/// <summary>
		///     The location/directory/path/file/name/whatever.ext
		///     <para>Has been filtered through Uri.AbsoluteUri already.</para>
		/// </summary>
		[NotNull]
		[JsonProperty]
		public String U { get; }

		/// <summary>
		///     What effect will this have down the road?
		/// </summary>
		private Unique() => this.U = String.Empty;

		/// <summary>
		/// </summary>
		/// <param name="location"></param>
		/// <exception cref="ArgumentEmptyException">When <paramref name="location" /> was parsed down to nothing.</exception>
		/// <exception cref="UriFormatException">When <paramref name="location" /> could not be parsed.</exception>
		protected Unique( TrimmedString location ) {
			if ( location.IsEmpty ) {
				throw new ArgumentEmptyException( "Location cannot be null or whitespace." );
			}

			if ( Uri.TryCreate( location, UriKind.Absolute, out var uri ) ) {
				this.U = uri.AbsoluteUri;
			}
			else {
				throw new UriFormatException( $"Unable to parse the String `{location}` into a Uri" );
			}
		}

		/// <summary>
		///     Static comparison.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Unique left, Unique right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return false;
			}

			return String.Equals( left.U, right.U, StringComparison.Ordinal );
		}

		public static Boolean operator !=( [CanBeNull] Unique unique1, [CanBeNull] Unique unique2 ) => !Equals( unique1, unique2 );

		public static Boolean operator ==( [CanBeNull] Unique unique1, [CanBeNull] Unique unique2 ) => Equals( unique1, unique2 );

		public static Boolean TryCreate( TrimmedString location, [NotNull] out Unique unique ) {
			if ( !location.IsEmpty ) {
				try {
					unique = new Unique( location: location );

					return true;
				}
				catch ( ArgumentNullException ) { }
				catch ( UriFormatException ) { }
			}

			unique = Empty;

			return false;
		}

		/// <summary>
		///     If the <paramref name="uri" /> is parsed, then <paramref name="unique" /> will never be null.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="unique"></param>
		/// <returns></returns>
		public static Boolean TryCreate( [CanBeNull] Uri uri, [NotNull] out Unique unique ) {
			if ( uri is null ) {

				unique = Empty;

				return false;
			}

			if ( uri.IsAbsoluteUri ) {
				unique = new Unique( location: uri.AbsoluteUri );

				return true;
			}

			unique = Empty;

			return false;
		}

		/// <summary>
		///     Enumerates the <see cref="Document" /> as a sequence of <see cref="Byte" />.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Byte> AsBytes( CancellationToken? token = null, TimeSpan? timeout = null ) {
			using ( var client = new WebClient().Add( timeout, token ) ) {
				using ( var stream = client.OpenRead( this.ToUri() ) ) {
					if ( stream?.CanRead != true ) {
						yield break;
					}

					while ( true ) {
						var a = stream.ReadByte();

						if ( a == EOFMarker ) {
							yield break;
						}

						yield return ( Byte ) a;
					}
				}
			}
		}

		/// <summary>
		///     Enumerates the <see cref="Document" /> as a sequence of <see cref="Int16" />.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Int32> AsInt16( CancellationToken? token, TimeSpan? timeout ) {

			using ( var client = new WebClient().Add( timeout, token ) ) {
				using ( var stream = client.OpenRead( this.ToUri() ) ) {
					if ( stream?.CanRead != true ) {
						yield break;
					}

					while ( true ) {
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
			}
		}

		/// <summary>
		///     Enumerates the <see cref="Document" /> as a sequence of <see cref="Int32" />.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Int32> AsInt32( CancellationToken? token, TimeSpan? timeout ) {

			using ( var client = new WebClient().Add( timeout, token ) ) {
				using ( var stream = client.OpenRead( this.ToUri() ) ) {
					if ( stream?.CanRead != true ) {
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
			}
		}

		public override Boolean Equals( Object obj ) => Equals( this, obj as Unique );

		public Boolean Equals( Unique other ) => Equals( this, other );

		public override Int32 GetHashCode() => this.U.GetHashCode();

		/// <summary>
		///     Legacy name for a windows folder.
		/// </summary>
		public Boolean IsDirectory() => this.ToDirectoryInfo()?.Attributes.HasFlag( FileAttributes.Directory ) ?? false;

		public Boolean IsFile() => !this.ToFileInfo()?.Attributes.HasFlag( FileAttributes.Directory ) ?? false;

		/// <summary>
		///     Is this a windows folder (directory)?
		/// </summary>
		/// <returns></returns>
		public Boolean IsFolder() => this.IsDirectory();

		/// <summary>
		///     <para>Gets the size in bytes of the location.</para>
		///     <para>A value of -1 indicates an error, timeout, or exception.</para>
		/// </summary>
		/// <param name="token"></param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public async Task<Int64> Length( CancellationToken? token = null, TimeSpan? timeout = null ) {
			try {
				using ( var client = new WebClient().Add( timeout, token ) ) {
					try {
						await client.OpenReadTaskAsync( this.ToUri() ).NoUI();

						var header = client.ResponseHeaders[ "Content-Length" ];

						if ( Int64.TryParse( header, out var result ) ) {
							return result;
						}
					}
					catch ( WebException exception ) {
						exception.More();
					}
				}
			}
			catch ( Exception exception ) {
				exception.More();
			}

			return -1;
		}

		[CanBeNull]
		public DirectoryInfo ToDirectoryInfo() {
			try {
				if ( this.ToUri().AbsoluteUri.StartsWith( Protocols.File, StringComparison.Ordinal ) ) {
					return new DirectoryInfo( this.ToUri().AbsolutePath );
				}
			}
			catch ( Exception exception ) {
				exception.More();
			}

			return null;
		}

		[CanBeNull]
		public FileInfo ToFileInfo() {
			try {

				if ( this.ToUri().AbsoluteUri.StartsWith( Protocols.File, StringComparison.Ordinal ) ) {
					return new FileInfo( this.ToUri().AbsolutePath );
				}
			}
			catch ( Exception exception ) {
				exception.More();
			}

			return null;
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override String ToString() => $"{this.U}";

		/// <summary>
		///     <para>Returns the <see cref="Uri" />.</para>
		///     <para>
		///         The AbsoluteUri property includes the entire URI stored in the Uri instance, including all fragments and
		///         query strings.
		///     </para>
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public Uri ToUri() {
			Uri.TryCreate( this.U, UriKind.Absolute, out var result ); //this should NEVER fail, because only valid Uri have been parsed in the ctor.

			return result;
		}

		public static class Protocols {

			public const String File = "file:///";
		}
	}
}