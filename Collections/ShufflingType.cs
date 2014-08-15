namespace Librainian.Collections {
    using System;
    using System.Collections.Concurrent;

    public enum ShufflingType {

        /// <summary>
        /// Uses OrderBy( Random.Next ).ThenBy( Random.Next ). This might be the fastest.
        /// </summary>
        ByRandom,

        /// <summary>
        /// This is the one I understand the best.
        /// </summary>
        ByHarker,

        /// <summary>
        /// Uses <see cref="ConcurrentBag{T}"/>, which can introduce /some/ randomness, but is horribly nondeterministic and unrandom.
        /// This method is so horrible, that please: Do NOT use it. If you *have* to, do many many iterations.
        /// </summary>
        ByBags,

        /// <summary>
        /// Uses <see cref="Guid.NewGuid"/> to introduce randomness.
        /// </summary>
        ByGuid,
    }
}