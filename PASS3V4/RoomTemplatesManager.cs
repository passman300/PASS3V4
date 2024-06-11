//Author: Colin Wang
//File Name: RoomTemplatesManager.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 30, 2024
//Modified Date: June 10, 2024
//Description: manages all the types of room templates, based on the type of room


using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace PASS3V4
{
    public class RoomTemplatesManager
    {
        // dictionary of room templates
        Dictionary<Room.RoomType, RoomTemplate> roomTemplates = new Dictionary<Room.RoomType, RoomTemplate>(); // <room type, room template>

        /// <summary>
        /// return is a room exists in the room templates
        /// </summary>
        /// <param name="roomType"></param>
        /// <returns> true if the room exists otherwise false </returns>
        public bool HasRoom(Room.RoomType roomType)
        {
            return roomTemplates.ContainsKey(roomType);
        }

        /// <summary>
        /// Gets the room template from the given room type
        /// </summary>
        /// <param name="roomType"></param>
        /// <returns> room template </returns>
        public RoomTemplate GetRoomTemplate(Room.RoomType roomType)
        {
            return roomTemplates[roomType];
        }

        /// <summary>
        /// add a new room template
        /// </summary>
        /// <param name="roomType"></param>
        /// <param name="roomTemplate"></param>
        public void AddRoomTemplate( Room.RoomType roomType, RoomTemplate roomTemplate)
        {
            roomTemplates.Add(roomType, roomTemplate);
        }

        ///  <summary>
        /// update the room template, based on the current room type
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="roomType"></param>
        public void UpdateRoomTemplate(GameTime gameTime, Room.RoomType roomType)
        {
            roomTemplates[roomType].Update(gameTime);
        }

    }
}
