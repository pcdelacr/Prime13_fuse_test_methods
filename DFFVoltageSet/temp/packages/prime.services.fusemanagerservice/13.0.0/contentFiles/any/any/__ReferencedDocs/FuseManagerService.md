[[_TOC_]]

FuseManager Service is intended to give all the needs required to work with virtual fuses. This includes writing, reading fuses, generating fuses for FuseOverride (HCS & FDS) and more.

This service, arranges fuses into two logical levels. Higher level is the Namespace, and lower level within the namespace is the Tag. Fuses exists within a tag.

The namespace will define what register to use, the heirarchical index, HCS and FDS fuses names , fuse chassis and more... The users are responsible for the namespaces definitions which is given through an input .json file. Below in [Configuration files](#Configuration-files) section you can find more information.

Tags is decided by user during the API calls. Tag will be provided as a parameter to API function call when writing a fuse. And tags later can be used when reading fuses, or generating HCS and FDS.

The service utilize 2 different storages:
1. Dynamic **Primary Storage** (from here on known as primary storage) is the storage where all the fuses values and tags being stored. This storage will take precedence in all fuse data management activities when duplicates exists in different storage; the data in this storage is always considered as 'latest data'.
2. Static **Latched Storage** (from here on known as latched storage) is the storage where known good fuse values has been identified by users is stored. To store the fuse data into this storage, user has to call 'save' APIs. Once the value in this storage is stored, it will not get reset unless a DeviceStart is executed or the data is being overwritten by the same API.

When reading back the stored fuse data, generating the DFF string or the HCS and FDS, the data in both storage are combined and if there are duplicates, the data from primary storage will be taken overwriting the data in latched storage.

In the event where user found current fuse values are not good for use further down the execution, user can always call 'restore' APIs to discard the data in the primary storage and fill primary storage with the data from latched storage.

Please be advised, that Prime provides a complementary out-of-the-box test methods that can simplify some virtual fuse related functionaly, like generating HCS and FDS and writing them to shared storage, reseting fuses namespace, exporting/importing from dff and more. Check out test methods documentation for more information.

During this documentation you will find the [Prerequisites](#Prerequisites) of the current service, the format of [Configuration files](#Configuration-files) with an explanation of their fields and usages, also an extended overview of the methods provided by every [interface](#Service-Interfaces).

# Prerequisites

<p style="font-size: 20px;"><span style="color: #cf6679;">This Service makes use of Aleph initialization for parsing and validation of the configuration file. Please refer to Aleph documentation for details.</span></p>

[Link to Aleph Documentation](/[Internal%2DOnly]-Prime-Developers-Wiki/Wiki-master/General/Special-ENV-Variables-in-Prime)

## Flame
Flame team is the team responsible to define everything related to fuses, like the structure of the fuse, what fuses to allocate, to what teams and etc.

This Prime service uses a Nuget from Flame team to calculate the FDS and HCS based on the requested chassis and other attributes (which is decided by FlameKey and FlameFile. See below).

# Configuration files
Fuse input file.

This file is consumed by Aleph, see [ALEPH Files](https://dev.azure.com/mit-us/PRIME/_wiki/wikis/PRIME.wiki/181/Special-ENV-Variables-in-Prime?anchor=aleph_files) for more information on setup.

Format file name: {customer file name}*.vfconfigs.json*
Example file:
```json
{
  "VirtualFuseConfigs": [
    {
      "Namespace": [ "U1", "U2", "U3", "U4" ],
      "FlameKey": "CPU_GRANITE_RAPIDS_COMPUTE_DIE_M",
      "FlameFile": "~HDMT_TPL_DIR\\Modules\\FUSEMANAGER\\FUSEMANAGER\\InputFiles\\PrimeMetaData\\RegisterDetails.json",
      "FusesWithEnableBit": [
        {
          "FuseName": "MLC_ROW1",
          "EnableBitFuseName": "MLC_ROW1_EN",
          "EnableBinaryValue": "1"
        },
        {
          "FuseName": "fuse/fuse_name2",
          "EnableBitFuseName": "fuse/fuse_enable_bit_name2",
          "EnableBinaryValue": "1"
        }
      ]
    },
    {
      "Namespace": [ "PCH" ],
      "FlameFile": "~HDMT_TPL_DIR\\Modules\\FUSEMANAGER\\FUSEMANAGER\\InputFiles\\PrimeMetaData\\RegisterDetails.json",
      "FlameKey": "CPU_GRANITE_RAPIDS_IO_DIE"
    }
  ]
}
```
Field details:
- _**Namespace**_: Namespaces that users can use to arrange their fuses. Namespaces under this defined object will be associated with the same FlameKey, FlameFile & FusesWithEnableBit.
- _**FlameKey**_: This is Flame key that contains the configuration that the user want to use. This flame key is defined by Flame, in the FlameFile.
- _**FlameFile**_: Flame file (called by Flame usually RegisterDetails.json files and usually provided by Flame) It defines for each key, important attributes like fuse chassis version, descriptor version, gap size, port id and more.
- _**FusesWithEnableBit**_: This section defines enable bit fuses. Note that these fuses can be utilized automatically or manually when user write to the associated fuse. More about this can be found in the APIs below.
  - _**FuseName**_: The associated fuse name. When user chooses the service to hadle the enable bits, each time this fuse is written, the enable bit fuse will be written with 'EnableBinaryValue'.
  - _**EnableBitFuseName**_: The name of the enable bit fuse.
  - _**EnableBinaryValue**_: The value that tells whether this fuse is enabled or not.

Note:
> RegisterDetails.json file is a file provided by Flame. Users need to copy it and use it in their TP.
>> This file can point to another files like Vfmetadata_<DieId>.json & Dummydescriptor_<DieId>.json which also should be provided by Flame.
> Basically in Prime, to simplify the usage for users, they are only controlling namespace names, tags and enable fuses, other configs come from Flame files.

# Service Interfaces
Fuse service give the user ability to get handle to namespace, store/read fuses and values, arrange them by needed tags, and ability to generate HCS and FDS.
Moreover, service allows uers to import all fuses and data of a namespace from DFF.

## Classes and structs
### VirtualFuseData
``` csharp
public struct VirtualFuseData
{
  public VirtualFuse HeapControlSection { get; }
  public VirtualFuse FuseDataSection { get; }
}
```
```HeapControlSection``` contains the value and the breakdowns by bit sections of the descriptor (HCS).
<br>```FuseDataSection``` contains the value and the breakdowns per fuses (FDS).

### VirtualFuse
```csharp
public class VirtualFuse
{
  public string Value { get; set; }
  public List<ValueDetails> ValueDetails { get; set; }
}
```

```Value``` is a binary string of the generated value. This might be the HCS or FDS.
<br>```ValueDetails``` is the bit breakdown of the value. In HCS, it is broken down into sections/fields. In FDS, it is broken down into their respective fuses.

### ValueDetails
```csharp
public class ValueDetails
{
  public string Name { get; set; }
  public uint Start { get; set; }
  public uint End { get; set; }
}
```

```Name``` is the name of the details. In HCS, this is the section/field name. In FDS, this is the fuse name.
<br>```Start``` is the bit number where the section/field/fuse start in ```Value```.
<br>```End``` is the bit number where the section/field/fuse end in ```Value```.


## IFuseService interface
This interface allows user to get handle to the fuses namespace.

### GetVirtualFuseHandler interface
|IVirtualFuseHandler GetVirtualFuseHandler(string fusesNamespace)|
|------------------------|

The _**GetVirtualFuseHandler**_ method returns a handler for 'fuseNamespace' to allow user interaction with fuses and values.

#### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_ |
|----------------------|-------------|-------------------|
| fusesNamespace | string | name of fuses namespace. |


#### Return Value
Handler of fuses namespace ('fuseNamespace').

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)

IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
```

## IVirtualFuseHandler interface
This interface provides direct access to fuses namespace to allow user to store/read values, arrange into tags and generate HCS and FDS.

### StoreVirtualFuse interface
|void StoreVirtualFuse(Dictionary<string, string> fuseNamesAndValues, string tag);|
|------------------------|

The _**StoreVirtualFuse**_ this method receives dictionary of fuses names and values, insert them on the given 'tag'. Note that we are already in a specific namespace that our handler point to. (See above.) All associated enabale bit fuses will also be written with their enable value (EnableBinaryValue).

#### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_ |
|----------------------|-------------|-------------------|
| fuseNamesAndValues | Dictionary<string, string> | Ditionary of fuse name to value. |
| tag | string | Tag to use.|

#### Return Value
None.

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)

IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);

Dictionary<string, string> fusesAndValues = new Dictionary<string, string>();
(... do some specific methodology calculation and populate fusesAndValues ...)

virtualFuseHandler.StoreVirtualFuse(fusesAndValues, "MLC);
```

### ReadVirtualFusesForSpecificTag interface
| Dictionary<string, string> ReadVirtualFusesForSpecificTag(string tag);|
|------------------------|

The _**ReadVirtualFusesForSpecificTag**_ reads all fuses and data for a given 'tag'. This includes the enable bits defined in this namespace handler.

#### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_ |
|----------------------|-------------|-------------------|
| tag | string | Tag to use.|

#### Return Value
Returns dictionary of fuses names and values.

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)

IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
Dictionary<string, string> fusesAndValues = virtualFuseHandler.ReadVirtualFusesForSpecificTag("MLC");
```

### ReadVirtualFuseWithoutEnableBitsForSpecificTag interface
|Dictionary<string, string> ReadVirtualFuseWithoutEnableBitsForSpecificTag(string tag);|
|------------------------|

The _**ReadVirtualFuseWithoutEnableBitsForSpecificTag**_ reads all fuses and data for a given 'tag'. This method will not include the enable bit fuses of this current namespace.
#### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_ |
|----------------------|-------------|-------------------|
| tag | string | Tag to use.|

#### Return Value
Returns dictionary of fuses names and values.

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)

IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
Dictionary<string, string> fusesAndValues = virtualFuseHandler.ReadVirtualFuseWithoutEnableBitsForSpecificTag("MLC");
```

### ReadAllVirtualFuse interface
|Dictionary<string, string> ReadAllVirtualFuse();|
|------------------------|

The _**ReadAllVirtualFuse**_ reads all fuses and data of all tags exist under curent namespace including the enable bit fuses.

#### Return Value
Returns dictionary of fuses names and values.

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)

IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
Dictionary<string, string> fusesAndValues = virtualFuseHandler.ReadAllVirtualFuse();
```

### ReadAllVirtualFuseWithoutEnableBits interface
|Dictionary<string, string> ReadAllVirtualFuseWithoutEnableBits();|
|------------------------|

The _**ReadAllVirtualFuseWithoutEnableBits**_ reads all fuses and data of all tags exist under curent namespace without the enable bit fuses.

#### Return Value
Returns dictionary of fuses names and values.

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)

IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
Dictionary<string, string> fusesAndValues = virtualFuseHandler.ReadAllVirtualFuseWithoutEnableBits();
```

### GenerateVirtualFuseWithoutStoredData interface
|VirtualFuseData GenerateVirtualFuseWithoutStoredData(Dictionary<string, string> fusesNamesAndValues);|
|------------------------|

The _**GenerateVirtualFuseWithoutStoredData**_ generates HCS and FDS of the given fuses name and values without taking into considerations other fuses in the namespace; only the ones passed in the argument.

If method is called with empty dictionary, the interface will return default descriptor in the VirtualFuseData.HeapControlSection.

#### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_ |
|----------------------|-------------|-------------------|
| fusesNamesAndValues | Dictionary<string, string> | fuses names and values to generate HCS and FDS for.|

#### Return Value
Returns VirtualFuseData object (which contains VirtualFuseData object that holds the descriptor and fuse data).

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)

IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
Dictionary<string, string> fusesAndValues = virtualFuseHandler.ReadVirtualFusesForSpecificTag("MLC");
VirtualFuseData generatedFuse = virtualFuseHandler.GenerateVirtualFuseWithoutStoredData(fusesAndValues);
Prime.Services.Console.PrintDebug($"HCS={generatedFuse.HeapControlSection.Value} FDS={generatedFuse.FuseDataSection.Value}");
```

### GenerateVirtualFuseWithTaggedData interface
|VirtualFuseData GenerateVirtualFuseWithTaggedData(List<string> tags, Dictionary<string, string> extraFuses);|
|------------------------|

The _**GenerateVirtualFuseWithTaggedData**_ generates HCS and FDS of the given fuses name and values ('extraFuses'), taking into considerations other fuses in the namespace under the given tags ('tags' argument).

If none of the tags specified exists in the service storage, the interface will return default descriptor in the VirtualFuseData.HeapControlSection.

If any of specified tags does not exists in the service storage, the service will not generate the fuse for the non-existent tags. Remaining tags that can be found in service storage will be generated as usual.

#### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_ |
|----------------------|-------------|-------------------|
| tags | List<string> | lise of tags to use to pull fuses data from (in addition to the extraFuses).|
| extraFuses | Dictionary<string, string> | fuses names and values to generate HCS and FDS for (in addition to the existing ones in the namespace)|

#### Return Value
Returns VirtualFuseData object (which contains VirtualFuseData object that holds the descriptor and fuse data).

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)
List<string> tags = new List<string>() { "LLC" };
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
Dictionary<string, string> fusesAndValues = virtualFuseHandler.ReadVirtualFusesForSpecificTag("MLC");
VirtualFuseData generatedFuse = virtualFuseHandler.GenerateVirtualFuseWithTaggedData(tags, fusesAndValues);
Prime.Services.Console.PrintDebug($"HCS={generatedFuse.HeapControlSection.Value} FDS={generatedFuse.FuseDataSection.Value}");
```

### GenerateVirtualFuseWithTaggedData interface
|VirtualFuseData GenerateVirtualFuseWithTaggedData(List<string> tags);|
|------------------------|

The _**GenerateVirtualFuseWithTaggedData**_ generates HCS and FDS of the fuses in the namespace under the given tags ('tags' argument).

If none of the tags specified exists in the service storage, the interface will return default descriptor in the VirtualFuseData.HeapControlSection.

If any of specified tags does not exists in the service storage, the service will not generate the fuse for the non-existent tags. Remaining tags that can be found in service storage will be generated as usual.

#### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_ |
|----------------------|-------------|-------------------|
| tags | List<string> | lise of tags to use to pull fuses data from (in addition to the extraFuses).|

#### Return Value
Returns VirtualFuseData object (which contains VirtualFuseData object that holds the descriptor and fuse data).

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)
List<string> tags = new List<string>() { "LLC", "MLC"};
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
VirtualFuseData generatedFuse = virtualFuseHandler.GenerateVirtualFuseWithTaggedData(tags);
Prime.Services.Console.PrintDebug($"HCS={generatedFuse.HeapControlSection.Value} FDS={generatedFuse.FuseDataSection.Value}");
```

### GenerateVirtualFuseWithAllData interface
|VirtualFuseData GenerateVirtualFuseWithAllData(Dictionary<string, string> extraFuses)|
|------------------------|

The _**GenerateVirtualFuseWithAllData**_ generates HCS and FDS of the fuses in the namespace under the given tags ('tags' argument) in addition to all other fuses data of all tags under the namespace.

If the service storage does not contain any fuses and no extra fuses are given, the interface will return default descriptor in the VirtualFuseData.HeapControlSection.

#### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_ |
|----------------------|-------------|-------------------|
| extraFuses | Dictionary<string, string> | fuses names and values to generate HCS and FDS for (in addition to the existing ones in the namespace)|

#### Return Value
Returns VirtualFuseData object (which contains VirtualFuseData object that holds the descriptor and fuse data).

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
Dictionary<string, string> fusesAndValues = virtualFuseHandler.ReadVirtualFusesForSpecificTag("MLC");
VirtualFuseData generatedFuse = virtualFuseHandler.GenerateVirtualFuseWithAllData(fusesAndValues);
Prime.Services.Console.PrintDebug($"HCS={generatedFuse.HeapControlSection.Value} FDS={generatedFuse.FuseDataSection.Value}");
```

### GenerateVirtualFuseWithAllData interface
|VirtualFuseData GenerateVirtualFuseWithAllData();|
|------------------------|

The _**GenerateVirtualFuseWithAllData**_ generates HCS and FDS of the fuses under all tags in the namespace.

If the service storage does not contain any fuses, the interface will return default descriptor in the VirtualFuseData.HeapControlSection.

#### Return Value
Returns VirtualFuseData object (which contains VirtualFuseData object that holds the descriptor and fuse data).

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
VirtualFuseData generatedFuse = virtualFuseHandler.GenerateVirtualFuseWithAllData();
Prime.Services.Console.PrintDebug($"HCS={generatedFuse.HeapControlSection.Value} FDS={generatedFuse.FuseDataSection.Value}");
```

### GenerateVirtualFuseDffString interface
|string GenerateVirtualFuseDffString();|
|------------------------|

The _**GenerateVirtualFuseDffString**_ generates string that contains all tags, fuses names and values of the current namespace, to be exported later to dff. (used by VirtualFuseExportToDff test method).

#### Return Value
string of all data in the namespace (tags, fuses names and values) in the following format: "TAG1^FUSE11:VAL&amp;FUSE12:VAL|TAG2^FUSE21:VAL&amp;FUSE22:VAL.."

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
string stringToExportToDFF = virtualFuseHandler.GenerateVirtualFuseDffString();
(.. user can export stringToExportToDFF to a dff ... but usually it's handled by VirtualFuse* test methods ..)
```

### UpdateFusesDataFromDffString interface
|void UpdateFusesDataFromDffString(string stringFromDff);|
|------------------------|

The _**UpdateFusesDataFromDffString**_ reads string from DFF, decodes it and update the data of the current namespace (with all tags and fuses).

#### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_ |
|----------------------|-------------|-------------------|
| stringFromDff | string | string in this format ("TAG1^FUSE11:VAL&amp;FUSE12:VAL|TAG2^FUSE21:VAL&amp;FUSE22:VAL..")|

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
string stringFromDFF = (.. read from dff ..)
virtualFuseHandler.UpdateFusesDataFromDffString(stringFromDFF);
```

### ResetVirtualFuseForTag interface
|void ResetVirtualFuseForTag(string tag);|
|------------------------|

The _**ResetVirtualFuseForTag**_ clears all fuse under given 'tag'.

#### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_ |
|----------------------|-------------|-------------------|
| tag | string | tag to clear all fuses under.|

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
virtualFuseHandler.ResetVirtualFuseForTag("MLC");
```

### ResetAllVirtualFuse interface
|void ResetAllVirtualFuse();|
|------------------------|

The _**ResetAllVirtualFuse**_ clears all fuse under current namespace.

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
virtualFuseHandler.ResetAllVirtualFuse();
```

### TagExistsInNamespace interface
|bool TagExistsInNamespace(string tag);|
|------------------------|

The _**TagExistsInNamespace**_ determines if tag has already been populated for the current namespace

#### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_ |
|----------------------|-------------|-------------------|
| tag | string | tag to be found.|


#### Return Value

Flag value set to TRUE when tag was found in the virtual fuse handler, FALSE when tag does not exist within the virtual fuse handler

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
bool doesTagExists = virtualFuseHandler.TagExistsInNamespace("MLC");
```

### SaveCurrentFuseStorage interface
| void SaveCurrentFuseStorage(); |
|--------------------------------|

The _**SaveCurrentFuseStorage**_ save the fuse data into latch storage.


#### Return Value
None

#### Usage Example

``` csharp
using Prime.FuseManagerService;

(...)

(...)
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
virtualFuseHandler.SaveCurrentFuseStorage();
```

### SaveCurrentFuseStorageForTag interface
| void SaveCurrentFuseStorageForTag(string tag); |
|------------------------------------------------|

The _**SaveCurrentFuseStorageForTag**_ will save the fuse data that was tagged with *tag* into the latch storage.

#### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_                                                      |
|----------------------|-------------|------------------------------------------------------------------------|
| tag | string | Name that the fuse data is tagged to will be saved into latch storage. |

#### Return Value
None

#### Usage Example
```csharp
using Prime.FuseManagerService;

(...)
    
(...)
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
virtualFuseHandler.SaveCurrentFuseStorageForTag("MLC");

```

### RestoreSavedFuses interface
| void RestoreSavedFuses(); |
|---------------------------|

The _**RestoreSavedFuses**_ will discard the fuse data stored in primary storage and restore the fuse data in latch storage back into primary storage. Fuse data in latch storage will not be removed.

#### Return Value
None

#### Usage Example
```csharp
using Prime.FuseManagerService;

(...)
    
(...)
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
virtualFuseHandler.RestoreSavedFuses();
```


### RestoreSavedFusesForTag(string tag);
| void RestoreSavedFusesForTag(string tag); |
|-------------------------------------------|

The _**RestoreSavedFusesForTag**_ will discard the fuse data that is tagged to *tag* stored in primary storage and restore the fuse data in latch storage back into primary storage. Fuse data in latch storage will not be removed.

#### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_                                                       |
|----------------------|-------------|-------------------------------------------------------------------------|
| tag | string | Name that the fuse data is tagged to will be saved into latch storage.  |

#### Return Value
None

#### Usage Example
```csharp
using Prime.FuseManagerService;

(...)
    
(...)
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace);
virtualFuseHandler.RestoreSavedFusesForTag("MLC");
```

### SetHandlerToGenerateFuseDataMap()
| IVirtualFuseHandler SetHandlerToGenerateFuseMapData() |
|-------------------------------------------------------|

The _**SetHandlerToGenerateFuseMapData**_ will set the handler to generate FDS mapping in IVirtualFuseData. The data will details out the location of the fuse values in the generated FDS.

#### Return Value
The handler object instance.

#### Usage Example

```csharp
using Prime.FuseManagerService;

(...)
    
IVirtualFuseHandler virtualFuseHandler = Base.ServiceStore<IFuseManagerService>.Service.GetVirtualFuseHandler(this.Namespace).SetHandlerToGenerateFuseDataMap();
IVirtualFuseData virtualFuseData = virtualFuseHandler.GenerateVirtualFuseWithAllData();
foreach (var fuseDataMap in virtualFuseData.FuseDataMap)
{
    Base.ServiceStore<IConsoleService>.Service.PrintDebug(() => $"Fuse: {fuseDataMap.FuseName} | Range: {fuseDataMap.StartBit} - {fuseDataMap.EndBit}");
}

// Example Output:
// Fuse: Fuse1 | Range: 0 - 4
// Fuse: Fuse2 | Range: 5 - 9
```

# Glossary

| Abbreviation | Meaning |
| ----- | ----- |
| API | Application Programming Interface |
| DFF | Data Feed-Forward |
| FDS | Fuse Data Section |
| HCS | Heap Control Section |