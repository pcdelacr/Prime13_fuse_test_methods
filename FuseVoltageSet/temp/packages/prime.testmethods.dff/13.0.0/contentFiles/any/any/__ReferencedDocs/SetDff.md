<h1>Prime Test-Method Specification REP</h1>
Revision 1.0.0

June 2021

[[_TOC_]]

## Methodology
The SetDff test method provides capability to Set the DFF value to the Prime's Internal Database for the current unit.

The test methods allowed user to Set Dff in multiple ways:
1. Set by whole token.
1. Set by Field Name.

The value to set can be in many ways:
1. UserVar
1. Shared storage.
1. Literal value.

This test method allows:
1. User to set the dff to the token from different die id than the current die id as long as it exist in the current dff database. 
2. Multiple tokens and values to be set in a single execution given that number of value matches with the number of token names defined.

A failing execution will be logged to ituff if the test method is not able to set the dff token.

If a DFFException occurs, then the test method will exit port 0. Other types of exception will exit through port -1. 

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the SetDff test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Default Value** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ | -----|
| TokenName| Yes | String |     Any alpha-numeric and case sensitive.       | n/a| Token name to pull from DFF. Delimited by ",". |
| TokenValue| No | String |     Any alpha-numeric and case sensitive.       | n/a| Token value to set. Delimited by ",". |
| DieId| No | String |     Any alpha-numeric and case sensitive.       | n/a| Die Id to pull the dff from. |

**Notes:**
- For TokenValue parameter, if the value contains "." and not preceded with "Storage", it will perceive that the value needs to be resolved from UserVar.
- If TokenValue parameter is preceded by "Storage." it will try to resolve the value from sharedstorage of the storage name after the "." for example "Storage.A" means it will pull the value from sharedstorage of key "A".
- Other than the above two situations, the testmethod will store the value as literal value from the parameter itself.
- Only one die id is allowed to set to. Hence the multiple tokens defined need to be based on the same die id defined.
 
## Datalog output
Datalog is for this test method is only for when it fails to set the Dff. It follows the DffService datalog example:
```python
  2_tname_SETDFF_<TokenName>.<FieldName>_<Die ID>_<TestInstanceName>
  2_strgval_<Kill Status>,<Fail Type>,<Value Set>,<Current Value>
```
For example:

```python
2_tname_SETDFF_RandomToken_U4_Dff::SetDffRandomToken_F0
2_strgval_,DNE,ValueDontMatter
```

- Descriptions for datalogging above

    |Field|Description|
    |-|-|
    |TokenName|Token to Set|
    |FieldName|Optional: Field Provided by User when setting. If not provided, skip printing <.FieldName>|
    |Die ID|Die ID Used during Set DFF|
    |TestInstanceName|Current Test Instance Name|
    |Kill Status|Token's Kill Status according to enabled modules|
    |Validation Results|PASS or FAIL|
    |Fail Type|Print first encountered fail when validating according to priority defined as below. Empty if PASS|
    |Value Set|The new value for the token|
    |Current Value|Original value that is in the token|
- The Fail Type Acronyms are as follow, with descending priority order of the validation MTL rules check 

    |Fail Type|Description|Valid when Kill Status is|
    |-|-|-|
    |DAV|DFF token Access Violation |KILL & NON-KILL|
    |DNE|DFF token Not Exist|KILL & NON-KILL|
    |DNEF|DFF token Field Not Exist|KILL & NON-KILL|
    |DDC|DFF token Delimiter Count Check|KILL & NON-KILL|
    |DINC|DFF token Invalid Character Check|KILL & NON-KILL|
    |DRO|DFF token Required/Optional Check|KILL Only|
    |DFC|DFF token Format Check|KILL Only|
    |DSL|DFF token String Length Check|KILL Only|
    |DRCAV/DRCDV|DFF token Range Check Allowed/Disallowed Values|KILL Only|
## Custom User Code Hooks

N/A

## TPL Samples
**TPL Sample1:**
Set DFF by Whole Token and using literal value:
```python
Import PrimeSetDffTestMethod.xml;

Test PrimeSetDffTestMethod PrimeSetDffWholeLiteralValue_P1
{
    TokenName = "TOKF";
    TokenValue = "123|456|679";
}
```

**TPL Sample2:**
Set DFF by Field and using userVar value:
```python
Import PrimeSetDffTestMethod.xml;

Test PrimeSetDffTestMethod PrimeSetDffByFieldUserVarValue_P1
{
    TokenName = "TOKF.F1";
    TokenValue = "UserVarA.ValueA";
}
```

**TPL Sample3:**
Set Multiple DFF and using sharedstorage value:
```python
Import PrimeSetDffTestMethod.xml;

Test PrimeSetDffTestMethod PrimeSetDffMultiTokensSharedStorage_P1
{
    TokenName = "TOKF.F1,TOKF.F2,TOKE";
    TokenValue = "Storage.ValueA,UserVarA.ValueA,123";
}
```
**TPL Sample4:**
Set Multiple DFF and using sharedstorage value with Die Id:
```python
Import PrimeSetDffTestMethod.xml;

Test PrimeSetDffTestMethod PrimeSetDffMultiTokensWithDieId_P1
{
    TokenName = "TOKF.F1,TOKF.F2,TOKE";
    TokenValue = "Storage.ValueA,UserVarA.ValueA,123";
    DieId = "U1.U2";
}
```
## Exit Ports

The GetDff test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

  
## Additional Dependencies

N/A

## Version tracking

| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| June, 2021 | 1.0.0       | Mohd Faiz Mohd Asri|              |


## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
- **DFF**: Data Feed Forward