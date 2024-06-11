//Author: Colin Wang
//File Name: Room.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 7, 2024
//Modified Date: June 10, 2024
//Description: The overall room class, which is the parent of all other rooms. Contains the general variables and methods for all rooms

using System.Collections.Generic;
using System.IO;
using System.Linq;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace PASS3V4
{
    public class Room
    {
        /// <summary> 
        /// Struct to store the adjacent rooms. of each room
        /// </summary>
        public struct AdjacentRooms
        {
            // each room has 4 adjacent rooms, nothing else
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
            }

            /// <summary> 
            /// set the values of the adjacent rooms
            /// </summary>
            /// <param name="x"></param>
            /// <param name="x"></param>
            /// <param name="topRoom"></param>
            /// <returns></returns>

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

        /// <summary> 
        /// all the various types of rooms
        /// </summary>
        public enum RoomType
        {
            SpawnRoom,
            ExitRoom,
            BasicRoom,
            BossRoom
        }

        public const int SPAWN_OFFSET = 30; // the offset from the door to the spawn point

        public RoomType roomType; // the type of room

        public AdjacentRooms adjacentRooms = new(); // the adjacent rooms

        public IsDoorCollision isDoorCollision = new(); // the collision of the door

        public static RoomTemplatesManager roomTemplatesManager = new(); // the room templates

        // mob room stats
        protected int mobSpawns;
        protected Queue<List<Mob>> mobWaves = new();

        // spawnposition of the mobs are random
        protected Vector2 spawnPosition;

        // various projectiles
        protected List<Projectile> playerProjectiles = new();
        protected List<Projectile> mobProjectiles = new();

        // DEBUGs
        public static bool isDebug = false;

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

        /// <summary> 
        /// Gets the mobs waves in the room
        /// </summary>
        /// <returns></returns>
        public virtual Queue<List<Mob>> GetMobWaves() => mobWaves;

        /// <summary> 
        /// loads the room from file
        /// </summary>
        /// <param name="content"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="tileMapPath"></param>
        /// <param name="roomStatsPath"></param>
        /// <param name="roomType"></param>
        public virtual void LoadRoom(ContentManager content, GraphicsDevice graphicsDevice, string tileMapPath, string roomStatsPath, RoomType roomType)
        {
            if (!Room.roomTemplatesManager.HasRoom(roomType)) LoadTileMap(graphicsDevice, tileMapPath, roomType);

            LoadMobsData(content, graphicsDevice, roomStatsPath);
        }

        /// <summary> 
        /// Load the tile map from file and room type
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="filePath"></param>
        /// <param name="roomType"></param>
        public virtual void LoadTileMap(GraphicsDevice graphicsDevice, string filePath, RoomType roomType)
        {
            TileMapReader tileMapReader = new TileMapReader(filePath); // create the tile map reader

            tileMapReader.LoadTileMapFile(graphicsDevice); // load the tile map

            roomTemplatesManager.AddRoomTemplate(roomType, tileMapReader.GetRoomTemplate()); // add the room template from the tile map
        }

        /// <summary> 
        /// Load the mobs data from file
        /// </summary>
        /// <param name="content"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="filePath"></param>
        public virtual void LoadMobsData(ContentManager content, GraphicsDevice graphicsDevice, string filePath)
        {
            int lineCount = 0;

            try
            {
                StreamReader r = new StreamReader(filePath);
                string[] data = r.ReadToEnd().Split("\r\n"); // load all the data and split by new line

                r.Close();
                r = null;

                if (data.Length == 0 || data[0] == "") return; // if the file is empty return

                // iterate through the data by line
                foreach (string line in data)
                {
                    LoadMobWave(content, graphicsDevice, line.Split(',').Select(int.Parse).ToList()); // load the mob wave from the line split by comma and convert to int
                    lineCount++; // increment the line count
                }           
            }
            catch (System.IO.FileNotFoundException e)
            {
                // file not found
                // need to create a new file

                // create a new file, that is blank
                Game1.sw = File.CreateText(filePath);
                Game1.sw.Close();
            }
        }

        /// <summary> 
        /// Load the mob wave from the line
        /// </summary>
        /// <param name="content"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="data"></param>
        protected virtual void LoadMobWave(ContentManager content, GraphicsDevice graphicsDevice, List<int> data)
        {
            List<Mob> tempMobs = new(); // create a temporary list

            // iterate through the data
            foreach (int mob in data)
            {
                // set a spawn point for the variable mob
                Vector2 spawnPoint = new Vector2(Game1.rng.Next(roomTemplatesManager.GetRoomTemplate(roomType).SpawnArea.Left, roomTemplatesManager.GetRoomTemplate(roomType).SpawnArea.Right),
                    Game1.rng.Next(roomTemplatesManager.GetRoomTemplate(roomType).SpawnArea.Top, roomTemplatesManager.GetRoomTemplate(roomType).SpawnArea.Bottom));

                // create the mob
                switch (mob)
                {
                    case (int)Mob.MobType.Skull:
                        tempMobs.Add(new Skull(content, graphicsDevice, spawnPoint)); // create a skull
                        break;
                }
            }
            
            // add the mobs to the wave
            mobWaves.Enqueue(tempMobs);
        }

        /// <summary>
        /// update the room, as well as everything in it (player, mob, etc)
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="player"></param>
        /// <param name="kb"></param>
        /// <param name="prevKb"></param>
        /// <param name="mouse"></param>
        /// <param name="prevMouse"></param>
        public virtual void Update(GameTime gameTime, Player player, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse)
        {
            // update that specific room type
            roomTemplatesManager.UpdateRoomTemplate(gameTime, roomType);

            // update the player
            player.Update(gameTime, kb, prevKb, mouse, prevMouse, roomTemplatesManager.GetRoomTemplate(roomType).WallRecs.ToArray());

            // check if the player collides with the top door
            if (adjacentRooms.topRoom != (-1, -1) && roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.top.Intersects(player.GetFeetRec()))
            {
                // set a range of rooms that the player can travel through
                isDoorCollision.top = true;
                isDoorCollision.bottom = false;
                isDoorCollision.left = false;
                isDoorCollision.right = false;
            }
            // check if collsion with bottom door
            else if (adjacentRooms.bottomRoom != (-1, -1) && roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.bottom.Intersects(player.GetFeetRec()))
            {
                // set a range of rooms that the player can travel through
                
                isDoorCollision.top = false;
                isDoorCollision.bottom = true;
                isDoorCollision.left = false;
                isDoorCollision.right = false;
            }
            // check if collsion with left door
            else if (adjacentRooms.leftRoom != (-1, -1) && roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.left.Intersects(player.GetFeetRec()))
            {
                // set a range of rooms that the player can travel through
                isDoorCollision.top = false;
                isDoorCollision.bottom = false;
                isDoorCollision.left = true;
                isDoorCollision.right = false;
            }
            // check if collsion with right door
            else if (adjacentRooms.rightRoom != (-1, -1) && roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.right.Intersects(player.GetFeetRec()))
            {
                // set a range of rooms that the player can travel through
                isDoorCollision.top = false;
                isDoorCollision.bottom = false;
                isDoorCollision.left = false;
                isDoorCollision.right = true;
            }
            else 
            {
                // set all rooms to false (no doors)
                isDoorCollision.top = false;
                isDoorCollision.bottom = false;
                isDoorCollision.left = false;
                isDoorCollision.right = false;
            }

            // if player has shot an arrow, add it the projectiles
            if (player.GetFlyingArrow().IsShot) AddPlayerProjectiles(player.GetFlyingArrow().Arrow);


            // update the arrows (player projectiles)
            UpdateArrows();

            // update the mobs
            UpdateMobs(gameTime, player);
        }

        /// <summary>
        /// Update all arrows (player projectiles)
        /// </summary>
        private void UpdateArrows()
        {
            // iterate through the player projectiles
            for (int i = 0; i < playerProjectiles.Count; i++)
            {
                // update the arrow
                playerProjectiles[i].UpdateFlying();

                // if the arrow is out of bounds, remove it
                if (playerProjectiles[i].HitBox.Left > Game1.SCREEN_WIDTH || playerProjectiles[i].HitBox.Right < 0 ||
                    playerProjectiles[i].HitBox.Top > Game1.SCREEN_HEIGHT || playerProjectiles[i].HitBox.Bottom < 0)
                {
                    playerProjectiles.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary> 
        /// update the all mob projectiles
        /// </summary>
        /// <param name="player"></param>
        protected void UpdateMobProjectiles(Player player)
        {
            // iterate through the mob projectiles
            for (int i = 0; i < mobProjectiles.Count; i++)
            {
                // update each projectile
                mobProjectiles[i].UpdateFlying();
                
                // if the player collides with the projectile, reduce the player's health
                if (mobProjectiles[i].IsColliding(player.GetHurtBox())) player.CurrentHealth -= mobProjectiles[i].Damage;

                // if the player's health is below or equal to 0, set the player's state to die
                if (player.CurrentHealth <= 0)
                {
                    Player.state = Player.PlayerState.Die;
                    break;
                }
            }
        }

        /// <summary>
        /// Updates all mobs in the room.
        /// 
        /// This function iterates through the mob waves, updating each mob's position and health. 
        /// If a mob is dead, it is removed from the wave. If a mob is colliding with the player, the player's health is reduced.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="player">The player object.</param>
        protected virtual void UpdateMobs(GameTime gameTime, Player player)
        {
            for (int i = 0; mobWaves.Size() > 0 && (i < mobWaves.Peek().Count); i++)
            {
                // update the mob's position and health
                mobWaves.Peek()[i].Update(gameTime, roomTemplatesManager.GetRoomTemplate(roomType).WallRecs.ToArray());

                // if the mob is dead, remove it from the wave
                if (mobWaves.Peek()[i].IsDead)
                {
                    mobWaves.Peek().RemoveAt(i);
                    i--;
                }
                // if the mob is colliding with the player, reduce the player's health
                else if (mobWaves.Peek()[i].GetHurtBox().Intersects(player.GetHurtBox()))
                {
                    player.CurrentHealth -= mobWaves.Peek()[i].Damage;
                }
            }
        }
        
        // Add a player's projectile
        public void AddPlayerProjectiles(Projectile projectile)
        {
            playerProjectiles.Add(projectile);
        }

        // Add a mob's projectile
        public void AddMobProjectiles(Projectile projectile)
        {
            mobProjectiles.Add(projectile);
        }

        /// <summary> 
        /// Draw all player projectiles
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawPlayerProjeectitles(SpriteBatch spriteBatch)
        {
            // iterate through the player projectiles
            for (int i = 0; i < playerProjectiles.Count; i++)
            {
                // draw the projectiles
                playerProjectiles[i].Draw(spriteBatch);
            }
        }

        /// <summary>
        /// draw all the entities (players and mobs)
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void DrawEntities(SpriteBatch spriteBatch)
        {
            // draw the player's projectiles
            DrawPlayerProjeectitles(spriteBatch);

            // draw the mobs
            DrawMobs(spriteBatch);
        }

        ///  <summary>
        /// draw the front of the room
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawFront(SpriteBatch spriteBatch)
        {   
            // draw the front layers
            for (int i = 0; i < roomTemplatesManager.GetRoomTemplate(roomType).FrontLayers.Count; i++)
            {
                roomTemplatesManager.GetRoomTemplate(roomType).FrontLayers[i].Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Draw the back of the room
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawBack(SpriteBatch spriteBatch)
        {
            // draw the back layers
            for (int i = 0; i < roomTemplatesManager.GetRoomTemplate(roomType).BackLayers.Count; i++)
            {
                roomTemplatesManager.GetRoomTemplate(roomType).BackLayers[i].Draw(spriteBatch);
            }

            // draw the doors
            DrawDoors(spriteBatch);
        }

        /// <summary>
        /// draw the doors
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawDoors(SpriteBatch spriteBatch)
        {
            // iterate through the door layers, and draw each of them if they are not empty
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

        /// <summary>
        /// draw the wall hitboxes (DEBUG)
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawWallHitboxes(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < roomTemplatesManager.GetRoomTemplate(roomType).WallRecs.Count; i++)
            {
                spriteBatch.Draw(Assets.pixel, roomTemplatesManager.GetRoomTemplate(roomType).WallRecs[i], Color.Blue * 0.2f);
            }

            if (adjacentRooms.topRoom != (-1, -1)) spriteBatch.Draw(Assets.pixel, roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.top, Color.Pink * 0.9f);
            if (adjacentRooms.bottomRoom != (-1, -1)) spriteBatch.Draw(Assets.pixel, roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.bottom, Color.Pink * 0.9f);
            if (adjacentRooms.leftRoom != (-1, -1)) spriteBatch.Draw(Assets.pixel, roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.left, Color.Pink * 0.9f);
            if (adjacentRooms.rightRoom != (-1, -1)) spriteBatch.Draw(Assets.pixel, roomTemplatesManager.GetRoomTemplate(roomType).DoorRecs.right, Color.Pink * 0.9f);
        }


        /// <summary>
        /// draw the mobs of the room
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawMobs(SpriteBatch spriteBatch)
        {
            if (mobWaves.Size() == 0) return; // if there are no mobs, return

            // iterate through the mobs of the room and draw each
            for (int i = 0; i < mobWaves.Peek().Count; i++)
            {
                mobWaves.Peek()[i].Draw(spriteBatch, isDebug);
            }
        }
    }
}
