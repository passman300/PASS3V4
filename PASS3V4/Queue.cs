using System.Collections.Generic;

namespace PASS3V4
{
    public class Queue<T>
    {
        private List<T> queue = new();

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

        public bool IsEmpty() => queue.Count == 0;

        public int Size() => queue.Count;
    }
}
