using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameUtility;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static PASS3V4.Player;

namespace PASS3V4
{
    public class Entity
    {
        public struct FeetRecs
        {
            private const int RL_WIDTH = 6;
            private const int RL_HEIGHT = 7;
            private const int TB_WIDTH = 20;
            private const int TB_HEIGHT = 5;

            private const int RL_OFFSET_X = 0;
            private const int RL_OFFSET_Y = 2;
            private const int TB_OFFSET_X = 6;
            private const int TB_OFFSET_Y = 0;

            public Rectangle leftRec { get; private set; }
            public Rectangle rightRec { get; private set; }
            public Rectangle topRec { get; private set; }
            public Rectangle bottomRec { get; private set; }

            public FeetRecs(Vector2 position)
            {
                leftRec = new Rectangle((int)position.X + RL_OFFSET_X, (int)position.Y + RL_OFFSET_Y, RL_WIDTH, RL_HEIGHT);
                rightRec = new Rectangle(leftRec.Right + RL_OFFSET_X + TB_WIDTH, (int)position.Y + RL_OFFSET_Y, RL_WIDTH, RL_HEIGHT);
                topRec = new Rectangle((int)position.X + TB_OFFSET_X, (int)position.Y + TB_OFFSET_Y, TB_WIDTH, TB_HEIGHT);
                bottomRec = new Rectangle((int)position.X + TB_OFFSET_X, (int)position.Y + TB_OFFSET_Y + TB_HEIGHT, TB_WIDTH, TB_HEIGHT);
            }
            public void Update(Vector2 position)
            {
                leftRec = new Rectangle((int)position.X + RL_OFFSET_X, (int)position.Y + RL_OFFSET_Y, RL_WIDTH, RL_HEIGHT);
                rightRec = new Rectangle(leftRec.Right + RL_OFFSET_X + TB_WIDTH, (int)position.Y + RL_OFFSET_Y, RL_WIDTH, RL_HEIGHT);
                topRec = new Rectangle((int)position.X + TB_OFFSET_X, (int)position.Y + TB_OFFSET_Y, TB_WIDTH, TB_HEIGHT);
                bottomRec = new Rectangle((int)position.X + TB_OFFSET_X, (int)position.Y + TB_OFFSET_Y + TB_HEIGHT, TB_WIDTH, TB_HEIGHT);
            }
        }

        protected const byte LEFT = 0;
        protected const byte RIGHT = 1;

        protected ContentManager Content;

        protected GraphicsDevice graphicsDevice;

        protected Texture2D img;

        protected Rectangle hurtBox;
        protected Rectangle feetRec;
        protected FeetRecs feetRecs;

        protected GameRectangle debugHurtBox;
        protected GameRectangle debugAnimBox;
        protected GameRectangle debugFeetBox;
        protected Vector2 position;
        protected Vector2 hurtPosition;
        protected Vector2 feetPosition;
        protected Vector2 centerPosition;
        protected float speed;
        protected Vector2 velocity;

        protected byte direction;




        public Entity(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Content = content;

            this.graphicsDevice = graphicsDevice;
        }


        public Vector2 GetPosition() { return position; }

        public void SetPosition(Vector2 position) { this.position = position; }

        public Rectangle GetHurtBox() { return hurtBox; }
        public void SetHurtBox(Rectangle hurtBox) { this.hurtBox = hurtBox; }

        public void SetFeetRec(Rectangle feetRec) { this.feetRec = feetRec; }

        public Rectangle GetFeetRec() { return feetRec; }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb, Rectangle[] wallRecs = null) { }

        public virtual void Draw(SpriteBatch spriteBatch, bool debug = false)
        {
            if (debug)
            {
                debugHurtBox.Draw(spriteBatch, Color.Blue * 0.5f, true);

                debugAnimBox.Draw(spriteBatch, Color.Red * 0.5f, true);
            }
        }

    }
}
