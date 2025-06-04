# Serialize and deserialize JSON using JsonSerializer

This text explores the use of the [JsonSerializer](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializer) .Net class in serializing and deserializing .Net classes to [JSON](https://www.json.org/json-en.html).

.Net types related to serialization are found in the following namespaces

- [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/api/system.text.json). Contains the `JsonSerializer`, the [JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions) and other main types.
- [System.Text.Json.Serialization](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization). Contains attributes, converters and similar auxiliary classes used in customizing serialization.

There is of course the excellent [Newtonsoft.Json](https://www.newtonsoft.com/json) library but `JsonSerializer` is worth using it since it is the native solution provided by .Net and there is no need to install any [NuGet](https://www.nuget.org/) package in order to use it.

`JsonSerializer` can be used with .Net Core 3.0 and later and with .Net Standard 2.0.

## Basics

The following code entities are used in this text.

```
public enum Status
{
    None,
    Pending,
    InProgress,
    AllCompleted
}

public class Part
{
    public string Code { get; set; }
    public decimal Amount { get; set; } 
    public bool IsCompleted { get; set; }
}
```

`JsonSerializer` is a *static* class. No need to create an instance.

```
// serialization
Part P = new(); 
string JsonText =  JsonSerializer.Serialize(P);

// de-serialization
Part P2 = JsonSerializer.Deserialize<Part>(JsonText);

// or
P2 = JsonSerializer.Deserialize(JsonText, typeof(Part)) as Part;
```
 
## JsonSerializerOptions

The `Serialize()` and `Deserialize()` methods accept a [JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions) parameter.


```
JsonSerializerOptions JsonOptions = new();

// serialization
Part P = new(); 
string JsonText = JsonSerializer.Serialize(P, JsonOptions);

// de-serialization
Part P2 = JsonSerializer.Deserialize<Part>(JsonText, JsonOptions);

// or
P2 = JsonSerializer.Deserialize(JsonText, typeof(Part), JsonOptions) as Part;
```
 
The `JsonSerializerOptions` controls the behavior of the `JsonSerializer`. It provides a great number of properties in order to customize the serialization operation.

Notable properties worth exploring are:

- [Converters](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.converters)
- [DefaultIgnoreCondition](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.defaultignorecondition)
- [DictionaryKeyPolicy](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.dictionarykeypolicy)
- [IgnoreReadOnlyProperties](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.ignorereadonlyproperties)
- [IncludeFields](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.includefields)
- [NumberHandling](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.numberhandling)
- [PropertyNameCaseInsensitive](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.propertynamecaseinsensitive)
- [PropertyNamingPolicy](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.propertynamingpolicy)
- [ReadCommentHandling](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.readcommenthandling)
- [ReferenceHandler](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.referencehandler)
- [TypeInfoResolver](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.typeinforesolver)
- [Web](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.web)
- [WriteIndented](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.writeindented)

An example of creating a `JsonSerializerOptions` instance.

```
JsonSerializerOptions Result = new();

Result.PropertyNamingPolicy = null;
Result.PropertyNameCaseInsensitive = true;
Result.WriteIndented = true;
Result.IgnoreReadOnlyProperties = true;
Result.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;  
Result.ReadCommentHandling = JsonCommentHandling.Skip;
Result.AllowTrailingCommas = true;
Result.NumberHandling = JsonNumberHandling.AllowReadingFromString;
Result.ReferenceHandler = ReferenceHandler.Preserve;  

return Result;
```



## Attributes

The [System.Text.Json.Serialization](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization) namespace provides a great number of attributes that can be used with properties or classes in order to control the serialization.

Notable attributes worth exploring are:

#### [JsonConstructor]

Indicates a constructor that should be used by the serializer.

```
public class Part
{
    public Part() 
    {
    }
    [JsonConstructor]
    public Part(string code) 
    {
       Code = code;
    }
    public string Code { get; set; }  
    public decimal Amount { get; set; }  
    public bool IsCompleted { get; set; } 
}
```

#### [JsonConverter]

Specifies what converter type to be used in serialization.

```
[JsonConverter(typeof(JsonStringEnumConverter))]
public Status Status { get; set; }

[JsonConverter(typeof(DateOnlyConverter))]
public DateOnly BirthDate { get; set; }
```

The `[JsonConverter(typeof(JsonStringEnumConverter))]` can be used with *enum* types also.

```
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Status
{
    None,
    Pending,
    InProgress,
    AllCompleted
}
```

#### [JsonIgnore]
Indicates that the property should be ignored in serialization.

```
[JsonIgnore]
public string Secret { get; set; }

[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
public string Message { get; set; } // ignored when null, the default

[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
public int Value { get; set; } // ignored when 0, the default

[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
public int Value2 { get; set; } =  123; // ignored when 123, the default
```

#### [JsonInclude]

Forces serialization of a public field or a public property even when it has just a private setter.

```
[JsonInclude]
public int Age;

[JsonInclude]
public string ReadOnlyProperty { get; private set; }
```

#### [JsonNumberHandling]

Controls how a number is serialized or deserialized by using a [JsonNumberHandling](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonnumberhandling) setting.

```
[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public int Amount { get; set; }
```

#### [JsonPropertyName]

Controls the name under which the property is serialized to or deserialized from.

```
[JsonPropertyName("user_name")]
public string UserName { get; set; }
```

#### [JsonPropertyOrder]

Controls the serialization order of the property.

```
[JsonPropertyOrder(5)]
public string Name { get; set; }
```

#### [JsonRequired]

Dictates that the property must be present.

```
[JsonRequired]
public string Name { get; set; }
```

## Attributes from other namespaces

> **None of the following attributes** is used in `System.Text.Json` serialization. 
>
> Only the attributes found in `System.Text.Json.Serialization` are taken into account. The `System.Text.Json` namespace has its own attributes, all with `Json` as prefix.

Except of the `System.Text.Json` there are some other namespaces providing validation attributes.

- `System.ComponentModel.DataAnnotations` namespace. Provides a number of attributes such as `[MaxLength]`, `[Required]`, `[Range]`, etc. Used in `Binary Serialization` or `SOAP Serialization`. Also used in Asp.Net Core `MVC` and `WebAPI` model validation.
- `System.Runtime.Serialization` namespace. Provides  attributes such as `[DataContract]` and `[DataMember]`. Used by `DataContractSerializer`.
- `System.Xml.Serialization` namespace. Provides attributes such as `[XmlRoot]`, `[XmlElement]`, `[XmlAttribute]`. Used in `XML Serialization`.



The static [System.ComponentModel.DataAnnotations.Validator](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validator) class can be used to manually validate classes annotated with [ValidationAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validationattribute) derived attributes, such as `MaxLength` and `Range`.

```
MyModel Model = JsonSerializer.Deserialize<MyModel>(JsonText);

List<string> ErrorList = Validate(Model);

...

public List<string> Validate(object Instance)
{
    List<string> ErrorList = new();
    var validationContext = new ValidationContext(Instance);
    var validationResults = new List<ValidationResult>();

    // not valid?
    // collect validation errors in a list
    if (!Validator.TryValidateObject(Instance, validationContext, validationResults, true))
    {
        foreach (var validationResult in validationResults)
        {
            ErrorList.Add(validationResult.ErrorMessage);
        }        
    }

    return ErrorList; // empty list means Instance is valid
}
```

In Asp.Net Core MVC or WebAPI controllers the `ModelState.IsValid` is used to validate attributes based on `System.ComponentModel.DataAnnotations` namespace annotations.

## Customize serialization with Converters

A [converter](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to) converts an object or a value to and from JSON text.  

A custom converter can also be used.

```
[JsonConverter(typeof(CustomDateOnlyConverter))]
public DateOnly BirthDate { get; set; }

...

public class CustomDateOnlyConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
    }
}
```

There are two patterns in creating a custom converter

- [Basic pattern](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to#sample-basic-converter). The custom converter derives from [`JsonConverter<TValue>`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter-1) class.
- [Factory pattern](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to#sample-factory-pattern-converter). The custom converter derives from [JsonConverterFactory](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverterfactory) class.

A custom converter can be registered

- by adding an instance of the custom converter to [JsonSerializerOptions.Converters](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.converters) collection
- by applying the `[JsonConverter]` to a class that represents a custom value type
- by applying the `[JsonConverter]` to properties that require the custom converter.


When there are multiple converters applied then there are [rules that dictate the order](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to#converter-registration-precedence) by which a converter is chosen in serialization.


 

## Customize serialization with a Resolver and Modifier functions

The [JsonSerializerOptions.TypeInfoResolver](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.typeinforesolver) property provides a way to plug `modifier` functions of the type `Action<JsonTypeInfo>` into serialization.

Inline modifiers with lambda functions.

```
JsonSerializerOptions JsonOptions = new()
{
    TypeInfoResolver = new DefaultJsonTypeInfoResolver
    {
        Modifiers =
        {
            (TypeInfo) => {
                //...
            },
            (TypeInfo) => {
                //...
            }
        }
    }
};
```

Or usual methods in a class.

```
static public class Helper
{
    static public void ModifierFunc1(JsonTypeInfo TypeInfo)
    {
        //...
    }
    static public void ModifierFunc2(JsonTypeInfo TypeInfo)
    {
        //...
    }
}

JsonSerializerOptions JsonOptions = new();
Json.Options.Modifiers.Add(Helper.ModifierFunc1);
Json.Options.Modifiers.Add(Helper.ModifierFunc2);
```

Here is a custom resolver that excludes a list of specified properties from serialization.

```
public class ExcludePropertiesTypeInfoResolver : DefaultJsonTypeInfoResolver
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
 
    public ExcludePropertiesTypeInfoResolver(string[] ExcludeProperties)
    {
        this.ExcludeProperties = ExcludeProperties;
        this.Modifiers.Insert(0, ModifierFunc);
    }
}
```

And here is how to use it.

```
var ExcludeProperties = new[] { "Prop1", "Prop2"};

JsonSerializerOptions JsonOptions = new()
{
    TypeInfoResolver = new ExcludePropertiesTypeInfoResolver(ExcludeProperties)
}; 
```

The [JsonTypeInfo](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.metadata.jsontypeinfo) provides metadata about the type being serialized.

Regarding modifier functions .Net Core docs provide a number of [examples](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/custom-contracts#modifiers).

## Property Name Casing

The casing of a property name, such as camel-casing, is controlled by the [JsonSerializerOptions.PropertyNamingPolicy](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.propertynamingpolicy).

`PropertyNamingPolicy` property is a [JsonNamingPolicy](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonnamingpolicy) derived class. When it is **null**, the default, property names remain unchanged.

That `JsonNamingPolicy` class provides a number of static properties that return a `JsonNamingPolicy` derived class instance, such as [CamelCase](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonnamingpolicy.camelcase) or [SnakeCaseLower](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonnamingpolicy.snakecaselower). Each one for a specific casing.

```
JsonSerializerOptions JsonOptions = new();

JsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CameCase; 
```

## The PopulateObject() problem.

`System.Text.Json` provides a solution under the title [Populate initialized properties](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/populate-properties) which is far from what is actully needed.

Frequently there are cases where there is an already constructed instance that needs to be populated using data coming as json text. This is a problem that the `System.Text.Json` has no solution to offer yet.

The `JsonSerializer.Deserialize()` methods always create and return a new instance. This is not always what an application needs. 

But deep in the .Net source code there is a class containing a method that do just that. The [Microsoft.Graph.DerivedTypeConverter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.graph.derivedtypeconverter) contains a **private** method, named [PopulateObject()](https://github.com/microsoftgraph/msgraph-sdk-dotnet-core/blob/57861dc4aea6c33908838915c97fc02105b6e788/src/Microsoft.Graph.Core/Serialization/DerivedTypeConverter.cs#L112-L114) which does exactly what it says.

 
> The project that accompanies this text contains a static class under the name `NetJson` which provides, among other useful utilities, a `PopulateObject()` method. This `PopulateObject()` method is just the code from the `Microsoft.Graph.DerivedTypeConverter.PopulateObject()` private method.
>
> `static public void PopulateObject(object Instance, string JsonText, JsonSerializerOptions Options)`


## The Document Object Model (DOM) of System.Text.Json 

Except of the `JsonSerializer` the .Net Core serialization sub-system provides a [DOM model](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-dom) too.

The system provides two ways in building a DOM model.

- [System.Text.Json.JsonObject](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonobject) class, along with [JsonNode](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonnode), [JsonArray](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonarray) and [JsonValue](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonvalue) classes
- [System.Text.Json.JsonDocument](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocument) along with [JsonElement](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement) class.

## JsonObject, JsonNode, JsonArray and JsonValue

These are mutable classes, meaning the application may add, modify or remove elements in the DOM tree.

- `JsonNode` is an abstract class. Besides that it provides a great number of **static** helper methods for adding elements to the DOM tree. It also serves as the base class for the others in this group.
- `JsonObject` represents a mutable DOM object.
- `JsonArray` represents a mutable DOM array object.
- `JsonValue` represents a mutable DOM value object.


All the above provide the following properties 
- an integer indexer property `Item[Int32]`
- a string indexer property `Item[String]`
- a `Count` property

All the above provide the following methods 
- `AsObject()` 
- `AsArray()`  
- `AsValue()`  
- `GetPath()`
- `GetPropertyName()`
- `GetType()`
- `GetValue<T>()`
- `GetValueKind()` which returns a value of the [JsonValueKind](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonvaluekind) enum
- `ReplaceWith<T>(T)`
- `ToJsonString()`

The `JsonObject` and `JsonArray` provide the following methods too

- `Add()`
- `Clear()`
- `Remove()`
- `RemoveAt()`

#### Primitive Values using JsonValue.Create(AnyPrimitiveValue)

`JsonObject` properties are `Key-Value` pairs, i.e. `JORoot.Add(PropertyName, PropertyValue)`.

The `JsonValue.Create(AnyPrimitiveValue)` is used with primitive Types such as string and integer, in creating new `JsonValue` objects.

```
JsonObject JORoot = new();
JORoot.Add("String", JsonValue.Create("This is a JsonValue"));
JORoot.Add("DateTime", JsonValue.Create(DateTime.Now));
JORoot.Add("Integer", JsonValue.Create(123));
```

#### Removing a Property
Using the `Remove(PropertyName)` removes the property.

```
JORoot.Remove("DateTime");
```

#### JsonObject is a Dictionary-like object

`JsonObject` instances are `Dictionary-like` objects and a new `JsonObject` can be initialized as following.

```
JsonObject JORoot = new JsonObject
{
    ["Key1"] = "This is a string",
    ["Key2"] = DateTime.Now,
    ["Key3"] = false,
};
```
#### The JsonNode.Parse() method

The `JsonNode.Parse(JsonText)` **static** method parses text and returns a `JsonNode` object.

```
JsonObject JORoot = new();
JORoot.Add("Person", JsonNode.Parse("""{ "Name": "John Doe", "Age": 30 }"""));
```
#### Using the JsonValue.Create\<T\>() with non-primitive Types

The `JsonValue.Create<T>()` is used with non-primitive Types such as user defined classes, in creating new `JsonValue` objects.

```
JsonObject JORoot = new();
JORoot.Add("Part", JsonValue.Create<Part>(new Part()));
```
#### Initializing a JsonArray

```
JsonObject JORoot = new();
JsonArray JOArray = new JsonArray() { 123, true, DateTime.Now, "string value" };
JORoot.Add("Array", JOArray);
```

#### Using a Dictionary<string, JsonNode>

A `Dictionary<string, JsonNode>` can be used in adding a property to a `JsonObject` object.

The `Dictionary<string, JsonNode>` dictionary has to be converted to a `JsonObject` first.

This can be done because `JsonObject` provides a suitable constructor.

`public JsonObject(IEnumerable<KeyValuePair<string, JsonNode?>> properties, JsonNodeOptions? options = null)`

```
JsonObject JORoot = new();
 
var Dictionary = new Dictionary<string, JsonNode>
{
    ["Key1"] = "This is a string",
    ["Key2"] = DateTime.Now,
    ["Key3"] = false,
    ["Key4"] = JsonValue.Create<Status>(Status.InProgress),
};

JsonObject DicNode = new JsonObject(Dictionary);
JORoot.Add("Dictionary", DicNode);
```

#### Accessing Properties and Values

```
string DemoJsonText = """
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

JsonNode RootNode = JsonNode.Parse(DemoJsonText);

// accessing array
JsonNode PartsNode = RootNode["Parts"];
JsonNode FirstPartNode = PartsNode[0]; 

// adding new property
JsonObject ThirdPartNode = new();
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

// get the path
string S2 = RootNode["Parts"][0].GetPath();
// $.Parts[0]   where $ denotes the root node
```

## JsonDocument

`JsonDocument` is used in building a **read-only** DOM. It provides the `RootElement` of type `JsonElement`.

`JsonDocument` elements are accessed using the `JsonElement` class.

The `JsonElement` class provides enumerators in order to iterate over its elements.

The `JsonElement` class provides methods such as `GetInt32()` and `TryGetInt32()` which convert JSON text to .Net primitive types.

> **NOTE**: `JsonElement` is an `IDisposable` type.

```
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
```

 















