// Copyright 2018 Protiguous.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Lynara/SystemRestorePoints.cs" was last cleaned by Protiguous on 2018/02/10 at 10:23 PM

namespace Librainian.OperatingSystem {
	using System;
	using Microsoft.VisualBasic;

	public static class SystemRestorePoints {

		/// <summary>
		/// Untested.
		/// </summary>
		/// <param name="title"></param>
		/// <returns></returns>
		public static Boolean CreateRestorePoint(String title = null ) {
			if ( String.IsNullOrWhiteSpace( value: title ) ) {
				var now = DateTime.Now;
				title = "Restore point at " + now.ToLongDateString() + " " + now.ToLongTimeString();
			}

			dynamic restorePoint = Interaction.GetObject( "winmgmts:\\\\.\\root\\default:Systemrestore" );

			return restorePoint?.CreateRestorePoint( title, 0, 100 ) == 0;
		}
	}
}