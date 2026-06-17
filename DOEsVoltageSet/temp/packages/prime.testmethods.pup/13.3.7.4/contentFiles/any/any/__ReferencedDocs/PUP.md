[[_TOC_]]

## REP for PUP Test method

This **REP** is intended to describe the PUP Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Datalog output** – A detailed description of what is datalogged by his TestMethod

  - **Custom User Code hooks** – A list of functions available to the user code to override

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document 
## Methodology

PUP - Per Unit Plist - is a methodology developed to improve [TTR](#Acronyms) to [DPM](#Acronyms) ratio (currently supported by ConTTR methodology in Evergreen). This is achieved by defining for triggered plists reduced content (slim) version. Slim plist version will created from the full one by excluding the patterns which do not fail a content (redundant patterns). This methodology is based on machine learning model developed by Advanced Analytics team (AA) which recommends which instances need to run a plist with a full content and which at reduced (slim). 
In order to make sure that slim plist version does not exclude by mistake patterns which do fail a content -  some units/instances will be executed in [TTRM](#Acronyms) (monitor) mode - i.e running it  in full mode , but checking if patterns which failed during the execution do not appear in the slim plist version. In case they are this unit should be binned by the test program as Bin37 (TTRM escape bin)

More details about the methodology can be found in PupService SDK

The PUP test method gives user an ability to set PUP related settings from the test instance parameters.
Those settings will be populated during Verify phase. During TP's main flow execution PUP data base aggregates the methodology related data. For example which instance executed slim plist ,  which monitor etc. During PUP instance execute phase this data is printed to Ituff as described in Ituff printouts section.

<span style="color: GreenYellow">**PUP test instance should be placed in the TP's TestPlanEndFlow before EVG's [EOD](#Acronyms) (iCBkgndTest) instance is executed.**</span>


## Test Instance Parameters

The table below lists and describes the test instance parameters supported by this test method

| **Parameter Name** | **Required?** | **Type**        | **Values**             | **Comments**                                                                        |
|--------------------|---------------|-----------------|------------------------|-------------------------------------------------------------------------------------|
| PupDebugMode       | No            | String (choice) | Off(**default**) , On  | Enables or disables PUP specific debug messages.             |
| Mode               | No            | String (choice) | Off(**default**)  , On | Off - PUP is disabled, On- PUP is enabled.                  |
| MonitorLoopNum     | No            | Integer         | 1 (**default**)        | Number of loops for plist re-execution in Monitor algorithm.  |   |
| PatternsFilePath     | No            | String         | Empty (**default**)        | Patterns json file path. Usually defined relative to PUP_PATTERNS_DIR env file token where root folder path is defined  |   |
| EFuseUserFormat | No | String |  | EFuse format provided by user vars, where every user var name should be between **{ }**. Eg, "{SCVars.SC_CurrentIdUser}_{SCVars.SC_CurrentIdPositions}". Otherwise EFuse will be selected using Data Feed Forward (DFF). |
| PupMatchMode | No | String |  | Defines mode by which PUP will match the current die / unit. Supported values - VID_ONLY -  match by comparing the current unit id (visual ID) with the ones specified in PUP.json record , EFUSE_ONLY - match by comparing the current eFuse with the ones specified in PUP.json record, VID_AND_EFUSE -  compares both unit visual ID and die eFuse ,  both need to be present in PUP.json file, EFUSE_AND_VID_OPTIONAL - compares by both eFuse and visual id , but compare by VID is optional if not specified (default mode)|
| TargetedPlistsFilePath | No | String |  | Path of the targeted plists in csv format for PUP on SORT environment|




## Datalog output

## In CLASS/SDT environment
Some of the PUP data is printed at LOT level and some at UNIT level

Lot level ituff data (keys and values):

- PUP_PATTERNS_DIR -  Patterns file path
- PUP_FILE_PATH -  PUP file path (pulled from **SCVars.SC_PUP_FILE_PATH**)
- PUP_MODE -  PUP enabled or disabled

Per config set plists:
- key - P:<ConfigSetName> , value -  pipe separated config set plists (P: prefix to indicate that the value are plists)
```
4_tsattrs_P:<ConfigSetName>,<ConfigSetPlist1>|<ConfigSetPlist2>|<ConfigSetPlist3>
```

Per config set instances:
- key - I:<ConfigSetName> , value -  pipe separated config set instances (I: prefix to indicate that the value are instances)
```
4_tsattrs_I:<ConfigSetName>,<Instance1>|<Instance2>|<Instance13>
```

- Maestro TP names under MSTP_NAME key specify the pipe separated list of MAESTRO TP names to which tested units in the PUP file belong

Example
```
4_tsattrs_PUP_PATTERNS_DIR,I:\patterns\patterns.pup.json
4_tsattrs_PUP_FILE_PATH,~HDMT_TPL_DIR\Modules\PUP\InputFiles\PUP.json
4_tsattrs_PUP_MODE,ON
4_tsattrs_PUP_MODE,ENABLED
4_tsattrs_MSTP_NAME,ADL0000D00S135|ADL0000E00S137
4_tsattrs_I:FastFullBase,PUP::FASTExecuteFullPlistFullMode_P1
4_tsattrs_P:FastFullBase,incremental_search_plist3
4_tsattrs_I:FastMonitorValidation,PUP::FASTMonitorFailKeepAndSkipPattern_F0^PUP::FASTMonitorFailKeepPattern_F0^PUP::FASTMonitorFailSkipPattern_F0
4_tsattrs_P:FastMonitorValidation,incremental_search_plist6
4_tsattrs_P:FastCacheBase2,pup_cached_plist
4_tsattrs_P:FastShortBase,incremental_search_plist|incremental_sub_plist_8|incremental_sub_plist_9
4_tsattrs_P:FastShortBase4,incremental_search_plist4
4_tsattrs_P:NoMatchingFastShortBase,incremental_sub_plist_2
4_tsattrs_I:FastCacheBase2,PUP::FASTExecuteFullPlistExcludeCriteriaEntryVmin_P1
4_tsattrs_I:FastCacheBase,PUP::FASTExecuteFullPlistExcludeCriteriaBaseNumber_P1^PUP::FASTExecutePupSlimPlistShortModeResolvedByCache_F0
4_tsattrs_I:FastMonitorBase,PUP::FASTExecuteFullPlistMonitorMode_P1
4_tsattrs_I:FastShortBase4,PUP::FASTExecutePupSlimPlistMatchingOnlyEFuse_F0

```



Unit level ituff data:
Unit level data is printed to ARIES db (2_ level) or MIDAS (0_level) according to the settings in **RunTimeLibraryVars.iCGL_EnableMIDAS** uservar

- List of config sets

```
  0_tname_pup_configsets
  0_strgval_<ConfigSet1>,<ConfigSet2>
```

- Per configSet PUP execution mode - Full [F] , Short/Slim [S] , Monitor [M]
  
```
  0_tname_pup_mode
  0_strgval_<ConfigSet1Mode>,<ConfigSet2Mode>
```

- Number of time the configSet was "active" during the flow - i.e. number of instances running a plist belonging to the corresponding configet pup_configsets

Example:
```
0_tname_pup_configsets
0_strgval_FastCacheBase,FastCacheBase2,FastMonitorBase,FastShortBase
0_tname_pup_mode
0_strgval_S,S,M,S
0_tname_pup_count
0_strgval_1,1,0,1|1|1
0_tname_excluderesult
0_strgval_T,T,NA,T|T|T
0_tname_excludereasoncount
0_strgval_1,1,0,1|1|1
```

## In SORT environment

When running in SORT environment -  the ituff tokens related to CLASS will be printed with "NA" 
```
2_tname_pup_configsets
2_strgval_NA
2_tname_pup_mode
2_strgval_NA
2_tname_pup_count
2_strgval_NA
2_tname_pup_excluderesult
2_strgval_NA
2_tname_pup_excludereasoncount
2_strgval_NA
```

In addition to that in SORT the below tokens are being printed

```
2_tname_pup_npr_execution_modes
2_strgval_CLEARED
2_tname_pup_dieinfo1
2_strgval_P3=NA|P8=NA|P46=S|P11=NA|P37=NA|P6=NA|P15=NA|P16=NA|P18=F|P32=S|P19=F|P20=F|P21=NA|P40=NA|P23=S|P24=NA|P27=F|P28=S
2_tname_pup_dieinfo2
2_strgval_Y
2_tname_pup_dieinfo3
2_strgval_NA
2_tname_pup_dietested
2_strgval_Y
```

For more details on the meaning of the tokens  - please refer to PUP Service SDK in the release wiki
## Exit Ports

The Dc test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**                             |
|---------------|---------------|---------------------------------------------|
| **-2**        | ***Alarm***   | Any alarm condition                         |
| **-1**        | ***Error***   | Any software condition error                |
| **0**         | ***Fail***    | Failing condition. Failed to print to Ituff |
| **1**         | ***Pass***    | Passing condition                           |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TTR**: Test Time Reduction
  - **TTR**: Test Time Reduction Monitor
  - **DPM**: Defect Per Million
  - **EOD**: End Of Device

## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**    |
|---------------------------|-------------|-------------------|-----------------|
| Feb 15<sup>st</sup>, 2021 | 1.0.0       | Slava Yablonovich | Initial version |
|                           |             |                   |                 |
