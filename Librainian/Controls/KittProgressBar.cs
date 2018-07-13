// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "KittProgressBar.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "KittProgressBar.cs" was last formatted by Protiguous on 2018/07/10 at 8:57 PM.

namespace Librainian.Controls {

	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	/// <summary>A replacement for the default ProgressBar control.</summary>
	[DefaultEvent( "ValueChanged" )]
	[Obsolete( "Under construction!" )]
	public class KittProgressBar : UserControl {

		private readonly Timer mGlowAnimation = new Timer();

		private Boolean _mAnimate = true;

		private Color _mBackgroundColor = Color.FromArgb( 201, 201, 201 );

		private Int32 _mGlowPosition = -325;

		private Color _mHighlightColor = Color.White;

		private Int32 _mMaxValue = 100;

		private Container components;

		private Color mEndColor = Color.FromArgb( 0, 211, 40 );

		private Color mGlowColor = Color.FromArgb( 150, 255, 255, 255 );

		private Int32 mMinValue;

		private Color mStartColor = Color.FromArgb( 210, 0, 0 );

		private Int32 mValue;

		/// <summary>When the MaxValue property is changed.</summary>
		public delegate void MaxChangedHandler( Object sender, EventArgs e );

		/// <summary>When the MinValue property is changed.</summary>
		public delegate void MinChangedHandler( Object sender, EventArgs e );

		/// <summary>When the Value property is changed.</summary>
		public delegate void ValueChangedHandler( Object sender, EventArgs e );

		/// <summary>Whether the glow is animated.</summary>
		[Category( "Highlights and Glows" )]
		[DefaultValue( typeof( Boolean ), "true" )]
		[Description( "Whether the glow is animated or not." )]
		public Boolean Animate {
			get => this._mAnimate;

			set {
				this._mAnimate = value;

				if ( value ) { this.mGlowAnimation.Start(); }
				else { this.mGlowAnimation.Stop(); }

				this.Invalidate();
			}
		}

		/// <summary>The color of the background.</summary>
		[Category( "Highlights and Glows" )]
		[DefaultValue( typeof( Color ), "201,201,201" )]
		[Description( "The color of the background." )]
		public Color BackgroundColor {
			get => this._mBackgroundColor;

			set {
				this._mBackgroundColor = value;
				this.Invalidate();
			}
		}

		/// <summary>
		///     The end color for the progress bar. 210, 000, 000 = Red 210, 202, 000 = Yellow 000, 163,
		///     211 = Blue 000, 211, 040 = Green
		/// </summary>
		[Category( "Bar" )]
		[DefaultValue( typeof( Color ), "0, 211, 40" )]
		[Description( "The end color for the progress bar." + "210, 000, 000 = Red\n" + "210, 202, 000 = Yellow\n" + "000, 163, 211 = Blue\n" + "000, 211, 040 = Green\n" )]
		public Color EndColor {
			get => this.mEndColor;

			set {
				this.mEndColor = value;
				this.Invalidate();
			}
		}

		/// <summary>The color of the glow.</summary>
		[Category( "Highlights and Glows" )]
		[DefaultValue( typeof( Color ), "150, 255, 255, 255" )]
		[Description( "The color of the glow." )]
		public Color GlowColor {
			get => this.mGlowColor;

			set {
				this.mGlowColor = value;
				this.Invalidate();
			}
		}

		/// <summary>The color of the highlights.</summary>
		[Category( "Highlights and Glows" )]
		[DefaultValue( typeof( Color ), "White" )]
		[Description( "The color of the highlights." )]
		public Color HighlightColor {
			get => this._mHighlightColor;

			set {
				this._mHighlightColor = value;
				this.Invalidate();
			}
		}

		/// <summary>The maximum value for the Value property.</summary>
		[Category( "Value" )]
		[DefaultValue( 100 )]
		[Description( "The maximum value for the Value property." )]
		public Int32 MaxValue {
			get => this._mMaxValue;

			set {
				this._mMaxValue = value;

				if ( value > this.MaxValue ) { this.Value = this.MaxValue; }

				if ( this.Value < this.MaxValue ) { this.mGlowAnimation.Start(); }

				this.MaxChanged?.Invoke( this, EventArgs.Empty );
				this.Invalidate();
			}
		}

		/// <summary>The minimum value for the Value property.</summary>
		[Category( "Value" )]
		[DefaultValue( 0 )]
		[Description( "The minimum value for the Value property." )]
		public Int32 MinValue {
			get => this.mMinValue;

			set {
				this.mMinValue = value;

				if ( value < this.MinValue ) { this.Value = this.MinValue; }

				this.MinChanged?.Invoke( this, EventArgs.Empty );
				this.Invalidate();
			}
		}

		/// <summary>
		///     The start color for the progress bar. 210, 000, 000 = Red 210, 202, 000 = Yellow 000,
		///     163, 211 = Blue 000, 211, 040
		///     = Green
		/// </summary>
		[Category( "Bar" )]
		[DefaultValue( typeof( Color ), "210, 0, 0" )]
		[Description( "The start color for the progress bar." + "210, 000, 000 = Red\n" + "210, 202, 000 = Yellow\n" + "000, 163, 211 = Blue\n" + "000, 211, 040 = Green\n" )]
		public Color StartColor {
			get => this.mStartColor;

			set {
				this.mStartColor = value;
				this.Invalidate();
			}
		}

		/// <summary>The value that is displayed on the progress bar.</summary>
		[Category( "Value" )]
		[DefaultValue( 0 )]
		[Description( "The value that is displayed on the progress bar." )]
		public Int32 Value {
			get => this.mValue;

			set {
				if ( value > this.MaxValue || value < this.MinValue ) { return; }

				this.mValue = value;

				if ( value < this.MaxValue ) { this.mGlowAnimation.Start(); }

				if ( value == this.MaxValue ) { this.mGlowAnimation.Stop(); }

				this.ValueChanged?.Invoke( this, EventArgs.Empty );
				this.Invalidate();
			}
		}

		/// <summary>Create the control and initialize it.</summary>
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

				if ( this.Value < this.MaxValue ) { this.mGlowAnimation.Start(); }
			}
		}

		/// <summary>When the MaxValue property is changed.</summary>
		public event MaxChangedHandler MaxChanged;

		/// <summary>When the MinValue property is changed.</summary>
		public event MinChangedHandler MinChanged;

		/// <summary>When the Value property is changed.</summary>
		public event ValueChangedHandler ValueChanged;

		/// <summary>Clean up any resources being used.</summary>
		protected override void Dispose( Boolean disposing ) {
			if ( disposing ) { this.components?.Dispose(); }

			base.Dispose( disposing );
		}

		private static Boolean InDesignMode() => LicenseManager.UsageMode == LicenseUsageMode.Designtime;

		private static GraphicsPath RoundRect( RectangleF r, Single r1, Single r2, Single r3, Single r4 ) {
			Single x = r.X, y = r.Y, w = r.Width, h = r.Height;
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
				Width = ( Int32 ) ( this.Value * 1.0F / ( this.MaxValue - this.MinValue ) * this.Width )
			};

			g.FillRectangle( new SolidBrush( this.GetIntermediateColor() ), r );
		}

		private void DrawBarShadows( Graphics g ) {
			var lr = new Rectangle( 1, 2, 15, this.Height - 3 );
			var lg = new LinearGradientBrush( lr, Color.White, Color.White, LinearGradientMode.Horizontal );

			var lc = new ColorBlend( 3 ) {
				Colors = new[] {
					Color.Transparent, Color.FromArgb( 40, 0, 0, 0 ), Color.Transparent
				},
				Positions = new[] {
					0.0F, 0.2F, 1.0F
				}
			};

			lg.InterpolationColors = lc;

			lr.X--;
			g.FillRectangle( lg, lr );

			var rr = new Rectangle( this.Width - 3, 2, 15, this.Height - 3 ) {
				X = ( Int32 ) ( this.Value * 1.0F / ( this.MaxValue - this.MinValue ) * this.Width ) - 14
			};

			var rg = new LinearGradientBrush( rr, Color.Black, Color.Black, LinearGradientMode.Horizontal );

			var rc = new ColorBlend( 3 ) {
				Colors = new[] {
					Color.Transparent, Color.FromArgb( 40, 0, 0, 0 ), Color.Transparent
				},
				Positions = new[] {
					0.0F, 0.8F, 1.0F
				}
			};

			rg.InterpolationColors = rc;

			g.FillRectangle( rg, rr );
		}

		private void DrawGlow( Graphics g ) {
			var r = new Rectangle( this._mGlowPosition, 0, 60, this.Height );
			var lgb = new LinearGradientBrush( r, Color.White, Color.White, LinearGradientMode.Horizontal );

			var cb = new ColorBlend( 4 ) {
				Colors = new[] {
					Color.Transparent, this.GlowColor, this.GlowColor, Color.Transparent
				},
				Positions = new[] {
					0.0F, 0.5F, 0.6F, 1.0F
				}
			};

			lgb.InterpolationColors = cb;

			var clip = new Rectangle( 1, 2, this.Width - 3, this.Height - 3 ) {
				Width = ( Int32 ) ( this.Value * 1.0F / ( this.MaxValue - this.MinValue ) * this.Width )
			};

			g.SetClip( clip );
			g.FillRectangle( lgb, r );
			g.ResetClip();
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

		private void DrawOuterStroke( Graphics g ) {
			var r = this.ClientRectangle;
			r.Width--;
			r.Height--;
			var rr = RoundRect( r, 2, 2, 2, 2 );
			g.DrawPath( new Pen( Color.FromArgb( 178, 178, 178 ) ), rr );
		}

		private Color GetIntermediateColor() {
			var c = this.StartColor;
			var c2 = this.EndColor;

			var pc = this.Value * 1.0F / ( this.MaxValue - this.MinValue );

			Int32 ca = c.A, cr = c.R, cg = c.G, cb = c.B;
			Int32 c2a = c2.A, c2r = c2.R, c2g = c2.G, c2b = c2.B;

			var a = ( Int32 ) Math.Abs( ca + ( ca - c2a ) * pc );
			var r = ( Int32 ) Math.Abs( cr - ( cr - c2r ) * pc );
			var g = ( Int32 ) Math.Abs( cg - ( cg - c2g ) * pc );
			var b = ( Int32 ) Math.Abs( cb - ( cb - c2b ) * pc );

			if ( a > 255 ) { a = 255; }

			if ( r > 255 ) { r = 255; }

			if ( g > 255 ) { g = 255; }

			if ( b > 255 ) { b = 255; }

			return Color.FromArgb( a, r, g, b );
		}

		/// <summary>
		///     Required method for Designer support - do not modify the contents of this method with
		///     the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.SuspendLayout();

			// KittProgressBar
			this.Name = "KittProgressBar";
			this.Size = new Size( 349, 94 );
			this.ResumeLayout( false );
		}

		private void mGlowAnimation_Tick( Object sender, EventArgs e ) {
			if ( this.Animate ) {
				this._mGlowPosition += 4;

				if ( this._mGlowPosition > this.Width ) { this._mGlowPosition = -300; }

				this.Invalidate();
			}
			else {
				this.mGlowAnimation.Stop();
				this._mGlowPosition = -320;
			}
		}

		private void ProgressBar_Paint( Object sender, PaintEventArgs e ) {
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

		/// <summary>Required designer variable.</summary>
#pragma warning disable 649
#pragma warning restore 649
	}
}