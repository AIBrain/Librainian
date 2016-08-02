// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/SmtpClientExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Internet {

    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;

    /// <summary>
    ///     <para>Extension methods for working with SmtpClient asynchronously.</para>
    ///     <para>Copyright (c) Microsoft Corporation. All rights reserved.</para>
    /// </summary>
    public static class SmtpClientExtensions {

        /// <summary>Sends an e-mail message asynchronously.</summary>
        /// <param name="smtpClient">The client.</param>
        /// <param name="message">A MailMessage that contains the message to send.</param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A Task that represents the asynchronous send.</returns>
        public static Task SendTask( this SmtpClient smtpClient, MailMessage message, Object userToken ) => SendTaskCore( smtpClient, userToken, tcs => smtpClient.SendAsync( message, tcs ) );

        /// <summary>Sends an e-mail message asynchronously.</summary>
        /// <param name="smtpClient">The client.</param>
        /// <param name="from">A String that contains the address information of the message sender.</param>
        /// <param name="recipients">
        ///     A String that contains the address that the message is sent to.
        /// </param>
        /// <param name="subject">A String that contains the subject line for the message.</param>
        /// <param name="body">A String that contains the message body.</param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A Task that represents the asynchronous send.</returns>
        public static Task SendTask( this SmtpClient smtpClient, String from, String recipients, String subject, String body, Object userToken ) => SendTaskCore( smtpClient, userToken, tcs => smtpClient.SendAsync( @from, recipients, subject, body, tcs ) );

        /// <summary>The core implementation of SendTask.</summary>
        /// <param name="smtpClient">The client.</param>
        /// <param name="userToken">The user-supplied state.</param>
        /// <param name="sendAsync">
        ///     A delegate that initiates the asynchronous send. The provided TaskCompletionSource must
        ///     be passed as the user-supplied state to the actual SmtpClient.SendAsync method.
        /// </param>
        /// <returns></returns>
        private static Task SendTaskCore( SmtpClient smtpClient, Object userToken, Action<TaskCompletionSource<Object>> sendAsync ) {

            // Validate we're being used with a real smtpClient. The rest of the arg validation will
            // happen in the call to sendAsync.
            if ( smtpClient == null ) {
                throw new ArgumentNullException( nameof( smtpClient ) );
            }

            // Create a TaskCompletionSource to represent the operation
            var tcs = new TaskCompletionSource<Object>( userToken );

            // Register a handler that will transfer completion results to the TCS Task
            SendCompletedEventHandler handler = null;
            handler = ( sender, e ) => EapCommon.HandleCompletion( tcs, e, () => null, () => smtpClient.SendCompleted -= handler );
            smtpClient.SendCompleted += handler;

            // Try to start the async operation. If starting it fails (due to parameter validation)
            // unregister the handler before allowing the exception to propagate.
            try {
                sendAsync( tcs );
            }
            catch ( Exception exc ) {
                smtpClient.SendCompleted -= handler;
                tcs.TrySetException( exc );
            }

            // Return the task to represent the asynchronous operation
            return tcs.Task;
        }
    }
}