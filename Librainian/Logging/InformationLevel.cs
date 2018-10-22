namespace Librainian.Logging {

    using System;
    using System.Diagnostics;

    public enum InformationLevel : Byte {

        /// <summary>
        ///     The root of all basic information.
        /// <para>More information than <see cref="Verbose"/>.</para>
        /// <para>Less information than <see cref="Debug"/>.</para>
        /// </summary>
        Basic,

        /// <summary>
        ///     Just debugging information.
        /// <para>More information than <see cref="Trace"/>.</para>
        /// <para>Less information than <see cref="Info"/>.</para>
        /// </summary>
        Debug,

        /// <summary>
        /// Useful information.
        /// <para>More information than <see cref="Debug"/>.</para>
        /// <para>Less information than <see cref="Extra"/>.</para>
        /// </summary>
        Info,

        /// <summary>
        /// Extra information.
        /// <para>More information than <see cref="Info"/>.</para>
        /// <para>Less information than <see cref="Verbose"/>.</para>
        /// </summary>
        Extra,

        /// <summary>
        ///     The root of all extra information.
        /// </summary>
        /// <para>More information than <see cref="Extra"/>.</para>
        Verbose,
    }

}