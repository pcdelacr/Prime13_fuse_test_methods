# **REP for DeviceEndDatalog**

This **REP** (**Prime Test-Method Specification**) is intended to describe the functionality and architecture of DeviceEndDatalog Prime Test Method.

[[_TOC_]]

# <span id="Methodology" class="anchor"></span> **Methodology**
The purpose of `DeviceEndDatalog` is to create and log to ituff level 2 and level 3 footers for any mode (package, wafer or single die). 
The Test Method consists of the main class `PrimeDeviceEndDatalogTestMethod` and two classes, `Level2` and `Level3`each
assigned to a specific level. Additionally, it includes an interface `IDeviceEndExtensions` designed for users to override
the default footers.

## <p><span style="color:#ff0000"><strong>Ituff Sensitivity Warning</strong></span></p>
<p><span style="color:#ff0000">Do not enable <em>InstanceSummaryMode</em> for this test method instances. It will cause Ituff to be printed out-of-sequence causing failure down the line. A better solution is being worked on.</span></p>


## **Verify**
:large_blue_diamond: Initializes level classes

## **Execute**
:large_blue_diamond: Constructs ituff level 3 footer  
:large_blue_diamond: Constructs ituff level 2 footer  
:large_blue_diamond: Writes ituff level 3 footer  
:large_blue_diamond: Writes ituff level 2 footer  

# <span id="Parameters" class="anchor"></span> **Test Instance Parameters**

| **Parameter Name** | **Required?** | **Type**         | **Values**                                        | **Default Value** | **Comments**                                                                                                                                                                                                                                                                                                                                                           |
|--------------------|---------------|------------------|---------------------------------------------------|-------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| ProcessOlbTokens   | Optional      | String (choice)  | Enabled, Disabled                                 | Enabled           | Specifies whether the test instance should process the OLB tokens.                                                                                                                                                                                                                                                                                                     |
| TbinOverrideKey    | Optional      | String           | Shared storage key containing a 7 or 8 digit bin. | Empty string.     | The bin is used to override the final bin in ITUFF when TBIN is enabled, or when the TOS data bin is a good bin, indicating that TBIN determined the die was good (in this case, the value written to TOS is a concatenation of all the good IBs). DeviceEndDatalog does not verify if the bin exists in the `.bdefs` file. This feature is based on Sort methodology. |

# <span id="Output" class="anchor"></span> **Datalog output**
This section provides all the relevant information for the output datalog that conforms the level 2 and 3 footer.

## <span style="color:DarkSeaGreen"> **Ituff level 2** </span> 
The level 2 footer contains the following information:

2_tname_hwrt  
2_mrslt_`{hardwareRetest}`  
2_tname_hwrt_orig_curfbin  
2_mrslt_`{hardwareRetestCurfbin}`  
2_tname_hwrt_orig_curibin  
2_mrslt_`{hardwareRetestCuribin}`  
2_tname_Memory_TestProcessHost_Bytes  
2_mrslt_`{testProcessHost}`  
2_tname_Memory_HdmtAppProcess_Bytes  
2_mrslt_`{hdmtAppProcess}`  
2_lend

| **Associated Token** | **Obtained from** | **Conditions** | **Example** |
|------------------|------------------|------------------|------------------|
| hwrt | Value read from station controller uservar `EI_HWRT_RETEST_NUM` | If uservar does not exist or is empty, token will be logged with no associated value | 2_tname_hwrt <br/> 2_mrslt_2 |
| hwrt_orig_curfbin | Value read from station controller uservar `EI_HWRT_ORG_SOFT_BIN` | If uservar does not exist or is empty, token will be logged with no associated value | 2_tname_hwrt_orig_curfbin <br/> 2_mrslt_9722 |
| hwrt_orig_curibin | Value read from station controller uservar `EI_HWRT_ORG_HARD_BIN` | If uservar does not exist or is empty, token will be logged with no associated value | 2_tname_hwrt_orig_curibin <br/> 2_mrslt_97 |
| Memory_TestProcessHost_Bytes | Value read from windows process  | If process does not exist or is empty, a message "The TestProcessHost was not found." will be displayed. | 2_tname_Memory_TestProcessHost_Bytes <br/> 2_mrslt_464457728 |
| HdmtAppProcess | Value read from windows process | If process does not exist or is empty, a message "The HdmtApp process was not found." will be displayed. | 2_tname_Memory_HdmtAppProcess <br/> 2_mrslt_263225344 |

## <span style="color:DarkSeaGreen"> **Ituff level 3** </span> 
The level 3 footer contains the following information:

This level provides the capability to use the BMFC binning information.

3_dvtsteddt_`{deviceStartTime}`  
3_ttime_`{testTime}`  
3_tpass_{`{passCounters}`}  
3_tfail_{`{failCounters}`}  
3_subflstpid_`{processType}`  
3_binn_`{finalBin}`  
3_comnt_SDSIB_`{sdsib}`  
3_comnt_SDSFB_`{sdsfb}`  
3_comnt_SDTIB_`{sdtib}`  
3_comnt_SDTFB_`{sdtfb}`  
3_curdtbin_`{curdtbin}`  
3_curftbin_`{curftbin}`  
3_curitbin_`{curitbin}`  
3_curfbin_`{curfbin}`  
3_curibin_`{curibin}`  
3_sscurfbin_`{bin}`_`{value}`  
3_trslt_pass  
3_trslt_fail  
3_binpinfails_`{FailPinInfo}`  
3_lend

| **Associated Token** | **Obtained from**                                                                                                                                                                                      | **Conditions** | **Example** |
|------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------|------------------|
| dvtsteddt | Snapshot of the current device end time                                                                                                                                                                | | 3_dvtsteddt_20211103235045 |
| ttime | Timespan between the current device start time and the current device end time                                                                                                                         | | 3_ttime_156.1907 |
| tpass | Value is obtained from BinCounterService method `GetPassCountersForUnit`                                                                                                                               | | 3_tpass_{79210031,99141486,99141488} |
| tfail | Value is obtained from 3 different sources <br/> * BinCounterService method `GetFailCountersForUnit` <br/> * ScoreboardService method `GetAllCounters` <br/> * ScoreboardService `GetExternalCounters` | Only if Hybrid mode is active will external counters be appended to the resulting string <br/> In hybrid mode, a unique list of counters will be appended to the resulting string | 3_tfail_{27020200,27380008} |
| subflstpid | Value read from station controller uservar `SC_CURRENT_PROCESS_TYPE`                                                                                                                                   | If uservar does not exist or is empty, token will be logged with no associated value <br/> Token will only be processed whenever uservar `SC_HCSLMODE` is set to TRUE and uservar `SC_TEST_FLOW` is set to SORT | 2_subflstpid_CLASSHOT |
| binn | Value obtained from BinCounterService method `GetCurrentDataBin`                                                                                                                                       | | 2_binn_79210000 |
| SDSIB | Value is read from sharedStorage integer table with key `MERGED_SDS_IB`                                                                                                                                | Default value is 69 unless otherwise indicated. Logged only when uservar `RunTimeLibraryVars.iCGL_EnableSdsSdtMergeSocketBining` is set to TRUE and whenever test flow is SingleDie, meaning station controller uservar `SC_TEST_FLOW` is set to one of the following values SDTSORT, SDSORT or SDSSORT | 3_comnt_SDSIB_1 |
| SDSFB | Value is read from sharedStorage integer table with key `MERGED_SDS_FB`                                                                                                                                | Default value is 6901 unless otherwise indicated. Logged only when uservar `RunTimeLibraryVars.iCGL_EnableSdsSdtMergeSocketBining` is set to TRUE and whenever test flow is SingleDie, meaning station controller uservar `SC_TEST_FLOW` is set to one of the following values SDTSORT, SDSORT or SDSSORT | 3_comnt_SDSFB_101 |
| SDTIB | Value is read from sharedStorage integer table with key `MERGED_SDT_IB`                                                                                                                                | Default value is 69 unless otherwise indicated. Logged only when uservar `RunTimeLibraryVars.iCGL_EnableSdsSdtMergeSocketBining` is set to TRUE and whenever test flow is SingleDie, meaning station controller uservar `SC_TEST_FLOW` is set to one of the following values SDTSORT, SDSORT or SDSSORT | 3_comnt_SDTIB_7 |
| SDTFB | Value is read from sharedStorage integer table with key `MERGED_SDT_FB`                                                                                                                                | Default value is 6901 unless otherwise indicated. Logged only when uservar `RunTimeLibraryVars.iCGL_EnableSdsSdtMergeSocketBining` is set to TRUE and whenever test flow is SingleDie, meaning station controller uservar `SC_TEST_FLOW` is set to one of the following values SDTSORT, SDSORT or SDSSORT | 3_comnt_SDTFB_701 |
| curdtbin | Value is read from sharedStorage integer table with key `NG_DTBIN`                                                                                                                                     | Default value is 6901 unless otherwise indicated. Logged only when uservar `RunTimeLibraryVars.iCGL_EnableSdsSdtMergeSocketBining` is set to TRUE and whenever test flow is SingleDie, meaning station controller uservar `SC_TEST_FLOW` is set to one of the following values SDTSORT, SDSORT or SDSSORT | 3_curdtbin_989 |
| curftbin | Value is read from sharedStorage integer table with key `NG_FTBIN`                                                                                                                                     | Default value is 69 unless otherwise indicated. Logged only when uservar `RunTimeLibraryVars.iCGL_EnableSdsSdtMergeSocketBining` is set to TRUE and whenever test flow is SingleDie, meaning station controller uservar `SC_TEST_FLOW` is set to one of the following values SDTSORT, SDSORT or SDSSORT | 3_curftbin_9899 |
| curitbin | Value is read from sharedStorage integer table with key `NG_ITBIN`                                                                                                                                     | Default value is 6901 unless otherwise indicated. Logged only when uservar `RunTimeLibraryVars.iCGL_EnableSdsSdtMergeSocketBining` is set to TRUE and whenever test flow is SingleDie, meaning station controller uservar `SC_TEST_FLOW` is set to one of the following values SDTSORT, SDSORT or SDSSORT | 3_curftbin_98 |
| curfbin | Value obtained from BinCounterService method `GetCurrentSoftBin`                                                                                                                                       | | 3_curfbin_2100 |
| curibin | Value obtained from BinCounterService method `GetCurrentHardBin`                                                                                                                                       | | 3_curibin_1 |
| sscurfbin | Value to be obtained from DieService (WIP)                                                                                                                                                             | | |
| trslt | | If errorElements token is empty and current hard bin contains a value between 0 and 7 the DUT status is pass. Else, DUT status is fail | 3_trslt_pass |

### Ituff level 3 when TBIN is enabled
When TBIN is enabled (the final bin is 9 digits long) or the final bin is a good bin and the `TbinOverrideKey` parameter is set, 
the final bin, functional bin, and interface bin in Ituff will be overridden using the value stored in the shared storage key specified in the parameter.  

For example:
- Final bin from BinCounterService = 221740001
- TbinOverrideKey = "ValidBin"
- "ValidBin" value in shared storage = 74630001
- Ituff result for the following tokens will be:
  - 3_binn_74630001
  - 3_curfbin_7463
  - 3_curibin_74

If TBIN is not enabled the Ituff result will be the same as the default behavior, even if the `TbinOverrideKey`
parameter is set.  
If TBIN is enabled or the final bin is a good bin, but the `TbinOverrideKey` parameter is not set, the Ituff result will be the same as the default
behavior.  
If TBIN is enabled or the final bin is a good bin, and the `TbinOverrideKey` contains a value that is either less than 7 digits or more than 8 digits long, the
test instance is expected to fail.

## Functional bin

 To be in sync with OdeseBinConverterTM the same capability can be found in this TM for the final Functional bin calculation. Referencing how OdeseBinConverterTM works, in case the length of the bin is greater than 4, the method can take the indexes described on the uservars:

 - `RunTimeLibraryVars.iCGL_PrimeSimplifiedBinningIndexesForPassingBin`
 - `RunTimeLibraryVars.iCGL_PrimeSimplifiedBinningIndexesForFailingBin`

to create a 4-digit bin. For example:

| **8-Digit SoftBin**|**4-Digits Softbin**|**Uservar**|
|--------------------|------------------ |-------|
| 90440012|4400|RunTimeLibraryVars.iCGL_PrimeSimplifiedBinningIndexesForFailingBin = "2,3,4,5"|
| 90620012|9200|RunTimeLibraryVars.iCGL_PrimeSimplifiedBinningIndexesForFailingBin = "0,3,4,5"|
| 90621212|6200|RunTimeLibraryVars.iCGL_PrimeSimplifiedBinningIndexesForFailingBin = "2,3,4,5"|
| 10010100| 101|RunTimeLibraryVars.iCGL_PrimeSimplifiedBinningIndexesForPassingBin = "2,3,4,5"|

Given the case that none of uservars exists, the methods create a 4-digit bin using the default values. Default binning format is the 4 middle numbers of an 8 digit bin.

### Example of default test method behavior (not using ODESE):
| **8-Digit SoftBin**|**4-Digits Softbin**|
|--------------------|------------------ |
| 90440012|4400|

<span style="color: #FF5662">Notes:</span>

[1] This Index count is from 0 to 7.  
[2] User MUST specify 4 indexes on each uservar.  
[3] This binning logic will also affect the SubStructureFunctionalBin obtained from `DieIdBinningService` in similar fashion.  
[4] If the current bin is a reserved bin by TOS or the parsing of the indexes fail to return an int number the default 4 digit softbin used will be **9890**.


# <span id="Hooks" class="anchor"></span> **Custom User Code Hooks**
The `PrimeDeviceEndDatalogTestMethod` supports the following extensions.

```csharp
    string ModifyLevel3Header(string formattedHeader);
```

This extension is invoked at the end of the Test Method's execute, allowing the user to modify the level 3 header
before it is logged to ituff. It receives the complete level 3 package header as a parameter. The default implementation
returns the formatted header created by the Test Method.

```csharp
    string ModifyLevel2Header(string formattedHeader);
```

This extension is invoked at the end of the Test Method's execute, allowing the user to modify the level 2 header
before it is logged to ituff. It receives the complete level 2 package header as a parameter. The default implementation
returns the formatted header created by the Test Method.

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note** </span></font> :warning:
> Please note that these extensions do not verify or format the ituff header in any way. By using these extensions, the
user assumes responsibility for the content that will be logged into the output file.

# <span id="TPL" class="anchor"></span> **TPL Samples**
Example shows a test with all instance parameters, including the optional ones.

```python
    CSharpTest PrimeDeviceEndDatalogTestMethod DeviceEndDatalog_P1 {
        ProcessOlbTokens = Disabled;
        TbinOverrideKey = "ValidBinKey";
        LogLevel = "Enabled";
    }
```

# <span id="FlowOrder" class="anchor"></span> **Test Program Flow Order Requirements**

The image below illustrates the required order for Prime instances to correctly generate the ITUFF <font size="3"><span style="color:OrangeRed"> Device </font>
testing session <font size="3"><span style="color:OrangeRed">Footer</font>.

1. PrimeDeviceEndDatalogTestMethod 
1. PrimeOdeseBinConverterTestMethod
1. PrimeDeviceEndFinalizeTestMethod
1. HMI_TestPlanSetup (in OperationMode = TESTPLAN_END) (<font size="3"><span style="color:OrangeRed">TOS3 Only</font>)

::: mermaid
	graph LR
	classDef PRIME fill:#005B85;
	classDef TOS fill:#B24501;
		subgraph Device End Subflow
			direction LR         
			id2(PrimeDeviceEndDatalogTestMethod ):::PRIME --> 
			id3(**PrimeOdeseBinConverterTestMethod**):::PRIME -->
			id4(PrimeDeviceEndFinalizeTestMethod):::PRIME -->
			id5("HMI_TestPlanSetup (TOS3 Only)"):::TOS
		end
:::

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note for TOS3** </span></font> :warning:
>For mDUT usage the HMI_TestPlanSetup instance must be executed; this HMI instance will flush all the datalog buffers for each DUT avoiding data interleaving.
>
>HMI_TestPlanSetup (OperationMode = TESTPLAN_END) instance must be after the PrimeDeviceEndFinalizeTestMethod instance.

# <span id="ExitPorts" class="anchor"></span> **Exit Ports**

The `PrimeDeviceEndDatalogTestMethod` supports the following exit ports:

| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |

# <span id="Dependencies" class="anchor"></span> **Additional Dependencies**

This test method has a dependency with all Lot Test Methods from PRIME. 
The use of the `PrimeDeviceEndDatalogTestMethod` without Lot TMs is not a supported setup, **any attempt to not follow this rule can cause errors**. 
It is also intended to be used after PRIME DeviceStart TMs.

# <span id="VersionTracking" class="anchor"></span> **Version tracking**

| **Date**      | **Version** | **Author** | **Comments**                                                                                                        |
|---------------|-------------|------------|---------------------------------------------------------------------------------------------------------------------|
| Mar 10, 2022  | 1.0.0       | mjhernan   | DeviceEndDatalog TM added                                                                                           |
| Apr 18, 2022  | 1.0.1       | mjhernan   | Fixing tables format                                                                                                |
| Jun 29, 2022  | 1.1.0       | mjhernan   | Removing finalizations from TM                                                                                      |
| Jul 6, 2022   | 1.1.1       | mjhernan   | Adding capability to log BMFC information                                                                           |
| Aug 12, 2022  | 1.1.2       | hramirez   | Adding Flow order documentation                                                                                     |
| Sept 13, 2023 | 12.02.02    | khaijiet   | Print FailPinInfo in SharedStorage.<br> #40929 <br>That stored by test method.<br> #39827                           |
| Nov 3, 2023   | 12.03.00    | khaijiet   | Turn off printing of FailPinInfo (Witch Project 3_ token) until sort tp and ituff upload infra is ready.<br> #45199 |
| Apr 11, 2024  | 13.01.00    | jabarran   | Changing default bin used when failing SoftBinConverter logic.<br> #50045                                           |
| Aug 13, 2024  | 13.01.00    | ckhoh      | Update Flow order documentation                                                                                     |
| Feb 14, 2025  | 13.02.00    | rppintor   | Adding support to override final bin when TBIN is enabled                                                                                           |


# <span id="Acronyms" class="anchor"></span> **Acronyms**

Definition of acronyms used in this document:

  - **REP**: Prime Test-Method Specification
  - **BMFC**: Bin Matrix Flow Control
  - **TBIN**: Total Bin