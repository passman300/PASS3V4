﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    namespace PASS3V4
{
    public class TileMapReader
    {
        string filePath;

        Dictionary<int, TileSetFileIO> tileSets = new Dictionary<int, TileSetFileIO>();

        List<TileLayer> frontLayers = new List<TileLayer>();
        List<TileLayer> backLayers = new List<TileLayer>();
        List<Rectangle> wallRecs = new List<Rectangle>();

        private string curObjectName;

        private int curLayerOrder = 0;

        private int curLayerId;
        private string curLayerName;
        private int width;
        private int height;

        Stack<XMLData> tokenStack = new Stack<XMLData>();
        
        private StreamReader reader;

        public TileMapReader(string filePath)
        {
            this.filePath = filePath;
        }

        public void LoadTileMapFile(GraphicsDevice graphicsDevice)
        {
            reader = new StreamReader(filePath);
            reader.ReadLine();

            string line;
            XMLData data;

            while (!reader.EndOfStream)
            {
                if (tokenStack.Count() > 0 && (tokenStack.Top().GetToken() == "data"))
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
                case "object":
                    switch (tokenStack.Top().GetParamterValue("name"))
                    {
                        case "WallRecs":
                            wallRecs.Add(new Rectangle((int)float.Parse(data.GetParamterValue("x")),
                            (int)float.Parse(data.GetParamterValue("y")),
                            (int)float.Parse(data.GetParamterValue("width")),
                            (int)float.Parse(data.GetParamterValue("height"))));
                            break;
                    }
                    break;

                case "tileset":
                    tileSets[int.Parse(data.GetParamterValue("firstgid"))] = new TileSetFileIO("Tiled/" + data.GetParamterValue("source"));
                    break;
                case "property":
                    backLayers[^1].IsFront = bool.Parse(data.GetParamterValue("value"));

                    if (backLayers[^1].IsFront)
                    {
                        frontLayers.Add(backLayers[^1]);
                        backLayers.RemoveAt(backLayers.Count - 1);
                    }
                    break;
            }
        }

        private void ReadHeader(XMLData data)
        {
            switch (data.GetToken())
            {
                case "objectgroup":
                    curObjectName = data.GetParamterValue("name"); 

                    tokenStack.Push(data);
                    break;

                case "layer":
                    curLayerId = int.Parse(data.GetParamterValue("id"));
                    curLayerName = data.GetParamterValue("name");
                    width = int.Parse(data.GetParamterValue("width"));
                    height = int.Parse(data.GetParamterValue("height"));

                    backLayers.Add(new TileLayer(curLayerName, curLayerOrder));

                    tokenStack.Push(data);
                    break;
                case "data":
                    tokenStack.Push(data);
                    break;
                case "properties":
                    tokenStack.Push(data);
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

            if (frontLayers.Count > 0 && (frontLayers[^1].Tiles == null)) frontLayers[frontLayers.Count - 1].LoadTiles(tiles.ToArray());
            else backLayers[^1].LoadTiles(tiles.ToArray());

            tokenStack.Pop();
        }

        private void ReadFooter(XMLData data)
        {
            if (tokenStack.Count() > 0 && (tokenStack.Top().GetToken() == data.GetToken()))
            {
                if (data.GetToken() == "layer") curLayerOrder++;

                tokenStack.Pop();
            }
        }

        public TileLayer[] GetFrontLayers()
        {
            return frontLayers.ToArray<TileLayer>();
        }

        public TileLayer[] GetBackLayers()
        {
            return backLayers.ToArray<TileLayer>();
        }

        public Rectangle[] GetWallRecs()
        {
            return wallRecs.ToArray<Rectangle>();
        }
    }
}