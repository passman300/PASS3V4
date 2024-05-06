using System;
using System.Collections.Generic;
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
    public class Player
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

        const byte LEFT = 0;
        const byte RIGHT = 1;

        const int BREAD_CRUMB_DIST = 10;

        private ContentManager Content;

        private GraphicsDevice graphicsDevice;

        private State state = State.Idle;

        private Animation[] anim;

        private Rectangle hurtBox;
        private GameRectangle debugHurtBox;
        private GameRectangle debugAnimBox;
        private Vector2 position;
        private Vector2 centerPosition;
        private float speed;

        private byte direction;

        private float health;

        private List<Vector2> breadCrumbs = new List<Vector2>();

        private bool DEBUG = true;



        public Player(ContentManager content, GraphicsDevice graphicsDevice, string filePath)
        {
            Content = content;

            this.graphicsDevice = graphicsDevice;

            // Load animations
            anim = Animation.LoadAnimations("AnimationSheets/" + filePath, content);

            hurtBox = new Rectangle(HURT_BOX_OFFSET_X, HURT_BOX_OFFSET_Y, HURT_BOX_WIDTH, HURT_BOX_HEIGHT);

            debugHurtBox = new GameRectangle(graphicsDevice, hurtBox.X, hurtBox.Y, hurtBox.Width, hurtBox.Height);

            debugAnimBox = new GameRectangle(graphicsDevice, anim[0].GetDestRec().X, anim[0].GetDestRec().Y, anim[0].GetDestRec().Width, anim[0].GetDestRec().Height);

            speed = SPEED_BASE;
        }

        public Vector2 GetPosition() { return position; }

        public void SetPosition(Vector2 position) { this.position = position; }

        public Rectangle GetHurtBox() { return hurtBox; }
        public void SetHurtBox(Rectangle hurtBox) { this.hurtBox = hurtBox; }

        public void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb)
        {
            anim[(int)state].Update(gameTime);

            switch (state)
            {
                case State.Idle:
                    UpdateIdle(gameTime, kb);
                    break;
                case State.Walk:
                    UpdateWalk(gameTime, kb);
                    break;
            }
        }

        private void UpdateWalk(GameTime gameTime, KeyboardState kb)
        {
            if (!kb.IsKeyDown(Keys.W) && !kb.IsKeyDown(Keys.S) && !kb.IsKeyDown(Keys.A) && !kb.IsKeyDown(Keys.D)) state = State.Idle;
            else
            {
                // movement: up, down, left, right
                byte up = kb.IsKeyDown(Keys.W) ? (byte)1 : (byte)0;
                byte down = kb.IsKeyDown(Keys.S) ? (byte)1 : (byte)0;
                byte left = kb.IsKeyDown(Keys.A) ? (byte)1 : (byte)0;
                byte right = kb.IsKeyDown(Keys.D) ? (byte)1 : (byte)0;

                if (left == 1 && right == 0) direction = LEFT;
                if (right == 1 && left == 0) direction = RIGHT;

                Vector2 velocity = new Vector2(right - left, down - up);

                if (velocity.LengthSquared() > 0)
                {
                    velocity = Vector2.Normalize(velocity) * speed;
                }

                position += velocity;
            }


            // update the animation and hurt box
            UpdateRecs(position);

            // update bread crumbs
            UpdateBreadCrumbs();
        }

        /// <summary>
        /// Update both the animation and the hurtbox rectangles
        /// </summary>
        private void UpdateRecs(Vector2 position)
        {
            // update the location of the animation
            anim[(int)state].TranslateTo(position.X, position.Y);

            // since the when the animation flips, the hurtbox needs to be adjusted
            if (direction == RIGHT)
            {
                hurtBox.X = (int)(position.X + HURT_BOX_OFFSET_X);
                hurtBox.Y = (int)(position.Y + HURT_BOX_OFFSET_Y);
            }
            else if (direction == LEFT)
            {
                hurtBox.X = (int)(position.X + anim[(int)state].GetDestRec().Width - HURT_BOX_OFFSET_X - HURT_BOX_WIDTH);
                hurtBox.Y = (int)(position.Y + HURT_BOX_OFFSET_Y);
            }


            centerPosition = hurtBox.Center.ToVector2();

            debugHurtBox = new GameRectangle(graphicsDevice, hurtBox.X, hurtBox.Y, hurtBox.Width, hurtBox.Height);
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



        public void Draw(SpriteBatch spriteBatch)
        {
            if (direction == LEFT)
            {
                anim[(int)state].Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
            }
            else if (direction == RIGHT)
            {
                anim[(int)state].Draw(spriteBatch, Color.White);
            }

            if (DEBUG)
            {
                debugHurtBox.Draw(spriteBatch, Color.Blue * 0.5f, true);

                debugAnimBox.Draw(spriteBatch, Color.Red * 0.5f, true);

                foreach (Vector2 v in breadCrumbs)
                {
                    spriteBatch.Draw(Assets.pixels, new Rectangle((int)v.X, (int)v.Y, 3, 3), Color.Red);
                }
            }


        }


    }
}
