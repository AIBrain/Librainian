// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "IIni.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "IIni.cs" was last formatted by Protiguous on 2018/11/19 at 8:54 PM.

namespace Librainian.Persistence {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using System.Threading.Tasks;
	using ComputerSystem.FileSystem;
	using JetBrains.Annotations;

	public interface IIni {

		/// <summary>
		/// </summary>
		Document Document { get; set; }

		Guid ID { get; }

		IReadOnlyList<Section> Sections { get; }

		/// <summary>
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key">    </param>
		/// <returns></returns>
		String this[ [CanBeNull] String section, String key ] { [DebuggerStepThrough] [CanBeNull] get; [DebuggerStepThrough] set; }

		/// <summary>
		///     <para>Add in all of the sections, and key-value-pairs from the <see cref="IniFile" />.</para>
		///     <para>Performs a file save.</para>
		/// </summary>
		/// <param name="iniFile"></param>
		/// <returns></returns>
		Boolean Add( [CanBeNull] IniFile iniFile );

		/// <summary>
		///     Removes all data from all sections.
		/// </summary>
		/// <returns></returns>
		Boolean Clear();

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		///     <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise,
		///     <see langword="false" />.
		/// </returns>
		Boolean Equals( Ini other );

		/// <summary>
		///     Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///     <see langword="true" /> if the specified object is equal to the current object; otherwise,
		///     <see langword="false" />.
		/// </returns>
		Boolean Equals( Object obj );

		/// <summary>
		///     Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		Int32 GetHashCode();

		Task<Boolean> ReloadAsync( CancellationToken token = default );

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		String ToString();

		Boolean TryRemove( [NotNull] String section );

		Boolean TryRemove( [NotNull] String section, String key );

		/// <summary>
		///     Saves the <see cref="Ini.Data" /> to the <see cref="Ini.Document" />.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<Boolean> WriteAsync( CancellationToken cancellationToken = default );

	}

}