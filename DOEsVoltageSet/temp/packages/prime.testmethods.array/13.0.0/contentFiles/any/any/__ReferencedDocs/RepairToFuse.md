[[_TOC_]]

# Methodology
RepairToFuse Test Method is specially designed for fuse programming preparation. The general idea behind this methodology is to post-process the repair capture data and generate individual sets of information to indicate which repair elements need to be used at specific repair domains.

At the end of the execution of the test instance, the expected output will be in the form of pairs of FuseName/FuseValue; which can be consumed either through the FuseManager service by the Flame tool or through the SharedStorage service.

## Execution general flow
A general flow indicating the steps to be followed within the Execute method of the RepairToFuse methodology is presented below.

![Dynamic flow execute](./DocumentationFlows/ExecuteFlow.png)

**Note:** This flow serves the purpose of representing at high level the steps to be follow for creating the final results.

The highlighted green steps represent the TM extensions that the user can overwrite for modifying any part of the process

## Test Method Exceptions
The following sections contain a table showing the exceptions to be thrown at Prime RepairToFuse Test Method

### Verify Exceptions
| Exception Message | Cause | Possible Solution |
| --- | --- | --- |
| Missing parameter defining filepath to Resource File. This is required by all instances of Repair | Test Method parameter `ResourcesFilePath` is either null or empty | Review test instance definition in mtpl file to ensure parameter name is written correctly and it has an assigned value |
| Resource file=[\<file path\>] does not exist | Test Method parameter does not contain a valid file path | Review test instance definition in mtpl file to ensure test method parameter points to an existing file path |
| Missing parameter defining filepath to RepairToFuse File | Test method parameter `RepairToFusefilePath` is either null or empty | Review test instance definition in mtpl file to ensure parameter name is written correctly and it has an assigned value |
| RepairToFuse file=[\<file path\>] does not exist | Test method parameter does not contain a valid file path | Review test instance definition in mtpl file to ensure test method parameter points to an existing file path |
| Duplicated hash names: [\<FuseHash list\>] | RepairToFuse configuration file contains multiple `FuseHashes` assigned the same `HashName` | Review all FuseHashes in the RepairToFuse configuration file pointed at by the test instance to make sure they all have a unique name |
| Duplicated map names=[\<FuseMap list\>] | RepairToFuse configuration file contains multiple `FuseMaps` assigned the same `MapName` | Review all FuseMaps in the RepairToFuse configuration file pointed at by the test instance to make sure they all have a unique name |
| FuseMap points to nonexistent FuseHashes=[\<FuseHash list>\] | RepairToFuse configuration file contains one or more `FuseMaps` that points to nonexistent `FuseHash` values | Review the `FuseHashReplacementInformation` in the configuration file to ensure the `Keyword` corresponds to a fuse hash defined in the same configuration file |
 ProcessResource with Resources=[\<Resource list>\] defines both static and dynamic outputs. | At least one ProcessResource definition contains both dynamic and static output while only one output type is allow for each ProcessResource | Review all ProcessResource in the configuration file to ensure each one of them defines one type of output, either static or dynamic |
 | ProcessResource with Resources=[\<Resource list>\] does not define any dynamic or static output | At least one ProcessResource definition does not define either dynamic or static output | Review all ProcessResource in the configuration file to ensure each one of them defines one type of output, either static or dynamic |
 | ProcessResource does not point to any resource | At least one ProcessResource has the property `Resources` but no resource `Name` has been defined | Review all Resources in the ProcessResource section defined in the configuration file to ensure all of them includes at leas one resource name |
 | ProcessResource contains the following duplicated resources=[\<Resource list>\] | At least one ProcessResource has the property `Resources` defining duplicated resource `Name` values | Review all Resources in the ProcessResource section defined in the configuration file to ensure all of the resource names are unique |
 | ProcessResource points to nonexistent FuseMaps=[\<FuseMap list>\] | At least one ProcessResource points to a FuseMap that is not defined in the configuration file | Review all outputs in each ProcessResource and make sure all of them point to FuseMaps that are defined within the same configuration file | 
 | ReMapIndex is not allowed while having FuseIndex set to default value=[-1] | At least one Static Output defines `ReMapIndex` with `FuseIndex` set to default value -1, which is not allowed | Review all ReMapIndex defined in the configuration file and make sure none of them are paired with a default FuseIndex |
 | Static output with FuseData=[\<FuseData>\] and OutputFuse=[\<FuseMap>\] contains duplicated Sources=[\<Source list>\] in ReMapIndex | Static output definition has ReMapIndex with duplicated source values, which is not allowed. ReMapIndex must be circular referenced | Review all ReMapIndex definitions to make sure none of them contains duplicated source values |
 | Static output with FuseData=[\<FuseData>\] OutputFuse=[\<OutputFuse>\] contains duplicated Targets=[\<Target list>\] in ReMapIndex | Static output definition has ReMapIndex with duplicated target values, which is not allowed. ReMapIndex must be circular referenced | Review all ReMapIndex definitions to make sure none of them contains duplicated target values |
 | SourceIndex=[\<Source index list>\] is not defined as a Target in ReMapIndex for static output with FuseData=[\<FuseData>\] and OutputFuse=[\<OutputFuse>\] | ReMapIndex is not circular referenced, which means the source index is remap to another value but nothing is being remapped to the source index | Review all ReMapIndex definitions to make sure all of the source index are being referenced by a target index and vice versa |
 | MapIndexToStartLocation is not allowed while having FuseIndex set to default value=[-1] | At least one Dynamic Output defines `MapIndexToStartLocation` with `FuseIndex` set to default value -1, which is not allowed | Review all MapIndexToStartLocation defined in the configuration file and make sure none of them are paired with a default FuseIndex |
 | Namespace [\<Namespace>\] does not exist in storage | Test Method parameter `Namespace` is assigned a value that does not exist in the .vfconfigs.json file | Make sure that the `ALEPH_FILE` variable in the environment file is correctly pointing to the desire .vfconfigs.json file. <br/> Review test instance definition in mtpl file to ensure parameter name is written correctly and it has a value that exists in the .vfconfigs.json file |

### Execute Exceptions
| Exception | Cause | Possible Solution |
| --- | --- | --- |
| Resource=[\<Resource name>\] was not found in list=[\<Resource list>\] | ProcessResource in R2F configuration file points to a resource definition that does not exist in the resource configuration file | Review test instance definition in the mtpl file to confirm that the test instance parameters point to the expected input files <br/> | Confirm that the value assigned to the resource name in the R2F configuration file matches exactly the resource name defined in the resource configuration file |
| FuseGroups=[\<FuseGroup list>\] are duplicated within the repair solution=[\<Repair solution>\] | Repair solution contains duplicated fuse groups which will cause an error while inserting the data | As repair solutions comes from previous instances, review TP flow to detect which instance may be causing the duplicated data |
| Format=[\<String format>\] contains variable=[\<Defect variable>\] that is not defined as a resource DefectVariable | Either FuseData format, FuseIndex format or Hash keyword defined in the configuration file uses a defect variable that is not defined in the resource definition | Review ProcessResource to make sure is pointing at the desired Resource <br/> Review the string format to make sure all defect variables being used are spelled correctly both in the ProcessResource and in the Resource definition |
| FuseIndex=[\<FuseIndex value>\] already exists for the current dynamic output. | ProcessResource of dynamic output type contains two fuse data with the exact same fuse index value which cannot both be inserted into the same final binary string | Review the FuseData and FuseIndex format to make sure that is defined correctly |
| FuseData=[\<FuseData value>\] cannot be inserted as FuseMap=[\<FuseMap name>\] is fulled with previous fuse data | FuseMap pointed at by the process resource is already full with previous data | Review ProcessResource and make sure that is pointing to the correct FuseMap <br/> Review the FuseData format to make sure it has the correct size |
| Cannot insert fuseData=[\<FuseData value>\] into fuse value because there is no space left to insert the data | FuseMap is not yet full but FuseData size is longer than the amount of available spaces in the final binary string | Review ProcessResource and make sure that is pointing to the correct FuseMap <br/> Review the FuseData format to make sure it has the correct size |
| Domain=[\<Domain value>\] in resource={resource} exceeds the number of available spares=[\<Available spares>\] | Fuse groups found in the repair solution exceeds the amount of available spares in the Resource configuration file | Make sure that the ProcessResource points to the correct Resource <br/> Review the Resource definition to make sure it's correctly defined |
| Error converting number=[\<Spare counter decimal value>\] to binary with binarySize of=[\<Binary size>\], since binary result is=[\<Spare counter binary value>\] with length of=[\<Binary size>\] | Spare counter value converted to binary is greater in size than the `Spares` definition in the Resource configuration file | Review the ProcessResource to make sure it's pointing to the expected resource <br/> Review the Resource definition to make sure that the Spares property has the expected value |

# Concepts
This section is focused on providing basic examples on the different sub-process used by the RepairToFuse methodology. This includes:

- Decoding the repair solution ([here](#decoding-repair-solution))
- Obtaining the fuse groups from the repair solution ([here](#getting-the-fuse-groups))
- Getting the defect variables value from each fuse group ([here](#defect-variables-values))
- Computing the fuse data and fuse index for each fuse group ([here](#fusedata--fuseindex))
- Generate the final binary string for Static Output processing ([here](#static-output))
- ReMapIndex for static outputs ([here](#remapindex))
- Generate the final binary string for Dynamic Output processing ([here](#dynamic-output))
- MapIndexToStartLocation for dynamic outputs ([here](#mapindextostartlocation))
- Creating the fuse values ([here](#create-final-fuses))
- Sharing fuse values with services ([here](#share-final-fuses))

## Decoding repair solution
RepairTM creates a set of solutions per resource, after choosing the most repairable defects, the corresponding fuse groups are stored into a single binary string.

The current section is based on the resource definition presented in the snapshot below.

```json
"Resources": [
  {
    "Name": "MLCDatRow",
    "Spares": 6,
    "DefectVars": {
      "Domain": [
        {"Name": "slice", "Quantity": 3, "SubStructureIdentifier": "true"}
      ],
      "Element": [
        {"Name": "half", "Quantity": 2},
        {"Name": "quad", "Quantity": 2},
        {"Name": "bank", "Quantity": 2}
      ]
    }
  }
]
```

RepairToFuse test method accesses this information through the resource name defined in the Resource file `MLCDatRow`.

With the snippet presented above it can be determine that the fuse group is 5 bits in size. Let's suppose then that the solution is given by 011011110001000.

> `MLCDatRow` gives solution `0011011110001000`

## Getting the fuse groups
Given that the fuse group is of 5 bits in size, the following table represents how to split up the repair solution into smaller chunks

| Solution            | Value                                              |
|---------------------|----------------------------------------------------|
| **Binary Solution** | 011011110001000                                    |
| **Fuse group 1**    | <span style="color:Tomato ">01101</span>1110001000 |
| **Fuse group 2**    | 01101<span style="color:Tomato ">11100</span>01000 |
| **Fuse group 3**    | 0110111100<span style="color:Tomato ">01000</span> |

Then, the list of fuse groups to be processed in order to create the different fuse data will be: `01101, 11100, 01000`

## Defect variables values
For each fuse group it is then possible to obtain the value associated to each defect variable. Following the same example, the defect variables for the current resource are: slice(2 bits), half(1 bit), quad(1 bit), bank(1 bit).

Every fuse group is always the concatenation of the value of all defect variables in sequential order, by reverse engineering this rule it can easily be obtained the value of the defect variables that correspond to said fuse group.

| Fuse group | slice | half | quad | bank |
|------------|-------|------|------|------|
| 01101      | 01    | 1    | 0    | 1    |
| 11100      | 11    | 1    | 0    | 0    |
| 01000      | 01    | 0    | 0    | 0    |

## FuseData & FuseIndex
With the defect variables value, both FuseData and FuseIndex can be generated. This will be done by replacing the defect variables name used in the string format with their corresponding value

```json
...
"FuseData": '11,slice,quad,0',
"FuseIndex": 'slice,half'
...
``` 

As there are three fuse groups, there will be three fuse data; giving a 1:1 correspondance.

| slice | half | quad | bank | fuse data | fuse index |
|-------|------|------|------|-----------|------------|
| <span style="color:Tomato">01</span> | <span style="color:YellowGreen">1</span> | <span style="color:CornflowerBlue">0</span> | 1 | 11<span style="color:Tomato">01</span><span style="color:CornflowerBlue">0</span>0 | <span style="color:Tomato">01</span><span style="color:YellowGreen">1</span> |
| <span style="color:Tomato">11</span> | <span style="color:YellowGreen">1</span> | <span style="color:CornflowerBlue">0</span> | 0 | 11<span style="color:Tomato">11</span><span style="color:CornflowerBlue">0</span>0 | <span style="color:Tomato">11</span><span style="color:YellowGreen">1</span> |
| <span style="color:Tomato">01</span> | <span style="color:YellowGreen">0</span> | <span style="color:CornflowerBlue">0</span> | 0 | 11<span style="color:Tomato">01</span><span style="color:CornflowerBlue">0</span>0 | <span style="color:Tomato">01</span><span style="color:YellowGreen">0</span> |

## Generate final binary strings
With the previous concepts now explained, the following sections go into further detail on how both static and dynamic outputs are processed in order to create the final fuse information.

The following snippet will be used as example for both Static and Dynamic output sections, this will help further understand the difference on how each type is processed

```json
"FuseHashes": [
  {
    "HashName": "SliceMap4c",
    "HashValues": [
      { "Key": 0,  "Value": "c0_r2" },
      { "Key": 1,  "Value": "c0_r4" },
      { "Key": 2,  "Value": "c1_r3" },
      { "Key": 3,  "Value": "c1_r5" }
    ]
  }
],

"FuseMaps": [
  {
    "MapName": "PMUCSRowData",
    "FuseHashReplacementInformation": [
      {"Keyword": "SliceMap4c", "DefectVariable": "slice"}
    ],
    "MapValues": [
      { "Size": 7, "Default": "0", "FuseName": "SliceMap4c_row0" },
      { "Size": 7, "Default": "0", "FuseName": "SliceMap4c_row1" },
      { "Size": 7, "Default": "0", "FuseName": "SliceMap4c_row2" },
      { "Size": 7, "Default": "0", "FuseName": "SliceMap4c_row3" }
    ]
  }
]
```

Each Output (static or dynamic) points to a single Fuse Map, which total size determines the size of the final binary string. As `PMUCSRowData` has 4 fuses of size 7 each, the total size of the final binary string is 28 bits.

Fuse Name | Fuse Size | Initial value |
--- | --- | --- |
SliceMap4c_row0 | 7 | _ _ _ _ _ _ _ |
SliceMap4c_row1 | 7 | _ _ _ _ _ _ _ |
SliceMap4c_row2 | 7 | _ _ _ _ _ _ _ |
SliceMap4c_row3 | 7 | _ _ _ _ _ _ _ |
Final binary string | 28 | _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ |

Now, each Hash Value has its own final binary string
Hash value | String size | Initial Value |
--- | --- | --- |
c0_r2 | 28 | _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ |
c0_r4 | 28 | _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ |
c1_r3 | 28 | _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ |
c1_r5 | 28 | _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ |

Which string is targeted is defined by the value of the defect variable defined in the `FuseHashReplacementInformation`, in this case `slice`.

The following sections for both Dynamic and Static output will be based on the following resource definition

```json
"Resources": [
  {
    "Name": "LDB",
    "DefectVariables": {
      "Domain": [
        { "Name": "slice", "Quantity": "4" }
      ],
      "Element": [
        { "Name": "quad", "Quantity": "4" },
        { "Name": "lr", "Quantity": "2" }
      ]
    }
  }
]
```

### Static output
Static outputs are defined by having each fuse data assigned to an specific location of the final binary string. This location is independent of the fuse data and it's assigned solely on the value of the FuseIdex.

**Snapshot of repair to fuse file for static output**
```json
"ProcessResources": [
  {
    "Resources": [
      { "Name": "LDB" }
    ],
    "StaticOutputs": [
      {
        "OutputFuse": "PMUCSRowData",
        "FuseData": "quad,slice,lr",
        "FuseIndex": "quad"
      }
    ]
  }
]
```

With the snapshots presented above representing an example of the configuration files, the following section explains how each fuse data is inserted into the final binary string. 

For the sake of simplicity, let us assume that the following table represents the obtained data from resource LDB

| slice | quad | lr | Fuse Data | Fuse Index |
| --- | --- | --- | --- | --- |
| 00 | 00 | 0 | 00000 | 00 |
| 00 | 10 | 0 | 00100 | 10 |
| 01 | 00 | 1 | 01001 | 00 |
| 01 | 11 | 1 | 01111 | 11 |
| 11 | 01 | 1 | 11011 | 01 |

In each case, the slice defect variable determines which final binary string will be targeted, a summary table is presented below

| Hash Key (slice) | Hash Value | Fuse Data | Fuse index |
| --- | --- | --- | --- |
| 00 | <span style="color:YellowGreen">c0_r2</span> | 00000 | <span style="background-color: LightBlue;color:black">00</span> |
| 00 | <span style="color:YellowGreen">c0_r2</span> | 00100 | <span style="background-color: LightSalmon;color:black">10</span> |
| 01 | <span style="color:Darkorange">c0_r4</span> | 01001 | <span style="background-color: LightBlue;color:black">00</span> |
| 01 | <span style="color:Darkorange">c0_r4</span> | 01111 | <span style="background-color: LightGreen;color:black">11</span> |
| 11 | <span style="color:Orchid">c1_r5</span> | 11011 | <span style="background-color:LightPink;color:black">01</span> |

Now, each Fuse Data is inserted into the final string. Within this final binary string, each fuse index is represented by a different colored chunk that's the size of the Fuse Data.

By inserting the data into each hash value the fuse data presented in the table above, RepairToFuse obtains the following results for the static output.

Hash value | String size | Final Value |
--- | --- | --- |
<span style="color:YellowGreen">c0_r2</span> | 28 | <span style="background-color: LightBlue;color:black">00000</span> <span style="background-color:LightPink;color:black">\_ _ _ _ _</span> <span style="background-color:LightSalmon;color:black">00100</span> <span style="background-color:LightGreen;color:black">\_ _ _ _ _</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |
<span style="color:Darkorange">c0_r4</span> | 28 | <span style="background-color: LightBlue;color:black">01001</span> <span style="background-color:LightPink;color:black">\_ _ _ _ _</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">01111</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |
c1_r3 | 28 | <span style="background-color: LightBlue;color:black">\_ _ _ _ _</span> <span style="background-color:LightPink;color:black">\_ _ _ _ _</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">\_ _ _ _ _</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |
<span style="color:Orchid">c1_r5</span> | 28 | <span style="background-color: LightBlue;color:black">\_ _ _ _ _</span> <span style="background-color:LightPink;color:black">11011</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">\_ _ _ _ _</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |

### ReMapIndex
In the case that the static output defines a remap index section, this should be processed before inserting the fuse data into the final binary string.

```json
"ReMapIndex":[
  { "Source": 0, "Target": 3 },
  { "Source": 3, "Target": 1 },
  { "Source": 1, "Target": 0 }
]
```

Considering the ReMapIndex presented above, the summary table for the static output should look like

| Hash Key (slice) | Hash Value | Fuse Data | Source Fuse index | Target Fuse Index |
|------------------|------------|-----------|-------------------|-------------------|
| 00 | <span style="color:YellowGreen">c0_r2</span> | 00000 | <span style="background-color: LightBlue;color:black">00</span> | <span style="background-color: LightGreen;color:black">11</span> |
| 00 | <span style="color:YellowGreen">c0_r2</span> | 00100 | <span style="background-color: LightSalmon;color:black">10</span> | <span style="background-color: LightSalmon;color:black">10</span> |
| 01 | <span style="color:Darkorange">c0_r4</span> | 01001 | <span style="background-color: LightBlue;color:black">00</span> | <span style="background-color: LightGreen;color:black">11</span> |
| 01 | <span style="color:Darkorange">c0_r4</span> | 01111 | <span style="background-color: LightGreen;color:black">11</span> | <span style="background-color:LightPink;color:black">01</span> |
| 11 | <span style="color:Orchid">c1_r5</span> | 11011 | <span style="background-color:LightPink;color:black">01</span> | <span style="background-color: LightBlue;color:black">00</span> |

With that in mind, the final binary strings for the static output will be represented by the following table

Hash value | String size | Final Value |
--- | --- | --- |
<span style="color:YellowGreen">c0_r2</span> | 28 | <span style="background-color: LightBlue;color:black">\_ _ _ _ _</span> <span style="background-color:LightPink;color:black">\_ _ _ _ _</span> <span style="background-color:LightSalmon;color:black">00100</span> <span style="background-color:LightGreen;color:black">00000</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |
<span style="color:Darkorange">c0_r4</span> | 28 | <span style="background-color: LightBlue;color:black">\_ _ _ _ _</span> <span style="background-color:LightPink;color:black">01111</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">01001</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |
c1_r3 | 28 | <span style="background-color: LightBlue;color:black">\_ _ _ _ _</span> <span style="background-color:LightPink;color:black">\_ _ _ _ _</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">\_ _ _ _ _</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |
<span style="color:Orchid">c1_r5</span> | 28 | <span style="background-color: LightBlue;color:black">11011</span> <span style="background-color:LightPink;color:black">\_ _ _ _ _</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">\_ _ _ _ _</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |

### Dynamic output
Dynamic outputs are characterized by not having a fixed position within the final binary string, each fuse data is positioned in the next available spot.

To better understand how dynamic output works follow the example below.

**Snapshot of repair to fuse file for static output**
```json
"ProcessResources": [
  {
    "Resources": [
      { "Name": "LDB" }
    ],
    "DynamicOutputs": [
      {
        "OutputFuse": "PMUCSRowData",
        "FuseData": "quad,slice,lr",
      }
    ]
  }
]
```

For the sake of simplicity, let us assume that the following table represents the obtained data from resource LDB

| slice | quad | lr | Fuse Data |
| --- | --- | --- | --- |
| 00 | 00 | 0 | 00000 |
| 00 | 10 | 0 | 00100 |
| 01 | 00 | 1 | 01001 |
| 01 | 11 | 1 | 01111 |
| 11 | 01 | 1 | 11011 |

In each case, the slice defect variable determines which final binary string will be targeted, a summary table is presented below

| Hash Key (slice) | Hash Value | Fuse Data |
| --- | --- | --- |
| 00 | <span style="color:YellowGreen">c0_r2</span> | 00000 |
| 00 | <span style="color:YellowGreen">c0_r2</span> | 00100 |
| 01 | <span style="color:Darkorange">c0_r4</span> | 01001 |
| 01 | <span style="color:Darkorange">c0_r4</span> | 01111 |
| 11 | <span style="color:Orchid">c1_r5</span> | 11011 |

As this is the case for a dynamic output, ech fuse data will be inserted in the order in which they come.

Hash value | String size | Final Value |
--- | --- | --- |
<span style="color:YellowGreen">c0_r2</span> | 28 | <span style="background-color: LightBlue;color:black">00000</span> <span style="background-color:LightPink;color:black">00100</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">\_ _ _ _ _</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |
<span style="color:Darkorange">c0_r4</span> | 28 | <span style="background-color: LightBlue;color:black">01001</span> <span style="background-color:LightPink;color:black">01111</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">\_ _ _ _ _</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |
c1_r3 | 28 | <span style="background-color: LightBlue;color:black">\_ _ _ _ _</span> <span style="background-color:LightPink;color:black">\_ _ _ _ _</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">\_ _ _ _ _</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |
<span style="color:Orchid">c1_r5</span> | 28 | <span style="background-color: LightBlue;color:black">11011</span> <span style="background-color:LightPink;color:black">\_ _ _ _ _</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">\_ _ _ _ _</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |

### MapIndexToStartLocation
In the case that the dynamic output defines a MapIndexToStartLocation section, this should be processed before inserting the fuse data into the final binary string.

```json
"MapIndexToStartLocation":[
  { "index": 0,  "location": 15  },
  { "index": 1,  "location": 5  }
]
```

With this new information, the dynamic output would have the following order.

Hash value | String size | Final Value |
--- | --- | --- |
<span style="color:YellowGreen">c0_r2</span> | 28 | <span style="background-color: LightBlue;color:black">\_ _ _ _ _</span> <span style="background-color:LightPink;color:black">00100</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">00000</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |
<span style="color:Darkorange">c0_r4</span> | 28 | <span style="background-color: LightBlue;color:black">\_ _ _ _ _</span> <span style="background-color:LightPink;color:black">01111</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">01001</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |
c1_r3 | 28 | <span style="background-color: LightBlue;color:black">\_ _ _ _ _</span> <span style="background-color:LightPink;color:black">\_ _ _ _ _</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">\_ _ _ _ _</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |
<span style="color:Orchid">c1_r5</span> | 28 | <span style="background-color: LightBlue;color:black">\_ _ _ _ _</span> <span style="background-color:LightPink;color:black">\_ _ _ _ _</span> <span style="background-color:LightSalmon;color:black">\_ _ _ _ _</span> <span style="background-color:LightGreen;color:black">11011</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |

## Create final fuses
Whether the information was obtained by processing dynamic or static outputs the generation of the final fuses remains the same. Let us assume that the fuse map definition is the same as given by [Generate final binary strings](#generate-final-binary-strings) section in which there are four fuses of size of 7 bits.

The following example shows the generation of fuses for one final binary string of the static output
Hash value | String size | Final Value |
--- | --- | --- |
<span style="color:YellowGreen">c0_r2</span> | 28 | <span style="background-color: LightBlue;color:black">00000</span> <span style="background-color:LightPink;color:black">\_ _ _ _ _</span> <span style="background-color:LightSalmon;color:black">00100</span> <span style="background-color:LightGreen;color:black">\_ _ _ _ _</span> <span style="background-color:LightSeaGreen;color:black">\_ _ _ _ _</span> _ _ _ |

This final binary string will have to be split up into the sizes of the final fuses

| Fuse name | Fuse value |
| ------------------- | ---------- |
| c0_r2_row0 | <span style="background-color: DarkKhaki;color:black">00000_ _</span> _ _ _00100\_ _ _ _ _ _ _ _ _ _ _ _ _ |
| c0_r2_row1 | 00000_ _ <span style="background-color: DarkKhaki;color:black">_ _ _0010</span>0\_ _ _ _ _ _ _ _ _ _ _ _ _ |
| c0_r2_row2 | 00000_ _ _ _ \_0010<span style="background-color: DarkKhaki;color:black">0_ _ _ _ _ _</span> _ _ _ _ _ _ _ |
| c0_r2_row3 | 00000_ _ _ _ _00100\_ _ _ _ _ _ <span style="background-color: DarkKhaki;color:black">\_ _ _ _ _ _ _</span> |

After that process is done, only the fuses that contains real data will be stored in either the FuseManager or the SharedStorage service. In this case `c0_r2_row3` does not contain any real data, so this item will be ignored.

The table below summarizes the valid fuses after completing their information with their default value, in this case 0 (<span style="color:Tomato ">Characters shown in red</span>).

| Fuse name | Fuse value | Fuse value with default |
| --------- | ---------- | ----------------------- |
| c0_r2_row0 | 00000_ _ | 00000<span style="color:Tomato ">00</span> |
| c0_r2_row1 | _ _ _0010 | <span style="color:Tomato ">000</span>0010 |
| c0_r2_row2 | 0_ _ _ _ _ _ | 0<span style="color:Tomato ">000000</span> |

## Share final fuses
Both services store the final fuse information under a dictionary that contains the FuseName as `key` and the FuseValue as `value`. This information will be stored under the name of the FuseMap that generated said fuses.

# Parameters
| ParameterName | Type | Description | IsRequired | Default Value |
|---------------|------|-------------|------------|---------------|
| ResourceFilePath | File | Path to resource json file configuration | Yes | - |
| RepairToFuseFilePath | File | Path to repair to fuse json file configuration | Yes | - |
| FuseNamespace | String | Value to get the virtual fuse handler | Yes | - |
| EnableFuseReset | String | Determines if R2F fuses in FuseManager service must be reset. <br/> **Options are:** True (_default_), False | No | True |

# Input Files
RepairToFuse Test Method consumes two Json configuration files. The ResourceFile from which the methodology obtains the resource information and the RepairToFuse file which defines how to process the fuse group data. Both of these input files are described in the below sections.

## Resource file
The resource file is necessary in the repair to fuse methodology to obtain the defect variables information that will allow to decode the repair solution and then store the concatenated fusedata in the shared storage with their respective keys.

An example of the resource file looks as follows

```json
{
  "Resources": [
    {
      "Name": "LDB",
      "Spares": 6,
      "DefectVars": {
        "Domain": [
          { "Name": "slice", "Quantity": 64, "SubStructureIdentifier": "true" }
        ],
        "Element": [
          { "Name": "quad", "Quantity": 4 },
          { "Name": "lr", "Quantity": 127 },
          { "Name": "bank", "Quantity": 31 }
        ]
      }
    }
  ]
}
```

- _**Resources**_: Grouping field. Used to provide resource definitions.
  - _**Name**_: Field to provide the resource name.
  - _**Spares**_: Field to provide the amount of available spares per resource.
  - _**DefectVars**_: Grouping field. Used to provide the resource variables into the Domain and Element arrays.
    - _**Domain**_: Grouping field. Used to provide the list the variables considered as Domain.
	- _**Element**_: Grouping field. Used to provide the list the variables considered as Element.
	- _**Name**_: Field to provide the variable name.
	- _**Quantity**_: Field to provide the variable quantity.
    - _**SubStructureIdentifier**_: Field to provide the value indicating whether the defect var it is the tracking variable or not. <span style="color:Tomato "> Not used in RepairToFuse methodology</span>.
    - _**EnableValue**_: Field to provide a defined value for the repair solution. <span style="color:Tomato "> Not used in RepairToFuse methodology</span>.
    - _**Ignore**_: Field to provide a value indicating whether it is ignored or not. <span style="color:Tomato "> Not used in RepairToFuse methodology</span>.

## RepairToFuse file
The RepairToFuse input files provides all the necessary information to process the information obtained from the repair solutions as well as the necessary information to create the fuses that will be shared through the FuseManager or the SharedStorage service.

The structure of the file is as follows:

```json
{
  "VersionHistory": [ ... ],

  "GlobalMapping": {
    "FuseHashes": [ ... ],
    "FuseMaps": [ ... ]
  },

  "ProcessResources": [
    {
      "Resources": [ ... ],
      "DynamicOutputs": [ ... ],
      "StaticOutputs": [ ... ],
      "EnableCounterOutput": [ ... ]
    }
  ]
}
```

The actual content of each section has been replaced with [...] in order to simplify and better represent the overall structure. 

Now, each section of the file will be defined separately to improve readability, but keep in mind that all of this information should be contained in a single file when creating the RepairToFuse input file that will be used by the test instance.

### VersionHistory
```json
"VersionHistory": [
  {
    "Revision": "00",
    "Comment": "Example of file description"
  }
]
```
- _**VersionHistory**_: Grouping field. Contains json version information to keep track of any change or improvement in the configuration file.
  - _**Revision**_: Current json file version. This should always be an integer.
  - _**Comment**_: Explicitly describes any changes or improvements for the current file.

### GlobalMapping
```json
"GlobalMapping": {
  "FuseHashes": [
    {
      "HashName": "SliceMap2c",
      "HashValues": [
        { "Key": 0,  "Value": "c0_r2" },
        { "Key": 1,  "Value": "c0_r4" }
      ]
    }
  ],
  "FuseMaps": [
    {
      "MapName": "CABISTRepair",
      "OutputType": "Dynamics",
      "InvertData": "True",
      "FuseHashReplacementInformation": [
        { "Keyword": "SliceMap2c", "DefectVariable": "slice" }
      ],
      "MapValues": [
        { "Size": 32,  "FuseName": "pma_mspmas0_cha_scf_scha_SliceMap2c/cha_fuses_0_GROUP7_1" },
        { "Size": 32,  "FuseName": "pma_mspmas0_cha_scf_scha_SliceMap2c/cha_fuses_0_GROUP7_2" },
        { "Size": 32,  "FuseName": "pma_mspmas0_cha_scf_scha_SliceMap2c/cha_fuses_0_GROUP7_3" }
      ]
    }
  ]
},
```
- _**GlobalMapping**_: Grouping field. Contains all the required information to process the fuse information and create a unique identifier to each fuse.
  - _**FuseHashes**_: Grouping field. Can contain multiple user defined hashes that will allow to create unique fuse names.
    - _**HashName**_: Unique hash identifier to indicate to the OutputFuse which information to use to make the fuse names unique.
    - _**HashValues**_: Grouping field. The purpose of thie field is to contain (Key, Value) pairs to make the fuses name unique.
      - _**Key**_: Value that must have the Defect Variable to use the corresponding hash value.
      - _**Value**_: String to be inserted into the fuse name when the Defect Variable has a certain value.
  - _**FuseMaps**_: Grouping field. Contains one or more output fuse maps define by the user; each of which describes the `Size` of the fuse value and the `FuseName` to be processed from the process resources results.
    - _**MapName**_: Indicates the name of the map to be pointed at by the ProcessResource.
    - _**OutputType**_: Sets the output type to provide information on how to process the fuse data. Options are: Static or Dynamic.
    - _**InvertData**_: When set to true, the generated data will be inverted for processing purposes.
    - _**FuseHashReplacementInformation**_: Grouping field. Contains all the information that it's relevant for making the fuse name unique.
      - _**Keyword**_: Corresponds to the keyword that must be searched and replaced in the fuse name.
      - _**DefectVariable**_: Provides the name of the defect variable to be used to find the correct hash value in the "Keyword" definition.
    - _**MapValues**_: Contains a list of fuse names with their corresponding size to be post-processed from the final output.
      - _**Size**_: Total size of the final fuse.
      - _**Default**_: Default value to be used to complete the corresponding size in case the final fuse does not fulfill the requirement.
      - _**FuseName**_: Unique name to correctly identify the generated fuse. This will later be shared using the FuseManager service.
      - _**SharedStorageName**_: Unique name to correctly identify the generated fuse. This will later be shared using the SharedStorage service.

### ProcessResources
```json
{
  "ProcessResources": [
    {
      "Resources": [
        { "Name": "REPNDTLTC" },
        { "Name": "REPNDTSTC" }
      ],
      "DynamicOutputs": [
        {
          "OutputFuse": "CABISTRepair",
          "Default": "0",
          "FuseIndex": "arraycode",
          "FuseData": "1,subary,wordline",
          "SizeMultipleOf": "64",
          "Direction": "MSB_LSB",
          "MapIndexToStartLocation":[
            { "index": 5,  "location": 7  },
            { "index": 6,  "location": 0  },
            { "index": 7,  "location": 20 }
          ]
        }
      ],
      "EnableCounterOutput": [
        {
          "OutputFuse": "CABISTRepairCFG",
          "Default": "0",
          "CounterLocation": "5-12",
          "EnableLocation": "17",
          "EnableValue": "1"
        }
      ]
    },
    {
      "Resources": [
        { "Name": "REPMLCDATROW" }
      ],
      "StaticOutputs": [
        {
          "OutputFuse": "CoreMLCDataRow",
          "Default": "0",
          "FuseIndex": "slice,block3,block2,spare",
          "FuseData": "1,wordline",
          "Direction": "MSB_LSB",
          "ReMapIndex":[
            { "Source": 11, "Target": 27 },
            { "Source": 19, "Target": 11 },
            { "Source": 27, "Target": 31 },
            { "Source": 31, "Target": 19 }
          ]
        }
      ]
    }
  ]
}
```

- _**ProcessResources**_: Grouping field. Contains all the relevant information to create and insert the fuse data into the respective binary string.
  - _**Resources**_: Field to provide the resource list to point at for the defect variables information.
    - _**Name**_: Name of the resource from which to obtain the repair solution.
  - _**DynamicOutputs**_: Grouping field. Dedicated section that contains the information for dynamically creating the final binary string.
    - _**OutputFuse**_: Points to a single `OutputFuseMap` to indicate how to process the fuses name and value.
    - _**FuseData**_: Fuse data format to be written down in the final binary string.
    - _**FuseIndex**_: Indicates the order in which the fuse data populates the targeted process resource. When set to default value **-1** the output will be populated sequentially.
    - _**SizeMultipleOf**_: Module size for each output. The total number if bits with information is either 0 or a multiple of `SizeMultipleOf`` value. This number is also used to determine the amount of default characters to add to comply with the expected fuse size.
    - _**InvertData**_: When set to true, the generated data will be inverted before inserting into the final value.
    - _**MapIndexToStartLocation**_: Grouping field. Used to indicate the start position for the fuse data represented by the index. <span style="color:Tomato">Not compatible with FuseIndex set to default value **-1**.</span> 
      - _**Index**_: Indicates which data needs to be repositioned to the new location.
      - _**StartLocation**_: Indicates the location bit from which to start the fuse data.
  - _**StaticOutputs**_: Grouping field. Dedicated section that contains the information for statically creating the final binary string.
    - _**OutputFuse**_: Points to a single `OutputFuseMap` to indicate how to process the fuses name and value.
    - _**FuseData**_: Fuse data format to be written down in the final binary string.
    - _**FuseIndex**_: Indicates the order in which the fuse data populates the targeted process resource. When set to default value **-1** the output will be populated sequentially.
    - _**InvertData**_: When set to true, the generated data will be inverted before inserting into the final value.
    - _**ReMapIndex**_: Grouping field. Used to provide the information to switch up fuse data values within the process resource. <span style="color:Tomato">Not compatible with FuseIndex set to default value **-1**.</span>
        - _**Source**_: Current index of the fuse data within the final output.
        - _**Target**_: New index of the fuse data within the final output.
  - _**EnableCounter**_: Grouping field. Use to track the process resource information either for `DynamicOutput` or `StaticOutput`.
    - _**OutputFuse**_: Points to a single `OutputFuseMap` to indicate how to process the fuses name and value.
    - _**CounterLocation**_: Field to provide in which set of bits to store the counter information.
    - _**EnableLocation**_: Field to provide in which bit to write the `EnableValue` to indicate the current repair fuse data is enabled.
    - _**EnableLocationValue**_: Information to be written in the `EnableLocation` bit.

## Connection between files
In order to provide a visual representation of how configuration files are connected with each other, the graphic below shows which object points to another information defined either in the resource file or the repair to fuse file.

![Static flow execute](./DocumentationFlows/FileConnection.png)

# Custom User Code hooks
The following is a list of all the customer's available extensions, which are called either during the Execute() of the base test method:

```csharp
/// <summary>
/// This function is called in the core of the execute flow.
/// It's intend is to provide the user a extension method to modify any step of the Static ProcessResource processing.
/// </summary>
/// <param name="processResource">Object that contains the list of resources and the list of dynamic outputs to be processed in order to create and insert the fuse data into the corresponding final binary strings.</param>
void ExecuteDynamicProcessResource(RepairToFuseFileDescriptor.ProcessResource processResource);

/// <summary>
/// This function is called in the core of the execute flow.
/// It's intend is to provide the user a extension method to modify any step of the Dynamic ProcessResource processing.
/// </summary>
/// <param name="processResource">Object that contains the list of resources and the list of static outputs to be processed in order to create and insert the fuse data into the corresponding final binary strings.</param>
void ExecuteStaticProcessResource(RepairToFuseFileDescriptor.ProcessResource processResource);

/// <summary>
/// This function is called at the end of the Execute flow.
/// <remarks>Default behavior: Creates the individual fuses by following the configuration defined in the RepairToFuse input file.
/// This result is then shared with either the FuseManager service or the SharedStorage service</remarks>
/// </summary>
void SharedIndividualFuses();
```

# Console output
During the execution flow the Test Method logs into the console which data is being process in order to keep track of this information in case of debugging.

The following snippet shows a real-life example of how the console will look like when debug level is enabled for RepairToFuse. 

Please note that this information is only printed when there are no exceptions in the execution of the test instance.

## Dynamic output console log
```
[2023-Aug-11 14:14:57.200][DUT: 1]--- DYNAMIC OUTPUT PROCESSING ---

 -D- FuseMap=[ PMUCSRowData ]
 -D- FuseData=[ slice,101,side ]
 -D- Hashes=[ SideMap4c ]
 -D- Resource=[ PMUCSRowData ]
 -D- RepairSolution=[ D7 ]
 -D- FuseGroups=[ 1101, 0111]

| FuseGroup |    Domain   |       Element        | Spare | FuseData | FuseIndex | HashKey |
===========================================================================================
|   1101    | [slice, 11] | [iosel, 0] [side, 1] |  00   |  110111  |    -1     |    1    |
|   0111    | [slice, 01] | [iosel, 1] [side, 1] |  00   |  110110  |    -1     |    1    |
```

## Static output console log
```
[2023-Aug-11 14:14:57.206][DUT: 1]--- STATIC OUTPUT PROCESSING ---

 -D- FuseMap=[ PMUCSColData ]
 -D- FuseData=[ iosel,slice,spare ]
 -D- Hashes=[ SliceMap4c ]
 -D- Resource=[ PMUCSColData ]
 -D- RepairSolution=[ 0000000100001110100101011 ]
 -D- FuseGroups=[ 00000, 00100, 00111, 01001, 01011]

| FuseGroup |    Domain   |        Element        | Spare | FuseData | FuseIndex | HashKey |
============================================================================================
|   00000   | [slice, 00] | [iosel, 00] [side, 0] |   00  |  000000  |     0     |    0    |
|   00100   | [slice, 00] | [iosel, 10] [side, 0] |   01  |  100001  |     2     |    0    |
|   00111   | [slice, 00] | [iosel, 11] [side, 1] |   10  |  110010  |     3     |    0    |
|   01001   | [slice, 01] | [iosel, 00] [side, 1] |   00  |  000100  |     0     |    1    |
|   01011   | [slice, 01] | [iosel, 01] [side, 1] |   01  |  010101  |     1     |    1    |
```

# TPL Samples

The following example shows a test instance that points to the mandatory input files using the namespace U1. Notice that for this instance the parameter `EnableFuseReset` is set to used its default value TRUE.

```python
PrimeRepairToFuseTestMethod R2F_methodology_P1
{
  ResourcesFilePath = "resources.json";
  RepairToFuseFilePath = "repairToFuse.json";
  FuseNamespace = "U1";
}
```

# Exit ports

| Exit port | Condition  | Description |
|-----------|------------|-------------|
| -1 | Error | Error due to any of the exceptions described in [Execute Exceptions section](#131-execute-exceptions) |
| 1 | Pass | Passing condition |

# Acronyms
- **R2F:** Repair To Fuse
