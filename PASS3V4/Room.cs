using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameUtility;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace PASS3V4
{
    public class Room
    {
        public struct AdjacentRooms
        {
            public (int x, int y) topRoom;
            public (int x, int y) bottomRoom;
            public (int x, int y) leftRoom;
            public (int x, int y) rightRoom;

            public AdjacentRooms()
            {
                topRoom = (-1, -1);
                bottomRoom = (-1, -1);
                leftRoom = (-1, -1);
                rightRoom = (-1, -1);

                //this.topRoom = topRoom;
                //this.bottomRoom = bottomRoom;
                //this.leftRoom = leftRoom;
                //this.rightRoom = rightRoom;
            }

            public (int x, int y) SetTop((int x, int y) topRoom) => this.topRoom = topRoom;
            public (int x, int y) SetBottom((int x, int y) bottomRoom) => this.bottomRoom = bottomRoom;
            public (int x, int y) SetLeft((int x, int y) leftRoom) => this.leftRoom = leftRoom;
            public (int x, int y) SetRight((int x, int y) rightRoom) => this.rightRoom = rightRoom;
        }

        public struct IsDoorCollision
        {
            public bool top;
            public bool bottom;
            public bool right;
            public bool left;
        }

        public enum RoomType
        {
            SpawnRoom,
            ExitRoom,
            BasicRoom,
            BossRoom
        }

        public const int SPAWN_OFFSET = 30;

        public RoomType roomType;

        public AdjacentRooms adjacentRooms = new AdjacentRooms();

        public IsDoorCollision isDoorCollision = new IsDoorCollision();

        public static RoomTemplatesManager roomTemplatesManager = new RoomTemplatesManager();

        protected int mobSpawns;
        protected List<Entity> mobs = new();

        protected Vector2 spawnPosition;

        protected List<Arrow> flyingArrows = new();

        public static bool isDebug = true;

        public Room()
        {
            // TDOD: Load room from file
            
            /*
             * create a file system based on the types of rooms
             *      each floder contains a tsx and a txt of room stats
             *      
             * create the room stats of mobs, waves, if 
             */
        }

        public virtual void LoadRoom(GraphicsDevice graphicsDevice, string tileMapPath, string roomStatsPath, RoomType roomType)
        {
            LoadTileMap(graphicsDevice, tileMapPath, roomType);

        }

        public virtual void LoadTileMap(GraphicsDevice graphicsDevice, string filePath, RoomType roomType)
        {
            TileMapReader tileMapReader = new TileMapReader(filePath);

            tileMapReader.LoadTileMapFile(graphicsDevice);

            roomTemplatesManager.AddRoomTemplate(roomType, tileMapReader.GetRoomTemplate());

            //BackLayers = tileMapReader.GetBackLayers();
            //FrontLayers = tileMapReader.GetFrontLayers();
            //WallRecs = tileMapReader.GetWallRecs();
            //DoorRec = tileMapReader.GetDoorRec();
        }

        public virtual void Update(GameTime gameTime, Player player, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse)
        {
            roomTemplatesManager.UpdateRoomTemplate(gameTime, roomType);

            player.Update(gameTime, kb, prevKb, mouse, prevMouse, roomTemplatesManager.GetRoomTemplate(roomType).WallRecs.ToArray());

            for (int i = 0; i < mobs.Count; i++)
            {
                mobs[i].Update(gameTime, kb, prevKb, mouse, prevMouse, roomTemplatesManager.GetRoomTemplate(roomType).WallRecs.ToArray());

                if (mobs[i].IsDead)
                {
                    mobs.RemoveAt(i);
                    i--;
                }
            }

            // check if the player collides with one of the doors;
            if (adjacentRooms.topRoom != (-1, -1) && roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.top.Intersects(player.GetFeetRec()))
            {
                isDoorCollision.top = true;
                isDoorCollision.bottom = false;
                isDoorCollision.left = false;
                isDoorCollision.right = false;
            }
            else if (adjacentRooms.bottomRoom != (-1, -1) && roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.bottom.Intersects(player.GetFeetRec()))
            {
                isDoorCollision.top = false;
                isDoorCollision.bottom = true;
                isDoorCollision.left = false;
                isDoorCollision.right = false;
            }
            else if (adjacentRooms.leftRoom != (-1, -1) && roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.left.Intersects(player.GetFeetRec()))
            {
                isDoorCollision.top = false;
                isDoorCollision.bottom = false;
                isDoorCollision.left = true;
                isDoorCollision.right = false;
            }
            else if (adjacentRooms.rightRoom != (-1, -1) && roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.right.Intersects(player.GetFeetRec()))
            {
                isDoorCollision.top = false;
                isDoorCollision.bottom = false;
                isDoorCollision.left = false;
                isDoorCollision.right = true;
            }
            else 
            {
                isDoorCollision.top = false;
                isDoorCollision.bottom = false;
                isDoorCollision.left = false;
                isDoorCollision.right = false;
            }

            if (player.GetFlyingArrow().IsShot) AddFlyingArrow(player.GetFlyingArrow().Arrow);

            UpdateArrows();

            
        }

        private void UpdateArrows()
        {
            for (int i = 0; i < flyingArrows.Count; i++)
            {
                flyingArrows[i].UpdateFlying();

                if (flyingArrows[i].HitBox.Left > Game1.SCREEN_WIdTH || flyingArrows[i].HitBox.Right < 0 || 
                    flyingArrows[i].HitBox.Top > Game1.SCREEN_HEIGHT || flyingArrows[i].HitBox.Bottom < 0)
                {
                    flyingArrows.RemoveAt(i);
                    i--;
                }
            }
        }

        public void AddFlyingArrow(Arrow arrow)
        {
            flyingArrows.Add(arrow);
        }

        private void DrawFlyingArrows(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < flyingArrows.Count; i++)
            {
                flyingArrows[i].Draw(spriteBatch);
            }
        }

        public virtual void DrawEntities(SpriteBatch spriteBatch)
        {
            DrawFlyingArrows(spriteBatch);
        }

        public void DrawFront(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < roomTemplatesManager.GetRoomTemplate(roomType).FrontLayers.Count; i++)
            {
                roomTemplatesManager.GetRoomTemplate(roomType).FrontLayers[i].Draw(spriteBatch);
            }
        }

        public void DrawBack(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < roomTemplatesManager.GetRoomTemplate(roomType).BackLayers.Count; i++)
            {
                roomTemplatesManager.GetRoomTemplate(roomType).BackLayers[i].Draw(spriteBatch);
            }

            DrawDoors(spriteBatch);
        }

        public void DrawDoors(SpriteBatch spriteBatch)
        {

            for (int i = 0; i < roomTemplatesManager.GetRoomTemplate(roomType).DoorLayers.Count; i++)
            {
                if (adjacentRooms.topRoom != (-1, -1) && roomTemplatesManager.GetRoomTemplate(roomType).DoorLayers[i].Name.Contains("Top"))
                    roomTemplatesManager.GetRoomTemplate(roomType).DoorLayers[i].Draw(spriteBatch);
                else if (adjacentRooms.bottomRoom != (-1, -1) && roomTemplatesManager.GetRoomTemplate(roomType).DoorLayers[i].Name.Contains("Bottom"))
                    roomTemplatesManager.GetRoomTemplate(roomType).DoorLayers[i].Draw(spriteBatch);
                else if (adjacentRooms.leftRoom != (-1, -1) && roomTemplatesManager.GetRoomTemplate(roomType).DoorLayers[i].Name.Contains("Left"))
                    roomTemplatesManager.GetRoomTemplate(roomType).DoorLayers[i].Draw(spriteBatch);
                else if (adjacentRooms.rightRoom != (-1, -1) && roomTemplatesManager.GetRoomTemplate(roomType).DoorLayers[i].Name.Contains("Right"))
                    roomTemplatesManager.GetRoomTemplate(roomType).DoorLayers[i].Draw(spriteBatch);
            }
        }

        public void DrawWallHitboxes(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < roomTemplatesManager.GetRoomTemplate(roomType).WallRecs.Count; i++)
            {
                spriteBatch.Draw(Assets.pixels, roomTemplatesManager.GetRoomTemplate(roomType).WallRecs[i], Color.Blue * 0.2f);
            }

            if (adjacentRooms.topRoom != (-1, -1)) spriteBatch.Draw(Assets.pixels, roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.top, Color.Pink * 0.9f);
            if (adjacentRooms.bottomRoom != (-1, -1)) spriteBatch.Draw(Assets.pixels, roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.bottom, Color.Pink * 0.9f);
            if (adjacentRooms.leftRoom != (-1, -1)) spriteBatch.Draw(Assets.pixels, roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.left, Color.Pink * 0.9f);
            if (adjacentRooms.rightRoom != (-1, -1)) spriteBatch.Draw(Assets.pixels, roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.right, Color.Pink * 0.9f);
        }
    }
}
