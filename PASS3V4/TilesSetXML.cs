using PASS3V4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace PASS3V4
{
    public class TileSetXML
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

        public Dictionary<int, TileTemplate> TileDict;

        private Stack<string> tokenStack;
        private string currentToken;
        private XmlReader reader;

        public TileSetXML(string filePath)
        {
            this.filePath = filePath;
        }

        /*        public void LoadTilesXML()
        {
            using (FileStream stream = System.IO.File.OpenRead(filePath))
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.ConformanceLevel = ConformanceLevel.Auto;

                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    while (reader.Read())
                    {
                        // check if there is a start element (token)
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "tileset":
                                    ReadBasicData();
                                    break;
                                case "image":
                                    ReadImageData();
                                    break;
                                case "tile":
                                    ReadTileData();
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void ReadBasicData()
        {
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "name":
                        if (!string.IsNullOrEmpty(reader.Value))
                            name = reader.Value;
                        break;
                    case "tilewidth":
                        if (!string.IsNullOrEmpty(reader.Value))
                            tileWidth = int.Parse(reader.Value);
                        break;
                    case "tileheight":
                        if (!string.IsNullOrEmpty(reader.Value))
                            tileHeight = int.Parse(reader.Value);
                        break;
                    case "tilecount":
                        if (!string.IsNullOrEmpty(reader.Value))
                            tileCount = int.Parse(reader.Value);
                        break;
                    case "columns":
                        if (!string.IsNullOrEmpty(reader.Value))
                            col = int.Parse(reader.Value);
                        break;
                }
            }
        }

        public void ReadImageData()
        {
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "source":
                        if (!string.IsNullOrEmpty(reader.Value))
                            imageSource = reader.Value;
                        break;
                    case "width":
                        if (!string.IsNullOrEmpty(reader.Value))
                            width = int.Parse(reader.Value);
                        break;
                    case "height":
                        if (!string.IsNullOrEmpty(reader.Value))
                            height = int.Parse(reader.Value);
                        break;
                }

            }
        }

        private void ReadTileData()
        {
            int id = int.Parse(reader.GetAttribute("id"));

            if (reader.IsEmptyElement) return;

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "properties":
                            ReadProperties();
                            break;
                        case "animation":
                            ReadAnimation();
                            break;
                    }
                }
            }
        }

        private void ReadProperties()
        {
            reader.MoveToContent();

            while (reader.Read())
            {

            }
        }

        private void LoadBasicData(string name, int tileWidth, int tileHeight, int tileCount, int col)
        {
            this.name = name;
            this.tileCount = tileCount;
            this.col = col;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
        }
        private void LoadImageData(string imageSource, int width, int height)
        {
            this.imageSource = imageSource;
            this.width = width;
            this.height = height;
        }



        */

    }
}
