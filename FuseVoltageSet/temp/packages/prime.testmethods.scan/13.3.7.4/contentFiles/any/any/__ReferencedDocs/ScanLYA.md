**prime Test-Method Specification REP**


Jan 2024

[[_TOC_]]

## REP for ScanLYA

This **REP** is intended to describe the ScanLYA Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Datalog output** – A detailed description of what is datalogged by his TestMethod

  - **Custom User Code hooks** – A list of functions available to the user code to override

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document

## Methodology

The ScanLYA test method provides capability to execute multiple scan plists, capture the failures, and process them using a `IStreamingScanNetwork` object to generate an HRY data that will be printed to Ituff datalog, as well as SPOFI datalog.
For more information about decoding understanding, proceed to [SSN decoding](##SSNdecoding)] to understand the calculation.

The test method will take into account the channel-linking capability if used in Pattern list, when calculating the failure distance from last label.

The initial effort of ScanLYA test method is to reduce TT overhead in ScanHRYSSN and ScanSPOFI test method. Much of the overhead is contributed from duplication in SPOFI datalogging test instances setup in the TP, and HRY decode algorithm that constantly matching the regexes.
For current ScanFI implementation, multiple ScanHRYSSN test method would need to be setup for multiple plist execution, with combination of ScanSPOFI test method that linked to ScanHRYSSN relevant port exit. Hence leading to a complex 4-5 instances setup for 3 plists for example.

With ScanLYA, it is being consolidated into 1 template, that exhibits the nature of ScanHRYSSN to decode the HRY for multiple plist, and combination of ScanSPOFI test method to handle SPOFI datalogging, as well as some optimization on HRY decoding.

The template has gone through some POC with 3 main plist, ATPG, CHAIN, and Dport. This helps to explain the rationale of having a few more parameters.
As the methodology are quite same with ScanHRYSSN test method, here are few key differences between the two templates:
1. Patlist, ChainPatlist, and DportPatlist parameter are added into ScanLYA test method. Chain patlist and Dport patlist is an optional parameter.
2. TotalFailCaptureCount and PerPatFailCaptureCount numbers will apply towards all 3 patlist when performing plist execution.
3. ChainHRYInputFile parameter is added for Chain patlist, it must be define when Chain patlist enabled.
4. When PartitionsToIgnore is enable, partition masking will apply towards all the enabled plist's HRY string. 
For example, if ATPG and Chain patlist is enable and it is passing with HRY 1111, masking the last two partition will become 1100 for both ATPG and Chain's HRY.
5. PatternsRegexesForKill parameter will apply towards all the enabled patlist, if specified.
For example, if ATPG, CHAIN and DPORT patlist is enable, patterns specifed in PatternsRegexesForKill will be apply towards these three patlist.
6. When ATPG and Chain patlist are enabled, two SharedStorageExportKey are required to store ATPG and CHAIN hry string respectively. 
When there is only ATPG patlist enable, template is expecting only 1 sharedStorage key.

Starting from Prime v12.3.1, ScanService is removed and business logic has been shift to Scan test method itself. The expected changes on the TP side are:
1. PinMap file no longer load from AlephInit, instead load from test instance using parameter PinMappingFile, similar to HRYInputFile.
2. Therefore, PinMappingFile parameter is required, along with the path.

Verify Flowchart

::: mermaid
flowchart TB
    Start([Start])
    Start --> InitTest[Initialize functional test and capture settings]
    InitTest --> InitPatternList[Initialize patternRegexToKill pattern list]
    InitPatternList --> ParseJson[Read and parse pinMap JSON file]
    ParseJson --> SanityCheckConfig[Sanity check for configuration]
    SanityCheckConfig --> SanityCheckPattern[Sanity check for PatternRegexesToKill]
    SanityCheckPattern --> SanityCheckStatus[Sanity check for ExpectedPartitionsStatuses]
    SanityCheckStatus --> End([End])

    style Start stroke:#90ee90,stroke-width:2px
    style InitTest stroke:#0000FF,stroke-width:2px
    style InitPatternList stroke:#0000FF,stroke-width:2px
    style ParseJson stroke:#0000FF,stroke-width:2px
    style SanityCheckConfig stroke:#0000FF,stroke-width:2px
    style SanityCheckPattern stroke:#0000FF,stroke-width:2px
    style SanityCheckStatus stroke:#0000FF,stroke-width:2px
    style End stroke:#FF0000,stroke-width:2px
:::

Execute Flowchart

1. Overall ScanLYA Execution

::: mermaid
  flowchart TD
    Start([Start]) --> A[Post process ATPG patlist]
    A --> B{Is ATPG functional test pass?}
    B -- Yes --> C[Set ATPG pass port]
    B -- No --> D[Post process CHAIN patlist]
    D --> E{Is CHAIN functional test pass?}
    E -- Yes --> F[Set CHAIN pass port]
    E -- No --> G{Is Dport Patlist exist?}
    G -- No --> H[Set CHAIN fail port]
    G -- Yes --> I[Post process Dport patlist]
    I --> J{Is Dport functional test pass?}
    J -- Yes --> K[Set Dport pass port]
    J -- No --> L[Set Dport fail port]
    C --> End([End])
    F --> End
    H --> End
    K --> End
    L --> End

    style Start stroke:#90ee90,stroke-width:2px
    style A stroke:#0000FF,stroke-width:2px
    style C stroke:#0000FF,stroke-width:2px
    style D stroke:#0000FF,stroke-width:2px
    style F stroke:#0000FF,stroke-width:2px
    style H stroke:#0000FF,stroke-width:2px
    style I stroke:#0000FF,stroke-width:2px
    style K stroke:#0000FF,stroke-width:2px
    style L stroke:#0000FF,stroke-width:2px
    style B stroke:#FFA500,stroke-width:2px
    style E stroke:#FFA500,stroke-width:2px
    style G stroke:#FFA500,stroke-width:2px
    style J stroke:#FFA500,stroke-width:2px
    style End stroke:#FF0000,stroke-width:2px
:::

2. ATPG Plist Flow Execution

::: mermaid
flowchart TD
    Start([Start]) --> A[Process pattern regex for kill]
    A --> B{Is ATPG functional test pass?}
    B -- No --> C[Print failure data to ituff]
    C --> D[Generate HRY string and print to ituff]
    D --> E{Is CHAIN patlist enabled?}
    E -- Yes --> F[Go to CHAIN process]
    E -- No --> G[Print ATPG SPOFI]
    G --> H[Set fail port]
    H --> End([End])
    B -- Yes --> I[Generate HRY string and print to ituff]
    I --> J[Set pass port]
    J --> End

    style Start stroke:#90ee90,stroke-width:2px
    style A stroke:#0000FF,stroke-width:2px
    style C stroke:#0000FF,stroke-width:2px
    style D stroke:#0000FF,stroke-width:2px
    style F stroke:#0000FF,stroke-width:2px
    style G stroke:#0000FF,stroke-width:2px
    style H stroke:#0000FF,stroke-width:2px
    style I stroke:#0000FF,stroke-width:2px
    style J stroke:#0000FF,stroke-width:2px
    style E stroke:#FFA500,stroke-width:2px
    style B stroke:#FFA500,stroke-width:2px
    style End stroke:#FF0000,stroke-width:2px
:::

3. CHAIN Plist Flow Execution

::: mermaid

flowchart TD 
    Start([Start]) --> A[Process pattern regex for kill]
    A --> B{Is CHAIN functional test pass?} 
    B -- No --> C[Print failure data to ituff] 
    C --> D[Print CHAIN SPOFI] 
    D --> E[Generate HRY string and print to ituff] 
    E --> F[Set fail exit port]
    F --> End([End])
    B -- Yes --> G[Print ATPG SPOFI] 
    G --> H[Generate HRY string and print to ituff]
    H --> I[Set pass exit port]
    I --> End

    style Start stroke:#90ee90,stroke-width:2px
    style A stroke:#0000FF,stroke-width:2px
    style C stroke:#0000FF,stroke-width:2px
    style D stroke:#0000FF,stroke-width:2px
    style E stroke:#0000FF,stroke-width:2px
    style F stroke:#0000FF,stroke-width:2px
    style G stroke:#0000FF,stroke-width:2px
    style H stroke:#0000FF,stroke-width:2px
    style I stroke:#0000FF,stroke-width:2px
    style B stroke:#FFA500,stroke-width:2px
    style End stroke:#FF0000,stroke-width:2px
:::

4. DPORT Plist Flow Execution

::: mermaid

flowchart TD
    Start([Start]) --> A{Is Dport functional test pass?}
    A -- No --> B[Print failure data to ituff]
    B --> C[Print DPORT SPOFI]
    C --> End([End])
    A -- Yes --> C

    style Start stroke:#90ee90,stroke-width:2px
    style B stroke:#0000FF,stroke-width:2px
    style C stroke:#0000FF,stroke-width:2px
    style A stroke:#FFA500,stroke-width:2px
    style End stroke:#FF0000,stroke-width:2px
:::

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ScanLYA test method

| **Parameter Name**     | **Required?** | **Type**        | **Values**                                                                    | **Comments**            |
| ---------------------- | ------------- | --------------- | ----------------------------------------------------------------------------  | ----------------------- |
| Patlist                | Yes           | Plist           | ATPG Plist name to be executed.                                               |                         |
| ChainPatlist           | No            | Plist           | Chain Plist name to be executed.                                              |  Default = Empty String   |
| DportPatlist           | No            | Plist           | Dport Plist name to be executed.                                              |  Default = Empty String                      |
| TimingTc               | Yes           | TimingCondition | Timing test condition required for all enabled plist execution.               |                         |
| LevelsTc               | Yes           | LevelsCondition | Levels test condition required for all enabled plist execution.               |                         |
| PrePlist               | No            | String          | PrePlist callback to plist execution.                                         |                         |
| MaskPins               | No            | String          | Comma separated list of pins for which the fail data capture will be skipped. | Default = Empty String  |
| TotalFailCaptureCount  | Yes           | UnsignedInteger | Number of total failures to capture for all plist enabled.                    |                         |
| PerPatFailCaptureCount | No            | UnsignedInteger | Number of per pattern failures to capture for all plist enabled               | Default = 1             |
| PatternsRegexesForKill | No            | CommaSeparatedString | List of regexes, when a pattern that failed matches one of this regexes, will exit from dedicated port 4. | Default = Empty String |
| PartitionsToIgnore     | No            | CommaSeparatedString | List of partitions to ignore. When a failing partition matches on of the ignore partition the thes method will set a 9 in the HRY result for all plist that enabled. | Default = Empty String |
| ExpectedPartitionsStatuses | No            | CommaSeparatedString | List of expected partition statuses. List of values with format "PartitionName=0/1/8/9/X" (X - don't care.). This parameter does take effect if the plist does not report any failure. | Default = Empty String |
| SetUnassignedToUntestedPatterns               | No       | SetUnassignedToUntestedPatternsMode          | Sets to unassigned or 9 to the HRY result, to those partitions that don't have any pattern related in the current Patlist. Allowed values=[ENABLED, DISABLED]                                         |            Default = "DISABLED"             |
| HPTPMode               | No. Default is DISABLED | String (Choice) | Disable to use regular SSN to decode. Enable to use HPTP algorithm to decode. | Default = DISABLED |
| HRYInputFile           | Yes           | File            | .Json input file for ATPG HRY.                                                     |                         |
| ChainHRYInputFile      | Yes           | File            | .Json input file for Chain HRY.                                                     |                         |
| PinMappingFile         | Yes           | File            | .Json input file for pinMapping    |      |
| SharedStorageHRYExportKey | No         | CommaSeparatedString | Keys use to export HRYs string to sharedStorage in IP scope, for ATPG and CHAIN. | Default = Empty String. If specified, two sharedStorage keys are expected for ATPG and CHAIN respectively.           |
| GenerateSPOFI          | No            | ENABLED/DISABLED | This parameter control the SPOFI logging for ATPG, CHAIN and DPORT patlist. Set to enable to allow all the patlist to generate SPOFI. | Default = ENABLED |
| EnableScanPatLookUp        | No            | String (Choice) | ENABLED (Default), DISABLED |  Option to disable pattern lookup when it was not disabled through the uservar "__main__::_UserVars.EnableScanPatLookUp". ENABLED by default. |
| EnableReadingLabels        | No            | String (Choice) | ENABLED (Default), DISABLED |  Option to enable/disable previous label reading and associated cycle count. ENABLED by default. |
| VectorCountOffset          | No            | Int             | Default 0 |  Option to print SPOFI vector count using a fixed integer offset. |
| EnableSpofiPartitionName | No | ENABLED/DISABLED | ENABLED, DISABLED (Default) | Options to enable/disable partition name to be printed on SPOFI log. DISABLED by default. |

## Datalog output

The above [example](#example) will print to Ituff datalog the following data:

If only ATPG patlist is enable:
```
2_tname_<instance name>_HRY_RAWSTR
2_strgval_0111
2_tname_HRY_<instance name>_<failing parition>
2_strgval_0
```

If multiple plist is enable:
```
2_tname_<ATPG/CHAIN/DPORT>_<instance name>_HRY_RAWSTR
2_strgval_0111
2_tname_HRY_<instance name>_<failing parition>
2_strgval_0
```

## SPOFI output

Example of SPOFI output data:

1. Uncompress pattern, without partition name.
```
2_ttype_HDMTG2
2_plist_scanTestTime2
2_tname_Scan::CHAIN_MegaTemplateLYA_P1
2_fail_limits:min_0_max_10_cat_0
2_pttrn_DomainA_All_DPIN_Dig:scan_hry_pattern_all_passing_testTime2
2_pattern_mode:uncompressed
Label_1 0 xxHPCC_DPIN_Dig_slcA_A3 -1 0 H -1 1
Label_1 0 xxHPCC_DPIN_Dig_slcA_A3 -1 1 H -1 2
Label_1 0 xxHPCC_DPIN_Dig_slcA_A3 -1 2 H -1 3
Label_1 0 xxHPCC_DPIN_Dig_slcA_A3 -1 3 H -1 4
Label_1 0 xxHPCC_DPIN_Dig_slcA_A3 -1 4 H -1 5
Label_1 0 xxHPCC_DPIN_Dig_slcA_A3 -1 5 L -1 6
Label_1 0 xxHPCC_DPIN_Dig_slcA_A3 -1 6 H -1 7
Label_1 0 xxHPCC_DPIN_Dig_slcA_A3 -1 7 H -1 8
Label_1 0 xxHPCC_DPIN_Dig_slcA_A3 -1 8 H -1 9
Label_1 0 xxHPCC_DPIN_Dig_slcA_A3 -1 9 H -1 10
DSS_LIMIT_CATASTROPHIC
```

2. Uncompress pattern, with partition name.
```
2_ttype_HDMTG2
2_plist_passing_scanlya_atpg
2_tname_Scan::ATPG_ScanLYA_ATPGFail_ChainPass_P2
2_fail_limits:min_0_max_90_cat_0
2_pttrn_DomainA_All_DPIN_Dig:scan_hryssn_atpg
2_pattern_mode:uncompressed
Label_1 0 xxHPCC_DPIN_Dig_slcA_A0 -1 1 H -1 2 # par=[pariitcotc]
Label_1 0 xxHPCC_DPIN_Dig_slcA_A0 -1 2 H -1 3 # par=[pariitcotc]
Label_1 0 xxHPCC_DPIN_Dig_slcA_A0 -1 3 H -1 4 # par=[pariirp]
Label_1 0 xxHPCC_DPIN_Dig_slcA_A0 -1 4 H -1 5 # par=[pariirp]
```

3. Compress pattern, without partition name.
```
2_ttype_HDMTG2
2_plist_compress_plist
2_tname_Scan::ScanLYA_ATPGCompressedFail_EnablePartition_P3
2_fail_limits:min_0_max_90_cat_0
2_pttrn_DomainA_All_DPIN_Dig:compress_01
2_pattern_mode:compressed
Label1A 0 xxHPCC_DPIN_Dig_slcA_A0 -1 4 H -1 15 # cc = [11] rma=[4]
Label1A 0 xxHPCC_DPIN_Dig_slcA_A0 -1 5 H -1 16 # cc = [11] rma=[5]
Label1A 0 xxHPCC_DPIN_Dig_slcA_A0 -1 6 H -1 17 # cc = [11] rma=[6]
Label1A 0 xxHPCC_DPIN_Dig_slcA_A0 -1 7 H -1 18 # cc = [11] rma=[7]
Label1A 0 xxHPCC_DPIN_Dig_slcA_A0 -1 8 H -1 19 # cc = [11] rma=[8]
2_pttrn_scan_hry_pattern_all_passing_1
NO_FAIL
```

4. Compress pattern, with partition name.
```
2_ttype_HDMTG2
2_plist_compress_plist
2_tname_Scan::ScanLYA_ATPGCompressedFail_EnablePartition_P3
2_fail_limits:min_0_max_90_cat_0
2_pttrn_DomainA_All_DPIN_Dig:compress_01
2_pattern_mode:compressed
Label1A 0 xxHPCC_DPIN_Dig_slcA_A0 -1 4 H -1 15 # cc = [11] rma=[4] par=[pariitcotc]
Label1A 0 xxHPCC_DPIN_Dig_slcA_A0 -1 5 H -1 16 # cc = [11] rma=[5] par=[pariitcotc]
Label1A 0 xxHPCC_DPIN_Dig_slcA_A0 -1 6 H -1 17 # cc = [11] rma=[6] par=[pariirp]
Label1A 0 xxHPCC_DPIN_Dig_slcA_A0 -1 7 H -1 18 # cc = [11] rma=[7] par=[pariirp]
Label1A 0 xxHPCC_DPIN_Dig_slcA_A0 -1 8 H -1 19 # cc = [11] rma=[8] par=[pariiommu]
2_pttrn_scan_hry_pattern_all_passing_1
NO_FAIL
```

# Configuration files
ScanLYATestMethod manage two configuration files, one is the [Pin mapping configuration file](#Pin-mapping-configuration-file) to provide a unique pin mapping, and other file is the [SSN configuration file](#SSN-configuration-file).

## Pin mapping configuration file

Format file name: {customer file name}*.ScanPinMapping.json*
Example file:
```json
{
  "PinNumbers": 32,
  "SSNBusWidth" : 32,
  "NotSSNBusPinsRelated": ["UnrelatedPinName"],
  "PinsMapping": [
    {
      "DataPath": 1,
      "PinName": "PinName1"
    },
    {
      "DataPath": 2,
      "PinName": "PinName2"
    },
    {
      "DataPath": 3,
      "PinName": "PinName3"
    },
    {
      "DataPath": 4,
      "PinName": "PinName4"
    },
    {
      "DataPath": 5,
      "PinName": "PinName5"
    },
    {
      "DataPath": 6,
      "PinName": "PinName6"
    },
    {
      "DataPath": 7,
      "PinName": "PinName7"
    },

    ( ... )

    ( ... )

    {
      "DataPath": 32,
      "PinName": "PinName32"
    }
  ]
}
```
Field details:
- _**PinNumbers**_: Required field used to provide the number of pins that are going to be specified in this file.
- _**SSNBusWidth**_: Optional field only taking effect when HPTP mode is chosen. This will overwrite PinNumbers and taking into HPTP equation as the bus size.
For example, SSNBusWidth of 32 and PinNumbers of 8 is specify, only SSNBusWidth of 32 will be consume in HPTP mode. Normal SSN mode does not affect by this field.
- _**NotSSNBusPinsRelated**_: Optional field to indicate failures pins that are not Scan pins. When fail pin(s) matched this field, it will skip the
SSN decode and directly reflect on HRY index, based on PatternRegex in HRY json file.
- _**PinsMapping**_: Grouping field. Used to provide a tuple of pin mapping.
  - _**DataPath**_: Field to provide the index number of the data path. Index numbers begins with number 1.
  - _**PinName**_: Field to provide the name of the pin to associate to the index path.

## SSN configuration file

Format file name: {customer file name}*.json*
Example file:
```json
{
    "HRYLength": 4,
    "Patterns": [
        {
            "PacketSize": 54,
            "OutputPacketOffset": 10,
            "PatternRegex": ".*ssn.*|.*main.*",
            "Partitions": [
                {
                    "BitPositions": "0-53",
                    "HRYIndex": 0,
                    "HRYPrint": "pariirp"
                }
            ]
        },
        {
            "PacketSize": 54,
            "OutputPacketOffset": 10,
            "PatternRegex": ".*compress.*|.*scan.*",
            "Partitions": [
                {
                    "BitPositions": "0-12,13,14-16",
                    "HRYIndex": 1,
                    "HRYPrint": "pariirp"
                },
                {
                    "BitPositions": "17-38",
                    "HRYIndex": 2,
                    "HRYPrint": "pariiommu"
                },
                {
                    "BitPositions": "39-53",
                    "HRYIndex": 3,
                    "HRYPrint": "pariitcotc"
                }
            ]
        }
    ]
}
```
Field details:
- _**HRYLength**_: Required field used to provide the HRY size.
- _**Patterns**_: Grouping field. Used to provide a list of pattern Scan description.
  - _**PacketSize**_: Field to provide a number indicating the packet size by pattern.
  - _**OutputPacketOffset**_: Filed to provide the offset from the start of the pattern to the first bit of the partitions.
  - _**PatternRegex**_: Field to provide a regex to match those patterns that consider the following partitions.
  - _**Partitions**_: Grouping field. Used to provide a list of partition descriptions.
    - _**BitPositions**_: Field to provide a string range position, where the first bit position of the pattern is the index 0 (zero). The corresponding format is a comma separated list of the following items: single numbers ("2" or "11") and ranges, where the first limit must be lower than the second limit e.g "2-5", which indicates a range that contains the numbers: 2, 3, 4 and 5.
    - _**HRYIndex**_: Field to provide the partition index in the HRY.
    - _**HRYPrint**_: Field to provide the partition name or string that identifies the current partition where the failing partiitons are printed in the ITUFF stream.


## SSN decoding
To understand the SSN decoding methodology, let's consider the following example.

Assuming one failure describe as follows:
| Failing Pattern            | scan\_hry\_pattern\_all\_passing\_1 |
| -------------------------- | --------------------- |
| Failing Pin                | PinName3              |
| Failure Cycle | 12                    |

The first step consists of the SSN packet position calculation, which is ruled by this equation:

$$
Cycle\_Offset = testerFailureCycle - OutputPacketOffset
$$

$$
Starter\_bit = (Cycle\_Offset * PinNumbers) \% PacketSize
$$

Where `OutputPacketOffset` and `PacketSize` are taken from the pattern section that `scan_hry_pattern_all_passing_1` matches from the configuration file, in this case this pattern matches the regex `.*compress.*|.*scan.*`. On the other hand, the value `testerFilureCycle` is taken from the pattern failure.

Replacing the variables with our example values, it is got:

$$
Starter\_bit = [(12 - 10) * 32] \% 54
$$

$$
Starter\_bit = 10
$$

Then, we use that starter bit to create a new counter from the position of the failing pin, in this case the failing pin is `PinName3` and its index value is 3 from the [pin mapping file](#Pin-mapping-configuration-file).

So, adding the pin failing index plus the resulted ssn packet position:

$$
partition\_position = Starter\_bit + pin\_position
$$

$$
partition\_position = 10 + 3
$$

$$
partition\_position = 13
$$

Once the `partition_position` is resolved, it will show which is the failing partition, because this value corresponds a bit position specified in the field `BitPositions`, of the matched patter section. For this example, the failing partition is `pariirp`, since the number 13 is into the `BitPositions="0-12,13,14-16` in that specific partition.

## HPTP decoding
The first step is to find out HPTP Vector using following formula:

$$
Cycle\_Offset = testerFailureCycle - OutputPacketOffset
$$

$$
HPTP\_Vector = Cycle\_Offset \ \% \ 4
$$

The general formula for HPTP decode is stated as below:
$$
SSN\_Position = Starter\_Bit + HPTP\_Row + (Pin\_position * 4)
$$

To determine starterBit:
$$
Cycle\_offset = testerFailureCycle - OutputPacketOffset
$$

$$
 StarterBit = Roundown[(Cycle\_offset) / 4), 0] * SSN\_bus\_width \ \% \ packet\_size
$$

To determine HPTP Vector:

If HPTP Vector = 0, HPTP Row = +0

If HPTP Vector = 1, HPTP Row = +2

If HPTP Vector = 2, HPTP Row = +1

If HPTP Vector = 3, HPTP Row = +3


To determine pin position multiplier:
$$
Pin\_Position\_with\_multiplier = Pin\_position * 4
$$

Assuming one failure describe as follows:
| Failing Pattern            | scan\_hry\_pattern\_all\_passing\_1 |
| -------------------------- | --------------------- |
| Failing Pin                | PinName3              |
| Failure Cycle | 12                    |

1. Find out the HPTP Vector to determine HPTP Row. In this case, HPTP vector 2 means we need to +1 for the HPTPRow.
$$
HPTP\_Vector = Modulo((12 - 10), 4)
$$
$$
HPTP\_Vector = 2
$$
2. Find out starter bit. Assuming we have packet size of 54, and bus width of 8 in pinMapping.
$$
Cycle\_offset = 12 - 10
$$
$$
Cycle\_offset = 2
$$
$$
StarterBit = Roundown[(2 / 4), 0] * 8 \ \% \ 54
$$
$$
StarterBit = 0
$$

3. To determine pin position multiplier:
$$
Pin\_Position\_with\_multiplier = 3 * 4
$$
$$
Pin\_Position\_with\_multiplier = 12
$$

4. Adds up the formula to find out SSN Position.
$$
SSN\_Position = 0 + 1 + 12
$$
$$
SSN\_Position = 13
$$

### Exception:
If SSN position acquired at the end has a value larger than packet size (54) for example 60, substraction will be taken place.
$$
SSN\_Position = 60 - 54
$$
$$
SSN\_Position = 6
$$
6 will be the new SSN position value.

# Global TimeOut
Global Time out is a feature to avoid hang when data processing is taking more time as expected by tester,
time out feature is enabled if user have defined the uservar **TimeOutLimit** in miliseconds as double value.
Also the user can set **TimeOutLimit** using sharingStorageService with a value as "uint" value.
Example:
```C#
Prime.Base.ServiceStore<ISharedStorageService>.Service.InsertRowAtTable("TimeOutLimit", (uint)this.TimeOut, Context.DUT, ResetPolicy.RESET_AT_DEVICE_START, this.SessionContext);
```

Time out feature verify during execution of data-processing, if time limit is reached from the start of unit execution,
the execution is aborted and ended the test, salving the previos instances results.

TimeOut feature is disabled if user execute the instances stand alone!, it is disabled if DeviceStart time is defined but not DeviceEnd,
The user can see if this case was catched with this print in console with debug mode:
```
[DUT: 1_IPA]TimeOut disabled, instance execution as stand alone!

## Global timeOut console output

Console output if time limit is reached.

[2023-Oct-30 09:48:16.433][A][TAL][DUT: 1] StartTest <PrimeTestName>::<TestPlanName>::<TestInstanceName>
[2023-Oct-30 09:48:18.362][A][HAL][DUT: 1] Starting burst group execution.
[2023-Oct-30 09:48:28.838][A][HAL][DUT: 1] Waiting 30000ms for execution to finish
[2023-Oct-30 09:48:37.807][A][HAL][DUT: 1] Starting burst group execution.
[2023-Oct-30 09:48:37.807][A][HAL][DUT: 1] Waiting 30000ms for execution to finish
[DUT: 1]ERROR:
|	Error in instance=[<TestPlanName>::<TestInstanceName>]:
|	Prime.Base.Exceptions.TestMethodException occured : Failed by TimeOut, TimeOutLimit=[10000], Current Time=[23552.5544].
[DUT: 1]STACK TRACE:
   at Prime.TestMethods.TimeOutFeaturesExtensions.ThrowTimeOutException(Double myTimeOutValue, Double elapsedTime)

   at Prime.TestMethods.TimeOutFeaturesExtensions.TimeOutMonitor(Thread myThread, Task myTask, CancellationTokenSource tokenSource, ISessionContextProviderContainer context)

   at Prime.TestMethods.TimeOutFeaturesExtensions.ForEachTimeOutReturnList[T,T1,T2,T3](IDictionary`2 dic, Func`5 func, T2 p)

   at Prime.TestMethods.LSARaster.PrimeLSARasterTestMethod.ExportDefectsToRepair(String outputTag, Dictionary`2 db)

   ...

   at Prime.TestMethods.TestMethodBase.Prime.Kernel.ITelemetryAwareTestMethod.InnerExecute()

   at Prime.Kernel.PrimeKernel.InnerExecute()

   at Prime.Kernel.PrimeKernel.Execute()
[2023-Oct-30 09:48:39.614][A][TAL][DUT: 1] StopTest <PrimeTestName>::<TestPlanName>::<TestInstanceName>
```

# TPL Samples

Here are a few test instance examples using the ScanLYA test method**  

```python
CSharpTest PrimeScanLYATestMethod ScanLYA_ATPGFail_ChainPass_P2
{
    Patlist = "passing_scanlya_atpg";
    ChainPatlist = "passing_scanlya_chain";
    LevelsTc = "Scan::basic_func_lvl_nom";
    TimingsTc = "Scan::basic_func_timing_10MHz_20MHz";
    PerPatFailCaptureCount = 90;
    TotalFailCaptureCount = 90;
    HRYInputFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Scan/Modules/Scan/InputFiles/TemplateHrySCAN.json";
    ChainHRYInputFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Scan/Modules/Scan/InputFiles/TemplateHrySCAN.json";
    PinMappingFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Scan/Modules/Scan/InputFiles/PinMappingHPTPWithBusSize.ScanPinMapping.json";
    LogLevel = "Enabled";
}

CSharpTest PrimeScanLYATestMethod ScanLYA_DportPass_P4
{
    Patlist = "passing_scanlya_atpg";
    ChainPatlist = "passing_scanlya_chain";
    DportPatlist = "passing_scanlya_dport";
    LevelsTc = "Scan::basic_func_lvl_nom";
    TimingsTc = "Scan::basic_func_timing_10MHz_20MHz";
    PerPatFailCaptureCount = 90;
    TotalFailCaptureCount = 90;
    HRYInputFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Scan/Modules/Scan/InputFiles/TemplateHrySCAN.json";
    ChainHRYInputFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Scan/Modules/Scan/InputFiles/TemplateHrySCAN.json";
    PinMappingFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Scan/Modules/Scan/InputFiles/PinMappingHPTPWithBusSize.ScanPinMapping.json";
    LogLevel = "Enabled";
}

CSharpTest PrimeScanLYATestMethod ScanLYA_ATPGChainFail_WithPartitionMask_F3
{
    Patlist = "passing_scanlya_atpg";
    ChainPatlist = "passing_scanlya_chain";
    LevelsTc = "Scan::basic_func_lvl_nom";
    TimingsTc = "Scan::basic_func_timing_10MHz_20MHz";
    PerPatFailCaptureCount = 90;
    TotalFailCaptureCount = 90;
    PartitionsToIgnore = "pariirp,pariiommu";
    HRYInputFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Scan/Modules/Scan/InputFiles/TemplateHrySCAN.json";
    ChainHRYInputFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Scan/Modules/Scan/InputFiles/TemplateHrySCAN.json";
    PinMappingFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Scan/Modules/Scan/InputFiles/PinMappingHPTPWithBusSize.ScanPinMapping.json";
    LogLevel = "Enabled";
}

CSharpTest PrimeScanLYATestMethod ScanLYA_SingleATPGFail_F3
{
    Patlist = "passing_scanlya_atpg";
    LevelsTc = "Scan::basic_func_lvl_nom";
    TimingsTc = "Scan::basic_func_timing_10MHz_20MHz";
    PerPatFailCaptureCount = 90;
    TotalFailCaptureCount = 90;
    PartitionsToIgnore = "pariirp,pariiommu";
    HRYInputFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Scan/Modules/Scan/InputFiles/TemplateHrySCAN.json";
    PinMappingFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Scan/Modules/Scan/InputFiles/PinMappingHPTPWithBusSize.ScanPinMapping.json";
    LogLevel = "Enabled";
}

CSharpTest PrimeScanLYATestMethod ScanLYA_NoDefaultATPGPatlist_ChainOnly_FNEG1
{
	Patlist = "";
	ChainPatlist = "passing_scanlya_chain";
    LevelsTc = "Scan::basic_func_lvl_nom";
    TimingsTc = "Scan::basic_func_timing_10MHz_20MHz";
    PerPatFailCaptureCount = 90;
    TotalFailCaptureCount = 90;
    PartitionsToIgnore = "pariirp,pariiommu";
	HRYInputFile = "";
    PinMappingFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Scan/Modules/Scan/InputFiles/PinMappingHPTPWithBusSize.ScanPinMapping.json";
    LogLevel = "Enabled";
}
```
# Exit Ports

The ScanLYA test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                                          |
| ------------- | ------------- | ---------------------------------------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition.                                                                     |
| **-1**        | ***Error***   | Any software condition error.                                                            |
| **0**         | ***Fail***    | Failing condition.                                                                       |
| **1**         | ***Pass***    | ATPG Patlist passing condition.               |
| **2**         | ***Pass***    | CHAIN Patlist passing condition.  |
| **3**         | ***Fail***    | Single ATPG Patlist or CHAIN Patlist failing condition. |
| **4**         | ***Pass***    | Dport Patlist passing condition.              |
| **5**         | ***Fail***    | Dport Patlist failing condition.              |

# Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**          |
| ------------------------- | ----------- | ----------------- | --------------------- |
| Jan 19<sup>th</sup>, 2022 | 1.0.0       | Brian J. Morera   | Initial Version.      |
| Oct 18<sup>th</sup>, 2023  | 12.3 | Didier Armando Jimenez Retana | [Prime global processing results timeout to avoid hang](https://dev.azure.com/mit-us/PRIME/_workitems/edit/38198) |
| Jan 22<sup>nd</sup>, 2024 | 12.3.1 | Tiow, Hian Seng | [Enable PinMappingFile and SharedStorageHRYExportKey parameter](https://dev.azure.com/mit-us/PRIME/_workitems/edit/43729/) |
| May 10<sup>th</sup>, 2024 | 13.1.0 | Tiow, Hian Seng | [[SCAN] ScanSSNHRY new feature to support HPTP](https://dev.azure.com/mit-us/PRIME/_workitems/edit/46839/) |
| Jan 8<sup>th</sup>, 2025 | 13.2.0 | Tiow, Hian Seng | [[ScanLYA] Mega Template](https://dev.azure.com/mit-us/PRIME/_workitems/edit/49023/) |
| Mar 13<sup>th</sup>, 2025 | 13.2.0 | Tiow, Hian Seng | [[ScanLYA] Enable single-plist-only execution with capability to control SPOFI logging](https://dev.azure.com/mit-us/PRIME/_workitems/edit/54470/)|
| Mar 31<sup>st</sup>, 2025  | 13.2 | McDonald, Lauren | [ScanSPOFI-lite: reduced printing for SSN content types] (https://dev.azure.com/mit-us/PRIME/_workitems/edit/57092/) |
| Jun 24<sup></sup>, 2025 | 13.3 ER1 | Tiow, Hian Seng | [[ScanLYA] Optimization for SPOFI to include partition name in ScanLYA.](https://dev.azure.com/mit-us/PRIME/_workitems/edit/56578/) |

# Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SSN**: Streaming Scan Network