[[_TOC_]]

## REP for Lya
This **REP** is intended to describe the Prime Lya Test Method.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this Test Method intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Input files** – A detailed description of the Json input files.

  - **Custom User Code Hooks** – A detailed description of custom user code.

  - **Console output** – A detailed description of what is printed to console by this Test Method

  - **TPL Samples** – Examples of how to use this Test Method in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Acronyms** - Definition of acronyms used in this document

  - **Version tracking** – With author names, so you always have a name to address
  
## Methodology
The Lya Test Method is intended to take care of all the array low yield analysis. This Test Method is designed to malfunction if not extended to implement the decoding parser and the array mapper.

This methodology can be executed in any of the three available execution modes: Manual, Manufacturing, GoodCell.

### Manual Execution Mode
Specifically designed for debug support by directly specifying the list of variables to be used for pattern modify. This mode allows the user to run the test instance in a standalone manner by removing all external dependencies and only relying on the input configuration file.

### Manufacturing Execution Mode
Default execution mode.

### GoodCell Execution Mode
This mode has the capability to indicate the input coordinates by using either logical or physical address. Finally, regardless of the address format being used in the input coordinates, the datalog output will always be reported in the form of physical address.

## Test Instance Parameters
The table below lists and describes the test instance parameters supported by the Lya test method

| **Parameter Name**   | **Required?** | **Type**        | **Description/Values**                                                                | **Comments*                                          |
| ------------------   | ------------- | --------------- | ------------------------------------------------------------------------------------- | -----------------------------------------------------|
| TimingsTc            | Yes           | TimingCondition | Timing test condition to be applied prior to pattern execution                        |                                                      |
| LevelsTc             | Yes           | LevelsCondition | Levels test condition to be applied prior to pattern execution                        |                                                      |
| LyaConfigFile        | Yes           | File            | Lya configuration file path                                                           |                                                      |
| BitLinePin           | Yes           | String          | Pin name which acts as Bit Line                                                       |                                                      |
| BitLineBarPin        | Yes           | String          | Pin name which acts as Bit Line Bar                                                   |                                                      |
| VccPin               | Yes           | String          | Pin name of VCC pin to base high force value on                                       |                                                      |
| VccMaxLevel          | No            | Double          | Maximum value to set high force value to                                              | Default is -555.5                                    |
| BaseNumber           | No            | String          | Base number to be used in output datalog                                              | Default is 0                                         |
| MeasurementThreshold | Yes           | Integer         | Threshold value that creates a range to determine the validity of the DC measurements | Measurements outside of range are considered a valid |
| TargetArray          | Yes           | String          | JSON configuration set name                                                           |                                                      |
| ExecutionMode        | No            | String (choice) | "Manual", "GoodCell", "Manufacturing"                                                 | Default is "Manufacturing"                           |
| MaxLyaCount          | No            | Integer         | Maximum number of LYA tests per instance allowed                                      | Default is 99999                                     |

### Exit port conditions
The test instance parameters `MeasurementThreshold` and `MaxLyaCount` define the exit port of the LYA test instance. 

The exit conditions priority is defined as follows:
1. If all LYA measurements are within the range defined by the `MeasurementThreshold` parameter, then measurements are considered invalid and exit port is 3.
2. If the count of cells tested exceeds the max value defined by `MaxLyaCount` parameter, then exit port is 2.
3. If none of the above conditions are fulfilled then exit port is 1.

The image presented below shows a graphic representation of how the exit port is selected.

![alt text](./Attachments/ExitPorts.png)

### Voltage Force
LYA Test Methods determines which voltages will be applied to obtain the LYA measurements. The test instance parameters used make this decision are:

* VccPin
* VccMaxLevel

By default, the min voltage will be always 0; max voltage will be taken from the VccPin - VForce attribute value. This will be overwritten by the VccMaxLevel value only when defined as an instance parameter.

```python
  ...
  VccPin = "HDDPS_HC_gangx2_0p075ohm1";
  VccMaxLevel = 1.0;
  ...
```

This range will later be splitted to obtain a list of voltages corresponding to the number of points defined in the configuration file. For the above example if `NumPoints=11` then the ForceVoltages will be:

* 0.0
* 0.1
* 0.2
* 0.3
* 0.4
* 0.5
* 0.6
* 0.7
* 0.8
* 0.9
* 1.0

## Input files
The LYA testmethod consumes one JSON configuration file.

Example file:
```json
{
  "LyaConfigurations": [
  {
    "ArrayName": "Array1",
    "PatternModifyGlobals": [
      {
        "Name": "Global1",
        "Size": 9,
        "DefectVariable": "WordLine",
        "Value": 5
      },
      {
        "Name": "Global2",
        "Size": 4,
        "Value": 10
      },
      {
        "Name": "Global3",
        "Size": 10,
        "Value": 10
      },
      {
        "Name": "Global4",
        "Size": 4,
        "Value": 12
      }
    ],
    "IoPatternModify": {
      "IoSize": 40,
      "Direction": "msb_lsb",
      "BitLocation": "Global4",
      "Enable": true,
      "Labels": [
        {
          "Name": "DATA_IN_0",
          "Size": 10,
          "Pingroup": "xxHPCC_DPIN_Dig_slcA_A0"
        },
        {
          "Name": "DATA_IN_1",
          "Size": 10,
          "Pingroup": "xxHPCC_DPIN_Dig_slcA_A1"
        },
        {
          "Name": "DATA_IN_2",
          "Size": 10,
          "Pingroup": "xxHPCC_DPIN_Dig_slcA_A2"
        },
        {
          "Name": "DATA_IN_3",
          "Size": 10,
          "Pingroup": "xxHPCC_DPIN_Dig_slcA_A3",
          "Offset": 1
        }
      ]
    },
    "GoodCells": [
      {
        "Ssi": 1,
        "Block": 1,
        "Row": 1,
        "Column": 1
      },
      {
        "Ssi": 1,
        "Block": 1,
        "Row": 1,
        "Column": 1
      }
    ],
    "PatternModifyGroups": [
      {
        "PreSetupId": [
          "ALL"
        ],
        "PatternModifySetup": [
          {
            "PinGroupName": "xxHPCC_DPIN_Dig_slcA_A7",
            "Label": "31",
            "Domain": "LEG",
            "Ratio": 1,
            "Direction": "msb_lsb",
            "Offset": 0,
            "PatternModifyGlobal": "Global1",
            "PatternRegEx": ""
          },
          {
            "PinGroupName": "xxHPCC_DPIN_Dig_slcA_A7",
            "Label": "FISRT_LOOP_DOMA",
            "Domain": "LEG",
            "Ratio": 1,
            "Direction": "lsb_msb",
            "Offset": 0,
            "PatternModifyGlobal": "Global2"
          }
        ]
      },
      {
        "PreSetupId": [
          "Lya_Mux_Check",
          "Lya_Rd"
        ],
        "PatternModifySetup": [
          {
            "PinGroupName": "xxHPCC_DPIN_Dig_slcA_A2",
            "Label": "31",
            "Domain": "LEG",
            "Ratio": 1,
            "Direction": "msb_lsb",
            "Offset": 0,
            "PatternModifyGlobal": "Global1"
          },
          {
            "PinGroupName": "xxHPCC_DPIN_Dig_slcA_A2",
            "Label": "FISRT_LOOP_DOMA",
            "Domain": "LEG",
            "Ratio": 1,
            "Direction": "lsb_msb",
            "Offset": 0,
            "PatternModifyGlobal": "Global2"
          }
        ]
      },
      {
        "PreSetupId": [
          "Lya_Rd2",
          "Lya_Rd1_WAoff"
        ],
        "PatternModifySetup": [
          {
            "PinGroupName": "xxHPCC_DPIN_Dig_slcA_A3",
            "Label": "31",
            "Domain": "LEG",
            "Ratio": 1,
            "Direction": "msb_lsb",
            "Offset": 0,
            "PatternModifyGlobal": "Global1"
          },
          {
            "PinGroupName": "xxHPCC_DPIN_Dig_slcA_A3",
            "Label": "FISRT_LOOP_DOMA",
            "Domain": "LEG",
            "Ratio": 1,
            "Direction": "lsb_msb",
            "Offset": 0,
            "PatternModifyGlobal": "Global2"
          },
          {
            "PinGroupName": "xxHPCC_DPIN_Dig_slcA_A3",
            "Label": "40",
            "Domain": "LEG",
            "Ratio": 1,
            "Direction": "lsb_msb",
            "Offset": 0,
            "PatternModifyGlobal": "Global3"
          }
        ]
      }
    ],
    "SetupTests": [
      {
        "Name": "Lya_Mux_Check",
        "Patlist": "Lya_plist",
        "BurstEnabled": false
      },
      {
        "Name": "Lya_Rd",
        "Patlist": "Lya_plist",
        "BurstEnabled": false
      },
      {
        "Name": "Lya_Rd2",
        "Patlist": "Lya_plist",
        "BurstEnabled": false,
        "IoPatModEnabled": false
      },
      {
        "Name": "Lya_Rd1_WAoff",
        "Patlist": "Lya_plist",
        "BurstEnabled": false,
        "IoPatModEnabled": false
      },
      {
        "Name": "Lyasetuptest",
        "Patlist": "Lya_plist",
        "BurstEnabled": false
      }
    ],
    "GroupTests": [
      {
        "Name": "LYA_test_with_WA_OFF",
        "BaseNumberOffset": 0,
        "Tests": [
          {
            "Name": "EnterLya",
            "NumPoints": 0,
            "PreSetup": "Lya_Mux_Check",
            "PostSetup": "Lyasetuptest",
          },
          {
            "Name": "CellSelect",
            "NumPoints": 0,
            "Presetup": "Lya_Rd2",
            "PostSetup": "Lyasetuptest",
          },
          {
            "Name": "BLR",
            "Numpoints": 3,
            "Presetup": "Lya_Rd",
            "PostSetup": "Lyasetuptest",
            "VoltageForce": "0.1,0.2,0.3"
          },
          {
            "Name": "BLINT",
            "Numpoints": 0,
            "Presetup": "Lya_Rd",
            "PostSetup": "Lyasetuptest",
          },
          {
            "Name": "BLbTP",
            "Numpoints": 5,
            "Presetup": "Lya_Rd",
            "PostSetup": "Lyasetuptest",
          }
        ]
      },
      {
        "Name": "LYA_test_with_WA_ON",
        "BaseNumberOffset": 1,
        "Tests": [
          {
            "Name": "EnterLya",
            "NumPoints": 1,
            "Presetup": "Lya_Rd1_WAoff",
            "PostSetup": "Lyasetuptest",
          },
          {
            "Name": "CellSelect",
            "NumPoints": 1,
            "Presetup": "Lya_Rd",
            "PostSetup": "Lyasetuptest",
          },
          {
            "Name": "BLEXT",
            "Numpoints": 2,
            "Presetup": "Lya_Rd",
            "PostSetup": "Lyasetuptest",
          },
          {
            "Name": "PCR",
            "Numpoints": 3,
            "Presetup": "Lya_Rd",
            "PostSetup": "Lyasetuptest",
            "VoltageForce": "0.1,0.2,0.3"
          },
          {
            "Name": "BLTP",
            "Numpoints": 3,
            "Presetup": "Lya_Rd",
            "PostSetup": "Lyasetuptest",
            "VoltageForce": "1.3,0.7,0.1"
          }
        ]
      }
    ]
  }]
}
```

- _**LyaConfigurations**_: Grouping field used to define LYA configurations per array name.
  - _**ArrayName**_: Field to provide array definition name.
  - _**PatternModifyGlobals**_: Grouping field used to define global variables and their definitions.
    - _**Name**_: Variable name.
    - _**Size**_: Integer field to indicate the variable bit length of the value.
    - _**DefectVariable**_: Optional field to indicate the variable name to locate with the variable mapper for the lookup value.
    - _**Value**_: Integer field to indicate the variable value.  To be used as default for Manual mode or if DefectVariable is not defined.
  - _**IoPatternModify**_: Grouping field used to indicate the IO pattern modify parameters.
    - _**IoSize**_: Integer field to indicate the total number of bits involved with IO pattern modify.
    - _**Direction**_: Optional field to indicate the direction of the IO pattern modify bit string: LSB_MSB or MSB_LSB.  Default is MSB_LSB.
    - _**BitLocation**_: Field to indicate the location of the single bit to modify via a global variable.
    - _**Enable**_: Optional boolean field to indicate the direction of the IO pattern modify bit string.  Default is true.
    - _**Labels**_: Grouping field used to indicate the IO pattern modify label parameters.
      - _**Name**_: Field to indicate the label name or address.
      - _**Size**_: Integer field to indicate the bit length for this particular IO pattern modify.
      - _**Pingroup**_: Field to indicate the pin name(s). Example: "P001".  **NOTE: Only supports single pin for the moment!!**
      - _**Offset**_: Optional integer field to show the vector offset from the label to start IO pattern modify.  Offset is 0.
  - _**GoodCells**_: Optional grouping field used to indicate list of cells to test in Darray format.
    - _**Ssi**_: Integer field to indicate SSI value for a cell.
    - _**Block**_: Integer field to indicate Block value for a cell.
    - _**Row**_: Integer field to indicate Row value for a cell.
    - _**Column**_: Integer field to indicate Column value for a cell.
  - _**PatternModifyGroups**_: Grouping field used to indicate the Vector pattern modify parameters.
    - _**PreSetupId**_: Field to indicate the setup ID CSV to match with LYA tests.
    - _**PatternModifySetup**_: Grouping field used to indicate the individual Vector pattern modify parameters.
      - _**PinGroupName**_: Field to indicate the pin name(s). Example: "P001".  **NOTE: Only supports single pin for the moment!!**
      - _**Label**_: Field to indicate the label name or address.
      - _**Domain**_: Optional field to indicate the selected pattern domain for Vector pattern modify.  **NOTE: Not supported for the moment!!**
      - _**Ratio**_: Field to indicate the setup ID to select functional post-test vectors.  Default is empty string.
      - _**Direction**_: Optional field to indicate the direction of the IO pattern modify bit string: LSB_MSB or MSB_LSB.  Default is MSB_LSB.
      - _**Offset**_: Optional integer field to show the vector offset from the label to start Vector pattern modify.  Offset is 0.
      - _**PatternModifyGlobal**_: Field to indicate the location of the single bit to modify via a global variable.
      - _**PatternRegEx**_: Optional field to indicate the regular expression to be used to determine which patterns need to be searched for. When no value is defined, the test will look for the specified label in all existing patterns.
  - _**SetupTests**_: Grouping field used to indicate the functional setup tests.
    - _**Name**_: Field to indicate the setup ID to match with LYA tests.
    - _**Patlist**_: Field to indicate the pattern list to run for this setup test.
    - _**BurstEnabled**_: Optional boolean field to indicate the wanted burst setting for this pattern list.  Default is true.  **NOTE: Not supported for the moment!!**
    - _**IoPatModEnabled**_: Optional boolean field to enable the IO and Vector pattern modifies per setup test.  Default is true.  Does not affect setupID = "ALL".
  - _**GroupTests**_: Grouping field used to indicate the group test parameters.
    - _**Name**_: Field to provide group test definition name.
    - _**BaseNumberOffset**_: Optional field to indicate the value to be added to basenumber field for datalog output.  Default is 0.
    - _**Tests**_: Grouping field used to indicate the LYA test parameters.
      - _**Name**_: Field to indicate the DC test mode.  Must be EnterLya, CellSelect, BLR, BLbR, PCR, PCbR, BLTP, BLbTP, BLINT, BLbINT, BLEXT, or BLbEXT.
      - _**NumPoints**_: Integer field to indicate the number of measurements to be taken.
      - _**PreSetup**_: Field to indicate the setup ID to match with the appropriate functional setup test and Vector pattern modify entries.
      - _**PostSetup**_: Optional field to indicate the setup ID to select functional post-test vectors.  Default is empty string.
      - _**VoltageForce**_: Optional field to show actual voltage force CSV to be used for DC test.  Amount must match NumPoints.  Default is empty string.

# Custom User Code Hooks
The following is a list of all the customer's available extensions, which are called either during Verify() or Execute() of the base test method:
```csharp
Verify()
  1 IArray CreateArray();
Execute()
  2 LyaTestInstanceResults CreateTestInstanceResults();
  3 void ExecuteDatalog(LyaTestInstanceResults results);
```
**NOTE: Items 1 & 3 need to be extended for even base functionality, along with the variable mapper.**

To get a description of each of the available extension functions, the corresponding interface definition file can be consulted directly in the source code released with PRIME:
```
    [prime_release_path]\src\TestMethods\Array\Source\Lya\PrimeLyaTestMethod.cs
```

## TPL Samples
Here is a test instance example, including the needed extensions

Extension file
```csharp
    public class LyaExtension : PrimeLyaTestMethod, ILyaExtensions
    {
        /// <inheritdoc/>
        IArray ILyaExtensions.CreateArray()
        {
            if (this.IsArrayNameEqualTo("Array1"))
            {
                var variableMapper = new LyaVariableMapper();
                var directArrayRanges = new DirectArrayRanges(new uint[] { 0, 0, 0, 0, 0, 0 });
                return new Prime.TestMethods.Array.Shared.DecodingMode.ArrayImplementation.Array(variableMapper, directArrayRanges);
            }

            throw new FatalException(
                $"There is no Array/VariableMapper implementation for TargetArray=[{this.TargetArray}]");
        }
```

Variable Mapper file excerpt
```csharp
    internal class LyaVariableMapper : IVariableMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LyaVariableMapper"/> class.
        /// </summary>
        public LyaVariableMapper()
        {
        }
		.....

        /// <inheritdoc />
        public IRepairVariables DArrayToDefect(IRepairVariables directAddress)
        {
            var directArrayVariables = directAddress.GetVariables();
            var ssi = this.GetValue(directArrayVariables, "ssi");
            var block = this.GetValue(directArrayVariables, "block");
            var row = this.GetValue(directArrayVariables, "row");
            var column = this.GetValue(directArrayVariables, "column");
            var blocks96 = UserFunctions.Mod(block, 96);
            var halfx = UserFunctions.Div(blocks96, 48);
            var half = 1 - halfx;
            var blocks48 = UserFunctions.Mod(blocks96, 48);
            var sidex = UserFunctions.And(UserFunctions.ShiftRight(row, 8), 1);
            var side = 1 - sidex;
            var wordlinex = UserFunctions.And(row, 255);
            var halfio = UserFunctions.ShiftRight(column, 3);
            return new RepairVariables(
                new List<IVariable>
            {
                new Variable("input0", block),
                new Variable("input1", row),
                new Variable("input2", column),
                new Variable("slice", ssi),
                new Variable("side", side),
                new Variable("wordline", (uint)Math.Abs((255 * side) - wordlinex)),
                new Variable("colsel", UserFunctions.And(column, 3)),
                new Variable("bank",  UserFunctions.Mod(blocks48, 12)),
                new Variable("chunk", UserFunctions.And(UserFunctions.ShiftRight(column, 2), 1)),
                new Variable("quad", UserFunctions.Div(blocks48, 12)),
                new Variable("quadio", (38 * half) + halfio),
                new Variable("half", half),
                new Variable("halfio", halfio),
                new Variable("dummyR", 0),
                new Variable("dummyC", 0),
                new Variable("dummyB", 0),
            }, directAddress.GetId());
        }
		.....
    }
```

Main .mtpl file
```python
Import LyaExtension.xml;
Import PrimeLyaTestMethod.xml;

Test LyaExtension PrimeLyaTest_3_P1
{
    TimingsTc = "Lya::basic_func_timing_10MHz_20MHz";
    LevelsTc = "Lya::iValLevelMin";
    LogLevel = "Enabled";
    ExecutionMode = "GoodCell";
    LyaConfigFile = "~HDMT_TPL_DIR/TestPrograms/Lya/Modules/Lya/InputFiles/Hdmt/Lya.inputfile_v1.0.json";
    BitLinePin = "xxHPCC_DPIN_PMU_slcA_Short";
    BitLineBarPin = "xxHPCC_DPIN_PMU_slcB_Short";
    VccPin = "HDDPS_HC_gangx2_0p075ohm1";
    VccMaxLevel = -1.5;
    BaseNumber = 0;
    TargetArray = "Array1";
}
```

## Exit Ports
The test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                |
| ------------- | ------------- | -----------------------------------------------|
| **-2**        | ***Alarm***   | Any alarm condition                            |
| **-1**        | ***Error***   | Any software condition error                   |
| **0**         | ***Fail***    | Failing condition.                             |
| **1**         | ***Pass***    | Passing condition                              |
| **2**         | ***Pass***    | Passing condition with MaxLya limit reached    |
| **3**         | ***Pass***    | Passing condition with invalid DC measurements |

## Acronyms
Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TPL**: Test Programming Language
  - **LA**:	Logical Address format
  - **DA**: Physical Direct Array or D-array address format referred as DRC
  - **DRC**: D-array Block, Row, Column.
  - **Defect**:	Physical Address or “Defect” variables, previously known as BSV’s.

## Version tracking
| **Date**                  | **Version** | **Author**           | **Comments**                      |
| ------------------------- | ----------- | -------------------- | --------------------------------- |
| Jun 15<sup>th</sup>, 2023 | 1.0.0       | Kevin Krake          | Initial document                  |
