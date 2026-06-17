**Prime Test-Method Specification REP**

[[_TOC_]]

## REP for SortBinTrace Test Method

This **REP** is intended to describe the behavior and features of the SortBinTrace Prime Test Method.

# Introduction

The SortBinTrace Test Method aims to provide the infrastructure and data requirements needed to support Binning Methodologies used in Sort. These Methodologies make use of certain data from failing instances, namely: instance name, exit port, bin number and whether the instance was bypassed or not in order to compute a final bin. These four data items will be collectively referred to as "BinTrace Data" for any given instance.

BinTrace Data is accumulated in containers commonly called "BinTrace Arrays" where an array typically represents a Subflow in the Test Program. Therefore a BinTrace Array will contain all captured BinTrace Data for a particular Subflow of the Test Program.

The SortBinTrace Test Method provides the functionality to turn on or off BinTrace Data capture for a given BinTrace Array.

**Important Note**: Even though the SortBinTrace Test Method and BinTrace Service provide the funcionality to capture BinTrace Data from EVG test instances in a Hybrid TP, EVG's BinTrace cannot be turned on at the same time as Prime's BinTrace. If the user enables both BinTraces simultaneously, then Prime's BinTrace will take precedence and EVG's Bintrace will be turned off.

# User Interface Parameters

The table below lists and describes the user interface parameters provided by the SortBinTrace Test Method.

| **Parameter Name** | **Mandatory?** | **Type** | **Description** |
|--------------------|----------------|----------|-----------------|
|BinTraceMode        |Yes             |String (Choice) |Sets the BinTrace Mode.<br/>Available modes are "On" and "Off".|
|BinTraceName        |No              |String |Specifies the name of the BinTrace Array to store BinTrace Data into. <br/>This parameter is required only when *BinTraceMode* == On.|

# SortBinTrace Test Method Exit Ports

The SortBinTrace Test Method supports the following exit ports.

| **Port Number** | **Exit Condition** | **Description** |
|-----------------|--------------------|-----------------|
|**-2**           |***Alarm***         |Any hardware alarm.|
|**-1**           |***Error***         |Any software error or software execution failure.|
|**0**            |***Fail***          |Test Method failing condition.|
|**1**            |***Pass***          |Test Method passing condition.|

# BinTrace Modes

## BinTrace Mode On

When parameter *BinTraceMode* is set to "On" the Test Method will enable BinTrace Data capture for the specified *BinTraceName*. For this mode, parameter *BinTraceName* is mandatory, if a BinTrace name is not provided the instance will fail on Verify.

As soon as *BinTraceMode* is set to "On" the BinTrace Array will also be initialized with an empty container, this is done to differentiate empty arrays, where nothing was captured, from arrays that were not initialized at all.

Once BinTrace is on, at the end of execution of every instance, whether from EVG or Prime, the BinTrace Data for that instance will be saved into the active BinTrace Array provided that is a failing instance for which a Bin Number was assigned to the Exit Port, otherwise the data for that instance will be ignored.

Only one BinTrace Array may be active at any given time, so the Test Method will throw an exception if BinTrace is set to On while another BinTrace Array is already enabled.

All BinTrace Data will be stored into Prime's Shared Storage with a "DUT" Storage Context.

## BinTrace Mode Off

When parameter *BinTraceMode* is set to "Off" the Test Method will disable BinTrace Data capture for the active BinTrace Array. On this mode, parameter *BinTraceName* is not required and will in fact be ignored, since there can only be one BinTrace Array active at any given time then the active one will be disabled.

# Acronyms

Definition of acronyms used in this document:

  - **DUT**: Device Under Test
  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TP**: Test Program
