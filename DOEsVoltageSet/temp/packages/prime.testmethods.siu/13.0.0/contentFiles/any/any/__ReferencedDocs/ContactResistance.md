[[_TOC_]]

## REP for Contact Resistance

This **REP** is intended to describe the Contact Resistance Prime TestMethod.

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

The Contact Resistance test method provides capability to perform DC opens and shorts tests, as well as, Contact Resistance tests.
These tests can be performed on individual pins or pin groups.

### Verify
![image.png](./.attachments/CRESVerify.png)
  

### Execute
![image.png](./.attachments/CRESExecute.png)

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Contact Resistance test method:

| **Parameter Name** | **Required?** | **Type**        | **Values**                                                                           | **Comments**                 |
| ------------------ | ------------- | --------------- | ------------------------------------------------------------------------------------ | ---------------------------- |
| LevelsTc           | Yes           | LevelsCondition | Levels TC required to setup and perform the Contact Resistance measurement                           |                              | |
| SetupFilePath       | Yes            | String | Path to the setup file | 
|                    |               |     

### Levels File
The DC measurements setup is implemented in Levels block.
For this, each measured pin or pin group has its own setup which has an OPMode of type ISVM and should be measured in using the  *StartMeasurement* attribute. For this Test Method, the pins or pin groups must have one and only one setup. Additionally, the set up for individual pins should be individual and not the setup of a pin group containing that pin.

Here is the example of measurement setup in level block:

```python

Levels ContactResistanceLevel
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
The setup file is a json file with the following format:
```python
{
  "XIUs": [
    {
      "Name": "XIU_1",
      "Pins": [
        {
          "Channel": 8056,
          "Name": "xxHPCC_DPIN_PMU_slcB_Short",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false, # optional parameter, default value => false
          "ShortsMask": true, # optional parameter, default value => false
          "ContactResistanceMask": true # optional parameter, default value => false
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
    },
    {
      "Name": "XIU_2",
		...
      ]
    }
  ]
}
```

As you can see, the file contains a list of XIU objects that contain the following fields:
1. Name. Contains XIU names or a regex. Note: When using Regex, the first one that matches the XIU name will be the one used so if a regex is more specific, it should come before the regex that is more general. Additionally, if no name or regex matches the current XIU name, an exception will be thrown.
1. Pin Groups. Its the list of pin groups to which the shorts, opens and contact resistance tests will be performed.
1. Pins. It is a list of pin specific configurations. It can be empty and can also contain pins with duplicated names and channel numbers. If a Pin is specified in this section but it does not belong to any of the Pin Groups, no test will be perfomed on it. On the contrary, if a pin that belongs to a pin group does not have any configuration matching both, its name and its channel number, the contact resistance test for that pin will be masked by default and the Opens and shorts test will be executed. 


## Datalog output

Contact Resistance results are logged to Ituff in “composite” format as shown below
```python
2_tname_ContactResistance::PrimeContactResistanceTestMethod_AllTestsPass_P1_TESTSHORTSOPENS_DC
2_category_pc
2_composite_8000_0.3999901
2_composite_8001_3.3999825
2_composite_8002_0.3899956
2_composite_8003_0.0000000
2_composite_8004_1.9999886
2_composite_8005_5.0000000
2_composite_8006_0.0569344
2_composite_8007_0.3399849
2_composite_8008_0.3399849
2_composite_8009_0.3399849
2_composite_8010_0.3399849
2_composite_8011_0.3399849
2_composite_8012_0.3399849
2_composite_8013_0.3399849
2_composite_8014_0.3399849
2_composite_8015_0.3399849
2_msunit_V
```

When any of the tests fail, the raw voltage measures printed with postfix "TESTSHORTSOPENS_DC" will be separated under 2 categories: pc (passing channel) and fc (failing channels).
```python
2_tname_ContactResistance::PrimeContactResistanceTestMethod_AllTestsFail_F2_TESTSHORTSOPENS_DC
2_category_pc
2_composite_8001_1.3999939
2_composite_8005_5.0000000
2_msunit_V
2_tname_ContactResistance::PrimeContactResistanceTestMethod_AllTestsFail_F2_TESTSHORTSOPENS_DC
2_category_fc
2_composite_8000_1.3999939
2_composite_8002_1.8999863
2_composite_8003_0.0000000
2_composite_8004_1.9999886
2_composite_8006_1.0569191
2_composite_8007_1.3999939
2_composite_8008_1.3999939
2_composite_8009_1.3999939
2_composite_8010_1.3999939
2_composite_8011_1.3999939
2_composite_8012_1.3999939
2_composite_8013_1.3999939
2_composite_8014_1.3999939
2_composite_8015_1.3999939
2_msunit_V
```

The channels failing the shorts test are printed to the ituff in the following way:
```python
2_tname_ContactResistance::PrimeContactResistanceTestMethod_AllTestsFail_F2_shorts
2_faildata_{8000,8002,8004,8006,8007,8008,8009,8010,8011,8012,8013,8014,8015}
```
In a similarly, the channels for pins failing the opens test are printed too:
```python
2_tname_ContactResistance::PrimeContactResistanceTestMethod_OpensAndCresTestsFail_F3_opens
2_faildata_{8009}
```
Additionally, when the Contact Resistance fail the failing pins will be printed:
```python
2_tname_ContactResistance::PrimeContactResistanceTestMethod_OpensAndCresTestsFail_F3_fpr1
2_strgval_12|xxHPCC_DPIN_PMU_slcA_Short|1405.46729478711|xxHPCC_DPIN_PMU_slcA_50ohm3|899.066062|xxHPCC_DPIN_PMU_slcA_1Kohm|1113.86949768945|xxHPCC_DPIN_PMU_slcA_2Kohm|1188.63397179883|xxHPCC_DPIN_PMU_slcA_10Kohm|1403.47794979883|xxHPCC_DPIN_PMU_slcA_20Kohm|6241.46609579883
2_tname_ContactResistance::PrimeContactResistanceTestMethod_OpensAndCresTestsFail_F3_fpr2
2_strgval_12|xxHPCC_DPIN_PMU_slcA_180Kohm|1175.18626979883|xxHPCC_DPIN_PMU_slcA_1Mohm1|1246.54130479883|xxHPCC_DPIN_PMU_slcA_1Mohm2|1380.11790979883|xxHPCC_DPIN_PMU_slcA_DiodeNeg|1219.26510779883|xxHPCC_DPIN_PMU_slcA_DiodePos|1412.53505079883|xxHPCC_DPIN_PMU_slcA_Open|1216.60007979883
```
Finally, the total number of pins passing/failing the shorts and opens tests are printed to the ituff. 
```python
2_tname_ContactResistance::PrimeContactResistanceTestMethod_OpensAndCresTestsFail_F3_FAILING_OPENS
2_mrslt_1
2_tname_ContactResistance::PrimeContactResistanceTestMethod_OpensAndCresTestsFail_F3_FAILING_SHORTS
2_mrslt_0
2_tname_ContactResistance::PrimeContactResistanceTestMethod_OpensAndCresTestsFail_F3_PASSING_OPENS
2_mrslt_11
2_tname_ContactResistance::PrimeContactResistanceTestMethod_OpensAndCresTestsFail_F3_PASSING_SHORTS
2_mrslt_13
```

## TPL Samples

Here are a few test instance examples using the Contact Resistance test method:

```python
Import PrimeContactResistanceTestMethod.xml;
Test PrimeContactResistanceTestMethod PrimeContactResistanceTestMethod_AllTestsPass_P1
{
   LevelsTc = "ContactResistance::SecondContactResistanceTestMethodTC";
   SetupFilePath = "~HDMT_TPL_DIR/Modules/ContactResistance/ContactResistance/InputFiles/AllTestsPassV2.json";
}
```
Here is an example of a setup Json file:
```python
{
  "XIUs": [
    {
      "Name": "SPHT04100010",
      "Pins": [
        {
          "Channel": 8056,
          "Name": "xxHPCC_DPIN_PMU_slcB_Short",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": false,
          "ContactResistanceMask": false
        },
        {
          "Channel": 8057,
          "Name": "xxHPCC_DPIN_PMU_slcB_50ohm1",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": true,
          "ShortsMask": true,
          "ContactResistanceMask": true
        },
		{
          "Channel": 8000,
          "Name": "xxHPCC_DPIN_PMU_slcA_Short",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": false,
          "ContactResistanceMask": false
        },
        {
          "Channel": 8001,
          "Name": "xxHPCC_DPIN_PMU_slcA_50ohm1",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": true,
          "ShortsMask": true,
          "ContactResistanceMask": true
        },
        {
          "Channel": 8002,
          "Name": "xxHPCC_DPIN_PMU_slcA_50ohm2",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": true,
          "ShortsMask": false,
          "ContactResistanceMask": true
        },
        {
          "Channel": 8003,
          "Name": "xxHPCC_DPIN_PMU_slcA_50ohm2",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": true,
          "ShortsMask": false,
          "ContactResistanceMask": true
        },
        {
          "Channel": 8002,
          "Name": "xxHPCC_DPIN_PMU_slcA_50ohm3",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": true,
          "ContactResistanceMask": false
        },
        {
          "Channel": 8003,
          "Name": "xxHPCC_DPIN_PMU_slcA_50ohm3",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": true,
          "ContactResistanceMask": false
        },
        {
          "Channel": 8004,
          "Name": "xxHPCC_DPIN_PMU_slcA_100ohm",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": true,
          "ShortsMask": false,
          "ContactResistanceMask": true
        },
        {
          "Channel": 8005,
          "Name": "xxHPCC_DPIN_PMU_slcA_200ohm",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": true,
          "ShortsMask": false,
          "ContactResistanceMask": true
        },
        {
          "Channel": 8006,
          "Name": "xxHPCC_DPIN_PMU_slcA_1Kohm",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": true,
          "ContactResistanceMask": false
        },
        {
          "Channel": 8007,
          "Name": "xxHPCC_DPIN_PMU_slcA_2Kohm",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": false,
          "ContactResistanceMask": false
        },
        {
          "Channel": 8008,
          "Name": "xxHPCC_DPIN_PMU_slcA_10Kohm",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": false,
          "ContactResistanceMask": false
        },
        {
          "Channel": 8009,
          "Name": "xxHPCC_DPIN_PMU_slcA_20Kohm",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": false,
          "ContactResistanceMask": false
        },
        {
          "Channel": 8010,
          "Name": "xxHPCC_DPIN_PMU_slcA_180Kohm",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": false,
          "ContactResistanceMask": false
        },
        {
          "Channel": 8011,
          "Name": "xxHPCC_DPIN_PMU_slcA_1Mohm1",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": false,
          "ContactResistanceMask": false
        },
        {
          "Channel": 8012,
          "Name": "xxHPCC_DPIN_PMU_slcA_1Mohm2",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": false,
          "ContactResistanceMask": false
        },
        {
          "Channel": 8013,
          "Name": "xxHPCC_DPIN_PMU_slcA_DiodeNeg",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": false,
          "ContactResistanceMask": false
        },
        {
          "Channel": 8014,
          "Name": "xxHPCC_DPIN_PMU_slcA_DiodePos",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": false,
          "ContactResistanceMask": false
        },
        {
          "Channel": 8015,
          "Name": "xxHPCC_DPIN_PMU_slcA_Open",
          "BaseReading": 0,
          "ContactResistanceLimit": 999,
          "OpensMask": false,
          "ShortsMask": false,
          "ContactResistanceMask": false
        }
      ],
      "PinGroups": [
        {
          "Name": "xxHPCC_DPIN_PMU_slcB_Short",
          "xxHPCC_DPIN_PMU_slcB_Short": null,
          "ShortsLimit": 0.25,
          "OpensLimit": 1.49
        },
        {
          "Name": "DomainA_DPIN_PMU",
          "ShortsLimit": 0.25,
          "OpensLimit": 1.49
        }
      ]
    },
    {
      "Name": "TLZNXX.XXXXX",
      "Pins": [
        {
          "Channel": 1029,
          "Name": "AUDCLK",
          "BaseReading": 0,
          "ContactResistanceLimit": 999999,
          "OpensMask": false,
          "ShortsMask": true
        },
        {
          "Channel": 1005,
          "Name": "AUDIN",
          "BaseReading": 0,
          "ContactResistanceLimit": 999999,
          "OpensMask": true,
          "ContactResistanceMask": false
        }
      ],
      "PinGroups": [
        {
          "Name": "vipr_eza_pins",
          "ShortsLimit": -0.2,
          "OpensLimit": -1.49
        },
        {
          "Name": "all_leg",
          "ShortsLimit": -0.3,
          "OpensLimit": -3
        }
      ]
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
| **2**         | ***Fail***    | Shorts tests failing. Opens and Cres tests might have failed too.       |
| **3**         | ***Fail***    | Opens tests failing. Cres tests might have failed too.                  |
| **4**         | ***Fail ***   | Contact Resistance tests failing.                                       |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SHOPS**: **Sh**orts and **Op**en**s** test methodology
  - **DC: direct current**
  - **CRES: Contact Resistance**
## Version tracking

| **Date**                   | **Version**    | **Author**        | **Comments**    |
| -------------------------- | -------------- | ----------------- | --------------- |
| Jan  31<sup>st</sup>, 2022 | 1.0.0          | Andrea Gomez      | Initial version |
| July 15<sup>th</sup>, 2022 | 1.0.1          | Chen Tat, Khoh    | Ituff faildata to comply PinFinder tools format. |
| June  2<sup>nd</sup>, 2023 | 1.0.2          | Andrea Gomez      | Print raw data in pc or fc depending on whether the pin passes all tests. |
| June  8<sup>th</sup>, 2023 | 1.0.3          | Andrea Gomez      | Pins list in configuration file can now be empty or have duplicated pin names and channel numbers. |
| Sept 13<sup>th</sup>, 2023 | Prime 12.02.02 | Khai Jie, Teoh    | Include fail pin name and fail channel printing to ituff.<br> #39827 |
| Sept 29<sup>th</sup>, 2023 | Prime 13.00.00 | Andrea Gomez      | Remove unnecessary ituff print. |
| Nov  21<sup>th</sup>, 2023 | Prime 12.03.00 | Khai Jie, Teoh    | Disable Witch Project '2_tname_failchannel' and '3_binpinfails' printing to ituff.<br> #46008 |