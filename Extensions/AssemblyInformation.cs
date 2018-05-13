// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/AssemblyInformation.cs" was last cleaned by Protiguous on 2018/05/12 at 1:22 AM

namespace Librainian.Extensions {

    using System;
    using System.IO;
    using System.Reflection;

    public static class AssemblyInformation {

        public static String Company {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyCompanyAttribute ), false );

                return attributes.Length == 0 ? String.Empty : ( ( AssemblyCompanyAttribute )attributes[0] ).Company;
            }
        }

        public static String Copyright {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyCopyrightAttribute ), false );

                return attributes.Length == 0 ? String.Empty : ( ( AssemblyCopyrightAttribute )attributes[0] ).Copyright;
            }
        }

        public static String Description {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyDescriptionAttribute ), false );

                return attributes.Length == 0 ? String.Empty : ( ( AssemblyDescriptionAttribute )attributes[0] ).Description;
            }
        }

        public static String Product {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyProductAttribute ), false );

                return attributes.Length == 0 ? String.Empty : ( ( AssemblyProductAttribute )attributes[0] ).Product;
            }
        }

        public static String Title {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyTitleAttribute ), false );

                if ( attributes.Length <= 0 ) {
                    return Path.GetFileNameWithoutExtension( Assembly.GetExecutingAssembly().CodeBase );
                }

                var titleAttribute = ( AssemblyTitleAttribute )attributes[0];

                return titleAttribute.Title != String.Empty ? titleAttribute.Title : Path.GetFileNameWithoutExtension( Assembly.GetExecutingAssembly().CodeBase );
            }
        }

        public static String Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}