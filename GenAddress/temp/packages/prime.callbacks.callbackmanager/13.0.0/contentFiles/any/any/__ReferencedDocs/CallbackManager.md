## Prime's Out-of-the-box Callbacks

Revision 1.0.4

Jun 2022

[[_TOC_]]


This **REP** is intended to describe the Prime's out-of-the-box callbacks.

Please note that this document will mention some services dependencies but it's not intended to explain in deep those services.

Every single Prime out-of-the-box callback follows the signature `string (string)`, with the parameter being ignored or returning empty when it doesn't apply.

## FlowEvent callbacks
* `PrimeSetCurrentTimeSnapShot`: Saves a timestamp when the callback gets executed to the given key in sharedStorage. This is intended to be used with FlowEvents. If its not used as flow event provide the key name as second param. When this callback is used as FlowEvent the first parameter will always be the current port number of the given test flow, this is not used and is automatically populated by TOS, hence only key argument is required.
* `PrimeSetDeviceStartTimeSnapshot`: Callback intended to be used ONLY as a flow event for the beginning of the TestPlanStart flow. The functionality of this method is to store a snapshot of the current device start time. The signature of this method do not recieve any arguments.

For more details on how to use a callback as a flowEvent please revisit the documentation https://dev.azure.com/mit-us/PrimeWiki/_wiki/wikis/PrimeWiki.wiki/33695/FlowEvents

### PrimeMemorySnapshot callback
* PrimeMemorySnapshot:
  * Print the memory used at that given time of the execution.
  * The argument is used to set a tname custom postfix, the argument can be empty.

**PrimeMemorySnapshot callbacks examples:**
```python
## PrimeMemorySnapshot related callbacks test.
Test PrimePauseTestMethod WriteToItuffMemorySnapshot_P1
{
    LogLevel = "PRIME_DEBUG";
    PreInstance = "PrimeMemorySnapshot()";
    SleepTime = 1;
}

Test PrimePauseTestMethod WriteToItuffMemorySnapshotWithCustomPostFix_P1
{
    LogLevel = "PRIME_DEBUG";
    PreInstance = "PrimeMemorySnapshot(MyCustomPostFix)";
    SleepTime = 1;
}
```

**PrimeMemorySnapshot ituff output examples:**
```python
2_tname_CALLBACKS::WriteToItuffMemorySnapshot_P1_MemoryShapshot
2_strgval_PrivateBytes=554541056B
2_tname_CALLBACKS::WriteToItuffMemorySnapshotWithCustomPostFix_P1_MemoryShapshot_MyCustomPostFix
2_strgval_PrivateBytes=555429888B
```

### PrimeReadPatternPinData callback
* PrimeReadPatternPinData:
  * Print vectors for an specific pattern in memory.
  * The argument must follow the structure: `token|pattern|pin|a/l=address or label|numberOfVectors`.
    * token: it is used to set a custom tname for ituff.
    * pattern: pattern name to read.
    * pin: pin name or pin group name which vectors are going to be printed.
    * a/l=address or label: Label or Address to start reading from.
    * numberOfVectors: number of vector to read.

**PrimeReadPatternPinData callbacks examples:**
```python
## PrimeReadPatternPinData related callbacks test.
Test Dummy WriteToItuffPinDataSpecifyingLabel_P1
{
    LogLevel = "Enabled";
    PreInstance = "PrimeReadPatternPinData(MyTokenName2|generic|xxHPCC_DPIN_Dig_slcB_A0|l=Start|6)";
}

Test Dummy WriteToItuffPinDataSpecifyingPinGroup_P1
{
    LogLevel = "Enabled";
    PreInstance = "PrimeReadPatternPinData(MyTokenName3|generic|HPCC_DPIN_Dig_slcB_by2B_grp|a=1|36)";
}
```

**PrimeReadPatternPinData ituff output examples:**
```python
2_tname_MyTokenName2_xxHPCC_DPIN_Dig_slcB_A0
2_rawnhex_msbF_6_13
2_tname_MyTokenName3_xxHPCC_DPIN_Dig_slcB_B0
2_rawnhex_msbF_36_DE32B3A2
2_tname_MyTokenName3_xxHPCC_DPIN_Dig_slcB_B1
2_rawnhex_msbF_36_63878D1C8
```


### PatternModify callbacks
* PrimeEnablePatternModifyCache: 
  * Enables the pattern modify application cache in the service.
* PrimeDisablePatternModifyCache:
  * Disables the pattern modify application cache in the service.

### PinMonitor callbacks
* PrimePinMonitorPause: 
  * Pause monitoring of the pins associated to the configName given as input.
  * Checks for existence in activeConfigs.
  * Evaluates the data for a given configName, before pausing.
* PrimePinMonitorResume:
  * Resumes monitoring of the pins associated to the configName given as input.

**PinMonitor callbacks examples:**
```python
## PinMonitor related callbacks test.
Test PrimePauseTestMethod PinMonitorPause_P1
{
    LogLevel = "PRIME_DEBUG";
    PreInstance = "PrimePinMonitorPause(PinXyz)";
    SleepTime = 1;
}

Test PrimePauseTestMethod PinMonitorResume_P1
{
    LogLevel = "PRIME_DEBUG";
    PreInstance = "PrimePinMonitorResume(PinXyz)";
    SleepTime = 1;
}
```

### SmartTc callbacks

* PrimeEnableSmartTc: Enables SmartTc.
* PrimeDisableSmartTc: Disables SmartTc.

### SharedStorageService callbacks

The `SharedStorageService` related callbacks are devided in two categories: gets and inserts.

* Gets: The getters receives as parameter the name of a key in the `SharedStorageService`.
These callbacks return and print in console the value converted to string of the key in the `SharedStorageService`.
Callbacks:
  * PrimeGetStringFromDutTable: Gets the given key from the string table with DUT context.
  * PrimeGetIntegerFromDutTable: Gets the given key from the integer table with DUT context.
  * PrimeGetDoubleFromDutTable: Gets the given key from the double table with DUT context.
  * PrimeGetStringFromIpTable: Gets the given key from the string table with IP context.
  * PrimeGetIntegerFromIpTable: Gets the given key from the integer table with IP context.
  * PrimeGetDoubleFromIpTable: Gets the given key from the double table with IP context.

* Inserts: The inserts receive as parameter the `key=value`, inserting the `value` in the `SharedStorageService` for the given `key`.
These callbacks return an empty string.
Callbacks:
  * PrimeInsertStringToDutTable: Inserts the given value with the given key on the string table with DUT context.
  * PrimeInsertIntegerToDutTable: Inserts the given value with the given key on the integer table with DUT context.
  * PrimeInsertDoubleToDutTable: Inserts the given value with the given key on the string table with DUT context.
  * PrimeInsertStringToIpTable: Inserts the given value with the given key on the string table with IP context.
  * PrimeInsertIntegerToIpTable: Inserts the given value with the given key on the integer table with IP context.
  * PrimeInsertDoubleToIpTable: Inserts the given value with the given key on the string table with IP context.

## UserVarService callbacks

The `UserVarService` related callbacks are devided in two categories: gets and sets.

* Getters: The getters receives as parameter the name of a user variable that these callbacks will return and print in console.
Callbacks:
  * PrimeGetStringUserVar: Gets the given user variable name from the string.
  * PrimeGetIntegerUserVar: Gets the given user variable name from the integer.
  * PrimeGetDoubleUserVar: Gets the given user variable name from the double.

* Setters: The inserts receives as parameter the `name=value`, saving the `value` to the user variable `name`.
These callbacks return an empty string.
Callbacks:
  * PrimeSetStringUserVar: Sets the given string value to the given user variable name.
  * PrimeSetIntegerUserVar: Sets the given integer value to the given user variable name.
  * PrimeSetDoubleUserVar: Sets the given double value to the given user variable name.

## TestProgramService callbacks

* `PrimeExecuteTestInstance`: Executes the test instance name received on the parameter. Returns the resulting port of the test instance execution.

## ThreadAlignment callbacks

The `ThreadAlignment` Callbacks offer two simple functions:
  * `PrimeAcquireOnlyMeLock`: this funtion requires one argument a number greather than 0 that is the timeout for the thread lock.
  * `PrimeReleaseOnlyMeLock`: function to release the lock.

For more details please review the PrimeThreadAlignment Service documentation.

## Version tracking


| **Date**       | **Version** | **Author**     | **Comments** |
| -------------- | ----------- | -------------- | ------------ |
| Jan 11<sup>th</sup>, 2022 | 1.0.0       | lchavarr       |              |
| Jan 13<sup>th</sup>, 2022 | 1.0.1       | dajimene       | Added PinMonitor callbacks. |
| Feb 14<sup>th</sup>, 2022 | 1.0.2       | hramirez       | Added ThreadAlignment callbacks. |
| Feb 28<sup>th</sup>, 2022 | 1.0.3       | dajimene       | Added PrimeMemorySnapshot callbacks. |
| Jun 28<sup>th</sup>, 2022 | 1.0.4       | mjhernan       | Added PrimeSetDeviceStartTimeSnapshot callback |
| Mar 31<sup>th</sup>, 2023 | 1.0.4       | rppintor       | Added PrimeReadPatternPinData callback |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System