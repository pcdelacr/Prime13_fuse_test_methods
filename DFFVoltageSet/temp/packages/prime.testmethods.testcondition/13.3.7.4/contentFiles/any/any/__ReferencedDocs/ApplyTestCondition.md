**prime Test-Method Specification REP**

Revision 1.0.0

Jul 2020

[[_TOC_]]

## REP for ApplyTestCondition

This **REP** is intended to describe the ApplyTestCondition Prime TestMethod.

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

The ApplyTestCondition test method provides capability to apply a defined test condition to the tester.

This is a new feature enabled for Prime, which is making use of the feature provided by TOS in order to apply a set of pre-defined configurations that are already available on the tester. This is an important set towards getting rid of redundant configuration parameters given previously to the test configuration files by the test plan.
This test method during verify time will check that provided Test Condition name does exists in the tester, throwing an error if it is not found. Then during Execute phase it will send the command to the tester to apply the test condition.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ApplyTestCondition test method

| **Parameter Name**    | **Required?** | **Type**        | **Values**                  | **Comments** |
| --------------------- | ------------- | --------------- | --------------------------- | ------------ |
| TestConditionCategory | Yes           | String (choice) | LEVELS\_SETUP (**default**) |              |
|                       |               |                 | LEVELS\_POWER\_ON           |              |
|                       |               |                 | TIMING                      |              |
|                       |               |                 | RELAY                       |              |
|                       |               |                 | THERMAL                     |              |
| TestConditionName     | Yes           | String          | Valid Test Condition Name   |              |
| AlarmPortRedirect     | No            | String (choice) | DISABLED  (**default**)     | Default alarm port=[-2] behavior. |
|                       |               |                 | ENABLED                     | Enable the alarm port redirect to port=[2]. |

**Notes:**

  - When TestConditionCategory == LEVELS\_POWER\_ON the test method will update the value for PowerUpTCName

## Datalog output

This test Method does not create a datalog

## Custom User Code Hooks

Here is the list of functions available to the user code to override.

\<TBD\>


## TPL Samples

Here are a few test instance examples using the ApplyTestCondition test method

TPL Sample1: \<Setting a power summing test condition\>

```javascript
Import PrimeApplyTestConditionTestMethod.xml;

Test PrimeApplyTestConditionTestMethod ApplyTestConditionPowerSumm
{
   TestConditionName = "PowerSumming_TC";
   TestConditionCategory = "THERMAL";
}
```

## Exit Ports

The ApplyTestCondition test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |
| **2**         | ***Alarm***     | Any alarm condition if the AlarmPortRedirect is enabled. |

## Additional Dependencies

More dependencies to consider for this TestMethod to well operate:

  - All provided test condition names must be valid test condition names, just check they are the same as the defined test conditions     on the tester

## Version tracking


| **Date**       | **Version** | **Author**     | **Comments** |
| -------------- | ----------- | -------------- | ------------ |
| Mar 19th, 2020 | 1.0.0       | Alberto Suarez |              |
|                |             |                |              |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System