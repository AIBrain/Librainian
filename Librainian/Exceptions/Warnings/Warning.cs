#nullable enable

namespace Librainian.Exceptions.Warnings {

	using System;
	using System.Diagnostics;
	using System.Runtime.Serialization;
	using JetBrains.Annotations;
	using Logging;

	/// <inheritdoc />
	/// <summary>
	///     <para>Generic Warning</para>
	///     <para><see cref="Debugger.Break" /> if a <see cref="Debugger" /> is attached.</para>
	///     <para>This should be handled, but allow program to continue.</para>
	/// </summary>
	[Serializable]
	public class Warning : Exception {

		public Warning() => "".Break();

		public Warning( String? message ) : base( message ) => message.Break();

		public Warning( String? message, [CanBeNull] Exception? inner ) : base( message, inner ) => message.Break();

		protected Warning( SerializationInfo serializationInfo, StreamingContext streamingContext ) {
			this.SerializationInfo = serializationInfo;
			this.StreamingContext = streamingContext;
		}

		public SerializationInfo? SerializationInfo { get; protected set; }
		public StreamingContext? StreamingContext { get; protected set; }

	}

}