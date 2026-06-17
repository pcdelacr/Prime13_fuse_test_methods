**prime Test-Method Specification REP**

Revision 1.2.0

March 2022

[[_TOC_]]

## REP for ScanSPOFI

This **REP** is intended to describe the ScanSPOFI Prime TestMethod.

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

The ScanSPOFI test method provides capability to execute a scan plist,capture the failures, and process them to generate a SPOFI data that will be printed to ScanFI datalog stream.

The test method will take into account the channel-linking capability if used in Pattern list, when calculating the vector count from previous label for each failure.

Important Note:

**The format of ScanFI data is decided by Scan team and by the tools
that consumes ScanFI datalog stream. Thus, in many cases user might
notice legacy fields that have a constant values being printed to ScanFI
datalog.**

ScanFI datalog format:

The following information will be printed when at least one failure captured. If no failure captured, nothing will be printed to ScanFI datalog stream.

\[1\] Instance Information will be printed before the failures
information.

**{PlatformId}**: constant value of “HDMTG2”.
**{MinFailLimit}**: constant value of “0”.
**{CatFailLimit}**: constant value of “0”.
**{plist}**: value of “Patlist” parameter.
**{instance}**: name of current instance.
**{captureCount}**: value of “TotalFailCaptureCount” parameter (when
value not 0). Otherwise, value of “PerPatternFailCaptureCount”
parameter.

```
2_ttype_{PlatformId}"
2_plist_{plist}"
2_tname_{instance}"
2_fail_limits:min_{MinFailLimit}_max_{captureCount}_cat_{CatFailLimit}"
```

\[2\] Failures information.

**{domain:pattern}**: domain and pattern name of the printed failures that will follow.

**{CompressionMode}**: Value of “uncompressed” or “compressed”.

```
2_pttrn_{domain:pattern}
2_pattern_mode:{CompressionMode}
```

#### Uncompressed mode
Failure information format in uncompressed mode:

| Previous label.                 | 0 | Pin               | Scan chain length.              | Vectors from previous label.                                                                   | Pattern data                                                                        | \-1 | Cycle                              |
| ------------------------------- | - | ----------------- | ------------------------------- | ---------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------- | --- | ---------------------------------- |
| See below for more information. |   | Failing pin name. | See below for more information. | Vector count between current failure and previous label. (taking into account channel-linking) | Current pattern data. See [Read pattern data](#Read-pattern-data) for more details. |     | Cycles of current printed failure. |

**Label and Scan chain extraction:**

  - “Previous label” and “Scan chain length” are extracted from the
    previous label of the captured failure.
    
      - Example: **LabelX\_77\_99** =\> Previous label will be “LabelX”.
        Scan chain length will be “77”.

  - If previous label doesn’t match regular expression
    (string)\_(integer)\_(integer) then the “Previous label” will be the
    full label. And “Scan chain length” will be -1.
    
      - Example: **LabelY\_77\_99\_ABC** =\> Previous label will be
        “LabelY\_77\_99\_ABC”. Scan chain length will be “-1”.

#### Compreesed mode
Failure information format in compressed mode:

| Previous label.                 |  0  | Pin | -1 | Vectors from previous label suffix. | Pattern data | \-1 | Cycle | Debug comment |
| ------------------------------- | --- | --- | --- | --- | --- | --- | --- | --- |
| See below for more information. |     | Failing pin name. | | Vector count between current failure and previous label suffix value. | Current pattern data. See [Read pattern data](#Read-pattern-data) for more details. |  | Cycles of current printed failure. | # cc=[Label suffix] rma=[Cycles from previous label] |

**Label, Label suffix extraction:**

  - “Previous label” and “Label suffix” are extracted from the
    previous label of the captured failure.
    
      - Example: **LabelX\_cc10** =\> Previous label will be “LabelX”.
        Label suffix will be “10”.
 - If previous label doesn't match regular expression
   (string)_cc(integer) then the “Previous label” will be “FIRST_LABEL”
   and  “Label suffix” will be “1”

#### Read pattern data
The **Pattern data** field will indicates a single character between __X__ (no matter value), __L__ (low value) and __H__ (high value). Where __L__ and __H__ comes from reading the current pattern data and invert it, while __X__ indicates that pattern data was not read. In order to enable turn on pattern  lookup need to set  user **_UserVars.EnableScanPatLookUp** equal to **TRUE**. By default  this option is **FALSE**.

Please consider that the user user var value is read in Verify time, so if the value is changed during execute, the new value won't be considered.

#### RMA limitations
The RMA value is used in [Compreesed mode](#Compreesed-mode) and is intended to represent the number of **cycles** from the previous label to the current scan failure, but this is not always truth and this relies on the pattern. RMA calculation does not considers cycles that come from repeat instructions or other meta data vector, only considers cycles from data vector and scan vector types.

For a better comprehension on how RMA is calculated, lets understand the following representation of a pattern, where V represents a vector of data type and S a channel linked vector. S0 to S3 are the vectors in the linked vector, for this example the link mode is 4.
```
 ...
 ...
 ...
label1_cc_1000: V {}            # 1 cycle
                S {S0 S1 S2 S3} # 4 cycle
                S {S0 S1 S2 S3} # 4 cycle
                S {S0 S1 S2 S3} # S3 failed
                S {S0 S1 S2 S3}
...
...
...
```

Let's say that S3 of the third S vector failed. So, the number of cycles from the previous label and the failure in S3, is calculated as follows:
```
1 (cycle from V) + 4 (cycles from first S) + 4 (cycles from second S) + 3 (link count of S3 in the third S vector) = 8
```

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ScanSPOFI test method

| **Parameter Name**         | **Required?** | **Type**        | **Values**                                                                   | **Comments**                              |
| -------------------------- | ------------- | --------------- | ---------------------------------------------------------------------------- | ----------------------------------------- |
| Patlist                    | Yes           | Plist           | Plist name to be executed                                                    |                                           |
| TimingTc                   | Yes           | TimingCondition | Timing test condition required for plist execution                           |                                           |
| LevelsTc                   | Yes           | LevelsCondition | Levels test condition required for plist execution                           |                                           |
| PrePlist                   | No            | String          | PrePlist callback to plist execution                                         |                                           |
| MaskPins                   | No            | String          | Comma separated list of pins for which the fail data capture will be skipped | Default is Empty String                   |
| TotalFailCaptureCount      | Yes           | UnsignedInteger | Number of total failures to capture per plist                                | Limited to tester capture memory capacity |
| PerPatternFailCaptureCount | Yes           | UnsignedInteger | Number of failures to capture per pattern                                    | Limited to tester capture memory capacity |
| PatternsRegexesForKill | No            | CommaSeparatedString | List of regexes, when a pattern that failed matches one of these regexes, will exit from dedicated port 2. | Default = Empty String |
| EngineeringSpofiOutputFilePath | No            | File | When populated, this should point to local engineering file path to ber created. All Spofi prints will be re-routed to it. If empty, prints will go to Scan datalog as normal | Default = Empty String |

**Notes:**

  - User not allowed to provide 0 values for both limits
    (TotalFailCaptureCount and PerPatternFailCaptureCount). Exception
     will be thrown.

## Datalog output

Example of ScanFI datalog print generated by ScanSPOFI test method:

```
2_ttype_HDMTG2
2_plist_MyPlist
2_tname_SCN::ScanSPOFI_PerPlist
2_fail_limits:min_0_max_2_cat_0
2_pttrn_domain_x1:pattern_x1
2_pattern_mode:uncompressed
labelx1 0 pin_x1 55 5 X -1 7
labelx1_55_66_XX 0 pin_x2 -1 15 X -1 17
2_pttrn_domain_y1:pattern_y1
2_pattern_mode:uncompressed
label_y1 0 pin_y1 66 6 X -1 8
label_y1 0 pin_y2 66 6 X -1 8
2_pttrn_default:scan_compress_01
2_pattern_mode:compressed
FIRST_LABEL 0 pin_y1-1 2 H -1 3 # cc=[1] rma=[0]
FIRST_LABEL 0 pin_y2 -1 11 L -1 12 # cc=[1] rma=[2]
Label1 0 pin_y1 -1 0 L -1 15 # cc=[15] rma=[1]
```

**IMPORTANT**
The above datalog printout is executed by default from CustomPostProcessResults hook (see below )
In case user will override CustomPostProcessResults hook function there are few options for printing the results to datalog:
1. User will implement their own logic for results printout to Ituff as part of CustomPostProcessResults function implementation
2. To print the default datalog output in the above format (composite) user will call the DatalogService API WriteToScanFi in CustomPostProcessResults implementation
```python
        void IScanSPOFIExtensions.CustomPostProcessResults(ScanSpofiTestInstanceResults results)
        {
            if (!results.ScanResults.Count.Equals(0) && !results.FunctionalResult)
            {
                var spofiFormat = this.GetSPOFIFormat(results.ScanResults);
                Prime.Services.DatalogService.WriteToScanFi(spofiFormat);
            }

            this.patternToKillMatcher.ProcessPatterns(results.ScanResults.Select(x => x.GetPatternName()).ToList());
            if (!this.patternToKillMatcher.IsMatchToKill(true))
            {
                results.ExitPort = results.FunctionalResult ? TestMethodPassPort : TestMethodFailPort;
            }
            else
            {
                results.ExitPort = TestMethodsFailPortMatchingPatternsToKill;
            }
        }
```
3. The `DSS_LIMIT_CATASTROPHIC` string will be printed at the end of the SPOFI print out when at least one limit (TotalFailCaptureCount or PerPatternFailCaptureCount) has been reached. The pattern name will be printed to the console log in the case the per pattern limit is reached.
4. If a regex to kill (from PatternsRegexesForKill parameter) is matched, the SPOFI datalog won't be printed, because the pattern to kill is resolved right after the current instance receives the failure data. This is done intentionally in order to avoid the failure of data processing.

## Custom User Code Hooks

```python
    public interface IScanSPOFIExtensions
    {
        /// <summary>
        /// Called prior to any test in Execute to generate the test instance results object.
        /// </summary>
        /// <returns>The test instance results object.</returns>
        ScanSpofiTestInstanceResults CreateTestInstanceResults();

        /// <summary>
        /// Called right after successful execution to provide the user the ability to post process exit port, scan fail results, and functional result.
        /// </summary>
        /// <param name="results">The object for holding the exit port, scan fail results, and functional result.</param>
        void CustomPostProcessResults(ScanSpofiTestInstanceResults results);

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

Here are an instance example using the ScanSPOFI test method**  

```python
Import PrimeScanSPOFITestMethod.xml;

Test PrimeScanSPOFITestMethod PrimeScanSPOFI_PerPlist
{
   Patlist = "array_pbist_llc_dat_lya_autoclkmux0_s0_list";
   TimingsTc = "BASE::cpu_func_sdr_univ_sta_univ_univ_b100_t100_d100";
   LevelsTc = "__main__::leakage_tc";
   TotalFailCaptureCount = 1;
   PerPatternFailCaptureCount = 0;
   PatternsRegexesForKill = "start_pattern0.*,start_pattern1.*";
}
```

## Exit Ports

The ScanSPOFI test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                           |
| ------------- | ------------- | ------------------------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition.                                                      |
| **-1**        | ***Error***   | Any software condition error.                                             |
| **0**         | ***Fail***    | Failing condition. Failures captured.                                     |
| **1**         | ***Pass***    | Passing condition. Plist execution passed without capturing any failures. |
| **2**         | ***Fail***    | Failing condition. Plist execution failed matching kill patterns regexes. |

## Additional Dependencies

More dependencies to consider for this TestMethod to well operate:

  - N/A.

## Version tracking

| **Date**                  | **Version** | **Author**      | **Comments**                                                              |
| ------------------------- | ----------- | --------------- | ------------------------------------------------------------------------- |
| Apr 1<sup>st</sup>, 2020  | 1.0.0       | Wajde Dakwar    | Initial Version.                                                          |
| May 4<sup>th</sup>, 2020  | 1.0.1       | Wajde Dakwar    | Added ability to control both fail capture limits: total and per pattern. |
| Jun 9<sup>th</sup>, 2021  | 1.0.2       | Kevin D Krake   | Adding pin mask ability.                                                  |
| Nov 2<sup>nd</sup>, 2021  | 1.0.3       | Brian J. Morera | Adding current pattern data ability and support of compressed mode.       |
| Mar 29<sup>th</sup>, 2022 | 1.1.0       | Kevin D Krake   | Adding new Results method for exit port handling.                         |
| Jun 15<sup>th</sup>, 2022 | 1.2.0       | Brian J. Morera | Adding patterns to kill capability.                                       |
| Feb 15<sup>th</sup>, 2023 | 1.3.0       | Wajde Dakwar | Adding writing Spofi to engineering file capability.                         |
| Oct 18<sup>th</sup>, 2023  | 12.3 | Didier Armando Jimenez Retana | [Prime global processing results timeout to avoid hang](https://dev.azure.com/mit-us/PRIME/_workitems/edit/38198) |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language