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
    public class Room : TileMap
    {
        public struct AdjacentRooms
        {
            public (int x, int y) topRoom;
            public (int x, int y) bottomRoom;
            public (int x, int y) leftRoom;
            public (int x, int y) rightRoom;

            public AdjacentRooms((int x, int y) topRoom, (int x, int y) bottomRoom, (int x, int y) leftRoom, (int x, int y) rightRoom)
            {
                this.topRoom = topRoom;
                this.bottomRoom = bottomRoom;
                this.leftRoom = leftRoom;
                this.rightRoom = rightRoom;
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
            BasicRoom,
            BossRoom
        }

        public AdjacentRooms adjacentRooms = new AdjacentRooms();

        public IsDoorCollision isDoorCollision { get; set; }

        private int mobSpawns;

        private Vector2 spawnPosition;

        private List<Arrow> flyingArrows = new();

        public Room() : base()
        {
            // TDOD: Load room from file
            
            /*
             * create a file system based on the types of rooms
             *      each floder contains a tsx and a txt of room stats
             *      
             * create the room stats of mobs, waves, if 
             */
        }

        public override void Update(GameTime gameTime, Player player, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse)
        {
            for (int i = 0; i < BackLayers.Length; i++)
            {
                BackLayers[i].Update(gameTime);
            }
            for (int i = 0; i < FrontLayers.Length; i++)
            {
                FrontLayers[i].Update(gameTime);
            }

            player.Update(gameTime, kb, prevKb, mouse, prevMouse, WallRecs);

            if (player.GetFlyingArrow().IsShot) AddFlyingArrow(player.GetFlyingArrow().Arrow);

            UpdateArrows();

            
        }

        private void UpdateArrows()
        {
            for (int i = 0; i < flyingArrows.Count; i++)
            {
                flyingArrows[i].UpdateFlying();

                if (flyingArrows[i].GetHitBox().Left > Game1.SCREEN_WIDTH || flyingArrows[i].GetHitBox().Right < 0 || 
                    flyingArrows[i].GetHitBox().Top > Game1.SCREEN_HEIGHT || flyingArrows[i].GetHitBox().Bottom < 0)
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

        public override void DrawEntities(SpriteBatch spriteBatch)
        {
            DrawFlyingArrows(spriteBatch);
        }
    }
}
