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
// "Librainian/Match.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Parsing {
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class Match< T, TResult > {
        public static Func< T, TResult > On< TCxt >( Func< OpenMatchContext< T, TResult >, TCxt > cond1, Func< TCxt, ClosedMatchContext > cond2 ) where TCxt : MatchContext< T, TResult > {
            var ctx = cond2( cond1( new ContextImpl() ) );
            return ( ( ContextImpl ) ctx ).Compile();
        }

        public static Func< T, TResult > On< TCtx1, TCtx2 >( Func< OpenMatchContext< T, TResult >, TCtx1 > cond1, Func< TCtx1, TCtx2 > cond2, Func< TCtx2, ClosedMatchContext > cond3 ) where TCtx1 : MatchContext< T, TResult > where TCtx2 : MatchContext< T, TResult > {
            var ctx = cond3( cond2( cond1( new ContextImpl() ) ) );
            return ( ( ContextImpl ) ctx ).Compile();
        }

        public static Func< T, TResult > On< TCtx1, TCtx2, TCtx3 >( Func< OpenMatchContext< T, TResult >, TCtx1 > cond1, Func< TCtx1, TCtx2 > cond2, Func< TCtx2, TCtx3 > cond3, Func< TCtx3, ClosedMatchContext > cond4 ) where TCtx1 : MatchContext< T, TResult > where TCtx2 : MatchContext< T, TResult > where TCtx3 : MatchContext< T, TResult > {
            var ctx = cond4( cond3( cond2( cond1( new ContextImpl() ) ) ) );
            return ( ( ContextImpl ) ctx ).Compile();
        }

        public static Func< T, TResult > On< TCtx1, TCtx2, TCtx3, TCtx4 >( Func< OpenMatchContext< T, TResult >, TCtx1 > cond1, Func< TCtx1, TCtx2 > cond2, Func< TCtx2, TCtx3 > cond3, Func< TCtx3, TCtx4 > cond4, Func< TCtx4, ClosedMatchContext > cond5 ) where TCtx1 : MatchContext< T, TResult > where TCtx2 : MatchContext< T, TResult > where TCtx3 : MatchContext< T, TResult > where TCtx4 : MatchContext< T, TResult > {
            var ctx = cond5( cond4( cond3( cond2( cond1( new ContextImpl() ) ) ) ) );
            return ( ( ContextImpl ) ctx ).Compile();
        }

        private sealed class ContextImpl : OpenMatchContext< T, TResult > {
            private readonly ReadOnlyCollection< MatchExpression > _matches;

            public ContextImpl() {
                this._matches = Enumerable.Empty< MatchExpression >().ToList().AsReadOnly();
            }

            public ContextImpl( ContextImpl baseContext, MatchExpression newExpr ) {
                this._matches = baseContext._matches.ConcatSingle( newExpr ).ToList().AsReadOnly();
            }

            public Func< T, TResult > Compile() {
                return value => this._matches.First( expr => expr.Matches( value ) ).Evaluate( value );
            }

            public override OpenMatchContext< T, TResult > Guard( Func< T, Boolean > failWhen, Func< T, TResult > failWith ) => new ContextImpl( this, new MatchExpression( failWhen, failWith ) );

            public override ClosedMatchContext Return( TResult result ) {
                return new ContextImpl( this, new MatchExpression( t => true, t => result ) );
            }

            public override ClosedMatchContext Return( Func< T, TResult > resultProjection ) {
                return new ContextImpl( this, new MatchExpression( t => true, resultProjection ) );
            }

            public override IntermediateMatchResultContext< T, TResult > When( Func< T, Boolean > condition ) => new IntermediateContextImpl( this, condition );
        }

        private sealed class IntermediateContextImpl : IntermediateMatchResultContext< T, TResult > {
            private readonly ContextImpl _baseContext;

            private readonly Func< T, Boolean > _condition;

            public IntermediateContextImpl( ContextImpl baseContext, Func< T, Boolean > condition ) {
                this._baseContext = baseContext;
                this._condition = condition;
            }

            public override MatchContext< T, TResult > Return( TResult result ) {
                return new ContextImpl( this._baseContext, new MatchExpression( this._condition, t => result ) );
            }

            public override MatchContext< T, TResult > Return( Func< T, TResult > resultProjection ) => new ContextImpl( this._baseContext, new MatchExpression( this._condition, resultProjection ) );
        }

        private sealed class MatchExpression {
            private readonly Func< T, TResult > _getResult;

            private readonly Func< T, Boolean > _isMatch;

            public MatchExpression( Func< T, Boolean > isMatch, Func< T, TResult > getResult ) {
                this._isMatch = isMatch;
                this._getResult = getResult;
            }

            public TResult Evaluate( T value ) => this._getResult( value );

            public Boolean Matches( T value ) => this._isMatch( value );
        }
    }
}
