
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

        private async Task< Boolean > WaitAsync( TimeSpan timeout ) => await this._semaphore.WaitAsync( timeout );

        private void Release() => this._semaphore.Release();

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim( initialCount: 0, maxCount: 1 );

        [ CanBeNull ]
        public async Task< object > RunFunction( String id, String function, TimeSpan timeout ) {
            try {
                var doWeHaveAccess = await WaitAsync( timeout );
                if ( !doWeHaveAccess ) {
                    return null;
                }
                var result = this.WebControl.Invoke( new Action( () => this.WebControl.ExecuteJavascript( String.Format( "document.getElementById( {0} ).{1}();", id, function ) ) ) );
                return result;
            }
            catch ( Exception exception ) {
                exception.More();
            }
            finally {
                this.Release();
            }
            return null;
        }

        public async Task< bool > Value( String id, String text ) {
            try {
                await WaitAsync();
                this.WebControl.Invoke( new Action( () => this.WebControl.ExecuteJavascript( String.Format( "document.getElementById( {0} ).value=\"{1}\";", id, text ) ) ) );
                return true;
            }
            catch ( Exception exception ) {
                exception.More();
            }
            finally {
                this.Release();
            }
            return false;
        }


    }
}
