**prime Test-Method Specification REP**

Revision 11.0.0

Jul 2022

[[_TOC_]]

## REP for ThermalControlSet

This **REP** is intended to describe the ThermalControlSet Prime TestMethod.

## Methodology

The ThermalControlSet test method applies the index of the control set name provided on the *ControlSet* parameter, based on the information previously read by ThermalControlSet Aleph Initialization.

<p style="font-size: 20px;"><span style="color: #cf6679;">For more details about the configuration file please check the ThermalControlSetService wiki page.</span></p>

The mapping between indexes and control sets should be previously loaded to memory  during Init.
The TemperatureSet configuration should be previously selected by the ThermalControlSetInit TestMethod.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ThermalControlSet test method

| **Parameter Name** | **Required** | **Type** | **Values**                                              | **Comments**                                                                                                    |
| ------------------ | ------------ | -------- |---------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------|
| ControlSet         | Yes          | String   | Name of the control set                                 | Must exist in the configuration file and have been set on the ControlSetInitTestMethod                          |
| UeiPinName         | Yes          | String   | Name of the pin in which the ControlSet will be applied | Must exist in the Aleph Init configuration file and it can be of type UeiClientPort0 or UeiClientPort0Multizone |

**Notes:**

1. Before calling any instance of this Test Method the ControlSetConfiguration file should be loaded during Aleph and a TemperatureSet selected using the ThermalControlSetInit TestMethod.
  
## Datalog output
The ControlSet mode is printed to the ituff.

```
CSharpTest PrimeThermalControlSetTestMethod ControlSetApply_Pass_P1
{
   ControlSet = "DISPENSE";
   UeiPinName = "UVS_100";
}
```
```
2_tname_ThermalControlSet::ControlSetApply_Pass_P1_ControlSetApply
2_strgval_DISPENSE
```
The relevant information is printed to the console.

## TPL Samples

Here are a few test instance examples using the ThermalControlSet test method

**TPL Sample1: Regular call to load an IndexTag named Dispense**

```python
Import PrimeThermalControlSetTestMethod.xml;

Test PrimeThermalControlSetTestMethod ControlSetApplyDispense
{
    ControlSet = "DISPENSE";
    UeiPinName = "IPA::PIN1";
}
```

## Exit Ports

The ThermalControlSet test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition          |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |

## Additional Dependencies

This test method only depends on a previously successful execution of the ThermalControlSetInit test method, and the data read and set into the ThermalService by it.

## Version tracking


| **Date**       | **Version** | **Author**     | **Comments**                       |
| -------------- | ----------- | -------------- | ---------------------------------- |
| Mar 5th, 2020  | 1.0.0       | Alberto Suarez | Initial document released          |
| Mar 30th, 2020 | 1.0.1       | Alberto Suarez | Updated based on review’s feedback |
| Jul 19th, 2022  | 11.0.0      | Johnny Mata | Multizone support |

## Acronyms

Definition of acronyms used in this document:

  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TDAU:** Thermal diode acquisition unit