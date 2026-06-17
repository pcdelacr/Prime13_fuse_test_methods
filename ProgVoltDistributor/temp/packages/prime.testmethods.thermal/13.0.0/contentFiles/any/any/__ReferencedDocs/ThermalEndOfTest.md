**prime Test-Method Specification REP**

Revision 1.0.0

March 2022

[[_TOC_]]

## REP for ThermalEndOfTest

This **REP** is intended to describe the ThermalEndOfTest Prime TestMethod.

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

<br/>

## <span style="color:#B03A2E">EndOfTest new Test Method</span>
This new test method PrimeEndOfTestTestMethod is going to provide the functionalies of the previous EOT mode of the PrimeThermalSingleMeasurementTestMethod.
A few TP changes are required from the user side, previously the user could make use of the EOT mode with PrimeThermalSingleMeasurementTestMethod instances, now PrimeEndOfTestTestMethod instances are required, see the example below.

<br/>

<u>Previous Use of the PrimeThermalSingleMeasurementTestMethod's EOT mode</u>:
```python
Import PrimeThermalSingleMeasurementTestMethod.xml;

Test PrimeThermalSingleMeasurementTestMethod SingleMeasurementEOT
{
   PinNames = "TDAU_CH_PCH,TDAU_CH_GT";
   MeasurementType = "EOT";
   UpperTolerance = "10";
   LowerTolerance = "10";
   PcsDatalogSelector = "0, 1";
}
```

<br/>

<u>New use with the PrimeEndOfTestTestMethod</u>:

```python
Import PrimeThermalEndOfTestTestMethod.xml;

Test PrimeThermalEndOfTestTestMethod EndOfTest
{
   PinNames = "TDAU_CH_PCH,TDAU_CH_GT";
   UpperTolerance = "10,5";
   LowerTolerance = "10,20";
   PcsDatalogSelector = "0, 1";
}
```

<br/><br/>

## Methodology

The ThermalEndOfTest test method is in charge of calculating the PCS statistics from the temperature measurements taken by ThermalSingleMeasurement test method.
ThermalEndOfTest retrieve the temperature measurements from the shared storage and calculates the PSC tokens to print them to ituff.

### Execute

Step 1: Gets and sets the setPoint from the "SC_TEMPERATURE" userVar using the StationControllerService.  
Step 2: Calculates the temperature limits.  
Step 3: All profile measurements are summarized to prepare the PCS data.  
Step 4: The PCS statistics are printed to ituff.  
Step 5: The result port is returned, its value is set according to the status of limit verification described below.  

**Limits:**
  - Low temperature Limit = set point - LowerTolerance. If a measurement is lower than this limit, the test will fail and report 2 as exit port.
  - High temperature Limit = set point + UpperTolerance. If a measurement is higher than this limit, the test will fail and report 3 as exit port.
  - If a measurement is higher than the IntegrityLowLimit, the test will fail and report 4 as exit port.
  - If a measurement is higher than the IntegrityHighLimit, the test will fail and report 5 as exit port.

<u>Flow chart for Execute</u>:
::: mermaid
graph TD
    A((START))
    A-->A1[GetSetPoint from UserVar]
    A1-->A2[Calculates Temperature limits]
    A2-->A3{ContinusReadStatus?}
    A3-->|Enabled|A4[Stop data collection]
    A3-->|Disabled|D[Sumarize all stored measurements]
    A4-->B[Sumarize profile measurements]
    B-->C[Delete data collection]
    C-->E[Print to ituff]
    D-->E[Print to ituff]
    E-->F[Return result port]
    F-->G((END))
:::


## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ThermalSingleMeasurement test method

| **Parameter Name** | **Required?** | **Type**        | **Units**                    | **Description**                                                      | **Comments**    |
| ------------------ | :-----------: | --------------- | ---------------------------- | -------------------------------------------------------------------- | --------------- |
| PinNames           | Yes           | String (list)   | Pin names                    | Name the pins to use, can be a single pin or several comma separated | Comma separated |
| UpperTolerance     | Yes           | String (List)   | Celsius degrees (positive)   | Tolerance value to calculate the high limits of the measurement, it can be entered as list of numbers or using userVar names | When a number is entered as a value, it must be an integer |
| LowerTolerance     | Yes           | String (List)   | Celsius degrees (positive)   | Tolerance value to calculate the low limits of the measurement, it can be entered as list of numbers or using userVar names | When a number is entered as a value, it must be an integer. |
| ContinuousReadSampleRate| No       | Integer         | Milliseconds (multiples of 4)| Sample rate time (milliseconds) in which the continuous read will be performed | default: 4ms |
| IntegrityHighLimit | No            | Integer         | Celsius degrees (positive)   | High temperature limit of a measurement integrity check     | default: 200    |
| IntegrityLowLimit  | No            | Integer         | Celsius degrees (positive)   | Low temperature limit of a measurement integrity check      | default: 130    |
| PcsDatalogSelector | No            | String (list)   | Character ids                | When running different cores, it allows to differentiate each core by setting a datalog selector for each pin | Comma separated |

**Notes:**

  - A uservar name can also be used as input to UpperTolerance and LowerTolerance (tolerance parameters).
  -  For Tolerance inputs, if one value is entered, the tolerance will be applied to all pins provided in PinNames. Otherwise, the list should be the same length as PinNames and each tolerance in the list will be applied to the cooresponding pin in PinNames(First tolerance to first pin, etc.)  
  - The count of characters used in PcsDatalogSelector must match the number of defined pins if defined.
  - When PcsDatalogSelector not defined or has no values the test will automatically assign indexes for data logs
  - If **ContinuousRead** mode is enabled during ThermalSingleMeasurement SOT measurement type, temperature collection is triggered for the **PinNames** every **ContinuousReadSampleRate** milliseconds. The data collection will be stopped during ThermalEndOfTest instance, and the PCS tokens are going to be calculated based on those collected temperatures.
  - To print the **PCS_SOT_Thermal_Head_Temperature_Read** token in SORT (DTC) the uservar "SC_CHUCKID" must be defined.


## Datalog output

ThermalEndOfTest will print results to ituff, here is an example of the printing:

```
2\_tname\_PCS\_SETPOINT
2\_mrslt\_10
2\_msunit\_C
2_tname_PCS_SOT_Thermal_Head_Temperature_Read
2_mrslt_70
2_msunit_C
2\_tname\_PCS\_SOT0
2\_mrslt\_98
2\_msunit\_C
2\_tname\_PCS\_TJRISE0
2\_mrslt\_2.3
2\_msunit\_C
2\_tname\_PCS\_TJDROOP0
2\_mrslt\_-2.3
2\_msunit\_C
2\_tname\_PCS\_SOT1
2\_mrslt\_96
2\_msunit\_C
2\_tname\_PCS\_TJRISE1
2\_mrslt\_-0.5
2\_msunit\_C
2\_tname\_PCS\_TJDROOP1
2\_mrslt\_0.5
2\_msunit\_C
```

## Custom User Code Hooks

TBD

## TPL Samples

Here is a test instance example using the ThermalEndOfTest test method

```python
Import PrimeThermalEndOfTestTestMethod.xml;

Test PrimeThermalEndOfTestTestMethod EndOfTest
{
   PinNames = "TDAU_CH_PCH,TDAU_CH_GT";
   UpperTolerance = "10";
   LowerTolerance = "10";
   PcsDatalogSelector = "0, 1";
}
```

## Exit Ports

The ThermalEndOfTest test method supports the following exit
ports:

| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition          |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |
| **2**         | ***Fail***    | Fails due to low limit       |
| **3**         | ***Fail***    | Fails due to high limit      |
| **4**         | ***Fail***    | Fails due to integrity low limit  |
| **5**         | ***Fail***    | Fails due to integrity high limit |

## Additional Dependencies
This test depends on PrimeThermalSinglemeasurementTestMethod to take measurements before it is executed

## Version tracking

| **Date**                   | **Version** | **Author**        | **Comments**                           |
| -------------------------- | ----------- | ----------------- | -------------------------------------- |
| Mar 31<sup>th</sup>, 2022  | 9.0         | Maria N. Rojas    | Initial document release               |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SCOC**: Single current offset calibration
  - **PCS**: Process Control System