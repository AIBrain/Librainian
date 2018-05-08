// Copyright 2016 Protiguous.
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
// "Librainian/ChainElement.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Magic {

    public class ChainElement {
        private readonly ChainElement _next;

        protected ChainElement( ChainElement next ) => this._next = next;

	    protected ChainElement() {
        }

        public T As<T>( T defaultValue ) where T : class {
            if ( this is T ) {
                return this as T;
            }

            return this._next != null ? this._next.As( defaultValue ) : defaultValue;
        }
    }
}