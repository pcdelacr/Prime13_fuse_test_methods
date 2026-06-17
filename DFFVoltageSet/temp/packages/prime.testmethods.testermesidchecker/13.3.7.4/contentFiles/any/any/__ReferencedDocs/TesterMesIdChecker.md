**prime Test-Method Specification REP**

Dec 2022

[[_TOC_]]

# Introduction

The main purpose of this Test Method is to validate that the different MesId currently installed is the correct MesId intended for the Test Program.

# Methodology

TesterMesIdChecker Test Method will read the TesterMesId name from tester and match it against the list of Regex specified in **ValidTesterMesIdRegex** parameter.

# Test Instance Parameters

| **Parameter Name**    		| **Required?**    | **Type**        		| **Values**                      										|
| ----------------------------- | ---------------- | ----------------------	| --------------------------------------------------------------------	|
| ValidTesterMesIdRegex         | Yes              | String (Regex pattern) |  Comma separated list of regexes to match with the TesterMesId name.	|

# TPL Samples

Here are a few test instance examples using the TesterMesIdChecker test method

```javascript
Import PrimeTesterMesIdCheckerTestMethod.xml;

Test PrimeTesterMesIdCheckerTestMethod TesterMesIdCheck_P1
{
   ValidTesterMesIdRegex = "V[a-zA-Z0-9]*TesterMesId,Du[a-zA-Z0-9]*TesterMesId";
}
```
The test instance above will match with TesterMesId Name
- "ValidTesterMesId"
- "Valid1TesterMesId"
- "Dummy121TesterMesId"

# Exit Ports

The TesterMesIdChecker test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                                    																|
| ------------- | ------------- | ------------------------------------------------------------------------------------------------------------------------------------------		|
| **-2**        | ***Alarm***   | Any alarm condition                                                                																|
| **-1**        | ***Error***   | Any software condition error                                                       																|
| **0**         | ***Fail***    | **Failing condition.** Tester name does not match with any of specified Tester MesId regexes, OR `CustomValidation()` returned a `false`.			|
| **1**         | ***Pass***    | **Passing condition.** Tester name is valid AND `CustomValidation()` returned a `true`. |

# Version tracking

| **Date**   	| **Prime release** | **Author**    		| **Comments**	|
| -------------	| ----------- 		| ------------- 		| ------------	|
| December 2022 | 12.00.00       	|Azizi, Nurul Aziemah	|				|

# Acronyms

Definition of acronyms used in this document:

  - **HDMT**: High Density Modular Tester
  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TPL**: Test Programming Language