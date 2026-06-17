**prime Test-Method Specification REP**

Revision 1.0.0

Feb 2021

[[_TOC_]]

## REP for FlowLoopControl

This **REP** is intended to describe the FlowLoopControl Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Datalog output** – A detailed description of what is datalogged by this TestMethod

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document 

## Methodology

The FlowLoopControl test method modifies Flow loop config for an existing flowItem. 
Below is an example input json file format: 
```
{
	"FlowItemControl": [{
		"ConfigName": "MinFifteenSecondsLoop",
		"FlowItemsList": [{
			"FlowName": "Stress::StressTestMethods::PauseTest_TwoSeconds_P1",
			"SetLoopDurationInSeconds": "10",
			"SetBreakRange": "-2:-1"
		}]
	}, {
		"ConfigName": "SetTenLoops",
		"FlowItemsList": [{
			"FlowName": "Stress::StressTestMethods::PauseTest_TwoSeconds_P1",
			"SetCount": "10",
			"SetBreakRange": "-2:-1"
		}]
	}]
}
```
## Understanding JSON file
Following table describes the JSON file  inputs:
| **JSON input**           | **Required?** | **Contains**                                                             |   **Comments**   |
| ------------------------ | ------------- | ------------------------------------------------------------------------ | ---------------- |
| FlowItemControl          | Yes           | 'ConfigName', 'FlowItemsList'                                            | Is a master token, expected once per file.                 |
| ConfigName               | Yes           | String                                                                   | Alias to the configuration to be applied, Expected atleast once. |
| FlowItemsList            | Yes           | 'FlowName', ['SetLoopDurationInSeconds'/'SetLoopCount'], 'SetBreakRange' | Specifies exact modification items for the FlowItem listed.|
| FlowName                 | Yes           | String                                                                   | Specifies flow item name, as appears at a TestInstance. <Module>::<Flow>::<FlowItemInstanceName>|
| SetLoopDurationInSeconds | Yes*          | String                                                                   | Sets the minimum Loop Duration for a Flow Loop item in seconds.|
| SetLoopCount             | Yes*          | String                                                                   | Sets the Loop count for a Flow Loop Item.|
| SetBreakRange            | No            | String                                                                   | Sets or modifies the Break Range for this Flow Loop Item.|

As indicated in the table above [Yes*], as parameters SetLoopDurationInSeconds and SetLoopCount are exclusive to each other. If both are specified in the JSON file, the instance's verify would error out as port -1. 

## Test Instance Parameters
The table below lists and describes the test instance parameters supported by the FlowLoopControl test method

| **Parameter Name** | **Required?** | **Type** |        **Values**        |   **Comments**   |
| ------------------ | ------------- | -------- | ------------------------ | ---------------- |
| InputJsonConfigFile| Yes           | String   |                          | Input Json format file, that defines the flowItem(s) to be modified.                 |
| SetConfig          | Yes           | String   |                          | One of the ConfigName defined in the input json file.|
**Notes:**
## TPL Samples

Here are a few test instance examples using the FlowLoopControl test method
Example Input file breifing all possible combinations in the Json File:
```
{
	"FlowItemControl": [{
		"ConfigName": "MinFifteenSecondsLoop",
		"FlowItemsList": [{
			"FlowName": "Stress::StressTestMethods::PauseTest_TwoSeconds_P1",
			"SetLoopDurationInSeconds": "10",
			"SetBreakRange": "-2:-1"
		}]
	}, {
		"ConfigName": "SetMultipleLoopItems",
		"FlowItemsList": [{
			"FlowName": "Stress::StressTestMethods::PauseTest_TwoSeconds_P1",
			"SetLoopDurationInSeconds": "10",
			"SetBreakRange": "-2,-1,0"
		},
		{
			"FlowName": "Stress::StressTestMethods::PauseTest_FourSeconds_P1",
			"SetLoopCount": "5",
			"SetBreakRange": "-2:-1"
		}]
	}]
}
```

TPL Samples:  
```python
Import PrimeFlowLoopControlTestMethod.xml;
Test PrimeFlowLoopControlTestMethod FlowLoopControl_ChangeTime_P1
{
    SetConfig = "MinFifteenSecondsLoop";
    InputJsonConfigFile = "~HDMT_TPL_DIR\\Modules\\Stress\\InputFiles\\FLConfig.json";
}
Test PrimeFlowLoopControlTestMethod FlowLoopControl_ChangeTimeNCount_P1
{
    SetConfig = "SetMultipleLoopItems";
    InputJsonConfigFile = "~HDMT_TPL_DIR\\Modules\\Stress\\InputFiles\\FLConfig.json";
}
```
## Datalog output
NA.

## Exit Ports

The FlowLoopControl test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

## Additional Dependencies

NA.

## Version tracking


| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| Mar 26th, 2021 | 1.0.0       | Vipin Sunkari|              |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
