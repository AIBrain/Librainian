// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Inet.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

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
            if ( !( WebRequest.Create( url ) is HttpWebRequest webReq ) ) {
                return content.ToArray();
            }
            using ( var response = await webReq.GetResponseAsync() )

            // The previous statement abbreviates the following two statements.

            //Task<WebResponse> responseTask = webReq.GetResponseAsync();
            //using (WebResponse response = await responseTask)
            {
                // Get the data stream that is associated with the specified url.
                using ( var responseStream = response.GetResponseStream() ) {

                    // Read the bytes in responseStream and copy them to content.
                    if ( responseStream != null ) {
                        await responseStream.CopyToAsync( content );
                    }

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