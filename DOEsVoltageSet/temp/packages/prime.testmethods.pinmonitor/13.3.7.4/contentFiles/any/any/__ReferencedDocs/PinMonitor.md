**prime Test-Method Specification REP**

[[_TOC_]]

## REP for PinMonitor

This **REP** is intended to describe the PinMonitor Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Aleph File** – A JSON file to be parsed at Prime INIT TestMethod

  - **Method modes** – A detailed description of the different modes available to run the TestMethod.

  - **Datalog output** – A detailed description of what is data-logged by his TestMethod

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document 

## Methodology

The PinMonitor test method provides capability to monitor a pin(s). 

The Start, Stop and StopAll mode give users flexibility to control the region of monitoring. 

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the PinMonitor test method.

| **Parameter Name**    | **Required?**    | **Type**        | **Values**                                                       |
| --------------------- | ---------------- | --------------- | -----------------------------------------------------------------|
| Mode                  | Yes              | String (choice) | Start (**default**)                                              |
|                       |                  |                 | Stop                                                             |
|                       |                  |                 | StopAll                                                          |
| ConfigurationName     | No*              | String          | The name of the config, which exists in a parsed JSON file. *Required in Start and Stop modes.        
| DumpMode              | Yes              | String (choice) | Stats: only prints overall statistics at the "Stop" mode.
|                       |                  |                 | Full: Stats + prints the raw pin data at the end of each test instance in ituff.
|                       |                  |                 | AverageInstance: Stats + prints the averaged pin data per instance at the end of each test instance in ituff.
| SamplingInterval      | No               | Integer         | Interval(mSec) at which a pin needs to be monitored (Default = 4ms). |
| TagName               | No               | String          | An optional identifier to be appended to the stats record printed to ituff.
| ImmediateKill         | No               | String (choice) | Disabled (**default**)                                              |
|                       |                  |                 | Enabled: Enables ImmediateKill when limits are exceeded. Only applies to Start mode                                                             |

JSON parsing is done at the Prime Init TestMethod. 
The user can call multiple of Start monitors, and can stop at any moment as desired. Configuration name(s) called in the instance should match one of the names defined in the json file.

###Virtual Pin
A Virtual Pin is activated when a user defines a pin using GroupedPins in the Aleph File.
This virtual pin is used to track the average of the maximum values across all measurement streams reported in pinMonitor.

## Method modes
The table below lists and describes the modes supported by the PinMonitor test method.


| **Mode**    | **Function**                                                                                     |
|-------------|--------------------------------------------------------------------------------------------------|
| Start       | Starts measuring activity based on the input configuration.                                      |
| Stop        | Stops the measuring for the specific configuration specified in the ConfigurationName parameter. Logs all recorded data for the specific configuration and performs limit checks.                                                                                   |
| StopAll     | Stops all the active configurations. Logs all recorded data for all active configurations. Does not perform any limit checks.                                    |                        

## Configuration states
The table below lists and describes the possible states a PinMonitor configuration could be at a given point in time.


| **State**   | **Description**                                                                                               |
|-------------|---------------------------------------------------------------------------------------------------------------|
| Initial     | Initial state of a PinMonitor configuration after any pin activity recording has been started.                |
| Active      | Determines that pin activity is being recorded for the PinMonitor configuration. A configuration will be in Active state either through a Start instance or after a Pause->Resume operation (via Resume Callback).    |
| Paused      | Determines that pin activity recording has been paused (via Pause Callback) for the PinMonitor configuration. |
| Stopped     | Determines that pin activity is no longer being recorded for the PinMonitor configuration.                    | 

## Configuration state transitions
The table below lists and describes the possible state transitions that can be performed over a PinMonitor configuration.


| **State**   | **Valid state transitions**                              |
|-------------|----------------------------------------------------------|
| Initial     | Active                                                   |
| Active      | Paused / Stopped                                         |
| Paused      | Active / Stopped                                         |
| Stopped     | *None* (pin activity recording has been stopped already) | 

## ImmediateKill
Immediate kill mode allows for PinMonitor to exit before stop if kill limits are exceeded.
This is achieved by running a check during post execute against each pin's kill limits.
If kill limits are exceeded, post execute will log the failure and throw an alarm exception.
To retrieve previous immediate kill failures, PinMonitorService includes a GetAlarms() API which will return a list of previously stored AlarmInfo objects.
Because a kill occurs through an alarm being thrown, it is up to the user to include proper StopAll calls during TestPlanEnd. 

Kill limits are defined as the following:
| **Pin Type**   | **Scenarios to activate a kill**                              |
|-------------|----------------------------------------------------------|
| Thermal     | TjDroop is over lower tolerance                                                   |
|             | TjRise is over upper tolerance                                                |
|             | Maximum is over integrity limit                                                |
|             | Minimum is under integrity limit                                               |
| Current     | Maximum is over upper limit       |
|             | Minimum is under lower limit       |
| Voltage     | Maximum is over upper limit       |
|             | Minimum is under lower limit      |

Average limits will not activate a kill.

## Datalog output


## Aleph File

```json
{
  "Configurations":
  [
    {
      "Name": "Abc",
      "Pins":
      [
        {
          "Name": "Abc",
          "Types": 
          [
            {
              "Type": "Voltage",
              "UpperTolerance": 5.0,
              "LowerTolerance": 5.0,
              "AverageUpperTolerance": 5.0,
              "AverageLowerTolerance": 5.0
            }, 
            {
              "Type": "Current",
              "UpperTolerance": 5.0,
              "LowerTolerance": 5.0,
              "AverageUpperTolerance": 5.0,
              "AverageLowerTolerance": 5.0
            }
          ]
        }
      ],
      "TestClassToExclude":
      [
        "ICScreenTest"
      ]
    },
    {
      "Name": "PinXyz",
      "Pins":
      [
        {
          "Name": "xyz",
          "Types":
          [
            {
              "Type": "Voltage",
              "UpperTolerance": 5.0,
              "LowerTolerance": 5.0,
              "AverageUpperTolerance": 5.0,
              "AverageLowerTolerance": 5.0
            }
          ]
        }
      ]
    },
    {
      "Name": "TDAU",
      "Pins":
      [
        {
          "Name": "TDAU_00",
          "Types":
          [
            {
              "Type": "Thermal",
              "SetPoint": 90,
              "UpperTolerance": "PinMonitorUserVars.upperTolerance",
              "LowerTolerance": 5.0,
              "AverageUpperTolerance": 5.0,
              "AverageLowerTolerance": "PinMonitorUserVars.AverageLowerTolerance",
              "IntegrityHighLimit": 25,
              "IntegrityLowLimit": 20,
              "PcsDatalogSelector": "0"
            }
          ]
        },
        {
          "Name": "VirtualPinTDAU",
          "GroupedPins":
          [
            "TDAU_00",
            "TDAU_01",
            "TDAU_02"
          ],
          "Types":
          [
            {
              "Type": "Thermal",
              "SetPoint": 90, 
              "UpperTolerance": "PinMonitorUserVars.upperTolerance",
              "LowerTolerance": 5.0,
              "AverageUpperTolerance": 5.0,
              "AverageLowerTolerance": "PinMonitorUserVars.AverageLowerTolerance",
              "IntegrityHighLimit": 25,
              "IntegrityLowLimit": 20,
              "PcsDatalogSelector": "1"
            }
          ]
        }
      ]
    }
  ]
}
```
## Details of the Json Objects
| **JSON object** |     Properties         |    Mandatory?        |      **Description**          | **Notes**                                |
| ----------------|------------------------|----------------------| ----------------------------- | -----------------------------------------|
| Configurations  |                        |                      | Array of configuration        | Atleast one must be defined.             |
|                 | Name                   |       Yes            | Name of the Configuration     | Must be unique.                          |
|                 | Pins                   |       Yes            | Pins to be monitored          | Atleast one must be defined.             |
|                 | TestClassToExclude     |       No             | Name of the test classes to ignore statistics calculation.|              |
| Pins            |                        |                      | Array of pins                 | The name and type of the pin must be specified.|
|                 | Name                   |       Yes            | Name of the pin               |                                          |
|                 | Types                  |       Yes            | Collection of types           |                                          | 
| Types           |                        |                      | Definition of settings for the pin to monitor.|                           |      
|                 | Type                   |       Yes            | Type of the pin               | Allowed Values: [Voltage, Current, Thermal].|
|                 | SetPoint               |       No             | Set point value used to compare limits of all the data points|  Should be a double or a uservar.<br>Default value is zero. |
|                 | UpperTolerance         |       No             | Upper tolerance of all the data points. <br> For non-thermal pins, upper limit is calculated using setPoint + upperTolerance. <br>For thermal pins, upper tolerance is compared directly to TjRise |   Should be a non-negative double or a uservar                                   |
|                 | LowerTolerance         |       No             | Lower tolerance of all the data points. <br>For non-thermal pins, lower limit is calculated using setPoint - lowerTolerance.  <br>For thermal pins, lower tolerance is compared directly to TjRise |   Should be a non-negative double or a uservar                                   |
|                 | AverageUpperTolerance  |       No             | Upper tolerance of all the average data points. Average upper limit is calculated using setPoint + AverageUpperTolerance | Should be a non-negative double or a uservar.                          |
|                 | AverageLowerTolerance  |       No             | Lower tolerance of all the average data points. Average lower limit is calculated using setPoint - AverageLowerTolerance    |  Should be a non-negative double or a uservar.                       |
|                 | IntegrityHighLimit     |       No             | High integrity limit for the pin| Applicable for Thermal pins only. Instance exits port 5 if exceeded.|
|                 | IntegrityLowLimit      |       No             | Low integrity limit for the pin | Applicable for Thermal pins only. Instance exits port 4 if exceeded.|
|                 | PcsDatalogSelector     |       No             | Optional unique identifier to be appended to SOT, TJRISE and TJDROOP PCS tokens.<br>This parameter only accepts one value not comma separated value.| Applicable for Thermal pins only.|

## TPL Samples

Here are a few test instance examples using the PinMonitor test method.

TPL Sample1: 

```
Test PrimePinMonitorTestMethod PrimePinProfilerTestMethod_StartMonitor_P1
{
   LogLevel = "PRIME_DEBUG";
   ConfigurationName = "abc";
   DumpMode = "Full";
   Mode = "Start";
   SamplingInterval = 4;
}
Test PrimePinMonitorTestMethod PrimePinProfilerTestMethod_StopMonitor_P1
{
   LogLevel = "PRIME_DEBUG";
   ConfigurationName = "abc";
   DumpMode = "Stats";
   Mode = "Stop";
   TagName = "SampleTagName";
}
Test PrimePinMonitorTestMethod PrimePinProfilerTestMethod_StopAllMonitor_P1
{
   LogLevel = "PRIME_DEBUG";
   Mode = "StopAll";
   TagName = "SampleTagName";
}

```

## Exit Ports

The PinMonitor test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing any limits specified |
| **1**         | ***Pass***      | Passing condition            |
| **2**         | ***Fail***      | Thermal low limit (TjDroop exceeded LowerTolerance)         |
| **3**         | ***Fail***      | Thermal high limit (TjRise exceeded LowerTolerance)         |
| **4**         | ***Fail***      | Integrity low limit         |
| **5**         | ***Fail***      | Integrity high limit         |

## Ituff Examples
Example Ituff after PinMonitor Start instances where the Post execute prints pin profile related to full mode to ituff.
```
0_tname_UNKNOWN
0_comnt_PROFILE#TD0#Thermal#87.900002
2_tname_TPI_VCC::VCCIN_HCDPS_VCCCONTCPG_K_START_X_X_X_VSIM_VCCIN
2_mrslt_40.3258
2_msunit_A
2_comnt_faildata_{0060001100}
0_tname_UNKNOWN
0_comnt_PROFILE#VCCFA_EHV_FIVRA#Voltage#0.000000|0.006592|0.006592|0.006592|0.006592|0.006592|0.006592|0.006592|0.006592|0.006592
0_tname_UNKNOWN
0_comnt_PROFILE#VCCIN#Voltage#0.000000|0.000000|0.000000|0.000000|0.000000|0.000000|0.000000|0.000000|0.000000|0.000000
0_tname_UNKNOWN
0_comnt_PROFILE#VL_VCCCFC_D00#Voltage#-0.000153|0.000000|0.000000|0.000000|0.000000|0.000153|0.000153|0.000000|0.000000|0.000305
0_tname_UNKNOWN
0_comnt_PROFILE#TD0#Thermal#87.800003|87.800003|87.800003|87.800003|87.800003|87.800003|87.800003|87.800003|87.800003|87.800003
```

Sample ituff for PinMonitor Stop mode:
Stats will be printed in below format.
0_comnt_STATS#<pin_name>#<type>#<min>_<max>_<Average_min>_<Average_max>_<Average_Average>
```
0_tname_TPI_BASE_SPR::CTRL_PINMONITOR_STOP_X_X_X_X
0_comnt_STATS#VCCFA_EHV_FIVRA#Voltage#0.000000_0.006592_0.000000_0.001527_0.000155
0_tname_TPI_BASE_SPR::CTRL_PINMONITOR_STOP_X_X_X_X
0_comnt_STATS#VCCFA_EHV_FIVRA#Current#-0.026306_0.029419_-0.004707_0.029419_0.018007
0_tname_TPI_BASE_SPR::CTRL_PINMONITOR_STOP_X_X_X_X
0_comnt_STATS#VCCIN#Voltage#0.000000_0.000244_0.000000_0.000041_0.000004
0_tname_TPI_BASE_SPR::CTRL_PINMONITOR_STOP_X_X_X_X
0_comnt_STATS#VL_VCCCFC_D00#Voltage#-0.000153_0.021210_0.000000_0.004305_0.000483
0_tname_TPI_BASE_SPR::CTRL_PINMONITOR_STOP_X_X_X_X
0_comnt_STATS#TD0#Thermal#87.800003_222.199997_87.866318_222.199997_155.164442
```
Ituff for PinMonitor StopAll mode will contain the same information as Stop mode for each active configuration.

### Thermal Prints

## Additional Dependencies

More dependencies to consider for this TestMethod to well operate:


## Version tracking


| **Date**                  | **Version** | **Author**     | **Comments**                        |
| ------------------------- | ----------- | -------------- | ----------------------------------- |
| Aug 29<sup>th</sup>, 2021 | 1.0.0       | vsunkari       | Initial Version.                    |
| Sep 21<sup>th</sup>, 2021 | 2.0.0       | vsunkari       | After TP validation.                |
| Nov 14<sup>th</sup>, 2022 | 3.0.0       | rppintor       | Dump mode global parameter.         |
| Dec 19<sup>th</sup>, 2022 | 4.0.0       | vasqueza       | Added StopAll mode.                 |
| Sep 28<sup>th</sup>, 2023 | 4.1.0       | vasqueza       | Added SOT, TJRISE and TJDROOP PCS tokens.|
| Nov 7<sup>th</sup>, 2023  | 5.0.0       | lvoepel        | Changed limits to tolerances based on a set point value and added support for user variables for limit calculations.|
| Dec 15<sup>th</sup>, 2023 | 5.1.0       | lvoepel        | Added exit ports for TJRise and TJDroop failiures.|
| Feb 15<sup>th</sup>, 2024 | 6.0.0       | vasqueza       | Added state description and allowed state transition table.|
| Aug 19<sup>th</sup>, 2024 | 13.0.1      | dajimene       | Added virtual pin.<br> #42497 |
| Sep 3<sup>rd</sup>, 2024  | 13.0.1       | lvoepel        | Added immediate kill.<br> #49247 |


## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
