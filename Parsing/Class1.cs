namespace Librainian.Parsing {
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(false)]
    [Flags]
    public enum StringSplitOptions {
        None = 0,
        RemoveEmptyEntries = 1
    }
}