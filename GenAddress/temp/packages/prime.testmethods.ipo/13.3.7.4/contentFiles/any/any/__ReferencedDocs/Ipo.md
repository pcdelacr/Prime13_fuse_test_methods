<h1>Prime Test-Method Specification REP</h1>

[[_TOC_]]

## ⚠️ Important Notice

> **🚫 WARNING: Do NOT use `TestType: FLOW`**
>
> `TestType: FLOW` is currently **not supported**. It causes issues with flow ID tracking in TRACE.
>
> ✅ Please use `TestType: INSTANCE` only until further notice.

# Download Sample Configuration File
<font color="green">**Download** </font>[IPO Sharedstorage](./.attachments/ipo_sharedstorage.json) <br>
<font color="green">**Download** </font>[IPO Sharedstorage with Decremental Step](./.attachments/ipo_sharedstorage_withDecrementalStep.json) <br>
<font color="green">**Download** </font>[IPO Uservar](./.attachments/ipo_uservar.json) **For Decremental Step, refer to SharedStorage example configuration file. <br> 
<font color="green">**Download** </font>[IPO TestCondition](./.attachments/ipo_testcondition.json) <br>
<font color="green">**Download** </font>[IPO Pattern Modify with Address](./.attachments/ipo_patmod_withAddress.json) <br>
<font color="green">**Download** </font>[IPO Pattern Modify with AddressLabel](./.attachments/ipo_patmod_withAddressLabel.json) <br>

# Methodology
IPO is a testmethod that allow user to iteratively modify test setups and execute test with IPO test instance. User is required to provide configuration file that contain test name to be execute and modification to apply before executing the test.<br>
The supported modifier are "SharedStorage", "Uservar", "TestCondition" and "Pattern".<br>
Maximum allowed Loop is 3.<br>
User is allow to perform single step modification, multiple step modification or mixture of supported modifier.<br>

**Note**
<font color="red">**IPO is a test setup and executor. Users should be aware that test time is influenced by their IPO configuration and the test method used to execute the test. Therefore, they are responsible for managing and optimizing test time accordingly.**</font>

# Verfiy
1. Validate TestInstanceParameters.
2. Get and Validate user ConfigurationFile content with JSON schema.
3. Create configurationHandle that will be used at Execute.
   - The handle contains information of TestsToExecute and LoopToExecute.

# Execute
1. Get TestsToExecute from handle.
2. Get LoopToExecute from handle.
3. If `RestoreMode` is `ENABLED`, execute Loop's SaveRestore to snapshot all modifier initial value.
4. Start IPO Loop execution. Refer to [Methodology](#methodology) for iteration execution.
5. Start `Extension-CustomDatalog` for print eachTest-Concatenated-IterationExitPortResult to ituff.
6. If `StoreIterationResult` is setup in `ConfigurationFile`, store number of excuted iteration to storage and write to ituff.
7. If `RestoreMode` is `ENABLED`, execute Loop's Restore to apply modifier snapshot value that is before IPO Loop execution.

# Test Instance Parameters
| **Parameter Name**             | **Required?** | **Type**        | **Values**                                              | **Comments**                      |
| ------------------------------ | ------------- | --------------- | ------------------------------------------------------- | --------------------------------- |
| ConfigurationFile              | Yes           | File            | Path to configuration file.                             | Able to use with "~HDMT_TPL_DIR". |
| RestoreMode                    | Optional      | String          | ENABLED or DISABLED.                                    | Default is set to DISABLED. Set to ENABLED for restore modified data defined in ConfigurationFile to its initial value after complete IPO execution. |
| ExecutionMode                  | Yes           | String          | Refer to [ExecutionMode Overview](#executionmode-overview). | Refer to [ExecutionMode Overview](#executionmode-overview). |
| PreInstance                    | Optional      | String          | PrimeSetTestNameOverride(CustomInstanceName)            | This is to overwrite executing test ituff printing instance name of "2_tname_<overwrite>". |

## ExecutionMode Overview
This section describes how IPO handles test iterations based on different `ExecutionMode` settings. Each mode determines when IPO stops executing and which exit port is triggered.
**Note**
- Applicable to all `ExecutionMode`.
   - If a test fails with -1, IPO will stop iteration and exit with port -1.
   - If a test fails with -2, IPO will stop iteration and exit with port -2.
   - User required to review the error of executed test.
- For Non-`ExecuteAll` mode, if all iteration is executed and no condition met, it will exit port 1.
- 🧩 Symbol Legend
   - ✔️ Test Passed (returns 1)
   - ❌ Test Failed (returns 0 only; not -1 or -2)
   - ⚪ Not Executed / Don't Care

**ExecuteAll** (TestType: FLOW and INSTANCE)
IPO will execute all iteration by ignoring test result, and exit port 1.

**FirstPass** (TestType: FLOW and INSTANCE)
IPO will stop iteration when first `TestsToExecute` passed and exit port 2. 
Assume user setup 4 iteration and 3 `TestsToExecute` (TestA, TestB, TestC).
| Iteration | TestA | TestB | TestC | Execution |
| --------- | ----- | ----- | ----- | --------- |
| 0         | ❌   | ❌   | ❌   | Condition not met, execute next iteration. |
| 1         | ❌   | ✔️   | ⚪   | Condition met, stop iteration. |
| 2         | ⚪   | ⚪   | ⚪   | Not executed. |
| 3         | ⚪   | ⚪   | ⚪   | Not executed. |

**FirstFail** (TestType: FLOW and INSTANCE)
IPO will stop iteration when first `TestsToExecute` failed and exit port 2. 
Assume user setup 4 iteration and 3 `TestsToExecute` (TestA, TestB, TestC).
| Iteration | TestA | TestB | TestC | Execution |
| --------- | ----- | ----- | ----- | --------- |
| 0         | ✔️   | ✔️   | ✔️   | Condition not met, execute next iteration. |
| 1         | ✔️   | ❌   | ⚪   | Condition met, stop iteration. |
| 2         | ⚪   | ⚪   | ⚪   | Not executed. |
| 3         | ⚪   | ⚪   | ⚪   | Not executed. |

**FirstAllInstancePass** (TestType: INSTANCE only) 
IPO will stop iteration when all `TestsToExecute` passed during same iteration, and exit port 2.
Assume user setup 4 iteration and 3 `TestsToExecute` (TestA, TestB, TestC).
| Iteration | TestA | TestB | TestC | Execution |
| --------- | ----- | ----- | ----- | --------- |
| 0         | ✔️   | ❌   | ❌   | Condition not met, execute next iteration. |
| 1         | ❌   | ✔️   | ❌   | Condition not met, execute next iteration. |
| 2         | ✔️   | ✔️   | ✔️   | Condition met, stop iteration. |
| 3         | ⚪   | ⚪   | ⚪   | Not executed. |

**FirstAllInstanceFail** (TestType: INSTANCE only)
IPO will stop iteration when all `TestsToExecute` failed during same iteration, and exit port 2.
Assume user setup 4 iteration and 3 `TestsToExecute` (TestA, TestB, TestC).
| Iteration | TestA | TestB | TestC | Execution |
| --------- | ----- | ----- | ----- | --------- |
| 0         | ✔️   | ❌   | ❌   | Condition not met, execute next iteration. |
| 1         | ❌   | ✔️   | ❌   | Condition not met, execute next iteration. |
| 2         | ❌   | ❌   | ❌   | Condition met, stop iteration. |
| 3         | ⚪   | ⚪   | ⚪   | Not executed. |

# Exit Port
| Exit Port | Description |
| --------- | ----------- |
|  0        | Catch all fail. |
|  1        | IPO sucessfully execute all iteration.<br> ExecutionMode<br> - `ExecuteAll` complete execute all iteration.<br> - Non-`ExecuteAll` complete execute all iteration but no condition met. |
|  2        | IPO ExecutionMode `FirstPass` / `FirstFail` / `FirstAllInstancePass` / `FirstAllInstanceFail` condition met. |
| -1        | IPO executed test return -1. |
| -2        | IPO executed test return -2. |

# ConfigurationFile Json Attribute
**<font color="blue">Blue Text means is a container for attribute.</font>
## Main Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
|<font color="blue">Configuration</font>|Required |User can set only one Configuration. Use different ConfigurationFile for another Configuration setup.|

## Configuration Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
|<font color="blue">TestsToExecute</font>| Required | User to define test to execute by IPO. |
|<font color="blue">StoreIterationResult</font>| Optional | User to define storage information to store the iteration result. |
|<font color="blue">PatternInfos</font>| Required for Pattern Modifier | Contain pattern information for IPO pattern modifier. |
|<font color="blue">Loop</font>| Required | User to define Minimum 1 Loop or Maximum 3 Loop. |

### TestToExecute Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
| Name | Required | Full Test Instance name or Flow name for execution. |
| TestType | Required | "INSTANCE" or "FLOW". |

### StoreIterationResult Attribute
```Json
    "StoreIterationResult": {
      "SharedStorage": {
        "Key": "IpoIterationResultKey"
      }
    },
```

### PatternInfos Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
| Name | Required | Name of this pattern info. Pattern modifier will provide name that match the intended pattern info to use. |
| NumberOfVectors | Required | Expected number of vectors (bits) or ExpectedDataSize. |
| <font color="blue">PatternModifyInfos</font> | Required | "INSTANCE" or "FLOW". |

#### PatternInfos Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
| PatternsRegex | Required | patterns regex use for pattern modification. |
| PatList | Required | pattern list name. |
| StartAddress | Required | start address to use for pattern modification. can be address or address label. |
| Offset | Required | the offset from the StartAddress. |

## Loop Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
| Id                                  | Optional | Name of the Loop. Only use by user. |
| <font color="blue">Iteration</font> | Required | Iterations of the loop. |

## Iteration Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
| Id                                      | Optional    | Name of the Iteration. Only use by user. |
| StepCount                               | Situational | Number of expension/step for the iteration. It is require to modifiy numeric data of SharedStorage, Uservar and TestCondition. |
| <font color="blue">TestCondition</font> | Optional    | Modify TestCondition. |
| <font color="blue">SharedStorage</font> | Optional    | Modify SharedStorage. |
| <font color="blue">Pattern</font>       | Optional    | Modify Pattern. |
| <font color="blue">UserVar</font>       | Optional    | Modify Uservar. |

### TestCondition Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
| Name        | Required | test condition name. example, <Module>::basic_func_timing_10MHz_20MHz |
| Parameter   | Required | parameter name. example, mult1 |
| Start       | Required | Double type. Must be pair with Step attribute and Iteration StepCount attribute. |
| Step        | Required | Double type. Must be pair with Start attribute and Iteration StepCount attribute. |

### SharedStorage Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
| Key         | Required | Name of sharedstorage to modify. |
| Context     | Required | Context type of sharedstorage. LOT or DUT or IP. |
| ResetPolicy | Required | ResetPolicy of sharedstorage. RESET_AT_LOT_START or RESET_AT_DEVICE_START or NEVER_RESET. |
| DataType    | Required | Datatype of sharedstorage to modify. STRING or DOUBLE or INT. |
| Value       | Optional | String type. Cannot use with Start and Step attribute. |
| Start       | Optional | Double type. Must be pair with Step attribute and Iteration StepCount attribute. Cannot use with Value attribute. |
| Step        | Optional | Double type. Must be pair with Start attribute and Iteration StepCount attribute. Cannot use with Value attribute. |

### Pattern Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
| Name                          | Required | name of PatternInfo to use for the pattern modifier. |
| <font color="blue">Pin</font> | Required | Context type of sharedstorage. LOT or DUT or IP. |

#### Pin Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
| Name | Required | name of pin to modify. |
| Data | Required | data to apply on pin. It is expected to match the NumberOfVectors defined at the PatternInfo. |

### UserVar Attribute
|Attribute|Required?|Description|
|---------|---------|-----------|
| Key         | Required | Full name of uservar to modify. example, "_UserVars.KeyA" |
| DataType    | Required | Datatype of uservar to modify. STRING or DOUBLE or INT. |
| Value       | Optional | String type. Cannot use with Start and Step attribute. |
| Start       | Optional | Double type. Must be pair with Step attribute and Iteration StepCount attribute. Cannot use with Value attribute. |
| Step        | Optional | Double type. Must be pair with Start attribute and Iteration StepCount attribute. Cannot use with Value attribute. |

# FAQ
***Search [Intel Stackoverflow](https://stackoverflow.intel.com/) with Tags [Prime](https://stackoverflow.intel.com/posts/tagged/7832) for answered question or Post new question.***
## How to calculate total iteration executed by IPO.
```
Assume,
Loop0 having SharedStorageA modification from 0.0 to 0.2 (3 step)
Loop1 having UservarA modification from 0.0 to 0.2 (3 step)
Loop2 having SharedStorageB modification toggle of True and False (2 step)

   Total Iteration Execute by IPO = #Loop0 Step * #Loop1 Step * #Loop2 Step
                                  = 3 * 3 * 2
                                  = 18
```
```
Assume ExecuteAll mode, No error in applying modification and executing test is return passing result.
Example: Execute TestA and TestB with modify SharedStorageA from 0.0 to 0.2 and UservarA from 0.0 to 0.2 in single iteration, SharedStorageB iteratively modify to True and False.
Translation to loop setup: Loop0 - SharedStorageA and UservarA. Loop1 - SharedstorageB.
Total Iteration = 6, why not 18? because SharedStorageA and UservarA modification is setup to apply at the same iteration.
| Iteration | Modification                                                  | Execute Test                       |
| --------- | -Loop0------------------------------ | -Loop1---------------- | ---------------------------------- |
|0          | SharedStorageA = 0.0, UservarA = 0.0 | SharedStorageB = True  | Execute TestA, then Execute TestB. |
|1          | SharedStorageA = 0.0, UservarA = 0.0 | SharedStorageB = False | Execute TestA, then Execute TestB. |
|2          | SharedStorageA = 0.1, UservarA = 0.1 | SharedStorageB = True  | Execute TestA, then Execute TestB. |
|3          | SharedStorageA = 0.1, UservarA = 0.1 | SharedStorageB = False | Execute TestA, then Execute TestB. |
|4          | SharedStorageA = 0.2, UservarA = 0.2 | SharedStorageB = True  | Execute TestA, then Execute TestB. |
|5          | SharedStorageA = 0.2, UservarA = 0.2 | SharedStorageB = False | Execute TestA, then Execute TestB. |
```
```
Assume ExecuteAll mode, No error in applying modification and executing test is return passing result.
Example: Execute TestA and TestB with modify SharedStorageA from 0.0 to 0.2, UservarA from 0.0 to 0.2, SharedStorageB iteratively modify to True and False.
Translation to loop setup: Loop0 - SharedStorageA, Loop1 - UservarA and Loop2 - SharedStorageB.
Total Iteration = 18
|Iteration | Modification                                                   | Execute Test                       |
|__________|_Loop0________________| Loop1__________|_Loop2__________________|____________________________________|
|0         | SharedStorageA = 0.0 | UservarA = 0.0 | SharedStorageB = True  | Execute TestA, then Execute TestB. |
|1         | SharedStorageA = 0.0 | UservarA = 0.0 | SharedStorageB = False | Execute TestA, then Execute TestB. |
|2         | SharedStorageA = 0.0 | UservarA = 0.1 | SharedStorageB = True  | Execute TestA, then Execute TestB. |
|3         | SharedStorageA = 0.0 | UservarA = 0.1 | SharedStorageB = False | Execute TestA, then Execute TestB. |
|4         | SharedStorageA = 0.0 | UservarA = 0.2 | SharedStorageB = True  | Execute TestA, then Execute TestB. |
|5         | SharedStorageA = 0.0 | UservarA = 0.2 | SharedStorageB = False | Execute TestA, then Execute TestB. |
|6         | SharedStorageA = 0.1 | UservarA = 0.0 | SharedStorageB = True  | Execute TestA, then Execute TestB. |
|7         | SharedStorageA = 0.1 | UservarA = 0.0 | SharedStorageB = False | Execute TestA, then Execute TestB. |
|8         | SharedStorageA = 0.1 | UservarA = 0.1 | SharedStorageB = True  | Execute TestA, then Execute TestB. |
|9         | SharedStorageA = 0.1 | UservarA = 0.1 | SharedStorageB = False | Execute TestA, then Execute TestB. |
|10        | SharedStorageA = 0.1 | UservarA = 0.2 | SharedStorageB = True  | Execute TestA, then Execute TestB. |
|11        | SharedStorageA = 0.1 | UservarA = 0.2 | SharedStorageB = False | Execute TestA, then Execute TestB. |
|12        | SharedStorageA = 0.2 | UservarA = 0.0 | SharedStorageB = True  | Execute TestA, then Execute TestB. |
|13        | SharedStorageA = 0.2 | UservarA = 0.0 | SharedStorageB = False | Execute TestA, then Execute TestB. |
|14        | SharedStorageA = 0.2 | UservarA = 0.1 | SharedStorageB = True  | Execute TestA, then Execute TestB. |
|15        | SharedStorageA = 0.2 | UservarA = 0.1 | SharedStorageB = False | Execute TestA, then Execute TestB. |
|16        | SharedStorageA = 0.2 | UservarA = 0.2 | SharedStorageB = True  | Execute TestA, then Execute TestB. |
|17        | SharedStorageA = 0.2 | UservarA = 0.2 | SharedStorageB = False | Execute TestA, then Execute TestB. |
```

## Why is the sharedstorage value did not change according to the expected value that configure in setup file?

<font color="red">**User need to make sure the modifier SharedStorage Key and DataType is same as SharedStorage Key and DataType refered by user Test.** </font>

Possible is due to UserTest_P1 is referring to SharedStorage Key name SharedStorage.KeyA and DataType INT. But in IPO configuration file, the DataType is set to DOUBLE (mismatch with user test that intended to refer DataType INT). 

## Why ituff test instance name is missing part of the string or Error due to test instance name exceed limit?
```
2_tname_DBERROR
2_strgval_LimitExceeded_Ipo::ModifyFuseBurnSharedStorage_12345_P1_Ipo::DynamicMask_SharedStorage_FuseGroup_DatalogRLE_PatternExecute_VBump_12345_P1_0_0_0
2_tname_Ipo::ModifyFuseBurnSharedStorage_12345_P1_Ipo::DynamicMask_SharedStorage_FuseGroup_DatalogRLE_PatternExecute_VBump_12345_P1_0_0_
2_strgval_1
```

The ituff printing has string limit of 129 character, anything more than that will be chop off. The total character count is included with Ipo loop iteration postfix. 

Example:
| Description | Value |
| -- | -- |
| Name of Module | MyModuleName |
| Name of Test to Execute | MyTestToExecute ... 12345_P1 |
| Name of Test that use IPO Test Method | MyTestThatUseIpoTestMethod ... 12345_P1 |
| IPO loop iteration postfix | < Loop1 Iteration >\_< Loop2 Iteration >\_< Loop3 Iteration > |

****Notes**

1. IPO loop iteration postfix will only works when TestType is set to "INSTANCE".

| Ituff TestInstanceName | # character | ituff printing |
| ---------------- | ----------- | -------------- |
| MyModuleName::MyTestThatUseIpoTestMethod_<font color="red">**1234** </font>_P1_MyModuleName::MyTestToExecute_12345_P1_0_0_0 | 129 | By Test (Test Result) <br> 2_tname_MyModuleName::MyTestToExecute_12345_P1_0_0_0 <br> 2_strgval_<MyTestDatalog>|
| | | By Ipo (Test Exit Port) <br> 2_tname_MyModuleName::MyTestThatUseIpoTestMethod_<font color="red">**1234** </font>_P1_MyModuleName::MyTestToExecute_12345_P1_0_0_0 <br> 2_strgval_1 |
| MyModuleName::MyTestThatUseIpoTestMethod_<font color="red">**12345** </font>_P1_MyModuleName::MyTestToExecute_12345_P1_0_0_0 | 130 | By Test (Test Result) <br> 2_tname_MyModuleName::MyTestToExecute_12345_P1_0_0_0 <br> 2_strgval_<MyTestDatalog>
| | | Datalog Error <br> 2_tname_DBERROR <br> 2_strgval_LimitExceeded_MyModuleName::MyTestThatUseIpoTestMethod_<font color="red">**12345** </font>_P1_MyModuleName::MyTestToExecute_12345_P1_0_0_0 <br> |

Solution 1 : Change Name of Test to Execute to a shorter name.

Solution 2 : Define PreInstance in Ipo Test Instance Parameter to apply overwrite for the printing "2_tname_<overwrite>".
```
Test IpoBasicTest MiniFLow1_TestCondition_P1
{
    PreInstance = "PrimeSetTestNameOverride(SomeName)";
    ModifierType = "TESTCONDITION";
    ExecuteMode = "DATALOG";
    ExpansionType = "INCREMENTAL";
    LogLevel = "Enabled";
}
```
| PreInstance? | Test Name | # character | ituff printing |
| ------------------------- | ---------------------- | ----------- | -------------- |
| Not defined | MyModuleName::MyTestThatUseIpoTestMethod_1234_P1_MyModuleName::MyTestToExecute_12345_P1_0_0_0 | 129 | By Test (Test Result) <br> 2_tname_MyModuleName::MyTestToExecute_12345_P1_0_0_0 <br> 2_strgval_<MyTestDatalog>|
|             |                                                                                                   |     | By Ipo (Test Exit Port) <br> 2_tname_MyModuleName::MyTestThatUseIpoTestMethod_1234_P1_<font color="red">**MyModuleName::MyTestToExecute_12345_P1** </font>_0_0_0 <br> 2_strgval_1 |
| PrimeSetTestNameOverride(SomeName)  | Same as above                                                                                     | 129 | By Test (Test Result) <br> 2_tname_MyModuleName::MyTestToExecute_12345_P1_0_0_0 <br> 2_strgval_<MyTestDatalog>|
|             |                                                                                                   |     | By Ipo (Test Exit Port) <br> 2_tname_MyModuleName::MyTestThatUseIpoTestMethod_1234_P1_<font color="red">**SomeName** </font>_0_0_0 <br> 2_strgval_1 |

## What is the meaning of binary ituff format print by IPO in Default behaviour?
```
Logic
| Test Exit Port Result | Is 1 or CustomPassingPort ? | Print as |
| --------------------- | --------------------------- | -------- |
| 0                     | No                          | 1        |
| 1                     | Yes                         | 0        |
| none 0 and 1          | No                          | 1        |
| none 0 and 1          | Yes                         | 0        | 

Scenario - CustomPassingPort is not provided for the TestsToExecute, but the executed result is all binary.
The IPO executed result is 11110000 (10 iteration executed).
IPO will print to ituff as 00001111.

Scenario - CustomPassingPort is not provided for the TestsToExecute, but the executed result is contain non-binary.
The IPO executed result is 12340000 (10 iteration executed).
IPO will print to ituff as 01111111. **User is expected to define CustomPassingPort for the TestsToExecute to handle exit port that is not 0 and 1.

Scenario - CustomPassingPort (2,3,4) is provided for the TestsToExecute.
The IPO executed result is 12340000 (10 iteration executed).
IPO will print to ituff as 00001111.

**Note: By default, the IPO Extension-CustomDatalog is using Binary format, user can use Usercode to override the default behaviour.
```

# Version Tracking
| Prime Version |Prime ticket reference| Author | Comments |
| ------------- | -------------------- | -----------| ------------ |
| 13.04.00      | #62148 | Teoh, Khai Jie | New ExecutionMode FirstAllInstancePass and FirstAllInstanceFail. New feature StoreIterationResult. |
| 13.03.00      | #58640 | Lee, Yeong Jui | Fix ituff tname IPO loop iteration postfix |
| 13.00.00      | #28833 <br> #28836 <br> #28840 <br> | Teoh, Khai Jie <br> Lee, Yeong Jui | Initial version.|