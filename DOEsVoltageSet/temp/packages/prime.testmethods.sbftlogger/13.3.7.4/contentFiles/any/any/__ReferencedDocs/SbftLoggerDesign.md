[[_TOC_]]

## REP for ArrayFusing

This **REP** is intended to describe the ArrayFusing Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Console output** – A detailed description of what is printed to console by this TestMethod

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Acronyms** - Definition of acronyms used in this document

  - **Version tracking** – With author names, so you always have a name to address
  
## Methodology

The SbftLogger Design test method provides the capability to set the design information to be used in the SbftLogger Execute Test Method. The only function of this Test Method is to save design information in Shared Storage within the LOT Context. 

This Test Method will need to be run each time a new design information is required.

## Exit Ports

The test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                              |
| ------------- | ------------- | -------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                          |
| **-1**        | ***Error***   | Any software condition error                 |
| **0**         | ***Fail***    | Failing condition.                           |
| **1**         | ***Pass***    | Passing condition                            |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TPL**: Test Programming Language


## Version tracking

| **Date**                  | **Version** | **Author**           | **Comments**    |
| ------------------------- | ----------- | -------------------- | --------------- |
| Apr 27<sup>th</sup>, 2022 | 1.0.0       | Chun-Yu (Joseph) Yang| Initial version |

