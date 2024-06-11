//Author: Colin Wang
//File Name: XMLData.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 7, 2024
//Modified Date: June 10, 2024
//Description: the basic class for the xml data, and is accessible by the properties

using System;
using System.Collections.Generic;


namespace PASS3V4
{
    public class XMLData
    {
        private string token; // the first 'word' of a xml line

        private string[] data; // the rest of the line

        private Dictionary<string, string> parameters = new Dictionary<string, string>(); // attributes, value

        // booleans to determine if the line is a header, footer, or one line
        public bool IsHeader { get; private set; }
        public bool IsFooter { get; private set; }
        public bool IsOneLine { get; private set; }

        public bool NotXML { get; private set; }

        /// <summary>
        /// Creates a new XMLData object
        /// </summary>
        /// <param name="line"></param>
        public XMLData(string line)
        {
            NotXML = true;

            line = line.Trim();

            // check if the line is a header, footer, or one line
            if (line[0] == '<' && line[line.Length - 2] == '/' && line[^1] == '>')
            {
                IsOneLine = true;
                IsHeader = false;
                IsFooter = false;
                NotXML = false;

                line = line.Substring(1, line.Length - 3);
            }
            else if (line[0] == '<' && line[1] == '/' && line[^1] == '>')
            {
                IsFooter = true;
                IsHeader = false;
                IsOneLine = false;
                NotXML = false;

                line = line.Substring(2, line.Length - 3);
            }

            else if (line[0] == '<' && line[^1] == '>')
            {
                IsHeader = true;
                IsFooter = false;
                IsOneLine = false;
                NotXML = false;

                line = line[1..^1];
            }

            FormatData(line);
        }

        /// <summary>
        /// Returns a dictionary of parameters
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters() => parameters;

        /// <summary>
        /// Returns a parameter value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetParamterValue(string key)
        {
            return parameters[key];
        }

        /// <summary>
        /// Returns the token
        /// </summary>
        /// <returns></returns>
        public string GetToken() => token;

        /// <summary>
        /// Formats the data
        /// </summary>
        /// <param name="line"></param>
        private void FormatData(string line)
        {
            data = line.Split(' ');

            token = data[0];

            for (int i = 1; i < data.Length; i++)
            {
                parameters[SplitParameter(data[i]).Item1] = SplitParameter(data[i]).Item2;
            }
        }

        private static Tuple<string, string> SplitParameter(string parameter)
        {

            string id = parameter.Split('=')[0];
            id = id.Trim('"');

            string value = parameter.Split('=')[1];
            value = value.Trim('"');
            return Tuple.Create(id, value);
        }
    }   
}
