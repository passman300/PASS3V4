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
        private const int LEVEL_WIDTH = 9;
        private const int LEVEL_HEIGHT = 8;

        private const int CENTER_X = 5;
        private const int CENTER_Y = 4;

        private int levelNumber;
        private int numberOfRooms;

        private (int x, int y) currentRoom;

        private int[,] roomSeeds = new int[LEVEL_WIDTH, LEVEL_HEIGHT];
        private Room[,] rooms = new Room[LEVEL_WIDTH, LEVEL_HEIGHT];

        public Level(int levelNumber)
        {
            this.levelNumber = levelNumber;
            numberOfRooms = Game1.rng.Next(5, 10) * levelNumber;

            for (int x = 0; x < LEVEL_WIDTH; x++)
            {
                for (int y = 0; y < LEVEL_HEIGHT; y++)
                {
                    roomSeeds[x, y] = -1;
                    rooms[x, y] = new Room();
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

            while (!roomQueue.IsEmpty() && roomQueue.Size() < numberOfRooms)
            {
                (int x, int y) currentRoom = roomQueue.Dequeue();

                if (currentRoom.x == CENTER_X && currentRoom.y == CENTER_Y) roomSeeds[currentRoom.x, currentRoom.y] = 0;
                else roomSeeds[currentRoom.x, currentRoom.y] = Game1.rng.Next(0, 100);

                bool top = Game1.rng.Next(0, 2) == 0;
                bool bottom = Game1.rng.Next(0, 2) == 0;
                bool right = Game1.rng.Next(0, 2) == 0;
                bool left = Game1.rng.Next(0, 2) == 0;

                if (top && currentRoom.y > 0 && roomSeeds[currentRoom.x, currentRoom.y - 1] == -1)
                {
                    roomQueue.Enqueue((currentRoom.x, currentRoom.y - 1));
                    rooms[currentRoom.x, currentRoom.y].adjacentRooms.topRoom = (currentRoom.x, currentRoom.y - 1);
                }
                if (bottom && currentRoom.y < LEVEL_HEIGHT - 1 && roomSeeds[currentRoom.x, currentRoom.y + 1] == -1)
                {
                    roomQueue.Enqueue((currentRoom.x, currentRoom.y + 1));
                    rooms[currentRoom.x, currentRoom.y].adjacentRooms.bottomRoom = (currentRoom.x, currentRoom.y + 1);
                }
                if (right && currentRoom.x < LEVEL_WIDTH - 1 && roomSeeds[currentRoom.x + 1, currentRoom.y] == -1)
                {
                    roomQueue.Enqueue((currentRoom.x + 1, currentRoom.y));
                    rooms[currentRoom.x, currentRoom.y].adjacentRooms.rightRoom = (currentRoom.x + 1, currentRoom.y);
                }
                if (left && currentRoom.x > 0 && roomSeeds[currentRoom.x - 1, currentRoom.y] == -1)
                {
                    roomQueue.Enqueue((currentRoom.x - 1, currentRoom.y));
                }
                rooms[currentRoom.x, currentRoom.y].adjacentRooms.leftRoom = (currentRoom.x - 1, currentRoom.y);
            }
        }

        private void GenerateRooms(GraphicsDevice graphicsDevice)
        {
            for (int x = 0; x < LEVEL_WIDTH; x++)
            {
                for (int y = 0; y < LEVEL_HEIGHT; y++)
                {
                    if (roomSeeds[x, y] != -1)
                    {
                        rooms[x, y].LoadTileMap(graphicsDevice, "Tiled/BasicRoom.tmx");
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
            rooms[currentRoom.x, currentRoom.y].Update(gameTime, player, kb, prevKb, mouse, prevMouse);
        }

        public void Draw(SpriteBatch spriteBatch, Player player)
        {

            rooms[currentRoom.x, currentRoom.y].DrawBack(spriteBatch);

            player.Draw(spriteBatch, Player.isDebug);

            rooms[currentRoom.x, currentRoom.y].DrawEntities(spriteBatch);

            rooms[currentRoom.x, currentRoom.y].DrawFront(spriteBatch);

            // DEBUG
            if (TileMap.isDebug) rooms[currentRoom.x, currentRoom.y].DrawWallHitboxes(spriteBatch);
        }

    }
}
