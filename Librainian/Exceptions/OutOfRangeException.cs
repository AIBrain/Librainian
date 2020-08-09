#nullable enable

namespace Librainian.Exceptions {

	using System;
	using System.Runtime.Serialization;
	using JetBrains.Annotations;
	using Parsing;
	using Warnings;

	/// <summary>Use when a value is out of range. (Too low or too high)</summary>
	[Serializable]
	public class OutOfRangeException : Exception {

		/// <summary>Disallow no message.</summary>
		private OutOfRangeException() { }

		public OutOfRangeException( [CanBeNull] String message ) : base( message ) {
			if ( message.IsNullOrEmpty() ) {
				throw new NotAllowedException( "A message must be provided." );
			}
		}

		public OutOfRangeException( [CanBeNull] String message, [CanBeNull] Exception inner ) : base( message, inner ) {
			if ( message.IsNullOrEmpty() ) {
				throw new NotAllowedException( "A message must be provided." );
			}
		}

		protected OutOfRangeException( SerializationInfo serializationInfo, StreamingContext streamingContext ) => throw new NotImplementedException();

	}

}