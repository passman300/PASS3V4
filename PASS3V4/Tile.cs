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
using Animation = GameUtility.Animation;

namespace PASS3V4
{
    public class Tile
    {
        public enum Properties
        {
            Collision,
            Animated
        }

        const int WIDTH = 32;
        const int HEIGHT = 32;

        private Texture2D tileSetImg;
        private List<Texture2D> tileImg = new List<Texture2D>();

        private int tileIDperRow;
        private List<int> tileID = new List<int>();

        private Vector3 position; // x, y, z (layer)

        private Rectangle hitBox;

        private Dictionary<Properties, bool> properties;

        private int collisionDamage;

        private Timer frameDuration;

        private int curFrame;

        public Tile(GraphicsDevice graphicsDevice, Texture2D tileSetImg, int tileID, Vector3 position, bool isCollidable, int collisionDamage)
        {
            (this.tileID).Add(tileID);

            this.position = position;

            properties[Properties.Collision] = isCollidable;
            this.collisionDamage = collisionDamage;

            // set all animation properties and variables to false, or 0
            properties[Properties.Animated] = false;
            frameDuration = new Timer(0, false);
            curFrame = 0;

            // set up the tile image
            SetTileImages(graphicsDevice, tileSetImg, this.tileID);

            // set up the hitbox
            hitBox = new Rectangle((int)position.X, (int)position.Y, WIDTH, HEIGHT);
        }

        public Tile(GraphicsDevice graphicsDevice, Texture2D tileSetImg, List<int> tileIDs, Vector3 position, bool isCollidable, int collisionDamage, int animDuration)
        {
            this.tileID = tileIDs;

            this.position = position;

            properties[Properties.Collision] = isCollidable;
            this.collisionDamage = collisionDamage;

            properties[Properties.Animated] = true;
            frameDuration = new Timer(animDuration, true);
            curFrame = 0;

            // set up the tile image
            SetTileImages(graphicsDevice, tileSetImg, tileID);

            // set up the hitbox
            hitBox = new Rectangle((int)position.X, (int)position.Y, WIDTH, HEIGHT);
        }

        private void SetTileImages(GraphicsDevice graphicsDevice, Texture2D tileSetImg, List<int> tileID)
        {
            tileIDperRow = tileSetImg.Width / WIDTH;


            for (int i = 0; i < tileID.Count; i++)
            {
                // note: tileID, row and col is 0 indexed
                int row = tileID[i] / tileIDperRow;
                int col = tileID[i] % tileIDperRow;

                Rectangle sourceRec = new Rectangle(col * WIDTH, row * HEIGHT, WIDTH, HEIGHT);

                tileImg.Add(new Texture2D(graphicsDevice, WIDTH, HEIGHT));
                Color[] data = new Color[WIDTH * HEIGHT];
                tileSetImg.GetData(0, sourceRec, data, 0, data.Length);
                tileImg[tileImg.Count - 1].SetData(data);
            }
        }

        private void Update(GameTime gameTime)
        {
            if (properties[Properties.Animated])
            {
                frameDuration.Update(gameTime);

                if (frameDuration.IsFinished())
                {
                    curFrame = (curFrame + 1) % tileID.Count;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tileImg[curFrame], hitBox, Color.White);
        }


    }
}
