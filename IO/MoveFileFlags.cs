namespace Librainian.IO {
    using System;

    [Flags]
    enum MoveFileFlags {
        MovefileReplaceExisting = 0x00000001,
        MovefileCopyAllowed = 0x00000002,
        MovefileDelayUntilReboot = 0x00000004,
        MovefileWriteThrough = 0x00000008,
        MovefileCreateHardlink = 0x00000010,
        MovefileFailIfNotTrackable = 0x00000020
    }
}