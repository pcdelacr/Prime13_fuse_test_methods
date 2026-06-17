<h1>Prime ScanHRYStf Test-Method Specification REP</h1>

Revision 1.1.0

Mar 2022

[[_TOC_]]

## Methodology

The ScanHRYStf test method provides capability to execute a scan plist, capture the failures, and process them to generate an HRY data that will be printed to Ituff datalog.

The test method will take into account the channel-linking capability if used in Pattern list, when calculating the failure distance from last label.

This test method implements the common HRY-STF generation algorithm which is described in the section below.

## HRY-STF Mapping Algorithm

The test method purpose is to generate HRY string, which describes the status of each tested partition defined by user. To do that, test method relies on two main files:
- Mapping Descriptor File (.json)
- Binary Mapping File (.bin).

These two files which will be described in the section below, should be generated through SPF tool.

**Note:** for the short-term, those files are generated through a convertor script developed by Prime developers, until feature is fully supported by SPF tool.

### HRY Symbols

|Symbol| Meaning |
|--|--|
| 0 | Fail. It means that there was a defect that mapped into this HRY index.|
| 1 | Pass. It means that no defects found maps to this HRY index & capture limits has not been reached. |
| 8 | Unknown. It means that no defect found that maps to this HRY index, but total capture limits has been reached. |
| 9 | Unassigned. This means that this index is not part of the HRY definition that is provided by user in the descriptor .json input file. |

### Mapping Descriptor File (.json)

This file defines the patterns, labels, and cycle ranges in which the captured data from the Plist execution should fail in. Any captured defect which doesn't fit at any pattern, label and cycle range combination, will make the test method exit from a fail port (0).

Moreover, this file will hold the mapping from partition (PID) to index in the HRY string. Also, for each pattern, the first section will describe which partitions (PIDs) this specific pattern should test.

See examples below for more information.

### Descriptor File (.json) Patterns Validation Flag

By default, this template will generate an error when there are patterns in the plist that are not represented in .json file. (Example: .json patterns are fewer than plist patterns)

On the other hand, if .json file contains patterns that do not exist in the plist (Example : .json patterns are more than plist patterns), Stf will not cross check with plist by default.
To enable this validation, user need to set _UserVars.EnableScanStfPatternsCheck to TRUE. If the condition is met, the template will generate an error during Verify.

### Binary Mapping File (.bin)

This is a binary file, that holds the actual mapping of captured defect location to partition. The defect location is calculated based on the pattern order in the descriptor file + label order + offset from the first address of the matching cycles range.

See examples below for more information.

### Example of HRY Generation

#### Descriptor file example:

![DescriptorExampleForWiki_ScanHRYStf.PNG](../.attachments/ScanHRYStf/DescriptorExampleForWiki_ScanHRYStf-057baad1-93a6-483a-9f56-07825c72052e.PNG)

#### Binary file example:

![FixedBinaryExampleForWiki_ScanHRYStf.PNG](../.attachments/ScanHRYStf/FixedBinaryExampleForWiki_ScanHRYStf-fa9cbaf8-8e8c-4e7b-9781-af34cda985bc.PNG)

#### Pattern example:

![pattern_example.PNG](../.attachments/ScanHRYStf/pattern_example-a13d6d86-9c1f-4c1f-afb3-884f32cedea3.PNG)

#### Defect Parsing 

Assuming executing the plist produced 2 defects:
- Defect #1: pattern: scan_hry_stf_pattern_1 | label: Label_1 | Vector: 4
- Defect #2: pattern: scan_hry_stf_pattern_1 | label: Label_2 | Vector: 10

This is how the mapping will be done:
- Defect #1 falls under this range:

![ScanHRYStf_Wiki_Defect1MatchRange.PNG](../.attachments/ScanHRYStf/ScanHRYStf_Wiki_Defect1MatchRange-655a67fb-2ec6-4367-9318-195b47ace460.PNG)

The distance from the beginning of the range is 0, because Vector==4, and beginning of the range is also 4. Since this is the first pattern/label combination so there is no offset to add to the seek address, it means that we need to read the .bin file (Binary input) at position 0, and there we find the index that we need to update in the HRY. If we look in the binary file we have 06 there -> we need to update HRY index 6 with fail symbol - which is 0.

- Defect #2 falls under this range:

![ScanHRYStf_Wiki_Defect2MatchRange.PNG](../.attachments/ScanHRYStf/ScanHRYStf_Wiki_Defect2MatchRange-6347f78a-cd36-446b-8406-78b41b1e71e7.PNG)

The distance from the beginning of the range is 3, because Vector==10, and beginning of the range is 8. Since we have 12 possible mappings before this range, we need to add the offset which is 12. It means that we need to read the .bin file (Binary input) at position 12+3 -> 15, and there we find the index that we need to update in the HRY. If we look in the binary file we have 05 there -> we need to update HRY index 5 with fail symbol - which is 0.

#### Final HRY ituff print for this example:

2_tname_<InstanceName>_HRY_RAWSTR
2_strgval_91111001991

**Note:** the '9' means that there's a hole on that specific index, based on the definition of the user in the input file. Please see HRY symbols section for description of the symbols.

### Compressed Mode
When parameter 'CompressedMode' is 'On'. Test method will support compressed content.
Compressed content from test method perspective, is a pattern that can contain labels that end with "_cc<numeric>".
When previous label of any failure, is matching the above format, the calculation of 'distance from label' will be different. In this case, the distance from label (the range in which the test method do the lookup when mapping partition) will be [actual cycle] - [label_cycle]. IMPRTANT NOTE: the label that is need to be defined in descriptor file is without the "_cc<numberic>" suffix. (this is a requirement by scan users).

#### Example:
If previous label of a failure is: "abc_cc445".
And failure cycle is 885.

Then the distance from label is [885] - [445] which is 440.
The label that will be used for lookup in descriptor file is "abc". (the suffix is omitted).

In this case, test method expects to see in descriptor file an entry for label="abc" that has "440" cycle in range. Then the actual mapping will continue as usual (see above.).

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ScanHRYStf test method

| **Parameter Name**            | **Required?** | **Type**             | **Values**                                                                                                                                                                  | **Comments**                              |
| ----------------------------- | ------------- | -------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------- |
| Patlist                       | Yes           | Plist                | Plist name to be executed                                                                                                                                                   |                                           |
| LevelsTc                      | Yes           | LevelsCondition      | Levels test condition required for plist execution                                                                                                                          |                                           |
| TimingTc                      | Yes           | TimingCondition      | Timing test condition required for plist execution                                                                                                                          |                                           |
| TotalFailCaptureCount         | Yes           | UnsignedInteger      | Number of total failures to capture per plist.                                                                                                                              | Limited to tester capture memory capacity |
| PerPatternFailCaptureCount    | Yes           | UnsignedInteger      | Number of failures to capture per pattern.                                                                                                                                  | Limited to tester capture memory capacity |
| PrePlist                      | No            | String               | PrePlist callback to plist execution.                                                                                                                                       | Default is Empty String                   |
| MaskPins                      | No            | String               | Comma separated list of pins for which the fail data capture will be skipped                                                                                                | Default is Empty String                   |
| PatternsNotToProcessForHRY    | No            | CommaSeparatedString | Patterns regexes to exclude when processing failures for HRY.                                                                                                               |                                           |
| PatternsRegexesForKill        | No            | CommaSeparatedString | List of regexes, when a pattern that failed matches one of this regexes, will exit from dedicated port 3.                                                                   | Default = Empty String                    |
| HryMappingDescriptorInputFile | Yes           | File                 | HRY mapping descriptor file. This file contents should define the way we map a specific failure to an actual HRY index.                                                     |                                           |
| HryMappingInputFile           | Yes           | File                 | HRY mapping file. This should be a binary file, that contains mapping information to be accessed in run-time, to determine for a specific failure, the HRY partition index. |                                           |
| SanityCheckInputFile          | No            | File                 | When this parameter is populated the test method will run in special mode. This special mode will read the input file,                                                      |                                           |
|                               |               |                      | which holds expected mapping for each possible failure, and checks if test method methodology can produce the same expected mapping.                                        |                                           |
|                               |               |                      | Otherwise, there might be a problem in .json and .bin inputs. If all maps as expected test method pass verify. Otherwise if fails.                                          |                                           |
| CompressedMode                       | No           | String (Enum)                | On/Off (Default is 'Off')                                                                                                                                                   |   When On, test method will support compressed labels when mapping defects. See below for more information.                                              |

**Notes:**

  - N/A.
  
## Datalog output

2_tname_<InstanceName>_HRY_RAWSTR
2_strgval_91111001991

**IMPORTANT**
The above datalog printout is executed by default from CustomPostProcessResults hook (see below )
In case user will override CustomPostProcessResults hook function there are few options for printing the results to datalog:
1. User will implement their own logic for results printout to Ituff as part of CustomPostProcessResults function implementation
2. To print the default datalog output in the above format (composite) user will call the DatalogService API WriteToItuff in CustomPostProcessResults implementation
```python
        void IScanHRYStfExtensions.CustomPostProcessResults(ScanHryStfTestInstanceResults results)
        {
            this.capturedFailingPatterns = this.hryMappingInfo.ProcessFailures(results.ScanResults, this.IsCaptureLimitsReached(), this.patternsToIgnoreRegex, this.patternsToKillRegex);
            this.PrintHRY();

            results.ExitPort = results.FunctionalResult ? TestMethodPassPort : this.PatternListFailPort();
        }
```

## Custom User Code Hooks

```python
    public interface IScanHRYStfExtensions
    {
        /// <summary>
        /// Called prior to any test in Execute to generate the test instance results object.
        /// </summary>
        /// <returns>The test instance results object.</returns>
        ScanHryStfTestInstanceResults CreateTestInstanceResults();

        /// <summary>
        /// Called right after successful execution to provide the user the ability to post process exit port, scan fail results, and functional result.
        /// </summary>
        /// <param name="results">The object for holding the exit port, scan fail results, and functional result.</param>
        void CustomPostProcessResults(ScanHryStfTestInstanceResults results);

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

Here are a few test instance examples using the ScanHRYStf test method from SampleTP

**TPL Sample1:**

```python
Import PrimeScanHRYStfTestMethod.xml;

Test PrimeScanHRYStfTestMethod PrimeScanHRYStf_NoFailuresCaptured_P1
{
   Patlist = "scan_hry_stf";
   TimingsTc = "BASE::cpu_func_sdr_univ_sta_univ_univ_b100_t100_d100";
   LevelsTc = "__main__::leakage_tc";
   TotalFailCaptureCount = 1024000;
   PerPatternFailCaptureCount = 1;
   HryMappingDescriptorInputFile = "~HDMT_TPL_DIR/Modules/Scan/InputFiles/HRYStfDescriptorFileExample.json";
   HryMappingInputFile = "~HDMT_TPL_DIR/Modules/Scan/InputFiles/HRYMapping.bin";
}
```

## Exit Ports

The ScanHRYStf test method supports the following exit ports:

| **Exit Port** | **Condition**  | **Description**                                                           |
| ------------- | -------------- | ------------------------------------------------------------------------- |
| **-2**        | ***Alarm***    | Any alarm condition                                                       |
| **-1**        | ***Error***    | Any software condition error                                              |
| **0**         | ***Fail***     | Failing condition. Defects could not be processed successfully.           |
| **1**         | ***Pass***     | Passing condition. No defects captured.                                   |
| **2**         | ***Fail***     | Failing condition. Failure captured and processed successfully.           |
| **3**         | ***Fail***     | Failing condition. Plist execution failed matching kill patterns regexes. |

  
## Additional Dependencies

More dependencies to consider for this TestMethod to well operate:

## Version tracking

| **Date**       | **Version** | **Author**      | **Comments**                                      |
| -------------- | ----------- | --------------- | ------------------------------------------------- |
| May 9th, 2020  | 1.0.0       | Wajde Dakwar    | Added full documentation with examples.           |
| Jun 9th, 2021  | 1.0.1       | Kevin D Krake   | Adding pin mask ability.                          |
| Jun 26th, 2021 | 1.0.2       | Brian J. Morera | Adding Patterns to kill parameter.                |
| Mar 29th, 2022 | 1.1.0       | Kevin D Krake   | Adding new Results method for exit port handling. |
| Oct 18<sup>th</sup>, 2023  | 12.3 | Didier Armando Jimenez Retana | [Prime global processing results timeout to avoid hang](https://dev.azure.com/mit-us/PRIME/_workitems/edit/38198) |
| Feb 14<sup>th</sup>, 2025 | 13.2 | Tiow, Hian Seng | [Do Not Trigger Error when HryMappingDescriptorInputFile configure pattern not exist in Patlist](https://dev.azure.com/mit-us/PRIME/_workitems/edit/55740/)|
| Jun 25<sup>th</sup>, 2025 | 13.3 ER1 | Tiow, Hian Seng | [[ScanHRYStf] to improve UNTESTED-8 read out](https://dev.azure.com/mit-us/PRIME/_workitems/edit/55665/) |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System