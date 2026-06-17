**prime Test-Method Specification REP**

Revision 1.0.0

Nov 2021

[[_TOC_]]

## REP for ParticipatingDutLogger

This **REP** is intended to describe the ParticipatingDutLogger Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Datalog output** – A detailed description of what is datalogged by his TestMethod

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document 

## Methodology

The ParticipatingDutLogger test method prints the number of participating DUTs to the ituff.

This test method defines the number of participating DUTs as $defined DUTs - (disabled DUTs + on hold DUTs)$

## Test Instance Parameters

This test method does not have parameters.

## Datalog output

```
2_tname_<TestInstanceName>
2_strgval_<ParticipatingDuts>
```

## Custom User Code Hooks

This test method does not have user code hooks.


## TPL Samples

Here are a few test instance examples using the ParticipatingDutLogger test method:

```javascript
Import PrimeParticipatingDutLoggerTestMethod.xml;

Test PrimeParticipatingDutLoggerTestMethod ParticipatingDutLoggerOnAGivenPointOfTheFlow
{
}
```

## Exit Ports

The ParticipatingDutLogger test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

## Additional Dependencies

More dependencies to consider for this TestMethod to well operate:

## Version tracking


| **Date**       | **Version** | **Author**     | **Comments** |
| -------------- | ----------- | -------------- | ------------ |
| Nov 5th, 2021  | 1.0.0       | lchavarr       |              |
|                |             |                |              |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System