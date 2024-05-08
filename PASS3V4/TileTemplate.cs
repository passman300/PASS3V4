using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace PASS3V4
{
    public class TileTemplate
    {
        public bool IsCollision { get; set; }
        public int Damage { get; set; }
        public bool IsAnimated { get; set; }
        public int AnimationDur { get; set; }
        public Texture2D Image { get; set; }

        private OrderedSet<int> frames = new OrderedSet<int>();

        public OrderedSet<int> Frames
        {
            get { return frames; }
            set { frames = value; }
        }

        public TileTemplate()
        {

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
