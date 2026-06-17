**Prime Test-Method Specification REP**

Revision 1.0.0

May 2021

[[_TOC_]]

## REP for FlowLoopExit

This **REP** is intended to describe the FlowLoopExit Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Datalog output** – A detailed description of what is datalogged by this TestMethod

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document 

## Methodology

The FlowLoopExit test method allows alternate exit from a flow loop for an existing FlowItem based on the looping status. 
If invoked and the current loop value has reached or exceeded the total loop value from the flow loop config, the alternate exit port 0 is utilized.  Otherwise, this instance will exit out port 1.
 
## Test Instance Parameters
The table below lists and describes the test instance parameters supported by the FlowLoopExit test method.

| **Parameter Name** | **Required?** | **Type** |        **Values**        |   **Comments**                                               |
| ------------------ | ------------- | -------- | ------------------------ | ------------------------------------------------------------ |
| FlowItemName       | Yes           | String   |                          | TestInstance Flow Item Name <Module>::<Flow>::<LoopFlowItem>.|
**Notes:**
## TPL Samples

Here is a sample test instance examples using the FlowLoopExit test method.
```python
Import PrimeFlowLoopExitTestMethod.xml;

Test PrimeFlowLoopExitTestMethod FlowLoopExit_NormalExit_P1
{
    FlowItemName = "Stress::StressTestMethods::StressExitFlow";
}

```
## Datalog output
Looping status values will be debug output to the console window if activated.
```python
NORMAL EXIT
=========================
Running Execute() for test instance=[Stress::FlowLoopExit_NormalExit_P1]
=========================
[2021-Apr-12 01:14:18] [DUT: 12]Current loop value is: 2.
[2021-Apr-12 01:14:18] [DUT: 12]
[2021-Apr-12 01:14:18] [DUT: 12]Total loop value is: 5.
[2021-Apr-12 01:14:18] [DUT: 12]
[2021-Apr-12 01:14:18] [DUT: 12]TestInstance=[Stress::FlowLoopExit_NormalExit_P1] exit port=[1].
,Stress::FlowLoopExit_NormalExit_P1,Pass

ALTERNATE EXIT
=========================
Running Execute() for test instance=[Stress::FlowLoopExit_AlternateExit_F0]
=========================
[2021-Apr-12 01:17:42] [DUT: 12]Current loop value is: 5.
[2021-Apr-12 01:17:42] [DUT: 12]
[2021-Apr-12 01:17:42] [DUT: 12]Total loop value is: 5.
[2021-Apr-12 01:17:42] [DUT: 12]
[2021-Apr-12 01:17:42] [DUT: 12]TestInstance=[Stress::FlowLoopExit_AlternateExit_F0] exit port=[0].
,Stress::FlowLoopExit_AlternateExit_F0,Fail

```

## Exit Ports

The FlowLoopExit test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**                             |
| ------------- | --------------- | --------------------------------------------|
| **-2**        | ***Alarm***     | Any alarm condition                         |
| **-1**        | ***Error***     | Any software condition error                |
| **0**         | ***Fail***      | Failing condition - alternate loop exit     |
| **1**         | ***Pass***      | Passing condition - normal loop exit        |

## Additional Dependencies

The flow looping for the particular FlowItem must be set either manually in the MTPL file or with the FlowLoopControl test method.
NOTE: The "BreakLoopOn" in manual setup or "SetBreakRange" parameter in the JSON file must include the selected port to use the alternate looping exit properly.

## Version tracking


| **Date**       | **Version** | **Author**   | **Comments**              |
| -------------- | ----------- | ------------ | ---------------           |
| Apr 12th, 2021 | 1.0.0       | Kevin Krake  | Initial Release           |             |
| May 21st, 2021 | 1.0.1       | Kevin Krake  | Swapped exit port meaning |             |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
