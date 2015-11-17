// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/BasicFTPClient.cs" was last cleaned by Rick on 2015/06/12 at 2:56 PM

namespace Librainian.Internet.FTP {

    using System;
    using System.IO;
    using System.Net;

    public class BasicFtpClient {

        public String Host {
            get; set;
        }

        public String Password {
            get; set;
        }

        public Int32 Port {
            get; set;
        }

        public String Username {
            get; set;
        }

        public BasicFtpClient() {
            this.Username = "anonymous";
            this.Password = "anonymous@internet.com";
            this.Port = 21;
            this.Host = "";
        }

        public BasicFtpClient(String theUser, String thePassword, String theHost) {
            this.Username = theUser;
            this.Password = thePassword;
            this.Host = theHost;
            this.Port = 21;
        }

        /// <summary>
        /// This method downloads the given file name from the FTP Server and returns a byte array
        /// containing its contents. Throws a WebException on encountering a network error.
        /// </summary>
        public Byte[] DownloadData(String path) {

            // Get the object used to communicate with the Server.
            var request = new WebClient {
                Credentials = new NetworkCredential( userName: this.Username, password: this.Password )
            };

            // Logon to the Server using username + password
            return request.DownloadData( this.BuildServerUri( path ) );
        }

        /// <summary>
        /// This method downloads the FTP file specified by "ftppath" and saves it to "destfile".
        /// Throws a WebException on encountering a network error.
        /// </summary>
        public void DownloadFile(String ftppath, String destfile) {

            // Download the data
            var data = this.DownloadData( ftppath );

            // Save the data to disk
            var fs = new FileStream( destfile, FileMode.Create );
            fs.Write( data, 0, data.Length );
            fs.Close();
        }

        /// <summary>Upload a byte[] to the FTP Server</summary>
        /// <param name="path">Path on the FTP Server (upload/myfile.txt)</param>
        /// <param name="data">A byte[] containing the data to upload</param>
        /// <returns>The Server response in a byte[]</returns>
        public Byte[] UploadData(String path, Byte[] data) {

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
        public Byte[] UploadFile(String ftppath, String srcfile) {

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

        private Uri BuildServerUri(String path) => new Uri( $"ftp://{this.Host}:{this.Port}/{path}" );
    }
}