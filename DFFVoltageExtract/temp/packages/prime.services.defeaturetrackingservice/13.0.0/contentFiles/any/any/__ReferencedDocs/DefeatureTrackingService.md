[[_TOC_]]

# Prerequisites

This Service makes use of Aleph initialization for parsing and validation of the configuration file. Please refer to [Aleph documentation](https://dev.azure.com/mit-us/PrimeWiki/_wiki/wikis/PrimeWiki.wiki/28879/Special-ENV-Variables-in-Prime) for details.


#Defeature Tracking Service
The _**Defeature Tracking Service**_ provides the necessary infrastructure to track whether specific elements of a die are featured/defeatured for the purposes of recovery methodologies. This is done via the creation of tracking structures which contain elements for which their featured/defeatured state will be tracked. Tracked elements will have a default value of "featured" ("false" or "0") and can then be moved to a "defeatured" ("true" or "1") state as needed.

The data for all tracking structures and their elements is stored into Prime's Shared Storage in order to guarantee access from any test instance. Tracking Structure data is treated as unique to each DUT in a test program LOT and will be discarded from the Shared Storage memory once the DUT testing is finished.


#Configuration File
The json configuration file, which is parsed and validated by Aleph during Init flow, is the one responsible for defining the pool of elements to track as well as the grouping of elements per tracking structure.

Configuration file has split into 2 files in Prime v10.0, mainly to separate validCombinations section into other file. Hence, configuration files that are expected to parse by Aleph are:
- defeaturetracking.json file
- validcombinations.json file

Multiple validcombinations files are allowed.

##Naming Convention
The filename of the configuration files must follow the following convention:
- `{custom file name}.defeaturetracking.json`
- `{custom file name}.validcombinations.json`

##Configuration File Example
The following is an example of a simple DefeatureTracking configuration file:

|basic_example.defeaturetracking.json|
|--|
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
Example of a ValidCombinations configuration file:
|configuration_name.validcombinations.json|
|--|
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

As the example shows, defeatureTracking configuration file is comprised of three main sections which are required:
1. "Elements": this first section defines the pool of elements that can be tracked among the tracking structures.
1. "TrackingStructures": this section specifies the names of the tracking structures and the elements that each one will be tracking. It has two mandatory sub-sections per each structure.
   - "StructureNames": The name of the tracking structure to create. If more than one name is given in this comma-separated list, then the service creates the listed structures with the same elements, but the structures will be functionally independent.
   - "StructureGroups": The groups of elements that's resides under the structure (sub-structure).
      - "Name": The name of the group.
      - "TrackingElements": The list of elements to group and track under this tracking structure. All the elements listed here must be found in the "Elements" section described above, otherwise the configuration file validation fails with an exception during Init.
1. "Rules": This section defines the of list rules that will be used for validation of the data being tracked.
   - "Name": The name of the rule set.
   - "Single-Fail": Option to enable single-fail check.
      - "type": Choice of "Literal" or "UserVar". Using "UserVar" will allow user to define the value as a UserVar and the actual data can be defined within the UserVar.
      - "value": When using "Literal" type, the option is a boolean of true or false; if set to true, the tracking data will be compared against the original value  (per initialized) to not have mismatch. **Init will fail when value is not true or false.** When using "UserVar", the value should be a UserVar that contains the boolean value of true or false.
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

#Defeature Tracking Service Interface Methods

##CreateTrackingStructureHandler

|ITrackingStructureHandler CreateTrackingStructureHandler(string structureName)|
|--|


The _**CreateTrackingStructureHandler**_ method returns an interface to a "Tracking Structure Handler" which gives access to data operations that can be performed unto a specific tracking structure. Since the returned Tracking Structure Handler is linked to a fixed tracking structure, the need to provide the target structure name for each operation is eliminated.

###Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| structureName| string  | The name of the tracking structure for which a handler is needed. |

###Return Value
Tracking Structure Handler interface.

###Usage Example
The following C# code excerpt exemplifies the usage of the CreateTrackingStructureHandler method in order to get a handler to a tracking structure called "Struct1":
``` csharp
using Prime.DefeatureTrackingService;

(...)

private ITrackingStructureHandler structHandler;

(...)

this.structHandler = Prime.Services.DefeatureTrackingService.CreateTrackingStructureHandler("Struct1");
```


##CreateTrackingStructureGroupHandler

|ITrackingStructureHandler CreateTrackingStructureGroupHandler(string structureName, string structureGroupName)
|--|

The _**CreateTrackingStructureHandler**_ method returns an interface to a "Tracking Structure Handler" which gives access to data operations that can be performed unto a specific structure group. Since the returned Tracking Structure Handler is linked to a fixed structure group, the need to provide the target structure name for each operation is eliminated.

###Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| structureName| string  | The name of the tracking structure for which a handler is needed. |
| structureGroupName| string  | The name of the structure group for which a handler is needed. |

###Return Value
Tracking Structure Handler interface.

###Usage Example
The following C# code excerpt exemplifies the usage of the CreateTrackingStructureGroupHandler method in order to get a handler to structure group "Die0" under tracking structure called "Struct1":
``` csharp
using Prime.DefeatureTrackingService;

(...)

private ITrackingStructureHandler structHandler;

(...)

this.structHandler = Prime.Services.DefeatureTrackingService.CreateTrackingStructureGroupHandler("Struct1", "Die0");
```

#Tracking Structure Handler Interface Methods
Once a Tracking Structure Handler is acquired, the interface gives access to the following methods for manipulation of defeature-tracking data:

##InitializeElementValues

|void InitializeElementValues(BitArray values)|
|--|

The _**InitializeElementValues**_ method initializes the values of the structure elements with the specified value. The number of element values given must match the size of the structure. The MSB will be stored into the first element and the LSB will be stored into the last element of the structure defined in the configuration file.

###Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| value| bool  | The values to initialize the structure with. |

###Return Value
None.

###Usage Example
``` csharp
this.structHandler.InitializeElementValues(new BitArray(4, false));
```
##UpdateElementValue

|void UpdateElementValue(string elementName, bool value)|
|--|

The _**UpdateElementValue**_ method updates a value into the specified element of the Tracking Structure.

###Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| elementName| string | The name of the element unto which the value will be stored. |
| value | bool| The value to store. |

###Return Value
None.

###Usage Example
``` csharp
this.structHandler.StoreElementValue("CORE0", false);
```

##UpdateElementValues

|void UpdateElementValues(BitArray values)|
|--|

The _**UpdateElementValues**_ method updates the given values into the Tracking Structure. The number of element values given must match the size of the Tracking Structure. The MSB will be stored into the first element and the LSB will be stored into the last element of the tracking structure as defined in the Defeature Tracking Configuration File.

###Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| values | BitArray | The values to store. |

###Return Value
None.

###Usage Example
``` csharp
this.structHandler.StoreElementValue("CORE0", false);
```

##ReadElementValue

|bool ReadElementValue(string elementName)|
|--|

The _**ReadElementValue**_ method reads and returns a value from the specified element of the Tracking Structure.

###Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| elementName | string | The name of the element from which the value will be read. |

###Return Value
The boolean value read from the element.

###Usage Example
``` csharp
bool isCoreFourDefeatured = this.structHandler.ReadElementValue("CORE4");
```

##ReadElementValues

|BitArray ReadElementValues()|
|--|

The _**ReadElementValues**_ method Reads the values from the Tracking Structure. The MSB will match the value of the first element and the LSB will match the one from the last element of the tracking structure as defined in the Defeature Tracking Configuration File.

###Parameters
None.

###Return Value
A bit array containing the values of of the tracking structure elements.

###Usage Example
``` csharp
BitArray structValues = this.structHandler.ReadElementValues();
bool isCoreZeroDefeatured = structValues.Get(0);
```

##ReadOriginalElementValue

|bool ReadOriginalElementValue(string elementName)|
|--|

The _**ReadOriginalElementValue**_ method reads and returns the original value from the specified element of the Tracking Structure as initialized.

###Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| elementName | string | The name of the element from which the value will be read. |

###Return Value
The boolean value read from the element.

###Usage Example
``` csharp
bool isCoreFourOriginallyDefeatured = this.structHandler.ReadOriginalElementValue("CORE4");
```

##ReadOriginalElementValues

|BitArray ReadOriginalElementValues()|
|--|

The _**ReadElementValues**_ method Reads the values from the Tracking Structureas initialized. The MSB will match the value of the first element and the LSB will match the one from the last element of the tracking structure as defined in the Defeature Tracking Configuration File.

###Parameters
None.

###Return Value
A bit array containing the values of of the tracking structure elements.

###Usage Example
``` csharp
BitArray structOriginalValues = this.structHandler.ReadOriginalElementValues();
bool isCoreZeroOriginallyDefeatured = structValues.Get(0);
```

##PopulateRule

|void PopulateRule(string ruleName)|
|--|

The _**PopulateRule**_ method populate the rule to the tracking handler to be used for rule evaluation against the structure. More than one rule can be populated into the handler.

###Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| ruleName | string | The name of the rule to be populate that's defined under "Rules" section in the configuration file. |

###Return Value
None

###Usage Example
``` csharp
this.structHandler.PopulateRule("Rule1");
this.structHandler.PopulateRUle("Rule2");
```

##EvaluateRule

|bool EvaluateRule(BitArray value)|
|--|

The _**EvaluateRule**_ method evaluate the value of the structure against the rules populated in the structure. If more than one rule is populated, all populated rules will be evaluated against the value.

###Parameters
| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| value | BitArray | The value to be evaluated against the rules. |

###Return Value
True if the data complies with the rules.

###Usage Example
``` csharp
BitArray trackingData = this.structHandler.ReadElementValues();

(...)

trackingData.Set(2, true); // After test, the result shows that element #2 needs to be defeatured

(...)

this.structHandler.PopulateRule("Rule1");
this.structHandler.PopulateRule("Rule2");
bool isDataValid = this.structHandler.EvaluateRule(trackingData);
```