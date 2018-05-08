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
// "Librainian/Int256.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Maths.Numbers {

    using System;

    /// <summary>
    ///     Struct with 4 readonly-fields.
    /// </summary>
    //TODO this class needs fleshed out
    public struct Int256 {

        public Int256( Int64 bits0, Int64 bits1, Int64 bits2, Int64 bits3 ) {
            this.Bits0 = bits0;
            this.Bits1 = bits1;
            this.Bits2 = bits2;
            this.Bits3 = bits3;
        }

        public Int64 Bits0 {
            get;
        }

        public Int64 Bits1 {
            get;
        }

        public Int64 Bits2 {
            get;
        }

        public Int64 Bits3 {
            get;
        }
    }
}