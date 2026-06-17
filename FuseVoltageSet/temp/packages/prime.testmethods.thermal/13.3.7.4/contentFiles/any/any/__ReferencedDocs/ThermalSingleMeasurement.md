PrimeThermalSingleMeasurementTestMethod User Guide
===========
This document is intended to help you get started using the PrimeThermalSingleMeasurementTestMethod, or otherwise known as ***ThermalSingleMeasurement***.

[[_TOC_]]

# Use cases
This test method performs measurements on defined pins. How measurements are stored and logged depends on the defined mode.

# Test Instance Parameters

| **Parameter Name**       | **Required?** | **Type**               | **Values**                                                                                                                                                              | **Description**                                                                                                                                                                                                                                                                                                                                                                                                                            |
|--------------------------|---------------|------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| ContinuousRead           | No            | String (choice)        | Enable (**default**), Disable                                                                                                                                           | Only for instances in SOT mode. If enabled, this instance will start a collection of thermal data on specified pins for PCS calculations in your test program's *PrimeEndOfTestTestMethod* instances. Data collection will only stop when an instance of *PrimeEndOfTestTestMethod* is executed.                                                                                                                                           |
| ContinuousReadSampleRate | No            | Integer                | Milliseconds as integer values (must be defined as a multiple of four). Default = 4ms.                                                                                  | If ContinuousRead is enabled, this parameter determines the rate at which the tester should sample your defined pins for thermal data. You must provide a multiple of four in this parameter. The fastest sample rate is 4ms.                                                                                                                                                                                                              |
| DffTokens                | No            | Comma separated string | Dff Tokens in your test program.                                                                                                                                        | Only for instances in SOT mode. List of dff tokens to store "start of thermal" measurements for each pin. If left empty, this parameter is ignored. If this parameter is populated, the amount of tokens must match the amount of pins defined for your test instance.                                                                                                                                                                     |
| IntegrityHighLimit       | No            | Integer                | Celsius degrees (positive). Default = 200.                                                                                                                              | Maximum allowed temperature for a thermal measurement.                                                                                                                                                                                                                                                                                                                                                                                     |
| IntegrityLowLimit        | No            | Integer                | Celsius degrees (positive). Default = 130.                                                                                                                              | Minimum allowed temperature for a thermal measurement.                                                                                                                                                                                                                                                                                                                                                                                     |
| LowerTolerance           | Yes           | String                 | List of comma delimited integers in Celsius degrees (positive). String can also point to a user var containing tolerance values.                                        | List of integers corresponding to the lower allowed thermal tolerance for your pins. Define a single tolerance to be enabled for all pins, or multiple tolerances to define a unique tolerance for each pin defined by this test instance. If defining multiple tolerances, the amount must match the amount of pins defined in your test instance. You can also provide a string pointing to a user var containing your tolerance values. |
| MeasurementType          | No            | String (choice)        | SOT, TJ (**default**), CHECK_DIE_FORCE                                                                                                                                  |                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| PcsDatalogSelector       | No            | Comma separated string | Defines values used to represent each pin when printing "CHECK_DIE_FORCE" measurements to ituff. Must be of equal length to your PinNames parameter if this is defined. |                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| PinNames                 | Yes           | Comma separated string | Pins defined in your test program.                                                                                                                                      | Pins to take thermal measurements of.                                                                                                                                                                                                                                                                                                                                                                                                      |
| PinNamesExcluded         | No            | Comma separated string | Pins names to exclude.                                                                                                                                                  |                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| PreMeasurementLevelName  | No            | String                 | Name of the Level test Condition to be applied before the measurement.                                                                                                  |                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| RetriesDelay             | No            | Unsigned Integer       | Milliseconds as integer values. Default = 0.                                                                                                                            | The delay between retries in ms.                                                                                                                                                                                                                                                                                                                                                                                                           |
| RetriesMaxCount          | No            | Unsigned Integer       | Milliseconds as integer values. Default = 0.                                                                                                                            | Only for instances **NOT** in SOT mode. Maximum amount of times to re-measure the test instance's pins defined in PinNames in the case of a failing measurement on any pin.                                                                                                                                                                                                                                                                |
| StorageTag               | No            | String                 | A string to use as a key in SharedStorage.                                                                                                                              | A key to use to store this test instance's thermal measurements as a generic measurement in SharedStorage. Measurement can be later retrieved in your test program by calling Prime.Services.Thermal.TdauGetStoredMeasurementByTag()                                                                                                                                                                                                       |
| UpperTolerance           | Yes           | String                 | List of comma delimited integers in Celsius degrees (positive). String can also point to a user var containing tolerance values.                                        | List of integers corresponding to the upper allowed thermal tolerance for your pins. Define a single tolerance to be enabled for all pins, or multiple tolerances to define a unique tolerance for each pin defined by this test instance. If defining multiple tolerances, the amount must match the amount of pins defined in your test instance. You can also provide a string pointing to a user var containing your tolerance values. |
| UserVarStoreNames        | No            | String (list)          | Names for the user vars defined in your test program.                                                                                                                   | List of user vars to store measurements for each pin. If left empty, this parameter is ignored. If this parameter is populated, the amount of user vars must match the amount of pins defined for your test instance.                                                                                                                                                                                                                      |

**Notes:**
  - PcsDatalogSelector param is only used when in "CHECK_DIE_FORCE" mode
    - If this param is not defined, the default value for each pin will be its index in the pin in the PinNames parameter.
  - ContinuousRead param is only used when in "SOT" mode.
  - Expected format for the uservar names in UserVarStoreNames or the tolerance parameters: 
    - ```Module::Collection.UserVarName```
  - Upper/Lower tolerance values are only updated during Verify() time. If taking a value from a uservar, ensure the desired value is populated during this instance's Verify().
  - To print the **PCS_SOT_Thermal_Head_Temperature_Read** token in SORT (DTC) the uservar "SC_CHUCKID" must be defined.

## MeasurementType modes

The ThermalSingleMeasurement test method provides capability to perform a single measurement of the thermal pins provided by the user.

This test method provides 3 modes:

### Start of thermal (SOT)
Clear all stored measurements, perform initial measurements on defined pins, log measurements to ituff, and store thermal head temperature for PCS logging.

Also used for starting ContinuousRead if the test instance enables this feature.

### Thermal junction (TJ)
Perform a single measurement on defined pins, print the measurements to ituff, then store measurements for later PCS calculations when EOT is executed.

### Check Die Force (CHECK_DIE_FORCE)
Perform a single measurement on defined pins, then print it to ituff. Calculations are not stored for PCS calculations when EOT is executed.

## Limits
The test method defines thermal limits that, if exceeded, will cause the test method to exit out of ports other than 1.

- Low temperature Limit = set point - LowerTolerance. If a measurement is higher than this limit, but lower than high temperature limit, the test will fail and report 2 as exit port.
- High temperature Limit = set point + UpperTolerance. If a measurement is higher than this limit, but lower than the integrity limits, the test will fail and report 3 as exit port.
- If a measurement is higher than the IntegrityLowLimit, but lower than the IntegrityHighLimit, the test will fail and report 4 as exit port.
- If a measurement is higher than the IntegrityHighLimit, the test will fail and report 5 as exit port.

### Defining limits
For the test method to conform to the above behavior, you must define your limits as such:
```
Low temperature limits < High temperature limits < Low integrity limit < High integrity limit
```

## Retries
A test instance can be set to "retry" a measurement in the case of a failing pin measurement.

When performing retries, only the last measurement is accounted for each pin. The test method drops any measurement from previous failing retry.

# Datalog output

Each mode will print results to ituff. The following are examples for each mode:

## SOT
```
2\_tname\_TDAU\_CH\_PCH\_SotInstanceName
2\_mrslt\_100.3
2\_msunit\_C
2_tname_Thermal::SingleMeasurementSOT_P1_Thermal_Head_Read
2_mrslt_70.0
2_msunit_C
```

## TJ
```
2\_tname\_TDAU\_CH\_PCH\_TjInstanceName
2\_mrslt\_95.5
2\_msunit\_C
```

## CHECK_DIE_FORCE
```
2_tname_PCS_CHECKDIEFORCE0
2_mrslt_23.0
2_msunit_C
2_tname_PCS_CHECKDIEFORCE1
2_mrslt_23.1
2_msunit_C
```

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

**TPL Sample5: Only print measurements to ituff using CHECK_DIE_FORCE**
```python
Import PrimeThermalSingleMeasurementTestMethod.xml;

Test PrimeThermalSingleMeasurementTestMethod SOTMultiLimits_MultiChannel
{
   PinNames = "TDAU_CH_PCH,TDAU_CH_CORE,TDAU_CH_GT";
   MeasurementType = "CHECK_DIE_FORCE";
   UpperTolerance = "10";
   LowerTolerance = "10";
```

**TPL Sample6: Define index for each pin using DatalogSelector (CHECK_DIE_FORCE only)**
```python
Import PrimeThermalSingleMeasurementTestMethod.xml;

Test PrimeThermalSingleMeasurementTestMethod SOTMultiLimits_MultiChannel
{
   PinNames = "TDAU_CH_PCH,TDAU_CH_CORE,TDAU_CH_GT";
   MeasurementType = "CHECK_DIE_FORCE";
   UpperTolerance = "10";
   LowerTolerance = "10";
   PcsDatalogSelector = "98,2,3" # TDAU_CHN_PCH => 98, TDAU_CH_CORE => 2, TDAU_CH_GT => 3
}
```

## Exit Ports

The ThermalSingleMeasurement test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                   |
|---------------|---------------|-----------------------------------|
| **-2**        | ***Alarm***   | Any alarm condition               |
| **-1**        | ***Error***   | Any software condition error      |
| **0**         | ***Fail***    | Failing condition                 |
| **1**         | ***Pass***    | Passing condition                 |
| **2**         | ***Fail***    | Fails due to low limit            |
| **3**         | ***Fail***    | Fails due to high limit           |
| **4**         | ***Fail***    | Fails due to integrity low limit  |
| **5**         | ***Fail***    | Fails due to integrity high limit |

## Additional Dependencies

This test depends on SCOC calibration to be executed before.
When ContinuousRead mode is enabled, this test depends on PrimeThermalEndOfTestTestMethod to stop the data collection.

## Version tracking

| **Date**        | **Version** | **Author**                    | **Comments**                                                                                             |
|-----------------|-------------|-------------------------------|----------------------------------------------------------------------------------------------------------|
| Mar 3rd, 2020   | 1.0.0       | Alberto Suarez                | Initial document release                                                                                 |
| Mar 30th, 2020  | 1.0.1       | Alberto Suarez                | Added new parameter PcsDatalogSelector, changed from limits to range referenced to Temperature Set Point |
| May 4th, 2020   | 1.0.2       | Alberto Suarez                | Added offset and note for case when Range = 0                                                            |
| July 10th, 2020 | 2.0         | Johnny Ariel Mata             | Capability to store thermal measurements in uservars                                                     |
| Oct 7th, 2020   | 3.0         | Johnny Ariel Mata             | Adding tolerance parameters instead of range                                                             |
| Nov 5th, 2020   | 3.1         | Johnny Ariel Mata             | Adding integrity checks                                                                                  |
| Mar 24th, 2021  | 5.1         | Didier Armando Jiménez Retana | Added Dff capability                                                                                     |
| May 21th, 2021  | 5.1         | Johnny Ariel Mata             | Enabling Continuous read mode                                                                            |
| Mar 31th, 2022  | 9.0         | Maria N. Rojas                | Removed EOT mode                                                                                         |
| Aug 14st, 2024  | 13.1        | Caio Fernandes                | Update "CHECK_DIE_FORCE" behavior + updating document information.                                       |

## Acronyms

Definition of acronyms used in this document:

- **DTC**: Device Thermal controller.
- **EOT**: End Of Test. Usually refers to execution of PrimeEndOfTestTestMethod.
- **HDMT**: High Density Modular Tester.
- **PCS**: Process Control System. A methodology developed by the thermal team to determine the integrity of the tester.
- **SCOC**: Single current offset calibration.
- **SOT**: Start of Test.
- **TDAU**: Thermal Device Acquisition Unit. The equipment used to take thermal measurements on specified pins.
- **TJ**: Thermal Junction.
- **TPL**: Test Programming Language.