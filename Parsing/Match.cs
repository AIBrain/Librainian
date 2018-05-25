// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Match.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Match.cs" was last formatted by Protiguous on 2018/05/24 at 7:30 PM.

namespace Librainian.Parsing {

    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class Match<T, TResult> {

        public static Func<T, TResult> On<TCxt>( Func<OpenMatchContext<T, TResult>, TCxt> cond1, Func<TCxt, ClosedMatchContext> cond2 ) where TCxt : MatchContext<T, TResult> {
            var ctx = cond2( cond1( new ContextImpl() ) );

            return ( ( ContextImpl )ctx ).Compile();
        }

        public static Func<T, TResult> On<TCtx1, TCtx2>( Func<OpenMatchContext<T, TResult>, TCtx1> cond1, Func<TCtx1, TCtx2> cond2, Func<TCtx2, ClosedMatchContext> cond3 )
            where TCtx1 : MatchContext<T, TResult> where TCtx2 : MatchContext<T, TResult> {
            var ctx = cond3( cond2( cond1( new ContextImpl() ) ) );

            return ( ( ContextImpl )ctx ).Compile();
        }

        public static Func<T, TResult> On<TCtx1, TCtx2, TCtx3>( Func<OpenMatchContext<T, TResult>, TCtx1> cond1, Func<TCtx1, TCtx2> cond2, Func<TCtx2, TCtx3> cond3, Func<TCtx3, ClosedMatchContext> cond4 )
            where TCtx1 : MatchContext<T, TResult> where TCtx2 : MatchContext<T, TResult> where TCtx3 : MatchContext<T, TResult> {
            var ctx = cond4( cond3( cond2( cond1( new ContextImpl() ) ) ) );

            return ( ( ContextImpl )ctx ).Compile();
        }

        public static Func<T, TResult> On<TCtx1, TCtx2, TCtx3, TCtx4>( Func<OpenMatchContext<T, TResult>, TCtx1> cond1, Func<TCtx1, TCtx2> cond2, Func<TCtx2, TCtx3> cond3, Func<TCtx3, TCtx4> cond4,
            Func<TCtx4, ClosedMatchContext> cond5 ) where TCtx1 : MatchContext<T, TResult> where TCtx2 : MatchContext<T, TResult> where TCtx3 : MatchContext<T, TResult> where TCtx4 : MatchContext<T, TResult> {
            var ctx = cond5( cond4( cond3( cond2( cond1( new ContextImpl() ) ) ) ) );

            return ( ( ContextImpl )ctx ).Compile();
        }

        private sealed class ContextImpl : OpenMatchContext<T, TResult> {

            private readonly ReadOnlyCollection<MatchExpression> _matches;

            public ContextImpl() => this._matches = Enumerable.Empty<MatchExpression>().ToList().AsReadOnly();

            public ContextImpl( ContextImpl baseContext, MatchExpression newExpr ) => this._matches = baseContext._matches.ConcatSingle( newExpr ).ToList().AsReadOnly();

            public Func<T, TResult> Compile() => value => this._matches.First( expr => expr.Matches( value ) ).Evaluate( value );

            public override OpenMatchContext<T, TResult> Guard( Func<T, Boolean> failWhen, Func<T, TResult> failWith ) => new ContextImpl( this, new MatchExpression( failWhen, failWith ) );

            public override ClosedMatchContext Return( TResult result ) => new ContextImpl( this, new MatchExpression( t => true, t => result ) );

            public override ClosedMatchContext Return( Func<T, TResult> resultProjection ) => new ContextImpl( this, new MatchExpression( t => true, resultProjection ) );

            public override IntermediateMatchResultContext<T, TResult> When( Func<T, Boolean> condition ) => new IntermediateContextImpl( this, condition );
        }

        private sealed class IntermediateContextImpl : IntermediateMatchResultContext<T, TResult> {

            private readonly ContextImpl _baseContext;

            private readonly Func<T, Boolean> _condition;

            public IntermediateContextImpl( ContextImpl baseContext, Func<T, Boolean> condition ) {
                this._baseContext = baseContext;
                this._condition = condition;
            }

            public override MatchContext<T, TResult> Return( TResult result ) => new ContextImpl( this._baseContext, new MatchExpression( this._condition, t => result ) );

            public override MatchContext<T, TResult> Return( Func<T, TResult> resultProjection ) => new ContextImpl( this._baseContext, new MatchExpression( this._condition, resultProjection ) );
        }

        private sealed class MatchExpression {

            private readonly Func<T, TResult> _getResult;

            private readonly Func<T, Boolean> _isMatch;

            public MatchExpression( Func<T, Boolean> isMatch, Func<T, TResult> getResult ) {
                this._isMatch = isMatch;
                this._getResult = getResult;
            }

            public TResult Evaluate( T value ) => this._getResult( value );

            public Boolean Matches( T value ) => this._isMatch( value );
        }
    }
}