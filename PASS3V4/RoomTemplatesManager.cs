using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameUtility;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PASS3V4
{
    public class RoomTemplatesManager
    {
        Dictionary<Room.RoomType, RoomTemplate> roomTemplates = new Dictionary<Room.RoomType, RoomTemplate>(); // <room type, room template>

        public bool HasRoom(Room.RoomType roomType)
        {
            return roomTemplates.ContainsKey(roomType);
        }

        public RoomTemplate GetRoomTemplate(Room.RoomType roomType)
        {
            return roomTemplates[roomType];
        }

        public void AddRoomTemplate( Room.RoomType roomType, RoomTemplate roomTemplate)
        {
            roomTemplates.Add(roomType, roomTemplate);
        }

        public void UpdateRoomTemplate(GameTime gameTime, Room.RoomType roomType)
        {
            roomTemplates[roomType].Update(gameTime);
        }

    }
}
