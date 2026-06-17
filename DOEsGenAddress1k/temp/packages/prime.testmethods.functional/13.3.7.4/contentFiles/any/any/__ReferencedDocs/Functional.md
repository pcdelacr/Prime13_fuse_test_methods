<h1>Prime Test-Method Specification REP</h1>
<script type="text/javascript" async
  src="https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.7/MathJax.js?config=TeX-MML-AM_CHTML">
</script>
Revision 1.0.3

Dec 2021

[[_TOC_]]

## Methodology
The Functional test method provides capability to perform plist execution, as well as to optionally capture CTV(Capture This Vector) data and failures.
Starting Prime <b>v13.2.1</b>, `CtvCapturePins` is allow to use pin group. Request ticket: [58253](https://dev.azure.com/mit-us/PRIME/_workitems/edit/58253)

## Test Instance Parameters

The following table outlines the test instance parameters supported by the Functional test method:

| **Parameter Name**                  | **Required?** | **Type**          | **Values**                                                                                                                       | **Default Value**                | **Comments**                                                                                                                                                   |
|-------------------------------------|---------------|-------------------|----------------------------------------------------------------------------------------------------------------------------------|----------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Patlist                             | Yes           | Plist             | Plist name to be executed                                                                                                        |                                  |                                                                                                                                                                |
| LevelsTc                            | Yes           | LevelsCondition   | Levels test condition required for plist execution                                                                               |                                  |                                                                                                                                                                |
| TimingTc                            | Yes           | TimingCondition   | Timing test condition required for plist execution                                                                               |                                  |                                                                                                                                                                |
| MaskPins                            | No            | String            | Comma separated list of pins to be excluded from fail data capture                                                               | Empty String                     |                                                                                                                                                                |
| FailuresToCaptureTotal              | No            | UnsignedInteger   | Total number of failures to capture.                                                                                             | 0                                | To capture failures, value must be >= 1                                                                                                                        |
| FailuresToCapturePerPattern         | No            | UnsignedInteger   | Number of per pattern failures to capture.                                                                                       | 0                                | For per pattern failures to be captured, FailuresToCaptureTotal must be >= 1                                                                                   |
| CtvCapturePins                      | No            | String            | Comma separated list of pins for which CTV data should be captured                                                               | Empty String                     | To capture CTVs, this value must be provided                                                                                                                   |
| CtvCycleCaptureMode                 | No            | enum              | Enum to determine whether CTVs are captured by cycle. ENABLED enables cycle capture, DISABLED disables                           | DISABLED                         | To capture CTVs, CtvCapturePins must be provided                                                                                                               |
| MaxFailuresToItuff                  | No            | UnsignedInteger   | Maximum number of failures to be printed to ituff                                                                                | 0                                | Value must be smaller than _FailuresToCaptureTotal_. If 0, no failures will be printed to Ituff.                                                               |
| MaxFailuresPerPatternToItuff        | No            | UnsignedInteger   | Maximum number of failures per pattern to be printed to ituff. Total number of failures will never exceed MaxFailuresToItuff     | 0                                | Value must be less than _MaxFailuresToItuff_ and _FailuresToCapturePerPattern_. If 0, then number of failures per pattern will be set to _MaxFailuresToItuff_. |
| AlarmPortRedirect                   | No            | String (choice)   | DISABLED (**default**), ENABLED                                                                                                  | Default alarm port=[-2] behavior. | Enable the alarm port redirect to port=[4].                                                                                                                    |
| DtsConfigurationName                | No            | String            | Name of a configuration from the DtsProcessingService.json                                                                       | Empty String                     | if this parameter is not empty, CTVs are going to be captured, processed and logged according to the DTS configuration.                                        |
| DynamicMaskingPluginName            | No            | String            | Plugin name to use for dynamic pin masking.                                                                                      | Empty String                     |                                                                                                                                                                |
| DynamicMaskingPluginUserInput       | No            | String            | The user input to customize the `DynamicMaskingPluginName` plugin.                                                               | Empty String                     |                                                                                                                                                                |
| FunctionalTestPlugin                    | No            | String            | The plugin name to extend the functional test to execute before and after the `IFunctionalTest.Execute`.                         | Empty String                     |                                                                                                                                                                |
| FunctionalTestPluginInput               | No            | String            | The user input to customize the `FunctionalTestPluginInput` plugin.                                                              | Empty String                     |                                                                                                                                                                |
| ProcessFailuresPlugin                    | No            | String            | The plugin name to extend the current instance to customize the processing of the failures resulting from the PatList execution. | Prime.Plugins.Functional.FailuresToDatalog                      | The default value for this plugin is the Prime.Plugins.Functional.FailuresToDatalog which datalogs the information of the failures to the ituff.               |
| ProcessFailuresPluginInput               | No            | String            | The user input to customize the `ProcessFailuresPlugin` plugin.                                                                  | Empty String                     |                                                                                                                                                                |
| ProcessCtvPerPinPlugin                    | No            | String            | The plugin name to extend the current instance to customize the processing of the CTV per pin resulting from the PatList execution.                                  |                       |                |
| ProcessCtvPerPinPluginInput               | No            | String            | The user input to customize the `ProcessCtvPerPinPlugin` plugin.                                                                 | Empty String                     |                                                                                                                                                                |

**Error Conditions:**

  - If FailuresToCapturePerPatten is >= 1 but FailuresToCaptureTotal is zero, an error will be thrown.
  - Likewise if CtvCycleCaptureMode is ENABLED but CtvCapturePins is empty, an error will be thrown.
  - If CtvCycleCaptureMode is ENABLED and a DtsConfigurationName is defined an error will be thrown, since only per pin caprture is allowed fro DTS processing.

## Console output
### Pass
```
[2021-Apr-12 08:54:02] [DUT: 1]Plist=[func_plist] has finished burst index=[0] with result=[PASS].
,Functional::PrimeFuncCaptureFailuresFuncTest_P1,1,Pass
```

### Fail
```
[2021-Apr-12 09:17:33] [DUT: 1]ERROR:
|	Error in instance=[Functional::PrimeFuncCaptureCtvFuncTest_P1]:
|	Prime.Base.Exceptions.FatalException occured : Execute for plist=[func_plist] failed during burst=[0].
Failed executing plist=[func_plist].
```

## Datalog output
```python
2_tname_Functional::CaptureFailures_FiveFailuresButPrintFour_F0
2_category_1
2_fdpmv_100
2_fcpmv_-1
2_fsdmv_-1
2_pttrn_DomainA_All_DPIN_Dig:failures:failures_burstoff_plist
2_vcont_1101
2_faildata_8032,8033
2_tname_Functional::CaptureFailures_FiveFailuresButPrintFour_F0_1
2_category_1
2_fdpmv_102
2_fcpmv_-1
2_fsdmv_-1
2_pttrn_DomainA_All_DPIN_Dig:failures:failures_burstoff_plist
2_vcont_1103
2_faildata_8032
2_tname_Functional::CaptureFailures_FiveFailuresButPrintFour_F0_2
2_category_1
2_fdpmv_104
2_fcpmv_-1
2_fsdmv_-1
2_pttrn_DomainA_All_DPIN_Dig:failures:failures_burstoff_plist
2_vcont_1105
2_faildata_8033
2_tname_Functional::CaptureFailures_FiveFailuresButPrintFour_F0_3
2_category_1
2_fdpmv_106
2_fcpmv_-1
2_fsdmv_-1
2_pttrn_DomainA_All_DPIN_Dig:failures:failures_burstoff_plist
2_vcont_1107
2_faildata_8032,8033
```

## Custom User Code Hooks

Here is the list of functions available to the user code to override:

### List<string> GetDynamicPinMask()
- Can be used to populate mask pins after TP load
### bool ProcessCtvPerPin(Dictionary<string, string> ctvData)
- Takes a the results of a capture CTV per pin test in the form of a dictionary of pinNames to CTV data. User defined processing is then performed on the results.
- Default behavior is to print all ctv data to debug console and return True.
### bool ProcessFailures(ICaptureFailureTest captureFailureTest)
- Takes capture failure test and processes the results.
- Default behavior is to log failures to ituff (based on MaxFailuresToItuff input parameter) and then to print each per cycle failure to console. Test returns True if no failures were found, False otherwise.
### bool ProcessCtvPerCycle(ICaptureCtvPerCycleTest captureCtvPerCycleTest)
- Takes capture CTV per cycle test and processes the results.
- Default behavior is to do nothing and return True.

Processing functions are used by Execute() to determine which port to exit from. If any Process* function returns false or functional test failed, then the exit port will be 0. If all Process* functions returns true and functional test pass, then exit port will be 1 by default. Additionally, users can determine which port is going to be returned by setting the value of "ExitPort" property inside any of the Process* functions following the same rules mentioned before. Processing functions run are based on type of test being used.
If a DtsConfigurationName is defined the DtsProcessingService will be called during Execute(). For more information check the Service documentation.

## Plugins

Here is the list of plugins available for this Test Method:

## TPL Samples

Below are samples for several types of functional test which FuncTestMethod can be used for:

**FuncNoCapture:**

```python
Import PrimeFuncTestMethod.xml;

Test PrimeFuncTestMethod PrimeFuncNoCaptureFuncTest_P1
{
   Patlist = "func_plist";
   LevelsTc = "Functional::basic_func_lvl_nom";
   TimingsTc = "Functional::basic_func_timing_10MHz_20MHz";
}
```
**FuncCaptureFailures:**

```python
Import PrimeFuncTestMethod.xml;

Test PrimeFuncTestMethod PrimeFuncCaptureFailures_P1
{
   Patlist = "func_plist";
   TimingsTc = "Functional::basic_func_timing_10MHz_20MHz";
   LevelsTc = "Functional::basic_func_lvl_nom";
   FailuresToCaptureTotal = 4;
}
```
**FuncCaptureScoreboard:**

```python
Import PrimeFuncTestMethod.xml;

Test PrimeFuncTestMethod PrimeFuncCaptureScoreboard_P1
{
   Patlist = "func_plist";
   TimingsTc = "Functional::basic_func_timing_10MHz_20MHz";
   LevelsTc = "Functional::basic_func_lvl_nom";
   FailuresToCaptureTotal = 4;
   PerPatFailCaptureCount = 1;
}
```
**FuncCaptureCTV (with CTV capture per cycle enabled):**

```python
Import PrimeFuncTestMethod.xml;

Test PrimeFuncTestMethod PrimeFuncCaptureCtvPerCycle_P1
{
   Patlist = "func_plist";
   TimingsTc = "Functional::basic_func_timing_10MHz_20MHz";
   LevelsTc = "Functional::basic_func_lvl_nom";
   CtvCapturePins = "xxHPCC_DPIN_Dig_slcA_AA0, xxHPCC_DPIN_Dig_slcA_AA1, xxHPCC_DPIN_Dig_slcA_AA2";
   CtvCapturePerCycleMode = "ENABLED";
}
```
**FuncCaptureFailuresAndCTV (with CTV capture per cycle disabled):**

```python
Import PrimeFuncTestMethod.xml;

Test PrimeFuncTestMethod PrimeFuncCaptureFailuresAndCtv_P1
{
   Patlist = "func_plist";
   TimingsTc = "Functional::basic_func_timing_10MHz_20MHz";
   LevelsTc = "Functional::basic_func_lvl_nom";
   CtvCapturePins = "xxHPCC_DPIN_Dig_slcA_AA0, xxHPCC_DPIN_Dig_slcA_AA1, xxHPCC_DPIN_Dig_slcA_AA2";
   CtvCapturePerCycleMode = "DISABLED";
   FailuresToCaptureTotal = 4;
}
```

## Exit Ports

Functional test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**                                                                              |
| ------------- | --------------- |----------------------------------------------------------------------------------------------|
| **-2**        | ***Alarm***     | Any alarm condition                                                                          |
| **-1**        | ***Error***     | Any software condition error                                                                 |
| **0**         | ***Fail***      | Whenever the hdmtOS reports the plist execution as failure,<br/> Or, if an extensions fails. |
| **1**         | ***Pass***      | Passing condition                                                                            |
| **4**         | ***Alarm***     | Any alarm condition if the AlarmPortRedirect is enabled.                                     |
| **5**         | ***Fail***      | DTS processing error.                                                                       |


## Additional Dependencies

N/A

## Version tracking

| **Date**                  | **Version** | **Author**                    | **Comments**                               |
| ------------------------- | ----------- | ----------------------------- | ------------------------------------------ |
| Apr 13<sup>th</sup>, 2021 | 1.0.0       | Lauren McDonald               |  Initial Commit                            |
| Oct 8<sup>th</sup>, 2021  | 1.0.1       | Lauren McDonald               |  Added ituff prints for failures           |
| Oct 19<sup>th</sup>, 2021 | 1.0.2       | Lauren McDonald               |  Added processing user code hooks          |
| Dec 17<sup>th</sup>, 2021 | 1.0.3       | Johnny A Mata Vega            |  Added Dts processing support              |
| Jul 26<sup>th</sup>, 2024 | 13.01.00    | Raquel Pinto Rosales          |  Added IDynamicMaskingPlugin <br> #50347   |
| Ago 26<sup>th</sup>, 2024 | 13.01.00    | Didier Armando Jimenez Retana |  Added IFunctionalPlugin for FrequencyMeasure<br> #45391 |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **CTV**: Capture This Vector
  - **DTS**: Digital Thermal Sensor
  - **ITUFF**: **I**ntel **T**ext **U**nified **F**ormat **F**ile.
