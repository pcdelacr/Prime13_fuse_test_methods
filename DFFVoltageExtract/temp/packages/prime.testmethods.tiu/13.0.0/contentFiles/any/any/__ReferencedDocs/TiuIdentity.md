**prime Test-Method Specification REP**

Nov 2022

[[_TOC_]]

# Introduction

The main purpose of this Test Method is to validate that the TIU currently installed is the correct TIU intended for the Test Program.

# Methodology

TiuIdentity Test Method will read the TIU name from tester and match it against the list of Regex specified in **ValidTiuRegex** parameter. Once the match is found, the test method will load the TIU name to "SCVars.TP_TESTBOARD_ID" uservar.


# Test Instance Parameters

| **Parameter Name**    | **Required?**    | **Type**        | **Values**                      |
| --------------------- | ---------------- | --------------- | ------------------------------- |
| ValidTiuRegex         | Yes              | String (Regex pattern) |  Comma separated list of regexes to match with the TIU name.            |

# Custom User Code Hooks

Two user extendable hook is available for user to extend the functionality (e.g additional serial number validation).

```csharp
PreExecuteDecision ITiuIdentityExtension.PreExecuteEvaluation();
bool ITiuIdentityExtension.CustomValidation();
```

## PreExecuteEvaluation()

This method executes right before the Tiu Name is being checked against **ValidTiuRegex**. The method expected a return of `PreExecuteDecision` which the main code will use to decide whether to continue execution of the test method, or to stop execution and exit through port 0 or 1.

The return value:
```csharp
public enum PreExecuteDecision
{
   /// <summary>
   /// Continue the execution.
   /// </summary>
   Continue,

   /// <summary>
   /// Stop the execution and exit port 0.
   /// </summary>
   StopPort0,

   /// <summary>
   /// Stop the execution and exit port 1.
   /// </summary>
   StopPort1,
}
```

Example:
```csharp
PreExecuteDecision ITiuIdentityExtension.PreExecuteEvaluation()
{
   if (TesterIsOffline())
   {
      return PreExecuteDecision.StopPort1;
   }

   if (EepromValidationFailed())
   {
      return PreExecuteDecision.StopPort0;
   }

   return PreExecuteDecision.Continue;
}
```

This method is being used to check if the tester is offline and stop execution through exit port 1 if the tester is found offline as default implementation.

## CustomValidation()

This method is provided to user for the purpose of doing custom validation after TIU name is checked against **ValidTiuRegex** (e.g additional serial number check). It requires a return of boolean value (true or false). A false will cause the test method to fail and exit through port 0.

# TPL Samples

Here are a few test instance examples using the TiuIdentity test method

```javascript
Import PrimeTiuIdentityTestMethod.xml;

Test PrimeTiuIdentityTestMethod TiuNameCheck_P1
{
   ValidTiuRegex = "V[a-zA-Z0-9]*Tiu,D[a-zA-Z0-9]*Tiu";
}
```
The test instance above will match with TIU Name
- "ValidTiu"
- "Valid25Tiu
- "Dummy109Tiu"

# Exit Ports

The TiuIdentity test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                                    |
| ------------- | ------------- | ---------------------------------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                                                                |
| **-1**        | ***Error***   | Any software condition error                                                       |
| **0**         | ***Fail***    | **Failing condition.** TIU name doesn't match any of the regexes specified, OR `PreExecuteEvaluation()` returns `StopPort0`, OR `CustomValidation()` returned a `false`.          |
| **1**         | ***Pass***    | **Passing condition.** TIU name match found AND `CustomValidation()` returned a `true`, OR `PreExecuteEvaluation()` returns `StopPort1`. |

# Version tracking

| **Date**   | **Prime release** | **Author**    | **Comments** |
| ---------- | ----------- | ------------- | ------------ |
| Dec 2nd 2021 | 7.02.00       | Yusof, Adam Malik | 1st version             |
| Nov 24 2022 | 12.00.00       | Yusof, Adam Malik | Introduction of new extension method `PreExecuteEvaluation()`             |

# Acronyms

Definition of acronyms used in this document:

  - **DUT**: Device Under Testing
  - **TIU**: Test Interface Unit
  - **HDMT**: High Density Modular Tester
  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TPL**: Test Programming Language