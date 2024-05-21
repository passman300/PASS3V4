using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PASS3V4
{
    public class TileMap
    {
        public TileLayer[] BackLayers { get; set; }

        public TileLayer[] FrontLayers { get; set; }

        public Rectangle[] WallRecs { get; set; }

        private TileMapReader tileMapReader;

        private int splitLayer;

        public TileMap(string filePath, GraphicsDevice graphicsDevice)
        {
            tileMapReader = new TileMapReader(filePath);

            tileMapReader.LoadTileMapFile(graphicsDevice);

            BackLayers = tileMapReader.GetBackLayers();
            FrontLayers = tileMapReader.GetFrontLayers();
            WallRecs = tileMapReader.GetWallRecs();
        }

        public virtual void Update(GameTime gameTime)
        {
            for (int i = 0; i < BackLayers.Length; i++)
            {
                BackLayers[i].Update(gameTime);
            }
            for (int i = 0; i < FrontLayers.Length; i++)
            {
                FrontLayers[i].Update(gameTime);
            }
        }

        public virtual void Update(GameTime gameTime, Player player, KeyboardState kb, KeyboardState prevKb)
        {

        }

        public void DrawFront(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < FrontLayers.Length; i++)
            {
                FrontLayers[i].Draw(spriteBatch);
            }
        }

        public void DrawBack(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < BackLayers.Length; i++)
            {
                BackLayers[i].Draw(spriteBatch);
            }
        }

        public void DrawWallHitboxes(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < WallRecs.Length; i++)
            {
                spriteBatch.Draw(Assets.pixels, WallRecs[i], Color.Blue * 0.2f);
            }
        }
    }
}
