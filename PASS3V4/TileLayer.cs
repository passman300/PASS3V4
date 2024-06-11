//Author: Colin Wang
//File Name: TileLayer.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 8, 2024
//Modified Date: June 10, 2024
//Description: Tile layer is a list of tiles, but can overlap with other tile layers.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace PASS3V4
{
    public class TileLayer
    {   
        // name of the layer
        public string Name { get; set; }

        // order of the layer
        public int LayerOrder { get; set; }

        // is the layer is front
        public bool IsFront { get; set; }

        // list of tiles
        public List<Tile> Tiles { get; private set; } = new List<Tile>();

        /// <summary>
        /// constructor of the tile layers
        /// </summary>
        /// <param name="name"></param>
        /// <param name="layerOrder"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public TileLayer(string name, int layerOrder, int width, int height)
        {   
            // set the name and layer order
            Name = name;
            LayerOrder = layerOrder;

            // set the front to false
            IsFront = false;
        }

        /// <summary>
        /// overloaded constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="layerOrder"></param>
        /// <param name="isFront"></param>
        public TileLayer(string name, int layerOrder, bool isFront)
        {
            // set the name and layer order
            Name = name;
            LayerOrder = layerOrder;

            // set the value of isfront to the parameter
            IsFront = isFront;
        }

        /// <summary>
        /// load the tile
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="index"></param>
        public void LoadTile(Tile tile, int index)
        {
            Tiles[index] = tile;
        }

        /// <summary>
        /// add the tile to the tile layer
        /// </summary>
        /// <param name="tile"></param>
        public void AddTile(Tile tile)
        {
            Tiles.Add(tile);
        }

        /// <summary>
        /// update the tile layer
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {   
            // iterate through the list of tiles and update each tile
            for (int i = 0; i < Tiles.Count; i++)
            {
                Tiles[i].Update(gameTime);
            }
        }

        /// <summary>
        /// draw all the tiles in the tile layer
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                Tiles[i].Draw(spriteBatch);
            }
        }
    }
}
