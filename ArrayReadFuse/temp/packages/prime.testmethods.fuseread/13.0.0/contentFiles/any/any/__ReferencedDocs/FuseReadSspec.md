<h1>Prime Test-Method Specification REP</h1>

Jan 2024

[[_TOC_]]

## Important Notice
<font color="red">**From Prime v12.00.00 onward**</font><br>
- 'Reverse' attribute in 'ConfigurationFile' is only applicable to Pattern Mode (ExecutionMode set to ExecutePattern). Simulation String is always expected to be MSB->LSB (LHS->RHS). <br>
- when 'Reverse' attribute is set to true and Execute Pattern Mode, this will Reverse the string of 'datalog fuse string' and 'fuse string composite' that will be compare with mask. <br>

<font color="red">**From Prime v11.1 onward**</font><br>
- Fuseread testmethod no longer parse *.fuseread.json (configuration) file with AlephInit. The parsing will be at Verify(), please define the with test instance parameter 'ConfigurationFile'.<br>

## Methodology
The FuseReadSspec test method is use to compare fuse string (simulation string or ctv data) with sspec value (static value and dynamic value).

Fuse string can be datalog in format of "RLE", "HEX", "DEFLATE", "BINARY".

By Default, Fuse String is stored to sharedstorage with the key "fusestring_<registerName>". Unless user defined test instance parameter "SharedStorageKeyToStore". See "Test Instance Parameters" section for description.

Sspec value can be a static value or get from storage (UserVar, DFF and Sharedstorage).

Sspec value with 's' is to skip mismatch report for the bit, while 'm' is to replace with value from storage.

## Json File Setup
### [Example] Configuration JSON File.
[See Example. (if the link not working, proceed by manually navigate to FuseRead Readme.md)](./../../Readme.md) <br>

### [Example] Voltage JSON File to Enable VBump Software Trigger.
[See Example. (if the link not working, proceed by manually navigate to FuseRead Readme.md)](./../../Readme.md) <br>

## Test Instance Parameters
The table below lists and describes the test instance parameters supported by the Fuse Read Sspec test method.

| **Parameter Name**             | **Required?** | **Type**        | **Values**                                         | **Comments**                                                         |
| ------------------------------ | ------------- | --------------- | -------------------------------------------------- | -------------------------------------------------------------------- |
| ConfigurationFile              | Yes           | File            | Path to configuration file.                        | Able to use with "~HDMT_TPL_DIR".|
| ConfigName                     | Yes           | String          | FuseRead configuration name                        | Configuration name for execute sspec. Can have one name only. |
| RegisterName                   | Yes           | String          | FuseRead configuration register name               | Register name for execute sspec. Can have multiple register name separated by comma. |
| SharedStorageKeyToStore        | No            | CommaSeparatedString | SharedStorage Key name.                       | Allow user to store fusestring to provided sharedstorage key. The key must match number of 'RegisterName'. Example, 'RegisterName'="CPU0,PCH0", 'SharedStorageKeyToStore'="AnyKeyA,AnyKeyB". fusestring of CPU0 will store to AnyKeyA, PCH0 will store to AnyKeyB.|
| Patlist                        | No            | Plist           | Plist name to be executed.                         | Default is empty. |
| LevelsTc                       | No            | LevelsCondition | Levels test condition required for plist execution | Default is empty. |
| TimingsTc                      | No            | TimingCondition | Timing test condition required for plist execution | Default is empty. |
| PrePatlist                     | No            | String          | Preplist element to be executed                    | Default is empty. |
| ExecutionMode                  | No            | String (choice) | ExecutePattern (Default), UsePreviousData, SimulationMode  | By default is in ExecutePattern mode. See below table for more details. |
| EnableCaptureFunctionalFailure | No            | String (choice) | False, True (Default)                              | False indicate CaptureCtvPerPinTest, true indicate CaptureFailureAndCtvPerPinTest. |
| MaskPins                       | No            | String          | Comma separated list of pins for which the fail data capture will be skipped | Default is empty string                    |
| FuseGroupToDatalog             | No            | CommaSeparatedString | Valid Register FuseGroupName | Default is empty. Use to datalog the fuse string of the FuseGroupName.|


Table for ExecutionMode:

| **ExecutionMode**             | **Pattern Executed?** | **Fuse String Source**                                          |
| ------------------------------| --------------------- | --------------------------------------------------------------- | 
| ExecutePattern(Default)       | Yes                   | Ctv from pattern execution.                                     |
| UsePreviousData               | No                    | Fuse string from previous fuseread instance.                    |
| SimulationMode                | No                    | Simulation string defined in input file or set through callback.|

**Special Case

FuseGroupToDatalog

If test insteance parameter FuseGroupToDatalog is being defined, the fuse string value of the fusegroup defined will be:
- Printed to Ituff
- Stored in DUT scope shared storage(string type) in the format of "fuseread_<FuseGroupName>" or "fuseread_<dieIDName>_<FuseGroupName>".

if the FuseGroupName having backslash (\\), ex AAA\BBB_C, it is required to written as double backslash (\\\\) because single backslash is considered as programming syntax for escape.

Example for instance parameter. FuseGroupToDatalog = "AAA\\\BBB_C,XXX\\\YYY_Z";

Example for fuseDef file. "Name": "AAA\\\BBB_C";

The printing to ituff that include fuse group name will still appear as single backslash.
```
2_tname_<instance_name>_<registerName>_AAA\BBB_C
2_tname_<instance_name>_<registerName>_XXX\YYY_Z
```

## Console Output
```
=========================
Running Verify() for test instance=[FUSEREAD::ExecutePattern_MultipleRegister_MultipleConfiguration_StaticSspec_UservarFA69_DatalogBinary_P1]
=========================
[DUT: 1]Below are the list of parameters and its value for this Instance:
[DUT: 1]BypassPort: -1
[DUT: 1]ConfigName: Config19
[DUT: 1]EnableCaptureFunctionalFailure: False
[DUT: 1]LevelsTc: FUSEREAD::basic_func_lvl_nom
[DUT: 1]LogLevel: PRIME_DEBUG
[DUT: 1]MaskPins:
[DUT: 1]MemoryAndTimeProfiling: DISABLED
[DUT: 1]Patlist: ctv_plist
[DUT: 1]PrePatlist:
[DUT: 1]RegisterName: CPU0_2,PCH0
[DUT: 1]ExecutionMode: ExecutePattern
[DUT: 1]TimingsTc: FUSEREAD::basic_func_timing_10MHz_20MHz
[DUT: 1]
[DUT: 1]================================================================
[DUT: 1]Constructing handler for ConfigName=[Config19] RegisterName=[CPU0_2] ExecutionMode=[ExecutePattern].
[DUT: 1]Constructing handler for ConfigName=[Config19] RegisterName=[PCH0] ExecutionMode=[ExecutePattern].
[DUT: 1]Complete setup fuse read handler=[2].
=========================
Running Execute()
=========================
[DUT: 1]Execute Sspec in ExecutionMode=[ExecutePattern].
[DUT: 1]Captured CtvData fuse value (LSB->MSB)=[1111111000000111100].
[DUT: 1]FuseString (MSB->LSB)=[0011110000001111111]
[DUT: 1]Printed to ituff:
[DUT: 1]2_tname_FUSEREAD::ExecutePattern_MultipleRegister_MultipleConfiguration_StaticSspec_UservarFA69_DatalogBinary_P1_CPU0_2_fd1
[DUT: 1]2_strgalt_fus_msbF_0011110000001111111
[DUT: 1]
[DUT: 1]Fuse Composite String for Sspc=[FA69] FuseGroup=[*] String=[0011110000001111111]
[DUT: 1]Execute Sspec=[FA69] FuseGroup=[*] with FuseString[MSB->LSB]=[0011110000001111111] sspec value[MSB->LSB]=[0011110000001111111].
[DUT: 1]STRING INDEX                   : 1111111110000000000
[DUT: 1]                               : 8765432109876543210
[DUT: 1]FUSE COMPOSITE VALUE           : 0011110000001111111
[DUT: 1]SSPEC VALUE                    : 0011110000001111111
[DUT: 1]Fuse Composite String for Sspc=[FA69] FuseGroup=[GROUP_A] String=[111111]
[DUT: 1]Execute Sspec=[FA69] FuseGroup=[GROUP_A] with FuseString[MSB->LSB]=[111111] sspec value[MSB->LSB]=[111111].
[DUT: 1]STRING INDEX                   : 543210
[DUT: 1]FUSE COMPOSITE VALUE           : 111111
[DUT: 1]SSPEC VALUE                    : 111111
[DUT: 1]Captured CtvData fuse value (LSB->MSB)=[1111111000000111100].
[DUT: 1]FuseString (MSB->LSB)=[0011110000001111111]
[DUT: 1]Printed to ituff:
[DUT: 1]2_tname_FUSEREAD::ExecutePattern_MultipleRegister_MultipleConfiguration_StaticSspec_UservarFA69_DatalogBinary_P1_PCH0_fd1
[DUT: 1]2_strgalt_fus_msbF_0011110000001111111
[DUT: 1]
[DUT: 1]Fuse Composite String for Sspc=[FA69] FuseGroup=[*] String=[0011110000001111111]
[DUT: 1]Execute Sspec=[FA69] FuseGroup=[*] with FuseString[MSB->LSB]=[0011110000001111111] sspec value[MSB->LSB]=[0011110000001111111].
[DUT: 1]STRING INDEX                   : 1111111110000000000
[DUT: 1]                               : 8765432109876543210
[DUT: 1]FUSE COMPOSITE VALUE           : 0011110000001111111
[DUT: 1]SSPEC VALUE                    : 0011110000001111111
```

## Datalog output
```
2_tname_FUSEREAD::ExecutePattern_MultipleRegister_MultipleConfiguration_StaticSspec_UservarFA69_DatalogBinary_P1_CPU0_2_fd1
2_strgalt_fus_msbF_0011110000001111111
2_tname_FUSEREAD::ExecutePattern_MultipleRegister_MultipleConfiguration_StaticSspec_UservarFA69_DatalogBinary_P1_PCH0_fd1
2_strgalt_fus_msbF_0011110000001111111
```

## Custom User Code Hooks
Example - SampleTP - Test CustomExecuteSSPEC ExecutePattern_MultiRegister_MultiConfig_StaticSspec_UservarFA69_DatalogBinary_CustomSSPEC_P1

bool CustomExecute(IFuseReadHandle fuseReadHandle);
```c++
/// <summary>
/// Same execute sspec but with additional message printed.
/// </summary>
/// <param name="fuseReadHandle">Fuse read handler that contain test register information.</param>
/// <returns>Status of execute sspec.</returns>
bool IFuseReadSspecExtensions.CustomExecute(IFuseReadHandle fuseReadHandle)
{
    // Customize code
    Prime.Services.ConsoleService.PrintDebug($"this message is from customize execute sspec.");

    // Test method Code (always take the test method code from the Prime version you are using)-- start
    if (!fuseReadHandle.ExecuteSSPEC())
    {
        Prime.Services.ConsoleService.PrintError($"Sspec execution failed.");
        return false;
    }
    return true;
    // Test method Code -- end
}
```
void IFuseReadSspecExtensions.CustomDatalogFuseStringByFuseGroupName(IFuseReadHandle fuseReadHandle);
```c++
/// <summary>
/// Same DatalogFuseStringByFuseGroupName but with additional message printed.
/// </summary>
/// <param name="fuseReadHandle">Fuse read handler that contain test register information.</param>
void IFuseReadSspecExtensions.CustomDatalogFuseStringByFuseGroupName(IFuseReadHandle fuseReadHandle)
{
	// Customize code
	Prime.Services.ConsoleService.PrintDebug($"this message is from sspec CustomDatalogFuseStringByFuseGroupName.");

	// Test method Code (always take the test method code from the Prime version you are using) -- start
	Prime.Services.ConsoleService.PrintDebug($"Datalog FuseGroup=[{this.FuseGroupToDatalog}].");

	List<string> fuseGroupNameToDatalog = this.FuseGroupToDatalog;
	foreach (var fuseGroupName in fuseGroupNameToDatalog)
	{
		try
		{
			var ituffStrValWritter = Prime.Services.DatalogService.GetItuffStrgvalWriter();
			ituffStrValWritter.SetTnamePostfix($"_{fuseReadHandle.GetRegisterName()}_{fuseGroupName}");
			ituffStrValWritter.SetData(fuseReadHandle.GetFuseStringByFuseGroup(fuseGroupName));
			Prime.Services.DatalogService.WriteToItuff(ituffStrValWritter);
		}
		catch
		{
			throw new TestMethodException($"This FuseGroupName=[{fuseGroupName}] is not valid for Register=[{fuseReadHandle.GetRegisterName()}].");
		}
	}
	// Test method Code -- end
}
```

## TPL Samples
Here are a few test instance in SampleTP examples using the Fuse Read Sspec test method:
*Refer to inputfiles for the configuration

```c++
# Execute Sspec, Multiple Register, Static sspec, multiple configuration, no mismatch bits
Test PrimeFuseReadSspecTestMethod ExecutePattern_MultipleRegister_MultipleConfiguration_StaticSspec_UservarFA69_DatalogBinary_P1
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseRead/FuseRead/InputFiles/scenario19.fuseRead.json";
	ConfigName = "Config19";
	RegisterName = "CPU0_2,PCH0";
	ExecutionMode = "ExecutePattern";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
}

# Execute Sspec, Multiple Register, Static sspec, multiple configuration, no mismatch bits. Store fuse string to custom sharedstorage.
Test PrimeFuseReadSspecTestMethod ExecutePattern_MultipleRegister_MultipleConfiguration_StaticSspec_UservarFA69_DatalogBinary_P1
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseRead/FuseRead/InputFiles/scenario19.fuseRead.json";
	ConfigName = "Config19";
	RegisterName = "CPU0_2,PCH0";
	SharedStorageKeyToStore = "AnyKeyCPU,AnyKeyPCH";
	ExecutionMode = "ExecutePattern";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
}

# Execute Sspec, Single Register, Dynamic sspec, multiple configuration, contain mismatch bits
Test PrimeFuseReadSspecTestMethod ExecutePattern_SingleRegister_MultipleConfiguration_DynamicSspec_UserVar_SharedStorage_DatalogBinary_F0
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseRead/FuseRead/InputFiles/scenario21.fuseRead.json";
	ConfigName = "Config21";
	RegisterName = "CPU0_2";
	ExecutionMode = "ExecutePattern";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	FuseGroupToDatalog = "GROUP_A,GROUP_B";
}

# Execute Sspec, Single Register, Dynamic sspec, multiple configuration, contain mismatch bits. Store fuse string to custom sharedstorage.
Test PrimeFuseReadSspecTestMethod ExecutePattern_SingleRegister_MultipleConfiguration_DynamicSspec_UserVar_SharedStorage_DatalogBinary_F0
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseRead/FuseRead/InputFiles/scenario21.fuseRead.json";
	ConfigName = "Config21";
	RegisterName = "CPU0_2";
	SharedStorageKeyToStore = "AnyKeyCPU";
	ExecutionMode = "ExecutePattern";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	FuseGroupToDatalog = "GROUP_A,GROUP_B";
}
```

## Exit Ports
FuseReadSspec test method supports the following exit ports:
| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

## Additional Dependencies
N/A

## Version Tracking
| Prime Version |Prime ticket reference| **Comments** |
| ------------- | -------------------- | ------------ |
| 13.00.00      | #43528               | New test instance parameter 'SharedStorageKeyToStore' for storing fusestring to user provided sharedstorage key.|
| 13.00.00      | #40512               | Enhance to support UsePreviousData execution mode.|
| 12.00.00      | #36224               | 'Reverse' attribute in configuration file should only apply to Pattern Mode and not apply to Simulation Mode on the Simulation String.|
| 12.00.00      | #35539               | Enhance to support VBUMP Software Trigger like evergreen for FuseReadSspecTM.|
| 12.00.00      | -                    | Removed Engineering printing to console. |
| 11.01.00      | #32055               | Parse configuration file at testmethod instead of AlephInit. |
| 11.01.00      | #33560               | Removal of SetSimulationString interface. |
| 11.01.00      | #33523               | New logic for callback/preinstance of PrimeSetSimulationString. |
| 11.00.00      | #29647               | engineering version |

## Acronyms
Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **DFF**: Data Feed Forward