//Author: Colin Wang
//File Name: Entity.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 8, 2024
//Modified Date: June 10, 2024
//Description: General entity class for both players and mobs


using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PASS3V4
{
    /// <summary>
    /// Represents an entity in the game world.
    /// Contains basic properties and methods for all entities.
    /// </summary>
    public class Entity
    {
        // direction constants of the entity
        protected const byte LEFT = 0;
        protected const byte RIGHT = 1;

        protected GraphicsDevice graphicsDevice;

        protected Texture2D img;
        protected Animation[] anim;

        protected Rectangle imgBox; // the rectangle for drawing the image
        protected Rectangle hurtBox; // the rectangle for detecting collision with other entities
        protected Point hurtBoxOffset; // the offset for hurtBox relative to the position
        protected Rectangle feetRec; // the rectangle for detecting collision with the floor
        protected Point feetRecOffset; // the offset for feetRec relative to the position

        protected Vector2 position; // the position of the entity
        protected Vector2 hurtPosition; // the position for collision detection
        protected Vector2 centerPosition; // the center position of the entity

        protected float Speed { get; set; } // the speed of the entity
        protected Vector2 Velocity { get; set; } // the velocity of the entity

        protected byte direction; // the direction of the entity (LEFT or RIGHT)

        public int CurrentHealth { get; set; } // the current health of the entity
        public int MaxHealth { get; set; } // the maximum health of the entity
        public bool IsDead { get; set; } // indicates if the entity is dead
        public int Damage { get; set; } // the damage that the entity deals

        public HealthBar HealthBar { get; set; } // the health bar of the entity

        protected GameRectangle debugFeetRec; // the rectangle for debugging the feetRec
        protected GameRectangle debugHurtBox; // the rectangle for debugging the hurtBox
        protected GameRectangle debugImgBox; // the rectangle for debugging the imgBox

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="content">The ContentManager for loading content.</param>
        /// <param name="graphicsDevice">The GraphicsDevice for drawing.</param>
        public Entity(ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        /// <summary> <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Rectangle GetFeetRec()
        {
            return feetRec;
        }

        public Rectangle GetHurtBox()
        {
            return hurtBox;
        }

        /// <summary>
        /// Sets the spawn position of the entity.
        /// </summary>
        /// <param name="spawnPos">The spawn position.</param>
        public virtual void SetSpawn(Vector2 spawnPos)
        {
            position = spawnPos;
        }

        /// <summary>
        /// Initializes the rectangles for collision detection and debugging.
        /// </summary>
        /// <param name="hurtBoxWidth">The width of the hurtBox.</param>
        /// <param name="hurtBoxHeight">The height of the hurtBox.</param>
        /// <param name="hurtBoxOffsetX">The X offset of the hurtBox relative to the position.</param>
        /// <param name="hurtBoxOffsetY">The Y offset of the hurtBox relative to the position.</param>
        /// <param name="feetWidth">The width of the feetRec.</param>
        /// <param name="feetHeight">The height of the feetRec.</param>
        /// <param name="feetOffsetX">The X offset of the feetRec relative to the position.</param>
        /// <param name="feetOffsetY">The Y offset of the feetRec relative to the position.</param>
        public virtual void InitializeRectangles(int hurtBoxWidth, int hurtBoxHeight, int hurtBoxOffsetX, int hurtBoxOffsetY,
            int feetWidth, int feetHeight, int feetOffsetX, int feetOffsetY)
        {
            anim[0].TranslateTo(position.X, position.Y);

            hurtBoxOffset = new Point(hurtBoxOffsetX, hurtBoxOffsetY);
            feetRecOffset = new Point(feetOffsetX, feetOffsetY);


            hurtBox = new Rectangle((int)position.X + hurtBoxOffset.X, (int)position.Y + hurtBoxOffset.Y, hurtBoxWidth, hurtBoxHeight);
            feetRec = new Rectangle((int)hurtBox.X + feetRecOffset.X, (int)hurtBox.Y + feetRecOffset.Y, feetWidth, feetHeight);
        }

        /// <summary>
        /// Initializes the rectangles for collision detection and debugging with frame size.
        /// </summary>
        /// <param name="hurtBoxWidth">The width of the hurtBox.</param>
        /// <param name="hurtBoxHeight">The height of the hurtBox.</param>
        /// <param name="hurtBoxOffsetX">The X offset of the hurtBox relative to the position.</param>
        /// <param name="hurtBoxOffsetY">The Y offset of the hurtBox relative to the position.</param>
        /// <param name="feetWidth">The width of the feetRec.</param>
        /// <param name="feetHeight">The height of the feetRec.</param>
        /// <param name="feetOffsetX">The X offset of the feetRec relative to the position.</param>
        /// <param name="feetOffsetY">The Y offset of the feetRec relative to the position.</param>
        /// <param name="frameSize">The size of the frame.</param>
        public virtual void InitializeRectangles(int hurtBoxWidth, int hurtBoxHeight, int hurtBoxOffsetX, int hurtBoxOffsetY,
            int feetWidth, int feetHeight, int feetOffsetX, int feetOffsetY, Vector2 frameSize)
        {
            hurtBoxOffset = new Point(hurtBoxOffsetX, hurtBoxOffsetY);
            feetRecOffset = new Point(feetOffsetX, feetOffsetY);


            hurtBox = new Rectangle((int)position.X + hurtBoxOffset.X, (int)position.Y + hurtBoxOffset.Y, hurtBoxWidth, hurtBoxHeight);
            feetRec = new Rectangle((int)hurtBox.X + feetRecOffset.X, (int)hurtBox.Y + feetRecOffset.Y, feetWidth, feetHeight);

            imgBox = new Rectangle(0, 0, feetRec.Width, feetRec.Height);
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// Updates the entity. With wall rectangles.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="wallRecs"></param>
        public virtual void Update(GameTime gameTime, Rectangle[] wallRecs) { }

        /// <summary>
        /// Updates the entity. With wall rectangles.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="wallRecs"></param>
        public virtual void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse, Rectangle[] wallRecs) { }

        /// <summary>
        /// Updates the entity's position and collision boxes.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="wallRecs">The array of wall collision rectangles.</param>
        public virtual void UpdateRecs(bool isAnimation = true, int state = 0)
        {
            if (isAnimation)
            {
                anim[state].TranslateTo(position.X, position.Y);
            }
            else
            {
                imgBox.X = (int)position.X;
                imgBox.Y = (int)position.Y;
            }

            hurtBox.X = (int)position.X + hurtBoxOffset.X;
            hurtBox.Y = (int)position.Y + hurtBoxOffset.Y;

            feetRec.X = (int)hurtBox.X + feetRecOffset.X;
            feetRec.Y = (int)hurtBox.Y + feetRecOffset.Y;

            centerPosition = hurtBox.Center.ToVector2();
        }

        /// <summary>
        /// Moves the entity based on collision detection with wall rectangles.
        /// </summary>
        /// <param name="wallRecs">The array of wall collision rectangles.</param>
        /// <param name="x">The X movement.</param>
        /// <param name="y">The Y movement.</param>
        protected virtual void Move(Rectangle[] wallRecs, float x, float y)
        {
            if (x != 0 && y != 0)
            {
                Move(wallRecs, 0, y);
                Move(wallRecs, x, 0);
                return;
            }
            position += CheckWallCollision(wallRecs, x, y).Velocity;

        }

        /// <summary>
        /// Checks for collision with wall rectangles and returns the collision information.
        /// </summary>
        /// <param name="wallRecs">The array of wall collision rectangles.</param>
        /// <param name="x">The X movement.</param>
        /// <param name="y">The Y movement.</param>
        /// <returns>A tuple indicating if there is collision and the collision velocity.</returns>
        protected virtual (bool IsCollided, Vector2 Velocity) CheckWallCollision(Rectangle[] wallRecs, float x, float y)
        {
            float newX = x;
            float newY = y;

            bool isCollided = false;

            Rectangle newFeetRec = new((int)(feetRec.X + x), (int)(feetRec.Y + y), feetRec.Width, feetRec.Height);

            foreach (Rectangle rec in wallRecs) // Check collision with all walls
            {
                if (newFeetRec.Intersects(rec)) // Check collision with current wall
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
    

        /// <summary>
        /// Draws the entity on the sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch for drawing.</param>
        /// <param name="isDebug">Indicates if debug information should be drawn.</param>
        public virtual void Draw(SpriteBatch spriteBatch, bool isDebug = false)
        {
            // ...
        }

        /// <summary>
        /// Moves the feet center position of the entity.
        /// </summary>
        /// <param name="newCenterPosition">The new center position.</param>
        public virtual void MoveFeetCenterPosition(Vector2 newCenterPosition)
        {
            position.X = newCenterPosition.X - ((float)feetRec.Width / 2) - feetRecOffset.X - hurtBoxOffset.X;
            position.Y = newCenterPosition.Y - ((float)feetRec.Height / 2) - feetRecOffset.Y - hurtBoxOffset.Y;

            InitializeRectangles(hurtBox.Width, hurtBox.Height, hurtBoxOffset.X, hurtBoxOffset.Y, feetRec.Width, feetRec.Height, feetRecOffset.X, feetRecOffset.Y);
        }
    }
}
