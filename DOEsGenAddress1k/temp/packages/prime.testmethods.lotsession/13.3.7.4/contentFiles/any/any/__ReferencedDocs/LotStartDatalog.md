# REP for Prime's LotStartDatalog TestMethod

This **REP** is intended to describe the LotStartDatalog Prime TestMethod.

[[_TOC_]]

# <span id="Methodology" class="anchor"></span> **Methodology**

## <p><span style="color:#ff0000"><strong>Ituff Sensitivity Warning</strong></span></p>
<p><span style="color:#ff0000">Do not enable <em>InstanceSummaryMode</em> for this test method instances. It will cause Ituff to be printed out-of-sequence causing failure down the line. A better solution is being worked on.</span></p>

## **Key Aspects**
:large_blue_diamond: This test method will create the datalog Lot header for ituff and other outputs (ir required) before a Lot has been run.  
:large_blue_diamond: This test method is intended to be used **after LotStartSetup TM**.  
:large_blue_diamond: There should only be **one** instance of this kind and it should be after the `HMI_Ituff` instance, which is in charge of starting the streams.

## **Verify**
:large_blue_diamond: Set debug console implementation if enabled.

## **Execute**
:large_blue_diamond: Set Lot start date.  
:large_blue_diamond: Construct ituff header.  
:large_blue_diamond: Write output header to datalog(ituff).  
:large_blue_diamond: Write output header to ScanFi (if required).  
:large_blue_diamond: Write output header to Raster (if required).  
:large_blue_diamond: Write output header to TFile (if required).  
:large_blue_diamond: Write output header to Telemetry (if required).  

# <span id="Parameters" class="anchor"></span> **Test Instance Parameters**

The table below lists and describes the test instance parameters supported by the LotStartDatalog test method

| **Parameter Name** | **Is required** | **Type** | **Supported Values** | **Default Value** |
|------------------|------------------|------------------|------------------|------------------|
| LogLevel | Yes | String | Enabled \|\|  Disabled | Disabled |
| StreamDestination** | No | String | OUTPUT_TO_FILE \|\|  OUTPUT_TO_FILE_AND_PUDL | OUTPUT_TO_FILE.|

** Note StreamDestination only works in TOS4. In TOS3 it does nothing. It is used to enable simple outputs to file such as ituff, scanfi, raster or enabling both output to file and PUDL for the files.

# <span id="Output" class="anchor"></span> **Datalog output**
This test method is in charge of logging headers to different outputs. Each output will be explained separately. Starting with the ituff output since it is **ALWAYS** printed.

The supported outputs are:  
:large_blue_diamond: Ituff  
:large_blue_diamond: ScanFi - **Same header as Ituff.**  
:large_blue_diamond: Telemetry  - **Same header as Ituff.**  
:large_blue_diamond: TFile    
:large_blue_diamond: Raster

To enable other outputs it is done via the following uservars:

- `_UserVars.iCGL_EnableRaster`
- `_UserVars.iCGL_EnableScanFI`
- `_UserVars.iCGL_EnableTFile`
- `_UserVars.iCGL_EnableTelemetry`

Said uservars must be equal to `"TRUE"` to enable their respective header output.
## **Ituff output**

Here you will find the explanation of each level logged by this TM. This includes each token that could appear, conditions on how it appears (if it applies) and any depedencies for how it works.

 As a general rule of thumb if a token printed to ituff shows up as empty it is most likely due to the uservar related to the token is empty or does not exist. 

 At the end there is a section with more details regarding certain <span style="color:lime"> special tokens</span>. These special tokens change behaviour based on specific conditions.

**Note that the following items are in order of appearance.**

## <span style="color:DarkSeaGreen"> **Ituff level 7** </span> 

| **Associated Token** | **Description** | **Format** | **Conditions** |
|------------------|------------------|------------------|------------------|
| lbeg | Start of level. | 7_lbeg | |
| filcret | **File creation time.** The token contains the file creation time, this time is taken the moment this token is populated. This date is formatted as: `yyyyMMddHHmmss` | 7_filcret_{Date} | |
| fmtver | **Format version token.** This token always logs the keyword `PRME`. Indicates the TP is using Prime for datalogging ituff lot start | 7_fmtver_PRME | Use Prime for logging ituff Lot Start |
| dsrcprg | **Run-Time Library and TOS.** Contains information regarding the currently source program being used. <br/> * **Prime version**: Comes from `PRIME_BASE` in environment file <br/> * **TOS version**: Is the value stored in `HDMT_PROFILE` in the environment file <br/> * **Tester platform type**: Currently will always be `HDMTG2` <br/> * **Compilation target**: Is built from the `tosVersion` obtained. It uses a regex expression to match the TOS version and give it a specific format | 7_dsrcprg_{testerPlatformType}_  {primeVersion}\_{compileTarget}_{tosVersion} | This format requires two **environment** variables to be populated correctly (**HDMT_PROFILE** and **PRIME_BASE**). If any of those are empty or do not exist the intended format will be incorrect and items will be empty. |
| ULT_DB_REQD | **Ult required.** Level 7 prints a line to indicate ULT database is required, this is controlled based on the following uservar: `DFFVars.WRITE_MODE` | 7_comnt_ULT_DB_REQD | Uservar must exist and has a value of either `"WRITE"` or `"LOCALWRITE"` for the line to be printed to ituff.

## <span style="color:DarkSeaGreen"> **Ituff level 6** </span> 
| **Associated Token** | **Description** | **Format** | **Conditions** |
|------------------|------------------|------------------|------------------|
| lbeg | Start of level | 6_lbeg | | 
| hndfile | **Handler file.** Comes from uservar `SCVars.SC_Handler_Recipe` | 6_comnt_hndfile_{handlerFile} | If this uservar is empty or does not exit this will not be logged. | 
| stplfile | **stpl file.** Comes from uservar `SCVars.SC_STPL` | 6_comnt_stplfile_{stplFile} | If this uservar is empty or does not exit this will not be logged. | 
| socfile | **Socket file.** Comes from uservar `SCVars.SC_Soc_File` | 6_comnt_socfile_{socFile} | If this uservar is empty or does not exit this will not be logged. |
| envfile | **Environment file.** Comes from uservar `SCVars.SC_Env_File` | 6_comnt_envfile_{envFile} | If this uservar is empty or does not exit this will not be logged. |
| PININDEXTABLE | **Pin index table.** In efforts to reduce ituff size, Prime PinService has the capability of taking an input json file to indicate a pin name and its mapping to a number. For example: <br/> <span style="color:orange"> "HDDPS_LC_gangx2_2p5ohm2" </span> : <span style="color:lightgreen"> 100 </span> <br/> This way in specific test methods the pin name will not be used and instead the number will be used. | 6_comnt_PININDEXTABLE_{pinName0}\|{pinMappedNumber0}\|{pinName1}\|{pinMappedNumber1} |  These mappings will be printed here for all pins used **only if such feature is being used**. Otherwise there is no pin index table to datalog. |
| lotid | **Lot name.** The lot name is always logged at this point. The lot name comes from the uservar `SCVars.SC_LOTNAME | 6_lotid_{lotName} | If this uservar is empty the token value will be left empty. | 

From this point, the generated information varies depending on the type of **test flow**. The possible types are: <span style="color:orange"> SingleDieSort test flow</span>, <span style="color:yellow"> Package test flow</span> and <span style="color:lime">  WaferSort </span>. The given test flow is controlled based on the value of the following uservar: `SCVars.SC_TEST_FLOW`. 

This station controller variable must **always** exist and be populated. 

For it to be **Package test flow** said uservar can have the following values: 
- `"QA", "CLASS", "SCLASS", "CCLASS"`. 

For it to be **SingleDieSort test flow** said uservar can have the following values: 
- `"SDTSORT", "SDSORT", "SDSSORT"`. 

For it to be **WaferSort test flow** said uservar can have the following values: 
- `"SORT", "SSORT"`.

---

## <span style="color:DarkSeaGreen"> **Ituff level 6 Package Test Flow** </span> 

| **Associated Token** | **Description** | **Format** | **Conditions** |
|------------------|------------------|------------------|------------------|
| packg | **Package value.** The package name is taken from the uservar `SCVars.SC_PACKAGE` | 6_packg_{package} | |
| prdct | **Product identifier.** The product id is formed based on a combination of uservars. Product id begins with the value stored in `SCVars.SC_DEVICE`. Then if the value of `SCVars.SC_REV` does not equal `"-"` or whitespace, it is added to the product id. Lastly the value in `SCVars.SC_STEP` is appended to the product id. <br/> In summary the logic is (lets assume SC_REV is not "-" or whitespace): <br/> ProductId = SCVars.SC_DEVICE <br/> ProductId += SCVars.SC_REV <br/> ProductId += SCVars.SC_STEP | 6_prdct_{ProductId} | |
| engid | **Eng identifier.** The Eng id is taken from the uservar `SCVars.SC_ENGID` | 6_engid_{engid} |
| dieslct | **Die selector.** The selected die value is taken from uservaar `SCVars.SC_DIESLCT` | `6_dieslct_{value}` | If uservar doesn't exist or is empty it will not be logged. |
| sspec | **SSPEC.** Details regarding SSPEC are taken from `SCVars.SC_SSPEC` | `6_sspec_{value}` | |
| fabid | **Fabid.** Fab id information is taken from uservar `SCVars.SC_FABID` | `6_fabid_{value}` | |
| prgnm | **Program name.** The test program name can be obtained from 2 different methods. The usual way is extrating the program name from the TP path obtained from TOS api. The user has the capacity to override this value using the uservar `RunTimeLibraryVars.iCGL_TpAltName` | 6_prgnm_{programName} | If uservar `RunTimeLibraryVars.iCGL_TpAltName` exists and has a value this will be used instead of name extracted from TOS api TP path.
| testertype | The tester type is obtained from uservar `SysCUserVars.SC_MODULE_TYPE` | 6_testertype_{value} | The valid tester types currently are: `"HDMT","SDX","HDMX","HBI","HOP"`. Any other will be considered **UNDEFINED**. If its undefined the tester type will not be datalogged. |

---

## <span style="color:DarkSeaGreen"> **Ituff level 6 SingleDieSort Test Flow** </span> 
| **Associated Token** | **Description** | **Format** | **Conditions** |
|------------------|------------------|------------------|------------------|
| fusedlot | **Fused lot name.** The fused lot name is obtained based on the previous Lot Name from `SCVars.LOT_NAME`. The value stored is taken and a substring of 7 characters is taken. This due to fused lot having a **maximum length of 7 characters**| 6_fusedlot_{value} |  | |
| prdct | **Product id.** In **single die sort** the product id will be added the value stored in `SCVars.SC_PACKAGE` based on specific conditions | 6_prdct_{value} | If the value inside is 1 character long or 2 characters but **not equal to "WS"** it will be used. From this point on the product id works just like in **Package test flow** |
| prgnm | **Program name.** The test program name can be obtained from 2 different methods. The usual way is via the TOS api for the test program name. The user has the capacity to override this value using the uservar `RunTimeLibraryVars.iCGL_TpAltName` | 6_prgnm_{programName} | If `RunTimeLibraryVars.iCGL_TpAltName` exists and has a value this will be used instead of the TOS api name. |

---

## <span style="color:DarkSeaGreen"> **Ituff level 6 WaferSort Test Flow** </span>
| **Associated Token** | **Description** | **Format** | **Conditions** |
|------------------|------------------|------------------|------------------|
| fusedlot | **Fused lot name.** The fused lot name is obtained based on the previous Lot Name from `SCVars.LOT_NAME`. The value stored is taken and a substring of 7 characters is taken. This due to fused lot having a **maximum length of 7 characters**| 6_fusedlot_{value} | This token only gets datalogged when **testerType == "HOP"**. | |
| prdct | **Product identifier.** The product id is formed based on a combination of uservars. Product id begins with the value stored in `SCVars.SC_DEVICE`. Then if the value of `SCVars.SC_REV` does not equal `"-"` or whitespace, it is added to the product id. Lastly the value in `SCVars.SC_STEP` is appended to the product id. <br/> In summary the logic is (lets assume SC_REV is not "-" or whitespace): <br/> ProductId = SCVars.SC_DEVICE <br/> ProductId += SCVars.SC_REV <br/> ProductId += SCVars.SC_STEP | 6_prdct_{ProductId} | |
| sspec | **Specification number.** Details regarding SSPEC are taken from `SCVars.SC_SSPEC` | 6_sspec_{value} | |
| prgnm | **Program name.** The test program name can be obtained from 2 different methods. The usual way is the name stored in `SCVars.TP_PROGRAM_NAME`. The user has the capacity to override this value using the uservar `RunTimeLibraryVars.iCGL_TpAltName` | 6_prgnm_{programName} | If `SCVars.TP_PROGRAM_NAME` exists and has a value this will be used instead of `SCVars.TP_PROGRAM_NAME` |
| testertype | The tester type is obtained from uservar `SysCUserVars.SC_MODULE_TYPE` | 6_testertype_{value} | The valid tester types currently are: `"HDMT","SDX","HDMX","HBI","HOP"`. This token only gets datalogged in this test flow if **testerType == "HOP"**. |

---

## <span style="color:DarkSeaGreen"> **Rest of ituff level 6 (regardless of test flow)** </span>
| **Associated Token** | **Description** | **Format** | **Conditions** |
|------------------|------------------|------------------|------------------|
| ssid, sscat, ssprdct, ssdesc | **SSID data.** SSID ituff tokens are logged at this point regardless of test flow. Said tokens come from SSID ituff tokens inside `_UserVars.DCF_XmlPath` | 6_ssid_{value} <br/> 6_sscat_{value} <br/> 6_ssprdct_{value} <br/> 6_ssdesc_{value} | They **must** follow the correct format: <br/> `"{SSID_0}:{SSCAT_0}:{SSPRDCT_0}:{SSDESC_0},{SSID_1}:{SSCAT_1}:{SSPRDCT_1}:{SSDESC_1},..."` <br/> Any incorrect format **will not work and exceptions will be thrown.** **If tokens inside `_UserVars.DCF_XmlPath` is empty nothing will be logged** |
| PCAT | **PCAT information.** PCAT (Principal Components Analysis Test) information will be logged at the end of level 6 ituff. This information is provided by TriggeredDC TM | | If this test method is **not** being used there is **no PCAT information to datalog here**. More detailed information on the format can be sound in said TM |

---

## <span style="color:DarkSeaGreen"> **Ituff level 5** </span>
| **Associated Token** | **Description** | **Format** | **Conditions** |
|------------------|------------------|------------------|------------------|
| lbeg | Start of level | 5_lbeg | |
| flstpid | **Flow Step.** The value stored in `SCVars.SC_TEST_FLOW` is used here | 5_flstpid_{value} | This uservar variable **must always exist and not be empty.** This due to using this variable to know which kind of test flow is being used
| lcode | Lcode value is taken from uservar `SCVars.SC_LOCN` | 5_lcode_{value} | |

## <span style="color:DarkSeaGreen"> **Ituff level 4** </span>
| **Associated Token** | **Description** | **Format** | **Conditions** |
|------------------|------------------|------------------|------------------|
| lbeg | Start of level | 4_lbeg | |
| ptype_TB | Flag at start of ituff to trigger Illiad[Sort dispo system] to apply legacy ibin or next gen tbin | 4_ptype_TB | Uservar `RunTimeLibraryVars.iCGL_EnableTbinShip` must exists and set to **TRUE** |
| sysid | System id token is taken from uservar `SCVars.SC_MODULEID` | 4_sysid_{value} | |
| oprtr | This is taken from uservar `SCVars.SC_OPRID` | 4_oprtr_{value} | |
| owneremail | The value comes from `_UserVars.OWNREMAIL` | 4_ownremail_{email} | This is only logged when the current test flow **is not SingleDieSort** and the tester type **is not HOP.** If this variable is not empty and its **length is less than 50 characters** it will be logged |
| facid | This value comes from uservar `SCVars.SC_FACILITYID` | 4_facid_{value} | 
| tempr | This is taken from uservar `SCVars.SC_TEMPERATURE` | 4_tempr_{value} | |

From this point foward what appears will depend on the current test flow type. In this case the flows can be **SingleDieSort, Package, Other**, other being anything that's not the first two options. 

---

## <span style="color:DarkSeaGreen"> **Ituff level 4 SingleDieSort test flow** </span>
| **Associated Token** | **Description** | **Format** | **Conditions** |
|------------------|------------------|------------------|------------------|
| wafid | The wafer id token is taken from `SCVars.SC_LOT_WAFER` | 4_wafid_{value} | |

---

## <span style="color:DarkSeaGreen"> **Ituff level 4 Package test flow** </span>
| **Associated Token** | **Description** | **Format** | **Conditions** |
|------------------|------------------|------------------|------------------|
| ldbid | **Test board id.** The test board id token information is taken from uservar `SCVars.TP_TESTBOARD_ID` | 4_ldbid_{value} | |
| hdnid | **Handler id.** The handler id information is taken from uservar `SCVars.EI_HANDLER_ID` | 4_hndid_{value} | |
| smrynam | **Summary name token.** Information is taken from `SCVars.SC_SUMMARY_NAME` | 4_smrynam_{value} | |
| spbicrqd | In the case the test flow falls into **Functional class**, an extra token is printed | 4_spbicrqd | Only when `SCVars.SC_TEST_FLOW = "CLASS"` | 

---

## <span style="color:DarkSeaGreen"> **Ituff level 4 other test flows** </span>

This applies to any other test flow the didnt fall into the other two categories.

| **Associated Token** | **Description** | **Format** | **Conditions** |
|------------------|------------------|------------------|------------------|
| prbcd | Information is taken from uservar `SCVars.TP_TESTBOARD_ID` | 4_prbcd_{value} | |
| prber | Information is taken from uservar `SCVars.EI_HANDLER_ID` | 4_prber_{value} | If value from uservar is empty it will be datalogged as `4_prber_UNKNOWN`. |
| wafid | The wafer id token is taken from `SCVars.SC_LOT_WAFER` | 4_wafid_{value} | |
| eleczhtfstp | The Eleczhtfstpn token information is taken from uservar `SCVars.SC_ELECZHTFSTPN` | 4_eleczhtfstpn_{value} | |
| eleczht | The Eleczht token information is taken from uservar `SCVars.SC_ELECZHT` | 4_eleczht_{value} | |

---

## <span style="color:DarkSeaGreen"> **Rest of ituff level 4 (regardless of test flow)** </span>
| **Associated Token** | **Description** | **Format** | **Conditions** |
|------------------|------------------|------------------|------------------|
| begindt | **Begin datetime token.** The datetime is taken the moment ituff level 4 building is started. It has the following format: `"yyyyMMddHHmmss"` | 4_begindt_{value} | |
| FRV_SPEC | **Frv spec token.** The token `tsattrs_FRV_SPEC,{value}` information is taken from a special uservar `FUSEVars.UFG_FREV_SPEC` | 4_tsattrs_FRV_SPEC,{value} | For this token to appear this uservar **must exist, and not be empty** |
| DFF_MTL_Revision | The value to be used will be the field called *MTL_Name* from the input *.xml* file | 4_tsattrs_DFF_MTL_Revision,{value} | Only appears in the case where DFF MTL tokens are being used. If there is no input file this token **will not appear** |
| flex_bom_recipe | The token information is taken from uservar `SCVars.SC_FLEXBOMRECIPE` | 4_tsattrs_flex_bom_recipe,{value} | The item is printed when uservar **exists and is not empty** | 
| flex_bom_hri | The token information is taken from uservar `SCVars.SC_FLEXBOMHRI` | 4_tsattrs_flex_bom_hri,{value} | The item is printed when uservar **exists and is not empty** 
| EEPROM_TIUDESIGNID | Information is obtained from TOS, no uservar or other configuration is required | 4_tsattrs_EEPROM_TIUDESIGNID,{value} | This token is only printed when the tester type is **HDMX** and **HBI** |
| tplpath, stplpath, socpath, envpath | **Test program file information.** At level 4 all files used for the current test program are logged as `tsattrs`. The files logged are: Test plan file in used (\*.tpl), Sub test plan file (\*stpl), socket file (\*.soc), environment file (\*.env). This information is obtained from the *Test Program Service* | 4_tsattrs_tplpath,{path} <br/> 4_tsattrs_stplpath,{path} <br/> 4_tsattrs_socpath,{path} <br/> 4_tsattrs_envpath,{path} | |
| PreLoadDelta, CfgLoadDelta, FinalLoadDelta, FinalLoadEpoch | **Test plan load times.** Certain tokens related to the test plan loading time are logged. This information is logged for legacy purposes. Four items are logged but only the loading time *(CfgLoadDelta)* is valid and implemented in TOS | 4_tsattrs_TSS_PreLoadDelta,0  4_tsattrs_TSS_CfgLoadDelta,{value} 4_tsattrs_TSS_FinalLoadDelta,0  4_tsattrs_TSS_FinalLoadEpoch,0 |
| DelayLotTime | **Delay lot time.** The delay lot time information is only available and logged when theres more than one lot run. This is the time difference between the ending time of a lot taken when **Lot End Datalog TM** begins and the time **Lot Start Datalog TM** starts. The time difference is given **in seconds** | 4_tsattrs_DelayLotTime,{value} | |
| FlowInitExecTimeSiteC | **Flow Init time.** The flow init time information as the name implies is the aproximate time the test program Init Flow took. This time is obtained from the moment Prime Init TM starts until the test program Init flow ends. Due to this it is an **aproximation** of how long Init flow took. This time is given in **milliseconds**. | 4_tsattrs_FlowInitExecTimeSiteC,{value} | |
| tsattrs | **PCAT Variables.** (Principal Components Analysis Test) level 4 information is **optional** as long as Triggered DC TM is not in use. This information comes from the following uservars: `SCVars.PCATModelNames`, `SCVars.PCATModelTokens`, `SCVars.PCATEnableFlags`.| `4_tsattrs_{PCATModelToken},{PCATModelName}`  `4_tsattrs_{PCATModelToken}FLAG,{PCATEnableFlag}` | All uservars **must only match the following regex for valid characters: "^[0-9A-Za-z_]+$"** otherwise an exception will be thrown. All uservars can contain several values separated by a comma. The amount of values/items provided **MUST** be the same quantity for each uservar.|  
PROCESS_STEP_SEQUENCE  CURRENT_PROCESS_STEP  CURRENT_PROCESS_TYPE| **Process Information**. At this level process related information is logged if conditions are met. There are three important uservars: `SCVars.SC_PROCESS_STEP_SEQUENCE`  `SCVars.SC_CURRENT_PROCESS_STEP`  `SCVars.SC_CURRENT_PROCESS_TYPE` | `4_tsattrs_PROCESS_STEP_SEQUENCE,{value}`  `4_tsattrs_CURRENT_PROCESS_STEP,{value}`  `4_tsattrs_CURRENT_PROCESS_TYPE,{value}` | If any of these uservars exist and are **NOT** empty information will be logged. A fourth uservar exists to indicate where **HCSL** mode is enabled, `SCVars.SC_HCSLMODE`. When this uservar is equal to `TRUE` all previous uservars become **OBLIGATORY** and **can't be empty**.
| tsattrs |**Per unit variables**: Per unit size logging is required to be enabled via uservar: `PerUnitDataSizeVars.PerUnitSizeLoggingEnable`. To enable this logging capability this uservar must be equal to `TRUE`. Once enabled the following uservars should be populated: `PerUnitDataSizeVars.PerUnitSizeMIDAS`: Can contain multiple values. String values separated with `\|`. `PerUnitDataSizeVars.PerUnitSizeARIES`: Can contain multiple values. String values separated with `\|`. `PerUnitDataSizeVars.ProductIdentifier`.  `PerUnitDataSizeVars.TimeinProductLife`.  `SCVars.SC_CURRENT_PROCESS_TYPE`.  `PerUnitDataSizeVars.CheckSum`. They must all exist and be populated, otherwise **information will be missing, and ouput format will be different**. | `4_tsattrs_{PerUnitSizeARIES[0]},{ProductIdentifier},{TimeinProductLife},{SC_CURRENT_PROCESS_TYPE},{PerUnitSizeARIES[LastElement]},{CheckSum}`  `4_tsattrs_{PerUnitSizeMIDAS[0]},{ProductIdentifier},{TimeinProductLife},{SC_CURRENT_PROCESS_TYPE},{PerUnitSizeMIDAS[LastElement]},{CheckSum}` |  \* Please note PerUnitSizeARIES[0] means the **FIRST** element from the `\|` separated string uservar. PerUnitSizeARIES[LastElement] is the **LAST** element. The rest are the contents in each uservar.| 
| tsattrs | **Tsattrs collection**: The user has the capacity of adding uservars to the collection `TSATTRSVars`, each uservar in this collection **if it exists** will be logged.|`4_tsattrs_{uservarName},{uservarValue}` | Certain conditions will modify the behavior of what is logged based on the value of said uservars.  If a uservar is empty the printed `uservarValue` will be `"EMPTY"`.  If the contents of a uservar are longer than 256 characters the printed value will be `"ERRORSTRINGLEN"`.  If the contents of a uservar contains **invalid characters** the printed value will be `"ERRORSTRINGFORMAT"`.|
| uddtype | **UDD variables**: UDD (User defined data) variable printing is now managed via DatalogService and the SPM implementation for UDD. More information can be found in said service. | `4_uddtype_{uservarContents}` | |
| tsattrs | **Service Files**: Service name and input files are printed as tsattrs. This is enabled via the following uservar: `String PrintServiceFileToItuff`. Further details can be found at InitTM official documentation: https://dev.azure.com/mit-us/PrimeWiki | `4_tsattrs_{ServiceName},{ServiceFiles}` | This is only printed if enabled.
| tsattrs_OverallInitFlowStatistics | Includes memory changes between the moment **InitializeLibraryTM** is executed and the end of the init flow. | `4_tsattrs_OverallInitFlowStatistics,PrivateBytesBefore(b)=717910016,  PrivateBytesAfter(b)=834813952,PrivateBytesDelta(b)=116903936,  VirtualBytesBefore(b)=7019753472,VirtualBytesAfter(b)=7308570624,  VirtualBytesDelta(b)=288817152` | If instances are executed before **InitializeLibraryTM** memory calculations will be dirty.
| tsattrs_OverallServiceStatistics | Includes overall exeution time and memory impact initializing Prime services. Includes overall memory impact. | `4_tsattrs_OverallServiceStatistics,TotalExecutionTime(ms)=2239.00,  PrivateBytesBefore(b)=759676928,PrivateBytesAfter(b)=819662848,  PrivateBytesDelta(b)=59985920,VirtualBytesBefore(b)=7037878272,  VirtualBytesAfter(b)=7297245184,VirtualBytesDelta(b)=259366912` | |
| tsattrs_OverallVerifyAllInstancesStatistics | Includes overall execution time and memory impact verifying all instances, only makes sense if **InitializeIntancesTM** verifies all TM instances. | `4_tsattrs_OverallVerifyAllInstancesStatistics,TotalExecutionTime(ms)=15.00,  PrivateBytesBefore(b)=822661120,PrivateBytesAfter(b)=822706176,  PrivateBytesDelta(b)=45056,VirtualBytesBefore(b)=7297376256,  VirtualBytesAfter(b)=7297376256,VirtualBytesDelta(b)=0` | Refer to **InitializeIntancesTM** documentation for more details of how to verify all instances via this TM. |
tsattrs_PerServiceInitialization | Includes execution time and memory impact per prime service initialized (can be several). | `4_tsattrs_PerServiceInitialization,Service=FuseDBService,ElapsedTime(ms)=36,  PrivateBytesBefore(b)=760569856,PrivateBytesAfter(b)=760696832,PrivateBytesDelta(b)=126976,  VirtualBytesBefore(b)=7040040960,VirtualBytesAfter(b)=7040106496,VirtualBytesDelta(b)=65536` | Can vary depending on prime services used. |
tsattrs_TopMemoryConsumersByInstance | Includes the top 15 instances that caused the most notable memory consumption increases when verified. | `4_tsattrs_TopMemoryConsumersByInstance,Instance=Base::SharedStorageDumpRestore_P1,  PrivateBytesBefore(b)=793657344,PrivateBytesAfter(b)=803295232,PrivateBytesDelta(b)=9637888,  VirtualBytesBefore(b)=7243243520,VirtualBytesAfter(b)=7248162816,VirtualBytesDelta(b)=4919296` | |
| tsattrs_TopMemoryConsumerByTestClass | Includes the top 15 test method classes and their aggregated memory impact seen when this kind of instance is verified. | `4_tsattrs_TopMemoryConsumerByTestClass,ClassName=FivrDomainsTestMethod,  AccumulatedElapsedTime(ms)=3552,AccumulatedPrivateBytes(b)=83791872,  AccumulatedVirtualBytes(b)=7802880` | If the instance was verified before calling **InitializeIntancesTM** it is normal the memory consumption is low since it was already verified. | 
| tsattrs_PrimeNugetsVersion | Print the Prime Nuget versions used on the running TP. | `4_tsattrs_PrimeNugetsVersion_<version>` |  | 
| tsattrs_PatchNugetsVersion | Include the names and versions of all official DLLs that have a version different from PrimeNugetsVersion. | `4_tsattrs_PatchNugetsVersion1_<dllName1=version1 dllName2=version2...>` | It is only printed if there are supersedes. If there are more than 5 DLLs (supersedes), they will be printed in a separate token with the same name but with an index due to the character limit `4_tsattrs_PatchNugetsVersion2_<dllName6=version6 dllName7=version7...>` |
| tsattrs_EngPatchNugetsVersion | Include the names and versions of all engineering DLLs that have a version different from PrimeNugetsVersion. | `4_tsattrs_EngPatchNugetsVersion1_<dllName1=version1 dllName2=version2...>` | It is only printed if there are engineering supersedes. If there are more than 5 DLLs (supersedes), they will be printed in a separate token with the same name but with an index due to the character limit `4_tsattrs_EngPatchNugetsVersion2_<dllName6=version6 dllName7=version7...>` |

## <span style="color:DarkSeaGreen"> **Ituff level 4 SingleDieSort test flow** </span>
| **Associated Token** | **Description** | **Format** | **Conditions** |
|------------------|------------------|------------------|------------------|
| introtype | **Intro method**: This token will only appear in **SingleDieSort test flow**. Otherwise it will **not** show up. The value comes from `SCVars.SC_INTRO_METHOD`. | `4_tsattrs_introtype,{value}`|

---

**At this point nothing else is printed for the current lot, ituff datalogging is completed**.

## <span style="color:LightBlue"> **Special token details** </span>
Certain tokens change behavior based on _interesting_ conditions. This section intent is to clarify in more detail than the previous table on how this tokens work.

<span style="color:Pink"> **Level 6 package test flow.** </span>

The token `prdct` works in the following way:

Setup 1:

```python
SCVars.SC_DEVICE = "P1"
SCVars.SC_REV = "2"
SCVars.SC_STEP = "3"
```
Since the value of `SCVars.SC_REV` is not equal to "-" or whitespace it **will** be used to build the `prdct` token, resulting in:
`6_prdct_{P123}`

Setup 2:

```python
SCVars.SC_DEVICE = "P1"
SCVars.SC_REV = "-"
SCVars.SC_STEP = "3"
```
Since the value of `SCVars.SC_REV` is  equal to "-" it is ignored, resulting in:
`6_prdct_{P13}

Setup 3:

```python
SCVars.SC_DEVICE = "P1"
SCVars.SC_REV = " "
SCVars.SC_STEP = "3"
```
Since the value of `SCVars.SC_REV` is  equal to whitespace it is ignored, resulting in:
`6_prdct_{P13}

<span style="color:Pink"> **Level 6 SingleDie test flow.** </span>

The token `prdct` works in the following way in **single die sort** the product id will be added the value stored in
`SCVars.SC_PACKAGE` based on specific conditions. If the value inside is 1 character long or 2 characters but **not equal to "WS"** it will be used. From this point on the product id works just like in **Package test flow**. 

Using examples based on the Package example:

Example with `SCVars.SC_PACKAGE="1"`
```python
6_prdct_1P123
```

Example with `SCVars.SC_PACKAGE="WS"`
```python
6_prdct_P123
```

Example with `SCVars.SC_PACKAGE="WX"`
```python
6_prdct_WXP123
```

Example with `SCVars.SC_PACKAGE="WXS"`
```python
6_prdct_P123
```

<span style="color:Pink"> **Level 4 PCAT.** </span>

As mentioned before the amount of items per uservar must be equal. This means the amount of comma separated values must be the same for all three of the uservars.

Example for a **CORRECT** setup:

`SCVars.PCATModelNames = "1,2,3"`

`SCVars.PCATModelTokens = "1,2,3"`

`SCVars.PCATEnableFlags = "TRUE,FALSE,TRUE"`

Example for a **INCORRECT** setup:

`SCVars.PCATModelNames = "1,2,3"`

`SCVars.PCATModelTokens = "1,2,3"`

`SCVars.PCATEnableFlags = "TRUE,FALSE"`

Enabled Flags: Can only take values of `TRUE` or `FALSE`, any other value is **NOT** valid.

Model Tokens: Each item provided can have a maximum length of 32. This due to Aries limitations.

Model Names: Each provided can have a maximum length of 256. This due to Aries limitations.

## **TFile output**

TFile header output will primarily display information taken from several uservars. With the condition that most if not all must not be empty. The uservar details are:

| Collection | **Name**       | Required 
| -------------- | -------------- | ----------- | 
| SCVars | SC_LOTNAME | Yes. Can't be empty. |
| SCVars | SC_LOCN | Yes. Can't be empty. |
| SCVars | SC_DEVICE | Yes. Can't be empty. |
| SCVars | SC_REV | Yes. |
| SCVars | SC_STEP| Yes. |
|SCVars | TP_PROGRAM_NAME | Yes. Can't be empty. |
| _UserVars | FAB_PROCESS | Yes. Can't be empty. |
| SCVars |SC_SUMMARY_NAME |Yes. Can't be empty. |
| SCVars | SC_LOT_WAFER | Yes. Can't be empty. |

## **Raster output**
Raster output uses certain uservars to add information to the header:
| Collection | **Name**       | Required 
| -------------- | -------------- | ----------- | 
| SCVars | SC_LOCN | No. Information from this variable will be missing when not found.|
| SCVars | SC_LOTNAME | No. Information from this variable will be missing when not found. |
| _UserVars | FAB_PROCESS | Yes. Advised to not be empty, not forced. |
| RunTimeLibraryVars | iCGL_EnableLsaSupport | No. LSA support disabled when not found.|

Raster header also uses the Environment variable `PRIME_BASE` to obtain information. If this environment variable does not exist it is assumed as **empty**.

# <span id="Hooks" class="anchor"></span> **Custom User Code Hooks**
LotStartDatalog test method supports the following extensions:
```csharp
    /// <summary>
    /// Class to define extendable methods.
    /// </summary>
    public interface ILotStartDatalogExtensions
    {
        /// <summary>
        /// Called at execute once level 7 header is built.
        /// Method to be implemented by user to modify level 7 header values before they are printed out.
        /// This allows the user to modify this level as desired, for example adding new items or modifying
        /// existing ones. Default implementation outputs basic working level 7 header format.
        /// </summary>
        /// <param name="headerData">Contains lot level 7 header data.</param>
        /// <returns>Modified header.</returns>
        string ModifyLotLevel7Header(string headerData);

        /// <summary>
        /// Called at execute once level 6 header is built.
        /// Method to be implemented by user to modify level 6 header values before they are printed out.
        /// This allows the user to modify this level as desired, for example adding new items or modifying
        /// existing ones. Default implementation outputs basic working level 6 header format.
        /// </summary>
        /// <param name="headerData">Contains lot level 6 header data.</param>
        /// <returns>Modified header.</returns>
        string ModifyLotLevel6Header(string headerData);

        /// <summary>
        /// Called at execute once level 5 header is built.
        /// Method to be implemented by user to modify level 5 header values before they are printed out.
        /// This allows the user to modify this level as desired, for example adding new items or modifying
        /// existing ones. Default implementation outputs basic working level 5 header format.
        /// </summary>
        /// <param name="headerData">Contains lot level 5 header data.</param>
        /// <returns>Modified header.</returns>
        string ModifyLotLevel5Header(string headerData);

        /// <summary>
        /// Called at execute once level 4 header is built.
        /// Method to be implemented by user to modify level 4 header values before they are printed out.
        /// This allows the user to modify this level as desired, for example adding new items or modifying
        /// existing ones. Default implementation outputs basic working level 4 header format.
        /// </summary>
        /// <param name="headerData">Contains lot level 4 header data.</param>
        /// <returns>Modified header.</returns>
        string ModifyLotLevel4Header(string headerData);
    }
```

All extension methods essentially provide the exact same capability. Each method allows the user to utilize the **basic** generated header for each level and modify it as the user pleases. The ``default`` implementation will return the already generated basic header for each level (as explained previously). 

The user could make changes such as:
- Introducing a new ituff line.
- Modifying default ituff and deleting lines.

The idea for each extension method is to take in the whole string for each ituff level, the user can then use C# code to modify the string. Lastly return the resulting string for the header level.

**Note:** If a user decides to modify the base ituff, it becomes **the users responsability to make sure any new line or modification is compatible with ituff format.**

# <span id="TPL" class="anchor"></span> **TPL Samples**

```python
Import PrimeLotStartDatalog.xml;
 
Test PrimeLotStartDatalog LotStartDatalog_LOTSTARTFLOW_INSTANCE
{
}
```

# <span id="FlowOrder" class="anchor"></span> **Test Program Flow order requirements**

The image below ilustrates the order requiremetns for Prime instances in order to generate correctly the ITUFF <font size="3"><span style="color:OrangeRed"> Lot </font>testing session <font size="3"><span style="color:OrangeRed">Header</font>.

::: mermaid
    graph LR
    classDef PRIME fill:#005B85;
    classDef TOS fill:#B24501;
        subgraph Lot Start Subflow
            direction LR
            id0("HMI_Ituff (TOS3 Only)"):::TOS -->
            id2("PrimeLotStartSetupTestMethod"):::PRIME -->
            id3("PrimeLotStartDatalogTestMethod"):::PRIME
        end
:::

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note for TOS3** </span></font> :warning:
>For HDMT the ITUFF and PUDL control is handled by TOS through the HMI Test Methods. Instances must exist on the Start and End Lot Flows to create and close the ITUFF streams.
>
>HMI_Ituff (OperationMode = LOTTEST_START) instance must be before the PrimeLotStartSetupTestMethod instance.  

# <span id="ExitPorts" class="anchor"></span> **Exit Ports**

The LotStartDatalog test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |

# <span id="Dependencies" class="anchor"></span> **Additional Dependencies**

This test method has a dependency with all Device TMs from PRIME. The use of LotStartDatalog without Device TMs is not a supported setup, **any attempt to not follow this rule can cause errors**. 

**It is also intended to be used after PRIME's LotStartSetup TM and HMI_ITUFF.**

Please read carefully previous datalog information. Certain TMs can change the behavior of what information is datalog. For example:

- TriggeredDC: Adds level 6 PCAT information.
- PUPService: Adds level 4 PUP information.
- SPMHelper: Adds UDD tokens to level 4.
- SimbaService: Adds level 4 Simba header information.

# <span id="VersionTracking" class="anchor"></span> **Version tracking**

| **Date**       | **Version** | **Author**     | **Comments** |
| -------------- | ----------- | -------------- | ------------ |
| Mar 10, 2022 | 1.0.0| jabarran| LotEndDatalog TM added. | 
| Aug 12, 2022 | 1.0.1 | hramirez | Adding Flow order documentation |
| Feb 7, 2023 | 1.1.0 | jabarran | HOP Fixes and documentation update |
| Aug 13, 2024 | 13.01.00 | ckhoh    | Update Flow order documentation |

# <span id="Acronyms" class="anchor"></span> **Acronyms**

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **HDMX**: Next Generation HDMT
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **TM**: Test Method
  - **SCVar**: Station Controller uservar.
  - **MTT**: Multi trial test.