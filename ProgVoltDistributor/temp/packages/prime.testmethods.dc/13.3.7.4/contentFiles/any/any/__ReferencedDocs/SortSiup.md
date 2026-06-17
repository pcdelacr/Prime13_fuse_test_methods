# SortSiup Documentation

----

## Introduction
This Test Method used for SIU Probe Protection testing. It enhances the TriggerDc method provided by MIT to include Shared Storage, config file capabilities, per pin fail port abilities, and thermal measurements.

----

## Test Instance Parameters

Parameter Name | Required? | Type | Description | Options | Default
---- | ---- | ---- | ---- | ---- | ----
Patlist | YES | Plist | Defines the pattern for PVSiupBase to run. |  | Empty string
LevelsTc | YES | LevelsCondition | LevelsTc that runs before the pattern. |  | Empty string
TimingsTc | YES | TimingsCondition | Set TimingsTc to use |  | Empty string
Pins | NO | CommaSeparatedString | Pins to take measurements from. |  | Empty string
MeasurementTypes | NO | CommaSeparatedString | Whether to measure current or voltage. Depends on levels settings. | Current(C) or Voltage(V) | Empty string
LowLimits | NO | CommaSeparatedString | Low limits for the pins under which to cause a limit failure. |  | Empty string
HighLimits | NO | CommaSeparatedString | High limits for the pins over which to cause a limit failure. |  | Empty string
TriggerMapName | NO | String | Trigger map to run hardware level triggers from the pattern. |  | Empty string
AlarmLevelsTc | NO | LevelsCondition | LevelsTc to use after an alarm when AlarmPortRedirect is enabled |  | Empty string
AlarmPortRedirect | NO | EnableDisable | If Enabled, Alarm will redirect out alarm redirect port. | EnabledDisabled | Enabled
LevelsToModify | NO | String | Levels test condition to modify. |  | Empty string
ClampHigh | NO | CommaSeparatedString | High clamps in levels test condition for pins to set. |  | Empty string
ClampLow | NO | CommaSeparatedString | Low clamps in levels test condition for pins to set. |  | Empty string
ClampPinsToModify | NO | CommaSeparatedString | Pins in levels test condition to modify. |  | Empty string
ModifyIRange | NO | CommaSeparatedString | IRange in levels test condition for pins to set. |  | Empty string
CompressItuffLines | NO | EnableDisable | Will compress ituff lines to Prime DEFLATE32 format | EnabledDisabled | Disabled
WrapItuffLines | NO | EnableDisable | Toggle for whether iTUFF lines should wrap after the hardcoded limit | EnabledDisabled | Enabled
ConfigFile | NO | String | Config file for pins and limits. |  | Empty string
ConfigSet | NO unless ConfigFile | String | Config set to run if ConfigFile is defined. |  | Empty string
DTSEnableDisable | NO | EnableDisable | Whether to take DTS measurements or not.  | EnabledDisabled | Disabled
DTSPins | NO unless DTSEnableDisable | CommaSeparatedString | For DTS thermal measurement. Pins to get DTS data from. |  | Empty string
EnableFlushSmartTc | NO | EnableDisable | If Enabled, SmartTc cache will be flushed prior to running PVSiupBase. | EnabledDisabled | Disabled
EnablePerRailFailPorts | NO | EnableDisable | Enabled Per Pin fail ports. | EnabledDisabled | Disabled
MaskPins | NO | CommaSeparatedString | Mask pins to mask out pattern errors coming from a particular pin. |  | Empty string
PrePlist | NO | String | Defines a plist to run prior to the main pattern. |  | Empty string
SharedStoragePrefix | NO | String | Prefix for storing the results in shared storage.  If not set result(s) will not be stored. |  | Empty string

----

## JSON File Parameters

Parameter Name | Type | Description
---- | ---- | ----
MaskPins | CommaSeparatedString | See Test Instance Parameters
TriggerMapName | String | See Test Instance Parameters
SharedStoragePrefix | String | See Test Instance Parameters
DTSPins | CommaSeparatedString | See Test Instance Parameters
DTSEnableDisable | EnableDisable | See Test Instance Parameters
| | 
| **Pins**
MeasurementType | String | Current(C) or Voltage(V) measurement
LowLimit | String | Low limit, including A or V for current or voltage
HighLimit | String | Low limit, including A or V for current or voltage
| | 
| **LevelsToModify**
IClampLo | String | Low clamp, including A for current
IClampHi | String | High clamp, including A for current
IRange | String | IRange of the current clamp
VClampLo | String | Low clamp, including V for voltage
VClampHi | String | High clamp, including V for voltage
VRange | String | Vrange of the voltage clamp

Notes:
- A parameter cannot be set in both the test template and the .json config. Doing so will cause a validation error.
- Pins can also not be set in both the test template and the .json config. Pick one.

---

## TPL Example
```tcl
Test SortSiup SIUP_X_ICC_K_START_TAP_X_X_X_NOM_PH5
{
	LevelsTc = "BASE::lvl_base_basic_allps";
	Patlist = "tpi_siup_ph5_sort_list";
	TimingsTc = "BASE::tim_d11r11_1x_t100_s400";
	TriggerMapName = "VBUMPIMEAS_SIUP_NOMV_TriggerMap";
	ConfigFile = "./Modules/TPI_SIU_PH5/InputFiles/siup_nomv_ph5.json";
	AlarmPortRedirect = "ENABLED";
	AlarmLevelsTc = "BASE::dps_alarm_basic_allps";
	SharedStoragePrefix = "SIUP_START_NOMV_PH5";
	BypassPort = -1;
}
```

----

## JSON ConfigFile syntax
```json
{
  "ConfigSets": {
    "ABC": {
      "MaskPins": "",
      "Pins": {
        "Pin1": {
          "MeasurementType": "Current",
          "LowLimit": "0.5mA",
          "HighLimit": "1.5mA"
        },
        "Pin2": {
          "MeasurementType": "Current",
          "LowLimit": "1.5uA",
          "HighLimit": "2.5uA"
        },
        "Pin3": {
          "MeasurementType": "Current",
          "LowLimit": "2.5pA",
          "HighLimit": "3.5pA"
        },
        "Pin4": {
          "MeasurementType": "Current",
          "LowLimit": "3.5fA",
          "HighLimit": "4.5fA"
        }
      },
      "TriggerMapName": "",
      "SharedStoragePrefix": "",
      "DTSPins": [ "ZZ_SORT_CDIE_TAP_TDO" ],
      "DTSEnableDisable": "Disabled",
      "LevelsToModify": {
        "bf_lvl_nom_lvl": {
          "VCCR_HC": {
            "IClampLo": "0.2A",
            "IClampHi": "0.6A",
            "IRange": "IR1_2A"
          }
        }
      }
    },
    "FORCONFLICT": {
      "MaskPins": "",
      "Pins": {
        "Pin1": {
          "MeasurementType": "Current",
          "LowLimit": "0.5mA",
          "HighLimit": "1.5mA"
        },
        "Pin2": {
          "MeasurementType": "Current",
          "LowLimit": "1.5uA",
          "HighLimit": "2.5uA"
        },
        "Pin3": {
          "MeasurementType": "Current",
          "LowLimit": "2.5pA",
          "HighLimit": "3.5pA"
        },
        "Pin4": {
          "MeasurementType": "Current",
          "LowLimit": "3.5fA",
          "HighLimit": "4.5fA"
        }
      },
      "TriggerMapName": "",
      "SharedStoragePrefix": "SweetPotato",
      "DTSPins": [ "ZZ_SORT_CDIE_TAP_TDO" ],
      "DTSEnableDisable": "Disabled",
      "LevelsToModify": {
        "bf_lvl_nom_lvl": {
          "VCCR_HC": {
            "IClampLo": "0.2A",
            "IClampHi": "0.6A",
            "IRange": "IR1_2A"
          }
        }
      }
    },
    "ANOTHER": {
      "MaskPins": "",
      "Pins": {
        "Pin1": {
          "MeasurementType": "Current",
          "LowLimit": "0.7mA",
          "HighLimit": "1.5mA"
        },
        "Pin2": {
          "MeasurementType": "Current",
          "LowLimit": "1.5uA",
          "HighLimit": "2.5uA"
        },
        "Pin3": {
          "MeasurementType": "Current",
          "LowLimit": "2.5pA",
          "HighLimit": "3.5pA"
        },
        "Pin4": {
          "MeasurementType": "Current",
          "LowLimit": "3.5fA",
          "HighLimit": "4.5fA"
        }
      },
      "TriggerMapName": "",
      "SharedStoragePrefix": "",
      "DTSPins": [ "ZZ_SORT_CDIE_TAP_TDO" ],
      "DTSEnableDisable": "Disabled",
      "LevelsToModify": {
        "bf_lvl_nom_lvl": {
          "VCCR_HC": {
            "IClampLo": "0.2A",
            "IClampHi": "0.6A",
            "IRange": "IR1_2A"
          }
        },
        "bf_hvqk_lvl_nom_lvl": {
          "VCCR_HC": {
            "VClampLo": "-1.5V",
            "VClampHi": "2V",
            "VRange": "VR6V"
          }
        }
      }
    }
  }
}
```

----

## Console output 
```
=========================
Running Execute() for test instance=[TPI_SIU_STATIC::PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1]
=========================
[2024-Dec-03 11:05:34.259][DUT: 1]Setting pin VCCCORE5_HC attributes IRANGE=IR24A,ICLAMPHI=1.400A,ICLAMPLO=-5A.
[2024-Dec-03 11:05:34.259][DUT: 1]PowerUpTc name =[dps_on_0V_lvl_cat0].
[2024-Dec-03 11:05:34.260][DUT: 1]Applying test condition=[dps_on_0V_lvl_cat0] with SmartTC Category=[LEVELS_POWER_ON].
[2024-Dec-03 11:05:34.260][DUT: 1]Test Condition Applied was skipped by SmartTc
[2024-Dec-03 11:05:34.260][DUT: 1]Applying test condition=[siup_lvl_siup_500mv] with SmartTC, Category=[LEVELS_SETUP].
[2024-Dec-03 11:05:34.282][DUT: 1]Applying test condition=[cpu_ssn_timing_tclk100_sclk100_cclk400] with SmartTC, Category=[TIMING].
[2024-Dec-03 11:05:34.283][DUT: 1]Functional Test settings:
- Plist name=[siup_infra_reset].
- Levels test condition=[siup_lvl_siup_500mv].
- Timings test condition=[cpu_ssn_timing_tclk100_sclk100_cclk400].
- No pin mask set.
- No edge counter pins set.
- No software trigger set.
- Trigger map=[TPI_SIU_STATIC_ALL_TriggerMap].
- Failure settings:
  --Total failure capture=[1].
  --Per pattern failure capture=[0].
[2024-Dec-03 11:05:34.292][DUT: 1]Plist=[siup_infra_reset], has finished all burst with result=[PASS].
...
[2024-Dec-03 12:04:43.755][DUT: 1]PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCCORE3_HC added to shared storage with value 0.10086441.
[2024-Dec-03 12:04:43.755][DUT: 1]PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCCORE4_HC added to shared storage with value 0.120887756.
[2024-Dec-03 12:04:43.755][DUT: 1]PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCCORE5_HC added to shared storage with value 0.089958191.
[2024-Dec-03 12:04:43.755][DUT: 1]PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCATOM0_HV added to shared storage with value 0.046594841.
[2024-Dec-03 12:04:43.755][DUT: 1]PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCATOM1_HV added to shared storage with value 0.03950359.
...
[2024-Dec-03 11:05:34.296][DUT: 1]Printed to Ituff:
... (See ituff section below)
[2024-Dec-03 11:05:34.296][DUT: 1]Restoring pin VCCCORE5_HC attributes IRANGE=IR24A,ICLAMPHI=24,ICLAMPLO=-5 from the stack.
[2024-Dec-03 11:05:34.297][DUT: 1]1 failing rails.
[2024-Dec-03 11:05:34.297][DUT: 1]VCCCORE5_HC failed
```

----

## Datalog output
The test method will print 2 lines for each pin under test.
The first line prints all the unique measurements triggered by the test. The second line is the average of all the measurements.
```
...
2_tname_TPI_SIU_STATIC::PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCCORE3_HC_all
2_strgval_0.087036|0.087418|0.093044|0.097908|0.102772|0.091232|0.091232|0.090469|0.090851|0.094189
2_tname_TPI_SIU_STATIC::PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCCORE3_HC
2_mrslt_0.092615128
2_tname_TPI_SIU_STATIC::PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCCORE4_HC_all
2_strgval_0.119247|0.118484|0.112000|0.106754|0.104084|0.122204|0.126400|0.113525|0.106373|0.114288
2_tname_TPI_SIU_STATIC::PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCCORE4_HC
2_mrslt_0.114336014
2_tname_TPI_SIU_STATIC::PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCCORE5_HC_all
2_strgval_0.088242|0.095108|0.102737|0.080612|0.083664|0.082901|0.077560|0.079849|0.090149|0.092438
2_tname_TPI_SIU_STATIC::PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCCORE5_HC
2_mrslt_0.087326050
2_tname_TPI_SIU_STATIC::PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCATOM0_HV_all
2_strgval_0.043668|0.044144|0.043782|0.041635|0.042474|0.038539|0.041521|0.041044|0.040091|0.041997
2_tname_TPI_SIU_STATIC::PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCATOM0_HV
2_mrslt_0.041889516
2_tname_TPI_SIU_STATIC::PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCATOM1_HV_all
2_strgval_0.045823|0.045823|0.046304|0.045343|0.047265|0.044862|0.045946|0.042697|0.042939|0.041497
2_tname_TPI_SIU_STATIC::PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_VCCATOM1_HV
2_mrslt_0.044850116
...
```

After all the pins have printed, if there was a failure, an additional line is also printed.
The line format is number of unique pin failures, a list of the failed pins, a list of all the unique measurements that failed the limit.
```
2_tname_TPI_SIU_STATIC::PGT_X_DC_K_START_X_X_X_X_500MV_INFRA_F1_FAILED0
2_strgval_PinsFailed_1,Pins_VCCCORE5_HC,FailedMeas_VCCCORE5_HC_0|0.0882415771484375|0.01,VCCCORE5_HC_1|0.0951080322265625|0.01,VCCCORE5_HC_7|0.0798492431640625|0.01,VCCCORE5_HC_8|0.09014892578125|0.01
```

----

## Exit Ports

Exit Port|Condition|Description
---|---|---
-2|Alarm|Port fail for HW alarm condition
-1|Error|Port fail for SW error condition
0|Fail|Limit failure port
1|Pass|Pass port
2|Fail|Pattern failure port
3|Fail|Alarm port (when redirected)
4-99|Fail|Per pin failure ports
