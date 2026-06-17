[[_TOC_]]

## REP for PassBin To Sspec And FuseAtClassBin Converter

This **REP** is intended to describe the PassBin To Sspec And FuseAtClassBin Converter Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)
  
  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Acronyms** - Definition of acronyms used in this document
## Methodology

The PassBin To Sspec And FuseAtClassBin Converter test method provides capability to get the Sspec and Fuse at Class (or Fact Bin) that corresponds to a specific PassBin.
 
***Note:*** In order to use this test method, an instance with the Fuse At Class test method with FactMode diferent to Off must have run before.

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
BinToQdfAndFactBin(Execute Start) -->GetUserFuncParams[Read PassBin value and SspecBinsMapping from the shared storage]
--> GSDSinputPassBin[Search in the mapping object for the pass bin] --> passBinFound{Was the PassBin found?}
--> |No|PassBinNotFound[Throw exception]
passBinFound--> |Yes|SaveAssociatedValues[Save associated sspec and fact bin to the shared storage]--> PASS --> End(End)
style BinToQdfAndFactBin fill:#00AEEF,stroke:#0000,stroke-width:2px,color:#fff
style passBinFound fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style GSDSinputPassBin fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style SaveAssociatedValues fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style PassBinNotFound fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style GetUserFuncParams fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style End fill:#00AEEF,stroke:#0000,stroke-width:2px,color:#fff
style PASS fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
:::

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the PassBin To Sspec And FuseAtClassBin Converter test method:

| **Parameter Name** | **Required?** | **Type**        | **Values**                     | **Comments**                 |
| ------------------ | ------------- | --------------- | ------------------------------ | ---------------------------- |
| PassBinKey         | Yes           |  String         | Shared storage key where the PassBin is stored. | The shared storage context should always be "DUT".   |
| SspecBinsMappingObjectKey  | Yes   |  String         | Key of the SspecBinMapping to be used on the shared storage. |  |
| SspecBinsMappingObjectContext | Yes |  String        | Context of the SspecBinMapping to be used on the shared storage. | Allowed values: "LOT" or "DUT". |
| SspecKey           | Yes           |  String         | Shared storage key in which the Sspec should be stored. | It will be saved using the shared storage context "DUT". |
| FactBinKey         | Yes           |  String         | Shared storage key in which the Fuse At Class bin should be stored. | It will be saved using the shared storage context "DUT". |



## TPL Samples

Here are a few test instance examples using the PassBin To Sspec And FuseAtClassBin Converter test method:

```python
Import PrimePassBinToSspecAndFuseAtClassBinConverterTestMethod.xml;
Test PrimePassBinToSspecAndFuseAtClassBinConverterTestMethod PrimePassBinToSspecAndFuseAtClassBinConverterTestMethod_HappyPathUsingGeneratedObjectUnitLevel_P1
{
    SspecBinsMappingObjectContext = "DUT";
    LogLevel = "PRIME_DEBUG";
    SspecBinsMappingObjectKey = "MappingObjUnitKey";
    PassBinKey = "passBinUnitPassBinKey";
    FactBinKey = "factBinUnitPassBinKey";
    SspecKey = "SspecBinUnitPassBinKey";
}
```


## Exit Ports

The PassBin To Sspec And FuseAtClassBin Converter test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**                                                         |
| ------------- | ------------- | ------------------------------------------------------------------------|
| **0**         | ***Fail***    | Failing condition.                                                      |
| **1**         | ***Pass***    | Sspec and Fact Bin was found for PassBin stored in shared storage with the PassBinKey paramenter.          |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **FaCT**: **F**use **A**t **C**lass (**T**est)
## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**    |
| ------------------------- | ----------- | ----------------- | --------------- |
| Apr 29<sup>th</sup>, 2022 | 1.0.0       | Andrea Gomez      | Initial version |
