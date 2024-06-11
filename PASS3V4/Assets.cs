//Author: Colin Wang, based on Alexander Protuc Assets class
//File Name: Assets.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: March 20, 2024, Remade on April 1, 2024, Modified on May 12 2024 for this project
//Modified Date: June 6, 2024
//Description: Assets class for the game, loads all assets (fonts, images, sounds, etc.) to the game

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PASS3V4
{
    public class Assets
    {
        // local content manager
        public static ContentManager Content { get; set; }

        private static string loadPath; // path to load assets from

        #region Fonts
        public static SpriteFont debugFont; // debug font

        #endregion

        #region Images
        #region Misc
        // blank pixel
        public static Texture2D pixel;

        // tile set image
        public static Texture2D dungeonTileSetImg;



        #endregion

        #region Sprites
        #region MobSprites
        public static Texture2D skullImg; // skull image
        #endregion

        #region Player
        // if you are looking for the player sprite, it is loaded in Player.cs when initializing the animation 
        #endregion

        #region Weapons
        public static Texture2D weaponSetImg; // weapon set image
        public static Texture2D fireballImg; // fireball image
        #endregion

        #region UI
        public static Texture2D frameImg; // health frame image
        public static Texture2D barImg; // health bar image
        #endregion


        #endregion

        #endregion

        // audio 


        // songs

        /// <summary>
        /// method loads all assets to the game
        /// </summary>
        public static void Initialize()
        {
            #region Fonts
            loadPath = "Fonts";

            debugFont = Load<SpriteFont>("DebugFont");

            #endregion

            #region Images

            #region Misc
            loadPath = "Images/Tileset";
            dungeonTileSetImg = Load<Texture2D>("RotatedSetImage");
            #endregion

            #region Sprites
            loadPath = "Images/Sprites";

            // mob sprites
            skullImg = Load<Texture2D>("Enemies/Skull");

            // weapon set image
            weaponSetImg = Load<Texture2D>("Weapons/WeaponsSetImage");
            fireballImg = Load<Texture2D>("Weapons/Fireball");

            // blank pixel
            pixel = Load<Texture2D>("BlankPixel");

            // UI
            frameImg = Load<Texture2D>("UI/HealthFrameImage");
            barImg = Load<Texture2D>("UI/HealthBarImage");
            #endregion
            #endregion
        }

        /// <summary>
        /// method loads an asset
        /// </summary>
        /// <typeparam name="T"></typeparam> the type of the asset
        /// <param name="file"></param> file to load
        /// <returns></returns>\        `       
        private static T Load<T>(string file) => Content.Load<T>($"{loadPath}/{file}");
    }
}
