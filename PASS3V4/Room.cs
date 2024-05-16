using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameUtility;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace PASS3V4
{
    public class Room : TileMap
    {
        private int mobSpawns;

        private List<int> adjacentRooms = new();

        private Vector2 spawnPosition;

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

        public override void Update(GameTime gameTime, Player player, KeyboardState kb, KeyboardState prevKb)
        {
            for (int i = 0; i < BackLayers.Length; i++)
            {
                BackLayers[i].Update(gameTime);
            }
            for (int i = 0; i < FrontLayers.Length; i++)
            {
                FrontLayers[i].Update(gameTime);
            }

            player.Update(gameTime, kb, prevKb, WallRecs);
        }
    }
}
