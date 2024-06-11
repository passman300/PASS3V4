using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

/*
NOTE: NOTE ACTUALLY USED IN THE GAME
*/


namespace PASS3V4.Data_Structures
{
    /// <summary>
    /// A simple implementation of a singly linked list.
    /// </summary>
    public class LinkedList
    {
        private Node head; // The head node of the list.
        private Node tail; // The tail node of the list.

        /// <summary>
        /// Gets the size of the list.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedList"/> class.
        /// </summary>
        public LinkedList()
        {
            head = null; // Set head and tail to null
            tail = null;
            Size = 0;
        }

        /// <summary>
        /// Adds a node at the end of the list.
        /// </summary>
        /// <param name="data">The data to be stored in the new node.</param>
        public void AddLast(Vector2 data)
        {
            Node newNode = new (data); // Create a new node

            // If the list is empty, set both head and tail to the new node
            if (head == null)
            {
                head = newNode;
                tail = newNode;
            }
            else // Otherwise, add the new node after the tail
            {
                Node current = head;

                // iterate to the end of the list
                while (current.Next != null)
                {
                    current = current.Next;
                }

                // add the new node
                current.Next = newNode;

                // set the tail to the new node
                tail = newNode;
            }

            // increase the size of the list
            Size++;
        }

        /// <summary>
        /// Adds a node at the beginning of the list.
        /// </summary>
        /// <param name="data">The data to be stored in the new node.</param>
        public void AddFirst(Vector2 data)
        {
            Node newNode = new(data);

            // if the list is empty, set both head and tail to the new node
            if (head != null)
            {
                newNode.Next = head;
            }

            // set the head to the new node
            head = newNode;

            // increase the size of the list
            Size++;
        }

        /// <summary>
        /// Removes the last node from the list.
        /// </summary>
        public void RemoveLast()
        {
            if (head == null) return; // if the list is empty, do nothing

            if (head.Next == null)
            {
                head = null; // if the list has only one node, set both head and tail to null
                return;
            }

            Node current = head;

            // iterate to the end of the list
            while (current.Next.Next != null)
            {
                current = current.Next;
            }

            // set the tail to the previous node
            current.Next = null;
            tail = current;

            // decrease the size of the list
            Size = Math.Max(0, Size - 1);
        }

        /// <summary>
        /// Removes the first node from the list.
        /// </summary>
        public void RemoveFirst()
        {
            if (head == null) return; // if the list is empty, do nothing

            head = head.Next; // set the head to the next node
            Size = Math.Max(0, Size - 1); // decrease the size of the list

        }

        /// <summary>
        /// Peeks at the first node in the list without removing it.
        /// </summary>
        /// <returns>The data of the first node.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the list is empty.</exception>
        public Vector2 PeekFirst()
        {
            if (head == null)
            {
                throw new InvalidOperationException("List is empty."); // DEBUG
            }
            return head.Data;
        }

        /// <summary>
        /// Peeks at the last node in the list without removing it.
        /// </summary>
        /// <returns>The data of the last node.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the list is empty.</exception>
        public Vector2 PeekLast()
        {
            if (head == null)
            {
                throw new InvalidOperationException("List is empty.");
            }

            return tail.Data;
        }

        /// <summary>
        /// Peeks at the node at the specified index without removing it.
        /// </summary>
        /// <param name="index">The index of the node.</param>
        /// <returns>The data of the node at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
        public Vector2 PeekAt(int index)
        {
            if (index < 0 || index >= Size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range."); // DEBUG
            }

            Node current = head;

            // iterate to the node at the specified index
            for (int i = 0; i < index; i++)
            {
                current = current.Next;
            }

            // return the data of the node
            return current.Data;
        }

        /// <summary>
        /// Gets the head node of the list.
        /// </summary>
        /// <returns>The head node.</returns>
        public Node GetHead() => head;

        /// <summary>
        /// Gets the tail node of the list.
        /// </summary>
        /// <returns>The tail node.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the list is empty.</exception>
        public Node GetTail()
        {
            if (head == null)
            {
                throw new InvalidOperationException("List is empty."); // DEBUG
            }
            return tail;
        }

        /// <summary>
        /// Gets the node at the specified index.
        /// </summary>
        /// <param name="index">The index of the node.</param>
        /// <returns>The node at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
        public Node GetNodeAt(int index)
        {
            if (index < 0 || index >= Size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range."); // DEBUG
            }

            Node current = head;

            // iterate to the node at the specified index
            for (int i = 0; i < index; i++)
            {
                current = current.Next;
            }

            // return the node
            return current;
        }

        /// <summary>
        /// Prints all nodes in the list.
        /// </summary>
        public void PrintList()
        {
            Node current = head; 

            // iterate through the linked list until the end is reached
            while (current != null)
            {

                // output the data of the node
                Debug.WriteLine(current.Data);
                current = current.Next;
            }
        }

        /// <summary>
        /// Converts the list to a list of Vector2s.
        /// </summary>
        /// <returns>A list of Vector2s.</returns>
        public List<Vector2> ToList()
        {
            List<Vector2> list = new(); // create new list
            Node current = head;


            // iterate through the linked list until the end is reached
            while (current != null)
            {
                list.Add(current.Data); // add the data of the current node to the list
                current = current.Next;
            }
            
            // return the list
            return list;

        }
    }

}
