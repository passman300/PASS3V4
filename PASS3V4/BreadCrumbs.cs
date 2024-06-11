//Author: Colin Wang
//File Name: BreadCrumbs.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: June 6, 2024
//Modified Date: June 10, 2024
//Description: A collection of breadcrumbs that the player has visited, used to track the player's path. It uses the DoubleLinkedList class


namespace PASS3V4
{
    /// <summary>
    /// Represents a collection of breadcrumbs that the player has visited.
    /// </summary>
    public class BreadCrumbs : Data_Structures.DoubleLinkedList
    {
        /// <summary>
        /// Initializes a new instance of the BreadCrumbs class.
        /// </summary>
        public BreadCrumbs() : base()
        {
        }

        /// <summary>
        /// Clears all breadcrumbs from the collection.
        /// </summary>
        public void Clear()
        {
            head = null; // Set the head of the linked list to null.
            tail = null; // Set the tail of the linked list to null.
            size = 0; // Set the size of the linked list to 0.
        }
    }
}
