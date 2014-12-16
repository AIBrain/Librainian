namespace Librainian.Internet {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Awesomium.Windows.Forms;
    using CsQuery;
    using JetBrains.Annotations;
    using Threading;

    public static class InternetExtensions {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <returns></returns>
        public static IEnumerable<Uri> GetAllLinks( this WebControl webBrowser ) {
            var html = webBrowser.GetBrowserHTML();

            var cq = new CQ( html: html, parsingMode: HtmlParsingMode.Auto, parsingOptions: HtmlParsingOptions.AllowSelfClosingTags, docType: DocType.HTML5 );

            var anchors = cq[ "a" ].ToList();

            foreach ( var href in anchors.Select( domObject => domObject[ "href" ] ) ) {
                Uri uri;
                if ( !Uri.TryCreate( href, UriKind.Absolute, out uri ) ) {
                    continue;
                }
                yield return uri;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <returns></returns>
        [CanBeNull]
        public static String GetBrowserHTML( this WebControl webBrowser ) {
            try {
                if ( webBrowser != null ) {
                    var result = webBrowser.Invoke( new Func<string>( () => webBrowser.ExecuteJavascriptWithResult( "document.getElementsByTagName('html')[0].innerHTML" ) ) );

                    if ( result is String ) {
                        return result as String;
                    }
                    return result.ToString();
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return null;
        }

        /// <summary>
        /// <para>Retrieve the <see cref="Uri" /> of the <paramref name="webBrowser"/>.</para>
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <returns></returns>
        [NotNull]
        public static Uri GetBrowserLocation( this WebControl webBrowser ) {
            try {
                var browser = webBrowser;
                if ( browser != null ) {
                    var result = browser.Invoke( new Func<Uri>( () => browser.Source ) );
                    return ( Uri )result;
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return new Uri( "about:blank" );
        }

        [CanBeNull]
        public static String GetInnerText( this WebControl webBrowser ) {
            try {
                if ( webBrowser != null ) {
                    var result = webBrowser.Invoke( new Func<string>( () => webBrowser.ExecuteJavascriptWithResult( "document.getElementsByTagName('html')[0].innerText" ) ) );

                    if ( result is String ) {
                        return result as String;
                    }
                    return result.ToString();
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return null;
        }
    }
}
