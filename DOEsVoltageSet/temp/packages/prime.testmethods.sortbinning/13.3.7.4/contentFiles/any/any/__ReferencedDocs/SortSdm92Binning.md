**Prime Test-Method Specification REP**

[[_TOC_]]

## REP for SortSdm92Binning Test Method

This **REP** is intended to describe the behavior and features of the SortSdm92Binning Prime Test Method.

# Introduction

The PrimeSortSdm92Binning Test Method is designed to manage the binning process for units that have not undergone complete testing in the Singulated Die Merged (SDM) flow. This flow involves testing units in a sequence: cold (SDS), hot (SDT), and then cold again. The method applies logic to determine if a unit should be re-binned to 92XX, where 'XX' represents the hard bin of a hot failure.
The re-binned process is triggered under specific conditions:

    1-The unit must have fully passed the cold test, indicated by a cold bin value less than 7.
    2-The unit must not have been stressed, denoted by the stress indicator token not being equal to 1.
    3-The hot test must not have been completed, shown by a hot bin value not equal to 69.
    4-There must be failures in the pre-stress bin traces from the hot test.

The bin traces are evaluated using the CheckpointBinChecker.ComputeCheckpointBadBin method, and the final binning is processed using the Bin92Computer.ComputeBin92 method.

The code implementation verifies these conditions and executes the binning logic accordingly. It checks the cold bin result to ensure it is within acceptable limits, evaluates the bin traces for any failures, and confirms whether the unit has been stressed. If the unit meets all criteria, it is re-binned to the 92XX category, and the new bin result is applied to tester and logged to ituff.

# User Interface Parameters

The table below lists and describes the test instance parameters supported by the PrimeSortSdm92Binning TM

| **Parameter Name**     | **Mandatory?** | **Type**                          | **Description**                                                                                | **Comments**                                                                                                                                                                                                         |
|------------------------|----------------|-----------------------------------|------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| ColdBin				 | Yes            | COLDBIN Shared Storage token key  | Integer token to give bin result good or bad												   |                                                                                                                                                                                                                      |
| HotBin				 | Yes            | HOTBIN Shared Storage token key   | Integer token to give bin result good or bad	   											   |                                                                                                                                                                                                                      |
| BinTraceList			 | Yes            | BinTraces to Rebin                | Flow Bintraces we want to rebin on															   |                                                                                                                                                                                                                      |
| StressIndictatorToken  | Yes            | ISSTRESS Shared Storage token key | Token to denote if stress has been applied to the die										   |                                                                                                                                                                                                                      |

# SortSdm92Binning Test Method Exit Ports

The SortSdm92Binning Test Method supports the following exit ports.

| **Port Number** | **Exit Condition** | **Description** |
|-----------------|--------------------|-----------------|
|**-2**           |***Alarm***         |Any hardware alarm.|
|**-1**           |***Error***         |Any software error or software execution failure.|
|**0**            |***Fail***          |Test Method failing condition.|
|**1**            |***Pass***          |No need to re-bin as 92XX and continue to next test.|
|**2**            |***Fail***          |Unit re-binned to 92XX.|

# TPL Samples

```
Test SortSdm92Binning CTRL_UB_X_K_TESTPLANENDFLOW_X_X_X_X_SDM92
{
	ColdBin = "SDS_SortBinner_DB";
	HotBin = "SDT_SortBinner_DB";
	BinTraceList = "BINTRACESDTSTART,BINTRACESDTBEGIN";
	StressIndictatorToken = "ISSTRESS";
}
```

# Console output ( example debug mode)

```
=========================
Running Execute() for test instance=[TPI_BASE::CTRL_HMI_X_K_TESTPLANENDFLOW_X_X_X_X_ApplyBin]
=========================
[2024-Oct-22 13:51:54.157][DUT: 1]Imported Coldresult = [1010001].
[2024-Oct-22 13:51:54.159][DUT: 1]Running Checkpoint algorithm.
[2024-Oct-22 13:51:54.159][DUT: 1]Checkpoint: Processing fail bin from all specified BinTrace Subflows.
[2024-Oct-22 13:51:54.160][DUT: 1]Checkpoint: Processing Bintrace data for array=[TEST].
[2024-Oct-22 13:51:54.215][DUT: 1]Checkpoint: Searching for a fail bin within Bintrace array=[TEST].
[2024-Oct-22 13:51:54.224][DUT: 1]Read hot hardbin = [94].
[2024-Oct-22 13:51:54.225][DUT: 1]Running Bin92 algorithm.
[2024-Oct-22 13:51:54.225][DUT: 1]New 92fail bin = [9294].
[2024-Oct-22 13:51:54.225][DUT: 1]Modified to databin 92fail bin = [92940001].
[2024-Oct-22 13:51:54.238][DUT: 1]Bin=[92940001] was successfully set into Tester.
[2024-Oct-22 13:51:54.284][DUT: 1]Printed to Ituff:
2_tname_SortBinning::Sdm92Binning_P2
2_strgval_92940001|SortBinning::Sdm92Binning_P2|ISBIN26_0_WASBIN26_0_ISVADTL_0_WASVADTL_0_ISVADTLRECOVERY_0_WASVADTLRECOVERY_0
[2024-Oct-22 13:51:54.284][DUT: 1]Store SharedStorage Key=[SETBIN_INSTANCENAME_KEY] Value=[SortBinning::Sdm92Binning_P2]
[2024-Oct-22 13:51:54.285][DUT: 1]New hardbin = [92].
[2024-Oct-22 13:51:54.285][DUT: 1]New softbin = [92940001].
[2024-Oct-22 13:51:54.285][DUT: 1]New databin = [92940001].
[2024-Oct-22 13:51:54.296][DUT: 1]Test instance=[SortBinning::Sdm92Binning_P2] executed using 290.636200 ms
[2024-Oct-22 13:51:54.296][DUT: 1]TestInstance=[SortBinning::Sdm92Binning_P2] exit port=[2].
[2024-Oct-22 13:51:54.309][DUT: 1]
```

# Ituff snippet (good example)
```
2_tname_SortBinning::Sdm92Binning_P2
2_strgval_92940001|SortBinning::Sdm92Binning_P2|ISBIN26_0_WASBIN26_0_ISVADTL_0_WASVADTL_0_ISVADTLRECOVERY_0_WASVADTLRECOVERY_0
```

# Acronyms

Definition of acronyms used in this document:

  - **TM**: Test Method
  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **SDS**: Singulated Die Sort
  - **SDM**: Singulated Die Merged
  - **SDT**: Singulated Die Test 

