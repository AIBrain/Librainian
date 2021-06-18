// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "FormExtensions.cs" last touched on 2021-05-05 at 5:51 PM by Protiguous.

#nullable enable

namespace Librainian.Controls {

	using System;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Forms;
	using Exceptions;
	using Logging;
	using Microsoft.Win32;
	using Persistence;

	public static class FormExtensions {

		public static Boolean IsFullyVisibleOnAnyScreen( this Form form ) {
			if ( form is null ) {
				throw new ArgumentEmptyException( nameof( form ) );
			}

			var desktopBounds = form.DesktopBounds;

			return Screen.AllScreens.Any( screen => screen.WorkingArea.Contains( desktopBounds ) );
		}

		public static Boolean IsVisibleOnAnyScreen( this Rectangle rect ) => Screen.AllScreens.Any( screen => screen.WorkingArea.IntersectsWith( rect ) );

		public static void LoadLocation( this Form form ) {
			if ( form is null ) {
				throw new ArgumentEmptyException( nameof( form ) );
			}

			if ( form.Name is null ) {
				throw new ArgumentEmptyException( nameof( form ) );
			}

			var x = AppRegistry.GetInt32( nameof( form.Location ), form.Name, nameof( form.Location.X ) );
			var y = AppRegistry.GetInt32( nameof( form.Location ), form.Name, nameof( form.Location.Y ) );

			if ( x.HasValue && y.HasValue ) {
				form.InvokeAction( () => form.Location = new( x.Value, y.Value ) );
			}
		}

		/// <summary>
		///     <seealso cref="SaveSize(Form)" />
		/// </summary>
		/// <param name="form"></param>
		public static void LoadSize( this Form form ) {
			if ( form is null ) {
				throw new ArgumentEmptyException( nameof( form ) );
			}

			if ( form.Name is null ) {
				throw new ArgumentEmptyException( nameof( form ) );
			}

			if ( AppRegistry.TheApplication is null ) {
				throw new InvalidOperationException( "Application registry not set up." );
			}

			$"Loading form {form.Name} position from registry.".Log();

			var width = AppRegistry.GetInt32( nameof( form.Size ), form.Name, nameof( form.Size.Width ) );
			var height = AppRegistry.GetInt32( nameof( form.Size ), form.Name, nameof( form.Size.Height ) );

			if ( width.HasValue && height.HasValue ) {
				form.Size( new( width.Value, height.Value ) );
			}
		}

		/// <summary>Safely set the <see cref="Control.Location" /> of a <see cref="Form" /> across threads.</summary>
		/// <remarks></remarks>
		public static void Location( this Form form, Point location ) {
			if ( form is null ) {
				throw new ArgumentEmptyException( nameof( form ) );
			}

			form.InvokeAction( () => form.Location = new( location.X, location.Y ) );
		}

		public static void SaveLocation( this Form? form ) {
			if ( form is null ) {
				throw new ArgumentEmptyException( nameof( form ) );
			}

			if ( AppRegistry.TheApplication is null ) {
				throw new InvalidOperationException( "Application registry not set up." );
			}

			//$"Saving form {form.Name} position to registry key {AppRegistry.TheApplication.Name}.".Verbose();

			AppRegistry.Set( nameof( form.Location ), form.Name ?? throw new InvalidOperationException(), nameof( form.Location.X ),
				form.WindowState == FormWindowState.Normal ? form.Location.X : form.RestoreBounds.X, RegistryValueKind.DWord );

			AppRegistry.Set( nameof( form.Location ), form.Name, nameof( form.Location.Y ),
				form.WindowState == FormWindowState.Normal ? form.Location.Y : form.RestoreBounds.Y, RegistryValueKind.DWord );
		}

		/// <summary>
		///     <seealso cref="LoadSize(Form)" />
		/// </summary>
		/// <param name="form"></param>
		public static void SaveSize( this Form form ) {
			if ( form is null ) {
				throw new ArgumentEmptyException( nameof( form ) );
			}

			if ( form.Name is null ) {
				throw new ArgumentEmptyException( nameof( form ) );
			}

			if ( AppRegistry.TheApplication is null ) {
				throw new InvalidOperationException( "Application registry not set up." );
			}

			$"Saving form {form.Name} position to registry key {AppRegistry.TheApplication.Name}.".Log();

			AppRegistry.Set( nameof( form.Size ), form.Name, nameof( form.Size.Width ),
				form.WindowState == FormWindowState.Normal ? form.Size.Width : form.RestoreBounds.Size.Width, RegistryValueKind.DWord );

			AppRegistry.Set( nameof( form.Size ), form.Name, nameof( form.Size.Height ),
				form.WindowState == FormWindowState.Normal ? form.Size.Height : form.RestoreBounds.Size.Height, RegistryValueKind.DWord );
		}

		/// <summary>Safely get the <see cref="Form.Size" />() of a <see cref="Form" /> across threads.</summary>
		/// <param name="form"></param>
		/// <returns></returns>
		public static Size Size( this Form form ) {
			if ( form is null ) {
				throw new ArgumentEmptyException( nameof( form ) );
			}

			return form.InvokeRequired ? ( Size )form.Invoke( new Func<Size>( () => form.Size ) ) : form.Size;
		}

		/// <summary>Safely set the <see cref="Control.Text" /> of a control across threads.</summary>
		/// <remarks></remarks>
		public static void Size( this Form form, Size size ) {
			if ( form is null ) {
				throw new ArgumentEmptyException( nameof( form ) );
			}

			form.InvokeAction( () => form.Size = size );
		}
	}
}