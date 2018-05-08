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
// "Librainian/WebClientExtensions.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Internet {

    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    ///     <para>Extension methods for working with WebClient asynchronously.</para>
    ///     <para>Copyright (c) Microsoft Corporation. All rights reserved.</para>
    /// </summary>
    public static class WebClientExtensions {

        /// <summary>Downloads the resource with the specified URI as a byte array, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <returns>A Task that contains the downloaded data.</returns>
        public static Task<Byte[]> DownloadDataTask( this WebClient webClient, String address ) => DownloadDataTask( webClient, new Uri( address ) );

        /// <summary>Downloads the resource with the specified URI as a byte array, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <returns>A Task that contains the downloaded data.</returns>
        public static Task<Byte[]> DownloadDataTask( this WebClient webClient, Uri address ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<Byte[]>( address );

            // Setup the callback event handler
	        void Handler( Object sender, DownloadDataCompletedEventArgs e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.DownloadDataCompleted -= Handler );

	        webClient.DownloadDataCompleted += Handler;

            // Start the async work
            try {
                webClient.DownloadDataAsync( address, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.DownloadDataCompleted -= Handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Downloads the resource with the specified URI to a local file, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <param name="fileName">The name of the local file that is to receive the data.</param>
        /// <returns>A Task that contains the downloaded data.</returns>
        public static Task DownloadFileTask( this WebClient webClient, String address, String fileName ) => DownloadFileTask( webClient, new Uri( address ), fileName );

        /// <summary>Downloads the resource with the specified URI to a local file, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <param name="fileName">The name of the local file that is to receive the data.</param>
        /// <returns>A Task that contains the downloaded data.</returns>
        public static Task DownloadFileTask( this WebClient webClient, Uri address, String fileName ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<Object>( address );

            // Setup the callback event handler
	        void Handler( Object sender, AsyncCompletedEventArgs e ) => EapCommon.HandleCompletion( tcs, e, () => null, () => webClient.DownloadFileCompleted -= Handler );

	        webClient.DownloadFileCompleted += Handler;

            // Start the async work
            try {
                webClient.DownloadFileAsync( address, fileName, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.DownloadFileCompleted -= Handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Downloads the resource with the specified URI as a String, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <returns>A Task that contains the downloaded String.</returns>
        public static Task<String> DownloadStringTask( this WebClient webClient, String address ) => DownloadStringTask( webClient, new Uri( address ) );

        /// <summary>Downloads the resource with the specified URI as a String, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <returns>A Task that contains the downloaded String.</returns>
        public static Task<String> DownloadStringTask( this WebClient webClient, Uri address ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<String>( address );

            // Setup the callback event handler
	        void Handler( Object sender, DownloadStringCompletedEventArgs e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.DownloadStringCompleted -= Handler );

	        webClient.DownloadStringCompleted += Handler;

            // Start the async work
            try {
                webClient.DownloadStringAsync( address, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.DownloadStringCompleted -= Handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Opens a readable stream for the data downloaded from a resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI for which the stream should be opened.</param>
        /// <returns>A Task that contains the opened stream.</returns>
        public static Task<Stream> OpenReadTask( this WebClient webClient, String address ) => OpenReadTask( webClient, new Uri( address ) );

        /// <summary>Opens a readable stream for the data downloaded from a resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI for which the stream should be opened.</param>
        /// <returns>A Task that contains the opened stream.</returns>
        public static Task<Stream> OpenReadTask( this WebClient webClient, Uri address ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<Stream>( address );

            // Setup the callback event handler
	        void Handler( Object sender, OpenReadCompletedEventArgs e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.OpenReadCompleted -= Handler );

	        webClient.OpenReadCompleted += Handler;

            // Start the async work
            try {
                webClient.OpenReadAsync( address, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.OpenReadCompleted -= Handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Opens a writeable stream for uploading data to a resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI for which the stream should be opened.</param>
        /// <param name="method">The HTTP method that should be used to open the stream.</param>
        /// <returns>A Task that contains the opened stream.</returns>
        public static Task<Stream> OpenWriteTask( this WebClient webClient, String address, String method ) => OpenWriteTask( webClient, new Uri( address ), method );

        /// <summary>Opens a writeable stream for uploading data to a resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI for which the stream should be opened.</param>
        /// <param name="method">The HTTP method that should be used to open the stream.</param>
        /// <returns>A Task that contains the opened stream.</returns>
        public static Task<Stream> OpenWriteTask( this WebClient webClient, Uri address, String method ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<Stream>( address );

            // Setup the callback event handler
	        void Handler( Object sender, OpenWriteCompletedEventArgs e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.OpenWriteCompleted -= Handler );

	        webClient.OpenWriteCompleted += Handler;

            // Start the async work
            try {
                webClient.OpenWriteAsync( address, method, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.OpenWriteCompleted -= Handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Uploads data to the specified resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI to which the data should be uploaded.</param>
        /// <param name="method">The HTTP method that should be used to upload the data.</param>
        /// <param name="data">The data to upload.</param>
        /// <returns>A Task containing the data in the response from the upload.</returns>
        public static Task<Byte[]> UploadDataTask( this WebClient webClient, String address, String method, Byte[] data ) => UploadDataTask( webClient, new Uri( address ), method, data );

        /// <summary>Uploads data to the specified resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI to which the data should be uploaded.</param>
        /// <param name="method">The HTTP method that should be used to upload the data.</param>
        /// <param name="data">The data to upload.</param>
        /// <returns>A Task containing the data in the response from the upload.</returns>
        public static Task<Byte[]> UploadDataTask( this WebClient webClient, Uri address, String method, Byte[] data ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<Byte[]>( address );

            // Setup the callback event handler
	        void Handler( Object sender, UploadDataCompletedEventArgs e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.UploadDataCompleted -= Handler );

	        webClient.UploadDataCompleted += Handler;

            // Start the async work
            try {
                webClient.UploadDataAsync( address, method, data, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.UploadDataCompleted -= Handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Uploads a file to the specified resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI to which the file should be uploaded.</param>
        /// <param name="method">The HTTP method that should be used to upload the file.</param>
        /// <param name="fileName">A path to the file to upload.</param>
        /// <returns>A Task containing the data in the response from the upload.</returns>
        public static Task<Byte[]> UploadFileTask( this WebClient webClient, String address, String method, String fileName ) => UploadFileTask( webClient, new Uri( address ), method, fileName );

        /// <summary>Uploads a file to the specified resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI to which the file should be uploaded.</param>
        /// <param name="method">The HTTP method that should be used to upload the file.</param>
        /// <param name="fileName">A path to the file to upload.</param>
        /// <returns>A Task containing the data in the response from the upload.</returns>
        public static Task<Byte[]> UploadFileTask( this WebClient webClient, Uri address, String method, String fileName ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<Byte[]>( address );

            // Setup the callback event handler
	        void Handler( Object sender, UploadFileCompletedEventArgs e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.UploadFileCompleted -= Handler );

	        webClient.UploadFileCompleted += Handler;

            // Start the async work
            try {
                webClient.UploadFileAsync( address, method, fileName, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.UploadFileCompleted -= Handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Uploads data in a String to the specified resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI to which the data should be uploaded.</param>
        /// <param name="method">The HTTP method that should be used to upload the data.</param>
        /// <param name="data">The data to upload.</param>
        /// <returns>A Task containing the data in the response from the upload.</returns>
        public static Task<String> UploadStringTask( this WebClient webClient, String address, String method, String data ) => UploadStringTask( webClient, new Uri( address ), method, data );

        /// <summary>Uploads data in a String to the specified resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI to which the data should be uploaded.</param>
        /// <param name="method">The HTTP method that should be used to upload the data.</param>
        /// <param name="data">The data to upload.</param>
        /// <returns>A Task containing the data in the response from the upload.</returns>
        public static Task<String> UploadStringTask( this WebClient webClient, Uri address, String method, String data ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<String>( address );

            // Setup the callback event handler
	        void Handler( Object sender, UploadStringCompletedEventArgs e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.UploadStringCompleted -= Handler );

	        webClient.UploadStringCompleted += Handler;

            // Start the async work
            try {
                webClient.UploadStringAsync( address, method, data, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.UploadStringCompleted -= Handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }
    }
}