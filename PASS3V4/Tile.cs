using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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

        private Rectangle boundingBox;
        private List<Rectangle> hitBoxs = new List<Rectangle>();

        private Dictionary<Properties, bool> properties = new Dictionary<Properties, bool>();

        private int collisionDamage;

        private Timer frameDuration;

        private int curFrame;

        private GameRectangle degbugBounding;

        private List<GameRectangle> debugHitBoxs = new List<GameRectangle>();

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
            boundingBox = new Rectangle((int)position.X, (int)position.Y, WIDTH, HEIGHT);

            foreach (Rectangle hitBox in tileTemplate.HitBoxs)
            {
                hitBoxs.Add(new Rectangle((int)(hitBox.X + position.X), (int)(hitBox.Y + position.Y), hitBox.Width, hitBox.Height));
                debugHitBoxs.Add(new GameRectangle(graphicsDevice, hitBoxs[hitBoxs.Count - 1]));
            }

            degbugBounding = new GameRectangle(graphicsDevice, boundingBox);
        }

        public Rectangle GetBoundingBox() { return boundingBox; }

        public Rectangle[] GetHitBoxes() { return hitBoxs.ToArray(); }

        public Dictionary<Properties, bool> GetProperties() { return properties; }

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
            spriteBatch.Draw(tileImg[curFrame], boundingBox, Color.White);


            if (debug)
            {
                if (properties[Properties.Collision])
                {
                    //degbugBounding.Draw(spriteBatch, Color.Red, false);

                    foreach (GameRectangle hitBox in debugHitBoxs)
                    {
                        hitBox.Draw(spriteBatch, Color.Blue, true);
                    }
                }
            }
        }


    }
}
