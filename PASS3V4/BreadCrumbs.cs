using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3V4
{
    public class BreadCrumbs : Queue<Vector2>
    {
        public BreadCrumbs() : base()
        {
        }

        public List<Vector2> GetQueue() => queue;
    }
}
