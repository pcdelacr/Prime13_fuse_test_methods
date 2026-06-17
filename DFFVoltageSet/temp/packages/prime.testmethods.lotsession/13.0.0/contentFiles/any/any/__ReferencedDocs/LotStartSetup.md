**prime Test-Method Specification REP**

Revision 1.0.0

Mar 2022

[[_TOC_]]

# <span id="Methodology" class="anchor"></span> **Methodology**

The LotStartSetup Test Method is in charge of doing any necessary initialization and validation prior to a Lot Run. This test method is intended to be used **before** any other Lot test method. 

## <p><span style="color:#ff0000"><strong>Ituff Sensitivity Warning</strong></span></p>
<p><span style="color:#ff0000">Do not enable <em>InstanceSummaryMode</em> for this test method instances. It will cause Ituff to be printed out-of-sequence causing failure down the line. A better solution is being worked on.</span></p>

## <span id="_Toc36332601" class="anchor"></span>**Verify**

:large_blue_diamond: Verify the uservar **FAB_PROCESS** exists.

## <span id="_Toc36332601" class="anchor"></span>**Execute**

:large_blue_diamond: Initialize shared storage tables for LOT context. This mean the LOT tables are cleared here.  
:large_blue_diamond: Verify the SCVar **SC_TEST_FLOW** exists. Based on this SCVar a different set of SCVars are also verified. More details can be found later [on in this document](#Uservars).  
:large_blue_diamond: DffService UBE database is inititalized here.  
:large_blue_diamond: DffService tokens are merged here to their respective OPTYPES.  

# <span id="Parameters" class="anchor"></span> **Test Instance Parameters**

The table below lists and describes the test instance parameters supported by the LotStartSetup test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Comments**                           |
| ------------------ | ------------- | -------- | ---------- | -------------------------------------- |
| LogLevel     | Yes            | String   | “PRIME_DEBUG”, "DISABLED"    | Default value - DISABLED |

# <span id="TPL" class="anchor"></span> **TPL Samples**

Here are a test instance example using the LotStartSetup test method in
LotStartFlow.

```python
Import PrimeLotStartSetup.xml;
 
Test PrimeLotStartSetup LotStartSetup_LOTSTARTFLOW_INSTANCE
{
}
```
# <span id="FlowOrder" class="anchor"></span> **Test Program Flow order requirements**

The image below ilustrates the order requiremetns for the Evergreen and Prime instances to correctly <font size="3"><span style="color:OrangeRed">Setup</font> the <font size="3"><span style="color:OrangeRed"> Lot </font> testing session in the libraries.

::: mermaid
    graph LR
    classDef EVG fill:#708541;
    classDef PRIME fill:#005B85;
    classDef TOS fill:#B24501;
        subgraph Lot Start Subflow
            direction LR
            id0("HMI_Ituff"):::TOS -->
            id1("iCItuffTest"):::EVG -->
            id2("PrimeLotStartSetupTestMethod"):::PRIME -->
            id3("PrimeLotStartDatalogTestMethod"):::PRIME
        end
:::

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note** </span></font> :warning:
>For HDMT the ITUFF and PUDL control is handled by TOS through the HMI Test Methods. Instances must exist on the Start and End Lot Flows to create and close the ITUFF streams.
>
>HMI_Ituff (OperationMode = LOTTEST_START) instance must be before the PrimeLotStartSetupTestMethod instance.  

# <span id="ExitPorts" class="anchor"></span> **Exit Ports**

The LotStartSetup test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |

# <span id="Dependencies" class="anchor"></span> **Additional Dependencies**
This test method is meant to be used with LotStartDatalog and Device TMs. On the other hand certain uservars and SCVars need to be considered:

## <span id="Uservars" class="anchor"></span> **Uservars**

| Collection | **Name**       | **Usage** | **Comments**     |
| -------------- | -------------- | ----------- | -------------- |
| SCVars | SC_TEST_FLOW** | Indicates the current test program flow type. | Based on its value flow type can be: Package, SingleDie or Wafer.
| SCVars | SC_DEVICE | Later on used in LotStartDatalog TM. | Must exist and not be empty when the test flow is **SingleDie or Wafer**.
| SCVars | SC_LOTNAME | Later on used in LotStartDatalog TM. | Must exist and not be empty when test flow is **SingleDie or Wafer**.
| SCVars | SC_LOCN | Later on used in LotStartDatalog TM. | Must exist and not be empty when test flow is **SingleDie or Wafer**.
| SCVars | SC_PARTIALWAFID | Later on used in LotStartDatalog TM. | Must exist when test flow is **SingleDie**.
| SCVars | SC_LOT_WAFER* | Later on used in LotStartDatalog TM. | Must exist when test flow is **SingleDie or Wafer**. In SingleDie it will be modifed, in Wafer it must exist and not be empty.
| SCVars | SC_WAFERID* | Modified here and later on used in LotStartDatalog TM. | Must exist when test flow is **SingleDie**.
| _UserVars | FAB_PROCESS | Later on used in LotStartDatalog TM. | Must **always** exist.

\* Note: When Test Flow is SingleDie the value stored in SC_PARTIALWAFID will be used to set values to SC_LOT_WAFER and SC_WAFERID. It will work as follows: 
SC_PARTIALWAFID = "WAFER0T123" &rarr; <span style="color:yellow"> SC_LOT_WAFER = "WAFER0"</span>, <span style="color:lightgreen">SC_WAFERID = "WAFER0"</span>.

\*\* Note: This station controller variable must **always** exist and be populated. 

For it to be **Package test flow** said uservar can have the following values: 
- `"QA", "CLASS", "SCLASS", "CCLASS"`. 

For it to be **SingleDieSort test flow** said uservar can have the following values: 
- `"SDTSORT", "SDSORT", "SDSSORT"`. 

For it to be **WaferSort test flow** said uservar can have the following values: 
- `"SORT", "SSORT"`.

# <span id="VersionTracking" class="anchor"></span> **Version tracking**

| **Date**       | **Version** | **Author**     | **Comments** |
| -------------- | ----------- | -------------- | ------------ |
| Mar 10, 2022 | 1.0.0| jabarran| LotEndDatalog TM added. | 
| Aug 12, 2022 | 1.0.1 | hramirez | Adding Flow order documentation |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **TM**: Test Method
  - **SCVar**: Station Controller uservar.