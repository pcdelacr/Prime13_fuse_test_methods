**prime Test-Method Specification REP**

Revision 1.0.0

Oct 2022

[[_TOC_]]

## REP for PowerPremonitionResponseDeviceStart

This **REP** is intended to describe the PowerPremonitionResponseDeviceStart Prime TestMethod.

## Methodology

This Test Method is responsible for parsing the PPR Look Up Table (LUT). The LUT contains the configuration for the thermal control actions that will be taken per test instance (the ones that were tagged during PowerPremonitionResponseInit).
The PowerPremonitionResponseInit along with the PowerPremonitionDeviceStart TMs enable the PPR methodology from PRIME perspective,
both test instances must be executed to activate the PowerPremonitionService methods that run in the "background" before and after every instance.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the PowerPremonitionResponseInit test method

| **Parameter Name**   | **Required?** | **Type**        | **Values**          | **Comments**                                            |
|----------------------|:-------------:|-----------------|---------------------|---------------------------------------------------------|
| ValidLutTimeOut      |      No       | int             |                     | Seconds to wait for a valid LUT                         |
| ThermalController    |      No       | String (choice) | DTC                 | Used to differentiate between CLASS and SORT operations |
|                      |               |                 | TCC (**default**)   |                                                         |
| IsProfileModeEnabled |      No       | String (choice) | False (**default**) |                                                         |
|                      |               |                 | True                |                                                         |


## Look Up Table (LUT)

The LUT is a configuration "file" that is created in the DPC (Data Path Controller) by the PPRA (Algorithm external to PRIME). 
It is sent to PRIME through the uservar called **SC_PPR_LUT_CONTENT** in JSON format. The uservar is updated by the PPRA for every DUT during TESTPLANSTART flow.
The information in the LUT is used by PRIME to configure the PPR actions that will be taken per TI for the current DUT.

**LUT Example**

```json
{
  "LutTimeStamp": "20210716T185605732",
  "PreEmphasisTimeOut": 2,
  "PostEmphasisTimeOut": 3,
  "PprControlSet": "PPR",
  "ActionDefinition": [
    {
      "TestInstanceName": "PowerPremonitionResponse::PauseTest_OneSecond_P1",
      "TTRActions": [
        {
          "TtrMode": [
            "FULL",
            "SHORT"
          ],
          "PowerScalingFactor": 1,
          "TemperatureOffset": 5,
          "ReleaseTemperatureUpperTolerance": 53,
          "ReleaseTemperatureLowerTolerance": 3,
          "PowerSurgeTime": 0,
          "DesiredTestTemperatureScUserGlobal": "SC_TEMPERATURE",
          "PreEmphasisTimeOutOverride": 2,
          "PostEmphasisTimeOutOverride": 3
        },
        {
          "TtrMode": [
            "MONITOR"
          ],
          "PowerScalingFactor": 7,
          "TemperatureOffset": 5,
          "ReleaseTemperatureUpperTolerance": 53,
          "ReleaseTemperatureLowerTolerance": 3,
          "PowerSurgeTime": 90,
          "DesiredTestTemperatureScUserGlobal": "SC_TEMPERATURE",
          "PreEmphasisTimeOutOverride": 2
        }
      ]
    },
    {
      "TestInstanceName": "ARR_CORE::LSAMLC_X_CR_VMAX_K_VMAXIAFX_0800_STATETAGLRULMAX_X",
      "TTRActions": [
        {
          "TtrMode": [
            "FULL"
          ],
          "PowerScalingFactor": 1.5384126089885455,
          "TemperatureOffset": 27,
          "ReleaseTemperatureUpperTolerance": 8,
          "ReleaseTemperatureLowerTolerance": 3,
          "PowerSurgeTime": 212,
          "DesiredTestTemperatureScUserGlobal": "SC_TEMPERATURE",
          "PreEmphasisTimeOutOverride": 2
        }
      ]
    }
  ]
}
```


**Notes:**
1. An instance of the PowerPremonitionDeviceStart TM should be placed during TESTPLANSTART to parse the updated uservar for every DUT.
2. The purpose of this TM is just configuration. No thermal control actions take place during its execution.

## PPR stream output

Valid Look up table
```
2_comnt_validLut
2_comnt_luttimestamp_20241028T200421107
```

Invalid Look up table
```
2_comnt_InvalidLut
```

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

PowerPremonitionResponse Algorithm (PPRA) that is external to Prime code 

## Version tracking

| **Date**                   | **Version** | **Author**        | **Comments**               |
| -------------------------- | ----------- | ----------------- |----------------------------|
| Sep 15th, 2022             | Prime 11    | Johnny Mata       | Initial document release   |

## Acronyms

Definition of acronyms used in this document:

  - **DPC**: Data Path controller
  - **DTC**: Device Thermal controller
  - **HDMT**: High Density Modular Tester
  - **LUT**: Look Up Table
  - **PPRA**: Power Premonition Response Algorithm
  - **PPR**: Power Premonition Response
  - **TCC**: Thermal Controller Card
  - **TPL**: Test Programming Language