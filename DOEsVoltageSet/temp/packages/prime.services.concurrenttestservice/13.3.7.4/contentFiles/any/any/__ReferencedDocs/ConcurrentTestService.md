# ConcurrentTestService

| Contents      |    
| :----------- |  
| 1. [Introduction](#introduction)      |   
| 2. [Configuration Files](#configuration-files)   |   
| 3. [API Description](#api-description)   |   

###

----  
## 1. Introduction  

The ConcurrentTestService is meant to be used when testing with plists that utilize Concurrent Trace test methodology. For details see [CTT Search](https://wiki.ith.intel.com/pages/viewpage.action?spaceKey=ccrcontent&title=CTT+Search+-+Safe+Zone+Search)  
This service provides the infrastructure which can be used to determine which IP/PowerSupplies are failing and to update the Plist/IFuncTest in order to perform an incremental search.  


###

----  
## 2. Configuration Files  

Basic Definitions  
* CTT = Concurrent Test Technologies
* CTTGroup
  * A Group name for several IP which will share a common set of failure decoding.  
  * Should mostly map to VminForwarding Domain names.  
  * Only used with ConcurrentTest service input files.  
  * Each CTTGroup name must be unique.  
* CTTInstance
  * An IP instance within a CTTGroup.
  * Should mostly map to VminForwarding Instance names.
  * Only used with ConcurrentTest service input files.  
  * Each CTTInstance name should be unique within a CTTGroup.  


###

----  
### 2.1 TestProgramSpec Configuration Files   
  

----  
##### Top-Level Tokens
* **Project** - Type:String   
  * The Project/Product that this file is used for. Currently not used.  
* **CCRModuleName** - Type:String   
  * Prime PatConfig SetPoint Module name to use for any pattern-modify.  
* **ResourcePowerRailMaping** - Type:List of [PowerRailMapping](#powerrailmapping-tokens)  
  * Defines the resource mapping for all the different IPs which will be run concurrently.  

----  
##### PowerRailMapping Tokens  
* **GroupName** - Type:String  
  * The CTTGroup Name for this object. Must be unique.  
* **FlowDomain** - Type:String  
  * The TestProgram/FlowControl Domain name which will be used to get the current/active Flow ID when testing.  
  * See also
    * Prime.Services.TestProgramService.IsDffEnableForDomainFlow()
    * Prime.Services.TestProgramService.GetDffTokenNameForDomainFlow()
    * Prime.Services.TestProgramService.GetDomainCurrentFlow()
    * PrimeFlowControlStartTestMethod
* **InstanceToVminFwdInstance** - Type:Dictionary\<String, String>
  * Key=CTTInstance Name
  * Value=VminForwarding Instance Name
  * This dictionary maps the CTTInstance Name to the corresponding VminForwarding Instance name.  
* **InstanceToPowerRail** - Type:Dictionary\<String, String>  
  * Key=CTTInstance Name
  * Value=Pin or FIVR/DLVR voltage rail
  * This dictionary maps the CTTInstance Name to the corresponding tester resource used to set its voltage. It should be either a pin or FIVR/DLVR voltage rail previously defined.  

----  
Example: 
```json
{
  "Project": "RPLS-282",
  "CCRModuleName": "Mccr",
  "ResourcesPowerRailMapping": [
    {
      "GroupName": "CORES",
      "FlowDomain": "CORE",
      "InstanceToVminFwdInstance": {
        "CORE0": "CR0",
        "CORE1": "CR1",
        "CORE2": "CR2",
        "CORE3": "CR3"
      },
      "InstanceToPowerRail": {
        "CORE0": "VCC_IN0",
        "CORE1": "VCC_IN1"
      }
    },
    {
      "GroupName": "ATOMS",
      "FlowDomain": "ATOM",
      "InstanceToVminFwdInstance": {
        "ATOM0": "ATM0",
        "ATOM1": "ATM1",
        "ATOM2": "ATM2",
        "ATOM3": "ATM3"
      },
      "InstanceToPowerRail": {
        "ATOM0": "TP_NAME_FOR_ATOM0_PWR",
        "ATOM1": "TP_NAME_FOR_ATOM1_PWR",
        "ATOM2": "TP_NAME_FOR_ATOM2_PWR",
        "ATOM3": "TP_NAME_FOR_ATOM3_PWR"
      }
    },
    {
      "GroupName": "CCF",
      "FlowDomain": "CLR",
      "InstanceToVminFwdInstance": {
        "CCF": "CLR0"
      },
      "InstanceToPowerRail": {
        "CCF": "CCF_POWER_RAIL"
      }
    },
    {
      "GroupName": "GT",
      "FlowDomain": "GT",
      "InstanceToVminFwdInstance": {
        "GT": "GT0"
      },
      "InstanceToPowerRail": {
        "GT": "GT_POWER_RAIL"
      }
    }
  ]
}
```


###

----  
### 2.2 ContentSpec Configuration File   

----  
##### Top-Level Tokens
* **Project** - Type:String  
  * The Project/Product that this file is used for. Currently not used.  
* **ContentGroupsDef** - Type:Dictionary\<String, [ContentGroup](#contentgroup-tokens)>  
  * Key=Content Name, must be unique but it is not used/referenced anywhere.    
  * Value=[ContentGroup](#contentgroup-tokens) definition/object.  
  * The ContentGroup object gives a way to categorize failures based on failing pattern names.  
  * this could be a list, the key name doesn't do anything??
* **PowerGroupsDef** - Type:Dictionary\<String, [PowerGroup](#powergroup-tokens)>   
  * Key=CTTGroup Name
  * Value=[PowerGroup](#powergroup-tokens)  
  * The PowerGroup object defines when to tick (increase the voltage) for a given IP based on the CTTGroup names.  

----  
##### ContentGroup Tokens   
* **Tag** - Type:String   
  * The tag is the content type which will be used by the PowerGroup objects.
  * It does NOT have to be unique, multiple ContentGroup objects can use the same tag.
  * It is only used within this file.
* **PatternNames** - Type:List of String
  * This is a list of RegularExpressions which will be run against failing pattern names.  
  * If any match then this Tag is considered active.

----  
##### PowerGroup Tokens   
* **Description** - Type:String   
  * not used anywhere in code.  
* **TickOnTag** - Type:List of String   
  * List of ContentGroup Tags. If any Tag is active then the FailureMapDef will be executed to determine which CTTInstances should tick.  
* **FailureMapDef** - Type:[FailMapWrapper](#failmapwrapper-tokens)   
  * Contains the information needed to map a failure to a specifice CTTInstance.  
* **FlowDef** - Type:List of [FlowDefinition](#flowdefinition-tokens)   
  * Contains the information needed to do the PatConfig SetPoint to setup the CTTGroup/CTTInstance configuration for the test.  

----  
##### FailMapWrapper Tokens   
(only one sub-token is allowed, see FailMapping for more details - TBD) 
* **PinBasedMapDef** - Type:Dictionary\<String, List of String> - Optional   
  * Key=Failing PinName (must match the pin in the pattern exactly, including IP scoping)
  * Value=List of CTTInstances to mark as failing. 
  * As simple pin-based decoder.  
* **PacketService** - Type:PacketBasedMap - Optional    
  * not implemented yet, need more details
* **UserMethod** - Type:UserMethodMap - Optional  
  * Contains a user-defined method to determine the failures.
  * Params - Type:Dictionary\<String, String>
    * A dictionary containing arguments for the Decoder function.
  * Validator - Type:String - Optional
    * the name of a User-Defined method to call to validate the DecoderParams and perform any one-time setup.
    * called when the IConcurrentTestManager object is created.
    * signature: public static void UserFailValidator(string plist, Dictionary\<string, string> parameters);
  * Decoder - Type:String
    * the name of a User-Defined method to call to decode the pattern failures.
    * The return value is a HashSet containing the names of the failing CTT Instances.
    * signature: public static HashSet\<string> UserFailDecoder(IFunctionalTest test, Dictionary\<string, string> parameters);
  * Assembly - Type:String
    * The name of the assembly that contains the Validator and Decoder functions.
    * It must already be loaded into the AppDomain of TestMethod.
  * Class - Type:String
    * The full namespace/name of the class which contains the Validator and Decoder functions.
          


----  
##### FlowDefinition Tokens  
* **Name** - Type:String   
  * The PatConfig SetPoint Group to use to setup the configuration for this CTTGroup/CTTInstance.  
  * The SetPoint Name is generated from the VminForwarding Frequency for this CTTGroup/CTTInsstance.   
* **Units** - Type:String - Optional    
  * Supports "Mhz" and "Ghz".
  * The CTTGroup/CTTInstance frequency will be converted Ghz or Mhz if this is specified and the units will be appended to the name. 
  * There will not be any trailing 0's in the name to match the current format for SetPoints -- 5.4Ghz, 100Mhz, 4.1Ghz, 4Ghz, 0.865Ghz ...  
* **Module** - Type:String - Optional    
  * The PatConfig SetPoint Module to use. If not supplied the Module from the TestProgramSpec CCRModuleName will be used.  
* **Source** - Type:String - Optional    
  * The specific CTTInstance name to get the frequency of. If not specified, the 1st CTTInstance in the group will be used.  
  * This is used when there are different frequencies for different instances in the group (like slow cores and fast cores).  Multiple FlowDefs can be specified with different SetPoint Groups and values.  

----  
Example: 
```json
{
  "Project": "RPLS-282",
  "ContentGroupsDef": {
    "reset": {
      "Tag": "RESET_FAILURE",
      "PatternNames": [
        "^.{37}(01)_"
      ]
    },
    "mid_or_precat": {
      "Tag": "MIDCAT_FAILURE",
      "PatternNames": [
        ".*precat",
        ".*cleanx"
      ]
    },
    "core_sbft": {
      "Tag": "CORE_FAILURE",
      "PatternNames": [
        "^.{39}(01)_",
        "^.{39}(04)_"
      ]
    },
    "core_array": {
      "Tag": "CORE_FAILURE",
      "PatternNames": [
        "^.{39}(0a)_"
      ]
    },
    "ccf_failure": {
      "Tag": "CCF_FAILURE",
      "PatternNames": [
        "^.{39}(08)_"
      ]
    }
  },
  "PowerGroupsDef": {
    "CORES": {
      "Description": "handling BIG Core Ticking def",
      "TickOnTag": [
        "CORE_FAILURE",
        "RESET_FAILURE"
      ],
      "FailureMapDef": {
        "PinBasedMapDef": {
          "xxnoab_08": [
            "CORE0"
          ],
          "xxnoab_09": [
            "CORE1"
          ],
          "xxnoab_10": [
            "CORE2"
          ],
          "xxnoab_11": [
            "CORE3"
          ],
          "xxtdo": [
            "CORE0",
            "CORE1",
            "CORE2",
            "CORE3"
          ]
        }
      },
      "FlowDef": [
        {
          "Name": "fast_core_ratio",
          "Units": "Mhz",
          "Source": "CORE0"
        },
        {
          "Name": "slow_core_ratio",
          "Units": "Ghz",
          "Source": "CORE2"
        }
      ]
    },
    "ATOMS": {
      "TickOnTag": [
        "ATOM_FAILURE",
        "RESET_FAILURE"
      ],
      "FailureMapDef": {
        "PinBasedMapDef": {
          "xxnoab_12": [
            "ATOM0"
          ],
          "xxnoab_13": [
            "ATOM1"
          ],
          "xxnoab_14": [
            "ATOM2"
          ],
          "xxnoab_15": [
            "ATOM3"
          ]
        }
      },
      "FlowDef": [
        {
          "Name": "atom_ratio"
        }
      ]
    },
    "CCF": {
      "TickOnTag": [
        "CORE_FAILURE",
        "CCF_FAILURE"
      ],
      "FailureMapDef": {
        "PacketService": {
          "ConfigurationName": "RPL_CCF_PACKET_SERVICe",
          "Params": {
            "ObservePinValidFlag": "",
            "ObserveDataPins": [
              {
                "CORE0": "NEED_SOME_PIN_AND_MORE_INFO_PAVEL_TO_HELP"
              }
            ]
          }
        }
      },
      "FlowDef": [
        {
          "Name": "ring_ratio"
        }
      ]
    },
    "GT": {
      "TickOnTag": [
        "GT_FAILURE"
      ],
      "FailureMapDef": {
        "UserMethod": {
          "Assembly": "ConcurrentVminTC",
          "Class": "ConcurrentVminTC",
          "Validator": "GtFailValidator",
          "Decoder": "GtFailDecoder",
          "Params": {
            "Arg1": "SomeValue1",
            "Arg2": "SomeValue2"
          }
        }
      },
      "FlowDef": [
        {
          "Name": "gt_ratio"
        }
      ]
    }
  }
}
```

###

----  
### 2.3 BundleSearchSpec Configuration File   
  
----  
##### Top-Level Tokens
* **Project** - Type:String  
  * The Project/Product that this file is used for. Currently not used.  
* **CTTSearchCfg** - Type:Dictionary\<String, [SearchConfiguration](#searchconfiguration-tokens)>  
  * Key=PatternList Name
  * Value=Search Configuration object.
  * This contains the Frequency and SafeZone definitions for each ConcurrentTest PList.   

----  
##### SearchConfiguration Tokens  
* **BundleFreqDef** - Type:Dictionary\<String, String>   
  * Key=CTTGroup Name
  * Value=VminForwarding CornerIdentifier (without the vmin instance -- F1, F2, F3, ...)
  * This is used to get the frequency for each CTTGroup which will be used to create the PatConfig SetPoint to setup the configuration.  
* **SafeZoneInstrumentalDef** - Type:List of [SafeZoneDefinition](#safezonedefinition-tokens)   
  * List of SafeZones to use as the start of each burst. Order does not matter. 
  * When a plist execution fails, the next plist execution will start from the failing patterns previous safe zone.  

----  
##### SafeZoneDefinition Tokens   
* **Name** - Type:String   
  * Pattern name to set as starting point.   
* **PreBurstListName** - Type:String   
  * The value to set the PatternLists PreBurstPList Option.  


----  
Example:
```json
{
  "Project": "RPLS-282",
  "CTTSearchCfg": {
    "plist1": {
      "BundleFreqDef": {
        "CORES": "F1",
        "ATOMS": "F3",
        "GT": "F1",
        "CCF": "F1"
      },
      "SafeZoneInstrumentalDef": [
        {
          "Name": "d1018153F2715487B_Mu_VRB024T_CInm1a000001_a080816xx0800044x21xxad_RB1PeiTC007I0ae_LIk0xA42x0nxt0000xxx_avx_512_randmom_mnemonics_addressing_modes",
          "PreBurstListName": "rpl_reda_161_12t_full_preburstlist"
        },
        {
          "Name": "d1018017F3049200B_Gb_VRB024T_CInm1a000001_a080816xx0800044x21xxad_RB1PeiTC007I0ae_LIk0xA42x0nxp0000000_exe_bf16_cascade_no_ps_denorm_norm_qnan_sn",
          "PreBurstListName": "rpl_reda_161_12t_full_preburstlist"
        },
        {
          "Name": "d1017915F2715107B_MW_VRB024T_CInm1a000001_a080816xx0800044x21xxad_RB1PeiTC007I0ae_LIk0xA42x0nxt0000xxx_avx_512_dependent_followed_independent_ins",
          "PreBurstListName": "rpl_reda_161_12t_full_preburstlist"
        }
      ]
    }
  }
}
```


###

----  
## 3. API Description     

----  
##### Service API   
* TestProgramSpec LoadTestProgramSpec(string fileName)   
  * Loads the TestProgramSpec json file into an object.  
  * If the config file is moved to ALEPH then this can be hidden/removed.  
* BundleSearchSpec LoadBundleSearchSpec(string fileName)   
  * Loads the BundleSearchSpec json file into an object.  
  * If the config file is moved to ALEPH then this can be hidden/removed.  
* ContentSpec LoadContentSpec(string fileName)   
  * Loads the ContentSpec json file into an object.  
  * If the config file is moved to ALEPH then this can be hidden/removed.  
* [IConcurrentTestManager](#iConcurrentTestmanager-api) GetManager(TestProgramSpec tpSpec, BundleSearchSpec bundleSpec, ContentSpec content, string plist)   
  * Creates the IConcurrentTestManager object for the specific plist.
  * If the config files are moved to ALEPH then they should be removed from the parameters.  

----  
##### IConcurrentTestManager API
* void ApplySetPointsForCornerFrequencies();
  * Applies the PatConfig SetPoints specified in the config files for the plist.
  * Uses the following information:
    * From the TestProgramSpec
      * CCRModuleName for the SetPoint Module name.
      * FlowDomain for the FlowId
      * InstanceToVminFwdInstance for the VminForwarding Instance names
    * From the ContentSpec
      * FlowDef for the PatConfig SetPoints Group name, Module name (optional override).
    * From the BundleSearchSpec
      * BundleFreqDef for the VminForwarding CornerId  
* HashSet\<string> GetFailingPowerRails(ICaptureFailureTest funcTest);
  * Returns the Failing PowerRails given the ICaptureFailureTest object.
* void UpdateStartAndPreBurstFromFailure(ICaptureFailureTest funcTest);
  * Updates the ICaptureFailureTest StartPattern and PreBurstPlist for its Plist based on the nearest SafeZone to the current StartingPattern.
  * Use this when extending PrimeVminSearchTestMethod. It can be called in IVminSearchExtensions.ApplyMask to work with basic search and Scoreboard, and takes advantage of the base class automatically setting the start pattern based on the failure.  
* void UpdateStartAndPreBurstFromCurrentStart(ICaptureFailureTest funcTest);
  * Updates the ICaptureFailureTest StartPattern and PreBurstPlist for its Plist based on the nearest SafeZone to the current first failing pattern.
  * Use this when not using PrimeVminSearchTestMethod or if manually preventing it from automatically setting the StartingPattern.
* List\<[PowerRailMapping](#powerrailmapping-fields)> GetPowerRailToVminForwardingCornerMap();
  * Gets all the information about the current tests CTTGroups/CTTInstances VminForwarding and PowerRail information.
* void Reset();
  * Resets the PreBurstPlist plist option for this test.
  * This DOES NOT reset the ICaptureFailureTest StartingPattern, the user must reset that manually (or rely on PrimeVminSearchTestMethod)

----  
##### PowerRailMapping Fields   
* public string PowerRail { get; }
* public string CTTGroup { get; }
* public string CTTInstance { get; }
* public string VminInstance { get; }
* public string VminFrequencyCorner { get; }
* public int Flow { get; }

