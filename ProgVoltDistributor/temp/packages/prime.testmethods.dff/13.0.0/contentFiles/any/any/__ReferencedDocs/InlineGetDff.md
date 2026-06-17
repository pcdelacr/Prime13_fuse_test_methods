<h1>Prime Test-Method Specification REP</h1>
Revision 1.0.0

Jul 2020

[[_TOC_]]

## Methodology
The InlineGetDff test method provides capability to run a GetInlineDff based on inlineDffEfuseId's that are already loaded into shared storage. 

Execute first retrieves a list of strings from shared storage based on inlineDffEfuseId. It's assumed that this key has been populated prior to the test method being called. If fuse IDs are found in shared storage, GetInlineDff will be called with the fuse IDs found and with the given timeout value. If there are no fuse IDs stored in shared storage, then the get inline dff command is skipped.

If an exception occurs during this portion of the code, then the execution will exit port 0.

After the GetInlineDff call, a call is made to InquireForUltDownloaded() which will check for any ULT data that has been stored in shared storage for printing. If data is found, then the prints will either be: 

```python
2_lsep
2_comnt_InlineDFF_DownloadData_Successful
2_comnt_ULT_{ultData}
```
or
```python
2_lsep
2_comnt_InlineDFF_DownloadData_Unsuccessful
2_comnt_ULT_{ultData}
```
depending on whether the ULT data found is valid.

If a UltException occurs or the ULT data was not found even after a successful API execution, then the test method will exit port 2.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the MyTestMethodName test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Default Value** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ | -----|
| TimeOut | No | Integer |     Must Be Non-negative       | 1000   | Number of seconds before GetInlineDff call times out. |

**Notes:**

  - 
## Datalog output
N/A

## Custom User Code Hooks

N/A

## TPL Samples

Here is an example of a tpl using InlineGetDff with a timeout of 30 seconds:

**TPL Sample1:**

```python
Import InlineGetDff.xml;

Test InlineGetDff PrimeInlineGetDff_F0
{
    TimeOut = 30;
}
```
## Exit Ports

The InlineGetDff test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |
| **2**         | ***Fail*** | ULT Database related exception occurred or ULT data wasn't available from DB, after successful execution of the GetDff API|

  
## Additional Dependencies

N/A

## Version tracking

| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| May 19th, 2020 | 1.0.0       | Lauren McDonald |              |
|                |             |              |              |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
- **DFF**: Data Feed Forward
- **ULT**: Unit Level Traceability 