using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PASS3V4
{
    public class TileTemplate
    {
        public bool IsCollision { get; set; }
        public int Damage { get; set; }
        public bool IsAnimated { get; set; }
        public int AnimationDur { get; set; }
        public Texture2D Image { get; set; }
        public OrderedSet<Rectangle> HitBoxes { get; set; }

        public OrderedSet<int> Frames { get; set; }

        public TileTemplate()
        {
            Frames = new OrderedSet<int>();

            HitBoxes = new OrderedSet<Rectangle>();
        }

        //public void SetAllProperties(bool isCollision, int damage, bool isAnimated, int animationDur)
        //{
        //    IsCollision = isCollision;

        //    Damage = damage;

        //    IsAnimated = isAnimated;

        //    AnimationDur = animationDur;
        //}
    }
}
