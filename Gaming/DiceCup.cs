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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/DiceCup.cs" was last cleaned by Rick on 2014/08/14 at 12:35 AM

#endregion License & Information

namespace Librainian.Gaming {
    using System;
    using System.Collections.Concurrent;
    using Annotations;

    public class DiceCup : IGameContainer {
        private readonly ConcurrentBag<IDice> _contents = new ConcurrentBag<IDice>();

        public void Add( [NotNull] IDice dice ) {
            if ( dice == null ) {
                throw new ArgumentNullException( "dice" );
            }

            this._contents.Add( dice );
        }

        public void PourDice( IPlayerTable table ) {
            while ( !table._contents.IsEmpty ) {
                //trytake me
                //tryadd table
            }
        }
    }

    public interface IGameContainer {

        void Add( IDice dice );
    }

    public abstract class IPlayerTable : IGameContainer {
        internal readonly ConcurrentBag<IDice> _contents = new ConcurrentBag<IDice>();

        public abstract void Add( IDice dice );

        public abstract Boolean MoveOneDice( IGameContainer destination );
    }

    public class PlayerTable : IPlayerTable {

        public override void Add( [NotNull] IDice dice ) {
            if ( dice == null ) {
                throw new ArgumentNullException( "dice" );
            }

            this._contents.Add( dice );
        }

        public override Boolean MoveOneDice( IGameContainer destination ) {
            var me = this;
            var source = me;

            if ( source._contents.IsEmpty ) { return false; }

            return false;
            
        }
    }
}