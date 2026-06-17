# **REP for DeviceStartWaferDatalog**

This **REP** (**prime Test-Method Specification**) is intended to describe the functionality and architecture of DeviceStartWaferDatalog Prime TestMethod.

[[_TOC_]]

# <span id="Methodology" class="anchor"></span> **Methodology**
The purpose of DeviceStartWaferDatalog is to create and log to ituff level 2 and level 3 headers for single die mode. The TM is composed by the main class **PrimeDeviceStartWaferDatalogTestMethod** and two classes assign to each level **Level2** and **Level3** in addition to an interface **IDeviceStartWaferDatalogExtensions** design specifically for the user to override the default headers.

## <p><span style="color:#ff0000"><strong>Ituff Sensitivity Warning</strong></span></p>
<p><span style="color:#ff0000">Do not enable <em>InstanceSummaryMode</em> for this test method instances. It will cause Ituff to be printed out-of-sequence causing failure down the line. A better solution is being worked on.</span></p>

## **Key Aspects**
:large_blue_diamond: Keep in mind that only **one** DeviceStartDatalog instance (`Package`, `SingleDie` or `Wafer`) must be used by TP.

## **Verify**
:large_blue_diamond: Initializes level classes

## **Execute**
:large_blue_diamond: Constructs ituff level 3 package header  
:large_blue_diamond: Constructs ituff level 2 package header  
:large_blue_diamond: Writes ituff level 3 package header  
:large_blue_diamond: Writes ituff level 2 package header

# <span id="Parameters" class="anchor"></span> **Test Instance Parameters**

| **Parameter Name** | **Description** |**Is required** | **Type** | **Supported Values** | **Default Value** |
|------------------|------------------|------------------|------------------|------------------|------------------|

# <span id="Output" class="anchor"></span> **Datalog output**
This section provides all relevant information

## <span style="color:DarkSeaGreen"> **Ituff level 3** </span> 
The level 3 header for package mode contains the following information

3_lbeg  
3_lsep  
3_xloc_`{waferXLocation}`  
3_yloc_`{waferYLocation}`  
3_dvtststdt_`{deviceStartTime}`  
3_socket_`{socket}`  
3_siteid_`{siteId}`  
3_prttesterid_`{testerId}`

| **Associated Token** | **Obtained from** | **Conditions** | **Example** |
|------------------|------------------|------------------|------------------|
| lbeg | Start of level | Always log at start of level 3 header | 3_lbeg |
| lsep | Level separator | | 3_lsep |
| xloc | Value read from station controller uservar `EI_X_COORDINATE` | If either `EI_X_COORDINATE` or `EI_Y_COORDINATE` is empty, no token will be logged to ituff | 3_xloc_1 |
| yloc | Value read from station controller uservar `EI_Y_COORDINATE` | If either `EI_X_COORDINATE` or `EI_Y_COORDINATE` is empty, no token will be logged to ituff | 3_yloc_-1 |
| dvtststdt | Snapshot of the current device start time | | 3_dvtststdt_20211103205839 |
| socket | Value is obtained from TestProgramService method GetCurrentDutId | | 3_socket_1 |
| siteid | Value is read from station controller uservar `SC_SITEID` | If value is empty, no token will be logged to ituff | 3_siteid_A301 |
| prttesterid | Value is read from station controller uservar `SC_PRTTESTERID` | If value is empty, no token will be logged to ituff | 3_prttesterid_C HXT T62312 |

## <span style="color:DarkSeaGreen"> **Ituff level 2** </span> 
The level 2 header for package mode contains the following information

2_lbeg  
2_lsep  
2_tname_tseq  
2_mrslt_`{testSequence}`  
2_lsep  
2_tname_tdnseq  
2_mrslt_`{testEndSequence}`  
2_lsep  
2_tname_delay_index_time  
2_mrslt_`{delayIndexTime}`  
2_msunit_S

| **Associated Token** | **Obtained from** | **Conditions** | **Example** |
|------------------|------------------|------------------|------------------|
| lbeg | Start of level | Always log at start of level 2 header | 2_lbeg |
| lsep | Level separator | | 2_lsep |
| tseq | Value is obtained from TestProgramService method IncrementUnitCountForSite | | 2_tname_tseq <br/> 2_mrslt_1 |
| tdnseq | Value is read from station controller uservar `SC_TDNSEQ` | If uservar contains a value that cannot be converted to an integer, the Test Method will print an error to the console. If integer value contained in uservar is **not** greater than 0, printing will be skipped from ituff | 2_tname_tdnseq <br/>2_mrslt_1 |
| delay_index_time | Time span between the end of previous unit (Obtained from SharedStorage under key `DEVICE_END_TIME`) and current time (Obtained from snapshot) | Logged only from second unit forward | 2_tname_delay_index_time <br/> 2_mrslt_25.0828 <br/> 2_msunit_S |

# <span id="Hooks" class="anchor"></span> **Custom User Code Hooks**
DeviceStartWaferDatalog Test Method support the following extensions

```csharp
    string ModifyLevel3Header(string formattedHeader);
```

This extension is called at the end of the Test Method's execute to provide the user a way to modify the level 3 header before logging to ituff. This extension recieves as a parameter the complete level 3 package header. The default implementation will return the formatted header created by the Test Method.

```csharp
    string ModifyLevel2Header(string formattedHeader);
```

This extension is called at the end of the Test Method's execute to provide the user a way to modify the level 2 header before logging to ituff. This extension recieves as a parameter the complete level 2 package header. The default implementation will return the formatted header created by the Test Method.

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note** </span></font> :warning:
> Keep in mind that by using these extensions do not verify or format the ituff header in any way. By using these, user owns responsibility of what will be logged into the desired output file.

# <span id="TPL" class="anchor"></span> **TPL Samples**
In order to create a new instance of DeviceStartWaferDatalog the following test needs to be added to the tpl file. Example shows a test with all instance parameters, including the optional ones.

```python
    import PrimeDeviceStartWaferDatalogTestMethod.xml;

    Test PrimeDeviceStartWaferDatalogTestMethod DeviceStartWaferDatalogTestMethod_P1 {
        LogLevel = "PRIME_DEBUG";
    }
```

# <span id="FlowOrder" class="anchor"></span> **Test Program Flow order requirements**

The image below ilustrates the order requiremetns for the Evergreen and Prime instances in order to generate correctly the ITUFF <font size="3"><span style="color:OrangeRed"> Device </font> testing session <font size="3"><span style="color:OrangeRed">Header</font>.

1. PrimeDeviceStartSetupTestMethod  
1. PrimeDeviceStart<font size="3"><span style="color:OrangeRed">[***]</font>DatalogTestMethod
1. iCBkndTest (in StartOfDevice Mode) 

::: mermaid
	graph LR 
	classDef EVG fill:#708541;
	classDef PRIME fill:#005B85;
	classDef TOS fill:#B24501;
		subgraph Device Start Subflow
			direction LR
			id0("HMI_TestPlanSetup "):::TOS --> 
			id1(PrimeDeviceStartSetupTestMethod ):::PRIME -->
			id2("PrimeDeviceStart[***]DatalogTestMethod"):::PRIME -->
			id3(iCBkndTest):::EVG
		end
:::

>Note: depending on your socket type (package, single-die or wafer) please use the corresponding device start datalog test method.
> * PrimeDeviceStart<font size="3"><span style="color:OrangeRed">Package</font>DatalogTestMethod
> * PrimeDeviceStart<font size="3"><span style="color:OrangeRed">SingleDie</font>DatalogTestMethod
> * PrimeDeviceStart<font size="3"><span style="color:OrangeRed">Wafer</font>DatalogTestMethod  

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note** </span></font> :warning:
> Please be aware that the PrimeDeviceStart<font size="3"><span style="color:OrangeRed">[***]</font>DatalogTestMethod instance MUST be after a PrimeDeviceStartSetupTestMethod instance. Otherwise the TP will crash due to initialization errors.

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note** </span></font> :warning:
>For mDUT usage the HMI_TestPlanSetup instance must be executed; this HMI instance will flush all the datalog buffers for each DUT avoiding data interleaving.
>
>HMI_TestPlanSetup (OperationMode = TESTPLAN_START) instance must be before the PrimeDeviceStartSetupTestMethod instance.

# <span id="ExitPorts" class="anchor"></span> **Exit Ports**

The DeviceStartForPackage Test Method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |

# <span id="Dependencies" class="anchor"></span> **Additional Dependencies**

This test method has a dependency with all Lot TMs from PRIME. The use of DeviceStartWaferDatalog without Lot TMs is not a supported setup, **any attempt to not follow this rule can cause errors**. It is also intended to be used after PRIME's DeviceStartSetup TM.

# <span id="VersionTracking" class="anchor"></span> **Version tracking**

| **Date**       | **Version** | **Author**     | **Comments** |
| -------------- | ----------- | -------------- | ------------ |
| Mar 10, 2022 | 1.0.0 | mjhernan | DeviceStartForPackage TM added |
| Apr 18, 2022 | 1.0.1 | mjhernan | Fixing tables format |
| Aug 12, 2022 | 1.0.2 | hramirez | Adding Flow order documentation |

# <span id="Acronyms" class="anchor"></span> **Acronyms**

Definition of acronyms used in this document:

  - **REP**: Prime Trst-Method Specification