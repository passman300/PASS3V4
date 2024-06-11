//Author: Colin Wang
//File Name: RoomTemplate.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 30, 2024
//Modified Date: June 10, 2024
//Description: Template class for rooms, such as their layers and doors

using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace PASS3V4
{
    public class RoomTemplate
    {
        public int MaxMobs { get; set; } // maximum number of mobs in the room
        public int MinMobs { get; set; } // minimum number of mobs in the room

        public List<TileLayer> FrontLayers { get; set; } = new(); // front layers of the room
        public List<TileLayer> BackLayers { get; set; } = new(); // back layers of the room

        public List<TileLayer> DoorLayers { get; set; } = new(); // door layers of the room

        public List<Rectangle> WallRecs { get; set; } = new(); // wall rectangles of the room

        public (Rectangle top, Rectangle bottom, Rectangle left, Rectangle right) DoorRecs = (new Rectangle(), new Rectangle(), new Rectangle(), new Rectangle());
        public (Vector2 top, Vector2 bottom, Vector2 left, Vector2 right) SpawnPoints = (Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero);

        public Rectangle ExitRec { get; set; } // the exit rectangle of the level (portal)
        public Queue<List<Mob>> MobWaves { get; set; } = new(); // queue of list of mobs in the room
        public Rectangle SpawnArea { get; set; } // the area in which mobs can spawn

        ///  <summary>
        /// update all the rooms layers
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            foreach (TileLayer layer in FrontLayers)
            {
                layer.Update(gameTime);
            }
            foreach (TileLayer layer in BackLayers)
            {
                layer.Update(gameTime);
            }
        }
    }
}
