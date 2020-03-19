// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Unique.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", File: "Unique.cs" was last formatted by Protiguous on 2020/03/18 at 10:26 AM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Exceptions;
    using Internet;
    using JetBrains.Annotations;
    using Logging;
    using Newtonsoft.Json;
    using Parsing;

    // ReSharper disable RedundantUsingDirective
    using DirectoryInfo = Pri.LongPath.DirectoryInfo;
    using FileInfo = Pri.LongPath.FileInfo;

    // ReSharper restore RedundantUsingDirective

    /// <summary>
    ///     <para>A custom class for the location of a file, directory, network location, or internet address/location.</para>
    ///     <para>The idea centers around a <see cref="Uri" />, which points to a single location.</para>
    ///     <para>A string is stored instead of the Uri itself, a tradeoff of memory vs computational time.</para>
    ///     <para>Locations should be case-sensitive (<see cref="Equals(Object)" />).</para>
    ///     <para>It's...<see cref="Unique" />!</para>
    /// </summary>
    [Serializable]
    public class Unique : IEquatable<Unique> {

        public Boolean Equals( Unique other ) => Equals( this, other );

        [NotNull]
        [JsonProperty]
        private readonly Uri u;

        /// <summary>The location/directory/path/file/name/whatever.ext
        /// <para>Has been filtered through Uri.AbsoluteUri already.</para>
        /// </summary>
        [NotNull]
        [JsonIgnore]
        public Uri U => this.u;

        /// <summary>Just an easier to use mnemonic.</summary>
        [NotNull]
        [JsonIgnore]
        public String AbsolutePath => this.U.AbsolutePath;

        private const Int32 EOFMarker = -1;

        /// <summary>A <see cref="Unique" /> that points to nowhere.</summary>
        [NotNull]
        public static readonly Unique Empty = new Unique();

        /// <summary>What effect will this have down the road?</summary>
        private Unique() => Uri.TryCreate( String.Empty, UriKind.RelativeOrAbsolute, out this.u );

        /// <summary></summary>
        /// <param name="location"></param>
        /// <exception cref="ArgumentEmptyException">When <paramref name="location" /> was parsed down to nothing.</exception>
        /// <exception cref="UriFormatException">When <paramref name="location" /> could not be parsed.</exception>
        protected Unique( TrimmedString location ) {
            if ( location.IsEmpty() ) {
                throw new ArgumentEmptyException( "Location cannot be null or whitespace." );
            }

            if ( Uri.TryCreate( location, UriKind.Absolute, out var uri ) ) {
                this.u = uri;
            }
            else {
                throw new UriFormatException( $"Unable to parse the String `{location}` into a Uri" );
            }
        }

        /// <summary>Static (Ordinal) comparison.</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Unique left, [CanBeNull] Unique right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return default;
            }

            return String.Equals( left.AbsolutePath, right.AbsolutePath, StringComparison.Ordinal );
        }

        public static Boolean operator !=( [CanBeNull] Unique left, [CanBeNull] Unique right ) => !Equals( left, right );

        public static Boolean operator ==( [CanBeNull] Unique left, [CanBeNull] Unique right ) => Equals( left, right );

        public static Boolean TryCreate( TrimmedString location, [NotNull] out Unique unique ) {
            if ( !location.IsEmpty() ) {
                try {
                    unique = new Unique( location );

                    return true;
                }
                catch ( ArgumentNullException ) { }
                catch ( UriFormatException ) { }
            }

            unique = Empty;

            return default;
        }

        /// <summary>If the <paramref name="uri" /> is parsed, then <paramref name="unique" /> will never be null.</summary>
        /// <param name="uri"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        public static Boolean TryCreate( [CanBeNull] Uri uri, [NotNull] out Unique unique ) {
            if ( uri is null ) {

                unique = Empty;

                return default;
            }

            if ( uri.IsAbsoluteUri ) {
                unique = new Unique( uri.AbsoluteUri );

                return true;
            }

            unique = Empty;

            return default;
        }

        /// <summary>Enumerates the <see cref="Document" /> as a sequence of <see cref="Byte" />.</summary>
        /// <returns></returns>
        public IEnumerable<Byte> AsBytes( TimeSpan timeout, CancellationToken token ) {
            using var client = new WebClient().SetTimeoutAndCancel( timeout, token );

            using var stream = client.OpenRead( this.U );

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

        /// <summary>Enumerates the <see cref="Document" /> as a sequence of <see cref="Int16" />.</summary>
        /// <returns></returns>
        public IEnumerable<Int32> AsInt16( TimeSpan timeout, CancellationToken token ) {

            using var client = new WebClient().SetTimeoutAndCancel( timeout, token );

            using var stream = client.OpenRead( this.U );

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

        /// <summary>Enumerates the <see cref="Document" /> as a sequence of <see cref="Int32" />.</summary>
        /// <returns></returns>
        public IEnumerable<Int32> AsInt32( TimeSpan timeout, CancellationToken token ) {

            using var client = new WebClient().SetTimeoutAndCancel( timeout, token );

            using var stream = client.OpenRead( this.U );

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

        public override Boolean Equals( Object obj ) => Equals( this, obj as Unique );

        public override Int32 GetHashCode() => this.U.GetHashCode();

        /// <summary>Legacy name for a windows folder.</summary>
        public Boolean IsDirectory() => this.ToDirectoryInfo()?.Attributes.HasFlag( FileAttributes.Directory ) ?? false;

        public Boolean IsFile() => !this.ToFileInfo()?.Attributes.HasFlag( FileAttributes.Directory ) ?? false;

        /// <summary>Is this a windows folder (directory)?</summary>
        /// <returns></returns>
        public Boolean IsFolder() => this.IsDirectory();

        /// <summary>
        ///     <para>Gets the size in bytes of the location.</para>
        ///     <para>A value of -1 indicates an error, timeout, or exception.</para>
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Int64> Length( TimeSpan timeout, CancellationToken token ) {
            try {
                try {
                    using var client = new WebClient().SetTimeoutAndCancel( timeout, token );
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

        [CanBeNull]
        public DirectoryInfo? ToDirectoryInfo() {
            try {
                if ( this.U.IsFile ) {
                    return new DirectoryInfo( this.AbsolutePath );
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return null;
        }

        [CanBeNull]
        public FileInfo? ToFileInfo() {
            try {
                if ( this.U.IsFile ) {
                    return new FileInfo( this.AbsolutePath );
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return null;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => $"{this.AbsolutePath}";

    }

}