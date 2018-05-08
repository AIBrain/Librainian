// Copyright 2018 Protiguous.
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
// "Librainian/ActorException.cs" was last cleaned by Protiguous on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;
    using System.Runtime.Serialization;

    /// <summary>Thrown when the actor fails.</summary>
    /// <seealso cref="Actor" />
    [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2240:ImplementISerializableCorrectly" )]
    [Serializable]
    public class ActorException : Exception {

        public ActorException( String because ) => this.Reason = because;

	    protected ActorException( SerializationInfo info, StreamingContext context ) : base( info, context ) { }

        public String Reason {
            get;
        }

        //[SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]
        //public override void GetObjectData( SerializationInfo info, StreamingContext context ) {
        //    if ( info is null ) {
        //        throw new ArgumentNullException( nameof( info ) );
        //    }

        //    //info.AddValue( "Text", _Text );
        //    //TODO
        //}
    }
}