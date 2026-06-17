## REP for VirtualFuseExportToDFF

[[_TOC_]]

This **REP** is intended to describe the VirtualFuseExportToDFF Prime TestMethod.

## Methodology

This test method will export fuse data of given namespace (from 'Namespace' parameter) to a dff ('DffToken' parameter). It will take into considerations all tags in this namespace.

### Verify

Test method will get the fuse handler of the given namespace from 'Namespace' parameter, if 'Namespace' is not found verify will fail.

### Execute
Test method will read all fuses and data from all tags in namespace (from 'Namespace' parameter) and will export the data to dff.


Exporting all fuse data to dff in this format: 
```
Tag1^Fuse1:00010&Fuse2:01000|Tag2^Fuse3:00010&Fuse4:01000
```

Note that another test method (VirtualFuseImportFromDFF) can read this dff value, decode it and populate namespace with it. For more information read VirtualFuseImportFromDFF documentation.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the VirtualFuseExportToDFF test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                                                                           | **Comments**                 |
| ------------------ | ------------- | --------------- | ------------------------------------------------------------------------------------ | ---------------------------- |
| Namespace          | Yes           | String          |  Name of fuses namespace .                                      |
| DffToken           | Yes           | String          |  Name of dff to write fuses data into.                               |

## TPL Samples

```
Test PrimeVirtualFuseExportToDffTestMethod ExportToDff_P1
{
    LogLevel = "PRIME_DEBUG";
	Namespace = "U1";
	DffToken = "VFUSE";
}
```

## Exit Ports

The VirtualFuseExportToDFF test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**                              |
| ------------- | ------------- | -------------------------------------------- |
| **0**         | ***Fail***    | Failing condition. 		                   |
| **1**         | ***Pass***    | Pass case. Fuses written successfully.   |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SHOPS**: **Sh**orts and **Op**en**s** test methodology

## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**    |
| ------------------------- | ----------- | ----------------- | --------------- |
| January 8<sup>th</sup>, 2023 | 1.1       | Dakwar, Wajde    | More details about how Execute phase works + slight typo fixes. |
| June 29<sup>th</sup>, 2022 | 1.0.0       | Dakwar, Wajde    | Initial version |
