using System.Text.Json;
using System.Xml.Serialization;

namespace EasySaveLogger
{
    // Class to serialize the dictionary into a list of key-value pairs
    public class XmlItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

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

        // XML Serialization method
        internal static string xmlSerializer(List<Dictionary<string, object>> obj)
        {
            var serializableList = obj.Select(d =>
                d.Select(
                    kv => new XmlItem { Key = kv.Key, Value = kv.Value?.ToString() }
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
            XmlSerializer serializer = new XmlSerializer(typeof(List<List<XmlItem>>));
            List<List<XmlItem>> xmlArrayofArrayofXmlItems;
            using (StringReader reader = new StringReader(obj))
            {
                xmlArrayofArrayofXmlItems = (List<List<XmlItem>>)serializer.Deserialize(reader);
            }
            List<Dictionary<string, object>> deserializedListofDict = new List<Dictionary<string, object>>();
            foreach (var xmlArrayofXmlItems in xmlArrayofArrayofXmlItems)
            {
                Dictionary<string, object> deserializedDict = new Dictionary<string, object>();
                foreach (var xmlItem in xmlArrayofXmlItems)
                {
                    deserializedDict.Add(xmlItem.Key, xmlItem.Value);
                }
                deserializedListofDict.Add(deserializedDict);
            }
            return deserializedListofDict;
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

        public static List<Dictionary<string, object>> ReadLog(string filePath = "defaultLog.json")
        {

            string typeFile = filePath.Split('.').LastOrDefault();
            DeserializeDelegate deserializer;
            List<Dictionary<string, object>> logs;

            switch (typeFile)
            {
                case "json":
                    deserializer = jsonDeserializer;
                    break;
                case "xml":
                    deserializer = xmlDeserializer;
                    break;
                default:
                    deserializer = jsonDeserializer;
                    break;
            }

            try
            {
                string existingFile = File.ReadAllText(filePath);
                logs = deserialize(existingFile, deserializer);
            }

            catch (Exception ex)
            {
                logs = new List<Dictionary<string, object>> { new Dictionary<string, object> { { "timestamp", "Erreur" }, { "saveName", ex.Message } } };
            }
            return logs;
        }
    }
}