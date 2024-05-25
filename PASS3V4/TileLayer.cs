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

        public Tile[] Tiles { get; private set; }

        public TileLayer(string name, int layerOrder, int width, int height)
        {
            Name = name;
            LayerOrder = layerOrder;

            Tiles = new Tile[width * height];

            IsFront = false;
        }

        public TileLayer(string name, int layerOrder, bool isFront)
        {
            Name = name;
            LayerOrder = layerOrder;

            IsFront = isFront;
        }

        public void LoadTile(Tile tile, int index)
        {
            Tiles[index] = tile;
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
