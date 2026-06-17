[[_TOC_]]

## REP for GfxMbistToTag

This **REP** is intended to describe the GfxMbistHryToTag Prime TestMethod.

Prior to reading this REP, it is recommend to read the GfxAggregator SDK to get an overview of the GFX methodology and the infrastructure for it in PRIME.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Console output** – A detailed description of what is printed to console by this TestMethod

  - **Custom User Code hooks** – A list of functions available to the user code to override

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document

## Methodology
> PR [#8092](https://dev.azure.com/mit-us/PRIME/_git/PRIME/pullrequest/8092)  
> Ticket [#47353](https://dev.azure.com/mit-us/PRIME/_workitems/edit/47353)

PTL Array will be using MbistVminSearch to find out the failing BPs (best ports) or what is known in GFX termoninology as PartId or EngineIds.
MbistVminSearch can also work on lower resolution than PartId/EngineId: controller/memory level.
 
In the solution Prime suggests in this ticket, GFX will know how to extract vid statuses from the HRY that MbistVminSearch exports.

Full discussion and requirements confirmatation can be found in ticket.

The connection between MbistVminSearch and GFX will happen through a new test method, called GfxMbistHryToTag.

The new test method 'GfxMbistHryToTag' will know how to read an HRY from shared storage, and create a new tag that can be used later
in GFX flow like any other tag, e.g. in GfxEvaluate and other GFX test methods.


![Diagram](.attachments/Diagram.png)

## Test Instance Parameters
## Interface
### Parameters

| **Parameter Name**            | **Required?** | **Type**        | **Common GFX Param**        |**Description**                                                                                        | **Default value**                  | **Comments**                       			|
| ------------------            | ------------- | --------------- | --------------------------- | ------------------------------------------------------------------------------------------------------ | ---------------------------------- | -------------------------------------------   |
| Area                          | Yes           | String          | Yes                         |  the area name to which the recovery groups and SKUs are relevant                               |                                    |                                   			|
| Content                       | Yes           | String          | Yes                         |  Specify the content name to which the recovery groups and SKUs are relevant                            |                                    |Content must be unique per area     			|
| Tag                           | Yes           | String          | Yes                         |  Specify the tag to mark the EID’s status of current instance in SharedStorage                          |                                    |                                  			    |
| TagPassSymbol                           | No           | String          | No                         |  Symbol for the pass character to describe passing Eid in the created tag. Default is '1'.                          |                                    |                                  			    |
| TagFailSymbol                           | No           | String          | No                         |  Symbol for the fail character to describe failing Eid in the created tag. Default is '0'.                          |                                    |                                  			    |
| HryMapKeys                       | Yes           | String          | No                          |  Comma separated list of shared storage keys that hold the compoenent name from MbistVminSearch     |                                    |                                  			    |
| HryValueKeys                     | Yes           | String          | No                          |  Comma separated list of shared storage keys that hold the compoenent status from MbistVminSearch     |                                    |                                  			    |
| HryPassSymbols                     | Yes           | String          | No                          |  Comma separated list of pass characters (symbols) from the HRY.                          |                                    |                                  			    |
| HryMappingConfig                     | Yes           | File          | No                          |  Path of input file that holds the mapping from component name in the "HryMapKeys" shared storage, to actual eid.                          |                                    |                                  			    |

## Decoding
The test method will know the Engine Ids (Eid) to mark as fail based on the mapping provided in 'HryMappingConfig' parameter.
Any HRY symbol that is not a pass symbol (based on HryPassSymbols parameters) will be considered as fail.
See example below for more information.
##  Example

HryMappingConfig parameter - input file example:
    
    {
         "BP2_WBP0_MEM2": 0,
         "BP0_WBP1_MEM2": 1,
         "BP41_WBP2_MEM2": 2,
         ....
    }

SharedStorages examples written by MbistVminSearch

    mapKey1: "BP0_WBP1_MEM1, BP0_WBP1_MEM2, BP1_WBP0_MEM1, BP2_WBP0_MEM1, BP2_WBP0_MEM2"
    hryKey1: "11P1X"
    
    mapKey2: "BP41_WBP2_MEM2, BP3_WBP0_MEM1"
    hryKey2: "01"


Instance of MbistVminSearch:

    Test PrimeGfxMbistHryToTagTestMethod PrimeGfxMbistHryToTagTestMethod_P1
    {
        LogLevel = "Enabled";
        Area = "GT_PRIME";
	    Content = "SCAN";
	    Tag = "MyMbistNewTag";
	    TagPassSymbol = "0";
	    TagFailSymbol = "1";
	    HryMapKeys = "mapKey1, mapKey2";
	    HryValueKeys = "hryKey1, hryKey2";
	    HryPassSymbols = "1,U,P";
    }
 
After decoding new tag will be created. Tag content will contain the following status per Eid:

    Eid 0 -> fail (due to BP2_WBP0_MEM2 status from HryMap1 and HryValue1 - last X char in the hry...)
    Eid 2 -> fail (due to BP41_WBP2_MEM2 status from HryMap1 and HryValue1 - the first 0 in the hry.)


## Assumptions & Behavior
1) The shared storage content is under MbistVminSearch responsibility, meaning that if something wrong with the values that lead to wrong decoding, MbistVminSearch
usage or code need to be checked by user.
2) The size of HryMapKeys and HryValueKeys shared storage content should match other wise test method will exit port 0. "BP1_WBP2_MEM2, BP3_WBP0_MEM1" and "01" OK. "BP1_WBP2_MEM2, BP3_WBP0_MEM1" and "011" WRONG. 
3) **Important Note** when marking a failing engine id, the associated PartId will also be considered as fail.

## Datalog output
None.

## TPL Samples
    Test PrimeGfxMbistHryToTagTestMethod PrimeGfxMbistHryToTagTestMethod_P1
    {
        LogLevel = "Enabled";
        Area = "GT_PRIME";
	    Content = "SCAN";
	    Tag = "MyMbistNewTag";
	    TagPassSymbol = "0";
	    TagFailSymbol = "1";
	    HryMapKeys = "mapKey1, mapKey2";
	    HryValueKeys = "hryKey1, hryKey2";
	    HryPassSymbols = "1,U,P";
        HryMappingConfig = "~HDMT_TPL_DIR/.../Modules/Gfx/InputFiles/GfxMbistHryToTag.gfxmbisthrytotag.json";
    }

## Exit Ports
1 if HRY decoded successfully and tag generated.
0 in any case of failure with the proper error message printed to console.

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **CTV**: **C**apture **T**his **V**ector
  - **DMEM**: **D**igital acquisition **MEM**ory

## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**    |
| ------------------------- | ----------- | ----------------- | --------------- |
| Jan 30<sup>th</sup>, 2024  | 1.0         | Dakwar, Wajde     | Full capabilities based on Ticket #47353. |
