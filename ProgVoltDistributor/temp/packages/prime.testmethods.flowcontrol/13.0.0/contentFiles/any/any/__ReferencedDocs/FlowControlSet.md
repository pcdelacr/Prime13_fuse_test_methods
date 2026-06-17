prime Test-Method Specification REP

Revision 1.0.0

March 2021

[[_TOC_]]

## Methodology
A test method to update the flow value of a given flow domain name.

<span style="color: #FF5662">Notes:</span>
[1] Please note that Flow control is a solution for products that didn’t move to TORCH or TOS MTT syntax. MTT provides the same capabilities as Flow Control.
[2] Single Flow recovery supported on Evergreen can be achieved by a user callback code that compares a parameter with the flow number and the value returned by Prime.Services.TestProgramService.GetDomainCurrentFlow.

## Test Instance Parameters

| **Parameter Name** | **Required?** | **Type** | **Values** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ |
| DomainName         | Yes           | String   |            |Domain Name.|
| DomainValue        | Yes           | Integer  |            |Domain New Value.|

## OTPL Sample1:
``` Perl
Test PrimeFlowControlSetTestMethod FlowControlSet_CPU_r1_P1
{
	DomainName = "CPU";
	DomainValue = 1;
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

## Additional Dependencies
### Related Test Methods
* FlowControlFork
* FlowControlStart
* FlowControlEnd

### Related Services
* Prime.Services.TestProgramService.CreateFlowDomain
* Prime.Services.TestProgramService.GetDomainCurrentFlow
* Prime.Services.TestProgramService.SetDomainCurrentFlow
* Prime.Services.TestProgramService.IsDffEnableForDomainFlow
* Prime.Services.TestProgramService.GetDffTokenNameForDomainFlow



| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| March 22, 2021 | 1.0.0       | Humberto Ramirez |  Initial doc|