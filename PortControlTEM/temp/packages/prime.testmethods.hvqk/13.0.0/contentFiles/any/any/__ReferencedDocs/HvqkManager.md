[[_TOC_]]

## REP for HvqkManager

This **REP** is intended to describe the HvqkManager Prime TestMethod.

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
This Test Method is to works with Prime Hvqk Test Method. Please refer to Prime Hvqk Test Method for Hvqk details.

This HvqkManager has 2 modes, for interacting with data results table sharedstorage built from Hvqk test method.

The START MODE is to clear all results from the data results table without wiping the whole table.

The END MODE is to print all the data results from sharedstorage, the printing details will be at Datalog Output.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the CurrentDieIdManager test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Default Value** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ | -----|
| HvqkModes  | Yes           | String(Choices)          | Enum START and END mode to choose.|              |

*Hvqk TM and HvqkManager TM verify will trigger metadata population if its empty. HVQK metadata file is read from a uservar "HVQKVars.PATH_TO_METADATA"
 
## Datalog output
There are three levels of results, rollup required as shown in Figure 2.
- Local (Instance) level results represent the lowest passing voltage observed at the instance level during waterfall. 
- Aggregate (Domain) level results is the lowest voltage across a group of instance results designed to group multiple instances within a single rollup token e.g. Core or GT.
- Test Program level results represents the lowest voltage achieved across all instance levels.  Instance and Domain results are logged as voltage values. The Test Program level result is logged as in “indicator level” which represents the lowest voltage step achieved.  This “indicator level” is logged due to the multiple voltages that can be applied across different power domains.

![image.png](./.attachments/TieredTokenAggregation.png)

| **Token Level** | **Name** | **Units** | **Description** |
| Test Program | HVQK_MAX_INDICATOR | Indicator Level <5:0> | The maximum indicator identifies the lowest voltage step observed across all aggregate and local results. |
| Aggregate | HVQK_<aggregate>_VOLTAGE | Volts (V) | Lowest voltage achieved within an aggregate group of local results. Local results within an aggregate groups must have equivalent voltages at each step |
| Local | HVQK_<aggregate>_<local>_VOLTAGE | Volts (V) | Lowest voltage achieved during single waterfall execution. If failure occurs at lowest voltage step, value of -1 is logged into local result. |

Example encoding of indicator level vs aggregate vs local results:

![image.png](./.attachments/ExampleResultsTable.png)

In the example above, the green boxes represent the resultant voltages across all local results.
The voltage value identified would be logged in representative local results.

The aggregate results represent the lowest voltage achieved within each grouping of aggregate level.  In this example, the aggregate results that would be logged include:
- HVQK_IA_VOLTAGE = 1.2
- HVQK_GT_VOLTAGE = 1.5
- HVQK_SOC_VOLTAGE = 1.6
- HVQK_IO_VOLTAGE = 1.6

The Test Program level result represents the lowest voltage achieved across all local (aggregate results).  Since multiple voltages are associated with each “domain”, the max indicator would represent the step associated with the lowest voltage. 
- HVQK_MAX_INDICATOR = 1 (Represents the lowest indicator level: HVQK_IA_CORESBFT_VOLTAGE = 1.2)

With the same aggregation, There are also alarm status for every instances and domains level, the tname in ituff line will have extra "CLAMP".
Each token should have one of 4 defined values:
- 0 - no alarm occurred
- 1 - alarm occurred
- 555 - instance skipped, no previous stress
- 999 - instance skipped, unit stressed previously

There is a possibility to define non- standard results logging format in INIT mode setup file. (see more here).
In non standard mode users provides the non standard sub-flow ID in the setup file (separate INIT mode instance which defines among other the instances of the non standard flow )
This ID is printed in all the results tokens instead “HVQK” prefix

For example if the non standard flow ID = “SDTSTRESS” – the aggregated results will be printed as below

SDTSTRESS_SDTSOC_CACHE_VOLTAGE = 1.6
SDTSTRESS_SDTSOC_SCAN_VOLTAGE = 1.6
SDTSTRESS_SDTSOC_VOLTAGE = 1.6
SDTSTRESS_SDTGT_CACHE_VOLTAGE = 1.6
SDTSTRESS_SDTGT_SCAN_VOLTAGE = 1.6
SDTSTRESS_SDTGT_VOLTAGE = 1.6

For non standard flow max indicator will not be printed.

Indication of previously stressed unit
Normally the unit needs to be stressed only once in a lifetime
But there are cases indicated by Q&R where unit needs to be re-stressed according to the below logic.
The indication whether or the unit was stressed is done through GSDS called "G.U.S.WASSTRESS" or at Prime prespective on WASSTRESS string table in sharedstorage under context of DUT.

In the event that stress was previously applied and stress was skipped for the current sequence, all tokens are logged with a value of “999” indicating stress was skipped.  This applies to ALL tokens including HVQK_MAX_INDICATOR.  
In the event NON_STANDARD stress is applied through a force stress, the value of WASSTRESS is overrided and actual results are logged


This is the example screenshot of ituff print

![image.png](./.attachments/ItuffPrintExample.png)

## Custom User Code Hooks
N/A

## TPL Samples
**Test Instance Sample on Start Mode**
```
Test PrimeHvqkManagerTestMethod PrimeHvqkManagerStart_P1
{
    HvqkMode = "START";
    BypassPort = -1;
    LogLevel = "PRIME_DEBUG";
}
```

**Test Instance Sample on End Mode**
```
Test PrimeHvqkManagerTestMethod PrimeHvqkManagerEnd_P1
{
    HvqkMode = "END";
    BypassPort = -1;
    LogLevel = "PRIME_DEBUG";
}
```

## Exit Ports

The Hvqk test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

  
## Additional Dependencies

N/A

## Version tracking

| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| November, 2023 | 13.0.0       | Lee, Yeong Jui| [42744](https://dev.azure.com/mit-us/PRIME/_workitems/edit/42744) [HVQK] Creating HVQK Clamp roll-up tokens similar to Waterfall tokens|         |
| October, 2023 | 12.3.0       | Lee, Yeong Jui| [42661](https://dev.azure.com/mit-us/PRIME/_workitems/edit/42661/) [HVQK] HVQK & HVQK manager rules that need to be applied             |
| April, 2023 | 12.1.0       | Lee, Yeong Jui| [38935](https://dev.azure.com/mit-us/PRIME/_workitems/edit/38935/) Changing Get WASSTRESS SharedStorage from LOT Context to DUT.             |
| April, 2022 | 1.0.0       | Lee, Yeong Jui|              |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System