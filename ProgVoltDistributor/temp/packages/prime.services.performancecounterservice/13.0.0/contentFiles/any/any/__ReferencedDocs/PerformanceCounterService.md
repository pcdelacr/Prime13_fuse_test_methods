[[TOC]]

# Performance Counter
The **Performance Counter** provides the necessary infrastructure for users to track how much certain APIs were being called and how much time elapsed in those APIs execution. PRIME Performance Counter is developed on top of Telemtry infrastructure to collect the data required with minimal hit on test time. The counter is capable of tracking the data on 2 levels; global TP level and instance level. The data will be printed to datalog and TOS console at DeviceEndFinalize if debug log is enabled (DeviceEndFinalize Test Method parameter LogLevel="Enabled").

## Enabling the counter
To enable the counter, you'll need to set 2 parameter in **_PrimeInitializeLibraryTestMethod_**:-

| Parameter Name | Value Type | Value | Details |
| ---- | ----- | ----- | ---- |
| PerformanceCounterLogLevel | Choices | None, GlobalOnly(Default), GlobalAndInstance |  Options to enable Performance Counter logging. _GlobalOnly_ will only log TP level data. GlobalAndInstance will log TP level data and instance level data. Please refer to Performance Counter SDK documentations for more details.|
| PerformanceCounterSampleRate | Integer | Defaulted to 1, any positive integer is accepted. | This option is to enable the counter only on every N<sup>th</sup> samples. Example: Value of 3 will enable counter on every 3rd sample (1,2 disabled; 3 enabled; 4,5 disabled; 6 enabled). 0 will disable the counter ("don't count any sample"), 1 will enable the counter on the sample.

## Supported APIs
These APIs are supported by the counter at the moment.

| Category | API Name | Comments |
| -------- | -------- | -------- |
| Functional | Execute | Keep track of every Functional Test executions. |
| DcTest | LoadLevel | Keep track of every DcTest level loading (Level Test Condition execute). |

These counters will keep track how many times they are being called and also the total execution times of the API.
If you require more data to be included in the counters, [open a request ticket here](https://goto.intel.com/primetickets)

## Output
### Datalog
The counters information will be printed to datalog as level 2/0 (depends on TP enables MIDAS or not) with _**strgval**_ format by _**PrimeDeviceEndDatalogTestMethod**_

```
2_tname_[Instance Name]_[Counter Category]_[Counter Item]_N_ApiCalled
2_strgval_[how many times the api was called]
2_tname_[Instance Name]_[Counter Category]_[COunter Item]_ElapsedTime
2_strgval_[execution time]
2_tname_[Counter Category]_[Counter Item]_N_ApiCalled
2_strgval_[how many times the api called]
2_tname_[Counter Category]_[Counter Item]_ElapsedTime
2_strgval_[execution time]

Example:
2_tname_Functional_Alarms::AlarmItuffPrintingTest2_TestForTnameLongerThan128CharsMustBeTruncatedTo128Char_Functional_Execute_N_ApiCalled
2_strgval_1
2_tname_Functional_Alarms::AlarmItuffPrintingTest2_TestForTnameLongerThan128CharsMustBeTruncatedTo128Char_Functional_Execute_ElapsedTime
2_strgval_0.43
2_tname_Functional_Execute_N_ApiCalled
2_strgval_1
2_tname_Functional_Execute_ElapsedTime
2_strgval_0.43
```

### Console
The counter information will be printed to console if debug log is enabled in _**PrimeDeviceEndFinalizeTestMethod**_
```
[DUT: 1]Global counters detail:
Functional_Execute_N_ApiCalled : 33
Functional_Execute_ElapsedTime (ms) : 79.52
DcTest_LoadLevel_N_ApiCalled : 23
DcTest_LoadLevel_ElapsedTime (ms) : 83.38


Instance counters detail:
Dc::FuncDcInheritanceTest_F0_Functional_Execute_N_ApiCalled : 1
Dc::FuncDcInheritanceTest_F0_Functional_Execute_ElapsedTime (ms) : 3.65
Dc::FuncDcInheritanceTest_P1_Functional_Execute_N_ApiCalled : 1
Dc::FuncDcInheritanceTest_P1_Functional_Execute_ElapsedTime (ms) : 0.87
Dc::FuncDcInheritanceTest_F0_DcTest_LoadLevel_N_ApiCalled : 1
Dc::FuncDcInheritanceTest_F0_DcTest_LoadLevel_ElapsedTime (ms) : 1.20
Dc::FuncDcInheritanceTest_P1_DcTest_LoadLevel_N_ApiCalled : 1
Dc::FuncDcInheritanceTest_P1_DcTest_LoadLevel_ElapsedTime (ms) : 0.57
```