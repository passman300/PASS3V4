using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3V4.Data_Structures
{
    /// <summary>
    /// Represents a generic stack data structure.
    /// </summary>
    /// <typeparam name="T">The type of elements in the stack.</typeparam>
    public class Stack<T>
    {
        private List<T> stack = new(); // <T>

        /// <summary>
        /// Gets the top element of the stack.
        /// </summary>
        /// <exception cref="Exception">Thrown when the stack is empty.</exception>
        /// <returns>The top element of the stack.</returns>
        public T Top()
        {
            if (stack.Count == 0) throw new Exception("Stack is empty.");
            return stack[^1];
        }

        /// <summary>
        /// Gets the element at the specified index from the top of the stack.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <exception cref="Exception">Thrown when the stack is empty.</exception>
        /// <returns>The element at the specified index from the top of the stack.</returns>
        public T Top(int index)
        {
            if (stack.Count == 0) throw new Exception("Stack is empty.");
            return stack[stack.Count - 1 - index];
        }

        /// <summary>
        /// Removes and returns the top element of the stack.
        /// </summary>
        /// <exception cref="Exception">Thrown when the stack is empty.</exception>
        /// <returns>The removed top element of the stack.</returns>
        public T Pop()
        {
            if (stack.Count == 0) throw new Exception("Stack is empty."); // DEBUG

            T temp = Top();
            stack.RemoveAt(stack.Count - 1); // remove the top element
            return temp;
        }

        /// <summary>
        /// Adds an element to the top of the stack.
        /// </summary>
        /// <param name="item">The object to push onto the stack.</param>
        public void Push(T item)
        {
            stack.Add(item);
        }

        /// <summary>
        /// Gets the number of elements contained in the stack.
        /// </summary>
        /// <returns>The number of elements contained in the stack.</returns>
        public int Count()
        {
            return stack.Count;
        }
    }
}
