**Prime Test-Method Specification REP**

[[_TOC_]]

## REP for SortApplyBin Test Method

This **REP** is intended to describe the behavior and features of the SortApplyBin Prime Test Method.

# Introduction

The SortApplyBin test method sets the tester's bin to the value specified by the BinResult parameter, but only if the current bin is not 92 or a TOS alarm bin. The bin value is retrieved from shared storage using the BinResult key. If a TOS alarm is set or the current bin is 92, the method does not attempt to set a new bin and returns 1. If the bin is set successfully, the method returns 1; if setting the bin fails (for example, if the bin does not exist in the bdef file), the method returns 0.

----

## Test Instance Parameters
The table below lists and describes the test instance parameters supported by the SortApplyBin test method

| **Parameter Name**   | **Required?** | **Type**  | **Values**    | **Comments**   |
|-----------|-----------|-------------------------------|--------------------------|--------------------------|
| BinResult          | Yes       | Shared Storage   | Any Shared Storage token that contains a bin  |    |

----

# SortApplyBin Test Method Exit Ports

The SortApplyBin Test Method supports the following exit ports.

| **Port Number** | **Exit Condition** | **Description** |
|-----------------|--------------------|-----------------|
|**-2**           |***Alarm***         |Any hardware alarm.|
|**-1**           |***Error***         |Any software error or software execution failure.|
|**0**            |***Fail***          |Test Method failing condition.|
|**1**            |***Pass***          |Test Method passing condition.|


## TPL Samples

```
CSharpTest PrimeSortApplyBinTestMethod CTRL_X_X_K_TESTPLANENDFLOW_X_X_X_X_APPLYSDSBIN
{
	BypassPort = -1;
	BinResult = "SDS_SortBinner_DB";
}
```

## Console output (example debug mode)

```
=========================
Running Execute() for test instance=[TPI_BASE::CTRL_HMI_X_K_TESTPLANENDFLOW_X_X_X_X_ApplyBin]
=========================
[2024-Mar-26 07:06:08.167][DUT: 1]Trying to set = [4440000].
[2024-Mar-26 07:06:08.213][DUT: 1]Test instance=[TPI_BASE::CTRL_HMI_X_K_TESTPLANENDFLOW_X_X_X_X_ApplyBin] executed using 47.167000 ms
[2024-Mar-26 07:06:08.213][DUT: 1]TestInstance=[TPI_BASE::CTRL_HMI_X_K_TESTPLANENDFLOW_X_X_X_X_ApplyBin] exit port=[1].
[2024-Mar-26 07:06:08.227][DUT: 1]Smart test condition has been disabled
```

## Console Result

```
[2024-Mar-26 07:06:08.273][HdmtOS] DUT :1 DutEndTest completed with SUCCESS  SoftBin = 4440000, HardBin = 4, TopBin = 0, ExitPort = 1
[2024-Mar-26 07:06:08.273][A][TAL][DUT: 1] DUT 1: DutEndTest completed with SUCCESS  SoftBin = 4440000, HardBin = 4, TopBin = 0, ExitPort = 1
```