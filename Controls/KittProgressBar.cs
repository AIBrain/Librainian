#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian2/KittProgressBar.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM
#endregion

namespace Librainian.Controls {
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>
    ///     A replacement for the default ProgressBar control.
    /// </summary>
    [DefaultEvent( "ValueChanged" )]
    public class KittProgressBar : UserControl {
        private readonly Timer mGlowAnimation = new Timer();

        /// <summary>
        ///     Required designer variable.
        /// </summary>
#pragma warning disable 649
        private Container components;
#pragma warning restore 649

        private Boolean _mAnimate = true;

        private Color _mBackgroundColor = Color.FromArgb( 201, 201, 201 );

        private Color mEndColor = Color.FromArgb( 0, 211, 40 );

        private Color mGlowColor = Color.FromArgb( 150, 255, 255, 255 );

        private int _mGlowPosition = -325;

        private Color _mHighlightColor = Color.White;

        private int _mMaxValue = 100;

        private int mMinValue;

        private Color mStartColor = Color.FromArgb( 210, 0, 0 );

        private int mValue;

        /// <summary>
        ///     Create the control and initialize it.
        /// </summary>
        public KittProgressBar() {
            // This call is required by the Windows.Forms Form Designer.
            this.InitializeComponent();
            this.SetStyle( ControlStyles.AllPaintingInWmPaint, true );
            this.SetStyle( ControlStyles.DoubleBuffer, true );
            this.SetStyle( ControlStyles.ResizeRedraw, true );
            this.SetStyle( ControlStyles.Selectable, true );
            this.SetStyle( ControlStyles.SupportsTransparentBackColor, true );
            this.SetStyle( ControlStyles.UserPaint, true );
            this.BackColor = Color.Transparent;
            if ( !InDesignMode() ) {
                this.mGlowAnimation.Tick += this.mGlowAnimation_Tick;
                this.mGlowAnimation.Interval = 15;
                if ( this.Value < this.MaxValue ) {
                    this.mGlowAnimation.Start();
                }
            }
        }

        /// <summary>
        ///     The value that is displayed on the progress bar.
        /// </summary>
        [Category( "Value" )]
        [DefaultValue( 0 )]
        [Description( "The value that is displayed on the progress bar." )]
        public int Value {
            get { return this.mValue; }
            set {
                if ( value > this.MaxValue || value < this.MinValue ) {
                    return;
                }
                this.mValue = value;
                if ( value < this.MaxValue ) {
                    this.mGlowAnimation.Start();
                }
                if ( value == this.MaxValue ) {
                    this.mGlowAnimation.Stop();
                }
                var vc = this.ValueChanged;
                if ( vc != null ) {
                    vc( this, EventArgs.Empty );
                }
                this.Invalidate();
            }
        }

        /// <summary>
        ///     The maximum value for the Value property.
        /// </summary>
        [Category( "Value" )]
        [DefaultValue( 100 )]
        [Description( "The maximum value for the Value property." )]
        public int MaxValue {
            get { return this._mMaxValue; }
            set {
                this._mMaxValue = value;
                if ( value > this.MaxValue ) {
                    this.Value = this.MaxValue;
                }
                if ( this.Value < this.MaxValue ) {
                    this.mGlowAnimation.Start();
                }
                var mc = this.MaxChanged;
                if ( mc != null ) {
                    mc( this, EventArgs.Empty );
                }
                this.Invalidate();
            }
        }

        /// <summary>
        ///     The minimum value for the Value property.
        /// </summary>
        [Category( "Value" )]
        [DefaultValue( 0 )]
        [Description( "The minimum value for the Value property." )]
        public int MinValue {
            get { return this.mMinValue; }
            set {
                this.mMinValue = value;
                if ( value < this.MinValue ) {
                    this.Value = this.MinValue;
                }
                var mc = this.MinChanged;
                if ( mc != null ) {
                    mc( this, EventArgs.Empty );
                }
                this.Invalidate();
            }
        }

        /// <summary>
        ///     The start color for the progress bar. 210, 000, 000 = Red 210, 202, 000 = Yellow 000, 163, 211 = Blue 000, 211, 040
        ///     = Green
        /// </summary>
        [Category( "Bar" )]
        [DefaultValue( typeof ( Color ), "210, 0, 0" )]
        [Description( "The start color for the progress bar." + "210, 000, 000 = Red\n" + "210, 202, 000 = Yellow\n" + "000, 163, 211 = Blue\n" + "000, 211, 040 = Green\n" )]
        public Color StartColor {
            get { return this.mStartColor; }
            set {
                this.mStartColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        ///     The end color for the progress bar. 210, 000, 000 = Red 210, 202, 000 = Yellow 000, 163, 211 = Blue 000, 211, 040 =
        ///     Green
        /// </summary>
        [Category( "Bar" )]
        [DefaultValue( typeof ( Color ), "0, 211, 40" )]
        [Description( "The end color for the progress bar." + "210, 000, 000 = Red\n" + "210, 202, 000 = Yellow\n" + "000, 163, 211 = Blue\n" + "000, 211, 040 = Green\n" )]
        public Color EndColor {
            get { return this.mEndColor; }
            set {
                this.mEndColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        ///     The color of the highlights.
        /// </summary>
        [Category( "Highlights and Glows" )]
        [DefaultValue( typeof ( Color ), "White" )]
        [Description( "The color of the highlights." )]
        public Color HighlightColor {
            get { return this._mHighlightColor; }
            set {
                this._mHighlightColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        ///     The color of the background.
        /// </summary>
        [Category( "Highlights and Glows" )]
        [DefaultValue( typeof ( Color ), "201,201,201" )]
        [Description( "The color of the background." )]
        public Color BackgroundColor {
            get { return this._mBackgroundColor; }
            set {
                this._mBackgroundColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        ///     Whether the glow is animated.
        /// </summary>
        [Category( "Highlights and Glows" )]
        [DefaultValue( typeof ( Boolean ), "true" )]
        [Description( "Whether the glow is animated or not." )]
        public Boolean Animate {
            get { return this._mAnimate; }
            set {
                this._mAnimate = value;
                if ( value ) {
                    this.mGlowAnimation.Start();
                }
                else {
                    this.mGlowAnimation.Stop();
                }
                this.Invalidate();
            }
        }

        /// <summary>
        ///     The color of the glow.
        /// </summary>
        [Category( "Highlights and Glows" )]
        [DefaultValue( typeof ( Color ), "150, 255, 255, 255" )]
        [Description( "The color of the glow." )]
        public Color GlowColor {
            get { return this.mGlowColor; }
            set {
                this.mGlowColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        protected override void Dispose( Boolean disposing ) {
            if ( disposing ) {
                if ( this.components != null ) {
                    this.components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        /// <summary>
        ///     Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // KittProgressBar
            // 
            this.Name = "KittProgressBar";
            this.Size = new Size( 349, 94 );
            this.ResumeLayout( false );
        }

        private void DrawBackground( Graphics g ) {
            var r = this.ClientRectangle;
            r.Width--;
            r.Height--;
            var rr = RoundRect( r, 2, 2, 2, 2 );
            g.FillPath( new SolidBrush( this.BackgroundColor ), rr );
        }

        private void DrawBackgroundShadows( Graphics g ) {
            var lr = new Rectangle( 2, 2, 10, this.Height - 5 );
            var lg = new LinearGradientBrush( lr, Color.FromArgb( 30, 0, 0, 0 ), Color.Transparent, LinearGradientMode.Horizontal );
            lr.X--;
            g.FillRectangle( lg, lr );

            var rr = new Rectangle( this.Width - 12, 2, 10, this.Height - 5 );
            var rg = new LinearGradientBrush( rr, Color.Transparent, Color.FromArgb( 20, 0, 0, 0 ), LinearGradientMode.Horizontal );
            g.FillRectangle( rg, rr );
        }

        private void DrawBar( Graphics g ) {
            var r = new Rectangle( 1, 2, this.Width - 3, this.Height - 3 ) {
                                                                               Width = ( int ) ( this.Value*1.0F/( this.MaxValue - this.MinValue )*this.Width )
                                                                           };
            g.FillRectangle( new SolidBrush( this.GetIntermediateColor() ), r );
        }

        private void DrawBarShadows( Graphics g ) {
            var lr = new Rectangle( 1, 2, 15, this.Height - 3 );
            var lg = new LinearGradientBrush( lr, Color.White, Color.White, LinearGradientMode.Horizontal );

            var lc = new ColorBlend( 3 ) {
                                             Colors = new[] { Color.Transparent, Color.FromArgb( 40, 0, 0, 0 ), Color.Transparent },
                                             Positions = new[] { 0.0F, 0.2F, 1.0F }
                                         };
            lg.InterpolationColors = lc;

            lr.X--;
            g.FillRectangle( lg, lr );

            var rr = new Rectangle( this.Width - 3, 2, 15, this.Height - 3 ) {
                                                                                 X = ( int ) ( this.Value*1.0F/( this.MaxValue - this.MinValue )*this.Width ) - 14
                                                                             };
            var rg = new LinearGradientBrush( rr, Color.Black, Color.Black, LinearGradientMode.Horizontal );

            var rc = new ColorBlend( 3 ) {
                                             Colors = new[] { Color.Transparent, Color.FromArgb( 40, 0, 0, 0 ), Color.Transparent },
                                             Positions = new[] { 0.0F, 0.8F, 1.0F }
                                         };
            rg.InterpolationColors = rc;

            g.FillRectangle( rg, rr );
        }

        private void DrawHighlight( Graphics g ) {
            var tr = new Rectangle( 1, 1, this.Width - 1, 6 );
            var tp = RoundRect( tr, 2, 2, 0, 0 );

            g.SetClip( tp );
            var tg = new LinearGradientBrush( tr, Color.White, Color.FromArgb( 128, Color.White ), LinearGradientMode.Vertical );
            g.FillPath( tg, tp );
            g.ResetClip();

            var br = new Rectangle( 1, this.Height - 8, this.Width - 1, 6 );
            var bp = RoundRect( br, 0, 0, 2, 2 );

            g.SetClip( bp );
            var bg = new LinearGradientBrush( br, Color.Transparent, Color.FromArgb( 100, this.HighlightColor ), LinearGradientMode.Vertical );
            g.FillPath( bg, bp );
            g.ResetClip();
        }

        private void DrawInnerStroke( Graphics g ) {
            var r = this.ClientRectangle;
            r.X++;
            r.Y++;
            r.Width -= 3;
            r.Height -= 3;
            var rr = RoundRect( r, 2, 2, 2, 2 );
            g.DrawPath( new Pen( Color.FromArgb( 100, Color.White ) ), rr );
        }

        private void DrawGlow( Graphics g ) {
            var r = new Rectangle( this._mGlowPosition, 0, 60, this.Height );
            var lgb = new LinearGradientBrush( r, Color.White, Color.White, LinearGradientMode.Horizontal );

            var cb = new ColorBlend( 4 ) {
                                             Colors = new[] { Color.Transparent, this.GlowColor, this.GlowColor, Color.Transparent },
                                             Positions = new[] { 0.0F, 0.5F, 0.6F, 1.0F }
                                         };
            lgb.InterpolationColors = cb;

            var clip = new Rectangle( 1, 2, this.Width - 3, this.Height - 3 ) {
                                                                                  Width = ( int ) ( this.Value*1.0F/( this.MaxValue - this.MinValue )*this.Width )
                                                                              };
            g.SetClip( clip );
            g.FillRectangle( lgb, r );
            g.ResetClip();
        }

        private void DrawOuterStroke( Graphics g ) {
            var r = this.ClientRectangle;
            r.Width--;
            r.Height--;
            var rr = RoundRect( r, 2, 2, 2, 2 );
            g.DrawPath( new Pen( Color.FromArgb( 178, 178, 178 ) ), rr );
        }

        #region -  Functions  -
        private static GraphicsPath RoundRect( RectangleF r, float r1, float r2, float r3, float r4 ) {
            float x = r.X, y = r.Y, w = r.Width, h = r.Height;
            var rr = new GraphicsPath();
            rr.AddBezier( x, y + r1, x, y, x + r1, y, x + r1, y );
            rr.AddLine( x + r1, y, x + w - r2, y );
            rr.AddBezier( x + w - r2, y, x + w, y, x + w, y + r2, x + w, y + r2 );
            rr.AddLine( x + w, y + r2, x + w, y + h - r3 );
            rr.AddBezier( x + w, y + h - r3, x + w, y + h, x + w - r3, y + h, x + w - r3, y + h );
            rr.AddLine( x + w - r3, y + h, x + r4, y + h );
            rr.AddBezier( x + r4, y + h, x, y + h, x, y + h - r4, x, y + h - r4 );
            rr.AddLine( x, y + h - r4, x, y + r1 );
            return rr;
        }

        private static Boolean InDesignMode() {
            return ( LicenseManager.UsageMode == LicenseUsageMode.Designtime );
        }

        private Color GetIntermediateColor() {
            var c = this.StartColor;
            var c2 = this.EndColor;

            var pc = this.Value*1.0F/( this.MaxValue - this.MinValue );

            int ca = c.A, cr = c.R, cg = c.G, cb = c.B;
            int c2a = c2.A, c2r = c2.R, c2g = c2.G, c2b = c2.B;

            var a = ( int ) Math.Abs( ca + ( ca - c2a )*pc );
            var r = ( int ) Math.Abs( cr - ( ( cr - c2r )*pc ) );
            var g = ( int ) Math.Abs( cg - ( ( cg - c2g )*pc ) );
            var b = ( int ) Math.Abs( cb - ( ( cb - c2b )*pc ) );

            if ( a > 255 ) {
                a = 255;
            }
            if ( r > 255 ) {
                r = 255;
            }
            if ( g > 255 ) {
                g = 255;
            }
            if ( b > 255 ) {
                b = 255;
            }

            return ( Color.FromArgb( a, r, g, b ) );
        }
        #endregion

        private void ProgressBar_Paint( object sender, PaintEventArgs e ) {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            this.DrawBackground( e.Graphics );
            this.DrawBackgroundShadows( e.Graphics );
            this.DrawBar( e.Graphics );
            this.DrawBarShadows( e.Graphics );
            this.DrawHighlight( e.Graphics );
            this.DrawInnerStroke( e.Graphics );
            this.DrawGlow( e.Graphics );
            this.DrawOuterStroke( e.Graphics );
        }

        private void mGlowAnimation_Tick( object sender, EventArgs e ) {
            if ( this.Animate ) {
                this._mGlowPosition += 4;
                if ( this._mGlowPosition > this.Width ) {
                    this._mGlowPosition = -300;
                }
                this.Invalidate();
            }
            else {
                this.mGlowAnimation.Stop();
                this._mGlowPosition = -320;
            }
        }

        #region -  Events  -
        /// <summary>
        ///     When the MaxValue property is changed.
        /// </summary>
        public delegate void MaxChangedHandler( object sender, EventArgs e );

        /// <summary>
        ///     When the MinValue property is changed.
        /// </summary>
        public delegate void MinChangedHandler( object sender, EventArgs e );

        /// <summary>
        ///     When the Value property is changed.
        /// </summary>
        public delegate void ValueChangedHandler( object sender, EventArgs e );

        /// <summary>
        ///     When the Value property is changed.
        /// </summary>
        public event ValueChangedHandler ValueChanged;

        /// <summary>
        ///     When the MinValue property is changed.
        /// </summary>
        public event MinChangedHandler MinChanged;

        /// <summary>
        ///     When the MaxValue property is changed.
        /// </summary>
        public event MaxChangedHandler MaxChanged;
        #endregion
    }
}
