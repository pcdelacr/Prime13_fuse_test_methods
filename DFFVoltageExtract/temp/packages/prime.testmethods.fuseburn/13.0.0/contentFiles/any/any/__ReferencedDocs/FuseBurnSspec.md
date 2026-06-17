<h1>Prime Test-Method Specification REP</h1>

<DIV style="padding: 15px 35px 15px 15px;margin-bottom: 20px;border: 1px solid rgb(235, 204, 209);color: rgb(169, 68, 66);background-color: rgb(242,222,222);font-family: Verdana, sans-serif;font-size: 15px;font-style: normal;font-weight: 400;letter-spacing: normal;text-align: start;text-indent: 0px;text-transform: none;white-space: normal;word-spacing: 0px">
  <STRONG style="font-weight: bolder">/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////</STRONG><SPAN> </SPAN><br>
  <font color="red">**This is engineering version - Development of this test method is WIP - It must not be used in production.**</font><br>
  <STRONG style="font-weight: bolder">/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////</STRONG><SPAN> </SPAN><br>
</DIV>

May 2024

[[_TOC_]]

## Methodology
The FuseBurn SSPEC TestMethod will get Fuse String that stored in sharedstorage by FuseRead TestMethod.

The Fuse String will then compare with SSPEC Value which obtained from Sspec file (ExecuteSspec). see [Test Method Execute](###test-method-execute) for detail explanation.

The Fuse String after ExecuteSspec will go through modification with parameter set in BurnPatternConfig. see [Test Method Execute](###test-method-execute) for detail explanation.

The formatted fuse string will pass to PatConfig then proceed to execute Functional Test.

## FuseBurnMask Structure JSON File Example
[Download](./.attachments/FuseBurnSspecSample.json)
```json
{
    "Configurations": [																				
        {
            "Name": "ConfigValidFormat",																		
			"DatalogFormat": "RLE",																	
			"Registers": [ 
			{
				"Name": "CPU0", 
				"DataLabel": "Burn_Data_Label", 
				"DataLabelOffset": 0, 
				"FuseGroupsToHide": "FUSE_GROUP1,FUSE_GROUP2",								
				"SSPEC": 
				{
					"Enable": true, 
					"BurnRange": "0,1-3,8-14,17-18",
				}
			},
			{
				"Name": "PCH0", 
				"DataLabel": "FUSE_BURN_DATA_LABEL", 
				"DataLabelOffset": 0, 
				"FuseGroupsToHide": "PCH_FUSE1",
				"SSPEC": 
				{
					"Enable": true,
					"BurnRange": "0,1-3,8-14,17-18",
					"Dynamic": [ 
					{
						"FuseGroup": "*", 
						"uservar": 
						{
							"Collection": "uservar", 
							"Name": "_UserVars.FuseBurnFuseGroupUservar" 
						}
					},
					{
						"FuseGroup": "*", 
						"sharedstorage": 
						{
							"Name": "VAR1", 
							"Scope": "DUT" 
						}
					},
					{
						"FuseGroup": "GROUP_C",
						"DFF": 
						{
							"Name": "VOLT",
							"Optype": "PBIC_S2",
							"DieID": "DieID"
						}
					}
					]
				}
			}
			],
			"BurnPatternConfig":																	
			{
				"ReverseString": "false",														
				"Ratio": 1,																	
				"DataBlockSize": 3, 														
				"GapSize": 2, 																					
				"PatternsRegex": "fuse_burn_pat_BSM1_pass"									
			}
        }
	]
}
```
Download the parser, unzip, open the console window and run the script,
[<span style="color:green;font-size:2em">**Click to Download SspecParser**</span>](./.attachments/SspecParser.zip)

Usage: SspecParser.exe sspec.txt moduleName.

Output filename will be : <moduleName>.sspec.json.
```shell
MIT ATE Test Methods
SspecParser v1.0

Usage: SspecParser.exe sspec.txt moduleName.

Optional arguments:
        [-h],[--help],[?/] = Displays the parser help.

Positional arguments:
        [sspec.txt]: Sspec definition file.

        Example:

Visibility:Core0:                 mm0000110111mmmmmm100011ssssss000010mm0
FUSEDATA:  Core0:       QHBO    : n : mm0000110111mmmmmm100011ssssss000010mm0
FUSEDATA:  Core0:       QHBQ    : n : mm0000110111mmmmmm100011ssssss000010mm0
FUSEDATA:  Core0:       QHXX    : n : mm0000110111mmmmmm100011ssssss000010mm0
Visibility:Core1:                 mm0000110111mmmmmm100011ssssss000010mm0
FUSEDATA:  Core1:       QHQA    : n : mm0000110111mmmmmm100011ssssss000010mm0
FUSEDATA:  Core1:       QHQB    : n : mm0000110111mmmmmm100011ssssss000010mm0
FUSEDATA:  Core1:       QHQC    : n : mm0000110111mmmmmm100011ssssss000010mm0

```


## Test Instance Parameters
The table below lists and describes the test instance parameters supported by the FuseBurnMask test method.

| **Parameter Name**         | **Required?** | **Type**        | **Values**                                         | **Comments**                                                         |
| -------------------------- | ------------- | --------------- | -------------------------------------------------- | -------------------------------------------------------------------- |
|ConfigurationFile|Yes|File|Path to configuration file.|Able to use with "~HDMT_TPL_DIR".|
|ConfigName|Yes|String|Name of configuration in configuration file.|Can only define single name.|
|RegisterNames|Yes|CommaSeparatedString|Name of register in configuration file.|Can define single or multiple name with comma delimiter.|
|SharedStorageKeyToRead|No|CommaSeparatedString|SharedStorage Key name.| Get fusestring from user provided sharedstorage key. The key must match number of 'RegisterNames'. Example, 'RegisterNames'="CPU0,PCH0", 'SharedStorageKeyToRead'="AnyKeyA,AnyKeyB". CPU0 will use fusestring of AnyKeyA, PCH0 will use fusestring of AnyKeyB.|
|Patlist|No|Plist|PatternList to execute.|Default will be empty.|
|PrePatlist|No|String|Pre PatternList to execute.|Default will be empty.|
|TimingsTc|No|TimingCondition|TimingsTc for pattern execution.|Default will be empty.|
|LevelsTc|No|LevelsCondition|LevelsTc for pattern execution.|Default will be empty.|
|MaskPins|No|CommaSeparatedString|Pin to mask.|Default will be empty.|
|MaxNumberOfFails|No|UnsignedInteger|Maximum number of failture to capture.|Default will set to 1.|
|SkipPatternExecute|No|Boolean|set ENABLED to setup functional test.|By default it is set to DISABLED.|

## Test Method Verify
This section explain the verfiy process of FuseBurnMask TestMethod.

1. Validate the 'Required' test instance parameter. If test instance parameter *MaskPins*, it will check if the Pin exist.
2. Get configuration information by parsing the file that user provided with test instance parameter *ConfigurationFile*.
3. Validate configuration information with FuseBurn Schema.
4. Validate configuration information with FuseBurn Validator.
5. Setup Handler that consist of Register Data and Patconfig Handler.
    - Setup Register Handler for name of register user provided with test instance parameter *RegisterNames*. 
    - Setup PatConfig Handler with information from test instance parameter *BurnPatternSetName*, *Patlist* and from configuration information *BurnPatternConfig PatternsRegex*.
6. Setup Function Test if test instance parameter *SkipPatternExecute* is set to ENABLED.<br>
![image.png](./.attachments/FuseBurnTestMethod-Verify-Flow.png)

ote: Starting from Prime v12.0.0 onwards, fuseDataLabel is an optional for every register defined in fuseDef file. When running Verify, fuseBurnMask will check if skipPatternExecute is disabled and fail appropriately. 
If it is disabled, fuseBurnMask is expecting fuseDataLabel to be defined, otherwise error will be thrown.

## Test Method Execute
This section explain the execute process of FuseBurnSSPEC TestMethod. **This process is repeated for every register handler.

1. Get FuseString from sharedstorage that stored by FuseRead TestMethod.
    - The data is store with Key "fusestring_<registerName>" at DUT level.
    - You must either run FuseRead TestMethod first or have user code that store the fusestring with *Key* and *Context* mentioned above.
2. Run *CustomExecute* which ExecuteSspec. This step can be customize with user code, see [Custom User Code Hooks](###custom-user-code-hooks).
    - Check if Sspec is enabled and get Sspec configuration from configuration file.
    - Process fuse string if the Mask is with *FuseGroupsToHide* enabled. The bit will change to '0' depend on the Fuse Group address (the address is from fusedef).
    - Get Fuse Address related to the Mask *FuseGroup*.
    - Replace dynamic value ('m') in Mask value with respective storage value (can be from DFF, sharedstorage, uservar) that defined in Mask's *Dynamic*.
    - (If *BurnRange* is enabled) the burn range mask (MSB->LSB) will populate based on register size with the bit that *BurnRange* address will set as '1' while other set as '0'. 
    - (If *BurnRange* is enabled) the burn range mask will be post process to extract the string based on *FuseGroup* address, note that "FuseGroup" only supported "*".
    - (If *BurnRange* is enabled) the burn range mask will then perform AND-Bitwise with the mask value. see [AND-Bitwise Logic](###logic).
    - Update FuseBurnData with mask value. Bit that contain 's' will be remained as skip bit. 
    - Print FuseBurnData string that apply with the mask *FuseGroupsToHide*. The bit will change to '0' depend on the Fuse Group address (the address is from fusedef).
3. Get FinalFuseStringToBurn that applied with *BurnPatternConfig* information in configuration file. (ReverseString, Ratio, DataBlockSize, GapSize)
    - Reverse the fuse string if *BurnPatternConfig ReverseString* set to true.
    - Expand fuse string by ratio. The ratio is applied to per bit. Example, Ratio=2 FuseString='1010' -> FuseString='11001100'.
    - Post process the fuse string by Data Block Size and Gap Size (represented by 's'). Example, Fuse String '11001100' with Data Block Size=2 and Gap Size=1 will get '<font color="blue">11</font>**s**<font color="blue">00</font>**s**<font color="blue">11</font>**s**<font color="blue">00</font>'.
    - Update skip character from 's' to '_'.
4. Set the FinalFuseStringToBurn to PatConfig and Apply Pat Modify.
5. Datalog FuseString to ituff with strgVal and name will include name of register .
6. Execute Functional Test if *SkipPatternExecute* is set to ENABLED.
7. Return Execute Result.<br>
![image.png](./.attachments/FuseBurnTestMethod-Execute-Flow.png)

## Logic
AND-Bitwise Logic
| Mask Value | Burn Range Mask Value | Result |
| ---------- | --------------------- | ------ |
|0|0|0|
|0|1|0|
|1|0|0|
|1|1|1|
|s|1|s (skip)|
** Mask value will contain '0','1' and 's'. Burn Range Mask value will contain '0' and '1' only.

## Custom User Code Hooks
void CustomExecute(FuseBurnMaskTestInstanceResults fuseBurnMaskTestInstanceResults)

## TPL Samples
Here are a few test instance examples using the test method:
```c++
## Static Sspec, FullString, Enable BurnRange, with SkipBits_with FuseGroupToHide, Datalog as binary, simulation disabled.
Test PrimeFuseBurnSSPECTestMethod StaticSspec_FullString_EnableBurnRange_SkipBits_FuseGroupToHide_DatalogBinary_SkipPatternExecuteDisabled_P1
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseBurn/FuseBurn/InputFiles/scenario7_sspec.fuseBurn.json";
	ConfigName = "Config7BurnRange"; #USER TODO: define value
	RegisterNames = "CPU0"; #USER TODO: define value
	Patlist = "ctv_plist";
	LevelsTc = "FUSEBURN::basic_func_lvl_nom";
	TimingsTc = "FUSEBURN::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	SkipPatternExecute = "DISABLED";
}

## Static Sspec, FullString, Datalog as binary, reverse pattern execute.
Test PrimeFuseBurnSSPECTestMethod StaticSspec_Fullstring_DatalogBinary_SkipPatternExecuteDisabled_Reverse_P1
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseBurn/FuseBurn/InputFiles/scenario4_sspec.fuseBurn.json";
	ConfigName = "Config4MultiRegister"; #USER TODO: define value
	RegisterNames = "CPU0"; #USER TODO: define value
	Patlist = "ctv_plist";
	LevelsTc = "FUSEBURN::basic_func_lvl_nom";
	TimingsTc = "FUSEBURN::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	SkipPatternExecute = "DISABLED";
}

## Dynamic Sspec, FullString, MultiRegisters, Datalog as binary, simulation disabled.
Test PrimeFuseBurnSSPECTestMethod DynamicSspec_Fullstring_MultiRegisters_DatalogBinary_SkipPatternExecuteDisabled_P1
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseBurn/FuseBurn/InputFiles/scenario5_sspec.fuseBurn.json";
	ConfigName = "Config5DynamicSspec"; #USER TODO: define value
	RegisterNames = "CPU0,PCH0"; #USER TODO: define value
	Patlist = "ctv_plist";
	LevelsTc = "FUSEBURN::basic_func_lvl_nom";
	TimingsTc = "FUSEBURN::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	SkipPatternExecute = "DISABLED";
}
 
## Dynamic Sspec, FullString, SingleRegister, Datalog as binary, simulation disabled. Get fusestring from custom storage.
Test PrimeFuseBurnSSPECTestMethod DynamicSspec_Fullstring_MultiRegisters_DatalogBinary_SkipPatternExecuteDisabled_P1
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseBurn/FuseBurn/InputFiles/scenario5_sspec.fuseBurn.json";
	ConfigName = "Config5DynamicSspec"; #USER TODO: define value
	RegisterNames = "CPU0,PCH0"; #USER TODO: define value
	SharedStorageKeyToRead = "AnyKeyCPU";
	Patlist = "ctv_plist";
	LevelsTc = "FUSEBURN::basic_func_lvl_nom";
	TimingsTc = "FUSEBURN::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	SkipPatternExecute = "DISABLED";
}

## Dynamic Sspec, FullString, MultiRegisters, Datalog as binary, simulation disabled. Get fusestring from custom storage.
Test PrimeFuseBurnSSPECTestMethod DynamicSspec_Fullstring_MultiRegisters_DatalogBinary_SkipPatternExecuteDisabled_P1
{
	ConfigurationFile = "~HDMT_TPL_DIR/Modules/FuseBurn/FuseBurn/InputFiles/scenario5_sspec.fuseBurn.json";
	ConfigName = "Config5DynamicSspec"; #USER TODO: define value
	RegisterNames = "CPU0,PCH0"; #USER TODO: define value
	SharedStorageKeyToRead = "AnyKeyCPU, AnyKeyPCH";
	Patlist = "ctv_plist";
	LevelsTc = "FUSEBURN::basic_func_lvl_nom";
	TimingsTc = "FUSEBURN::basic_func_timing_10MHz_20MHz";
	LogLevel = "PRIME_DEBUG";
	SkipPatternExecute = "DISABLED";
}
```
**More can be find at SampleTp fuseburn module.

## Exit Ports
Test method supports the following exit ports:
| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

## Additional Dependencies
N/A

## Debug Error Prompt by Json Schema
Before proceed to validate your input json file (configuration) against fuse burn sspec schema, you should have your json file validated with <u>***any***</u> online json file validator so that it is syntax and format error free.

The General error message prompt at the console by Json Schema validator.
```c++
Prime.Base.Exceptions.TestMethodException: Errors in configuration files were found: [File [<Test Instance Parameter - ConfigurationFile>] has the following errors:
```

Look for the most inward error message
```c++
{
    ArrayItemNotValid: #/Configurations[0].Registers[0]
    {
        ArrayItemNotValid: #/Configurations[0].Registers[0].Masks[1]
        {
            ArrayItemNotValid: #/Configurations[0].Registers[0].Masks[1].Dynamic[0]
            {
                ...
            }
        }
    }
}
```
For case above, the actual error is from 3rd message.
```json
            ArrayItemNotValid: #/Configurations[0].Registers[0].Masks[1].Dynamic[0]
            {
                <The error message>
            }
```

### Attribute Not within valid option list
Input Example
```c++
    ...
    {
        "DatalogFormat": "HELLOWORLD",
    }	
    ...
```
Error Message: <font color="red">NotInEnumeration:</font> <font color="blue">#/Configurations[0].DatalogFormat</font>

Explanation: The *first configuration* (<font color="blue">#/Configurations[0]</font>, array start with 0) having error at its attribute <font color="blue">DatalogFormat</font> because "HELLOWORLD" is neither of valid <font color="blue">DatalogFormat</font> option "RLE", "BINARY", "HEX" or "DEFLATE".

###  Attribute is set to invalid data Type
```c++
    ...
    {
        "Name": "MEROM2",
        "DataLabel": "Burn_Data_Label",
        "DataLabelOffset" : "abc",
    }
    ...
```
Error Message: <font color="red">IntegerExpected:</font> <font color="blue">#/Configurations[0].Registers[1].DataLabelOffset</font>

Explanation: The *first configuration* (<font color="blue">#/Configurations[0]</font>, array start with 0) of its *second register* (<font color="blue">#/Configurations[0].Registers[1]</font>) having error at its attribute <font color="blue">DataLabelOffset</font> because "abc" is a string type and is not interger type.

## Version Tracking
| Prime Version |Prime ticket reference| **Comments** |
| ------------- | -------------------- | ------------ |
| 13.00.00      | #50536               | Standardize the naming and configuration of FuseGroupToHide. |
| 13.00.00      | #43528               | New test instance parameter 'SharedStorageKeyToRead' for getting fusestring from user provided sharedstorage key.|
| 12.0          | #28786               | engineering version |

## Acronyms
Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **DFF**: Data Feed Forward