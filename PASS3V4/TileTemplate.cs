//Author: Colin Wang
//File Name: TileTemplate.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 5, 2024
//Modified Date: June 10, 2024
//Description: The basic template for a tile, contains basic properties of all tiles like the image, damage, and hitboxes

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PASS3V4
{
    public class TileTemplate
    {
        public bool IsCollision { get; set; } // if the tile is a collision
        public int Damage { get; set; } // store the damage of the tile
        public bool IsAnimated { get; set; } // if the tile is animated
        public int AnimationDur { get; set; } // store the duration of the animation
        public Texture2D Image { get; set; } // store the image of the tile
        public OrderedSet<Rectangle> HitBoxes { get; set; } // store the hitboxes of the tile

        public OrderedSet<int> Frames { get; set; } // store the frames of the tile

        /// <summary>
        /// Constructs a new TileTemplate
        /// </summary>
        public TileTemplate()
        {
            Frames = new OrderedSet<int>(); // initialize the frames

            HitBoxes = new OrderedSet<Rectangle>(); // initialize the hitboxes
        }
    }
}
