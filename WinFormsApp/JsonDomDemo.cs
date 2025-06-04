using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Windows.Forms;

namespace WinFormsApp
{
    static public class JsonDomDemo
    {
        static JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = true, TypeInfoResolver = new DefaultJsonTypeInfoResolver() };
        static string DemoJsonText = """
                {
                  "Id": 1,
                  "Name": "Model 1",
                  "Status": "InProgress",
                  "Active": true,
                  "Parts": [
                      {
                        "Code": "001",
                        "Amount": 1.2,
                        "IsCompleted": true
                      },
                      {
                        "Code": "002",
                        "Amount": 3.4,
                        "IsCompleted": false
                      }
                  ],
                  "Properties": {
                    "John": "Doe",
                    "NiceCar": "Volvo"
                  },
                  "DT": "2025-06-04T00:59:25.6948527+03:00"
                }
                """;

        static void JsonObjectWithPrimitiveTypes(StringBuilder SB)
        {
            JsonObject JORoot = new JsonObject();
            JORoot.Add("String", JsonValue.Create("This is a JsonValue"));
            JORoot.Add("DateTime", JsonValue.Create(DateTime.Now));
            JORoot.Add("Integer", JsonValue.Create(123));

 
            SB.AppendLine(" ● Primitive Values using JsonValue.Create(ANY_PRIMITIVE_VALUE)");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(""""
                JsonObject JORoot = new JsonObject();
                JORoot.Add("String", JsonValue.Create("This is a JsonValue"));
                JORoot.Add("DateTime", JsonValue.Create(DateTime.Now));
                JORoot.Add("Integer", JsonValue.Create(123));
                """");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(JORoot.ToJsonString(JsonOptions));
            SB.AppendLine("------------------------------------------");
        }
        static void JsonObjectRemoveProperty(StringBuilder SB)
        {
            JsonObject JORoot = new JsonObject();
            JORoot.Add("String", JsonValue.Create("This is a JsonValue"));
            JORoot.Add("DateTime", JsonValue.Create(DateTime.Now));
            JORoot.Add("Integer", JsonValue.Create(123));

            JORoot.Remove("DateTime");

            SB.AppendLine(" ● Remove a Property");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(""""
                JsonObject JORoot = new JsonObject();
                JORoot.Add("String", JsonValue.Create("This is a JsonValue"));
                JORoot.Add("DateTime", JsonValue.Create(DateTime.Now));
                JORoot.Add("Integer", JsonValue.Create(123));

                JORoot.Remove("DateTime");
                """");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(JORoot.ToJsonString(JsonOptions));
            SB.AppendLine("------------------------------------------");
        }
        static void JsonObjectIsDictionaryLike(StringBuilder SB)
        {
            JsonObject JORoot = new JsonObject
            {
                ["Key1"] = "This is a string",
                ["Key2"] = DateTime.Now,
                ["Key3"] = false,
            };

            SB.AppendLine(" ● JsonObject Is a Dictionary-like object");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(""""
                JsonObject JORoot = new JsonObject
                {
                    ["Key1"] = "This is a string",
                    ["Key2"] = DateTime.Now,
                    ["Key3"] = false,
                };
                """");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(JORoot.ToJsonString(JsonOptions));
            SB.AppendLine("------------------------------------------");
        }
        static void JsonObjectWithParse(StringBuilder SB)
        {
            JsonObject JORoot = new JsonObject();
            JORoot.Add("Person", JsonNode.Parse("""{ "Name": "John Doe", "Age": 30 }"""));

 
            SB.AppendLine(" ● JsonNode.Parse()");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(""""
                JsonObject JORoot = new JsonObject();
                JORoot.Add("Person", JsonNode.Parse("""{ "Name": "John Doe", "Age": 30 }"""));
                """");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(JORoot.ToJsonString(JsonOptions));
            SB.AppendLine("------------------------------------------");
        }
        static void JsonObjectWithNonPrimitiveTypes(StringBuilder SB)
        {
            JsonObject JORoot = new JsonObject();
            JORoot.Add("Part", JsonValue.Create<Part>(new Part()));

            SB.AppendLine(" ● Using JsonValue.Create<T>() with non-primitive Types");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(""""
                JsonObject JORoot = new JsonObject();
                JORoot.Add("Part", JsonValue.Create<Part>(new Part()));
                """");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(JORoot.ToJsonString(JsonOptions));
            SB.AppendLine("------------------------------------------");
        }
        static void JsonObjectWithArray(StringBuilder SB)
        {
            JsonObject JORoot = new JsonObject();
            JsonArray JOArray = new JsonArray() { 123, true, DateTime.Now, "string value" };
            JORoot.Add("Array", JOArray);

            SB.AppendLine(" ● Array with JsonArray");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(""""
                JsonObject JORoot = new JsonObject();
                JsonArray JOArray = new JsonArray() { 123, true, DateTime.Now, "string value" };
                JORoot.Add("Array", JOArray);
                """");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(JORoot.ToJsonString(JsonOptions));
            SB.AppendLine("------------------------------------------");
        }
        static void JsonObjectWithDictionary(StringBuilder SB)
        {
            JsonObject JORoot = new JsonObject();
 
            var Dictionary = new Dictionary<string, JsonNode>
            {
                ["Key1"] = "This is a string",
                ["Key2"] = DateTime.Now,
                ["Key3"] = false,
                ["Key4"] = JsonValue.Create<Status>(Status.InProgress),
            };

            JsonObject DicNode = new JsonObject(Dictionary);
            JORoot.Add("Dictionary", DicNode);

            SB.AppendLine(" ● Using a Dictionary<string, JsonNode>");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(""""
                JsonObject JORoot = new JsonObject();

                var Dictionary = new Dictionary<string, JsonNode>
                {
                    ["Key1"] = "value1",
                    ["Key2"] = DateTime.Now,
                    ["Key3"] = false,
                    ["Key4"] = JsonValue.Create<Status>(Status.InProgress),
                };

                JsonObject DicNode = new JsonObject(Dictionary);
                JORoot.Add("Dictionary", DicNode);
                """");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(JORoot.ToJsonString(JsonOptions));
            SB.AppendLine("------------------------------------------");
        }
        static void JsonObjectPropertyAccess(StringBuilder SB)
        { 
            JsonNode RootNode = JsonNode.Parse(DemoJsonText);

            // accessing array
            JsonNode PartsNode = RootNode["Parts"];
            JsonNode FirstPartNode = PartsNode[0]; 

            // adding new property
            JsonObject ThirdPartNode = new JsonObject();
            ThirdPartNode["Code"] = "003";
            ThirdPartNode["Amount"] = 12.3;
            ThirdPartNode["IsCompleted"] = true;

            // get node as array
            JsonArray ArrayNode = PartsNode.AsArray();
            ArrayNode.Add(ThirdPartNode);

            // typecasting nodes
            JsonNode PropNode = ThirdPartNode["Amount"];
            double V = (double)PropNode;
            // 12.3

            string S = (string)ThirdPartNode["Code"];
            // 003

            // using the GetValue<T>()
            DateTime DateTimeNode = RootNode["DT"].GetValue<DateTime>();

            // getting the path
            string S2 = RootNode["Parts"][0].GetPath();
            // $.Parts[0]   where $ denotes the root node


            SB.AppendLine(" ● Accessing Properties");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine($""""
                {DemoJsonText}

                JsonNode RootNode = JsonNode.Parse(JsonText);

                // accessing array
                JsonNode PartsNode = RootNode["Parts"];
                JsonNode FirstPartNode = PartsNode[0]; 

                // adding new property
                JsonObject ThirdPartNode = new JsonObject();
                ThirdPartNode["Code"] = "003";
                ThirdPartNode["Amount"] = 12.3;
                ThirdPartNode["IsCompleted"] = true;

                // get node as array
                JsonArray ArrayNode = PartsNode.AsArray();
                ArrayNode.Add(ThirdPartNode);

                // typecasting nodes
                JsonNode PropNode = ThirdPartNode["Amount"];
                double V = (double)PropNode;
                // 12.3

                string S = (string)ThirdPartNode["Code"];
                // 003

                // using the GetValue<T>()
                DateTime DateTimeNode = RootNode["DT"].GetValue<DateTime>();

                string S2 = RootNode["Parts"][0].GetPath();
                // $.Parts[0]   where $ denotes the root node
                """");
            SB.AppendLine("------------------------------------------");
        }
        static public void JsonDocumentDemo(StringBuilder SB)
        {
 
            double Total = 0;

            using (JsonDocument Doc = JsonDocument.Parse(DemoJsonText))
            {
                JsonElement Root = Doc.RootElement;
                JsonElement PartsProperty = Root.GetProperty("Parts");
                foreach (JsonElement PartProperty in PartsProperty.EnumerateArray())
                {
                    if (PartProperty.TryGetProperty("Amount", out JsonElement AmountElement))
                    {
                        Total += AmountElement.GetDouble();
                    }
                }
            }

            SB.AppendLine(" ● JsonDocument");
            SB.AppendLine("------------------------------------------");
            SB.AppendLine(DemoJsonText);
            SB.AppendLine();
            SB.AppendLine("""
                double Total = 0;

                using (JsonDocument Doc = JsonDocument.Parse(DemoJsonText))
                {
                    JsonElement Root = Doc.RootElement;
                    JsonElement PartsProperty = Root.GetProperty("Parts");
                    foreach (JsonElement PartProperty in PartsProperty.EnumerateArray())
                    {
                        if (PartProperty.TryGetProperty("Amount", out JsonElement AmountElement))
                        {
                            Total += AmountElement.GetDouble();
                        }
                    }
                } 
                """);
            SB.AppendLine("------------------------------------------");


        }
        static public string JsonObjectDemo()
        {
            StringBuilder SB = new();

            JsonObjectWithPrimitiveTypes(SB);
            JsonObjectRemoveProperty(SB);
            JsonObjectIsDictionaryLike(SB);
            JsonObjectWithParse(SB);
            JsonObjectWithNonPrimitiveTypes(SB);
            JsonObjectWithArray(SB);
            JsonObjectWithDictionary(SB);
            JsonObjectPropertyAccess(SB);

            JsonDocumentDemo(SB);

            return SB.ToString();
        }



    }
}
