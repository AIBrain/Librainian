// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ZipStorer.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ZipStorer.cs" was last formatted by Protiguous on 2018/07/13 at 1:32 AM.

namespace Librainian.OperatingSystem.Compression {

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using System.Text;
	using JetBrains.Annotations;
	using Magic;

	/// <summary>
	///     Class for compression/decompression file. Represents a Zip file.
	/// </summary>
	/// <see cref="http://zipstorer.codeplex.com/" />
	public class ZipStorer : ABetterClassDispose {

		// Static CRC32 Table
		private static readonly UInt32[] CrcTable;

		// Default filename encoder
		private static readonly Encoding DefaultEncoding = Encoding.GetEncoding( 437 );

		// List of files to store
		private readonly List<ZipFileEntry> _files = new List<ZipFileEntry>();

		// File access for Open method
		private FileAccess _access;

		// Central dir image
		private Byte[] _centralDirImage;

		// General comment
		private String _comment = "";

		// Existing files in zip
		private UInt16 _existingFiles;

		// Filename of storage file
		private String _fileName;

		// Stream object of storage file
		private Stream _zipFileStream;

		/// <summary>
		///     True if UTF8 encoding for filename and comments, false if default (CP 437)
		/// </summary>
		public Boolean EncodeUtf8;

		/// <summary>
		///     Force deflate algorithm even if it inflates the stored file. Off by default.
		/// </summary>
		public Boolean ForceDeflating;

		// Static constructor. Just invoked once in order to create the CRC32 lookup table.
		static ZipStorer() {

			// Generate CRC32 table
			CrcTable = new UInt32[ 256 ];

			for ( var i = 0; i < CrcTable.Length; i++ ) {
				var c = ( UInt32 )i;

				for ( var j = 0; j < 8; j++ ) {
					if ( ( c & 1 ) != 0 ) { c = 3988292384 ^ (c >> 1); }
					else { c >>= 1; }
				}

				CrcTable[ i ] = c;
			}
		}

		/// <summary>
		///     Compression method enumeration
		/// </summary>
		public enum Compression : UInt16 {

			/// <summary>
			///     Uncompressed storage
			/// </summary>
			Store = 0,

			/// <summary>
			///     Deflate compression method
			/// </summary>
			Deflate = 8
		}

		private static UInt32 DateTimeToDosTime( DateTime dt ) => ( UInt32 )( (dt.Second / 2) | (dt.Minute << 5) | (dt.Hour << 11) | (dt.Day << 16) | (dt.Month << 21) | ((dt.Year - 1980) << 25) );

		private static DateTime DosTimeToDateTime( UInt32 dt ) =>
			new DateTime( ( Int32 )( dt >> 25 ) + 1980, ( Int32 )( dt >> 21 ) & 15, ( Int32 )( dt >> 16 ) & 31, ( Int32 )( dt >> 11 ) & 31, ( Int32 )( dt >> 5 ) & 63, ( Int32 )( dt & 31 ) * 2 );

		// Replaces backslashes with slashes to store in zip header
		[NotNull]
		private static String NormalizedFilename( [NotNull] String _filename ) {
			var filename = _filename.Replace( '\\', '/' );

			var pos = filename.IndexOf( ':' );

			if ( pos >= 0 ) { filename = filename.Remove( 0, pos + 1 ); }

			return filename.Trim( '/' );
		}

		// Calculate the file offset by reading the corresponding local header
		private UInt32 GetFileOffset( UInt32 headerOffset ) {
			var buffer = new Byte[ 2 ];

			this._zipFileStream.Seek( headerOffset + 26, SeekOrigin.Begin );
			this._zipFileStream.Read( buffer, 0, 2 );
			var filenameSize = BitConverter.ToUInt16( buffer, 0 );
			this._zipFileStream.Read( buffer, 0, 2 );
			var extraSize = BitConverter.ToUInt16( buffer, 0 );

			return ( UInt32 )( 30 + filenameSize + extraSize + headerOffset );
		}

		// Reads the end-of-central-directory record
		private Boolean ReadFileInfo() {
			if ( this._zipFileStream.Length < 22 ) { return false; }

			try {
				this._zipFileStream.Seek( -17, SeekOrigin.End );
				var br = new BinaryReader( this._zipFileStream );

				do {
					this._zipFileStream.Seek( -5, SeekOrigin.Current );
					var sig = br.ReadUInt32();

					if ( sig == 0x06054b50 ) {
						this._zipFileStream.Seek( 6, SeekOrigin.Current );

						var entries = br.ReadUInt16();
						var centralSize = br.ReadInt32();
						var centralDirOffset = br.ReadUInt32();
						var commentSize = br.ReadUInt16();

						// check if comment field is the very last data in file
						if ( this._zipFileStream.Position + commentSize != this._zipFileStream.Length ) { return false; }

						// Copy entire central directory to a memory buffer
						this._existingFiles = entries;
						this._centralDirImage = new Byte[ centralSize ];
						this._zipFileStream.Seek( centralDirOffset, SeekOrigin.Begin );
						this._zipFileStream.Read( this._centralDirImage, 0, centralSize );

						// Leave the pointer at the beginning of central dir, to append new files
						this._zipFileStream.Seek( centralDirOffset, SeekOrigin.Begin );

						return true;
					}
				} while ( this._zipFileStream.Position > 0 );
			}
			catch {

				// ignored
			}

			return false;
		}

		// Copies all source file into storage file
		private void Store( ref ZipFileEntry zfe, [NotNull] Stream source ) {
			var buffer = new Byte[ 16384 ];
			Int32 bytesRead;
			UInt32 totalRead = 0;

			var posStart = this._zipFileStream.Position;
			var sourceStart = source.Position;

			var outStream = zfe.Method == Compression.Store ? this._zipFileStream : new DeflateStream( this._zipFileStream, CompressionMode.Compress, true );

			zfe.Crc32 = 0 ^ 0xffffffff;

			do {
				bytesRead = source.Read( buffer, 0, buffer.Length );
				totalRead += ( UInt32 )bytesRead;

				if ( bytesRead > 0 ) {
					outStream.Write( buffer, 0, bytesRead );

					for ( UInt32 i = 0; i < bytesRead; i++ ) { zfe.Crc32 = CrcTable[ ( zfe.Crc32 ^ buffer[ i ] ) & 0xFF ] ^ (zfe.Crc32 >> 8); }
				}
			} while ( bytesRead == buffer.Length );

			outStream.Flush();

			if ( zfe.Method == Compression.Deflate ) { outStream.Dispose(); }

			zfe.Crc32 ^= 0xffffffff;
			zfe.FileSize = totalRead;
			zfe.CompressedSize = ( UInt32 )( this._zipFileStream.Position - posStart );

			// Verify for real compression
			if ( zfe.Method == Compression.Deflate && !this.ForceDeflating && source.CanSeek && zfe.CompressedSize > zfe.FileSize ) {

				// Start operation again with Store algorithm
				zfe.Method = Compression.Store;
				this._zipFileStream.Position = posStart;
				this._zipFileStream.SetLength( posStart );
				source.Position = sourceStart;
				this.Store( ref zfe, source );
			}
		}

		private void UpdateCrcAndSizes( ref ZipFileEntry zfe ) {
			var lastPos = this._zipFileStream.Position; // remember position

			this._zipFileStream.Position = zfe.HeaderOffset + 8;
			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )zfe.Method ), 0, 2 ); // zipping method

			this._zipFileStream.Position = zfe.HeaderOffset + 14;
			this._zipFileStream.Write( BitConverter.GetBytes( zfe.Crc32 ), 0, 4 ); // Update CRC
			this._zipFileStream.Write( BitConverter.GetBytes( zfe.CompressedSize ), 0, 4 ); // Compressed size
			this._zipFileStream.Write( BitConverter.GetBytes( zfe.FileSize ), 0, 4 ); // Uncompressed size

			this._zipFileStream.Position = lastPos; // restore position
		}

		private void WriteCentralDirRecord( ZipFileEntry zfe ) {
			var encoder = zfe.EncodeUtf8 ? Encoding.UTF8 : DefaultEncoding;
			var encodedFilename = encoder.GetBytes( zfe.FilenameInZip );
			var encodedComment = encoder.GetBytes( zfe.Comment );

			this._zipFileStream.Write( new Byte[] {
				80, 75, 1, 2, 23, 0xB, 20, 0
			}, 0, 8 );

			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )( zfe.EncodeUtf8 ? 0x0800 : 0 ) ), 0, 2 ); // filename and comment encoding
			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )zfe.Method ), 0, 2 ); // zipping method
			this._zipFileStream.Write( BitConverter.GetBytes( DateTimeToDosTime( zfe.ModifyTime ) ), 0, 4 ); // zipping date and time
			this._zipFileStream.Write( BitConverter.GetBytes( zfe.Crc32 ), 0, 4 ); // file CRC
			this._zipFileStream.Write( BitConverter.GetBytes( zfe.CompressedSize ), 0, 4 ); // compressed file size
			this._zipFileStream.Write( BitConverter.GetBytes( zfe.FileSize ), 0, 4 ); // uncompressed file size
			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )encodedFilename.Length ), 0, 2 ); // Filename in zip
			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )0 ), 0, 2 ); // extra length
			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )encodedComment.Length ), 0, 2 );

			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )0 ), 0, 2 ); // disk=0
			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )0 ), 0, 2 ); // file type: binary
			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )0 ), 0, 2 ); // Internal file attributes
			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )0x8100 ), 0, 2 ); // External file attributes (normal/readable)
			this._zipFileStream.Write( BitConverter.GetBytes( zfe.HeaderOffset ), 0, 4 ); // Offset of header

			this._zipFileStream.Write( encodedFilename, 0, encodedFilename.Length );
			this._zipFileStream.Write( encodedComment, 0, encodedComment.Length );
		}

		private void WriteEndRecord( UInt32 size, UInt32 offset ) {
			var encoder = this.EncodeUtf8 ? Encoding.UTF8 : DefaultEncoding;
			var encodedComment = encoder.GetBytes( this._comment );

			this._zipFileStream.Write( new Byte[] {
				80, 75, 5, 6, 0, 0, 0, 0
			}, 0, 8 );

			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )this._files.Count + this._existingFiles ), 0, 2 );
			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )this._files.Count + this._existingFiles ), 0, 2 );
			this._zipFileStream.Write( BitConverter.GetBytes( size ), 0, 4 );
			this._zipFileStream.Write( BitConverter.GetBytes( offset ), 0, 4 );
			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )encodedComment.Length ), 0, 2 );
			this._zipFileStream.Write( encodedComment, 0, encodedComment.Length );
		}

		private void WriteLocalHeader( ref ZipFileEntry zfe ) {
			var pos = this._zipFileStream.Position;
			var encoder = zfe.EncodeUtf8 ? Encoding.UTF8 : DefaultEncoding;
			var encodedFilename = encoder.GetBytes( zfe.FilenameInZip );

			this._zipFileStream.Write( new Byte[] {
				80, 75, 3, 4, 20, 0
			}, 0, 6 ); // No extra header

			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )( zfe.EncodeUtf8 ? 0x0800 : 0 ) ), 0, 2 ); // filename and comment encoding
			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )zfe.Method ), 0, 2 ); // zipping method
			this._zipFileStream.Write( BitConverter.GetBytes( DateTimeToDosTime( zfe.ModifyTime ) ), 0, 4 ); // zipping date and time

			this._zipFileStream.Write( new Byte[] {
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			}, 0, 12 ); // unused CRC, un/compressed size, updated later

			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )encodedFilename.Length ), 0, 2 ); // filename length
			this._zipFileStream.Write( BitConverter.GetBytes( ( UInt16 )0 ), 0, 2 ); // extra length

			this._zipFileStream.Write( encodedFilename, 0, encodedFilename.Length );
			zfe.HeaderSize = ( UInt32 )( this._zipFileStream.Position - pos );
		}

		/// <summary>
		///     Method to create a new storage file
		/// </summary>
		/// <param name="filename">Full path of Zip file to create</param>
		/// <param name="comment"> General comment for Zip file</param>
		/// <returns>A valid ZipStorer object</returns>
		[NotNull]
		public static ZipStorer Create( [NotNull] String filename, String comment ) {
			Stream stream = new FileStream( filename, FileMode.Create, FileAccess.ReadWrite );

			var zip = Create( stream, comment );
			zip._comment = comment;
			zip._fileName = filename;

			return zip;
		}

		/// <summary>
		///     Method to create a new zip storage in a stream
		/// </summary>
		/// <param name="stream"> </param>
		/// <param name="comment"></param>
		/// <returns>A valid ZipStorer object</returns>
		[NotNull]
		public static ZipStorer Create( Stream stream, String comment ) {
			var zip = new ZipStorer {
				_comment = comment,
				_zipFileStream = stream,
				_access = FileAccess.Write
			};

			return zip;
		}

		/// <summary>
		///     Method to open an existing storage file
		/// </summary>
		/// <param name="filename">Full path of Zip file to open</param>
		/// <param name="access">  File access mode as used in FileStream constructor</param>
		/// <returns>A valid ZipStorer object</returns>
		[NotNull]
		public static ZipStorer Open( [NotNull] String filename, FileAccess access ) {
			var stream = new FileStream( filename, FileMode.Open, access == FileAccess.Read ? FileAccess.Read : FileAccess.ReadWrite ) as Stream;

			var zip = Open( stream, access );
			zip._fileName = filename;

			return zip;
		}

		/// <summary>
		///     Method to open an existing storage from stream
		/// </summary>
		/// <param name="stream">Already opened stream with zip contents</param>
		/// <param name="access">File access mode for stream operations</param>
		/// <returns>A valid ZipStorer object</returns>
		[NotNull]
		public static ZipStorer Open( [NotNull] Stream stream, FileAccess access ) {
			if ( !stream.CanSeek && access != FileAccess.Read ) { throw new InvalidOperationException( "Stream cannot seek" ); }

			var zip = new ZipStorer {
				_zipFileStream = stream,
				_access = access
			};

			//zip.FileName = _filename;

			if ( zip.ReadFileInfo() ) { return zip; }

			throw new InvalidDataException();
		}

		/// <summary>
		///     Removes one of many files in storage. It creates a new Zip file.
		/// </summary>
		/// <param name="zip"> Reference to the current Zip object</param>
		/// <param name="zfes">List of Entries to remove from storage</param>
		/// <returns>True if success, false if not</returns>
		/// <remarks>This method only works for storage of type FileStream</remarks>
		public static Boolean RemoveEntries( [NotNull] ref ZipStorer zip, List<ZipFileEntry> zfes ) {
			if ( !( zip._zipFileStream is FileStream ) ) { throw new InvalidOperationException( "RemoveEntries is allowed just over streams of type FileStream" ); }

			//Get full list of entries
			var fullList = zip.ReadCentralDir();

			//In order to delete we need to create a copy of the zip file excluding the selected items
			var tempZipName = Path.GetTempFileName();
			var tempEntryName = Path.GetTempFileName();

			try {
				var tempZip = Create( tempZipName, String.Empty );

				foreach ( var zfe in fullList ) {
					if ( !zfes.Contains( zfe ) ) {
						if ( zip.ExtractFile( zfe, tempEntryName ) ) { tempZip.AddFile( zfe.Method, tempEntryName, zfe.FilenameInZip, zfe.Comment ); }
					}
				}

				zip.Close();
				tempZip.Close();

				File.Delete( zip._fileName );
				File.Move( tempZipName, zip._fileName );

				zip = Open( zip._fileName, zip._access );
			}
			catch { return false; }
			finally {
				if ( File.Exists( tempZipName ) ) { File.Delete( tempZipName ); }

				if ( File.Exists( tempEntryName ) ) { File.Delete( tempEntryName ); }
			}

			return true;
		}

		/// <summary>
		///     Add full contents of a file into the Zip storage
		/// </summary>
		/// <param name="method">       Compression method</param>
		/// <param name="pathname">     Full path of file to add to Zip storage</param>
		/// <param name="filenameInZip">Filename and path as desired in Zip directory</param>
		/// <param name="comment">      Comment for stored file</param>
		public void AddFile( Compression method, [NotNull] String pathname, [NotNull] String filenameInZip, String comment ) {
			if ( this._access == FileAccess.Read ) { throw new InvalidOperationException( "Writing is not allowed" ); }

			var stream = new FileStream( pathname, FileMode.Open, FileAccess.Read );
			this.AddStream( method, filenameInZip, stream, File.GetLastWriteTime( pathname ), comment );
			stream.Close();
		}

		/// <summary>
		///     Add full contents of a stream into the Zip storage
		/// </summary>
		/// <param name="method">       Compression method</param>
		/// <param name="filenameInZip">Filename and path as desired in Zip directory</param>
		/// <param name="source">       Stream object containing the data to store in Zip</param>
		/// <param name="modTime">      Modification time of the data to store</param>
		/// <param name="comment">      Comment for stored file</param>
		public void AddStream( Compression method, [NotNull] String filenameInZip, [NotNull] Stream source, DateTime modTime, [CanBeNull] String comment ) {
			if ( this._access == FileAccess.Read ) { throw new InvalidOperationException( "Writing is not allowed" ); }

			//Int64 offset;
			if ( !this._files.Any() ) {

				//offset = 0;
			}
			else {
				var last = this._files[ this._files.Count - 1 ];

				//offset = last.HeaderOffset + last.HeaderSize;
			}

			// Prepare the fileinfo
			var zfe = new ZipFileEntry {
				Method = method,
				EncodeUtf8 = this.EncodeUtf8,
				FilenameInZip = NormalizedFilename( filenameInZip ),
				Comment = comment ?? "",
				Crc32 = 0,
				HeaderOffset = ( UInt32 )this._zipFileStream.Position,
				ModifyTime = modTime
			};

			// Even though we write the header now, it will have to be rewritten, since we don't know compressed size or crc. to be updated later offset within file of the start of this local record

			// Write local header
			this.WriteLocalHeader( ref zfe );
			zfe.FileOffset = ( UInt32 )this._zipFileStream.Position;

			// Write file to zip (store)
			this.Store( ref zfe, source );
			source.Close();

			this.UpdateCrcAndSizes( ref zfe );

			this._files.Add( zfe );
		}

		/// <summary>
		///     Updates central directory (if pertinent) and close the Zip storage
		/// </summary>
		/// <remarks>This is a required step, unless automatic dispose is used</remarks>
		public void Close() {
			if ( this._access != FileAccess.Read ) {
				var centralOffset = ( UInt32 )this._zipFileStream.Position;
				UInt32 centralSize = 0;

				if ( this._centralDirImage != null ) { this._zipFileStream.Write( this._centralDirImage, 0, this._centralDirImage.Length ); }

				foreach ( var t in this._files ) {
					var pos = this._zipFileStream.Position;
					this.WriteCentralDirRecord( t );
					centralSize += ( UInt32 )( this._zipFileStream.Position - pos );
				}

				if ( this._centralDirImage != null ) { this.WriteEndRecord( centralSize + ( UInt32 )this._centralDirImage.Length, centralOffset ); }
				else { this.WriteEndRecord( centralSize, centralOffset ); }
			}

			if ( this._zipFileStream == null ) { return; }

			this._zipFileStream.Flush();
			this._zipFileStream.Dispose();
			this._zipFileStream = null;
		}

		/// <summary>
		///     Dispose any disposable members. Closes the Zip file stream.
		/// </summary>
		public override void DisposeManaged() => this.Close();

		/// <summary>
		///     Copy the contents of a stored file into a physical file
		/// </summary>
		/// <param name="zfe">     Entry information of file to extract</param>
		/// <param name="filename">Name of file to store uncompressed data</param>
		/// <returns>True if success, false if not.</returns>
		/// <remarks>Unique compression methods are Store and Deflate</remarks>
		public Boolean ExtractFile( ZipFileEntry zfe, [NotNull] String filename ) {
			if ( filename == null ) { throw new ArgumentNullException( nameof( filename ) ); }

			// Make sure the parent directory exists
			var path = Path.GetDirectoryName( filename );

			if ( !Directory.Exists( path ) ) { Directory.CreateDirectory( path ); }

			// Check it is directory. If so, do nothing
			if ( Directory.Exists( filename ) ) { return true; }

			Stream output = new FileStream( filename, FileMode.Create, FileAccess.Write );
			var result = this.ExtractFile( zfe, output );

			if ( result ) { output.Close(); }

			File.SetCreationTime( filename, zfe.ModifyTime );
			File.SetLastWriteTime( filename, zfe.ModifyTime );

			return result;
		}

		/// <summary>
		///     Copy the contents of a stored file into an opened stream
		/// </summary>
		/// <param name="zfe">   Entry information of file to extract</param>
		/// <param name="stream">Stream to store the uncompressed data</param>
		/// <returns>True if success, false if not.</returns>
		/// <remarks>Unique compression methods are Store and Deflate</remarks>
		public Boolean ExtractFile( ZipFileEntry zfe, [NotNull] Stream stream ) {
			if ( !stream.CanWrite ) { throw new InvalidOperationException( "Stream cannot be written" ); }

			// check signature
			var signature = new Byte[ 4 ];
			this._zipFileStream.Seek( zfe.HeaderOffset, SeekOrigin.Begin );
			this._zipFileStream.Read( signature, 0, 4 );

			if ( BitConverter.ToUInt32( signature, 0 ) != 0x04034b50 ) { return false; }

			// Select input stream for inflating or just reading
			Stream inStream;

			switch ( zfe.Method ) {
				case Compression.Store:
					inStream = this._zipFileStream;

					break;

				case Compression.Deflate:
					inStream = new DeflateStream( this._zipFileStream, CompressionMode.Decompress, true );

					break;

				default: return false;
			}

			// Buffered copy
			var buffer = new Byte[ 16384 ];
			this._zipFileStream.Seek( zfe.FileOffset, SeekOrigin.Begin );
			var bytesPending = zfe.FileSize;

			while ( bytesPending > 0 ) {
				var bytesRead = inStream.Read( buffer, 0, ( Int32 )Math.Min( bytesPending, buffer.Length ) );
				stream.Write( buffer, 0, bytesRead );
				bytesPending -= ( UInt32 )bytesRead;
			}

			stream.Flush();

			if ( zfe.Method == Compression.Deflate ) { inStream.Dispose(); }

			return true;
		}

		/// <summary>
		///     Read all the file records in the central directory
		/// </summary>
		/// <returns>List of all entries in directory</returns>
		[NotNull]
		public List<ZipFileEntry> ReadCentralDir() {
			if ( this._centralDirImage == null ) { throw new InvalidOperationException( "Central directory currently does not exist" ); }

			var result = new List<ZipFileEntry>();

			for ( var pointer = 0; pointer < this._centralDirImage.Length; ) {
				var signature = BitConverter.ToUInt32( this._centralDirImage, pointer );

				if ( signature != 0x02014b50 ) { break; }

				var encodeUtf8 = ( BitConverter.ToUInt16( this._centralDirImage, pointer + 8 ) & 0x0800 ) != 0;
				var method = BitConverter.ToUInt16( this._centralDirImage, pointer + 10 );
				var modifyTime = BitConverter.ToUInt32( this._centralDirImage, pointer + 12 );
				var crc32 = BitConverter.ToUInt32( this._centralDirImage, pointer + 16 );
				var comprSize = BitConverter.ToUInt32( this._centralDirImage, pointer + 20 );
				var fileSize = BitConverter.ToUInt32( this._centralDirImage, pointer + 24 );
				var filenameSize = BitConverter.ToUInt16( this._centralDirImage, pointer + 28 );
				var extraSize = BitConverter.ToUInt16( this._centralDirImage, pointer + 30 );
				var commentSize = BitConverter.ToUInt16( this._centralDirImage, pointer + 32 );
				var headerOffset = BitConverter.ToUInt32( this._centralDirImage, pointer + 42 );
				var headerSize = ( UInt32 )( 46 + filenameSize + extraSize + commentSize );

				var encoder = encodeUtf8 ? Encoding.UTF8 : DefaultEncoding;

				var zfe = new ZipFileEntry {
					Method = ( Compression )method,
					FilenameInZip = encoder.GetString( this._centralDirImage, pointer + 46, filenameSize ),
					FileOffset = this.GetFileOffset( headerOffset ),
					FileSize = fileSize,
					CompressedSize = comprSize,
					HeaderOffset = headerOffset,
					HeaderSize = headerSize,
					Crc32 = crc32,
					ModifyTime = DosTimeToDateTime( modifyTime )
				};

				if ( commentSize > 0 ) { zfe.Comment = encoder.GetString( this._centralDirImage, pointer + 46 + filenameSize + extraSize, commentSize ); }

				result.Add( zfe );
				pointer += 46 + filenameSize + extraSize + commentSize;
			}

			return result;
		}

		/// <summary>
		///     Represents an entry in Zip file directory
		/// </summary>
		public struct ZipFileEntry {

			/// <summary>
			///     User comment for file
			/// </summary>
			public String Comment;

			/// <summary>
			///     Compressed file size
			/// </summary>
			public UInt32 CompressedSize;

			/// <summary>
			///     32-bit checksum of entire file
			/// </summary>
			public UInt32 Crc32;

			/// <summary>
			///     True if UTF8 encoding for filename and comments, false if default (CP 437)
			/// </summary>
			public Boolean EncodeUtf8;

			/// <summary>
			///     Full path and filename as stored in Zip
			/// </summary>
			public String FilenameInZip;

			/// <summary>
			///     Offset of file inside Zip storage
			/// </summary>
			public UInt32 FileOffset;

			/// <summary>
			///     Original file size
			/// </summary>
			public UInt32 FileSize;

			/// <summary>
			///     Offset of header information inside Zip storage
			/// </summary>
			public UInt32 HeaderOffset;

			/// <summary>
			///     Size of header information
			/// </summary>
			public UInt32 HeaderSize;

			/// <summary>
			///     Compression method
			/// </summary>
			public Compression Method;

			/// <summary>
			///     Last modification time of file
			/// </summary>
			public DateTime ModifyTime;

			/// <summary>
			///     Overridden method
			/// </summary>
			/// <returns>Filename in Zip</returns>
			public override String ToString() => this.FilenameInZip;
		}

		/* Local file header:
            local file header signature     4 bytes  (0x04034b50)
            version needed to extract       2 bytes
            general purpose bit flag        2 bytes
            compression method              2 bytes
            last mod file time              2 bytes
            last mod file date              2 bytes
            crc-32                          4 bytes
            compressed size                 4 bytes
            uncompressed size               4 bytes
            filename length                 2 bytes
            extra field length              2 bytes

            filename (variable size)
            extra field (variable size)
        */
		/* Central directory's File header:
            central file header signature   4 bytes  (0x02014b50)
            version made by                 2 bytes
            version needed to extract       2 bytes
            general purpose bit flag        2 bytes
            compression method              2 bytes
            last mod file time              2 bytes
            last mod file date              2 bytes
            crc-32                          4 bytes
            compressed size                 4 bytes
            uncompressed size               4 bytes
            filename length                 2 bytes
            extra field length              2 bytes
            file comment length             2 bytes
            disk number start               2 bytes
            internal file attributes        2 bytes
            external file attributes        4 bytes
            relative offset of local header 4 bytes

            filename (variable size)
            extra field (variable size)
            file comment (variable size)
        */
		/* End of central dir record:
            end of central dir signature    4 bytes  (0x06054b50)
            number of this disk             2 bytes
            number of the disk with the
            start of the central directory  2 bytes
            total number of entries in
            the central dir on this disk    2 bytes
            total number of entries in
            the central dir                 2 bytes
            size of the central directory   4 bytes
            offset of start of central
            directory with respect to
            the starting disk number        4 bytes
            zipfile comment length          2 bytes
            zipfile comment (variable size)
        */
		/* DOS Date and time:
            MS-DOS date. The date is a packed value with the following format. Bits Description
                0-4 Day of the month (1–31)
                5-8 Month (1 = January, 2 = February, and so on)
                9-15 Year offset from 1980 (add 1980 to get actual year)
            MS-DOS time. The time is a packed value with the following format. Bits Description
                0-4 Second divided by 2
                5-10 Minute (0–59)
                11-15 Hour (0–23 on a 24-hour clock)
        */

		/* CRC32 algorithm
          The 'magic number' for the CRC is 0xdebb20e3.
          The proper CRC pre and post conditioning
          is used, meaning that the CRC register is
          pre-conditioned with all ones (a starting value
          of 0xffffffff) and the value is post-conditioned by
          taking the one's complement of the CRC residual.
          If bit 3 of the general purpose flag is set, this
          field is set to zero in the local header and the correct
          value is put in the data descriptor and in the central
          directory.
        */
	}
}