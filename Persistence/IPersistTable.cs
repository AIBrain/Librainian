namespace Librainian.Persistence {
    using System;
    using System.Runtime.Serialization;
    using Annotations;
    using IO;
    using Microsoft.Isam.Esent.Collections.Generic;
    using Ninject;

    public interface IPersistTable< in TKey, TValue> : IInitializable, /*IDictionary<TKey,TValue>,*/ IDisposable
        where TKey : IComparable
        where TValue : class {

        [NotNull]
        Folder Folder {
            get;
        }

        /// <summary>
        ///     <para>
        ///         Here is where we interject <see cref="NetDataContractSerializer"/> to serialize to and from a <see cref="String"/> so the
        ///         <see cref="PersistentDictionary{TKey,TValue}"/> has no trouble with it.
        ///     </para>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [CanBeNull]
        new TValue this[ TKey key ] {
            get;
            set;
        }
    }
}