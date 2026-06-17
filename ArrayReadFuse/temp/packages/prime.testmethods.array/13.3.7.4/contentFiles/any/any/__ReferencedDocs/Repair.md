[[_TOC_]]

## REP for Repair
This **REP** is intended to describe the Prime Repair TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Input files** – A detailed description of the Json input files.

  - **Custom User Code Hooks** – A detailed description of custom user code.

  - **Console output** – A detailed description of what is printed to console by this TestMethod

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Acronyms** - Definition of acronyms used in this document

  - **Version tracking** – With author names, so you always have a name to address

## Methodology
The Repair test method is intended to take care of all the array repair and raster post capture. 

> <span style="color:tomato">**NOTE:**</span> This test method is designed to malfunction without being extended to implement the decoding parser and the array mapper.

## Test Instance Parameters
The table below lists and describes the test instance parameters supported by the Repair test method

| **Parameter Name** | **Required?** | **Type**        | **Description/Values**                                                                 | **Comments*                                                                                                                   |
| ------------------ | ------------- | --------------- |----------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------|
| ResourceFile | Only for Repair mode | File | Resource Json file path | Required parameter for Repair mode. Test Method is expected to fail when value is empty or does not point to an existing file |
| ArrayFile | Yes | File | Array Json file path | Required parameter. Test Method is expected to fail when value does not point to an existing file                             |
| TargetArray | Yes | String | Array name that will be used by the current instance |                                                                                                                               |
| DecoderMatchLabel | Yes | String | Selects set of decoding parameters to use from the array file |                                                                                                                               |
| DataLog | No | String (choice) | Selects the output type in which to log the instance results | **Options are:** ItuffOnly (_default_), TFile, RFileRaw, RFileReduced, LYA                                                    |
| LyaCellSelection  | Only when `DataLog` is set to Lya | String (choice) | LYA cell select algorithm | **Options are:** FirstCell (_default_), LastCell                                                                              |
| OperationMode | No | String (choice) | Selects the instance operation mode | **Options are:** Repair (_default_), Raster                                                                                   |
| AlgorithmPriority | No | String (choice) | Selects the prioritization of the repair algorithm criteria | **Options are:** Default (_default_), NoSpares, ResourceOnly <br/> Used for multi-dimensional algorithm                       |
| BaseNumber | Only when `DataLog` is set to: RFileRaw, RFileReduced or Lya | String | Base number to be used in output datalog | Default value is empty                                                                                                        |
| InputStorageKey | No | String | Shared storage key or user var that contains the input data | Default value is empty. <br/> Mutually exclusive with InputForDebug                                                           |
| DeleteInputStorage | No | String (choice) | Indicates whether to delete the InputStorageKey content before instance exection ends | **Options are:** Disabled (_default_), Enabled                                                                                |
| InputForDebug | No | String | Shared storage key or user var that contains the input data. Format is: ssi,set,way,bit\|set,way,bit,ssi\|... | Default value is empty. <br/> Mutually exclusive with InputStorageKey                                                         |
| DecodingMode | No | String | Sets the decoding mode to be used in the ITUFF printing data | Default value is UndefinedDecodingType                                                                                        |

## Input files
The Repair test method consumes two Json configuration files (Resource and Array).

### Resource File
Example file:
```json
{
  "Resources": [
    {
      "Name": "LDB",
      "Spares": 6,
      "DefectVars": {
        "Domain": [
          { "Name": "dummyB", "Quantity": 64, "SubStructureIdentifier": "true" },
          { "Name": "quad", "Quantity": 4, "EnableValue": [ 0, 7, 9] }
        ],
        "Element": [
          { "Name": "quadio", "Quantity": 127, "Ignore": "true" },
          { "Name": "bank", "Quantity": 31 },
          { "Name": "side", "Quantity": 2, "Compare": "False" },
          { "Name": "wordline", "Quantity": 255 },
          { "Name": "colsel", "Quantity": 4 }
        ]
      }
    }
  ]
}
```

- _**Resources**_: Grouping field. Used to provide resource definitions.
  - _**Name**_: Field to provide the resource name.
  - _**Spares**_: Field to provide the amount of available spares.
  - _**DefectVars**_: Grouping field. Used to provide the resource variables into the Domain and Element arrays.
    - _**Domain**_: Grouping field. Used to provide the list of variables considered as Domain.
      - _**Name**_: Field to provide the variable name.
      - _**Quantity**_: Field to provide the variable quantity.
      - _**SubStructureIdentifier**_: Field to provide a flag indicating wether or not the variable is a tracking variable.
      - _**EnableValue**_: Field to provide a list of unsigned integers to be used as enable values.
      - _**Ignore**_: Field to provide a value indicating if the defect variable should be ignored.
    - _**Element**_: Grouping field. Used to provide the list the variables considered as Element.
      - _**Name**_: Field to provide the variable name.
      - _**Quantity**_: Field to provide the variable quantity.
      - _**SubStructureIdentifier**_: Field to provide a flag indicating wether or not the variable is a tracking variable.
      - _**EnableValue**_: Field to provide a list of unsigned integers to be used as enable values.
      - _**Ignore**_: Field to provide a value indicating if the defect variable should be ignored.
      - _**Compare**_: Field to provide a value indicating if the defect variable should be used to define incrementalRepair, Default value **TRUE**.

### Array File
Example file:
```json
{
  "Arrays": [
    {
      "Name":"llcdat",
      "SkipIncrementalRepairCheck": false,
      "Globals": {
        "DefectMatrixMaxSize":5000,
        "SubStructure": {
          "Quantity": 40,
          "Order": "MsbL",
          "DisabledTracking": "DisableSsiForRepairKey",
          "RepairedTracking": "RepairedSsiKey",
          "UnrepairableTracking": "UnrepairableSsiKey"
        }
      },
      "TFileFormats": [
        { "OutputVariable":"halfio", "Format":"Hex", "BitLength":255 }
      ],
      "RasterParameters": {
        "Rows":511,
        "Cols":303,
        "RowDec":24,
        "ColDec":20,
        "RowPF":3,
        "ColPF":3,
        "MAF":1024,
        "MAS":0,
        "BitDensity":0.33
      },
      "DecodingParameters": [
        {
          "MatchLabel": "Default",
          "ResourceList":[
            "LDR",
            "LDC",
            "LDB"
          ],
          "Parameters":[
            { "Name":"disable_ssi", "Value":"true" },
          ]
        }
      ],
      "DirectArrayMapRanges": {
        "Block": {
          "Min": 0,
          "Max": 999
        },
        "Row": {
          "Min": 0,
          "Max": 999
        },
        "Column": {
          "Min": 0,
          "Max": 999
        }
      }
    }
  ]
}
```

- _**Arrays**_: Grouping field. Used to provide array definitions.
  - _**Name**_: Field to provide the array name.
  - _**SkipIncrementalRepairCheck**_: Optional field. Used to ignore the unrepairable defects with reason as `ConditionalFail`. They will not be taken into account as unrepairable defects but will still be used to select the instance's exit port. Default value is set to false.
  - _**Globals**_: Grouping field. Used to indicate the resource global variables.
    - _**DefectMatrixMaxSize**_: Field to provide the maximun number of defects Repair will process, the instance will process up to this limit. In case the defects exceed this value, the excess of defects will be ignored.
    - _**SubStructure**_: Grouping field. Used to provide the substructure identifying tracking information
      - _**Quantity**_: Field to provide the maximun value of indexes to track.
      - _**Order**_: Field to provide where the most significan bit is located (MsbL or MsbR).
      - _**DisabledTracking**_: Field to provide the storage key for the disabled tracking.
      - _**RepairedTracking**_: Field to provide the storage key for the repaired tracking.
      - _**UnrepairableTracking**_: Field to provide the storage key for the unrepairable tracking.
  - _**TFileFormats**_: Grouping field, required when DataLog is TFile. Used to indicate which variable is going to be datalog in the T-file and in which format.
    - _**OutputVariable**_: Field to provide the defect variable name to be log in the TFile.
	- _**Format**_: Optional field. Field to provide the variable format: Bin, Dec and Hex. Default value is Bin.
	- _**BitLength**_: Required field when Format is Bin or Hex. Field to provide the variable bit length of the value.
  - _**RasterParameters**_: Grouping field, required when DataLog is RFileReduced or LYA. Used to provide the raster parameters.
    - _**Rows**_: Field to provide the quantity of rows.
	- _**Cols**_: Field to provide the quantity of columns.
	- _**RowDec**_: Field to provide the row decoders.
	- _**ColDec**_: Field to provide the column decoders.
	- _**RowPF**_: Field to provide the row vs single bit threshold.
    - _**ColPF**_: Field to provide the column vs single bit threshold.
	- _**MAF**_: Field to provide the massive array fail.
	- _**MAS**_: Field to provide the massive array success.
	- _**BitDensity**_: Field to provide the bit density.
  - _**DecodingParameters**_: Grouping field. Used to indicate the decoding parameters.
	  - _**MatchLabel**_: Optional field. Field to distinguish between different sets of decoding parameters.
    - _**ResourceList**_: Field to provide the resource name list to be used.
    - _**Parameters**_: Grouping field. Indicates the decoding parameters.
      - _**Name**_: Field to provide the name variable.
	  - _**Value**_: Field to provide the string value.
	  - _**Bits**_: Field to provide the bit range. Example: "0-3,5,7,10-14".
  - _**DirectArrayMapRanges**_: Grouping field. Optional range definition to define min and max values for direct array variables.
    - _**Block**_: Contains min and max values for Block direct array variable.
    - _**Row**_: Contains min and max values for Row direct array variable.
    - _**Column**_: Contains min and max values for Column direct array variable.


# Custom User Code Hooks
The following is a list of all the customer's available extensions, which are called either during Verify() or Execute() of the base test method:

To get a description of each of the available extension functions, the corresponding interface definition file can be consulted directly in the source code released with prime:
```
    [prime_release_path]\src\TestMethods\Array\Source\Repair\PrimeRepairTestMethod.cs
```

## Verify Extensions
```csharp
/// <summary>
/// Creates the IArray implementation.
/// </summary>
/// <remarks>This extension is called during the Verify() method and its purpose is to populate the target array VariableMapper.</remarks>
/// <returns>Object that implements IArray interface.</returns>
IArray CreateArray();

/// <summary>
/// The extending logic will set the parameters required by the user to perform raster operation mode.
/// </summary>
/// <param name="parameters">Decoding parameter's information.</param>
/// <remarks>
/// <para>This extension is called during the Verify() method.</para>
/// <para>User can access ParameterParser class and used it to set the decoder parameter values.</para>
/// </remarks>
void SetDecodingParameters(List<(string name, string bits, string value)> parameters);
```

## Execute Extensions
```csharp
/// <summary>
/// Called prior to any test in Execute() method to generate the test instance results object.
/// </summary>
/// <remarks>By using this object, user can have access to the following information.
/// <list type = "bullet" >
/// <item>
/// <term>RemainingResources</term>
/// <description>DataTable object that stores the resulted spares per resource.</description>
/// </item>
/// <item>
/// <term>UnrepairableDefects</term>
/// <description>List containing the unrepairable defect ID and the unrepairable reason.</description>
/// </item>
/// <item>
/// <term>Defects</term>
/// <description>List of IRepairableVariables that contains all defect definitions parsed by the current test instance.</description>
/// </item>
/// <item>
/// <term>ExitPort</term>
/// <description>Outgoing exit port for the current test instance.</description>
/// </item>
/// </list>
/// </remarks>
/// <returns>The test instance results object.</returns>
RepairTestInstanceResults CreateTestInstanceResults();

/// <summary>
/// Gets the logical addresses for the incoming defects.
/// </summary>
/// <param name="reverseIncomeData">Indicates weather to reverse or not the income data. Default value is set to TRUE.</param>
/// <returns>A list of IRepairVariables containing the logical address information for each defect.</returns>
List<IRepairVariables> GetLogicalAddresses(bool reverseIncomeData = true);

/// <summary>
/// Executes the repair algorithm.
/// </summary>
/// <param name="solutionTable">Table to apply the algorithm on.</param>
/// <param name="sparesByResource">Table that indicates the spares by resource.</param>
/// <param name="sparesByDomain">Dictionary to map the defect domain groups per resource.</param>
/// <param name="ignoreDefectGroupsPerResource">Dictionary to map the defect ignore groups per resource.</param>
/// <param name="algorithmPriority">Enum to prioritize MaxDefects, remaining spare quantity, and resource order in repair algorithm.</param>
/// <returns>A tuple that contains the used resources with its value, the resultant spare by resources, and a table of unrepairable defects.</returns>
(List<(string name, uint value, List<uint> defectIds)> usedResources, DataTable remainingSpareResources, List<Tuple<uint, string>> unrepairableDefects) ExecuteRepairAlgorithm(DataTable solutionTable, DataTable sparesByResource, Dictionary<string, List<string>> sparesByDomain, Dictionary<string, List<string>> ignoreDefectGroupsPerResource, PrimeRepairTestMethod.RepairAlgorithmPriority algorithmPriority);

/// <summary>
/// Called right after successful execution to provide the user the ability to post process exit port, scan fail results, and functional result.
/// </summary>
/// <param name="results">The object for holding the exit port, scan fail results, and functional result.</param>
void CustomPostProcessResults(RepairTestInstanceResults results);

/// <summary>
/// Called after the parsing of Logical Addresses from raw failing data.
/// User implements any logic specific to printing their failing address to ituff.
/// </summary>
/// <param name="logicalAddresses">Logical addresses that will be logged to ituff.</param>
void LogLogicalAddressToItuff(List<IRepairVariables> logicalAddresses);

/// <summary>
/// Called after the parsing of Logical Addresses from raw failing data and logging to ituff.
/// User implements any logic specific to printing their failing address to the T-file.
/// </summary>
/// <param name="logicalAddresses">Logical addresses that will be logged to T-file.</param>
void LogLogicalAddressToTFile(List<IRepairVariables> logicalAddresses);
```

# Multidimensional Algorithm
The extension ExecuteRepairAlgorithm is designed to be replaced as needed.  By default, it calls the generic multi-dimensional algorithm which is described here.

Inputs
  - Solution table of the possible repair solutions per resource for each found defect.  A repair solution is basically a decimal coding of resource element settings necessary for the repair.
  - Spare table with the number of available spares per resource for each found defect.
  - Lookup table showing domain linkages between solutions.  Meaning some solutions may share a pool of spare resources.
  - Repair priority selection from AlgorithmPriority parameter.
```
Solutions                Spares
DefID Res1 Res2 Res3     Res1 Res2 Res3
  0     0   24   14        1    2    3
  1     0    0    0        1    2    3
  2     2    0    4        1    2    3
  3     0   16   12        1    2    3

DomainLinks
Key   Value(DefectIDs)
Res1  {"0,1,2,3"} - means the solutions using this resource must share the same spare quantity
Res2  {"0","1","2","3"} - means the spare quantity is unique for each solution under this resource
Res3  <> - no entry means same as in Resource2 above
```

The algorithm itself changes depending on the AlgorithmPriority parameter as below
  - If "Default" is selected, the algorithm will first select solutions that will repair the most number of defects, next will select based on the highest number of available spares,
  and then will select in resource priority order (essentially left to right columns).
  - If "NoSpares" is selected, the algorithm will first select solutions that will repair the most number of defects, and then will select in resource priority order.
  - If "ResourceOnly" is selected, only the resource priority order is used (clean out all the spares in the first resource before moving on).
```
"Default" mode selected for example.

Solution "0" from Resource1 repairs three defects (0, 1, 3).
Because the spares in Resource1 are in the same domain, there is no spare available for the last defect.
Therefore, Solution "4" is selected to repair the last defect (2).
Resource3 spares are not linked, so only the one row is decremented.

Solutions                Spares
DefID Res1 Res2 Res3     Res1 Res2 Res3
  0    *0*  24   14       *0*   2    3
  1    *0*   0    0       *0*   2    3
  2     2    0   *4*      *0*   2   *2*
  3    *0*  16   12       *0*   2    3
```

Outputs
  - List of the chosen repair solutions and their matching resource name and defect ID.
  - Same spare table with the updated number of available spares per resource.
  - Solution table showing only the remaining defects that could not be repaired via the chosen algorithm.
```
UsedResources
Index ResourceName Solution DefectID
  0       Res1         0        0
  1       Res3         4        2

Spares                   UnrepairableDefects (empty in this example)
DefID Res1 Res2 Res3     Res1 Res2 Res3
  0     0    2    3        x    x    x
  1     0    2    3
  2     0    2    2
  3     0    2    3
```

# IncrementalRepair
The incrementalRepair occurs when there is a pre-existing raster solution that matches the defect under analysis.

There are two types of IncrementalRepair:
- **Full**: When the defect matches all elements of a previous solution.
- **Partial**: When there are differences only in the elements defined with the parameter Compare="False".

### Ituff OutPut Syntax
The string value syntax for address matching is as follows:
```
2_tname_<Instance Name>_CMPR_IR
2_strgval_AdrMatch|F<FuseDecValue>|<partial or full>|<LA_address>|<resource_name>|F...
```
Multiple results are delimited by “|F”.

## TPL Samples
Here is a test instance example using the ArrayFusing test method

```python
Import PrimeRepairTestMethod.xml;

Test PrimeRepairTestMethod Example_P1
{
   ArrayFile = "~HDMT_TPL_DIR/Modules/ArrayRepair/ArrayRepair/InputFiles/Repair/arrays.json";
   TargetArray = "llcdat";
   LogLevel = "PRIME_DEBUG";
   DataLog = "RFileReduced";
   OperationMode = "Raster";
   BaseNumber = "10210";
   InputStorageKey = "ArrayRepair::_UserVars.ARR_RASTER1";
   DeleteInputStorage = "true";
}
```

## Exit Ports
The test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                              |
| ------------- | ------------- | -------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition.                          |
| **-1**        | ***Error***   | Any software condition error.                 |
| **0**         | ***Fail***    | Failing condition.                           |
| **1**         | ***Pass***    | OperationMode parameter set to "Raster" or no defects found when OperationMode is "Repair".                           |
| **2**         | ***Pass***    | All defects found are repairable.                            |
| **3**         | ***Pass***    | There are both repairable and unrepairable defects.                            |
| **4**         | ***Pass***    | All defects found are unrepairable.                            |

**Notes**: 
- Whether defects are found or not in Raster OperationMode, the passing exit port is always going to be port 1.  
- Exit ports 2, 3 and 4 are exclusively for Repair OperationMode. 
- Executing this test method with multiple SSIs and exiting through ports 2, 3, or 4 indicates that at least one SSI has defects, even if other SSIs have no defects.

## Acronyms
Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TPL**: Test Programming Language
  - **LA**:	Logical Address format
  - **DA**: Physical Direct Array or D-array address format referred as DRC
  - **DRC**: D-array Block, Row, Column.
  - **Defect**:	Physical Address or “Defect” variables, previously known as BSV’s.
  - **L2P**: LA to Defect, but also used to further map to DA.
  - **P2L**: Defect to logical, but also used to map from DA2Defect2LA.
  - **L2D2L**: LA to D-array Address to LA selfcheck. (LA2Defect2DA2Defect2LA)
  - **D2L2D**: DA to Logical Address to DA selfcheck. (DA2Defect2LA2Defect2DA)
  - **SSI**: Sub-Structure Identifier.
  - **IR**: IncrementalRepair.
