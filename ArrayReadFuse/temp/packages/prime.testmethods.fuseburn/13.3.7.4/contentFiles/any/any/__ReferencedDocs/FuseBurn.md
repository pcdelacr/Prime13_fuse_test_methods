This page contain information that shared across fuseburn testmethod.

[[_TOC_]]

# Configurations
## **BurnPatternConfig for GetFinalFuseStringToBurn()**
What is `FinalFuseString`?<br>
`FinalFuseString` also known as FinalFuseStringToBurn. It is a fusestring that applied with `BurnPatternConfig`, then set to PatConfig for PatternModifiy.

### How to configure?
<details><summary><font color="blue">Expand for detail</font></summary>
**Note that the example show here is only for related feature.

```json
{
  "Configurations": [
    {
      "Name": "ExampleConfiguration",
      "Registers": [
        {
          "Name": "CPU0",
          "DataLabel": "FUSE_BURN_DATA_LABEL",
          "DataLabelOffset": 0,
          "Masks": [
            {
              "Name": "burn_fuseGroup",
              "FuseGroup": "GROUP_A",
              "Value": "101001"
            }
          ]
        }
      ],
      "BurnPatternConfig": {
        "ReverseString": "FALSE",
        "Ratio": 2,
        "DataBlockSize": 3,
        "GapSize": 2,
        "PatternsRegex": "ctvs"
      }
    }
  ]
}
```


</details>

### Applying BurnPatternConfig
<details><summary><font color="blue">Expand for detail</font></summary>

Assuming the register CPU0 fusestring from SharedStorage stored by FuseRead is 1011110000001111111 (MSB->LSB).<br>
We are executing Mask=[burn_fuseGroup] with FuseGroup=[GROUP_A] (StartAddress=[0] and EndAddress=[5], refer to FuseDef for FuseGroup address.)<br>
![image.png](./.attachments/FuseBurn/BurnPatternConfigExplain.png)

```console
At PatternEditor GUI, you can observe the "Vector#" (or address) started from 0 with top-down arrangement. 
Example,
Vector# PinA
0       1
1       1
2       1
3       0
4       0
5       0

               S(LSB)    E(MSB)
PatternIndex = 0 1 2 3 4 5
PatternData  = 1 1 1 0 0 0

Assume fusestring (MSB->LSB) with mask value 000111 and no "BurnPatternConfig".
            E(MSB)    S(LSB)
MaskIndex = 5 4 3 2 1 0
MaskValue = 0 0 0 1 1 1

Default reverse by testmethod is needed to match the PatternModification format.
```

|Context |Value |
|------- |----- |
|FuseString that set to PatConfig for PatternModification |11__000__011__001__1__________________________________________ |
|For explain purpose |11ss000ss011ss001ss1ssssssssssssssssssssssssssssssssssssssssss |
|Configuration DataLabel |FUSE_BURN_DATA_LABEL (at adress 156) |
|Configuration DataLabelOffset |0 |

Simplify table showing process of PatternModification
|Vector# (Address) |Value |
|----------------- |----- |
|156 - 157 |11 |
|158 - 159 |skip |
|160 - 162 |000 |
|163 - 164 |skip |
|165 - 167 |011 |
|168 - 169 |skip |
|170 - 172 |001 |
|173 - 174 |skip |
|175 |1 |
</details>

### Console
<details><summary><font color="blue">Expand for detail</font></summary>

```console
BurnPatternConfig ReverseString=[False].
Fuse Data now reformatted with ratio=[2].
New Fuse Data=[ssssssssssssssssssssssssss110011000011].
Fuse Data now reformatted by BlockSize=[3] and GapSize=[2].
New Fuse Data=[ssssssssssssssssssssssssssssssssssssssssss1ss100ss110ss000ss11].
IMPORTANT: Fuse burn data will be written in reverse order; MSB of burn data is written at (Label+Offset).
Storing FinalFuseStringToBurn=[11__000__011__001__1__________________________________________]
FinalFuseStringToBurn stored to SharedStorage Key=[FinalFuseStringBurnMask_PCH0] Context=[DUT]
-- On The Fly --
    Ratio            : '1'
    Data Size (exp)  : '62'
    Configurations :
        Data Size (exp)  : '62'
        Start Address    : 'FUSE_BURN_DATA_LABEL'
        Start Offset     : '0'
        Pins             : 'xxHPCC_DPIN_Dig_slcA_A0'
    Patterns address :
        For Config 0    
        Pattern          : 'ctvs'
            Range            : 'Start=[156], End=[217], Size=[62]'
            Pin              : 'xxHPCC_DPIN_Dig_slcA_A0'
            Domains          : 'DomainA_All_DPIN_Dig'
            Reverse          : 'False'
On the Fly handle: 
Start Address = [156], End Address = [157]. Pattern name = [ctvs.
Raw Data = [11]. Target Pin = [xxHPCC_DPIN_Dig_slcA_A0], Target Domain = [DomainA_All_DPIN_Dig].
On the Fly handle: 
Start Address = [160], End Address = [162]. Pattern name = [ctvs.
Raw Data = [000]. Target Pin = [xxHPCC_DPIN_Dig_slcA_A0], Target Domain = [DomainA_All_DPIN_Dig].
On the Fly handle: 
Start Address = [165], End Address = [167]. Pattern name = [ctvs.
Raw Data = [011]. Target Pin = [xxHPCC_DPIN_Dig_slcA_A0], Target Domain = [DomainA_All_DPIN_Dig].
On the Fly handle: 
Start Address = [170], End Address = [172]. Pattern name = [ctvs.
Raw Data = [001]. Target Pin = [xxHPCC_DPIN_Dig_slcA_A0], Target Domain = [DomainA_All_DPIN_Dig].
On the Fly handle: 
Start Address = [175], End Address = [175]. Pattern name = [ctvs.
Raw Data = [1]. Target Pin = [xxHPCC_DPIN_Dig_slcA_A0], Target Domain = [DomainA_All_DPIN_Dig].
-- On The Fly --
    Ratio            : '1'
    Data Size (exp)  : '62'
    Configurations :
        Data Size (exp)  : '62'
        Start Address    : 'FUSE_BURN_DATA_LABEL'
        Start Offset     : '0'
        Pins             : 'xxHPCC_DPIN_Dig_slcA_A0'
    Patterns address :
        For Config 0    
        Pattern          : 'ctvs'
            Range            : 'Start=[156], End=[217], Size=[62]'
            Pin              : 'xxHPCC_DPIN_Dig_slcA_A0'
            Domains          : 'DomainA_All_DPIN_Dig'
            Reverse          : 'False'
```
</details>

### FAQ
<details><summary><font color="blue">Expand for detail</font></summary>
<font color="brown">Why converting skip character 's' to underscore '_'?</font><br>
It is required by PatConfig.<br><br>

<font color="brown">Why have a Default Reverse by TestMethod?</font><br>
FuseBurnTestMethod is expecting fusestring and mask value is arranged from MSB->LSB and Engineer also interpret string arrangement from Left to Right as MSB->LSB.<br>
The default reverse of string is to match the PatternModification format.
</details>

# Features
## **[TestMethod] StoreFinalFuseString to SharedStorage**
This feature allow user to store [FinalFuseString](#**burnpatternconfig-for-getfinalfusestringtoburn()**) to multiple SharedStorage. The process of storing only happen after `ExecuteMask` with returned Port 1.<br>
The setup is through `StoreFinalFuseString` attribute and it is per register setup.<br>

### How to configure?
<details><summary><font color="blue">Expand for detail</font></summary>
**Note that the example show here is only for related feature.

```json
{
  "Configurations": [
    {
      "Name": "ExampleConfiguration",
      "Registers": [
        {
          "Name": "CPU0",
          "StoreFinalFuseString": [
            {
              "sharedstorage": {
                "Name": "FinalFuseStringBurnMask_CPU0",
                "Scope": "DUT"
              }
            },
            {
              "sharedstorage": {
                "Name": "FinalFuseStringBurnMask_CPU0_2",
                "Scope": "DUT"
              }
            }
          ]
        },
        {
          "Name": "PCH0",
          "StoreFinalFuseString": [
            {
              "sharedstorage": {
                "Name": "FinalFuseStringBurnMask_PCH0",
                "Scope": "DUT"
              }
            }
          ]
        }
      ]
    }
  ]
}
```
When FuseBurnTestMethod execute the ExampleConfiguration for multiple register mode,
|Register |StoreFinalFuseString |
|-------- |-------------------- |
|CPU0 |Store to SharedStorage "Key=[FinalFuseStringBurnMask_CPU0] Context=[DUT]" and "Key=[FinalFuseStringBurnMask_CPU0_2] Context=[DUT]" |
|PCH0 |Store to SharedStorage "Key=[FinalFuseStringBurnMask_PCH0] Context=[DUT]" |
</details>

### Console
<details><summary><font color="blue">Expand for detail</font></summary>

```console
... when Executing Register CPU0 ...
Storing FinalFuseStringToBurn=[1010011110111101010]
FinalFuseStringToBurn stored to SharedStorage Key=[FinalFuseStringBurnMask_CPU0] Context=[DUT]
FinalFuseStringToBurn stored to SharedStorage Key=[FinalFuseStringBurnMask_CPU0_2] Context=[DUT]
...

... when Executing Register PCH0 ...
Storing FinalFuseStringToBurn=[1010011110111101010]
FinalFuseStringToBurn stored to SharedStorage Key=[FinalFuseStringBurnMask_PCH0] Context=[DUT]
...
```
</details>

## **[MiniFlow] ULTEncode**
**<font color="red">This API is exclusively for MiniFlow and has been requested and designed specifically for Sort Only and Single Register Only.</font>**<br>
**<font color="red">When using FuseBurn Miniflow ULTEncode, Both BurnMaskTM and BurnSSPECTM must have at least 1 MaskBurn/SspecBurn. </font>**<br>
When MiniFlow execute ULTEncode.
1. It will get the string to be encode from provided uservar name. User is required to provide the uservar for `Lot`, `Wafer`, `XLocation`, `YLocation` in the configuration file.
2. The retrieved uservar string will be encoded to format based on `CtvsOrder` and `BitCount`.
3. Upon successful encoding, the encoded string will be saved to the uservar name specified in `StoreEncode`.

### How to configure?
<details><summary><font color="blue">Expand for detail for BurnMask</font></summary>
**Note that the example show here is only for related feature.

```json
{
  "Configurations": [
    {
      "Name": "ExampleConfiguration",
      "Registers": [
        {
          "Name": "CPU0",
          "DataLabel": "FUSE_BURN_DATA_LABEL",
          "DataLabelOffset": 0,
          "Masks": [
            {
              "Name": "burn_0_full_string",
              "FuseGroup": "*",
              "Value": "0000000000000000000"
            }
          ],
          "ULTEncode": {
            "Lot": {
              "uservar": {
                "Name": "_UserVars.UservarKeyForLot"
              }
            },
            "Wafer": {
              "uservar": {
                "Name": "_UserVars.UservarKeyForWafer"
              }
            },
            "XLocation": {
              "uservar": {
                "Name": "_UserVars.UservarKeyForXLocation"
              }
            },
            "YLocation": {
              "uservar": {
                "Name": "_UserVars.UservarKeyForYLocation"
              }
            },
            "StoreEncode": [
              {
                "uservar": {
                  "Name": "_UserVars.MaskUservarKeyAToStoreEncodedString"
                }
              },
              {
                "uservar": {
                  "Name": "_UserVars.MaskUservarKeyBToStoreEncodedString"
                }
              }
            ],
            "CtvsOrder": "MSB",
            "BitCount": 64
          }
        }
      ],
      "BurnPatternConfig": 
      {
        "ReverseString": "FALSE",
        "Ratio": 1,
        "PatternsRegex": "ctvs"
      }
    }
  ]
}
```
</details>

<details><summary><font color="blue">Expand for detail for BurnSSPEC</font></summary>
**Note that the example show here is only for related feature.

```json
{
  "Configurations": [
    {
      "Name": "ExampleConfiguration",
      "Registers": [
        {
          "Name": "CPU0",
          "DataLabel": "FUSE_BURN_DATA_LABEL",
          "DataLabelOffset": 0,
          "SSPEC": [
            {
              "Enable": true
            }
          ],
          "ULTEncode": {
            "Lot": {
              "uservar": {
                "Name": "_UserVars.UservarKeyForLot"
              }
            },
            "Wafer": {
              "uservar": {
                "Name": "_UserVars.UservarKeyForWafer"
              }
            },
            "XLocation": {
              "uservar": {
                "Name": "_UserVars.UservarKeyForXLocation"
              }
            },
            "YLocation": {
              "uservar": {
                "Name": "_UserVars.UservarKeyForYLocation"
              }
            },
            "StoreEncode": [
              {
                "uservar": {
                  "Name": "_UserVars.SspecUservarKeyAToStoreEncodedString"
                }
              },
              {
                "uservar": {
                  "Name": "_UserVars.SspecUservarKeyBToStoreEncodedString"
                }
              }
            ],
            "CtvsOrder": "MSB",
            "BitCount": 64
          }
        }
      ],
      "BurnPatternConfig": {
        "ReverseString": "FALSE",
        "Ratio": 1,
        "PatternsRegex": "ctvs"
      }
    }
  ]
}
```
</details>

### Console
<details><summary><font color="blue">Expand for detail</font></summary>

```console
... when ULTEncode Pass ...
<Lot>_<Wafer>_<XLocation>_<YLocation> was encode to <EncodedString> and saved into _UserVars.MaskUservarKeyAToStoreEncodedString uservar.
<Lot>_<Wafer>_<XLocation>_<YLocation> was encode to <EncodedString> and saved into _UserVars.MaskUservarKeyBToStoreEncodedString uservar.
...continue miniflow...
...

... when ULTEncode Throw TestMethodException ...
ERROR:
|	Error in instance=[FuseBurn::MiniFlow_ULTEncode_NoSetup_F0]:
|	ERROR: FuseBurnMask test method fail at Execute. Exception: Register=[CPU0] attempt to execute ULTEncode with no configuration.
STACK TRACE:
at Execute C:\agent\_work\75\s\main\src\TestMethods\FuseBurn\Source\FuseBurnMask\PrimeFuseBurnMaskTestMethod.cs(342)
TestInstance=[FuseBurn::MiniFlow_ULTEncode_NoSetup_F0]. Execute result=[0]
[2024-12-30T06:05:58.729205-06:00][A][TAL][DUT: 1] StopTest MiniFlowExecuteULTEncode::FuseBurn::MiniFlow_ULTEncode_NoSetup_F0
...

... when MiniFlow only have ULTEncode, no MaskBurn or SspecBurn ...
ERROR:
|	Error in instance=[FuseBurn::MiniFlow_UserReferenceBurnSspecPlugin_P1]:
|	ERROR: FuseBurnSspec test method fail at Execute. Exception: Modify Data contains Only Skip Data for configuration at index [0], Please check your configuration.
STACK TRACE:
at Execute C:\0_ticket\53469_FBMiniFlowULTEncode\main\src\TestMethods\FuseBurn\Source\FuseBurnSspec\PrimeFuseBurnSspecTestMethod.cs(298)
TestInstance=[FuseBurn::MiniFlow_UserReferenceBurnSspecPlugin_P1]. Execute result=[0]
[2025-01-16T13:58:10.038409+08:00][A][TAL][DUT: 1] StopTest MiniFlowUserReferenceBurnSspec::FuseBurn::MiniFlow_UserReferenceBurnSspecPlugin_P1
```
</details>