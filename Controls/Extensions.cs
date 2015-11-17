// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Extensions.cs" was last cleaned by Rick on 2015/06/12 at 2:52 PM

namespace Librainian.Controls {

    public static class Extensions {

        //public static void Output( this WebControl webbrowser, String message ) {
        //    if ( webbrowser == null ) {
        //        return;
        //    }
        //    //webbrowser.on
        //    if ( webbrowser.InvokeRequired ) {
        //        webbrowser.BeginInvoke( new Action( () => CreateDivInsideBrowser( ref webbrowser, message ) ) );
        //    }
        //    else {
        //        CreateDivInsideBrowser( ref webbrowser, message );
        //    }
        //}

        /*
                public static void OnThread( [CanBeNull] this Form form, [CanBeNull] Action action ) {
                    if ( null == form ) {
                        return;
                    }
                    if ( null == action ) {
                        return;
                    }
                    form.InvokeIfRequired( action );
                }
        */

        /*
                public static void OnThread( [CanBeNull] this Control control, [CanBeNull] Action<Control> action ) {
                    if ( null == control ) {
                        return;
                    }
                    if ( null == action ) {
                        return;
                    }
                    if ( control.InvokeRequired ) {
                        control.BeginInvoke( action, control );
                        control.Invalidate();
                    }
                    else {
                        action( control );
                        control.Invalidate();
                    }
                }
        */

        //[Obsolete( "Untested" )]
        /*
        private static void CreateLabelControlInsideFlow( ref FlowLayoutPanel flow, String message ) {
            try {
                if ( flow == null ) {
                    return;
                }
                if ( flow.VerticalScroll.Visible && flow.Controls.Count > 50 ) {
                    Control oldest = null;
                    foreach ( Control control in flow.Controls ) {
                        if ( !( control.Tag is DateTime ) ) { continue; }
                        var when = ( DateTime )control.Tag;
                        if ( null == oldest ) {
                            oldest = control;
                        }
                        else {
                            if ( when < ( DateTime )oldest.Tag ) {
                                oldest = control;
                            }
                        }
                    }
                    if ( null != oldest ) {
                        flow.Controls.Remove( oldest );
                    }
                }

                var temp = new Label {
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding( 0 ),
                    Margin = new Padding( 0 ),
                    Font = new Font( FontFamily.GenericSansSerif, 3, FontStyle.Regular, GraphicsUnit.Millimeter ),
                    BorderStyle = BorderStyle.None,
                    AutoSize = false,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                    Tag = DateTime.UtcNow
                };
                temp.Width = temp.Parent.Width; // -( temp.Parent.Width / 5 );
                flow.Controls.Add( temp );
                temp.SetText( message );
                flow.AutoScroll = true;
                flow.ScrollControlIntoView( temp );
                flow.Update();

                return;
            }
            catch ( Exception error ) {
                Utility.LogException( error );
                return;
            }
        }
        */

        /*
        public static void Output( this FlowLayoutPanel flow, String message ) {
            var mainForm = flow.FindForm() as MainForm;
            if ( mainForm == null ) return;
            if ( flow.InvokeRequired ) {
                flow.BeginInvoke( new Action( () => CreateLabelControlInsideFlow( ref flow, message ) ) );
            }
            else {
                CreateLabelControlInsideFlow( ref flow, message );
            }
        }
        */

        //public static Boolean RemoveTags( this WebControl browser, String tagName, int keepAtMost = 50 ) {
        //    if ( null == browser ) {
        //        return false;
        //    }
        //    //TODO
        //    //if ( null == browser.rem ) {
        //    //    return false;
        //    //}
        //    //while ( null != browser.Document && browser.Document.GetElementsByTagName( tagName ).Count > keepAtMost ) {
        //    //    var item = browser.Document.GetElementsByTagName( tagName )[ 0 ];
        //    //    item.OuterHtml = String.Empty;
        //    //    browser.BeginInvoke( new Action( browser.Update ) );
        //    //}

        //    return true;
        //}

        //private static Boolean CreateDivInsideBrowser( ref WebControl webbrowser, String message ) {
        //    try {
        //        if ( null == webbrowser ) {
        //            return false;
        //        }

        // //webbrowser.cl webbrowser.LoadHTML( String.Format( "
        // <div><span>{0}</span></div>
        // <br />
        // {1}", message, Environment.NewLine ) ); //webbrowser.LoadCompleted += ( sender, args )
        // =&gt; { }; while ( webbrowser.IsLoadingPage ) { WebCore.Update(); }

        // var script = "for (var i = 0; i < 30; i++) {" + "var newdiv =
        // document.createElement('div');" + "var txt = document.createTextNode('This text was added
        // to the DIV for i = ' + i + '.');" + "newdiv.appendChild(txt);" +
        // "document.getElementById('myDiv').appendChild(newdiv); " + "}";
        // webbrowser.ExecuteJavascript( script ); //webbrowser.Focus(); while (
        // webbrowser.IsLoadingPage ) { WebCore.Update(); }

        // //webbrowser.LoadHTML( message ); //var html = webbrowser.Text; //var html = webbrowser.PageContents;

        // //while ( !webbrowser.IsDomReady ) { Thread.Yield(); Thread.Sleep( 0 ); } //if (
        // !webbrowser.IsLive ) { return false; } //if ( !webbrowser.IsLoadingPage ) { return false; }

        // //bob.ToString().TimeDebug();

        // //while ( null == webbrowser.Document ) { // Application.DoEvents(); //}

        // //var div = webbrowser.LoadHTML( .Document.CreateElement( "DIV" );

        // //var span = webbrowser.Document.CreateElement( "SPAN" ); //if ( message.StartsWith(
        // "ECHO:" ) ) { // if ( span != null ) { // span.InnerText = message.Replace( "ECHO:",
        // String.Empty ); // span.Style = "font-variant:small-caps; font-size:small"; // } //}
        // //else if ( message.StartsWith( "INFO:" ) ) { // message = message.Replace( "INFO:",
        // String.Empty ); // if ( message.StartsWith( "<" ) ) { // if ( span != null ) { //
        // span.InnerHtml = message; // span.Style = "font-style: oblique; font-size:xx-small"; // }
        // // } // else { // if ( span != null ) { // span.InnerText = message; // span.Style =
        // "font-style: oblique; font-size:xx-small"; // } // } //} //else { // if ( span != null )
        // { // span.InnerText = message; // span.Style =
        // "font-style:normal;font-size:small;font-family:Comic Sans MS;"; // } //}

        // //if ( div != null ) { // if ( span != null ) { // div.AppendChild( span ); // } // while
        // ( null == webbrowser.Document.Body ) { // Application.DoEvents(); // } //
        // webbrowser.Document.Body.AppendChild( div ); // div.ScrollIntoView( false ); //} //webbrowser.Update();

        // //Application.DoEvents();

        //        return true;
        //    }
        //    catch ( Exception exception ) {
        //        exception.Error();
        //    }
        //    return false;
        //}

        //public static void Write( this RichTextBox textBox, String message, Boolean italic = false ) {
        //    if ( textBox == null ) {
        //        return;
        //    }
        //    var method = new Action( () => {
        //        if ( italic ) {
        //            textBox.SelectionStart = textBox.Text.Length;
        //            textBox.SelectionLength = message.Length;
        //            var style = textBox.SelectionFont.Style;
        //            style |= FontStyle.Italic;
        //            textBox.SelectionFont = new Font( textBox.SelectionFont, style );
        //        }
        //        textBox.AppendText( message );
        //        textBox.SelectionStart = textBox.Text.Length;
        //        textBox.ScrollToCaret();
        //        textBox.Invalidate();
        //    } );
        //    if ( textBox.InvokeRequired ) {
        //        textBox.BeginInvoke( method );
        //    }
        //    else {
        //        method();
        //    }
        //}

        //public static void WriteLine( this RichTextBox textBox, String message ) { AppendToTextBox( textBox, message, false ); }
    }
}