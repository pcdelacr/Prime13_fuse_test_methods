**prime Test-Method Specification REP**

Revision 1.0.0

Jul 2022

[[_TOC_]]

## REP for PowerPremonitionResponseInit

This **REP** is intended to describe the PowerPremonitionResponseInit Prime TestMethod.

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

## <span style="color:#B03A2E">Power Premonition Response Methodology is still under development.
## <span style="color:#B03A2E">This is an initial draft for the PPR documentation, for futher information about the methodolgy please submit a Prime Ticket on goto/primetickets.

<br/>

## Methodology

The PowerPremonitionResponseInit along with the PowerPremonitionDeviceStart enables the PPR methodology from Primes perspectivetest.
This Test Method is responsible of tagging the test instances that are relevant for the PPR methodology. For the tagged instances, power and temperature data will be collected and send to the PPR algorithm (external to Prime) via PPR PUDL stream   


## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the PowerPremonitionResponseInit test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                 | **Comments**    |
| ------------------ | ------------- | --------------- | -------------------------- | --------------- |
| ProfileInstanceFilePath| Yes       | String          | Path to a file             |                 |
| SerialCaptureDtsProfile| No        | String (choice) | ENABLED                    |  Add DTS data to the PPR stream. Needs special pattern configuration|
|                    |               |                 | DISBALED (**default**)     |                 |
| ThermalController  | No            | String (choice) | DTC                        |  Used to differentiate between CLASS and SORT operations|
|                    |               |                 | TCC (**default**)          |                 |


**Configuration File (ProfileInstanceFile) Example**

```json
{
  "DefaultTolerances": {
    "UTT": 15.0,
    "LTT": 15.0
  },
  "Rjh": 0.25,
  "PowerScalingFactor": 1.0,
  "PowerScalingMultiplierUserVarName": "userVarName"
  "TestInstance2Tolerances": {
    "PPR::EvgPause_P1": [
      {
      "UTT": 3.0,
      "LTT": 5.0,
      "PowerSumPinName": "POWERSUM_09",
      "UeiPinName": "UVS_100",
      "TtrMode": [
          "MONITOR",
          "FULL",
          "SHORT"
        ]
    },
    {
      "UTT": 3.0,
      "LTT": 5.0,
      "PowerSumPinName": "POWERSUM_07",
      "UeiPinName": "UVS_100",
      "TtrMode": [
          "MONITOR",
          "FULL",
          "SHORT"
        ]
    }
    ],
    "Thermal::SingleMeasurementSOT_P1": [
      {
      "UTT": 3.0,
      "LTT": 5.0,
      "PowerSumPinName": "POWERSUM_09",
      "UeiPinName": "UVS_100"
    }],
    "Thermal::ThermalRamp_MultiChannel_P1": [{
      "UTT": 3.0,
      "LTT": 5.0,
      "PowerSumPinName": "POWERSUM_09",
      "UeiPinName": "UVS_100"
    }
    ]
  }
}
```

**Description**
PowerScalingFactor: Gets or sets the scale to modify the power seen by the tester.
PowerScalingMultiplierUserVarName: Gets or sets the SC UserVariable that contains a value to multiply with the PowerScalingFactor.
TestInstance2Tolerances: Gets or sets Dictionary mapping the name of the tagged instances to a pin configuration.


**Notes:**
Each Instance in the configuration file must be indicated by its module and the instance name as Module::InstanceName, otherwise an error will be thrown.

## Datalog output


## Custom User Code Hooks

TBD

## TPL Samples

Here are a few test instance examples using the ThermalSingleMeasurement test method

**TPL Sample1: SOT Multiple Limits with Multiple Channels**

```python
Import PrimePowerPremonitionResponseInitTestMethod.xml;

Test PrimePowerPremonitionResponseInitTestMethod AtivatePpr_P1
{
   ProfileInstanceFilePath = "~HDMT_TPL_DIR/Modules/thermal/THERMAL/InputFiles/PprConfiguration.json";
   ThermalController = "DTC";
   SerialCaptureDtsProfile = "DISABLED";
   LogLevel = "PRIME_DEBUG";
}
```


## Exit Ports

The PrimePowerPremonitionResponseInitTestMethod test method supports the following exit
ports:

| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition          |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |

## Additional Dependencies

PPR algorith that is external to Prime code 

## Version tracking

| **Date**                   | **Version** | **Author**        | **Comments**                                                                                             |
| -------------------------- | ----------- | ----------------- | -------------------------------------------------------------------------------------------------------- |
| Jul 15th, 2022             | Prime 11    | Johnny Mata       | Initial document release                                                                                 |
| Oct 10th, 2022             | Prime 12    | Maria Rojas       | Added verification for module prefix in the instances in the configuration file                         |


## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **PPR**: Power Premonition Response
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TCC**: Thermal Controller Card
  - **DTC**: Device Thermal controller