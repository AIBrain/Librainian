namespace Librainian.Magic {
    using System;

    public abstract class ManagedDisposable : IDisposable {

        /// <summary>
        /// dispose of managed resources
        /// </summary>
        public abstract void Dispose();
    }
}