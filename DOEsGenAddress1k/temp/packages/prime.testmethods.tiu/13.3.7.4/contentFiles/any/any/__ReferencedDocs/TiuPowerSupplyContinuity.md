**Prime Test-Method Specification REP**

November 2023

[[_TOC_]]

# Introduction

The main purpose of this Test Method is to validate that the digital power supply (DPS) pins on the TIU are in good condition, ensuring that the tests can be executed properly.


# Methodology

The `PrimeTIUPowerSupplyContinuityTestMethod` will execute DC current measurements on the pins specified in the `Pins` parameter. If `HighLimit` and `LowLimit` are specified, the measurement will be compared against these limits. If the measurement falls outside the specified limits, the test method will exit through port 0. This test method supports only the pins listed in the resources below:  

- LCDPS
- HCDPS
- LC
- HC
- VLC
- HBILC
- HBIHC

The DC measurement is performed without Smart TC, and the pins specified for the measurement must be in VSIM mode.

## Example of a Level Block

```javascript
Version 1.0;

Levels TiuValidationDps_Lvl
{
    all_HC   
    {
        OPMode = "VSIM";
        VForce = 1V; 
        IRange = "IR24A"; 
        IClampHi = 10A; 
        IClampLo = -10A;
        FreeDriveTime = 0.065;   # 
        VSlewStepRatio = 127;
        PowerSequence = True;
    }
    SequenceBreak 1mS;
    HDDPS_HC_nogang_12ohm2
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

Here is a test instance example using the PrimeTiuPowerSupplyContinuityTestMethod.

```javascript
Test PrimeTiuPowerSupplyContinuityTestMethod TiuDpsPinCurrentWithinLimit_P1
{
	LogLevel = "PRIME_DEBUG";
    HighLimit = "0.5,0.5,0.5";
	LevelsTc = "TIUPOWERSUPPLYCONTINUITY::TiuValidationDps_TC"; 
	LowLimit = "0.1,0.1,0.1"; 
	Pins = "HDDPS_HC_nogang_12ohm2,all_LC_5ohm,HDDPS_HC_nogang_5ohm2";
}
```

# Sample of Measurement Result Printout
```javascript
[2022-Feb-25 17:21:14.920][DUT: 1]************************************************************
[2022-Feb-25 17:21:14.920][DUT: 1]	TIU Pin Measurement Result.
[2022-Feb-25 17:21:14.920][DUT: 1]************************************************************
[2022-Feb-25 17:21:14.920][DUT: 1]--------------------------------------------------------------------------------
[2022-Feb-25 17:21:14.920][DUT: 1]| PinGroup                       | Pin                            | Current(A) |
[2022-Feb-25 17:21:14.920][DUT: 1]--------------------------------------------------------------------------------
[2022-Feb-25 17:21:14.920][DUT: 1]|                                | HDDPS_HC_nogang_12ohm2         |  0.3000000 |
[2022-Feb-25 17:21:14.920][DUT: 1]|                                | HDDPS_HC_nogang_5ohm2          |  0.3000000 |
[2022-Feb-25 17:21:14.920][DUT: 1]| all_LC_5ohm                    | HDDPS_LC_nogang_5ohm1          |  0.3000000 |
[2022-Feb-25 17:21:14.920][DUT: 1]| all_LC_5ohm                    | HDDPS_LC_nogang_5ohm2          |  0.3000000 |
```


# Exit Ports

The PrimeTiuPowerSupplyContinuityTestMethod supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                                    |
| ------------- | ------------- | ---------------------------------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition.                                                                |
| **-1**        | ***Error***   | Any software condition error.                                                       |
| **0**         | ***Fail***    | Failing condition. Measurement is not within limits.          |
| **1**         | ***Pass***    | Passing condition. Measurement is within limits. |


# Version tracking

| Prime Version     | **Author** | Prime ticket reference | **Comments** |
| ----------------- | ---------- | ---------------------- | ------------ |
| 8.02.00           | Yusof, Adam Malik     |                 | 1st version |
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