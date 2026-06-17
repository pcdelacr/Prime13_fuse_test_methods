## CommonParam

Revision 1.0.0

July 2022

[[_TOC_]]


This **REP** is mainly intended to describe the Common Parameter available in Prime and it's usages.
For any information on how to actually add the common param to be part of the test method/usercode parameter, please refer to the General-> Frequent Asked-> Common Parameters documentation.

Please note that this document will mention some services dependencies but it's not intended to explain in deep those services.

<hr>

### FivrConditionCommonParam
This set of common parameters allows using one of the features provided by Voltage Service (FIVR Conditions) without creating additional user code. With these common parameters, the user could set a FIVR Condition to be applied before the corresponding test method execution begins. A requirement from the test method where this functionality can be used is that it must have a plist parameter.

Two common parameters must be used in conjunction in order to effectively enable the feature. The provided parameters are as follows:
- **FivrConditionName**: specifies the name of the Fivr Condition to apply. The appropriate configuration files must exist and be already loaded by Aleph initialization as specified in Voltage Service documentation.
- **FivrConditionPlistParamName**: specifies the name of the parameter which points to the plist to execute (**not** the plist name itself). This is required since User Code may exist where the parameter name is not standardized, the feature should still be available under these circumstances.

**FivrConditionCommonParam examples:**
```python
## In this example patterns in plist MyPlist would be modified based on the predefined configuration FIVR_CORE0_LC.
Test ExampleTest FivrConditionCommonParameter
{
   Patlist = "MyPlist";
   FivrConditionName = "FIVR_CORE0_LC";
   FivrConditionPlistParamName = "Patlist";
   ...
}
```
Here, a Fivr Condition "FIVR_CORE0_LC" is applied in a ExampleTest test instance, and "FivrConditionPlistParamName" is given the value of "Patlist" which is the name of the parameter that points to the plist name for this specific Test Method.

In order for this to work the configuration files described above for FIVR Conditions must be in place as well as the enabling of these common parameters for the "ExampleTest" test method in the corresponding header files. The latter is achieved by copying the "ExampleTestCommonParams.xml" from Prime's resources\preheaders\CommonParams folder into the Test Program's supersedes\preheaders folder and modifying that xml file to import the Fivr Condition common parameters as follows:

```xml
<?xml version="1.0" encoding="utf-8"?>
<TestLibraryInterfaces xmlns:xsi=...>
  <TestLibrary name="PrimeTestInstance">
    <TestClass name="ExampleTestCommonParams" />
    <Imports>
      <FileName>FivrConditionCommonParam.xml</FileName>
    </Imports>
    <PublicBases/>
    <Parameters/>
    <ExitPorts/>
  </TestLibrary>
</TestLibraryInterfaces>

```

<hr>

### FlowIndexCommonParam
The FlowIndexCommonParam enables the prime users to implement a similar capability like the "Single Flow recovery" offered in Evergreen.  

Basically the FlowIndexCommonParam offers two parameters:  
1.  **FlowIndex** Parameter to specify the instance flow index on Dynamic Flows. This is used for legacy programs that are not adopting TOS MTT speed flows.
2.  **FlowIndexCallbackName** Flow Index callback function name. in the format of CALLBACKNAME(ARGS). This callback allow the user to specify any check before executing the test instance Execute function. User Callback MUST return only \"CONTINUE\" or \"FAIL\" any other returned value is Invalid and will exit PORT -1.

**FlowIndexCommonParam examples:**
```python
Test PrimePauseTestMethod Pause_IO_c1_r1_P0
{
    SleepTime = 10;
    FlowIndex = "1";
    ExitPort = 0;
    FlowIndexCallbackName = "CheckSingleRecoveryFlow(IO)";
    LogLevel = "PRIME_DEBUG";
}
```

**As reference below is a sample code for the FlowIndexCallbackName callback:**

``` C#
public static string CheckSingleRecoveryFlow(string args)
{
    var indexValue = Prime.Services.TestProgramService.GetCurrentTestInstanceParameter("FlowIndex");
    var indexValueForDomain = Prime.Services.TestProgramService.GetDomainCurrentFlow(args);

    if (indexValueNumber == indexValueForDomain)
    {
        return "CONTINUE";
    }
    else
    {
        return "FAIL";
    }
}
```
<hr>

### PatConfigService SetPoints' common parameters
The PatConfigService SetPoints' common parameters is a set of five parameters:
- **SetPointsPlistParamName**: The name of the current instance parameter containing the PList name to apply the pattern modify. If it's empty, it will use the parameter=[`Patlist`] by default.
- **SetPointsRegEx**: The regular expression to filter the list of patterns.
- **SetPointsPlistMode**: The PList mode to `Local` or `Global`. If `Local`, Prime will apply the pattern modifications based on the patterns of the PLlist resolved by the `SetPointsPlistParamName`. If `Global`, Prime will ignore the `SetPointsPlistParamName` and will apply the `SetPointsRegEx` to all the loaded patterns. Default: `Local`.
- **SetPointsPreInstance**: The list of Group names and Set point value to apply before test execution.
- **SetPointsPostInstance**: The list of Group names and Set point value to apply after test execution.

**PatConfigSetPointsCommonParam examples:**
```python
## PrimeMemorySnapshot related callbacks test.
Test PrimePauseTestMethod WriteToItuffMemorySnapshot_P1
{
    LogLevel = "PRIME_DEBUG";
    Patlist = "SearchPlist";
    SetPointsPreInstance = "Cache:RatioChange:0.4GHz";
    SetPointsPostInstance = "Cache:RatioChange:1.0GHz";
    SetPointsPlistParamName = "Patlist";
    SetPointsPlistMode = "Local";
    SetPointsRegEx = ".*_rst_.*";
    SleepTime = 1;
}
```

For more detail, go to the PatConfigService documentation.

<hr>

### PostInstanceCommonParam
PostInstance common param is meant to execute callbacks after executing the instance logic:
Verify -> Execute Test Method -> Execute PostInstance

There is no additional logic needs to be added in the user code in order to get the post-instance to work.

**PostInstanceCommonParam examples:**
```python
## Using PrimePauseTestMethod as an example. In this example, post-instance is used to call the PrimeMemorySnapshot callback.
Test PrimePauseTestMethod WriteToItuffMemorySnapshot_P1
{
    LogLevel = "PRIME_DEBUG";
    PostInstance = "PrimeMemorySnapshot()";
    SleepTime = 1;
}
```

<hr>

### PreInstanceCommonParam
PreInstance common param is meant to execute callbacks prior to executing the instance logic:
Verify -> Execute PreInstance -> Execute Test Method

There is no additional logic needs to be added in the user code in order to get the pre-instance to work.

**PreInstanceCommonParam examples:**
```python
## Using PrimePauseTestMethod as an example. In this example, pre-instance is used to call the PrimeMemorySnapshot callback.
Test PrimePauseTestMethod WriteToItuffMemorySnapshot_P1
{
    LogLevel = "PRIME_DEBUG";
    PreInstance = "PrimeMemorySnapshot()";
    SleepTime = 1;
}
```

<hr>

### PrePlistCommonParam
PrePlist common param is meant to execute callbacks prior to executing the plist in a test method/user code:
Verify -> While Executing Test Method 
			-> Before Executing Plist, execute callbacks from pre-plist -> Execute Plist

This common param is meant to be used together with test methods/user code that have plist execution. No specific code changes needed to implement this. Simply add the parameter and it will take effect prior to plist execution.

**PrePlistCommonParam examples:**
```python
## PrimeMemorySnapshot related callbacks test.
Test PrimeFunctionalTestMethod PrimeFuncCaptureFailuresFuncTest_P1
{
    PrePlist = "PrimeMemorySnapshot()";
    Patlist = "passing_plist";
    TimingsTc = "Functional::basic_func_timing_10MHz_20MHz";
    LevelsTc = "Functional::basic_func_lvl_nom";
    FailuresToCaptureTotal = 4;
    LogLevel = "PRIME_DEBUG";
}
```

<hr>


### RelayTestConditionName
The RelayTestConditionName recevies a test condition name of type `Level` which will be applied at the beginning of the test instance using the SmartTC category `RELAY`.

**RelayTestConditionName examples:**
```python
Test PrimePauseTestMethod WriteToItuffMemorySnapshot_P1
{
    LogLevel = "PRIME_DEBUG";
    RelayTestConditionName = "RelayTC";
    SleepTime = 1;
}
```

<hr>

### SoftwareTriggerCallBack
The SoftwareTrigger common parameter receives the name of the callback Prime will call on the event of the software trigger (a.k.a TOS trigger) during the plist execution.

This callback will received the number of the trigger instruction as a string.

**SoftwareTriggerCallBack examples:**
```python
Test PrimePauseTestMethod WriteToItuffMemorySnapshot_P1
{
    LogLevel = "PRIME_DEBUG";
    SoftwareTriggerCallBack = "CallbackToProcessSoftwareTrigger";
    SleepTime = 1;
}
```

## Version tracking


| **Date**       | **Version** | **Author**     | **Comments** |
| -------------- | ----------- | -------------- | ------------ |
| July 5<sup>th</sup>, 2022 | 1.0.0       | mmohdfai       | Initial rev of the document. |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System