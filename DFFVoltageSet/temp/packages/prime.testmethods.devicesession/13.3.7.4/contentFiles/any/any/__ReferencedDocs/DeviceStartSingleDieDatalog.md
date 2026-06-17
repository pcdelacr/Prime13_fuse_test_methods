# **REP for DeviceStartSingleDieDatalog**

This **REP** (**prime Test-Method Specification**) is intended to describe the functionality and architecture of DeviceStartSingleDieDatalog Prime TestMethod.

[[_TOC_]]

# <span id="Methodology" class="anchor"></span> **Methodology**
The purpose of DeviceStartSingleDieDatalog is to create and log to ituff level 2 and level 3 headers for single die mode. The TM is composed by the main class **PrimeStartSingleDieDatalogTestMethod** and two classes assign to each level **Level2** and **Level3** in addition to an interface **IStartSingleDieDatalogExtensions** design specifically for the user to override the default headers.

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
3_wafxloc_`{waferXLocation}`  
3_wafyloc_`{waferYLocation}`  
3_dvtststdt_`{deviceStartTime}`  
3_partialwafid_`{partialWaferId}`  
3_carrierid_`{carrierId}`  
3_carrierxloc_`{carrierXLocation}`  
3_carrieryloc_`{carrierYLocation}`  
3_siteid_`{siteId}`  
3_cellid_`{cellId}`  
3_prttesterid_`{testerId}`  
3_chuckid_`{chuckId}`  
3_thermalhdid_`{thermalHeadId}`  
3_tiuid_`{tiuId}`

| **Associated Token** | **Obtained from** | **Conditions** | **Example** |
|------------------|------------------|------------------|------------------|
| lbeg | Start of level | Always log at start of level 3 header | 3_lbeg |
| lsep | Level separator | | 3_lsep |
| wafxloc | Value is read from station controller uservar `EI_X_COORDINATE` | If either `EI_X_COORDINATE` or `EI_Y_COORDINATE` is empty, no token will be logged to ituff | 3_wafxloc_1 |
| wafyloc | Value is read from station controller uservar `EI_Y_COORDINATE` | If either `EI_X_COORDINATE` or `EI_Y_COORDINATE` is empty, no token will be logged to ituff | 3_wafyloc |
| dvtststdt | Snapshot of the current device start time | | 3_dvtststdt_20211103205839 |
| partialwafid | Value is read from station controller uservar `SC_PARTIALWAFID` | If value is empty, no token will be logged to ituff | 3_partialwafid_W123 |
| carrierid | Value is read from station controller uservar `SC_TRAYID` | If value is empty, no token will be logged to ituff | 3_carrierid_T123 |
| carrierxloc | Value is read from station controller uservar `SC_TRAYX` | If either `SC_TRAYX` or `SC_TRAYY` is empty, no token will be logged to ituff | 3_carrierxloc_1 | 
| carrieryloc | Value is read from station controller uservar `SC_TRAYY` | If either `SC_TRAYX` or `SC_TRAYY` is empty, no token will be logged to ituff | 3_carrieryloc_2 |
| siteid | Value is read from station controller uservar `SC_SITEID` | If value is empty, no token will be logged to ituff | 3_siteid_A301 |
| cellid | Value is read from station controller uservar `SC_CELLID` | If value is empty, no token will be logged to ituff | 3_cellid_SDC123 |
| prttesterid | Value is read from station controller uservar `SC_PRTTESTERID` | If value is empty, no token will be logged to ituff | 3_prttesterid_C HXT T62312 |
| chuckid | Value is read from station controller uservar `SC_CHUCKID_INFO` | If value is empty, no token will be logged to ituff | 3_chuckid_1 |
| thermalhdid | Value is read from station controller uservar `SC_THERMALHDID` | If value is empty, no token will be logged to ituff | 3_thermalhdid_65433 |
| tiuid | Value is obtained from TestProgramService method GetTiuName | | 3_tiuid_TID_TIUABCDE123_A201 |

## <span style="color:DarkSeaGreen"> **Ituff level 2** </span> 
The level 2 header for package mode contains the following information

2_lbeg  
2_lsep  
2_tname_tseq  
2_mrslt_`{testSequence}`  
2_tname_tdnseq  
2_mrslt_`{testEndSequence}`  
2_tname_delay_index_time  
2_mrslt_`{delayIndexTime}`  
2_msunit_S

| **Associated Token** | **Obtained from** | **Conditions** | **Example** |
|------------------|------------------|------------------|------------------|
| lbeg | Start of level | Always log at start of level 2 header | 2_lbeg |
| lsep | Level separator | | 2_lsep |
| tseq | Value is obtained from TestProgramService method IncrementUnitCountForSite | | 2_tname_tseq <br/> 2_mrslt_1 |
| tdnseq | Value is read from station controller uservar `SC_TDNSEQ` | If uservar contains a value that cannot be converted to an integer, the Test Method will print an error to the console. If integer value contained in uservar is **not** greater than 0, printing will be skipped from ituff | 2_tname_tdnseq <br/> 2_mrslt_1 |
| delay_index_time | Time span between the end of previous unit (Obtained from SharedStorage under key `DEVICE_END_TIME`) and current time (Obtained from snapshot) | Logged only from second unit forward | 2_tname_delay_index_time <br/> 2_mrslt_25.0828 <br/> 2_msunit_S <br/> |

# <span id="Hooks" class="anchor"></span> **Custom User Code Hooks**
DeviceStartSingleDieDatalog Test Method support the following extensions

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
In order to create a new instance of DeviceStartSingleDieDatalog the following test needs to be added to the tpl file. Example shows a test with all instance parameters, including the optional ones.

```python
    import PrimeDeviceStartSingleDieDatalogTestMethod.xml;

    Test PrimeDeviceStartSingleDieDatalogTestMethod SingleDieDatalogTestMethod_P1 {
        LogLevel = "PRIME_DEBUG";
    }
```

# <span id="FlowOrder" class="anchor"></span> **Test Program Flow order requirements**

The image below ilustrates the order requiremetns for Prime instances in order to generate correctly the ITUFF <font size="3"><span style="color:OrangeRed"> Device </font> testing session <font size="3"><span style="color:OrangeRed">Header</font>.

1. PrimeDeviceStartSetupTestMethod  
1. PrimeDeviceStart<font size="3"><span style="color:OrangeRed">[***]</font>DatalogTestMethod

::: mermaid
	graph LR 
	classDef PRIME fill:#005B85;
	classDef TOS fill:#B24501;
		subgraph Device Start Subflow
			direction LR
			id0("HMI_TestPlanSetup (TOS3 Only)"):::TOS --> 
			id1(PrimeDeviceStartSetupTestMethod ):::PRIME -->
			id2("PrimeDeviceStart[***]DatalogTestMethod"):::PRIME
		end
:::

>Note: depending on your socket type (package, single-die or wafer) please use the corresponding device start datalog test method.
> * PrimeDeviceStart<font size="3"><span style="color:OrangeRed">Package</font>DatalogTestMethod
> * PrimeDeviceStart<font size="3"><span style="color:OrangeRed">SingleDie</font>DatalogTestMethod
> * PrimeDeviceStart<font size="3"><span style="color:OrangeRed">Wafer</font>DatalogTestMethod  

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note** </span></font> :warning:
> Please be aware that the PrimeDeviceStart<font size="3"><span style="color:OrangeRed">[***]</font>DatalogTestMethod instance MUST be after a PrimeDeviceStartSetupTestMethod instance. Otherwise the TP will crash due to initialization errors.

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note for TOS3** </span></font> :warning:
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

This test method has a dependency with all Lot TMs from PRIME. The use of DeviceStartSingleDieDatalogTestMethod without Lot TMs is not a supported setup, **any attempt to not follow this rule can cause errors**. It is also intended to be used after PRIME's DeviceStartSetup TM.

# <span id="VersionTracking" class="anchor"></span> **Version tracking**

| **Date**       | **Version** | **Author**     | **Comments** |
| -------------- | ----------- | -------------- | ------------ |
| Mar 10, 2022 | 1.0.0 | mjhernan | DeviceStartForPackage TM added |
| Apr 18, 2022 | 1.0.1 | mjhernan | Fixing tables format |
| Aug 12, 2022 | 1.0.2 | hramirez | Adding Flow order documentation |
| Aug 13, 2024 | 13.01.00 | ckhoh    | Update Flow order documentation |

# <span id="Acronyms" class="anchor"></span> **Acronyms**

Definition of acronyms used in this document:

  - **REP**: Prime Test-Method Specification