namespace Librainian.Magic {
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="http://stackoverflow.com/questions/898828/finalize-dispose-pattern-in-c-sharp"/>
    public class NativeDisposable : IDisposable {

        public void Dispose() {
            try {
                this.CleanUpNativeResource();
            }
            finally {
                GC.SuppressFinalize( this );
            }
        }

        protected virtual void CleanUpNativeResource() {
            // ...
        }

        ~NativeDisposable() {
            this.CleanUpNativeResource();
        }

    }
}