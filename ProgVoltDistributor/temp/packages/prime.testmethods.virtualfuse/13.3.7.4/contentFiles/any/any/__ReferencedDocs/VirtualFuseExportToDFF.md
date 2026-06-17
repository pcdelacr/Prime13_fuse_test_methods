## REP for VirtualFuseExportToDFF

[[_TOC_]]

This **REP** is intended to describe the VirtualFuseExportToDFF Prime TestMethod.

## Methodology

This test method will export fuse data of given namespace (from 'Namespace' parameter) to a dff ('DffToken' parameter). It will take into considerations all tags in this namespace.

This test method can generate DFF in new format (backslash `\` delimiter) or old format (pipe `|` delimiter). Please refer to FuseManager Service documentation on how to get the DFF generated in which format.

### New Format
- `Tag1^Fuse1:00010&Fuse2:01000\Tag2^Fuse3:00010&Fuse4:01000...`

### Old Format
- `Tag1^Fuse1:00010&Fuse2:01000|Tag2^Fuse3:00010&Fuse4:01000...`

### Verify

Test method will get the fuse handler of the given namespace from 'Namespace' parameter, if 'Namespace' is not found verify will fail.

### Execute
Test method will read all fuses and data from all tags in namespace (from 'Namespace' parameter) and will export the data to dff.


Exporting all fuse data to dff in this format: 
<br>New format:
```
Tag2^Fuse3:00010&Fuse4:01000\Tag1^Fuse1:00010&Fuse2:01000\SplitDataTag^SplitFuse:010111001
```

OR

Old format:
```
Tag2^Fuse3:00010&Fuse4:01000|Tag1^Fuse1:00010&Fuse2:01000|SplitDataTag^SplitFuse:010111001
```

Note that another test method (VirtualFuseImportFromDFF) can read this dff value, decode it and populate namespace with it. For more information read VirtualFuseImportFromDFF documentation.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the VirtualFuseExportToDFF test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                                                         | **Comments** |
| ------------------ | ------------- | --------------- | ------------------------------------------------------------------ | ------------ |
| Namespace          | Yes           | String          | Name of fuses namespace .                                          |              |
| DffToken           | Yes           | String          | Name of dff to write fuses data into.                              |              |
| DffTokenMaxLimit   | No            | UnsignedInteger | Maximum size limit for each DFF token.                             |              |
| DffTokenQuantity   | No            | UnsignedInteger | Quantity of DFF tokens to be generated based on the DffToken name. |              |
| ZeroBasedIndexSplit           | No           | Bool          |  Indicates if the DFF to be imported is zero-based indexed or one-based index. Defaulted to `false`, which indicates one-based index                                    |

If both DffTokenMaxLimit and DffTokenQuantity are set, the fuse data will be divided into segments based on the DffTokenMaxLimit size. Here’s an example

- DffToken = "VFUSE";
- DffTokenMaxLimit = 20;
- DffTokenQuantity = 7;

Given the virtual fuse data: `Tag2^Fuse3:00010&Fuse4:01000\Tag1^Fuse1:00010&Fuse2:01000\SplitDataTag^SplitFuse:010111001`

The generated DFF tokens and their values will be:

| DFF Token | DFF Value            |
| --------- | -------------------- |
| VFUSE1    | Tag2^Fuse3:00010&Fus |
| VFUSE2    | e4:01000\Tag1^Fuse1: |
| VFUSE3    | 00010&Fuse2:01000\Sp |
| VFUSE4    | litDataTag^SplitFuse |
| VFUSE5    | :010111001;          |
| VFUSE6    | ;                    |
| VFUSE7    | ;                    |

Note that the final DFF token with valid data will be appended with a semi-colon (;) to indicate the user that is the end of the fuse data and they should stop reading there. If DffTokenQuantity is higher than the number of segments created, the extra tokens will just contain a semicolon (;) to show they are empty.

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


| **Exit Port** | **Condition** | **Description**                          |
| ------------- | ------------- | ---------------------------------------- |
| **0**         | ***Fail***    | Failing condition. 		                   |
| **1**         | ***Pass***    | Pass case. Fuses written successfully.   |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SHOPS**: **Sh**orts and **Op**en**s** test methodology

## Version tracking

| **Date**                     | **Version** | **Author**       | **Comments**                                                    |
| ---------------------------- | ----------- | ---------------- | --------------------------------------------------------------- |
| June 29<sup>th</sup>, 2022   | 1.0.0       | Dakwar, Wajde    | Initial version                                                 |
| January 8<sup>th</sup>, 2023 | 1.1         | Dakwar, Wajde    | More details about how Execute phase works + slight typo fixes. |
| October 3<sup>th</sup>, 2024 | 1.2         | Maria Hernandez  | Adding new capability to split fuse data by size                |
