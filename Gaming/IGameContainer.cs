﻿// This notice must be kept visible in the source.
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
// "Librainian/IGameContainer.cs" was last cleaned by Rick on 2014/12/09 at 6:06 AM

namespace Librainian.Gaming {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Annotations;

    public interface IGameContainer {

        [NotNull]

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        ConcurrentBag<IGameItem> Contents { get; }

        /// <summary>
        /// Add one game item
        /// </summary>
        /// <param name="item"></param>
        void Add( IGameItem item );

        /// <summary>
        /// Public list to bag of <see cref="IGameItem"/>.
        /// </summary>
        IEnumerable<IGameItem> GetContents();

        /// <summary>
        /// Give all items to the <see cref="destination"/>.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        Boolean MoveAll( IGameContainer destination );

        /// <summary>
        /// Take one item and give it to the <see cref="destination"/>.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        Boolean MoveOne( IGameContainer destination );

        /// <summary>
        /// Try to take one game item.
        /// </summary>
        /// <param name="item"></param>
        Boolean TryTake( out IGameItem item );
    }
}