#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/WithTime.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Collections {
    using System;
    using System.Runtime.Serialization;

    [DataContract( IsReference = true )]
    public class WithTime< T > {
        [DataMember]  public readonly T Item;

        [DataMember]  public readonly DateTime TimeStamp;

        public WithTime( T item ) {
            this.TimeStamp = DateTime.UtcNow;
            this.Item = item;
        }

        public override String ToString() => String.Format( "{0} @ {1:s}", this.Item, this.TimeStamp );
    }
}
