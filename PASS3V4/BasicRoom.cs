//Author: Colin Wang
//File Name: BasicRoom.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 31, 2024
//Modified Date: June 10, 2024
//Description: Basicroom class for the game, should have 'Basic' mob spawning 


namespace PASS3V4
{
    public class BasicRoom : Room
    {
        /// <summary>
        /// Constructor for the BasicRoom class. Sets the roomType to BasicRoom.
        /// </summary>
        /// <remarks>
        /// This constructor calls the base constructor of the Room class.
        /// </remarks>
        public BasicRoom() : base()
        {
            // Set the roomType to BasicRoom
            roomType = RoomType.BasicRoom;
        }
    }
}
