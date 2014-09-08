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
    using System.Collections.Generic;
    using System.Linq;
    using Annotations;

    public class GameContainer : IGameContainer {

        public ConcurrentBag<IGameItem> Contents {
            get;
            private set;
        }

        public GameContainer() {
            this.Contents = new ConcurrentBag<IGameItem>();
        }

        /// <summary>
        /// Public list to bag of <see cref="IGameItem"/>.
        /// </summary>
        [NotNull]
        public IEnumerable<IGameItem> GetContents() {
            return this.Contents;
        }

        /// <summary>
        /// Add one game item
        /// </summary>
        /// <param name="item"></param>
        public void Add( IGameItem item ) {
            if ( default( IGameItem ) != item ) {
                this.Contents.Add( item );
            }
        }


        /// <summary>
        /// Try to take one game item.
        /// </summary>
        /// <param name="item"></param>
        public Boolean TryTake( out IGameItem item ) {
            return this.Contents.TryTake( out item );
        }

        public Boolean MoveOne( IGameContainer destination ) {
            IGameItem item;
            if ( !this.TryTake( out item ) ) {
                return false;
            }
            destination.Add( item );
            return true;
        }

        public void MoveAll<TPieceType>( IGameContainer destination, Action onEachItem = null ) {
            var localDump = new List<IGameItem>();
            while ( Contents.Cast<TPieceType>().Any() ) {
                IGameItem item;
                if ( !this.TryTake( out item ) ) {
                    continue;
                }
                if ( item is TPieceType ) {
                    destination.Add( item );
                }
                else {
                    localDump.Add( item );
                }
            }
            foreach ( var gameItem in localDump ) {
                Contents.Add( gameItem );
            }
        }

        public Boolean MoveAll( IGameContainer destination ) {
            var diceMoved = 0UL;
            IGameItem dice;
            while ( this.Contents.TryTake( out dice ) ) {
                destination.Add( dice );
                diceMoved++;
            }
            return diceMoved > 0;
        }
    }
}