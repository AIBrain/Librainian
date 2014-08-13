namespace Librainian.Collections {
    using System;
    using System.Collections.Concurrent;

    public enum ShufflingType {

        /// <summary>
        /// Uses OrderBy( Random.Next ).ThenBy( Random.Next ). This might be the fastest.
        /// </summary>
        Random,

        /// <summary>
        /// This is the one I understand the best.
        /// </summary>
        HarkerShuffle,

        /// <summary>
        /// Possibly slower. But uses <see cref="ConcurrentBag{T}"/> (which can introduce more randomness).
        /// </summary>
        ByBuckets,

        /// <summary>
        /// Uses <see cref="Guid.NewGuid"/> to introduce randomness.
        /// </summary>
        ByGuid,
    }
}