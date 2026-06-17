**prime Test-Method Specification REP**

Revision 11.0.0

Jul 2022

[[_TOC_]]

## REP for ThermalControlSet

This **REP** is intended to describe the ThermalControlSet Prime TestMethod.

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

The ThermalControlSet test method applies the index of the control set name provided on the *ControlSet* parameter, based on the information previously read by ThermalControlSet Aleph Initialization.

<p style="font-size: 20px;"><span style="color: #cf6679;">For more details about the configuration file please check the ThermalControlSetService wiki page.</span></p>

The mapping between indexes and control sets should be previously loaded to memory  during Init.
The TemperatureSet configuration should be previously selected by the ThermalControlSetInit TestMethod.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ThermalControlSet test method

| **Parameter Name** | **Required** | **Type** | **Values**              | **Comments**                                                                     |
| ------------------ | ------------ | -------- | ----------------------- | -------------------------------------------------------------------------------- |
| ControlSet         | Yes          | String   | Name of the control set | Must exists in the configuration file and set selected on the Init’s test method |
| UeiPinName         | Yes          | String   | Name of the pin in which the ControlSet will be applied | Must exists in the Aleph Init configuration file and it can be of type UeiClientPort0 or UeiClientPort0Multizone |

**Notes:**

1. Before calling any instance of this Test Method the ControlSetConfiguration file should be loaded during Aleph and a TemperatureSet selected using the ThermalControlSetInit TestMethod.
  
## Datalog output

This test method does not print any information to ituff nor datalog. It
only prints relevant information to the console.


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

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TDAU:** Thermal diode acquisition unit