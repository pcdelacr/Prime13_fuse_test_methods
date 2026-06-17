[[_TOC_]]

# Prerequisites

This Service makes use of Aleph initialization for parsing and validation of the configuration file. Please refer to [Aleph documentation](https://dev.azure.com/mit-us/PrimeWiki/_wiki/wikis/PrimeWiki.wiki/28879/Special-ENV-Variables-in-Prime) for details.

# Defeature Tracking Service
The _**Defeature Tracking Service**_ provides the necessary infrastructure to track whether specific elements of a die are featured/defeatured for the purposes of recovery methodologies. This is done via the creation of tracking structures which contain elements for which their featured/defeatured state will be tracked. Tracked elements will have a default value of "featured" ("false" or "0") and can then be moved to a "defeatured" ("true" or "1") state as needed. Any element that has already been "defeatured" cannot be "re-featured".

The data for all tracking structures and their elements is stored into Prime's Shared Storage in order to guarantee access from any test instance. Tracking Structure data is treated as unique to each DUT in a test program LOT and will be discarded from the Shared Storage memory once the DUT testing is finished.

## Structure dependency
This feature introduces a mechanism to link two structures, a High-Level Structure and a Low-Level Structure (has more or equal number of elements than the high-level structure), in a way that the updates to one structure may affect the other.

When an element in the High-Level Structure is defeatured (set to true), all the dependent elements in the Low-Level Structure also get defeatured. On the other hand, when elements in the Low-Level Structure are updated, the corresponding element in the High-Level Structure is defeatured only if all its dependent elements in the Low-Level Structure are defeatured. If not all dependent elements are defeatured, the High-Level Structure element remains unchanged.

The structure dependency works only in one way, meaning that it can be used to defeature dependent elements, but if an element is already defeatured, the dependency cannot be used to set that element back to featured.

**Important:** Even when having dependent structures, both of them must be initialized. This means the users needs to use the `InitializeElementValues` API with both structures before they can be updated. The `InitializeElementValues` API takes into account the dependencies between structures, so when initializing a structure that has dependencies, the method will also initialize the dependent structure. The dependent structure must be initialized as well in order to set the original values for that structure and to match the current values accordingly.

To use this feature, the section "Dependencies" must be added to the configuration file, check an example in [Configuration file: dependency example](#configuration-file-dependency-example).

The feature will be used through the APIs: [InitializeElementValues](#initializeelementvalues), [UpdateElementValue](#updateelementvalue) and [UpdateElementValues](#updateelementvalues), check each API documentation for more details and examples on how it works.

### Multi-tier Dependencies
You can now define the dependencies in multi-tier.

Consider 3 structures below:-<br>
```
Structure A [ A1, A2 ]
Structure B [ B1, B2, B3, B4 ]
Structure C [ C1, C2, C3, C4, C5, C6, C7, C8 ]
Links [[ A1, A4 ]]
Dependency {
    HighLevelStructure = Structure A
    HighLevelElement = A1
    LowLevelStructure = Structure B
    LowLevelElements = [ B1, B2 ]
},
{
    HighLevelStructure = Structure B
    HighLevelElement = B1
    LowLevelStructure = Structure C
    LowLevelElements = [ C2, C2 ]
}
.
.
.
.
```
The dependency structure is now as follows

::: mermaid
block-beta
columns 3
    block:A(("StructA"))
        A1
        space
        A2
    end
    space:5
    block:B(("StructB"))
        B1
        space
        B2
        space
        B3
        space
        B4        
    end   
    space:5
    block:C(("StructC"))
        C1
        space
        C2
        space
        C3
        space
        C4
        space:2
        C5
        space
        C6
        space
        C7
        space
        C8          
    end     
    A1 --> B1
    A1 --> B2
    A2 --> B3
    A2 --> B4
    B1 --> C1
    B1 --> C2
    B2 --> C3
    B2 --> C4
    B3 --> C5
    B3 --> C6
    B4 --> C7
    B4 --> C8
:::

Editor's Note:<br>
If the mermaid diagram is not displayed above due to different mermaid versions being used, the diagram looks like below.
<br>![Structure Block Diagram](.attachment\StructureBlockDiagram.png)

<br>**IMPORTANT**<br>
Few things that you'll need to consider.
- The longer the chain, the longer it will take to propagate the data.
    - `A -> B -> C -> D` is will be updated longer than `A -> B -> C`.
- Be careful not to configure a looped dependency. This will result in init failure.
    - `A -> B -> A` is not allowed as the data propagation will result in infinite loop.

## IntraStructure Link
This feature introduces a mechanism to link elements from the same structure together. When an element within the group is defeatured, the code will defeature all the elements in the group.

Consider a structure with 4 elements:-<br>
```
Structure A [ E1, E2, E3, E4]
Links [[ E1, E4 ]]
```

If `E1` is defeatured, `E4` will be defeatured together and vice versa.

IntraStructure Link usage can be used in conjunction with Dependency. When an element linked to a another element with dependency, the dependency will be updated accordingly as described above.

Consider 2 structures below:-<br>
```
Structure A [ A1, A2, A3, A4]
Structure B [ B1, B2]
Links [[ A1, A4 ]]
Dependency {
    HighLevelStructure = Structure B
    HighLevelElement = B1
    LowLevelStructure = Structure A
    LowLevelElements = [ A1, A2 ]
},
{
    HighLevelStructure = Structure B
    HighLevelElement = B2
    LowLevelStructure = Structure A
    LowLevelElements = [ A3, A4 ]
}
```

Let's say current value of `Structure A` is `0100` (`A2` defeatured) and `Structure B` is `00`. When `A4` is defeatured, `A1` is also defeatured. Now `A1` and `A2` are both defeatured and they have dependency on `B1`, `Structure B` will now be `10`.

To use this feature, the ```TrackingStructure``` should have property ```Links``` added, check an example in [Configuration File: IntraStructure Link](#configuration-file-intrastructure-link-example).

## Data Propagation

If ever that you need the option to determine how the data propagated between high level and low level, you can do so by including `Propagation` under the dependencies that you want to control how the data propagate. You'll have 3 valid options; `BothWays`, `HighToLow`, `LowToHigh`.

| Options | Description |
| -- | -- |
| BothWays | This option will propagate the data both ways, from high level to low level and low level to high level. |
| HighToLow | This option will only propagate the data to low level if you update the high level structure. If you update low level structure, the data won't sync-up with high level structure. |
| LowToHigh | This option will only propagate the data to high level if you update the low level structure. If you update high level structure, the data won't sync-up with low level structure.

**IMPORTANT!**<br>
When an element is dependent on each other, it should not act independently. User is responsible to ensure that this setting doesn't cause quality issue.

## Ituff Printing

You can now ask the service to print to ituff whenever an update to the data occurs by calling 4 interfaces in ITrackingStructureHandler.

```csharp
void UpdateElementValueWithItuffPrint(string elementName, bool value);
void UpdateElementValuesWithItuffPrint(BitArray values);
void UpdateElementValueWithStructureItuffPrint(string elementName, bool value);
void UpdateElementValuesWithStructureItuffPrint(BitArray values);
```

By updating the defeatured tracking data using these 4 interfaces, the service will print to ituff. They have 2 different formats.

### Structure Level Ituff Print
This format is intended for printing in a SORT TP, however there's no hard restriction is placed for users to print in this format. This prints will print in `strgval` format and the values printed are for the whole structure.
The interface to use when updating the structure to get this print format are:
```csharp
void UpdateElementValueWithStructureItuffPrint(string elementName, bool value);
void UpdateElementValuesWithStructureItuffPrint(BitArray values);
```

Format:
```
2_tname_<instanceName>::<structureName>
2_strgval_TestResult:b<vectors>|Incoming:b<vectors>|Outgoing:b<vectors>
```

The printing will repeat for of all the dependency structures. However, the dependency structure will not print the `TestResult` and `Incoming` values. The values will always be printed as `0` for these 2 fields.

Sample:
```
2_tname_DisableTwoElementsOutOfFour_P1::StructA
2_strgval_TestResult:b1100|Incoming:b0000|Outgoing:b1100
2_tname_DisableTwoElementsOutOfFour_P1::StructA
2_strgval_TestResult:b0000|Incoming:b0000|Outgoing:b1100
```

### Structure Group Level Ituff Print
This format is intended for printing in a CLASS TP, however there's no hard restriction is placed for users to print in this format. This prints will print in `strgval` format and the values printed are per structure group defined with `tssid` token.
The interface to use when updating the structure to get this print format are:

```csharp
void UpdateElementValueWithItuffPrint(string elementName, bool value);
void UpdateElementValuesWithItuffPrint(BitArray values);
```

When printing in this format, the structure group name will be used as the value for `2_tssid_` token. It is user's responsibility to ensure that the structure group name is a valid SSID and the token is limited to 9 characters. The format will be as follows:

Format:
```
2_tname_<instanceName>::<structureName>
2_tssid_<structureGroup>
2_strgval_TestResult:b<vectors>|Incoming:b<vectors>|Outgoing:b<vectors>
```

Sample:
```
2_tname_DisableTwoElementsOutOfFour_P1::StructA
2_tssid_GroupA1
2_strgval_TestResults:b11|Incoming:b00|Outgoing:b11
2_tname_DisableTwoElementsOutOfFour_P1::StructA
2_tssid_GroupA2
2_strgval_TestResults:b00|Incoming:b00|Outgoing:b00
2_tname_DisableTwoElementsOutOfFour_P1::StructB
2_tssid_GroupB1
2_strgval_TestResults:b1100|Incoming:b0000|Outgoing:b1100
```

However, there's limitation to the ituff print-out.

### Dependency Limitation
When there's dependency, the service doesn't have information on what's the `TestResults` for the dependent structures since they are not tested directly but the update happens due to 'after effect'. As such, this information will always be printed as `0`.

Consider the 2 structures as follows:
```
StructA [ GroupA1 { EA1, EA2 } ] [ GroupA2 { EA3, EA4 } ]
StructB [ GroupB1 { EB1, EB2, EB3, EB4 } ]

HighLevelStructure : StructA
HighLevelElement: EA1
LowLevelStructure: StructB
LowLevelElements: [ EB1, EB2 ]
```

#### Structure Print Format (SORT)
```
2_tname_DisableTwoElementsOutOfFour_P1::StructA
2_strgval_TestResults:b1100|Incoming:b0000|Outgoing:b1100
2_tname_DisableTwoElementsOutOfFour_P1::StructB
2_strgval_TestResults:b0000|Incoming:b1000|Outgoing:b1100
```

#### Structure Group Print Format (CLASS)
```
2_tname_DisableTwoElementsOutOfFour_P1::StructA
2_tssid_GroupA1
2_strgval_TestResults:b11|Incoming:b00|Outgoing:b11
2_tname_DisableTwoElementsOutOfFour_P1::StructA
2_tssid_GroupA2
2_strgval_TestResults:b00|Incoming:b00|Outgoing:b00
2_tname_DisableTwoElementsOutOfFour_P1::StructB
2_tssid_GroupB1
2_strgval_TestResults:b0000|Incoming:b1000|Outgoing:b1100
```

### Structure Name limitation
When printing to ituff, Structure Group Name will be used as the value for `2_tssid_` token. As such, user is responsible to ensure that the name is referring to a valid SSID.
The service itself will not validate the name to be a valid ssid. And to comply with `2_tssid_` token requirement, the token will be truncated to only 9 characters.

A valid SSID in current implementation is `Ux.Uy` which is a 9 character string.

## Dependency Rule Evaluation
You can evaluate dependent structures against the specific rules as required. The service introduced new interface to specify what rules to evaluate against which dependent structure.

`void PopulateRuleForDependent(string dependentStructureName, string dependentRuleName)`

Example:
Consider the 2 structures as follows:
```
StructA [ GroupA1 { EA1, EA2 } ] [ GroupA2 { EA3, EA4 } ]
StructB [ GroupB1 { EB1, EB2, EB3, EB4 } ]

HighLevelStructure : StructA
HighLevelElement: EA1
LowLevelStructure: StructB
LowLevelElements: [ EB1, EB2 ]
```

You'll want to make sure `StructB` always have minimum quantity of 2 elements being enabled so you defined the rule:
```json
"Rules": [
{
    "Name": "MinQty2"
    "MinQuantity": {
    "type": "Literal",
    "value": 2
    }
},
```

You'll execute it as follows:
```csharp
var trackingStructureHandler = Prime.Base.ServiceStore<IDefeatureTrackingService>.Service.CreateTrackingStructureHandler("StructA", this.SessionContext);
trackingStructureHandler.PopulateRuleForDependent("StructB", "MinQty2");

bool[] value = { true, false, false, false };
var ruleEvaluationResult = trackingStructureHandler.EvaluateRule(new BitArray(value));
if (!ruleEvaluationResult)
{
    throw new TestMethodException("Rule evaluation failed. We might have less than 2 enabled elements on the dependent structures.");
}
```

# Configuration File
The json configuration file, which is parsed and validated by Aleph during Init flow, is the one responsible for defining the pool of elements to track as well as the grouping of elements per tracking structure.

Configuration file has split into 2 files in Prime v10.0, mainly to separate validCombinations section into other file. Hence, configuration files that are expected to parse by Aleph are:

- defeaturetracking.json file
- validcombinations.json file

Multiple validcombinations files are allowed.

## Naming Convention
The filename of the configuration files must follow the following convention:

- `{custom file name}.defeaturetracking.json`
- `{custom file name}.validcombinations.json`

## Configuration File Example
The following is an example of a simple DefeatureTracking configuration file:

| basic_example.defeaturetracking.json |
| ------------------------------------ |

```json
{
  "Elements" : [
    "CORE0",
    "CORE1",
    "CORE2",
    "CORE3",
    "CORE4",
    "CORE5",
    "CORE6",
    "CORE7",
    "CORE8",
    "CORE9"
  ],
  "TrackingStructures" : [
    {
      "StructureNames": [ "Struct0", "Struct1" ],
      "StructureGroups": [
        {
          "Name": "Die0",
          "TrackingElements": [ "CORE0", "CORE1" ]
        },
        {
          "Name": "Die1",
          "TrackingElements": [ "CORE2", "CORE3" ]
        }
      ]
    },
    {
      "StructureNames": [ "Struct2" ],
      "StructureGroups": [
        {
          "Name": "SubStruct2",
          "TrackingElements": [
            "CORE4",
            "CORE5",
            "CORE6",
            "CORE7"
          ]
        }
      ]
    }
  ],
  "Rules": [
    {
      "Name": "Rule1",
      "Single-Fail": {
        "type": "Literal",
        "value": false
      },
      "MinQuantity": {
        "type": "Literal",
        "value": 2
      },
      "ValidCombination": {
        "type": "Literal",
        "value": [ "ValidComboSet3", "ValidComboSet3.1" ],
        "combo_results" :  "00"
      }
    },
    {
      "Name": "StructureGroupRule1",
      "Single-Fail": {
        "type": "Literal",
        "value": false
      },
      "MinQuantity": {
        "type": "Literal",
        "value": 1
      },
      "ValidCombination": {
        "type": "Literal",
        "value": [ "ValidComboSet3" ]
      }
    },  
    {
      "Name": "Rule2",
      "Single-Fail": {
        "type": "UserVar",
        "value": "DefeatureRulesVars.SingleFail"
      },
      "MinQuantity": {
        "type": "UserVar",
        "value": "DefeatureRulesVars.MinQuantity"
      },
      "ValidCombination": {
        "type": "UserVar",
        "value": [ "DefeatureRulesVars.ValidCombination" ]
      }
    }
  ]
}

```

## Configuration File Dependency Example
The following example shows how to add the **"Dependencies"** section to the DefeatureTracking configuration file (the Rules section is being omitted on the example by adding "...", this is just to emphasize the Dependencies section, users must not remove the Rules section):

|dependency_example.defeaturetracking.json|
|--|
```json
{
  "Elements": [
    "Core00",
    "Core01",
    "Core02",
    "Core03",
    "Core10",
    "Core11",
    "Core12",
    "Core13",
    "Core20",
    "Core21",
    "Core22",
    "Core23",
    "Core30",
    "Core31",
    "Core32",
    "Core33",
    "StackDie0",
    "StackDie1",
    "StackDie2",
    "StackDie3"
  ],
  "TrackingStructures": [
    {
      "StructureNames": [ "CORETracking" ],
      "StructureGroups": [
        {
          "Name": "StackDie0Group",
          "TrackingElements": [ "Core00", "Core01", "Core02", "Core03", "Core10", "Core11", "Core12", "Core13"]
        },
        {
          "Name": "StackDie1Group",
          "TrackingElements": [ "Core20", "Core21", "Core22", "Core23", "Core30", "Core31", "Core32", "Core33"]
        }
      ]
    },
    {
      "StructureNames": [ "XBar" ],
      "StructureGroups": [
        {
          "Name": "BaseDie0",
          "TrackingElements": [ "StackDie0", "StackDie1"]
        },
        {
          "Name": "BaseDie1",
          "TrackingElements": [ "StackDie2", "StackDie3"]
        }
      ]
    }
  ],
  "Dependencies": [
    {
      "HighLevelStructure": "XBar",
      "HighLevelElement": "StackDie0",
      "LowLevelStructure": "CORETracking",
      "LowLevelElements": ["Core00", "Core01", "Core02", "Core03"]
    },
    {
      "HighLevelStructure": "XBar",
      "HighLevelElement": "StackDie1",
      "LowLevelStructure": "CORETracking",
      "LowLevelElements": ["Core10", "Core11", "Core12", "Core13"]
      "Propagation": "HighToLow"
    },
    {
      "HighLevelStructure": "XBar",
      "HighLevelElement": "StackDie2",
      "LowLevelStructure": "CORETracking",
      "LowLevelElements": ["Core20", "Core21", "Core22", "Core23"]
      "Propagation": "LowToHigh"
    },
    {
      "HighLevelStructure": "XBar",
      "HighLevelElement": "StackDie3",
      "LowLevelStructure": "CORETracking",
      "LowLevelElements": ["Core30", "Core31", "Core32", "Core33"]
      "Propagation": "BothWays"
    }
  ],
  "Rules": [
    {
    .
    .
    .
```

## Configuration File Intrastructure Link Example
```json
{
  "$schema": "DefeatureTrackingServiceConfiguration.schema.json",
  "Elements": [
    "Core00",
    "Core01",
    "Core10",
    "Core11",
    "Core20",
    "Core21",
    "Core30",
    "Core31"
  ],
  "TrackingStructures": [
    {
      "StructureNames": [ "CoreTracking" ],
      "StructureGroups": [
        {
          "Name": "Stack0Group",
          "TrackingElements": [ "Core00", "Core01", "Core10", "Core11" ]
        },
        {
          "Name": "Stack1Group",
          "TrackingElements": [ "Core20", "Core21", "Core30", "Core31" ]
        }
      ],
      "Links": [
        [ "Core10", "Core30" ],
        [ "Core00", "Core01" ]
      ]
    }
  ],
  "Rules": [
    {
      "Name": "Rule1",
      "Single-Fail": {
        "type": "Literal",
        "value": false
      }
    }
  ]
}
```

## Example of a ValidCombinations configuration file:

| configuration_name.validcombinations.json |
| ----------------------------------------- |

```json
{
  "$schema": "ValidCombinations.schema.json",
  "ValidCombinations": [
    {
      "Set": "ValidComboSet1",
      "Vector": [
        "0000",
        "1010",
        "0101"
      ]
    },
    {
      "Set": "ValidComboSet2",
      "Vector": [
        "00000",
        "01010"
      ]
    },
    {
      "Set": "ValidComboSet3",
      "Vector": [
        "00",
        "01"
      ]
    },
    {
      "Set": "ValidComboSet3.1",
      "Vector": [
        "00",
        "10"
      ]
    }
  ]
}
```

As the examples shows, defeatureTracking configuration file consist of three main sections which are required (1, 2 and 4), and an optional section (3):

1. "Elements": this first section defines the pool of elements that can be tracked among the tracking structures.
2. "TrackingStructures": this section specifies the names of the tracking structures and the elements that each one will be tracking. It has two mandatory subsections per each structure.
   - "StructureNames": The name of the tracking structure to create. If more than one name is given in this comma-separated list, then the service creates the listed structures with the same elements, but the structures will be functionally independent.
   - "StructureGroups": The groups of elements that's resides under the structure (sub-structure).
     - "Name": The name of the group.
     - "TrackingElements": The list of elements to group and track under this tracking structure. All the elements listed here must be found in the "Elements" section described above, otherwise the configuration file validation fails with an exception during Init.
3.  "Dependencies": This section is optional, it only needs to be added if the user needs to use the Dependency feature. When added, it defines which and how the structures are linked.
       - "HighLevelStructure": The name of the structure to be used as the highLevelStructure. This structure should be the same length or shorter than the lowLevelStructure (it should have equal or fewer elements), and it should be defined in the "TrackingStructures" section. Once a structure has been set as a highLevel it should not be set as a lowLevel structure in any other dependency.
       - "HighLevelElement": Name of the element from the highLevelStructure to be linked to other elements in the lowLevelStructure. This element should be defined as part of the corresponding structure in the "TrackingStructures" section. Once an element has been linked to a group of elements, it cannot be linked to other elements in a different dependency in the same file.
       - "LowLevelStructure": The name of the structure to be used as the lowLevelStructure. This structure should have equal or more elements than the highLevelStructure, and it should also be defined in the "TrackingStructures" section. Once a structure has been set as lowLevel it should not be set as a highLevel structure in any other dependency.
       - "LowLevelElements":  Array with the name of the elements from the lowLevelStructure to be linked to the highLevelStructure element. These element should be defined as part of the corresponding structure in the "TrackingStructures" section. Once these elements have been linked to an element, they cannot be linked to other elements in a different dependency in the same file.
4. "Rules": This section defines the of list rules that will be used for validation of the data being tracked.
   - "Name": The name of the rule set.
   - "Single-Fail": Option to enable single-fail check.
     - "type": Choice of "Literal" or "UserVar". Using "UserVar" will allow user to define the value as a UserVar and the actual data can be defined within the UserVar.
     - "value": When using "Literal" type, the option is a boolean true or false; if set to true, the tracking data will be compared against the original value  (per initialized) to not have mismatch.**Init will fail when value is not true or false.** When using "UserVar", the value should be a UserVar that contains the boolean value of true or false.
   - "MinQuantity": The minimum quantity of elements that the devices under tests needed to be featured. If the execution is based on TrackingStructures, the minimum quantity is for the whole TrackingStructures (summation of all groups' featured elements). If the execution is based on StructureGroup, the minimum quantity is only for the StructureGroup itself.
     - "type": Choice of "Literal" or "UserVar". Using "UserVar" will allow user to define the value as a UserVar and the actual data can be defined within the UserVar.
     - "value": When using "Literal" type, the value is any integer of minimum amount of elements needed to be featured. When using "UserVar", the value should be a UserVar that contains integer of minimum amount of elements needed to be featured.
   - "ValidCombination": The list of valid combination sets that's going to be used for validation of the data being tracked. If this section is defined using UserVar, defining more than 1 UserVar will cause configuration file validation fails during Init.
     - "type": Choice of "Literal" or "UserVar". Using "UserVar" will allow user to define the value as a UserVar and the actual data can be defined within the UserVar.
     - "value": When using "Literal" type, the value a list of ValidCombination sets. When using "UserVar", the value should be a list of one (1) UserVar that contains the value a list of ValidCombination sets. The ValidCombination sets must be defined in the "ValidCombinations" file or the rule will fail at PopulateRule().

ValidCombination configuration file on the other hand has one required section:

1. "ValidCombinations": This section defines the combination of featured/defeatured that is deemed as valid for the device under tests.
   - "Set": The name of the valid combinations sets.
   - "Vector": The list of featured/defeatured elements that is deemed as valid for the device under tests.

# Defeature Tracking Service Interface Methods

## CreateTrackingStructureHandler

| ITrackingStructureHandler CreateTrackingStructureHandler(string structureName) |
| ------------------------------------------------------------------------------ |

The _**CreateTrackingStructureHandler**_ method returns an interface to a "Tracking Structure Handler" which gives access to data operations that can be performed unto a specific tracking structure. Since the returned Tracking Structure Handler is linked to a fixed tracking structure, the need to provide the target structure name for each operation is eliminated.

### Parameters

| _**Parameter Name**_ | _**Type**_ | _**Description**_                                         |
| ---------------------------- | ------------------ | ----------------------------------------------------------------- |
| structureName                | string             | The name of the tracking structure for which a handler is needed. |

### Return Value
Tracking Structure Handler interface.

### Usage Example
The following C# code excerpt exemplifies the usage of the CreateTrackingStructureHandler method in order to get a handler to a tracking structure called "Struct1":

```csharp
using Prime.DefeatureTrackingService;

(...)

private ITrackingStructureHandler structHandler;

(...)

this.structHandler = Prime.Services.DefeatureTrackingService.CreateTrackingStructureHandler("Struct1");
```

## CreateTrackingStructureGroupHandler

| ITrackingStructureHandler CreateTrackingStructureGroupHandler(string structureName, string structureGroupName) |
| -------------------------------------------------------------------------------------------------------------- |

The _**CreateTrackingStructureHandler**_ method returns an interface to a "Tracking Structure Handler" which gives access to data operations that can be performed unto a specific structure group. Since the returned Tracking Structure Handler is linked to a fixed structure group, the need to provide the target structure name for each operation is eliminated.

### Parameters

| _**Parameter Name**_ | _**Type**_ | _**Description**_                                         |
| ---------------------------- | ------------------ | ----------------------------------------------------------------- |
| structureName                | string             | The name of the tracking structure for which a handler is needed. |
| structureGroupName           | string             | The name of the structure group for which a handler is needed.    |

### Return Value
Tracking Structure Handler interface.

### Usage Example
The following C# code excerpt exemplifies the usage of the CreateTrackingStructureGroupHandler method in order to get a handler to structure group "Die0" under tracking structure called "Struct1":

```csharp
using Prime.DefeatureTrackingService;

(...)

private ITrackingStructureHandler structHandler;

(...)

this.structHandler = Prime.Services.DefeatureTrackingService.CreateTrackingStructureGroupHandler("Struct1", "Die0");
```

## DoesRulesExists

| bool DoesRuleExists(string ruleName, ISessionContextProviderContainer sessionContext) |
| ---------------------|

The _**DoesRuleExists**_ methods will look into the configuration and determine if the specified rule does exists in the configuration or not. This allow users to check the existence of a rule without having to go through handler creation and populating the rule into the handler only to check the existence of the rule.

### Parameters
| _**Parameter Name**_ | _**Type**_ | _**Description**_                                         |
| ---------------------------- | ------------------ | ----------------------------------------------------------------- |
| ruleName                | string             | The name of the rule to be checked for existence. |
| sessionContext           | ISessionContextProviderContainer             | The session context information container for the operation.    |

### Return Value
The method will return TRUE if the specified rule exists in the configuration (input file), FALSE if the specified rule does not exists.

### Usage Example
```csharp
string ruleToCheck = "Rule1";
if (!Prime.Base.ServiceStore<IDefeatureTrackingService>.Service.DoesRuleExists(ruleToCheck, this.sessionContext))
{
    Prime.Base.ServiceStore<IConsoleService>.Service.PrintError(() => $"{ruleToCheck} does exists in the configuration", this.sessionContext);
}
```

# Tracking Structure Handler Interface Methods
Once a Tracking Structure Handler is acquired, the interface gives access to the following methods for manipulation of defeature-tracking data:

## InitializeElementValues

| void InitializeElementValues(BitArray values) |
| --------------------------------------------- |

The _**InitializeElementValues**_ method initializes the values of the structure elements with the specified value. The number of element values given must match the size of the structure. The MSB will be stored into the first element and the LSB will be stored into the last element of the structure defined in the configuration file.

### Parameters

| _**Parameter Name**_ | _**Type**_ | _**Description**_                    |
|----------------------|------------| -------------------------------------------- |
| values               | BitArray   | The values to initialize the structure with. |

### Return Value
None.

### Usage Example

```csharp
this.structHandler.InitializeElementValues(new BitArray(4, false));
```

### Example using dependencies:
### Usage Example with Dependencies and Specific Initialization Values

If we have the following structures and dependencies:

```json
"StructureNames": ["CORETracking"],
"TrackingElements": [ "Core00", "Core01", "Core02", "Core03", "Core10", "Core11", "Core12", "Core13"],

"StructureNames": ["XBar"],
"TrackingElements": [ "StackDie0", "StackDie1", "StackDie2", "StackDie3"],

"Dependencies": [
{
"HighLevelStructure": "XBar",
"HighLevelElement": "StackDie0",
"LowLevelStructure": "CORETracking",
"LowLevelElements": ["Core00", "Core01"]
},
{
"HighLevelStructure": "XBar",
"HighLevelElement": "StackDie1",
"LowLevelStructure": "CORETracking",
"LowLevelElements": ["Core02", "Core03"]
},
{
"HighLevelStructure": "XBar",
"HighLevelElement": "StackDie2",
"LowLevelStructure": "CORETracking",
"LowLevelElements": ["Core10", "Core11",]
},
{
"HighLevelStructure": "XBar",
"HighLevelElement": "StackDie3",
"LowLevelStructure": "CORETracking",
"LowLevelElements": ["Core12", "Core13"]
}
]
```

The InitializeElementValues method can be used as follows:

```csharp
BitArray coreTrackingValues = new BitArray(new bool[] { true, true, true, true, false, true, false, true });
this.coreTrackingHandler.InitializeElementValues(coreTrackingValues);

BitArray xBarValues = new BitArray(new bool[] { false, true, false, true });
this.xBarHandler.InitializeElementValues(xBarValues);

// After these initializations, the structures "XBar" and "CoreTracking" have their elements set as specified. The InitializeElementValues method takes into account the dependencies between structures, so when initializing a structure that has dependencies, the method will also initialize the dependent structures accordingly.  The original and current values of the structures will look as follows:  
/*
XBar originalValues: false, true, false, true
XBar currentValues: true, true, false, true

CORETracking originalValues: true, true, true, true, false, true, false, true
CORETracking currentValues: true, true, true, true, false, true, true, true
*/

```

## UpdateElementValue & UpdateElementValueWithItuffPrint

|Interfaces|
| ------------------------------------------------------- |
| void UpdateElementValue(string elementName, bool value) |
| void UpdateElementValueWithItuffPrint(string elementName, bool value) |

The _**UpdateElementValue**_ method updates a value into the specified element of the Tracking Structure.

The _**UpdateElementValueWithItuffPrint**_ is doing exactly the same, but also will print the update data to ituff. Refer [Ituff Printing] for more information.

### Parameters

| _**Parameter Name**_ | _**Type**_ | _**Description**_                                    |
| ---------------------------- | ------------------ | ------------------------------------------------------------ |
| elementName                  | string             | The name of the element unto which the value will be stored. |
| value                        | bool               | The value to store.                                          |

### Return Value
None.

### Usage Example

```csharp
this.structHandler.UpdateElementValue("CORE0", false);
this.structHandler.UpdateElementValueWithItuffPrint("CORE0", false);
```

### Example using dependencies

If we have the following structures and dependencies:
```json
"StructureNames": ["CORETracking"],
"TrackingElements": [ "Core00", "Core01", "Core02", "Core03", "Core10", "Core11", "Core12", "Core13"],

"StructureNames": ["XBar"],
"TrackingElements": [ "StackDie0", "StackDie1"],


"Dependencies":
{
    "HighLevelStructure": "XBar",
    "HighLevelElement": "StackDie0",
    "LowLevelStructure": "CORETracking",
    "LowLevelElements": ["Core00", "Core01", "Core02", "Core03"]
},
{
    "HighLevelStructure": "XBar",
    "HighLevelElement": "StackDie1",
    "LowLevelStructure": "CORETracking",
    "LowLevelElements": ["Core10", "Core11", "Core12", "Core13"]
},
```

The UpdateElementValue can be used as usual, but if there are dependencies on the structure being updated then the dependent structure will be updated accordingly.
For this example lets assume both structures "XBar" and "CoreTracking" were initialized with all elements set as false (featured) 
```csharp
// For this example lets assume we are using the API from the structureHandler of XBar
this.structureHandler.UpdateElementValue("StackDie0", true);
this.structureHandler.UpdateElementValueWithItuffPrint("CORE0", false);

// from this update the structures will look as follows:
// XBar:
// - StackDie0: true
// - StackDie1: false
// 
// CORETracking:
// - Core00: true
// - Core01: true
// - Core02: true
// - Core03: true
// - Core10: false
// - Core11: false
// - Core12: false
// - Core13: false
```

As can be seen in the example above, when updating "StackDie0" a highLevelStructure's element, all the dependent elements on the lowLevel structure also get updated.

## UpdateElementValues & UpdateElementValuesWithItuffPrint;

|Interfaces|
| ----------------------------------------- |
| void UpdateElementValues(BitArray values) |
| void UpdateElementValuesWithItuffPrint(BitArray values) |


The _**UpdateElementValues**_ method updates the given values into the Tracking Structure. The number of element values given must match the size of the Tracking Structure. The MSB will be stored into the first element and the LSB will be stored into the last element of the tracking structure as defined in the Defeature Tracking Configuration File.

The _**UpdateElementValuesWithItuffPrint**_ is doing exactly the same, but also will print the update data to ituff. Refer [Ituff Printing] for more information.

### Parameters

| _**Parameter Name**_ | _**Type**_ | _**Description**_ |
| ---------------------------- | ------------------ | ------------------------- |
| values                       | BitArray           | The values to store.      |

### Return Value
None.

### Usage Example

```csharp
BitArray values = new BitArray(new bool[] { true, true, false, true });
this.structHandler.UpdateElementValues(values);
this.structHandler.UpdateElementValuesWithItuffPrint(values);
```

### Example using dependencies

If we have the following structures and dependencies:
```json
"StructureNames": ["CORETracking"],
"TrackingElements": [ "Core00", "Core01", "Core02", "Core03", "Core10", "Core11", "Core12", "Core13"],

"StructureNames": ["XBar"],
"TrackingElements": [ "StackDie0", "StackDie1"],


"Dependencies":
{
    "HighLevelStructure": "XBar",
    "HighLevelElement": "StackDie0",
    "LowLevelStructure": "CORETracking",
    "LowLevelElements": ["Core00", "Core01", "Core02", "Core03"]
},
{
    "HighLevelStructure": "XBar",
    "HighLevelElement": "StackDie1",
    "LowLevelStructure": "CORETracking",
    "LowLevelElements": ["Core10", "Core11", "Core12", "Core13"]
},
```

The UpdateElementValues or UpdateElementValuesWithItuffPrint can be used as usual, but if there are dependencies on the structure being updated then the dependent structure will be updated accordingly.
For this example lets assume both structures "XBar" and "CoreTracking" were initialized with all elements set as false (featured) 
```csharp
// For this example lets assume we are using the handler of the CORETracking structure
BitArray values = new BitArray(new bool[] { true, true, false, true, true, true, true, true });
this.structureHandler.UpdateElementValues(values);
this.structureHandler.UpdateElementValuesWithItuffPrint(values);

// from this update the structures will look as follows:
// XBar:
// - StackDie0: false
// - StackDie1: true
// 
// CORETracking:
// - Core00: true
// - Core01: true
// - Core02: false
// - Core03: true
// - Core10: true
// - Core11: true
// - Core12: true
// - Core13: true
```

As can be seen in the example above, when updating the lowLevelStructure all the dependent elements of XBar StackDie1 are set as defeatured, as consequence StackDie1 is also set as defeatured. On the other hand, not all the dependent elements of StackDie0 are defeatured, so StackDie0 is kept as false.

## ReadElementValue

| bool ReadElementValue(string elementName) |
| ----------------------------------------- |

The _**ReadElementValue**_ method reads and returns a value from the specified element of the Tracking Structure.

### Parameters

| _**Parameter Name**_ | _**Type**_ | _**Description**_                                  |
| ---------------------------- | ------------------ | ---------------------------------------------------------- |
| elementName                  | string             | The name of the element from which the value will be read. |

### Return Value
The boolean value read from the element.

### Usage Example

```csharp
bool isCoreFourDefeatured = this.structHandler.ReadElementValue("CORE4");
```

## ReadElementValues

| BitArray ReadElementValues() |
| ---------------------------- |

The _**ReadElementValues**_ method Reads the values from the Tracking Structure. The MSB will match the value of the first element and the LSB will match the one from the last element of the tracking structure as defined in the Defeature Tracking Configuration File.

### Parameters
None.

### Return Value
A bit array containing the values of the tracking structure elements.

### Usage Example

```csharp
BitArray structValues = this.structHandler.ReadElementValues();
bool isCoreZeroDefeatured = structValues.Get(0);
```

## ReadOriginalElementValue

| bool ReadOriginalElementValue(string elementName) |
| ------------------------------------------------- |

The _**ReadOriginalElementValue**_ method reads and returns the original value from the specified element of the Tracking Structure as initialized.

### Parameters

| _**Parameter Name**_ | _**Type**_ | _**Description**_                                  |
| ---------------------------- | ------------------ | ---------------------------------------------------------- |
| elementName                  | string             | The name of the element from which the value will be read. |

### Return Value
The boolean value read from the element.

### Usage Example

```csharp
bool isCoreFourOriginallyDefeatured = this.structHandler.ReadOriginalElementValue("CORE4");
```

## ReadOriginalElementValues

| BitArray ReadOriginalElementValues() |
| ------------------------------------ |

The _**ReadElementValues**_ method Reads the values from the Tracking Structures initialized. The MSB will match the value of the first element and the LSB will match the one from the last element of the tracking structure as defined in the Defeature Tracking Configuration File.

### Parameters
None.

###Return Value
A bit array containing the values of the tracking structure elements.

### Usage Example

```csharp
BitArray structOriginalValues = this.structHandler.ReadOriginalElementValues();
bool isCoreZeroOriginallyDefeatured = structValues.Get(0);
```

## PopulateRule

| void PopulateRule(string ruleName) |
| ---------------------------------- |

The _**PopulateRule**_ method populate the rule to the tracking handler to be used for rule evaluation against the structure. More than one rule can be populated into the handler.

### Parameters

| _**Parameter Name**_ | _**Type**_ | _**Description**_                                                                           |
| ---------------------------- | ------------------ | --------------------------------------------------------------------------------------------------- |
| ruleName                     | string             | The name of the rule to be populate that's defined under "Rules" section in the configuration file. |

### Return Value
None

### Usage Example

```csharp
this.structHandler.PopulateRule("Rule1");
this.structHandler.PopulateRule("Rule2");
```

### Exceptions

- If any of the rules contained in the Rule element being populated (from the configuration file) has`"type": "UserVar"` and the corresponding UserVar does not exist an exception is thrown.

## PopulateRuleForDependent

| void PopulateRuleForDependent(string dependentStructureName, string dependentRuleName) |
| ---------------------------------- |

The _**PopulateRuleForDependent**_ method populate the rule to the tracking handler to be used for rule evaluation against the dependenct structure. More than one rule can be populated into the handler. The rule evaluation will happen under `EvaluateRule` method, if the rule is populated for the dependent structure `EvaluateRule` will evaluate the rule against the dependent structure based on the value of the parent structure.

### Parameters

| _**Parameter Name**_ | _**Type**_ | _**Description**_                                                                           |
| ---------------------------- | ------------------ | --------------------------------------------------------------------------------------------------- |
| dependentStructureName                     | string             | The name of the dependent structure to be populate that's defined under "Rules" section in the configuration file. The structure must exists as tracking strucutre in the config. |
| dependentRuleName                     | string             | The name of the rule to be populated for the dependent structure that's defined under "Rules" section in the configuration file. |

### Return Value
None

### Usage Example

```csharp
this.structHandler.PopulateRuleForDependent("StructureA", "Rule1");
```

### Exceptions

- If any of the rules contained in the Rule element being populated (from the configuration file) has`"type": "UserVar"` and the corresponding UserVar does not exist an exception is thrown.

## EvaluateRule

| bool EvaluateRule(BitArray value) |
| --------------------------------- |

The _**EvaluateRule**_ method evaluate the value of the structure against the rules populated in the structure. If more than one rule is populated, all populated rules will be evaluated against the value.

### Parameters

| _**Parameter Name**_ | _**Type**_ | _**Description**_                    |
| ---------------------------- | ------------------ | -------------------------------------------- |
| value                        | BitArray           | The value to be evaluated against the rules. |

### Return Value
True if the data complies with the rules.

### Usage Example

```csharp
BitArray trackingData = this.structHandler.ReadElementValues();

(...)

trackingData.Set(2, true); // After test, the result shows that element #2 needs to be defeatured

(...)

this.structHandler.PopulateRule("Rule1");
this.structHandler.PopulateRule("Rule2");
bool isDataValid = this.structHandler.EvaluateRule(trackingData);
```

# Version tracking

| **Date**   | **Prime release** | **Author**    | **Comments** |
| ---------- | ----------- | ------------- | ------------ |
| March 12, 2025 | 13.02.00 | Yusof, Adam Malik | Data Propagation Option and IntraStructure Links. #57860 |
| April 1, 2025 | 13.02.01 | Yusof, Adam Malik | Ituff Printing and MultiTier Dependency.<br>#58518<br>#58638  |