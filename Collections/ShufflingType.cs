namespace Librainian.Collections {
    using System;
    using System.Collections.Concurrent;

    public enum ShufflingType {

        /// <summary>
        /// This one is works best with smaller lists and more iterations.
        /// </summary>
        ByHarker,

        /// <summary>
        /// Uses OrderBy( Random.Next ).ThenBy( Random.Next ). This is _might_ be the fastest for larger sets.
        /// </summary>
        ByRandom,

        /// <summary>
        /// Uses <see cref="ConcurrentBag{T}"/>, which can introduce /some/ randomness, but is horribly nondeterministic and unrandom.
        /// This method is so horrible, that please: Do NOT use it. If you *have* to, do many many iterations.
        /// </summary>
        ByBags,

        /// <summary>
        /// Uses <see cref="Guid.NewGuid"/> to introduce randomness.
        /// </summary>
        ByGuid,

        /// <summary>
        /// Let the algorithm choose the optimal.
        /// </summary>
        AutoChoice,
    }
}