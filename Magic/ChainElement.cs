// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/ChainElement.cs" was last cleaned by Rick on 2015/10/11 at 12:27 PM

namespace Librainian.Magic {

    public class ChainElement {

        private readonly ChainElement _next;

        protected ChainElement( ChainElement next ) {
            this._next = next;
        }

        protected ChainElement() {
        }

        public T As< T >( T defaultValue ) where T : class {
            if ( this is T ) {
                return this as T;
            }

            if ( this._next != null ) {
                return this._next.As( defaultValue );
            }

            return defaultValue;
        }

    }

}
