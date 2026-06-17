[[_TOC_]]

## REP for IVCurve

This **REP** is intended to describe the IVCurve Prime TestMethod.

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

The IVCurve test method provides capability to perform raw Dc measurements (Voltage and Current).
The Dc measurements setup is implemented in Levels block.
IVCurve will always run a DC test at a defined force setpoint to determine pass/fail status, as well as an optional sweep against a force range.
There is an optional functional test to run before any DC testing if all parameters are set.
Each measured pin is being setup, measured (using *StartMeasurement* attribute) and then reset to its original value sequentially.
DPS and DPins are now supported.

Here is the example of serial DC measurement setup in level block :

```python
Levels sample_IVCurve
{
    HPCC_DPIN_Dig_All
    {
        PinModeSel = "Digital";
        VIH = 1;
        VIL = 0;
        TermMode = "TermVRef";
        TermVRef = 0.5;
        VOX = 0.5;
        FixedDriveState = "Off";
    }
    SequenceBreak 1mS;
    all_HC   
    {
        OPModeCheck = "VSIM";
        VForce = 1V; 
        IRange = "IR24A"; 
        IClampHi = 10A; 
        IClampLo = -10A;
        FreeDriveTime = 0.065;
        VSlewStepRatio = 127;
        #PowerSequence = True;
        
        # Mandatory Parameters for measurement.
        StartMeasurement = True;     
        PreMeasurementDelay = 5mS;   
        SamplingCount = 1; 
        SamplingRatio = 1;       
        SamplingMode = "Trace";
    }
    SequenceBreak 1mS;
    all_VLC
    {
        #PowerSequence = True;
        OPModeCheck = "ISVM";
        IRange = "IR256mA";
        IForce = 0.001A;
        VClamp = 1V;
        StartMeasurement = True;
        PreMeasurementDelay = 1;
        SamplingRatio = 1;
        SamplingMode = "Average";
        SamplingCount = 1;
    }
    SequenceBreak 1mS;
    DomainA_DPIN_PMU
    {
        OPMode = "ISVM";
        IRange = "IR1mA";
        IForce = 0.001A;
        VClampHi = 2V;
        VClampLo = -1.5V;
        StartMeasurement = True;
        PreMeasurementDelay = 20us;
        SamplingCount = 1;
        SamplingRatio = 1;
        SamplingMode = "Trace";
        PinModeSel = "PMU";
    }
} #End of sample_IVCurve

```

### Verify

  - Validate pins and force parameters

  - Validate functional test parameters

  - Validate Levels block

  - Validate limits (numeric with units)

  - Match number of pins to number of limits

### Execute

  - Execute optional functional test

  - Adjust force value to defined setpoint only if mode is not levels.

  - Apply Levels to HW (if Levels was provided by the user)

  - Gather the measured values

  - Match measured values against the provided limits

  - Repeat above measurements for sweep over defined force range

  - Print results to Datalog(ituff)

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the IVCurve test method

| **Parameter Name**  | **Required?** | **Type**                      | **Values**                                                                                         | **Comments**                                                                                                                                                                                                                        |
|---------------------|---------------|-------------------------------|----------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| DcLevels            | Yes           | LevelsCondition               | Levels TC required to setup and perform the DC measurement                                         |                                                                                                                                                                                                                                     |
| MeasurementTypes    | No            | CommaSeparatedString (choice) | Comma separated list of measurement types, Current(**default**), Voltage or inital letter "C", "V" | If only one measurement type, it will be applied to all pins                                                                                                                                                                        |
| Pins                | Yes           | CommaSeparatedString          | Comma separated list of measured pins                                                              |                                                                                                                                                                                                                                     |
| Patlist             | No            | Plist                         | Plist name to be executed                                                                          |                                                                                                                                                                                                                                     |
| LevelsTc            | No            | LevelsCondition               | Levels test condition to be applied prior to pattern execution                                     |                                                                                                                                                                                                                                     |
| TimingsTc           | No            | TimingCondition               | Timing test condition to be applied prior to pattern execution                                     |                                                                                                                                                                                                                                     |
| LowLimits           | No            | CommaSeparatedString          | Comma separated list of real numbers with units representing measurement low limits                | Default value – empty string                                                                                                                                                                                                        |
| HighLimits          | No            | CommaSeparatedString          | Comma separated list of real numbers with units representing measurement high limits               | Default value – empty string                                                                                                                                                                                                        |
| DatalogLevel        | No            | String (choice)               | FAIL_ONLY(**default**), ALL, COMPRESS, PINMAP_COMPRESS                                             | FAIL_ONLY - prints to Ituff only fail result.<br> ALL - prints both fail and pass results.<br> COMPRESS - print all result in ituff compress format.<br> PINMAP_COMPRESS - print all result in ituff compress format with pinMapId. <br>**NOTE**: Refer to [DC BusinessLogic](../../../../BusinessLogic/Dc/Readme.md) Wiki for more detail.|
| ForceStartValue     | No            | CommaSeparatedDouble          | Units default to V or A depending on measurement type                                              | Start value. MIN value for sweep mode.                                                                                                                                                                                              |
| ForceStopValue      | No            | CommaSeparatedDouble          | Units default to V or A depending on measurement type                                              | Stop value. MAX value for sweep mode.                                                                                                                                                                                               |
| ForceStepSize       | No            | CommaSeparatedDouble          | Units default to V or A depending on measurement type                                              | Step size while sweeping voltage or current for sweep mode.                                                                                                                                                                         |
| ForceSetPoint       | Yes           | CommaSeparatedDouble          | Units default to V or A depending on measurement type                                              | Force setpoint.                                                                                                                                                                                                                     |
| Mode                | No            | String (choice)               | Simple  (**default**)                                                                              | No sweep is run. Applies force value before DC test and restores force value after test.                                                                                                                                            |
|                     |               |                               | Characterization                                                                                   | Sweep is run if force range is defined.                                                                                                                                                                                             |
|                     |               |                               | Levels                                                                                             | No sweep is run. Applies a level before Dc test, does not use force value or SetPinAttributes at all only applies DcLevels and runs functional test before DC test if provided.                                                     |
| FlushSmartTcLevels  | No            | String (choice)               | Disabled                                                                                           | SmartTC levels are not ignored.                                                                                                                                                                                                     |
|                     |               |                               | Enabled  (**default**)                                                                             | SmartTC levels are ignored.                                                                                                                                                                                                         |
| AlarmPortRedirect   | No            | String (choice)               | Disabled  (**default**)                                                                            | Default alarm port=[-2] behavior.                                                                                                                                                                                                   |
|                     |               |                               | Enabled                                                                                            | Enable the alarm port redirect to port=[2].                                                                                                                                                                                         |
| AlarmModeDelay      | No            | UnsignedInteger               | 0  (**default**)                                                                                   | Once test is complete waits a given time in uS before checking for alarms. Only works if AlarmPortRedirect is enabled. Value can't be bigger than 1000.                                                                              |
| IRange              | No            | CommaSeparatedString          | Comma separated list of IRanges to use per pin. If only one is provided will be used for all pins. |                                                                                                                                                                                                                                     |
| SamplingMode        | No            | String                        | Sampling mode for pin measurement. Default is "Trace"                                              |                                                                                                                                                                                                                                     |
| SamplingCount       | No            | CommaSeparatedString          | Sampling count per pin for measurement. Default is 1                                               |                                                                                                                                                                                                                                     |
| PreMeasurementDelay | No            | CommaSeparatedString          | PreMeasurementDelay count per pin for measurement. Default is 100uS.                               | Always applied by default over the levels value.                                                                                                                                                                                    |
| FreeDriveTime       | No            | CommaSeparatedDouble          | FreeDriveTime count per pin for measurement. Default is value from level.                          | Only use if force value used and resource type requires it. Usually used for VSIM.                                                                                                                                                  |
| IClampHi            | No            | CommaSeparatedDouble          | IClampHi count per pin for measurement. Default is value from level.                               | Only use if force value used and resource type requires it. Usually used for VSIM.                                                                                                                                                  |
| IClampLo            | No            | CommaSeparatedDouble          | IClampLo count per pin for measurement. Default is value from level.                               | Only use if force value used and resource type requires it. Usually used for VSIM.                                                                                                                                                  |
| DatalogPrecisionPoint      | No            | UnsignedInteger               | 4  (**default**)                                                                                   | To set datalog precision point for double values. If no value is defined, it will use legacy default 4 precision point.                                                                              |

## Console output (debug mode)

Measurement results can be printed to console in debug mode in form of below table

Passing case results printout

```python
************************************************************
   Printed from C# instance [IVCurve::PrimeIVCurveSimpleExample] of DcTestCs test method
************************************************************

--------------------------------------------------
| Pin                    |     Result   | Status |
-------------------------------------------------
| HDDPS_LC_nogang_12ohm1 |  2.0000000 V | Pass   |
--------------------------------------------------
```

Failing case results printout

```python
************************************************************
   Printed from C# instance [IVCurve::IVCurveInheritanceTest] of DcTestCs test method
************************************************************

--------------------------------------------
| Pin              |     Result   | Status |
-------------------------------------------
| HDDPS_VLC_16ohm1 | -0.3000000 V | Fail   |
| HDDPS_VLC_16ohm1 | -0.3000000 V | Fail   |
| HDDPS_VLC_16ohm1 | -0.3000000 V | Fail   |
--------------------------------------------
```

## Datalog output

Dc results are logged to Ituff in “composite” format as shown below

**Without sweep:**

```python
2_tname_IVCurve::PrimeIVCurveSimpleExample_P1
2_category_pc
2_composite_0000019_2.0000000
2_composite_0011011_0.3456780
```

**With sweep:**

```python
2_tname_IVCurve::PrimeIVCurveSimpleExampleSweep_P1_HDDPS_LC_nogang_12ohm1_0
2_mrslt_1.2000
2_comnt_fvalue_0.2
2_tname_IVCurve::PrimeIVCurveSimpleExampleSweep_P1_HDDPS_LC_nogang_12ohm1_1
2_mrslt_1.4500
2_comnt_fvalue_0.3
2_tname_IVCurve::PrimeIVCurveSimpleExampleSweep_P1_HDDPS_LC_nogang_12ohm1_2
2_mrslt_1.7000
2_comnt_fvalue_0.4
2_tname_IVCurve::PrimeIVCurveSimpleExampleSweep_P1
2_category_pc
2_composite_0000019_2.0000000
```

**Example of fail pin:**

```python
2_tname_IVCurve::PrimeIVCurveSimpleExample_TwoPinsVoltageCurrent_F0
2_category_fc
2_composite_0000019_2.0000000
2_tname_IVCurve::PrimeIVCurveSimpleExample_TwoPinsVoltageCurrent_F0
2_category_fc
2_composite_0000029_0.1500000
```

**IMPORTANT**
The above datalog printout is executed by default from CustomPostProcessResults hook (see below )
In case user will override CustomPostProcessResults hook function there are few options for printing the results to datalog:
1. User will implement their own logic for results printout to Ituff as part of CustomPostProcessResults function implementation
2. To print the default datalog output in the above format (composite) user will call the IDcResult API PrintToDatalog in CustomPostProcessResults implementation
3. User will also be able to customize post-processing of the exit port

## Custom User Code Hooks

IVCurve test method supports the following extensions:

```c#
/// <summary>
/// Called early during Execute to add some user-custom behavior before the test starts.
/// </summary>
void CustomPreExecute();

/// <summary>
/// Called right after successful execution to provide the user the ability to post process exit port, and DC results.
/// </summary>
/// <param name="results">The object for holding the exit port and complete DC results.</param>
void CustomPostProcessResults(IVCurveTestInstanceResults results);

/// <summary>
/// Called prior to any test in Execute to generate the test instance results object.
/// </summary>
/// <returns>The test instance results object.</returns>
IVCurveTestInstanceResults CreateTestInstanceResults();

/// <summary>
/// Called intraloop to provide the user the ability to process DC results.
/// </summary>
/// <param name="results">The object for holding the exit port and complete DC results.</param>
/// <param name="perPinResults">The per-pin DC results from the IVCurve test.</param>
void DcProcess(IVCurveTestInstanceResults results, IDcResults perPinResults);

/// <summary>
/// Called intraloop to provide the user the ability to process DC sweep results.
/// </summary>
/// <param name="results">The object for holding the exit port and complete DC results.</param>
/// <param name="perPinSweepResults">The per-pin DC sweep results from the IVCurve test (force value, DcResults).</param>
void DcSweepProcess(IVCurveTestInstanceResults results, List<DcSweepResults> perPinSweepResults);

/// <summary>
/// Gets the list of pins to mask execution after execution. The test method will merge this list with the ones from the test instance parameter.
/// </summary>
/// <returns>The list of mask pins.</returns>
List<string> GetDynamicPinMask();
```

## TPL Samples

Here are a few test instance examples using the IVCurve test method
In all cases, the pins are DPS.

**With pins and optional functional test**

```python
Import PrimeIVCurveTestMethod.xml;

Test PrimeIVCurveTestMethod PrimeIVCurveSimpleExample
{
   Patlist = "ivcurve_plist";
   TimingsTc = "IVCurve::basic_func_timing_10MHz_20MHz";
   LevelsTc = "IVCurve::SampleTC";
   DcLevels = "IVCurve::Functional";
   Pins = "HDDPS_LC_nogang_12ohm1,xxHPCC_DPIN_PMU_slcA_100ohm";
   MeasurementTypes = "Voltage,Current";
   LowLimits = "1.9V,0.1A";
   HighLimits = "2.1V,0.2A";
   ForceSetPoint = "0.1,1";
   LogLevel = "PRIME_DEBUG";
   DatalogLevel = "All";
}
```

**With pingroups and no functional testing**

```python
Import PrimeIVCurveTestMethod.xml;

Test PrimeIVCurveTestMethod PrimeIVCurveSimpleExample_Pingroup
{
   DcLevels = "IVCurve::SampleIVCurveMethodTC";
   Pins = "all_HC_w_Real_Unused_grp";
   MeasurementTypes = "Current";
   LowLimits = "1mA";
   HighLimits = "5mA";
   ForceSetPoint = "1";
   LogLevel = "PRIME_DEBUG";
   DatalogLevel = "All";
}
```

**With sweep**

```python
Import PrimeIVCurveTestMethod.xml;

Test PrimeIVCurveTestMethod PrimeIVCurveSimpleExampleSweep_P1
{
   DcLevels = "IVCurve::FunctionalWithCVCTC";
   Pins = "HDDPS_LC_nogang_12ohm1";
   MeasurementTypes = "Voltage";
   LowLimits = "1.9V";
   HighLimits = "2.1V";
   ForceSetPoint = "0.1";
   ForceStartValue = "0.2";
   ForceStopValue = "0.4";
   ForceStepSize = "0.1";
   Mode = "Characterization";
   LogLevel = "PRIME_DEBUG";
   DatalogLevel = "All";
}
```

**Levels mode**

```python
Import PrimeIVCurveTestMethod.xml;

Test PrimeIVCurveTestMethod PrimeIVCurveSimpleExampleSweep_P1
{
   DcLevels = "IVCurve::FunctionalWithCVCTC";
   Pins = "HDDPS_LC_nogang_12ohm1";
   MeasurementTypes = "Voltage";
   LowLimits = "1.9V";
   HighLimits = "2.1V";
   Mode = "Levels";
   LogLevel = "PRIME_DEBUG";
   DatalogLevel = "All";
}
```

## Exit Ports

The IVCurve test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                          |
| ------------- | ------------- | -------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                                      |
| **-1**        | ***Error***   | Any software condition error                             |
| **0**         | ***Fail***    | Failing condition. Results are out of limits             |
| **1**         | ***Pass***    | Passing condition                                        |
| **2**         | ***Alarm***   | Any alarm condition if the AlarmPortRedirect is enabled. |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SHOPS**: **Sh**orts and **Op**en**s** test methodology
  - **DC: direct current**

## Version tracking

| **Date**                  | **Version** | **Author**      | **Comments**                                                                                  |
|---------------------------|-------------|-----------------|-----------------------------------------------------------------------------------------------|
| Mar 11<sup>st</sup>, 2022 | 1.0.0       | Kevin D Krake   | Initial version                                                                               |
| May  3<sup>rd</sup>, 2022 | 1.1.0       | Kevin D Krake   | Added sweep capability                                                                        |
| Jun 15<sup>th</sup>, 2022 | 1.2.0       | Kevin D Krake   | Added DPin support                                                                            |
| Aug 25<sup>th</sup>, 2022 | 1.2.1       | Kevin D Krake   | Changed SmartTc argument name                                                                 |
| Aug 31<sup>th</sup>, 2023 | 1.3.0       | Maria Hernandez | Added parameters for optional functional test                                                 |
| Sep 13<sup>th</sup>, 2023 | 12.02.02    | Teoh, Khai Jie  | include fail pin name and fail channel printing to ituff.<br> #39827                          |
| Nov 21<sup>th</sup>, 2023 | 12.03.00    | Teoh, Khai Jie  | Disable Witch Project '2_tname_failchannel' and '3_binpinfails' printing to ituff.<br> #46008 |
| Feb 23<sup>th</sup>, 2024 | 13.00.00    | Jose, Barrantes | Adding additional configuration parameters.                                                   |
| April 24<sup>th</sup>, 2025 | 13.03.00    | Mohd Asri, Mohd Faiz | Added parameter to control datalog precision point printing and change console log printing format. #59804                                                   |
