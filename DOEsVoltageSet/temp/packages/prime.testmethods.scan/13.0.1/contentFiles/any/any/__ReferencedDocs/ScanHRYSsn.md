**prime Test-Method Specification REP**

Revision 2.0.1

Jan 2024

[[_TOC_]]

## REP for ScanHRYSSN

This **REP** is intended to describe the ScanHRY SSN Prime TestMethod.

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

The ScanHRYSSN test method provides capability to execute a scan plist, capture the failures, and process them using a `IStreamingScanNetwork` object to generate an HRY data that will be printed to Ituff datalog. For more information about decoding understanding, proceed to [SSN decoding](##SSNdecoding)] to understand the calculation.

The test method will take into account the channel-linking capability if used in Pattern list, when calculating the failure distance from last label.

Starting from Prime v12.3.1, ScanService is removed and business logic has been shift to ScanHRYSSN test method itself. The expected changes on the TP side are:
1. PinMap file no longer load from AlephInit, instead load from test instance using parameter PinMappingFile, similar to HRYInputFile.
2. Therefore, PinMappingFile parameter is required, along with the path.

Verify Flowchart

![image.png](./attachments/SSNHryVerifyFlowchart.png)

Execute Flowchart

![image.png](./attachments/SSNHryExecuteFlowchart.png)

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ScanHRY test method

| **Parameter Name**     | **Required?** | **Type**        | **Values**                                                                    | **Comments**            |
| ---------------------- | ------------- | --------------- | ----------------------------------------------------------------------------  | ----------------------- |
| Patlist                | Yes           | Plist           | Plist name to be executed.                                                    |                         |
| TimingTc               | Yes           | TimingCondition | Timing test condition required for plist execution.                           |                         |
| LevelsTc               | Yes           | LevelsCondition | Levels test condition required for plist execution.                           |                         |
| PrePlist               | No            | String          | PrePlist callback to plist execution.                                         |                         |
| MaskPins               | No            | String          | Comma separated list of pins for which the fail data capture will be skipped. | Default = Empty String  |
| TotalFailCaptureCount  | Yes           | UnsignedInteger | Number of total failures to capture.                                      |                         |
| PerPatFailCaptureCount | No            | UnsignedInteger | Number of per pattern failures to capture.                                    | Default = 1             |
| PatternsRegexesForKill | No            | CommaSeparatedString | List of regexes, when a pattern that failed matches one of this regexes, will exit from dedicated port 4. | Default = Empty String |
| PartitionsToIgnore | No            | CommaSeparatedString | List of partitions to ignore. When a failing partition matches on of the ignore partition the thes method will set a 9 in the HRY result. | Default = Empty String |
| ExpectedPartitionsStatuses | No            | CommaSeparatedString | List of expected partition statuses. List of values with format "PartitionName=0/1/8/9/X" (X - don't care.). This parameter does take effect if the plist does not report any failure. | Default = Empty String |
| SetUnassignedToUntestedPatterns               | No            | SetUnassignedToUntestedPatternsMode          | Sets to unassigned or 9 to the HRY result, to those partitions that don't have any pattern related in the current Patlist. Allowed values=[ENABLED, DISABLED]                                         |            Default = "DISABLED"             |
| HPTPMode               | No. Default is DISABLED | String (Choice) | Disable to use regular SSN to decode. Enable to use HPTP algorithm to decode. | Default = DISABLED |
| HRYInputFile           | Yes           | File            | .Json input file for HRY.                                                     |                         |
| PinMappingFile         | Yes           | File            | .Json input file for pinMapping    |      |
| SharedStorageHRYExportKey | No         | String | Key use to export HRY string to sharedStorage in IP scope. |            |

## Datalog output

The above [example](#example) will print to Ituff datalog the following data:

```
2_tname_<instance name>_HRY_RAWSTR
2_strgval_0111
2_tname_HRY_<instance name>_<failing parition>
2_strgval_0
```


# Configuration files
ScanHRYSSNTestMethod manage two configuration files, one is the [Pin mapping configuration file](#Pin-mapping-configuration-file) to provide a unique pin mapping, and other file is the [SSN configuration file](#SSN-configuration-file).

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
StarterBit = 16
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
SSN\_Position = 16 + 1 + 12
$$
$$
SSN\_Position = 29
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

```
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

Here are a few test instance examples using the ScanHRY test method**  

```python
Import PrimeScanHRYSSNTestMethod.xml;

Test PrimeScanHRYSSNTestMethod PrimeScanHRYSSN
{
   Patlist = "array_pbist_llc_dat_lya_autoclkmux0_s0_list";
   TimingsTc = "BASE::cpu_func_sdr_univ_sta_univ_univ_b100_t100_d100";
   LevelsTc = "__main__::leakage_tc";
   PerPatFailCaptureCount = 1;
   TotalFailCaptureCount = 10;
   HRYInputFile = "~HDMT_TPL_DIR/Modules/Scan/InputFiles/HRYSSNInputFileExample.json";
   PinMappingFile = "~HDMT_TPL_DIR/Modules/Scan/InputFiles/PinMapping.ScanPinMapping.json";
}
```
# Exit Ports

The ScanHRY test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                                          |
| ------------- | ------------- | ---------------------------------------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition.                                                                     |
| **-1**        | ***Error***   | Any software condition error.                                                            |
| **0**         | ***Fail***    | Failing condition.                                                                       |
| **1**         | ***Pass***    | Passing condition. Plist execution passed without capturing any failures.                |
| **2**         | ***Pass***    | Passing condition. Plist execution failed, and captured failures decoded successfully.   |
| **3**         | ***Fail***    | Failing condition. Plist execution failed, and captured failures decoded unsuccessfully. |
| **4**         | ***Fail***    | Failing condition. Plist execution failed matching kill patterns regexes.                |

# Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**          |
| ------------------------- | ----------- | ----------------- | --------------------- |
| Jan 19<sup>th</sup>, 2022 | 1.0.0       | Brian J. Morera   | Initial Version.      |
| Oct 18<sup>th</sup>, 2023  | 12.3 | Didier Armando Jimenez Retana | [Prime global processing results timeout to avoid hang](https://dev.azure.com/mit-us/PRIME/_workitems/edit/38198) |
| Jan 22<sup>nd</sup>, 2024 | 12.3.1 | Tiow, Hian Seng | [Enable PinMappingFile and SharedStorageHRYExportKey parameter](https://dev.azure.com/mit-us/PRIME/_workitems/edit/43729/) |
| May 10<sup>th</sup>, 2024 | 13.1.0 | Tiow, Hian Seng | [[SCAN] ScanSSNHRY new feature to support HPTP](https://dev.azure.com/mit-us/PRIME/_workitems/edit/46839/) |

# Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SSN**: Streaming Scan Network