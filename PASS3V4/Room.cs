using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PASS3V4
{
    public class Room : TileMap
    {
        private int mobSpawns;

        private List<int> adjacentRooms = new List<int>();

        public Room(string filePath, GraphicsDevice graphicsDevice) : base(filePath, graphicsDevice)
        {
            // TDOD: Load room from file

            /*
             * create a file system based on the types of rooms
             *      each floder contains a tsx and a txt of room stats
             *      
             * create the room stats of mobs, waves, if 
             */

        }
    }
}
