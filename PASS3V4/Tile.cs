using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PASS3V4
{
    public class Tile
    {
        public enum Properties
        {
            Collision,
            Animated
        }

        public const int WIDTH = 32;
        public const int HEIGHT = 32;

        private List<Texture2D> tileImg = new List<Texture2D>();

        private int tileIDperRow;
        private OrderedSet<int> tileID = new OrderedSet<int>();

        private Rectangle hitBox;

        private Dictionary<Properties, bool> properties = new Dictionary<Properties, bool>();

        private int collisionDamage;

        private Timer frameDuration;

        private int curFrame;

        GameRectangle temp;

        public Tile(GraphicsDevice graphicsDevice, Texture2D tileSetImg, int tileID, Vector3 position, TileTemplate tileTemplate)
        {
            if (tileTemplate.Frames.Count > 0)
                this.tileID = tileTemplate.Frames;
            this.tileID.Add(tileID);

            properties[Properties.Collision] = tileTemplate.IsCollision;
            this.collisionDamage = tileTemplate.Damage;

            properties[Properties.Animated] = tileTemplate.IsAnimated;
            frameDuration = new Timer(tileTemplate.AnimationDur, true);
            curFrame = 0;

            // set up the tile image
            SetTileImages(graphicsDevice, tileSetImg, this.tileID);

            // set up the hitbox
            hitBox = new Rectangle((int)position.X, (int)position.Y, WIDTH, HEIGHT);
            temp = new GameRectangle(graphicsDevice, hitBox);
        }

        public Tile(GraphicsDevice graphicsDevice, Texture2D tileSetImg, OrderedSet<int> tileIDs, Vector3 position, bool isCollidable, int collisionDamage, int animDuration)
        {
            this.tileID = tileIDs;

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

        private void SetTileImages(GraphicsDevice graphicsDevice, Texture2D tileSetImg, OrderedSet<int> tileID)
        {
            tileIDperRow = tileSetImg.Width / WIDTH;

            Rectangle sourceRec = new Rectangle(0, 0, WIDTH, HEIGHT);

            foreach (int t in tileID)
            {
                // note: tileID, row and col is 0 indexed
                int row = t / tileIDperRow;
                int col = t % tileIDperRow;

                //Rectangle sourceRec = new Rectangle(col * WIDTH, row * HEIGHT, WIDTH, HEIGHT);
                sourceRec.X = col * WIDTH;
                sourceRec.Y = row * HEIGHT;

                tileImg.Add(new Texture2D(graphicsDevice, WIDTH, HEIGHT));
                Color[] data = new Color[WIDTH * HEIGHT];
                tileSetImg.GetData(0, sourceRec, data, 0, data.Length);
                tileImg[tileImg.Count - 1].SetData(data);
            }
        }

        public void Update(GameTime gameTime)
        {
            frameDuration.Update(gameTime);

            if (properties[Properties.Animated] && frameDuration.IsFinished())
            {
                curFrame = (curFrame + 1) % tileID.Count;

                frameDuration.ResetTimer(true);
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool debug = false)
        {
            spriteBatch.Draw(tileImg[curFrame], hitBox, Color.White);


            if (debug)
            {
                if (properties[Properties.Collision])
                {
                    temp.Draw(spriteBatch, Color.Red, false);
                }
            }
        }


    }
}
