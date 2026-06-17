**prime Test-Method Specification REP**

Revision 1.0.2

Feb 2021

[[_TOC_]]

## REP for TimeUnderStress

This **REP** is intended to describe the TimeUnderStressControlPrime TestMethod.

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

The _TimeUnderStressControl_ test method logs user enabled, stress related testtime. This testtime represents the stress duration between _Start_ mode and the last instance of _LoopStatus_ mode. <br>
This methodology offers two distinct _stress time_ recording methods: _Standard_ and _AutoStressDetect_, each comprising three sub-modes: _Start_, _LoopStatus_ and _End_.

## AutoStressDetect vs Standard mode
### AutoStressDetectMode

This mode is used for measuring and logging stress and non-stress times for specific _VoltageSupplies_ based on a _VoltageThreshold_ to determine whether or not the "supplies" are being stressed. <br>
The _TimeUnderStress_ methodology will take measurements via TOS API and perform all calculations to determine xTime and sTime based on the data provided via instance parameters. <br>
To use _AutoStressDetect_, _SamplingInterval_, _VoltageThreshold_, _VoltageSupplies_, and _StressTime_ must all be defined. <br>
In case only some of these inputs are defined, an exception will be thrown and the instance will fail _Verify()_.

### StandardMode

Standard mode is the "manual" alternative for measuring stress time without actually considering any parameters/limits. <br> 
In this mode there is no TOS API involved and no Prime side calculations. The mode works under the assumption that all the time, through the instance execution, should be considered as stress time. <br>
If _Start_ is executed omitting _SamplingInterval_, _VoltageThreshold_, _VoltageSupplies_, or _StressTime_, the instance will run in _StandardMode_.

## For Standard mode:
#### Start:
1. Checks for a previous stress time value from DFF STIME token.
2. If nonzero, subtracts this value from a DateTime.Now object to effectively add in previous stress time.
2. Stores the above object in shared storage as '_TUSLog_Start_DateTime_' key.
3. Sets uservar [__UserVars.StressTimeInSeconds_] as 0.
#### LoopStatus:
1. Gets the time object stored in shared storage as '_TUSLog_Start_DateTime_' key.
2. Calculates the time difference between '_TUSLog_Start_DateTime_' and current DateTime.Now object as a double with seconds precision. In other words, this is calculating the time difference between 'start' and 'loopStatus' mode instances.
3. Stores this time difference double in shared storage as '_TimeDifference_From_Start_To_LastSnapshot_' key.
4. Issues the _FlushAllDataStreamLogs_ API, if one of the two following conditions are met per unit basis:
    a. If it is the very first time.
    b. If the previous API was executed beyond the amount of time (seconds) specified at _FlushTimeSpan_ parameter.

If Flush command is issued, a set of temporary tokens are recorded in the ituff as below
```
       3_comnt_footer_begin
       2_lsep
       2_comnt_DFF_Data_PKG_<CustomDffToken>=<STIME>,<TemporaryTokenToPrint1>=<DffValue1>,<TemporaryTokenToPrint2>=<DffValue2>...
       2_lend
       3_curfbin_9820
       3_curibin_98
       3_trslt_fail
       3_comnt_footer_end
```
Where CustomDffToken corresponds to test method input CustomDffToken and TemporaryTokenToPrintN corresponds to each entry in the input TemporaryTokensToPrint.

These temporary tokens will be post processed by DPC, thus removing them from the final ituff. [Mainly used to recover from Binning Service TimeOut Unit Suspensions.]

5. Compare's current time for the flowloop to the loop's total expected loop time. If one loop has completed and the time is over the total expected, the test instance will exit port 2, otherwise it will exit port 1.

#### End:
1. Gets the double value from shared storage with '_TimeDifference_From_Start_To_LastSnapshot_' key.
2. If the value is greater than 0, then it is stored as an Ituff token, as below. 
```
       2_tname_TUSLogger_End_E0S_stime
       2_mrslt_100
```
3. Sets the uservar [__UserVars.StressTimeInSeconds_] with appropriate stress time value.
4. Writes appropriate stress time value to DFF STIME token.

## For AutoStressDetect mode:
#### Start:
1. Writes the target stress time to the given flow loop for future use.
2. Activates TimeUnderStress service to monitor the given voltage supplies at the given sampling interval. Time will be considered "UnderStress" if any of the given supply pins are above the threshold voltage during a sampling interval.

#### LoopStatus:
1. Gets the current Stress (Stime) and nonstress (xtime) and prints to datalog. 
2. Prints the current stime, xtime, and flow loop summary data to ituff
Summary Data is in the Format:
```
CLV_{completedLoopCount}_CLT_{completedTimeInSeconds}_TLV_{totalLoopCount}_STL_{targetTimeUnderStress}_ST_{sTime}_XT_{xTime}";
```
3. Checks if the flow loop will exceed MaxFlowLoopTime before the target stress time is reached. 
If so, then the test instance will exit port -1.
4. Checks if the sum of sTime {stress Time} and xTime {non-stress time} is approximately equal to completedTimeInSeconds.
If it doesn't, this mean there has been data which is not accounted for either stime or xtime, thus it shall exit port -1.
5. If previous stress time reported in Dff exceeds target stress time, or if the current stress time is over target and we've completed one loop, we will exit port 2.
6. If none of the above conditions are true, we will continue and exit port 1.

#### End:
If a Time Under Stress profile is active:
1. Prints current xtime and stime to ituff
2. Stops timeUnderStressService and ends data collection.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the TimeUnderStress test method

| **Parameter Name** | **Required?** | **Type** |        **Values**    | **Comments**                                                                                                                                                                                                                                                                           |
| ------------------ | ------------- | -------- | ---------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Mode               | Yes           | String   | Start, LoopStatus, End, EndAll |                                                                                                                                                                                                                                                                                        |
| DatalogUserBin     | No            | String   |                      | Default ["9820"]. Only used in _LoopStatus_ mode.                                                                                                                                                                                                                                      |
| FlushTimeSpan      | No            | Integer  |                      | Default [30]. Only used in _LoopStatus_ mode.                                                                                                                                                                                                                                          |
| FlowItemName       | Yes | String   |                      | FlowItemName to get default stress time loop duration                                                                                                                                                                                                                                  |
| MaxFlowLoopTime    | Yes for AutoStress Mode | Integer  |                      | Maximum flow loop time in seconds                                                                                                                                                                                                                                                      |
| VoltageSupplies    | Yes for AutoStress Mode | CommaSeparatedString |          | List of voltage supplies to monitor for stress                                                                                                                                                                                                                                         |
| VoltageThreshold   | Yes for AutoStress Mode | Double | | Either a single value (that applies to all voltage supplies) or a comma separated list of threshold values (one per voltage suppy) to determine if a voltage supply is under stress. A voltage supply is considered under stress if a measurement is above the corresponding threshold |
| SamplingInterval   | Yes for AutoStress Mode | Integer |                       | Sampling interval in milliseconds for AutoStressDetect mode. This should always be multiple of 4.                                                                                                                                                                                      |
| StressTime | Yes for AutoStress Mode | Integer |                       | Target stress time in seconds                                                                                                                                                                                                                                                          |
| DieName            | No            | String   |   | Default 'PKG' diename is used while Dff get/set stime calls.                                                                                                                                                                                                                           |
| CustomDffToken     | Yes for AutoStress Mode            | String   |   | This string will be used as DFF token to either save to or read the STIME token from DFF and also as a unique identifier for each configuration. Must be provided during start to take effect.                                                                                         |
| TemporaryTokensToPrint     | No            | CommaSeparatedString   |   | This string represents a list of dff tokens to be printed during temporary ituff prints. Dff tokens must already be defined in Dff's master token list in order to be printed. Must be provided during start to take effect.                                                           |


## **Notes:**
1. Parameters are loaded in via TimeUnderStressControl's "Start()" function. 
2. To enable _AutoStressDetect_, mode parameters are all or nothing. If using StandardMode, none should be provided. If using AutoStressDetection, all are required.
3. _DieName_ parameter is only used at the _Start_ mode, if not specified the default value will be "PKG".
4. _SamplingInterval_ is always divided by 4, before feeding to the StartSensorCollection Hdmt API, that only accepts sampleRatio (rate of 4ms). 

## TPL Samples

Here are a few test instance examples using the TimeUnderStressControl test method

TPL Sample1:  
```python
Import PrimeTimeUnderStressControlTestMethod.xml;
Test PrimeTimeUnderStressControlTestMethod TUSControl_Start_P1
{
   Mode = "Start";
   DatalogUserBin = "9820";
   FlushTimeSpan = 30;
   CustomDffToken = "SFLOW1";
   TemporaryTokensToPrint = "AKDFlag";
}

Test PrimeTimeUnderStressControlTestMethod TUSControl_LoopStatus_P1
{
   Mode = "LoopStatus";
}

Test PrimeTimeUnderStressControlTestMethod TUSControl_End_P1
{
   Mode = "End";
}

Test PrimeTimeUnderStressControlTestMethod TUSControl_End_P1
{
   Mode = "EndAll";
}
```
## Datalog output
From _LoopStatus AutoDetect_ mode Test Instance:
```
2_tname_Stress::TusControl_AutoDetectMode_LoopStatus_P1_SFLOW1_LoopStatus_stime
2_strgval_<time>
2_tname_Stress::TusControl_AutoDetectMode_LoopStatus_P1_SFLOW1_LoopStatus_xtime
2_strgval_<time>
2_tname_Stress::TusControl_AutoDetectMode_LoopStatus_P1_SFLOW1_LoopStatus_summary
2_strgval_CLV_<time>_CLT_<time>_TLV_<time>_STL_<time>_ST_<time>_XT_<time>
```
From _End_ mode Test Instance:
```
2_tname_Stress::TUSLogger_End_P1_SFLOW1_stime
2_strgval_12
```

## Exit Ports

The TimeUnderStressControl test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition/ flowloop continue port           |
| **2**         | ***Pass*** | FlowLoop has exited            |

## Additional Notes:
DieName parameter is not needed for this template anymore, as previously this parameter was designed to be used for saving STIME for each die. As the die count increased and more than one Die is being stressed at the same time, complexity of calculating for individual Die STIME increased. Hence this parameter is ignored in the latest code commit published in v13.2.2.
## Additional Dependencies

More dependencies to consider for this TestMethod to well operate:

## Version tracking


| **Date**       | **Version** | **Author**    | **Comments**                          |
| -------------- | ----------- | ------------- | ------------------------------------- |
| Feb 22th, 2021 | 1.0.0       | Vipin Sunkari | Initial Release                       |
| Jun 15th, 2021 | 1.0.1       | Kevin D. Krake| Added ability to read STIME DFF token |
| Aug 4th, 2021  | 1.0.2       | Lauren McDonald| Changed To TimeUnderStressControl and added FlowLoopExit functionality|
| Jan 6th, 2023  | 1.0.3       | Alejandro Vásquez| Added active profile check to Stop |
| Feb 23rd, 2023 | 1.0.4       | Alejandro Vásquez| Added support for multiple voltage threshold |
| Sep 09th, 2023 | 1.0.5       | Alejandro Vásquez| Added CustomDffToken parameter and EndAll mode |
| Mar 11th, 2024 | 1.0.6       | Alejandro Vásquez| Adding more details on AutoStressDetect vs StandardMode |
| Oct 31th, 2024 | 1.0.7       | Lauren McDonald | Adding TemporaryTokensToPrint parameter
## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
