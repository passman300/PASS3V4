using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

        public Dictionary<int, TileTemplate> tileDict = new Dictionary<int, TileTemplate>();

        private Stack<string> tokenStack = new Stack<string>();
        private StreamReader reader;

        private int currentTileId;

        private int animationDurSum = 0;

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
                    imageSource = data.GetParameters()["source"];
                    int tempIndex = imageSource.IndexOf("Images");
                    imageSource = imageSource.Substring(tempIndex, imageSource.Length - tempIndex); 
                    width = int.Parse(data.GetParameters()["width"]);
                    height = int.Parse(data.GetParameters()["height"]);

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

            }
        }

        private void SetProperties(XMLData data)
        {
            string name = data.GetParameters()["name"];

            switch (name)
            {
                case "Collision":
                    tileDict[currentTileId].IsCollision = true;
                    break;
                case "Damage":
                    tileDict[currentTileId].Damage = int.Parse(data.GetParameters()["value"]);
                    break;
            }
        }

        private void SetAnimation(XMLData data)
        {
            tileDict[currentTileId].Frames.Add(int.Parse(data.GetParameters()["tileid"]));
            tileDict[currentTileId].AnimationDur = (int.Parse(data.GetParameters()["duration"]));
            tileDict[currentTileId].IsAnimated = true;

        }

        private void ReadHeader(XMLData data)
        {
            switch (data.GetToken())
            {
                case "tileset":
                    tileWidth = int.Parse(data.GetParameters()["tilewidth"]);
                    tileHeight = int.Parse(data.GetParameters()["tileheight"]);
                    tileCount = int.Parse(data.GetParameters()["tilecount"]);
                    col = int.Parse(data.GetParameters()["columns"]);

                    tokenStack.Push(data.GetToken());
                    break;

                case "tile":
                    currentTileId = int.Parse(data.GetParameters()["id"]);

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
            }
        }

        private void ReadFooter(XMLData data)
        {
            if (tokenStack.Top() == data.GetToken())
            {
                tokenStack.Pop();
            }
        }
    }
}
