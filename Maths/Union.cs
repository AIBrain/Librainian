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
// "Librainian/Union.cs" was last cleaned by Protiguous on 2018/02/21 at 12:05 AM

namespace Librainian.Maths {
    using System;
    using System.Runtime.InteropServices;

    [StructLayout( layoutKind: LayoutKind.Explicit )]
    public struct Union {
        public Union( UInt64 value ) {
            this.High = default;
            this.Low = default;
            this.Value = value;
        }

        public Union( UInt32 high, UInt32 low ) {
            this.Value = 0;
            this.High = high;
            this.Low = low;
        }

        [FieldOffset( offset: 0 )]
        public readonly UInt64 Value;

        [FieldOffset( offset: 0 )]
        public readonly UInt32 High;

        [FieldOffset( offset: 4 )]
        public readonly UInt32 Low;
    }
}