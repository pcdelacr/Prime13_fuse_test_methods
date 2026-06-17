prime Test-Method Specification REP

Revision 1.0.0

March 2021

[[_TOC_]]

## Methodology
A simple test method will wait n amount of milliseconds and return the user's specified port.

Notes:
* SleepTime must be greater than 1mS and smaller than 60,000mS.
* Port must be between 0 and 5."

## Test Instance Parameters

| **Parameter Name** | **Required?** | **Type** | **Values** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ |
| SleepTime          | Yes           | Integer  |            |sleep time in milliseconds.|
| ExitPort           | No           | Integer  |            |return Port after Sleep Time. Range from 0 to 5. (Default=1).|

## OTPL Sample1:
``` Perl
Test PrimePauseTestMethod Pause_CPU_c3_r2_P3
{
	SleepTime = 10;
	ExitPort = 3;
	LogLevel = "PRIME_DEBUG";
}
```
## Exit Ports

The CallbacksRegistrar test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **1**         | ***Pass***      | Passing condition            |
| **2**         | ***Pass***      | Passing condition            |
| **3**         | ***Pass***      | Passing condition            |
| **4**         | ***Pass***      | Passing condition            |
| **5**         | ***Pass***      | Passing condition            |

## Additional Dependencies

None.

| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| March 22, 2021 | 1.0.0       | Humberto Ramirez |  Initial doc|
| October 22, 2021 | 7.0.0       | Mohd Faiz Mohd Asri |  Increased max pause time to 60000ms|          