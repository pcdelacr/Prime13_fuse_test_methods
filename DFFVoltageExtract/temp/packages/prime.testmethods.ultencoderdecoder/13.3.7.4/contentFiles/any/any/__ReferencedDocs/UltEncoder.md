**prime Test-Method Specification REP**

Revision 1.0.1

October 2021

[[_TOC_]]

## REP for UltEncoder

This **REP** is intended to describe the UltEncoder Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describing each instance parameter (Name, Type, Default, Required?)

  - **Datalog output** – A detailed description of what is data logged by this TestMethod

  - **Custom User Code hooks** – A list of functions available to the user code to override

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describing each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document

## Methodology

This test method is intended to encode the lot number, wafer number and location X and Y of the Die Id into a chain of 0's and 1's saved in user var.

The UltEncoder test method provides the capability to perform encoding from DFF, a set of four user vars or standards CTSCVars collection with Lot number, Wafer, X Location, and Y Location. 

This method allows you to encode  the lot number, wafer number and location X and Y  for 50, 56 and 64 bits.

When you use 50 bits count to encode the ULT, it is done with the following conversion:

![image-1](.attachments\UltEncoder\image-1.png)

When you use 56 bits count to encode the ULT. it is done with the following conversion:

![image-2](.attachments\UltEncoder\image-2.png)

When you use 64 bits count to encode the ULT it is done with the following conversion:

![image-3](.attachments\UltEncoder\image-3.png)
Intel Universal decoding.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the UltEncoder test method

| **Parameter Name**    | **Required?** | **Type** | **Values**                                                   | **Comments**                                                 |
| --------------------- | ------------- | -------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| BitCount              | Yes           | Integer  | Number of expected bits to capture. Currently, the supported values are 50, 56, or 64. |                                                              |
| DieIdName             | No            | String   | You use it when you want to get Lot number, Wafer,  X and Y location from DFF. Ex. U2.U1. | If you are planning to use variables leave this blank.       |
| UserVarForLotName     | No            | String   | These four variables are  necessary to define all of them if they are different from the default values. | Leave all four blank to use the default values.  CTSCVars.SC_LOTNAME |
| UserVarForWafer       | No            | String   |                                                              | CTSCVars.SC_LOT_WAFER                                        |
| UserVarForXCoordinate | No            | String   |                                                              | CTSCVars.EI_X_COORDINATE                                     |
| UserVarForYCoordinate | No            | String   |                                                              | CTSCVars.EI_Y_COORDINATE                                     |
| CTVs order            | Yes           | LSB/MSB  | This denotes whether the first bit is the least significant [LSB]  or the most significant [MSB]. Default is LSB |                                                              |
| TargetUserVar         | Yes           | String   | This is the name of the user var where ULT encoded is saved. |                                                              |

## Console output

When LogLevel is TEST\_METHOD - test method shows the actual encoded data

![screencaptured](.attachments\UltEncoder\screencaptured.png)

## Datalog output

Functional test execution results are not logged.

## Custom User Code Hooks

Here is the list of functions available to the user code to override.

  - *void **CustomVerify**()* – Stubbed in Prime. Can be overridden by User Code. The test method performs custom verify operation.
  - *void **CustomPostProcess**()* – Stubbed in Prime. Can be overridden by User Code. The test method executes the plist and captures CTV data. The data processing will be done in this hook which user should override and implement.
  - *string **EncodeULT**(ULT **ult**, ulong **bitCount**)* – Stubbed in Prime. Can be overridden by User Code. The test method encodes the ULT (Lot, Wafer, XLocation, YLocation) into a binary string of 50, 54 or 64 char according to bitCount. The method is used in Execute to encode and generate the  binary string. 

## TPL Samples

Here are a few test instance examples using the UltEncoder test method

```python
Import PrimeUltEncoderTestMethod.xml;

Test EncodeULT PrimeUltEncoderTestMethod
{
   DieIdName = "U1";
   BitCount = "56";
   TargetUserVar = "UserVar.ULTfuse"
}

Test EncodeCTSCVar PrimeUltEncoderTestMethod
{
   BitCount = "50";
   TargetUserVar = "UserVar.ULTCTSC"
}

```

## **Exit Ports**

The UltEncoder test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Execution has failed         |
| **1**         | ***Pass***      | Execution has passed         |
| **2**         | ***Fail***      | Encode logic failed          |
| **N**         | ***Pass/Fail*** | Failing condition            |

## Version tracking

| **Date**    | **Version** | **Author**       | **Comments**    |
| ----------- | ----------- | ---------------- | --------------- |
| Oct 9, 2021 | 1.0.0       | Gilbert Figueroa | Initial version |
| Dec. 02, 2022 | 1.0.1     | Javier Alpizar   | Updating Port 2 documentation |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High-Density Modular Tester
  - **TPL**: Test Programming Language
  - **CTV**: Capture This Vector
  - **ULT** Means Unit Level Traceability is a consolidated online database application that provides unit-based traceability across operations.