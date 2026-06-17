[[_TOC_]]

## REP for Electrical Z Alignment

This **REP** is intended to describe the Electrical Z Alignment Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Datalog output** – A detailed description of what is datalogged by his TestMethod
  
  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document
## Methodology

The Electrical Z Alignment test method is intented to be used by the Station Controller to automatically adjust the initial position of the tester's wafer chuck until all pins of the DUT make contact with the wafer bumps. The user var TP_ZHEIGHT_INST should contain the name of an instance of this test method and it will be triggered by TOS. This TM performs DC opens and shorts tests and sets the test result in the SC_EZA_TEST_RESULT user var as well as set the value of the pins failing the opens on the SC_EZA_OPEN_PINS user vars. These user vars are then read by TOS to determine whether the instance should be triggered again or not.
These tests can be performed on individual pins or pin groups.

Here is a flow chart with the logic of the Verify of this test method:
### Verify
::: mermaid
graph  TB
A(Start) --> B[Read Electrical Z Alignment Json setup file]
   B -->C{Levels found?} -->|No| G(Throw exception)
   C -->|Yes| D[Initialize TM Members] --> E(End)
:::
  
Likewise, here is the Execute Flow:
### Execute
![image.png](./.attachments/EZA_Execute.png)

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Contact Resistance test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                                                                           | **Comments**                 |
| ------------------ | ------------- | --------------- | ------------------------------------------------------------------------------------ | ---------------------------- |
| LevelsTc           | Yes           | LevelsCondition | Levels TC required to setup and perform the Contact Resistance measurement.                           |                              | |
| SetupFilePath       | Yes            | String | Path to the json setup file. | 
|                    |               |     
| ItuffDataloggingEnabled       | No            | String | TRUE or FALSE. | Determines whether the pins failed the short or opens tests.
|                    |               | 
### Levels File
The DC measurements setup is implemented in Levels block.
For this, each measured pin or pin group has its own setup which has an OPMode of type ISVM and should be measured in using the  *StartMeasurement* attribute. For this Test Method, the pins or pin groups must have one and only one setup. Additionally, the set up for individual pins should be individual and not the setup of a pin group containing that pin.

Here is the example of measurement setup in level block:

```python

Levels ElectricalZAlignmentLevel
{
    ###### Pin group 1  ######
    DomainA_DPIN_PMU
    {
        OPMode = "ISVM";
        IRange = "IR1mA";
        IForce = 0.001A;
        VClampHi = 2V;
        VClampLo = -1.5V;
        StartMeasurement = True; # Attribute that triggers measurement on the supply. 
        PreMeasurementDelay = 100us; # Time to wait from VForce before collecting measurements.
        SamplingCount = 1;
        SamplingRatio = 1; 
        SamplingMode = "Trace";
        PinModeSel = "PMU";
    }
    SequenceBreak 1mS;
    ###### Pin 1  ######
    xxHPCC_DPIN_PMU_slcB_Short
    {
        OPMode = "ISVM";
        IRange = "IR1mA";
        IForce = 0.001A;
        VClampHi = 2V;
        VClampLo = -1.5V;
        StartMeasurement = True;
        PreMeasurementDelay = 20us;
        SamplingCount = 1;
        SamplingRatio = 1;
        PinModeSel = "PMU";
    }
}

```
### Setup file
The setup file is a json file with the following format.
```python
{
  "Pins": [
    {
      "Channel": 8056,
      "Name": "xxHPCC_DPIN_PMU_slcB_Short",
      "OpensMask": false, # optional parameter, default value => false
      "ShortsMask": false # optional parameter, default value => false
    },
    ...
   ],
   "PinGroups": [
     {
       "Name": "xxHPCC_DPIN_PMU_slcB_Short", # This can be a pin or a pin group
       "ShortsLimit": 0.25,
       "OpensLimit": 1.49
     },
     ...
  ]
}
```
Notes:
1. The tests will be performed on all pins belonging to any of the Pin Groups.
1. Pins section can be empty. When there is no configuration matching both, the name and channel number of a pin that is part of one of the defined pin groups, the opens and short tests will be performed by default. 

## Datalog output

Electrical Z Alignment results are logged to Ituff in “composite” format as shown below
```python
2_tname_ElectricalZAlignment::PrimeElectricalZAlignmentTestMethod_Sample_P1EZA_touchdata
2_category_zheight
2_composite_12_13.0000000
2_msunit_M
```

When the shorts fail, the failing channels are printed to the ituff in the following way:
```python
2_tname_ElectricalZAlignment::PrimeElectricalZAlignmentTestMethod_Sample_P1_Opens
2_faildata_8000,8001,8003,8004,8005,8006,8007,8008,8009,8010,8011,8012,8013,8014,8015,8057
2_tname_ElectricalZAlignment::PrimeElectricalZAlignmentTestMethod_Sample_P1_Shorts
2_faildata_8015
2_tname_ElectricalZAlignment::PrimeElectricalZAlignmentTestMethod_Sample_P1_Opens
2_faildata_8057
```
In a similar way, the channels for pins failing the opens test are printed too:
```python
2_comnt_Instance_ElectricalZAlignment::PrimeElectricalZAlignmentTestMethod_Sample_P1_execution_1_status_OPEN_failed_pins_xxHPCC_DPIN_PMU_slcA_Short,xxHPCC_DPIN_PMU_slcA_50ohm1,xxHPCC_DPIN_PMU_slcA_50ohm3,xxHPCC_DPIN_PMU_slcA_100ohm,xxHPCC_DPIN_PMU_slcA_200ohm,xxHPCC_DPIN_PMU_slcA_1Kohm,xxHPCC_DPIN_PMU_slcA_2Kohm,xxHPCC_DPIN_PMU_slcA_10Kohm,xxHPCC_DPIN_PMU_slcA_20Kohm,xxHPCC_DPIN_PMU_slcA_180Kohm,xxHPCC_DPIN_PMU_slcA_1Mohm1,xxHPCC_DPIN_PMU_slcA_1Mohm2,xxHPCC_DPIN_PMU_slcA_DiodeNeg,xxHPCC_DPIN_PMU_slcA_DiodePos,xxHPCC_DPIN_PMU_slcA_Open,xxHPCC_DPIN_PMU_slcB_50ohm1
```

## TPL Samples

Here are a few test instance examples using the Electrical Z Alignment test method:

```python
Import PrimeElectricalZAlignmentTestMethod.xml;
Test PrimeElectricalZAlignmentTestMethod PrimeElectricalZAlignmentTestMethod_ZHeightStatusIsContact_P1
{
   LevelsTc = "ElectricalZAlignment::SecondElectricalZAlignmentTestMethodTC";
   LogLevel = "PRIME_DEBUG";
   ItuffDataloggingEnabled = "TRUE";
   SetupFilePath = "~HDMT_TPL_DIR/Modules/ElectricalZAlignment/ElectricalZAlignment/InputFiles/DefaultSetup.json";
}
```
Here is an example of a setup Json file:
```python
{
  "Pins": [
    {
      "Channel": 8000,
      "Name": "xxHPCC_DPIN_PMU_slcA_Short",
      "OpensMask": false,
      "ShortsMask": true
    },
    {
      "Channel": 8001,
      "Name": "xxHPCC_DPIN_PMU_slcA_50ohm1",
      "OpensMask": false,
      "ShortsMask": true
    },
    {
      "Channel": 8002,
      "Name": "xxHPCC_DPIN_PMU_slcA_50ohm2",
      "OpensMask": true
    },
    {
      "Channel": 8003,
      "Name": "xxHPCC_DPIN_PMU_slcA_50ohm3"
    },
    {
      "Channel": 8004,
      "Name": "xxHPCC_DPIN_PMU_slcA_100ohm"
    },
    {
      "Channel": 8005,
      "Name": "xxHPCC_DPIN_PMU_slcA_200ohm"
    },
    {
      "Channel": 8006,
      "Name": "xxHPCC_DPIN_PMU_slcA_1Kohm"
    },
    {
      "Channel": 8007,
      "Name": "xxHPCC_DPIN_PMU_slcA_2Kohm"
    },
    {
      "Channel": 8008,
      "Name": "xxHPCC_DPIN_PMU_slcA_10Kohm"
    },
    {
      "Channel": 8009,
      "Name": "xxHPCC_DPIN_PMU_slcA_20Kohm"
    },
    {
      "Channel": 8010,
      "Name": "xxHPCC_DPIN_PMU_slcA_180Kohm"
    },
    {
      "Channel": 8011,
      "Name": "xxHPCC_DPIN_PMU_slcA_1Mohm1"
    },
    {
      "Channel": 8012,
      "Name": "xxHPCC_DPIN_PMU_slcA_1Mohm2"
    },
    {
      "Channel": 8013,
      "Name": "xxHPCC_DPIN_PMU_slcA_DiodeNeg"
    },
    {
      "Channel": 8014,
      "Name": "xxHPCC_DPIN_PMU_slcA_DiodePos"
    },
    {
      "Channel": 8015,
      "Name": "xxHPCC_DPIN_PMU_slcA_Open"
    },
    {
      "Channel": 9000,
      "Name": "xxHPCC_DPIN_PMU_slcA_Short"
    },
    {
      "Channel": 9001,
      "Name": "xxHPCC_DPIN_PMU_slcA_50ohm1"
    },
    {
      "Channel": 9002,
      "Name": "xxHPCC_DPIN_PMU_slcA_50ohm2"
    },
    {
      "Channel": 9003,
      "Name": "xxHPCC_DPIN_PMU_slcA_50ohm3"
    },
    {
      "Channel": 9004,
      "Name": "xxHPCC_DPIN_PMU_slcA_100ohm"
    },
    {
      "Channel": 9005,
      "Name": "xxHPCC_DPIN_PMU_slcA_200ohm"
    },
    {
      "Channel": 9006,
      "Name": "xxHPCC_DPIN_PMU_slcA_1Kohm"
    },
    {
      "Channel": 9007,
      "Name": "xxHPCC_DPIN_PMU_slcA_2Kohm"
    },
    {
      "Channel": 9008,
      "Name": "xxHPCC_DPIN_PMU_slcA_10Kohm"
    },
    {
      "Channel": 9009,
      "Name": "xxHPCC_DPIN_PMU_slcA_20Kohm"
    },
    {
      "Channel": 9010,
      "Name": "xxHPCC_DPIN_PMU_slcA_180Kohm"
    },
    {
      "Channel": 9011,
      "Name": "xxHPCC_DPIN_PMU_slcA_1Mohm1"
    },
    {
      "Channel": 9012,
      "Name": "xxHPCC_DPIN_PMU_slcA_1Mohm2"
    },
    {
      "Channel": 9013,
      "Name": "xxHPCC_DPIN_PMU_slcA_DiodeNeg"
    },
    {
      "Channel": 9014,
      "Name": "xxHPCC_DPIN_PMU_slcA_DiodePos"
    },
    {
      "Channel": 9015,
      "Name": "xxHPCC_DPIN_PMU_slcA_Open"
    },
    {
      "Channel": 8056,
      "Name": "xxHPCC_DPIN_PMU_slcB_Short",
      "OpensMask": true,
      "ShortsMask": true
    },
    {
      "Channel": 8057,
      "Name": "xxHPCC_DPIN_PMU_slcB_50ohm1",
      "OpensMask": false,
      "ShortsMask": true
    },
    {
      "Channel": 8058,
      "Name": "xxHPCC_DPIN_PMU_slcB_50ohm2",
      "OpensMask": true
    },
    {
      "Channel": 8059,
      "Name": "xxHPCC_DPIN_PMU_slcB_50ohm3"
    },
    {
      "Channel": 8060,
      "Name": "xxHPCC_DPIN_PMU_slcB_100ohm"
    },
    {
      "Channel": 8061,
      "Name": "xxHPCC_DPIN_PMU_slcB_200ohm"
    },
    {
      "Channel": 8062,
      "Name": "xxHPCC_DPIN_PMU_slcB_1Kohm"
    },
    {
      "Channel": 8063,
      "Name": "xxHPCC_DPIN_PMU_slcB_2Kohm"
    },
    {
      "Channel": 8064,
      "Name": "xxHPCC_DPIN_PMU_slcB_10Kohm"
    },
    {
      "Channel": 8065,
      "Name": "xxHPCC_DPIN_PMU_slcB_20Kohm"
    },
    {
      "Channel": 8066,
      "Name": "xxHPCC_DPIN_PMU_slcB_180Kohm"
    },
    {
      "Channel": 8067,
      "Name": "xxHPCC_DPIN_PMU_slcB_1Mohm1"
    },
    {
      "Channel": 8068,
      "Name": "xxHPCC_DPIN_PMU_slcB_1Mohm2"
    },
    {
      "Channel": 8069,
      "Name": "xxHPCC_DPIN_PMU_slcB_DiodeNeg"
    },
    {
      "Channel": 8070,
      "Name": "xxHPCC_DPIN_PMU_slcB_DiodePos"
    },
    {
      "Channel": 8071,
      "Name": "xxHPCC_DPIN_PMU_slcB_Open"
    },
    {
      "Channel": 9056,
      "Name": "xxHPCC_DPIN_PMU_slcB_Short",
      "OpensMask": true,
      "ShortsMask": true
    },
    {
      "Channel": 9057,
      "Name": "xxHPCC_DPIN_PMU_slcB_50ohm1",
      "OpensMask": false,
      "ShortsMask": true
    },
    {
      "Channel": 9058,
      "Name": "xxHPCC_DPIN_PMU_slcB_50ohm2",
      "OpensMask": true
    },
    {
      "Channel": 9059,
      "Name": "xxHPCC_DPIN_PMU_slcB_50ohm3"
    },
    {
      "Channel": 9060,
      "Name": "xxHPCC_DPIN_PMU_slcB_100ohm"
    },
    {
      "Channel": 9061,
      "Name": "xxHPCC_DPIN_PMU_slcB_200ohm"
    },
    {
      "Channel": 9062,
      "Name": "xxHPCC_DPIN_PMU_slcB_1Kohm"
    },
    {
      "Channel": 9063,
      "Name": "xxHPCC_DPIN_PMU_slcB_2Kohm"
    },
    {
      "Channel": 9064,
      "Name": "xxHPCC_DPIN_PMU_slcB_10Kohm"
    },
    {
      "Channel": 9065,
      "Name": "xxHPCC_DPIN_PMU_slcB_20Kohm"
    },
    {
      "Channel": 9066,
      "Name": "xxHPCC_DPIN_PMU_slcB_190Kohm"
    },
    {
      "Channel": 9067,
      "Name": "xxHPCC_DPIN_PMU_slcB_1Mohm1"
    },
    {
      "Channel": 9068,
      "Name": "xxHPCC_DPIN_PMU_slcB_1Mohm2"
    },
    {
      "Channel": 9069,
      "Name": "xxHPCC_DPIN_PMU_slcB_DiodeNeg"
    },
    {
      "Channel": 9070,
      "Name": "xxHPCC_DPIN_PMU_slcB_DiodePos"
    },
    {
      "Channel": 9071,
      "Name": "xxHPCC_DPIN_PMU_slcB_Open"
    }
  ],
  "PinGroups": [
    {
      "Name": "DomainA_DPIN_PMU",
      "ShortsLimit": 0.2,
      "OpensLimit": 1.49
    },
    {
      "Name": "xxHPCC_DPIN_PMU_slcB_Short",
      "ShortsLimit": 0.25,
      "OpensLimit": 8.49
    },
    {
      "Name": "xxHPCC_DPIN_PMU_slcB_50ohm1",
      "ShortsLimit": 0.15,
      "OpensLimit": 0.99
    }
  ]
}
```
Note: The fields OpensMask, ShortsMask and ContactResistanceMask are optional.

## Exit Ports

The Contact Resistance test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**                                                         |
| ------------- | ------------- | ------------------------------------------------------------------------|
| **-2**        | ***Alarm***   | Any alarm condition                                                     |
| **-1**        | ***Error***   | Any software condition error                                            |
| **0**         | ***Fail***    | Failing condition.                                                      |
| **1**         | ***Pass***    | All Opens, shorts and contact resistance tests pass condition.          |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SHOPS**: **Sh**orts and **Op**en**s** test methodology
  - **DC: direct current**
  - **EZA: Electrical Z Alignment**
## Version tracking

| **Date**                   | **Version** | **Author**        | **Comments**    |
| -------------------------- | ----------- | ----------------- | --------------- |
| Jan  31<sup>st</sup>, 2022 | 1.0.0       | Andrea Gomez      | Initial version |
| June  8<sup>th</sup>, 2023 | 1.0.3       | Andrea Gomez      | Pins list in configuration file can now be empty or have duplicated pin names and channel numbers. |
| Sept 13<sup>th</sup>, 2023 | Prime 12.02.02 | Khai Jie, Teoh | include fail pin name and fail channel printing to ituff.<br> #39827 |
| Nov  21<sup>th</sup>, 2023 | Prime 12.03.00 | Khai Jie, Teoh | Disable Witch Project '2_tname_failchannel' and '3_binpinfails' printing to ituff.<br> #46008 |