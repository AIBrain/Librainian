// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/GameContainer.cs" was last cleaned by Rick on 2014/12/09 at 6:06 AM

namespace Librainian.Gaming {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Annotations;

    public class GameContainer : IGameContainer {

        public GameContainer() {
            this.Contents = new ConcurrentBag<IGameItem>();
        }

        public ConcurrentBag<IGameItem> Contents { get; }

        /// <summary>
        /// Add one game item
        /// </summary>
        /// <param name="item"></param>
        public void Add( IGameItem item ) {
            if ( default(IGameItem) != item ) {
                this.Contents.Add( item );
            }
        }

        /// <summary>
        /// Public list to bag of <see cref="IGameItem"/>.
        /// </summary>
        [NotNull]
        public IEnumerable<IGameItem> GetContents() => this.Contents;

        public Boolean MoveAll( IGameContainer destination ) {
            var diceMoved = 0UL;
            while ( this.Contents.Any() ) {
                if ( this.MoveOne( destination ) ) {
                    diceMoved++;
                }
            }

            //IGameItem gameItem;
            //while ( this.Contents.TryTake( out gameItem ) ) {
            //    var dice = gameItem as Dice;
            //    if ( dice != null ) {
            //        dice.Roll();
            //    }
            //    destination.Add( gameItem );

            //}
            return diceMoved > 0;
        }

        public Boolean MoveOne( IGameContainer destination ) {
            IGameItem gameItem;
            if ( !this.TryTake( out gameItem ) ) {
                return false;
            }
            var dice = gameItem as Dice;
            dice?.Roll();
            destination.Add( gameItem );
            return true;
        }

        /// <summary>
        /// Try to take one game item.
        /// </summary>
        /// <param name="item"></param>
        public Boolean TryTake( out IGameItem item ) => this.Contents.TryTake( out item );

        //public void MoveAll<TPieceType>( IGameContainer destination, Action<TPieceType> onEachItem = null ) where TPieceType : class {
        //    var localDump = new List<IGameItem>();
        //    while ( Contents.Cast<TPieceType>().Any() ) {
        //        IGameItem item;
        //        if ( !this.TryTake( out item ) ) {
        //            continue;
        //        }
        //        if ( item is TPieceType ) {
        //            destination.Add( item );
        //            if ( onEachItem != null ) {
        //                onEachItem(item as TPieceType);
        //            }
        //        }
        //        else {
        //            localDump.Add( item );
        //        }
        //    }
        //    foreach ( var gameItem in localDump ) {
        //        Contents.Add( gameItem );
        //    }
        //}
    }
}