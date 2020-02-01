// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "BasicFTPClient.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "BasicFTPClient.cs" was last formatted by Protiguous on 2020/01/31 at 12:25 AM.

namespace Librainian.Internet.FTP {

    using System;
    using System.IO;
    using System.Net;
    using JetBrains.Annotations;

    public class BasicFtpClient {

        public String Host { get; set; }

        public String Password { get; set; }

        public Int32 Port { get; set; }

        public String Username { get; set; }

        public BasicFtpClient() {
            this.Username = "anonymous";
            this.Password = "anonymous@internet.com";
            this.Port = 21;
            this.Host = "";
        }

        public BasicFtpClient( [CanBeNull] String theUser, [CanBeNull] String thePassword, [CanBeNull] String theHost ) {
            this.Username = theUser;
            this.Password = thePassword;
            this.Host = theHost;
            this.Port = 21;
        }

        [NotNull]
        private Uri BuildServerUri( [CanBeNull] String path ) => new Uri( $"ftp://{this.Host}:{this.Port}/{path}" );

        /// <summary>This method downloads the given file name from the FTP Server and returns a byte array containing its contents. Throws a WebException on encountering a network error.</summary>
        [CanBeNull]
        public Byte[] DownloadData( [CanBeNull] String path ) {

            // Get the object used to communicate with the Server.
            var request = new WebClient {
                Credentials = new NetworkCredential( userName: this.Username, password: this.Password )
            };

            // Logon to the Server using username + password
            return request.DownloadData( this.BuildServerUri( path ) );
        }

        /// <summary>This method downloads the FTP file specified by "ftppath" and saves it to "destfile". Throws a WebException on encountering a network error.</summary>
        public void DownloadFile( [CanBeNull] String ftppath, [NotNull] String destfile ) {

            // Download the data
            var data = this.DownloadData( ftppath );

            // Save the data to disk

            if ( data != null ) {
                using ( var fs = new FileStream( destfile, FileMode.Create ) ) {
                    fs.Write( data, 0, data.Length );
                    fs.Close();
                }
            }
        }

        /// <summary>Upload a byte[] to the FTP Server</summary>
        /// <param name="path">Path on the FTP Server (upload/myfile.txt)</param>
        /// <param name="data">A byte[] containing the data to upload</param>
        /// <returns>The Server response in a byte[]</returns>
        [CanBeNull]
        public Byte[] UploadData( [CanBeNull] String path, [NotNull] Byte[] data ) {

            // Get the object used to communicate with the Server.
            var request = new WebClient {
                Credentials = new NetworkCredential( userName: this.Username, password: this.Password )
            };

            // Logon to the Server using username + password
            return request.UploadData( this.BuildServerUri( path ), data );
        }

        /// <summary>Load a file from disk and upload it to the FTP Server</summary>
        /// <param name="ftppath">Path on the FTP Server (/upload/myfile.txt)</param>
        /// <param name="srcfile">File on the local harddisk to upload</param>
        /// <returns>The Server response in a byte[]</returns>
        [CanBeNull]
        public Byte[] UploadFile( [CanBeNull] String ftppath, [NotNull] String srcfile ) {

            // Read the data from disk
            var fs = new FileStream( srcfile, FileMode.Open );
            var fileData = new Byte[ fs.Length ];

            var numBytesToRead = ( Int32 ) fs.Length;
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