using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PASS3V4
{
    public class Player : Entity
    {
        public enum State
        {
            Idle,
            Walk,
            Run,
            Attack,
            Hurt,
            Die
        }

        const float SPEED_BASE = 4f;
        const float SPRINT_SCALE = 1.25f;

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

        const int NUM_WEAPONS = 2;

        private State state = State.Idle;

        private float health;

        private Animation[] anim;

        private List<Vector2> breadCrumbs = new();

        private Weapon[] weapons = new Weapon[NUM_WEAPONS];
        private int activeWeapon = 0;

        public Player(ContentManager content, GraphicsDevice graphicsDevice, string csvFilePath) : base(content, graphicsDevice)
        {
            position = new Vector2(100, 100);

            // Load animations
            anim = Animation.LoadAnimations("AnimationSheets/" + csvFilePath, content);

            hurtBox = new Rectangle((int)(HURT_BOX_OFFSET_X + position.X), (int)(HURT_BOX_OFFSET_Y + position.Y), HURT_BOX_WIDTH, HURT_BOX_HEIGHT);

            feetRec = new Rectangle(hurtBox.X + FEET_OFFSET_X, hurtBox.Y + FEET_OFFSET_Y, FEET_WIDTH, FEET_HEIGHT);

            speed = SPEED_BASE;

            debugFeetRec = new GameRectangle(graphicsDevice, feetRec);
            debugHurtBox = new GameRectangle(graphicsDevice, hurtBox);
            debugAnimBox = new GameRectangle(graphicsDevice, anim[(int)state].GetDestRec());

            SetupWeapon(0, new LargeSword(graphicsDevice, centerPosition));
            SetupWeapon(1, new Bow(graphicsDevice, centerPosition));
        }

        private void SetupWeapon(int index, Weapon weapon)
        {
            weapons[index] = weapon;
        }


        public override void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse, Rectangle[] wallRecs = null)
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
                case State.Run:
                    UpdateWalk(gameTime, kb, wallRecs);
                    break;
            }

            UpdateRecs();

            // update bread crumbs
            UpdateBreadCrumbs();

            if (kb.IsKeyDown(Keys.E) && prevKb.IsKeyUp(Keys.E)) activeWeapon = (activeWeapon + 1) % 2;

            weapons[activeWeapon].Update(graphicsDevice, gameTime, centerPosition, mouse, prevMouse);
        }

        private void UpdateWalk(GameTime gameTime, KeyboardState kb, Rectangle[] wallRecs)
        {
            if ((kb.IsKeyDown(Keys.W) ^ kb.IsKeyDown(Keys.S)) || (kb.IsKeyDown(Keys.A) ^ kb.IsKeyDown(Keys.D)))
            {
                // movement: up, down, left, right
                bool up = kb.IsKeyDown(Keys.W);
                bool down = kb.IsKeyDown(Keys.S);
                bool left = kb.IsKeyDown(Keys.A);
                bool right = kb.IsKeyDown(Keys.D);

                bool isSprinting = kb.IsKeyDown(Keys.LeftShift);

                if (left && !right) direction = LEFT;
                else if (right && !left) direction = RIGHT;


                if ((right && left) || (!right && !left)) velocity.X = 0;
                else if (right && !left) velocity.X = 1;
                else if (left && !right) velocity.X = -1;
                if ((up && down) || (!up && !down)) velocity.Y = 0;
                else if (up && !down) velocity.Y = -1;
                else if (down && !up) velocity.Y = 1;

                if (velocity.LengthSquared() > 0)
                {
                    velocity = Vector2.Normalize(velocity) * speed;

                    if (isSprinting) state = State.Run;
                    else state = State.Walk;
                }

                if (isSprinting) velocity *= SPRINT_SCALE;

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

            position += CheckWallCollision(wallRecs, x, y).Velocity; 
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

                feetRec.X = hurtBox.X + FEET_OFFSET_X;
                feetRec.Y = hurtBox.Y + FEET_OFFSET_Y;
            }
            else if (direction == LEFT)
            {
                hurtBox.X = (int)(position.X + anim[(int)state].GetDestRec().Width - HURT_BOX_OFFSET_X - HURT_BOX_WIDTH);
                hurtBox.Y = (int)(position.Y + HURT_BOX_OFFSET_Y);

                feetRec.X = hurtBox.X + HURT_BOX_WIDTH - FEET_WIDTH - FEET_OFFSET_X;
                feetRec.Y = hurtBox.Y + FEET_OFFSET_Y;
            }

            debugFeetRec.TranslateTo(feetRec.X, feetRec.Y);
            debugHurtBox.TranslateTo(hurtBox.X, hurtBox.Y);
            debugAnimBox.TranslateTo(anim[(int)state].GetDestRec().X, anim[(int)state].GetDestRec().Y);

            centerPosition = hurtBox.Center.ToVector2();
        }

        private void UpdateIdle(GameTime gameTime, KeyboardState kb)
        {
            if ((kb.IsKeyDown(Keys.W) ^ kb.IsKeyDown(Keys.S)) || (kb.IsKeyDown(Keys.A) ^ kb.IsKeyDown(Keys.D))) state = State.Walk;
        }

        private void UpdateBreadCrumbs()
        {
            // only update bread crumbs if the player is 3 or more pixels away from previous
            // don't use sqaure root because it's expensive, so it 9 pixels

            if (breadCrumbs.Count == 0)
            {
                breadCrumbs.Add(centerPosition);
            }

            else if (Vector2.DistanceSquared(centerPosition, breadCrumbs[^1]) >= Math.Pow(BREAD_CRUMB_DIST, 2))
            {
                breadCrumbs.Add(centerPosition);
            }
        }

        private (bool IsCollided, Vector2 Velocity) CheckWallCollision(Rectangle[] wallRecs, float x, float y)
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

            if (Math.Abs(newX) > speed * (State.Run == state ? SPRINT_SCALE : 1))
                newX = 0;

            if (Math.Abs(newY) > speed * (State.Run == state ? SPRINT_SCALE : 1))
                newY = 0;

            return (isCollided, new Vector2(newX, newY));
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

            weapons[activeWeapon].Draw(spriteBatch);

            if (true)
            {
                spriteBatch.DrawString(Assets.debugFont, string.Format(" position: X: {0}, Y: {1}", position.X, position.Y), new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(Assets.debugFont, direction.ToString(), new Vector2(10, 30), Color.White);
                spriteBatch.DrawString(Assets.debugFont, velocity.ToString(), new Vector2(10, 50), Color.White);
                spriteBatch.DrawString(Assets.debugFont, weapons[activeWeapon].GetAngle().ToString(), new Vector2(10, 70), Color.White);

                debugFeetRec.Draw(spriteBatch, Color.Red, false);
                debugHurtBox.Draw(spriteBatch, Color.Green, false);
                debugAnimBox.Draw(spriteBatch, Color.Purple, false);


                if (false)
                {
                    foreach (Vector2 v in breadCrumbs)
                    {
                        spriteBatch.Draw(Assets.pixels, new Rectangle((int)v.X, (int)v.Y, 3, 3), Color.Red);
                    }
                }

            }


        }


    }
}
