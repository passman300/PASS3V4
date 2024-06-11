using System.Collections.Generic;

namespace PASS3V4
{
    public class Queue<T>
    {   
        // implementing a queue using a list, behind the scenes
        protected List<T> queue = new();

        /// <summary>
        /// constructor of the queue
        /// </summary>
        public Queue() { }

        /// <summary>
        /// remove the first item in the queue
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            T item = queue[0];
            queue.RemoveAt(0);
            return item;
        }


        /// <summary> <summary>
        /// Add a new item to the queue.
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            queue.Add(item);
        }

        /// <summary>
        /// peeks the first item in the queue
        /// </summary>
        /// <returns></returns>
        public T Peek() => queue[0];


        ///  <summary>
        /// peeks the item at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns> the data of the node at the specified index </returns>
        public T Peek(int index) => queue[index];


        /// <summary> <summary>
        /// peeks the last item in the queue
        /// </summary>
        /// <returns> the data of the node at the specified index </returns>
        public T PeekLast() => queue[^1];

        /// <summary>
        /// check if the queue is empty
        /// </summary>
        /// <returns> true if empty, false if not </returns>
        public bool IsEmpty() => queue.Count == 0;

        /// <summary>
        /// returns the size of the queue
        /// </summary>
        /// <returns> size of the queue. </returns>
        public int Size() => queue.Count;

    }
}
