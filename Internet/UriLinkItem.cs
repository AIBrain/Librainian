namespace Librainian.Internet {
    using System;

    public struct UriLinkItem {
        public Uri Href;

        public String Text;

        public override String ToString() {
            return String.Format( "{0}->{1}", this.Href, this.Text );
        }
    }
}