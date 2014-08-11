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
// "Librainian/MatchContextExtensions.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Parsing {
    using System;
    using System.Collections.Generic;

    public static class MatchContextExtensions {
        public static IntermediateMatchResultContext< T, TResult > When< T, TResult >( this MatchContext< T, TResult > ctx, T value ) where T : IEquatable< T > {
            var comp = EqualityComparer< T >.Default;
            return ctx.When( t => comp.Equals( t, value ) );
        }

        public static IntermediateMatchResultContext< T, TResult > When< T, TResult >( this MatchContext< T, TResult > ctx, T value1, T value2 ) where T : IEquatable< T > {
            var comp = EqualityComparer< T >.Default;
            return ctx.When( t => comp.Equals( t, value1 ) || comp.Equals( t, value2 ) );
        }
    }
}
