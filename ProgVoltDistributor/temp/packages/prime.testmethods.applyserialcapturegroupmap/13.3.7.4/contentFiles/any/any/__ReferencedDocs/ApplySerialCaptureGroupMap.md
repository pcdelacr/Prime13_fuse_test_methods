**prime Test-Method Specification REP**

Revision 1.0.0

Jan 2022

[[_TOC_]]

## REP for ApplySerialCaptureGroupMap

This **REP** is intended to describe the ApplySerialCaptureGroupMap Prime TestMethod.

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

The ApplySerialCaptureGroupMap test method provides capability to apply a serial capture group configuration. The group must be defined in the .scgm of the Test Program.

Serial Capture Stream (SCS) is a new generic method for capturing a data stream from the DUT to send to logging, for parametric logging. The first usage model is to capture DTS data. 
The SerialCaptureGroupMap is used to configure the UEI groups in FW for max/avg calculations sent over UEI.
These need to be applied before applying the Serial Capture attributes so RCTC FW knows which virtual SC pins to apply them to.


## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ApplySerialCaptureGroupMap test method

| **Parameter Name**    | **Required?** | **Type**        | **Values**                  | **Comments** |
| --------------------- | ------------- | --------------- | --------------------------- | ------------ |
| CaptureGroupMapName   | yes           | String          |                             | name of a group defined in the .scgm file. |

**Notes:**

  - The group must exists in the .scgm file of the Test Program.

## Datalog output

This test Method does not create a datalog

## Custom User Code Hooks

Here is the list of functions available to the user code to override.

\<TBD\>


## TPL Samples

Here are a few test instance examples using the ApplySerialCaptureGroupMap test method

TPL Sample1: \<applying the "Group1" configuration\>

```javascript
Import PrimeApplySerialCaptureGroupMapTestMethod.xml;

Test PrimeApplySerialCaptureGroupMapTestMethod ApplyGroup
{
   CaptureGroupMapName = "Group1";
}
```

## Exit Ports

The ApplySerialCaptureGroupMap test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

## Additional Dependencies

More dependencies to consider for this TestMethod to well operate:

  - Groups need to be applied before applying the Serial Capture attributes so RCTC FW knows which virtual SC pins to apply them to.

## Version tracking


| **Date**       | **Version** | **Author**     | **Comments** |
| -------------- | ----------- | -------------- | ------------ |
| Jan 12th, 2022 | 1.0.0       | Ariel Mata     | initial SCS support             |
|                |             |                |              |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System