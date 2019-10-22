using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ChoETL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using YamlDotNet.Serialization;


namespace NetworkApp
{
    class Functions
    {
        Queue<string> listOfRoutes = new Queue<string> ( );
        List<JObject> jsonData = new List<JObject> ( );
        int counter = 2;

        public static JObject GetData ( JObject rawObject )
        {
            var mime_type = rawObject["mime_type"];
            var data = rawObject["data"];
            JObject returnData = null;

            if (mime_type == null)
            {
                JArray jsonArray = JArray.Parse ( data.ToString ( ) );
                JObject json = JObject.Parse ( jsonArray[0].ToString ( ) );
                return json;
            }
            else
            if (mime_type.ToString ( ) == "application/xml")
            {
                return returnData = GetXmlToJson ( data.ToString ( ) );
            }
            else
            if (mime_type.ToString ( ) == "text/csv")
            {
                return returnData = GetCsvToJson ( data.ToString ( ) );
            }
            else
            if (mime_type.ToString ( ) == "application/x-yaml")
            {
                return returnData = GetYamlToJson ( data.ToString ( ) );
            }
            return null;
        }

        public static JObject GetXmlToJson ( string location )
        {
            XmlDocument doc = new XmlDocument ( );
            doc.LoadXml ( location );
            string jsonText = JsonConvert.SerializeXmlNode ( doc );
            JObject json = JObject.Parse ( jsonText ); 
            return json;
        }

        public static JObject GetYamlToJson ( string location )
        {
            var r = new StringReader ( location );
            var deserializer = new Deserializer ( );
            var yamlObject = deserializer.Deserialize ( r );
            var serializer = new Newtonsoft.Json.JsonSerializer ( );
            var w = new StringWriter ( );
            serializer.Serialize ( w, yamlObject );
            string jsonText = w.ToString ( );
            JArray jsonArray = JArray.Parse ( jsonText );
            JObject json = JObject.Parse ( jsonArray[0].ToString ( ) );
            return json;
        }

        public static JObject GetCsvToJson ( string location )
        {
            StringBuilder sb = new StringBuilder ( );
            using (var p = ChoCSVReader.LoadText ( location )
                .WithFirstLineHeader ( )
                )
            {
                using (var w = new ChoJSONWriter ( sb ))
                    w.Write ( p );
            }
            JArray jsonArray = JArray.Parse ( sb.ToString ( ) );
            JObject json = JObject.Parse ( jsonArray[1].ToString ( ) );
            return json;
        }
    }
}
