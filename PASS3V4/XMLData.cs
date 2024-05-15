using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3V4
{
    public class XMLData
    {
        private string token;

        private string[] data;

        private Dictionary<string, string> parameters = new Dictionary<string, string>(); // attributes, value

        public bool isHeader { get; private set; }
        public bool isFooter { get; private set; }
        public bool isOneLine { get; private set; }

        public bool notXML { get; private set; }

        public XMLData(string line)
        {
            notXML = true;

            line = line.Trim();

            if (line[0] == '<' && line[line.Length - 2] == '/' && line[^1] == '>')
            {
                isOneLine = true;
                isHeader = false;
                isFooter = false;
                notXML = false;

                line = line.Substring(1, line.Length - 3);
            }
            else if (line[0] == '<' && line[1] == '/' && line[^1] == '>')
            {
                isFooter = true;
                isHeader = false;
                isOneLine = false;
                notXML = false;

                line = line.Substring(2, line.Length - 3);
            }

            else if (line[0] == '<' && line[^1] == '>')
            {
                isHeader = true;
                isFooter = false;
                isOneLine = false;
                notXML = false;

                line = line[1..^1];
            }

            FormatData(line);
        }

        public Dictionary<string, string> GetParameters() => parameters;

        public string GetParamterValue(string key)
        {
            return parameters[key];
        }

        public string GetToken() => token;

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
