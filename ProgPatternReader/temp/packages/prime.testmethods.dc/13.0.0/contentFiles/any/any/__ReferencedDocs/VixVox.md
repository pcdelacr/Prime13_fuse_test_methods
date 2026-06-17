<h1>Prime Test-Method Specification REP</h1>

November 2023

[[_TOC_]]

## Methodology
The VixVox test method provides capability to validate functionality of the I/O buffers. For VIX test, this test method will parse 

For VIX test (Voltage Input Low / Voltage Input High - VIL/VIH) :
This test method will parse the output of the Tap Data Output (TDO) pin and use a map to find which external pin names failed.  The failing pins will then be mapped to the appropriate tester channel found via the test program pin def.  The data log will then contain the same tester channel number (instead of TDO) as the Vix test.

For VOX test (Voltage Output Low / Voltage Output High - VOL/VOH) : 
No special mapping is needed as it is simply a function test execution.

## Known bugs ##
| Impacted Prime Version | Issue fixed in Prime version | Issue | Comments |
|------------------------|------------------------------|-------|---------|
|Prime V10 - Prime V11.0 | Prime V11.1 | Failpins carry forward to next VixVox test. | Problem due to modified pin attribute do not restore properly. https://dev.azure.com/mit-us/PRIME/_workitems/edit/30315/ |
|Prime V10 - Prime V11.0 | Prime V11.1 | High test time vs Evg iCVixVox | Problem due to logic of EOTPowerDown inverted. https://dev.azure.com/mit-us/PRIME/_workitems/edit/32732 |

### Pattern Requirements for VIX test
Both Vix patterns need to have labels indicating the Boundary Scan pin order.  This is done by encoding the Pin Name within the label ‘*Pin_<pin name>’ on the cycle where the TAP Data Output pin is strobed for each pin in the chain. If TAP to Bus Clock ratio is not 1:1, the pattern should still only contain ONE label (and preferably one strobe) per pin.
For IDUT setup, label can be decode with IDUT_<Idut domain>\_Pin\_<Pin name> syntax.

**Label Example 1**:
| Label | Decoded Label |
|-------|---------------|
| mypattern_**Pin_**xxbuspin1:                 | **xxbuspin1 |
| **Pin_**xxbuspin2:                           | **xxbuspin2 |
| mypattern_IDUT_Domain\_A\_**Pin_**xxbuspin3: | ```Domain_A::**xxbuspin3``` |
| IDUT_Domain\_A\_**Pin_**xxbuspin4:           | ```Domain_A::**xxbuspin4``` |
| xIDUT_Domain\_A\_**Pin_**xxbuspin5:           | **xxbuspin5 |
 
The special token “**Pin_**”, note the upper case “**P**”, is used to identify the start of a Pin Name in the label.

Some pattern automation, may append the domain of the label to the label.

**Label Example 2**:
| Label | Decoded Label |
|-------|---------------|
| mypattern_Pin_xxbuspin1_defdomain:                 | xxbuspin1 |
| Pin_xxbuspin2_defdomain:                           | xxbuspin2 |
 
The test template will attempt to remove all known domain names for the pattern from the end of labels to allow proper identification of the Pin Name.  In this case, the “_defdomain” would be removed prior to identifying the Pin Name of “xxbuspin1”.

The <pin name> must match _EXACTLY_ with the name found in the test program socket file; it is case sensitive.

Some Boundary Scan elements may not have an associated external pin name; in these cases no label is placed on the TAP clock cycle.  This adds an additional requirement that the patterns be “flat” during the SHIFT-DR state to prevent the unconnected elements fails from being mis-interrupted.

**Pattern Example**:
```
abcd:
           {  V { xxHPCC_DPIN_Dig_slcA_A0=0; xxHPCC_DPIN_Dig_slcA_AA0=L; } }
           {  V { xxHPCC_DPIN_Dig_slcA_A0=0; xxHPCC_DPIN_Dig_slcA_AA0=L; } }
sample1_Pin_xxHPCC_DPIN_Dig_slcA_AA0:
           {  V { xxHPCC_DPIN_Dig_slcA_A0=1; xxHPCC_DPIN_Dig_slcA_AA0=H; } }
sample1_Pin_xxHPCC_DPIN_Dig_slcA_AA1:
           {  V { xxHPCC_DPIN_Dig_slcA_A0=1; xxHPCC_DPIN_Dig_slcA_AA0=H; } }
sample1_Pin_xxHPCC_DPIN_Dig_slcA_AA2:
           {  V { xxHPCC_DPIN_Dig_slcA_A0=1; xxHPCC_DPIN_Dig_slcA_AA0=H; } }
sample1_Pin_xxHPCC_DPIN_Dig_slcA_AA3:
           {  V { xxHPCC_DPIN_Dig_slcA_A0=1; xxHPCC_DPIN_Dig_slcA_AA0=H; } }
sample1_Pin_xxHPCC_DPIN_Dig_slcA_AA4:
           {  V { xxHPCC_DPIN_Dig_slcA_A0=0; xxHPCC_DPIN_Dig_slcA_AA0=H; } }
           {  V { xxHPCC_DPIN_Dig_slcA_A0=1; xxHPCC_DPIN_Dig_slcA_AA0=H; } }
sample1_Pin_xxHPCC_DPIN_Dig_slcA_AA6:
           {  V { xxHPCC_DPIN_Dig_slcA_A0=1; xxHPCC_DPIN_Dig_slcA_AA0=H; } }
sample1_Pin_xxHPCC_DPIN_Dig_slcA_AA7:
           {  V { xxHPCC_DPIN_Dig_slcA_A0=1; xxHPCC_DPIN_Dig_slcA_AA0=H; } }
sample1_Pin_xxHPCC_DPIN_Dig_slcA_BB0:
           {  V { xxHPCC_DPIN_Dig_slcA_A0=1; xxHPCC_DPIN_Dig_slcA_AA0=H; } }
sample1_Pin_xxHPCC_DPIN_Dig_slcA_BB1:
           {  V { xxHPCC_DPIN_Dig_slcA_A0=1; xxHPCC_DPIN_Dig_slcA_AA0=H; } }
           {  V { xxHPCC_DPIN_Dig_slcA_A0=0; xxHPCC_DPIN_Dig_slcA_AA0=L; } }
           {  V { xxHPCC_DPIN_Dig_slcA_A0=0; xxHPCC_DPIN_Dig_slcA_AA0=L; } }
           {  V { xxHPCC_DPIN_Dig_slcA_A0=1; xxHPCC_DPIN_Dig_slcA_AA0=H; } }
           {  V { xxHPCC_DPIN_Dig_slcA_A0=1; xxHPCC_DPIN_Dig_slcA_AA0=H; } }
           {  V { xxHPCC_DPIN_Dig_slcA_A0=1; xxHPCC_DPIN_Dig_slcA_AA0=H; } }
           {  V { xxHPCC_DPIN_Dig_slcA_A0=0; xxHPCC_DPIN_Dig_slcA_AA0=L; } }
```
 
### Pattern List Requirements
Within the pattern list provide to the test instance, there needs to be defined a pattern list which matches “*vih_*” for VIH Search,  “*vil_*” for VIL Search, “*voh_*” for VOH Search, and “*vol_*” for VOL Search.  The must be ONE AND ONLY ONE pattern list defined for each type which matches the regular expression.

In general, there needs to be a pattern list for each of the four VixVox patterns to facilitate proper searches. 

**Pattern List Example**:
```
GlobalPList vixvox_fail_list
{
  GlobalPList vox_fail_list
  {
    GlobalPList bscan_fail_vox_list
    {
      GlobalPList vol_fail_list  [BurstOff] [Mask xxHPCC_DPIN_Dig_slcA_A0]
      {
        Pat tap_extest0_fail;
      }
      GlobalPList voh_fail_list  [BurstOff] [Mask xxHPCC_DPIN_Dig_slcA_A0]
      {
        Pat tap_extest1_fail;
      }
    }
  }

  GlobalPList vix_fail_list
  {
    GlobalPList vil_fail_list  [BurstOff] [Mask xxHPCC_DPIN_Dig_slcA_A0]
    {
      Pat tap_sample0_fail;
    }
    GlobalPList vih_fail_list  [BurstOff] [Mask xxHPCC_DPIN_Dig_slcA_A0]
    {
      Pat tap_sample1_fail;
    }
  }
}
```

Note only “Global Plist” are currently supported. Plist attributes are not inherited from the parent plists.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the GetDff test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                                                                   | **Comments*                        |
| ------------------ | ------------- | --------------- | -----------------------------------------------------------------------------| ---------------------------------- |
| Patlist            | Yes           | Plist           | Plist name to be executed                                                    |                                    |
| TimingsTc          | Yes           | TimingCondition | Levels test condition required for plist execution                           |                                    |
| LevelsTc           | Yes           | LevelsCondition | Timing test condition required for plist execution                           |                                    |
| CatastrophicFailLimit           | No            | Integer          | Specifies the number of fail pins in which the results are considered as a catastrophic fail detection. 9999 will be ituffed if the number of fail pins exceeds this limit. | Default value is 0 if none is defined.|
| TdoPinName           | Yes            | String          | Name of the TAP Data Output pin.  Needed to parse the output of the Boundary Scan chain. |                                    |
| VixPinsMask        | No           | String          | List of pins to mask on the boundary scan chain for Voltage In testing. Individual pin names only; pin groups not supported.           |                                    |
| VoxPinsMask        | No           | String          | List of external pins to mask for Voltage Out testing. Individual pin names only; pin groups not supported.     | |
| ExecuteMode        | No           | String (choice)            | Execution mode for this instance.  Either "OnePass" for standard functional execution,  “SEARCH_VIX” for a Voltage-In levels search, or “SEARCH_VOX”  for a Voltage-Out levels search. | Default is "OnePass" |
| VihVohSearchParams | Yes (SearchMode only) | String (comma seperated) | Search parameters for Voltage In/Out High for a specific pin group <”pin group”: start V: end V: step size>: i.e. “all_ddr:-0.2:0.4:0.1” One per pin group required. This is a REQUIRED parameter only if “ExecuteMode=SEARCH_VIX” or “ExecuteMode=SEARCH_VOX”. | Will work with "V" notation, i.e. “all_ddr:-0.2v:0.4v:0.1v” as well to support parameters transfer from EVG VixVox template. |
| VilVolSearchParams | Yes (SearchMode only) | String (comma seperated) | Search parameters for Voltage In/Out HLow for a specific pin group <”pin group”: start V: end V: step size>: i.e. “all_ddr:-0.2:0.4:0.1” One per pin group required. This is a REQUIRED parameter only if “ExecuteMode=SEARCH_VIX” or “ExecuteMode=SEARCH_VOX”. | Will work with "V" notation, i.e. “all_ddr:-0.2v:0.4v:0.1v” as well to support parameters transfer from EVG VixVox template. |
| DvMode             | Yes (SearchMode only) | Choice of FALSE (default) , TRUE, PERPINITUFF | DvMode selection. | |
| HryLoggingMode     | No           | Choice of DISABLED (default), ENABLED.      | Enable or Disable HRY logging mode. | |
| OnePassHryPins     | No           | String                                      | Hry Pins of interest for OnePass mode execution. | Required when executing OnePass with HRY mode enabled. |

**Notes:**
- As of this version, only OnePass, Search, DV is supported. Other modes are still WIP including HRY.
- If the total number of failing pins exceeds the user defined catastrophic_fail_limit (config file or instance parameter), then this is consider a catastrophic fail and “9999” will be logged instead of the individual failing channels. 
- If catastrophic_fail_limit is not being defined, it will be assigned to the total number of pins. This means that user will only hit the catastrophic fail limit if all pins fail.

## Modes Of Operation

## OnePass
This mode simply executes the pattern list provided via “patlist” instance parameter. The functionality is much the same as the standard functional test template plus the ability to map TAP Data Output failures to the appropriate tester channel.
If fails are detected on only external pins then the Exit Port will be Port 3 [Vox Only Fail].  If fails are detected only on the TAP Data Output pin on cycles with a mapping label then the Exit Port will be Port 2 [Vix Only Fail].  If failing data is found for both types of pins, then the Exit Port will be Port 0 [Vix & Vox Fail].  Is every other output pin that is not xxTDO considered a Vox pin.

### Datalog output
* Note1: Input “VIX” pins can only fail via the BSCAN TDO output pin.  Therefore, a prefix of “9” is put in front of the “DUT-mapped” output Channel# for each input pin that fails to indicate that this is not the actual channel that failed.

Vox pins failure ituff log:
```python
2_tname_VIXVOX::Execute_OnePass_VixVoxFail_F0
2_fdpmv_17
2_fcpmv_-1
2_fsdmv_-1
2_pttrn_tap_extest0_fail
2_vcont_18
2_faildata_{8036,8037,8038,8039}
```
Vix pins failure ituff log:

```python
2_tname_VIXVOX::Execute_OnePass_VixFail_F2
2_fdpmv_8
2_fcpmv_-1
2_fsdmv_-1
2_pttrn_tap_sample1_single_failvix
2_vcont_8
2_faildata_{98036}
```
In order to decode the output of the TDO pin and map each fail to an external pin name, this test template will capture ALL Fails.  All Fails is the only mode available.  However, each failing pin is reported only once per pattern.  The failing address reported (2_fdpmv_###) is the first address to contain a failing pin.

### Custom User Code Hooks

N/A

## SearchMode
Depending on SEARCH_VIX or SEARCH_VOX mode, these mode execute a level search on both VIL/VOL and VIH/VOH pattern list for each search parameter sets provided. By default, the search will stop on first fail test point of each search parameter sets provided.

The levels search is done using only the standard Timings and Levels information provided by the Instance Parameters TimingTc and LevelTc respectively.

The general algorithm use is as follows:
```
Load the standard Timings
Load the Pattern List
For Each Pin Group (search parameter set)
    Load the standard Levels
    For each Test Point [Voltage value]
        Force the Test Point Voltage on all pins in Pin Group
        Execute Pattern List
        Check Status
    End Test Point loop
End Pin Group loop
```
The same algorithm is applied for all VIH, VOH, VIL, VOL pattern list. Search wil always start with VIH/VOH followed by VIL/VOL.

SEARCH_VIX will requires pattern list name with matching regular expression of ".*vih_.*" for VIH, and ".*vil_.*" for VIL. eg. patternList1_vih_pinGroupOdd
SEARCH_VOX will requires pattern list name with matching regular expression of ".*voh_.*" for VOH, and ".*vol_.*" for VOL. eg. patternList2_vol_pinGroupEven

There must be ONE and ONLY ONE matching pattern list name defined for each type. If no match is found, or more than one match is found, then the test instance will Fail at Verify.

VihVohSearchParams and VilVolSearchParams must be provided with parameter sets.

When all fail, Vix Search will exit port 2, Vox Search will exit port 3.

### Datalog output

SEARCH_VIX,
```
2_tname_VIXVOX::Execute_SearchVix_FailVih_P1_vih_HPCC_DPIN_Dig_slcA_All
2_strgval_3|ixxHPCC_DPIN_Dig_slcA_AA4|1.35|ixxHPCC_DPIN_Dig_slcA_AA6|1.35|ixxHPCC_DPIN_Dig_slcA_BB0|1.35
2_comnt_failpins_{ixxHPCC_DPIN_Dig_slcA_AA4,ixxHPCC_DPIN_Dig_slcA_AA6,ixxHPCC_DPIN_Dig_slcA_BB0}
```
```
2_tname_VIXVOX::Execute_SearchVix_FailVil_P1_vil_HPCC_DPIN_Dig_slcA_All
2_strgval_3|ixxHPCC_DPIN_Dig_slcA_AA1|0.15|ixxHPCC_DPIN_Dig_slcA_AA2|0.15|ixxHPCC_DPIN_Dig_slcA_AA3|0.15
2_comnt_failpins_{ixxHPCC_DPIN_Dig_slcA_AA1,ixxHPCC_DPIN_Dig_slcA_AA2,ixxHPCC_DPIN_Dig_slcA_AA3}
```
SEARCH_VIX,
```
2_tname_VIXVOX::Execute_SearchVox_FailVoh_P1_voh_HPCC_DPIN_Dig_slcA_All
2_strgval_3|oxxHPCC_DPIN_Dig_slcA_AA1|1.55|oxxHPCC_DPIN_Dig_slcA_AA2|1.55|oxxHPCC_DPIN_Dig_slcA_AA3|1.55
2_comnt_failpins_{oxxHPCC_DPIN_Dig_slcA_AA1,oxxHPCC_DPIN_Dig_slcA_AA2,oxxHPCC_DPIN_Dig_slcA_AA3}
```
```
2_tname_VIXVOX::Execute_SearchVox_FailVol_P1_vol_HPCC_DPIN_Dig_slcA_All
2_strgval_3|oxxHPCC_DPIN_Dig_slcA_AA4|0.05|oxxHPCC_DPIN_Dig_slcA_AA5|0.05|oxxHPCC_DPIN_Dig_slcA_AA6|0.05
2_comnt_failpins_{oxxHPCC_DPIN_Dig_slcA_AA4,oxxHPCC_DPIN_Dig_slcA_AA5,oxxHPCC_DPIN_Dig_slcA_AA6}
```

2_tname_VIXVOX::Execute_SearchVix_FailVih_P1_**vih**_**HPCC_DPIN_Dig_slcA_All**
- **vih** indicate failing parameter sets from **VihVohSearchParams**.
- **HPCC_DPIN_Dig_slcA_All** indicate failing pin group. Parameter sets used VihVohSearchParams = "**HPCC_DPIN_Dig_slcA_All**:1.6V:1.3V:-0.025V".

2_strgval_**3**|ixxHPCC_DPIN_Dig_slcA_AA4|1.35|**ixxHPCC_DPIN_Dig_slcA_AA6**|**1.35**|ixxHPCC_DPIN_Dig_slcA_BB0|1.35
- **3** indicate number of pin failure. 3 pins are failing in this example.
- **ixxHPCC_DPIN_Dig_slcA_AA6** indicate the failing pin followed by voltage of **1.35**.

## SearchMode with DvMode = True
Setting DvMode = True will provide ituff output with best and worst case failure level for each search parameter group to give a quick upper and lower margin of that group. Ituff will print out the 1st level failure and the pins that failed at that level. Ituff will also print out a 2nd level if and when ALL pins in the search parameter group fail along with a list of all the unmasked pins of that group.
When all fail, Vix Search will exit port 2, Vox Search will exit port 3.

### Datalog output
```
2_tname_VIXVOX::Execute_SearchVix_DvMode_FailVil_MaskPin_P1_vil_HPCC_DPIN_Dig_slcA_All
2_strgval_3|ixxHPCC_DPIN_Dig_slcA_AA4|0.025|ixxHPCC_DPIN_Dig_slcA_AA6|0.025|ixxHPCC_DPIN_Dig_slcA_BB0|0.025
2_comnt_failpins_{ixxHPCC_DPIN_Dig_slcA_AA4,ixxHPCC_DPIN_Dig_slcA_AA6,ixxHPCC_DPIN_Dig_slcA_BB0}
2_tname_VIXVOX::Execute_SearchVix_DvMode_FailVil_MaskPin_P1_vil_HPCC_DPIN_Dig_slcA_All
2_strgval_4|ixxHPCC_DPIN_Dig_slcA_AA2|0.15|ixxHPCC_DPIN_Dig_slcA_AA4|0.15|ixxHPCC_DPIN_Dig_slcA_AA6|0.15|ixxHPCC_DPIN_Dig_slcA_BB0|0.15
2_comnt_failpins_{ixxHPCC_DPIN_Dig_slcA_AA2,ixxHPCC_DPIN_Dig_slcA_AA4,ixxHPCC_DPIN_Dig_slcA_AA6,ixxHPCC_DPIN_Dig_slcA_BB0}
```

When all test point fail, Vix Search will exit port 2, Vox Search will exit port 3, and print failure of 1st test point to ituff.

## SearchMode with DvMode = PerPinItuff
Setting DvMode = PerPinItuff provide ituff output a list of every pin that fails during each search paramter group. This list captures the level at which each pin first fails. If a pin never fails, no Ituff data will exist for that pin. If ALL pins in a search type pass then there will be no ITUFF output for that search parameter.
When all fail, Vix Search will exit port 2, Vox Search will exit port 3.

### Datalog output
```
2_tname_VIXVOX::Execute_SearchVix_DvModePerPinItuff_FailVih_MaskPin_P1_vih_HPCC_DPIN_Dig_slcA_All
2_strgval_4|ixxHPCC_DPIN_Dig_slcA_AA4|1.475|ixxHPCC_DPIN_Dig_slcA_AA6|1.475|ixxHPCC_DPIN_Dig_slcA_BB0|1.475|ixxHPCC_DPIN_Dig_slcA_AA2|1.35
2_comnt_failpins_{ixxHPCC_DPIN_Dig_slcA_AA4,ixxHPCC_DPIN_Dig_slcA_AA6,ixxHPCC_DPIN_Dig_slcA_BB0,ixxHPCC_DPIN_Dig_slcA_AA2}
```

When all test point fail, Vix Search will exit port 2, Vox Search will exit port 3, and print failure of 1st test point to ituff.

## HRY Logging Mode
HRY logging mode can be turn on by setting HryLog = "ENABLED". Additional OnePassHryPins parameter are required when executing it with OnePass mode.
Below is the definition for each of the HRY bits:
| Integer | Meaning           |
|---------|-------------------|
| 0       | Fail              |
| 1       | Pass              |
| 2       | Fail Low Limit    |
| 3       | Fail High Limit   |
| 4       | Railed Low        |
| 5       | Railed High       |
| 7       | DFT Fail          |
| 8       | Not tested        |

For VixVox, the only bits that is going to be used are 0, 1, 4, 5, 7, and 8.

### HRY logging for Vox Search
- In VOX search mode if HRY logging is turned on, it will log the pin of interest status to the HRY bits. Pins of interest will always be pin/pin group defined in search parameters (VihVohSearchParams and VilVolSearchParams).
- The pin order for HRY bits is from left to right (MSB). The way the pin arranged is:
  - If the pin defined is a pin group, the pin arrangement in the pin group will follow exactly as what is defined in the .pin file.
  - VOH and VOL pins will be printed in different line.
- This is how the pins are marked with each bits of the HRY bits:
  - For a failing pin, it will be marked as 0 [Fail].
  -For a pin that’s failing every test point, it will be marked as:
    - 4 [Railed Low]: For a negative search (high to low)
    - 5 [Railed High]: For a positive search (low to high)
  - For a pin that’s being masked it will be marked as 8 [Not tested]
  - Any other pin that is failing but is not in the list of pins of interest, it won’t be logged at all.
- Bit 7 [DFT Fail] is not going to be used in VOX search as VOX does not use TAP.

### HRY logging for VIX Search
- VIX search is similar to VOX with a slight different.
- For VIX search that uses the TAP pin it will checked with pin names in label.
  - If the pin is defined in the parameter, but is not defined in the label it will marked those pins as 8 [Not tested].
  - If the pin is not defined in the parameter but is defined in the label it will ignore those pins.
  - If the TDO pin is failing outside the label range, it will mark all the pins of interest as 7 [DFT fail].

### HRY logging for One Pass
- The only HRY data status that will be seen in ONE_PASS mode is 0[Fail], 1[Pass], 8[Untested] and 9[Invalid_Data].
- Note: Only the first failing pattern will be printed in datalog, hence only the first failing pattern contains the HRY datalogging. Do setup your plist appropriately if you required HRY datalogging for all your patterns.
- HRY status setting condition:
  - Pins will be executed as in a func test and produce a 0 and 1 according to the pass/fail status
  -  If the TDO pin is failing outside the label range, all pins specified in one_pass_hry_pins will be marked as 9[Invalid Data]
  - There will be 5 condition where the pins will then be set as 8 [Not_tested]
    - [1] pins specified in mask_pins template parameter
    - [2] pins specified in vix_pin_mask
    - [3] pins specified in vox_pin_mask
    - [4] pins specified that is masked in plist level
    - [5] pins specified that does not exist in vix pin label or vox pins 

### HRY logging examples
- All passing pins,
```
2_tname_VIXVOX::Execute_OnePass_Hry_AllPass_P1_HRY
2_strgval_111111111
```
- Masked pins,
```
2_tname_VIXVOX::Execute_OnePass_Hry_MaskPins_P1_HRY
2_strgval_881111111
```
- All failing pin,
```
2_tname_VIXVOX::Execute_OnePass_Hry_AllFail_P3_HRY
2_strgval_000000000
```
- Invalid Data,
```
2_tname_VIXVOX::Execute_OnePass_Hry_TDOFail_InvalidData_P3_HRY
2_strgval_999999999
```
- Railed High and Railed Low in search,
```
2_tname_VIXVOX::Execute_SearchVox_Hry_RailedHigh_RailedLow_P3_voh_HPCC_DPIN_Dig_slcA_All_HRY
2_strgval_85555555555555555555
2_tname_VIXVOX::Execute_SearchVox_Hry_RailedHigh_RailedLow_P3_vol_HPCC_DPIN_Dig_slcA_All_HRY
2_strgval_84444444444444444444
```
- DFT Fail,
```
2_tname_VIXVOX::Execute_SearchVix_Hry_DFTFail_P1_vih_HPCC_DPIN_Dig_slcA_All_HRY
2_strgval_77777777777777777777
2_tname_VIXVOX::Execute_SearchVix_Hry_DFTFail_P1_vil_HPCC_DPIN_Dig_slcA_All_HRY
2_strgval_77777777777777777777
```

## TPL Samples
**TPL Sample:**
```python
Import PrimeGetDffTestMethod.xml;

Test PrimeVixVoxTestMethod Execute_OnePass_BareMinimum_ToPass_P1
{
	LevelsTc = "VIXVOX::basic_func_lvl_nom";
	Patlist = "vixvox_list";
	TdoPinName = "xxHPCC_DPIN_Dig_slcA_AA0";
	TimingsTc = "VIXVOX::basic_func_timing_10MHz_20MHz";
	CatastrophicFailLimit = 0;
}
```

## Exit Ports

The test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**         | ***Alarm***    | Any alarm condition  |
| **-1**         | ***Error***    | Any software condition error |
| **0**         | ***Fail***      | Failed Both Vix and Vox Test |
| **1**         | ***Pass***      | Passing condition            |
| **2**         | ***Fail***      | Failed Only Vix Test |
| **3**         | ***Fail***      | Failed Only Vox Test |
  
## Additional Dependencies

N/A

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **VOH**: Voltage Output High
  - **VOL**: Voltage Output Low
  - **VIH**: Voltage Input High
  - **VIL**: Voltage Input Low
  - **VOX**: Short formed for both VOH and VOL
  - **VIX**: Short formed for both VIH and VIL
  - **TDO**: Test data out

## Version tracking

| **Date**        | **Version**    | **Author**          | **Comments** |
| --------------- | -------------- | ------------------- | ------------ |
| March     2022  | 1.0.0          | Mohd Faiz Mohd Asri | First Rev of VixVox with One Pass Mode Enabled             |
| April     2022  | 1.1.0          | Chen Tat, Khoh      | Enabled Search mode, Dv mode, added EOT power down.             |
| June      2022  | Prime V10.0    | Chen Tat, Khoh      | Enabled Dv mode, added EOT power down.             |
| June      2022  | Prime V11.0    | Chen Tat, Khoh      | Enable HRY mode.                                   |
| July      2022  | Prime V11.1    | Chen Tat, Khoh      | Ituff printout to follow Pin Finder tools format.  |
| July      2022  | Prime V11.1    | Chen Tat, Khoh      | Added known bugs. |
| October   2022  | Prime V12      | Chen Tat, Khoh      | Added known bugs. Remove EOTPowerDown parameters due to it replaced by ApplyEndSequence common parameter. |
| November  2022  | Prime V12      | Chen Tat, Khoh      | Add support for IDUT. |
| September 2023  | Prime 12.02.02 | Teoh, Khai Jie      | include fail pin name and fail channel printing to ituff. #39827 |
| November  2023  | Prime 12.03.00 | Teoh, Khai Jie      | enable witch project printing for Vixvox OnePassMode. #45199 |
| November  2023  | Prime 12.03.00 | Teoh, Khai Jie      | Disable Witch Project '2_tname_failchannel' and '3_binpinfails' printing to ituff.<br> #46008 |