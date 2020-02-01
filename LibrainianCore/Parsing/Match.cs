// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Match.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Match.cs" was last formatted by Protiguous on 2020/01/31 at 12:29 AM.

namespace LibrainianCore.Parsing {

    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using JetBrains.Annotations;

    public static class Match<T, TResult> {

        [NotNull]
        public static Func<T, TResult> On<TCxt>( [NotNull] Func<OpenMatchContext<T, TResult>, TCxt> cond1, [NotNull] Func<TCxt, ClosedMatchContext> cond2 )
            where TCxt : MatchContext<T, TResult> {
            var ctx = cond2( cond1( new ContextImpl() ) );

            return ( ( ContextImpl )ctx ).Compile();
        }

        [NotNull]
        public static Func<T, TResult> On<TCtx1, TCtx2>( [NotNull] Func<OpenMatchContext<T, TResult>, TCtx1> cond1, [NotNull] Func<TCtx1, TCtx2> cond2,
            [NotNull] Func<TCtx2, ClosedMatchContext> cond3 ) where TCtx1 : MatchContext<T, TResult> where TCtx2 : MatchContext<T, TResult> {
            var ctx = cond3( cond2( cond1( new ContextImpl() ) ) );

            return ( ( ContextImpl )ctx ).Compile();
        }

        [NotNull]
        public static Func<T, TResult> On<TCtx1, TCtx2, TCtx3>( [NotNull] Func<OpenMatchContext<T, TResult>, TCtx1> cond1, [NotNull] Func<TCtx1, TCtx2> cond2,
            [NotNull] Func<TCtx2, TCtx3> cond3, [NotNull] Func<TCtx3, ClosedMatchContext> cond4 ) where TCtx1 : MatchContext<T, TResult>
            where TCtx2 : MatchContext<T, TResult>
            where TCtx3 : MatchContext<T, TResult> {
            var ctx = cond4( cond3( cond2( cond1( new ContextImpl() ) ) ) );

            return ( ( ContextImpl )ctx ).Compile();
        }

        [NotNull]
        public static Func<T, TResult> On<TCtx1, TCtx2, TCtx3, TCtx4>( [NotNull] Func<OpenMatchContext<T, TResult>, TCtx1> cond1, [NotNull] Func<TCtx1, TCtx2> cond2,
            [NotNull] Func<TCtx2, TCtx3> cond3, [NotNull] Func<TCtx3, TCtx4> cond4, [NotNull] Func<TCtx4, ClosedMatchContext> cond5 ) where TCtx1 : MatchContext<T, TResult>
            where TCtx2 : MatchContext<T, TResult>
            where TCtx3 : MatchContext<T, TResult>
            where TCtx4 : MatchContext<T, TResult> {
            var ctx = cond5( cond4( cond3( cond2( cond1( new ContextImpl() ) ) ) ) );

            return ( ( ContextImpl )ctx ).Compile();
        }

        private sealed class ContextImpl : OpenMatchContext<T, TResult> {

            private readonly ReadOnlyCollection<MatchExpression> _matches;

            public ContextImpl() => this._matches = Enumerable.Empty<MatchExpression>().ToList().AsReadOnly();

            public ContextImpl( [NotNull] ContextImpl baseContext, [CanBeNull] MatchExpression newExpr ) =>
                this._matches = baseContext._matches.ConcatSingle( newExpr ).ToList().AsReadOnly();

            [NotNull]
            public Func<T, TResult> Compile() => value => this._matches.First( expr => expr.Matches( value ) ).Evaluate( value );

            [NotNull]
            public override OpenMatchContext<T, TResult> Guard( [CanBeNull] Func<T, Boolean> failWhen, [CanBeNull] Func<T, TResult> failWith ) =>
                new ContextImpl( this, new MatchExpression( failWhen, failWith ) );

            [NotNull]
            public override ClosedMatchContext Return( [CanBeNull] TResult result ) => new ContextImpl( this, new MatchExpression( t => true, t => result ) );

            [NotNull]
            public override ClosedMatchContext Return( [CanBeNull] Func<T, TResult> resultProjection ) =>
                new ContextImpl( this, new MatchExpression( t => true, resultProjection ) );

            [NotNull]
            public override IntermediateMatchResultContext<T, TResult> When( [CanBeNull] Func<T, Boolean> condition ) => new IntermediateContextImpl( this, condition );
        }

        private sealed class IntermediateContextImpl : IntermediateMatchResultContext<T, TResult> {

            private readonly ContextImpl _baseContext;

            private readonly Func<T, Boolean> _condition;

            public IntermediateContextImpl( [CanBeNull] ContextImpl baseContext, [CanBeNull] Func<T, Boolean> condition ) {
                this._baseContext = baseContext;
                this._condition = condition;
            }

            [NotNull]
            public override MatchContext<T, TResult> Return( [CanBeNull] TResult result ) =>
                new ContextImpl( this._baseContext, new MatchExpression( this._condition, t => result ) );

            [NotNull]
            public override MatchContext<T, TResult> Return( [CanBeNull] Func<T, TResult> resultProjection ) =>
                new ContextImpl( this._baseContext, new MatchExpression( this._condition, resultProjection ) );
        }

        private sealed class MatchExpression {

            private readonly Func<T, TResult> _getResult;

            private readonly Func<T, Boolean> _isMatch;

            public MatchExpression( [CanBeNull] Func<T, Boolean> isMatch, [CanBeNull] Func<T, TResult> getResult ) {
                this._isMatch = isMatch;
                this._getResult = getResult;
            }

            [CanBeNull]
            public TResult Evaluate( [CanBeNull] T value ) => this._getResult( value );

            public Boolean Matches( [CanBeNull] T value ) => this._isMatch( value );
        }
    }
}