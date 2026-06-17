# PrimeAlarmReportTestMethod Specification REP
Revision 1
April 2024

[[_TOC_]]
This doc is intended to help you get started with using the PrimeAlarmReportTestMethod, or otherwise known as ***AlarmReport***.

In this document, you will find the below sections:

  - **Summary** - Summary of the purpose of the test method
  - **Use cases** – A detailed description of this TestMethod intention and purpose
  - **Parameters** – A table that describes each instance parameter (Name, Required?, Type, Values, Description)
  - **Log samples** - Samples of ituff prints with the MTPL test instance definition used to generate the log. All samples use the same set of alarms thrown.
  - **Exit ports** - A table describes each exit port
  - **Version tracking** – Tracks updates to this documentation.
  - **Acronyms** - Definition of acronyms used in this document 

# Summary
AlarmReport retrieves all Tier E alarms that have occurred during the current DUT Session, then logs them to ituff.

# Use case
For use of consolidating all alarm information for the current DUT session into a single test instance for later querying in databases (ex. ACME). 

The test method prints each alarm exception on its own string value print (strgval) into ituff, appending the alarm exception information to ituff in the following format:

```
2_strgval_<test instance triggering alarm>|<alarm type>|<slot>|<channel>|<pin>|<module>
```

A given instance can throw multiple alarms during its execution. If an instance does print multiple alarms, AlarmReport will print each alarm on its own line (unless the OnlyPrintFinalAlarmInfo parameter is enabled).

Alarms recorded for a given unit are reset during execution of a unit's DeviceStartSetup instance.

# Parameters
The table below lists and describes the parameters used by the ***AlarmReport*** test method.

| **Name**                     | **Required?** | **Type** | **Values**      | **Default value** | **Description**                                                                                                                                                               |
|------------------------------|---------------|----------|-----------------|-------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| OnlyTestInstanceName         | No            | Bool     | "TRUE", "FALSE" | "FALSE"           | If true, removes module scoping from test instance names before appending them to ituff.                                                                                      |
| IncrementRepeatInstanceNames | No            | Bool     | "TRUE", "FALSE" | "TRUE"            | If true, appends a counter to each ituff print, indicating the amount of executions of a given test instance that threw a Tier E alarm. If a single execution of a test instance throws multiple alarms, the counter does not increment for each print. |
| PrintHandledAlarms           | No            | Bool     | "TRUE", "FALSE" | "FALSE"           | If true, prints out tier E alarms that were handled by the test instance. Otherwise, only print alarms that cause test instances to exit port -2.                             |
| OnlyPrintFinalAlarmInfo      | No            | Bool     | "TRUE", "FALSE" | "FALSE"           | If true, only the final alarm info is printed for each instance's thrown exception.                                                                                           | 

# Log samples

Here are the ituff prints created by PRIME whenever reports an alarm for a given test instance:
```
2_lsep
2_tname_HWALARM_Alarms::ThrowsAlarm_First_FNEG2|Slot:0|Pin:fake_pin|Type:SimulatedAlarmOne
2_strgval_Module:fake_module|Channel:0|Time:<time>
2_lsep
2_tname_HWALARM_Alarms::ThrowsAlarm_Second_FNEG2|Slot:0|Pin:fake_pin|Type:SimulatedAlarmTwo
2_strgval_Module:fake_module|Channel:0|Time:<time>
2_lsep
2_tname_HWALARM_Alarms::ThriceThrown_FNEG2|Slot:0|Pin:fake_pin|Type:ThriceThrown
2_strgval_Module:fake_module|Channel:0|Time:<time>
2_lsep
2_tname_HWALARM_Alarms::ThriceThrown_FNEG2|Slot:0|Pin:fake_pin|Type:ThriceThrown
2_strgval_Module:fake_module|Channel:0|Time:<time>
2_lsep
2_tname_HWALARM_Alarms::ThriceThrown_FNEG2|Slot:0|Pin:fake_pin|Type:ThriceThrown
2_strgval_Module:fake_module|Channel:0|Time:<time>
2_lsep
2_tname_HWALARM_Alarms::MultipleInfo_FNEG2|Slot:0|Pin:fake_pin|Type:Type1
2_strgval_Module:fake_module|Channel:0|Time:<time>
2_lsep
2_tname_HWALARM_Alarms::MultipleInfo_FNEG2|Slot:0|Pin:fake_pin|Type:Type2  
2_strgval_Module:fake_module|Channel:0|Time:<time>
2_lsep
2_tname_HWALARM_Alarms::MultipleInfo_FNEG2|Slot:0|Pin:fake_pin|Type:Type2
2_strgval_Module:fake_module|Channel:0|Time:<time>
2_lsep
2_tname_HWALARM_Alarms::MultipleInfo_FNEG2|Slot:0|Pin:fake_pin|Type:Type2
2_strgval_Module:fake_module|Channel:0|Time:<time>
```
All following examples will use the same set of alarms thrown above.
*Note*: <time> will be populated by the time that the instance threw the alarm during TP execution.

## Default parameters
Instances will be scoped and counters are enabled for instances.
```
Test PrimeAlarmReportTestMethod PrimeAlarmReportTestMethod_P1
{
}
```
```
2_tname_Prints::PrimeAlarmReportTestMethod_P1
2_strgval_Alarms::ThrowsAlarm_First_FNEG2_0|SimulatedAlarmOne|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_P1
2_strgval_Alarms::ThrowsAlarm_Second_FNEG2_0|SimulatedAlarmTwo|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_P1
2_strgval_Alarms::ThriceThrown_FNEG2_0|ThriceThrown|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_P1
2_strgval_Alarms::ThriceThrown_FNEG2_1|ThriceThrown|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_P1
2_strgval_Alarms::ThriceThrown_FNEG2_2|ThriceThrown|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_P1
2_strgval_Alarms::MultipleInfo_FNEG2_0|Type1|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_P1
2_strgval_Alarms::MultipleInfo_FNEG2_0|Type2|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_P1
2_strgval_Alarms::MultipleInfo_FNEG2_0|Type2|0|0|fake_pin|fake_module
```

Note how the counter does not increment for each alarm thrown by the "MultipleInfo_FNEG2" test instance. The counter increments for each run of a test instance, not for each individual alarm thrown by the instance.

## OnlyTestInstanceName = "TRUE";
No scope for modules will be appended to ituff prints.
```
Test PrimeAlarmReportTestMethod PrimeAlarmReportTestMethod_NoModuleScope_P1
{
    OnlyTestInstanceName = "TRUE";
}
```
```
2_tname_Prints::PrimeAlarmReportTestMethod_NoModuleScope_P1
2_strgval_ThrowsAlarm_First_FNEG2_0|SimulatedAlarmOne|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoModuleScope_P1
2_strgval_ThrowsAlarm_Second_FNEG2_0|SimulatedAlarmTwo|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoModuleScope_P1
2_strgval_ThriceThrown_FNEG2_0|ThriceThrown|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoModuleScope_P1
2_strgval_ThriceThrown_FNEG2_1|ThriceThrown|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoModuleScope_P1
2_strgval_ThriceThrown_FNEG2_2|ThriceThrown|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoModuleScope_P1
2_strgval_MultipleInfo_FNEG2_0|Type1|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoModuleScope_P1
2_strgval_MultipleInfo_FNEG2_0|Type2|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoModuleScope_P1
2_strgval_MultipleInfo_FNEG2_0|Type2|0|0|fake_pin|fake_module
```

## AppendCounterToInstances = "FALSE";
No counter will be added to ituff prints.
```
Test PrimeAlarmReportTestMethod PrimeAlarmReportTestMethod_NoIncrement_P1
{
    LogLevel = "Enabled";
    AppendCounterToInstances = "FALSE";
}
```
```
2_tname_Prints::PrimeAlarmReportTestMethod_NoIncrement_P1
2_strgval_Alarms::ThrowsAlarm_First_FNEG2|SimulatedAlarmOne|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoIncrement_P1
2_strgval_Alarms::ThrowsAlarm_Second_FNEG2|SimulatedAlarmTwo|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoIncrement_P1
2_strgval_Alarms::ThriceThrown_FNEG2|ThriceThrown|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoIncrement_P1
2_strgval_Alarms::ThriceThrown_FNEG2|ThriceThrown|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoIncrement_P1
2_strgval_Alarms::ThriceThrown_FNEG2|ThriceThrown|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoIncrement_P1
2_strgval_Alarms::MultipleInfo_FNEG2|Type1|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoIncrement_P1
2_strgval_Alarms::MultipleInfo_FNEG2|Type2|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_NoIncrement_P1
2_strgval_Alarms::MultipleInfo_FNEG2|Type2|0|0|fake_pin|fake_module
```

## OnlyPrintFinalAlarmInfo = "FALSE";
Only the final alarm an instance triggers is printed to ituff.
```
Test PrimeAlarmReportTestMethod PrimeAlarmReportTestMethod_OnlyPrintFinalAlarmInfo_P1
{
    LogLevel = "Enabled";
    OnlyPrintFinalAlarmInfo = "TRUE";
}
```
```
2_tname_Prints::PrimeAlarmReportTestMethod_OnlyPrintFinalAlarmInfo_P1
2_strgval_Alarms::ThrowsAlarm_First_FNEG2_0|SimulatedAlarmOne|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_OnlyPrintFinalAlarmInfo_P1
2_strgval_Alarms::ThrowsAlarm_Second_FNEG2_0|SimulatedAlarmTwo|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_OnlyPrintFinalAlarmInfo_P1
2_strgval_Alarms::ThriceThrown_FNEG2_0|ThriceThrown|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_OnlyPrintFinalAlarmInfo_P1
2_strgval_Alarms::ThriceThrown_FNEG2_1|ThriceThrown|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_OnlyPrintFinalAlarmInfo_P1
2_strgval_Alarms::ThriceThrown_FNEG2_2|ThriceThrown|0|0|fake_pin|fake_module
2_tname_Prints::PrimeAlarmReportTestMethod_OnlyPrintFinalAlarmInfo_P1
2_strgval_Alarms::MultipleInfo_FNEG2_0|Type2|0|0|fake_pin|fake_module
```

# Exit Ports
The ***AlarmReport*** test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                           |
|---------------|---------------|-------------------------------------------|
| **-2**        | ***Alarm***   | Any alarm condition.                      |
| **-1**        | ***Error***   | Any software condition error.             |
| **0**         | ***Fail***    | No alarms were reported by this instance. |
| **1**         | ***Pass***    | Alarms were reported by this instance.    |

# Version tracking
| **Date**         | **Prime Version** | **Comments**     |
|------------------|-------------------|------------------|
| April 14th, 2024 | 13.1.0            | Initial Version. |

# Acronyms
Definition of acronyms used in this document:
  - **MTPL**: Modular Test Programming Language