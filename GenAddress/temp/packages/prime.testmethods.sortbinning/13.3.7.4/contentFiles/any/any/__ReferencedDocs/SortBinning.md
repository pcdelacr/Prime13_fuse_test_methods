**Prime Test-Method Specification REP**

[[_TOC_]]

## REP for the SortBinning Suite of Test Methods

This **REP** is intended to introduce the Suite of Prime Test Methods for the Sort Binning Methodology.

# Introduction

This Suite of Test Methods is intended to be used in Sort Test Programs where Forced Flow Methodology is used.

# SortBinning Test Methods
The following is a list of all available Test Methods. Please refer to each Test Method's REP for more details.

## SortBinTrace Test Method
Provides the infrastructure and data requirements needed to support Binning Methodologies used in Sort. All other Test Methods of this Suite depend on the proper usage of this Test Method.

## SortApplyBin Test Method
Provides the ability to write a bin to the tester. The bin to be set must exist in the bdef, must not be bin 92, and must not be reserved by a TOS alarm.

## SortCheckpoint Test Method
Finds the first Fail Bin (if any) in the specified BinTrace Arrays and stores it in a Shared Storage Token.

## SortSetBin Test Method
Computes the final bin with the data stored in BinTrace Arrays and a configuration file and sets that final bin into TOS.

## SortCheckBin Test Method
Computes the final bin with the data stored in BinTrace Arrays and a configuration file but does **not** set that final bin into TOS.

## SortTBin Test Method
Computes the total bin based on up to six incoming temperature bins stored in Shared Storage. Also sets that total bin into TOS.

## SortSdm92Binning
Computes 92XX rebinning of units in the SDM flow by evaluating cold and hot test results, stress indicators, and bin traces. 

# EVG to Prime Migration Reference
The following table outlines which Prime Test Methods are equivalent to legacy EVG modes of the Ultrabinner Template for TP migration purposes.

|EVG Ultrabinner Mode|Prime Test Method Replacement|
|--------------------|--------------------|
|Read Config|Removed|
|Set Initial|Removed|
|CheckPoint|PrimeSortCheckpointTestMethod|
||PrimeSortCheckbinTestMethod|
|Set Bin|PrimeSortSetBinTestMethod|

# Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TP**: Test Program
