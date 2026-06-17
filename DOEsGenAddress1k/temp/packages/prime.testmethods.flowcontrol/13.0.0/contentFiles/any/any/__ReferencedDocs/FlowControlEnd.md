Prime Test-Method Specification REP

Revision 1.0.0

May 2021

[[_TOC_]]

## Methodology

It is a simple Test Method to datalog and set shared storage and/or DFF tokens for the flow number for a given domain.
Expected to run at the end of the flow.

<span style="color: #FF5662">Notes:</span>
[1] Please note that Flow control is a solution for products that didn’t move to TORCH or TOS MTT syntax. MTT provides the same capabilities as Flow Control.
[2] Single Flow recovery supported on Evergreen can be achieved by a user callback code that compares a parameter with the flow number and the value returned by Prime.Services.TestProgramService.GetDomainCurrentFlow.

## Test Instance Parameters

| **Parameter Name** | **Required?** | **Type** | **Values** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ |
| DomainName         | Yes           | String   |            | Domain Name.|
| SharedStorageKey   | No            | String   |            | SharedStorage Key (Integer Context.DUT).|
| DffTokenName       | No            | String   |            | DFF Token Name to be used as destination for flow number.|
| Datalog            | No            | Choice   |  Disabled | Specify if flow number needs to be printed to datalog.|

## OTPL Sample1:
``` Perl
Test PrimeFlowControlEndTestMethod FlowControlEnd_CPU_P1
{
	DomainName = "CPU";	
	SharedStorageKey = "";
	DffTokenName = "";
	Datalog = "Enabled";
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
* FlowControlSet
* FlowControlFork
* FlowControlStart

### Related Services
* Prime.Services.TestProgramService.CreateFlowDomain
* Prime.Services.TestProgramService.GetDomainCurrentFlow
* Prime.Services.TestProgramService.SetDomainCurrentFlow
* Prime.Services.TestProgramService.IsDffEnableForDomainFlow
* Prime.Services.TestProgramService.GetDffTokenNameForDomainFlow

## Version tracking


| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| April 15, 2021 | 1.0.0       | fmurillo|  Initial doc|       