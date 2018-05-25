// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "WebClientExtensions.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/WebClientExtensions.cs" was last formatted by Protiguous on 2018/05/24 at 6:39 PM.

namespace Librainian.Internet {

    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Threading;

    /// <summary>
    ///     <para>Extension methods for working with WebClient asynchronously.</para>
    ///     <para>Copyright (c) Microsoft Corporation. All rights reserved.</para>
    /// </summary>
    public static class WebClientExtensions {

        /// <summary>Downloads the resource with the specified URI as a byte array, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <returns>A Task that contains the downloaded data.</returns>
        public static async Task<Byte[]> DownloadDataTask( this WebClient webClient, String address ) => await DownloadDataTask( webClient, new Uri( address ) ).NoUI();

        /// <summary>Downloads the resource with the specified URI as a byte array, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <returns>A Task that contains the downloaded data.</returns>
        public static async Task<Byte[]> DownloadDataTask( this WebClient webClient, Uri address ) {
            try { return await webClient.DownloadDataTaskAsync( address ).NoUI(); }
            catch ( Exception exception ) { exception.More(); }

            return null;
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
            try { webClient.OpenReadAsync( address, tcs ); }
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
            try { webClient.OpenWriteAsync( address, method, tcs ); }
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
            try { webClient.UploadDataAsync( address, method, data, tcs ); }
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
            try { webClient.UploadFileAsync( address, method, fileName, tcs ); }
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
            try { webClient.UploadStringAsync( address, method, data, tcs ); }
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