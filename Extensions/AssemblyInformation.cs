// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "AssemblyInformation.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/AssemblyInformation.cs" was last formatted by Protiguous on 2018/05/24 at 7:07 PM.

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

                if ( attributes.Length <= 0 ) { return Path.GetFileNameWithoutExtension( Assembly.GetExecutingAssembly().CodeBase ); }

                var titleAttribute = ( AssemblyTitleAttribute )attributes[0];

                return titleAttribute.Title != String.Empty ? titleAttribute.Title : Path.GetFileNameWithoutExtension( Assembly.GetExecutingAssembly().CodeBase );
            }
        }

        public static String Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}