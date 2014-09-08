namespace Librainian.Gaming {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Annotations;

    public interface IGameContainer {

        [NotNull]

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        ConcurrentBag<IGameItem> Contents {
            get;
        }

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