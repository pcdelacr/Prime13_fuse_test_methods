prime Test-Method Specification REP

Revision 1.0.0

March 2021

[[_TOC_]]

## Methodology
The test method will return the port based on any of the following three conditions:

* Default value specified in the FlowControlStart instance
* The last value set on the FlowControlSet instance
* The DFF token's value is determined on the FlowControlStart instance in the DffTokenName If the DffTokenOverride is set to TRUE.

<span style="color: #FF5662">Notes:</span>
[1] Please note that Flow control is a solution for products that didn’t move to TORCH or TOS MTT syntax. MTT provides the same capabilities as Flow Control.
[2] Single Flow recovery supported on Evergreen can be achieved by a user callback code that compares a parameter with the flow number and the value returned by Prime.Services.TestProgramService.GetDomainCurrentFlow.

## Test Instance Parameters

| **Parameter Name** | **Required?** | **Type** | **Values** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ |
| DomainName         | Yes           | String   |            |Domain Name.|

## OTPL Sample1:
``` Perl
Test PrimeFlowControlForkTestMethod FlowControlFork_CPU_P2
{
	DomainName = "CPU";
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
| **6**         | ***Pass***      | Passing condition            |
| **7**         | ***Pass***      | Passing condition            |
| **8**         | ***Pass***      | Passing condition            |
| **9**         | ***Pass***      | Passing condition            |

## Additional Dependencies
### Related Test Methods
* FlowControlSet
* FlowControlStart
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