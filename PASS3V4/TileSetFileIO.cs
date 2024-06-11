//Author: Colin Wang
//File Name: TileSetFileIO.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 6, 2024
//Modified Date: June 10, 2024
//Description: Reads the tile set file with a stream reader and stack

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace PASS3V4
{
    public class TileSetFileIO
    {
        private string name; // name of the tileset
        private string filePath; // path to the tileset
        private string imageSource; // path to the image source

        private int tileCount; // number of tiles
        private int col; // number of columns

        private int tileWidth; // width of each tile
        private int tileHeight; // height of each tile

        private int width; // width of the image
        private int height; // height of the image

        public Dictionary<int, TileTemplate> tileDict = new(); // dictionary of tiles to their id

        private Data_Structures.Stack<string> tokenStack = new(); // stack of tokens
        private StreamReader reader; // reader for the file

        private int currentTileId; // current tile id

        /// <summary>
        /// Constructor of the tile set file IO
        /// </summary>
        /// <param name="filePath"></param>
        public TileSetFileIO(string filePath)
        {
            this.filePath = filePath;
            LoadTileSetFile(); // load the tile set file
        }

        /// <summary>
        /// Load the tile set file
        /// </summary>
        public void LoadTileSetFile()
        {
            reader = new StreamReader(filePath);

            while (!reader.EndOfStream) // iterate through the file
            {
                string line = reader.ReadLine(); // read the line

                // create a new XMLData object from the line
                XMLData data = new XMLData(line);

                // read the XMLData object, as either a header, footer, or basic data
                if (data.IsOneLine) ReadBasicData(data);
                else if (data.IsHeader) ReadHeader(data);
                else if (data.IsFooter) ReadFooter(data);
            }

            reader.Close();
        }

        /// <summary>
        /// Reads basic data from the tile set file (one liner)
        /// </summary>
        /// <param name="data"></param>
        private void ReadBasicData(XMLData data)
        {
            switch (data.GetToken())
            {
                case "image": // read the image data
                    imageSource = data.GetParamterValue("source");
                    int tempIndex = imageSource.IndexOf("Images");
                    imageSource = imageSource.Substring(tempIndex, imageSource.Length - tempIndex);
                    width = int.Parse(data.GetParamterValue("width"));
                    height = int.Parse(data.GetParamterValue("height"));

                    for (int i = 0; i < tileCount; i++)
                    {
                        tileDict[i] = new TileTemplate();
                        tileDict[i].Image = Assets.dungeonTileSetImg;
                    }
                    break;
                case "property": // read the property data
                    if (tileDict[currentTileId] != null) tileDict[currentTileId] = new TileTemplate();
                    SetProperties(data);
                    break;
                case "frame": // read the frame data
                    SetAnimation(data);
                    break;
                case "object": // read the object data
                    SetHitBox(data);
                    break;
            }
        }

        /// <summary>
        /// sets the properties of a tile
        /// </summary>
        /// <param name="data"></param>
        private void SetProperties(XMLData data)
        {
            string name = data.GetParamterValue("name");

            switch (name)
            {
                case "Collision": // set the collision property
                    tileDict[currentTileId].IsCollision = bool.Parse(data.GetParamterValue("value"));
                    break;
                case "Damage": // set the damage property
                    tileDict[currentTileId].Damage = int.Parse(data.GetParamterValue("value"));
                    break;
            }
        }

        /// <summary>
        /// sets the animation of a tile
        /// </summary>
        /// <param name="data"></param>
        private void SetAnimation(XMLData data)
        {
            // add the frame to the frames list
            tileDict[currentTileId].Frames.Add(int.Parse(data.GetParamterValue("tileid")));
            tileDict[currentTileId].AnimationDur = int.Parse(data.GetParamterValue("duration"));
            tileDict[currentTileId].IsAnimated = true;
        }

        /// <summary>
        /// sets the hitbox of a tile
        /// </summary>
        /// <param name="data"></param>
        private void SetHitBox(XMLData data)
        {
            // store the x, y, width, and height of the hitbox
            int offsetX = (int)float.Parse(data.GetParamterValue("x"));
            int offsetY = (int)float.Parse(data.GetParamterValue("y"));
            int width = (int)(float.Parse(data.GetParamterValue("width")));
            int height = (int)(float.Parse(data.GetParamterValue("height")));

            // add hitbox to the hitboxes list
            tileDict[currentTileId].HitBoxes.Add(new Rectangle(offsetX, offsetY, width, height));
        }

        /// <summary>
        /// Reads the header of the tile set file
        /// </summary>
        /// <param name="data"></param>
        private void ReadHeader(XMLData data)
        {
            switch (data.GetToken())
            {
                case "tileset": // read the tileset data
                    tileWidth = int.Parse(data.GetParamterValue("tilewidth"));
                    tileHeight = int.Parse(data.GetParamterValue("tileheight"));
                    tileCount = int.Parse(data.GetParamterValue("tilecount"));
                    col = int.Parse(data.GetParamterValue("columns"));

                    tokenStack.Push(data.GetToken());
                    break;

                case "tile": // read the tile data
                    currentTileId = int.Parse(data.GetParamterValue("id"));

                    tileDict[currentTileId] = new TileTemplate();

                    tokenStack.Push(data.GetToken());
                    break;

                case "properties": // read the properties data
                    tokenStack.Push(data.GetToken());
                    break;

                case "animation": // read the animation data
                    tokenStack.Push(data.GetToken());

                    if (tileDict[currentTileId] != null) tileDict[currentTileId] = new TileTemplate();
                    tileDict[currentTileId].Frames.Add(currentTileId);
                    break;
                case "objectgroup": // read the objectgroup data
                    tokenStack.Push(data.GetToken());
                    break;
            }
        }

        /// <summary>
        /// reads the footer of the tile set file
        /// </summary>
        /// <param name="data"></param>
        private void ReadFooter(XMLData data)
        {
            if (tokenStack.Top() == data.GetToken()) tokenStack.Pop(); // pop the token if the top token matches
        }
    }
}
