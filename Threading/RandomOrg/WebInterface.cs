namespace Librainian.Threading.RandomOrg {
    using System;
    using System.IO;
    using System.Net;

    internal class WebInterface {
        public static string GetWebPage( string url ) {
            try {
                var request = WebRequest.Create( url );
                request.Proxy = null;
                request.Credentials = CredentialCache.DefaultCredentials;

                using ( var response = request.GetResponse() as HttpWebResponse ) {
                    if ( response != null ) {
                        using ( var dataStream = response.GetResponseStream() ) {
                            if ( dataStream != null ) {
                                using ( var reader = new StreamReader( dataStream ) ) {
                                    var responseFromServer = reader.ReadToEnd();
                                    return responseFromServer;
                                }
                            }
                        }
                        response.Close();
                    }
                }
            }
            catch {
                throw new Exception( "Unable to connect to Random.org." );
            }
            return null;
        }
    }
}