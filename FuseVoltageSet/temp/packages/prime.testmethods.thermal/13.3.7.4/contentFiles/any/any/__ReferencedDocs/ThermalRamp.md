**prime Test-Method Specification REP**

Revision 1.0.4

Jun 2021

[[_TOC_]]

## REP for ThermalRamp

This **REP** is intended to describe the ThermalRamp Prime TestMethod.

## Methodology

The ThermalRamp test method provides capability to perform ramp tasks for thermal methods: presoak, normal ramp, soak ramp and ramp for fact.

Before running any ramp method SCO calibration must have been executed as thermal pins needs to be calibrated in order to take measurements. If no calibration routine has run and a measurement is attempted, tester will throw an exception regarding calibration being required.

Ramp methods inputs: list of TDAU pins (one per card), temperature set point(s), a temperature guard-band and offset in order to check when the set-point has been reached, a timeout in milliseconds is also provided in case temperature can’t be reached.

A second functionality provided by this test method provides is to optionally running a Plist, which allows the DUT to reach its desired operating temperature faster by exercising the circuitry resulting in a faster warm up.

The main differences on run modes are basically the way values are printed to ituff.

**Limits:**
  - Low temperature Limit = set point - LowerTolerance. In case of timeout and if a measurement is lower than this limit, the test will fail and report 2 as exit port.
  - High temperature Limit = set point + UpperTolerance. In case of timeout and if a measurement is higher than this limit, the test will fail and report 3 as exit port.
  - If a measurement is higher than the IntegrityLowLimit, the test will fail and report 4 as exit port.
  - If a measurement is higher than the IntegrityHighLimit, the test will fail and report 5 as exit port.


## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ThermalRamp test method

<table>
<thead>
<tr class="header">
<th><strong>Parameter Name</strong></th>
<th><strong>Required</strong></th>
<th><strong>Type</strong></th>
<th><strong>Values</strong></th>
<th><strong>Comments</strong></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td>PinNames</td>
<td>Yes</td>
<td>String (list)</td>
<td>Pin names</td>
<td>Comma separated list of pins</td>
</tr>
<tr class="even">
<td>RampMode</td>
<td>Yes</td>
<td>String</td>
<td>Presoak</td>
<td>Runs presoak (no ituff printing)</td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td>Normal (default)</td>
<td>Normal run</td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td>Soak</td>
<td>Soak run</td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td>Fact</td>
<td>Fact run</td>
</tr>
<tr class="even">
<td>LogPinToken</td>
<td>No</td>
<td>String (list)</td>
<td>Pin token</td>
<td>Comma separated list of tokens</td>
</tr>
<tr class="odd">
<td>LevelsTc</td>
<td>No</td>
<td>LevelsCondition</td>
<td>Level Condition<br />
Name</td>
<td>Levels test condition to be applied prior to pattern execution</td>
</tr>
<tr class="even">
<td>TimingsTc</td>
<td>No</td>
<td>TimingCondition</td>
<td>Timing Condition<br />
Name</td>
<td>Timing test condition to be applied prior to pattern execution</td>
</tr>
<tr class="odd">
<td>Patlist</td>
<td>No</td>
<td>String</td>
<td>Pattern of Patlist</td>
<td>Full path to pattern or patlist file</td>
</tr>
<tr class="even">
<td>SetPoints</td>
<td>Yes</td>
<td>Integer (list)</td>
<td>Integer</td>
<td>Comma separated list of values</td>
</tr>
<tr class="odd">
<td>UpperTolerance</td>
<td>Yes</td>
<td>Integer (list)</td>
<td>Celsius degrees (positive)</td>
<td>Used to calculate the high measurement limits from set point</td>
</tr>
<tr class="even">
<tr class="odd">
<td>LowerTolerance</td>
<td>Yes</td>
<td>Integer (list)</td>
<td>Celsius degrees (positive)</td>
<td>Used to calculate the low measurement limits from set point</td>
</tr>
</tr>
<tr class="even">
<tr class="odd">
<td>IntegrityHighLimit </td>
<td>No</td>
<td>Integer</td>
<td>Celsius degrees (positive)</td>
<td>High measurement integrity limit for hardware protection (default: 200)</td>
</tr>
</tr>
<tr class="even">
<tr class="odd">
<td>IntegrityLowLimit </td>
<td>No</td>
<td>Integer</td>
<td>Celsius degrees (positive)</td>
<td>Low measurement integrity limit for hardware protection (default: 130)</td>
</tr></tr>
<tr class="odd">
<td>Timeout</td>
<td>Yes</td>
<td>Integer</td>
<td>Integer</td>
<td>Time in milliseconds to wait before ending a failing test</td>
</tr>
<tr class="even">
<td>MaskPins</td>
<td>No</td>
<td>String</td>
<td>Pin names</td>
<td>Comma separated list of pins for which the fail data capture will be skipped, Default is Empty String</td>
</tr>
</tbody>
</table>

**Notes:**

  - If more than one set point is provided it needs to match the same number of pins
  - A uservar name can also be used as input to UpperTolerance and LowerTolerance (tolerance parameters).
  - The tolerance parameters must define the same number of elements.     
  - If the tolerance parameters have more than one value defined, the count must match the number of defined pins.

## Datalog output

This Test Method prints to ituff a temperature report after ramp is completed, except presoak which doesn’t prints to ituff.

Normal Ramp example:

0\_comnt\_ramp\_TDAU\_RAMP
0\_comnt\_mrslt\_TD0\_1\_110.7000
0\_comnt\_mrslt\_TD0\_2\_105.3000
0\_comnt\_mrslt\_TD0\_3\_94.4000
0\_tname\_ramptime
0\_mrslt\_150.1687

Soak Ramp example:

0\_comnt\_ramp\_TDAU\_RAMP\_FOR\_SOAK
0\_comnt\_mrslt\_TD0\_1\_120.7000
0\_comnt\_mrslt\_TD0\_2\_110.3000
0\_comnt\_mrslt\_TD0\_3\_95.4000
0\_tname\_soaktime
0\_mrslt\_150.1687

Besides the full list of measurements listed as comments (showed above) the Thermal Ramp Test Method will print the first and last temperatures per diode as Ituff measurements
Example:

2_tname_ThermalRamp::ThermalRamp_Single_P1_TDAU_00_START
2_mrslt_23.0000
2_tname_ThermalRamp::ThermalRamp_Single_P1_TDAU_00_STOP
2_mrslt_23.0000

Fact Ramp example:

2\_tname\_PCS\_FRST0
2\_mrslt\_95.2
2\_tname\_PCS\_FRET0
2\_mrslt\_96.1
2\_tname\_PCS\_FRR0
2\_mrslt\_-0.0013
2\_tname\_PCS\_FRT
2\_mrslt\_699.9018

Notes:

  - PCS\_FRSTX is first temperature reading for the diode “X”, X={0, 1, 2…}.
  - PCS\_FRETX is the last temperature reading for the diode “X”, X={0, 1, 2…}
  - PCS\_FRT is the total time spent in in ms.
  - PCS\_FRRX is the ramping rate for the diode “X”, X={0, 1, 2…}. It is defined as: (PCS\_FRSTX – PCS\_FRETX) / PCS\_FRT

## Custom User Code Hooks

Here is the list of functions available to the user code to override.

  - *void PrintToItuff(Dictionary\<string, double\> lastResults, Dictionary\<string, double\> firstMeas)*  
    - Takes care of formatting data and printing it to ituff, if user desired to use another format of do some treatment to the data before printing it can be done here.

  - *List<string> GetDynamicPinMask()*  
    - Populates pin mask pins after TP load, can be overidden by user code.

## TPL Samples

Here are a few test instance examples using the ThermalRamp test method

**TPL Samples:**

```python
Import PrimeThermalRampTestMethod.xml;

# Sample1: One Channel setup with limits and timeout
Test PrimeThermalRampTestMethod ThermalRamp_Single
{
   PinNames = "TDAU_CH_PCH";
   LogPinToken = "0";
   RampMode = Normal;
   LevelsTc = "";
   TimingsTc = "";
   Patlist = "";
   SetPoints = "25";
   Range = "2";
   Offset = 0;
   Timeout = "10";}

# Sample 2: Multiple Channels and set points
Test PrimeThermalRampTestMethod ThermalRamp_MultiChannel
{
   PinNames = "TDAU_CH_PCH,TDAU_CH_CORE,TDAU_CH_GT";
   LogPinToken = "1,2,3";
   RampMode = Soak;
   LevelsTc = "";
   TimingsTc = "";
   Patlist = "";
   SetPoints = "23, 24, 25";
   Range = "4";
   Offset = 0;
   Timeout = "10";
}

# Sample 3: One channel with Pattern and required test conditions
Test PrimeThermalRampTestMethod ThermalRamp_Pattern
{
   PinNames = "TDAU_CH_PCH";
   RampMode = Presoak;
   LevelsTc = " VCCPRIM_1P8_HC";
   TimingsTc = "cpu_func_sdr_univ";
   Patlist = " ~HDMT_TPL_DIR/Modules/Thermal/InputFiles/esoak.pat";
   SetPoints = "25";
   Range = "2";
   Offset = 0;
   Timeout = "10";
}
```
## Exit Ports

The ThermalRamp test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition          |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |
| **2**         | ***Fail***    | Low limit                    |
| **3**         | ***Fail***    | High limit                   |
| **4**         | ***Fail***    | Integrity low limit          |
| **5**         | ***Fail***    | Integrity high limit         |
## Additional Dependencies

This test method depends on ScocCalibration been executed before calling any type of Ramp. To run this ScocCalibration user needs to execute ApplyTestCondition test method setting the name of the ScocCalibration condition (i.e “TDAU\_SCOC\_TC”).

## Version tracking


| **Date**       | **Version** | **Author**        | **Comments**                           |
| -------------- | ----------- | ----------------- |----------------------------------------|
| Feb 28th, 2020 | 1.0.0       | Alberto Suarez    | Initial document released              |
| Mar 30th, 2020 | 1.0.1       | Alberto Suarez    | Updated document based on feedback     |
| Apr 28th, 2020 | 1.0.2       | Alberto Suarez    | Renamed to Ramp and added ramp modes   |
| Nov 5th, 2020  | 1.0.3       | Johnny Ariel Mata | adding tolerance and integrity check   |
| Jun 9th, 2021  | 1.0.4       | Kevin D. Krake    | Adding pin masking ability             |
| may 13th, 2024  | 13.0.0     |Johnny Ariel Mata  | Print first and last ramp temperatures |

## Acronyms

Definition of acronyms used in this document:

  - **HDMT**: High Density Modular Tester
  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **SCO-cal**: Single current offset calibration
  - **TDAU**: Thermal diode acquisition unit
  - **TPL**: Test Programming Language