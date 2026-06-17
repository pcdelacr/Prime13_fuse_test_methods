**prime Test-Method Specification REP**

Prime Release 13.00.00

November 2023

[[_TOC_]]

# REP for TriggeredDc

This **REP** is intended to describe the TriggeredDc Prime TestMethod.

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

The TriggeredDc test method provides the capability to perform raw Dc measurements (Voltage and Current) **<u>during</u>** plist execution.

It can be used to implement, for example, ICC methodology where plist is executed as a precondition to a DC test In order to be able to take the measurement during plist execution, EXTTrigger or TOSTrigger commands should be placed in the pattern.

The EXTTrigger enables the ability to apply any levels test condition while a pattern is executing. This feature requires a number of files to work together to operate. The main change here is the addition of the trigger names to the .pxr file as well as the addition of the .ptm file. These two files are the main files used to set up the ability to utilize triggers in functional patterns.

Hardware Test Conditions are also considered to be "triggerable" Test Conditions.  This is because all the sequencing for the Test Condition is stored locally on the instrument and this sequence is started via a simple trigger event.  This trigger event can be initiated via software using an HdmtApi(), or can be done via a specific triggering command from another instrument, i.e. HPCC.  The EXTTrigger pattern instruction is supported on HDMT in order to send hardware triggers within the HDMT
triggering system during pattern execution.

Unlike other ATE platforms which had specific triggers for certain actions (vbump triggers, measure triggers, etc.), the HDMT triggered test condition is a very generic solution, which can support any valid Test Condition sequence.  The EXTTrigger pattern instruction refers to a specific LevelsTC object, which is triggered to begin its execution sequence.  It does not matter what this sequence of operations is within the Test Condition, it just tells the Test Condition to start. The target Test Condition could be a simple StartMeasurement or VForce action, or it could be a complex series of pin operations.  The content of the Test Condition is irrelevant as far as the triggering system is
concerned.

More details can be found here:

C:\\Intel\\hdmt\\\<TOSVersion\>\\TOSUserSDK\\HdmtUserGuide\\HDMTSoftware (Common)\\HdmtPatternSpecification.pdf

### Pattern Trigger Map

The Pattern Trigger Map (PTM) file is used to provide a look-up table for translating trigger names found in the PXR and PAT files to actual Test Conditions defined within the TCG files. The current format only supports the ability to trigger levels test conditions with the LevelsTC keyword. Multiple trigger maps are supported, allowing the exact same pattern object to trigger different test conditions from run to run.

More details can be found here:

C:\\Intel\\hdmt\\\<TOSVersion\>\\TOSUserSDK\\HdmtUserGuide\\HDMTSoftware (Common)\\HdmtPatternSpecification.pdf

#### Pattern Trigger Map alephable

TestConditionService can apply ExtTrigger during init execution, this option can be used to apply ExtTrigger only one time per plist in Init instead multiples execution in each test methos.
For more details, check the TestConditionService documentation.

### Software Trigger

The TOSTrigger, a.k.a Software Trigger, is a callback to the library from the plist execution. A Software Trigger could tentatively do anything. On this test method, it will apply to hardware configuration provided by user with SoftwareTriggerConfiguration parameter. This configuration file follows a structure like this one:

```json
{
  "PinSettings": [
    {
      "PinName": "HDDPS_HC_nogang_12ohm1",
      "Indexes": [ 1, 2 ],
      "Settings": {
        "StartMeasurement": "True",
        "PreMeasurementDelay": "1",
        "SamplingRatio": "1",
        "SamplingMode": "Average",
        "SamplingCount": "1"
      }
    },
    {
      "PinName": "HDDPS_LC_nogang_12ohm1",
      "Indexes": [ 3, 2 ],
      "Settings": {
        "StartMeasurement": "True",
        "PreMeasurementDelay": "1",
        "SamplingRatio": "1",
        "SamplingMode": "Trace",
        "SamplingCount": "2"
      }
    }
  ]
}
```
The `Indexes` (to be renamed to `TriggerOperands`) have the list of operands to match during the plist execution. A block will be executed only if it matches the operand from the `TOSTrigger`. If this is not given, the block will be executed on each `TOSTrigger` called during the plist execution.

The `Settings` block mimics a pin configuration from a level block, it could have anything that a level block supports, and as any level block, the content must meet the hdmtOS expectations.

<p><span style="font-weight: bold; color:blue">Note:</span> Pin attributes defined in PinSetting are accessible through user extensions, if software Trigger is used the setup data is stored in TriggeredDcTestInstanceResults in <span style="font-weight: bold; color:darkgreen">SoftwareTriggerSettings</span> parameter.</p>

### Verify

  - Validate limits (numeric with units)
  - Validate test condition exist and valid
  - Validate pins
  - Match number of pins to number of limits
  - Validate plist exists

### Execute

  - Execute functional test (preconditions the DUT)
  - Apply DC Levels to HW (triggers DC measurement)
  - Gather the measured values
  - Match measured values against the provided limits
  - Print results to Datalog(ituff)

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the TriggeredDc test method

| **Parameter Name**           | **Required?** | **Type**        | **Values**                                                                                                                                                                       | **Comments**                                                                          |
| ---------------------------- | ------------- | --------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------- |
| MeasurementTypes             | No            | String (choice) | Comma separated list of measurement types, Current(**default**) , Voltage or inital letter "C", "V"                                                                              | if is used only one measurement type, that will be applied to all pins                |
| DatalogLevel                 | No            | String (choice) | FAIL_ONLY(**default**), ALL, [COMPRESS](#Compress-Datalog-Output), PINMAP_COMPRESS                                                                                               | FAIL_ONLY - prints to Ituff only fail result.<br> ALL - prints both fail and pass results.<br> COMPRESS - print all result in ituff compress format.<br> PINMAP_COMPRESS - print all result in ituff compress format with pinMapId. |
| Patlist                      | Yes           | Plist           | Plist name to be executed. Pattern needs to include EXTTrigger or TOSTrigger commands in order to take a measurement during the execution, see more details [here](#Methodology) |                                                                                       |
| PcatMode                     | No            | String (choice) | VOL, LKGHIGH, LKGLOW, DISABLED(**default**)                                                                                                                                      | If [PcatMode](#PCAT-Datalog) are different to DISABLED will enable to print Header line in level 6 and measurement results in PCAT format in ituff. This will also require 3 SCVars to be present related to PCAT.**|
| CtvPins                      | No            | String          | Comma separated list of pins for which CTV data should be captured                                                                                                               |                                                                                       |
| Pins                         | Yes           | String          | Comma separated list of measured pins. These pins must be included on the SoftwareTriggerConfiguration or the External trigger.                                                  |                                                                                       | 
| LowLimits                    | No            | String          | Comma separated list of real numbers with units representing measurement low limits.                                                                                             | Default value – empty string                                                          |
| HighLimits                   | No            | String          | Comma separated list of real numbers with units representing measurement high limits.                                                                                            | Default value – empty string                                                          |
| LevelsTc                     | Yes           | LevelsCondition | Levels test condition to be applied prior to pattern execution                                                                                                                   |                                                                                       |
| TimingsTc                    | Yes           | TimingCondition | Timing test condition to be applied prior to pattern execution                                                                                                                   |                                                                                       |
| MaskPins                     | No            | String          | Comma separated list of pins for which the fail data capture will be skipped                                                                                                     | Default value – empty string                                                          |
| TriggerMapName               | No*           | String          | PatternTrigger map name , see more details [here](#Pattern-Trigger-Map)                                                                                                          |                                                                                       |
| SoftwareTriggerConfiguration | No*           | String          | The path to the configuration file to the software trigger.                                                                                                                      | Default value – empty string                                                          |
| AlarmPortRedirect            | No            | String (choice) | DISABLED  (**default**)                                                                                                                                                          | Default alarm port=[-2] behavior.                                                     |
|                              |               |                 | ENABLED                                                                                                                                                                          | Enable the alarm port redirect to port=[3].                                           |

*At least one of the `TriggerMapName` or the `SoftwareTriggerConfiguration` must be provided.

** Station controller user variables required are: `PCATEnableFlags`, `PCATModelNames` and `PCATModelTokens`.

## Console output (debug mode)

Measurement results can be printed to console in debug mode in form of below table

Passing case results printout

![image.png](./.attachments/image-6d1e304f-42ae-4487-bdd9-5b29b32a9bbf.png)

Failing case results printout

![image.png](./.attachments/image-6efa96d6-51d4-4ade-8d05-df80a19ec9fd.png)

## Datalog output

Functional test execution results are logged as shown below:

<u>Functional part passed printout</u>:
```
2_tname_Func::PrimeTriggeredDcTest
2_strgval_pass
```

<u>Functional part failed printout</u>:
```
2_tname_Func::PrimeTriggeredDcTest
2_strgval_fail
```

Could not execute functional test (abnormal error or alarm)
```
2_tname_Func::PrimeTriggeredDcTest
2_strgval_notexecuted
```

Dc results are logged to Ituff in “composite” format as shown below

```
2_tname_Dc:: PrimeTriggeredDcTest
2_category_pc
2_composite_2028_0.001488
2_composite_2028_0.001488
2_composite_2020_0.002499
2_composite_2020_0.002499
2_composite_2004_0.003490
2_composite_2004_0.003490
2_composite_7051_0.000496
2_composite_7051_0.000496
2_composite_7107_0.000496
```

### Compress Datalog Output

*This datalog mode can be enabled through the parameter [DatalogLevel](#Test-Instance-Parameters) = Compress*

**Ituff format:**
```
2_tname_<test name>_<ituff token>_pc
2_strgval_<number of pins in this line>|<pin1>=<value>|<value>|...;<pin2>=<value>|<value>|...
2_comnt_passpins_{<pin1 channel number>,<pin2 channel number>,...}
2_tname_<test name>_<ituff token>_fc
2_strgval_<number of pins in this line>|<pin1>=<value>|<value>|...;<pin2>=<value>|<value>|...
2_comnt_failpins_{<pin1 channel number>,<pin2 channel number>,...}
```
**Example:**
```python
Test PrimeTriggeredDcTestMethod PGTResC0
{
   Patlist = "dc_plist";
   LevelsTc = "Dc::SecondDcTestMethodTC";
   TimingsTc = "Dc::basic_func_timing_10MHz_20MHz";
   Pins = "all_VLC";
   MeasurementTypes = "Voltage";
   LowLimits = "-200mV";
   HighLimits = "560mV";
   TriggerMapName = "Dc::TriggerDCMap";
   LogLevel = "PRIME_DEBUG";
   DatalogLevel = "Compress";
}
```

With the PrimeTriggeredDcTestMethod example above, let’s assume:
- **all_VLC** is a pinGroup defined with: HDDPS_VLC_16Kohm1, HDDPS_VLC_16ohm2, HDDPS_VLC_16ohm3, HDDPS_VLC_16Kohm4, HDDPS_VLC_16ohm1 and HDDPS_VLC_16ohm4.
- HDDPS_VLC_1600ohm3, HDDPS_VLC_160ohm2, HDDPS_VLC_160ohm3 measurements pass.
- All other measurements passed.

```
2_tname_PGTResC0_fc
2_strgval_3|HDDPS_VLC_16Kohm1=0.600000|HDDPS_VLC_16ohm2=-0.400000|HDDPS_VLC_16ohm3=0.570000
2_comnt_failpins_{0000006|0011001|0000014}
2_tname_PGTResC0_pc
2_strgval_3|HDDPS_VLC_16Kohm4=0.000000|HDDPS_VLC_16ohm1=0.010000|HDDPS_VLC_16ohm4=-0.190000
2_comnt_passpins_{0011009|0000000|0011015}
```

### PCAT Datalog

[PCAT](#Acronyms) Level-6 information is produced when a PCAT test instance has been properly verified. An Example, based on the current specification follows:

<p>6_comnt_<span style="color:yellow">PCAT_PinGrpTestSetMbr_</span><span style="color:green">VOL</span>!_<span style="color:cyan">15PnForce_3PgResult</span>!_<span style="color:orange">AM__PCAT_INORDER_ROX</span>!_<span style="color:red">P001_to_P009_odd</span>!_<span style="color:lightblue">P001|P003|P005|P007|P009</span>
6_comnt_<span style="color:yellow">PCAT_PinGrpTestSetMbr_</span><span style="color:green">VOL</span>!_<span style="color:cyan">15PnForce_3PgResult</span>!_<span style="color:orange">_AM__PCAT_INORDER_ROX</span>!_<span style="color:red">P002_to_P010_even</span>!_<span style="color:lightblue">P002|P004|P006|P008|P010</span>
6_comnt_<span style="color:yellow">PCAT_PinGrpTestSetMbr_</span><span style="color:green">VOL</span>!_<span style="color:cyan">15PnForce_3PgResult</span>!_<span style="color:orange">_AM__PCAT_INORDER_ROX</span>!_<span style="color:red">P011_to_P019_odd</span>!_<span style="color:lightblue">P011|P013|P015|P017|P019</span>
6_comnt_<span style="color:yellow">PCAT_PinGrpTestSetMbr_</span><span style="color:green">VOL</span>!_<span style="color:purple">_4PgForce_4PgResult</span>!_<span style="color:orange">_AM__PCAT_INORDER_ROX</span>!_<span style="color:red">P001_to_P009_odd</span>!_<span style="color:lightblue">P001|P003|P005|P007|P009</span>
6_comnt_<span style="color:yellow">PCAT_PinGrpTestSetMbr_</span><span style="color:green">VOL</span>!_<span style="color:purple">_4PgForce_4PgResult</span>!_<span style="color:orange">_AM__PCAT_INORDER_ROX</span>!_<span style="color:red">P002_to_P010_even</span>!_<span style="color:lightblue">P002|P004|P006|P008|P010</span>
6_comnt_<span style="color:yellow">PCAT_PinGrpTestSetMbr_</span><span style="color:green">VOL</span>!_<span style="color:purple">_4PgForce_4PgResult</span>!_<span style="color:orange">_AM__PCAT_INORDER_ROX</span>!_<span style="color:red">P011_to_P019_odd</span>!_<span style="color:lightblue">P011|P013|P015|P017|P019</span>
6_comnt_<span style="color:yellow">PCAT_PinGrpTestSetMbr_</span><span style="color:green">VOL</span>!_<span style="color:purple">_4PgForce_4PgResult</span>!_<span style="color:orange">_AM__PCAT_INORDER_ROX</span>!_<span style="color:red">P012_to_P020_even</span>!_<span style="color:lightblue">P012|P014|P016|P018|P020</span></p>

Brief description is as follows (please refer to corporate specification if more detail is required):

o    MIDAS parser requirements, mandate items, such as “!_”.

o    <span style="color:yellow">PCAT_PinGrpTestSetMbr</span>: constant, required for MIDAS consumption of PCAT-specific data processing.

o    <span style="color:green">VOL</span>: one of 3 enumerated types of PCAT-analysis.

o    <span style="color:cyan">15PnForce_3PgResult</span>: called the {PinGrpTestSetName} is a configuration set name, as produced by the PDE test segment module owner (I/O).

o    <span style="color:orange">AM__PCAT_INORDER_ROX</span>: test program instance name.
 - There are MIDAS needs to ensure ‘__PCAT_’ substring exists inside second component of the overall test instance name. Please refer to PCAT specification documentation.

o    <span style="color:red">P001_to_P009_odd</span>: test program pin-group name.

o    <span style="color:lightblue">P001|P003|P005|P007|P009</span>: test program pins, as referenced within the specific pin-group above.

**Note:** [PCAT](#Acronyms) datalog mode only work if the UserVariable <span style="color:yellow">PCATConfigurationVars.ItuffHeaderControl</span> is set as boolean true;

**IMPORTANT**
The above datalog printout is executed by default from CustomPostProcessResults hook (see below )
In case user will override CustomPostProcessResults hook function there are few options for printing the results to datalog:
1. User will implement their own logic for results printout to Ituff as part of CustomPostProcessResults function implementation
2. To print the default datalog output in the above format (composite) user will call the TriggeredDC function PrintToDatalog in CustomPostProcessResults implementation
```csharp
        void ITriggeredDcExtensions.CustomPostProcessResults(TriggeredDcTestInstanceResults results)
        {
            this.PrintToDatalog(results.DcResults);
            bool areDcResultsWithinLimits = results.DcResults.AreAllDcResultsWithinLimits(this.perPinDcSetup);
            results.ExitPort = (results.FunctionalResult && areDcResultsWithinLimits) ? TestMethodPassPort : TestMethodFailPort;

            if (results.FailPinsName.Count > 0)
            {
                var failPinsInfo = new List<FailPinInfo>();

                foreach (var pinName in results.FailPinsName.Distinct())
                {
                    failPinsInfo.Add(DcCommon.CreateDcFailPinInfo(pinName, string.Empty, this.InstanceName));
                }

                DcCommon.DatalogFailPinInfo(failPinsInfo, this.SessionContext);
                DcCommon.StoreFailPinInfoToSharedStorage(failPinsInfo, this.SessionContext);
            }
        }
```

## Custom User Code Hooks

TriggeredDc test method supports the following extensions available to the user code to override:

```csharp
/// <summary>
/// Interface for extending the DC Test Method.
/// </summary>
public interface ITriggeredDcExtensions
{
    /// <summary>
    /// Called early during Execute to add some user-custom behavior before the test starts.
    /// </summary>
    void CustomPreExecute();

    /// <summary>
    /// Called right after successful execution to provide the user the ability to post process exit port, CTV results, and DC results.
    /// </summary>
    /// <param name="results">The object for holding the exit port, CTV results, and DC results.</param>
    void CustomPostProcessResults(TriggeredDcTestInstanceResults results);

    /// <summary>
    /// An extension to process the CTVs from the plist execution.
    /// </summary>
    /// <param name="results">The object for holding the exit port, CTV results, and DC results.</param>
    void ProcessCtvResults(TriggeredDcTestInstanceResults results);

    /// <summary>
    /// Called prior to any test in Execute to generate the test instance results object.
    /// </summary>
    /// <returns>The test instance results object.</returns>
    TriggeredDcTestInstanceResults CreateTestInstanceResults();

    /// <summary>
    /// Gets the list of pins to mask during execution. The test method will merge this list with the ones from the test instance parameter.
    /// </summary>
    /// <returns>The list of mask pins.</returns>
    List<string> GetDynamicPinMask();

    /// <summary>
    /// Called in Verify() to set user's choice of Functional Test Mode.
    /// </summary>
    /// <returns>The functional test.</returns>
    IFunctionalTest CreateNewFunctionalTest();
}
```
- CreateTestInstanceResults - Creates test instance results object that holds all results and the exit port (to be done early in Execute).
- CustomPreExecute - Allows user to perform some pre-processing before the functional test and Dc measurement.
- ProcessCtvResults - Allows user to override CTV results processing for attachment to test instance results object.  
- CustomPostProcessResults - Allows user to post-process all results and override the exit port determined by the test method.  
Input is the test instance results object which includes functional test results, CTV data, DC results, and the exit port.
- GetDynamicPinMask - Allows user to populate pinmask pins after TP load.
- CreateNewFunctionalTest - Allows user to set their own Functional Test mode.

Example implementations of those functions can be seen in Prime's SampleTP , user code section - Prime\UserSDK\SampleTP\UserCode

## TPL Samples

Here are a few test instance examples using the TriggeredDc test method
<br>**Example 1:**
```python
Test PrimeTriggeredDcTestMethod PrimeTriggeredDcTest
{
   Patlist = "array_pbist_llc_dat_lya_autoclkmux0_s0_list";
   TimingsTc = "BASE::cpu_func_sdr_univ_sta_univ_univ_b100_t100_d100";
   LevelsTc = "__main__::icc_tc";
   Pins = "NOAB_00,NOAB_01,NOAB_02";
   MeasurementTypes = "Current";
   LowLimits = "0mA,0mA,0mA";
   HighLimits = "5mA,4mA,5mA";
   TriggerMapName = "EXT_BGREF_TriggerMap";
   LogLevel = "PRIME_DEBUG";
}
```
**Example 2:**
```python
Test PrimeTriggeredDcTestMethod PrimeTriggeredDcTest
{
   Patlist = "array_pbist_llc_dat_lya_autoclkmux0_s0_list";
   TimingsTc = "BASE::cpu_func_sdr_univ_sta_univ_univ_b100_t100_d100";
   LevelsTc = "__main__::icc_tc";
   Pins = "NOAB_00,NOAB_01,NOAB_02";
   MeasurementTypes = "C,V,C";
   LowLimits = "0mA,0mV,0mA";
   HighLimits = "5mA,4mV,5mA";
   TriggerMapName = "EXT_BGREF_TriggerMap";
   LogLevel = "PRIME_DEBUG";
}
```
**Example 3:**
```python
Test PrimeTriggeredDcTestMethod PrimeTriggeredDcTest
{
   Patlist = "array_pbist_llc_dat_lya_autoclkmux0_s0_list";
   TimingsTc = "BASE::cpu_func_sdr_univ_sta_univ_univ_b100_t100_d100";
   LevelsTc = "__main__::icc_tc";
   Pins = "NOAB_00,NOAB_01,NOAB_02";
   MeasurementTypes = "Voltage,Current,Voltage";
   LowLimits = "0V,0mA,0V";
   HighLimits = "2V,4mA,1.5V";
   TriggerMapName = "EXT_BGREF_TriggerMap";
   LogLevel = "PRIME_DEBUG";
}
```

- fixed “PCATVars” user variables are now mandatory in all test programs to indicate Principal Components Analysis Test processes associated with the product under test. This UserVar information is “read” during Ituff->STARTLOT and placed into the header.

## Exit Ports

The TriggeredDc test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                           |
| ------------- | ------------- | ------------------------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                                                       |
| **-1**        | ***Error***   | Any software condition error                                              |
| **0**         | ***Fail***    | Failing condition. Results are out of limits or plist execution ad failed |
| **1**         | ***Pass***    | Passing condition - results are within the limits                         |
| **3**         | ***Alarm***   | Any alarm condition if the AlarmPortRedirect is enabled.                  |

## Acronyms

Definition of acronyms used in this document:

  - **HDMT**: **H**igh **D**ensity **M**odular **T**ester
  - **PCAT**: **P**rincipal **C**omponents **A**nalysis **T**est
  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TPL**: **T**est **P**rogramming **L**anguage

## Version tracking

| **Date**                  | **Version** | **Author**            | **Comments**                                                                                                                        |
| ------------------------- | ----------- | --------------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| Feb 23<sup>rd</sup>, 2020 | 1.0.0       | Slava Yablonovich     | Initial version                                                                                                                     |
| May 24<sup>th</sup>, 2021 | 5.0         | Didier Jimenez Retana | [Allow the TriggeredDc test method to perform measurement type per Pin.](https://dev.azure.com/mit-us/PRIME/_workitems/edit/14215)  |
| Jun  9<sup>th</sup>, 2021 | 5.2         | Kevin D. Krake        | Adding pin mask ability                                                                                                             |
| Sep 17<sup>th</sup>, 2021 | 6.1         | Didier Jimenez Retana | [Adding Compress and PinMapCompress Datalog mode](https://dev.azure.com/mit-us/PRIME/_workitems/edit/16595)                         |
| Oct  1<sup>st</sup>, 2021 | 7.0         | Didier Jimenez Retana | [Adding Pcat Datalog mode](https://dev.azure.com/mit-us/PRIME/_workitems/edit/16373)                                                |
| Nov 15<sup>th</sup>, 2021 | 7.1         | Gadeer Awaisy         | Allow user to override PrintToDatalog                                                                                               |
| Feb  4<sup>th</sup>, 2022 | 8.1         | Didier Jimenez Retana | [Implement the PatternTriggerMap alephable.](https://dev.azure.com/mit-us/PRIME/_boards/board/t/US/Backlog%20items/?workitem=23606) |
| Mar 17<sup>th</sup>, 2022 | 9.0         | Kevin D Krake         | Adding new Results method for exit port handling         
| Nob  3<sup>rd</sup>, 2022 | 12          | Didier Jimenez Retana | [Expose SoftwareTriggerSettings in test method extensions.](https://dev.azure.com/mit-us/PRIME/_boards/board/t/CR/Backlog%20items/?workitem=31418)  |
| Sep 13<sup>th</sup>, 2023 | 12.02.02    | Teoh, Khai Jie        | include fail pin name and fail channel printing to ituff. #39827 |                                                                                                   |
| Nov  8<sup>th</sup>, 2023 | 13.00.00    | Yusof, Adam Malik     | [PrimeTriggeredDcTestMethod: unable to set capture fail modes.](https://dev.azure.com/mit-us/PRIME/_workitems/edit/45084)  |
| Nov 21<sup>th</sup>, 2023 | 12.03.00    | Teoh, Khai Jie        | Disable Witch Project '2_tname_failchannel' and '3_binpinfails' printing to ituff.<br> #46008 |