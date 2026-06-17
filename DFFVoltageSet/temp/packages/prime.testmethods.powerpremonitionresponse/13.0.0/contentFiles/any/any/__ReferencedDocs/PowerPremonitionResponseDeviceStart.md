**prime Test-Method Specification REP**

Revision 1.0.0

Oct 2022

[[_TOC_]]

## REP for PowerPremonitionResponseDeviceStart

This **REP** is intended to describe the PowerPremonitionResponseDeviceStart Prime TestMethod.

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

## <span style="color:#B03A2E">Power Premonition Response Methodology is still under development. More information can be found in Wiki--->Services--->PowerPremonitionResponseService
## <span style="color:#B03A2E">This is an initial draft for the PPR documentation, for futher information about the methodolgy please submit a Prime Ticket on goto/primetickets.

<br/>

## Methodology

This Test Method is responsible of parsing the PPR Look Up Table (LUT). The LUT contains the configuration for the thermal control actions that will be taken per test instance (the ones that were tagged during PowerPremonitionResponceInit).
The PowerPremonitionResponseInit along with the PowerPremonitionDeviceStart TMs enable the PPR methodology from PRIME perspective,
both test instances must be executed to activate the PowerPremonitionService methods that run in the "background" before and after every instance.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the PowerPremonitionResponseInit test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                 | **Comments**    |
| ------------------ | ------------- | --------------- | -------------------------- | --------------- |
| ValidLutTimeOut    |     No        | int             |                            |seconds to wait for a valid LUT|
| ThermalController  |     No        | String (choice) | DTC                        |  Used to differentiate between CLASS and SORT operations|
|                    |               |                 | TCC (**default**)          |                 |


## Look Up Table (LUT)

The LUT is a configuration "file" that is created in the DPC (Data Path Controller) by the PPRA (Algorithm external to PRIME). 
It is sent to PRIME through the uservar called **SC_PPR_LUT_CONTENT** in JSON format. The uservar is updated by the PPRA for every DUT during TESTPLANSTART flow.
The information in the LUT is used by PRIME to configure the PPR actions that will be taken per TI for the current DUT.

**LUT Example**

```json
{
  "LutTimeStamp": "20210716T185605732",
  "PreEmphasisTimeOut": 2,
  "PprControlSet": "PPR",
  "ActionDefinition": [
    {
      "TestInstanceName": "Thermal::ThermalRamp_MultiChannel_P1",
      "PowerScalingFactor": 1.0,
      "TemperatureOffset": 5,
      "ReleaseTemperatureUpperTolerance": 53,
      "ReleaseTemperatureLowerTolerance": 3,
      "PowerSurgeTime": 0,
      "DesiredTestTemperatureScUserGlobal": "SC_TEMPERATURE"
    },
    {
      "TestInstanceName": "ARR_CORE::LSA_CORE_VMIN_K_CHKCRF4_080816_VCORE_F4_4100_ALL_RF_1005",
      "PowerScalingFactor": 1.5384126089885455,
      "TemperatureOffset": 27,
      "ReleaseTemperatureUpperTolerance": 8,
      "ReleaseTemperatureLowerTolerance": 3,
      "PowerSurgeTime": 212,
      "DesiredTestTemperatureScUserGlobal": "SC_TEMPERATURE"
    }
  ]
}
```


**Notes:**
1. An instance of the PowerPremonitionDeviceStart TM should be placed during TESTPLANSTART to parse the updated uservar for every DUT.
2. the purpose of this TM is just configuration. No thermal control actions happend during its execution.
## Datalog output


## Custom User Code Hooks

TBD

## TPL Samples

Here are a few test instance examples using the PrimePowerPremonitionResponseDeviceStart test method

**TPL Sample1: CLASS **

```python
Import PrimePowerPremonitionResponseDeviceStartTestMethod.xml;

Test PrimePowerPremonitionResponseDeviceStartTestMethod ReadPprLut_P1
{
   ThermalController = "TCC";
   LogLevel = "PRIME_DEBUG";
}
```


## Exit Ports

The PrimePowerPremonitionResponseDeviceStartTestMethod test method supports the following exit
ports:

| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition          |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |

## Additional Dependencies

PowerPremonitionResponse Algorith (PPRA) that is external to Prime code 

## Version tracking

| **Date**                   | **Version** | **Author**        | **Comments**                                                                                             |
| -------------------------- | ----------- | ----------------- | -------------------------------------------------------------------------------------------------------- |
| Sep 15th, 2022             | Prime 11    | Johnny Mata       | Initial document release                                                                                 |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **PPR**: Power Premonition Response
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TCC**: Thermal Controller Card
  - **DTC**: Device Thermal controller
  - **PPRA**: Power Premonition Response Algorithm
  - **DPC**: Data Path controller
  - **LUT**: Look Up Table