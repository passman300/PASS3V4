using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PASS3V4
{
    public class TileMap
    {

        string filePath;

        Dictionary<int, TileSetFileIO> tileSets = new Dictionary<int, TileSetFileIO>();

        Dictionary<int, Tile[]> layer = new Dictionary<int, Tile[]>();

        Stack<string> tokenStack = new Stack<string>();

        private int width;
        private int height;

        private int currenLayer;

        private StreamReader reader;

        public TileMap(string filePath, GraphicsDevice graphicsDevice)
        {
            this.filePath = filePath;

            LoadTileMapFile(graphicsDevice);
        }

        private void LoadTileMapFile(GraphicsDevice graphicsDevice)
        {
            reader = new StreamReader(filePath);
            reader.ReadLine();

            string line;
            XMLData data;

            while (!reader.EndOfStream)
            {
                if (tokenStack.Count() > 0 && (tokenStack.Top() == "data"))
                {
                    LoadTiles(graphicsDevice);
                }
                else
                {
                    line = reader.ReadLine();

                    data = new XMLData(line);

                    if (data.isOneLine) ReadBasicData(data);
                    else if (data.isHeader) ReadHeader(data);
                    else if (data.isFooter) ReadFooter(data);
                }
            }

            reader.Close();
        }

        private void ReadBasicData(XMLData data)
        {
            switch (data.GetToken())
            {
                case "tileset":
                    tileSets[int.Parse(data.GetParameters()["firstgid"])] = new TileSetFileIO("Tiled/" + data.GetParameters()["source"]);
                    break;
            }
        }

        private void ReadHeader(XMLData data)
        {
            switch (data.GetToken())
            {
                case "layer":
                    currenLayer = int.Parse(data.GetParameters()["id"]);
                    width = int.Parse(data.GetParameters()["width"]);
                    height = int.Parse(data.GetParameters()["height"]);

                    tokenStack.Push(data.GetToken());
                    break;
                case "data":
                    tokenStack.Push(data.GetToken());
                    break;
            }
        }

        private void LoadTiles(GraphicsDevice graphicsDevice)
        {
            string line = reader.ReadLine();

            Vector3 pos = new Vector3();

            List<Tile> tiles = new List<Tile>();

            while (!reader.EndOfStream && new XMLData(line).notXML)
            {
                line = line.TrimEnd(',');
                int[] tileIDs = line.Split(',').Select(int.Parse).ToArray();

                for (int i = 0; i < tileIDs.Length; i++)
                {
                    tileIDs[i] = Math.Max(0, tileIDs[i] - 1);

                    pos.X = i * Tile.WIDTH;

                    int tile = tileIDs[i];
                    Dictionary<int, TileTemplate> tileData = new Dictionary<int, TileTemplate>();
                    tileData[tile] = tileSets[1].tileDict[tile];

                    tiles.Add(new Tile(graphicsDevice, Assets.dungeonTileSetImg, tile, pos, tileData[tile]));
                }

                pos.Y += Tile.HEIGHT;
                line = reader.ReadLine();
            }

            layer[currenLayer] = tiles.ToArray();

            tokenStack.Pop();
        }

        private void ReadFooter(XMLData data)
        {
            if (tokenStack.Count() > 0 && (tokenStack.Top() == data.GetToken()))
            {
                tokenStack.Pop();
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = layer.Count; i > 0; i--)
            {
                for (int j = 0; j < layer[i].Length; j++)
                {
                    layer[i][j].Update(gameTime);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = layer.Count; i > 0; i--)
            {
                for (int j = 0; j < layer[i].Length; j++)
                {
                    layer[i][j].Draw(spriteBatch, true);
                }
            }
        }
    }
}
