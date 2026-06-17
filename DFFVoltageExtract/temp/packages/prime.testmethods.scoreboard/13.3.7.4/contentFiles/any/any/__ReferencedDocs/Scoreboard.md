<h1>Prime Test-Method Specification REP</h1>
Revision 1.0.1

Oct 2021

[[_TOC_]]

## Methodology
The Scoreboard test method provides capability to perform plist execution but including store data in scoreboard, as well as to optionally capture CTV(Capture This Vector) data and failures.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Scoreboard test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Default Value** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ | -----|
| Patlist                      | Yes           | Plist           | Plist name to be executed                                                                                                               |              |                                                                                                                                                               |
| TimingTc                     | Yes           | TimingCondition | Timing test condition required for plist execution                                                                                      |              |                                                                                                                                                               |
| LevelsTc                     | Yes           | LevelsCondition | Levels test condition required for plist execution                                                                                      |              |                                                                                                                                                               |
| PrePlist                     | No            | Plist           | PrePlist callback name to be executed.                                                                                                  | Empty String |                                                                                                                                                               |
| PatternNameCounterIndexes               | Yes           | String          | Map of indexes to be extracted for each limiting pattern name (using the configured pattern name mapping) for each corresponding target.|              |                                                                                                                                                               |
| BaseNumbers                   | Yes           | CommaSeparatedInteger         | A list of numbers with which all generated Scoreboard counters will be individually prefixed with.                                      |              |                                                                                                                                                               |
| MaxFailsNum                      | Yes           | UnsignedInteger | The max number of fails that can be processed.                                                                                          |              | If the maximum number of failures is reached then a single additional counter will be logged. This maxfails counter will consist of the base number plus a string made up with instances of the character '9' only. The number of 9s used will be equal to the number of indexes on the PatternNameCounterIndexes. For example, with Base Number "1234" and a PatternNameCounterIndexes containing 5 indexes, the maxfails counter to be logged is "123499999".|
| MaskPins                     | No            | String          | Comma separated list of pins for which the fail data capture will be skipped.                                                           | Empty String |                                                                                                                                                               |
| CtvCapturePins               | No            | String          | Comma separated list of pins for which CTV data should be captured.                                                                     | Empty String | To capture CTVs, CtvCapturePins must be provided.                                                                                                             |
| CtvCapturePerCycleMode       | No            | Enum            | Enum to determine whether CTVs are captured by cycle. ENABLED enables cycle capture, DISABLED disables.                                 | DISABLED     | To capture CTVs, CtvCapturePins must be provided.                                                                                                             |

**Error Conditions:**

  - If FailuresToCapturePerPatten is >= 1 but FailuresToCaptureTotal is zero, an error will be thrown.
  - Likewise if CtvCycleCaptureMode is ENABLED but CtvCapturePins is empty, an error will be thrown.
  - If CtvCycleCaptureMode is ENABLED and a DtsConfigurationName is defined an error will be thrown, since only per pin caprture is allowed fro DTS processing.

### Explanations
IntraDut: For concurrents flows is recomended to use differentes base numbers for each flow.

## Console output
### Pass
```python
[2022-abr.-27 12:07:41.239][DUT: 1]Test instance=[Scoreboard::PrimeScoreboardTestMethod_NoFailures_P1] executed using 8.561700 ms
[2022-abr.-27 12:07:41.239][DUT: 1]TestInstance=[Scoreboard::PrimeScoreboardTestMethod_NoFailures_P1] exit port=[1].
,Scoreboard::PrimeScoreboardTestMethod_NoFailures_P1,1,Pass
```

### Fail
```python
[2022-abr.-27 12:13:44.437][DUT: 1]Test instance=[Scoreboard::PrimeScoreboardTestMethod_FailuresToCaptureTotalIsLessThanMaxFailuresToItuff_FNEG1] verified using 6.743200 ms
[2022-abr.-27 12:13:44.437][DUT: 1]TestInstance=[Scoreboard::PrimeScoreboardTestMethod_FailuresToCaptureTotalIsLessThanMaxFailuresToItuff_FNEG1] exit port=[-1].
,Scoreboard::PrimeScoreboardTestMethod_FailuresToCaptureTotalIsLessThanMaxFailuresToItuff_FNEG1,-1,Fail
```

### Scoreboard
```python
[2022-abr.-27 12:15:08.203][DUT: 1]Functional test execution failed
[2022-abr.-27 12:15:08.203][DUT: 1]------------------------------------------------------
[2022-abr.-27 12:15:08.203][DUT: 1]| Pattern    |     Domain   |    Address  |      Cycle  |        Pin  |    Channel  |
[2022-abr.-27 12:15:08.203][DUT: 1]------------------------------------------------------
[2022-abr.-27 12:15:08.203][DUT: 1]| O123456789_scoreboardSecondPattern_0123456789 | DomainA_All_DPIN_Dig   |          5  |          1  | xxHPCC_DPIN_Dig_slcA_AA3  |       8035  |
[2022-abr.-27 12:15:08.203][DUT: 1]| O123456789_scoreboardThirdPattern_0123456789 | DomainB_All_DPIN_Dig   |          3  |          2  | xxHPCC_DPIN_Dig_slcB_AA5  |       8093  |
[2022-abr.-27 12:15:08.203][DUT: 1]------------------------------------------------------
[2022-abr.-27 12:15:08.203][DUT: 1]
[2022-abr.-27 12:15:08.204][DUT: 1]ScoreboardService: Storing scoreboard counters=[3SO35965,3TO35965]
```

## Datalog output
```python
2_tname_Scoreboard::PrimeScoreboardTestMethod_FailDataProcessed_P0
2_category_1
2_fdpmv_1
2_fcpmv_-1
2_fsdmv_-1
2_pttrn_DomainA_All_DPIN_Dig:O123456789_scoreboardFirstPattern_0123456789:scoreboard_plist
2_vcont_1
2_faildata_8032
2_tname_Scoreboard::PrimeScoreboardTestMethod_FailDataProcessed_P0_1
2_category_1
2_fdpmv_3
2_fcpmv_-1
2_fsdmv_-1
2_pttrn_DomainA_All_DPIN_Dig:O123456789_scoreboardSecondPattern_0123456789:scoreboard_plist
2_vcont_1
2_faildata_8032
2_tname_Scoreboard::PrimeScoreboardTestMethod_FailDataProcessed_P0_2
2_category_1
2_fdpmv_3
2_fcpmv_-1
2_fsdmv_-1
2_pttrn_DomainA_All_DPIN_Dig:O123456789_scoreboardThirdPattern_0123456789:scoreboard_plist
2_vcont_1
2_faildata_8032
2_tname_Scoreboard::PrimeScoreboardTestMethod_FailDataProcessed_P0_1_scrb
2_strgval_1FO35965,1SO35965,1TO35965
```

## Custom User Code Hooks

Here is the list of functions available to the user code to override:

### List\<string> GetDynamicPinMask()
- Can be used to populate mask pins after TP load
### bool ProcessCtvPerPin(IScoreboardTestInstanceResults, Dictionary<string, string> ctvData)
- Takes a the results of a capture CTV per pin test in the form of a dictionary of pinNames to CTV data. User defined processing is then performed on the results.
- Default behavior is to print all ctv data to debug console and return True.
- Use IScoreboardTestInstanceResults to set port outPut.
### bool ProcessFailures(IScoreboardTestInstanceResults, ICaptureFailureTest captureFailureTest)
- Takes capture failure test and processes the results.
- Default behavior is to log failures to ituff (based on MaxFailuresToItuff input parameter) and then to print each per cycle failure to console. Test returns True if no failures were found, False otherwise.
- Use IScoreboardTestInstanceResults to set port outPut.
### bool ProcessCtvPerCycle(IScoreboardTestInstanceResults, ICaptureCtvPerCycleTest captureCtvPerCycleTest)
- Takes capture CTV per cycle test and processes the results.
- Default behavior is to do nothing and return True.
- Use IScoreboardTestInstanceResults to set port outPut.

### IScoreboardTestInstanceResults CreateTestInstanceResults


## TPL Samples

Below are samples for several types of Scoreboard test which PrimeScoreboardTestMethod can be used for:

**ScoreboardCaptureFailures:**

```python
Import PrimeScoreboardTestMethod.xml;

Test PrimeScoreboardTestMethod PrimeScoreboardTestMethod_FailDataProcessed_P0
{
    Patlist = "scoreboard_plist";
    TimingsTc = "basic_func_timing_10MHz_20MHz";
    LevelsTc = "basic_func_lvl_nom";
    LogLevel = "PRIME_DEBUG";
    BaseNumbers = "1";
    MaxFailsNum = 10;
    PatternNameCounterIndexes = "21,0,3,5,-1,-4,5";
}
```

**ScoreboardCaptureCtv (with CTV capture per cycle disabled):**

```python
Import PrimeScoreboardTestMethod.xml;

Test PrimeScoreboardTestMethod PrimeScoreboardTestMethod_FailDataProcessedWithCaptureCtvPerPin_P0
{
    Patlist = "scoreboard_plist";
    TimingsTc = "basic_func_timing_10MHz_20MHz";
    LevelsTc = "basic_func_lvl_nom";
    LogLevel = "PRIME_DEBUG";
    BaseNumbers = "2";
    MaxFailsNum = 10;
    PatternNameCounterIndexes = "21,0,3,5,-1,-4,5";
    CtvCapturePins = "xxHPCC_DPIN_Dig_slcA_AA0, xxHPCC_DPIN_Dig_slcB_AA0, xxHPCC_DPIN_Dig_slcA_AA1, xxHPCC_DPIN_Dig_slcB_AA1";
}
```

**ScoreboardCaptureCtv (with CTV capture per cycle enabled):**

```python
Import PrimeScoreboardTestMethod.xml;

Test PrimeScoreboardTestMethod PrimeScoreboardTestMethod_FailDataProcessedWithICaptureCtvPerCycleTest_P0
{
    Patlist = "scoreboard_plist";
    TimingsTc = "basic_func_timing_10MHz_20MHz";
    LevelsTc = "basic_func_lvl_nom";
    LogLevel = "PRIME_DEBUG";
    BaseNumbers = "3";
    MaxFailsNum = 10;
    PatternNameCounterIndexes = "21,0,3,5,-1,-4,5";
    CtvCapturePerCycleMode = "ENABLED";
    CtvCapturePins = "xxHPCC_DPIN_Dig_slcA_AA0, xxHPCC_DPIN_Dig_slcB_AA0, xxHPCC_DPIN_Dig_slcA_AA1, xxHPCC_DPIN_Dig_slcB_AA1";
}
```

## Exit Ports

Functional test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

## Additional Dependencies

N/A

## Version tracking

| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| Apr 27<sup>th</sup>, 2022 | 9.1.0       | Didier Jimenez Retana |  Initial Commit           |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **CTV**: Capture This Vector
  - **DTS**: Digital Thermal Sensor