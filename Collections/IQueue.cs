// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "IQueue.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/IQueue.cs" was last formatted by Protiguous on 2018/05/24 at 6:59 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A collection designed for holding elements prior to processing.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Besides basic <see cref="ICollection{T}" /> operations, queues provide additional insertion, extraction, and
    ///         inspection operations.
    ///     </para>
    ///     <para>
    ///         Each of these methods exists in two forms: one throws an exception if the operation fails, the other returns a
    ///         special value. The latter form of the insert operation is designed specifically for use with
    ///         capacity-restricted <see cref="IQueue{T}" /> implementations; in most implementations, insert operations cannot
    ///         fail.
    ///     </para>
    ///     <para>
    ///         Queues typically, but do not necessarily, order elements in a FIFO (first-in-first-out) manner. Among the
    ///         exceptions are priority queues, which order elements according to a supplied comparator, or the elements'
    ///         natural ordering, and LIFO queues (or stacks) which order the elements LIFO (last-in-first-out). Whatever the
    ///         ordering used, the head of the queue is that element which would be removed by a call to
    ///         <see
    ///             cref="Remove" />
    ///         or <see cref="Poll" />. In a FIFO queue, all new elements are inserted at the tail of the queue. Other kinds of
    ///         queues may use different placement rules. Every <see cref="IQueue{T}" /> implementation
    ///         must specify its ordering properties.
    ///     </para>
    ///     <para>
    ///         The <see cref="Offer" /> method inserts an element if possible, otherwise returning <c>false</c>. This differs
    ///         from the <see cref="ICollection{T}.Add" /> method, which can fail to add an element only by throwing an
    ///         exception. The <see cref="Offer" /> method is designed for use when failure is a normal, rather than
    ///         exceptional occurrence, for example, in fixed-capacity (or "bounded") queues.
    ///     </para>
    ///     <para>
    ///         The <see cref="Remove" /> and <see cref="Poll" /> methods remove and return the head of the queue. Exactly
    ///         which element is removed from the queue is a function of the queue's ordering policy, which differs from
    ///         implementation to implementation. The <see cref="Remove" /> and Poll <see cref="Poll" /> methods differ only in
    ///         their behavior when the queue is empty: the <see cref="Remove" /> method throws an exception, while the
    ///         <see cref="Poll" /> method returns <c>false</c>.
    ///     </para>
    ///     <para>The <see cref="Element" /> and <see cref="Peek" /> methods return, but do not remove, the head of the queue.</para>
    ///     <para>
    ///         The <see cref="IQueue{T}" /> interface does not define the blocking queue methods, which are common in
    ///         concurrent programming.
    ///     </para>
    ///     <para>
    ///         <see cref="IQueue{T}" /> implementations generally do not define element-based versions of methods
    ///         <see cref="object.Equals(object)" /> and <see cref="object.GetHashCode" />, but instead inherit the identity
    ///         based
    ///         versions from the class object, because element-based equality is not always well-defined for queues with the
    ///         same elements but different ordering properties.
    ///     </para>
    ///     <para>Based on the back port of JCP JSR-166.</para>
    /// </remarks>
    /// <typeparam name="T">The type of the elements in the queue.</typeparam>
    /// <author>Doug Lea</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <author>Kenneth Xu</author>
    internal interface IQueue<T> : ICollection<T> // JDK_1_6
    {

        /// <summary>
        ///     Gets the remaining capacity of a bounded queue or <see cref="int.MaxValue" /> if the queue is un-bounded.
        /// </summary>
        Int32 RemainingCapacity { get; }

        /// <summary>
        ///     Removes all available elements from this queue and invoke the given <paramref name="action" /> on each element in
        ///     order.
        /// </summary>
        /// <remarks>
        ///     This operation may be more efficient than repeatedly polling this queue. A failure encountered while attempting to
        ///     invoke the <paramref name="action" /> on the elements may result in elements being neither,
        ///     either or both in the queue or processed when the associated exception is thrown.
        ///     <example>
        ///         Drain to a non-generic list.
        ///         <code language="c#">
        /// IList c = ...;
        /// int count = Drain(delegate(T e) {c.Add(e);});
        /// </code>
        ///     </example>
        /// </remarks>
        /// <param name="action">The action to perform on each element.</param>
        /// <returns>The number of elements processed.</returns>
        /// <exception cref="System.InvalidOperationException">If the queue cannot be drained at this time.</exception>
        /// <exception cref="System.ArgumentNullException">If the specified action is <see langword="null" />.</exception>
        /// <seealso cref="IBlockingQueue{T}.DrainTo(System.Collections.Generic.ICollection{T})" />
        /// <seealso cref="IBlockingQueue{T}.DrainTo(System.Collections.Generic.ICollection{T},Int32)" />
        /// <seealso cref="Drain(System.Action{T},Int32)" />
        Int32 Drain( Action<T> action );

        /// <summary>
        ///     Removes all available elements that meet the criteria defined by <paramref name="criteria" /> from this queue and
        ///     invoke the given <paramref name="action" /> on each element in order.
        /// </summary>
        /// <remarks>
        ///     This operation may be more efficient than repeatedly polling this queue. A failure encountered while attempting to
        ///     invoke the <paramref name="action" /> on the elements may result in elements being neither,
        ///     either or both in the queue or processed when the associated exception is thrown.
        ///     <example>
        ///         Drain to a non-generic list.
        ///         <code language="c#">
        /// IList c = ...;
        /// int count = Drain(delegate(T e) {c.Add(e);});
        /// </code>
        ///     </example>
        /// </remarks>
        /// <param name="action">  The action to perform on each element.</param>
        /// <param name="criteria">The criteria to filter the elements.</param>
        /// <returns>The number of elements processed.</returns>
        /// <exception cref="System.InvalidOperationException">If the queue cannot be drained at this time.</exception>
        /// <exception cref="System.ArgumentNullException">If the specified action is <see langword="null" />.</exception>
        /// <seealso cref="Drain(System.Action{T},Int32)" />
        Int32 Drain( Action<T> action, Predicate<T> criteria );

        /// <summary>
        ///     Removes at most the given number of available elements from this queue and invoke the given
        ///     <paramref name="action" /> on each element in order.
        /// </summary>
        /// <remarks>
        ///     This operation may be more efficient than repeatedly polling this queue. A failure encountered while attempting to
        ///     invoke the <paramref name="action" /> on the elements may result in elements being neither,
        ///     either or both in the queue or processed when the associated exception is thrown.
        /// </remarks>
        /// <param name="action">     The action to perform on each element.</param>
        /// <param name="maxElements">the maximum number of elements to transfer</param>
        /// <returns>The number of elements processed.</returns>
        /// <exception cref="System.InvalidOperationException">If the queue cannot be drained at this time.</exception>
        /// <exception cref="System.ArgumentNullException">If the specified action is <see langword="null" />.</exception>
        /// <seealso cref="Drain(System.Action{T})" />
        Int32 Drain( Action<T> action, Int32 maxElements );

        /// <summary>
        ///     Removes at most the given number of available elements that meet the criteria defined by
        ///     <paramref name="criteria" /> from this queue and invoke the given <paramref name="action" /> on each element in
        ///     order.
        /// </summary>
        /// <remarks>
        ///     This operation may be more efficient than repeatedly polling this queue. A failure encountered while attempting to
        ///     invoke the <paramref name="action" /> on the elements may result in elements being neither,
        ///     either or both in the queue or processed when the associated exception is thrown.
        /// </remarks>
        /// <param name="action">     The action to perform on each element.</param>
        /// <param name="maxElements">the maximum number of elements to transfer</param>
        /// <param name="criteria">   The criteria to filter the elements.</param>
        /// <returns>The number of elements processed.</returns>
        /// <exception cref="System.InvalidOperationException">If the queue cannot be drained at this time.</exception>
        /// <exception cref="System.ArgumentNullException">If the specified action is <see langword="null" />.</exception>
        /// <seealso cref="Drain(System.Action{T})" />
        Int32 Drain( Action<T> action, Int32 maxElements, Predicate<T> criteria );

        /// <summary>
        ///     Retrieves, but does not remove, the head of this queue.
        /// </summary>
        /// <remarks>This method differs from <see cref="Peek(out T)" /> in that it throws an exception if this queue is empty.</remarks>
        /// <returns>The head of this queue.</returns>
        /// <exception cref="InvalidOperationException">If this queue is empty.</exception>
        T Element();

        /// <summary>
        ///     Inserts the specified element into this queue if it is possible to do so immediately without violating capacity
        ///     restrictions.
        /// </summary>
        /// <remarks>
        ///     When using a capacity-restricted queue, this method is generally preferable to
        ///     <see cref="ICollection{T}.Add" />, which can fail to insert an element only by throwing an exception.
        /// </remarks>
        /// <param name="element">The element to add.</param>
        /// <returns><c>true</c> if the element was added to this queue. Otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="element" /> is <c>null</c> and the queue implementation
        ///     doesn't allow <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If some property of the supplied <paramref name="element" /> prevents it from being
        ///     added to this queue.
        /// </exception>
        Boolean Offer( T element );

        /// <summary>
        ///     Retrieves, but does not remove, the head of this queue into out parameter <paramref name="element" />.
        /// </summary>
        /// <param name="element">The head of this queue. <c>default(T)</c> if queue is empty.</param>
        /// <returns><c>false</c> is the queue is empty. Otherwise <c>true</c>.</returns>
        Boolean Peek( out T element );

        /// <summary>
        ///     Retrieves and removes the head of this queue into out parameter <paramref name="element" />.
        /// </summary>
        /// <param name="element">Set to the head of this queue. <c>default(T)</c> if queue is empty.</param>
        /// <returns><c>false</c> if the queue is empty. Otherwise <c>true</c>.</returns>
        Boolean Poll( out T element );

        /// <summary>
        ///     Retrieves and removes the head of this queue.
        /// </summary>
        /// <returns>The head of this queue</returns>
        /// <exception cref="InvalidOperationException">If this queue is empty.</exception>
        T Remove();
    }
}