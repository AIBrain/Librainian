﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "SmtpClientExtensions.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "SmtpClientExtensions.cs" was last formatted by Protiguous on 2018/07/10 at 9:11 PM.

namespace Librainian.Internet
{

    using JetBrains.Annotations;
    using System;
    using System.ComponentModel;
    using System.Net.Mail;
    using System.Threading.Tasks;

    /// <summary>
    ///     <para>Extension methods for working with SmtpClient asynchronously.</para>
    ///     <para>Copyright (c) Microsoft Corporation. All rights reserved.</para>
    /// </summary>
    public static class SmtpClientExtensions
    {

        /// <summary>The core implementation of SendTask.</summary>
        /// <param name="smtpClient">The client.</param>
        /// <param name="userToken">The user-supplied state.</param>
        /// <param name="sendAsync">
        ///     A delegate that initiates the asynchronous send. The provided TaskCompletionSource must
        ///     be passed as the user-supplied state to the actual SmtpClient.SendAsync method.
        /// </param>
        /// <returns></returns>
        private static Task SendTaskCore([NotNull] SmtpClient smtpClient, Object userToken, Action<TaskCompletionSource<Object>> sendAsync)
        {

            // Validate we're being used with a real smtpClient. The rest of the arg validation will
            // happen in the call to sendAsync.
            if (smtpClient == null) { throw new ArgumentNullException(nameof(smtpClient)); }

            // Create a TaskCompletionSource to represent the operation
            var tcs = new TaskCompletionSource<Object>(userToken);

            // Register a handler that will transfer completion results to the TCS Task
            void Handler(Object sender, AsyncCompletedEventArgs e) => tcs.HandleCompletion(e, () => null, () => smtpClient.SendCompleted -= Handler);

            smtpClient.SendCompleted += Handler;

            // Try to start the async operation. If starting it fails (due to parameter validation)
            // unregister the handler before allowing the exception to propagate.
            try { sendAsync(tcs); }
            catch (Exception exc)
            {
                smtpClient.SendCompleted -= Handler;
                tcs.TrySetException(exc);
            }

            // Return the task to represent the asynchronous operation
            return tcs.Task;
        }

        /// <summary>Sends an e-mail message asynchronously.</summary>
        /// <param name="smtpClient">The client.</param>
        /// <param name="message">A MailMessage that contains the message to send.</param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A Task that represents the asynchronous send.</returns>
        public static Task SendTask([NotNull] this SmtpClient smtpClient, MailMessage message, Object userToken) => SendTaskCore(smtpClient, userToken, tcs => smtpClient.SendAsync(message, tcs));

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
        public static Task SendTask([NotNull] this SmtpClient smtpClient, String from, String recipients, String subject, String body, Object userToken) =>
            SendTaskCore(smtpClient, userToken, tcs => smtpClient.SendAsync(from, recipients, subject, body, tcs));
    }
}