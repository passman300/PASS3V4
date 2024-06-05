using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class Skeleton : Mob
    {
        private const int DAMAGE = 5;
        private const int HEALTH = 10;
        private const int SPEED = 2;
        private const int ATTACKDELAY = 1000;
        private const int RANGE = 5;

        

        public Skeleton(ContentManager content, GraphicsDevice graphicsDevice) : 
            base(content, graphicsDevice, MobType.Skeleton, DAMAGE, HEALTH, SPEED, RANGE)
        {

        }
    }
}
