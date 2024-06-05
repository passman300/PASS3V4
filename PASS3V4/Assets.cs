using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3V4
{
    public class Assets
    {
        public static ContentManager Content { get; set; }

        private static string loadPath; // path to load assets from

        // fonts
        public static SpriteFont debugFont;

        // images
        public static Texture2D pixels;
        public static Texture2D dungeonTileSetImg;

        public static Texture2D weaponSetImg;

        // audio  78i68u


        // songs

        /// <summary>
        /// method loads all assets to the game
        /// </summary>
        public static void Initialize()
        {
            // load fonts
            loadPath = "Fonts";

            debugFont = Load<SpriteFont>("DebugFont");


            // load all images
            loadPath = "Images/Sprites";

            pixels = Load<Texture2D>("BlankPixel");

            loadPath = "Images/Tileset";
            dungeonTileSetImg = Load<Texture2D>("RotatedSetImage");
            weaponSetImg = Load<Texture2D>("WeaponsSetImage");
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
