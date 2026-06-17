[[_TOC_]]

## REP for Sspec to FuseAtClassBin Converter

This **REP** is intended to describe the Sspec to FuseAtClassBin Converter Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)
  
  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Acronyms** - Definition of acronyms used in this document
## Methodology

The Sspec to FuseAtClassBin Converter test method provides capability to get the Fuse at Class that corresponds to a specific Sspec.
 
***Note:*** An instance of PrimeFuseAtClassTestMethod with "FactMode" set to a mode other than "Off" must have executed in your test program flow before executing an instance of this test class.


### Verify
::: mermaid
graph TB
style Verify fill:#00AEEF,stroke:#0000,stroke-width:2px,color:#fff
style End fill:#00AEEF,stroke:#0000,stroke-width:2px,color:#fff
style ParseContext fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
Verify(Verify Start) --> ParseContext[Validate that the shared storage<br>keys contain the Context.] --> End(End)
:::

### Execute
::: mermaid
graph TB
QdfToFactBin(Execute Start) -->GetUserFuncParams[Read Sspec value and SspecBinsMapping from the shared storage]
--> GSDSinputPassBin[Search in the mapping object for the sspec] --> passBinFound{Was the Sspec found?}
--> |No|PassBinNotFound[Throw exception]
passBinFound--> |Yes|SaveAssociatedValues[Save the associated fact bin to the shared storage]--> PASS --> End(End)
style QdfToFactBin fill:#00AEEF,stroke:#0000,stroke-width:2px,color:#fff
style passBinFound fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style GSDSinputPassBin fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style SaveAssociatedValues fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style PassBinNotFound fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style GetUserFuncParams fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style End fill:#00AEEF,stroke:#0000,stroke-width:2px,color:#fff
style PASS fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
:::

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Sspec to FuseAtClassBin Converter test method:

| **Parameter Name** | **Required?** | **Type**        | **Values**                     | **Comments**                 |
| ------------------ | ------------- | --------------- | ------------------------------ | ---------------------------- |
| SspecKey  | Yes           |  String         | Shared storage key where the Sspec is stored. | The shared storage context should always be "DUT".   |
| SspecBinsMappingKey  | Yes           |  String         | Shared storage key where the SspecBinsMapping object, which is a comma-delimited string of "passBin:sspec:factBin" formatted elements, is stored. | Allowed values: "LOT" or "DUT".   |
| SspecBinsMappingObjectContext | Yes |  String        | Context of the SspecBinMapping to be used on the shared storage. | Allowed values: "LOT" or "DUT". |
| FactBinKey  | Yes      |  String         | Shared storage key in which the Fuse At Class bin should be stored. | It will be saved using the shared storage context "DUT". |



## TPL Samples

Here are a few test instance examples using the Sspec to FuseAtClassBin Converter test method:

```python
Import PrimeSspecToFuseAtClassBinConverterTestMethod.xml;
Test PrimeSspecToFuseAtClassBinConverterTestMethod PrimeSspecToFuseAtClassBinConverterTestMethod_HappyPathUsingGeneratedObjectUnitLevel_P1
{
    FactBinKey = "factBinUnitSspecKey";
    SspecKey = "sspecUnitSspecKey";
    SspecBinsMappingObjectContext = "DUT";
    LogLevel = "PRIME_DEBUG";
    SspecBinsMappingObjectKey = "MappingObjUnitKey";
}
```


## Exit Ports

The Sspec to FuseAtClassBin Converter test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**                                                         |
| ------------- | ------------- | ------------------------------------------------------------------------|
| **0**         | ***Fail***    | Failing condition.                                                      |
| **1**         | ***Pass***    | Sspec and Fact Bin were found for PassBin stored in shared storage with the PassBinKey parameter.          |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **FaCT**: **F**use **A**t **C**lass (**T**est)
## Version tracking

| **Date**                  | **Version**   | **Author**           | **Comments**              |
| ------------------------- | ------------- | -------------------- | ------------------------- |
| Apr 29<sup>th</sup>, 2022 | 1.0.0         | Andrea Gomez         | Initial version           |
| Aug 22<sup>nd</sup>, 2024 | 14.00.00      | Raquel Pinto Rosales | Documentation enhancement |
