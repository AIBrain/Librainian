namespace Librainian.Persistence {
    using System.IO;

    public interface IPersistableSettings {
        /// <summary>
        ///     Returns the <see cref="MainStoragePath" /> as a <see cref="DirectoryInfo" />.
        /// </summary>
        DirectoryInfo MainStoragePath { get; set; }

        /// <summary>
        ///     check if we have a storagepath given for AIBrain.
        ///     if we don't, popup a dialog to ask.
        ///     Settings.
        /// </summary>
        /// <returns></returns>
        void ValidateStorageFolder();

        /// <summary>
        ///     ask user for folder/network path where to store AIBrain
        /// </summary>
        void AskUserForStorageFolder();
    }
}