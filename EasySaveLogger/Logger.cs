using System.Text.Json;
using System.Xml.Serialization;

namespace EasySaveLogger
{
    public static class Logger
    {
        // Define a delegate for serialization
        internal delegate string SerializeDelegate(List<Dictionary<string, object>> obj);
        // Define a delegate for deserialization
        internal delegate List<Dictionary<string, object>> DeserializeDelegate(string obj);

        // JSON Serialization method
        internal static string jsonSerializer(List<Dictionary<string, object>> obj)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(obj, options);
        }

        // JSON Deserialization method
        internal static List<Dictionary<string, object>> jsonDeserializer(string obj)
        {
            return JsonSerializer.Deserialize<List<Dictionary<string, object>>>(obj);
        }

        // Class to serialize the dictionary into a list of key-value pairs
        internal class XmlItem
        {
            public string key { get; set; }
            public string value { get; set; }
        }

        // XML Serialization method
        internal static string xmlSerializer(List<Dictionary<string, object>> obj)
        {
            var serializableList = obj.Select(d =>
                d.Select(
                    kv => new XmlItem { key = kv.Key, value = kv.Value?.ToString() }
                ).ToList()
            ).ToList();

            var serializer = new XmlSerializer(typeof(List<List<XmlItem>>));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, serializableList);
                return writer.ToString();
            }
        }

        // XML Deserialization method
        internal static List<Dictionary<string, object>> xmlDeserializer(string obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (StringReader reader = new StringReader(obj))
            {
                return (List<Dictionary<string, object>>)serializer.Deserialize(reader);
            }
        }

        // Function to use the delegate and serialize
        internal static string serialize(List<Dictionary<string, object>> obj, SerializeDelegate serializeMethod)
        {
            return serializeMethod(obj);
        }

        // Function to use the delegate and deserialize
        internal static List<Dictionary<string, object>> deserialize(string obj, DeserializeDelegate deserializeMethod)
        {
            return deserializeMethod(obj);
        }

        public static void Log(Dictionary<string, object> entryObject, string filePath = "defaultLog.json")
        {
            string typeFile = filePath.Split('.').LastOrDefault();
            SerializeDelegate serializer;
            DeserializeDelegate deserializer;

            switch (typeFile)
            {
                case "json":
                    serializer = jsonSerializer;
                    deserializer = jsonDeserializer;
                    break;
                case "xml":
                    serializer = xmlSerializer;
                    deserializer = xmlDeserializer;
                    break;
                default:
                    serializer = jsonSerializer;
                    deserializer = jsonDeserializer;
                    break;
            }

            string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs/", filePath);

            Directory.CreateDirectory(Path.GetDirectoryName(pathFile));

            List<Dictionary<string, object>> logs;

            try
            {
                //read the entire file 
                string existingFile = File.ReadAllText(pathFile);
                // Deserialize JSON into a list of dictionaries or initialize an empty list if null
                logs = deserialize(existingFile, deserializer);

            }
            catch
            {
                logs = new List<Dictionary<string, object>>();
            }

            if (entryObject != null)
            {
                logs.Add(entryObject);
            }

            // Serialize 'logs' to JSON and write it to a file with a new line at the end
            File.WriteAllText(pathFile, serializer(logs) + Environment.NewLine);
        }
    }
}