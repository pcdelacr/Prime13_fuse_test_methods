## REP for VirtualFuseExportToSharedStorage

[[_TOC_]]

This **REP** is intended to describe the VirtualFuseExportToSharedStorage Prime TestMethod.

## Methodology

This test method will write fuse HCS and FDS to shared storage keys to allows user to use them later on flow for example to do fuse override and etc.
If the there's no fuses being stored in the service OR none of the specified tags exists in the service's storage OR no fuse being tagged to the any of the specified tags, the test method and FuseManager Service will generate a default descriptor (dummy descriptor).

### Verify

Test method will get the fuse handler of the given namespace from *Namespace* parameter, if *Namespace* is not found verify will fail.

Test method will check the *FuseDescriptorGap* and *FuseDataGap* to not be empty.

If *Tags* is not defined, the test method will prepare to generate all the fuses in the namespace. If *Tags* is defined, it will prepare to only generate the fuses based on the defined tags only.

### Execute

Test method will read all fuses in given *Namespace* and given *Tags*, and will generate HCS and FDS. 

HCS and FDS will be generated through calls to **FuseService**. (please read FuseService documentation for more information).

The generated HCS and FDS size will be compared against the *Threshold*:-
- If both sizes are below the threshold:
  - If EnableLatch set to “Enable” in PrimeVirtualFuseExportToSharedStorage
    - The fuse data will be saved into latch.
  - HCS and FDS will be post-processed (reversed and left-padded with zeros).
  - HCS and FDS will be stored to SharedStorage using *HcsSharedStorageKey* and *FdsSharedStorageKey*.
  - Test method will exit through port 2.
- If both sizes are above the threshold:
  - If EnableLatch set to “Enable” in PrimeVirtualFuseExportToSharedStorage
    - The fuse data in the primary storage will be discarded and restore the data from latch storage.
  - Test method will **not** post process the HCS and FDS.
  - Test method will **not** store the HCS and FDS to SharedStorage.
  - It will exit through port 3.

::: mermaid
    flowchart TD
    A([VirtualFuseExportToSharedStorage Start Execute])
    B{Any data stored in FuseManager Service?}
    C("Generate Virtual Fuse (HCS and FDS)")
    D([Exit Port 1])
    E{Does HCS fit in Descriptor Gap Fuse?}
    F{Does FDS fit in Data Gap Fuse?}
    G(Reverse HCS)
    H(Zero-pad HCS to the left)
    I(Reverse FDS)
    J(Zero-pad FDS to the left)
    K(Store in SharedStorage with defined keys)
    L([Exit Port 2])
    M([Exit Port 3])
    N{Is Latching Enabled?}
    O(Copy to Latched Storage)
    P(Clean Primary Storage) 
    Q{Is mapping file generation enabled?}
    R(Generate mapping file)

    A --> B
    B --Y--> C
    B --N--> D
    C --> E
    E -- Y --> F
    E -- N --> M
    F -- N --> M
    F -- Y --> N
    Q -- Y --> R --> L
    Q -- N --> L
    N -- Y -->  O --> P
    P --> G --> H --> I --> J --> K
    N -- N --> G
    K --> Q
:::

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the VirtualFuseExportToSharedStorage test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                                                                           | **Comments**                 |
| ------------------ | ------------- | --------------- | ------------------------------------------------------------------------------------ | ---------------------------- |
| Namespace          | Yes           | String          |  Name of fuses namespace .                                      | |
| Tags          | No           | String (CommaSeparatedString)          |  List of tags to use when getting fuses data.                                    | If nothing is defined, the test method will generate HCS and FDS for all virtual fuses in the namespace. |
| HcsSharedStorageKey| Yes           | String          |  Shared storage key to use for writing Hcs.                               | |
| FdsSharedStorageKey| Yes           | String          |  Shared storage key to use for writing Fds.                               | |
| FuseDescriptorGap | Yes           | String          | The name of the fuse descriptor gap, it must be in the format of [Register.Group.Name] | This information will be used to query the fuse size for post-processing and threshold checking. |
| FuseDataGap | Yes           | String          |  The name of the fuse descriptor gap, it must be in the format of [Register.Group.Name] | This information will be used to query the fuse size for post-processing and threshold checking. |
| Threshold | No           | Integer          |  The size threshold of the generated virtual fuse in percentage. | Default value is 100. |
| EnableLatch | No	| String |  Enable/Disable the latch storage. | Valid choice is "Enable" or "Disable" |
| GenerateDataMapFile | No | String | Enable/Disable the map file generation. | A debug feature to generate fuse mappings in the for FDS to be used in conjunction with TAP_RMW tool. Valid choice is "Enable" or "Disable". If enabled, file is generated at C:\Temp\FuseMapFile.json |

## TPL Samples

```perl
Test PrimeVirtualFuseExportToSharedStorageTestMethod GenerateToSharedStorage_P1
{	
  LogLevel = "PRIME_DEBUG";
  Namespace = "U1"; 
  Tags = "Tag1"; 
  HcsSharedStorageKey = "VirtualFuse_Hcs";
  FdsSharedStorageKey = "VirtualFuse_Fds";
  FuseDescriptorGap = "CPU.VF_Heap_Descriptor_Gap.VF_Heap_Descriptor_Gap";
  FuseDataGap = "CPU.VF_Heap_Data_Gap.VF_Heap_Data_Gap";
  EnableLatch = "Enable";
  GenerateDataMapFile = "Enable";
}
```

## Sample of Fuse Data Map File for TAP_RMW
```json
{
  "VF_Gap_Field_Name": "CPU.VF_Heap_Data_Gap.VF_Heap_Data_Gap",
  "VF_Field_Map": {
    "Fuse2": "0 - 4",
    "Fuse1": "5 - 9"
  }
}
```

## Exit Ports

The VirtualFuseExportToSharedStorage test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**                              |
| ------------- | ------------- | -------------------------------------------- |
| **0**         | ***Fail***    | Failing condition. 		   				   |
| **1**         | ***Pass***    | Pass. No virtual fuses was stored, a dummy HCS is generated with FDS is zeroed out. |
| **2**         | ***Pass***    | Pass. Both generated HCS and FDS sizes are below threshold and exported successfully.     |
| **3**         | ***Pass***    | Pass. Either generated HCS or FDS size are above threshold, no HCS and FDS is stored in the SharedStorage.     |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SHOPS**: **Sh**orts and **Op**en**s** test methodology

## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**    |
| ------------------------- | ----------- | ----------------- | --------------- |
| April 19<sup>th</sup>, 2024 | 1.4 | Yusof, Adam malik | Test method will generate dummy descriptor when there's no fuse stored in the service. |
| October 06<sup>th</sup>, 2023 | 1.3 | Yusof, Adam malik | Latching mechanism introduction |
| August 23<sup>rd</sup>, 2023 | 1.2 | Yusof, Adam Malik | Test method will check generated HCS and FDS against the threshold, post-process and exiting different port depends on situation. |
| January 8<sup>th</sup>, 2023 | 1.1       | Dakwar, Wajde    | More details about how Execute phase works + slight typo fixes. |
| June 29<sup>th</sup>, 2022 | 1.0.0       | Dakwar, Wajde    | Initial version |
