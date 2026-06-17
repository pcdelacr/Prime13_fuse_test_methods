<h1>Prime PatConfigReApplyTestMethod Specification REP</h1>
Revision 6.0.0

Jul 2021

[[_TOC_]]

## Methodology

##External preparation
This test method allows the user to re-apply previously applied and stored pattern modifies for the current unit. **Fuse config re-aplication is not currently supported (WIP)**.

- Tag a configuration as "Storable" in the **Aleph Init** JSON files.

![image.png](./.attachments/image-c8a4a745-d78a-4a08-b758-0d2f0ab11b9f.png)

- When using the **regular PatConfig test method** JSON input file set the configuration handle as "ToBeStore".

![image.png](./.attachments/image-0539f48e-ccce-4836-bf8c-6692c670b998.png)
- If the Alpeh configuration was not marked as storable and the handle is set "ToBeStored" an exception will occur.
- During handle application the modification information will be stored
## PatConfigReApply specific 

- During verify, the handles from configurations marked as storable are going to be cached.  
- During Execute, a list containing the names of all the stored modifications is retrieved. Then, cached handles are filter and stored in another list, using the name list. Finally, the new handle list is sent to the ReApplyStoredData method that belongs to the PatConfig Service. Here, all the modifications are set and applied.
- 

#Example:

![image.png](./.attachments/image-c6a627d2-3b1f-4584-9ce9-88fd1cb28f13.png)

![image.png](./.attachments/image-6af471ce-48a8-4c6d-992e-ce48431b3f85.png)
## Test Instance Parameters
This test method does not receive any parameters.

## Datalog output
When using this test method in PrintDebug mode, it prints in console the the name of the configurations stored.

## TPL Sample

Since this test method does not receive any parameters the only thing needed is to call it from the TPL.


```python
Import PrimePatConfigReApplyTestMethod.xml;

Test PrimePatConfigReApplyTestMethod PrimePatConfigReApply
{
   
}
```
## Exit Ports

The MyTestMethodName test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

  
## Additional Dependencies

There should be at least one stored configuration.

## Version tracking

| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| Jul 27th, 2021 | 1.0.0       | Ariel Mata |                |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **PKG**: Package