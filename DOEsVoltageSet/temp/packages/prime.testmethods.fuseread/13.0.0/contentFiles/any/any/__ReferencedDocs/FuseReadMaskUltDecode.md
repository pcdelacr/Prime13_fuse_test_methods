<h1>Prime Test-Method Specification REP</h1>

Jan 2024

[[_TOC_]]

## Important Notice
<font color="red">**From Prime v12.00.00 onward**</font><br>
- 'Reverse' attribute in 'ConfigurationFile' is only applicable to Pattern Mode (ExecutionMode set to ExecutePattern). Simulation String is always expected to be MSB->LSB (LHS->RHS). <br>
- when 'Reverse' attribute is set to true and Execute Pattern Mode, this will Reverse the string of 'datalog fuse string' and 'fuse string composite' that will be compare with mask. <br>

## Methodology
The FuseReadMaskUltDecode test method have 2 main functions, one is use to compare fuse string (simulation string or ctv data) with mask value (static value and dynamic value) and save the mismatch result (if any), another is to use the same fuse string to decode to ULT (functionality is merged from UltDecoder Test Method). Additionally, there is another optional same mask execution but after the ultdecode functionality.

Fuse string can be datalog in format of "RLE", "HEX", "DEFLATE", "BINARY".

By Default, Fuse String is stored in the SharedStorage with the key "fusestring_< registerName >". Unless user defined test instance parameter "SharedStorageKeyToStore". See "Test Instance Parameters" section for description.

Mask value can be a static value or a dynamic value obtained from storage (UserVar, DFF and SharedStorage).

Mask value with 's' is to skip mismatch report for the bit, while 'm' is to replace with value from storage.

This test method is intended to decode the CTVs captured in the execution of a plist in lot number, wafer number and location X and Y of the Die Id.

The ULTDecoder functionality part decodes fuse string to Lot number, Wafer, X Location, and Y Location, it catches in the shared storage the lot, wafer, and location under the die id name with BusinessLogic.FuseLogicCollection.UltHandle functions, for DffRead to use.

When captured bit count is 50, the code is generated following this conversion:

![image.png](./.attachments/image-926e05fb-0c2b-4c03-a832-13c5d8c5bced1.png)

When captured bit count is 56, the code is generated following this conversion:

![image.png](./.attachments/image-0e9d3844-eeee-4d11-b038-1d47b5b66d8f1.png)

When captured bit count is 64, the code is generated following this conversion:

![Universal intel ULT decode.png](./.attachments/Universal%20intel%20ULT%20decode-e936fbde-838c-4a0c-9bdd-3e697d14afce1.png)

Intel Universal decoding.

## Json File Setup
### [Example] Configuration JSON File.
[See Example. (if the link is not working, proceed by manually navigating to FuseRead Readme.md)](./../../Readme.md) <br>

### [Example] Voltage JSON File to Enable VBump Software Trigger.
[See Example. (if the link is not working, proceed by manually navigating to FuseRead Readme.md)](./../../Readme.md) <br>

## Test Instance Parameters
The table below lists and describes the test instance parameters supported by the Fuse Read Mask ULT Decode test method.

| **Parameter Name**             | **Required?** | **Type**        | **Values**                                         | **Comments**                                                         |
| ------------------------------ | ------------- | --------------- | -------------------------------------------------- | -------------------------------------------------------------------- |
| ConfigurationFile              | Yes           | File            | Path to configuration file.                        | Able to use with "~HDMT_TPL_DIR". |
| ConfigName                     | Yes           | String          | FuseRead configuration name.                        | Configuration name for executing mask. Can have one name only. |
| VoltageFile                    | No            | File            | Path to voltage file.                              | Able to use with "~HDMT_TPL_DIR". Provide this file to enable functional test VBump Software Trigger.|
| RegisterName                   | Yes           | String          | FuseRead configuration register name.               | Register name for executing mask. Can have multiple register names separated by comma. |
| SharedStorageKeyToStore        | No            | CommaSeparatedString | SharedStorage Key name.                       | Allow user to store fusestring to provided sharedstorage key. The key must match number of 'RegisterName'. Example, 'RegisterName'="CPU0,PCH0", 'SharedStorageKeyToStore'="AnyKeyA,AnyKeyB". fusestring of CPU0 will store to AnyKeyA, PCH0 will store to AnyKeyB.|
| FailingMaskName                | No            | CommaSeparatedString | FuseRead configuration register mask name.     | Valid FailingMask name of the register or empty string. Empty string will bypass the functionality.  |
| PassingMaskName                | No            | CommaSeparatedString | FuseRead configuration register mask name.     | Valid PassingMask name of the register or empty string. Empty string will bypass the functionality.  |
| Patlist                        | No            | Plist           | Plist name to be executed.                         | Default is empty. |
| LevelsTc                       | No            | LevelsCondition | Levels test condition required for plist execution. | Default is empty. |
| TimingsTc                      | No            | TimingCondition | Timing test condition required for plist execution. | Default is empty. |
| PrePatlist                     | No            | String          | Preplist element to be executed.                    | Default is empty. |
| ExecutionMode                  | No            | String (choice) | ExecutePattern (Default), UsePreviousData, SimulationMode  | By default is in ExecutePattern mode. See below table for more details. |
| EnableCaptureFunctionalFailure | No            | String (choice) | ENABLED, DISABLED (Default)                              | DISABLED indicates CaptureCtvPerPinTest, ENABLED indicates CaptureFailureAndCtvPerPinTest. |
| MaskPins                       | No            | String          | Comma separated list of pins for which the fail data capture will be skipped. | Default is empty string.                    |
| FuseGroupToDatalog             | No            | CommaSeparatedString | Valid register FuseGroupName. | Default is empty. Use to datalog the fuse string of the FuseGroupName.|
| DieIdNames                     | Yes           | CommaSeparatedString | Die ID names.                                  | Multiple die ID names are allowed but need to match with RegisterName. If the register do not have UltDecode and need bypass, put "NA".|
| PackageEfuse                   | No            | String          | Die ID name.                                        | Default is empty. Die ID name needs to be from one of the DieIdNames, enable will print the die ID info under Package Efuse to Ituff. If MCP product does not need PKG, put "ALLOW_NO_PKG". |
| PrintUltDataPerDieIdToItuff    | No            | String (choice) | DISABLED, ENABLED (Default)                              | By default Ult info per die ID will print to ituff. DISABLED for SCP. |
| UltOffset                      | No            | CommaSeparatedInteger | OffSet Value for UltDecoder                        | If the CTVs have an offset you can define it here. Default 0, If one data is given it applies for all pins, otherwise a value for each pin must be given separated by commas. |
| PostUltExecuteMaskName         | No            | CommaSeparatedString | FuseRead configuration register mask name     | Valid mask name of the register or empty string. Empty string will bypass the functionality. Same as MaskName but happens after UltDecode.  |


Table for ExecutionMode:

| **ExecutionMode**             | **Pattern Executed?** | **Fuse String Source**                                          |
| ------------------------------| --------------------- | --------------------------------------------------------------- | 
| ExecutePattern(Default)       | Yes                   | Ctv from pattern execution.                                     |
| UsePreviousData               | No                    | Fuse string from previous fuseread instance.                    |
| SimulationMode                | No                    | Simulation string defined in input file or set through callback.|


### Passing mask & Failing mask logic :

FuseReadMaskUltDecode provide parameter failingMaskName and passingMaskName when executing masks.

When mask are specify in passingMaskName, if fuseString match with maskString, test method will continue to exit port 1, or else exit fail port if mismatch.

FailingMask act inversely to passingMask, given when FailingMaskName specified, if fuseString contains mismatch with maskString, this will lead to exit port 1, while a match will exit fail port instead.


### **Special Cases

1. FuseGroupToDatalog

         If test instance parameter FuseGroupToDatalog is being defined, the fuse string value of the fusegroup defined will be:
         - Printed to Ituff
         - Stored in DUT scope shared storage (string type) in the format of "fuseread_<FuseGroupName>" or "fuseread_<DieIDName>_<FuseGroupName>".

         If the FuseGroupName has backslash (\\), ex AAA\BBB_C, it is required to write them as double backslash (\\\\) because single backslash is considered as programming syntax for escape.

         Example for instance parameter. FuseGroupToDatalog = "AAA\\\BBB_C,XXX\\\YYY_Z";

         Example for fuseDef file. "Name": "AAA\\\BBB_C";

         The printing to ituff that includes fuse group name will still appear as single backslash.
         ```
         2_tname_<instance_name>_<registerName>_AAA\BBB_C
         2_tname_<instance_name>_<registerName>_XXX\YYY_Z
         ```

2. PrintUltDataPerDieIdToItuff

         If the parameter is enabled in SCP (Single Chip Product, which the only DieId is PKG), the upload to AQUA will fail and in the ituff data will appear a line with       
         "2_PREPROCERR_UNDEFINED_SUBSTRUCTURE_ID".

         Related work item: <https://dev.azure.com/mit-us/PRIME/_workitems/edit/35004>.

         Please make sure to set correctly between MCP and SCP.

## Parameters relation

![image.png](./.attachments/parameterRelationTable.png)


## Console Output
```
[A][TAL] StartTest PrimeFuseReadMaskUltDecodeTestMethod::FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1  2022-Sep-14 03:21:15
[DUT: 1]
=========================
Running Verify() for test instance=[FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1]
=========================
[DUT: 1]Below are the list of parameters and its value for this Instance:
[DUT: 1]ApplyEndSequence: DISABLED
[DUT: 1]BypassPort: -1
[DUT: 1]ConfigName: Config24
[DUT: 1]DieIdNames: U2.U1,U4
[DUT: 1]EnableCaptureFunctionalFailure: False
[DUT: 1]FuseGroupToDatalog:
[DUT: 1]IsInlineDff: DISABLED
[DUT: 1]LevelsTc: FUSEREAD::basic_func_lvl_nom
[DUT: 1]LogLevel: PRIME_DEBUG
[DUT: 1]PassingMaskName: Mask1
[DUT: 1]MaskPins:
[DUT: 1]MemoryAndTimeProfiling: DISABLED
[DUT: 1]PackageEfuse:
[DUT: 1]Patlist: ctv_plist
[DUT: 1]PostUltExecuteMaskName: Mask3,Mask2
[DUT: 1]PrePlist:
[DUT: 1]PrintUltDataPerDieIdToItuff: Enabled
[DUT: 1]RegisterName: CPU3,CPU4
[DUT: 1]ExecutionMode: ExecutePattern
[DUT: 1]TimingsTc: FUSEREAD::basic_func_timing_10MHz_20MHz
[DUT: 1]UltOffset: 0
[DUT: 1]
[DUT: 1]================================================================
[DUT: 1]Constructing handler for ConfigName=[Config24] RegisterName=[CPU3] SimulationMode=[False].
[DUT: 1]Constructing handler for ConfigName=[Config24] RegisterName=[CPU4] SimulationMode=[False].
[DUT: 1]Complete setup fuse read handler=[2].
[DUT: 1]Test instance=[FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1] verified using 20.919907 ms
[DUT: 1]
=========================
Running Execute() for test instance=[FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1]
=========================
[DUT: 1]PowerUpTc name is empty.
[DUT: 1]
[DUT: 1]PowerOnTC will not be applied. PowerOnTC Name is empty.
[DUT: 1]
[DUT: 1]Applied test condition=[FUSEREAD::basic_func_lvl_nom] but skipped by SmartTc.
[DUT: 1]
[DUT: 1]Applied test condition=[FUSEREAD::basic_func_timing_10MHz_20MHz] but skipped by SmartTc.
[DUT: 1]
[DUT: 1]Functional Test settings:
[DUT: 1]- Plist name=[ctv_plist].
[DUT: 1]- Levels test condition=[FUSEREAD::basic_func_lvl_nom].
[DUT: 1]- Timings test condition=[FUSEREAD::basic_func_timing_10MHz_20MHz].
[DUT: 1]- No pin mask set.
[DUT: 1]- No edge counter pins set.
[DUT: 1]- No software trigger set.
[DUT: 1]- No trigger map set.
[DUT: 1]- Ctv settings:
[DUT: 1]  --Ctv pins to capture=[xxHPCC_DPIN_Dig_slcB_A0, xxHPCC_DPIN_Dig_slcB_A1].
[DUT: 1]
[DUT: 1]Setting capture mode=[HdmtApi::CaptureMode::CaptureCTV] to domain=[DomainA_All_DPIN_Dig].
[DUT: 1]
[DUT: 1]Setting capture mode=[HdmtApi::CaptureMode::CaptureCTV] to domain=[DomainB_All_DPIN_Dig].
[DUT: 1]
[A][HAL] Starting burst group execution.
[A][HAL] Waiting 30000ms for execution to finish
[DUT: 1]	PinName=[xxHPCC_DPIN_Dig_slcB_A0]'s Ctv=[00000101010101010010101100000101010101010010101100].
[DUT: 1]	PinName=[xxHPCC_DPIN_Dig_slcB_A1]'s Ctv=[01000001010101010010101100000101010101010010101100].
[DUT: 1]Plist=[ctv_plist] has finished burst index=[0] with result=[PASS].
[DUT: 1]Execute Mask in ExecutionMode=[ExecutePattern].
[DUT: 1]Captured CtvData fuse value (LSB->MSB)=[00000101010101010010101100000101010101010010101100].
[DUT: 1]FuseString (MSB->LSB)=[00110101001010101010100000110101001010101010100000]
[DUT: 1]Printed to ituff:
[DUT: 1]2_tname_FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1_CPU3_fd1
[DUT: 1]2_strgalt_fus_msbF_00110101001010101010100000110101001010101010100000
[DUT: 1]
[DUT: 1]Fuse Composite String for Mask=[Mask1] FuseGroup=[*] String=[00110101001010101010100000110101001010101010100000]
[DUT: 1]Execute Mask=[Mask1] FuseGroup=[*] with FuseString[MSB->LSB]=[00110101001010101010100000110101001010101010100000] MaskValue[MSB->LSB]=[00110101001010101010100000110101001010101010100000].
[DUT: 1]STRING INDEX                   : 44444444443333333333222222222211111111110000000000
[DUT: 1]                               : 98765432109876543210987654321098765432109876543210
[DUT: 1]FUSE COMPOSITE VALUE           : 00110101001010101010100000110101001010101010100000
[DUT: 1]MASK VALUE                     : 00110101001010101010100000110101001010101010100000
[DUT: 1]FuseString for ULTDecode: 00110101001010101010100000110101001010101010100000
[DUT: 1]ULT data:
[DUT: 1]	Lot=[Y5106803]
[DUT: 1]	Wafer=[330]
[DUT: 1]	X Location=[-10]
[DUT: 1]	Y Location=[+00]
[DUT: 1]	Die ID=[U2.U1]
[DUT: 1][FUSEUFGL.S_U2U1_FULLBINARY] UserVar is set with value [00110101001010101010100000110101001010101010100000].
[FUSEUFGL.S_U2U1_GROUPNAME] UserVar is set with value [].
[FUSEUFGL.S_U2U1_LOT] UserVar is set with value [Y5106803].
[FUSEUFGL.S_U2U1_WAFER] UserVar is set with value [330].
[FUSEUFGL.S_U2U1_X] UserVar is set with value [-10].
[FUSEUFGL.S_U2U1_Y] UserVar is set with value [+00].
[FUSEUFGL.I_U2U1_WAFER] UserVar is set with value [330].
[FUSEUFGL.I_U2U1_X] UserVar is set with value [-10].
[FUSEUFGL.I_U2U1_Y] UserVar is set with value [+00].
[DUT: 1]Printed to ituff:
[DUT: 1]2_lsep
[DUT: 1]2_sstrlot_U2.U1_Y5106803
[DUT: 1]2_sstrwafer_U2.U1_330
[DUT: 1]2_sstrxloc_U2.U1_-10
[DUT: 1]2_sstryloc_U2.U1_+00
[DUT: 1]2_lsep
[DUT: 1]2_tname_FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1
[DUT: 1]2_tssid_U2.U1
[DUT: 1]2_strgalt_nsv_Y5106803_330_-10_+00
[DUT: 1]
[DUT: 1]Fuse Composite String for Mask=[Mask3] FuseGroup=[CPU_ULT_GROUP_B] String=[011010100101010]
[DUT: 1]Execute Mask=[Mask3] FuseGroup=[CPU_ULT_GROUP_B] with FuseString[MSB->LSB]=[011010100101010] MaskValue[MSB->LSB]=[011010100101010].
[DUT: 1]STRING INDEX                   : 111110000000000
[DUT: 1]                               : 432109876543210
[DUT: 1]FUSE COMPOSITE VALUE           : 011010100101010
[DUT: 1]MASK VALUE                     : 011010100101010
[DUT: 1]Fuse Composite String for Mask=[Mask2] FuseGroup=[CPU_ULT_GROUP_A] String=[1010100000]
[DUT: 1]Execute Mask=[Mask2] FuseGroup=[CPU_ULT_GROUP_A] with FuseString[MSB->LSB]=[1010100000] MaskValue[MSB->LSB]=[1010100000].
[DUT: 1]STRING INDEX                   : 0000000000
[DUT: 1]                               : 9876543210
[DUT: 1]FUSE COMPOSITE VALUE           : 1010100000
[DUT: 1]MASK VALUE                     : 1010100000
[DUT: 1]Captured CtvData fuse value (LSB->MSB)=[01000001010101010010101100000101010101010010101100].
[DUT: 1]FuseString (MSB->LSB)=[00110101001010101010100000110101001010101010000010]
[DUT: 1]Printed to ituff:
[DUT: 1]2_tname_FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1_CPU4_fd1
[DUT: 1]2_strgalt_fus_msbF_00110101001010101010100000110101001010101010000010
[DUT: 1]
[DUT: 1]Fuse Composite String for Mask=[Mask1] FuseGroup=[*] String=[00110101001010101010100000110101001010101010000010]
[DUT: 1]Execute Mask=[Mask1] FuseGroup=[*] with FuseString[MSB->LSB]=[00110101001010101010100000110101001010101010000010] MaskValue[MSB->LSB]=[00110101001010101010100000110101001010101010000010].
[DUT: 1]STRING INDEX                   : 44444444443333333333222222222211111111110000000000
[DUT: 1]                               : 98765432109876543210987654321098765432109876543210
[DUT: 1]FUSE COMPOSITE VALUE           : 00110101001010101010100000110101001010101010000010
[DUT: 1]MASK VALUE                     : 00110101001010101010100000110101001010101010000010
[DUT: 1]FuseString for ULTDecode: 00110101001010101010100000110101001010101010000010
[DUT: 1]ULT data:
[DUT: 1]	Lot=[Y5106803]
[DUT: 1]	Wafer=[330]
[DUT: 1]	X Location=[-10]
[DUT: 1]	Y Location=[+02]
[DUT: 1]	Die ID=[U4]
[DUT: 1][FUSEUFGL.S_U4_FULLBINARY] UserVar is set with value [00110101001010101010100000110101001010101010000010].
[FUSEUFGL.S_U4_GROUPNAME] UserVar is set with value [].
[FUSEUFGL.S_U4_LOT] UserVar is set with value [Y5106803].
[FUSEUFGL.S_U4_WAFER] UserVar is set with value [330].
[FUSEUFGL.S_U4_X] UserVar is set with value [-10].
[FUSEUFGL.S_U4_Y] UserVar is set with value [+02].
[FUSEUFGL.I_U4_WAFER] UserVar is set with value [330].
[FUSEUFGL.I_U4_X] UserVar is set with value [-10].
[FUSEUFGL.I_U4_Y] UserVar is set with value [+02].
[DUT: 1]Printed to ituff:
[DUT: 1]2_lsep
[DUT: 1]2_sstrlot_U4_Y5106803
[DUT: 1]2_sstrwafer_U4_330
[DUT: 1]2_sstrxloc_U4_-10
[DUT: 1]2_sstryloc_U4_+02
[DUT: 1]2_lsep
[DUT: 1]2_tname_FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1
[DUT: 1]2_tssid_U4
[DUT: 1]2_strgalt_nsv_Y5106803_330_-10_+02
[DUT: 1]
[DUT: 1]Fuse Composite String for Mask=[Mask3] FuseGroup=[CPU_ULT_GROUP_E] String=[011010100101010]
[DUT: 1]Execute Mask=[Mask3] FuseGroup=[CPU_ULT_GROUP_E] with FuseString[MSB->LSB]=[011010100101010] MaskValue[MSB->LSB]=[011010100101010].
[DUT: 1]STRING INDEX                   : 111110000000000
[DUT: 1]                               : 432109876543210
[DUT: 1]FUSE COMPOSITE VALUE           : 011010100101010
[DUT: 1]MASK VALUE                     : 011010100101010
[DUT: 1]Fuse Composite String for Mask=[Mask2] FuseGroup=[CPU_ULT_GROUP_D] String=[1010000010]
[DUT: 1]Execute Mask=[Mask2] FuseGroup=[CPU_ULT_GROUP_D] with FuseString[MSB->LSB]=[1010000010] MaskValue[MSB->LSB]=[1010000010].
[DUT: 1]STRING INDEX                   : 0000000000
[DUT: 1]                               : 9876543210
[DUT: 1]FUSE COMPOSITE VALUE           : 1010000010
[DUT: 1]MASK VALUE                     : 1010000010
[DUT: 1]Test instance=[FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1] executed using 42.892183 ms
[DUT: 1]TestInstance=[FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1] exit port=[1].
[A][TAL] StopTest PrimeFuseReadMaskUltDecodeTestMethod::FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1
```

## Datalog output
```
2_tname_FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1_CPU3_fd1
2_strgalt_fus_msbF_00110101001010101010100000110101001010101010100000
2_lsep
2_sstrlot_U2.U1_Y5106803
2_sstrwafer_U2.U1_330
2_sstrxloc_U2.U1_-10
2_sstryloc_U2.U1_+00
2_lsep
2_tname_FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1
2_tssid_U2.U1
2_strgalt_nsv_Y5106803_330_-10_+00
2_tname_FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1_CPU4_fd1
2_strgalt_fus_msbF_00110101001010101010100000110101001010101010000010
2_lsep
2_sstrlot_U4_Y5106803
2_sstrwafer_U4_330
2_sstrxloc_U4_-10
2_sstryloc_U4_+02
2_lsep
2_tname_FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1
2_tssid_U4
2_strgalt_nsv_Y5106803_330_-10_+02
```
If the ULTDecode fails
```
2_tname_FuseRead::ExecutePattern_x2_PostUltDecodeMask_1PreMask_UltDecode_Fail_F3_CPU0_fd1
2_strgalt_fus_msbF_1111111000000111100
2_lsep
2_sstrlot_U2.U1_9999999
2_sstrwafer_U2.U1_999
2_sstrxloc_U2.U1_99
2_sstryloc_U2.U1_99
2_lsep
2_tname_FuseRead::ExecutePattern_x2_PostUltDecodeMask_1PreMask_UltDecode_Fail_F3
2_tssid_U2.U1
2_strgalt_nsv_9999999_999_99_99
```

## Efuse datalog output example
When flow is SORT
```
2_lsep
2_tname_FuseRead::ExecutePattern_x1_PostUltDecodeMask_All_Pass_P1
2_comnt_trlot_Y5106803
2_comnt_trwafer_330
2_comnt_trxloc_-10
2_comnt_tryloc_+00
```

When flow is different from SORT
```
2_lsep
2_tname_FuseRead::ExecutePattern_x1_PostUltDecodeMask_All_Pass_P1
2_trlot_Y5106803
2_trwafer_330
2_trxloc_-10
2_tryloc_+00
```

## Custom User Code Hooks
Example:

void CustomExecute(FuseReadMaskTestInstanceResults fuseReadTestInstanceResults);
```c#
        /// <summary>
        /// Same execute mask but with additional message printed and execute multiple mask and customize the exit port.
        /// </summary>
        /// <param name="fuseReadTestInstanceResults">Fuse read handler that contain test register information.</param>
        void IFuseReadMaskUltDecodeExtensions.CustomExecute(FuseReadMaskUltDecodeTestInstanceResults fuseReadTestInstanceResults)
        {
            this.ExecuteFailingMask(fuseReadTestInstanceResults);
            if (fuseReadTestInstanceResults.ExitPort != 1)
            {
                return;
            }

            this.ExecutePassingMask(fuseReadTestInstanceResults);
        }
```
void CustomDatalogFuseStringByFuseGroupName(IFuseReadHandle fuseReadHandle);
```c#
        /// <summary>
        /// Same DatalogFuseStringByFuseGroupName but with additional message printed and customize the ituff instance name.
        /// </summary>
        /// <param name="fuseReadHandle">Fuse read handler that contain test register information.</param>
        void IFuseReadMaskExtensions.CustomDatalogFuseStringByFuseGroupName(IFuseReadHandle fuseReadHandle)
        {
            // Reminder, always take the test method code from the Prime version you are using as baseline)

            // Customize code
            Prime.Services.ConsoleService.PrintDebug($"this message is from mask CustomDatalogFuseStringByFuseGroupName.");

            Prime.Services.ConsoleService.PrintDebug($"Datalog FuseGroup=[{this.FuseGroupToDatalog}].");

            List<string> fuseGroupNameToDatalog = this.FuseGroupToDatalog;
            foreach (var fuseGroupName in fuseGroupNameToDatalog)
            {
                try
                {
                    var ituffStrValWritter = Prime.Services.DatalogService.GetItuffStrgvalWriter();

                    // Example to change ituff test instance name of FuseGroupToDatalog.
                    ituffStrValWritter.SetTnamePostfix($"_{fuseReadHandle.GetRegisterName()}_{fuseGroupName}_SOMETHING_CUSTOMIZE");
                    ituffStrValWritter.SetData(fuseReadHandle.GetFuseStringByFuseGroup(fuseGroupName));
                    Prime.Services.DatalogService.WriteToItuff(ituffStrValWritter);
                }
                catch
                {
                    throw new TestMethodException($"This FuseGroupName=[{fuseGroupName}] is not valid for Register=[{fuseReadHandle.GetRegisterName()}].");
                }
            }
        }
```
void UltCustomExecute(FuseReadMaskUltDecodeTestInstanceResults fuseReadTestInstanceResults);
```c#
        /// <summary>
        /// This function provide user the ability to customize execute UltDecode.
        /// </summary>
        /// <param name="fuseReadTestInstanceResults">Fuse read handle of the register.</param>
        void IFuseReadMaskUltDecodeExtensions.UltCustomExecute(FuseReadMaskUltDecodeTestInstanceResults fuseReadTestInstanceResults)
        {
            // Reminder, always take the test method code from the Prime version you are using as baseline)

            // Customize code
            Prime.Services.ConsoleService.PrintDebug($"this message is from UltCustomExecute.");
            fuseReadTestInstanceResults.ExitPort = this.UltDecoder(fuseReadTestInstanceResults);
        }
```
void PostUltCustomExecuteMask(FuseReadMaskUltDecodeTestInstanceResults fuseReadTestInstanceResults);
```c#
        /// <summary>
        /// This function provide user the ability to customize execute another mask after UltDecode.
        /// </summary>
        /// <param name="fuseReadTestInstanceResults">Fuse read handle of the register.</param>
        void IFuseReadMaskUltDecodeExtensions.PostUltCustomExecuteMask(FuseReadMaskUltDecodeTestInstanceResults fuseReadTestInstanceResults)
        {
            // Reminder, always take the test method code from the Prime version you are using as baseline)

            // Customize code
            Prime.Services.ConsoleService.PrintDebug($"this message is from customize post ultdecode execute mask. Executing for {this.MaskName}");
            if (this.postUltMaskNames.Any())
            {
                fuseReadTestInstanceResults.ExitPort = 1;
                foreach (var maskName in this.postUltMaskNames)
                {
                    if (!fuseReadTestInstanceResults.ExecuteFuseReadHandle.ExecuteMask(maskName))
                    {
                        Prime.Services.ConsoleService.PrintError($"Mask execution failed for MaskName=[{maskName}].");
                        fuseReadTestInstanceResults.ExitPort = fuseReadTestInstanceResults.ExecuteFuseReadHandle.GetFuseMaskFailPort(maskName);
                        break;
                    }
                }
            }
        }
```
void IFuseReadMaskExtensions.ExecutePassingMask(FuseReadMaskTestInstanceResults fuseReadTestInstanceResults);
```c++
        /// <summary>
        /// Same execute mask but with additional message printed and execute multiple mask.
        /// </summary>
        /// <param name="fuseReadTestInstanceResults">Fuse read handler that contain test register information.</param>
        void IFuseReadMaskExtensions.ExecutePassingMask(FuseReadMaskTestInstanceResults fuseReadTestInstanceResults)
        {
            Prime.Base.ServiceStore<IConsoleService>.Service.PrintDebug(() => $"this message is from customize execute passing mask. Executing for {this.PassingMaskName}", this.SessionContext);

            // Execute Multiple Mask and Get the status.
            Dictionary<string, bool> executeMaskStatus = new Dictionary<string, bool>();
            if (this.PassingMaskName != string.Empty)
            {
                List<string> maskNames = this.PassingMaskName;

                foreach (var executeMaskName in maskNames)
                {
                    executeMaskStatus.Add(executeMaskName, fuseReadTestInstanceResults.ExecuteFuseReadHandle.ExecuteMask(executeMaskName));
                }

                // Check any execute mask is fail (mismatch found).
                if (executeMaskStatus.ContainsValue(false))
                {
                    Prime.Base.ServiceStore<IConsoleService>.Service.PrintError($"The following Mask execution failed:");

                    foreach (var maskStatus in executeMaskStatus)
                    {
                        if (maskStatus.Value.Equals(false))
                        {
                            Prime.Base.ServiceStore<IConsoleService>.Service.PrintError($"MaskName=[{maskStatus.Key}] contains mismatch.");
                        }
                    }

                    fuseReadTestInstanceResults.ExitPort = 0;
                }
                else
                {
                    fuseReadTestInstanceResults.ExitPort = 1;
                }
            }
            else
            {
                Prime.Base.ServiceStore<IConsoleService>.Service.PrintDebug(() => $"MaskName is empty. Hence no execute mask.", this.SessionContext);
                fuseReadTestInstanceResults.ExitPort = 1;
            }
        }
```
void IFuseReadMaskExtensions.ExecuteFailingMask(FuseReadMaskTestInstanceResults fuseReadTestInstanceResults);
```c++
        /// <summary>
        /// Same execute mask but with additional message printed and execute multiple mask.
        /// </summary>
        /// <param name="fuseReadTestInstanceResults">Fuse read handler that contain test register information.</param>
        void IFuseReadMaskExtensions.ExecuteFailingMask(FuseReadMaskTestInstanceResults fuseReadTestInstanceResults)
        {
            Prime.Base.ServiceStore<IConsoleService>.Service.PrintDebug(() => $"this message is from customize execute failing mask. Executing for {this.FailingMaskName}", this.SessionContext);

            // Execute Multiple Mask and Get the status.
            Dictionary<string, bool> executeMaskStatus = new Dictionary<string, bool>();
            if (this.FailingMaskName != string.Empty)
            {
                List<string> maskNames = this.FailingMaskName;

                foreach (var executeMaskName in maskNames)
                {
                    executeMaskStatus.Add(executeMaskName, fuseReadTestInstanceResults.ExecuteFuseReadHandle.ExecuteMask(executeMaskName));
                }

                // Check any execute mask is passed (mismatch not found).
                if (executeMaskStatus.ContainsValue(true))
                {
                    Prime.Base.ServiceStore<IConsoleService>.Service.PrintError($"The following Mask execution passed:");

                    foreach (var maskStatus in executeMaskStatus)
                    {
                        if (maskStatus.Value.Equals(true))
                        {
                            Prime.Base.ServiceStore<IConsoleService>.Service.PrintError($"MaskName=[{maskStatus.Key}] does not have mismatch.");
                        }
                    }

                    fuseReadTestInstanceResults.ExitPort = 0;
                }
                else
                {
                    fuseReadTestInstanceResults.ExitPort = 1;
                }
            }
            else
            {
                Prime.Base.ServiceStore<IConsoleService>.Service.PrintDebug(() => $"MaskName is empty. Hence no execute mask.", this.SessionContext);
                fuseReadTestInstanceResults.ExitPort = 1;
            }
        }
```

## TPL Samples
Here are a few test instances examples using the Fuse Read Mask UltDecode test method:
*Refer to inputfiles for the configuration.

Execute without mask and UltDecoder, Single Register, Simulation Mode, Reverse String.
```c++
Test PrimeFuseReadMaskUltDecodeTestMethod SimulationMode_SingleRegister_Reverse_DatalogRLE_P1
{
   ConfigName = "Config2";
   PassingMaskName = "";
   RegisterName = "CPU0_2";
   LogLevel = "PRIME_DEBUG";
   DieIdNames = "NA";
}
```

Execute without mask and UltDecoder, Single Register, Execute Pattern.
```c++
Test PrimeFuseReadMaskUltDecodeTestMethod ExecutePattern_SingleRegister_FuncTestNoCaptureFailure_DatalogHex_P1
{
   ConfigName = "Config3";
   PassingMaskName = "";
   RegisterName = "CPU0_2";
   ExecutionMode = "ExecutePattern"; 
   EnableCaptureFunctionalFailure = "False";
   Patlist = "ctv_plist";
   PrePatlist = "";
   LevelsTc = "FUSEREAD::basic_func_lvl_nom";
   TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
   LogLevel = "PRIME_DEBUG";
   DieIdNames = "NA";
}
```

Execute with mask, Single Register, Execute Pattern. (No UltDecoder)
```c++
Test PrimeFuseReadMaskUltDecodeTestMethod ExecutePattern_SingleRegister_StaticMask_DatalogBinary_P1
{
   ConfigName = "Config7";
   PassingMaskName = "Mask1";
   RegisterName = "CPU0_2";
   ExecutionMode = "ExecutePattern";
   EnableCaptureFunctionalFailure = "False";
   Patlist = "ctv_plist";
   PrePatlist = "";
   LevelsTc = "FUSEREAD::basic_func_lvl_nom";
   TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
   LogLevel = "PRIME_DEBUG";
   DieIdNames = "NA";
}
```

Execute with mask, Multiple Register, Execute Pattern, Mask Off FuseGroup.
```c++
Test PrimeFuseReadMaskUltDecodeTestMethod ExecutePattern_MultipleRegister_FuseMaskOff_StaticMask_DatalogBinary_F0
{
   ConfigName = "Config17";
   PassingMaskName = "Mask1";
   RegisterName = "CPU0_2,PCH0";
   ExecutionMode = "ExecutePattern";
   Patlist = "ctv_plist";
   PrePatlist = "";
   LevelsTc = "FUSEREAD::basic_func_lvl_nom";
   TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
   LogLevel = "PRIME_DEBUG";
}
```

Execute with mask, Multiple Register, Execute Pattern, Mask Off FuseGroup, multiple UltDecode.
```c++
Test PrimeFuseReadMaskUltDecodeTestMethod ExecutePattern_x2_PostUltDecodeMask_All_Pass_P1
{
	ConfigName = "Config24";
	PassingMaskName = "Mask1";
	RegisterName = "CPU3,CPU4";
	ExecutionMode = "ExecutePattern";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	DieIdNames = "U2.U1,U4";
	PackageEfuse = "ALLOW_NO_PKG";
}
```
Example results on UserVar, this will have set of U2.U1, U4 only (NO PKG).

![UserVar2DieIDWithNoPackEfuse.png](./.attachments/UserVar2DieIDWithNoPackEfuse.png)

Execute with mask, Multiple Register, Execute Pattern, Mask Off FuseGroup, multiple UltDecode, Mask Off FuseGroup After UltDecode.
```c++
Test PrimeFuseReadMaskUltDecodeTestMethod ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1
{
	ConfigName = "Config24";
	PassingMaskName = "Mask1";
	RegisterName = "CPU3,CPU4";
	ExecutionMode = "ExecutePattern";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	DieIdNames = "U2.U1,U4";
	PostUltExecuteMaskName = "Mask3,Mask2";
	PackageEfuse = "U4";
}
```

Execute with mask, Multiple Registers, Execute Pattern, Mask Off FuseGroup, multiple UltDecode, with PackageEfuse Ituff.
```c++
Test PrimeFuseReadMaskUltDecodeTestMethod ExecutePattern_x2_PostUltDecodeMask_All_Pass_WithEFuse_P1
{
	ConfigName = "Config24";
	PassingMaskName = "Mask1";
	RegisterName = "CPU3,CPU4";
	ExecutionMode = "ExecutePattern";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	DieIdNames = "U2.U1,U4";
	PackageEfuse = "U4";
}
```
Execute Static failing mask, expecting test to pass when mismatch happen.
```c++
Test PrimeFuseReadMaskTestMethod ExecutePattern_StaticMask_FailingMaskName_ContainMismatch_P1
{
    ConfigurationFile = "~HDMT_TPL_DIR/TestPrograms/FuseRead/Modules/FuseRead/InputFiles/Hdmt/scenarioMask.fuseRead.json";
    ConfigName = "Config6"; 
    FailingMaskName = "Mask1"; 
    RegisterName = "CPU0"; 
    SimulationMode = "False";
    Patlist = "ctv_plist";
    PrePlist = "";
    LevelsTc = "FuseRead::basic_func_lvl_nom";
    TimingsTc = "FuseRead::basic_func_timing_10MHz_20MHz";
    LogLevel = "Enabled";
}
```

Execute Single Register, Static failing mask, expecting test to pass when mismatch happen. Store fusestring to custom storage.
```c++
Test PrimeFuseReadMaskTestMethod ExecutePattern_StaticMask_FailingMaskName_ContainMismatch_P1
{
    ConfigurationFile = "~HDMT_TPL_DIR/TestPrograms/FuseRead/Modules/FuseRead/InputFiles/Hdmt/scenarioMask.fuseRead.json";
    ConfigName = "Config6"; 
    FailingMaskName = "Mask1"; 
    RegisterName = "CPU0";
    SharedStorageKeyToStore = "AnyKeyCPU";
    SimulationMode = "False";
    Patlist = "ctv_plist";
    PrePlist = "";
    LevelsTc = "FuseRead::basic_func_lvl_nom";
    TimingsTc = "FuseRead::basic_func_timing_10MHz_20MHz";
    LogLevel = "Enabled";
}
```

Execute Multiple Register, Static failing mask, expecting test to pass when mismatch happen. Store fusestring to custom storage.
```c++
Test PrimeFuseReadMaskTestMethod ExecutePattern_StaticMask_FailingMaskName_ContainMismatch_P1
{
    ConfigurationFile = "~HDMT_TPL_DIR/TestPrograms/FuseRead/Modules/FuseRead/InputFiles/Hdmt/scenarioMask.fuseRead.json";
    ConfigName = "Config6"; 
    FailingMaskName = "Mask1"; 
    RegisterName = "CPU0,PCH0";
    SharedStorageKeyToStore = "AnyKeyCPU,AnyKeyPCH";
    SimulationMode = "False";
    Patlist = "ctv_plist";
    PrePlist = "";
    LevelsTc = "FuseRead::basic_func_lvl_nom";
    TimingsTc = "FuseRead::basic_func_timing_10MHz_20MHz";
    LogLevel = "Enabled";
}
```


Static failing mask with mismatch, and static passing mask with match, this is a scenario both failing and passing mask are use.
```c++
Test PrimeFuseReadMaskTestMethod ExecutePattern_StaticMask_FailingPassingMask_MismatchAndMatch_P1
{
    ConfigurationFile = "~HDMT_TPL_DIR/TestPrograms/FuseRead/Modules/FuseRead/InputFiles/Hdmt/scenarioMask.fuseRead.json";
    ConfigName = "Config7"; 
    FailingMaskName = "Mask4";
    PassingMaskName = "Mask1";
    RegisterName = "CPU0"; 
    SimulationMode = "False";
    Patlist = "ctv_plist";
    PrePlist = "";
    LevelsTc = "FuseRead::basic_func_lvl_nom";
    TimingsTc = "FuseRead::basic_func_timing_10MHz_20MHz";
    LogLevel = "Enabled";
}
```
Example results on UserVar, this will have set of PKG, U2.U1, U4

![UserVar2DieIDWithPackEfuse.png](./.attachments/UserVar2DieIDWithPackEfuse.png)

JsonFile Sample for MCP, that is used to set up instances above
```
{
    "Configurations": [
        {
            "Name": "Config24",
			"Registers": [
				{
					"Name": "CPU3",
					"SimulationString": "00000101010101010010101100000101010101010010101100",
					"Masks": [
						{
							"Name": "Mask1",
							"FuseGroup": "*",
							"Value": "00110101001010101010100000110101001010101010100000"
						},
						{
							"Name": "Mask2",
							"FuseGroup": "CPU_ULT_GROUP_A",
							"Value": "1010100000",
							"FailPort": 2
						},
						{
							"Name": "Mask3",
							"FuseGroup": "CPU_ULT_GROUP_B",
							"Value": "011010100101010"
						}
					],
					"UltDecode": {
						"FuseGroup": "ULTGROUP",
						"FailPort": 3
					}
				},
				{
					"Name": "CPU4",
					"SimulationString": "01000001010101010010101100000101010101010010101100",
					"Masks": [
						{
							"Name": "Mask1",
							"FuseGroup": "*",
							"Value": "00110101001010101010100000110101001010101010000010"
						},
						{
							"Name": "Mask2",
							"FuseGroup": "CPU_ULT_GROUP_D",
							"Value": "1010000010",
							"FailPort": 2
						},
						{
							"Name": "Mask3",
							"FuseGroup": "CPU_ULT_GROUP_E",
							"Value": "011010100101010"
						}
					],
					"UltDecode": {
						"FuseGroup": "*",
						"FailPort": 3
					}
				}
			]
        }
    ]
}
```
Execute UltDecode only, with no mask, by leaving MaskName and PostUltExecuteMaskName parameter empty.
<pre><code>Test PrimeFuseReadMaskUltDecodeTestMethod ExecutePattern_x2_UltDecode_NoMask_Pass_P1
{
	ConfigName = "Config24";
	<span style="background-color:yellow">MaskName = "";</span>
	RegisterName = "CPU3,CPU4";
	ExecutionMode = "ExecutePattern";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	DieIdNames = "U2.U1,U4";
	<span style="background-color:yellow">PostUltExecuteMaskName = "";</span>
	PackageEfuse = "U4";
}
</code></pre>

Execute mask only, with no ULTDecoder, by setting DieIdNames to "NA", with same amount in RegisterName. PackageEfuse also needs to be "NA".
<pre><code>Test PrimeFuseReadMaskUltDecodeTestMethod ExecutePattern_x2_NoUltDecode_1PreMask_2PostMask_Pass_P1
{
	ConfigName = "Config24";
	PassingMaskName = "Mask1";
	RegisterName = "CPU3,CPU4";
	ExecutionMode = "ExecutePattern";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	<span style="background-color:yellow">DieIdNames = "NA,NA";</span>
	PostUltExecuteMaskName = "Mask3,Mask2";
	<span style="background-color:yellow">PackageEfuse = "NA";</span>
}
</code></pre>
JsonFile Sample for instances above, <span style="background-color:yellow">"UltDecode" setting block is removed.</span>

Verify will check with parameter "DieIdNames" with "UltDecode" setting block from Json respectively, if there is mismatch, it will error out.
```
{
    "Configurations": [
        {
            "Name": "Config24",
			"Registers": [
				{
					"Name": "CPU3",
					"SimulationString": "00000101010101010010101100000101010101010010101100",
					"Masks": [
						{
							"Name": "Mask1",
							"FuseGroup": "*",
							"Value": "00110101001010101010100000110101001010101010100000"
						},
						{
							"Name": "Mask2",
							"FuseGroup": "CPU_ULT_GROUP_A",
							"Value": "1010100000",
							"FailPort": 2
						},
						{
							"Name": "Mask3",
							"FuseGroup": "CPU_ULT_GROUP_B",
							"Value": "011010100101010"
						}
					]
				},
				{
					"Name": "CPU4",
					"SimulationString": "01000001010101010010101100000101010101010010101100",
					"Masks": [
						{
							"Name": "Mask1",
							"FuseGroup": "*",
							"Value": "00110101001010101010100000110101001010101010000010"
						},
						{
							"Name": "Mask2",
							"FuseGroup": "CPU_ULT_GROUP_D",
							"Value": "1010000010",
							"FailPort": 2
						},
						{
							"Name": "Mask3",
							"FuseGroup": "CPU_ULT_GROUP_E",
							"Value": "011010100101010"
						}
					]
				}
			]
        }
    ]
}
```

Execute with masks, and only one of ULTDecoder, by setting one of the DieIdNames to "NA", but still with same amount in RegisterName. PackageEfuse also needs to be "NA".
<pre><code>Test PrimeFuseReadMaskUltDecodeTestMethod Execute_x2_1Mask_1UltDecode_1NADieIdName_P1
{
	ConfigName = "Config27";
	PassingMaskName = "Mask1";
	RegisterName = "CPU3,CPU4";
	ExecutionMode = "ExecutePattern";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	<span style="background-color:yellow">DieIdNames = "U2.U1,NA";</span>
	PostUltExecuteMaskName = "Mask3,Mask2";
	<span style="background-color:yellow">PackageEfuse = "NA";</span>
}
</code></pre>
JsonFile Sample to set up instances above, <span style="background-color:yellow">"UltDecode" setting block on desired "Registers" is removed.</span>

Verify will check with parameter "DieIdNames" with "UltDecode" setting block from Json respectively, if there is mismatch, it will error out.
```
{
    "Configurations": [
        {
            "Name": "Config27",
			"Registers": [
				{
					"Name": "CPU3",
					"SimulationString": "00000101010101010010101100000101010101010010101100",
					"Masks": [
						{
							"Name": "Mask1",
							"FuseGroup": "*",
							"Value": "00110101001010101010100000110101001010101010100000"
						},
						{
							"Name": "Mask2",
							"FuseGroup": "CPU_ULT_GROUP_A",
							"Value": "1010100000",
							"FailPort": 2
						},
						{
							"Name": "Mask3",
							"FuseGroup": "CPU_ULT_GROUP_B",
							"Value": "011010100101010"
						}
					],
					"UltDecode": {
						"FuseGroup": "ULTGROUP",
						"FailPort": 3
					}
				},
				{
					"Name": "CPU4",
					"SimulationString": "01000001010101010010101100000101010101010010101100",
					"Masks": [
						{
							"Name": "Mask1",
							"FuseGroup": "*",
							"Value": "00110101001010101010100000110101001010101010000010"
						},
						{
							"Name": "Mask2",
							"FuseGroup": "CPU_ULT_GROUP_D",
							"Value": "1010000010",
							"FailPort": 2
						},
						{
							"Name": "Mask3",
							"FuseGroup": "CPU_ULT_GROUP_E",
							"Value": "011010100101010"
						}
					]
				}
			]
        }
    ]
}
```
## Exit Ports
FuseReadMask test method supports the following exit ports:
| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

## Additional Dependencies
N/A

## FlowCharts
Verify Flow

![VerifyFlowChart.png](./.attachments/VerifyFlowChart.png)

Main Execute Flow

![MainExecuteFlowChart.png](./.attachments/MainExecuteFlowChart.png)

Current inner function extensions

When execute CustomExecute function, ExecuteFailingMask and ExecutePassingMask function will run in sequence, given if FailingMaskNames and PassingMaskNames parameter are not empty.
If FailingMaskNames parameter is empty, CustomExecute will only run ExecutePassingMask. Otherwise, ExecuteFailingMask will be executed only.

![ExecuteFailingMask.png](./.attachments/ExecuteFailingMask.png)   ![ExecutePassingMask.png](./.attachments/ExecutePassingMask.png)

![UltCustomExecuteFlowChart.png](./.attachments/UltCustomExecuteFlowChart.png)

![PostUltCustomExecuteFlowChart.png](./.attachments/PostUltCustomExecuteFlowChart.png)

![CustomDatalogFuseStringByFuseGroupNameFlowChart.png](./.attachments/CustomDatalogFuseStringByFuseGroupNameFlowChart.png)


## Version Tracking
| Prime Version |Prime ticket reference| **Comments** |
| ------------- | -------------------- | ------------ |
| 13.00.00      | #43528               | New test instance parameter 'SharedStorageKeyToStore' for storing fusestring to user provided sharedstorage key.|
| 13.00.00      | #40512               | Enhance to support UsePreviousData execution mode.|
| 12.00.00      | #40493               | Changed ituff print for SORT and added psn token.|
| 12.00.00      | #36828               | DieIdNames vs PackageEfuse in PrimeFuseReadMaskUltDecodeTestMethod|
| 12.00.00      | #35703               | To Decode ULT even this ULT is not available at UBE File|
| 12.00.00      | #36224               | 'Reverse' attribute in configuration file should only apply to Pattern Mode and not apply to Simulation Mode on the Simulation String.|
| 12.00.00      | -                    | Removed Engineering printing to console. |
| 12.00.00      | #27977               | Enable functional test vbump software trigger. Parameter is setup with fuse read VoltageFile. |
| 11.01.00      | #33989               | Enable dynamic load for configuration file instead of using AlephInit |
| 11.01.00      | #27599               | New Test Method Completion |

## Acronyms
Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **DFF**: Data Feed Forward