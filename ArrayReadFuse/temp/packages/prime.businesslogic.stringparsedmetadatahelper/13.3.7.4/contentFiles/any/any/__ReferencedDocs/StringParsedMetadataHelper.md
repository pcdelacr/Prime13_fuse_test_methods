**prime Test-Method Specification REP**

Revision 1.0.0

Mar 2022

[[_TOC_]]

## REP for StringParsedMetadataHelper

This **REP** is intended to describe the StringParsedMetadataHelper helper class and its usage model.

*SPM* stands for "*String-Parsed Metadata*" which is a MIDAS methodology to print complex data structures in an encoded way that external tools (MIDAS API) can analyze the data without the need for additional middle-man scripts to decode the data. 

The external tools documentation will not be provided in this document.

In this document, you will find the below sections:

  - **Methodology** – A detailed description about *SPM* methodology

  - **How To Use** – A detailed description about the StringParsedMetadataHelper class and its usage model

  - **Level Descriptors Detailed Documentation** – A detailed description about all the supported Level Descriptors

  - **Full Code Samples** – Examples of how to use the StringParsedMetadataHelper class in User Code

  - **Version Tracking** – With author names, so you always have a name to address
  
  - **Contacts** - *SPM* system contacts

## Methodology

*SPM* - *String-Parsed Metadata* is a MIDAS methodology to encode complex datastructre definition into a string, that can be later decoded and used by MIDAS API to extract data from the database. It uses the ituff *strgval* ***msunit*** attribute and level 4 ***uddtype*** tokens to define the structure of the *strgval* value.

The *SPM* architecture is based on the following building blocks called "*Level Descriptors*" (**Ld** for short):
1. LdNamedList: defines a matrix column header definition.
2. LdIndexAscendingList / LdIndexDescendingList: defines a matrix of auto column header definition based on numeric index.
3. LdDictionary (key -> value): defines a key/value based relationship. A dictionary value can be any of the above (e.g., Dictionary value can be a dictionary by itself).

The *SPM* system is using the ituff *Strgval* token to pass the value and the corresponding definition to the MIDAS API external.

For example, the below ituff content:
```python
4_uddtype_TOD_IDKTEST_ColA,COL_B,col$C  <-- NamedList column definition
...
2_tname_sometestwithstringdata          <-- Tname
2_msunit_SPMV1://[!IDKTEST]|//          <-- SPM encoded definition
2_strgval_12|13|22                      <-- SPM encoded data
```

Will result in the following data Matrix in the MIDAS API external tool:

| **SOMETESTWITHSTRINGDATA-COLA** | **SOMETESTWITHSTRINGDATA-COL_B** | **SOMETESTWITHSTRINGDATA-COL$C** 
| ------------------ | ------------- | -------- |
|12|13|22

For more information on *SPM* syntax and examples, please refer to this [Examples file](StringParsedMetadataHelper/SpecDocs/SPMV1%20Examples%20rev2.pdf) and [Spec File](StringParsedMetadataHelper/SpecDocs/String-Parsing%20Metadata%20Reference%20rev2.pdf) 

<small>*The spec files are here for reference, you should always contact MIDAS team for up-to-date files.*</small>

## How To Use
The StringParsedMetadataHelper class purpose is to provide a simplified, unified and abstracted Prime API for *SPM* usages. The helper class is in charge of all the decoding and formatting of the *strgval* token and associative level 4 *uddtype* entries (according to *SPM* spec), leaving the user code to handle only the business logic.

#### Preparation
In order to use the StringParsedMetadataHelper class, make sure you have a *PackageReference* in your csproj to the helper class nuget:
```csharp
<ItemGroup>
    <PackageReference Include="Prime.TestMethods.StringParsedMetadataHelper" Version="$(PrimeVersion)">
       <PrivateAssets>all</PrivateAssets>
       <ExcludeAssets>runtime</ExcludeAssets>
    </PackageReference>
</ItemGroup>
```

#### Declaration
You'll need to define a StringParsedMetadataHelper member in your UC:
```csharp
private StringParsedMetadataHelper myHelper;
```

#### Creating a StringParsedMetadataHelper object
The creation of the StringParsedMetadataHelper object is done via a static factory method `Create` that exist in the `StringParsedMetadataHelper` namespace.

```csharp
public static StringParsedMetadataHelper Create(LdBase topLevelDescriptor, string tNameForDatalog = null)
```
| Parameter | Description |
| --------- | ----------- |
| topLevelDescriptor      | LdDescriptor that defines the top level descriptor. It can hold heirarchy of Lds (see examples below) |
| tNameForDatalog | The string to use for "tname" under the *strgval* token, if omitted the current instance name will be used|


#### *Verify* Method
During *verify* method, the private member object needs to be created and configured.

The `topLevelDescriptor` is  only 1 required argument to pass to this method. In this argument you'll define the structure of the *SPM* data.

```csharp
this.myHelper = StringParsedMetadataHelper.Create(new LdNamedList("IDKTEST", "|", new List<string> { "Col1", "Col2", "Col3" }));
```
In the above example, a single *LdNamedList* is provided as the top level descriptor (for more information, please refer to the Ld classes [detailed documentation](#level-descriptors) down below).

#### *Execute* Method
The StringParsedMetadataHelper object holds the previous DUT data, so first need to clear it.

```csharp
this.myHelper.ClearData();
```

Now you can load data into the object, using the *SetData* method:
- Then current DUT's data needs to be loaded into the object. 
Data can be loaded only as a list of strings. The user owns the business logic to define the values.
- LdDictionary keys are always a string.
- For a *LdNamedList* class, the number of data elements must match the number of fields (level descriptors), an exception will be thrown otherwise.

```csharp
// assigning values to a LdList (Named/AscendingIndex/DescendingIndex)
this.myHelper.SetData(new List<string> { "12", "13", "23" });

// assigning values to a LdDictionary structure
// here we have 2 levls, 
//   "IA" is the key for the first level 
//   "4.300" is the key to the second level
//   third level is a LdList with two columns
this.myHelper["IA"]["4.300"].SetData(new List<string> { "1.095", "1.111" });
```

Once data is loaded to the object, the underline ituff writers can be fetched and can be directly routed to Prime *DatalogServices* or accumulated to a list for future usage.
```csharp
var listOfItuffWriters = this.myHelper.GetItuffFromData();
```

In the case you need to define a different UDD record scheme, you can clear the previous one.
```csharp
this.myHelper.ClearUddTokens();
```

## Level Descriptors Detailed Documentation

#### NamedList
Specialized LevelDescriptor for named column matrix. This object will generate the required ituff level 4 *uddtype* required records. It also contains logic to aggregate and distinct the list of all required *uddtype* across the whole enabled TP's instances that uses StringParsedMetadataHelper.

```csharp
LdNamedList(string name, string columnDataSeparator, IEnumerable<string> descriptors, string secondDataSeparator = null)
```

| Parameter | Description |
| --------- | ----------- |
| name      | The Name to identify this matrix column name definition |
| columnDataSeparator | A valid *SPM* char to separate the *strgval* data string to columns' values |
| descriptors | IEnumarable of column names |
| secondDataSeparator| A valid *SPM* second char to further separate the *strgval* data string to columns' values |

Expected *SPM msunit* format :`SPMV1:://[!<name>]<columnDataSeparator><secondDataSeparator>//` 

#### LdAscendIndexList / LdDescendIndexList
Specialized LevelDesciprtors for index based auto generated column names. As the columns' names are auto-generated by the MIDAS API, no *uddtype* record is required.

```csharp
LdAscendIndexList(string name, string dataSeparator, string startIndex)
LdDescendIndexList(string name, string dataSeparator, string startIndex)
```
| Parameter | Description |
| --------- | ----------- |
| name      | The Name to identify this list |
| dataSeparator | A valid *SPM* char to separate the *strgval* data string to columns' values |
| startIndex | A string representing the numberic start index and the format (number of decimal values) |

Expected *SPM msunit* format:

* *LdAscendIndexList*: `SPMV1:://[<name>]@F<startIndex><dataSeparator>//`
* *LdDescendIndexList*: `SPMV1:://[<name>]@R<startIndex><dataSeparator>//` 

#### LdDictionary
Specialized LevelDesciprtor for dictionary key/value based data structure. 

* An *LdDictionary* key is always a string.
* An *LdDictionary* value can be either any of other LevelDescriptors including a LdDictionary itself.
* The limitation of the number of hierarchy levels is limited by the overall size of the *msunit* attribute (25 chars)

```csharp
LdDictionary(string keyName, string keyValuePairsSeparatorChar, string keyValueSeparatorChar)

// initialize/access the Value level descriptor 
public LdBase ValueLevelDescriptor { get; set; }
```
| Parameter | Description |
| --------- | ----------- |
| keyName      | The Dictionary key Name to use for column name generation by the MIDAS API |
| keyValuePairsSeparatorChar | A valid *SPM* char to separate the *strgval* data string to KeyValue pairs |
| keyValueSeparatorChar | A valid *SPM* char to separate each of the *strgval* data KeyValue pairs to key and value |

Expected *SPM msunit* format:`SPMV1:://{<keyName>}<startIndex><dataSeparator>/<Value LevelDescriptor msunit>...//`

## Full Code Samples
Full code examples can also be found in the Prime Sample TP that is deployed as part of every release.

1. NamedList
```csharp
[PrimeTestMethod]
public class StringParsedMetadataNamedList : TestMethodBase
{
    private StringParsedMetadataHelper myHelper;

    /// <inheritdoc />
    public override void Verify()
    {
        Prime.Services.ConsoleService.PrintDebug("Create and define the spmHelper object");
        this.myHelper =
            StringParsedMetadataHelper.Create(new LdNamedList("IDKTEST", "|", new List<string> { "Col1", "Col2", "Col3" }));
    }

    /// <inheritdoc />
    [Returns(1, PortType.Pass, "Pass!")]
    [Returns(0, PortType.Fail, "Fail!")]
    public override int Execute()
    {
        this.myHelper.ClearData();

        // number of data elements must match the number of items in named list descriptors
        // or else an exception will be thrown
        this.myHelper.SetData(new List<string> { "12", "13", "23" });

        Prime.Services.DatalogService.WriteToItuff(this.myHelper.GetItuffFromData());

        return 1;
    }
    
    /// Expected ituff
    /// 4_uddtype_TOD_IDKTEST_Col1,Col2,Col3
    /// ...
    /// 2_tname_dfds
    /// 2_msunit_SPMV1://[!IDKTEST]|//
    /// 2_strgval_12|13|23
}
```

2. Dictionary
```csharp
[PrimeTestMethod]
public class StringParsedMetadataHierarchy : TestMethodBase
{
    private StringParsedMetadataHelper myHelper;

    /// <inheritdoc />
    public override void Verify()
    {
        Prime.Services.ConsoleService.PrintDebug("Create and define the spmHelper object");
        var topLevelStructure = new LdDictionary("IP", "_", ":")
        {
            ValueLevelDescriptor = new LdDictionary("FREQ", "%", "^")
            {
                ValueLevelDescriptor = new LdAscendIndexList("CORE", "V", "00"),
            },
        };
        this.myHelper = StringParsedMetadataHelper.Create(topLevelStructure);
    }

    /// <inheritdoc />
    [Returns(1, PortType.Pass, "Pass!")]
    [Returns(0, PortType.Fail, "Fail!")]
    public override int Execute()
    {
        this.myHelper.ClearData();

        // hierarchy should be access by using string keys only (SPM spec)
        this.myHelper["IA"]["4.300"].SetData(new List<string> { "1.095", "1.111" });
        this.myHelper["IA"]["3.700"].SetData(new List<string> { "0.973", "0.942" });
        this.myHelper["IAX2"]["4.300"].SetData(new List<string> { "1.115", "1.132" });
        this.myHelper["IAX2"]["3.700"].SetData(new List<string> { "1", "1" });
        this.myHelper["CLM"]["2.400"].SetData(new List<string> { "0.793" });

        Prime.Services.DatalogService.WriteToItuff(this.myHelper.GetItuffFromData());

        return 1;
    }
    
    /// Expected ituff
    /// 2_tname_[TP instance name]
    /// 2_msunit_SPMV1://{IP}_:/{FREQ}%^/[CORE]@F00V//
    /// 2_strgval_IA:4.300%1.095V1.111^3.700%0.973V0.942_IAX2:4.300%1.115V1.132%3.700%1V1_CLM:2.400%0.793
}
```

## Version Tracking

| **Date**     | **Version** | **Author**   | **Comments**               |
| ------------ | ----------- | ------------ | -------------------------- |
| Mar 20, 2022 | 1.0.0       | Nir Moses-Heller | Initial
| Mar 29, 2022 | 1.0.1       | Nir Moses-Heller | Update code examples|
|              |             |              |                            |

## Contacts

| Contact | Role |
| ------- | ---- |
| Nir Moses-Heller | Prime StringParsedMetadataHelper class developer |
| Steve D Deome | MPE *SPM* main contact |
| Miroslav Dzakovic | *SPM* syntax/spec/MIDAS owner |