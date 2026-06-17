<h1>Prime Test-Method Specification REP</h1>
Revision 1.0.0

March 12th

[[_TOC_]]

# Methodology
The GetDff test method provides capability to Get the DFF value from the Prime's Internal Database that's generated from the UBE and MTL file.

The test methods allowed user to Get Dff in multiple ways:
1. Get by whole token.
2. Get by Field Name.
3. Get by Index.
4. Get by VID, DieId, OpType, and token name within UNIT or ULT scope.

The value obtained can be stored or just leave it as a console printing:
1. Print to console.
2. Store to shared storage.
3. Store to UserVar.

However, only a single storage is allowed per execution. For example if user choose to store the value in sharedstorage, user cannot choose to store into uservar unless the test method is re-execute again with different setting.

A failing execution will be logged to ituff if the test method is not able to get the dff token.



If a DFFException occurs (such as example above), then the test method will exit port 0. Other types of exception will exit through port -1. 

# Test Instance Parameters

The table below lists and describes the test instance parameters supported by the GetDff test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Default Value** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ | -----|
| TokenName| Yes | String |     Any alpha-numeric and case sensitive.       | n/a| Token name to pull from DFF Database. |
| DieId | No* | String |     Any alpha-numeric and case sensitive.       | n/a| Die Id to pull the dff from. |
| OpType| No* | String |     Any alpha-numeric and case sensitive.       | n/a| Optype for the dff to pull from. |
| Storage| No* | String |     Any alpha-numeric and case sensitive.       | n/a| Storage name to store the dff data that was obtained. |
| Vid| No | String |     Any alpha-numeric and case sensitive.       | n/a| Vid to pull the DFF token data from. |
| ForceGetFromUBE| No | String |     DISABLED by default       | n/a| enable to forcefully get Dff token directly from UBE file. |
| SearchTokenFromUbeScope| No | String |     NONE, UNIT, ULT      | NONE | If enabled, test method matches to token within provided scope using VID, DieId, OpType, and Token params. |
**Notes:**
- For storage parameter, avoid using sharedstorage name with "." as the test method will perceive this as a UserVar.
- Likewise, if the storage parameter has "." it will perceive the name as a UserVar to store the value to.
- If no storage parameter is defined, it will just print the dff value to console if debug mode is enabled.
- If no die id or optype is defined, the test method will get the DFF from the current dff database for the current unit. The die id will use the current die id defined in the test program.
  - if current die id is empty, and no die id is defined in the test method, it will error out.
- When there's no token name defined, it will fail verify as token name is a required parameter.
- When the die id defined and optype does not match with the current unit dff database, it will pull the data from the UBE database that matches the request.
- If VID field is defined, user will also need to define the DieId and the Optype parameter as well. Also take note that the DieId parameter needs to be in Raw ULT value rather than Die value i.e. Y5106803_330_-10_+00 instead of U1.U2 due to the mapping is not there when VID is being used. However, if user is trying to pull PKG or Stack level data, PKG/U1/U2 (or any respective stack id) is valid values.
- if ForceGetFromUBE is ENABLED, DieId and Optype parameters are mandatory. reminder it only contain tokens from active DieId.
- SearchTokenFromUbeScope is NONE by default, if it's ULT or UNIT, VID, DieId and Optype are mandatory. UNIT will search token from UBE file within the VID's unit data (not belonging to any ULT). ULT searches token within all ULT levels within the provided VID. 
 
# Datalog output
If the field is not found it will log as such in ITUFF:
```python
2_tname_GETDFF::GetDffByDieIdOpTypeByFieldNoMTL_F0
2_strgval_NA,GDNEF,NA
```
or when the whole token itself doesn't exist it will log as such in ITUFF:

```python
2_tname_GETDFF::GetDffByDieIdOpTypeByFieldNoMTL_F0
2_strgval_NA,GDNE,NA
```

# TPL Samples
## Get DFF by Whole Token and store value to uservar
```python
Import PrimeGetDffTestMethod.xml;

Test PrimeGetDffTestMethod PrimeGetDffWholeStoreToUserVar_P1
{
    TokenName = "TOKF";
    Storage = "DFFVars.StoredValue";
}
```

## Get DFF by Field and store value to sharedstorage
```python
Import PrimeGetDffTestMethod.xml;

Test PrimeGetDffTestMethod PrimeGetDffByFieldStoreToSharedStorage_P1
{
    TokenName = "TOKF.F1";
    Storage = "StorageA";
}
```

## Get DFF by Index and do not store value (only print to console)
```python
Import PrimeGetDffTestMethod.xml;

Test PrimeGetDffTestMethod PrimeGetDffByIndexPrintToConsoleDoNotStore_P1
{
    LogLevel = "PRIME_DEBUG";
    TokenName = "TOKF[0]";
}
```

## Get DFF by Whole Token with die id defined
```python
Import PrimeGetDffTestMethod.xml;

Test PrimeGetDffTestMethod PrimeGetDffWholeStoreToUserVarWithDieId_P1
{
    TokenName = "TOKF";
    Storage = "DFFVars.StoredValue";
    DieId = "U2.U1";
}
```

## Get DFF by Whole Token with optype and die id defined
```python
Import PrimeGetDffTestMethod.xml;

Test PrimeGetDffTestMethod PrimeGetDffWholeStoreToUserVarWithDieIdAndOptype_P1
{
    TokenName = "TOKF";
    Storage = "DFFVars.StoredValue";
    DieId = "U2.U1";
    OpType = "PBIC_S2";
}
```

## Get DFF by using VID for PKG/Stack level data
```python
Import PrimeGetDffTestMethod.xml;

Test PrimeGetDffTestMethod GetDffByVid_PkgData_P1
{
    BypassPort = -1;
    LogLevel = "PRIME_DEBUG";
    TokenName = "SOT_T1";
    DieId = "PKG";
    OpType = "PBIC_S2";
    Vid = "000000000000Z5X00100000";
}
```

## Get DFF by using VID for ULT level data
```python
Import PrimeGetDffTestMethod.xml;

Test PrimeGetDffTestMethod GetDffByVid_UltData_P1
{
    BypassPort = -1;
    LogLevel = "PRIME_DEBUG";
    TokenName = "TOKA";
    DieId = "Y5106803_330_-10_+00";
    OpType = "PBIC_S2";
    Vid = "000000000000Z5X00100000";
}
```

## Get DFF while ForceGetFromUBE is enabled
```python
Import PrimeGetDffTestMethod.xml;

Test PrimeGetDffTestMethod GetDffFromUbe_P1
{
    LogLevel = "Enabled";
    TokenName = "TOKA";
    DieId = "U2.U1";
    OpType = "PBIC_DAB";
    ForceGetFromUBE = "ENABLED";
}
```

## Get DFF by using VID for ULT level data by using uservar as parameter
```python
Import PrimeGetDffTestMethod.xml;

Test PrimeGetDffTestMethod GetDffByVid_UltData_P1
{
    BypassPort = -1;
    LogLevel = "PRIME_DEBUG";
    TokenName = "TOKA";
    DieId = "UserVar.DieIdToken";
    OpType = "PBIC_S2";
    Vid = "UserVar.VidToken";
}
```

## Get DFF by using VID for UNIT level data
```python
Import PrimeGetDffTestMethod.xml;

Test PrimeGetDffTestMethod TryGetTokenFromUnitData_PkgData_P0
{
    BypassPort = -1;
    LogLevel = "PRIME_DEBUG";
    TokenName = "SOT_T1";
    DieId = "PKG";
    OpType = "PBIC_S2";
    Vid = "000000000000Z5X00100000";
    SearchTokenFromUbeScope = "UNIT";
}
```

## Get DFF by using VID for ULT level data
```python
Import PrimeGetDffTestMethod.xml;

Test PrimeGetDffTestMethod TryGetTokenFromUltData_PkgData_P1
{
    BypassPort = -1;
    LogLevel = "PRIME_DEBUG";
    TokenName = "TOKD";
    DieId = "U2.U1";
    OpType = "PBIC_S2";
    Vid = "000000000000Z5X00100000";
    SearchTokenFromUbeScope = "ULT";
}
```

# Exit Ports

The GetDff test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

  
# Additional Dependencies

N/A

# Version tracking

| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| 12th, March, 2025 | 13.2.0       | Zou Kevin/Caio Fernandes | #54993 - Capability to read and ULT level DFF tokens from UBE using VID |
| 25th July, 2023 | 12.1.0       | Mohd Faiz Mohd Asri | #39783 - GetDFFbyVID not able to interpret Variables (Uservars/GSDS) |
| April, 2023 | 12.0.0       | Lee, Yeong Jui | #37731 - [DFF] Not able to get DFF data from ube for Write optype |
| June, 2021 | 1.0.0       | Mohd Faiz Mohd Asri|              |

# Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **DFF**: Data Feed Forward