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
    public class Level
    {
        private const int LEVEL_WIdTH = 9;
        private const int LEVEL_HEIGHT = 8;

        private const int CENTER_X = 5;
        private const int CENTER_Y = 4;

        public bool IsNextLevel { get; private set; }

        private int levelNumber;
        private int numberOfRooms;

        private (int x, int y) currentRoom;

        private int[,] roomSeeds = new int[LEVEL_WIdTH, LEVEL_HEIGHT];
        private Dictionary<(int x, int y), Room> rooms = new();

        public static bool isDebug = true;

        public Level(int levelNumber)
        {
            this.levelNumber = levelNumber;
            numberOfRooms = Game1.rng.Next(5, 10) * levelNumber;

            for (int x = 0; x < LEVEL_WIdTH; x++)
            {
                for (int y = 0; y < LEVEL_HEIGHT; y++)
                {
                    roomSeeds[x, y] = -1;
                }
            }
            currentRoom = (CENTER_X, CENTER_Y);
        }

        public void Generate(GraphicsDevice graphicsDevice)
        {
            GenerateSeeds();
            GenerateRooms(graphicsDevice);
        }

        private void GenerateSeeds()
        {
            Queue<(int, int)> roomQueue = new();
            roomQueue.Enqueue((CENTER_X, CENTER_Y));

            rooms[(CENTER_X, CENTER_Y)] = new Room();


            while (!roomQueue.IsEmpty() && roomQueue.Size() < numberOfRooms)
            {
                (int x, int y) tempRoom = roomQueue.Dequeue();

                if (tempRoom.x == CENTER_X && tempRoom.y == CENTER_Y) roomSeeds[tempRoom.x, tempRoom.y] = 0;
                else roomSeeds[tempRoom.x, tempRoom.y] = Game1.rng.Next(0, 100);

                bool top = Game1.rng.Next(0, 2) == 0;
                bool bottom = Game1.rng.Next(0, 2) == 0;
                bool right = Game1.rng.Next(0, 2) == 0;
                bool left = Game1.rng.Next(0, 2) == 0;

                while (!(top || bottom || right || left))
                {
                    top = Game1.rng.Next(0, 2) == 0;
                    bottom = Game1.rng.Next(0, 2) == 0;
                    right = Game1.rng.Next(0, 2) == 0;
                    left = Game1.rng.Next(0, 2) == 0;
                }

                if (top && tempRoom.y > 0 && roomSeeds[tempRoom.x, tempRoom.y - 1] == -1)
                {
                    roomQueue.Enqueue((tempRoom.x, tempRoom.y - 1));
                    rooms[(tempRoom.x, tempRoom.y)].adjacentRooms.SetTop((tempRoom.x, tempRoom.y - 1));

                    rooms[(tempRoom.x, tempRoom.y - 1)] = new Room();
                    rooms[(tempRoom.x, tempRoom.y - 1)].adjacentRooms.SetBottom((tempRoom.x, tempRoom.y));
                }
                if (bottom && tempRoom.y < LEVEL_HEIGHT - 1 && roomSeeds[tempRoom.x, tempRoom.y + 1] == -1)
                {
                    roomQueue.Enqueue((tempRoom.x, tempRoom.y + 1));
                    rooms[(tempRoom.x, tempRoom.y)].adjacentRooms.SetBottom((tempRoom.x, tempRoom.y + 1));

                    rooms[(tempRoom.x, tempRoom.y + 1)] = new Room();
                    rooms[(tempRoom.x, tempRoom.y + 1)].adjacentRooms.SetTop((tempRoom.x, tempRoom.y));
                }
                if (right && tempRoom.x < LEVEL_WIdTH - 1 && roomSeeds[tempRoom.x + 1, tempRoom.y] == -1)
                {
                    roomQueue.Enqueue((tempRoom.x + 1, tempRoom.y));
                    rooms[(tempRoom.x, tempRoom.y)].adjacentRooms.SetRight((tempRoom.x + 1, tempRoom.y));

                    rooms[(tempRoom.x + 1, tempRoom.y)] = new Room();
                    rooms[(tempRoom.x + 1, tempRoom.y)].adjacentRooms.SetLeft((tempRoom.x, tempRoom.y));
                }
                if (left && tempRoom.x > 0 && roomSeeds[tempRoom.x - 1, tempRoom.y] == -1)
                {
                    roomQueue.Enqueue((tempRoom.x - 1, tempRoom.y));
                    rooms[(tempRoom.x, tempRoom.y)].adjacentRooms.SetLeft((tempRoom.x - 1, tempRoom.y));

                    rooms[(tempRoom.x - 1, tempRoom.y)] = new Room();
                    rooms[(tempRoom.x - 1, tempRoom.y)].adjacentRooms.SetRight((tempRoom.x, tempRoom.y));
                }
            }
            
        }

        private void GenerateRooms(GraphicsDevice graphicsDevice)
        {
            for (int x = 0; x < LEVEL_WIdTH; x++)
            {
                for (int y = 0; y < LEVEL_HEIGHT; y++)
                {
                    if (roomSeeds[x, y] != -1)
                    {
                        if (roomSeeds[x, y] < 50)
                        {
                            BasicRoom tempRoom = new BasicRoom
                            {
                                adjacentRooms = rooms[(x, y)].adjacentRooms
                            };

                            rooms[(x, y)] = tempRoom;

                            if (!Room.roomTemplatesManager.HasRoom(Room.RoomType.BasicRoom))
                            {
                                rooms[(x, y)].LoadTileMap(graphicsDevice, "Tiled/BasicRoom.tmx", Room.RoomType.BasicRoom);
                            }
                        }
                        else if (roomSeeds[x, y] >= 50)
                        {
                            ExitRoom tempRoom = new()
                            {
                                adjacentRooms = rooms[(x, y)].adjacentRooms
                            };

                            rooms[(x, y)] = tempRoom;

                            if (!Room.roomTemplatesManager.HasRoom(Room.RoomType.ExitRoom))
                            {
                                rooms[(x, y)].LoadTileMap(graphicsDevice, "Tiled/ExitRoom.tmx", Room.RoomType.ExitRoom);
                            }

                            rooms[(x, y)].roomType = Room.RoomType.ExitRoom;

                        }
                    }
                }
            }
        }

        public void ChangeRoom(int x, int y)
        {
            currentRoom = (x, y);
        }

        public void Update(GameTime gameTime, Player player, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse)
        {
            rooms[(currentRoom.x, currentRoom.y)].Update(gameTime, player, kb, prevKb, mouse, prevMouse);


            if (rooms[(currentRoom.x, currentRoom.y)].isDoorCollision.top)
            {
                currentRoom.y -= 1;

                // player trevels through the top door and is teleported to the bottom door of the next room
                player.MoveFeetCenterPosition(Room.roomTemplatesManager.GetRoomTemplate(rooms[(currentRoom.x, currentRoom.y)].roomType).SpawnPoints.bottom); 
            }                
                
                
            else if (rooms[(currentRoom.x, currentRoom.y)].isDoorCollision.bottom)
            {
                currentRoom.y++;

                // player travels through the bottom door and is teleported to the top door of the next room
                player.MoveFeetCenterPosition(Room.roomTemplatesManager.GetRoomTemplate(rooms[(currentRoom.x, currentRoom.y)].roomType).SpawnPoints.top);
            }
            else if (rooms[(currentRoom.x, currentRoom.y)].isDoorCollision.left)
            {
                currentRoom.x--;

                // player travels through the left door and is teleported to the right door of the next room
                player.MoveFeetCenterPosition(Room.roomTemplatesManager.GetRoomTemplate(rooms[(currentRoom.x, currentRoom.y)].roomType).SpawnPoints.right);
            }
            else if (rooms[(currentRoom.x, currentRoom.y)].isDoorCollision.right)
            {
                currentRoom.x++;

                // player travels through the right door and is teleported to the left door of the next room
                player.MoveFeetCenterPosition(Room.roomTemplatesManager.GetRoomTemplate(rooms[(currentRoom.x, currentRoom.y)].roomType).SpawnPoints.left);
            }


            // check if the room is an exit room
            if ((rooms[(currentRoom.x, currentRoom.y)].roomType == Room.RoomType.ExitRoom))
            {
                if (rooms[(currentRoom.x, currentRoom.y)] is ExitRoom room)
                {
                    if (room.IsExitCollision)
                    {
                        IsNextLevel = true;
                    }
                }
            }
                

        }

        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            rooms[(currentRoom.x, currentRoom.y)].DrawBack(spriteBatch);

            player.Draw(spriteBatch, Player.isDebug);

            rooms[(currentRoom.x, currentRoom.y)].DrawEntities(spriteBatch);

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
            }

            if (Room.isDebug)
            {
                rooms[(currentRoom.x, currentRoom.y)].DrawWallHitboxes(spriteBatch);

                spriteBatch.DrawString(Assets.debugFont, "spawn locations" + Room.roomTemplatesManager.GetRoomTemplate(rooms[(currentRoom.x, currentRoom.y)].roomType).SpawnPoints.ToString(), new Vector2(10, 700), Color.White);
            }
        }

    }
}
