//Author: Colin Wang
//File Name: AnimatedTile.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 29, 2024
//Modified Date: June 10, 2024
//Description: AnimatedTile class for the game, which has an animation. Should inherit from Tile class

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameUtility;
using System.Collections.Generic;


namespace PASS3V4
{
    /// <summary>
    /// Represents a tile that can be animated. Inherits from Tile class.
    /// </summary>
    public class AnimatedTile : Tile 
    {
        // list of all the tile images
        private List<Texture2D> tileImgs;

        // store each tile id
        protected OrderedSet<int> tileIds;

        // duration of each frame
        protected Timer frameDuration;

        // index of the current frame
        protected int curFrame;

        /// <summary>
        /// Initialize an AnimatedTile object.
        /// </summary>
        /// <param name="graphicsDevice">Graphics device.</param>
        /// <param name="tileSetImg">Tile set image.</param>
        /// <param name="tileId">Tile id.</param>
        /// <param name="position">Position of the tile.</param>
        /// <param name="tileTemplate">Template of the tile.</param>
        public AnimatedTile(GraphicsDevice graphicsDevice, Texture2D tileSetImg, int tileId, Vector3 position, TileTemplate tileTemplate) : base(graphicsDevice, tileSetImg, tileId, position, tileTemplate)
        {
            tileImgs = new();

            this.tileIds = tileTemplate.Frames;
            this.tileIds.Add(tileId);


            frameDuration = new Timer(tileTemplate.AnimationDur, true);
            curFrame = 0;

            SetTileImages(graphicsDevice, tileSetImg, tileIds);
        }

        /// <summary>
        /// Set the tile images based on the tile ids.
        /// </summary>
        /// <param name="graphicsDevice">Graphics device.</param>
        /// <param name="tileSetImg">Tile set image.</param>
        /// <param name="tileId">Tile id.</param>
        protected virtual void SetTileImages(GraphicsDevice graphicsDevice, Texture2D tileSetImg, OrderedSet<int> tileId)
        {
            // get the tile id of the row
            tileIdPerRow = tileSetImg.Width / WIDTH;
            
            // create the tile image source rectangle
            Rectangle sourceRec = new Rectangle(0, 0, WIDTH, HEIGHT);

            // note: tileId, row and col is 0 indexed
            // iterate through all the tile ids
            foreach (int t in tileId)
            {
                // note: tileId, row and col is 0 indexed
                int row = t / tileIdPerRow;
                int col = t % tileIdPerRow;

                //Rectangle sourceRec = new Rectangle(col * WIDTH, row * HEIGHT, WIDTH, HEIGHT);
                sourceRec.X = col * WIDTH;
                sourceRec.Y = row * HEIGHT;

                // add the tile image into the list
                tileImgs.Add(Util.Crop(tileSetImg, sourceRec));
            }
        }

        /// <summary>
        /// Update the animation frames.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public override void Update(GameTime gameTime)
        {
            // update the frame duration timer
            frameDuration.Update(gameTime);

            // check if the frame duration is finished
            if (properties[Properties.Animated] && frameDuration.IsFinished())
            {
                // update the current frame to the next frame
                curFrame = (curFrame + 1) % tileIds.Count;

                // reset the frame duration timer
                frameDuration.ResetTimer(true);
            }
        }

        /// <summary>
        /// Draw the animated tile.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw the current frame
            spriteBatch.Draw(tileImgs[curFrame], boundingBox, Color.White);
        }
    }
}
