[[_TOC_]]

## REP for Gfx

***************************
<span style="color: RED"> **!!! Development of this object is WIP -  It must not be used in production !!!** </span>
***************************

This **REP** is intended to describe the Prime Gfx.

In this document, you will find the below sections:

  - **Overview** – The GFX Aggregator infrastructure Details

  - **Methodology** – A detailed description of this object intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Console output** – A detailed description of what is printed to console by this object
  
  - **Datalog output** – A detailed description of what is printed to datalog by this object

  - **Custom User Code hooks** – A list of functions available to the user code to override

  - **TPL Samples** – Examples of how to use this object in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this object to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document

  
## Overview

Due to large size of Graphics IP, recovery is an important component in every test program (TP) at SORT and CLASS.

During early silicon, the TPs recover a large number of non-POR configurations to promote yield, design, and back-end product development.

Once a product PRQs, the numbers of recoveries go down substantially, but still recoverable configurations are still numerous.

The GFX Aggregator infrastructure was created in order to provide a flexible and scalable solution that will enable graphics recovery.

### Graphics DFT Overview

At a high level, VPG’s Graphics (GT) and Display (DE) IPs employs a modular Partition based design framework for all products at Intel.

Each Partition is typically ~1 million gates in the design.

At the lowest level, partitions are broken down into EIDs (engine identifier), which are local DFT engines embedded throughout the GT (or DE) design that are responsible for testing the attached logic or memory arrays. EIDs are interconnected via a DFT ring that runs through the entire GT (or DE) unit.

The EIDs report back via a DFT ring the status results of their testing.

In terms of scale, each Scan EID typically maps to a single partition, while multiple Array EIDs can map to a single partition. Because Scan and Array map to the same partitions, their results can be translated to a common partition-level granularity and easily combined. 

Both Scan and Array tests results must be combined in order to understand the true functioning state of a partition.

Apart from the structural-based tests of Scan and Cache, Graphics also uses 2 types of functional tests called EUDFT and SBFT. 

The EUDFT test, tests special partitions on the die called EUs (execution units). These tests are typically run across all the EUs in the design in parallel in HVM. 

However, during early silicon and low yield regimes, it is possible to run EUDFT tests on standalone EUs s and collect Pass/Fail info. Similarly, the SBFT test is also run on a pipe or IP level in HVM. 

This test however cannot be broken down to partition level.

### Understanding Graphics components

To illustrate the various components of graphics on a die, the following is an example of a SKL Graphics die.

The SKL GT unit consists of a un-slice plus a number of slices. The un-slice contains shared resources common to the entire GT unit. 

In addition, in this example of SKL GT3, we have 2 slices. As a thumb rule, the number of slices go up as the size of Graphics grows. 

Within each slice, there is some slice common logic, four banks of L3 cache, and three sub-slices. Each sub-slice in turn consists of some sub-slice common logic and 8 Execution Units (EU).

![image.png](./.attachments/ConfigurationExample.png)

While the above example is specific to SKL, all Graphics at Intel have similar if not the same building blocks of un-slice, slice, sub-slices, EUs, and slice common logic.

Due to the redundant and common makeup of the slices, there are many recovery options\ configurations available including disabling a slice, sub-slice and L3 bank. Each recovery configuration is a SKUG.

### High Level Graphics Test and Recovery Flow

1.	Scan and Cache partition test data is collected for individual tests.

2.	The data from step 1 (which indicates EID’s pass\fail status) is combined based on the partition that each Scan/Cache EID are associated to. 

3.	Based on the data collected, GFX aggregator infrastructure can run evaluation and provide the SKUG (Graphic SKU).

4.	The SKUG from Step 3, is used as the starting point to test functional content.

5.	Depending on the needs of the product, TP may continue with a waterfall flow on Func content in case of a failure. Otherwise, the passing SKUG will be the final SKUG that is DFF’ed to the next socket.

![image.png](./.attachments/HighLevelsGraphic.png)

**Example**

Given this SKU’s definition per Cache & Scan EID’s stats (1 indicate pass):

![image.png](./.attachments/Example1.png)

Then by the data collected by GFX aggregator (as seen below), the evaluate SKU will be GT2a:

[Reminder: Scan EID always maps 1:1 to Partition, while Cache EID may map N:1 to Partition]

![image.png](./.attachments/Example2.png)

Func is then configured to GT2a, tested and results are recorded. The Func SKUG will be the final result SKUG:

![image.png](./.attachments/Example3.png)

### Gfx flow example

Attached Visio diagram demonstrate typical Gfx flow.

This is not a full TP flow nor is it the full Gfx flow, this only come to highlight the usage of various Gfx test class instances across the TP.

![image.png](./.attachments/GfxFlow.jpg)

## Methodology

The Gfx infrastructure is used in order to specify and validate the config input files that contain the SKU’s and EID’s definition need for setting up the Gfx data structures – this is done during ALEPH INIT Verify flow.

Important requirements:

- The user should provide only two files, one of each type.

- Splitting one file to several files, for example one per area is wrong.

- Areas that been defined in Eid file must appear in Sku file - Otherwise an exception will be thrown.

- All required attributes must defined in the files - Otherwise an exception will be thrown.

- Sku file name should be with the suffix ".gfxsku.json" and Eid file should be with the suffix ".gfxeid.json".



## Definition of configuration files

### SKU

#### **SKU** File example

	"Area": [
    {
      "Name": "GT_CHIPLETS", "UpsRecoveryGroupStorageKey": { "Name": "GTSREC" },
      "RecoveryGroups": [
        { "Name": "DISP" },
        { "Name": "UNS_REQ_GT4" },
        { "Name": "UNS_REQ_GT3_GT4" },
        { "Name": "UNS_REQ" },
        { "Name": "UNS_VID" },
        { "Name": "2ND_VID" },
        { "Name": "LNIECS_0" },
        { "Name": "LNIECS_1" },
        { "Name": "GT4_RC0" }
      ],
      "SKUs": [
        {"Group": "5x8",
          "SKU": [
            { "Name": "5x8S012", "Recovery": "000000000",
              "Fuses": [
                { "Name": "GTSKU", 	  "Value": "111" },
                { "Name": "GT_SLICE", "Value": "000" },
                { "Name": "SS_DIS",   "Value": "10101" },
                { "Name": "L3", 	  "Value": "111" },
                { "Name": "EU", 	  "Value": "10010101011" }
              ]
            }
          ]
        },
		{"Group": "4x8",
          "SKU": [
            { "Name": "4x8S01", "Recovery": "000000110",
              "Fuses": [
                { "Name": "GTSKU", "Value": "111" },
                { "Name": "GT_SLICE", "Value": "000" },
                { "Name": "SS_DIS", "Value": "10101" },
                { "Name": "L3", "Value": "111" },
                { "Name": "EU", "Value": "10010101011" }
              ]
            },
            { "Name": "4x8S12", "Recovery": "000000011",
              "Fuses": [
                { "Name": "GTSKU", "Value": "111" },
                { "Name": "GT_SLICE", "Value": "000" },
                { "Name": "SS_DIS", "Value": "10101" },
                { "Name": "L3", "Value": "111" },
                { "Name": "EU", "Value": "10010101011" }
              ]
            }
          ]
        }
	]
	},
	{
      "Name": "CHIPLET_1", "RefArea": "GT_CHIPLETS",
      "UpsRecoveryGroupStorageKey": { "Name": "GT_CHIPLETSSRECCL1" },
      "RecoveryGroups": [
        { "Name": "2ND_VID" },
        { "Name": "LNIECS_0" },
        { "Name": "LNIECS_1" },
        { "Name": "GT4_RC0" }
      ],
      "SKUs": [
        { "Group": "1x8x16",
          "SKU": [
            { "Name": "1x8x16", "Recovery": "000000000",
              "Fuses": [
                { "Name": "FUSE_GT_CHIPLETS_SKU", "Value": "1000" },
                { "Name": "FUSE_GT_CHIPLETS_COMPUTE_DSS_EN", "Value": "0000000000000000000000000000000000000000000000000000000011111111" }
              ]
            }
          ]
        }
	  ]
	}


#### SKU’s definition config file

Following is the description of expected element and attributes in EID’s configuration input file:

| **Element**            |  **Allowed Repetitions**        | *Attribute**              | **Required?** | **Description**                  |
| ------------------            | ------------- | --------------- | ------------------------------------------------------------------------------------------------------ | ---------------------------------- |
| Area                            | Multiple times           | Name    | YES   |  Specify the area name to which the recovery groups and SKU’s definitions are relevant. <br/> Area name options are: DE, GT, IPU, COMPUTE, BASE, RAMBO|
| SubArea                              | Multiple times           | RefArea  | NO  |  Specify the sub area name to which the recovery groups and SKU’s definitions are relevant. <br/> Sub Area name options are: CHIPLET_1, CHIPLET_2, CHIPLET_3, CHIPLET_4, CHIPLET_5, CHIPLET_6, CHIPLET_7, CHIPLET_8|
| UpsRecoveryGroupStorageKey                               | Only once           | Name   | YES | Specify the storage key token name that will hold the recovery group string which identify the final SKU. This token is populated by PrimeGfxEvaluate test method                                   |
| RecoveryGroups                                   |Only once          |                                | YES | Used to group together recovery groups                                  |
| RecoveryGroupsEntry                                 | Multiple times          | Name                                         | YES |	Specify recovery group name. Recovery group name must be unique per area. <br/> These names are also used in the EID’s config input file to connect each EID to the recovery group it relevant for.|
| SKUs                               | Multiple times           | Group  | YES |  Specify name for group of SKU’s.|
| SKU                             | Multiple times           | Name <br/>  <br/> Recovery  | **Name:** YES  <br/> <br/> **Recovery:** YES | **Name:** Specify the SKU name. SKU name must be unique per area. <br/>  <br/> **Recovery:** Specify the recovery groups’ status that identify this SKU. 0 (zero) means recovery group is enable, 1 means recovery group is disable. The length of specified string must match the number of recovery groups defined above. Recovery string must be unique per area.|
| Fuses                                | Only once           |  | YES |  Used to group together fuses.|
| FusesEntry                          | Only once           | Name <br/> <br/> Value     | **Name:** YES  <br/> <br/> **Value:** YES |  **Name:** Specify the fuse name that needs to be set with the element value in order to config the SKU. <br/> <br/> **Value:** Specify the fuse value.|

**Notes**

SKU’s order in config file:

- The order in which the SKU’s are defined in the config file, denotes the order in which GT recovery will be handled.

- This is relevant for both PrimeGfxEvaluation test method and UPS GT recovery methodology.

Using the RefArea attribute:

- Any area name & RefArea combination is valid, as long as the recovery groups in that area are subset of the reference area recovery groups.

- The name & order of the subset recovery groups must also be kept, but they are not required to be sequential (i.e. not all recovery groups have to be specified).

- See the above file example, where area CHIPLET_1 is setting GT as RefArea, and the recovery groups of CHIPLET_1 are subset of GT recovery groups.

- Area's which use RefArea doesn't need to be specified in the EID config file.

- Using the RefArea attribute provide the ability to “group” Area’s into single execution while being able to evaluate each Area separately.

	For example, area’s CHIPLET_1 to CHIPLET_8 can all use the same GT RefArea, then with a single PrimeGfxScorebard instance they can all be executed.
	
	Later, each CHIPLET can be evaluated separately to determine its own SKU.



	
### EID

#### **EID** File example

	"Area": [
	{
      "Name": "GT_CHIPLETS",
      "HryHierarchy": { "EidNotTestedSymbol": "8", "EidNotExistSymbol": "9",
        "HrySymbol": [
          { "Symbol": "4", "Status": "FAIL" },
          { "Symbol": "3", "Status": "FAIL" },
          { "Symbol": "2", "Status": "FAIL" },
          { "Symbol": "0", "Status": "FAIL" },
          { "Symbol": "1", "Status": "PASS" }
        ]
      },
      "Content": [
        {
          "Name": "SCAN",
          "EIDs": { "MaxSize": 8192,
            "Eid": [
              { "Id": "0", "PartId": "0", "RecoveryGroup": "DISP", "HryLevels": "GT_CHIPLETS DISP GT_CHIPLETSCOMPTILE0-GT_CHIPLETSDSSM0-EUTOP0" },
              { "Id": "1", "PartId": "1", "RecoveryGroup": "UNS_VID", "HryLevels": "GT_CHIPLETS UNS_VID GT_CHIPLETSCOMPTILE0-GT_CHIPLETSDSSM0-EUTOP1" },
              { "Id": "2", "PartId": "2", "RecoveryGroup": "GT4_RC0", "HryLevels": "GT_CHIPLETS GT4_RC0 GT_CHIPLETSCOMPTILE0-GT_CHIPLETSDSSM0-EUTOP2" },
              { "Id": "3", "PartId": "3", "RecoveryGroup": "DISP", "HryLevels": "GT_CHIPLETS DISP GT_CHIPLETSCOMPTILE0-GT_CHIPLETSDSSM0-EUTOP3" },
              { "Id": "4", "PartId": "4", "RecoveryGroup": "GT4_RC0", "HryLevels": "GT_CHIPLETS GT4_RC0 GT_CHIPLETSCOMPTILE0-GT_CHIPLETSDSSM0-EUSIDE4" },
              { "Id": "5", "PartId": "5", "RecoveryGroup": "UNS_VID", "HryLevels": "GT_CHIPLETS UNS_VID GT_CHIPLETSCOMPTILE0-GT_CHIPLETSDSSM0-EUSIDE5" }
			 ]
		}
	}
	

#### EID’s definition config file

Following is the description of expected element and attributes in SKU’s configuration input file:

| **Element**          | **Allowed Repetitions**        | *Attribute**                                                                                        | **Required?**   | **Description**                  |
| ------------------            | ------------- | --------------- | ------------------------------------------------------------------------------------------------------ | ---------------------------------- |
| Area                               | Multiple times           | Name  | YES  |  Specify the area name to which the recovery groups and SKU’s definitions are relevant. <br/> Area name options are: DE, GT, IPU, COMPUTE, BASE, RAMBO|
| HryHierarchy                    | Only once           | EidNotTestedSymbol <br/> <br/> EidNotExistSymbol          | **EidNotTestedSymbol:** YES   <br/> <br/> **EidNotExistSymbol:** YES  |  **EidNotTestedSymbol:** Special  symbol to represent EID that isn’t defined at the configuration file. <br/>  <br/> **EidNotExistSymbol:** Special  symbol to represent EID that defined at the configuration file but not tested.|
| HrySymbol                   | Multiple times           | Symbol <br/> <br/> Status             | **Symbol:** YES   <br/> <br/> **Status:** YES| **Symbol:** Specify the HRY symbol to be used for HRY raw string and tree level datalog.<br/> Note that the symbols order denotes their precedence for the matter of HRY tree datalog, as well as for aggregating EID’s status from Tags for SKU evaluation. <br/> <br/> **Status:** Specify the symbol status. Status options are: PASS, FAIL.|
| Content                         |Multiple times          |       Name                                 | YES   | Specify the content name. <br/> Content name must be unique per area. <br/> Contact name options are: SCAN, ARRAY, FUNC, FEVROS.                                  |
| EIDs                            | Only once          | MaxSize                                         | YES     |	Specify the total number of EID’s for the relevant content.|
| Eid                           | Multiple times           | Id  <br/> <br/> PartId <br/> <br/> RecoveryGroup <br/> <br/> HryLevels      | **Id:** YES  <br/> <br/> **PartId:** YES  <br/> <br/> **RecoveryGroup:** YES  <br/> <br/> **HryLevels:** YES |  **Id:** Specify the identifier for this Eid.  <br/> <br/> **PartId:** Specify the partition id for this Eid. <br/> <br/> **RecoveryGroup:** Specify the recovery group for which this Eid is relevant. <br/> Recovery group name must match one of the recovery groups specified in SKU’s definition config file per the relevant area. <br/> <br/> **HryLevels:** Specify the HRY levels relevant for this Eid. <br/> Elements are space separated (multiple spaces are supported). <br/> Per content, all of these attribute values must contain the same number of space separated elements. <br/> The value specified in this attribute is used for HRY tree level datalog.|


	
## How SKU’s & EID’s config files content defines GT SKU 

The definitions for Scan EIDs, Cache EIDs, Partitions, and SKUGs have already been defined earlier in this document (see Graphics DFT Overview). 

Recovery groups are simply a collection of partitions. These recovery groups are constructed by VPG design and provided to MVE. 

In general, recovery groups tend to assemble partitions that have the exact same impact on the overall recovery. 

This is done to modularize the recovery process by splitting partitions up into sensible groups to promote manageability and better validation.

To summarize, a collection of Scan/Cache EIDs forms Partitions. A collection of Partitions form Recovery Groups. A collection of Recovery Groups forms SKUGs.

Below is a graphic illustration of the discussed structure:

![image.png](./.attachments/CacheEids.png)

![image.png](./.attachments/ScanEids.png)

![image.png](./.attachments/Partitions.png)

![image.png](./.attachments/RecoveryGroups.png)

![image.png](./.attachments/SKUG1.png)

![image.png](./.attachments/SKUG2-3.png)

##**HRY** tree level datalog

The HryLevels attribute, specified per each EID in EID’s definition config file, is used in order to perform HRY tree level datalog.

MO have the ability to control up to which level to do the datalog using the instance parameter HryTreeLevelDatalog which is part of any GFX related test method.

Taken the above EID’s configuration input file example, below diagrams illustrates how the HRY tree level will look like per Content.

Note that each path in the tree identifies one and only one EID per content (i.e. the number of tree leafs is equal to the number of EID’s per content).

Each node in the tree will hold the HRY symbol that reflects the accumulated EID’s statuses of its sub-tree.

The HRY symbol precedence for accumulation, is set by the order of definition in the input file.

For example, taken the above EID’s configuration input file, the HRY symbol “4” have the highest precedence, while “9” have the lowest precedence.

![image.png](./.attachments/exampleHryTree.jpg)

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **EID**: E**n**gine **I**dentifier
  - **TP**: T**e**st Pr**o**gram
  - **HRY**: H**u**man R**e**adable Y**i**eld
  

## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**    |
| ------------------------- | ----------- | ----------------- | --------------- |
| July 7<sup>th</sup>, 2022 | 1.0.0       | Awaisy, Gadeer| Initial version |
