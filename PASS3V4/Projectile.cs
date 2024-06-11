//Author: Colin Wang
//File Name: Projectile.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: June 4, 2024
//Modified Date: June 10, 2024
//Description: The parent class of all projectiles such as arrows and fireballs

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameUtility;

namespace PASS3V4
{
    public class Projectile
    {
        public enum ProjectileState
        {
            active,
            remove
        }

        // properties of the tile image
        protected Texture2D Img {get; set;}
        protected Rectangle SrcRec { get; set; } // source rectangle used to crop the image

        // properties of the velocity and angle
        public Vector2 Velocity { get; set; }
        public float Speed { get; set; } // should be the speed of the projectile

        // properties of the angle 
        public float Angle { get; set; }

        public int Damage { get; set; }

        public Vector2 Position { get; set; } // should start from the center of the player
        protected Vector2 Offset { get; set; } // should be the offset of the projectile to the center of the player
        protected Vector2 Origin { get; set; } // point of rotation of the projectile (hitbox and image)

        public Rectangle HitBox { get; set; }
        protected float HitBoxWidth { get; set; }
        protected float HitBoxHeight { get; set; }

        protected ProjectileState State { get; set; }

        // DEBUG
        public static bool isDebug = false;
        protected GameRectangle degbugHitBox;

        public Projectile(GraphicsDevice graphicsDevice, Texture2D img, Rectangle srcRec, float speed, float angle, Vector2 centerPos, Vector2 offset, Vector2 origin, int damage)
        {
            Img = img;
            SrcRec = srcRec;

            Speed = speed;
            Angle = angle;

            Position = centerPos + Offset;
            Offset = offset;
            Origin = origin;

            Damage = damage;

            InitializeHitBox(graphicsDevice, srcRec.Width, srcRec.Height);
        }

        public virtual void UpdateFlying()
        {
            Position += Velocity;
            HitBox = new Rectangle((int)(Position.X - HitBoxWidth / 2), (int)(Position.Y - HitBoxHeight / 2), (int)HitBoxWidth, (int)HitBoxHeight);

            if (isDebug) degbugHitBox.TranslateTo(HitBox.X, HitBox.Y);
        }


        protected virtual void InitializeHitBox(GraphicsDevice graphicsDevice, int hitBoxWidth, int hitBoxHeight)
        {
            HitBoxWidth = hitBoxWidth;
            HitBoxHeight = hitBoxHeight;

            HitBox = new Rectangle((int)(Position.X) - hitBoxWidth / 2, (int)(Position.Y) - hitBoxHeight, hitBoxWidth, hitBoxHeight);
            degbugHitBox = new GameRectangle(graphicsDevice, HitBox);
        }


        public virtual bool IsColliding(Rectangle other)
        {
            bool result = HitBox.Intersects(other);

            if (result) State = ProjectileState.remove;

            return result;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Img, Position - Offset, SrcRec, Color.White, Angle, Origin, 1, SpriteEffects.None, 0);

            // draw the hitbox
            if (isDebug)
            {
                degbugHitBox.Draw(spriteBatch, Color.Red, false);
                spriteBatch.DrawString(Assets.debugFont, Velocity.ToString(), HitBox.Center.ToVector2() - new Vector2(10, 10), Color.White);
            }
        }
    }
}
