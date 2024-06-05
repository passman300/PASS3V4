using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3V4
{
    public class ExitRoom : Room
    {
        public bool IsExitCollision { get; set; }

        public ExitRoom(): base()
        {
            IsExitCollision = false;

            roomType = RoomType.ExitRoom;
        }
    }
}
