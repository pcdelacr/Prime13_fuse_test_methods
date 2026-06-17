This page describe API that shared across Fuse Read Test Method and Fuse Burn Test Method.

[[_TOC_]]

# Data Manager
## Datalog
### <font color="blue">DatalogToItuff(Dictionary<string, string> datalogInfo) : void</font>
This API receive input of Dictionary with "Key"-type-string as tname to be datalog and "Value"-type-string as data to be datalog.<br>
The datalog will be in strgAlt format with "Key" be used as TnamePostFix. The ituff data string will include 'fus_msbF' and "Value" as printed data.<br>
```
| Input                                               | Output                                  |
| --------------------------------------------------- | --------------------------------------- |
| <fd1,0000000000000000000>,<fd2,1111111111111111111> | 2_tname_<TestInstanceName>_fd1          |
|                                                     | 2_strgalt_fus_msbF_000000000000000000   |
|                                                     | 2_tname_<TestInstanceName>_fd2          |
|                                                     | 2_strgalt_fus_msbF_1111111111111111111  |
```
### <font color="blue">DatalogToItuff(string postFix, string dataToSet) : void</font>
This API receive input of string-"postFix" and string-"dataToSet".<br>
The datalog will be in strgVal format with "postFix" be used as TnamePostFix and "dataToSet" as printed data.<br>
```
| Input      | Output                          |
| -----------| ------------------------------- |
| ABC, 00111 | 2_tname_<TestInstanceName>_ABC  |
|            | 2_strgval_00111                 |
```

### <font color="blue">DatalogFuseGroupFuseStringToItuff(string registerName, string fuseString, string fuseGroupName) : void</font>
The datalog will be in strgVal format with "registerName" and "fuseGroupName" be used as TnamePostFix and "fuseString" as printed data.<br>
```
| Input               | Output                                  |
| ------------------- | --------------------------------------- |
| CPU0, 00111, GROUPC | 2_tname_<TestInstanceName>_CPU0_GROUPC  |
|                     | 2_strgval_00111                         |
```

## Dff
### <font color="blue">GetValueFromDFF(string dffCustomVarName) : string</font>
<font color="red">This is only meant to be use for Fuse Read with **Prime v10.00.00** and **older** only.</font><br>
The API will parse "dffCustomVarName" with comma delimiter. "dffCustomVarName" is expected in the format of 'DffTokenName,DieId,OperationType'. Example: VOLT,1234,PBIC_S2.<br>
The parsed information will then be use to GetDff value from Dff storage.

### <font color="blue">GetValueFromDFF(string tokenName, string operationType, string targetDieID) : string</font>
<font color="red">This is only meant to be use for Fuse Read with **Prime v11.00.00** and **newer** only.</font><br>
The API will use the input information to GetDff value from Dff storage.

## SharedStorage
### <font color="blue">StoreFuseStringToSharedStorage(string registerName, string fuseString) : void</font>
This API is default use by Fuse Read TestMethod to store the executed "fuseString" to SharedStorage at Context DUT with SharedStorage Key of 'fusestring_<"registerName">'. <br>
The stored value ("fuseString") will be used later by Fuse Burn TestMethod when executing same "registerName".

### <font color="blue">StoreFuseGroupToDatalogToSharedStorage(string fusegroupName, string dieIdName, string fuseString) : void</font>
The API will use the input information to store "fuseString" to SharedStorage at Context DUT. <br>
The SharedStorage default Key is 'fuseread_"fusegroupName"' OR when "dieIdName" is provided then the SharedStorage Key is 'fuseread_"dieIdName"_"fusegroupName"'.

### <font color="blue">GetValueFromSharedStorage(string storageName) : string</font>
<font color="red">This is only meant to be use for Fuse Read with **Prime v10.00.00** and **older** only.</font><br>
The API will parse "storageName" with comma delimiter. "storageName" is expected in the format of 'VariableName,ContextType'. Example: VOLT,DUT.<br>
The parsed information will then be use to get String value from SharedStorage.

### <font color="blue">GetStringFromSharedStorage(string storageName, ScopeType.Context scope) : string</font>
The API will use the input information to get String value from SharedStorage.

### <font color="blue">GetDoubleFromSharedStorage(string storageName, ScopeType.Context scope) : double</font>
The API will use the input information to get Double value from SharedStorage.

### <font color="blue">SetValueToSharedStorage(string sharedStorageName, string valueToStore, Context contextTypeToStore) : void</font>
The API will use the input information to store "valueToStore" to SharedStorage with Key as "sharedStorageName" and Context of "contextTypeToStore".

### <font color="blue">SetValueToSharedStorage(string sharedStorageName, string valueToStore, ScopeType.Context contextTypeToStore) : void</font>
The API will use the input information to store "valueToStore" to SharedStorage with Key as "sharedStorageName" and Context of "contextTypeToStore".

### <font color="blue">ValidateContextType(string contextCheck) : Context</font>
This API is used to check supported SharedStorage Context Type : DUT, IP and LOT only.

## UserVar
### <font color="blue">GetValueFromUserVar(string userVarName) : string</font>
The API will use the input information to GetStringValue from UserVar storage.

### <font color="blue">SetValueToUserVar(string userVarName, string valueToStore) : void</font>
The API will use the input information to SetValue to UserVar storage with "userVarName" as Key and "valueToStore" as Value.

# Enums
## DatalogType
### <font color="blue">FuseReadOption</font>
The supported datalog format for Fuse Read are : RLE, DEFLATE, BINARY and HEX only.

### <font color="blue">FuseBurnOption</font>
The supported datalog format for Fuse Burn are : RLE, DEFLATE, and BINARY only.

## FuseGroupToHideEnabled
### <font color="blue">Option</font>
```
| Option      | Description                     |
| ------------| ------------------------------- |
| True        | Apply FuseGroupToHide.          |
| False       | Do Not Apply FuseGroupToHide.   |
```

## FuseSchemaType
### <font color="blue">Option</font>
```
| Option           | Description                                       |
| -----------------| ------------------------------------------------- |
| FuseRead         | Use json schema for fuse read configuration file. |
| FuseReadVoltage  | Use json schema for fuse read voltage file.       |
| FuseBurn         | Use json schema for fuse burn configuration file. |
| FuseBurnVoltage  | Use json schema for fuse burn voltage file.       |
```

## ScopeType
### <font color="blue">Context</font>
The SharedStorage Context type : DUT, IP and LOT only.

# Fuse Basic
## FuseBasic
### <font color="blue">GetFuseSkipCharacter() : char</font>
This API return the standard Fuse skip character which is 's'.

### <font color="blue">GetFuseDynamicCharacter() : char</font>
This API return the standard Fuse dynamic character which is 'm'.

### <font color="blue">UpdateSkipCharacter(string inputString) : string</font>
This API will replace old skip character '9' to the Fuse skip character 's'.

### <font color="blue">ConvertSkipCharacter(string inputString) : string</font>
This API will replace Fuse skip character 's' to PatConfig skip character '_'. Note that it is case sensitive for the skip character.

### <font color="blue">CheckTestInstanceLogLevel() : bool</font>
```
| Output  | Description                                               |
| ------- | --------------------------------------------------------- |
| True    | The current test instance log level is "PRIME_DEBUG".     |
| False   | The current test instance log level is not "PRIME_DEBUG". |
```

## FuseStringUtilities
### <font color="blue">ExtractFuseString(string inputFuseString, int startAddress, int endAddress) : string</font>
This API help to extract sub string of the provided fuse string by the provided addresses.
```
Assume: "inputFuseString" = 1111100000, the fusestring must arrange from MSB->LSB (RHS->LHS).
| Parameter                       | Description                                                                       |
| ------------------------------- | --------------------------------------------------------------------------------- |
| startAddress = 2 endAddress = 2 | Output = 0                                                                        |
|                                 |             MSB               LSB                                                 |
|                                 | Index        9 8 7 6 5 4 3 2 1 0                                                  |
|                                 | FuseString   1 1 1 1 1 0 0 0 0 0                                                  |
|                                 |                            ^      Take single bit only because addresses is same. |
| startAddress = 3 endAddress = 6 | Output = 1100                                                                     |
|                                 |             MSB               LSB                                                 |
|                                 | Index        9 8 7 6 5 4 3 2 1 0                                                  |
|                                 | FuseString   1 1 1 1 1 0 0 0 0 0                                                  |
|                                 |                    ^ - - ^        Take bits based on addresses.                   |
```

### <font color="blue">CreateStringIndexHeader(int numberOfBits) : List<string></font>
This API is used to create string index based on specified size.
```
| Parameter                       | Description                                                                       |
| ------------------------------- | --------------------------------------------------------------------------------- |
| numberOfBits = 10               | Output                                                                            |
|                                 |    List[0] = 9876543210  which is total 10 bits.                                  |
| numberOfBits = 20               | Output                                                                            |
|                                 |    List[0] = 11111111110000000000                                                 |
|                                 |    List[1] = 98765432109876543210  which is total 20 bits.                        |
```

### <font color="blue">ReverseString(string inputString) : string</font>
This API return reversed string of "inputString".

### <font color="blue">ExpandStringByRatio(int ratio, string inputString) : string</font>
This API return expended string based on ratio. The ratio is applied to all character of "inputString". <br>
```
Example: ratio = 2, "inputString" = 11001010. Output = 1111000011001100
before apply ratio 2   1   1   0   0   1   0   1   0
after apply ratio 2    11  11  00  00  11  00  11  00
```

### <font color="blue">ProcessStringByDataBlockSizeAndGapSize(int dataBlockSize, int gapSize, string inputString) : string</font>
This API return string that "inputString" is process with "dataBlockSize" and "gapSize".
```
Assume inputString = 123456789
| Parameter                       | Description                                                                       |
| ------------------------------- | --------------------------------------------------------------------------------- |
| dataBlockSize = 2 gapSize = 0   | Output = inputString = 123456789 due to gapSize = 0                               |
|                                 |                                                                                   |
| dataBlockSize = 2 gapSize = 2   | Output = 12ss34ss56ss78ss9                                                        |
|                                 |     dataBlockSize = 2 means 2 character per block,             12 34 56 78 9      |
|                                 |     gapSize = 2 means fill in 2 skip character between block,  12ss34ss56ss78ss9  |
| dataBlockSize = 3 gapSize = 2   | Output = 123ss456ss789                                                            |
|                                 |     dataBlockSize = 3 means 3 character per block,             123 456 789        |
|                                 |     gapSize = 2 means fill in 2 skip character between block,  123ss456ss789      |
```

### <font color="blue">GetFinalFuseStringToBurn(bool isBurnStringReverse, int ratio, int dataBlockSize, int gapSize, string fuseString) : string</font>
This API is used by Fuse Burn TestMethod. <br>
The API will process the fuseString with ReverseString(), ExpandStringByRatio(), ProcessStringByDataBlockSizeAndGapSize(), ConvertSkipCharacter(). The detail of each function is explained above.
```
Execution Sequence
1. ReverseString(), if "isBurnStringReverse" is true that set in configuration file.
2. ExpandStringByRatio(), Required Parameter is retrieve from configuration file.
3. ProcessStringByDataBlockSizeAndGapSize(), Required Parameter is retrieve from configuration file.
4. ConvertSkipCharacter(), Default behaviour.
5. ReverseString(), Default behaviour. IMPORTANT: Fuse burn data will be written in reverse order; MSB of burn data is written at (Label+Offset).")
```

### <font color="blue">BitwiseCompositeValue(string maskValue, string internalMaskValue) : StringBuilder</font>
This API perform AND-Bitwise using "maskValue" and "internalMaskValue" (aka Burn Range value) then return the result as string.

### <font color="blue">ReplaceDynamicValue(string maskName, string maskFuseGroup, string dynamicMaskValueString, string maskValue) : string</font>
The API will replace "maskValue" that contain character 'm' with "dynamicMaskValueString" that already retrieve from storage.
After successful executiong, the return string is expected to be only contain '0' and '1'.

### <font color="blue">ValidateDynamicMaskValueFromStorage(string maskValueFromStorage) : void</font>
This API is to check the "dynamicMaskValueString" that already retrieve from storage is not empty and only contain valid character : s,0,1

### <font color="blue">RetrieveFuseStringValueFromFuseRead(string registerName, out string fuseReadFuseString) : void</font>
This API is use by Fuse Burn TestMethod to get fusestring from SharedStorage that was stored by FuseRead TestMethod or by UserCode.
The expected SharedStorage Key is 'fusestring_"registerName"' at Context DUT level.

### <font color="blue">ExtractFuseStringComposite(List<char> tempFuseStringBreakDown, int startAddress, int endAddress) : List<int></font>
This API return index of composite fusestring based on provided "tempFuseStringBreakDown" and addresses.
```
Output = { 0, 1, 2, 3, };

startAddress = 4, endAddress = 7, tempFuseStringBreakDown = { '1', '1', '1', '1', '0', '0', '0', '0', };
    Reading                       MSB           LSB
    Fuse String                  - 1 1 1 1 0 0 0 0
    Index / Data address in List - 0 1 2 3 4 5 6 7
    User address                 - 7 6 5 4 3 2 1 0
    Actual Index to Get                    ^ ^ ^ ^   
```

### <font color="blue">GetRegisterToHandle(FuseBurnConfiguration.FuseBurnJsonFile fuseBurnJsonFile, string configurationName, string registerName) : FuseBurnConfiguration.Register</font>
This API is use by FuseBurn TestMethod to retrieve Register that matched "configurationName" and "registerName" from "fuseBurnJsonFile".

### <font color="blue">GetConfigurationDatalogFormat(FuseBurnConfiguration.FuseBurnJsonFile fuseBurnJsonFile, string configurationName) : DatalogType.FuseBurnOption</font>
This API is use by FuseBurn TestMethod to retrieve DatalogFormat defined in Configuration.

### <font color="blue">GetBurnPatternConfigToHandle(FuseBurnConfiguration.FuseBurnJsonFile fuseBurnJsonFile, string configurationName) : FuseBurnConfiguration.BurnPatternConfig</font>
This API is use by FuseBurn TestMethod to retrieve BurnPatternConfig defined in Configuration.

### <font color="blue">ExtractDynamicValue(FuseBurnConfiguration.Dynamic dynamicData, string fuseGroupName) : string</font>
This API is use by FuseBurn TestMethod to retrieve dynamic mask value from Storage (UserVar/SharedStorage/DFF).

### <font color="blue">BuildFullStringFuseBurnData(string maskValueToOverride, string fuseReadFuseString) : StringBuilder</font>
This API is use by FuseBurn TestMethod to Build Full String Fuse Burn data.

### <font color="blue">InitializeFuseBurnData(string fuseReadFuseString) : StringBuilder</font>
This API is use by FuseBurn TestMethod for default string that is full of standard skip character that length matched fuse string that retrieve from storage.

### <font color="blue">CreateSignificiantFigureIndex(int numberOfBits, int bitPerPackage) : string</font>
This is sub routine used by API CreateStringIndexHeader().

# Fuse Read
## Configurations
Data structure for Configuration after parse Configuration File and Validator to validate the data structure.

## Voltages
Data strucure for Voltage after parse Voltage File and Validator to validate the data structure.

## Fuse Read Handler
Get the fuse read handler based on provided requirement information : configurationName, registerName and executionMode (True for Simulation OR False for ExecutePattern). <br>
The API will return fuse read object that fulfill the requirement and exist in Fuse Read Configuration File.

# Fuse Burn
## Configurations
Data structure for Configuration after parse Configuration File and Validator to validate the data structure.
## Voltages
Data strucure for Voltage after parse Voltage File and Validator to validate the data structure.

# Json Schema Validator
The API that setup json schema validator based on schema type that get the schema rule setup in Prime for Fuse Read or Fuse Burn. <br>
The validator can perform validation on input json file against the selected Prime schema rule for Fuse Read or Fuse Burn.

# Ult Decode Basic
This API is used by Fuse Read MaskUltDecode TestMethod for Ult related feature.