// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "SimpleHierarchyWrapper.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "SimpleHierarchyWrapper.cs" was last formatted by Protiguous on 2018/07/10 at 8:49 PM.

namespace Librainian.AmazedSaint {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using JetBrains.Annotations;

	/// <summary>
	///     A concrete hierarchy wrapper
	/// </summary>
	public class SimpleHierarchyWrapper : IElasticHierarchyWrapper {

		public Object InternalContent { get; set; }

		public String InternalName { get; set; }

		public ElasticObject InternalParent { get; set; }

		public Object InternalValue { get; set; }

		public void AddAttribute( [NotNull] String key, ElasticObject value ) => this.Attributes.TryAdd( key, value );

		public void AddElement( [NotNull] ElasticObject element ) {
			if ( !this.Elements.ContainsKey( element.InternalName ) ) { this.Elements[ element.InternalName ] = new List<ElasticObject>(); }

			this.Elements[ element.InternalName ].Add( element );
		}

		[CanBeNull]
		public ElasticObject Attribute( String name ) => this.HasAttribute( name ) ? this.Attributes[ name ] : null;

		[CanBeNull]
		public ElasticObject Element( String name ) => this.GetElements().FirstOrDefault( item => item.InternalName == name );

		public IEnumerable<KeyValuePair<String, ElasticObject>> GetAttributes() => this.Attributes;

		public Object GetAttributeValue( [NotNull] String name ) => this.Attributes[ name ].InternalValue;

		[NotNull]
		public IEnumerable<ElasticObject> GetElements() => this.Elements.SelectMany( list => list.Value );

		public Boolean HasAttribute( [NotNull] String name ) => this.Attributes.ContainsKey( name );

		public void RemoveAttribute( [NotNull] String key ) => this.Attributes.TryRemove( key, out _ );

		public void RemoveElement( [NotNull] ElasticObject element ) {
			if ( !this.Elements.ContainsKey( element.InternalName ) ) { return; }

			if ( this.Elements[ element.InternalName ].Contains( element ) ) { this.Elements[ element.InternalName ].Remove( element ); }
		}

		public void SetAttributeValue( [NotNull] String name, Object obj ) => this.Attributes[ name ].InternalValue = obj;

		private ConcurrentDictionary<String, ElasticObject> Attributes { get; } = new ConcurrentDictionary<String, ElasticObject>();

		private ConcurrentDictionary<String, List<ElasticObject>> Elements { get; } = new ConcurrentDictionary<String, List<ElasticObject>>();
	}
}