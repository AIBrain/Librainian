﻿// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "SmtpClientExtensions.cs" last formatted on 2020-08-14 at 8:35 PM.

namespace Librainian.Internet {

	using System;
	using System.ComponentModel;
	using System.Net.Mail;
	using System.Threading.Tasks;
	using JetBrains.Annotations;

	/// <summary>
	///     <para>Extension methods for working with SmtpClient asynchronously.</para>
	///     <para>Copyright (c) Microsoft Corporation. All rights reserved.</para>
	/// </summary>
	public static class SmtpClientExtensions {

		/// <summary>The core implementation of SendTask.</summary>
		/// <param name="smtpClient">The client.</param>
		/// <param name="userToken">The user-supplied state.</param>
		/// <param name="sendAsync">
		///     A delegate that initiates the asynchronous send. The provided TaskCompletionSource must be passed as the
		///     user-supplied state to the actual
		///     SmtpClient.SendAsync method.
		/// </param>
		/// <returns></returns>
		[CanBeNull]
		private static Task SendTaskCore( [NotNull] SmtpClient smtpClient, [CanBeNull] Object? userToken, [CanBeNull] Action<TaskCompletionSource<Object>> sendAsync ) {
			// Validate we're being used with a real smtpClient. The rest of the arg validation will
			// happen in the call to sendAsync.
			if ( smtpClient is null ) {
				throw new ArgumentNullException( nameof( smtpClient ) );
			}

			// Create a TaskCompletionSource to represent the operation
			var tcs = new TaskCompletionSource<Object>( userToken, TaskCreationOptions.RunContinuationsAsynchronously );

			// Register a handler that will transfer completion results to the TCS Task
			void Handler( Object sender, AsyncCompletedEventArgs e ) => tcs.HandleCompletion( e, () => null, () => smtpClient.SendCompleted -= Handler );

			smtpClient.SendCompleted += Handler;

			// Try to start the async operation. If starting it fails (due to parameter validation)
			// unregister the handler before allowing the exception to propagate.
			try {
				sendAsync( tcs );
			}
			catch ( Exception exc ) {
				smtpClient.SendCompleted -= Handler;
				tcs.TrySetException( exc );
			}

			// Return the task to represent the asynchronous operation
			return tcs.Task;
		}

		/// <summary>Sends an e-mail message asynchronously.</summary>
		/// <param name="smtpClient">The client.</param>
		/// <param name="message">A MailMessage that contains the message to send.</param>
		/// <param name="userToken">A user-defined object stored in the resulting Task.</param>
		/// <returns>A Task that represents the asynchronous send.</returns>
		[CanBeNull]
		public static Task SendTask( [NotNull] this SmtpClient smtpClient, [NotNull] MailMessage message, [CanBeNull] Object? userToken ) {
			if ( message == null ) {
				throw new ArgumentNullException( nameof( message ) );
			}

			return SendTaskCore( smtpClient, userToken, tcs => smtpClient.SendAsync( message, tcs ) );
		}

		/// <summary>Sends an e-mail message asynchronously.</summary>
		/// <param name="smtpClient">The client.</param>
		/// <param name="from">A String that contains the address information of the message sender.</param>
		/// <param name="recipients">A String that contains the address that the message is sent to.</param>
		/// <param name="subject">A String that contains the subject line for the message.</param>
		/// <param name="body">A String that contains the message body.</param>
		/// <param name="userToken">A user-defined object stored in the resulting Task.</param>
		/// <returns>A Task that represents the asynchronous send.</returns>
		[CanBeNull]
		public static Task SendTask(
			[NotNull] this SmtpClient smtpClient,
			[NotNull] String from,
			[NotNull] String recipients,
			[CanBeNull] String? subject,
			[CanBeNull] String? body,
			[CanBeNull] Object? userToken
		) {
			if ( String.IsNullOrWhiteSpace( from ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( from ) );
			}

			if ( String.IsNullOrWhiteSpace( recipients ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( recipients ) );
			}

			return SendTaskCore( smtpClient, userToken, tcs => smtpClient.SendAsync( from, recipients, subject, body, tcs ) );
		}

	}

}