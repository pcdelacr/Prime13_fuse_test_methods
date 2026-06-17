# REP for Prime's InitializeLibrary test method.

This **REP** is intended to describe the InitializeLibrary Prime TestMethod.

[[_TOC_]]

# Methodology

The InitializeLibrary test method provides several capabilities:

1)  Saves the Init's start time.
2)  Initialize the init controller for the Init flow optimizations.
3)  Initialize Prime's out-of-the-box callbacks.

# Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Init test method

| **Parameter Name**      | **Required?** | **Type**                         | **Values**                                        | **Comments**                               |
| ----------------------- | ------------- | -------------------------------- |---------------------------------------------------| ------------------------------------------ |
| ForceFullInit           | No            | Choices                          | True(Default), False                              | When True, Prime will ignore any Init optimization and will force the full init flow. |
| PerformanceCounterLogLevel | No | Choices | None, GlobalOnly(Default), GlobalAndInstance      |  Options to enable Performance Counter logging. _GlobalOnly_ will only log TP level data. GlobalAndInstance will log TP level data and instance level data. Please refer to Performance Counter SDK documentations for more details.
| PerformanceCounterSampleRate | No | Integer | Defaulted to 1, any positive integer is accepted. | This option is to enable the counter only on N<sup>th</sup> samples. Example: Value of 3 will enable counter on 3rd sample. 0 will disable the counter ("don't count any sample"), 1 will enable the counter on the sample.
| TelemetryDebugInstances | No | String |                                                   | The path a file with a list of test instances to set the "TelemetryLevel=Debug" |
| GlobalSettingsFilePath | No | String |                                                   | The path to the global settings file. |
# Settings File
Some global settings in the library can be modified thru the `GlobalSetttingsFilePath`.
This file is a simple text file where each line is a key-value pair separated by a colon sign (`:`).
Example:
```
# This is a comment
SettingKey1 : SettingValue1
SettingKey2 : SettingValue2
```

The different settings that can be modified are specific per feature.
As these settings are global, they only apply on the DUT 0.
The test instance will only apply (refreshed) the changed settings from the last time they were applied.

# Datalog output

This Test Method does not create a datalog

# Custom User Code Hooks

**NA**

# TPL Samples

Here are a few test instance examples using the InitializeLibrary test method

**TPL Sample1**

```python
Import PrimeInitializeLibraryTestMethod.xml;

Test PrimeInitializeLibraryTestMethod PrimeInitializeLibrary
{
}
```

# Test Program Flow order requirements

The image below ilustrates the order requiremetns for Prime instances to correctly <font size="3"><span style="color:OrangeRed">Initialize</font> the libraries.

1. PrimeInitializeLibraryTestMethod
1. PrimeInitializeServicesTestMethod (Aleph)
1. PrimeInitializeInstancesTestMethod

::: mermaid
graph LR
  subgraph Init Subflow
  direction LR
  id2(PrimeInitializeLibraryTestMethod):::PRIME -->
  id3(PrimeInitializeServicesTestMethod):::PRIME -->
  id4(PrimeInitializeInstancesTestMethod):::PRIME
  end
classDef PRIME fill:#005B85;
:::

# TelemetryDebugInstances parameter
This parameter expects the path to a text file with a list of test instances to set
the `TelemetryLevel=Debug`.
This feature is very useful when debugging test program performance and the engineer needs to collect the telemetry
targeting a set of test instances.
A full instance name (as showed in the hdmtOS GUI) is expected per line, and empty lines or starting with `#` will be ignored.

# Exit Ports

The InitializeLibrary test method supports the following exit ports:

 
| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |
| **N**         | ***Pass/Fail*** | Failing condition            |

# Additional Dependencies

**NA**

# Version tracking

| **Date**       | **Version** | **Author**     | **Comments**    |
| -------------- | ----------- | -------------- | --------------- |
| Apr 5th, 2020  | 1.0.0       | Zamir Zuriel   | Initial version |
| Aug 31st, 2021 | 1.1.0       | Javier Alpizar | Add service files path saving and JSON Schema Validation features |
| Feb 10th, 2022 | 8.1.0       | Lim, Xin Yan   | Enable TorchRulesVar.AlephEnvVariable to support dynamic env var name for aleph files |
| Apr 22nd, 2022 | 9.1.0       | Lim, Xin Yan   | Enable TorchPrimeVar.AlephEnvVariable to support dynamic env var name for aleph files |
| Apr 22nd, 2022 | 9.1.0       | Lim, Xin Yan   | Fix bug for supporting dynamic env var name for aleph files |
| Jul 11th, 2022 | 11.0.0      | lchavarr       | Split Init's test methods |
| Aug 12, 2022 | 11.0.1 | hramirez | Adding Flow order documentation |
| Aug 13, 2024 | 13.01.00 | ckhoh    | Update Flow order documentation |

# Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System