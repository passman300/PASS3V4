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
    public class RoomTemplate
    {
        public int MaxMobs { get; set; }
        public int MinMobs { get; set; }

        public List<TileLayer> FrontLayers { get; set; } = new();
        public List<TileLayer> BackLayers { get; set; } = new();

        public List<TileLayer> DoorLayers { get; set; } = new();

        public List<Rectangle> WallRecs { get; set; } = new();

        public (Rectangle top, Rectangle bottom, Rectangle left, Rectangle right) DoorRecs = (new Rectangle(), new Rectangle(), new Rectangle(), new Rectangle());
        public (Vector2 top, Vector2 bottom, Vector2 left, Vector2 right) SpawnPoints = (Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero);

        public Rectangle ExitRec { get; set; }

        public void Update(GameTime gameTime)
        {
            foreach (TileLayer layer in FrontLayers)
            {
                layer.Update(gameTime);
            }
            foreach (TileLayer layer in BackLayers)
            {
                layer.Update(gameTime);
            }
        }
    }
}
