using System;

namespace Librainian.Magic {

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="http://stackoverflow.com/questions/898828/finalize-dispose-pattern-in-c-sharp"/>
    public class BetterDisposableClass : IDisposable {

        public void Dispose() {
            try {
                try {
                    this.CleanUpManagedResources();
                }
                finally {
                    this.CleanUpNativeResources();
                }
            }
            finally {
                GC.SuppressFinalize( this );
            }
        }

        protected virtual void CleanUpManagedResources() {
            // ...
        }
        protected virtual void CleanUpNativeResources() {
            // ...
        }

        ~BetterDisposableClass() {
            this.CleanUpNativeResources();
        }

    }
}
