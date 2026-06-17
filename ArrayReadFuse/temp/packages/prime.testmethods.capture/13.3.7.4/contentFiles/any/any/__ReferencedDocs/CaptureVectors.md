# PrimeCaptureVectorsTestMethod User Guide
This doc is intended to help you get started using the PrimeCaptureVectorsTestMethod, or otherwise known as ***CaptureVectors***.

# Use Cases
***CaptureVectors*** executes a functional test, collects data from pins defined by the user, then concatenates the data for insertion into SharedStorage.

# Parameters

| **Name**            | **Required?**                   | **Type** | **Values**                                                                                               |
|---------------------|---------------------------------|----------|----------------------------------------------------------------------------------------------------------|
| LevelsTC            | Yes                             | String   | Levels condition to apply for the functional test.                                                       |
| TimingsTC           | Yes                             | String   | Timings condition to apply for the functional test                                                       |
| PatList             | Yes                             | String   | Plist to execute for the functional test                                                                 |
| PrePlist            | No                              | String   | Plist to execute before executing PatList                                                                |
| Pins                | Yes                             | String   | Pins to capture data on during functional test execution.                                                |
| MaskPins            | No                              | String   | Pins to mask for functional test execution.                                                              |
| TotalCaptureCount   | Optional (default = 1000000)    | Int      | Maximum number of fails allowed to be captured by the functional test.                                   |
| ApplyEndSequence    | No (default = disabled)         | String   | To apply the end sequence after test method execution.                                                   |
| KeyForSharedStorage | Yes                             | String   | Key to use to insert the concatenated vector data into SharedStorage.                                    |
| FunctionalDataToUse | Yes                             | String   | "CTV", "PerCycleFails" The type of data to use when capturing data on your designated pins.              |
| DecodingMode        | Yes                             | String   | "PerPin", "PerSequence" Order your vector data sequentially by pin or by cycle.                          |
| Timeout             | Optional (default = no timeout) | Int      | The amount of time in (ms) to execute the test method before exiting out port 0                          |
| ReverseVectorOutput | Optional (default = false)      | String   | "True"/"False". Determines if the collected vectors should be reversed before storing in shared storage. |

## DecodingMode = 
The table below describes the behavior for each 'DecodingMode'.

| **Mode**    | **Function**                                                                                                                                                                  |
|-------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| PerPin      | Collect vector data for all defined data pins, then concatenate the data from each pin in the order of the data pins defined in your configuration.                           |
| PerSequence | Collect vector data for all defined data pins, then concatenate each cycle of the vector data. The order of the bits in each vector is defined by the order of the data pins. |

# Algorithms for "DecodingModes"

For both explanations/examples, the following pattern data is used:

| **Cycle** | **pin_00** | **pin_01** | **pin_02** | 
|-----------|------------|------------|------------|
| 1         | 1          | 1          | 1          |
| 2         | 1          | 1          | 0          |
| 3         | 0          | 1          | 1          |
| 4         | 1          | 1          | 0          |
| 5         | 0          | 1          | 1          |

This data can be created from collection of CTVs or from PerCycleFails (pattern failures).

## PerPin
All failing bits collected from a pin are formed into a string. All strings for each pin defined in test instance's Pins parameter are concatenated together in the order of the pins defined in your Pins parameter.

Ex.
Lets say we define the following test instance in your test program:

```
Test PrimeCaptureVectorsTestMethod MyExample
{
    ...
    Pins = "pin_00,pin_01,pin_02";
    FunctionalDataToUse = (CTV or PerCycleFails, doesn't matter);
    Mode = "PerPin";
    ...
}
```

Here is the data coming from each pin based off the vector data provided above:

```
var pin_00 = 11010
var pin_01 = 11111 
var pin_02 = 10101
```

The resulting final string that will be inserted into shared storage will be:
```
var myFinalString = pin_00 + pin_01 + pin_02 (concatenated in order defined in test instance's Pins parameter)
myFinalString == "11010" + "11111" + "10101"
```

## PerSequence
Bits are taken from each cycle from all defined Pins in the test instance's parameter, then formed into a string for that cycle. The order of the bits for a cycle's string are determined by the order of pins in the test instance's parameters.

Ex.
Lets say we define the following test instance in your test program:

```
Test PrimeCaptureVectorsTestMethod MyExample
{
    ...
    Pins = "pin_00,pin_01,pin_02";
    FunctionalDataToUse = (CTV or PerCycleFails, doesn't matter);
    Mode = "PerSequence";
    ...
}
```

Here is the data coming from each cycle of pattern fails:

```
var cycle1 = 111
var cycle2 = 110
var cycle3 = 011
var cycle4 = 110
var cycle5 = 011
```

The resulting final string that will be inserted into shared storage will be:
```
var myFinalString = cycle1 + cycle2 + cycle3 + cycle4 + cycle5 (concatenated in order of incoming cycles)
myFinalString == "111" + "110" + "011" + "110" + "011"
```

# TPL example
```
Test PrimeCaptureVectorsTestMethod CtvDataPerPinExample
{
    Patlist = "CtvPackets1_Plist";
    TimingsTc = "CaptureVectors::basic_timing_10MHz_20MHz";
    LevelsTc = "CaptureVectors::CaptureVectorsTC";
    KeyForSharedStorage = "PerPinMode_FailsSuccessfullyParsed_P1";
    Pins = "xxHPCC_DPIN_Dig_slcA_A2,xxHPCC_DPIN_Dig_slcA_A3,xxHPCC_DPIN_Dig_slcA_A4";
    FunctionalDataToUse = "CTV";
    Mode = "PerPin";
    LogLevel = "Enabled";
}

Test PrimeCaptureVectorsTestMethod CtvDataPerSequenceExample
{
    Patlist = "CtvPackets1_Plist";
    TimingsTc = "CaptureVectors::basic_timing_10MHz_20MHz";
    LevelsTc = "CaptureVectors::CaptureVectorsTC";
    KeyForSharedStorage = "PerSequence_NoFuncErrors_P2";
    Pins = "xxHPCC_DPIN_Dig_slcA_A2,xxHPCC_DPIN_Dig_slcA_A3,xxHPCC_DPIN_Dig_slcA_A4";
    FunctionalDataToUse = "CTV";
    Mode = "PerSequence";
    LogLevel = "Enabled";
}

Test PrimeCaptureVectorsTestMethod PerCycleFailsDataPerPinExample
{
    Patlist = "CtvPackets1_Plist";
    TimingsTc = "CaptureVectors::basic_timing_10MHz_20MHz";
    LevelsTc = "CaptureVectors::CaptureVectorsTC";
    KeyForSharedStorage = "PerPinMode_FailsSuccessfullyParsed_P1";
    Pins = "xxHPCC_DPIN_Dig_slcA_A2,xxHPCC_DPIN_Dig_slcA_A3,xxHPCC_DPIN_Dig_slcA_A4";
    FunctionalDataToUse = "PerCycleFails";
    Mode = "PerPin";
    LogLevel = "Enabled";
}

Test PrimeCaptureVectorsTestMethod PerCycleFailsDataPerSequenceExample
{
    Patlist = "CtvPackets1_Plist";
    TimingsTc = "CaptureVectors::basic_timing_10MHz_20MHz";
    LevelsTc = "CaptureVectors::CaptureVectorsTC";
    KeyForSharedStorage = "PerSequence_NoFuncErrors_P2";
    Pins = "xxHPCC_DPIN_Dig_slcA_A2,xxHPCC_DPIN_Dig_slcA_A3,xxHPCC_DPIN_Dig_slcA_A4";
    FunctionalDataToUse = "PerCycleFails";
    Mode = "PerSequence";
    LogLevel = "Enabled";
}
```

## Exit Ports
The ***CaptureVectors*** test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                                                        |
|---------------|---------------|--------------------------------------------------------------------------------------------------------|
| **-2**        | ***Alarm***   | Any alarm condition.                                                                                   |
| **-1**        | ***Error***   | Unexpected software error occurred (unhandled exception).                                              |
| **0**         | ***Fail***    | Timeout or failing algorithm condition.                                                                |
| **1**         | ***Pass***    | Functional test failed. Data parsed and inserted into SharedStorage.                                   |
| **2**         | ***Pass***    | - Functional test had no fails.<br/> - Fail on amble pattern<br/> - Failing on pin not defined by user <br/> - All vectors captured "0" in all pins |

## Version tracking
| **Date**          | **Prime Version** | **Author**     | **Comments**                                                                                                                                                                                                                         |
|-------------------|-------------------|----------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| August 25, 2023   | 12.05.00          | Caio Fernandes | Initial Version.                                                                                                                                                                                                                     |
| November 21, 2023 | 12.05.00          | Caio Fernandes | Adding ReverseOutputVectors documentation.                                                                                                                                                                                           |
| June 6, 2024      | 12.06.00          | Caio Fernandes | Fixing behavior of CTV mode to "stop on first fail". Updating port 2 to match CapturePackets and old EVG spec. More specific documentation. Fixing "Prime version" column in version tracking to match true introduction of changes. |
| February 14, 2025      | 13.02.00          | Mohd Faiz Mohd Asri | Adding additional condition for the test method to exit port 2 when all CTV bits are zero following behaviour from EVG's CapturePacket template. |