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
// "Librainian/BasicFTPClient.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Internet.FTP {
    using System;
    using System.IO;
    using System.Net;

    public class BasicFTPClient {
        public BasicFTPClient() {
            this.Username = "anonymous";
            this.Password = "anonymous@internet.com";
            this.Port = 21;
            this.Host = "";
        }

        public BasicFTPClient( string theUser, string thePassword, string theHost ) {
            this.Username = theUser;
            this.Password = thePassword;
            this.Host = theHost;
            this.Port = 21;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        /// <summary>
        ///     This method downloads the FTP file specified by "ftppath" and saves it to "destfile". Throws a WebException on
        ///     encountering a network error.
        /// </summary>
        public void DownloadFile( string ftppath, string destfile ) {
            // Download the data
            var Data = this.DownloadData( ftppath );

            // Save the data to disk
            var fs = new FileStream( destfile, FileMode.Create );
            fs.Write( Data, 0, Data.Length );
            fs.Close();
        }

        /// <summary>
        ///     This method downloads the given file name from the FTP server and returns a byte array containing its contents.
        ///     Throws a WebException on encountering a network error.
        /// </summary>
        public byte[] DownloadData( string path ) {
            // Get the object used to communicate with the server.
            var request = new WebClient {
                                            Credentials = new NetworkCredential( userName: this.Username, password: this.Password )
                                        };

            // Logon to the server using username + password
            return request.DownloadData( this.BuildServerUri( path ) );
        }

        private Uri BuildServerUri( string Path ) {
            return new Uri( String.Format( "ftp://{0}:{1}/{2}", this.Host, this.Port, Path ) );
        }

        /// <summary>
        ///     Load a file from disk and upload it to the FTP server
        /// </summary>
        /// <param name="ftppath"> Path on the FTP server (/upload/myfile.txt) </param>
        /// <param name="srcfile"> File on the local harddisk to upload </param>
        /// <returns> The server response in a byte[] </returns>
        public byte[] UploadFile( string ftppath, string srcfile ) {
            // Read the data from disk
            var fs = new FileStream( srcfile, FileMode.Open );
            var FileData = new byte[fs.Length];

            var numBytesToRead = ( int ) fs.Length;
            var numBytesRead = 0;
            while ( numBytesToRead > 0 ) {
                // Read may return anything from 0 to numBytesToRead.
                var n = fs.Read( FileData, numBytesRead, numBytesToRead );

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
            return this.UploadData( ftppath, FileData );
        }

        /// <summary>
        ///     Upload a byte[] to the FTP server
        /// </summary>
        /// <param name="path"> Path on the FTP server (upload/myfile.txt) </param>
        /// <param name="Data"> A byte[] containing the data to upload </param>
        /// <returns> The server response in a byte[] </returns>
        public byte[] UploadData( string path, byte[] Data ) {
            // Get the object used to communicate with the server.
            var request = new WebClient {
                                            Credentials = new NetworkCredential( userName: this.Username, password: this.Password )
                                        };

            // Logon to the server using username + password
            return request.UploadData( this.BuildServerUri( path ), Data );
        }
    }
}
