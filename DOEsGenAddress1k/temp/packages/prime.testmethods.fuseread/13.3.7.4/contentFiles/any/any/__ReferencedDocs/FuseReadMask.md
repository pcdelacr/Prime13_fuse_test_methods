<h1>Prime Test-Method Specification REP</h1>

July 2025

[[_TOC_]]

## Important Notice
<font color="red">**From Prime v12.00.00 onward**</font><br>
- 'Reverse' attribute in 'ConfigurationFile' is only applicable to Pattern Mode (ExecutionMode set to ExecutePattern). Simulation String is always expected to be MSB->LSB (LHS->RHS). <br>
- when 'Reverse' attribute is set to true and Execute Pattern Mode, this will Reverse the string of 'datalog fuse string' and 'fuse string composite' that will be compare with mask. <br>

<font color="red">**From Prime v11.01.00 onward**</font><br>
- Fuseread testmethod no longer parse *.fuseread.json (configuration) file with AlephInit. The parsing will be at Verify(), please define the with test instance parameter 'ConfigurationFile'.<br>

## Methodology
The FuseReadkMask test method is use to compare fuse string (simulation string or ctv data) with mask value (static value or dynamic value) then save the mismatch result (if any). 

Fuse string can be datalog in format of "RLE", "HEX", "DEFLATE", "BINARY". <font color="red">**When RLE format datalog failed, testmethod will do binary format datalog and exit port 0.</font>

By Default, Fuse String is stored to sharedstorage with the key "fusestring_<registerName>". Unless user defined test instance parameter "SharedStorageKeyToStore". See "Test Instance Parameters" section for description.

Mask value can be a static value or dynamic value that get from storage (UserVar, DFF and Sharedstorage).

Mask value with 's' is to skip mismatch report for the bit, while 'm' is to replace with value from storage.

## Json File Setup
### [Example] Configuration JSON File.
[See Example. (if the link not working, proceed by manually navigate to FuseRead Readme.md)](./../../Readme.md) <br>

### [Example] Voltage JSON File to Enable VBump Software Trigger.
[See Example. (if the link not working, proceed by manually navigate to FuseRead Readme.md)](./../../Readme.md) <br>

## Test Instance Parameters
The table below lists and describes the test instance parameters supported by the Fuse Read Mask test method.

| **Parameter Name**             | **Required?** | **Type**        | **Values**                                         | **Comments**                                                         |
| ------------------------------ | ------------- | --------------- | -------------------------------------------------- | -------------------------------------------------------------------- |
| ConfigurationFile              | Yes           | File            | Path to configuration file.                        | Able to use with "~HDMT_TPL_DIR".|
| ConfigName                     | Yes           | String          | FuseRead configuration name                        | Configuration name for execute mask. Can have one name only. |
| VoltageFile                    | No            | File            |Path to voltage file.                               | Able to use with "~HDMT_TPL_DIR". Provide this file to enable functional test VBump Software Trigger.|
| RegisterName                   | Yes           | String          | FuseRead configuration register name               | Register name for execute mask. Can have multiple register name separated by comma. |
| SharedStorageKeyToStore        | No            | CommaSeparatedString | SharedStorage Key name.                       | Allow user to store fusestring to provided sharedstorage key. The key must match number of 'RegisterName'. Example, 'RegisterName'="CPU0,PCH0", 'SharedStorageKeyToStore'="AnyKeyA,AnyKeyB". fusestring of CPU0 will store to AnyKeyA, PCH0 will store to AnyKeyB.|
| FailingMaskName                | No            | CommaSeparatedString | FuseRead configuration register mask name     | Valid FailingMask name of the register or empty string. Empty string will bypass the functionality.  |
| PassingMaskName                | No            | CommaSeparatedString | FuseRead configuration register mask name     | Valid PassingMask name of the register or empty string. Empty string will bypass the functionality.  |
| Patlist                        | No            | Plist           | Plist name to be executed.                         | Default is empty. |
| LevelsTc                       | No            | LevelsCondition | Levels test condition required for plist execution | Default is empty. |
| TimingsTc                      | No            | TimingCondition | Timing test condition required for plist execution | Default is empty. |
| PrePatlist                     | No            | String          | Preplist element to be executed                    | Default is empty. |
| ExecutionMode                  | No            | String (choice) | ExecutePattern (Default), UsePreviousData, SimulationMode  | By default is in ExecutePattern mode. See below table for more details. |
| EnableCaptureFunctionalFailure | No            | String (choice) | False, True (Default)                              | False indicate CaptureCtvPerPinTest, true indicate CaptureFailureAndCtvPerPinTest. |
| DatalogMode                    | No            | String (choice) | ENABLED(Default), DISABLED                         | To enable or disable the datalogging in ituff. |
| MaskPins                       | No            | String          | Comma separated list of pins for which the fail data capture will be skipped | Default is empty string                    |
| FuseGroupToDatalog             | No            | CommaSeparatedString | Valid Register FuseGroupName | Default is empty. Use to datalog the fuse string of the FuseGroupName.|

Table for ExecutionMode:

| **ExecutionMode**             | **Pattern Executed?** | **Fuse String Source**                                          |
| ------------------------------| --------------------- | --------------------------------------------------------------- | 
| ExecutePattern(Default)       | Yes                   | Ctv from pattern execution.                                     |
| UsePreviousData               | No                    | Fuse string from previous fuseread instance.                    |
| SimulationMode                | No                    | Simulation string defined in input file or set through callback.|

### Passing mask & Failing mask logic :

FuseReadMask provide parameter failingMaskName and passingMaskName when executing masks.

When mask are specify in passingMaskName, if fuseString match with maskString, test method will continue to exit port 1, or else exit fail port if mismatch.

FailingMask act inversely to passingMask, given when FailingMaskName specified, if fuseString contains mismatch with maskString, this will lead to exit port 1, while a match will exit fail port instead.

### **Special Case

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
Running Verify()
=========================
[DUT: 1]================================================================
[DUT: 1]Verify DLL Load information:
[DUT: 1]dllPathToLoad       [C:\Intel\hdmt\hdmtOS_3.9.2.0_Release\ActiveProfile\bin\Prime\Prime.TestMethods.FuseRead.dll].
[DUT: 1]testMethodNameSpace [Prime.TestMethods.FuseRead].
[DUT: 1]testMethodName      [PrimeFuseReadMaskTestMethod].
[DUT: 1]instanceName        [FUSEREAD::ExecutePattern_FuseStringx1_StaticMask_Mismatch02_MismatchToSharedStorage_DatalogBinary_F0].
[DUT: 1]================================================================
[DUT: 1]Below are the list of parameters and its value for this Instance:
[DUT: 1]BypassPort: -1
[DUT: 1]ConfigName: Config15
[DUT: 1]EnableCaptureFunctionalFailure: False
[DUT: 1]LevelsTc: FUSEREAD::basic_func_lvl_nom
[DUT: 1]LogLevel: PRIME_DEBUG
[DUT: 1]MaskName: Mask2
[DUT: 1]MaskPins:
[DUT: 1]MemoryAndTimeProfiling: DISABLED
[DUT: 1]Patlist: ctv_plist
[DUT: 1]PrePatlist:
[DUT: 1]RegisterName: CPU0
[DUT: 1]ExecutionMode: ExecutePattern
[DUT: 1]TimingsTc: FUSEREAD::basic_func_timing_10MHz_20MHz
[DUT: 1]
[DUT: 1]================================================================
[DUT: 1]Constructing handler for ConfigName=[Config15] RegisterName=[CPU0] ExecutionMode=[ExecutePattern].
[DUT: 1]Complete setup fuse read handler=[1].
[DUT: 1]
=========================
Running Execute()
=========================
[DUT: 1]Execute Mask in ExecutionMode=[ExecutePattern].
[DUT: 1]Captured CtvData fuse value (LSB->MSB)=[1111111000000111100].
[DUT: 1]FuseString (MSB->LSB)=[0011110000001111111]
[DUT: 1]Reverse Enabled for Datalog. ReverseFuseString=[1111111000000111100].
[DUT: 1]Printed to ituff:
[DUT: 1]2_tname_FUSEREAD::ExecutePattern_FuseStringx1_StaticMask_Mismatch02_MismatchToSharedStorage_DatalogBinary_F0_CPU0_fd1
[DUT: 1]2_strgalt_fus_msbF_1111111000000111100
[DUT: 1]
[DUT: 1]Fuse Composite String for Mask=[Mask2] FuseGroup=[GROUP_A] String=[111111]
[DUT: 1]Reverse Enabled for Execute Mask. FuseString=[111111] ReverseFuseString=[111111].
[DUT: 1]Execute Mask=[Mask2] FuseGroup=[GROUP_A] with FuseString[MSB->LSB]=[111111] MaskValue[MSB->LSB]=[111100].
[DUT: 1]STRING INDEX                   : 543210
[DUT: 1]FUSE COMPOSITE VALUE           : 111111
[DUT: 1]MASK VALUE                     : 111100
[DUT: 1]COMPARE VALUE                  :     ^^
[DUT: 1]MISMATCH BITS POSITION         : 1,0
[DUT: 1]Stored Mismatch Bits=[0,1] to StorageType=[sharedstorage] StorageName=[FailingBitsForMask2]
```

## Datalog output
```
2_tname_FUSEREAD::ExecutePattern_FuseStringx1_StaticMask_Mismatch02_MismatchToSharedStorage_DatalogBinary_F0_CPU0_fd1
2_strgalt_fus_msbF_1111111000000111100
```

## Custom User Code Hooks
Example - SampleTP - Test CustomExecuteMask ExecutePattern_SingleRegister_StaticMask_DatalogBinary_CustomExecuteMask_P1

void CustomExecute(FuseReadMaskTestInstanceResults fuseReadTestInstanceResults);
```c++
        /// <summary>
        /// Same execute mask but with additional message printed and execute multiple mask and customize the exit port.
        /// </summary>
        /// <param name="fuseReadTestInstanceResults">Fuse read handler that contain test register information.</param>
        void IFuseReadMaskUltDecodeExtensions.CustomExecute(FuseReadMaskUltDecodeTestInstanceResults fuseReadTestInstanceResults)
        {
            if (!string.IsNullOrEmpty(this.FailingMaskName))
            {
                ((IFuseReadMaskExtensions)this).ExecuteFailingMask(fuseReadTestInstanceResults);
                if (fuseReadTestInstanceResults.ExitPort != 1)
                {
                    return;
                }
            }

            if (!string.IsNullOrEmpty(this.PassingMaskName))
            {
                ((IFuseReadMaskExtensions)this).ExecutePassingMask(fuseReadTestInstanceResults);
            }
        }
```
void CustomDatalogFuseStringByFuseGroupName(IFuseReadHandle fuseReadHandle);
```c++
        /// <summary>
        /// Same DatalogFuseStringByFuseGroupName but with additional message printed and customize the ituff instance name.
        /// </summary>
        /// <param name="fuseReadHandle">Fuse read handler that contain test register information.</param>
        void IFuseReadMaskExtensions.CustomDatalogFuseStringByFuseGroupName(IFuseReadHandle fuseReadHandle)
        {
           if (!string.IsNullOrEmpty(this.FuseGroupToDatalog))
            {
                foreach (var fuseGroupName in this.FuseGroupToDatalog.ToList())
                {
                    var fuseStringOfFuseGroup = fuseReadHandle.GetFuseStringByFuseGroup(fuseGroupName);

                    FuseDataManager.SharedStorage.StoreFuseGroupToDatalogToSharedStorage(fuseGroupName, string.Empty, fuseStringOfFuseGroup, this.SessionContext);

                    if (this.DatalogMode == EnabledDisabledModes.ENABLED)
                    {
                        FuseDataManager.Datalog.DatalogFuseGroupFuseStringToItuff(fuseReadHandle.GetRegisterName(), fuseReadHandle.GetFuseStringForItuffByFuseGroup(fuseGroupName), fuseGroupName, this.SessionContext);
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
            var passingMaskNames = this.PassingMaskName.ToList();

            fuseReadTestInstanceResults.ExitPort = 0;
            foreach (var maskName in passingMaskNames)
            {
                var noMismatchFound = fuseReadTestInstanceResults.ExecuteFuseReadHandle.ExecuteMask(maskName);
                if (!noMismatchFound)
                {
                    if (this.DatalogMode == EnabledDisabledModes.ENABLED)
                    {
                        FuseDataManager.Datalog.DatalogFailingMaskToItuff(maskName, this.SessionContext);
                    }

                    fuseReadTestInstanceResults.ExitPort = fuseReadTestInstanceResults.ExecuteFuseReadHandle.GetFuseMaskFailPort(maskName);
                    Base.ServiceStore<IConsoleService>.Service.PrintError($"PassingMaskName=[{maskName}] failed to execute as it contains mismatch. Exiting fail port={fuseReadTestInstanceResults.ExitPort}", this.SessionContext);
                    break;
                }
                else
                {
                    fuseReadTestInstanceResults.ExitPort = 1;
                }
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
            var failingMaskNames = this.FailingMaskName.ToList();

            fuseReadTestInstanceResults.ExitPort = 0;
            foreach (var maskName in failingMaskNames)
            {
                var noMismatchFound = fuseReadTestInstanceResults.ExecuteFuseReadHandle.ExecuteMask(maskName);
                if (noMismatchFound)
                {
                    if (this.DatalogMode == EnabledDisabledModes.ENABLED)
                    {
                        FuseDataManager.Datalog.DatalogFailingMaskToItuff(maskName, this.SessionContext);
                    }

                    fuseReadTestInstanceResults.ExitPort = fuseReadTestInstanceResults.ExecuteFuseReadHandle.GetFuseMaskFailPort(maskName);
                    Base.ServiceStore<IConsoleService>.Service.PrintError($"FailingMaskName=[{maskName}] failed to execute as it does not contain mismatch. Exiting fail port={fuseReadTestInstanceResults.ExitPort}", this.SessionContext);
                    break;
                }
                else
                {
                    fuseReadTestInstanceResults.ExitPort = 1;
                }
            }
        }
```

## TPL Samples
Here are a few test instance in SampleTP examples using the Fuse Read Mask test method:
*Refer to inputfiles for the configuration

Execute without mask, Single Register, Simulation Mode, Reverse String
```c++
Test PrimeFuseReadMaskTestMethod SimulationMode_SingleRegister_Reverse_DatalogRLE_P1
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseRead/FuseRead/InputFiles/scenario2.fuseRead.json";
	ConfigName = "Config2";
	PassingMaskName = "";
	RegisterName = "CPU0_2";
	LogLevel = "PRIME_DEBUG";
}

```

Execute without mask, Single Register, Execute Pattern.
```c++
Test PrimeFuseReadMaskTestMethod ExecutePattern_SingleRegister_FuncTestNoCaptureFailure_DatalogHex_P1
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseRead/FuseRead/InputFiles/scenario3.fuseRead.json";
	ConfigName = "Config3";
	PassingMaskName = "";
	RegisterName = "CPU0_2";
	ExecutionMode = "ExecutePattern"; 
	EnableCaptureFunctionalFailure = "False";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
}
```

Execute passing mask, Single Register, Execute Pattern. Enabled functional test software trigger for vbump.
```c++
Test PrimeFuseReadMaskTestMethod ExecutePattern_SingleRegister_StaticMask_DatalogBinary_P1
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseRead/FuseRead/InputFiles/scenario7.fuseRead.json";
   VoltageFile = "~HDMT_TPL_DIR/Modules/FuseRead/FuseRead/InputFiles/fuseReadVoltageFile.json";
	ConfigName = "Config7";
	PassingMaskName = "Mask1";
	RegisterName = "CPU0_2";
	ExecutionMode = "ExecutePattern";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	FuseGroupToDatalog = "GROUP_A,GROUP_B";
}
```

Execute passing mask, Multiple Register, Execute Pattern, Mask Off FuseGroup
```c++
Test PrimeFuseReadMaskTestMethod ExecutePattern_MultipleRegister_FuseMaskOff_StaticMask_DatalogBinary_F0
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseRead/FuseRead/InputFiles/scenario17.fuseRead.json";
	ConfigName = "Config17";
	PassingMaskName = "Mask1";
	RegisterName = "CPU0_2,PCH0";
	ExecutionMode = "ExecutePattern";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
}
```

Execute Passing Mask, Multiple register, with preInstance set simulation string for both register.
```c++
Test PrimeFuseReadMaskTestMethod ExecuteMask_MultipleRegister_PreInstanceSetSimulationString_SimulationEnable_F0
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseRead/FuseRead/InputFiles/scenario17.fuseRead.json";
	ConfigName = "Config17";
	PassingMaskName = "Mask1";
	RegisterName = "CPU0_2,PCH0";
	ExecutionMode = "SimulationMode";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	PreInstance = "PrimeSetSimulationString(CPU0_2@1111111111111111111,PCH0@0000001111111111111)";
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

Execute Passing Mask, Single register, with preInstance set simulation string for register. Store fusestring to custom sharedstorage.
```c++
Test PrimeFuseReadMaskTestMethod ExecuteMask_MultipleRegister_PreInstanceSetSimulationString_SimulationEnable_F0
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseRead/FuseRead/InputFiles/scenario17.fuseRead.json";
	ConfigName = "Config17";
	PassingMaskName = "Mask1";
	RegisterName = "CPU0_2";
	SharedStorageKeyToStore = "AnyKeyCPU";
	ExecutionMode = "SimulationMode";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	PreInstance = "PrimeSetSimulationString(CPU0_2@1111111111111111111)";
}
```

Execute Passing Mask, Multiple register, with preInstance set simulation string for both register. Store fusestring to custom sharedstorage.
```c++
Test PrimeFuseReadMaskTestMethod ExecuteMask_MultipleRegister_PreInstanceSetSimulationString_SimulationEnable_F0
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseRead/FuseRead/InputFiles/scenario17.fuseRead.json";
	ConfigName = "Config17";
	PassingMaskName = "Mask1";
	RegisterName = "CPU0_2,PCH0";
	SharedStorageKeyToStore = "AnyKeyCPU", "AnyKeyPCH";
	ExecutionMode = "SimulationMode";
	Patlist = "ctv_plist";
	PrePlist = "";
	LevelsTc = "FUSEREAD::basic_func_lvl_nom";
	TimingsTc = "FUSEREAD::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	PreInstance = "PrimeSetSimulationString(CPU0_2@1111111111111111111,PCH0@0000001111111111111)";
}
```


## Exit Ports
FuseReadMask test method supports the following exit ports:
| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |
| **2-9**       | ***Fail***      | Failing condition            |
User can overwrite the passing/failing condition with the mtpl file using the Property PassFail.
If you want to use more than port 9 for the exit port, you can do so by using usercode or submit request to Prime team.

## Additional Dependencies
N/A

## Version Tracking
| Prime Version | Prime ticket reference | **Comments** |
| ------------- | ---------------------- | ------------ |
| 13.03.00      | #59279 <br> #60835     | Implement fuse string fail to convert RLE format error handling. When fail to datalog fusestring in RLE format, testmethod will datalog as binary format and exit port 0.|
| 13.00.00      | #43528                 | New test instance parameter 'SharedStorageKeyToStore' for storing fusestring to user provided sharedstorage key.|
| 13.00.00      | #40512                 | Enhance to support UsePreviousData execution mode.|
| 12.00.00      | #36224                 | 'Reverse' attribute in configuration file should only apply to Pattern Mode and not apply to Simulation Mode on the Simulation String.|
| 12.00.00      | -                      | Removed Engineering printing to console. |
| 12.00.00      | #27977                 | Enable functional test vbump software trigger. Parameter is setup with fuse read VoltageFile. |
| 11.01.00      | #32055                 | Parse configuration file at testmethod instead of AlephInit. |
| 11.01.00      | #33560                 | Removal of SetSimulationString interface. |
| 11.01.00      | #33523                 | New logic for callback/preinstance of PrimeSetSimulationString. |
| 11.00.00      | #29647                 | engineering version |

## Acronyms
Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **DFF**: Data Feed Forward