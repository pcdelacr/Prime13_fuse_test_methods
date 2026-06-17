**Prime Test-Method Specification REP**

Revision 1.0.0

February 2022

[[_TOC_]]

## REP for VMeasureLoop
This **REP** is intended to describe the VMeasureLoop Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Datalog output** – A detailed description of what is datalogged by this TestMethod

  - **Custom User Code hooks** – A list of functions available to the user code to override

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document

## Methodology
The VMeasureLoop test method provides capability to perform voltage measurements across multiple pins,
with the option to do a relay switch before the measurement is taken.

Additionally, there are options to capture CTVs, execute a preplist element, and save measurements to Shared Storage.

## Test Instance Parameters
The table below lists and describes the test instance parameters supported by the VMeasureLoop test method.

| **Parameter Name** | **Required?** | **Type** |        **Values**        |   **Comments**                                                     |
| ------------------ | ------------- | -------- | ------------------------ | ------------------------------------------------------------------ |
| ConfigFile         | Yes           | String   |                          |                                                                    |
| CapturePins        | No            | String   |                          | Comma separated list of pins for which CTV data should be captured.|

**Notes:**
- The configuration JSON file parsing is done during the template Verify.

- Configuration File Example:
```python
{
	"TestSetups": [{
		"SetupLabel": "testSetup1",
		"PatternList": "vmeasureloop1_plist",
		"Timing": "VMeasureLoop::basic_func_timing",
		"Levels": "VMeasureLoop::SampleDcTC",
		"PrePlist": "SomeCallback(someParam)",
	},
	{
		"SetupLabel": "testSetup2",
		"PatternList": "vmeasureloop2_plist",
		"Timing": "VMeasureLoop::basic_func_timing",
		"Levels": "VMeasureLoop::SampleDcTC",
	},],
	
	"VoltageMeasurements": [{
		"RelayToken": "VMeasureLoop::Relay_TC",
		"Pins": "xxHC_nogang_2ohm,xxHC_nogang_1ohm",
		"PrePause": 0.020,
		"LowLimit": 0.0,
		"HighLimit": 5.0,
	},
	{
		"Pins": "xxLC_nogang_5ohm",
		"PrePause": 0.020,
		"LowLimit": 0.0,
		"HighLimit": 5.0,
		"SharedStorageToken": "StoreMeas",
	},],
	
	"PostTestSetup": {
		"PatternList": "vmeasurelooppost_plist",
		"Timing": "VMeasureLoop::basic_func_timing",
		"Levels": "VMeasureLoop::SampleDcTC",
	},
}
```

- Configuration File Details:

| **JSON Object**                        | **Mandatory Properties**             | **Optional Properties**       | **Description**                                                | **Notes**                                                                   |
| -------------------------------------- | ------------------------------------ | ----------------------------- | -------------------------------------------------------------- | --------------------------------------------------------------------------- |
| **TestSetups**                         | SetupLabel,PatternList,Timing,Levels | PrePlist                      | Array of functional configurations.                            | At least one must be defined.                                               |
|   &nbsp;&nbsp;&nbsp;SetupLabel         |                                      |                               | Unique identifier for functional test setup.                   | Will be attached to test instance name in ITUFF.                            |
|   &nbsp;&nbsp;&nbsp;PatternList        |                                      |                               | Functional pattern list to execute.                            | Does not affect passing/failing status.                                     |
|   &nbsp;&nbsp;&nbsp;Timing             |                                      |                               | Name of timing test condition to apply.                        |                                                                             |
|   &nbsp;&nbsp;&nbsp;Levels             |                                      |                               | Name of levels test condition to apply.                        |                                                                             |
|   &nbsp;&nbsp;&nbsp;PrePlist           |                                      |                               | Name of callback function to execute prior to functional test. | Sample format - "SomeCallback(someParam)".                                  |
| **VoltageMeasurements**                | Pins,PrePause,LowLimit,HighLimit     | RelayToken,SharedStorageToken | Array of voltage measurement setups.                           | At least one must be defined.                                               |
|   &nbsp;&nbsp;&nbsp;Pins               |                                      |                               | Comma separated list of pins where DC measurements are taken.  |                                                                             |
|   &nbsp;&nbsp;&nbsp;PrePause           |                                      |                               | PreMeasurementDelay value (S) to set prior to measurement.     | In seconds.                                                                 |
|   &nbsp;&nbsp;&nbsp;LowLimit           |                                      |                               | Low limit (V) for all voltage measurements in this grouping.   | In volts.                                                                   |
|   &nbsp;&nbsp;&nbsp;HighLimit          |                                      |                               | High limit (V) for all voltage measurements in this grouping.  | In volts.                                                                   |
|   &nbsp;&nbsp;&nbsp;RelayToken         |                                      |                               | Name of relay test condition to apply before measurement.      |                                                                             |
|   &nbsp;&nbsp;&nbsp;SharedStorageToken |                                      |                               | Token to add to pin name for Shared Storage key.               | Sample Shared Storage key format - "SharedStorageToken_PinName_SetupLabel". |
| **PostTestSetup**                      | PatternList,Timing,Levels            |                               | Functional test setup to execute last.                         |                                                                             |
|   &nbsp;&nbsp;&nbsp;PatternList        |                                      |                               | Functional pattern list to execute.                            | Does not affect passing/failing status.                                     |
|   &nbsp;&nbsp;&nbsp;Timing             |                                      |                               | Name of timing test condition to apply.                        |                                                                             |
|   &nbsp;&nbsp;&nbsp;Levels             |                                      |                               | Name of levels test condition to apply.                        |                                                                             |

## Datalog output
```python
Sample ituff for VMeasureLoop DC only:
2_tname_VMeasureLoop::VMeasureLoop_testSetup1
2_category_pc
2_composite_00109_0.2000000
2_tname_VMeasureLoop::VMeasureLoop_testSetup2
2_category_pc
2_composite_00109_0.4000000
2_composite_00000_0.3000000

Sample ituff for VMeasureLoop with CTV pins:
2_tname_VMeasureLoop::VMeasureLoopWithCtvToItuff
2_strgval_xxDPIN_A010=101010101|xxDPIN_A026=010101010
```

## Custom User Code Hooks
Here is the list of functions available to the user code to override:

### void ProcessCtvData(Dictionary<string, string> ctvData)
This function is stubbed in Prime and can be overridden by User Code. The idea here is that the user can add specific code processing for CTV results(using the ITME technique).

The input to this function is a Dictionary where the Key is the Pin name and the Value is the CTV captured for that pin.
Custom post process functions can override the exit port by changing the "this.ExitPort" variable value.

## TPL Samples
Here are sample test instance examples using the VMeasureLoop test method.
```python
Import PrimeVMeasureLoopTestMethod.xml;

Test PrimeVMeasureLoopTestMethod VMeasureLoop
{
    ConfigFile = "~HDMT_TPL_DIR/Modules/VMEASURELOOP/VMEASURELOOP/InputFiles/VMeasureLoop.json";
}

Test VMeasureLoopWithCtvToItuff VMeasureLoopWithCtvToItuff
{
    CapturePins = "xxDPIN_A010, xxDPIN_A026";
    ConfigFile = "~HDMT_TPL_DIR/Modules/VMEASURELOOP/VMEASURELOOP/InputFiles/VMeasureLoop_VMeasureLoopWithCtv.json";
}
```

## Exit Ports
The VMeasureLoop test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**                               |
| ------------- | --------------- | --------------------------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition                           |
| **-1**        | ***Error***     | Any software condition error                  |
| **0**         | ***Fail***      | Failing condition - results are out of limits |
| **1**         | ***Pass***      | Passing condition - results are within limits |

## Additional Dependencies

## Version tracking

| **Date**        | **Version** | **Author**   | **Comments**    |
| --------------- | ----------- | ------------ | --------------- |
| Sept 30th, 2021 | 1.0.0       | Kevin Krake  | Initial Release |

## Acronyms
Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
