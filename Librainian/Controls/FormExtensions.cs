// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FormExtensions.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "FormExtensions.cs" was last formatted by Protiguous on 2019/08/08 at 6:46 AM.

namespace Librainian.Controls {

    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using JetBrains.Annotations;
    using Logging;
    using Microsoft.Win32;
    using Persistence;
    using Persistence.InIFiles;

    public static class FormExtensions {

        public static void LoadLocation( [NotNull] this Form form, [NotNull] String name, [NotNull] IniFile settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            if ( String.IsNullOrEmpty( value: name ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( name ) );
            }

            if ( Int32.TryParse( settings[ name, nameof( Point.X ) ], out var x ) && Int32.TryParse( settings[ name, nameof( Point.Y ) ], out var y ) ) {
                form.InvokeAction( () => {
                    form.SuspendLayout();
                    form.Location( new Point( x, y ) );
                    form.ResumeLayout();
                } );
            }
        }

        public static void LoadLocation( [NotNull] this Form form ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            var x = AppRegistry.GetInt32( nameof( form.Location ), form.Name, nameof( form.Location.X ) );
            var y = AppRegistry.GetInt32( nameof( form.Location ), form.Name, nameof( form.Location.Y ) );

            if ( x.HasValue && y.HasValue ) {
                form.InvokeAction( () => {
                    form.SuspendLayout();
                    form.Location( new Point( x.Value, y.Value ) );
                    form.ResumeLayout();
                } );
            }
        }

        public static void LoadPosition( [NotNull] this Form form, [NotNull] String name, [NotNull] PersistTable<String, String> settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( String.IsNullOrEmpty( value: name ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( name ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            var key = Cache.BuildKey( name, nameof( form.Location ) );
            var point = settings[ key ].Deserialize<Point>();

            if ( point.X != 0 || point.Y != 0 ) {
                form.Location( point );
            }

            key = Cache.BuildKey( name, nameof( form.Size ) );
            var size = settings[ key ].Deserialize<Size>();

            if ( size.Height != 0 || size.Width != 0 ) {
                form.Size( size );
            }
        }

        public static void LoadPosition( [NotNull] this Form form, [NotNull] String name, [NotNull] StringKVPTable settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( String.IsNullOrEmpty( value: name ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( name ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            var key = Cache.BuildKey( name, nameof( form.Location ) );
            var point = settings[ key ].Deserialize<Point>();

            if ( point.X != 0 || point.Y != 0 ) {
                form.Location( point );
            }

            key = Cache.BuildKey( name, nameof( form.Size ) );
            var size = settings[ key ].Deserialize<Size>();

            if ( size.Height != 0 || size.Width != 0 ) {
                form.Size( size );
            }
        }

        /// <summary>
        ///     <seealso cref="SaveSize(Form)" />
        /// </summary>
        /// <param name="form"></param>
        public static void LoadSize( [NotNull] this Form form ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( AppRegistry.TheApplication == null ) {
                throw new InvalidOperationException( "Application registry not set up." );
            }

            $"Loading form {form.Name} position from registry.".Log();

            var width = AppRegistry.GetInt32( nameof( form.Size ), form.Name, nameof( form.Size.Width ) );
            var height = AppRegistry.GetInt32( nameof( form.Size ), form.Name, nameof( form.Size.Height ) );

            if ( width.HasValue && height.HasValue ) {
                form.InvokeAction( () => {
                    form.SuspendLayout();
                    form.Size( new Size( width.Value, height.Value ) );
                    form.ResumeLayout();
                } );
            }
        }

        public static void LoadSize( [NotNull] this Form form, [NotNull] String name, [NotNull] IniFile settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            if ( String.IsNullOrEmpty( value: name ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( name ) );
            }

            if ( Int32.TryParse( settings[ name, nameof( form.Size.Width ) ], out var width ) &&
                 Int32.TryParse( settings[ name, nameof( form.Size.Height ) ], out var height ) ) {
                form.InvokeAction( () => {
                    form.SuspendLayout();
                    form.Size( new Size( width, height ) );
                    form.ResumeLayout();
                } );
            }
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Location" /> of a <see cref="Form" /> across threads.
        /// </summary>
        /// <remarks></remarks>
        public static void Location( [NotNull] this Form form, Point location ) {
            if ( form == null ) {
                throw new ArgumentNullException( paramName: nameof( form ) );
            }

            form.InvokeAction( () => {
                form.SuspendLayout();
                form.Location = location;
                form.ResumeLayout();
            } );
        }

        public static void SaveLocation( [NotNull] this Form form ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( AppRegistry.TheApplication == null ) {
                throw new InvalidOperationException( "Application registry not set up." );
            }

            $"Saving form {form.Name} position to registry.".Log();

            AppRegistry.Set( nameof( form.Location ), form.Name, nameof( form.Location.X ),
                form.WindowState == FormWindowState.Normal ? form.Location.X : form.RestoreBounds.Location.X, RegistryValueKind.DWord );

            AppRegistry.Set( nameof( form.Location ), form.Name, nameof( form.Location.Y ),
                form.WindowState == FormWindowState.Normal ? form.Location.Y : form.RestoreBounds.Location.Y, RegistryValueKind.DWord );
        }

        public static void SaveLocation( [NotNull] this Form form, [NotNull] IniFile settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            var name = form.Name ?? "UnknownForm";

            settings[ name, nameof( form.Location.X ) ] = form.WindowState == FormWindowState.Normal ? form.Location.X.ToString() : form.RestoreBounds.Location.X.ToString();
            settings[ name, nameof( form.Location.Y ) ] = form.WindowState == FormWindowState.Normal ? form.Location.Y.ToString() : form.RestoreBounds.Location.Y.ToString();
        }

        public static void SaveLocation( [NotNull] this Form form, String name, [NotNull] PersistTable<String, String> settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( paramName: nameof( settings ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings[ Cache.BuildKey( name, nameof( form.Location ) ) ] = form.Location.ToJSON();
        }

        public static void SaveLocation( [NotNull] this Form form, String name, [NotNull] StringKVPTable settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( paramName: nameof( settings ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings[ Cache.BuildKey( name, nameof( form.Location ) ) ] = form.Location.ToJSON();
        }

        public static void SaveLocation( [NotNull] this Form form, String name, [NotNull] IniFile settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( paramName: nameof( settings ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings[ nameof( form.Location ), Cache.BuildKey( name, nameof( form.Location ) ) ] = form.Location.ToJSON();
        }

        /// <summary>
        ///     <seealso cref="LoadSize(Form)" />
        /// </summary>
        /// <param name="form"></param>
        public static void SaveSize( [NotNull] this Form form ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( AppRegistry.TheApplication == null ) {
                throw new InvalidOperationException( "Application registry not set up." );
            }

            $"Saving form {form.Name} position to registry.".Log();

            AppRegistry.Set( nameof( form.Size ), form.Name, nameof( form.Size.Width ),
                form.WindowState == FormWindowState.Normal ? form.Size.Width : form.RestoreBounds.Size.Width, RegistryValueKind.DWord );

            AppRegistry.Set( nameof( form.Size ), form.Name, nameof( form.Size.Height ),
                form.WindowState == FormWindowState.Normal ? form.Size.Height : form.RestoreBounds.Size.Height, RegistryValueKind.DWord );
        }

        public static void SaveSize( [NotNull] this Form form, [NotNull] IniFile settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            var name = form.Name ?? "UnknownForm";

            settings[ name, nameof( form.Size.Width ) ] = form.WindowState == FormWindowState.Normal ? form.Size.Width.ToString() : form.RestoreBounds.Size.Width.ToString();

            settings[ name, nameof( form.Size.Height ) ] =
                form.WindowState == FormWindowState.Normal ? form.Size.Height.ToString() : form.RestoreBounds.Size.Height.ToString();
        }

        public static void SaveSize( [NotNull] this Form form, String name, [NotNull] PersistTable<String, String> settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( paramName: nameof( settings ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings[ Cache.BuildKey( name, nameof( form.Size ) ) ] = form.Size.ToJSON();
        }

        public static void SaveSize( [NotNull] this Form form, String name, [NotNull] StringKVPTable settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( paramName: nameof( settings ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings[ Cache.BuildKey( name, nameof( form.Size ) ) ] = form.Size.ToJSON();
        }

        public static void SaveSize( [NotNull] this Form form, String name, [NotNull] IniFile settings ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( paramName: nameof( settings ) );
            }

            if ( settings == null ) {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings[ nameof( form.Size ), Cache.BuildKey( name, nameof( form.Size ) ) ] = form.Size.ToJSON();
        }

        /// <summary>
        ///     Safely get the <see cref="Form.Size" />() of a <see cref="Form" /> across threads.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static Size Size( [NotNull] this Form form ) {
            if ( form == null ) {
                throw new ArgumentNullException( paramName: nameof( form ) );
            }

            return form.InvokeRequired ? ( Size ) form.Invoke( new Func<Size>( () => form.Size ) ) : form.Size;
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Text" /> of a control across threads.
        /// </summary>
        /// <remarks></remarks>
        public static void Size( [NotNull] this Form form, Size size ) {
            if ( form == null ) {
                throw new ArgumentNullException( paramName: nameof( form ) );
            }

            form.InvokeAction( () => {
                if ( form.IsDisposed ) {
                    return;
                }

                form.SuspendLayout();
                form.Size = size;
                form.ResumeLayout();
            } );
        }
    }
}