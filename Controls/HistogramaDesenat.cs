// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/HistogramaDesenat.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Controls {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    ///     Summary description for HistogramaDesenat.
    /// </summary>
    /// <seealso cref="http://www.codeproject.com/Articles/12125/A-simple-histogram-displaying-control" />
    public class HistogramaDesenat : UserControl {

        /// <summary>
        ///     Required designer variable.
        /// </summary>
#pragma warning disable 169
        private readonly Container _components;
#pragma warning restore 169

        private readonly Font _myFont = new Font( "Tahoma", 10 );

        private Boolean _myIsDrawing;

        private Int64 _myMaxValue;

        private Int32 _myOffset = 20;

        private Int64[] _myValues;

        private Single _myXUnit;

        private Single _myYUnit;

        public HistogramaDesenat() {

            // This call is required by the Windows.Forms Form Designer.
            this.InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call

            this.Paint += this.HistogramaDesenat_Paint;
            this.Resize += this.HistogramaDesenat_Resize;
        }

        [Category( "Histogram Options" )]
        [Description( "The color used within the control" )]
        public Color DisplayColor { get; } = Color.Black;

        //this gives the vertical unit used to scale our values
        //this gives the horizontal unit used to scale our values
        //the offset, in pixels, from the control margins.
        [Category( "Histogram Options" )]
        [Description( "The distance from the margins for the histogram" )]
        public Int32 Offset {
            set {
                if ( value > 0 ) {
                    this._myOffset = value;
                }
            }

            get {
                return this._myOffset;
            }
        }

        /// <summary>
        ///     We draw the histogram on the control
        /// </summary>
        /// <param name="values">The values being drawn</param>
        public void DrawHistogram( Int64[] values ) {
            this._myValues = new Int64[ values.Length ];
            values.CopyTo( this._myValues, 0 );

            this._myIsDrawing = true;
            this._myMaxValue = this.GetMaxim( this._myValues );

            this.ComputeXyUnitValues();
        }

        ///// <summary>
        /////     Clean up any resources being used.
        ///// </summary>
        //protected override void Dispose( Boolean disposing ) {
        //    if ( disposing ) {
        //        this._components?.Dispose();
        //    }
        //    base.Dispose( disposing );
        //}

        private void ComputeXyUnitValues() {
            this._myYUnit = ( Single )( this.Height - 2 * this._myOffset ) / this._myMaxValue;
            this._myXUnit = ( Single )( this.Width - 2 * this._myOffset ) / ( this._myValues.Length - 1 );
        }

        /// <summary>
        ///     We get the highest value from the array
        /// </summary>
        /// <param name="vals">The array of values in which we look</param>
        /// <returns>The maximum value</returns>
        private Int64 GetMaxim( IEnumerable<Int64> vals ) {
            if ( this._myIsDrawing ) {
                return vals.Concat( new Int64[] { 0 } ).Max();
            }
            return 1;
        }

        private void HistogramaDesenat_Paint( Object sender, PaintEventArgs e ) {
            if ( !this._myIsDrawing ) {
                return;
            }

            var g = e.Graphics;
            var myPen = new Pen( new SolidBrush( this.DisplayColor ), this._myXUnit );

            //The width of the pen is given by the XUnit for the control.
            for ( var i = 0; i < this._myValues.Length; i++ ) {

                //We draw each line
                g.DrawLine( myPen, new PointF( this._myOffset + i * this._myXUnit, this.Height - this._myOffset ), new PointF( this._myOffset + i * this._myXUnit, this.Height - this._myOffset - this._myValues[ i ] * this._myYUnit ) );

                //We plot the coresponding index for the maximum value.
                if ( this._myValues[ i ] != this._myMaxValue ) {
                    continue;
                }

                var mySize = g.MeasureString( i.ToString(), this._myFont );

                g.DrawString( i.ToString(), this._myFont, new SolidBrush( this.DisplayColor ), new PointF( this._myOffset + i * this._myXUnit - mySize.Width / 2, this.Height - this._myFont.Height ), StringFormat.GenericDefault );
            }

            //We draw the indexes for 0 and for the length of the array being plotted
            g.DrawString( "0", this._myFont, new SolidBrush( this.DisplayColor ), new PointF( this._myOffset, this.Height - this._myFont.Height ), StringFormat.GenericDefault );
            g.DrawString( s: ( this._myValues.Length - 1 ).ToString(), font: this._myFont, brush: new SolidBrush( this.DisplayColor ), point: new PointF( this._myOffset + this._myValues.Length * this._myXUnit - g.MeasureString( this._myValues.Length.ToString(), this._myFont ).Width, this.Height - this._myFont.Height ), format: StringFormat.GenericDefault );

            //We draw a rectangle surrounding the control.
            g.DrawRectangle( new Pen( new SolidBrush( Color.Black ), 1 ), 0, 0, this.Width - 1, this.Height - 1 );
        }

        private void HistogramaDesenat_Resize( Object sender, EventArgs e ) {
            if ( this._myIsDrawing ) {
                this.ComputeXyUnitValues();
            }
            this.Refresh();
        }

        /// <summary>
        ///     Required method for Designer support - do not modify the contents of this method with
        ///     the code editor.
        /// </summary>
        private void InitializeComponent() {

            // HistogramaDesenat
            this.Font = new Font( "Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0 );
            this.Name = "HistogramaDesenat";
            this.Size = new Size( 208, 176 );
        }
    }
}