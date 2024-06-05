using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace PASS3V4
{
    public class TileSetFileIO
    {
        private string name;
        private string filePath;
        private string imageSource;

        private int tileCount;
        private int col;

        private int tileWidth;
        private int tileHeight;

        private int width;
        private int height;

        public Dictionary<int, TileTemplate> tileDict = new();

        private Stack<string> tokenStack = new Stack<string>();
        private StreamReader reader;

        private int currentTileId;

        public TileSetFileIO(string filePath)
        {
            this.filePath = filePath;
            LoadTileSetFile();
        }


        public void LoadTileSetFile()
        {
            reader = new StreamReader(filePath);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                XMLData data = new XMLData(line);

                if (data.isOneLine) ReadBasicData(data);
                else if (data.isHeader) ReadHeader(data);
                else if (data.isFooter) ReadFooter(data);
            }

            reader.Close();
        }

        private void ReadBasicData(XMLData data)
        {
            switch (data.GetToken())
            {
                case "image":
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
                case "property":
                    if (tileDict[currentTileId] != null) tileDict[currentTileId] = new TileTemplate();
                    SetProperties(data);
                    break;
                case "frame":
                    SetAnimation(data);
                    break;
                case "object":
                    SetHitBox(data);
                    break;
            }
        }

        private void SetProperties(XMLData data)
        {
            string name = data.GetParamterValue("name");

            switch (name)
            {
                case "Collision":
                    tileDict[currentTileId].IsCollision = bool.Parse(data.GetParamterValue("value"));
                    break;
                case "Damage":
                    tileDict[currentTileId].Damage = int.Parse(data.GetParamterValue("value"));
                    break;
            }
        }

        private void SetAnimation(XMLData data)
        {
            tileDict[currentTileId].Frames.Add(int.Parse(data.GetParamterValue("tileid")));
            tileDict[currentTileId].AnimationDur = int.Parse(data.GetParamterValue("duration"));
            tileDict[currentTileId].IsAnimated = true;
        }

        private void SetHitBox(XMLData data)
        {
            int offsetX = (int)float.Parse(data.GetParamterValue("x"));
            int offsetY = (int)float.Parse(data.GetParamterValue("y"));
            int width = (int)(float.Parse(data.GetParamterValue("width")));
            int height = (int)(float.Parse(data.GetParamterValue("height")));

            tileDict[currentTileId].HitBoxes.Add(new Rectangle(offsetX, offsetY, width, height));
        }

        private void ReadHeader(XMLData data)
        {
            switch (data.GetToken())
            {
                case "tileset":
                    tileWidth = int.Parse(data.GetParamterValue("tilewidth"));
                    tileHeight = int.Parse(data.GetParamterValue("tileheight"));
                    tileCount = int.Parse(data.GetParamterValue("tilecount"));
                    col = int.Parse(data.GetParamterValue("columns"));

                    tokenStack.Push(data.GetToken());
                    break;

                case "tile":
                    currentTileId = int.Parse(data.GetParamterValue("id"));

                    tileDict[currentTileId] = new TileTemplate();

                    tokenStack.Push(data.GetToken());
                    break;

                case "properties":
                    tokenStack.Push(data.GetToken());
                    break;

                case "animation":
                    tokenStack.Push(data.GetToken());

                    if (tileDict[currentTileId] != null) tileDict[currentTileId] = new TileTemplate();
                    tileDict[currentTileId].Frames.Add(currentTileId);
                    break;
                case "objectgroup":
                    tokenStack.Push(data.GetToken());
                    break;
            }
        }

        private void ReadFooter(XMLData data)
        {
            if (tokenStack.Top() == data.GetToken()) tokenStack.Pop();
        }
    }
}
