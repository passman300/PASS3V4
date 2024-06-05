using Microsoft.Xna.Framework.Graphics;
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

        Dictionary<int, TileSetFileIO> tileSets = new();

        RoomTemplate roomTemplate = new();

        private string curObjectName;

        private int curLayerOrder = 0;

        private int curLayerId;
        private string curLayerName;
        private int width;
        private int height;

        Stack<XMLData> tokenStack = new();
        
        private StreamReader reader;

        public TileMapReader(string filePath) => this.filePath = filePath;

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
                            roomTemplate.WallRecs.Add(new Rectangle((int)float.Parse(data.GetParamterValue("x")),
                            (int)float.Parse(data.GetParamterValue("y")),
                            (int)float.Parse(data.GetParamterValue("width")),
                            (int)float.Parse(data.GetParamterValue("height"))));
                            break;
                        case "TopDoorRec":
                            roomTemplate.DoorRecs.top = new Rectangle((int)float.Parse(data.GetParamterValue("x")),(int)float.Parse(data.GetParamterValue("y")),(int)float.Parse(data.GetParamterValue("width")),(int)float.Parse(data.GetParamterValue("height")));
                            roomTemplate.SpawnPoints.top = new Vector2(roomTemplate.DoorRecs.top.X + roomTemplate.DoorRecs.top.Width / 2, roomTemplate.DoorRecs.top.Bottom + Room.SPAWN_OFFSET);
                            break;
                        case "BottomDoorRec":
                            roomTemplate.DoorRecs.bottom = new Rectangle((int)float.Parse(data.GetParamterValue("x")),(int)float.Parse(data.GetParamterValue("y")),(int)float.Parse(data.GetParamterValue("width")),(int)float.Parse(data.GetParamterValue("height")));
                            roomTemplate.SpawnPoints.bottom = new Vector2(roomTemplate.DoorRecs.bottom.X + roomTemplate.DoorRecs.bottom.Width / 2, roomTemplate.DoorRecs.bottom.Top - Room.SPAWN_OFFSET);
                            break;
                        case "LeftDoorRec":
                            roomTemplate.DoorRecs.left = new Rectangle((int)float.Parse(data.GetParamterValue("x")),(int)float.Parse(data.GetParamterValue("y")),(int)float.Parse(data.GetParamterValue("width")),(int)float.Parse(data.GetParamterValue("height")));
                            roomTemplate.SpawnPoints.left = new Vector2(roomTemplate.DoorRecs.left.Right + Room.SPAWN_OFFSET, roomTemplate.DoorRecs.left.Y + roomTemplate.DoorRecs.left.Height / 2);
                            break;
                        case "RightDoorRec":
                            roomTemplate.DoorRecs.right = new Rectangle((int)float.Parse(data.GetParamterValue("x")),(int)float.Parse(data.GetParamterValue("y")),(int)float.Parse(data.GetParamterValue("width")),(int)float.Parse(data.GetParamterValue("height")));
                            roomTemplate.SpawnPoints.right = new Vector2(roomTemplate.DoorRecs.right.Left - Room.SPAWN_OFFSET, roomTemplate.DoorRecs.right.Y + roomTemplate.DoorRecs.right.Height / 2);
                            break;
                        case "ExitRec":
                            roomTemplate.ExitRec = new Rectangle((int)float.Parse(data.GetParamterValue("x")), (int)float.Parse(data.GetParamterValue("y")), (int)float.Parse(data.GetParamterValue("width")), (int)float.Parse(data.GetParamterValue("height")));
                            break;
                    }
                    break;
                case "tileset":
                    tileSets[int.Parse(data.GetParamterValue("firstgid"))] = new TileSetFileIO("Tiled/" + data.GetParamterValue("source"));
                    break;
                case "property":
                    
                    // find what type of property it is
                    switch (data.GetParamterValue("name"))
                    {
                        case "MaxMobs":
                            roomTemplate.MaxMobs = int.Parse(data.GetParamterValue("value"));
                            break;
                        case "MinMobs":
                            roomTemplate.MinMobs = int.Parse(data.GetParamterValue("value"));
                            break;
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

                    if (data.GetParamterValue("name").Contains("Door")) 
                        roomTemplate.DoorLayers.Add(new TileLayer(curLayerName, curLayerOrder, width, height));
                    else roomTemplate.BackLayers.Add(new TileLayer(curLayerName, curLayerOrder, width, height));

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
            Vector3 pos = new();

            //List<Tile> tiles = new();

            int count = 0;

            while (!reader.EndOfStream && new XMLData(line).notXML)
            {
                line = line.TrimEnd(',');
                int[] tileIds = line.Split(',').Select(int.Parse).ToArray();

                for (int i = 0; i < tileIds.Length; i++)
                {
                    tileIds[i] = Math.Max(0, tileIds[i] - 1);

                    pos.X = i * Tile.WIdTH;

                    int tile = tileIds[i];
                    Dictionary<int, TileTemplate> tileData = new() { [tile] = tileSets[1].tileDict[tile] };


                    if (roomTemplate.DoorLayers.Count > 0 && tokenStack.Top(1).GetParamterValue("name").Contains("Door"))
                    {
                        if (tileData[tile].IsAnimated) roomTemplate.DoorLayers[^1].AddTile(new AnimatedTile(graphicsDevice, Assets.dungeonTileSetImg, tile, pos, tileData[tile]));
                        else
                            roomTemplate.DoorLayers[^1].AddTile(new Tile(graphicsDevice, Assets.dungeonTileSetImg, tile, pos, tileData[tile]));
                    }
                    else if (roomTemplate.FrontLayers.Count > 0 && (roomTemplate.FrontLayers[^1].Tiles[^1] == null))
                    {
                        if (tileData[tile].IsAnimated) roomTemplate.FrontLayers[^1].AddTile(new AnimatedTile(graphicsDevice, Assets.dungeonTileSetImg, tile, pos, tileData[tile]));
                        else roomTemplate.FrontLayers[^1].AddTile(new Tile(graphicsDevice, Assets.dungeonTileSetImg, tile, pos, tileData[tile]));
                    }
                    else
                    {
                        if (tileData[tile].IsAnimated) roomTemplate.BackLayers[^1].AddTile(new AnimatedTile(graphicsDevice, Assets.dungeonTileSetImg, tile, pos, tileData[tile])); 
                        else roomTemplate.BackLayers[^1].AddTile(new Tile(graphicsDevice, Assets.dungeonTileSetImg, tile, pos, tileData[tile]));
                    }


                    count++;
                    //tiles.Add(new Tile(graphicsDevice, Assets.dungeonTileSetImg, tile, pos, tileData[tile]));
                }

                pos.Y += Tile.HEIGHT;
                line = reader.ReadLine();
            }

            //if (frontLayers.Count > 0 && (frontLayers[^1].Tiles == null)) frontLayers[^1].LoadTiles(tiles.ToArray());
            //else backLayers[^1].LoadTiles(tiles.ToArray());

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

        public RoomTemplate GetRoomTemplate() => roomTemplate;

        public TileLayer[] GetFrontLayers() => roomTemplate.FrontLayers.ToArray<TileLayer>();

        public TileLayer[] GetBackLayers() => roomTemplate.BackLayers.ToArray<TileLayer>();

        public Rectangle[] GetWallRecs() => roomTemplate.WallRecs.ToArray<Rectangle>();

        public (Rectangle, Rectangle, Rectangle, Rectangle) GetDoorRecs() => roomTemplate.DoorRecs;
    }
}
