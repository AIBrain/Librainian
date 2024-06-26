﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "Sources.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

namespace Librainian.Interfaces;

public enum Sources {

	Unknown = 0,

	Default = 1,

	Self,

	/// <summary>Just info.</summary>
	Info,

	/// <summary>the user typed, pasted, or entered text into the input box</summary>
	UserText,

	/// <summary>the user clicked a button or interacted with the userinerface in some way.</summary>
	UserAction,

	/// <summary>the user dragged a folder/file over to the ai</summary>
	DragAndDrop,

	Document,

	Folder,

	Ear,

	Mouth,

	Tongue,

	Throat,

	AudioIn,

	LineIn,

	Aux,

	/// <summary>both ears?</summary>
	Stereo,

	Hand,

	Foot,

	Wikipedia,

	Internet,

	Url,

	Uri,

	FacebookPerson,

	FacebookPost,

	FacebookVideo,

	/// <summary>undefined social-internet</summary>
	SocialMediaPerson,

	SocialMediaPost,

	SocialMediaVideo,

	RedditUser,

	RedditPost,

	Twitter,

	/// <summary>a message from an ai</summary>
	AI,

	Back,

	Thigh,

	Knee,

	Ankle,

	Heel,

	BigToe,

	LittleToe,

	MiddleToe,

	/// <summary>direct message from a discord "user"</summary>
	DiscordUser,

	/// <summary>a message from a discord "channel"</summary>
	DiscordChannel,

	/// <summary>aka discord "server"</summary>
	DiscordGuild,

	/// <summary>Message was produced from a cortex.</summary>
	Cortex
}