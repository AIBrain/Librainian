// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "Folder.cs" belongs to Protiguous@Protiguous.com
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
// File "Folder.cs" was last formatted by Protiguous on 2018/06/26 at 12:55 AM.

namespace Librainian.ComputerSystems.FileSystem {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Security;
	using System.Security.Permissions;
	using System.Text;
	using System.Windows.Forms;
	using Extensions;
	using JetBrains.Annotations;
	using Maths;
	using Newtonsoft.Json;
	using OperatingSystem;
	using Parsing;

	/// <summary>
	///     //TODO add in long name (unc) support. Like 'ZedLongPaths' does
	/// </summary>
	[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
	[JsonObject]
	[Immutable]
	[Serializable]
	public class Folder : IEquatable<Folder> {

		/// <summary>
		///     "/"
		/// </summary>
		[JsonIgnore]
		[NotNull]
		public static String FolderAltSeparator { get; } = new String( new[] {
			Path.AltDirectorySeparatorChar
		} );

		/// <summary>
		///     "\"
		/// </summary>
		[JsonIgnore]
		[NotNull]
		public static String FolderSeparator { get; } = new String( new[] {
			Path.DirectorySeparatorChar
		} );

		[JsonIgnore]
		public static Char FolderSeparatorChar { get; } = Path.DirectorySeparatorChar;

		[JsonProperty]
		[NotNull]
		public DirectoryInfo Info { get; }

		[JsonIgnore]
		[NotNull]
		public String FullName => this.Info.FullName;

		[JsonIgnore]
		[NotNull]
		public String Name => this.Info.Name;

		/// <summary>
		/// </summary>
		/// <param name="fullPath"></param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( TrimmedString fullPath ) {
			if ( fullPath.IsEmpty ) {
				throw new ArgumentNullException( nameof( fullPath ) );
			}

			if ( !fullPath.TryGetFolderFromPath( out var directoryInfo, out _ ) ) {
				throw new InvalidOperationException( $"Unable to parse a valid path from `{fullPath}`" );
			}

			this.Info = directoryInfo ?? throw new InvalidOperationException( $"Unable to parse a valid path from `{fullPath}`" );
		}

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( Environment.SpecialFolder specialFolder ) : this( Environment.GetFolderPath( specialFolder ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( Environment.SpecialFolder specialFolder, [NotNull] String subFolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), subFolder ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( Environment.SpecialFolder specialFolder, [CanBeNull] String applicationName, [NotNull] String subFolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ),
			applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder ) ) { }

		/// <summary>
		///     <para>Pass null to automatically fill in <paramref name="companyName" /> and <paramref name="applicationName" /> .</para>
		/// </summary>
		/// <param name="specialFolder">  </param>
		/// <param name="companyName">    </param>
		/// <param name="applicationName"></param>
		/// <param name="subFolder">      </param>
		/// <param name="subSubfolder">   </param>
		/// <param name="subSubSubfolder"></param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( Environment.SpecialFolder specialFolder, [CanBeNull] String companyName, [CanBeNull] String applicationName, String subFolder, String subSubfolder, String subSubSubfolder ) : this(
			Path.Combine( Environment.GetFolderPath( specialFolder ), companyName ?? Application.CompanyName, applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder,
				subSubfolder, subSubSubfolder ) ) { }

		///// <summary>
		/////     <para>
		/////         Pass null to automatically fill in <paramref name="companyName" /> and
		/////         <paramref name="applicationName" /> .
		/////     </para>
		///// </summary>
		///// <param name="specialFolder"></param>
		///// <param name="companyName"></param>
		///// <param name="applicationName"></param>
		///// <param name="subFolder"></param>
		///// <param name="subSubfolder"></param>
		//public Folder( Environment.SpecialFolder specialFolder, String companyName, String applicationName, String subFolder, String subSubfolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), companyName ?? Application.CompanyName, applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder, subSubfolder ) ) {
		//}
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( [NotNull] String fullPath, [NotNull] String subFolder ) : this( Path.Combine( fullPath, subFolder ) ) { }

		///// <summary>
		/////     <para>
		/////         Pass null to automatically fill in <paramref name="companyName" /> and
		/////         <paramref name="applicationName" /> .
		/////     </para>
		///// </summary>
		///// <param name="specialFolder"></param>
		///// <param name="companyName"></param>
		///// <param name="applicationName"></param>
		///// <param name="subFolder"></param>
		//public Folder( Environment.SpecialFolder specialFolder, String companyName, String applicationName, String subFolder ) : this( Path.Combine( Environment.GetFolderPath( specialFolder ), companyName ?? Application.CompanyName, applicationName ?? Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName, subFolder ) ) {
		//}
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( [NotNull] Folder folder, [NotNull] String subFolder ) : this( Path.Combine( folder.FullName, subFolder ) ) { }

		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public Folder( [NotNull] FileSystemInfo fileSystemInfo ) : this( fileSystemInfo.FullName ) { }

		/// <summary>
		///     <para>Static comparison of the folder names (case sensitive) for equality.</para>
		///     <para>
		///         To compare the path of two <see cref="Folder" /> use
		///         <param name="left">todo: describe left parameter on Equals</param>
		///         <param name="right">
		///             todo: describe right parameter on
		///             Equals
		///         </param>
		///         <seealso /> .
		///     </para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] Folder left, [CanBeNull] Folder right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return false;
			}

			return left.FullName.Is( right.FullName );
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public static Folder GetTempFolder() {
			var tempFolder = new Folder( Path.GetTempPath() );

			if ( tempFolder.Exists() ) {
				return tempFolder;
			}

			if ( tempFolder.Create() ) {
				return tempFolder;
			}

			throw new DirectoryNotFoundException( $"Unable to create the user's temp folder `{tempFolder.FullName}`." );
		}

		[NotNull]
		public static implicit operator DirectoryInfo( [NotNull] Folder folder ) => folder.Info;

		/// <summary>
		///     Opens a folder in file explorer.
		/// </summary>
		public static void OpenWithExplorer( [CanBeNull] Folder folder ) {
			if ( folder != null ) {
				var windowsFolder = Environment.GetFolderPath( Environment.SpecialFolder.Windows );

				Process.Start( $@"{windowsFolder}\explorer.exe", $"/e,\"{folder.FullName}\"" );
			}
		}

		public static Boolean TryParse( [CanBeNull] String path, [CanBeNull] out Folder folder ) {
			folder = null;

			try {
				if ( String.IsNullOrWhiteSpace( path ) ) {
					return false;
				}

				path = path.Trim();

				if ( String.IsNullOrWhiteSpace( path ) ) {
					return false;
				}

				DirectoryInfo dirInfo;

				if ( Uri.TryCreate( path, UriKind.Absolute, out var uri ) ) {
					dirInfo = new DirectoryInfo( uri.LocalPath );
					folder = new Folder( dirInfo );

					return true;
				}

				dirInfo = new DirectoryInfo( path ); //try it anyways
				folder = new Folder( dirInfo );

				return true;
			}
			catch ( ArgumentException ) { }
			catch ( UriFormatException ) { }
			catch ( SecurityException ) { }
			catch ( PathTooLongException ) { }
			catch ( InvalidOperationException ) { }

			return false;
		}

		/// <summary>
		/// </summary>
		/// <param name="searchPattern"></param>
		/// <param name="randomize">    </param>
		/// <returns></returns>
		[ItemNotNull]
		public IEnumerable<Folder> BetterGetFolders( [CanBeNull] String searchPattern = "*", Boolean randomize = true ) {
			if ( String.IsNullOrEmpty( searchPattern ) ) {
				yield break;
			}

			if ( randomize ) {
				foreach ( var fileInfo in this.Info.BetterEnumerateDirectories( searchPattern ).OrderBy( info => Randem.Next() ) ) {
					yield return new Folder( fileInfo.FullName );
				}
			}
			else {
				foreach ( var fileInfo in this.Info.BetterEnumerateDirectories( searchPattern ) ) {
					yield return new Folder( fileInfo.FullName );
				}
			}
		}

		/// <summary>
		///     Returns a copy of the folder instance.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public Folder Clone() => new Folder( this );

		/// <summary>
		///     <para>Returns True if the folder exists.</para>
		/// </summary>
		/// <returns></returns>
		/// See also:
		/// <seealso cref="Delete"></seealso>
		public Boolean Create() {
			try {
				if ( this.Exists() ) {
					return true;
				}

				try {
					if ( this.Info.Parent?.Exists == false ) {
						new Folder( this.Info.Parent.FullName ).Create();
					}
				}
				catch ( Exception exception ) {
					exception.More();
				}

				this.Info.Create();

				return this.Exists();
			}
			catch ( IOException ) {
				return false;
			}
		}

		/// <summary>
		///     <para>Returns True if the folder no longer exists.</para>
		/// </summary>
		/// <returns></returns>
		/// <seealso cref="Create"></seealso>
		public Boolean Delete() {
			try {

				//safety checks
				if ( this.IsEmpty() ) {
					this.Info.Delete();

					return !this.Exists();
				}
			}
			catch ( IOException ) { }

			return false;
		}

		public Boolean DemandPermission( FileIOPermissionAccess access ) {
			try {
				var bob = new FileIOPermission( access: access, this.FullName );
				bob.Demand();

				return true;
			}
			catch ( ArgumentException exception ) {
				exception.More();
			}
			catch ( SecurityException ) { }

			return false;
		}

		public Boolean Equals( Folder other ) => Equals( this, other );

		/// <summary>
		///     Returns true if the <see cref="Folder" /> currently exists.
		/// </summary>
		/// <exception cref="IOException"></exception>
		/// <exception cref="SecurityException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		public Boolean Exists() {
			this.Refresh();

			return this.Info.Exists;
		}

		/// <summary>
		///     Free space available to the current user.
		/// </summary>
		/// <returns></returns>
		public UInt64 GetAvailableFreeSpace() {
			var driveLetter = this.GetDrive().ToString();
			var driveInfo = new DriveInfo( driveLetter );

			return ( UInt64 ) driveInfo.AvailableFreeSpace;
		}

		/// <summary>
		///     <para>Returns an enumerable collection of <see cref="Document" /> in the current directory.</para>
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public IEnumerable<Document> GetDocuments() {
			if ( !this.Info.Exists ) {
				this.Refresh();

				if ( !this.Info.Exists ) {
					return Enumerable.Empty<Document>();
				}
			}

			return this.Info.BetterEnumerateFiles().Select( fileInfo => new Document( fileInfo.FullName ) );
		}

		[NotNull]
		public IEnumerable<Document> GetDocuments( [NotNull] String searchPattern ) => this.Info.BetterEnumerateFiles( searchPattern ).Select( fileInfo => new Document( fileInfo.FullName ) );

		[NotNull]
		public IEnumerable<Document> GetDocuments( [NotNull] IEnumerable<String> searchPatterns ) => searchPatterns.SelectMany( this.GetDocuments );

		[NotNull]
		public Disk GetDrive() => new Disk( this.Info.Root.FullName );

		[ItemNotNull]
		public IEnumerable<Folder> GetFolders( [CanBeNull] String searchPattern, SearchOption searchOption = SearchOption.AllDirectories ) {
			if ( String.IsNullOrEmpty( searchPattern ) ) {
				yield break;
			}

			foreach ( var fileInfo in this.Info.BetterEnumerateDirectories( searchPattern, searchOption ) ) {
				yield return new Folder( fileInfo.FullName );
			}
		}

		public override Int32 GetHashCode() => this.FullName.GetHashCode();

		[CanBeNull]
		public Folder GetParent() => this.Info.Parent is null ? null : new Folder( this.Info.Parent );

		/// <summary>
		///     <para>Check if this <see cref="Folder" /> contains any <see cref="Folder" /> or <see cref="Document" /> .</para>
		/// </summary>
		/// <returns></returns>
		public Boolean IsEmpty() => !this.GetFolders( "*.*" ).Any() && !this.GetDocuments( "*.*" ).Any();

		public void OpenWithExplorer() => Windows.ExecuteExplorer( arguments: this.FullName );

		public void Refresh() => this.Info.Refresh();

		/// <summary>
		///     <para>Shorten the full path with "..."</para>
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public String ToCompactFormat() {
			var sb = new StringBuilder();
			NativeMethods.PathCompactPathEx( sb, this.FullName, this.FullName.Length, 0 ); //TODO untested. //HACK may be buggy on extensions also

			return sb.ToString();
		}

		/// <summary>
		///     Returns a String that represents the current object.
		/// </summary>
		/// <returns>A String that represents the current object.</returns>
		public override String ToString() => this.FullName;
	}
}