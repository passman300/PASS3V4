using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace PASS3V4
{
    public class TileLayer
    {
        public string Name { get; set; }

        public int LayerOrder { get; set; }

        public bool IsFront { get; set; }

        public Tile[] Tiles { get; set; }

        public TileLayer(string name, int layerOrder)
        {
            Name = name;
            LayerOrder = layerOrder;

            IsFront = false;
        }

        public TileLayer(string name, int layerOrder, bool isFront)
        {
            Name = name;
            LayerOrder = layerOrder;

            IsFront = isFront;
        }


        public void LoadTiles(Tile[] tiles)
        {
            Tiles = tiles;
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < Tiles.Length; i++)
            {
                Tiles[i].Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Tiles.Length; i++)
            {
                Tiles[i].Draw(spriteBatch);
            }
        }
    }
}
