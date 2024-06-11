//Author: Colin Wang
//File Name: OrderedSet.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 7, 2024
//Modified Date: June 10, 2024
//Description: set but the elements are ordered by their insertion order

using System.Collections;
using System.Collections.Generic;

namespace PASS3V4
{
    public class OrderedSet<T> : ICollection<T>
    {
        // IDictionary is used to map from T to LinkedListNode<T>
        private readonly IDictionary<T, LinkedListNode<T>> m_Dictionary;
        private readonly LinkedList<T> m_LinkedList; // list of items in the set

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedSet{T}"/> class using the default equality comparer.
        /// </summary>
        /// <remarks>
        /// This constructor uses the default equality comparer for the type <typeparamref name="T"/>.
        /// </remarks>
        public OrderedSet()
            : this(EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedSet{T}"/> class using the specified equality comparer.
        /// </summary>
        /// <param name="comparer">The equality comparer to use for the set.</param>
        /// <remarks>
        /// The specified equality comparer is used to determine equality between elements in the set.
        /// </remarks>
        public OrderedSet(IEqualityComparer<T> comparer)
        {
            // Initialize the dictionary with the specified equality comparer
            m_Dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);

            // Create a new linked list to store the items in the set
            m_LinkedList = new LinkedList<T>();
        }

        // Return the number of items in the set
        public int Count
        {
            get { return m_Dictionary.Count; }
        }

        // Return true if the set is read-only
        public virtual bool IsReadOnly
        {
            get { return m_Dictionary.IsReadOnly; }
        }

        // Add an item to the set
        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        // Add an item to the set
        public bool Add(T item)
        {
            if (m_Dictionary.ContainsKey(item)) return false;
            LinkedListNode<T> node = m_LinkedList.AddLast(item);
            m_Dictionary.Add(item, node);
            return true;
        }

        /// <summary>
        /// Clear the whole set
        /// </summary>
        public void Clear()
        {
            m_LinkedList.Clear();
            m_Dictionary.Clear();
        }

        /// <summary> <summary>
        /// remove the element from the game onced killed
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            LinkedListNode<T> node;
            bool found = m_Dictionary.TryGetValue(item, out node);
            if (!found) return false;
            m_Dictionary.Remove(item);
            m_LinkedList.Remove(node);
            return true;
        }

        /// <summary> 
        /// Gives you  the option to do 'foreach' and other loops
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return m_LinkedList.GetEnumerator();
        }

        /// <summary> 
        /// Gives you  the option to do 'foreach' and other loops
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary> <summary>
        /// returns if the item is in the set
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return m_Dictionary.ContainsKey(item);
        }

        /// <summary> <summary>
        /// copy the value at index of set to the array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            m_LinkedList.CopyTo(array, arrayIndex);
        }
    }
}
