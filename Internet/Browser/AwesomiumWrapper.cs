namespace Librainian.Internet.Browser {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Awesomium.Core;
    using Awesomium.Windows.Forms;
    using CsQuery;
    using JetBrains.Annotations;
    using Measurement.Time;
    using Threading;

    /// <summary>
    /// Semaphore wrapper for the <see cref="Awesomium"/> browser control ( <see cref="WebControl"/>).
    /// </summary>
    public class AwesomiumWrapper {
        [NotNull]
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim( initialCount: 0, maxCount: 1 );

        /// <summary>
        /// </summary>
        /// <param name="webControl"></param>
        /// <param name="timeout">How long to retry the commands.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AwesomiumWrapper( [NotNull] WebControl webControl, TimeSpan timeout ) {
            if ( webControl == null ) {
                throw new ArgumentNullException( "webControl" );
            }
            this.WebControl = webControl;
            this.Timeout = timeout;
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
        /// Access is gated by a semaphore.
        /// </summary>
        /// <param name="javascript">The javascript to execute</param>
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
	                        //this.WebControl.ExecuteJavascript( javascript );
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
        public async Task<Boolean> ExecuteJavascript( String id, String function ) {
            var javascript = String.Format( "document.getElementById( {0} ).{1}();", id, function );
            var result = await this.ExecuteJavascript( javascript );
            return result;
        }

        /// <summary>
        /// Access is gated by a semaphore.
        /// </summary>
        /// <param name="javascript"></param>
        /// <param name="retries"></param>
        /// <returns></returns>
        public async Task<JSValue> ExecuteJavascriptWithResult( String javascript, int retries = 1 ) {
            var doWeHaveAccess = false;
            while ( retries > 0 ) {
                retries--;
                try {
                    if ( retries > 0 ) {
                        doWeHaveAccess = await this.WaitAsync( Timeout );
                        if ( doWeHaveAccess ) {
                            var result = ( JSValue )this.WebControl.Invoke( new Func<JSValue>( () => this.WebControl.ExecuteJavascriptWithResult( javascript ) ) );
                            //var result = this.WebControl.ExecuteJavascriptWithResult( javascript ) ;
                            return result;
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
            return default(JSValue);
        }

        public async Task<bool> ClickSubmit( uint index ) {
            var javascript = String.Format( "document.querySelectorAll(\"button[type='submit']\")[{0}].click();", index );
            return await this.ExecuteJavascript( javascript );
        }

        /// <summary>
        /// <para>Retrieve the <see cref="Uri" /> of the <see cref="WebControl"/>.</para>
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public async Task<Uri> GetBrowserLocation() {
            var doWeHaveAccess = false;
            try {
                doWeHaveAccess = await this.WaitAsync( this.Timeout );
                if ( doWeHaveAccess ) {
                    var result = ( Uri )this.WebControl.Invoke( new Func<Uri>( () => this.WebControl.Source ) );
                    return result;
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

            return new Uri( "about:blank" );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public async Task<string> InnerHTML() {
            var doWeHaveAccess = false;
            try {
                doWeHaveAccess = await this.WaitAsync( this.Timeout );
                if ( doWeHaveAccess ) {
                    var result = this.WebControl.Invoke( new Func<string>( () => this.WebControl.ExecuteJavascriptWithResult( "document.getElementsByTagName('html')[0].innerHTML" ) ) );

                    if ( result is String ) {
                        return result as String;
                    }
                    return result.ToString();
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
            return String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public async Task<string> InnerText() {
            var doWeHaveAccess = false;
            try {
                doWeHaveAccess = await this.WaitAsync( this.Timeout );
                if ( doWeHaveAccess ) {
                    var result = this.WebControl.Invoke( new Func<string>( () => this.WebControl.ExecuteJavascriptWithResult( "document.getElementsByTagName('html')[0].innerText" ) ) );

                    if ( result is String ) {
                        return result as String;
                    }
                    return result.ToString();
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
            return String.Empty;
        }

        private static async void Throttle( TimeSpan? until = null ) {
            //TODO look into that semaphore waitgate thing...
            if ( !until.HasValue ) {
                until = Seconds.One;
            }
            var watch = Stopwatch.StartNew();
            do {
                Application.DoEvents();
                await Task.Delay( Milliseconds.Hertz111 );
                if ( watch.Elapsed >= until.Value ) {
                    break;
                }
            } while ( watch.Elapsed < until.Value );
            watch.Stop();
        }


        public async Task<Boolean> Navigate( [NotNull] Uri url, [CanBeNull] SimpleCancel simpleCancel = null ) {
            if ( url == null ) {
                throw new ArgumentNullException( "url" );
            }

            var doWeHaveAccess = false;
            try {
                doWeHaveAccess = await this.WaitAsync( this.Timeout );
                if ( !doWeHaveAccess ) {
                    return false;
                }

                var watchdog = Stopwatch.StartNew();

                var result = this.WebControl.Invoke( method: new Action( () => {
                    Log.Info( String.Format( "Navigating to {0}...", url ) );

                    this.WebControl.Source = url;

                    do {
                        //WebCore.Update(); //trust in WebCore.Run?
                        Throttle();

                        if ( simpleCancel != null && simpleCancel.IsCancellationRequested ) {
                            break;
                        }

                        if ( watchdog.Elapsed >= this.Timeout ) {
                            Log.Info( "*navigation^timed->out*" );
                            break;
                        }
                    } while ( this.WebControl.IsLoading || this.WebControl.IsNavigating );

                    Log.Info( "done navigating." );
                } ) );
                return WebControl.IsDocumentReady && WebControl.IsResponsive;
            }
            catch ( Exception exception ) {
                Debug.WriteLine( exception.Message );
            }
            finally {
                if ( doWeHaveAccess ) {
                    this.Release();
                }
            }
            return false;
        }


        /// <summary>
        /// Return all anchers' href.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Uri> GetAllAnchers() {
            var html = this.InnerHTML().Result;

            var cq = new CQ( html: html, parsingMode: HtmlParsingMode.Auto, parsingOptions: HtmlParsingOptions.AllowSelfClosingTags, docType: DocType.HTML5 );

            var anchors = cq[ "a" ].ToList();

            Uri uri = null;

            return from href in anchors.Select( domObject => domObject[ "href" ] )
                   where Uri.TryCreate( href, UriKind.Absolute, out uri )
                   select uri;
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
            var result = await this.ExecuteJavascriptWithResult( javascript );
            return result;
        }

        public async Task<bool> SetValue( String id, String text ) {
            var javascript = String.Format( "document.getElementById( {0} ).value=\"{1}\";", id, text );
            var result = await this.ExecuteJavascript( javascript );
            return result;
        }

        //public Task JsFireEvent( string getElementQuery, string eventName ) {
        //    var browser = this.WebBrowser1;
        //    if ( browser != null ) {
        //        browser.ExecuteJavascript( string.Format( @"
        //                    function fireEvent(element,event) {{
        //                        var evt = document.createEvent('HTMLEvents');
        //                        evt.initEvent(event, true, true ); // event type,bubbling,cancelable
        //                        element.dispatchEvent(evt);
        //                    }}
        //                    {0}", String.Format( "fireEvent({0}, '{1}');", getElementQuery, eventName ) ) );
        //    }
        //}


        private void Release() => this._semaphore.Release();

        private async Task<bool> WaitAsync( TimeSpan timeout ) => await this._semaphore.WaitAsync( timeout );
    }
}