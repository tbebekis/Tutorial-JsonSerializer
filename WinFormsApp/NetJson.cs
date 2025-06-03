namespace WinFormsApp
{

    using System.Buffers;
    using System.Net.Http.Json;
    using System.Reflection;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Text.Json.Serialization;
    using System.Text.Json.Serialization.Metadata;

    /// <summary>
    /// Helper json static class
    /// <para>SEE: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-overview </para>
    /// </summary>
    static public class NetJson
    {
 
        static JsonSerializerOptions fSerializerOptions;
        static JsonSerializerOptions fDefaultSerializerOptions;

        /// <summary>
        /// Adds unknown elements to a property that has the JsonExtensionData attribute. This is not
        /// done for us automagically since we hare using a custom converter
        /// <para><strong>NOTE: </strong> copied from https://github.com/microsoftgraph/msgraph-sdk-dotnet-core/blob/57861dc4aea6c33908838915c97fc02105b6e788/src/Microsoft.Graph.Core/Serialization/DerivedTypeConverter.cs</para>
        /// </summary>
        /// <param name="target">The target object</param>
        /// <param name="objectType">The object type</param>
        /// <param name="property">The json property</param>
        static void AddToAdditionalDataBag(object target, Type objectType, JsonProperty property)
        {
            // Get the property with the JsonExtensionData attribute and add the property to the collection
            var additionalDataInfo = objectType.GetProperties().FirstOrDefault(propertyInfo => ((MemberInfo)propertyInfo).GetCustomAttribute<JsonExtensionDataAttribute>() != null);
            if (additionalDataInfo != null)
            {
                var additionalData = additionalDataInfo.GetValue(target) as IDictionary<string, object> ?? new Dictionary<string, object>();
                additionalData.Add(property.Name, property.Value);
                additionalDataInfo.SetValue(target, additionalData);
            }
        }
        /// <summary>
        /// Populate an existing object with properties rather than create a new object. This custom implementation will be obsolete once
        /// System.Text.Json add support for this.
        /// Note : As this is a converter for derived type the expected inputs are either object or collection not value types.
        /// <para><strong>NOTE: </strong> copied from https://github.com/microsoftgraph/msgraph-sdk-dotnet-core/blob/57861dc4aea6c33908838915c97fc02105b6e788/src/Microsoft.Graph.Core/Serialization/DerivedTypeConverter.cs</para>
        /// </summary>
        /// <param name="target">The target object</param>
        /// <param name="json">The json element undergoing deserialization</param>
        /// <param name="options">The options to use for deserialization.</param>
        static void PopulateObjectInternal(object target, JsonElement json, JsonSerializerOptions options)
        {
            // We use the target type information since it maybe be derived. We do not want to leave out extra properties in the child class and put them in the additional data unnecessarily
            Type objectType = target.GetType();
            switch (json.ValueKind)
            {
                case JsonValueKind.Object:
                    {
                        // iterate through the object properties
                        foreach (var property in json.EnumerateObject())
                        {
                            // look up the property in the object definition using the mapping provided in the model attribute
                            var propertyInfo = objectType.GetProperties().FirstOrDefault((mappedProperty) =>
                            {
                                var attribute = mappedProperty.GetCustomAttribute<JsonPropertyNameAttribute>();
                                return attribute?.Name == property.Name;
                            });
                            if (propertyInfo == null)
                            {
                                //Add the property to AdditionalData as it doesn't exist as a member of the object
                                AddToAdditionalDataBag(target, objectType, property);
                                continue;
                            }

                            try
                            {
                                // Deserialize the property in and update the current object.
                                var parsedValue = JsonSerializer.Deserialize(property.Value.GetRawText(), propertyInfo.PropertyType, options);
                                propertyInfo.SetValue(target, parsedValue);
                            }
                            catch (JsonException)
                            {
                                //Add the property to AdditionalData as it can't be deserialized as a member. Eg. non existing enum member type
                                AddToAdditionalDataBag(target, objectType, property);
                            }
                        }

                        break;
                    }
                case JsonValueKind.Array:
                    {
                        //Its most likely a collectionPage instance so get its CurrentPage property
                        var collectionPropertyInfo = objectType.GetProperty("CurrentPage", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                        if (collectionPropertyInfo != null)
                        {
                            // Get the generic type info for deserialization
                            Type genericType = collectionPropertyInfo.PropertyType.GenericTypeArguments.FirstOrDefault();
                            int index = 0;
                            foreach (var property in json.EnumerateArray())
                            {
                                // Get the object instance
                                var instance = JsonSerializer.Deserialize(property.GetRawText(), genericType, options);

                                // Invoke the insert function to add it to the collection as it an IList
                                MethodInfo methodInfo = collectionPropertyInfo.PropertyType.GetMethods().FirstOrDefault(method => method.Name.Equals("Insert"));
                                object[] parameters = new object[] { index, instance };
                                if (methodInfo != null)
                                {
                                    methodInfo.Invoke(target, parameters);//insert the object to the page List
                                    index++;
                                }
                            }
                        }

                        break;
                    }
            }
        }

        // ● serialization options
        /// <summary>
        /// Creates and returns json serialization options
        /// </summary>
        static public JsonSerializerOptions CreateOptions(bool CameCase = false
            ,bool Formatted = true
            ,bool CaseInsensitiveProperties = true
            ,string[] ExcludeProperties = null)
        {
            if (CreateOptionsFunc != null)
                return CreateOptionsFunc(CameCase, Formatted, CaseInsensitiveProperties, ExcludeProperties);

            JsonSerializerOptions Result = new();

            Result.PropertyNamingPolicy = CameCase ? JsonNamingPolicy.CamelCase : new JsonNamingPolicyAsIs();
            Result.DictionaryKeyPolicy = Result.PropertyNamingPolicy;
            Result.PropertyNameCaseInsensitive = CaseInsensitiveProperties;
            Result.WriteIndented = Formatted;
            Result.IgnoreReadOnlyProperties = true;
            Result.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // JsonIgnoreCondition.Always;
            Result.ReadCommentHandling = JsonCommentHandling.Skip;
            Result.AllowTrailingCommas = true;
            Result.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            Result.ReferenceHandler = ReferenceHandler.Preserve; // or ReferenceHandler.IgnoreCycles
            Result.Converters.Insert(0, new JsonStringEnumConverter(Result.PropertyNamingPolicy));
            if (ExcludeProperties != null)
                Result.TypeInfoResolver = new ExcludePropertiesTypeInfoResolver(ExcludeProperties);

            return Result;
        }

        // ● serialize
        /// <summary>
        /// Serializes a specified instance.
        /// </summary>
        static public string Serialize(object Instance, JsonSerializerOptions JsonOptions)
        {
            JsonOptions = JsonOptions ?? SerializerOptions;
            string JsonText = JsonSerializer.Serialize(Instance, JsonOptions);
            return JsonText;
        }
        /// <summary>
        /// Serializes a specified instance.
        /// </summary>
        static public string Serialize(object Instance, bool Formatted, bool CameCase = false)
        {
            JsonSerializerOptions Options = CreateOptions(CameCase: CameCase, Formatted: Formatted);
            return Serialize(Instance, Options);
        }
        /// <summary>
        /// Serializes a specified instance.
        /// </summary>
        static public string Serialize(object Instance)
        {
            return Serialize(Instance, SerializerOptions);
        }
        /// <summary>
        /// Serializes a specified instance.
        /// </summary>
        static public string Serialize(object Instance, string[] ExcludeProperties)
        {
            JsonSerializerOptions Options = CreateOptions(ExcludeProperties: ExcludeProperties);
            return Serialize(Instance, Options);
        }

        // ● deserialize
        /// <summary>
        /// Deserializes (creates) an object of a specified type by deserializing a specified json text.
        /// <para>If no options specified then it uses the <see cref="SerializerOptions"/> options</para> 
        /// </summary>
        static public object Deserialize(string JsonText, Type ReturnType, JsonSerializerOptions JsonOptions = null)
        {
            JsonOptions = JsonOptions ?? SerializerOptions;
            return JsonSerializer.Deserialize(JsonText, ReturnType, JsonOptions);
        }
        /// <summary>
        /// Deserializes (creates) an object of a specified type by deserializing a specified json text.
        /// <para>If no options specified then it uses the <see cref="SerializerOptions"/> options</para> 
        /// </summary>
        static public T Deserialize<T>(string JsonText, JsonSerializerOptions JsonOptions = null)
        {
            JsonOptions = JsonOptions ?? SerializerOptions;
            return JsonSerializer.Deserialize<T>(JsonText, JsonOptions);
        }
        /// <summary>
        /// Loads an object's properties from a specified json text.
        /// <para>If no options specified then it uses the <see cref="SerializerOptions"/> options</para> 
        /// </summary>
        static public void PopulateObject(object Instance, string JsonText, JsonSerializerOptions JsonOptions = null)
        {
            if (Instance != null && !string.IsNullOrWhiteSpace(JsonText))
            {
                JsonOptions = JsonOptions ?? SerializerOptions;

                ReadOnlySequence<byte> Buffer = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(JsonText));
                Utf8JsonReader Reader = new Utf8JsonReader(Buffer);
                using (JsonDocument Doc = JsonDocument.ParseValue(ref Reader))
                {
                    PopulateObjectInternal(Instance, Doc.RootElement, JsonOptions);
                }
            }
        }

        // ● miscs
        /// <summary>
        /// Returns a specified json text as formatted for readability.
        /// </summary>
        static public string Format(string JsonText)
        {
            if (!string.IsNullOrWhiteSpace(JsonText))
            {
                var JsonOptions = CreateOptions(CameCase: false, Formatted: true);
#nullable enable
                JsonNode? Node = JsonSerializer.Deserialize<JsonNode>(JsonText);
#nullable disable
                JsonText = JsonSerializer.Serialize(Node, JsonOptions);
            }

            return JsonText;
        }
        /// <summary>
        /// Converts an object to JsonNode
        /// </summary>
        static public JsonNode ObjectToJsonNode(object Instance)
        {
            string JsonText = Serialize(Instance);
#nullable enable
            JsonNode? Node = JsonNode.Parse(JsonText);
#nullable disable

            return Node;
        }
        /// <summary>
        /// Converts a json text to a Dictionary instance.
        /// </summary>
        static public Dictionary<string, string> ToDictionary(string JsonText)
        {
            return Deserialize<Dictionary<string, string>>(JsonText);
        }
        /// <summary>
        /// Converts json text to a dynamic object which actually is a <see cref="JsonObject"/>
        /// </summary>
        static public dynamic ToDynamic(string JsonText)
        {
#nullable enable
            JsonNode? Node = JsonSerializer.Deserialize<JsonNode>(JsonText);
#nullable disable
            return Node as dynamic;
        }

        // ● to-from file
        /// <summary>
        /// Saves an instance as json text in a specified file.
        /// </summary>
        static public void SaveToFile(object Instance, string FilePath, string Encoding = "utf-8")
        {
            string Folder = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrWhiteSpace(Folder))
                Directory.CreateDirectory(Folder);
            string JsonText = Serialize(Instance);
            File.WriteAllText(FilePath, JsonText, System.Text.Encoding.GetEncoding(Encoding));
        }
 
        /// <summary>
        /// Loads the properties of an instance by reading the json text of a specified file.
        /// </summary>
        static public void LoadFromFile(object Instance, string FilePath, string Encoding = "utf-8")
        {
            if (File.Exists(FilePath))
            {
                string JsonText = File.ReadAllText(FilePath, System.Text.Encoding.GetEncoding(Encoding));
                PopulateObject(Instance, JsonText);
            }
        }
        /// <summary>
        ///  Creates and returns an object of ClassType using the json text of a specified file
        /// </summary>
        static public object LoadFromFile(Type ClassType, string FilePath, string Encoding = "utf-8")
        {
            if (File.Exists(FilePath))
            {
                string JsonText = File.ReadAllText(FilePath, System.Text.Encoding.GetEncoding(Encoding));
                return Deserialize(JsonText, ClassType);
            }

            return null;
        }

        // ● streams
        /// <summary>
        /// Converts Instance to a json string using the NewtonSoft json serializer and then to stream.
        /// <para>If no settings specified then it uses the default JsonSerializerSettings</para> 
        /// <para>NOTE: UTF8 encoding is used.</para>
        /// </summary>
        static public MemoryStream SerializeToStream(object Instance, JsonSerializerOptions JsonOptions = null)
        {
            MemoryStream MS = new MemoryStream();
            SerializeToStream(Instance, MS, JsonOptions);
            return MS;
        }
        /// <summary>
        /// Converts Instance to a json string using the NewtonSoft json serializer and then to stream.
        /// <para>If no settings specified then it uses the default JsonSerializerSettings</para> 
        /// <para>NOTE: UTF8 encoding is used.</para>
        /// </summary>
        static public void SerializeToStream(object Instance, Stream Stream, JsonSerializerOptions JsonOptions = null)
        {
            string JsonText = Serialize(Instance, JsonOptions);
            JsonTextToStream(JsonText, Stream);
        }
        /// <summary>
        /// Converts a specified json text to a stream.
        /// <para>NOTE: UTF8 encoding is used.</para>
        /// </summary>
        static public void JsonTextToStream(string JsonText, Stream Stream)
        {
            byte[] Buffer = Encoding.UTF8.GetBytes(JsonText);
            Stream.Write(Buffer, 0, Buffer.Length);
        }

        /// <summary>
        /// Reads the json text from a stream and then deserializes (creates) an object of a specified type.
        /// <para>If no settings specified then it uses the default JsonSerializerSettings</para> 
        /// <para>NOTE: UTF8 encoding is used.</para>
        /// </summary>
        static public object DeserializeFromStream(Type ClassType, Stream Stream, JsonSerializerOptions JsonOptions = null)
        {
            string JsonText = StreamToJsonText(Stream);
            return Deserialize(JsonText, ClassType, JsonOptions);
        }
        /// <summary>
        /// Loads an object's properties from a specified stream, after reading the json text from the stream.
        /// <para>If no settings specified then it uses the default JsonSerializerSettings</para> 
        /// <para>NOTE: UTF8 encoding is used.</para>
        /// </summary>
        static public void DeserializeFromStream(object Instance, Stream Stream, JsonSerializerOptions JsonOptions = null)
        {
            string JsonText = StreamToJsonText(Stream);
            PopulateObject(Instance, JsonText, JsonOptions);
        }
        /// <summary>
        /// Reads a stream as json text.
        /// <para>NOTE: UTF8 encoding is used.</para>
        /// </summary>
        static public string StreamToJsonText(Stream Stream)
        {
            string JsonText = string.Empty;
            if (Stream != null && Stream.Length > 0)
            {
                using (StreamReader reader = new StreamReader(Stream, Encoding.UTF8))
                {
                    JsonText = reader.ReadToEnd();
                }
            }

            return JsonText;
        }


        /// <summary>
        /// Returns the text of the input stream of a request (HttpContext.Request.Body) as a Dictionary. To be used when POST-ing json data.
        /// </summary>
        static public Dictionary<string, dynamic> GetRequestDic(Stream RequestBodyStream)
        {
            if (RequestBodyStream != null && RequestBodyStream.CanSeek)
            {
                string Text = StreamToJsonText(RequestBodyStream);
                if (!string.IsNullOrWhiteSpace(Text))
                {
                    return Deserialize<Dictionary<string, dynamic>>(Text);
                }
            }

            return new Dictionary<string, dynamic>();
        }

        // ● properties
        /// <summary>
        /// Returns the default serialization options
        /// </summary>
        static public JsonSerializerOptions DefaultSerializerOptions
        {
            get
            {
                if (fDefaultSerializerOptions == null)
                    fDefaultSerializerOptions = CreateOptions(CameCase: false, Formatted: true, CaseInsensitiveProperties: true);

                return fDefaultSerializerOptions;
            }
        }
        /// <summary>
        /// Used when no options or null options are specified.
        /// </summary>
        static public JsonSerializerOptions SerializerOptions
        {
            get => fSerializerOptions != null ? fSerializerOptions : DefaultSerializerOptions;
            set => fSerializerOptions = value;
        }
        /// <summary>
        /// This property is used whenever this class is about to create <see cref="JsonSerializerOptions"/>.
        /// <para>If this property is null then the <see cref="CreateOptions()"/> method is used. </para>
        /// <para>The following is the signature the callback should have.</para>
        /// <code>JsonSerializerOptions CreateOptionsFunc(bool CameCase, bool Formatted, bool CaseInsensitiveProperties = true, string[] ExcludeProperties)</code>
        /// </summary>
        static public Func<bool, bool, bool, string[], JsonSerializerOptions> CreateOptionsFunc { get; set; } 
    }



    /// <summary> 
    /// A <see cref="JsonNamingPolicy"/> that leaves property names as they are.
    /// <para>For example: <c>CustomerName</c> is serialized as <c>CustomerName</c> and <c>Customer_Name</c> is serialized as <c>Customer_Name</c></para>
    /// </summary>
    public class JsonNamingPolicyAsIs : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name;
        }
    }


    /// <summary>
    /// Used in excluding properties when serializing.
    /// <para>E.g. JsonConvert.SerializeObject(Instance, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new JsonNetContractResolver(ExcludeProperties) }) </para>
    /// </summary>
    internal class ExcludePropertiesTypeInfoResolver : DefaultJsonTypeInfoResolver
    {
        string[] ExcludeProperties = new string[0];

        static void RemoveAll<T>(IList<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    list.RemoveAt(i--);
                }
            }
        }
        void ModifierFunc(JsonTypeInfo TypeInfo)
        {
            if (TypeInfo.Kind != JsonTypeInfoKind.Object)
                return;

            RemoveAll(TypeInfo.Properties, prop => ExcludeProperties.Contains(prop.Name)); 
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ExcludePropertiesTypeInfoResolver(string[] ExcludeProperties)
        {
            this.ExcludeProperties = ExcludeProperties;
            this.Modifiers.Insert(0, ModifierFunc);
        }
    }

}
