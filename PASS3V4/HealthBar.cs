//Author: Colin Wang
//File Name: HealthBar.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: June 1, 2024
//Modified Date: June 10, 2024
//Description: Class which represents the exit room of an level. Uses a portal to exit the level. but doesn't full functional

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace PASS3V4
{
    public class HealthBar
    {
        // Default width and height for the health bar
        public const int DEFAULT_HEALTHBAR_WIDTH = 100;
        public const int DEFAULT_HEALTHBAR_HEIGHT = 10;

        // The color of the health bar
        private Color color;
        // The current health of the health bar
        private int health;
        // The maximum health of the health bar
        private int maxHealth;

        // The size of the health bar
        private Vector2 size;
        // The position of the top-left corner of the health bar
        private Vector2 pos;
        // The center position of the health bar
        private Vector2 centerPos;

        // The rectangle defining the frame of the health bar
        private Rectangle frameBox;
        // The rectangle defining the health portion of the health bar
        private Rectangle healthBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthBar"/> class.
        /// </summary>
        /// <param name="color">The color of the health bar.</param>
        /// <param name="health">The current health of the health bar.</param>
        /// <param name="maxHealth">The maximum health of the health bar.</param>
        /// <param name="size">The size of the health bar.</param>
        public HealthBar(Color color, int health, int maxHealth, Vector2 size)
        {
            // Set the color of the health bar
            this.color = color;

            // Set the current health of the health bar
            this.health = health;

            // Set the maximum health of the health bar
            this.maxHealth = maxHealth;

            // Set the size of the health bar
            this.size = size;
        }

        /// <summary>
        /// Updates the health bar with the current health and the new center position.
        /// </summary>
        /// <param name="currentHealth">The current health of the health bar.</param>
        /// <param name="centerPos">The new center position of the health bar.</param>
        public void Update(int currentHealth, Vector2 centerPos)
        {
            // Update the current health of the health bar
            health = currentHealth;

            // Calculate the new position of the top-left corner of the health bar
            pos = new Vector2(centerPos.X - size.X / 2, centerPos.Y - size.Y / 2);

            // Define the rectangle defining the frame of the health bar
            frameBox = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);

            // Define the rectangle defining the health portion of the health bar
            healthBox = new Rectangle((int)pos.X, (int)pos.Y, (int)(size.X * ((float)health / maxHealth)), (int)size.Y);
        }

        /// <summary> <summary>
        /// main draw code for the health car
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="offset"></param>
        public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            spriteBatch.Draw(Assets.frameImg, new Rectangle((int)(pos.X + offset.X), (int)(pos.Y + offset.Y), (int)size.X, (int)size.Y), Color.White);
            spriteBatch.Draw(Assets.barImg, new Rectangle((int)(pos.X + offset.X), (int)(pos.Y + offset.Y), healthBox.Width, (int)size.Y), color);
        }

        /// <summary>
        /// Draw the health bar with unspecified textures.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="frameTexture"></param>
        /// <param name="barTexture"></param>
        public void Draw(SpriteBatch spriteBatch, Texture2D frameTexture, Texture2D barTexture)
        {
            spriteBatch.Draw(frameTexture, frameBox, color);
            spriteBatch.Draw(barTexture, healthBox, color);
        }

    }
}
