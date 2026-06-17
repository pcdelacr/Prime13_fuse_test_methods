**Prime Test-Method Specification REP**

[[_TOC_]]

## REP for SortCheckpoint Test Method

This **REP** is intended to describe the behavior and features of the SortCheckpoint Prime Test Method.

# Introduction

The SortCheckpoint Test Method takes the BinTrace Arrays that were provided (via the *BinTracesToCheck* parameter) and traverses them in the order they were given seeking for a bad bin. As soon as the first bad bin is found, the bin number is stored into the Shared Storage as a String under the key "CHECKPOINT_BIN" with a "DUT" Storage Context and the Test Method exits through Port 2.

If no bad bin is found then the Test Method will exit through Port 1.

If any of the provided BinTrace Arrays are not initialized (with a *BinTraceMode* == 'On' of the SortBinTrace Test Method) by the time their content is attempted to be read, then execution will stop and the Test Method will exit through Port 0.

The SortCheckpoint Test Method can be used, for example, to determine if the Die should be Stressed, where if a bad bin exists on a particular Test Program Flow, then Stress is skipped.

This Test Method can be used at any point during the Main flow and does not require a configuration file. There is no limitation to running the Checkpoint algorithm unto a BinTrace Array that is still open.

# User Interface Parameters

The table below lists and describes the user interface parameters provided by the SortCheckpoint Test Method.

| **Parameter Name** | **Mandatory?** | **Type** | **Description** |
|--------------------|----------------|----------|-----------------|
| BinTracesToCheck   | Yes            | Comma-Separated String| Specifies the BinTrace Array(s), separated by a comma, that should be checked during Checkpoint execution.|

# SortCheckpoint Test Method Exit Ports

The SortCheckpoint Test Method supports the following exit ports.

| **Port Number** | **Exit Condition** | **Description** |
|-----------------|--------------------|-----------------|
|**-2**           |***Alarm***         |Any hardware alarm.|
|**-1**           |***Error***         |Any software error or software execution failure.|
|**0**            |***Fail***          |One or more BinTrace Arrays was not initialized.|
|**1**            |***Pass***          |No Bad Bin found.|
|**2**            |***Pass***          |Bad Bin found.|

# Ituff Output

This Test Method does **not** produce any ituff output.

# Acronyms

Definition of acronyms used in this document:

  - **DUT**: Device Under Test
  - **REP**: P**r**ime T**e**st-Method S**p**ecification