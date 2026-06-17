<h1>Prime Test-Method Specification REP</h1>

August 2025

[[_TOC_]]

This page contain information that shared across FuseRead TestMethod.

# FAQ
Search [Intel Stackoverflow](https://stackoverflow.intel.com/) with Tags [Prime](https://stackoverflow.intel.com/posts/tagged/7832) for answered question or Post new question.

# Download Sample File
<font color="green">**Download** </font>[General FuseRead Configuration](./.attachments/fuseReadConfigurationFile.json) <br>
<font color="green">**Download** </font>[FuseRead MaskUltDecode Configuration](./.attachments/fuseReadMaskUltDecodeConfigurationFile.json) <br>
<font color="green">**Download** </font>[FuseRead Voltage File](./.attachments/fuseReadVoltageFile.json) <br>

# [Explain] FuseRead Configuration JSON File.
**<font color="blue">Blue Text means is a container for attribute.</font>

## Configurations Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
|<font color="blue">Configurations</font>|Required |User can set Multiple Configurations.|

## Attribute of Configurations 
|Attribute|Required?|Description|
|---------|---------|-----------|
|Name     |Required |Name of the configuration.<br>Choose this configuration setup when match test instance parameter "ConfigName".|
|<font color="blue">Registers</font>|Required |User can set Multiple Register setting per Configuration.|

## Attribute of Registers 
|Attribute|Required?|Description|
|---------|---------|-----------|
|Name                                 |Required |Name of register.<br>Choose this when match test instance parameter "RegisterName"|
|SimulationString                     |Optional |Set this as fuse string when test instance parameter "SimulationMode" is true.<br>User is expected to define it with MSB->LSB (LHS->RHS) arrangement.<br>'Reverse' switch do not apply to SimulationString.|
|Reverse                              |Optional |Default is set to false. Set it to true to reverse the fuse string composite that will be compare with mask value.|
|DatalogFormat                        |Optional |Default is set to BINARY. This is to set the datalog format. Supported format: RLE, DEFLATE, BINARY, HEX.|
|FuseGroupsToHide                     |Optional |Provide fuse group name to mask off the fuse string bit to 0.|
|<font color="blue">Masks</font>      |Required for Mask mode |User can set multiple Mask setting per Register.|
|<font color="blue">SSPEC</font>      |Required for SSPEC mode |User can set multiple SSPEC setting per Register.|
|<font color="blue">MarginSweep</font>|Required for MarginSweep mode |User can set one MarginSweep setting only per Register.|
|<font color="blue">UltDecode</font>  |Required for MaskUltDecode mode |User can set one UltDecode setting only per Register.|

## Attribute of Masks 
|Attribute|Required?|Description|
|---------|---------|-----------|
|Name                                  |Required |Name of mask.<br>Choose this mask to execute when match test instance parameter "MaskName".|
|FuseGroup                             |Required |Name of fuse group.Use asterisk (*) for full fusestring.<br>Apply executing mask to this fuse group name.|
|Value                                 |Required |Mask value to execute.<br>Can be static value (binary) or dynamic value (m) that get from storage.|
|<font color="blue">Dynamic</font>     |Required when Mask Value contain 'm' |User can set multiple Dynamic setting per Dynamic Mask.|
|<font color="blue">MismatchBits</font>|Optional |User can set multiple MismatchBits setting to store the mismatch information.|
|FailPort                              |Optional and use by MaskUltDecode mode only.|

## Attribute of SSPEC 
|Attribute|Required?|Description|
|---------|---------|-----------|
|QDF                              |Required |Name of QDF.|
|FuseGroup                        |Required |Name of fuse group. Use asterisk (*) for full fusestring.<br>Apply executing QDF to this fuse group name.|
|Value                            |Required |Mask value to execute.<br>Can be static value (binary) or dynamic value (m) that get from storage.|
|<font color="blue">Dynamic</font>|Required when SSPEC Value contain 'm' |User can set multiple Dynamic setting per Dynamic Mask.|

## Attribute of MarginSweep 
|Attribute|Required?|Description|
|---------|---------|-----------|
|EnableCompare                                |Required |True will compare aggregated result of each margin result, false as readonly with no comparison. |
|ItuffPrintMode                               |Required |COMPOSITE or PER_READOUT mode.|
|EnableRestore                                |Required |Restore original pattern data when enabled, otherwise retain last modified pattern data.|
|MarginDefaultValue                           |Required |Pattern modification data to be restored. Value must be defined when EnableRestore set to TRUE. Otherwise it is empty.|
|<font color="blue">MarginSettings</font>     |Required |User can set multiple MarginSettings.|
|<font color="blue">PatternModifyConfig</font>|Required |Specify configuration name for pattern modify configuration.|

**When EnableRestore parameter is set to true, it is mandatory for user to define MarginDefaultValue, as test method will perform pattern modification again based on default value to restore the data.**

**Enabling restore also lead to test time increase.**

## Attribute of MarginSettings 
|Attribute|Required?|Description|
|---------|---------|-----------|
|Name  |Required |Margin name for pattern modification.|
|Value |Required |Margin pin data to apply.|

## Attribute of PatternModifyConfig 
|Attribute|Required?|Description|
|---------|---------|-----------|
|Name  |Required |Pattern modify configuration name.|
|Ratio |Optional |Pattern modify ratio.|
|PatternsRegEx|Required|Pattern regular expression.|

## Attribute of Dynamic 
|Attribute|Required?|Description|
|---------|---------|-----------|
|FuseGroup                               |Required |Name of Fuse Group for the dynamic value to apply.|
|<font color="blue">uservar</font>       |Optional |Choose only one storage for dynamic setting. Get dynamic value from uservar.|
|<font color="blue">sharedstorage</font> |Optional |Choose only one storage for dynamic setting. Get dynamic value from sharedstorage.|
|<font color="blue">dff</font>           |Optional |Choose only one storage for dynamic setting. Get dynamic value from dff.|

## Attribute of uservar (It is expected the intented storage is properly setup.)
|Attribute|Required?|Description|
|---------|---------|-----------|
|Collection |Required |Collection name of the uservar.|
|Name       |Required |Full variable name, expected Collection name and Variable name. example: FuseRead.ABC|

## Attribute of sharedstorage (It is expected the intented storage is properly setup.)
|Attribute|Required?|Description|
|---------|---------|-----------|
|Name  |Required |Variable name that exist in sharedstorage.|
|Scope |Required |One of DUT, LOT and IP only.|

## Attribute of dff (It is expected the intented storage is properly setup.)
|Attribute|Required?|Description|
|---------|---------|-----------|
|Name   |Required |Variable name exist in dff.|
|Optype |Required |Operation type.|
|DieID  |Required |Die Id.|

## Attribute of UltDecode 
|Attribute|Required?|Description|
|---------|---------|-----------|
|FuseGroup |Required |Fuse Group to apply.|
|FailPort  |Required |Intented Fail Port number.|

# [Explain] Voltage JSON File to Enable VBump Software Trigger.
To enable functional test software trigger for vbump, user is required to provide json file with the following configuration.
**<font color="blue">Blue Text means is a container for attribute.</font>

## Voltages Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
|<font color="blue">Voltages</font> |Required |A container to hold Voltage attribute information.|

## Attribute of Voltages 
|Attribute|Required?|Description|
|---------|---------|-----------|
|<font color="blue">Voltage</font>  |Required |A container to hold operand information.|

## Attribute of Voltage 
|Attribute|Required?|Description|
|---------|---------|-----------|
|Name     |Required |Name of executing Operand.|
|Operand  |Required |Interger value and must not duplicate within a Voltage attribute.|
|PowerPin |Required |Can define multiple pin name and voltage value. <br> At verify, <br> pin name will be check for pin existance and no duplicate pin name should exist within a operand. <br> voltage value can be either static or dynamic, cannot define both for a pin. For dynamic voltage value, only sharedstorage DUT and LOT is supported.<br>Verify will check the existance of the voltage value sharedstorage configuration. |

# [Explain] PrimeFuseRead*TestMethod test instance parameter "FuseGroupToDatalog"
This document describes how fuse string of fuse group are handled and stored when using various `PrimeFuseRead` test methods.

## 🔧 Test Instance Parameter: `FuseGroupToDatalog`

When the test instance parameter `FuseGroupToDatalog` is defined, the fuse string value for each specified fuse group will:

- Be **printed to Ituff**.
- Be **stored in DUT context shared storage** (string type) using **<font color="red">one</font>** of the following key formats:
  - `fuseread_<FuseGroupName>`
  - `fuseread_<DieIDName>_<FuseGroupName>`

## 🧪 Key Format Usage by Test Method

| Test Method                            | Key Format Used                                                      | Additional Conditions |
|----------------------------------------|----------------------------------------------------------------------|-----------------------|
| `PrimeFuseReadMaskTestMethod`          | `fuseread_<FuseGroupName>`                                           | —                     |
| `PrimeFuseReadMaskUltDecodeTestMethod` | `fuseread_<FuseGroupName>` or `fuseread_<DieIDName>_<FuseGroupName>` | Depends on whether `DieIdNames` is defined.<br>- If `DieIdNames` is **empty**, it uses `fuseread_<FuseGroupName>`.<br>- If `DieIdNames` is **defined**, it uses `fuseread_<DieIDName>_<FuseGroupName>`.     |
| `PrimeFuseReadSspecTestMethod`         | `fuseread_<FuseGroupName>`                                           | —                     |
| `PrimeFuseReadMarginSweepTestMethod`   | `fuseread_<FuseGroupName>`                                           | —                     |

## ⚠️ Per Register Per DieIdNames handling for `FuseGroupToDatalog` (`PrimeFuseReadMaskUltDecodeTestMethod` only)
RegisterName: CPU0,PCH0,DRNG0
FuseGroupToDatalog: ULT
DieIdNames: U2.U1,U4,NA

CPU0 will store fuse group 'ULT' fuse string to shared storage key `fuseread_U2.U1_ULT`
PCH0 will store fuse group 'ULT' fuse string to shared storage key `fuseread_U4_ULT`
DRNG0 will store fuse group 'ULT' fuse string to shared storage key `fuseread_ULT`

## ⚠️ Backslash Handling in Fuse Group Names
If a `FuseGroupName` contains a backslash (`\`), it must be escaped using **double backslashes (`\\`)** in both the test instance parameter and the fuse definition file.

### ✅ Example
FuseGroupName is `AAA\BBB_C` and `XXX\YYY_Z`.
- **Test Instance Parameter**:
FuseGroupToDatalog = `AAA\\\BBB_C,XXX\\\YYY_Z`;
- **FuseDef File**:
"Name": `AAA\\\BBB_C`;

Despite the double backslash in configuration, the Ituff output will display the fuse group name with a single backslash:
```
2_tname_<instance_name>_<registerName>_AAA\BBB_C
2_tname_<instance_name>_<registerName>_XXX\YYY_Z
```

# Example File
## General FuseRead Configuration
```json
{
   "Configurations": [
       {
         "Name": "Config999", // choose this configuration when match test instance parameter "ConfigName"
         "Registers": [
            {
               "Name": "CPU0", // choose this when match test instance parameter "RegisterName"
               "SimulationString": "1011110000001111111", // set this as fuse string when test instance parameter "SimulationMode" is true
               "FuseGroupsToHide": "GROUP_A", // mask off the fuse group based on fuse address
               "Reverse": true, // reverse the fuse string to compare and datalog
               "DatalogFormat": "RLE", // datalog fuse string in RLE format
               "Masks": [
                  {
                     "Name": "Mask1", // choose this when match test instance parameter "MaskName"
                     "FuseGroup": "*",
                     "Value": "mmmmmmmmmmmmmmmmmmm", // expecting dynamic value
                     "Dynamic": [
                        {
                           "FuseGroup": "*",
                           "uservar":
                           {
                              "Name": "FuseVars.CUSTOM_STRING"
                           }
                        },
                        {
                           "FuseGroup": "GROUP_A",
                           "sharedstorage":
                           {
                              "Name": "VAR1",
                              "Scope": "DUT"
                           }
                        },
                        {
                           "FuseGroup": "GROUP_B",
                           "DFF": {
                              "Name": "VARGBM1",
                              "Optype": "PBIC_S2",
                              "DieID": "GBM1"
                           }
                        }
                     ]
                  },
                  {
                     "Name": "Mask2",
                     "FuseGroup": "GROUP_A",
                     "Value": "111100"
                  },
                  {
                     "Name": "Mask3",
                     "FuseGroup": "GROUP_B",
                     "Value": "10000", // static mask
                     "MismatchBits": [
                        {
                           "sharedstorage":
                           {
                              "Name": "FailingBitsForMask1",
                              "Scope": "DUT"
                           }
                        }
                     ]
                  }
               ]
            },
            {
               "Name": "CPU1",
               "SimulationString": "1011110000001111111",
               "FuseGroupsToHide": "GROUP_A",
               "Masks": [
                  {
                     "Name": "Mask1",
                     "FuseGroup": "*",
                     "Value": "0011110000001000000",
                     "MismatchBits": [
                        {
                           "sharedstorage":
                           {
                              "Name": "FailingBitsForMask1",
                              "Scope": "DUT"
                           }
                        }
                     ]
                  }
               ]
            }
         ]
       }
   ]
}
```
## FuseRead Mask UltDecode File
```json
{
   "Configurations": [
      {
         "Name": "Config999", // choose this when match test instance parameter "ConfigName"
         "Registers": [
            {
               "Name": "CPU0", // choose this when match test instance parameter "RegisterName"
               "SimulationString": "1011110000001111111", // set this as fuse string when test instance parameter "SimulationMode" is true
               "FuseGroupsToHide": "GROUP_A", // mask off the fuse group based on fuse address
               "Reverse": true, // reverse the fuse string to compare and datalog
               "DatalogFormat": "RLE", // datalog fuse string in RLE format
               "Masks": [
                  {
                     "Name": "Mask1", // choose this when match test instance parameter "MaskName"
                     "FuseGroup": "*",
                     "Value": "mmmmmmmmmmmmmmmmmmm", // expecting dynamic value
                     "Dynamic": [
                        {
                           "FuseGroup": "*",
                           "uservar":
                           {
                              "Name": "FuseVars.CUSTOM_STRING"
                           }
                        },
                        {
                           "FuseGroup": "GROUP_A",
                           "sharedstorage":
                           {
                              "Name": "VAR1",
                              "Scope": "DUT"
                           }
                        },
                        {
                           "FuseGroup": "GROUP_B",
                           "DFF": {
                              "Name": "VARGBM1",
                              "Optype": "PBIC_S2",
                              "DieID": "GBM1"
                           }
                        }
                     ]
                  },
                  {
                     "Name": "Mask2",
                     "FuseGroup": "GROUP_A",
                     "Value": "111100",
                     "FailPort": 2
                  },
                  {
                     "Name": "Mask3",
                     "FuseGroup": "GROUP_B",
                     "Value": "10000", // static mask
                     "MismatchBits": [
                        {
                           "sharedstorage":
                           {
                              "Name": "FailingBitsForMask1",
                              "Scope": "DUT"
                           }
                        }
                     ]
                  }
               ],
               "UltDecode": {
                  "FuseGroup": "*",
                  "FailPort": 3
               }
            },
            {
               "Name": "CPU1",
               "SimulationString": "1011110000001111111",
               "FuseGroupsToHide": "GROUP_A",
               "Masks": [
                  {
                     "Name": "Mask1",
                     "FuseGroup": "*",
                     "Value": "0011110000001000000",
                     "MismatchBits": [
                        {
                           "sharedstorage":
                           {
                              "Name": "FailingBitsForMask1",
                              "Scope": "DUT"
                           }
                        }
                     ]
                  }
               ],
               "UltDecode": {
                  "FuseGroup": "ULT",
                  "FailPort": 4
               }
            }
         ]
      }
   ]
}
```
## FuseRead SSPEC File
```json
{
   "Configurations": [
      {
         "Name": "Config22",
         "Registers": [
         {
            "Name": "CPU0",
            "SimulationString": "1011110000001111111",
            "SSPEC": [
               {
                  "QDF": "DB59",
                  "FuseGroup": "*",
                  "Value": "mmmmmmmmmmmmmmmmmmm",
                  "Dynamic": [
                  {
                     "FuseGroup": "*",
                     "uservar":
                     {
                        "Name": "FuseVars.CUSTOM_STRING"
                     }
                  }
                  ]
               }
            ]
         },
         {
            "Name":"PCH0",
            "SimulationString":"1011110000001111111",
            "SSPEC":[
               {
                  "QDF":"DB59",
                  "FuseGroup": "PCH_FUSE1",
                  "Value": "mm",
                  "Dynamic": [
                  {
                     "FuseGroup": "PCH_FUSE1",
                     "DFF": {
                        "Name": "BINCHECK.ALLOWSINGLE",
                        "Optype": "PBIC_DAB",
                        "DieID": "U2.U1"
                   }
                  }
                  ]
               }
            ]
         }
         ]
      }
   ]
}
```
## FuseRead Voltage File
```json
{
  "Voltages": {
    "Voltage": [
      {
        "Name": "SNDC_VCCST_PreProgVoltage",
        "Operand": 1,
        "PowerPin": [
          {
            "Name": "VCCFUSE0_GPU0_LC",
            "Value": 0.5
          },
          {
            "Name": "VCCFUSE0_GPU1_LC",
            "sharedstorage": {
              "Name": "Fuse_Voltage_VCCST_Prog_BASE01",
              "Scope": "DUT"
            }
          }
        ]
      },
      {
        "Name": "SNDC_VCCFPGMx_PreProg",
        "Operand": 2,
        "PowerPin": [
          {
            "Name": "VCCFUSE0_GPU0_LC",
            "sharedstorage": {
              "Name": "Fuse_Voltage_VCCFPRGx_PreProg_BASE00",
              "Scope": "DUT"
            }
          },
          {
            "Name": "VCCFUSE0_GPU1_LC",
            "sharedstorage": {
              "Name": "Fuse_Voltage_VCCFPRGx_PreProg_BASE01",
              "Scope": "DUT"
            }
          }
        ]
      }
    ]
  }
}
```