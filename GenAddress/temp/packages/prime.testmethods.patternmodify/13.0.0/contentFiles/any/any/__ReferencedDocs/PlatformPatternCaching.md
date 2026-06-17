<h1>Prime Test-Method Specification REP</h1>
Revision 1.0.1

Oct 2021

[[_TOC_]]

## Methodology
The PlatformPatternCaching test method provides to modify the platform pattern caching behavior and the caching limits.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the PlatformPatternCaching test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ |
| CachingBehavior    | Yes           | String (choice)       | DoNotCache   |
|                    |               |                       | ForceCache   |
|                    |               |                       | MayCache     |
| EntryCapacity      | Yes           | UnsignedInteger       | Sets the entry capacity of the pattern memory cache. |
| VectorCapacity     | Yes           | UnsignedInteger       | Sets the maximum amount of vectors in the pattern memory cache. |
| MaxVectorEntrySize | Yes           | UnsignedInteger       | Sets the maximum size of an entry to be accepted into the cache. |

**For more details about the impact of those values, contact your hdmtOS rep.

**Error Conditions:**

  - If the hdmtApi called fails.


## Exit Ports

PlatformPatternCaching test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |


## Additional Dependencies

N/A

## Version tracking

| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| Jan 13th, 2021 | 1.0.0       | lchavarr     |  Initial Commit |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System