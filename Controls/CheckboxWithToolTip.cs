namespace Librainian.Controls {

    using System;
    using System.Windows.Forms;

    public class CheckboxWithToolTip : CheckBox {

        private ToolTip Tt { get; } = new ToolTip();

        public String TooltipText {
            get; set;
        }

        public CheckboxWithToolTip() {
            this.Tt.AutoPopDelay = 1500;
            this.Tt.InitialDelay = 400;
            this.Tt.IsBalloon = true;
            this.Tt.UseAnimation = true;
            this.Tt.UseFading = true;
            this.Tt.Active = true;
            this.MouseEnter += ( sender1, ea ) => {
                                   if ( String.IsNullOrEmpty( this.TooltipText ) ) {
                                       return;
                                   }
                                   this.Tt.SetToolTip( this, this.TooltipText );
                                   this.Tt.Show( this.TooltipText, this.Parent );
                               };
            this.MouseLeave += ( sender, args ) => {
                                   this.Tt.Hide( this.Parent );
                               };
        }

    }

}