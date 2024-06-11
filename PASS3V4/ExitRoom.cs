//Author: Colin Wang
//File Name: ExitRoom.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: June 1, 2024
//Modified Date: June 10, 2024
//Description: Class which represents the exit room of an level. Uses a portal to exit the level. but doesn't full functional

namespace PASS3V4
{
    public class ExitRoom : Room
    {
        // check if the player has collided with the exit
        public bool IsExitCollision { get; set; } 

        /// <summary>
        /// Constructor for the ExitRoom class.
        /// </summary>
        /// <remarks>
        /// This constructor calls the base constructor of the Room class and sets the roomType to ExitRoom.
        /// Sets the <see cref="IsExitCollision"/> property to false.
        /// </remarks>
        public ExitRoom(): base()
        {
            // Set the roomType to ExitRoom
            roomType = RoomType.ExitRoom;

            // Set the IsExitCollision property to false
            IsExitCollision = false;
        }
    }
}
