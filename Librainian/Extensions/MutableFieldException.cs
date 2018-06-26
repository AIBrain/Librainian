namespace Librainian.Extensions {

	using System;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Exceptions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	[JsonObject]
	[Serializable]
	internal class MutableFieldException : ImmutableFailureException {

		[NotNull]
		private static String FormatMessage( [NotNull] FieldInfo fieldInfo ) {
			if ( fieldInfo == null ) {
				throw new ArgumentNullException( paramName: nameof( fieldInfo ) );
			}

			return $"'{fieldInfo.DeclaringType}' is mutable because '{fieldInfo.Name}' of type '{fieldInfo.FieldType}' is mutable.";
		}

		protected MutableFieldException( [NotNull] SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) {
			if ( serializationInfo == null ) {
				throw new ArgumentNullException( paramName: nameof( serializationInfo ) );
			}
		}

		internal MutableFieldException( [NotNull] FieldInfo fieldInfo, Exception inner ) : base( fieldInfo.DeclaringType, FormatMessage( fieldInfo ), inner ) {
			if ( fieldInfo == null ) {
				throw new ArgumentNullException( paramName: nameof( fieldInfo ) );
			}
		}
	}
}