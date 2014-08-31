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
// "Librainian/Inet.cs" was last cleaned by Rick on 2014/08/19 at 1:27 PM
#endregion

namespace Librainian.Internet {
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    public static class Inet {
        private static async Task< byte[] > GetURLContentsAsync( string url ) {
            // The downloaded resource ends up in the variable named content. 
            var content = new MemoryStream();

            var webReq = WebRequest.Create( url ) as HttpWebRequest;

            // Send the request to the Internet resource and wait for 
            // the response.                 
            using ( var response = await webReq.GetResponseAsync() )

                // The previous statement abbreviates the following two statements. 

                //Task<WebResponse> responseTask = webReq.GetResponseAsync(); 
                //using (WebResponse response = await responseTask)
            {
                // Get the data stream that is associated with the specified url. 
                using ( var responseStream = response.GetResponseStream() ) {
                    // Read the bytes in responseStream and copy them to content. 
                    await responseStream.CopyToAsync( content );

                    // The previous statement abbreviates the following two statements. 

                    // CopyToAsync returns a Task, not a Task<T>. 
                    //Task copyTask = responseStream.CopyToAsync(content); 

                    // When copyTask is completed, content contains a copy of 
                    // responseStream. 
                    //await copyTask;
                }
            }
            // Return the result as a byte array. 
            return content.ToArray();
        }
    }
}
