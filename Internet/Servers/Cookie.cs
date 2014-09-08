namespace Librainian.Internet.Servers {
    using System;

    public class Cookie {
        public string name;
        public string value;
        public TimeSpan expire;

        public Cookie( string name, string value, TimeSpan expire ) {
            this.name = name;
            this.value = value;
            this.expire = expire;
        }
    }
}