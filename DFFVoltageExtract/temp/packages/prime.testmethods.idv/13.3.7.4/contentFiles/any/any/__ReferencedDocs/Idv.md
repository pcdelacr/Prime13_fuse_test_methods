**prime Test-Method Specification REP**

Revision 1.0.0

Set 2021

[[_TOC_]]

## REP for IDV

This **REP** is intended to describe the IDV Prime TestMethod.

In this document, you will find the below sections:

- **Methodology** – A detailed description of this TestMethod intention and purpose

- **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

- **Datalog output** – A detailed description of what is datalogged by his TestMethod

- **Custom User Code hooks** – A list of functions available to the user code to override

- **TPL Samples** – Examples of how to use this TestMethod in a TPL file

- **Exit Ports** - A table describes each exit port

- **Additional Dependencies** – More to consider for this TestMethod to operate

- **Version tracking** – With author names, so you always have a name to address

- **Acronyms** - Definition of acronyms used in this document.

## Methodology

IDV are circuit structures, called fublets, throughout the silicon die, with several oscillators per fublet. Reporting of oscillator frequencies is the required data needed by Fab PE, Bin-split management, Speed-path debug, PQRE, & PDE groups. During a pattern execution, oscillator “count” data is collected, via tester capture memory. During post-processing, calculations occur, and results are logged into ITUFF for loading into the relevant databases.

### Verify

- Validate test instance parameters.
- Validate list of capture pins from json files.
- Check all Oscillator groups from json file are the same size (same ammount of oscillators).
- Calculate expected bit count from plist execution.
$$
\sum_{i=1}^{n} N_i*R
$$
  - Where:
    - N=Total number of oscillator reads for all fublets on chain i (with DIP=Y).
    - R=Register Width.
    - n=Number of chains.
- Set functional test as well as trigger map if given.
- Create IdvStructure snapshot
  - IDV test method uses Prime shared storage service to store the actual structure of a IDV file with the purpose of reusing this data structure on following test instances using the same file.

### Execute

- Apply test conditions and functional test execution.
- Get bit array from Ctvs results from plist execution.
  - Verify expected bit count matches total capture bits count.
  - Reverse stream of bits.
  - Convert and store bit stream to decimal.
- Calculate Oscillators frequencies for all chains.
  - Calculate loop delay for chain.
  - Calculate oscillator frequencies.
  - Datalog to ituff.
- Calculate statistical rollups data for all calculation item on calculations file.
  - Calculate statiscical value according to json file calculation object.
  - Datalog to ituff
  - Export calculated statistic value to shared storage.

### IDV Structure JSON File example:**

```json
{
    "CounterWidth": 15,
    "TapGroup": {
        "Type": "Core_serial",
        "Chains": [
            {
                "Type": "core1",
                "Delay": 99,
                "Pin": "xxHPCC_DPIN_Dig_slcA_A0",
                "Fublets": [
                    {
                        "Name": 21,
                        "FubletTypes": [
                            "idvw01tmp_a","idvw01tmp_b"
                        ]
                    },
                    {
                        "Name": 22,
                        "FubletTypes": [
                            "idvw01tmp_a","idvw01tmp_b"
                        ]
                    },
                    {
                        "Name": 23,
                        "FubletTypes": [
                            "idvw01tmp_a"
                        ]
                    },
                    {
                        "Name": 24,
                        "FubletTypes": [
                            "idvw01tmp_b"
                        ]
                    }
                ]
            },
            {
                "Type": "core2",
                "Delay": 99,
                "Pin": "xxHPCC_DPIN_Dig_slcA_A0",
                "Fublets": [
                    {
                        "Name": 21,
                        "FubletTypes": [
                            "idvw01tmp_a"
                        ]
                    }
                    {
                        "Name": 32,
                        "FubletTypes": [
                            "idvw01tmp_a","idvw01tmp_b"
                        ]
                    }
                    {
                        "Name": 33,
                        "FubletTypes": [
                            "idvw01tmp_a","idvw01tmp_b"
                        ]
                    }
                ]
            }
        ]
    },
    "OscillatorGroups": [
        {
            "FubletType": "idvw01tmp_a",
            "Oscillators": [
                {
                    "Number": 660,
                    "Cycles": 1000,
                    "DataInPattern": true,
                    "Divider": 32,
                    "OutputToItuff": true
                },
                {
                    "Number": 661,
                    "Cycles": 1000,
                    "DataInPattern": true,
                    "Divider": 32,
                    "OutputToItuff": true
                },
                {
                    "Number": 662,
                    "Cycles": 1000,
                    "DataInPattern": true,
                    "Divider": 16,
                    "OutputToItuff": true
                }
            ]
        },
        {
            "FubletType": "idvw01tmp_b",
            "Oscillators": [
                {
                    "Number": 760,
                    "Cycles": 1000,
                    "DataInPattern": true,
                    "Divider": 32,
                    "OutputToItuff": true
                },
                {
                    "Number": 761,
                    "Cycles": 1000,
                    "DataInPattern": true,
                    "Divider": 32,
                    "OutputToItuff": true
                },
                {
                    "Number": 762,
                    "Cycles": 1000,
                    "DataInPattern": true,
                    "Divider": 16,
                    "OutputToItuff": true
                }
            ]
        }
    ]
}
```

### IDV Calculations table JSON File example:**

```json
{
    "Calculations": [
        {
            "Name": "IFMAX_IDV_NESTED_StdDeviation",
            "Oscillators": ["660"],
            "Fublets": ["21","22","23"],
            "Statistic": "Median"
        },
        {
            "Name": "IFMAX_IDV_NESTED2_Max",
            "Oscillators": ["661"],
            "Fublets": ["22-23","32-33"],
            "Statistic": "Maximum"
        },
        {
            "Name": "IFMAX_IDV_NESTED3_Stddev",
            "Oscillators": ["662-665","667-670"],
            "Fublets": ["21","22","23"],
            "Statistic": "StdDeviation",
            "Export": true
        },
        {
            "Name": "IFMAX_IDV_NESTED4_Mean",
            "Oscillators": ["760"],
            "Fublets": ["21-23"],
            "Statistic": "Mean",
            "Export": true
        },
        {
            "Name": "IFMAX_IDV_NESTED5_Min",
            "Oscillators": ["761"],
            "Fublets": ["21","22","23"],
            "Statistic": "Minimum",
            "Export": true
        },
        {
            "Name": "IFMAX_IDV_NESTED6_Median",
            "Oscillators": ["762"],
            "Fublets": ["22","23","32","33"],
            "Statistic": "StdDeviation",
            "Export": true
        }
    ]
}
```

Notice that both "Oscillators" and "Fublets" are expressed as lists of strings since they can both reference ranges of inputs (e.g. ["101-115","120-125","127"]), the underlying references however, need to be integers in order for the ranges to be expanded and processed. Failure to provide Oscillator or Fublet references that can be interpreted as integers and/or ascending ranges of integers will result in errors during Verify.

## Statistics for Calculations Table Json File
### Count

The Count statistic returns a count of the number of fublets that fall between two specified limits for the given oscillator.

The following json snippet shows the format for the Count Statistic:

```json
{
    "Calculations": [
        {
            "Name": "SMP_0804_H169P51ULVTLLND2D02_FULLDIE_0650_CNT",
            "Oscillators": ["804"],
            "Fublets": [ "100", "200", "300", "400", "500", "600", "700", "800", "900", "1000", "1100", "1200"],
            "Statistic": "Count",
            "HigherCountLimit": 910,
            "LowerCountLimit": 100
        }
    ]
}
```
In this example the IDV Test Method would return an output with the count of fublets between the values of 100 and 910 (inclusive) which are the lower and upper limits respectively.

If the User specifies the "Count" statistic but does not provide upper and lower limits the Test Method will fail during Verify. If the limits are specified for another statistic type the type will take precedence and the limits will not be used.

## Limit Kill for Calculations Table Json File

This feature measures the value of a Rollup calculation result against user-provided limits. The evaluation of limits may cause the Test Method to exit through new fail ports depending on the comparison outcome, as defined in the table below:

|Limit|Failing Criteria|Exit Port|
|-------------|--------|-----|
|Out of Range Limits|Value < LowerLimitKill OR Value > HigherLimitKill |2|
|In Range but Out of Process Limits|Value > ProcessLimitKill |3|

The Out of Range Limits will be evaluated first and if either of them fails the Test Method will exit through port 2. If the value is found to be in range then the Out of Process Limit can be evaluated and if it fails, the Test Method will exit through port 3. The Out of Process Limit is optional when using the Limit Kill feature. Out of Process Limit will not be evaluated unless Out of Range limits are defined first.

Failure of any limit will be logged to console and execution will be aborted to exit through the respective failure port.

### Limit Kill Configuration File Syntax

The following json snippet shows the format for the new Limit Kill Feature within the Calculations Table Json File:

```json
{
    "Calculations": [
        {
            "Name": "SMP_0804_H169P51ULVTLLND2D02_FULLDIE_0804_Med",
            "Oscillators": ["804"],
            "Fublets": [ "100", "200", "300", "400", "500", "600", "700", "800", "900", "1000", "1100", "1200"],
            "Statistic": "Median",
            "LowerLimitKill": 100,
            "HigherLimitKill": 500000,
            "ProcessLimitKill": 4700
        }
    ]
}
```
Here the parameters for Limit Kill have their values defined with the tokens "LowerLimitKill", "HigherLimitKill" and "ProcessLimitKill" within a calculation statement.

When Out Of Range Limits are evaluated first, the value of the calculation is verified to be between 100 (LowerLimitKill) and 500,000 (HigherLimitKill) for this example, with the limits being non-inclusive. If this comparison fails the Test Method exits through port 2. Using the Limit Kill feature is optional, however if either "LowerLimitKill" or "HigherLimitKill" is defined then the other one becomes mandatory, if only one is provided the Test Method will fail during Verify.

If the Out Of Range Limits validation passes then the Process Limit is evaluated, for this specific example the value of the calculation is verified to be less than 4700 (ProcessLimitKill), if this fails the Test Method exits through port 3, otherwise execution continues normally.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Idsk test method

| **Parameter Name**    | **Required?** | **Type**        | **Comments**                                               |
| ------------------    | ------------- | --------------- | ---------------------------------------------------------- |
| Patlist               | Yes           | Plist           | Plist name to be executed                                  |
| LevelsTc              | Yes           | LevelsCondition | Levels test condition required for plist execution         |
| TimingTc              | Yes           | TimingCondition | Timing test condition required for plist execution         |
| PrePlist              | No            | String          | PrePlist name to be executed                               |
| TapFrequency          | Yes           | Double          | Tap frequency in MHz                                       |
| IdvStructureFile      | Yes           | File            | This file will contain the chain/fublet/Osc definitions    |
| CalculationsTableFile | Yes           | File            | This file will contain the calculation/statistical rollups |
| RawDataLogging        | No            | String          | Indicates whether the raw data is logged into ITUFF or not. Default value is Enabled   |
| TriggerMapName        | No            | String          | PatternTrigger map name                                    |

# Datalog output

The Prime IDV test method support printing to the ITUFF in two ways:

- Raw data printing (oscillator frequencies).
  - For each oscillator (category), frequencies for associated the fublets (composite) are printed with the instance name appearing only once per chain.

```txt
2_comnt_A!_tname_IDV_IDV::PrimeIDVTest_1_P1
2_comnt_A!_category_101
2_comnt_A!_composite_201_190504.0000000
2_comnt_A!_composite_203_39932.8000000
2_comnt_A!_composite_204_14259.2000000
2_comnt_A!_category_102
2_comnt_A!_composite_201_201945.6000000
2_comnt_A!_composite_203_115584.0000000
2_comnt_A!_composite_204_16433.6000000
2_comnt_A!_category_103
2_comnt_A!_composite_201_51460.8000000
2_comnt_A!_composite_203_77836.8000000
2_comnt_A!_composite_204_37096.0000000
2_comnt_A!_category_104
2_comnt_A!_composite_201_194870.4000000
2_comnt_A!_composite_203_5228.8000000
2_comnt_A!_composite_204_161689.6000000
```

Note: When using the MIDAS database, the Test Method will log one tname line per category token to comply with the MIDAS format. The use of the MIDAS database is confirmed to Prime via a "true" value of the "iCGL_EnableMIDAS" uservar from the "RunTimeLibraryVars" collection.

- Statistical rollup printing.
  - For each calculation in input file, the name of the calculation as well as the calculated value is printed (name of the statistical operation must be encoded by user on the name of the calculation).

```txt
2_tname_IFMAX_IDV_NESTED_StdDeviation
2_mrslt_18758.87
2_tname_IFMAX_IDV_NESTED2_Max
2_mrslt_37281.89
2_tname_IFMAX_IDV_NESTED3_Stddev
2_mrslt_8657.23
2_tname_IFMAX_IDV_NESTED4_Mean
2_mrslt_26624.93
2_tname_IFMAX_IDV_NESTED5_Min
2_mrslt_38148.13
2_tname_IFMAX_IDV_NESTED6_Median
2_mrslt_4503.21
```

In some cases the IDV Test Method will log -999 as the value of a calculation, this is done when the Test Method is unable to calculate the result, for example when a Standard Deviation has less than 2 frequencies to compute the statistic, or a division by zero occurs.

```txt
2_tname_IFMAX_IDV_NESTED3_Stddev
2_mrslt_-999
```

# Custom User Code Hooks

The following is a list of all the customer's available extensions, which are called either during Verify() or Execute() of the base test method:

```csharp
Execute()
  1  void CalculateFrequenciesForAllChains(IdvStructureJsonFile idvStructureJsonFile, string perPatternCtvData, Dictionary<string, ConfigurationFile.OscillatorGroup> oscillatorGroupsPerFubletType);
  2  int CalculateRollUpData(CalculationsTableJsonFile calcTableJsonFile);
```

To get a description of each of the available extension functions, the corresponding interface definition file can be consulted directly in the source code released with prime:

```csharp
    prime_release_path\src\TestMethods\Idv\Idv\IPrimeIdvExtensions.cs
```

## TPL Samples

Here are a few test instance examples using the Idv test method.

For more examples please check the Sample Test program in the
PRIME user SDK directory.

**TPL Sample1**:

```csharp
Import PrimeIdvTestMethod.xml;

Test PrimeIdvTestMethod PrimeIDVTest_1_P1
{
   Patlist = "ctv_plist";
   TimingsTc = "IDV::basic_func_timing_10MHz_20MHz";
   LevelsTc = "IDV::basic_func_lvl_nom";
   TapFrequency = 50;
   IdvStructureFile = "~HDMT_TPL_DIR/Modules/IDV/IDV/InputFiles/idvStructure.json";
   CalculationsTableFile = "~HDMT_TPL_DIR/Modules/IDV/IDV/InputFiles/calculationsTable.json";
   TriggerMapName = "";
}
```

## Exit Ports

The IDV Test Method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                   |
| ------------- | ------------- | ------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                               |
| **-1**        | ***Error***   | Any software condition error                      |
| **0**         | ***Fail***    | Failing condition                                 |
| **1**         | ***Pass***    | Passing condition                                 |
| **2**         | ***Fail***    | Limit Kill Out of Range                           |
| **3**         | ***Fail***    | Limit Kill Within Range but Out of Process Limits |

## Additional Dependencies

More dependencies to consider for this TestMethod to well operate:

N\A

## Version tracking

| **Date**             | **Version** | **Author**                        | **Comments**                                                                                                           |
|----------------------|-------------|-----------------------------------|------------------------------------------------------------------------------------------------------------------------|
| Set 30th, 2020       | 1.0.0       | Esteban Ortega                    | Initial Release                                                                                                        |
| July 21th, 2022      | 1.1.0       | Esteban Ortega                    | Ituff format update                                                                                                    |
| September 22th, 2022 | 1.2.0       | Esteban Ortega                    | Adding possibility to avoid raw data printing for ITUFF space optimization                                             |
| Jan 12th, 2024       | prime 13.0  | Jose Gamez                        | Performance Improvements for TTR                                                                                       |
| Jun 11th, 2024       | prime 13.0  | Jose Gamez                        | Adds new Count Statistic for Rollup Data                                                                               |
| Jun 20th, 2024       | prime 13.0  | Jose Gamez                        | Adds Limit Kill Feature                                                                                                |
| Jun 26th, 2024       | prime 13.0  | Jose Gamez                        | Adds capability to specify multiple oscillators per rollup statistic                                                   |
| Jun 28th, 2024       | prime 13.0  | Jose Gamez                        | Performance Improvements for TTR                                                                                       |
| May 5th, 2025        | prime 13.2.2 | Jose Gamez                        | Fixes bug of ituff not loading in Midas due to multiple category tokens per tname token.                               |
| May 25th, 2025       | prime 13.2.2 | Slava Yablonovich </n> Jose Gamez | Fixes bug where a division by zero would datalog "NaN" or gibberish when calculating StdDeviation for Rollup Statistics. |

## Acronyms

Definition of acronyms used in this document:

- **REP**: P**r**ime T**e**st-Method S**p**ecification
- **HDMT**: High Density Modular Tester
- **TPL**: Test Programming Language
- **TOS**: Test Operating System
