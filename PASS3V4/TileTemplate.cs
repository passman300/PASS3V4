using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3V4
{
    public class TileTemplate
    {
        public bool IsCollision { get; set; }
        public int Damage { get; set; }

        public bool IsAnimated { get; set; }
        public int AnimationDur { get; set; }

        public TileTemplate()
        {
        }

        public void SetAllProperties(bool isCollision, int damage, bool isAnimated, int animationDur)
        {
            IsCollision = isCollision;

            Damage = damage;

            IsAnimated = isAnimated;

            AnimationDur = animationDur;
        }
    }
}
