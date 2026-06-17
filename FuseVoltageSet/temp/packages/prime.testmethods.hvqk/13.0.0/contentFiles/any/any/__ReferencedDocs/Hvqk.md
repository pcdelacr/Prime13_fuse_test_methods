[[_TOC_]]

## REP for Hvqk

This **REP** is intended to describe the Hvqk Prime TestMethod.

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
HVQK is a Quality and Reliability (QnR) test screen in HVM programs which stresses the DUT in order to detect latent defects. It is an integral part of the overall Qualification strategy for a product. Having a robust HVQK solution is part of the MTR, or Minimum Test Requirements, utilized to approve product release qualification. The amount of stress applied to the DUT is utilized for several key QnR Metrics which both measure the health of the process and the reliability of the product. Determining the amount of stress is the key requirement of each program. HVQK is a waterfall flow where the voltage stresses applied are descending, step to step. Each power domain can have its own waterfall flow (e.g., Core, Graphics, etc.)

![image.png](./.attachments/ConceptualWaterFallFlow.png)


The test class algorithm manipulates the power pins (domains) per voltage_step, executes a plist, and then loops to the next voltage_step until a stop condition is found. 

Step 0: Resolve first waterfall step in voltage_start XML element in input file – it can be either explicit value or expression.  This is done during verify. The resolved value will be the first voltage_step executed in the algorithm.

Step 1: For the current voltage_step, apply all voltages in the vforce elements utilizing the sub-elements to specify pin, value, pattern override (if designated), and levels block (if designated).

Step 2: Execute the plist to determine pass or fail. Please note that some patterns can trigger a voltage change mid-burst execution. 

Step 3: If the pattern execution is a pass, continue to step 3a. If the execution resulted in an alarm, skip to Step 4. If the execution returned a plist failure, skip to Step 5

Step 3a: Assign the voltageToken the value of the current voltage_step and exit port 1. 

Step 4. Determine the value of alarm_action for the current voltage_step. If this switch is set to CONTINUE, proceed to Step 4a. If the switch is set to EXIT, exit port -2. 

Step 4a. Apply the powerDownOnClamp levels followed by the powerUpOnClamp levels – this will clear out the clamp. 

Step 4b. If this is the last voltage_step, assign the voltageToken a value of “-1” and exit port 2.

Step 4c. If this is not the last voltage_step, index to the next voltage_step and return to Step 1.

Step 5: Determine the value of plist_exec_fail action for this instance. If this switch is set to CONTINUE, proceed to Step 5a. If the switch is set to EXIT, skip to Step 3a.

Step 5a. If this is the last voltage_step, assign the voltageToken a value of “-1” and exit port 2.

Step 5b. If this is not the last voltage_step, index to the next voltage_step and return to Step 1.

Step 5c. If this is the voltage_step with the plist_exec_failure_action set to FAIL, note the voltage step, continue waterfall according to plist_exec_fail and exit port 3.

![image.png](./.attachments/FlowChart.png)

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Hvqk test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Default Value** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ | -----|
| VoltageStepConfigFile  | Yes           | String          | Input file which defines HVQK waterfall steps. Each step defines its details.|              |
| Patlist                | Yes           | Plist           | Plist name to be executed                                                    |              |
| LevelsTc               | Yes           | LevelsCondition | Levels test condition required for plist execution                           |              |
| TimingTc               | Yes           | TimingCondition | Timing test condition required for plist execution                           |              |
| PowerDownLevel         | No            | String          | Optional set of PowerDown level when alert happened                          |              |
| PowerUpLevel           | No            | String          | Optional set of PowerUp level when alert happened                            |              |
| DtsConfigurationName           | No            | String          | Configuration when DTS processing is needed.(Aleph Input file)                            |              |
| AlarmHandleDelay       | No            | Integer         | Optional set of time(millisecond) delay when alert happened                  |              |

*Hvqk TM and HvqkManager TM verify will trigger metadata population if its empty. HVQK metadata file is read from a uservar "HVQKVars.PATH_TO_METADATA"

For DtsProcessing, please refer the wiki page under ServicesSDK\DtsProcessingService.

## Json File Sample for HVQK Metadata
```
{PRIME Ticket 37369: [BRITA] Flow Trace Broken for SRF Power On TP - Prime v10
	"VoltageIndicators": [
		{
			"Name": "IA",
			"VoltageSteps": {
					"1.6": 7,
					"1.5": 6,
					"1.4": 5,
					"1.3": 4,
					"1.2": 3,
					"1.1": 2,
					"1.0": 1
				}
		},
		{
			"Name": "GT",
			"VoltageSteps": {
					"1.6": 7,
					"1.5": 6,
					"1.4": 5,
					"1.3": 4,
					"1.2": 3,
					"1.1": 2,
					"1.0": 1
				}
		},
		{
			"Name": "CORE",
			"VoltageSteps": {
					"1.6": 7,
					"1.5": 6,
					"1.4": 5,
					"1.3": 4,
					"1.2": 3,
					"1.1": 2,
					"1.0": 1
				}
		},
		{
			"Name": "SCAN",
			"VoltageSteps": {
					"1.6": 7,
					"1.5": 6,
					"1.4": 5,
					"1.3": 4,
					"1.2": 3,
					"1.1": 2,
					"1.0": 1
				}
		}
    ],
	"HvqkInstances": [
		{
			"Name": "IA",
			"InstancesMapping": [
					"IA1",
					"IA2",
					"IA3",
					"IA4"
		]
		},
		{
			"Name": "GT",
			"InstancesMapping": [
					"GT1",
					"GT2",
					"GT3"
		]
		},
		{
			"Name": "CORE",
			"InstancesMapping": [
					"CORE1",
					"CORE2"
		]
		},
		{
			"Name": "SCAN",
			"InstancesMapping": [
					"SCAN1",
					"SCAN2"
		]
		}
	]
}
```
## Json File Sample for HVQK Instance Config
```
{
	"DomainName" : "IA",
	"InstanceName" : "IA1",
	"VoltageStart" : "HVQKVars.Start_Voltage", 
	"VoltageStop" : "HVQKVars.Stop_Voltage",
	"Pin": [ "HDDPS_HC_nogang_12ohm1" ],
	"VoltageSteps": [
		{
			"Name": "1.6",
			"AlarmAction" : "CONTINUE",
			"PlistExecFailAction" : "CONTINUE",
			"Vforce": {
				"LevelsBlockDirect": ["HVQK::basic_func_lvl_nom", "HVQK::basic_func_lvl_nom"],
					"ForceValue" : 1.0
				}
		},
		{
			"Name": "1.5",
			"AlarmAction" : "CONTINUE",
			"PlistExecFailAction" : "CONTINUE",
			"Vforce": {
					"ForceValue" : 1.4
				}
		},
		{
			"Name": "1.4",
			"AlarmAction" : "CONTINUE",
			"PlistExecFailAction" : "CONTINUE",
			"Vforce": {
					"ForceValue" : 1.2
				}
		},
		{
			"Name": "1.3",
			"AlarmAction" : "CONTINUE",
			"PlistExecFailAction" : "CONTINUE",
			"Vforce": {
					"ForceValue" : 0.5,
					"TriggerItem" : "HVQK::ExternalTrigger"
				}
		},
		{
			"Name": "1.2",
			"AlarmAction" : "EXIT",
			"PlistExecFailAction" : "CONTINUE",
			"Vforce": {
					"ForceValue" : 1.2,
					"PlistOverride" : "passing_plist"
				}
		},
		{
			"Name": "1.1",
			"AlarmAction" : "EXIT",
			"PlistExecFailAction" : "CONTINUE",
			"Vforce": {
					"ForceValue" : 1.1
				}
		},
    ]
}
```
**Note

-VoltageStart and VoltageStop are dynamic parameter with able to accepting value as static number, or a uservar.


## Datalog output
Currently the fail and pass execution ituff print will be similar to functional test method.

## Custom User Code Hooks
Hvqk test method supports the following extensions:

### void ExecutePreStepUserCode()
- this is usercode allow to intercept right before the stress is applied.

### void ExecutePrePlistUserCode()
- this is usercode that allow to intercept right before the plist is executed.

### void ExecutePostPlistUserCode()
- this is usercode that allow to intercept right after the plist is executed.

### void ExecutePostStepUserCode()
- this is usercode allow to intercept right after the stress is applied.

## TPL Samples
```
Test PrimeHvqkTestMethod HvqkCore_Core1Pass_P1
{
    VoltageStepConfigFile = "~HDMT_TPL_DIR/Modules/HVQK/HVQK/InputFiles/CORE_CORE1.hvqk.config.json";
    Patlist = "failures_plist";
    LevelsTc = "HVQK::basic_func_lvl_nom";
    TimingsTc = "HVQK::basic_func_timing_10MHz_20MHz";
	PowerDownLevel = "";
	PowerUpLevel = "";
	AlarmHandleDelay = "";
}
```

## Exit Ports

The Hvqk test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**         | ***Alarm***      | Alarm condition            |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |
| **2**         | ***Pass***      | Plist Execution Failed On Lowest Voltage            |
| **3**         | ***Pass***      | Plist Execution Failed On User Configured VMax            |

  
## Additional Dependencies

N/A

## Version tracking

| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| October, 2023 | 13.0.0       | Lee, Yeong Jui| [39334](https://dev.azure.com/mit-us/PRIME/_workitems/edit/39334/): Create new extension and adding DtsProcessing Capabilities to Hvqk TM |
| April, 2022 | 1.0.0       | Lee, Yeong Jui|              |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System