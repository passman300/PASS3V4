using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3V4
{
	/// <summary>
	/// Represents a node in a linked list with a Vector2 data and pointers to the next and previous nodes.
	/// </summary>
	public class Node
	{
		/// <summary>
		/// The data stored in the node.
		/// </summary>
		public Vector2 Data { get; set; }

		/// <summary>
		/// Pointer to the next node in the list.
		/// </summary>
		public Node Next { get; set; }

		/// <summary>
		/// Pointer to the previous node in the list.
		/// </summary>
		public Node Prev { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Node"/> class with the specified data.
		/// </summary>
		/// <param name="data">The data to be stored in the node.</param>
		public Node(Vector2 data)
		{
			Data = data;
			Next = null;
			Prev = null;
		}
	}
}
