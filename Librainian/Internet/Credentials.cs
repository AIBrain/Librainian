// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "Credentials.cs" last formatted on 2020-08-14 at 8:34 PM.

namespace Librainian.Internet;

using System;
using System.Diagnostics;
using Exceptions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Parsing;

/// <summary>Simple container for a <see cref="Username" /> and <see cref="Password" />.</summary>
[JsonObject]
public class Credentials {

	private String? _password;

	private String? _username;

	/// <summary>Alias for <see cref="Username" />.</summary>
	[JsonIgnore]
	public String? Name {
		[CanBeNull]
		get => this.Username;

		set => this.Username = value;
	}

	/// <summary>Alias for <see cref="Password" />.</summary>
	[JsonIgnore]
	public String? Pass {
		[CanBeNull]
		get => this.Password;

		set => this.Password = value;
	}

	/// <summary>Alias for <see cref="Password" />.</summary>
	[JsonIgnore]
	public String? Passcode {
		[CanBeNull]
		get => this.Password;

		set => this.Password = value;
	}

	/// <summary>Alias for <see cref="Password" />.</summary>
	[JsonIgnore]
	public String? PassCode {
		[CanBeNull]
		get => this.Password;

		set => this.Password = value;
	}

	/// <summary>The password property.</summary>
	public String? Password {
		get => this._password;
		set => this._password = value.Trimmed();
	}

	/// <summary>Alias for <see cref="Password" />.</summary>
	[JsonIgnore]
	public String? PassWord {
		[CanBeNull]
		get => this.Password;

		set => this.Password = value;
	}

	/// <summary>Alias for <see cref="Username" />.</summary>
	[JsonIgnore]
	public String? User {
		[CanBeNull]
		get => this.Username;

		set => this.Username = value;
	}

	/// <summary>Alias for <see cref="Username" />.</summary>
	[JsonIgnore]
	public String? Userid {
		[CanBeNull]
		get => this.Username;

		set => this.Username = value;
	}

	/// <summary>Alias for <see cref="Username" />.</summary>
	[JsonIgnore]
	public String? UserId {
		[CanBeNull]
		get => this.Username;

		set => this.Username = value;
	}

	/// <summary>Alias for <see cref="Username" />.</summary>
	[JsonIgnore]
	public String? UserID {
		[CanBeNull]
		get => this.Username;

		set => this.Username = value;
	}

	/// <summary>The *real* Username instance.</summary>
	public String? Username {
		get => this._username;
		set => this._username = value.Trimmed();
	}

	/// <summary>Alias for <see cref="Username" />.</summary>
	[JsonIgnore]
	public String? UserName {
		[CanBeNull]
		get => this.Username;

		set => this.Username = value;
	}

	/// <summary>
	///     Populates a <see cref="Credentials" /> object with the given <paramref name="username" /> and
	///     <paramref name="password" />.
	///     <para>Call <see cref="Validate" /> to confirm <see cref="Username" /> and <see cref="Password" /> are set.</para>
	/// </summary>
	/// <param name="username">Accepts Base64 encoded strings.</param>
	/// <param name="password">Accepts Base64 encoded strings.</param>
	public Credentials( String username, String password ) {
		if ( String.IsNullOrWhiteSpace( username ) ) {
			throw new ArgumentEmptyException( nameof( username ) );
		}

		if ( String.IsNullOrWhiteSpace( password ) ) {
			throw new ArgumentEmptyException( nameof( password ) );
		}

		this.Username = username.EndsWith( "=" ) ? username.FromBase64() : username;
		this.Password = password.EndsWith( "=" ) ? password.FromBase64() : password;
	}

	public override String ToString() => $"{this.Username ?? Symbols.Null} : {this.Password ?? Symbols.Null}";

	/// <summary>Ensure <see cref="Username" /> and <see cref="Password" /> are not null, empty, or whitespace.</summary>
	[DebuggerStepThrough]
	public Status Validate() {
		if ( String.IsNullOrWhiteSpace( this.Username ) || String.IsNullOrWhiteSpace( this.Password ) ) {
			return Status.Failure;
		}

		return Status.Success;
	}
}