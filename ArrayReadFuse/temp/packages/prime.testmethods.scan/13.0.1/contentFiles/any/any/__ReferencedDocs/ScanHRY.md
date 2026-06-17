**prime Test-Method Specification REP**

Revision 1.1.0

Mar 2022

[[_TOC_]]

## REP for ScanHRY

This **REP** is intended to describe the ScanHRY Prime TestMethod.

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

The ScanHRY test method provides capability to execute a scan plist, capture the failures, and process them to generate an HRY data that will be printed to Ituff datalog.

The test method will take into account the channel-linking capability if used in Pattern list, when calculating the failure distance from last label.

This test method implements the common HRY generation algorithm which will be described below, but user can choose to replace/override the algorithm by implement two methods: ReadInputFile() and GenerateHRY(…).

![image.png](./.attachments/image-a300e8d1-4d28-4fe7-9f71-c70d272a4bd6.png)

### Common HRY Algorithm

ScanHRY test method implement a common HRY algorithm which allows user to provide a mapping of failures information to HRY index.

Code will iterate over captured failures, and for each captured failure, code tries to find the matching object within the .Json file, to know how this failure should be processed.

Failure information such: failing pattern name, failing pin name and cycles from previous label will be used to find a match. Please see example below to have a better picture.

**Note:** The pins regex matching occurs by order of definition. If a pin failed, code will try to match the pin regexes defined under that pattern in the order defined in the .json input file. For example, user can define a section with ".*" regex at the end of pins definition section to match all other pins that user didn't specify explicitly. (See example below).

#### .Json File Description

For a file example, skip to next section.

<table>
<thead>
<tr class="header">
<th>HRYLength</th>
<th>The length of HRY string to be printed to ituff.</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td>GroupCount</td>
<td>The number of Group objects that should be declared under each “Groups” list of objects.</td>
</tr>
<tr class="even">
<td>Patterns</td>
<td>List of Pattern objects.</td>
</tr>
<tr class="odd">
<td>Pattern object [<a href="#example">line 5</a>]</td>
<td>Defines a Pattern object that describes how failures that falls under this object should be processed.</td>
</tr>
<tr class="even">
<td>PatternRegex</td>
<td>Pattern regular expression that tells if a failure should fall under current pattern object or not.</td>
</tr>
<tr class="odd">
<td>Groups</td>
<td>List of Group objects.</td>
</tr>
<tr class="even">
<td>Group object [<a href="#example">line 9</a>]</td>
<td>Defines a Group object that describes how failures that falls under this object should be processed.</td>
</tr>
<tr class="odd">
<td>GroupNumber</td>
<td><p>Non-negative number between 0 and GroupCount, that tells if a failure should fail under this group or not.</p>
<p>Given a failure, the group of that failure will be calculated as follow: {CyclesFromPreviousLabel}%{GroupCount}</p></td>
</tr>
<tr class="even">
<td>Pins</td>
<td>List of Pin objects.</td>
</tr>
<tr class="odd">
<td>Pin object [<a href="#example">line 13</a>]</td>
<td>Defines a Pin object that describes how failures that falls under this object should be processed.</td>
</tr>
<tr class="even">
<td>PinRegex</td>
<td>Pin regular expression that tells if a failure should fall under current Pin object or not.</td>
</tr>
<tr class="odd">
<td>Shifts</td>
<td>List of Shift objects.</td>
</tr>
<tr class="even">
<td>Shift object [<a href="#example">line 17</a>]</td>
<td>Defines a Shift object that describes how failures that falls under this object should be processed.</td>
</tr>
<tr class="odd">
<td>ShiftStart</td>
<td>Tells the lowest {CyclesFromPreviousLabel} that can match this Shift object.</td>
</tr>
<tr class="even">
<td>ShiftEnd</td>
<td>Tells the highest {CyclesFromPreviousLabel} that can match this Shift object.</td>
</tr>
<tr class="odd">
<td>HRYIndex</td>
<td>Tell what index in the HRY string that should be changed to ‘0’ (failing HRY symbol).</td>
</tr>
<tr class="even">
<td>HRYPrint</td>
<td>Data to print when a failure falls under this Shift object. This is usually a string that identifies the failing partition full path within the unit.</td>
</tr>
</tbody>
</table>

#### Example

Assuming we executed ScanHRY instance with the following parameters:

| Patlist                | ValidPlist     |
| ---------------------- | -------------- |
| LevelsTc               | ValidLevels    |
| TimingTc               | ValidTiming    |
| PerPatFailCaptureCount | 2              |
| HRYInputFile           | Example below. |

And we captured only one failure that has the following fail
information:

| Failing Pattern            | Scan\_main\_pmap\_077 |
| -------------------------- | --------------------- |
| Failing Pin                | P002                  |
| Cycles from previous label | 123                   |

Then this failure will match the shift object below because pattern, pin and cycles from previous label is within the range 0-222.

The output HRY string will be 0111. The 0 is because the matching shift, contains value of 0 in “HRYIndex” field. Moreover, an addition print will be performed to ituff that contains this value “GLM\_M0\_C0\_IEC” as this is the value of “HRYPrint” field in the matching shift. For the
full print see [Datalog output](#datalog-output) section below.

.Json file:

```json
{ "HRYLength": 4,
  "GroupCount": 1,
  "Patterns":
  [
    {
	"PatternRegex": "scan_main_pmap_0.*",
	"Groups":
	[
		{
			"GroupNumber":0,
			"Pins":
			[
			{
				"PinRegex": "P002",
				"Shifts":
				[
					{
						"ShiftStart": 0,
						"ShiftEnd": 222,
						"HRYIndex": 0,
						"HRYPrint": "GLM_M0_C0_IEC"
					},
                ]
			},
			{
				"PinRegex": ".*",
				"Shifts":
				[
					{
						"ShiftStart": 0,
						"ShiftEnd": 9999,
						"HRYIndex": 1,
						"HRYPrint": "OTHER_FAILS"
					},
                ]
			}
			]
		},
	]
    },
  ]
}
```

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ScanHRY test method

| **Parameter Name**         | **Required?** | **Type**             | **Values**                                                                                                   | **Comments**           |
| -------------------------- | ------------- | -------------------- | ------------------------------------------------------------------------------------------------------------ | ---------------------- |
| Patlist                    | Yes           | Plist                | Plist name to be executed.                                                                                   |                        |
| LevelsTc                   | Yes           | LevelsCondition      | Levels test condition required for plist execution.                                                          |                        |
| TimingTc                   | Yes           | TimingCondition      | Timing test condition required for plist execution.                                                          |                        |
| PrePlist                   | No            | String               | PrePlist callback to plist execution.                                                                        |                        |
| MaskPins                   | No            | String               | Comma separated list of pins for which the fail data capture will be skipped.                                | Default = Empty String |
| TotalFailCaptureCount      | Yes           | UnsignedInteger      | Number of total failures to capture.                                                                         |                        |
| PerPatFailCaptureCount     | No            | UnsignedInteger      | Number of per pattern failures to capture.                                                                   | Default = 1            |
| PatternsRegexesForKill     | No            | CommaSeparatedString | List of regexes, when a pattern that failed matches one of this regexes, will exit from dedicated port 4.    | Default = Empty String |
| ExpectedPartitionsStatuses | No            | CommaSeparatedString | List of expected partition statuses. List of values with format "PartitionName=0/1/8/9/X" (X - don't care.). | Default = Empty String |
| HRYInputFile               | Yes           | File                 | .Json input file for HRY.                                                                                    |                        |
| FirstFailDataItuffPrint    | No            | String               | When ENABLED it will print first fail data per pattern to Ituff.                                | Default = ENABLED     |

**Notes:**

  - No special notes.

## Datalog output

The above [example](#example) will print to Ituff datalog the following data:

```
2_tname_PrimeScanHRYHRY
2_pttrn_DomainA_All_DPIN_Dig:scan_hry_pattern_all_passing_2:passing_scan_hry
2_fdpmv_10
2_fcpmv_-1
2_fsdmv_-1
2_vcont_10
2_faildata_{8027}

2_tname_PrimeScanHRYHRY_RAWSTR
2_strgval_0111
2_tname_HRY_PrimeScanHRY_GLM_M0_C0_IEC
2_strgval_0
```
The Ituff output will contain two sections. The first section is the print out of the first failure data per pattern in the plist, and the second section is related to the HRY RAW String and the scan sections that failed.

Please note that the first section (first fail data) is printed only when the parameter "FirstFailDataItuffPrint" = ENABLED.

**IMPORTANT**
The above datalog printout is executed by default from CustomPostProcessResults hook (see below )
In case user will override CustomPostProcessResults hook function there are few options for printing the results to datalog:
1. User will implement their own logic for results printout to Ituff as part of CustomPostProcessResults function implementation
2. To print the default datalog output in the above format (composite) user will call the DatalogService API WriteToItuff in CustomPostProcessResults implementation
```python
        void IScanHRYExtensions.CustomPostProcessResults(ScanHryTestInstanceResults results)
        {
            Prime.Services.DatalogService.WriteToItuff(this.GenerateHRY(results.ScanResults));

            results.ExitPort = results.FunctionalResult ? TestMethodPassPort : this.PatternListFailPort();
        }
```

## Custom User Code Hooks

```python
    public interface IScanHRYExtensions
    {
        /// <summary>
        /// Called prior to any test in Execute to generate the test instance results object.
        /// </summary>
        /// <returns>The test instance results object.</returns>
        ScanHryTestInstanceResults CreateTestInstanceResults();

        /// <summary>
        /// Called right after successful execution to provide the user the ability to post process exit port, scan fail results, and functional result.
        /// </summary>
        /// <param name="results">The object for holding the exit port, scan fail results, and functional result.</param>
        void CustomPostProcessResults(ScanHryTestInstanceResults results);

        /// <summary>
        /// Gets the list of pins to mask execution after execution. The test method will merge this list with the ones from the test instance parameter.
        /// </summary>
        /// <returns>The list of mask pins.</returns>
        List<string> GetDynamicPinMask();
    }
```

Here is the list of functions available to the user code to override.

- CreateTestInstanceResults - Creates test instance results object that holds all results and the exit port (to be done early in Execute).

- CustomPostProcessResults - Allows user to post-process all results and override the exit port determined by the test method.  
Input is the test instance results object which includes functional results, raw scan fail data, and the exit port.

- GetDynamicPinMask - Allows user to populate pinmask pins after TP load.

## Global TimeOut
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

### Global timeOut console output

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

## TPL Samples

Here are a few test instance examples using the ScanHRY test method**  

```python
Import PrimeScanHRYTestMethod.xml;

Test PrimeScanHRYTestMethod PrimeScanHRY
{
   Patlist = "array_pbist_llc_dat_lya_autoclkmux0_s0_list";
   TimingsTc = "BASE::cpu_func_sdr_univ_sta_univ_univ_b100_t100_d100";
   LevelsTc = "__main__::leakage_tc";
   PerPatFailCaptureCount = 1;
   HRYInputFile = "~HDMT_TPL_DIR/Modules/Scan/InputFiles/HRYInputFileExample.json";
}
```

## Exit Ports

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

## Additional Dependencies

More dependencies to consider for this TestMethod to well operate:

## Version tracking

| **Date**                  | **Version** | **Author**    | **Comments**                                      |
| ------------------------- | ----------- | ------------- | ------------------------------------------------- |
| Mar 29<sup>th</sup>, 2020 | 1.0.0       | Wajde Dakwar  | Initial Version.                                  |
| Jun 9<sup>th</sup>, 2021  | 1.0.1       | Kevin D Krake | Adding pin mask ability.                          |
| Mar 29<sup>th</sup>, 2022 | 1.1.0       | Kevin D Krake | Adding new Results method for exit port handling. |
| Jan 18<sup>th</sup>, 2023 | 1.1.1       | Wajde Dakwar | Adding clarification about pin regex matching order. |
| Oct 18<sup>th</sup>, 2023 | 12.3 | Didier Armando Jimenez Retana | [Prime global processing results timeout to avoid hang](https://dev.azure.com/mit-us/PRIME/_workitems/edit/38198) |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language