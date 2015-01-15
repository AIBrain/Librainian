namespace Librainian.Internet.Browser {
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Awesomium.Windows.Forms;
    using JetBrains.Annotations;
    using Threading;

    /// <summary>
    /// Semaphore wrapper for the <see cref="Awesomium"/> browser control ( <see cref="WebControl"/>).
    /// </summary>
    public class AwesomiumWrapper {
        [ NotNull ] private readonly SemaphoreSlim _semaphore = new SemaphoreSlim( initialCount: 0, maxCount: 1 );

        /// <summary>
        /// </summary>
        /// <param name="webControl"></param>
        /// <param name="timeout"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AwesomiumWrapper( [NotNull] WebControl webControl, TimeSpan timeout ) {
            if ( webControl == null ) {
                throw new ArgumentNullException( "webControl" );
            }
            this.WebControl = webControl;
            this.Timeout = timeout;
            //this.WaitAsync( timeout: Seconds.One ).Wait();
            //this.Release();
        }

        /// <summary>
        /// Private ctor to prevent the wrapper from being used without a <see cref="WebControl"/>.
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        private AwesomiumWrapper() {
        }

        /// <summary>
        /// The control we want to control access to.
        /// </summary>
        [NotNull]
        public WebControl WebControl { get; }

        public TimeSpan Timeout { get; }

        /// <summary>
        /// </summary>
        /// <param name="javascript"></param>
        /// <param name="retries"></param>
        /// <returns></returns>
        public async Task<bool> ExecuteJavascript( String javascript, int retries = 1 ) {
            var doWeHaveAccess = false;
            while ( retries > 0 ) {
                retries--;
                try {
                    if ( retries > 0 ) {
                        doWeHaveAccess = await this.WaitAsync( Timeout );
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

        /// <summary>
        /// Run a function in javascript
        /// </summary>
        /// <param name="id"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public async Task<bool> ExecuteJavascript( String id, String function ) {
            var javascript = String.Format( "document.getElementById( {0} ).{1}();", id, function );
            var result = await this.ExecuteJavascript( javascript );
            return result;
        }

        public async Task<TResult> ExecuteJavascriptWithResult<TResult>( String javascript, int retries = 1 ) {
            var doWeHaveAccess = false;
            while ( retries > 0 ) {
                retries--;
                try {
                    if ( retries > 0 ) {
                        doWeHaveAccess = await this.WaitAsync( Timeout );
                        if ( doWeHaveAccess ) {
                            var result = this.WebControl.Invoke( new Action( () => this.WebControl.ExecuteJavascriptWithResult( javascript ) ) );
                            return result is TResult ? ( TResult )result : default(TResult);
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
            return default(TResult);
        }

        public async Task<bool> ClickSubmit( uint index ) {
            var javascript = String.Format( "document.querySelectorAll(\"button[type='submit']\")[{0}].click();", index );
            return await this.ExecuteJavascript( javascript );
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CanBeNull]
        public async Task<string> GetValue( [NotNull] String id ) {
            if ( String.IsNullOrWhiteSpace( id ) ) {
                throw new ArgumentNullException( "id" );
            }
            var javascript = String.Format( "document.getElementById( {0} ).value", id );
            var result = await this.ExecuteJavascriptWithResult<String>( javascript );
            return result;
        }

        public async Task<bool> SetValue( String id, String text ) {
            var javascript = String.Format( "document.getElementById( {0} ).value=\"{1}\";", id, text );
            var result = await this.ExecuteJavascript( javascript );
            return result;
        }

        private void Release() => this._semaphore.Release();

        private async Task<bool> WaitAsync( TimeSpan timeout ) => await this._semaphore.WaitAsync( timeout );
    }
}