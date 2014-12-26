
namespace Librainian.Internet.Browser {
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Awesomium.Windows.Forms;
    using JetBrains.Annotations;
    using Measurement.Time;
    using Threading;

    public class AwesomiumWrapper {

        /// <summary>
        /// The control we want to control access to.
        /// </summary>
        [NotNull]
        public WebControl WebControl { get; }

        public AwesomiumWrapper( [NotNull] WebControl webControl ) {
            if ( webControl == null ) {
                throw new ArgumentNullException( "webControl" );
            }
            WebControl = webControl;
            WaitAsync( Seconds.One ).Wait();
            Release();
        }

        private async Task<Boolean> WaitAsync( TimeSpan timeout ) => await this._semaphore.WaitAsync( timeout );

        private void Release() => this._semaphore.Release();

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim( initialCount: 0, maxCount: 1 );

        public async Task<Boolean> ExecuteJavascript( String javascript, TimeSpan timeout, int retries = 1 ) {
            var doWeHaveAccess = false;
            while ( retries > 0 ) {
                retries--;
                try {
                    if ( retries > 0 ) {
                        doWeHaveAccess = await this.WaitAsync( timeout );
                        if ( doWeHaveAccess ) {
                            this.WebControl.Invoke( new Action( () => this.WebControl.ExecuteJavascript( javascript ) ) );
                            return true;
                        }
                    }
                }
                catch ( Exception exception ) {
                    exception.More();
                }
                finally {
                    if ( doWeHaveAccess ) {
                        this.Release();
                    }
                }
            }
            return false;
        }

        public async Task<TResult> ExecuteJavascriptWithResult<TResult>( String javascript, TimeSpan timeout, int retries = 1 ) {
            var doWeHaveAccess = false;
            while ( retries > 0 ) {
                retries--;
                try {
                    if ( retries > 0 ) {
                        doWeHaveAccess = await this.WaitAsync( timeout );
                        if ( doWeHaveAccess ) {
                            var result = this.WebControl.Invoke( new Action( () => this.WebControl.ExecuteJavascriptWithResult( javascript ) ) ) ;
                            return result is TResult ? ( TResult ) result : default( TResult );
                        }
                    }
                }
                catch ( Exception exception ) {
                    exception.More();
                }
                finally {
                    if ( doWeHaveAccess ) {
                        this.Release();
                    }
                }
            }
            return default( TResult );
        }

        [CanBeNull]
        public async Task<Boolean> ExecuteJavascriptMethod( String id, String function, TimeSpan timeout ) {
            var javascript = String.Format( "document.getElementById( {0} ).{1}();", id, function );
            var result = await this.ExecuteJavascript( javascript, timeout );
            return result;
        }

        public async Task<Boolean> SetValue( String id, String text, TimeSpan timeout ) {
            var javascript = String.Format( "document.getElementById( {0} ).value=\"{1}\";", id, text );
            var result = await this.ExecuteJavascript( javascript, timeout );
            return result;
        }


    }
}
