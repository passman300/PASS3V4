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

namespace PASS3V4
{
    public class Mob : Entity
    {
        public enum MobType
        {
            Skeleton,
            Zombie,
            Ghost
        }
        public enum MobState
        {
            Spawning,
            Idle,
            Walk,
            Attack,
            GetHit,
            Dead
        }

        public MobType Type { get; set; } 
        public MobState State { get; set; }
        public int Range { get; set; } 

        public Mob(ContentManager content, GraphicsDevice graphicsDevice, MobType mobType, int damage, int health, int speed, int range) : base(content, graphicsDevice)
        {
            Type = mobType;

            Damage = damage;
            MaxHealth = health;
            CurrentHealth = health;
            Speed = speed;
            Range = range;

            State = MobState.Spawning;
        }

        public override void Update(GameTime gameTime)
        {
        }

        protected virtual void FollowPlayer(Rectangle[] wallRecs, BreadCrumbs breadCrumbs)
        {
            foreach (Vector2 crumbPos in breadCrumbs.GetQueue())
            {
                // check the distance between the mob (position) and the bread crumb (crumbPos)
                if (Vector2.DistanceSquared(position, crumbPos) <= Range)
                {
                    // move the mob towards the bread crumb
                    Velocity = Vector2.Normalize(position - crumbPos) * Speed;

                    Move(wallRecs, Velocity.X, Velocity.Y);
                }
            }
        }

        protected override void Move(Rectangle[] wallRecs, float x, float y)
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
    }
}
