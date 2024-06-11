//Author: Colin Wang
//File Name: Level.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 27, 2024
//Modified Date: June 10, 2024
//Description: The overall level class, which owns all the rooms

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace PASS3V4
{
    public class Level
    {
        // Level Dimensions
        private const int LEVEL_WIDTH = 9;
        private const int LEVEL_HEIGHT = 8;

        // Center Room
        private const int CENTER_X = 5;
        private const int CENTER_Y = 4;

        // Flag for next level
        public bool IsNextLevel { get; private set; }

        // Properties of the level
        private int levelNumber;
        private int numberOfRooms;

        // Current Room
        private (int x, int y) currentRoom;

        // Room generation variables
        private int[,] roomSeeds = new int[LEVEL_WIDTH, LEVEL_HEIGHT];
        private Dictionary<(int x, int y), Room> rooms = new();

        // DEBUG
        public static bool isDebug = false;

        /// <summary>
        /// Constructor for the Level class.
        /// </summary>
        /// <param name="levelNumber"></param> level number
        public Level(int levelNumber)
        {
            this.levelNumber = levelNumber;

            // Generate the number of rooms based on the level number
            numberOfRooms = Game1.rng.Next(5, 10) * levelNumber;

            // loop through all the rooms and set them to -1
            for (int x = 0; x < LEVEL_WIDTH; x++)
            {
                for (int y = 0; y < LEVEL_HEIGHT; y++)
                {
                    roomSeeds[x, y] = -1;
                }
            }
            // set the current room to be the center room
            currentRoom = (CENTER_X, CENTER_Y);
        }

        /// <summary>
        /// Generates the rooms for the level.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="graphicsDevice"></param>
        public void Generate(ContentManager content, GraphicsDevice graphicsDevice)
        {
            GenerateSeeds(); // Generate the seeds for the rooms
            GenerateRooms(content, graphicsDevice); // Generate the rooms
        }

        /// <summary>
        /// Generates the seeds for the rooms.
        /// </summary>
        private void GenerateSeeds()
        {
            Queue<(int, int)> roomQueue = new(); // Queue of rooms to be generated used for BFS
            roomQueue.Enqueue((CENTER_X, CENTER_Y));

            rooms[(CENTER_X, CENTER_Y)] = new Room();

            // BFS through the rooms and 
            while (!roomQueue.IsEmpty() && roomQueue.Size() < numberOfRooms)
            {
                (int x, int y) tempRoom = roomQueue.Dequeue(); // get the next room from the queue

                if (tempRoom.x == CENTER_X && tempRoom.y == CENTER_Y) roomSeeds[tempRoom.x, tempRoom.y] = 0;
                else roomSeeds[tempRoom.x, tempRoom.y] = Game1.rng.Next(0, 100); // set the seed for the room from 0 to 100

                // Generate the adjacent rooms, 1 for true, 0 for false
                bool top = Game1.rng.Next(0, 2) == 0;
                bool bottom = Game1.rng.Next(0, 2) == 0;
                bool right = Game1.rng.Next(0, 2) == 0;
                bool left = Game1.rng.Next(0, 2) == 0;

                // ensure that the room has at least one adjacent room
                while (!(top || bottom || right || left))
                {
                    top = Game1.rng.Next(0, 2) == 0;
                    bottom = Game1.rng.Next(0, 2) == 0;
                    right = Game1.rng.Next(0, 2) == 0;
                    left = Game1.rng.Next(0, 2) == 0;
                }

                // check if the queue is full, if so break
                if (roomQueue.Size() == numberOfRooms) break;


                // check all adjacent rooms, and add them to the queue if they are not already in the queue and within the level
                // check top room
                if (top && tempRoom.y > 0 && roomSeeds[tempRoom.x, tempRoom.y - 1] == -1)
                {
                    roomQueue.Enqueue((tempRoom.x, tempRoom.y - 1));
                    rooms[(tempRoom.x, tempRoom.y)].adjacentRooms.SetTop((tempRoom.x, tempRoom.y - 1));

                    rooms[(tempRoom.x, tempRoom.y - 1)] = new Room();
                    rooms[(tempRoom.x, tempRoom.y - 1)].adjacentRooms.SetBottom((tempRoom.x, tempRoom.y));
                }
                // check bottom room
                if (bottom && tempRoom.y < LEVEL_HEIGHT - 1 && roomSeeds[tempRoom.x, tempRoom.y + 1] == -1)
                {
                    roomQueue.Enqueue((tempRoom.x, tempRoom.y + 1));
                    rooms[(tempRoom.x, tempRoom.y)].adjacentRooms.SetBottom((tempRoom.x, tempRoom.y + 1));

                    rooms[(tempRoom.x, tempRoom.y + 1)] = new Room();
                    rooms[(tempRoom.x, tempRoom.y + 1)].adjacentRooms.SetTop((tempRoom.x, tempRoom.y));
                }
                // check right room
                if (right && tempRoom.x < LEVEL_WIDTH - 1 && roomSeeds[tempRoom.x + 1, tempRoom.y] == -1)
                {
                    roomQueue.Enqueue((tempRoom.x + 1, tempRoom.y));
                    rooms[(tempRoom.x, tempRoom.y)].adjacentRooms.SetRight((tempRoom.x + 1, tempRoom.y));

                    rooms[(tempRoom.x + 1, tempRoom.y)] = new Room();
                    rooms[(tempRoom.x + 1, tempRoom.y)].adjacentRooms.SetLeft((tempRoom.x, tempRoom.y));
                }
                // check left room
                if (left && tempRoom.x > 0 && roomSeeds[tempRoom.x - 1, tempRoom.y] == -1)
                {
                    roomQueue.Enqueue((tempRoom.x - 1, tempRoom.y));
                    rooms[(tempRoom.x, tempRoom.y)].adjacentRooms.SetLeft((tempRoom.x - 1, tempRoom.y));

                    rooms[(tempRoom.x - 1, tempRoom.y)] = new Room();
                    rooms[(tempRoom.x - 1, tempRoom.y)].adjacentRooms.SetRight((tempRoom.x, tempRoom.y));
                }
            }

        }


        /// <summary>
        /// Generates the rooms
        /// </summary>
        /// <param name="content"></param>
        /// <param name="graphicsDevice"></param>
        private void GenerateRooms(ContentManager content, GraphicsDevice graphicsDevice)
        {

            // Loop through all possible rooms locations
            for (int x = 0; x < LEVEL_WIDTH; x++)
            {
                for (int y = 0; y < LEVEL_HEIGHT; y++)
                {
                    // Check if the room seed is not -1, if so generate the room
                    if (roomSeeds[x, y] != -1)
                    {
                        // if the room seed is less than 50, generate a basic room
                        if (roomSeeds[x, y] < 50)
                        {
                            // Create a temp basic room which consists of the adjacent rooms from the store 'currentRoom'
                            BasicRoom tempRoom = new BasicRoom
                            {
                                adjacentRooms = rooms[(x, y)].adjacentRooms
                            };

                            rooms[(x, y)] = tempRoom; // set the room to the temp room
                            rooms[(x, y)].roomType = Room.RoomType.BasicRoom; // set the room type

                            // Load the basic room
                            rooms[(x, y)].LoadRoom(content, graphicsDevice, "Tiled/BasicRoom.tmx", "RoomData/BasicRoomStats.txt", Room.RoomType.BasicRoom);

                        }
                        // if the room seed is greater than 50, generate an exit room
                        else if (roomSeeds[x, y] >= 50)
                        {
                            // Create a temp exit room which consists of the adjacent rooms from the store 'currentRoom'
                            ExitRoom tempRoom = new()
                            {
                                adjacentRooms = rooms[(x, y)].adjacentRooms
                            };

                            rooms[(x, y)] = tempRoom; // set the room to the temp room
                            rooms[(x, y)].roomType = Room.RoomType.ExitRoom; // set the room type

                            // Load the exit room
                            rooms[(x, y)].LoadRoom(content, graphicsDevice, "Tiled/ExitRoom.tmx", "RoomData/ExitRoomStats.txt", Room.RoomType.ExitRoom);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Changes the current room, based on the x and y coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ChangeRoom(int x, int y)
        {
            currentRoom = (x, y);
        }

        /// <summary>
        /// Updates the level
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="player"></param>
        /// <param name="kb"></param>
        /// <param name="prevKb"></param>
        /// <param name="mouse"></param>
        /// <param name="prevMouse"></param>
        public void Update(GameTime gameTime, Player player, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse)
        {
            // Update the current room
            rooms[(currentRoom.x, currentRoom.y)].Update(gameTime, player, kb, prevKb, mouse, prevMouse);

            // Store if the player is moving onto the next room
            bool isNextRoom = false;

            // Check if the player is colliding with the top door
            if (rooms[(currentRoom.x, currentRoom.y)].isDoorCollision.top)
            {
                currentRoom.y -= 1;

                // player trevels through the top door and is teleported to the bottom door of the next room
                player.MoveFeetCenterPosition(Room.roomTemplatesManager.GetRoomTemplate(rooms[(currentRoom.x, currentRoom.y)].roomType).SpawnPoints.bottom);

                isNextRoom = true;
            }
            // Check if the player is colliding with the bottom door
            else if (rooms[(currentRoom.x, currentRoom.y)].isDoorCollision.bottom)
            {
                currentRoom.y++;

                // player travels through the bottom door and is teleported to the top door of the next room
                player.MoveFeetCenterPosition(Room.roomTemplatesManager.GetRoomTemplate(rooms[(currentRoom.x, currentRoom.y)].roomType).SpawnPoints.top);
                isNextRoom = true;
            }
            // Check if the player is colliding with the left door
            else if (rooms[(currentRoom.x, currentRoom.y)].isDoorCollision.left)
            {
                currentRoom.x--;

                // player travels through the left door and is teleported to the right door of the next room
                player.MoveFeetCenterPosition(Room.roomTemplatesManager.GetRoomTemplate(rooms[(currentRoom.x, currentRoom.y)].roomType).SpawnPoints.right);
                isNextRoom = true;

            }
            // Check if the player is colliding with the right door
            else if (rooms[(currentRoom.x, currentRoom.y)].isDoorCollision.right)
            {
                currentRoom.x++;

                // player travels through the right door and is teleported to the left door of the next room
                player.MoveFeetCenterPosition(Room.roomTemplatesManager.GetRoomTemplate(rooms[(currentRoom.x, currentRoom.y)].roomType).SpawnPoints.left);
                isNextRoom = true;

            }

            // if the player is moving to new room, clear the player's bread crumbs
            if (isNextRoom)
            {
                player.ClearBreadCrumbs();
            }

            // check if the room is an exit room
            if ((rooms[(currentRoom.x, currentRoom.y)].roomType == Room.RoomType.ExitRoom) &&
                rooms[(currentRoom.x, currentRoom.y)] is ExitRoom room &&
                room.IsExitCollision)
            {
                IsNextLevel = true; // set the next level to be true
            }
        }

        /// <summary>
        /// Draws the level
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="player"></param>
        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            // Draw the back layer
            rooms[(currentRoom.x, currentRoom.y)].DrawBack(spriteBatch);

            // Draw the player
            player.Draw(spriteBatch, Player.isDebug);

            // Draw the entities
            rooms[(currentRoom.x, currentRoom.y)].DrawEntities(spriteBatch);

            // Draw the front layer
            rooms[(currentRoom.x, currentRoom.y)].DrawFront(spriteBatch);

            // DEBUG
            if (isDebug)
            {
                spriteBatch.DrawString(Assets.debugFont, "currentRoom: " + currentRoom.ToString(), new Vector2(500, 10), Color.White);
                spriteBatch.DrawString(Assets.debugFont, "room type" + rooms[(currentRoom.x, currentRoom.y)].roomType.ToString(), new Vector2(500, 30), Color.White);
                spriteBatch.DrawString(Assets.debugFont, "top room" + rooms[(currentRoom.x, currentRoom.y)].adjacentRooms.topRoom.ToString(), new Vector2(500, 50), Color.White);
                spriteBatch.DrawString(Assets.debugFont, "bottom room" + rooms[(currentRoom.x, currentRoom.y)].adjacentRooms.bottomRoom.ToString(), new Vector2(500, 70), Color.White);
                spriteBatch.DrawString(Assets.debugFont, "left room" + rooms[(currentRoom.x, currentRoom.y)].adjacentRooms.leftRoom.ToString(), new Vector2(500, 90), Color.White);
                spriteBatch.DrawString(Assets.debugFont, "right room" + rooms[(currentRoom.x, currentRoom.y)].adjacentRooms.rightRoom.ToString(), new Vector2(500, 110), Color.White);

                spriteBatch.DrawString(Assets.debugFont, "mob wave" + rooms[(currentRoom.x, currentRoom.y)].GetMobWaves().Size().ToString(), new Vector2(500, 130), Color.White);
            }
            // DEBUG
            if (Room.isDebug)
            {
                rooms[(currentRoom.x, currentRoom.y)].DrawWallHitboxes(spriteBatch);

                spriteBatch.DrawString(Assets.debugFont, "spawn locations" + Room.roomTemplatesManager.GetRoomTemplate(rooms[(currentRoom.x, currentRoom.y)].roomType).SpawnPoints.ToString(), new Vector2(10, 700), Color.White);
            }
        }

    }
}
