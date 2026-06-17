<h1>Prime Test-Method Specification REP</h1>
Revision 1.0.0

August 2021

[[_TOC_]]

## Methodology
The CurrentDieIdManager test method provides capability to get and set of the current DieId mainly from DffService. The test method also adding UserVarService capability for its get and set operations.

The test methods allowed user to interact with Current DieId:
1. Get Current Die Id
2. Set Current Die Id

The value obtained can be stored to uservar or be printed to console:
1. Print to console.
2. Store to UserVar.
3. Get from UserVar.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the CurrentDieIdManager test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Default Value** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ | -----|
| OperationHandler| Yes | String |    GetOperation, SetOperation       | GetOperation|  |
| DieIdValue| No | String |     Any alpha-numeric and case sensitive.       | n/a| a value/uservar for Current DieId. |

**Notes:**
- While GetOperation, non-empty DieIdValue parameter will only allow existing UserVar, or else it will fail on verify.
- While on GetOperation, with DieIdValue empty, it will only print the current DieId to console log.
- DieIdValue on SetOperation can be a string of value(e.g. U2.U1) or a UserVar (e.g. UserVarCollection.UserVarName).
- While on SetOperation, with DieIdValue empty, it will clear the current DieId.
- Every print to console related activities need to switch LogLevel to "PRIME_DEBUG" to see its full informations.
- All FatalException will exit port 0.
 
## Datalog output
N/A

## Custom User Code Hooks
N/A

## TPL Samples
**TPL Sample1:**
SetCurrentDieId from UserVar:
```python
Import PrimeCurrentDieIdManagerTestMethod.xml;

Test PrimeCurrentDieIdManagerTestMethod SetCurrentDieIdFromUserVar_P1
{
    OperationHandler = "SetOperation";
    DieIdValue = "DFFVars.DieID";
}
```

**TPL Sample2:**
SetCurrentDieId by string value:
```python
Import PrimeCurrentDieIdManagerTestMethod.xml;

Test PrimeCurrentDieIdManagerTestMethod SetCurrentDieId_P1
{
    OperationHandler = "SetOperation";
    LogLevel = "PRIME_DEBUG";
    DieIdValue = "U2.U1";
}
```

**TPL Sample3:**
Get CurrentDieId (only print to console):
```python
Import PrimeCurrentDieIdManagerTestMethod.xml;

Test PrimeCurrentDieIdManagerTestMethod GetCurrentDieIdToConsoleLog_P1
{
    LogLevel = "PRIME_DEBUG";
}
```

**TPL Sample4:**
Get CurrentDieId to a UserVar:
```python
Import PrimeCurrentDieIdManagerTestMethod.xml;

Test PrimeCurrentDieIdManagerTestMethod GetCurrentDieIdToUserVar_P1
{
    OperationHandler = "GetOperation";
    LogLevel = "PRIME_DEBUG";
    DieIdValue = "DffVars.DieID1";
}
```

**TPL Sample5:**
Clear the CurrentDieId:
```python
Import PrimeCurrentDieIdManagerTestMethod.xml;

Test PrimeCurrentDieIdManagerTestMethod ClearCurrentDieId_P1
{
    OperationHandler = "SetOperation";
    LogLevel = "PRIME_DEBUG";
    DieIdValue = "";
}
```

## Exit Ports

The CurrentDieIdManager test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

  
## Additional Dependencies

N/A

## Version tracking

| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| August, 2021 | 1.0.0       | Lee, Yeong Jui|              |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
- **DFF**: Data Feed Forward