namespace Librainian.Persistence {
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Annotations;
    using IO;

    public interface IPersistTable< TKey, TValue> :  IDictionary<TKey,TValue>, IDisposable
        where TKey : IComparable
         {

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