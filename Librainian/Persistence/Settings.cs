// Copyright 2018, RS&I Inc.
// Authors: Rick Harker, Scott Rydalch, and Dave Reese.
// 
// Settings.cs last touched on 2018-08-31 at 5:16 PM

namespace Librainian.Persistence {

	using System;
	using Extensions;
	using JetBrains.Annotations;
	using Logging;
	using Microsoft.Win32;
	using Parsing;

	/// <summary>
	///     Store and retrieve values from the registry.
	/// </summary>
	public static class Settings {

		public static Boolean? GetBoolean(TrimmedString location, TrimmedString key) {
			var result = GetObject(location, key);

			return result is null ? (Boolean?)null : Convert.ToBoolean(result);
		}

		public static Byte? GetByte(TrimmedString location, TrimmedString key) {
			var result = GetObject(location, key);

			return result is null ? (Byte?)null : Convert.ToByte(result);
		}

		public static Int32? GetInt32(TrimmedString location, TrimmedString key) {
			var result = GetObject(location, key);

			return result is null ? (Int32?)null : Convert.ToInt32(result);
		}

		public static Int64? GetInt64(TrimmedString location, TrimmedString key) {
			var result = GetObject(location, key);

			return result is null ? (Int64?)null : Convert.ToInt64(result);
		}

		/// <summary>
		///     <para>Gets the value of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="location"></param>
		/// <param name="key">     </param>
		/// <returns></returns>
		[CanBeNull]
		public static Object GetObject(TrimmedString location, TrimmedString key) => Registry.Get(location, key);

		[CanBeNull]
		public static String GetString(TrimmedString location, TrimmedString key) =>
			Convert.ToString(GetObject(location, key)).NullIf(String.Empty);

		/// <summary>
		///     <para>Sets the <paramref name="value" /> of the current user's software's company's application's folder's key.</para>
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key">   </param>
		/// <param name="value"> </param>
		public static void Set(TrimmedString folder, TrimmedString key, [CanBeNull] Object value) {
			try {
				if (folder.IsEmpty()) {
					throw new ArgumentException(message: "Value cannot be null or whitespace.", paramName: nameof(folder));
				}

				if (key.IsEmpty()) {
					throw new ArgumentException(message: "Value cannot be null or whitespace.", paramName: nameof(key));
				}

				if (value is null) {
					Registry.Set(folder, key, null, RegistryValueKind.DWord);

					return;
				}

				switch (value) {
					case String s: {
							Registry.Set(folder, key, s);

							break;
						}

					case UInt64 u64: {
							Registry.Set(folder, key, u64, RegistryValueKind.QWord);

							break;
						}

					case Int64 i64: {
							Registry.Set(folder, key, i64, RegistryValueKind.QWord);

							break;
						}

					case UInt32 u32: {
							Registry.Set(folder, key, u32, RegistryValueKind.DWord);

							break;
						}
					case Int32 i32: {
							Registry.Set(folder, key, i32, RegistryValueKind.DWord);

							break;
						}

					case Boolean b: {
							Registry.Set(folder, key, b ? 1 : 0, RegistryValueKind.DWord);

							break;
						}
					case Enum e: {
							Registry.Set(folder, key, e, RegistryValueKind.DWord);

							break;
						}
					default: {
							$"Registry: unknown type {value}. Probably just a Nullable<T>().".Log();
							Registry.Set(folder, key, value, RegistryValueKind.Unknown);

							break;
						}
				}
			}
			catch (Exception exception) {
				exception.Log();
			}
		}

	}

}