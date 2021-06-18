// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".

#nullable enable

namespace Librainian.Internet.FTP {

	using System;
	using System.IO;
	using System.Net;
	using Exceptions;

	public class BasicFtpClient {

		public String Host { get; }

		public String Password { get; }

		public Int32 Port { get; }

		public String Username { get; }

		public BasicFtpClient() {
			this.Username = "anonymous";
			this.Password = "anonymous@internet.com";
			this.Port = 21;
			this.Host = "";
		}

		public BasicFtpClient( String theUser, String thePassword, String theHost ) {
			if ( String.IsNullOrWhiteSpace( theUser ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( theUser ) );
			}

			if ( String.IsNullOrWhiteSpace( thePassword ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( thePassword ) );
			}

			if ( String.IsNullOrWhiteSpace( theHost ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( theHost ) );
			}

			this.Username = theUser;
			this.Password = thePassword;
			this.Host = theHost;
			this.Port = 21;
		}

		private Uri BuildServerUri( String path ) {
			if ( String.IsNullOrWhiteSpace( path ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( path ) );
			}

			return new Uri( $"ftp://{this.Host}:{this.Port}/{path}" );
		}

		/// <summary>
		///     This method downloads the given file name from the FTP Server and returns a byte array containing its contents.
		///     Throws a WebException on encountering a network error.
		/// </summary>
		public Byte[] DownloadData( String path ) {
			if ( String.IsNullOrWhiteSpace( path ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( path ) );
			}

			// Get the object used to communicate with the Server.
			var request = new WebClient {
				Credentials = new NetworkCredential( this.Username, this.Password )
			};

			// Logon to the Server using username + password
			return request.DownloadData( this.BuildServerUri( path ) );
		}

		/// <summary>
		///     This method downloads the FTP file specified by "ftppath" and saves it to "destfile". Throws a WebException on
		///     encountering a network error.
		/// </summary>
		public void DownloadFile( String ftppath, String destfile ) {
			if ( String.IsNullOrWhiteSpace( ftppath ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( ftppath ) );
			}

			if ( String.IsNullOrWhiteSpace( destfile ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( destfile ) );
			}

			// Download the data
			var data = this.DownloadData( ftppath );

			// Save the data to disk

			using var fs = new FileStream( destfile, FileMode.Create );

			fs.Write( data, 0, data.Length );
			fs.Close();
		}

		/// <summary>
		///     Upload a byte[] to the FTP Server
		/// </summary>
		/// <param name="path">Path on the FTP Server (upload/myfile.txt)</param>
		/// <param name="data">A byte[] containing the data to upload</param>
		/// <returns>The Server response in a byte[]</returns>
		public Byte[] UploadData( String path, Byte[] data ) {
			if ( String.IsNullOrWhiteSpace( path ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( path ) );
			}

			if ( data == null ) {
				throw new ArgumentEmptyException( nameof( data ) );
			}

			// Get the object used to communicate with the Server.
			var request = new WebClient {
				Credentials = new NetworkCredential( this.Username, this.Password )
			};

			// Logon to the Server using username + password
			return request.UploadData( this.BuildServerUri( path ), data );
		}

		/// <summary>
		///     Load a file from disk and upload it to the FTP Server
		/// </summary>
		/// <param name="ftppath">Path on the FTP Server (/upload/myfile.txt)</param>
		/// <param name="srcfile">File on the local harddisk to upload</param>
		/// <returns>The Server response in a byte[]</returns>
		public Byte[] UploadFile( String ftppath, String srcfile ) {
			if ( String.IsNullOrWhiteSpace( ftppath ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( ftppath ) );
			}

			// Read the data from disk
			var fs = new FileStream( srcfile, FileMode.Open );
			var fileData = new Byte[ fs.Length ];

			var numBytesToRead = ( Int32 )fs.Length;
			var numBytesRead = 0;

			while ( numBytesToRead > 0 ) {

				// Read may return anything from 0 to numBytesToRead.
				var n = fs.Read( fileData, numBytesRead, numBytesToRead );

				// Break when the end of the file is reached.
				if ( n == 0 ) {
					break;
				}

				numBytesRead += n;
				numBytesToRead -= n;
			}

			//numBytesToRead = FileData.Length;
			fs.Close();

			// Upload the data from the buffer
			return this.UploadData( ftppath, fileData );
		}
	}
}