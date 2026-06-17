[[_TOC_]]

## REP for ArrayFusing

This **REP** is intended to describe the ArrayFusing Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Console output** – A detailed description of what is printed to console by this TestMethod

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Acronyms** - Definition of acronyms used in this document

  - **Version tracking** – With author names, so you always have a name to address
  
## Methodology

The ArrayFusing test method provides the capability to pattern-modify fuses according to previous repair results.
These repair results are imported from DFF or SharedStorage and converted to binary string according to the json file definitions.
As shown in the below plot, the ResourceToFuseDFF.json file contains the mapping information for repair resource and dff.
It also contains the info to generate fuse bit strings.
Once we have the fuse bit strings, we need the fuse related json files to help us do pattern modify.
Lastly, the test method has the capability to upload to DFF/sharedStorage. 
The scenario could be that the user gets repair solutions from sharedStorage and then at the end upload it to DFF.
Or it can be the other way around, the user gets repair solutions from DFF and then save it to sharedStorage at the end. 
Please refer to **Potential Usage Examples** session below for more details.

**Notes:**
- The fuse related json files (fusedef.json, fuseconfig.json, and fuseDataLabel.json) parsing is done during the Init flow via Aleph.
Please refer to PatConfig Service REP for more details.

![image.png](./.attachments/ArrayFusing/ArrayFusingHighLevel.png)

Let's take a step back and understand why we need this test method rather than integrating the function in Array Repair Test Method.
The main reason is to run iCLYA in Evergreen (or Prime LYA in the future) before the fusing happens.
This is to make sure the LYA (low yield analysis) get the unrepaired signature. 
The picture below show the potential flow when we execute Array Module. 
At the beginning of the flow, ArrayFusing may start fresh or start with one of the two setup below:
1. ArrayFusing gets previous repair values from DFF.
2. ArrayFusing gets previous repair values from SharedStorage.

Then at the end of the ArrayFusing execution, it may pick one of the two flows below:

3. ArrayFusing saves repair values that it fuses to DFF.
4. ArrayFusing saves repair values that it fuses to SharedStorage.

During the ArrayRepair execution, it will do:

5. Getting repair values in previous execution from SharedStorage.
6. Setting repair values found in the current execution to SharedStorage. 

With the help of DFF Database and sharedStorage, we are able to pass on the repair solution from test instance to test instance and most importantly, from socket to sockets.

![image.png](./.attachments/ArrayFusing/ArrayFusingAndArrayRepairOverall.png)

### Potential Usage Examples

#### Code Socket Example
ArrayFusing - Choose "default mode" (no previous solutions) and initialize the fuse string to default bits according to fuse json files. 
At the end it saves nothing to sharedStorage (see 4 above) because no previous solutions are imported.   
&#8595;  
ArrayRepair - Import from sharedStorage (see 5 above) but there is nothing as the test just started.
Then run repair algorithm and find repair solutions to save them to sharedStorage (see 6 above).   
&#8595;  
iCLYA - This is the test from EVG. In order to run iCLYA correctly, we cannot patmod to solution. That's the main reason why we split the ArrayFusing and ArrayRepair.  
&#8595;  
ArrayFusing - Choose "import from sharedStorage mode" (see 2 above) and set the fuse string according to the repair solutions.
Then convert those fuse string to DFF format and upload it. (see 3 above)

#### Hot Socket Example
ArrayFusing - Since we have previous solution from Cold socket, choose "Import from Dff mode" (see 1 above) and convert the Dff values to repair solutions. 
Now that we have the previous solutions, we can save it to sharedStorage (see 4 above) and use it to create fuse string and apply to it.  
&#8595;  
ArrayRepair - Import from sharedStorage (see 5 above) to get the solution that we just saved.
Mark those solution as used resource and then run repair algorithm to find newer repair solutions. Again saving it to sharedStorage (see 6 above).   
&#8595;  
iCLYA - Same as the cold socket example.  
&#8595;  
ArrayFusing - Choose "import from sharedStorage mode" (see 2 above) and set the fuse string according to the repair solutions.
Then convert those fuse string to DFF format and upload it (see 3 above)

### Verify

  - Validate Resource to Fuse Json File

  - Create Pattern Config Handler according to fuse config related files.
  
  - Initialize Fuse String according to fuse config related json files. 
  
### Execute

  - Import Solution - 4 modes to import solution from:
    - Default mode (import nothing). 
    - SharedStorage mode (import from Dut level key - ARRAY_REPAIR_SOLUTION, that is automatically created by ArrayRepair TestMethod). 
    - Dff mode (the dff token will be provided in the FuseResource json file).
    - Combined SharedStorage mode and Dff mode. Dff and SharedStorage values are combined using a bitwise OR in their binary representation.

  - Write Used Resources Status to SharedStorage
    - After picking the mode for import solution, we will save the solution to sharedStorage key (ARRAY_FUSING_USED_RESOURCES).
  
  - Write Fuses To Dff
    - We also convert the solution to DFF if user select the mode to upload to Dff (similarly, Dff tokens are all defined in FuseResource json file).

  - Apply Fuses 
    - Apply pattern modify for the fuse config handles.
    - If user chooses "IMPORT_SOLUTION_FROM_ARRAYREPAIR_AND_APPLY" mode, then the pattern modify will be applied to all the plist that has the matching PatternsRegEx in fuse config file.
    The reason to apply to all regex matching plist is because users are expected to run more test down the flow. It would be better to fix all the matching patterns.
    - Otherwise it only pattern modify the plist file user mentioned in the instance parameter.
  
  - Reset SharedStorage
    - value in the sharedStorage key (ARRAY_REPAIR_SOLUTION) will be wiped clean.
    
For detail explanation of fuse bit string, please click the [download link](./.attachments/ArrayFusing/ArrayFusing/ArrayFusing.pptx).

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ArrayFusing test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                                                                                                                                            | **Comments*                        |
| ------------------ | ------------- | --------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------| ---------------------------------- |
| Patlist            | Yes           | Plist           | Patlist for setting up the fuse handles.                                                                                                              |                                    |
| FusingMode         | Yes           | String (choice) | RESET_ALL_RESOURCES<br>IMPORT_SOLUTION_FROM_DFF_AND_APPLY<br>IMPORT_SOLUTION_FROM_ARRAYREPAIR_AND_APPLY<br>IMPORT_SOLUTION_FROM_STORAGE_DFF_AND_APPLY |                                    |
| DffUploadMode      | Yes           | String (choice) | ENABLED, DISABLED                                                                                                                                     |                                    |
| FuseResourceFile   | Yes           | File            | Fuse Resource Json file path                                                                                                                          |                                    |

**Notes:**
- The Fuse Resource JSON file parsing is done during the template Verify.

- Fuse Resources File Example:
```
{
	"FuseToDFF":
	[
		{
			"Fuse": "ArrayRepair_Fuse1",
			"DFF": "TOKENA"
		},
		{
			"Fuse": "ArrayRepair_Fuse2",
			"DFF": "TOKENB"
		}
	],
	"ResourceToFuse":
	[
		{
			"ResourceName":"S1|HS1|Q1|Row1",
			"Fuse":"ArrayRepair_Fuse1",
			"DataBits":[0,2,4],
			"EnableBits": [6]
		},
		{
			"ResourceName":"S1|HS1|Q2|Col1",
			"Fuse":"ArrayRepair_Fuse1",
			"DataBits":[1,3,5],
			"EnableBits": [7]
		},
		{
			"ResourceName":"S1|HS1|Row1",
			"Fuse":"ArrayRepair_Fuse2",
			"DataBits":[0,2,4],
			"EnableBits": [6]
		},
		{
			"ResourceName":"S1|HS2|Q1|Col1",
			"Fuse":"ArrayRepair_Fuse2",
			"DataBits":[1,3,5],
			"EnableBits": [7]
		}

    ]
}
```
- Fuse Resources Details:

| **JSON Object**                  | **Mandatory Properties**                 | **Description**                                                | **Notes**                                                                                            |
| -------------------------------- | ---------------------------------------- | -------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------- |
| **FuseToDFF**                    | Fuse,DFF                                 | Provide fuse and dff relationship.                             | Has to be 1 to 1 mapping.                                                                            |
|   &nbsp;&nbsp;&nbsp;Fuse         |                                          | Unique fuse name for each dff token.                           | Same fuse name that is used as the ResourceToFuse property.                                          |
|   &nbsp;&nbsp;&nbsp;DFF          |                                          | Unique dff token name to save compressed fuse string.          | Dff value store the hex value of the original fuse binary string.                                    |
| **ResourceToFuse**               | ResourceName, Fuse, DataBits, EnableBits | Provide fuse and repair resource relationship.                 | Each relationship is unique by ResourceName, ie. no duplicate ResourceName.                          |
|   &nbsp;&nbsp;&nbsp;ResourceName |                                          | Name of the repair resource.                                   | ResourceName should match the ArrayRepair resource json file.                                        |
|   &nbsp;&nbsp;&nbsp;Fuse         |                                          | Name of the fuse.                                              | Fuse is composed of many resources and is interleaving by data/enable bits.                          |
|   &nbsp;&nbsp;&nbsp;DataBits     |                                          | list of dataBits in the fuse bit string. (separated by comma)  | Combining the values in the corresponding bit locations represent a solution value (resource value). |
|   &nbsp;&nbsp;&nbsp;EnableBits   |                                          | list of enableBits in the fuse bit string. (separated by comma)| All the values in the corresponding bit locations has to be 1 to represent enable (resource used).   |

## Console output (debug mode)

Console window will show the fuse data string and the config handles.

![image.png](./.attachments/ArrayFusing/Console1.png)


## TPL Samples

Here is a test instance example using the ArrayFusing test method

```python
Import PrimeArrayFusingTestMethod.xml;

Test PrimeArrayFusingTestMethod ArrayFusing_Reset_P1
{
   Patlist = "ArrayRepair_plist";
   LogLevel = "PRIME_DEBUG";
   FuseResourceFile = "~HDMT_TPL_DIR/Modules/ArrayRepair/ArrayRepair/InputFiles/test.ResourceToFuseDFF.json";
   DffUploadMode = "DISABLED";
   FusingMode = "RESET_ALL_RESOURCES";
}
```

## Exit Ports

The test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                              |
| ------------- | ------------- | -------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                          |
| **-1**        | ***Error***   | Any software condition error                 |
| **0**         | ***Fail***    | Failing condition.                           |
| **1**         | ***Pass***    | Passing condition                            |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TPL**: Test Programming Language


## Version tracking

| **Date**                  | **Version** | **Author**           | **Comments**    |
| ------------------------- | ----------- | -------------------- | --------------- |
| Apr 27<sup>th</sup>, 2022 | 1.0.0       | Chun-Yu (Joseph) Yang| Initial version |