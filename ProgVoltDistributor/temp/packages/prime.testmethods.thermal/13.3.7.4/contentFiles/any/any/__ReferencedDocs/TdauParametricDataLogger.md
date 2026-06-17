**prime Test-Method Specification REP**

Revision 1.0.1

Mar 2021

[[_TOC_]]

## REP for TdauParametricDataLogger

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

## Methodology

The TdauParametricDataLogger test method provides capability to retract parametric voltage/current data from TDAU pins or pin groups (provided by the user), and to log that information to the Ituff.

**Parametric Data Attributes:**

| **Attribute** | **Description** |
| ------------------ | ------------- |
| **PinName:**       | Name of the pin.           |
| **Temperature:**       | Latest temperature reading of the TDAU channel.           |
| **ible1:**       | Base current at force current 1.           |
| **ible2:**       | Base current at force current 2.           |
| **ible3:**       | Base current at force current 3.           |
| **ibLeakageIe1:**       | Base current leakage at force current 1.           |
| **ibLeakageIe2:**       | Base current leakage at force current 2.           |
| **ibLeakageIe3:**       | Base current leakage at force current 3.           |
| **ieLeakageIe1:**       | Emitter current leakage at force current 1.           |
| **ieLeakageIe2:**       | Emitter current leakage at force current 2.            |
| **ieLeakageIe3:**       | Emitter current leakage at force current 3.            |
| **ieValIe1:**       | Emitter current at force current 1.           |
| **ieValIe2:**       | Emitter current at force current 2.          |
| **ieValIe3:**       | Emitter current at force current 3.           |
| **vbele1:**       | Emitter base voltage at force current 1.           |
| **vbele2:**       | Emitter base voltage at force current 2.           |
| **vbele3:**       | Emitter base voltage at force current 3.           |

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the TdauParametricDataLogger test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                 | **Comments**    |
| ------------------ | ------------- | --------------- | -------------------------- | --------------- |
| PinNames           | Yes           | String (list)   | name of TDAU pins          | Comma separated |
| DffTokens          | No            | String (list)   | name of Dff tokens per pin | Comma separated |

## DFF capability:
The DffTokens parameter on the ThermalParametricDataLogger exposes the capability to save the `TdauParametricDataLogger::vbeIe3` for each pin.
This parameter is optional, and when it's provided, it must be a comma-separated list of DFF tokens, one per pin.

**Notes:**

## Datalog output

Example for single pin output:
```
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_VbeIe1
2_mrslt_0.000010
2_msunit_V
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_VbeIe2
2_mrslt_0.000020
2_msunit_V
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_VbeIe3
2_mrslt_0.000040
2_msunit_V
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_IbIe1
2_mrslt_0.000009700
2_msunit_A
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_IbIe2
2_mrslt_0.000019700
2_msunit_A
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_IbIe3
2_mrslt_0.000039700
2_msunit_A
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_Ie_Leakage_Ie1
2_mrslt_0.000210000
2_msunit_A
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_Ie_Leakage_Ie2
2_mrslt_0.000220000
2_msunit_A
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_Ie_Leakage_Ie3
2_mrslt_0.000230000
2_msunit_A
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_Ib_Leakage_Ie1
2_mrslt_0.000002100
2_msunit_A
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_Ib_Leakage_Ie2
2_mrslt_0.000002200
2_msunit_A
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_Ib_Leakage_Ie3
2_mrslt_0.000002300
2_msunit_A
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_Ie_Val_Ie1
2_mrslt_0.002100000
2_msunit_A
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_Ie_Val_Ie2
2_mrslt_0.002200000
2_msunit_A
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_Ie_Val_Ie3
2_mrslt_0.002300000
2_msunit_A
2_tname_Thermal::SinglePinParametricDataLogger_P1_TDAU_CH_PCH_Temperature
2_mrslt_23.100000381
2_msunit_C
```

## TPL Samples

Here are a few test instance examples using the TdauParametricDataLogger test method

**TPL Sample1: Single pin data**

```python
Import PrimeTdauParametricDataLoggerTestMethod.xml;

Test PrimeTdauParametricDataLoggerTestMethod SinglePinParametricDataLogger_P1
{
   PinNames = "TDAU_CH_PCH";
}
```

**TPL Sample2: Multiple pin data**

```python
Import PrimeTdauParametricDataLoggerTestMethod.xml;

Test PrimeTdauParametricDataLoggerTestMethod SinglePinParametricDataLogger_P1
{
   PinNames = "TDAU_CH_PCH, TDAU_CH_GT, TDAU_CH_CORE";
}
```
**TPL Sample3: Multiple pin data and DffTokens**

```python
Import PrimeTdauParametricDataLoggerTestMethod.xml;

Test PrimeTdauParametricDataLoggerTestMethod SinglePinParametricDataLogger_WithDffCapability_P1
{
   PinNames = "TDAU_CH_PCH, TDAU_CH_GT, TDAU_CH_CORE";
   DffTokens = "dffToken1,dffToken2,dffToken3";
}
```

## Exit Ports

The TdauParametricDataLogger test method supports the following exit
ports:

| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-2**        | ***Alarm***   | Any hardware alarm condition          |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |


## Additional Dependencies

This test depends on SCOC calibration to be executed before

## Version tracking

| **Date**                   | **Version** | **Author**                    | **Comments**                                                                                             |
| -------------------------- | ----------- | ----------------------------- | --------------------------------------------------
| Oct 8<sup>th</sup>, 2020   | 1.0.0       | Johnny Ariel Mata             | Initial document release
| Mar 24<sup>th</sup>, 2021  | 1.0.1       | Didier Armando Jiménez Retana | Added Dff capability
## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TDAU**: Thermal Diode Acquisition Unit
  - **TPL**: Test Programming Language
  - **SCOC**: Single current offset calibration