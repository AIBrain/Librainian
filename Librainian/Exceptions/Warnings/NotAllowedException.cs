#nullable enable

namespace Librainian.Exceptions.Warnings {

	using System;
	using System.Runtime.Serialization;
	using JetBrains.Annotations;
	using Parsing;

	/// <summary><see cref="Warning" />: What the code just tried to do is <see cref="NotAllowedException" />.</summary>
	[Serializable]
	public class NotAllowedException : Warning {

		public NotAllowedException() : this( nameof( NotAllowedException ) ) { }
		public NotAllowedException( TrimmedString message ) : base( message ) { }

		public NotAllowedException( TrimmedString message, [CanBeNull] Exception inner ) : base( message, inner ) { }

		protected NotAllowedException( SerializationInfo serializationInfo, StreamingContext streamingContext ) {
			this.SerializationInfo = serializationInfo;
			this.StreamingContext = streamingContext;
		}

	}

}