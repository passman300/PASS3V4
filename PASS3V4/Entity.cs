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
        //protected Vector2 feetPosition;
        protected Vector2 centerPosition;

        protected float Speed { get; set; }
        protected Vector2 Velocity { get; set; }

        protected byte direction;

        protected int CurrentHealth { get; set; }
        protected int MaxHealth { get; set; }
        public bool IsDead { get; set; } = false; // make this public because it is used in Room class
        protected int Damage { get; set; }

        protected GameRectangle debugFeetRec;
        protected GameRectangle debugHurtBox;
        protected GameRectangle debugAnimBox;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="graphicsDevice"></param>
        public Entity(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Content = content;

            this.graphicsDevice = graphicsDevice;
        }


        public Vector2 GetPosition() => position;
        public void SetPosition(Vector2 position) => this.position = position;
        public void SetPosition(float x, float y) => this.position = new Vector2(x, y);

        public Vector2 GetCenterPosition() => centerPosition;
        public void SetCenterPosition(Vector2 centerPosition) => this.centerPosition = centerPosition;

        public Rectangle GetHurtBox() => hurtBox;
        public void SetHurtBox(Rectangle hurtBox) => this.hurtBox = hurtBox;

        public void SetFeetRec(Rectangle feetRec) => this.feetRec = feetRec;

        public Rectangle GetFeetRec() => feetRec;

        public virtual void Update(GameTime gameTime) { }

        public virtual void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb, Rectangle[] wallRecs = null) { }

        public virtual void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse, Rectangle[] wallRecs = null) { }

        protected virtual void Move(Rectangle[] wallRecs, float x, float y)
        {
            // check the x and y movement separately
            if (x != 0 && y != 0)
            {
                Move(wallRecs, 0, y);
                Move(wallRecs, x, 0);
                return;
            }

            position += CheckWallCollision(wallRecs, x, y).Velocity;
        }

        protected virtual (bool IsCollided, Vector2 Velocity) CheckWallCollision(Rectangle[] wallRecs, float x, float y)
        {
            float newX = x;
            float newY = y;

            bool isCollided = false;

            Rectangle newFeetRec = new((int)(feetRec.X + x), (int)(feetRec.Y + y), feetRec.Width, feetRec.Height);

            foreach (Rectangle rec in wallRecs)
            {
                if (newFeetRec.Intersects(rec))
                {
                    isCollided = true;

                    if (x > 0 && (feetRec.X + x) + feetRec.Width > rec.Left) // Collision from the left
                        newX = rec.Left - feetRec.Right;
                    else if (x < 0 && (feetRec.X + x) < rec.Right) // Collision from the right
                        newX = rec.Right - feetRec.Left;
                    else if (y > 0 && feetRec.Y + y + feetRec.Height > rec.Top) // Collision from the top
                        newY = rec.Top - feetRec.Bottom;
                    else if (y < 0 && feetRec.Y + y < rec.Bottom) // Collision from the bottom
                        newY = rec.Bottom - feetRec.Top;
                }
            }



            return (isCollided, new Vector2(newX, newY));
        }


        public virtual void Draw(SpriteBatch spriteBatch, bool isDebug = false)
        {
            if (isDebug)
            {
                spriteBatch.Draw(Assets.pixels, hurtBox, Color.Blue * 0.5f);
            }
        }

    }
}
