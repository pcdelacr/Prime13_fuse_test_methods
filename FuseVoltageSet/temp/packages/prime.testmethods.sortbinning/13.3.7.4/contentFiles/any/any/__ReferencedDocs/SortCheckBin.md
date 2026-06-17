**Prime Test-Method Specification REP**

[[_TOC_]]

## REP for SortCheckBin Test Method

This **REP** is intended to describe the behavior and features of the SortCheckBin Prime Test Method.

# Introduction

The SortCheckBin Test Method works mostly the same as the SortSetBin Test Method, with the following exceptions:

- At the end of its flow, SortCheckBin **does not** set the final bin into TOS. The final bin is however printed to ituff with the same format and exported to Shared Storage. 
- SortCheckBin **does not** have the "ExpectedBinTraceCount" parameter and therefore does not perform a check before Good Die Binning to ensure that the complete TP flow was executed.

Please refer to the SortSetBin Test Method documentation for more details on common behavior.

# User Interface Parameters

The table below lists and describes the user interface parameters provided by the SortCheckBin Test Method.

| **Parameter Name** | **Mandatory?** | **Type** | **Description** |
|--------------------|----------------|----------|-----------------|
| ConfigurationFile  | Yes            | String   | Specifies the path to the Test Method configuration file.|
| BinStorageKeyPrefix | No            | String   | Defines a prefix for the Shared Storage Keys where the final bin values are stored.|

# SortCheckBin Test Method Exit Ports

The SortCheckBin Test Method supports the following exit ports.

| **Port Number** | **Exit Condition** | **Description** |
|-----------------|--------------------|-----------------|
|**-2**           |***Alarm***         |Any hardware alarm.|
|**-1**           |***Error***         |Any software error or software execution failure.|
|**0**            |***Fail***          |IB not found in configuration file, invalid bin for POSTHVQK or another code failure.|
|**1**            |***Pass***          |Good Die.|
|**2**            |***Pass***          |Valid Bad Die|
|**3**            |***Pass***          |Bin 26 that will reroute to HVQK flows.|
|**4**            |***Fail***          |Unassigned bin.|
|**5**            |***Fail***          |No Matching Die Configuration.|

# Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
