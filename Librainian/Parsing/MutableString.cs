

namespace Librainian.Parsing
{

	using System;
	using System.Text;

	/// <summary>
	/// Mutable extended string type.
	/// </summary>
	public class MutableString {

		private const Int32 DefaultExtraBufferSize = 2048;
		private readonly StringBuilder _value;

		public MutableString() => this._value = new StringBuilder();
		public MutableString( Int32 capacity ) => this._value = new StringBuilder( capacity );
		public MutableString( Int32 capacity, Int32 maxCapacity ) => this._value = new StringBuilder( capacity, maxCapacity );

		public MutableString( StringBuilder value, Int32? bufferCapacity = DefaultExtraBufferSize ) : this() {
			this.Append( value.ToString() );
			try {
				this._value.Capacity += bufferCapacity ?? DefaultExtraBufferSize;
			}
			catch ( ArgumentOutOfRangeException ) {
			}
		}

		public MutableString( String value, Int32? bufferCapacity = DefaultExtraBufferSize ) : this() {
			this.Append( value );
			try {
				this._value.Capacity += bufferCapacity ?? DefaultExtraBufferSize;
			}
			catch ( ArgumentOutOfRangeException ) {
			}
		}

		public Char this[Int32 i]
		{
			get => this._value[i];
			set => this._value[i] = value;
		}

		public MutableString this[Int32 startIndex, Int32 length] => this.Value.Substring(startIndex, length);

		public Char this[Index i]
		{
			get => this._value[i];
			set => this._value[i] = value;
		}

		public MutableString this[Range r] => this.Value[r];

		public void Clear() => this._value.Clear();

		public Int32 Length => this._value.Length;

		public String Value
		{
			get => this._value.ToString();
			set
			{
				this.Clear();
				this.Append(value);
			}
		}

		public void Replace( MutableString oldValue, MutableString newValue ) => this._value.Replace( oldValue, newValue );

		public void Remove( Int32 startIndex, Int32 length ) => this._value.Remove( startIndex, length );

		public void Append( String value ) => this._value.Append( value );

		public void Append( MutableString value ) => this._value.Append( value );

		public static MutableString operator +(MutableString a, MutableString b)
		{
			a.Append(b);
			return a;
		}

		public static implicit operator String(MutableString value) => value.Value;

		public static implicit operator MutableString(String value) => new(value);

		public MutableString Copy() => new(this._value);

		public override String ToString() => this.Value;
	}
}