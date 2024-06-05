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
        public enum PlayerState
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

        public const int HURT_BOX_WIdTH = 32;
        public const int HURT_BOX_HEIGHT = 56;
        public const int HURT_BOX_OFFSET_X = 14;
        public const int HURT_BOX_OFFSET_Y = 8;

        const int FEET_OFFSET_X = 0;
        const int FEET_OFFSET_Y = 41;
        const int FEET_HEIGHT = 15;
        const int FEET_WIdTH = 32;

        const int BREAD_CRUMB_DIST = 10;
        const int MAX_BREAD_CRUMBS = 10;

        const int NUM_WEAPONS = 2;

        private PlayerState state = PlayerState.Idle;

        private Animation[] anim;

        private BreadCrumbs breadCrumbs = new();

        private Weapon[] weapons = new Weapon[NUM_WEAPONS];
        private int activeWeapon = 0;

        public static bool isDebug = true;

        public Player(ContentManager content, GraphicsDevice graphicsDevice, string csvFilePath) : base(content, graphicsDevice)
        {
            position = new Vector2(100, 100);

            // Load animations
            anim = Animation.LoadAnimations("AnimationSheets/" + csvFilePath, content);

            hurtBox = new Rectangle((int)(HURT_BOX_OFFSET_X + position.X), (int)(HURT_BOX_OFFSET_Y + position.Y), HURT_BOX_WIdTH, HURT_BOX_HEIGHT);

            feetRec = new Rectangle(hurtBox.X + FEET_OFFSET_X, hurtBox.Y + FEET_OFFSET_Y, FEET_WIdTH, FEET_HEIGHT);

            Speed = SPEED_BASE;

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

        public (bool IsShot, Arrow Arrow) GetFlyingArrow()
        {
            if (weapons[activeWeapon].GetType() == typeof(Bow)) 
                return ((Bow)weapons[activeWeapon]).GetFlyingArrow();

            return (false, null);
        }

        public override void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse, Rectangle[] wallRecs = null)
        {
            anim[(int)state].Update(gameTime);

            switch (state)
            {
                case PlayerState.Idle:
                    UpdateIdle(gameTime, kb);
                    break;
                case PlayerState.Walk:
                    UpdateWalk(gameTime, kb, wallRecs);
                    break;
                case PlayerState.Run:
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

                // check all cases for left right input
                if ((right && left) || (!right && !left)) Velocity = new Vector2(0, Velocity.Y); // got to create new vector for properties
                else if (right && !left) Velocity = new Vector2(1, Velocity.Y);
                else if (left && !right) Velocity = new Vector2(-1, Velocity.Y);

                // check all cases for up down input
                if ((up && down) || (!up && !down)) Velocity = new Vector2(Velocity.X, 0);
                else if (up && !down) Velocity = new Vector2(Velocity.X, -1); 
                else if (down && !up) Velocity = new Vector2(Velocity.X, 1);

                if (Velocity.LengthSquared() > 0)
                {
                    Velocity = Vector2.Normalize(Velocity) * Speed;

                    if (isSprinting) state = PlayerState.Run;
                    else state = PlayerState.Walk;
                }

                if (isSprinting) Velocity *= SPRINT_SCALE;

                Move(wallRecs, Velocity.X, Velocity.Y);

                if (breadCrumbs.Size() > MAX_BREAD_CRUMBS)
                {
                    breadCrumbs.Dequeue();
                }
            }
            else state = PlayerState.Idle;
        }

        protected override void Move(Rectangle[] wallRecs, float x, float y)
        {
            base.Move(wallRecs, x, y);
        }

        protected override (bool IsCollided, Vector2 Velocity) CheckWallCollision(Rectangle[] wallRecs, float x, float y)
        {
            (bool isCollided, Vector2 velocity) temp = base.CheckWallCollision(wallRecs, x, y);

            if (Math.Abs(temp.velocity.X) > Speed * (PlayerState.Run == state ? SPRINT_SCALE : 1)) temp.velocity.X = 0;

            if (Math.Abs(temp.velocity.Y) > Speed * (PlayerState.Run == state ? SPRINT_SCALE : 1)) temp.velocity.Y = 0;

            return temp;
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
                hurtBox.X = (int)(position.X + anim[(int)state].GetDestRec().Width - HURT_BOX_OFFSET_X - HURT_BOX_WIdTH);
                hurtBox.Y = (int)(position.Y + HURT_BOX_OFFSET_Y);

                feetRec.X = hurtBox.X + HURT_BOX_WIdTH - FEET_WIdTH - FEET_OFFSET_X;
                feetRec.Y = hurtBox.Y + FEET_OFFSET_Y;
            }

            centerPosition = hurtBox.Center.ToVector2();

            if (isDebug)
            {
                debugFeetRec.TranslateTo(feetRec.X, feetRec.Y);
                debugHurtBox.TranslateTo(hurtBox.X, hurtBox.Y);
                debugAnimBox.TranslateTo(anim[(int)state].GetDestRec().X, anim[(int)state].GetDestRec().Y);
            }
        }

        private void UpdateIdle(GameTime gameTime, KeyboardState kb)
        {
            if ((kb.IsKeyDown(Keys.W) ^ kb.IsKeyDown(Keys.S)) || (kb.IsKeyDown(Keys.A) ^ kb.IsKeyDown(Keys.D))) state = PlayerState.Walk;
        }

        private void UpdateBreadCrumbs()
        {
            // only update bread crumbs if the player is 3 or more pixels away from previous
            // don't use sqaure root because it's expensive, so it 9 pixels

            if (breadCrumbs.Size() == 0)
            {
                breadCrumbs.Enqueue(centerPosition);
            }

            else if (Vector2.DistanceSquared(centerPosition, breadCrumbs.PeekLast()) >= Math.Pow(BREAD_CRUMB_DIST, 2))
            {
                breadCrumbs.Enqueue(centerPosition);
            }
        }


        public override void Draw(SpriteBatch spriteBatch, bool filler = false)
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

            if (isDebug)
            {
                spriteBatch.DrawString(Assets.debugFont, string.Format(" position: X: {0}, Y: {1}", position.X, position.Y), new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(Assets.debugFont, direction.ToString(), new Vector2(10, 30), Color.White);
                spriteBatch.DrawString(Assets.debugFont, Velocity.ToString(), new Vector2(10, 50), Color.White);
                spriteBatch.DrawString(Assets.debugFont, weapons[activeWeapon].GetAngle().ToString(), new Vector2(10, 70), Color.White);

                debugFeetRec.Draw(spriteBatch, Color.Red, false);
                debugHurtBox.Draw(spriteBatch, Color.Green, false);
                debugAnimBox.Draw(spriteBatch, Color.Purple, false);


                if (false)
                {
                    foreach (Vector2 v in breadCrumbs.GetQueue())
                    {
                        spriteBatch.Draw(Assets.pixels, new Rectangle((int)v.X, (int)v.Y, 3, 3), Color.Red);
                    }
                }

            }
        }

        public void MoveFeetCenterPosition(Vector2 newCenterPosition)
        {
            float x = newCenterPosition.X;
            float y = newCenterPosition.Y;

            feetRec.X = (int)(x - feetRec.Width / 2);
            feetRec.Y = (int)(y - feetRec.Height / 2);

            if (direction == RIGHT)
            {
                position.X = x - (float)FEET_WIdTH / 2 - FEET_OFFSET_X - HURT_BOX_OFFSET_X;
                position.Y = y - (float)FEET_HEIGHT / 2 - FEET_OFFSET_Y - HURT_BOX_OFFSET_Y;
            }
            else if (direction == LEFT)
            {
                position.X = x - (float)FEET_WIdTH / 2 - (hurtBox.Width - FEET_WIdTH - FEET_OFFSET_X) - (anim[(int)state].GetDestRec().Width - HURT_BOX_WIdTH - HURT_BOX_OFFSET_X);
                position.Y = y - (float)FEET_HEIGHT / 2 - FEET_OFFSET_Y - HURT_BOX_OFFSET_Y;
            }
        }


    }
}
