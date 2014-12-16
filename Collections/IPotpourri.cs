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
// "Universe/IPotpourri.cs" was last cleaned by Rick on 2014/08/26 at 2:24 AM

#endregion License & Information

namespace Librainian.Collections {
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using JetBrains.Annotations;

    public interface IPotpourri<TKey> : IEnumerable<KeyValuePair<TKey, BigInteger>> {

        void Add( TKey key, BigInteger count );

        void Add( KeyValuePair<TKey, BigInteger> keyValuePair );

        void Clear();

        Boolean Contains( [CanBeNull] TKey key );

        BigInteger Count();

        BigInteger Count<TParticle>(  );

        /// <summary>
        /// Get all particles
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<TKey, BigInteger>> Get();

        /// <summary>
        /// Get all particles of type(<see cref="TParticle"/>).
        /// </summary>
        /// <typeparam name="TParticle"></typeparam>
        /// <returns></returns>
        IEnumerable<KeyValuePair<TParticle, BigInteger>> Get<TParticle>();

        Boolean Remove( TKey key, BigInteger count );

        Boolean RemoveAll( TKey key );

    }
}