# REP for Prime's LotEndFinalize TestMethod

This **REP** is intended to describe the LotEndFinalize Prime TestMethod.

[[_TOC_]]

# <span id="Methodology" class="anchor"></span> **Methodology**

The LotEndFinalize Test Method is in charge of doing any necessary actions after a Lot Run. This test method is intended to be used **after** all Lot test methods. 

## <p><span style="color:#ff0000"><strong>Ituff Sensitivity Warning</strong></span></p>
<p><span style="color:#ff0000">Do not enable <em>InstanceSummaryMode</em> for this test method instances. It will cause Ituff to be printed out-of-sequence causing failure down the line. A better solution is being worked on.</span></p>

## <span id="_Toc36332601" class="anchor"></span>**Execute**

:large_blue_diamond: When tester module type is HDMX, TIU counters are updated as part of the final Lot process. This applies to both HDMX and HBI.

### **TIU counter update**

TIU counters are updated as part of  ENDLOT operations.
Current values in EEPROM are incremented with the final count of units, either total or per site:
1.	Baseboard counter. (EEPROM 0, counter 0) + Final total tested unit count
2.	Personality Card counter. (EEPROM 1, counter 0, site X) + Final tested unit count at corresponding site
3.	Socket counter at site X. (EEPROM 1, counter 1, site X) + Final tested unit count at corresponding site
4.	Socket pin replacement counter at site X. (EEPROM 1, counter 2, site X) + Final tested unit count at corresponding site
5.	Socket cleaning counter at site X. (EEPROM 1, counter 3, site X) + Final tested unit count at corresponding site

# <span id="Parameters" class="anchor"></span> **Test Instance Parameters**

The table below lists and describes the test instance parameters supported by the LotEndFinalize test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Comments**                           |
| ------------------ | ------------- | -------- | ---------- | -------------------------------------- |
| LogLevel     | Yes            | String   | “PRIME_DEBUG”, "DISABLED"    | Default value - DISABLED |

# <span id="Hooks" class="anchor"></span> **TPL Samples**

```python
Import PrimeLotEndFinalize.xml;

Test PrimeLotEndFinalize LOTFINALIZE_LOTENDFLOW_INSTANCE
{
}
```
# <span id="FlowOrder" class="anchor"></span> **Test Program Flow order requirements**

The image below ilustrates the order requiremetns for the Evergreen and Prime instances to correctly <font size="3"><span style="color:OrangeRed">Finalize </font> the <font size="3"><span style="color:OrangeRed"> Lot </font>testing session in the libraries.

1. iCItuffTest
1. PrimeLotEndDatalogTestMethod
1. PrimeLotEndFinalizeTestMethod
1. HMI_ituff

::: mermaid
	graph LR  
	classDef EVG fill:#708541;
	classDef PRIME fill:#005B85;
	classDef TOS fill:#B24501;
		subgraph Lot End Subflow
			direction LR
			id1(iCItuffTest ):::EVG -->			
			id2(PrimeLotEndDatalogTestMethod):::PRIME -->
			id3(PrimeLotEndFinalizeTestMethod):::PRIME--> 
			id4("HMI_ituff"):::TOS
		end
:::

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note** </span></font> :warning:
>For HDMT the ITUFF and PUDL control is handled by TOS through the HMI Test Methods. Instances must exist on the Start and End Lot Flows to create and close the ITUFF streams.
>
>HMI_Ituff (OperationMode = LOTTEST_END) instance must be after the PrimeLotEndFinalizeTestMethod instance.  

# <span id="VersionTracking" class="anchor"></span> **Exit Ports**

The LotEndFinalize test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |

# <span id="VersionTracking" class="anchor"></span> **Additional Dependencies**

This test method should be used with Lot test methods and Device test methods.

# <span id="VersionTracking" class="anchor"></span> **Version tracking**

| **Date**       | **Version** | **Author**     | **Comments** |
| -------------- | ----------- | -------------- | ------------ |
| Mar 10, 2022 | 1.0.0| jabarran| LotEndDatalog TM added. | 
| Aug 12, 2022 | 1.0.1 | hramirez | Adding Flow order documentation |

# <span id="VersionTracking" class="anchor"></span> **Acronyms**

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **HDMX**: Next Generation HDMT
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **TM**: Test Method
  - **SCVar**: Station Controller uservar.
  - **EEPROM**: Electronically erasable programmable read-only memory.