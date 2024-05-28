using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3V4
{
    public class Stack<T>
    {
        private List<T> stack = new(); // <T>

        public T Top()
        {
            if (stack.Count == 0) throw new Exception("Stack is empty.");
            return stack[^1];
        }

        public T Pop()
        {
            if (stack.Count == 0) throw new Exception("Stack is empty.");
            T temp = Top();
            stack.RemoveAt(stack.Count - 1);
            return temp;
        }

        public void Push(T item)
        {
            stack.Add(item);
        }

        public int Count()
        {
            return stack.Count;
        }
    }
}
