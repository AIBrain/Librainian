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
    using JetBrains.Annotations;

    public class Container : IContainer {

	    private ConcurrentBag<IGameItem> Contents { get; } = new ConcurrentBag<IGameItem>();

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

        public Boolean MoveAll( IContainer destination ) {
            var moved = 0ul;

			while ( this.Contents.Any() ) {
                if ( this.MoveOne( destination ) ) {
                    moved++;
                }
            }

            return moved > 0;
        }

		/// <summary>
		/// <para>Move one <see cref="IGameItem"/> from this <see cref="Container"/> to another <see cref="Container"/>(<paramref name="destination"/>).</para>
		/// <para>If the <see cref="IGameItem"/> is a <see cref="Dice"/>, then the <see cref="Dice"/> will be <see cref="Dice.Roll"/></para>
		/// </summary>
		/// <param name="destination"></param>
		/// <returns></returns>
		public Boolean MoveOne( [ NotNull ] IContainer destination ) {
			if ( destination == null ) {
				throw new ArgumentNullException( "destination" );
			}

			IGameItem gameItem;
            if ( !this.TryTake( out gameItem ) ) {
                return false;
            }

			( gameItem as Dice )?.Roll();

            destination.Add( gameItem );
            return true;
        }

        /// <summary>
        /// Try to take one game item.
        /// </summary>
        /// <param name="item"></param>
        public Boolean TryTake( out IGameItem item ) => this.Contents.TryTake( out item );


	}
}