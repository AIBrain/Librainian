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
// "Librainian 2015/Folder.cs" was last cleaned by Rick on 2015/01/25 at 10:14 AM
#endregion

namespace Librainian.IO {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Security;
	using System.Text;
	using System.Windows.Forms;
	using Extensions;
	using JetBrains.Annotations;
	using Parsing;

	/// <summary>
	/// </summary>
	// ReSharper disable once UseNameofExpression
	[DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [ DataContract( IsReference = true ) ]
    [ Immutable ]
    public class Folder {
        //TODO add in long name (unc) support. Like 'ZedLongPaths' does

		/// <summary>
		///     "/"
		/// </summary>
		[ NotNull ]
		public static string FolderAltSeparator { get; } = new String( new[] { Path.AltDirectorySeparatorChar } );

		/// <summary>
		///     "\"
		/// </summary>
		[ NotNull ]
		public static string FolderSeparator { get; } = new String( new[] { Path.DirectorySeparatorChar } );

		[ NotNull ]
		public DirectoryInfo DirectoryInfo { get; }

		/// <summary>
		///     <para>The <see cref="Folder" /> .</para>
		/// </summary>
		[ NotNull ]
		public string OriginalFullPath { get; }

		[ NotNull ] public readonly Uri Uri;

        /// <summary>
        /// </summary>
        /// <param name="fullPath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public Folder( [ NotNull ] String fullPath ) {
            if ( String.IsNullOrWhiteSpace( fullPath ) ) {
                throw new ArgumentNullException( nameof( fullPath ) );
            }

            this.OriginalFullPath = fullPath;

	        DirectoryInfo directoryInfo;
	        if ( !IOExtensions.TryGetFolderFromPath( fullPath, out directoryInfo, out this.Uri ) ) {
                throw new InvalidOperationException( String.Format( "Unable to parse a valid path from `{0}`", fullPath ) );
            }
	        if ( directoryInfo != null ) {
		        this.DirectoryInfo = directoryInfo;
	        }
        }

        public Folder( Environment.SpecialFolder specialFolder ) : this( Environment.GetFolderPath( specialFolder ) ) { }

        public Folder( Environment.SpecialFolder specialFolder, String subFolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), subFolder ) ) { }

        public Folder( Environment.SpecialFolder specialFolder, String applicationName, String subFolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder ) ) { }

        /// <summary>
        ///     <para>
        ///         Pass null to automatically fill in <paramref name="companyName" /> and
        ///         <paramref name="applicationName" /> .
        ///     </para>
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <param name="companyName"></param>
        /// <param name="applicationName"></param>
        /// <param name="subFolder"></param>
        public Folder( Environment.SpecialFolder specialFolder, String companyName, String applicationName, String subFolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), companyName ?? Application.CompanyName, applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder ) ) { }

        /// <summary>
        ///     <para>
        ///         Pass null to automatically fill in <paramref name="companyName" /> and
        ///         <paramref name="applicationName" /> .
        ///     </para>
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <param name="companyName"></param>
        /// <param name="applicationName"></param>
        /// <param name="subFolder"></param>
        /// <param name="subSubfolder"></param>
        public Folder( Environment.SpecialFolder specialFolder, String companyName, String applicationName, String subFolder, String subSubfolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), companyName ?? Application.CompanyName, applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder, subSubfolder ) ) { }

        /// <summary>
        ///     <para>
        ///         Pass null to automatically fill in <paramref name="companyName" /> and
        ///         <paramref name="applicationName" /> .
        ///     </para>
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <param name="companyName"></param>
        /// <param name="applicationName"></param>
        /// <param name="subFolder"></param>
        /// <param name="subSubfolder"></param>
        /// <param name="subSubSubfolder"></param>
        public Folder( Environment.SpecialFolder specialFolder, String companyName, String applicationName, String subFolder, String subSubfolder, String subSubSubfolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), companyName ?? Application.CompanyName, applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder, subSubfolder, subSubSubfolder ) ) { }

        public Folder( String fullPath, String subFolder ) : this( Path.Combine( fullPath, subFolder ) ) { }

        public Folder( Folder folder, String subFolder ) : this( Path.Combine( folder.FullName, subFolder ) ) { }

        public Folder( FileSystemInfo fileSystemInfo ) : this( fileSystemInfo.FullName ) { }

        [ NotNull ]
        public String FullName => this.DirectoryInfo.FullName;

        private String DebuggerDisplay => this.FullName;
        public Folder Parent => new Folder( this.DirectoryInfo.Parent );

        public static implicit operator DirectoryInfo( Folder folder ) => folder.DirectoryInfo;

        /// <summary>
        ///     <para>Static comparison of the file names (case insensitive) and file sizes for equality.</para>
        ///     <para>
        ///         To compare the contents of two <see cref="Document" /> use
        ///         <see cref="IOExtensions.SameContent(Document,Document)" /> .
        ///     </para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [ CanBeNull ] Folder left, [ CanBeNull ] Folder right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }
            if ( ReferenceEquals( left, null ) || ReferenceEquals( right, null ) ) {
                return false;
            }
            return left.FullName.Like( right.FullName );
        }

        /// <summary>
        ///     <para>Returns True if the folder exists.</para>
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Delete"></seealso>
        public Boolean Create() {
            try {
                if ( this.Exists() ) {
                    return true;
                }
                this.DirectoryInfo.Create();
                return this.Exists();
            }
            catch ( IOException ) {
                return false;
            }
        }

        /// <summary>
        ///     <para>Returns True if the folder no longers exists.</para>
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Create"></seealso>
        public Boolean Delete() {
            try {
                //safety checks
                if ( this.IsEmpty() ) {
                    this.DirectoryInfo.Delete();
                    return !this.Exists();
                }
            }
            catch ( IOException ) { }
            return false;
        }

        /// <summary>
        ///     Returns true if the <see cref="Folder" /> currently exists.
        /// </summary>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        public bool Exists() {
            this.Refresh();
            return this.DirectoryInfo.Exists;
        }

        /// <summary>
        ///     <para>Returns an enumerable collection of <see cref="Document" /> in the current directory.</para>
        /// </summary>
        /// <returns></returns>
        public IEnumerable< Document > GetDocuments() {
            if ( !this.DirectoryInfo.Exists ) {
                this.Refresh();
                if ( !this.DirectoryInfo.Exists ) {
                    return Enumerable.Empty< Document >();
                }
            }
            return this.DirectoryInfo.EnumerateFiles().Select( fileInfo => new Document( fileInfo.FullName ) );
        }

        public IEnumerable< Document > GetDocuments( String searchPattern ) => this.DirectoryInfo.EnumerateFiles( searchPattern ).Select( fileInfo => new Document( fileInfo.FullName ) );

        public IEnumerable< Document > GetDocuments( IEnumerable< String > searchPatterns ) => searchPatterns.SelectMany( this.GetDocuments );

        public IEnumerable< Document > GetDocuments( IEnumerable< String > searchPatterns, SearchOption searchOption ) => searchPatterns.SelectMany( searchPattern => this.GetDocuments( searchPattern, searchOption ) );

        public IEnumerable< Document > GetDocuments( String searchPattern, SearchOption searchOption ) => this.DirectoryInfo.EnumerateFiles( searchPattern, searchOption ).Select( fileInfo => new Document( fileInfo.FullName ) );

        public IEnumerable< Folder > GetFolders() {
            if ( !this.DirectoryInfo.Exists ) {
                this.Refresh();
                if ( !this.DirectoryInfo.Exists ) {
                    return Enumerable.Empty< Folder >();
                }
            }
            return this.DirectoryInfo.EnumerateDirectories().Select( fileInfo => new Folder( fileInfo.FullName ) );
        }

        public IEnumerable< Folder > GetFolders( String searchPattern ) {
            if ( String.IsNullOrEmpty( searchPattern ) ) {
                yield break;
            }
            foreach ( var fileInfo in this.DirectoryInfo.EnumerateDirectories( searchPattern ) ) {
                yield return new Folder( fileInfo.FullName );
            }
        }

        public IEnumerable< Folder > GetFolders( String searchPattern, SearchOption searchOption ) {
            if ( String.IsNullOrEmpty( searchPattern ) ) {
                yield break;
            }
            foreach ( var fileInfo in this.DirectoryInfo.EnumerateDirectories( searchPattern, searchOption ) ) {
                yield return new Folder( fileInfo.FullName );
            }
        }

        public override int GetHashCode() => this.FullName.GetHashCode();

        /// <summary>
        ///     <para>
        ///         Check if this <see cref="Folder" /> contains any <see cref="Folder" /> or
        ///         <see cref="Document" /> .
        ///     </para>
        /// </summary>
        /// <returns></returns>
        public Boolean IsEmpty() => !this.GetFolders( "*.*" ).Any() && !this.GetDocuments( "*.*" ).Any();

        public void Refresh() => this.DirectoryInfo.Refresh();

        /// <summary>
        ///     <para>Shorten the full path with "..."</para>
        /// </summary>
        /// <returns></returns>
        public String ToCompactFormat() {
            var sb = new StringBuilder();
            NativeWin32.PathCompactPathEx( sb, this.FullName, this.FullName.Length, 0 ); //TODO untested. //HACK may be buggy on extensions also
            return sb.ToString();
        }

        /// <summary>
        ///     Returns a String that represents the current object.
        /// </summary>
        /// <returns>A String that represents the current object.</returns>
        public override String ToString() => DebuggerDisplay;
    }
}
