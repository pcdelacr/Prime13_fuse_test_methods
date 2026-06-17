[[_TOC_]]

# REP for PrimeSortVccContinuityTestMethod

## Important Notes
This Test Method has been recently adopted into Prime from User Code.

Current Unit Test coverage is 87%.

----
## Introduction
This test class performs the Vcc continuity test for functional and structural sort (die) test flows. The test method allows both serial and parallel testing as well as identifying failing power rails without requiring a fail flow. It provides various test modes and configurations to benefit test time, handle multiple ConfigSets for different parameters, and facilitate debug. Results of this test can be stored in SharedStorage, ensuring they are accessible for further analysis in MIM Cap and ADTL screens.

----
## Test Method Modes

This test method provides 3 different modes to be used in different contexts.

### a. Level_Mode

- In this mode, the level Test Condition is executed to force voltage/current on power supplies and measure current/voltage based on the StartMeasurement attribute.
  Depending on the level setup, the measurement is performed in parallel or serial testing.
- Results of measurement will be compared against limits set up in JSON ConfigFile at corresponding ConfigSet.
- Binning as the first failed in both cases: limit fail and clamp fail. The port of fail is based on the FailPort defined in JSON file.

### b. DC_Mode

- In this mode, typical power down level on all supplies is used. The test class will use this level information, the force value and other pin attributes in JSON file to apply
  on the measured pin. Each measured pin is being set up, measured, and then reset to 0V sequentially.
- Results of measurement will be compared against limits set up and binning on the first rail failed.
- Binning as the first failed in both cases: limit fail and clamp fail. The port of fail is based on the FailPort defined in JSON file.

### c. Characterization

- This test mode operates the same as DC_Mode with force value swept from start value to stop value in incremental step.
- No limit kill in characterization mode. When clamp happens, it will apply level power down from the LevelsTc parameter then continue on the next supply.
- When other alarms occur, it will exit on the alarm port.

----
## Test Instance Parameters
| Parameter Name      | Required? | Type   | Description                                                                                     | Options            | Default   |
|---------------------|-----------|--------|-------------------------------------------------------------------------------------------------|--------------------|-----------|
| LevelsTc            | Yes       | Level  | Sets LevelsTc to use                                                                            |                    |           |
| ConfigFile          | Yes       | String | JSON file path                                                                                  |                    |           |
| ConfigSet           | Yes       | String | Configuration to be used                                                                        |                    |           |
| TestMode            | No        | String | Test mode to execute                                                          | LEVEL_MODE, DC_MODE, CHARACTERIZATION |LEVEL_MODE |
| SharedStoragePrefix | No        | String | Sets a prefix for storing the results in shared storage. If not set results will not be stored. |                    |           |
| AlarmLevelsTc       | No        | Level  | LevelsTc to use to clear alarms when AlarmPortRedirect enabled                                  |                    |           |
| AlarmPortRedirect   | No        | String | Sets the alarm redirect from port -2 to port 2                                                  | DISABLED/ENABLED   | ENABLED   |

----
## JSON File Parameters
| Parameter Name       | LEVEL_MODE | DC_MODE | CHARACTERIZATION | Type             | Description                                           | Default Value | Comment                                  |
|----------------------|------------|---------|------------------|------------------|-------------------------------------------------------|---------------|------------------------------------------|
| FailPort             | YES        | YES     | YES              | Integer          | Specifies the intended fail port exit for the pin block. | Blank         | Fail port must be in the range of 3-99.   |
| Pin                  | YES        | YES     | YES              | String           |                                                       | Default       |                                          |
| LowerLimit           | YES        | YES     | NO               | Double           |                                                       | -9999         |                                          |
| UpperLimit           | YES        | YES     | NO               | Double           |                                                       | -9999         |                                          |
| ClampHi              | NO         | YES     | YES              | Double           |                                                       | -9999         |                                          |
| ClampLo              | NO         | YES     | YES              | Double           |                                                       | -9999         |                                          |
| Force                | NO         | YES     | NO               | Double           |                                                       | -9999         |                                          |
| MeasurementRange     | NO         | YES     | YES              | String           |                                                       | Blank         |                                          |
| PreMeasurementDelay  | NO         | NO      | NO               | Double           |                                                       | 0.005         |                                          |
| Type                 | NO         | NO      | NO               | MeasurementType  |                                                       | Current       | Options: Current, Voltage                |
| CharStart            | NO         | NO      | YES              | Double           |                                                       | -9999         |                                          |
| CharStop             | NO         | NO      | YES              | Double           |                                                       | -9999         |                                          |
| CharStep             | NO         | NO      | YES              | Double           |                                                       | -9999         |                                          |
| FreeDriveTime        | NO         | YES     | YES              | Double           |                                                       | Blank         |                                          |

----
## TPL Samples
Examples of test instance set up

```
CSharpTest PrimeSortVccContinuityTestMethod CONT_PARALLEL_ALLDPS_E_START_X_X_X_X_LEVELMODE
{
    ConfigFile = "./Modules/TPI_VCC/InputFiles/vcc_start.json";
    ConfigSet = "COLD_START_LEVEL";
    LevelsTc = "BASE::lvl_vcc_parallel_pwrdown";
    TestMode = "LEVEL_MODE";
    SharedStoragePrefix = "VCC_PARALLEL_START_LEVELMODE";
    AlarmPortRedirect = "ENABLED";
    AlarmLevelsTc = "BASE::dsp_setup_basic_allps";
}

CSharpTest PrimeSortVccContinuityTestMethod CONT_SERIAL_ALLDPS_E_START_X_X_X_X_DCMODE
{
    ConfigFile = "./Modules/TPI_VCC/InputFiles/vcc_start.json";
    ConfigSet = "COLD_START_DCMODE";
    LevelsTc = "BASE::lvl_vcc_pwrdown";
    TestMode = "DC_MODE";
}
```

----
## File syntax
Json file set up for each test modes.

```
{
  "ConfigSets": {
    "COLD_START_LEVEL": [
      {
        "FailPort": 3,      // Example of LEVEL_MODE
        "Pin": "VCC_SDX_IO_HC",
        "LowerLimit": -0.1,
        "UpperLimit": 0.15
      },
      {
        "FailPort": 4,
        "Pin": "VCC_SDX_CFC_HC",
        "LowerLimit": -0.1,
        "UpperLimit": 0.15
      }
    ],
    "COLD_START_DCMODE": [
      {
        "FailPort": 3,      // Example of DC_MODE
        "Pin": "VCC_SDX_CORE_36_35_26_25_HV",
        "LowerLimit": -0.1,
        "UpperLimit": 0.24,
        "ClampHi": 0.5,
        "ClampLo": -0.5,
        "Force": 0.1,
        "MeasurementRange": "IR500mA",
        "FreeDriveTime": "10mS",
        "PreMeasurementDelay": 0.01    // Option
      }
    ],
    "DC_MODE_ISVM": [
     {
        "FailPort": 3,     // Example of DC_MODE using ISVM
        "Pin": "VCC_SDX_CORE_16_15_06_05_HV",
        "LowerLimit": -0.1,
        "UpperLimit": 0.24,
        "ClampHi": 0.5,
        "ClampLo": -0.5,
        "Force": 0.005,
        "Type": "Voltage",
        "MeasurementRange": "VR6V",
        "PreMeasurementDelay": 0.01
      }
    ],
    "COLD_CHAR": [
      {
        "FailPort": 3,     // Example of CHARACTERIZATION
        "Pin": "VCC_SDX_IO_HC",
        "ClampHi": 0.5,
        "ClampLo": -0.5,
        "CharStart": 0.02,
        "CharStop": 0.5,
        "CharStep": 0.02,
        "MeasurementRange": "IR500mA",
        "PreMeasurementDelay": 0.015     // Option
      }
    ]
  }
}
```
----

## Levels Setup
In LEVEL_MODE, LevelsTC is used to set the force voltage, trigger the measurement and return power supply to 0V.

In DC_MODE and CHARACTERIZATION, LevelsTC is the power down all power rails.

Example level for LEVEL_MODE
```
Levels lvl_vcc_serial_hc
{
    .................                # initiate
    SequenceBreak 0mS;
    VCC_SDX_CORE_34_24_14_HC        # force value and measure
    {
        FreeDriveTime = 3mS;
        IClampHi = 500mA;
        IClampLo = -500mA;
        IRange = "IR500mA";
        OPModeCheck = "VSIM";
        PreMeasurementDelay = 10mS;
        SamplingCount = 1;
        SamplingMode = "Trace";
        SamplingRatio = 1;
        StartMeasurement = TRUE;
        VForce = 0.1V;
        VSlewStepRatio = 127;
    }
    SequenceBreak 10mS;
    VCC_SDX_CORE_34_24_14_HC         # return power rail to 0V
    {
        FreeDriveTime = 3mS;
        IClampHi = 500mA;
        IClampLo = -500mA;
        IRange = "IR500mA";
        OPModeCheck = "VSIM";
        VForce = 0V;
        VSlewStepRatio = 127;
    }
    VCC_SDX_CORE_66_65_HC            # continue with other rails
    {
        FreeDriveTime = 3mS;
        IClampHi = 500mA;
        IClampLo = -500mA;
        IRange = "IR500mA";
        OPModeCheck = "VSIM";
        PreMeasurementDelay = 10mS;
        SamplingCount = 1;
        SamplingMode = "Trace";
        SamplingRatio = 1;
        StartMeasurement = TRUE;
        VForce = 0.1V;
        VSlewStepRatio = 127;
    }
    SequenceBreak 10mS;
    VCC_SDX_CORE_66_65_HC
    {
        FreeDriveTime = 3mS;
        IClampHi = 500mA;
        IClampLo = -500mA;
        IRange = "IR500mA";
        OPModeCheck = "VSIM";
        VForce = 0V;
        VSlewStepRatio = 127;
    }
    ................
}
```
Example level for DC_MODE and CHARACTERIZATION
```
Levels lvl_vcc
{
    .................                # initiate
    SequenceBreak 0mS;
    VCC_SDX_CORE_16_15_06_05_HV        # Power down all rails to 0V
    {
        FreeDriveTime = 3mS;
        IClampHi = 500mA;
        IClampLo = -500mA;
        IRange = "IR500mA";
        OPMode = "VSIM";
        PowerSequence = TRUE;
        VForce = 0V;
        VRange = "VR6V";
        VSlewStepRatio = 127;
    }
    VCC_SDX_CORE_66_65_HC
    {
        FreeDriveTime = 3mS;
        IClampHi = 500mA;
        IClampLo = -500mA;
        IRange = "IR500mA";
        OPMode = "VSIM";
        PowerSequence = TRUE;
        VForce = 0V;
        VSlewStepRatio = 127;
    }
}
```
----
## Datalog output
Measurement is in 9 precision.

Passting test token format:
```
2_tname_[TestName]_[Pin]
2_mrslt_[Measurement]
2_msunit_[MeasurementUnit]
```

```
2_tname_TPI_VCC::CONT_PARALLEL_ALLDPS_E_START_X_X_X_X_LEVELMODE_VCC_SDX_CFC_HC
2_mrslt_0.009355554
2_msunit_A
```

Failing test token format:

```
2_tname_[TestName]_[Pin]_FAILED
2_strgval_[Pin]|[Measurement]|[LowerLimit]|[UpperLimit]
```

```
2_tname_TPI_VCC::CONT_PARALLEL_ALLDPS_E_START_X_X_X_X_LEVELMODE_VCC_SDX_CORE_16_15_06_05_HV_FAILED
2_strgval_VCC_SDX_CORE_16_15_06_05_HV|0.002983478|-0.49|0.0001
```

Charaterization data format:
```
2_tname_[TestName]_[Pin]_[ForceValue]
2_mrslt_[Measurement]
```

```
2_tname_TPI_VCC::CONT_SERIAL_ALLDPS_E_START_X_X_X_X_CHARMODE_VCC_SDX_CORE_16_15_06_05_HV_0
2_mrslt_-0.000270089
2_msunit_A
2_tname_TPI_VCC::CONT_SERIAL_ALLDPS_E_START_X_X_X_X_CHARMODE_VCC_SDX_CORE_16_15_06_05_HV_0.02
2_mrslt_0.000261028
2_msunit_A
2_tname_TPI_VCC::CONT_SERIAL_ALLDPS_E_START_X_X_X_X_CHARMODE_VCC_SDX_CORE_16_15_06_05_HV_0.04
2_mrslt_0.001294127
2_msunit_A
```

Clamp information:

```
2_tname_HWALARM_TPI_VCC::CONT_SERIAL_ALLDPS_E_START_X_X_X_X_DCMODE|Slot:5|Pin:VCC_SDX_CORE_16_15_06_05_HV|Type:DPSCurrentClamp
2_strgval_Module:HV|Channel:5000|Time:20240725072736
```
----

## Exit Ports
| Exit Port | Condition | Description                      |
|-----------|-----------|----------------------------------|
| -2        | Alarm     | PORT FAIL FOR HW ALARM CONDITION |
| -1        | Error     | PORT FAIL FOR SW ERROR CONDITION |
| 0         | Fail      | FAIL PORT                        |
| 1         | Pass      | PASS PORT                        |
| 2         | Fail      | ALARM PORT                       |
| 3 - 99    | Fail      | FAIL PORT for limit and Clamp       |

----