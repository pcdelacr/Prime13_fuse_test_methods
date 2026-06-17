[[_TOC_]]

## REP for Fuse at Class

This **REP** is intended to describe the Fuse at Class Prime TestMethod.

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

The Fuse at Class test method is in charge of reading the contents of the Lot Reservation File and the Fact Rules File, validating them, and creating a mapping object for Sspec, Fact Bin and Pass Bin translation. This new mapping object consists on a comma separated string with 3 element tuples in the format 'PassBin:Sspec:FactBin', which is saved on the shared storage and can be used by the PassBinToSspecFuseAtClassBinConverter and SspecToFuseAtClassBinConverter Test Methods.

Notes: 
Only one instance of this Test Method is supposed to be inside a Test Program, though this is not enforced by Prime.
This Test Method should not be used with the iCFaCT template and UF in Evergreen.


### Verify
::: mermaid
graph TB
style Verify fill:#00AEEF,stroke:#0000,stroke-width:2px,color:#fff
style End fill:#00AEEF,stroke:#0000,stroke-width:2px,color:#fff
style PASS fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style PopulateMode fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style ParseContext fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style FactModeOff fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style ValidateMappingGsdsFC fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style ValidateOptypeFC fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style ReadLrf fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style ValidateMrvUserVarParam fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style ValidateBinsInLrfSubsetOfFrf fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style DisableChecks fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff

Verify(Verify Start) --> PopulateMode[Parse fact mode from param or user var in the param.]
-->FactModeOff{Does fact mode equals FactMode_OFF?}--> ParseContext[Validate that the shared storage<br>keys contain the Context.] 
 --> |No|ValidateOptypeFC[Validate the Op type is defined and the op type in dff matches the one in the user var]
FactModeOff -->|Yes| PASS[Return pass]
ValidateOptypeFC-->ValidateMappingGsdsFC[Validate that the Shared storage value for the Bin Sspec Mapping is not empty.]
ValidateMappingGsdsFC-->ReadLrf[Read and parse Lrf <br>and Frf files]
     -->  ValidateMrvUserVarParam[Validate the Mrv, revision, mode, <br>frv parameters from the input files<br> and check if the version matches<br> depending on the versionMatchParam]
     --> ValidateBinsInLrfSubsetOfFrf[ Validate that the bins<br> present on the Lrf file <br>are a subset of the ones<br> present on the Frf file. ]
     -->DisableChecks[Enable and disable <br>depending on user <br>var values] --> PASS --> End(End)
:::

### Execute
::: mermaid
graph TB
style Exceute fill:#00AEEF,stroke:#0000,stroke-width:2px,color:#fff
style FactModeOff fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style CheckMrv fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style LogBinMapping fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style PopulateTsAttrs fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff
style End fill:#00AEEF,stroke:#0000,stroke-width:2px,color:#fff
style PASS fill:#0071C5,stroke:#0000,stroke-width:2px,color:#fff

Exceute(Exceute Start) 
-->FactModeOff{FactMode_OFF == factMode}--> |Yes| PASS[Return pass]
FactModeOff -->|No|CheckMrv[Check the Mrv, lot id, device, package, assembly revision and step]
-->LogBinMapping[Save Bin Sspec mapping object to the shared storage]
-->PopulateTsAttrs
--> PASS --> End(End)
:::

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Fuse at Class test method:

| **Parameter Name** | **Required?** | **Type**        | **Values**                     | **Comments**                 |
| ------------------ | ------------- | --------------- | ------------------------------ | ---------------------------- |
| FactRulesFilePath  | Yes           |  String         | Path of the Fact Rules File or name of the user var which contains the file. |    |
| LotReservationFilePath  | Yes      |  String         | Path of the Lot Reservation File or name of the user var which contains the file. |  |
| FactMode           | Yes           |  String         | Fact Mode. Possible values: OFF, LotLevel, UnitLevel.  | When off, the test method is skipped and the Mapping Object is not created.  |
| SspecBinsMappingObjectKey  | Yes   |  String         | Key of the SspecBinMapping to be used on the shared storage. |  |
| SspecBinsMappingObjectContext | Yes |  String        | Context of the SspecBinMapping to be used on the shared storage. | Allowed values: "LOT" or "DUT". |
| VersionMatch       | NO            |  String         | The type of version check that should be done between the LRF and FRF files. Possible values: EXACT, OFFSET, DONT_CARE. Default value: DONT_CARE | Exact: Checks the version in both files is the same. Offset: Validates the difference in version between both files is less than the VersionOffset value. Dont care: doesnt check the version. |
| VersionOffset      | No            |  Integer        | Allowed version offset between FRF and LRF files.  | Only needed when the VersionMatch is DONT_CARE.  |
| MrvUserVar         | No            |  String         | Name of the user var that contains the Mrv value. | It's optional when the Mrv in the LRF is NOTAPPLICABLE. |
| OpTypeUserVar      | No            |  String         | Name of the user var that contains the Op Type against which the LRF File should validate. |         |

### Lot Reservation File (LRF)
The LRF file is a xml file with the following format:

```xml
<?xml version='1.0' encoding='UTF-8'?>
<OperationInfo OperationName="GetGenericData" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <transaction_log />
  <op_flex_params>
    <op_flex_param name="lot_id" value="SampleTPLLot" />
    <op_flex_param name="assembly_name" value="EH4EDDCV A ABLAHBLAHBLAHB" />
    <op_flex_param name="qty" value="2000" />
    <op_flex_param name="hri" value="ANN_FLX_WTL2" />
    <op_flex_param name="mrv" value="FF1F2E3F4FEF" />
    <op_flex_param name="frv" value="BDUD0" />
    <op_flex_param name="revision" value="01" />
    <op_flex_param name="fba_solve_id" value="2613" />
    <op_flex_param name="mode" value="UnitLevel" />
  </op_flex_params>
  <element_list>
    <element>
      <type>index</type>
      <value>1</value>
      <flex_output_params>
        <flex_output_param name="priority" value="1" />
        <flex_output_param name="planning_bin" value="B1" />
        <flex_output_param name="pass_bin" value="1195" />
        <flex_output_param name="sspec" value="LBWZA" />
        <flex_output_param name="fba_fuse_qty" value="300" />
        <flex_output_param name="sb_reserved_qty" value="300" />
      </flex_output_params>
    </element>
    <element>
      <type>index</type>
      <value>2</value>
      <flex_output_params>
        <flex_output_param name="priority" value="1" />
        <flex_output_param name="planning_bin" value="B2" />
        <flex_output_param name="pass_bin" value="1196" />
        <flex_output_param name="sspec" value="LBWZA" />
        <flex_output_param name="fba_fuse_qty" value="30" />
        <flex_output_param name="sb_reserved_qty" value="30" />
      </flex_output_params>
    </element>
    ...
  </element_list>
</OperationInfo>
```

- Validations:
1. The ```python lot_id```  should match the one in the ```python SC_LOTNAME``` user var.
1. The ```python mvr```should be a 12 characters long, hexadecimal number and must match the value specified in the ```python MrvUserVar```.
1. The value in ```python assembly_name``` should match the values of ```python SCVars.SC_DEVICE```, ```python SCVars.SC_PACKAGE ```, ```python SCVars.SC_Rev``` (user var should contain an empty string if there is a space in the assembly name) and ```python SCVars.SC_STEP``` in the following way:
![image.png](./.attachments/AssemblyNameDescription.png)

### Fact Rules File (FRF)
The FRF file is also an xml file that has the below format:
```xml
<?xml version = "1.0" encoding = "utf-8"?>
<FaCT FRV="BDUD0" revision="01" mode="UnitLevel" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:DispatchLotListOutSchema="http://intel.com/apf/DispatchLotListOutSchema" xsi:noNamespaceSchemaLocation="Gen_FaCT_tt_FRF.xsd">
  <FinishedGood name="Q9RC" type="VST">
    <SmallLotDecisionFlags Enable = "1" PriorityLimit ="20" ProjectedUnitCount ="25" ReceivingPriorityLimit ="59" />
    <BOM SFP_name="L84AXKCVP" FACT_bin="42" PDE_name="3M4CXDCV_D">
      <Flow index="1" pass_bin="1195" planning_bin="B1" />
      <Flow index="2" pass_bin="1196" planning_bin="B2" />
      <Flow index="3" pass_bin="1197" planning_bin="B3" />
      <Flow index="4" pass_bin="1198" planning_bin="B4" />
      <Flow index="5" pass_bin="1199" planning_bin="B5" />
      <Flow index="6" pass_bin="1201" planning_bin="B6" />
      <Flow index="7" pass_bin="1202" planning_bin="B7" />
      <Flow index="8" pass_bin="1203" planning_bin="B8" />
      <Flow index="9" pass_bin="1204" planning_bin="B9" />
      <Flow index="10" pass_bin="1205" planning_bin="BA" />
      <Flow index="11" pass_bin="1206" planning_bin="BB" />
      <Flow index="12" pass_bin="1207" planning_bin="BC" />
      <Flow index="13" pass_bin="1208" planning_bin="BD" />
      <Flow index="14" pass_bin="1209" planning_bin="BE" />
    </BOM>
  </FinishedGood>
  <FinishedGood name="LBWX" type="PROD">
    <SmallLotDecisionFlags Enable = "1" PriorityLimit ="20" ProjectedUnitCount ="25" ReceivingPriorityLimit ="59" />
    <BOM SFP_name="L84AXKCVP" FACT_bin="42" PDE_name="EH4EDDCVAA">
      <Flow index="1" pass_bin="1195" planning_bin="B1" />
      <Flow index="2" pass_bin="1196" planning_bin="B2" />
      <Flow index="3" pass_bin="1197" planning_bin="B3" />
      <Flow index="4" pass_bin="1198" planning_bin="B4" />
      <Flow index="5" pass_bin="1199" planning_bin="B5" />
      ...
    </BOM>
  </FinishedGood>
  ...
</FaCT>
```
-Notes: 
The FinishGood name is the Sspec and the PDE_name corresponds to the BOM.

- Validations:
The FRF data validations are only done for the BOMs matching the LRF and which are under a FinishedGood name that matches the any of the LRF sspec values.
1. When VersionMatch parameter is EXACT, verifies that the FRF revision matches the one in the LRF. If the VersionMatch is Offset instead, it calculates the revision difference in both files and checks if it is less than the VersionOffset.
1. The FRV value in both files should be the same.
1. All combinations of Sspecs/BOM values present in the LRF should be present in the FRF as well. In a similar way, all of the sspec, planning bin and pass bin combinations from the LRF should also be in the FRF.

## TPL Samples

Here are a few test instance examples using the Fuse at Class test method:

```python
Import PrimeFuseAtClassTestMethod.xml;

Test PrimeFuseAtClassTestMethod PrimeFuseAtClassTestMethod_UnitLevelHappyPath_P1
{
    FactRulesFilePath = "~HDMT_TPL_DIR/Modules/FuseAtClass/FuseAtClass/InputFiles/FRF/golden_frf.xml";
    LotReservationFilePath = "~HDMT_TPL_DIR/Modules/FuseAtClass/FuseAtClass/InputFiles/LRF/lrf_version_diff.xml";
    FactMode = "UnitLevel";
    VersionMatch = "OFFSET";
    VersionOffset = 3;
    SspecBinsMappingObjectKey = "Key";
    SspecBinsMappingObjectContext = "DUT";
    MrvUserVar = "FuseAtClass::FaCT.MrvUserVar";
    LogLevel = "PRIME_DEBUG";
}
```
Here is an example of a lrf xml file:

```xml
<?xml version='1.0' encoding='UTF-8'?>
<OperationInfo OperationName="GetGenericData" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <transaction_log />
  <op_flex_params>
    <op_flex_param name="lot_id" value="SampleTPLLot" />
    <op_flex_param name="assembly_name" value="EH4EDDCV A ABLAHBLAHBLAHB" />
    <op_flex_param name="qty" value="2000" />
    <op_flex_param name="hri" value="ANN_FLX_WTL2" />
    <op_flex_param name="mrv" value="FF1F2E3F4FEF" />
    <op_flex_param name="frv" value="BDUD0" />
    <op_flex_param name="revision" value="01" />
    <op_flex_param name="fba_solve_id" value="2613" />
    <op_flex_param name="mode" value="UnitLevel" />
  </op_flex_params>
  <element_list>
    <element>
      <type>index</type>
      <value>1</value>
      <flex_output_params>
        <flex_output_param name="priority" value="1" />
        <flex_output_param name="planning_bin" value="B1" />
        <flex_output_param name="pass_bin" value="1195" />
        <flex_output_param name="sspec" value="LBWZA" />
        <flex_output_param name="fba_fuse_qty" value="300" />
        <flex_output_param name="sb_reserved_qty" value="300" />
      </flex_output_params>
    </element>
    <element>
      <type>index</type>
      <value>2</value>
      <flex_output_params>
        <flex_output_param name="priority" value="1" />
        <flex_output_param name="planning_bin" value="B2" />
        <flex_output_param name="pass_bin" value="1196" />
        <flex_output_param name="sspec" value="LBWZA" />
        <flex_output_param name="fba_fuse_qty" value="30" />
        <flex_output_param name="sb_reserved_qty" value="30" />
      </flex_output_params>
    </element>
    <element>
      <type>index</type>
      <value>3</value>
      <flex_output_params>
        <flex_output_param name="priority" value="1" />
        <flex_output_param name="planning_bin" value="B3" />
        <flex_output_param name="pass_bin" value="1197" />
        <flex_output_param name="sspec" value="LBWZA" />
        <flex_output_param name="fba_fuse_qty" value="1920" />
        <flex_output_param name="sb_reserved_qty" value="603" />
      </flex_output_params>
    </element>
  </element_list>
</OperationInfo>
```

An example of the FRF file is:
```xml
<?xml version = "1.0" encoding = "utf-8"?>
<FaCT FRV="BDUD0" revision="01" mode="UnitLevel" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:DispatchLotListOutSchema="http://intel.com/apf/DispatchLotListOutSchema" xsi:noNamespaceSchemaLocation="Gen_FaCT_tt_FRF.xsd">
  <FinishedGood name="Q9RC" type="VST">
    <SmallLotDecisionFlags Enable = "1" PriorityLimit ="20" ProjectedUnitCount ="25" ReceivingPriorityLimit ="59" />
    <BOM SFP_name="L84AXKCVP" FACT_bin="42" PDE_name="3M4CXDCV_D">
      <Flow index="1" pass_bin="1195" planning_bin="B1" />
      <Flow index="2" pass_bin="1196" planning_bin="B2" />
      <Flow index="3" pass_bin="1197" planning_bin="B3" />
      <Flow index="4" pass_bin="1198" planning_bin="B4" />
      <Flow index="5" pass_bin="1199" planning_bin="B5" />
      <Flow index="6" pass_bin="1201" planning_bin="B6" />
      <Flow index="7" pass_bin="1202" planning_bin="B7" />
      <Flow index="8" pass_bin="1203" planning_bin="B8" />
      <Flow index="9" pass_bin="1204" planning_bin="B9" />
      <Flow index="10" pass_bin="1205" planning_bin="BA" />
      <Flow index="11" pass_bin="1206" planning_bin="BB" />
      <Flow index="12" pass_bin="1207" planning_bin="BC" />
      <Flow index="13" pass_bin="1208" planning_bin="BD" />
      <Flow index="14" pass_bin="1209" planning_bin="BE" />
    </BOM>
  </FinishedGood>
  <FinishedGood name="LBWX" type="PROD">
    <SmallLotDecisionFlags Enable = "1" PriorityLimit ="20" ProjectedUnitCount ="25" ReceivingPriorityLimit ="59" />
    <BOM SFP_name="L84AXKCVP" FACT_bin="42" PDE_name="EH4EDDCVAA">
      <Flow index="1" pass_bin="1195" planning_bin="B1" />
      <Flow index="2" pass_bin="1196" planning_bin="B2" />
      <Flow index="3" pass_bin="1197" planning_bin="B3" />
      <Flow index="4" pass_bin="1198" planning_bin="B4" />
      <Flow index="5" pass_bin="1199" planning_bin="B5" />
      <Flow index="6" pass_bin="1201" planning_bin="B6" />
      <Flow index="7" pass_bin="1202" planning_bin="B7" />
      <Flow index="8" pass_bin="1203" planning_bin="B8" />
      <Flow index="9" pass_bin="1204" planning_bin="B9" />
      <Flow index="10" pass_bin="1205" planning_bin="BA" />
      <Flow index="11" pass_bin="1206" planning_bin="BB" />
      <Flow index="12" pass_bin="1207" planning_bin="BC" />
      <Flow index="13" pass_bin="1208" planning_bin="BD" />
      <Flow index="14" pass_bin="1209" planning_bin="BE" />
      <Flow index="15" pass_bin="1200" planning_bin="BF" />
    </BOM>
  </FinishedGood>
  <FinishedGood name="LBWY" type="PROD">
    <SmallLotDecisionFlags Enable = "1" PriorityLimit ="20" ProjectedUnitCount ="25" ReceivingPriorityLimit ="59" />
    <BOM SFP_name="L84AXKCVP" FACT_bin="42" PDE_name="EH4EDDCVAA">
      <Flow index="1" pass_bin="1195" planning_bin="B1" />
      <Flow index="2" pass_bin="1196" planning_bin="B2" />
      <Flow index="3" pass_bin="1197" planning_bin="B3" />
      <Flow index="4" pass_bin="1198" planning_bin="B4" />
      <Flow index="5" pass_bin="1199" planning_bin="B5" />
      <Flow index="6" pass_bin="1201" planning_bin="B6" />
      <Flow index="7" pass_bin="1202" planning_bin="B7" />
      <Flow index="8" pass_bin="1203" planning_bin="B8" />
      <Flow index="9" pass_bin="1204" planning_bin="B9" />
      <Flow index="10" pass_bin="1205" planning_bin="BA" />
      <Flow index="11" pass_bin="1206" planning_bin="BB" />
      <Flow index="12" pass_bin="1207" planning_bin="BC" />
      <Flow index="13" pass_bin="1208" planning_bin="BD" />
      <Flow index="14" pass_bin="1209" planning_bin="BE" />
      <Flow index="15" pass_bin="1200" planning_bin="BF" />
    </BOM>
  </FinishedGood>
  <FinishedGood name="LBWZA" type="PROD">
    <SmallLotDecisionFlags Enable = "1" PriorityLimit ="20" ProjectedUnitCount ="25" ReceivingPriorityLimit ="59" />
    <BOM SFP_name="L84AXKCVP" FACT_bin="38" PDE_name="EH4EDDCVAA">
      <Flow index="1" pass_bin="1195" planning_bin="B1" />
      <Flow index="2" pass_bin="1196" planning_bin="B2" />
      <Flow index="3" pass_bin="1197" planning_bin="B3" />
      <Flow index="4" pass_bin="1198" planning_bin="B4" />
      <Flow index="5" pass_bin="1199" planning_bin="B5" />
      <Flow index="6" pass_bin="1201" planning_bin="B6" />
      <Flow index="7" pass_bin="1202" planning_bin="B7" />
      <Flow index="8" pass_bin="1203" planning_bin="B8" />
      <Flow index="9" pass_bin="1204" planning_bin="B9" />
      <Flow index="10" pass_bin="1205" planning_bin="BA" />
      <Flow index="11" pass_bin="1206" planning_bin="BB" />
      <Flow index="12" pass_bin="1207" planning_bin="BC" />
      <Flow index="13" pass_bin="1208" planning_bin="BD" />
      <Flow index="14" pass_bin="1209" planning_bin="BE" />
      <Flow index="15" pass_bin="1200" planning_bin="BF" />
    </BOM>
  </FinishedGood>
</FaCT>
```
## User Var population
The following TSATTRSVars User Vars are populated during the FuseAtClass Test Method execution. 
| **Name** | **Value description** |
| ------------- | ------------- |
| **totalINs**         | Value of the tag “qty” in LRF.  |
| **FRFrevision**         | Revision value in the FRF file. |
| **FRFFRV**         | FRV value in FRF.  |
| **FRFrevisioninLRF**         | Revision value in the LRF file. |
| **FRVinLRF**         | FRV value in LRF.  |
| **FBAsolveID**         | The FBA solve ID that the smart bin used to generate the LRF, which corresponds to the  ```python fba_solve_id``` field.  |
| **FaCTMode**         | Fact Mode |
| **LRFData##**         | ## is a 2 digit number beginning with 00 and incrementing for each index entry in the LRF. Each of these correspond to the following values taken from each LRF element: ```python <priority>_<planning_bin>_<pass_bin>_<sspec>_<fba_fuse_qty>_<sb_reserved_qty>```. |

Note: User should be aware that the values in these should be cleared between different executions or else they could contain wrong values.

## Exit Ports

The Fuse at Class test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**                                                         |
| ------------- | ------------- | ------------------------------------------------------------------------|
| **0**         | ***Fail***    | Failing condition.                                                      |
| **1**         | ***Pass***    | The mapping object was created and saved successfully.          |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **FaCT**: **F**use **A**t **C**lass (**T**est)
  - **LRF**: **L**ot **R**eservation **F**ile
  - **FRF**: **F**act **R**ules **F**ile
  - **MRV**: **M**achine **R**eadable **V**alue

## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**    |
| ------------------------- | ----------- | ----------------- | --------------- |
| Apr 29<sup>th</sup>, 2022 | 1.0.0       | Andrea Gomez      | Initial version |
