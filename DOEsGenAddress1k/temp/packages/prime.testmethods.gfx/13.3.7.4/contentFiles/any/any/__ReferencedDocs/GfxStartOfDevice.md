[[_TOC_]]

## REP for GfxStartOfDevice

This **REP** is intended to describe the GfxStartOfDevice Prime test method.

Prior to reading this REP, it is recommend to read the GfxAggregator SDK to get an overview of the GFX methodology and the infrastructure for it in PRIME :)

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this test method intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Console output** – A detailed description of what is printed to console by this test method

  - **Exit Ports** - A table describes each exit port

  - **Acronyms** - Definition of acronyms used in this document

  - **Version tracking** – With author names, so you always have a name to address
  
## Methodology

This test method is used for:

•	Reset the dynamic data (of EID’s pass\fail status) aggregated by GFX infrastructure and get it ready for next unit execution.

•	Set the start & end SKU that will be used when SKU evaluation action is performed.

Note that GFX related test method (such as: PrimeGfxEvaluate, PrimeGfxScoreboard) also have the ability to set start & end SKU.

Test method specific setting will override the GfxStartOfDevice settings.


### Verify

Validate input parameters.

The area is required parameter, so it cannot be empty string - otherwise an appropriate exception will be thrown. 
  
### Execute

Sets the sku bounds for the specific area and reset the dynamic data.


## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ArrayFusing test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                                                                                          | **Comments*                        |
| ------------------ | ------------- | --------------- | ----------------------------------------------------------------------------------------------------| ---------------------------------- |
| Area                          | YES           | String          | Specify the area name to which the SKUs are relevant                               |                                    |                                   			|
| StartSku                      | NO            | String          | Specify the start SKU for evaluation.If value is not specified, start SKU will be set to the first SKU defined in SKU’s config input file. | |                  		    	|
| EndSku                        | NO            | String          | Specify the end SKU for evaluation.If value is not specified, end SKU will be set to the first SKU defined in SKU’s config input file.                                                                      |                                    |                           


## TPL Samples

Example of an GfxStartOfDevice test method test in the .mtpl file:

	Test PrimeGfxStartOfDevicetest method Start_Executue_Pass_Before_Flow_Simulate_One_Area_Scan_Content_p1
	{
		Area = "GT_PRIME";
		StartSku = "5x8S012";
		EndSku = "2x8S01";
	}

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
  - **TPL**: T**e**st **P**rogramming L**a**nguage
  - **EID**: E**n**gine **I**dentifier


## Version tracking

| **Date**                  | **Version** | **Author**           | **Comments**    |
| ------------------------- | ----------- | -------------------- | --------------- |
| June 27<sup>th</sup>, 2022 | 1.0.0       | Gadeer Awaisy 				     | Initial version |

