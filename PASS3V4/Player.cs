using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameUtility;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PASS3V4
{
    public class Player : Entity
    {
        public enum State
        {
            Idle,
            Walk,
            Attack,
            Hurt,
            Die
        }

        const int SPEED_BASE = 4;

        const int HURT_BOX_WIDTH = 32;
        const int HURT_BOX_HEIGHT = 56;
        const int HURT_BOX_OFFSET_X = 14;
        const int HURT_BOX_OFFSET_Y = 8;

        const int FEET_OFFSET_X = 0;
        const int FEET_OFFSET_Y = 41;
        const int FEET_HEIGHT = 15;
        const int FEET_WIDTH = 32;

        const int BREAD_CRUMB_DIST = 10;
        const int MAX_BREAD_CRUMBS = 10;

        private State state = State.Idle;

        private float health;

        private Animation[] anim;

        private List<Vector2> breadCrumbs = new List<Vector2>();



        public Player(ContentManager content, GraphicsDevice graphicsDevice, string csvFilePath) : base(content, graphicsDevice)
        {
            position = new Vector2(100, 100);

            // Load animations
            anim = Animation.LoadAnimations("AnimationSheets/" + csvFilePath, content);

            hurtBox = new Rectangle((int)(HURT_BOX_OFFSET_X + position.X), (int)(HURT_BOX_OFFSET_Y + position.Y), HURT_BOX_WIDTH, HURT_BOX_HEIGHT);

            feetRec = new Rectangle(hurtBox.X + FEET_OFFSET_X, hurtBox.Y + FEET_OFFSET_Y, FEET_WIDTH, FEET_HEIGHT);

            debugHurtBox = new GameRectangle(graphicsDevice, hurtBox.X, hurtBox.Y, hurtBox.Width, hurtBox.Height);

            debugFeetBox = new GameRectangle(graphicsDevice, feetRec.X, feetRec.Y, feetRec.Width, feetRec.Height);

            debugAnimBox = new GameRectangle(graphicsDevice, anim[0].GetDestRec().X, anim[0].GetDestRec().Y, anim[0].GetDestRec().Width, anim[0].GetDestRec().Height);

            speed = SPEED_BASE;
        }


        public override void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb, Rectangle[] wallRecs = null)
        {
            
            anim[(int)state].Update(gameTime);

            switch (state)
            {
                case State.Idle:
                    UpdateIdle(gameTime, kb);
                    break;
                case State.Walk:
                    UpdateWalk(gameTime, kb, wallRecs);
                    break;
            }

            UpdateRecs();

            // update bread crumbs
            UpdateBreadCrumbs();
        }

        private void UpdateWalk(GameTime gameTime, KeyboardState kb, Rectangle[] wallRecs)
        {
            if ((kb.IsKeyDown(Keys.W) != kb.IsKeyDown(Keys.S)) || (kb.IsKeyDown(Keys.A) != kb.IsKeyDown(Keys.D)))
            {
                // movement: up, down, left, right
                byte up = (byte)(kb.IsKeyDown(Keys.W) ? 1 : 0);
                byte down = (byte)(kb.IsKeyDown(Keys.S) ? 1 : 0);
                byte left = (byte)(kb.IsKeyDown(Keys.A) ? 1 : 0);
                byte right = (byte)(kb.IsKeyDown(Keys.D) ? 1 : 0);

                if (left == 1 && right == 0) direction = LEFT;
                else if (right == 1 && left == 0) direction = RIGHT;



                velocity.X = right - left;
                velocity.Y = down - up;

                if (velocity.LengthSquared() > 0)
                {
                    velocity = Vector2.Normalize(velocity) * speed;
                }


                Move(wallRecs, velocity.X, velocity.Y);

                if (breadCrumbs.Count > MAX_BREAD_CRUMBS)
                {
                    breadCrumbs.RemoveAt(0);
                }
            }
            else state = State.Idle;
        }

        private void Move(Rectangle[] wallRecs, float x, float y)
        {
            // check the x and y movement separately
            if (x != 0 && y != 0)
            {
                Move(wallRecs, 0, y);
                Move(wallRecs, x, 0);
                return;
            }

            Tuple<bool, Vector2> collisionResult = CheckWallCollision(wallRecs, x, y);
            position += collisionResult.Item2;
        }

        /// <summary>
        /// Update both the animation and the hurtbox rectangles
        /// </summary>
        private void UpdateRecs()
        {
            // update the location of the animation
            anim[(int)state].TranslateTo(position.X, position.Y);

            // since the when the animation flips, the hurtbox needs to be adjusted
            if (direction == RIGHT)
            {
                hurtBox.X = (int)(position.X + HURT_BOX_OFFSET_X);
                hurtBox.Y = (int)(position.Y + HURT_BOX_OFFSET_Y);

                feetRec.X = (int)(hurtBox.X + FEET_OFFSET_X);
                feetRec.Y = (int)(hurtBox.Y + FEET_OFFSET_Y);
            }
            else if (direction == LEFT)
            {
                hurtBox.X = (int)(position.X + anim[(int)state].GetDestRec().Width - HURT_BOX_OFFSET_X - HURT_BOX_WIDTH);
                hurtBox.Y = (int)(position.Y + HURT_BOX_OFFSET_Y);

                feetRec.X = (int)(hurtBox.X + HURT_BOX_WIDTH - FEET_WIDTH - FEET_OFFSET_X);
                feetRec.Y = (int)(hurtBox.Y + FEET_OFFSET_Y);
            }

            centerPosition = hurtBox.Center.ToVector2();

            debugHurtBox = new GameRectangle(graphicsDevice, hurtBox.X, hurtBox.Y, hurtBox.Width, hurtBox.Height);
            debugFeetBox = new GameRectangle(graphicsDevice, feetRec.X, feetRec.Y, feetRec.Width, feetRec.Height);
            debugAnimBox = new GameRectangle(graphicsDevice, (int)anim[(int)state].GetDestRec().X, (int)anim[(int)state].GetDestRec().Y, (int)anim[(int)state].GetDestRec().Width, (int)anim[(int)state].GetDestRec().Height);
        }

        private void UpdateIdle(GameTime gameTime, KeyboardState kb)
        {
            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.D)) state = State.Walk;
        }

        private void UpdateBreadCrumbs()
        {
            // only update bread crumbs if the player is 3 or more pixels away from previous
            // don't use sqaure root because it's expensive, so it 9 pixels

            if (breadCrumbs.Count == 0)
            {
                breadCrumbs.Add(centerPosition);
            }

            else if (Vector2.DistanceSquared(centerPosition, breadCrumbs[breadCrumbs.Count - 1]) >= Math.Pow(BREAD_CRUMB_DIST, 2))
            {
                breadCrumbs.Add(centerPosition);
            }
        }

        //public bool UpdateCollision(Tile[] tiles)
        //{
        //    bool isCollided = false;

        //    foreach (Tile tile in tiles)
        //    {
        //        if (tile.GetProperties()[Tile.Properties.Collision] && (feetRec.Intersects(tile.GetBoundingBox())))
        //        {
        //            foreach (Rectangle tileHitBox in tile.GetHitBoxs())
        //            {
        //                if (!feetRec.Intersects(tileHitBox)) continue;

        //                // Collision from the left
        //                if (velocity.X > 0 && feetRec.Right >= tileHitBox.Left && (feetRec.Bottom > tileHitBox.Top || feetRec.Top < tileHitBox.Bottom))
        //                {
        //                    position.X = tileHitBox.Left - FEET_WIDTH - FEET_OFFSET_X - HURT_BOX_OFFSET_X;
        //                }
        //                // Collision from the right
        //                else if (velocity.X < 0 && feetRec.Left <= tileHitBox.Right && (feetRec.Bottom > tileHitBox.Top || feetRec.Top < tileHitBox.Bottom))
        //                {
        //                    position.X = tileHitBox.Right - (anim[(int)state].GetDestRec().Width - FEET_WIDTH - FEET_OFFSET_X - HURT_BOX_OFFSET_X);
        //                }

        //                // Collision from the top
        //                if (velocity.Y > 0 && feetRec.Bottom >= tileHitBox.Top && (feetRec.Right > tileHitBox.Left || feetRec.Left < tileHitBox.Right))
        //                {
        //                    position.Y = tileHitBox.Top - FEET_HEIGHT - FEET_OFFSET_Y - HURT_BOX_OFFSET_Y;
        //                }
        //                // Collision from the bottom
        //                else if (velocity.Y < 0 && feetRec.Top <= tileHitBox.Bottom && (feetRec.Right > tileHitBox.Left || feetRec.Left < tileHitBox.Right))
        //                {
        //                    position.Y = tileHitBox.Bottom - FEET_OFFSET_Y - HURT_BOX_OFFSET_Y;
        //                }

        //                isCollided = true;
        //            }
        //        }
        //    }
        //    return isCollided;
        //}

        public Tuple<bool,Vector2> CheckWallCollision(Rectangle[] wallRecs,  float newX, float newY)
        {
            bool isCollided = false;

            Rectangle newFeetRec = new Rectangle((int)(feetRec.X + newX), (int)(feetRec.Y + newY), feetRec.Width, feetRec.Height);

            foreach (Rectangle rec in wallRecs)
            {
                if (newFeetRec.Intersects(rec))
                {
                    isCollided = true;

                    if (newX > 0 && newFeetRec.Right >= rec.Left && (newFeetRec.Bottom > rec.Top || newFeetRec.Top < rec.Bottom)) // Collision from the left
                        newX = rec.Left - feetRec.Right;
                    else if (newX < 0 && newFeetRec.Left <= rec.Right && (newFeetRec.Bottom > rec.Top || newFeetRec.Top < rec.Bottom)) // Collision from the right
                        newX = feetRec.Left - rec.Right;
                    else if (newY > 0 && newFeetRec.Bottom >= rec.Top && (newFeetRec.Right > rec.Left || newFeetRec.Left < rec.Right)) // Collision from the top
                        newY = rec.Top - feetRec.Bottom;
                    else if (newY < 0 && newFeetRec.Top <= rec.Bottom && (newFeetRec.Right > rec.Left || newFeetRec.Left < rec.Right)) // Collision from the bottom
                        newY = feetRec.Top - rec.Bottom;
                }
            }

            if (Math.Abs(newX) > speed)
                newX = 0;

            if (Math.Abs(newY) > speed)
                newY = 0;

            return Tuple.Create(isCollided, new Vector2(newX, newY));
        }

        public override void Draw(SpriteBatch spriteBatch, bool debug = false)
        {
            if (direction == LEFT)
            {
                anim[(int)state].Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
            }
            else if (direction == RIGHT)
            {
                anim[(int)state].Draw(spriteBatch, Color.White);
            }

            if (true)
            {
                spriteBatch.DrawString(Assets.debugFont, string.Format(" position: X: {0}, Y: {1}", position.X, position.Y), new Vector2(10, 10), Color.White);

                if (false)
                {
                    debugHurtBox.Draw(spriteBatch, Color.Blue * 0.5f, false);
                    debugFeetBox.Draw(spriteBatch, Color.Green, false);
                    debugAnimBox.Draw(spriteBatch, Color.Black * 0.5f, false);


                    foreach (Vector2 v in breadCrumbs)
                    {
                        spriteBatch.Draw(Assets.pixels, new Rectangle((int)v.X, (int)v.Y, 3, 3), Color.Red);
                    }
                }
                
            }


        }


    }
}
