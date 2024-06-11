//Author: Colin Wang
//File Name: TileMapReader.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 8, 2024
//Modified Date: June 10, 2024
//Description: Reads the tile map file

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
        string filePath; // the path to the tile map file

        Dictionary<int, TileSetFileIO> tileSets = new(); // the tile sets in the tile map

        RoomTemplate roomTemplate = new(); // the room template in the tile map

        private string curObjectName; // the name of the current object

        private int curLayerOrder = 0; // the order of the current layer
        
        private int curLayerId; // the id of the current layer
        private string curLayerName; // the name of the current layer

        // dimensions of the tile map
        private int width;
        private int height;

        // stack and file io for the tile map reader
        Data_Structures.Stack<XMLData> tokenStack = new();
        private StreamReader reader;

        ///  <summary>
        /// constructor for the tile map reader
        /// </summary>
        /// <param name="filePath"></param>
        public TileMapReader(string filePath) => this.filePath = filePath; // set the file path

        ///  <summary>
        /// Load the tile map file
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public void LoadTileMapFile(GraphicsDevice graphicsDevice)
        {   
            reader = new StreamReader(filePath);
            reader.ReadLine(); // read the first line

            string line;
            XMLData data; // xml data  is accessable by it's parameter value

            // read the rest of the file
            while (!reader.EndOfStream)
            {   
                // load the tiles if the first token is "data"
                if (tokenStack.Count() > 0 && (tokenStack.Top().GetToken() == "data"))
                {
                    LoadTiles(graphicsDevice);
                }
                else // the line is not "data", and is possibly a header or footer
                { 
                    line = reader.ReadLine();

                    data = new XMLData(line);

                    if (data.IsOneLine) ReadBasicData(data);
                    else if (data.IsHeader) ReadHeader(data);
                    else if (data.IsFooter) ReadFooter(data);
                }
            }

            reader.Close();
        }

        /// <summary>
        /// read basic data (one liners)
        /// </summary>
        /// <param name="data"></param>
        private void ReadBasicData(XMLData data)
        {
            // get the token value
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
                        case "SpawnArea":
                            roomTemplate.SpawnArea = new Rectangle((int)float.Parse(data.GetParamterValue("x")), (int)float.Parse(data.GetParamterValue("y")), (int)float.Parse(data.GetParamterValue("width")), (int)float.Parse(data.GetParamterValue("height")));
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

        /// <summary>
        /// read the header of the XML
        /// </summary>
        /// <param name="data"></param>
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

        /// <summary>
        /// load the tiles in the tile layer
        /// </summary>
        /// <param name="graphicsDevice"></param>
        private void LoadTiles(GraphicsDevice graphicsDevice)
        {
            string line = reader.ReadLine();
            Vector3 pos = new();

            int count = 0;

            while (!reader.EndOfStream && new XMLData(line).NotXML)
            {
                line = line.TrimEnd(',');
                int[] tileIds = line.Split(',').Select(int.Parse).ToArray();

                for (int i = 0; i < tileIds.Length; i++)
                {
                    tileIds[i] = Math.Max(0, tileIds[i] - 1); // the tile ids start at 1

                    pos.X = i * Tile.WIDTH; // the position of the tile is based on the position in the file

                    // get the tile data
                    int tile = tileIds[i];
                    Dictionary<int, TileTemplate> tileData = new() { [tile] = tileSets[1].tileDict[tile] };

                    // add the tiles to the room
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
                }

                // update the position of the tile in the y axis, as you move to the next line
                pos.Y += Tile.HEIGHT;
                line = reader.ReadLine();
            }

            tokenStack.Pop();
        }

        /// <summary>
        /// read the footer of the XML
        /// </summary>
        /// <param name="data"></param>
        private void ReadFooter(XMLData data)
        {   
            // check if the token is the same as the token in the top of the stack
            if (tokenStack.Count() > 0 && (tokenStack.Top().GetToken() == data.GetToken()))
            {
                if (data.GetToken() == "layer") curLayerOrder++; // increment the layer order

                tokenStack.Pop(); // pop the token
            }
        }


        /// <summary>
        /// get the room template
        /// </summary>
        /// <returns></returns>
        public RoomTemplate GetRoomTemplate() => roomTemplate;

        /// <summary>
        /// get the front layers
        /// </summary>
        /// <returns></returns>
        public TileLayer[] GetFrontLayers() => roomTemplate.FrontLayers.ToArray<TileLayer>();

        /// <summary>
        /// get the back layers
        /// </summary>
        /// <returns></returns>
        public TileLayer[] GetBackLayers() => roomTemplate.BackLayers.ToArray<TileLayer>();
    }
}
