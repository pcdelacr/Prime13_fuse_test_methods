## REP for VirtualFuseImportFromDFF

[[_TOC_]]

This **REP** is intended to describe the VirtualFuseImportFromDFF Prime TestMethod.

## Methodology

This test method will populate fuses data from dff (from 'DffToken' parameter). The fuse data from DFF must follow Prime's FuseService format.

### Verify

Test method will get the fuse handler of the given namespace from 'Namespace' parameter, if 'Namespace' is not found verify will fail.

### Execute

Test method will clear all fuse data stored under the given namespace from 'Na mespace' parameter, then will read dff from 'DffToken' parameter and update fuses data of this namespace.
Please refer to FuseManager Service documentation on how to get the DFF generated in which format.

The format that this test method expects are:
- `Tag1^Fuse1:00010&Fuse2:01000\Tag2^Fuse3:00010&Fuse4:01000...` (new DFF format) 
- `Tag1^Fuse1:00010&Fuse2:01000|Tag2^Fuse3:00010&Fuse4:01000...` (older DFF format) 

(the same format that VirtualFuseExportToDFF and FuseManager Service creates). This test method will decode it, and populate the internal database (tags, fuses names, values..) of the given namespace.

If dff not found, it will exit from port 0 - in this case'Namespace' will not be cleared.
If dff token exists, it will exit from port 1.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the VirtualFuseImportFromDFF test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                                                                           | **Comments**                 |
| ------------------ | ------------- | --------------- | ------------------------------------------------------------------------------------ | ---------------------------- |
| Namespace          | Yes           | String          |  Name of namespace for updating it's content from dff.                                      |
| DffToken           | Yes           | String          |  Name of dff to read fuses data from.                                    |
| DffIsSplit           | No           | Bool          |  Indicates if the DFF to imported is split or not. Defaulted to `false`, which indicates non-split DFF.                                     |
| ZeroBasedIndexSplit           | No           | Bool          |  Indicates if the DFF to be imported is zero-based indexed or one-based index. Defaulted to `false`, which indicates one-based index                                    |

## TPL Samples

```
Test PrimeVirtualFuseImportFromDffTestMethod ImportFromDff_P1
{
    LogLevel = "PRIME_DEBUG";
    Namespace = "U1";
    DffToken = "VFUSE";
}
```


## Exit Ports

The VirtualFuseImportFromDFF test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**                              |
| ------------- | ------------- | -------------------------------------------- |
| **0**         | ***Fail***    | Failing condition. 		                   |
| **1**         | ***Pass***    | Pass case. Fuses updated successfully.   |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SHOPS**: **Sh**orts and **Op**en**s** test methodology

## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**    |
| ------------------------- | ----------- | ----------------- | --------------- |
| August 8<sup>th</sup>, 2025 | 1.0.2    | Yusof, Adam Malik  | Information on new DFF format |
| January 8<sup>th</sup>, 2023 | 1.0.1       | Dakwar, Wajde    | Clarified namespace data clearing in port 0 case + more info about Execute. |
| June 29<sup>th</sup>, 2022 | 1.0.0       | Dakwar, Wajde    | Initial version |