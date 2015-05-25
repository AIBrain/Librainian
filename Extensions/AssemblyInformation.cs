namespace Librainian.Extensions {
    using System;
    using System.IO;
    using System.Reflection;

    public static class AssemblyInformation {

        public static string Company {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof(AssemblyCompanyAttribute), false );
                return attributes.Length == 0 ? String.Empty : ( ( AssemblyCompanyAttribute )attributes[ 0 ] ).Company;
            }
        }

        public static string Copyright {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof(AssemblyCopyrightAttribute), false );
                return attributes.Length == 0 ? String.Empty : ( ( AssemblyCopyrightAttribute )attributes[ 0 ] ).Copyright;
            }
        }

        public static string Description {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof(AssemblyDescriptionAttribute), false );
                return attributes.Length == 0 ? String.Empty : ( ( AssemblyDescriptionAttribute )attributes[ 0 ] ).Description;
            }
        }

        public static string Product {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof(AssemblyProductAttribute), false );
                return attributes.Length == 0 ? String.Empty : ( ( AssemblyProductAttribute )attributes[ 0 ] ).Product;
            }
        }

        public static string Title {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof(AssemblyTitleAttribute), false );
                if ( attributes.Length <= 0 ) {
                    return Path.GetFileNameWithoutExtension( Assembly.GetExecutingAssembly().CodeBase );
                }
                var titleAttribute = ( AssemblyTitleAttribute )attributes[ 0 ];
                return titleAttribute.Title != String.Empty ? titleAttribute.Title : Path.GetFileNameWithoutExtension( Assembly.GetExecutingAssembly().CodeBase );
            }
        }

        public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}