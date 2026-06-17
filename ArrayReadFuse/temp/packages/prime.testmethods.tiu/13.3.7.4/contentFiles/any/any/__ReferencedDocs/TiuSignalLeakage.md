**Prime Test-Method Specification REP**

November 2023

[[_TOC_]]

# Introduction

The main purpose of this Test Method is to validate that the signal pins on the TIU are in good condition for the test to be executed. This is achieved by performing DC current measurements on the specified pins and comparing the results against provided limits to detect any leakage.

# Methodology

The `PrimeTiuSignalPinLeakageTestMethod` will execute DC current measurements on the pins specified in the `Pins` parameter. If the `HighLimit` and `LowLimit` parameters are specified, the measured values will be compared against these limits. If any measurement is outside the specified limits, the test method will exit through port 0, indicating a failure. This test method supports only the pins listed in the resources below:    

- DPIN
- ENABLEDCLOCK
- DIFFERENTALENABLEDCLOCK
- KEEPMODEPIN
- HBIDPIN
- HBIENABLEDCLOCK

The DC measurement is performed without Smart TC, and the pins specified for the measurement must be in VSIM mode.

## Example of a Level Block
```javascript
Version 1.0;

Levels TiuValidationDpin_Lvl
{
    DomainA_DPIN_PMU
    {
        PinModeSel = "PMU";
        OPMode = "VSIM";
        VForce = 1V; 
        IRange = "IR40mA"; 
        IClampHi = 40mA; 
        IClampLo = -40mA;
    }
    SequenceBreak 1mS;
    xxHPCC_DPIN_PMU_slcA_50ohm1
    {
        # Mandatory Parameters for measurement.
        StartMeasurement = True;     
        PreMeasurementDelay = 5mS;   
        SamplingCount = 1; 
        SamplingRatio = 1;       
        SamplingMode = "Trace";
    }
}        
```


# Test Instance Parameters

| **Parameter Name**    | **Required?**    | **Type**        | **Values**                      |
| --------------------- | ---------------- | --------------- | ------------------------------- |
| Pins         | Yes              | String |  Comma separated list of pins to be measured.            |
| HighLimit         | No              | String |  Comma separated list of upper limits for the measurements. The number of limits must match the number of Pins. Each value must be higher than the corresponding LowLimit.            |
| LowLimit         | No              | String |  Comma separated list of lower limits for the measurements. The number of limits must match the number of Pins. Each value must be lower than the corresponding HighLimit.            |
| LevelsTc         | Yes              | String |  Level Test Condition Block to be used for DC measurement.          |


# Custom User Code Hooks

No user extendable hooks are provided.


# TPL Samples

Here are a few test instance examples using the PrimeTiuSignalPinLeakageTestMethod.

```javascript
Test PrimeTiuSignalPinLeakageTestMethod TiuSignalPinCurrentWithinLimit_P1
{
   LogLevel = "PRIME_DEBUG";
   HighLimit = "0.5,0.5,0.5";
   LevelsTc = "TIUSIGNALPINLEAK::TiuValidationDpin_TC"; 
   LowLimit = "0.1,0.1,0.1"; 
   Pins = "xxHPCC_DPIN_PMU_slcA_50ohm1,HPCC_DPIN_PMU_all_100ohm,xxHPCC_DPIN_PMU_slcB_50ohm3";
}
```

# Sample of Measurement Result Printout
```javascript
[DUT: 1]************************************************************
[DUT: 1]	TIU Signal Pin Measurement Result.
[DUT: 1]************************************************************
[DUT: 1]--------------------------------------------------------------------------------
[DUT: 1]| PinGroup                       | Pin                            | Current(A) |
[DUT: 1]--------------------------------------------------------------------------------
[DUT: 1]|                                | xxHPCC_DPIN_PMU_slcA_50ohm1    |  0.3000000 |
[DUT: 1]|                                | xxHPCC_DPIN_PMU_slcB_50ohm3    |  0.3000000 |
[DUT: 1]| HPCC_DPIN_PMU_all_100ohm       | xxHPCC_DPIN_PMU_slcA_100ohm    |  0.3000000 |
[DUT: 1]| HPCC_DPIN_PMU_all_100ohm       | xxHPCC_DPIN_PMU_slcB_100ohm    |  0.3000000 |
```


# Exit Ports

The PrimeTiuSignalPinLeakageTestMethod supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                                    |
| ------------- | ------------- | ---------------------------------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                                                                |
| **-1**        | ***Error***   | Any software condition error                                                       |
| **0**         | ***Fail***    | Failing condition. Measurement is not within limits.          |
| **1**         | ***Pass***    | Passing condition. Measurement is within limits. |


# Version tracking

| Prime Version     | **Author** | Prime ticket reference | **Comments** |
| ----------------- | ---------- | ---------------------- | ------------ |
| 8.01.00           | Yusof, Adam Malik     |                 | 1st version |
| 12.02.02          | Teoh, Khai Jie        | #39827          | include fail pin name and fail channel printing to ituff. |
| 12.03.00          | Teoh, Khai Jie        | #46008          | Disable Witch Project '2_tname_failchannel' and '3_binpinfails' printing to ituff. |
| 13.01.00          | Pinto Rosales, Raquel |                 | Enhance documentation. |


# Acronyms

Definition of acronyms used in this document:

  - **DUT**: Device Under Testing
  - **TIU**: Test Interface Unit
  - **HDMT**: High Density Modular Tester
  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TPL**: Test Programming Language