//Author: Colin Wang
//File Name: Tile.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 5, 2024
//Modified Date: June 10, 2024
//Description: Tile class for the game, should be inherited by all other tiles. Contains the general variables and the image of the tile

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

        // the width and height of the tile
        public const int WIDTH = 32;
        public const int HEIGHT = 32;

        // the image of the tile
        protected Texture2D tileImg;

        // the id of the tile
        protected int tileIdPerRow;
        protected int tileId;

        // the hitbox of the tile
        protected Rectangle boundingBox;
        protected List<Rectangle> hitBoxes;

        // the properties of the tile
        protected Dictionary<Properties, bool> properties;

        // the damage of the tile
        protected int collisionDamage;

        // DEBUG
        // the debug hitbox
        protected GameRectangle debugBounding;
        // the debug hitboxes
        protected List<GameRectangle> debugHitBoxes;
        // DEBUG
        public static bool isDebug = false;

        /// <summary>
        /// construct a new tile
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="tileSetImg"></param>
        /// <param name="tileId"></param>
        /// <param name="position"></param>
        /// <param name="tileTemplate"></param>
        public Tile(GraphicsDevice graphicsDevice, Texture2D tileSetImg, int tileId, Vector3 position, TileTemplate tileTemplate)
        {
            this.hitBoxes = new List<Rectangle>();
            this.properties = new Dictionary<Properties, bool>();

            if (isDebug) debugHitBoxes = new List<GameRectangle>();

            if (tileTemplate.Frames.Count <= 0) // only set the tile id if there are no frames
            {
                this.tileId = tileId;
            }
            
            // set up the properties
            properties[Properties.Collision] = tileTemplate.IsCollision;
            collisionDamage = tileTemplate.Damage;
            properties[Properties.Animated] = tileTemplate.IsAnimated;

            // set up the tile image
            if (!properties[Properties.Animated]) 
                SetTileImages(graphicsDevice, tileSetImg, this.tileId);

            // set up the hitbox
            boundingBox = new Rectangle((int)position.X, (int)position.Y, WIDTH, HEIGHT);

            // set up the hitboxes
            foreach (Rectangle hitBox in tileTemplate.HitBoxes)
            {
                hitBoxes.Add(new Rectangle((int)(hitBox.X + position.X), (int)(hitBox.Y + position.Y), hitBox.Width, hitBox.Height));
                if (isDebug) debugHitBoxes.Add(new GameRectangle(graphicsDevice, hitBoxes[hitBoxes.Count - 1]));
            }

            if (isDebug) debugBounding = new GameRectangle(graphicsDevice, boundingBox);
        }

        /// <summary>
        /// Set the tile images
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="tileSetImg"></param>
        /// <param name="tileId"></param>
        protected virtual void SetTileImages(GraphicsDevice graphicsDevice, Texture2D tileSetImg, int tileId)
        {
            // note: tileId, row and col is 0 indexed
            tileIdPerRow = tileSetImg.Width / WIDTH;

            // set the source rectangle of the tile
            Rectangle sourceRec = new Rectangle(0, 0, WIDTH, HEIGHT);

            // note: tileId, row and col is 0 indexed
            int row = tileId / tileIdPerRow;
            int col = tileId % tileIdPerRow;

            //Rectangle sourceRec = new Rectangle(col * WIDTH, row * HEIGHT, WIDTH, HEIGHT);
            sourceRec.X = col * WIDTH;
            sourceRec.Y = row * HEIGHT;

            // crop the tile set image with the source rectangle
            tileImg = Util.Crop(tileSetImg, sourceRec);
        }

        /// <summary>
        /// Update the the tile
        /// used for animated tiles
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            // image is still
        }

        ///  <summary>
        /// Draw the tile
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // draw the tile
            spriteBatch.Draw(tileImg, boundingBox, Color.White);

            // DEBUG
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
