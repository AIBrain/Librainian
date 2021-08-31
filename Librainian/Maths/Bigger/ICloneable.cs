namespace Librainian.Maths.Bigger {

	/// <summary>
	///     The ICloneable&lt;T&gt; interface represents an object that can clone itself.
	///     The interface defines a single method, <see cref="Duplicate" />
	///     which is called to create a clone of the instance.
	/// </summary>
	/// <typeparam name="T">The type the implemented Clone function is to return.</typeparam>
	public interface ICloneable<out T> {

		/// <summary>
		///     Returns a clone of this instance.
		/// </summary>
		T Duplicate();
	}
}