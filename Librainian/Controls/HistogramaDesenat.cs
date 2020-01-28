// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "HistogramaDesenat.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "HistogramaDesenat.cs" was last formatted by Protiguous on 2019/08/08 at 6:46 AM.

namespace Librainian.Controls {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using JetBrains.Annotations;

    /// <summary>
    ///     Summary description for HistogramaDesenat.
    /// </summary>
    /// <see cref="http://www.codeproject.com/Articles/12125/A-simple-histogram-displaying-control" />
    public class HistogramaDesenat : UserControl {

        /// <summary>
        ///     Required designer variable.
        /// </summary>
#pragma warning disable 169
        private Container _components;
#pragma warning restore 169

        private Boolean _myIsDrawing;

        private Int64 _myMaxValue;

        private Int32 _myOffset = 20;

        private Int64[] _myValues;

        private Single _myXUnit;

        private Single _myYUnit;

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

            get => this._myOffset;
        }

        private Font MyFont { get; } = new Font( "Tahoma", 10 );

        public HistogramaDesenat() {

            // This call is required by the Windows.Forms Form Designer.
            this.InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call

            this.Paint += this.HistogramaDesenat_Paint;
            this.Resize += this.HistogramaDesenat_Resize;
        }

        private void ComputeXyUnitValues() {
            this._myYUnit = ( Single ) ( this.Height - (2 * this._myOffset) ) / this._myMaxValue;
            this._myXUnit = ( Single ) ( this.Width - (2 * this._myOffset) ) / ( this._myValues.Length - 1 );
        }

        /// <summary>
        ///     Get the highest value from the array
        /// </summary>
        /// <param name="vals">The array of values in which we look</param>
        /// <returns>The maximum value</returns>
        private Int64 GetMaxim( [NotNull] IEnumerable<Int64> vals ) {
            if ( vals == null ) {
                throw new ArgumentNullException( paramName: nameof( vals ) );
            }

            if ( this._myIsDrawing ) {
                return vals.Concat( new Int64[] {
                    0
                } ).Max();
            }

            return 1;
        }

        private void HistogramaDesenat_Paint( [CanBeNull] Object sender, [CanBeNull] PaintEventArgs e ) {
            if ( e is null ) {
                return;
            }

            if ( !this._myIsDrawing ) {
                return;
            }

            using var displayBrush = new SolidBrush( this.DisplayColor );
            using var myPen = new Pen( displayBrush , this._myXUnit );

            for ( var i = 0; i < this._myValues.Length; i++ ) {

                //We draw each line
                e.Graphics.DrawLine( myPen, new PointF( this._myOffset + (i * this._myXUnit), this.Height - this._myOffset ),
                    new PointF( this._myOffset + (i * this._myXUnit), this.Height - this._myOffset - (this._myValues[ i ] * this._myYUnit) ) );

                //We plot the coresponding index for the maximum value.
                if ( this._myValues[ i ] != this._myMaxValue ) {
                    continue;
                }

                var mySize = e.Graphics.MeasureString( i.ToString(), this.MyFont );
                
                e.Graphics.DrawString( i.ToString(), this.MyFont, displayBrush,
                    new PointF( this._myOffset + (i * this._myXUnit) - (mySize.Width / 2), this.Height - this.MyFont.Height ), StringFormat.GenericDefault );
            }

            //We draw the indexes for 0 and for the length of the array being plotted
            e.Graphics.DrawString( "0", this.MyFont, displayBrush, new PointF( this._myOffset, this.Height - this.MyFont.Height ), StringFormat.GenericDefault );

            e.Graphics.DrawString( s: ( this._myValues.Length - 1 ).ToString(), font: this.MyFont, brush: displayBrush,
                point: new PointF( this._myOffset + (this._myValues.Length * this._myXUnit) - e.Graphics.MeasureString( this._myValues.Length.ToString(), this.MyFont ).Width,
                    this.Height - this.MyFont.Height ), format: StringFormat.GenericDefault );

            //We draw a rectangle surrounding the control.
            using var blackBrush = new SolidBrush( Color.Black );
            using var blackPen = new Pen( blackBrush, 1 );
            e.Graphics.DrawRectangle( blackPen , 0, 0, this.Width - 1, this.Height - 1 );
        }

        private void HistogramaDesenat_Resize( [CanBeNull] Object sender, [CanBeNull] EventArgs e ) {
            if ( this._myIsDrawing ) {
                this.ComputeXyUnitValues();
            }

            this.Refresh();
        }

        /// <summary>
        ///     Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

            // HistogramaDesenat
            this.Font = new Font( "Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0 );
            this.Name = "HistogramaDesenat";
            this.Size = new Size( 208, 176 );
        }

        /// <summary>
        ///     We draw the histogram on the control
        /// </summary>
        /// <param name="values">The values being drawn</param>
        public void DrawHistogram( [NotNull] Int64[] values ) {
            this._myValues = new Int64[ values.Length ];
            values.CopyTo( this._myValues, 0 );

            this._myIsDrawing = true;
            this._myMaxValue = this.GetMaxim( this._myValues );

            this.ComputeXyUnitValues();
        }
    }
}