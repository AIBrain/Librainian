﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
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
    using System.Linq;
    using System.Windows.Forms;
    using JetBrains.Annotations;
    using Logging;
    using Microsoft.Win32;
    using Persistence;

    public static class FormExtensions {

        public static Boolean IsFullyVisibleOnAnyScreen( [NotNull] this Form form ) {
            if ( form is null ) {
                throw new ArgumentNullException( paramName: nameof( form ) );
            }

            var desktopBounds = form.DesktopBounds;
            return Screen.AllScreens.Any( screen => screen.WorkingArea.Contains( desktopBounds ) );
        }

        public static Boolean IsVisibleOnAnyScreen( this Rectangle rect ) => Screen.AllScreens.Any( screen => screen.WorkingArea.IntersectsWith( rect ) );

        public static void LoadLocation( [NotNull] this Form form ) {
            if ( form is null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            var x = AppRegistry.GetInt32( nameof( form.DesktopLocation.X ), form.Name, nameof( form.DesktopLocation.X ) );
            var y = AppRegistry.GetInt32( nameof( form.DesktopLocation.Y ), form.Name, nameof( form.DesktopLocation.Y ) );

            if ( x.HasValue && y.HasValue ) {
                form.InvokeAction( () => form.SetDesktopLocation( x.Value, y.Value ) );
            }
        }

        /// <summary>
        ///     <seealso cref="SaveSize(Form)" />
        /// </summary>
        /// <param name="form"></param>
        public static void LoadSize( [NotNull] this Form form ) {
            if ( form is null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( AppRegistry.TheApplication is null ) {
                throw new InvalidOperationException( "Application registry not set up." );
            }

            $"Loading form {form.Name} position from registry.".Log();

            var width = AppRegistry.GetInt32( nameof( form.Size ), form.Name, nameof( form.Size.Width ) );
            var height = AppRegistry.GetInt32( nameof( form.Size ), form.Name, nameof( form.Size.Height ) );

            if ( width.HasValue && height.HasValue ) {
                form.Size( new Size( width.Value, height.Value ) );
            }
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Location" /> of a <see cref="Form" /> across threads.
        /// </summary>
        /// <remarks></remarks>
        public static void Location( [NotNull] this Form form, Point location ) {
            if ( form is null ) {
                throw new ArgumentNullException( paramName: nameof( form ) );
            }

            form.InvokeAction( () => form.SetDesktopLocation( location.X, location.Y ) );
        }

        public static void SaveLocation( [CanBeNull] this Form form ) {
            if ( form is null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( AppRegistry.TheApplication is null ) {
                throw new InvalidOperationException( "Application registry not set up." );
            }

            $"Saving form {form.Name} position to registry key {AppRegistry.TheApplication.Name}.".Trace();

            AppRegistry.Set( nameof( form.Location ), form.Name ?? throw new InvalidOperationException(), nameof( form.DesktopLocation.X ),
                form.WindowState == FormWindowState.Normal ? form.DesktopLocation.X : form.RestoreBounds.X, RegistryValueKind.DWord );

            AppRegistry.Set( nameof( form.Location ), form.Name, nameof( form.DesktopLocation.Y ),
                form.WindowState == FormWindowState.Normal ? form.DesktopLocation.Y : form.RestoreBounds.Y, RegistryValueKind.DWord );
        }

        /// <summary>
        ///     <seealso cref="LoadSize(Form)" />
        /// </summary>
        /// <param name="form"></param>
        public static void SaveSize( [NotNull] this Form form ) {
            if ( form is null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            if ( AppRegistry.TheApplication is null ) {
                throw new InvalidOperationException( "Application registry not set up." );
            }

            $"Saving form {form.Name} position to registry key {AppRegistry.TheApplication.Name}.".Log();

            AppRegistry.Set( nameof( form.Size ), form.Name, nameof( form.Size.Width ),
                form.WindowState == FormWindowState.Normal ? form.DesktopBounds.Width : form.RestoreBounds.Size.Width, RegistryValueKind.DWord );

            AppRegistry.Set( nameof( form.Size ), form.Name, nameof( form.Size.Height ),
                form.WindowState == FormWindowState.Normal ? form.DesktopBounds.Height : form.RestoreBounds.Size.Height, RegistryValueKind.DWord );
        }

        /// <summary>
        ///     Safely get the <see cref="Form.Size" />() of a <see cref="Form" /> across threads.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static Size Size( [NotNull] this Form form ) {
            if ( form is null ) {
                throw new ArgumentNullException( paramName: nameof( form ) );
            }

            return form.InvokeRequired ? ( Size )form.Invoke( new Func<Size>( () => form.Size ) ) : form.Size;
        }

        /// <summary>
        ///     Safely set the <see cref="Control.Text" /> of a control across threads.
        /// </summary>
        /// <remarks></remarks>
        public static void Size( [NotNull] this Form form, Size size ) {
            if ( form is null ) {
                throw new ArgumentNullException( paramName: nameof( form ) );
            }

            form.InvokeAction( () => form.Size = size );
        }
    }
}