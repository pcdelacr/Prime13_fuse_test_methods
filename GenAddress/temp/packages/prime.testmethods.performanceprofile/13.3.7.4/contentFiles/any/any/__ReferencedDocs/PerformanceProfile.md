[[_TOC_]]

## REP for PerformanceProfile

This **REP** is intended to describe the PerformanceProfile Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Console output** – A detailed description of what is being printed out in Console in debug mode by this TestMethod
  
  - **Datalog output** – A detailed description of what is datalogged by this TestMethod

  - **Telemetry level setting** – A reference to Telemetry log levels

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document

## Methodology

The PerformanceProfile test method provides capability to enable/disable test time summary, memory summary, and Telemetry logging for all instances in a test plan.

### Ituff level consideration.

Be mindful when enabling this outside of main flow of a TP. As the summary is printed to datalog with level 2 strgval, some test methods might cause the datalog to be printed in out of order; particularly for those test methods that prints header and footer. When dealing around these test methods, do set GlobalInstanceSummaryMode and MemorySummary to "DISABLED" before these test methods are executed.

Generally, avoid enabling them in Lot Start and Lot End flow as this will definitely cause ituff to be out-of-order. If you need to enable this in TestPlanStart flow or TestPlanEnd flow, do exercise caution on the Device Starts and Device End test methods. 


Do not enable before these test methods are executed:-
1. PrimeLotStartSetupTestMethod
2. PrimeLotStartDatalogTestMethod
3. PrimeDeviceStartSetupTestMethod
4. PrimeDeviceStartSingleDieDatalogTestMethod
5. PrimeDeviceStartWaferDatalogTestMethod
6. PrimeDeviceStartPackageDatalog

Do disable right before these test methods are executed:-
1. PrimeDeviceEndDatalogTestMethod
2. PrimeDeviceEndFinalizeTestMethod
3. PrimeLotEndDatalogTestMethod
4. PrimeLotEndFinalizeTestMethod

### Suggested Flow:
#### **Init Flow**:
Do not enable *GlobalInstanceSummaryMode* and *MemorySummary*. Ituff level will be mixed and will be invalid

#### **Lot Start Flow**:
Do not enable *GlobalInstanceSummaryMode* and *MemorySummary*. Ituff level will be mixed and will be invalid

#### **Test Plan Start Flow**:
::: mermaid
  graph LR
    A[HMI_TestPlanSetup]-->B[PrimeDeviceStartSetupTestMethod]-->C[PrimeDeviceStart<***>DatalogTestMethod]-->D["PrimePerformanceProfileTestMethod.GlobalInstanceSummaryMode = Enabled"]
:::

#### **Test Plan End Flow**:
::: mermaid
  graph LR
    A["PrimePerformanceProfileTestMethod.GlobalInstanceSummaryMode = Disabled"]-->B[PrimeDeviceEndDatalogTestMethod]-->C[**PrimeOdeseBinConverterTestMethod**]-->D[PrimeDeviceEndFinalizeTestMethod]-->E[HMI_TestPlanSetup]
:::

#### **Lot End Flow**:
Do not enable *GlobalInstanceSummaryMode* and *MemorySummary*. Ituff level will be mixed and will be invalid

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the PerformanceProfile test method

| **Parameter Name**             | **Required?** | **Type**        | **Values**              | **Comments**                                            |
| ------------------             | ------------- | --------------- | ------------------------| ------------------------------------------------------- |
| GlobalInstanceSummaryMode      | No            | String (choice) | Disabled (default)      | Default - Disables test time summary print to ituff.    |
|                                |               |                 | Enabled                 | Enables test time summary print to ituff.               |
| MemorySummary                  | No            | String (choice) | Disabled (default)      | Default - Disables memory summary print to ituff.       |
|                                |               |                 | Enabled                 | Enables memory summary print to ituff.                  |
| GlobalTelemetryLevel           | No            | String (choice) | None (default)          | Default - Disables Telemetry logging globally           |
|                                |               |                 | Debug                   | Enables Telemetry logging of Debug events               |
|                                |               |                 | Information             | Enables Telemetry logging of Information events         |
|                                |               |                 | Critical                | Enables Telemetry logging of Critical events            |

 
## What is in the console output?

Below is a console output when GlobalInstanceSummaryMode and MemorySummary is set to Enabled:

```
=========================
Running Execute() for test instance=[UserVarTestMethod::ReadBooleanUserVar_P1]
=========================
[2022-Feb-09 10:38:25.679][DUT: 1]Printed to ituff:
[2022-Feb-09 10:38:25.679][DUT: 1]2_tname_UserVarTestMethod::ReadBooleanUserVar_P1_BOOLEAN
[2022-Feb-09 10:38:25.679][DUT: 1]2_strgval_False
[2022-Feb-09 10:38:25.679][DUT: 1]
[2022-Feb-09 10:38:25.687][DUT: 1]Printed to ituff:
[2022-Feb-09 10:38:25.687][DUT: 1]2_tname_testtime_UserVarTestMethod::ReadBooleanUserVar_P1
[2022-Feb-09 10:38:25.687][DUT: 1]2_strgval_per_0.0ms_main_2.114ms_et_2.114ms
[2022-Feb-09 10:38:25.687][DUT: 1]Printed to ituff:
[2022-Feb-09 10:38:25.687][DUT: 1]2_tname_MemoryProfile_UserVarTestMethod::ReadBooleanUserVar_P1
[2022-Feb-09 10:38:25.687][DUT: 1]2_strgval_PrivateBytes_Start=501231616B_End=501231616B_Delta=0B
[2022-Feb-09 10:38:25.687][DUT: 1]Test instance=[UserVarTestMethod::ReadBooleanUserVar_P1] executed using 16.132218 ms
[2022-Feb-09 10:38:25.687][DUT: 1]TestInstance=[UserVarTestMethod::ReadBooleanUserVar_P1] exit port=[1].
```
The above console output contains the total test time and memory consumed for the particular test instance.

## Datalog output

Below is a datalog output when GlobalInstanceSummaryMode is set to Enabled:

```
2_tname_testtime_UserVarTestMethod::ReadBooleanUserVar_P1
2_strgval_pre_0.0ms_main_2.114ms_et_2.114ms
```

Below is a datalog output when MemorySummary is set to Enabled:

```
2_tname_MemoryProfile_UserVarTestMethod::ReadBooleanUserVar_P1
2_strgval_PrivateBytes_Start=501231616B_End=501231616B_Delta=0B
```

## Telemetry level setting

The GlobalTelemetryLevel parameter allows to define a global Telemetry log level to be used all across the test plan execution.

For more information on how to instrument and enable Telemetry in Prime or the meaning of the different log levels, please refer to the [Telemetry wiki](https://dev.azure.com/mit-us/PRIME/_wiki/wikis/PRIME.wiki/69226/Instrument-Prime-with-Telemetry)

## TPL Samples

Here are a few test instance examples using the PerformanceProfile test method

```python
Import PrimePerformanceProfileTestMethod.xml;
Test PrimePerformanceProfileTestMethod GlobalSummaryModeOn_P1
{
	GlobalInstanceSummaryMode = "Enabled";
	MemorySummary = "Enabled";
	LogLevel = "DISABLED";
}
```

```python
Import PrimePerformanceProfileTestMethod.xml;
Test PrimePerformanceProfileTestMethod DefineGlobalTelemetryLevel_P1
{
	GlobalTelemetryLevel = "Information";
	LogLevel = "DISABLED";
}
```

## Exit Ports

The PerformanceProfile test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**                              |
| ------------- | ------------- | -------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                          |
| **-1**        | ***Error***   | Any software condition error                 |
| **0**         | ***Fail***    | Failing condition.                           |
| **1**         | ***Pass***    | Passing condition                            |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
**

## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**                                                                                                                         |
| ------------------------- | ----------- | ----------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Feb 7<sup>th</sup>, 2022  | 1.0.0       | Ong Ping Ping     | Initial version                                                                                                                      |
| Aug 28<sup>th</sup>, 2023 | 2.0.0       | Alejandro Vasquez | Adjusted documentation after Telemetry and Instance Summary Mode changes [Prime 12]                                                  |
| Sept 9<sup>th</sup>, 2023 | 2.1.0       | Yusof, Adam Malik | Information on ituff level consideration and the recommended flow around the affected test methods.                                  |
| Mar 22<sup>th</sup>, 2024 | 12.05.00    | Chen Tat Khoh     | Change GlobalInstanceSummaryMode to print test time summary only to ituff, add MemorySummary to print memory summary only to ituff.  |
