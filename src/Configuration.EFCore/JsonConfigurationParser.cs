using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Configuration.EFCore
{
    internal class JsonConfigurationParser
    {

        private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _path = new Stack<string>();
        private string _currentPath;
        private JsonTextReader _jsonTextReader;

        private JsonConfigurationParser() { }

        public static IDictionary<string, string> Parse(string json)
        {
            return new JsonConfigurationParser().ParseJsonTree(json);
        }

        private IDictionary<string, string> ParseJsonTree(string json)
        {
            _data.Clear();

            _jsonTextReader = new JsonTextReader(new StringReader(json));
            _jsonTextReader.DateParseHandling = DateParseHandling.None;

            JObject jsonConfig = JObject.Load(_jsonTextReader);

            VisitInternal(jsonConfig);

            return _data;
        }

        private void VisitInternal(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    VisitInternal(token.Value<JObject>());
                    break;
                case JTokenType.Array:
                    VisitInternal(token.Value<JArray>());
                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Bytes:
                case JTokenType.Raw:
                case JTokenType.Null:
                    VisitLeaf(token.Value<JValue>());
                    break;
                default:
                    throw new FormatException($"Can not parse json of {_jsonTextReader.TokenType} from {_jsonTextReader.Path} at {_jsonTextReader.LineNumber} line and {_jsonTextReader.LinePosition}.");
            }
        }

        private void VisitInternal(JArray array)
        {
            for (int index = 0; index < array.Count; index++)
            {
                GoDescendant(index.ToString());
                VisitInternal(array[index]);
                GoAncestor();
            }
        }

        private void VisitInternal(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                GoDescendant(property.Name);
                VisitInternal(property.Value);
                GoAncestor();
            }
        }


        private void VisitLeaf(JValue data)
        {
            var key = _currentPath;

            if (_data.ContainsKey(key))
            {
                throw new FormatException($"Can not parse json. Duplicate key: {key}");
            }
            _data[key] = data.ToString(CultureInfo.InvariantCulture);
        }

        private void GoDescendant(string context)
        {
            _path.Push(context);
            _currentPath = ConfigurationPath.Combine(_path.Reverse());
        }

        private void GoAncestor()
        {
            _path.Pop();
            _currentPath = ConfigurationPath.Combine(_path.Reverse());
        }
    }
}
