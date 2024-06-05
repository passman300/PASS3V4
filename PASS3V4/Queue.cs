using System.Collections.Generic;

namespace PASS3V4
{
    public class Queue<T>
    {
        protected List<T> queue = new();

        public T Dequeue()
        {
            T item = queue[0];
            queue.RemoveAt(0);
            return item;
        }

        public void Enqueue(T item)
        {
            queue.Add(item);
        }

        public T Peek() => queue[0];

        public T Peek(int index) => queue[index];

        public T PeekLast() => queue[^1];

        public bool IsEmpty() => queue.Count == 0;

        public int Size() => queue.Count;

    }
}
