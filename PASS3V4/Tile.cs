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

        public const int WIdTH = 32;
        public const int HEIGHT = 32;

        protected Texture2D tileImg;

        protected int tileIdPerRow;
        protected int tileId;

        protected Rectangle boundingBox;
        protected List<Rectangle> hitBoxes;

        protected Dictionary<Properties, bool> properties;

        protected int collisionDamage;

        protected GameRectangle debugBounding;

        protected List<GameRectangle> debugHitBoxes;

        public static bool isDebug = false;

        public Tile(GraphicsDevice graphicsDevice, Texture2D tileSetImg, int tileId, Vector3 position, TileTemplate tileTemplate)
        {
            this.hitBoxes = new List<Rectangle>();
            this.properties = new Dictionary<Properties, bool>();

            if (isDebug) debugHitBoxes = new List<GameRectangle>();

            if (tileTemplate.Frames.Count > 0)
            {
                ;
            }
            else this.tileId = tileId;

            properties[Properties.Collision] = tileTemplate.IsCollision;
            collisionDamage = tileTemplate.Damage;

            properties[Properties.Animated] = tileTemplate.IsAnimated;

            // set up the tile image
            if (!properties[Properties.Animated]) 
                SetTileImages(graphicsDevice, tileSetImg, this.tileId);

            // set up the hitbox
            boundingBox = new Rectangle((int)position.X, (int)position.Y, WIdTH, HEIGHT);

            foreach (Rectangle hitBox in tileTemplate.HitBoxes)
            {
                hitBoxes.Add(new Rectangle((int)(hitBox.X + position.X), (int)(hitBox.Y + position.Y), hitBox.Width, hitBox.Height));
                if (isDebug) debugHitBoxes.Add(new GameRectangle(graphicsDevice, hitBoxes[hitBoxes.Count - 1]));
            }

            if (isDebug) debugBounding = new GameRectangle(graphicsDevice, boundingBox);
        }

        public Rectangle GetBoundingBox() { return boundingBox; }

        public Rectangle[] GetHitBoxes() { return hitBoxes.ToArray(); }

        public Dictionary<Properties, bool> GetProperties() { return properties; }



        protected virtual void SetTileImages(GraphicsDevice graphicsDevice, Texture2D tileSetImg, int tileId)
        {
            tileIdPerRow = tileSetImg.Width / WIdTH;

            Rectangle sourceRec = new Rectangle(0, 0, WIdTH, HEIGHT);

            // note: tileId, row and col is 0 indexed
            int row = tileId / tileIdPerRow;
            int col = tileId % tileIdPerRow;

            //Rectangle sourceRec = new Rectangle(col * WIdTH, row * HEIGHT, WIdTH, HEIGHT);
            sourceRec.X = col * WIdTH;
            sourceRec.Y = row * HEIGHT;

            tileImg = Util.Crop(tileSetImg, sourceRec);
        }

        public virtual void Update(GameTime gameTime)
        {
            // image is still
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tileImg, boundingBox, Color.White);

            if (isDebug)
            {
                if (properties[Properties.Collision])
                {
                    foreach (GameRectangle hitBox in debugHitBoxes)
                    {
                        hitBox.Draw(spriteBatch, Color.Blue, true);
                    }
                }
            }
        }


    }
}
