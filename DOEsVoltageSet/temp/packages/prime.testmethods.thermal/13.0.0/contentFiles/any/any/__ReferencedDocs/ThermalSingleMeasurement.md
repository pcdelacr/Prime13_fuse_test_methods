**prime Test-Method Specification REP**

Revision 1.0.0

Jul 2020

[[_TOC_]]

## REP for ThermalSingleMeasurement

This **REP** is intended to describe the ThermalSingleMeasurement Prime TestMethod.

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
ThermalSingleMeasurement now has only 3 modes (SOT, TJ, and CHECK_DIE_FORCE), the previous mode EOT is now a new test method called PrimeEndOfTestTestMethod, which is going to provide the functionalies of the previous EOT mode.  
A few TP changes are required from the user side, the user will have to change the previous PrimeThermalSingleMeasurementTestMethod instances that use the EOT mode for PrimeEndOfTestTestMethod instances as in the example below.

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
   UpperTolerance = "10";
   LowerTolerance = "10";
   PcsDatalogSelector = "0, 1";
}
```

<br/><br/>

## Methodology

The ThermalSingleMeasurement test method provides capability to perform a single measurement of the thermal pins provided by the user.

This test method provides 3 modes:

1)  SOT:

> Marks this measurement as the starting reference for the pin, only one
> is required, thus any repeated measurement with this tag will set the
> new measurement as the reference.

2)  TJ:

> Performs one measurement on each pin and stores it on the shared
> storage, this will store as many measurements as the memory allows.

3)  CHECK\_DIE\_FORCE:

This mode performs a measurement and logs it to ituff but the value won’t be taken into account for PCS calculations.

All these modes prints either their measurements or calculated values to ituff.

**Limits:**
  - Low temperature Limit = set point - LowerTolerance. If a measurement is lower than this limit, the test will fail and report 2 as exit port.
  - High temperature Limit = set point + UpperTolerance. If a measurement is higher than this limit, the test will fail and report 3 as exit port.
  - If a measurement is higher than the IntegrityLowLimit, the test will fail and report 4 as exit port.
  - If a measurement is higher than the IntegrityHighLimit, the test will fail and report 5 as exit port.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ThermalSingleMeasurement test method

| **Parameter Name** | **Required?** | **Type**         | **Values**                                          | **Comments**                                                     |
| ------------------ | ------------- |------------------|-----------------------------------------------------|------------------------------------------------------------------|
| MeasurementType    | Yes           | String (choice)  | SOT                                                 |                                                                  |
|                    |               |                  | TJ (**default**)                                    |                                                                  |
|                    |               |                  | CHECK\_DIE\_FORCE                                   |                                                                  |
| PinNames           | Yes           | String (list)    | Pin names                                           | Comma separated                                                  |
| UpperTolerance     | Yes           | Integer (list)   | Celsius degrees (positive)                          | Comma separated                                                  |
| LowerTolerance     | Yes           | Integer (list)   | Celsius degrees (positive)                          | Comma separated                                                  |
| ContinuousRead     | No            | String (choice)  | Enable or Disable                                   | Starts temperature collection during SOT.<br/>Default: `Enabled` |
| ContinuousReadSampleRate| No       | Integer          | Milliseconds (multiples of 4)                       | Default: 4ms                                                     |
| DffTokens          | No            | String (list)    | Dff Tokens only for SOT                             | Comma separated                                                  |
| IntegrityHighLimit | No            | Integer          | Celsius degrees (positive)                          | default: 200                                                     |
| IntegrityLowLimit  | No            | Integer          | Celsius degrees (positive)                          | default: 130                                                     |
| StorageTag         | No            | String           | Shared storage tag name                             |                                                                  |
| UserVarStoreNames  | No            | String (list)    | Names for the user vars                             | Comma separated                                                  |
| RetriesMaxCount  | No            | Unsigned Integer | The max number of retries once a measurement fails. | default: 0                                                       |
| RetriesDelay  | No            | Unsigned Integer | The delay between retries in ms.                    | default: 0                                                       |

**Notes:**

  - A uservar name can also be used as input to UpperTolerance and LowerTolerance (tolerance parameters).
  - The tolerance parameters must define the same number of elements.     
  - If the tolerance parameters have more than one value defined, the count must match the number of defined pins.
  - If StorageTag is not empty the thermal measurements will be stored in the shared storage using that tag name.
  - If UserVarStoreNames is not empty, the test method will store the thermal measurements in uservars. (first pin measurement will be store in first uservar name defined, and so on a so forth)
  - If UserVarStoreNames is not empty, its number of elements must match the number of PinNames
  - Expected format for the uservar names in UserVarStoreNames or the tolerance parameters:
    Module::Collection.UserVarName
  - **DffTokens** is only for SOT measurement type, it should has one token per Pin.
  - If **ContinuousRead** mode is enabled during SOT measurement type, temperature collection is triggered for the **PinNames** every **ContinuousReadSampleRate** milliseconds. The data collection will be stopped during PrimeThermalEndOfTestTestMethod instance, and the PCS tokens are going to be calculated based on those collected temperatures.
  - To print the **PCS_SOT_Thermal_Head_Temperature_Read** token in SORT (DTC) the uservar "SC_CHUCKID" must be defined.
  - Retries doesn't apply to SOT mode.
  - When applying retries, only the last measurement is accounted for each pin. The test method drops any measurement from previous failing retry.


## Datalog output

Each mode will print results to ituff, here is an example of that each
of these prints:

SOT:

2\_tname\_TDAU\_CH\_PCH\_SotInstanceName
2\_mrslt\_100.3
2\_msunit\_C
2_tname_Thermal::SingleMeasurementSOT_P1_Thermal_Head_Read
2_mrslt_70.0
2_msunit_C

TJ:

2\_tname\_TDAU\_CH\_PCH\_TjInstanceName
2\_mrslt\_95.5
2\_msunit\_C


## TPL Samples

Here are a few test instance examples using the ThermalSingleMeasurement test method

**TPL Sample1: SOT Multiple Limits with Multiple Channels**

```python
Import PrimeThermalSingleMeasurementTestMethod.xml;

Test PrimeThermalSingleMeasurementTestMethod SOTMultiLimits_MultiChannel
{
   PinNames = "TDAU_CH_PCH,TDAU_CH_CORE,TDAU_CH_GT";
   MeasurementType = "SOT";
   UpperTolerance = "10";
   LowerTolerance = "10";
}
```

**TPL Sample2: SOT Multiple Limits with Multiple Channels and DFF capability**

```python
Import PrimeThermalSingleMeasurementTestMethod.xml;

Test PrimeThermalSingleMeasurementTestMethod SOTAndDff_MultiChannel
{
   PinNames = "TDAU_CH_PCH,TDAU_CH_CORE,TDAU_CH_GT";
   MeasurementType = "SOT";
   UpperTolerance = "10";
   LowerTolerance = "10";
   DffTokens = "dffToken1,dffToken2,dffToken3";
}
```

**TPL Sample3: TJ Single Limits with Multiple Channels**

```python
Import PrimeThermalSingleMeasurementTestMethod.xml;

Test PrimeThermalSingleMeasurementTestMethod SingleMeasurementTJ_MultiChannel
{
   PinNames = "TDAU_CH_PCH,TDAU_CH_GT";
   MeasurementType = "TJ";
   UpperTolerance = "10";
   LowerTolerance = "10";
}
```

**TPL Sample4: Storing measurements in user vars**
```python
Import PrimeThermalSingleMeasurementTestMethod.xml;

Test PrimeThermalSingleMeasurementTestMethod SingleMeasurementTJ_MultiChannel_UserVars
{
   PinNames = "TDAU_CH_PCH,TDAU_CH_GT, TD0";
   MeasurementType = "TJ";
   UpperTolerance = "10";
   LowerTolerance = "10";
   UserVarStoreNames = “Thermal1, Thermal2, Thermal3”
}
```


## Exit Ports

The ThermalSingleMeasurement test method supports the following exit
ports:

| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition          |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |
| **2**         | ***Fail***    | Fails due to low limit       |
| **3**         | ***Fail***    | Fails due to high limit      |
| **4**         | ***Fail***    | Fails due to integrity low limit       |
| **5**         | ***Fail***    | Fails due to integrity high limit      |

## Additional Dependencies

This test depends on SCOC calibration to be executed before.
When ContinuousRead mode is enabled, this test depends on PrimeThermalEndOfTestTestMethod to stop the data collection.

## Version tracking

| **Date**                   | **Version** | **Author**        | **Comments**                                                                                             |
| -------------------------- | ----------- | ----------------- | -------------------------------------------------------------------------------------------------------- |
| Mar 3rd, 2020              | 1.0.0       | Alberto Suarez    | Initial document release                                                                                 |
| Mar 30th, 2020             | 1.0.1       | Alberto Suarez    | Added new parameter PcsDatalogSelector, changed from limits to range referenced to Temperature Set Point |
| May 4th, 2020              | 1.0.2       | Alberto Suarez    | Added offset and note for case when Range = 0                                                            |
| July 10<sup>th</sup>, 2020 | 2.0         | Johnny Ariel Mata | Capability to store thermal measurements in uservars                                                     |
| Oct 7<sup>th</sup>, 2020   | 3.0         | Johnny Ariel Mata | Adding tolerance parameters instead of range
| Nov 5<sup>th</sup>, 2020   | 3.1         | Johnny Ariel Mata | Adding integrity checks
| Mar 24<sup>th</sup>, 2021  | 5.1         | Didier Armando Jiménez Retana | Added Dff capability
| May 21<sup>th</sup>, 2021  | 5.1         | Johnny Ariel Mata | Enabling Continuous read mode
| Mar 31<sup>th</sup>, 2022  | 9.0         | Maria N. Rojas    | Removed EOT mode

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SCOC**: Single current offset calibration
  - **SOT**: Start of Test
  - **TJ**: Thermal Junction
  - **DTC**: Device Thermal controller
  - **PCS**: Process Control System