# **REP for DeviceEndFinalize**

This **REP** (**prime Test-Method Specification**) is intended to describe the functionality and architecture of DeviceEndFinalize Prime TestMethod.

[[_TOC_]]

# <span id="Methodology" class="anchor"></span> **Methodology**
The purpose of DeviceEndFinalize is to run the unit finalizations. The TM is composed only by the main class **PrimeDeviceEndFinalizeTestMethod**.

## <p><span style="color:#ff0000"><strong>Ituff Sensitivity Warning</strong></span></p>
<p><span style="color:#ff0000">Do not enable <em>InstanceSummaryMode</em> for this test method instances. It will cause Ituff to be printed out-of-sequence causing failure down the line. A better solution is being worked on.</span></p>

## **Verify**
:large_blue_diamond: Sets up PRIME Brita if enabled

## **Execute**
:large_blue_diamond: Calls pin service DeviceEnd  
:large_blue_diamond: Disables flow trace for PRIME Brita  
:large_blue_diamond: Disables simba bypass

# <span id="Parameters" class="anchor"></span> **Test Instance Parameters**

| **Parameter Name** | **Description** |**Is required** | **Type** | **Supported Values** | **Default Value** |
|------------------|------------------|------------------|------------------|------------------|------------------|

# <span id="TPL" class="anchor"></span> **TPL Samples**
In order to create a new instance of DeviceEndFinalize the following test needs to be added to the tpl file. Example shows a test with all instance parameters, including the optional ones.

```python
    import PrimeDeviceEndFinalizeTestMethod.xml;

    Test PrimeDeviceEndFinalizeTestMethod DeviceEndFinalize_P1 {
        LogLevel = "PRIME_DEBUG";
    }
```

# <span id="FlowOrder" class="anchor"></span> **Test Program Flow order requirements**

The image below ilustrates the order requiremetns for Prime instances to correctly <font size="3"><span style="color:OrangeRed">Finalize</font> the <font size="3"><span style="color:OrangeRed"> Device </font> testing session in the libraries.

1. PrimeDeviceEndDatalogTestMethod 
1. PrimeOdeseBinConverterTestMethod
1. PrimeDeviceEndFinalizeTestMethod
1. HMI_TestPlanSetup (in OperationMode = TESTPLAN_END) (<font size="3"><span style="color:OrangeRed">TOS3 Only</font>)

::: mermaid
	graph LR
	classDef PRIME fill:#005B85;
	classDef TOS fill:#B24501;
		subgraph Device End Subflow
			direction LR
			id2(PrimeDeviceEndDatalogTestMethod ):::PRIME --> 
			id3(**PrimeOdeseBinConverterTestMethod**):::PRIME -->
			id4(PrimeDeviceEndFinalizeTestMethod):::PRIME -->
			id5("HMI_TestPlanSetup (TOS3 Only)"):::TOS
		end
:::

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note for TOS3** </span></font> :warning:
>For mDUT usage the HMI_TestPlanSetup instance must be executed; this HMI instance will flush all the datalog buffers for each DUT avoiding data interleaving.
>
>HMI_TestPlanSetup (OperationMode = TESTPLAN_END) instance must be after the PrimeDeviceEndFinalizeTestMethod instance.

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note** </span></font> :warning:
> Please be aware that any PrimeDeviceEndFinalizeTestMethod instance MUST run after a PrimeDeviceEndDatalogTestMethod instance.

# <span id="ExitPorts" class="anchor"></span> **Exit Ports**

The DeviceEndFinalize Test Method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |

# <span id="Dependencies" class="anchor"></span> **Additional Dependencies**

This test method has a dependency with all Lot and Device TMs from PRIME. The use of DeviceEndFinalize without Lot and Device TMs is not a supported setup, **any attempt to not follow this rule can cause errors**.

# <span id="VersionTracking" class="anchor"></span> **Version tracking**

| **Date**       | **Version** | **Author**     | **Comments** |
| -------------- | ----------- | -------------- | ------------ |
| Jun 29, 2022 | 1.0.0 | mjhernan | DeviceEndFinalize TM added |
| Aug 12, 2022 | 1.0.1 | hramirez | Adding Flow order documentation |
| Aug 13, 2024 | 13.01.00 | ckhoh    | Update Flow order documentation |

# <span id="Acronyms" class="anchor"></span> **Acronyms**

Definition of acronyms used in this document:

  - **REP**: Prime Trst-Method Specification