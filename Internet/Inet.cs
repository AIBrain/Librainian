// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Inet.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Inet.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Internet {

    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    public static class Inet {

        private static async Task<Byte[]> GetUrlContentsAsync( String url ) {

            // The downloaded resource ends up in the variable named content.
            var content = new MemoryStream();

            // Send the request to the Internet resource and wait for the response. ReSharper
            // disable once PossibleNullReferenceException
            if ( !( WebRequest.Create( url ) is HttpWebRequest webReq ) ) { return content.ToArray(); }

            using ( var response = await webReq.GetResponseAsync() )

            // The previous statement abbreviates the following two statements.

            //Task<WebResponse> responseTask = webReq.GetResponseAsync();
            //using (WebResponse response = await responseTask)
            {
                // Get the data stream that is associated with the specified url.
                using ( var responseStream = response.GetResponseStream() ) {

                    // Read the bytes in responseStream and copy them to content.
                    if ( responseStream != null ) { await responseStream.CopyToAsync( content ); }

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