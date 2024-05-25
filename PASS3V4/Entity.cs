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
        protected const byte LEFT = 0;
        protected const byte RIGHT = 1;

        protected ContentManager Content;

        protected GraphicsDevice graphicsDevice;

        protected Texture2D img;

        protected Rectangle hurtBox;
        protected Rectangle feetRec;

        protected Vector2 position;
        protected Vector2 hurtPosition;
        protected Vector2 feetPosition;
        protected Vector2 centerPosition;
        protected float speed;
        protected Vector2 velocity;

        protected byte direction;

        protected GameRectangle debugFeetRec;
        protected GameRectangle debugHurtBox;
        protected GameRectangle debugAnimBox;

        public Entity(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Content = content;

            this.graphicsDevice = graphicsDevice;
        }


        public Vector2 GetPosition() => position;

        public void SetPosition(Vector2 position) => this.position = position;

        public Vector2 GetCenterPosition() => centerPosition;
        public void SetCenterPosition(Vector2 centerPosition) => this.centerPosition = centerPosition;

        public Rectangle GetHurtBox() => hurtBox;
        public void SetHurtBox(Rectangle hurtBox) => this.hurtBox = hurtBox;

        public void SetFeetRec(Rectangle feetRec) => this.feetRec = feetRec;

        public Rectangle GetFeetRec() => feetRec;

        public virtual void Update(GameTime gameTime) { }

        public virtual void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb, Rectangle[] wallRecs = null) { }

        public virtual void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse, Rectangle[] wallRecs = null) { }

        public virtual void Draw(SpriteBatch spriteBatch, bool debug = false)
        {
            if (debug)
            {
                spriteBatch.Draw(Assets.pixels, hurtBox, Color.Blue * 0.5f);

                spriteBatch.Draw(Assets.pixels, hurtBox, Color.Blue * 0.5f);
            }
        }

    }
}
