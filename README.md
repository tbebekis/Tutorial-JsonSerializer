# Serialize and deserialize JSON using JsonSerializer

This text explores the use of the [JsonSerializer](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializer) .Net class in serializing and deserializing .Net classes to [JSON](https://www.json.org/json-en.html).

.Net types related to serialization are found in the following namespaces

- [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/api/system.text.json). Contains the `JsonSerializer`, the [JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions) and other main types.
- [System.Text.Json.Serialization](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization). Contains attribute, converters and similar auxiliary classes used in customizing serialization.

There is of course the excellent [Newtonsoft.Json](https://www.newtonsoft.com/json) library out there. Besides that the `JsonSerializer` worths using it since it is the native solution offered by .Net and there is no need to install any [NuGet](https://www.nuget.org/) package in order to use it.

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
string JsonText =  JsonSerializer.Serialize(P, JsonOptions);

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
Result.DictionaryKeyPolicy = Result.PropertyNamingPolicy;
Result.PropertyNameCaseInsensitive = true;
Result.WriteIndented = true;
Result.IgnoreReadOnlyProperties = true;
Result.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;  
Result.ReadCommentHandling = JsonCommentHandling.Skip;
Result.AllowTrailingCommas = true;
Result.NumberHandling = JsonNumberHandling.AllowReadingFromString;
Result.ReferenceHandler = ReferenceHandler.Preserve;  
Result.Converters.Insert(0, new JsonStringEnumConverter(Result.PropertyNamingPolicy));

return Result;

...

public class JsonNamingPolicyAsIs : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name;
    }
}
```



## Attributes

The [System.Text.Json.Serialization](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization) namespace provides a great number of attributes that can be used with properties or classes in order to control the serialization.

Notable attributes worth exploring are:

#### [JsonConstructor]

Indicates a constructor that should be used in serialization.

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

Secifies what converter type to use in serialization.

```
[JsonConverter(typeof(JsonStringEnumConverter))]
public Status Status { get; set; }

[JsonConverter(typeof(DateOnlyConverter))]
public DateOnly BirthDate { get; set; }
```

The `[JsonConverter(typeof(JsonStringEnumConverter))]` can be used also with *enum* types.

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
A converter converts an object or a value to and from JSON. 

More on converters can be found at .Net Core [docs](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to).


#### [JsonIgnore]
Indicates that the property should be ignored in serialization.

```
[JsonIgnore]
public string Password { get; set; }

[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
public string Message { get; set; } // ignored when null, the default

[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
public int Value { get; set; } // ignored when 0, the default

[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
public int Value2 { get; set; } =  123; // ignored when 123, the default
```

#### [JsonInclude]

Forces serialization of public field or a public property even when it has just a private setter.

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

**None of the following attributes** is used in `System.Text.Json` serialization. 

Only the attributes found in `System.Text.Json.Serialization` are taken into account. The `System.Text.Json` namespace has its own attributes, all with `Json` as prefix.

Except of the `System.Text.Json` there are some other namespaces providing validation attributes.

- `System.ComponentModel.DataAnnotations` namespace. Provides a number of attributes such as `[MaxLength]`, `[Required]`, `[Range]`, etc. Used in `Binary Serialization` or `SOAP Serialization`. Also used in `MVC` and `WebAPI` model validation.
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

Here is a resolver that excludes a list of specified properties from serialization.

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

That `JsonNamingPolicy` class provides a number of static properties that return a `JsonNamingPolicy` derived class, such as [CamelCase](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonnamingpolicy.camelcase) or [SnakeCaseLower](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonnamingpolicy.snakecaselower). Each one for a specific casing.

```
JsonSerializerOptions JsonOptions = new();

JsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CameCase; 
```

## The PopulateObject() problem.

`System.Text.Json` provides a solution under the title [Populate initialized properties](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/populate-properties) which is far from what is actully needed.

Frequently there are cases where there is an already constructed instance that needs to be populated using data coming as json text. This is a problem that the `System.Text.Json` has no solution to offer yet.

The `JsonSerializer.Deserialize()` methods always create and return a new instance. This not always what an application needs. 

But deep in the .Net source code there is a class containing methods that do just that. The [Microsoft.Graph.DerivedTypeConverter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.graph.derivedtypeconverter) contains a **private** method, named [PopulateObject()](https://github.com/microsoftgraph/msgraph-sdk-dotnet-core/blob/57861dc4aea6c33908838915c97fc02105b6e788/src/Microsoft.Graph.Core/Serialization/DerivedTypeConverter.cs#L112-L114) which does exactly what is says.

 
The project that accompanies this text contains a static class under the name `NetJson` which provides, among other useful utilities, a `PopulateObject()` method. This `PopulateObject()` method is just the code from the `Microsoft.Graph.DerivedTypeConverter.PopulateObject()` private method.

`static public void PopulateObject(object Instance, string JsonText, JsonSerializerOptions Options)`



 















