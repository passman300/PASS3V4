using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace PASS3V4.Data_Structures
{
    public class DoubleLinkedList
    {
        // head node (first node in the list/queue)
        protected Node head;

        // tail node (last node in the list/queue)
        protected Node tail;
        protected int size; // size of the linked list

        public DoubleLinkedList()
        {
            // set head and tail to null
            head = null;
            tail = null;
            size = 0;
        }

        // get the size of the linked list
        public int Size
        {
            get { return size; }
        }

        // Add a node at the end of the list
        public void AddLast(Vector2 data)
        {
            Node newNode = new Node(data);
            if (tail == null) // if list is empty (both head and tail are null)
            {
                head = newNode;
                tail = newNode;
            }
            else // otherwise add the new node after the tail
            {
                tail.Next = newNode;
                newNode.Prev = tail;
                tail = newNode;
            }

            // increase the size of the linked list
            size++;
        }

        /// <summary>
        /// Add a node at the beginning of the list
        /// </summary>
        /// <param name="data">The data to be stored in the new node</param>
        public void AddFirst(Vector2 data)
        {
            // create a new node with the given data
            Node newNode = new Node(data);
            // if the list is empty, set both head and tail to the new node
            if (head == null)
            {
                head = newNode;
                tail = newNode;
            }
            // otherwise, add the new node before the current head
            else
            {
                newNode.Next = head;
                head.Prev = newNode;
                head = newNode;
            }
            // increase the size of the linked list
            size++;
        }


        /// <summary>
        /// Remove the first node from the list
        /// </summary>
        public void RemoveFirst()
        {
            // if the list is empty, do nothing
            if (head == null) return;

            // if the list has only one node, set both head and tail to null
            if (head == tail)
            {
                head = null;
                tail = null;
            }
            else
            {
                // otherwise, set the head to the next node and
                // set the previous node's next to null
                head = head.Next;
                head.Prev = null;
            }

            size--;
        }



        /// <summary>
        /// Remove the last node from the list
        /// </summary>
        public void RemoveLast()
        {
            // If the list is empty, do nothing
            if (tail == null) return;

            // If the list has only one node, set both head and tail to null
            if (head == tail)
            {
                head = null;
                tail = null;
            }
            else
            {
                // Otherwise, set the tail to the previous node and
                // set the next node's previous to null
                tail = tail.Prev;
                tail.Next = null;
            }

            // Decrease the size of the linked list
            size--;
        }

        /// <summary>
        /// Peek at the first node in the list
        /// </summary>
        /// <returns> The first node's data (head) </returns>
        public Vector2 PeakFirst()
        {
            if (head == null) throw new InvalidOperationException("The list is empty."); // DEBUG
            return head.Data;
        }

        /// <summary>
        /// Gets the last node in the list
        /// </summary>
        /// <returns> The last node's data (tail)</returns>
        public Vector2 PeakLast()
        {
            if (tail == null) throw new InvalidOperationException("The list is empty.");
            return tail.Data;
        }

        /// <summary>
        /// Peaks the node at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns> the data of the node at the specified index </returns>
        public Vector2 Peek(int index)
        {
            if (index < 0 || index >= size) // check if the node exists
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range."); // DEBUG
            }

            // have to start from the head, and iterate through the linked list until the index is reached

            Node current = head; // store the current node, which is the head at the start

            for (int i = 0; i < index; i++) // iterate through the linked list until the index is reached
            {
                current = current.Next;
            }

            // return the data of the current node
            return current.Data;
        }

        /// <summary>
        /// gets the head of the list
        /// </summary>
        /// <returns> head node </returns>
        public Node GetHead()
        {
            return head;
        }


        /// <summary> 
        /// gets the tail of the list
        /// </summary>
        /// <returns> tail node </returns>
        public Node GetTail()
        {
            return tail;
        }


        /// <summary>
        /// prints all nodes in the list (DEBUG)
        /// </summary>
        public void PrintList()
        {
            Node current = head;

            // iterate through the linked list until the end is reached
            while (current != null)
            {
                Console.WriteLine(current.Data); // print the data of the current node
                current = current.Next; // move to the next node
            }
        }

        /// <summary> 
        /// turns a linked list into a list
        /// </summary>
        /// <returns> list of the data in the linked list </returns>
        public List<Vector2> ToList()
        {
            List<Vector2> list = new(); // create new list
            
            Node current = head; // store the current node, which is the head at the start

            // iterate through the linked list until the end is reached
            while (current != null)
            {
                list.Add(current.Data); // add the data of the current node to the list
                current = current.Next; // move to the next node
            }

            // return the list
            return list;
        }
    }
}
