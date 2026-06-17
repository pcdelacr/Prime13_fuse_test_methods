Prime Test-Method Specification REP

Revision 1.0.0

March 2021

[[_TOC_]]

## Methodology

It is a simple Test Method to initialize the Domain Flow Control structure. This test method should be called at the begging of each unit, preferably in the test plan start flow.

Considerations:

1. DefaultValue must be a number greater than 0.
2. If DffTokenOverride is set to TRUE, the DffTokenName can’t be empty

<span style="color: #FF5662">Notes:</span>
[1] Please note that Flow control is a solution for products that didn’t move to TORCH or TOS MTT syntax. MTT provides the same capabilities as Flow Control.
[2] Single Flow recovery supported on Evergreen can be achieved by a user callback code that compares a parameter with the flow number and the value returned by Prime.Services.TestProgramService.GetDomainCurrentFlow.

## Test Instance Parameters

| **Parameter Name** | **Required?** | **Type** | **Values** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ |
| DomainName         | Yes           | String   |            |Domain Name.|
| DefaultValue       | Yes           | Integer  |            |Domain Initial default Value.|
| DffTokenName       | No            | String   |            | DFF Token Name to be used as source of domain flow number.|
| DffTokenOverride   | No            | Choice   |   False    | Specify if Fork will be based on DFF Token Value.|

## OTPL Sample1:
``` Perl
Test PrimeFlowControlStartTestMethod FlowControlStart_CPU_P1
{
	DomainName = "CPU";	
	DefaultValue = 2;
	DffTokenName = "";
	UseDffToken = "FALSE";
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
* FlowControlEnd

### Related Services
* Prime.Services.TestProgramService.CreateFlowDomain
* Prime.Services.TestProgramService.GetDomainCurrentFlow
* Prime.Services.TestProgramService.SetDomainCurrentFlow
* Prime.Services.TestProgramService.IsDffEnableForDomainFlow
* Prime.Services.TestProgramService.GetDffTokenNameForDomainFlow

## Version tracking


| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| March 22, 2021 | 1.0.0       | Humberto Ramirez |  Initial doc|       