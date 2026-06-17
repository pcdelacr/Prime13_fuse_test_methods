## REP for VirtualFuseReset

[[_TOC_]]

This **REP** is intended to describe the VirtualFuseReset Prime TestMethod.

## Methodology

This TestMethod will reset all fuses data, under the given namespace that is provided in instance parameter 'Namespace'.

### Verify

Test method will get the fuse handler of the given namespace from 'Namespace' parameter, if 'Namespace' is not found verify will fail.

### Execute

Test method will clear all fuse data stored under the given namespace from 'Namespace' parameter.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the VirtualFuseReset test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                                                                           | **Comments**                 |
| ------------------ | ------------- | --------------- | ------------------------------------------------------------------------------------ | ---------------------------- |
| Namespace          | Yes           | String          |  Name of namespace of fuses data to be cleared.                                      |

## TPL Samples
```
Test PrimeVirtualFuseResetTestMethod ResetVirtualFuse_P1
{
    LogLevel = "PRIME_DEBUG";
	Namespace = "U1";
}
```

## Exit Ports

The VirtualFuseReset test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**                              |
| ------------- | ------------- | -------------------------------------------- |
| **0**         | ***Fail***    | Failing condition. 		                   |
| **1**         | ***Pass***    | Pass case. Fuses cleared successfully under namespace.   |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SHOPS**: **Sh**orts and **Op**en**s** test methodology

## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**    |
| ------------------------- | ----------- | ----------------- | --------------- |
| June 29<sup>th</sup>, 2022 | 1.0.0       | Dakwar, Wajde    | Initial version |
