[[_TOC_]]

## REP for PacketMonitor

This **REP** is intended to describe the PacketMonitor Prime TestMethod.


## Methodology

Test method executes plist and capture the failure data. The captured data will be processed to build chunks, then from chunks to build packets. Each packet has fields defined by user in the input file. Fields values will be extracted from packets for those main purposes:
* Decoding  plist fails and writing to datalog (Scan or Raster) the values of the fields.
* Find the failing engine ids (EIDs) from the decoded  fails that shall be reported to GFX aggregator to find the SKU, print it, the HRY and all associated GFX prints. (See parameters for all options). 

### DFM Decoding Details
There are two decoding modes: DFM and DMEM. Below we see a detailed example of how DFM decoding works. DMEM explanation will follow on later sections, but the concept is very similar with few differences that we point out in this document.

To decode in DFM mode, the input file should contains this definition:
- "Mode": "DFM"

#### Chunks
Chunk is a binary string constructed from the data pins that captured per one cycle.
Data pins are defined in the input file.

Chunk size is the number of data pins defined.
Multiple chunks form a packet. How many chunks needed? Based on packet size which is also part of the .json input file.

Example from input file for DFM:

    "Pins": {

      "DOAPin": "P0",
      "CounterPins": ["P1","P2"],
      "DataPins": ["P3", "P4", "P5"]
    },
    
    "PacketSettings": {
        "PacketSize": 9,
        ...
        }
 
So in the example above, chunk size is 3 (because this is the number of data pins). Packet side should be multiplication of the number of pins. E.g. 9.

In the above example, possible  chunks can be:

    [P1 P2 P3 P4 P5]
     0  1  1  1  0 
     1  0  0  1  1 
     1  1  0  1  0 


### Counter Pins
Counter pins are used to validate correctness of the chunk that is being created per cycle. Users define what is the starting value of the counter, and order asc/dec. When forming the chunks, the counter needs to behave according to user definition.

#### Example of correct behavior:
     
     "CounterSettings": {
        "InitialValue": "01",
        "Order": "Ascending"
      },

In this example, the expected behavior can be seen in the chunks section above. Values of counter pins should start with value 01 and continue up until 11, and repeat.

#### Example of wrong counter
Let’s assume we are processing capture data of two cycles. First cycle, had those values captured: "P1, P2", "0,1" accordingly.  So the bit string is "01" it matches the "InitialValue" from the .json file. First chunk is valid, we continue to the next cycle. For this cycle we have captured the following data: "P1, P2", "1,1" accordingly. Bit string is “11”, it’s clear that the value of ‘11’ is not the consequent value of ‘01’, since we are expecting to see ‘10’ because “Order” is “Ascending”.

### Packets
Packets are formed from chunks. Each packet has a type. Type is determined based on bit values on certain locations.
Each packet type has fields. Fields describes data that will be extracted from the packet. Each field has start and end bit positions.

Each packet will be decoded and datalogged.

### DMEM (CTV) Decoding
DMEM decoding is similar to DFM, but different in the following ways:
- Only 1 data pin should be provided in input file. No multiple pins allowed.
- No counter pins usage. All counter pins section shouldn't exist in input file.

In this mode, the input file should have this defined:
- "Mode": "DMEM",

Example from input file for DMEM:

    "Pins": {

      "DOAPin": "P0",
      "DataPins": ["P1"]
    },
    
    "PacketSettings": {
        "PacketSize": 9,
        ...
        }
 
So in the example above, we expect the packet side should be multiplication of 9 since that's the packet size.

In the above example, captured CTVs on P1 can be:

    [P1] =  1  1  0  0  1  1  0  1  0 

This will form 1 packet. Note the difference between DMEM and DFM mode, here we get all the data on one pin, and no counter pins exist.

### DOA Failure
In both modes, DFM and DMEM, DOA mode failure check is a DFM check. Even in DMEM mode! (based on requirements from GFX team).

DOA failure happens when a DFM failure captured on DOA pin, before "DOAStartLabel" address (defiend in input file).

**Please note** that DOA pin can be on different domain, than the data pins. In this case, user might see in console that test method decoded some packets on the other domain before test method encounters DOA pin (test method has no control over the order of the domains that are being executed). But when DOA pin failure encountered, by default, decoding will stop and test method will exit from dedicated port (see ports section).

User can change the behavior of DOA pin encounter by explicitly define a different behaviour for DOA failure in Error Handling section. (See below Error Handling section).

When DOA fail encountered, this will be printed to datalog: "DOA_ERROR".

### Overflow failure
User can choose whether a field is considered an overflow indicator field or not, by controlling "Overflow" attribute. For example:

    {
     "Name": "o",
     "Type": "Dec",
     "StartBit": 4,
     "EndBit": 4,
     "OverFlow": "1",   <-- Packet considered as overflow if the value of the field matches the value provided here.
     "IsEid": "NON_EID"
    },

In the example above, if the packet has value of '1' in the 4th bit, then it will be considered as overflow packet, and special token will be printed to spofi after this packet "DSS_ERROR_MBP_OVERFLOW".

If value of the "OverFlow" attribute is empty ("") no overflow check will be performed on this packet. **The value of the "OverFlow" field is expected to be in Decimal base no matter what is the "Type" of the field.**

**Please note** that decoding by default won't stop if user didn't explicity changed the behaviour in the error handling section (See below Error Handling section).

#### Example of packet decoding
### Input snippet for the example (DFM mode)
	{
	  "CaptureSettings": {
		"Domain": "DomainA_All_DPIN_Dig",
		"StartLabel": "start_label",
		"EndLabel": "stop_label",
		"DOAStartLabel: "doa_start_label"
		"Mode": "DFM",
		"MaximumPackets": 1000,
		"Pins": {
		  "DOAPin": "PIN_99",
		  "CounterPins": [
			"PIN_01",
			"PIN_02"
		  ],
		  "DataPins": [
			"PIN_03",
			"PIN_04",
			"PIN_05"
		  ]
		},
		"DecodeSettings": {
		  "CounterSettings": {
			"InitialValue": "01",
			"EndValue": "11",
			"Order": "Ascending"
		  },
		  "PacketSettings": {
			"PacketSize": 9,
			"PacketId": {
				"StartBit": 0,
				"EndBit": 2
			},
			"FieldsMSBDirection": "LEFT",
			"PacketConfiguration": [
				{
					"Id": "110",
					"ReportToGFXAggregator" :  "FALSE", 
					"Name": "DataPacket",
					"Fields": [
						{
							"Name": "myid",
							"Type": "Dec",
							"StartBit": 0,
							"EndBit": 2,
							"OverFlow": "",
							"IsEid": "NON_EID"
						},
						{
							"Name": "eid",
							"Type": "Dec",
							"StartBit": 3,
							"EndBit": 6,
							"OverFlow": "",
							"IsEid": "NON_EID"
						},
						{
							"Name": "address",
							"Type": "Hex",
							"StartBit": 7,
							"EndBit": 8,
							"OverFlow": "",
							"IsEid": "NON_EID"
						},
						]
				}
			}
		}
	  },
	}
							
### DFM Mode: Building packet from chunks
If we take the data pins only from the 3 chunks in the example above, we get this

[ bits from data pins of chunk 1] [ bits .. chunk 2] [bits ...  chunk 3]

[1  1  0 ] [0  1  1 ] [ 0  1  0 ]

Final packet:
110001010

We determine type by looking into bits 0-2 (see above the json under "PacketSettings" section).
bits 0-2: 110

Since it matches "Id" value  of "DataPacket", then we know that we should decode the packet based on that.

This means we need to extract 3 fields from this packet, from different bits locations as can be seen above in the .json snippet.

Final print for this packet to ituff should be:

DSS_myid_110_eid_3_address_1

### DMEM Mode: Building packet
Building packets from captured CTVs (DMEM mode) is more simple. There are no chunks in this case.
All the captured data (after size checks passed) will be taken and split into packets (division in packet size - in our example is 9).

In our example above, the packets is data it self.
If this is the captured fails (CTVS):
[P1] =  1  1  0  0  1  1  0  1  0 

This is the packet:
110001010.

What comes next is the same like DFM... This packet fields will be decoded as explained above and printed to ituff. 

### Other Behaviours
#### All-zeros packet
When a packet created with all 0 bits, this packet will be ignored and it's field won't be decoded.

#### Pre/Post Ambles Reporting
When printing to ituff passing patterns with "NO_FAIL", pre/post ambles won't be included in the reporting.

#### Error Handling
It's possible to control test method behaviour when encountering specific kind of error, to either stop decoding or continue.

The default error handling defined by the test method is as follow:

| **Error Type** | **Behaviour** | **Scan Datalog Print** | **Notes** |
| -------------- | ------------- | --------- | --------- |
| DOA Fail       | Stop decoding.|DOA_ERROR | |
| Max packets reached | Stop decoding.|DSS_LIMIT_CATASTROPHIC | Max packets defined in input file. It's overall packets per plist.|
| All-zeros packet | Stop decoding. |None will be printed.| The packet won't be decoded (fields not datalogged). It's no fail case but decoding will be stopped. Exit port won't be affected. E.g. if no other errors encountered, port will be 2. See below. |
|Invalid packet Id | Continue. | DSS_ERROR_UNKNOWN_PACKET_[packetData] | |
|Invalid counter | Continue. | DSS_ERROR_INVALID_COUNTER_[chunksData] | Relevant only in DFM mode.|
|Overflow | Continue. |  DSS_ERROR_MBP_OVERFLOW | |

To override default behavour, user can define this section in the input file:

        "ErrorHandlingPerPacket": [
          {
            "PacketErrorType": "Overflow/InvalidId/InvalidCounter/DoaError/MaxAmountOfPacketsReached",
            "BehaviourType": "CONTINUE/NEXT_PATTERN/END_PATTERNS_RETEST_ON/END_PATTERNS_RETEST_OFF",
            "AdditionalPacketsToDecode": 10
          },
          ...
        ]

| IMPORTANT NOTE |
| --- |
| Not all behaviour supported on current code.|
| [1] AdditionalPacketsToDecode field has no impact. No implementation of the intended feature in current code. |
| [2] NEXT_PATTERN behaviour type has no impact, when used, current code will behave same as CONTINUE. |
### Verify

Test method will validate all inputs such as parameters and input file. It will parse the input file and prepare all the needed data structures. It will also setup the functional test, read the patterns names and amble names (for purpose of datalogging - see section below). 

### Execute
Test method will execute the plist and get failure data. Those failures will be decoded as described above, and will  be datalogged according to chosen datalog target (see parameters). 

If “GFXAggregation” PH parameter is set to true, test method will use GFX Service to perform the following:
* Write/update tag with EIDs status 
* Calculate SKU
* Write SKU to GSDS 
* Datalog SKU, HRY Raw, and HRY Tree.


## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the PacketMonitor test method

| **Parameter Name**            | **Required?** | **Type**        | **Description**                                                                                        | **Default value**                  | **Comments**                       			|
| ------------------            | ------------- | --------------- | ------------------------------------------------------------------------------------------------------ | ---------------------------------- | -------------------------------------------   |
| Patlist                       | Yes           | Plist           | Plist name to be executed                                                                              |                                    |                                    			|
| TimingsTc                     | Yes           | TimingCondition | Levels test condition required for plist execution                                                     |                                    |                                    			|
| LevelsTc                      | Yes           | LevelsCondition | Timing test condition required for plist execution                                                     |                                    |                                   			|
| PrePlist                      | No            | String          | PrePlist name to be executed                                                                           |                                    |                                   			|
| MaskPins                      | No            | String          | Comma separated list of pins for which the fail data capture will be skipped                           |                                    |                                  				|
| PacketDatalogMode                   | No           | String          | Datalog to print decoded packets into. Options: Scan/Raster/Disabled.                                     |  Disabled.                                  |                                   			|
| ConfigFile		            | Yes           | File            | Resource Json file path                                                                                |                                    |                                   			|
| MaximumTotalFailures		        | NO            | UnsignedInteger | Number of maximum failures to capture                                                	               |                                    |                                   			|
| Area                          | NO            | String          | Specify the area name to which the recovery groups and SKUs are relevant                               |                                    |                                   			|
| Content                       | NO            | String          | Specify the content name to which the recovery groups and SKUs are relevant                            |                                    |Content must be unique per area     			|
| Tag                           | NO            | String          | Specify the tag to mark the EID’s status of current instance in SharedStorage                          |                                    |                                  			    |
| StartSku                      | NO            | String          | Specify the start SKU for evaluation                                                                   |                                    |                                   		    |
| EndSku                        | NO            | String          | Specify the end SKU for evaluation                                                                     |                                    |                                    			|
| ResultSkuKey                  | NO            | String          | Specify the name of the result SKU that will be written to SharedStorage                               |                                    |                                    			|
| FailRecoveryGroupsKey         | NO            | String          | Name of the fail recovery groups for the evaluated SKU that will be written to SharedStorage           |                                    |												|
| HryRawStringDatalog           | NO            | String          | Specify if HRY raw string is to be datalog                                                             |default ENABLE                      |ENABLE/DISABLE									|
| HryTreeLevelDatalog           | NO            | String          | Specify up to which HRY tree level to datalog.                                                         |default -1                          |means there will be no HRY tree level datalog. |

## Console output (debug mode)

None.

## Datalog output

See above examples given in different sections.

## Custom User Code Hooks

None.

## TPL Samples

Here are a few test instance examples using the PacketMonitor test method

	Test PrimePacketMonitorTestMethod PacketMonitorTest_FNeg1
	{
		Patlist = "gfx_doa_list";
		LevelsTc = "Gfx::basic_func_lvl_nom";
		TimingsTc = "Gfx::basic_func_timing_10MHz_20MHz";
		LogLevel = "Enabled";
		Area = "GT_PRIME";
		StartSku = "";
		EndSku = "";
		Tag = "PacketMonitorTest";
		ResultSkuKey = "PacketMonitorTest_SKU_Result";
		HryRawStringDatalog = "ENABLED";
		HryTreeLevelDatalog = -1;
		Content = "SCAN";
		ConfigFile = "~HDMT_TPL_DIR/TestPrograms/Gfx/Modules/Gfx/InputFiles/PacketMonitor_ValidInputFile.json";
	}



## Exit Ports

The PacketMonitor test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**                              |
| ------------- | ------------- | -------------------------------------------- |
| **0**         | ***Fail***    | Decoding failure. 		 				   |
| **1**         | ***Pass***    | Passing condition.                           |
| **2**         | ***Fail***    | Packets created successfully, no errors occured during decoding/instance run.|
| **3**         | ***Fail***    | DOA pin failure.		   |
| **4**         | ***Fail***    | Overflow pin failure.     |
| **5**         | ***Fail***    | Chunks counter error.     |
| **6**         | ***Fail***    | Max packets reached.      |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SHOPS**: **Sh**orts and **Op**en**s** test methodology

## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**    |
| ------------------------- | ----------- | ----------------- | --------------- |
| Mar 28<sup>st</sup>, 2022 | 1.0.0       | Dakwar, Wajde	  | Initial version |
| Apr 30<sup>st</sup>, 2023 | 1.1.0       | Dakwar, Wajde	  | Added documentation of all main features. |
| Jul 03<sup>st</sup>, 2023 | 1.2.0       | Dakwar, Wajde	  | Added documentation of DMEM mode (CTV) and other new features. |