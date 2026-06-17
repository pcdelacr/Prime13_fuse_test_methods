[[_TOC_]]

# Prerequisites

<p style="font-size: 20px;"><span style="color: #cf6679;">This Service is using Aleph initialization. Please refer to Aleph documentation for details</span></p>

[Link to Aleph Documentation](/Learn/PRIME-Features/Aleph)

# TestConditionService

TestConditionService handles all test conditions operations: Timing , levels, thermal and SmartTC feature.

# Capabilities

## Pattern Trigger map
As the initialization of this service is enabled to apply multiple ExtTriggers, this feature can help to move the test time penalty of each apply ExtTrigger to Init process through aleph infrastructure.

### Configuration File
Pattern trigger map accept multiples aleph files, but not elements with the same plist name.

File can only be recognized if it is named with the base name=["PatternTriggerMap.json"] or with the same base name but by adding a custom prefix with a dot separator:

_**PatternTriggerMap.json**_

_**</span><span style="color:orange">\<MyCustomName\></span>.PatternTriggerMap.json**_

As follows an example of the content of this file:
```json
{
  "TestConditions": [
    {
      "Name": "MyFirstPlistName",
      "PatternTriggerMap": "MyFirstExtTriggerName"
    },
	{
      "Name": "MySecondPlistName",
      "PatternTriggerMap": "MySecondExtTriggerName"
    },
	{
      "Name": "MyThirdPlistName",
      "PatternTriggerMap": "Dc::TriggerDCMap"
    }
  ]
}
```

### Field details:

For this file only is enabled to parse an array estructure with _*Name*_ and _*PatternTriggerMap*_;
- _**Name**_: Plist to setup with an ExtTrigger.
- _**PatternTriggerMap**_: ExtTrigger name to apply.

<span style="font-weight: bold; color:red">Note:</span> If user use this way and overwrite pattern trigger map during the main flow, the user has the responsibility to restore the plist to the previous pattern trigger map setup.

### Console output (debug mode)
Each ExtTrigger applied during the initialization of this service can be printed to console in debug mode in form of list.

# APIs Description:
## TestConditionService APIs

| Method Name| Description  |
|--|--|
| **Exists**(*TestConditionName*) | Returns boolean indiction whether the given test condition name exists  |
| **SetIpEndSequenceTestCondition**(*TestConditionName, sessionContext*) | Sets the given test condition to apply on the "ApplyEndSequence" for the current IP.  |
| **ApplyEndSequence**(*sessionContext*) | Applies the TPL file specified EndSequence of the currently loaded test plan to the hardware. An **exception** *will be thrown in cases where KeepAlive execution is running during the call for that API*. |
| **CreateTestConditionFromChangeVariable**(*baseTestCondition, spectSetVariableName, SpecSetVariableValue*) | Build new test conditions on the fly based on existing one. *For Shmoo, IpSearch and Search usage, to pre-create test condition for better run time performance.* |
| **SetPowerUpTcName**(*powerUpTcName, sessionContext*) | Set the power up TC name. A valid TestConditionName should be given, *for disabling the PowerUp TC an Empty String should be passed.* |
| **GetPowerUpTcName**(*sessionContext*) | Get the current power up TC name. Empty String indicating that the PowerUp TC is disabled. |
| **IsSmartTcEnabled**() | Returns True if SmartTc is enabled. |
| **EnableSmartTc**() | Enable smartTC capability. |
| **DisableSmartTc**() | Disable smartTC capability. |
| **FlushSmartTcCategory**(*smartTcCategory*) | Flushes the smart test condition state for a given category if SmartTC is enabled. In cases where a NONE Category given an exception will be thrown. |
| **FlushAllSmartTcCategories**() | Flushes all the smart test condition state for all categories if SmartTC is enabled. |
| **ResolveAllTestConditions**() | Evaluate to propagate the values through all equations in all dependent spec sets, collection sets and test conditions. |
| **GetTesterPeriodValue**(*domainName*) | Returns the applied Timing TC period value for the given pattern timing domain name. |
| **GetTestCondition**(*TestConditionName, sessionContext*) | Returns a new TestCondition object corresponded to the given TC name. An exception will be thrown if the test condition name is invalid. |
| **GetTestCondition**(*TestConditionName, smartTcCategory, sessionContext*) | Returns a new TestCondition object corresponded to the given TC name and SmartTc category. An exception will be thrown if the test condition name is invalid.|
| **CreateDirectTestCondition**(*TestConditionName, TestConditionType, sessionContext*) | Creates a direct test condition for a given test condition name and type. DirectTestCondition is a TestCondition with only exception that SpecSetVariables are not allowed - onle UserVars.|

## TestCondition:

Represent a TestCondition Object, relevant for *GetTestCondition()* and *CreateDirectTestCondition()* APIs.


| Method Name| Description  |
|--|--|
| **GetName**() | Returns the TestCondition name.  |
| **GetPinAttributeValue**(*pinOrPinGroupName, pinAttributeName*) | Retrieve pin attribute value for given pin/group and attribute name.  |
| **GetSpecSetValue**(*specSetName*) | Retrieve spec set value.|
| **SetDcTestCounterName**(*counterName*) | Set the counter name for Dc Testing, used by telemetry system as a name.|
| **Apply**(*smartTcCategory*) | Applies the test condition with the given *smartTCCategory*. See the notes for ApplyEndSequence API |
| **Apply**() | Applies the test condition. See the notes for ApplyEndSequence API |
| **ForceApply**(*smartTcCategory*) | Applies the test condition and flushes the SmartTcCategroy (where it is not NONE). in cases where the Test Condition Type is TIMING and KeepAlive is currently executed the apply will be skipped. |
| **ForceApply**() | Force apply of the TestCondition without considering the SmartTc category. |
| **Resolve**() | Evaluate the equations in the TestCondition (based on SpecSet or UserVar values). |
| **SetPinAttributeValue**(*pinName, attributeName, attributeValue*) | Direct modification of TC attribute per specific pinName. |
| **SetSpecSetValue**(*specSetName, specSetValue*) | Direct modification of specSet variable used by the TestCondition without considering the selector used the that TestCondition.  |


### Examples:
```json
Levels Sample_Level
{
    samplePin
    {
	    SampleAttribute = "SamapleValue";
    }
}

ITestCondition tc = Prime.Services.GetTestCondition.Exists("ExistingTC");
tc.GetName() // "ExistingTc"
var attVal = tc.GetPinAttributeValue("SamplePin", "SampleAttribute");
// attVal = "SamapleValue"
tc.SetPinAttributeValue("Sample_Level", "SampleAttribute" , "NewAttributeValue");
tc.Apply(); // SampleAttribute for SamplePin will be changed to NewAttributeValue
```

# Version tracking

| **Date**   | **Prime release** | **Author**    | **Comments** |
| ---------- | ----------- | ------------- | ------------ |
| Feb 10<sup>st</sup>, 2022  | 8.1 | Didier Armando Jimenez Retana | [Implement the PatternTriggerMap alephable.](https://dev.azure.com/mit-us/PRIME/_boards/board/t/US/Backlog%20items/?workitem=23606) |
| May 17<sup>th</sup>, 2022 | 10.00.00       | Andrea Gomez Montero | Changing the Test Conditions PatternTriggerMap .json file format.|
| Nov 8, 2023 | 12.03.00 | IS Team | Updates/improvements to documentation. No API changes. |