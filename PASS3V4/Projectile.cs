using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameUtility;
using System;
using Microsoft.Xna.Framework.Content;
using System.ComponentModel.Design.Serialization;
using Microsoft.Xna.Framework.Input;
using System.Reflection.Metadata;

namespace PASS3V4
{
    public class Projectile
    {
        protected Texture2D Img {get; set;}
        protected Rectangle SrcRec { get; set; } // source rectangle used to crop the image

        protected Vector2 Velocity { get; set; }
        protected float Speed { get; set; }

        protected float Angle { get; set; }

        protected int Damage { get; set; }

        protected Vector2 Position { get; set; } // should start from the center of the player
        protected Vector2 Offset { get; set; } // should be the offset of the projectile to the center of the player
        protected Vector2 Origin { get; set; } // point of rotation of the projectile (hitbox and image)

        public Rectangle HitBox { get; set; }
        protected float HitBoxWidth { get; set; }
        protected float HitBoxHeight { get; set; }

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
        }
    }
}
