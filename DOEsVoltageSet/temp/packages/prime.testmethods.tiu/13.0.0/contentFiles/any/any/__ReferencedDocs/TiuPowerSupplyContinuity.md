**Prime Test-Method Specification REP**

November 2023

[[_TOC_]]

# Introduction

The main purpose of this Test Method is to validate the digital power supply pins (DPS) on the TIU is in good condition for test to be executed.

# Methodology

TIU Power Supply Continuity Test Method Test Method will execute DC current measurements on specified pins in the parameter "Pins". If "HighLimit" and "LowLimit" is specified, the measurement will be compared against the limits; if it's out of the limits, the test method will exit through port 0. The pins that are supported by this test method is only from the list of resources below:-

- LCDPS
- HCDPS
- LC
- HC
- VLC
- HBILC
- HBIHC

The DC measurement is done without Smart TC. The pins specified for the measurement must be in VSIM mode.

## Example of level block
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
| HighLimit         | No              | String |  Comma separated list of upper limits for the measurement to be compared against. The count must match with the Pins count. Value must be higher than LowLimit.            |
| LowLimit         | No              | String |  Comma separated list of lower limits for the measurement to be compared against. The count must match with the Pins count. Value must be lower than HighLimit.            |
| LevelsTc         | Yes              | String |  Level Test Condition Block to be used for DC measurement.          |

# Custom User Code Hooks

No user extendable hook is provided.

# TPL Samples

Here is a test instance example using the PrimeTiuPowerSupplyContinuityTestMethod.

```javascript
Import PrimeTiuPowerSupplyContinuityTestMethod.xml;

Test PrimeTiuPowerSupplyContinuityTestMethod TiuDpsPinCurrentWithinLimit_P1
{
	LogLevel = "PRIME_DEBUG";
    HighLimit = "0.5,0.5,0.5";
	LevelsTc = "TIUPOWERSUPPLYCONTINUITY::TiuValidationDps_TC"; 
	LowLimit = "0.1,0.1,0.1"; 
	Pins = "HDDPS_HC_nogang_12ohm2,all_LC_5ohm,HDDPS_HC_nogang_5ohm2";
}
```

# Sample of Measurement Result Print Out.
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

The PrimeTiuPowerSupplyContinuityTestMethod test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                                    |
| ------------- | ------------- | ---------------------------------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                                                                |
| **-1**        | ***Error***   | Any software condition error                                                       |
| **0**         | ***Fail***    | Failing condition. Measurement is not within limit.          |
| **1**         | ***Pass***    | Passing condition. Measurement is within limit.. |

# Version tracking

| Prime Version     | **Author** | Prime ticket reference | **Comments** |
| ----------------- | ---------- | ---------------------- | ------------ |
| 12.03.00          | Khai Jie, Teoh    | #46008          | Disable Witch Project '2_tname_failchannel' and '3_binpinfails' printing to ituff. |
| 12.02.02          | Teoh, Khai Jie    | #39827          | include fail pin name and fail channel printing to ituff. |
| 8.02.00           | Yusof, Adam Malik |                 | 1st version |

# Acronyms

Definition of acronyms used in this document:

  - **DUT**: Device Under Testing
  - **TIU**: Test Interface Unit
  - **HDMT**: High Density Modular Tester
  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TPL**: Test Programming Language