using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PASS3V4
{
    public class AnimatedTile : Tile
    {
        private List<Texture2D> tileImgs;

        protected OrderedSet<int> tileIds;

        protected Timer frameDuration;

        protected int curFrame;

        private static int count = 0;

        public AnimatedTile(GraphicsDevice graphicsDevice, Texture2D tileSetImg, int tileId, Vector3 position, TileTemplate tileTemplate) : base(graphicsDevice, tileSetImg, tileId, position, tileTemplate)
        {
            tileImgs = new();

            count++;

            this.tileIds = tileTemplate.Frames;
            this.tileIds.Add(tileId);


            frameDuration = new Timer(tileTemplate.AnimationDur, true);
            curFrame = 0;

            SetTileImages(graphicsDevice, tileSetImg, tileIds);
        }

        protected virtual void SetTileImages(GraphicsDevice graphicsDevice, Texture2D tileSetImg, OrderedSet<int> tileId)
        {
            tileIdPerRow = tileSetImg.Width / WIdTH;

            Rectangle sourceRec = new Rectangle(0, 0, WIdTH, HEIGHT);

            foreach (int t in tileId)
            {
                // note: tileId, row and col is 0 indexed
                int row = t / tileIdPerRow;
                int col = t % tileIdPerRow;

                //Rectangle sourceRec = new Rectangle(col * WIdTH, row * HEIGHT, WIdTH, HEIGHT);
                sourceRec.X = col * WIdTH;
                sourceRec.Y = row * HEIGHT;

                tileImgs.Add(Util.Crop(tileSetImg, sourceRec));
            }
        }

        public override void Update(GameTime gameTime)
        {
            frameDuration.Update(gameTime);

            if (properties[Properties.Animated] && frameDuration.IsFinished())
            {
                curFrame = (curFrame + 1) % tileIds.Count;

                frameDuration.ResetTimer(true);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tileImgs[curFrame], boundingBox, Color.White);
        }

    }
}
