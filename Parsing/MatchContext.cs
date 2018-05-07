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
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/MatchContext.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

namespace Librainian.Parsing {

    using System;

    public abstract class MatchContext<T, TResult> : ClosedMatchContext {

        public abstract ClosedMatchContext Return( TResult result );

        public abstract ClosedMatchContext Return( Func<T, TResult> resultProjection );

        public abstract IntermediateMatchResultContext<T, TResult> When( Func<T, Boolean> condition );
    }
}