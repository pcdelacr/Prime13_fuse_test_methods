# REP for DeviceStartSetup

This **REP** is intended to describe the DeviceStartSetup Prime TestMethod.

[[_TOC_]]

# Methodology

The DeviceStartSetup test method provides a placeholder for Prime activities needed to be executed at the beginning of the Device.  
Right now these are the features running in this TestMethod at `Verify`:
1. Verify the uservar `_UserVars.SC_GROUPID` exists. Otherwise an exception will be thrown.

Right now these are the features running in this TestMethod at `Execute`:
1. Reset the SharedStorageService data which is Per-Unit data, this includes `DUT` and `IP` tables.
2. Reset the PerformanceService recorded data which is Per-Unit data.
3. Enable SimbaService.
4. By default, Brita (Flow Trace) will be enabled. When the uservar `_UserVars.ENABLE_PRIME_BRITA` exists and is equal to `"FALSE"`, Brita Flow Trace will NOT be enabled.
5. Increment the current processed unit count by one for the current socket.
6. Increment the test order group count by one for the current DUT.
7. Decrement global sample rate value by one.

## <p><span style="color:#ff0000"><strong>Ituff Sensitivity Warning</strong></span></p>
<p><span style="color:#ff0000">Do not enable <em>InstanceSummaryMode</em> for this test method instances. It will cause Ituff to be printed out-of-sequence causing failure down the line. A better solution is being worked on.</span></p>

# Test Instance Parameters
The table below lists and describes the test instance parameters supported by the DeviceStartSetup test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Comments**                           |
| ------------------ | ------------- | -------- | ---------- | -------------------------------------- |
| LogLevel     | Yes            | String   | “PRIME_DEBUG”, "DISABLED"    | Default value - DISABLED  |
| SmartTc | No | SmartTcStatus | Enabled <br/> Disabled | Disable |

**Notes:**

  - DeviceStartSetup does not use any Instance Parameters but current TOS Version requires to use at least one instance parameter.

# Datalog output
This test method does not generate any output for any datalog.
# Custom User Code Hooks

There are no user code hooks available in this Test Method.

# TPL Samples

Here are a few test instance examples using the DeviceStartSetup test method

TPL Sample1:  

```python
Import PrimeDeviceStartSetupTestMethod.xml;
Test PrimeDeviceStartSetupTestMethod PrimeDeviceStart
{
   LogLevel = "PRIME_DEBUG";
}
```

# Test Program Flow order requirements

The image below ilustrates the order requiremetns for the Evergreen and Prime instances to correctly <font size="3"><span style="color:OrangeRed">Setup</font> the <font size="3"><span style="color:OrangeRed"> Device </font>testing session in the libraries.

1. PrimeDeviceStartSetupTestMethod  
1. PrimeDeviceStart[***]DatalogTestMethod
1. iCBkndTest (in StartOfDevice Mode)   

::: mermaid
	graph LR 
	classDef EVG fill:#708541;
	classDef PRIME fill:#005B85;
	classDef TOS fill:#B24501;
		subgraph Device Start Subflow
			direction LR
			id0("HMI_TestPlanSetup "):::TOS --> 
			id1(PrimeDeviceStartSetupTestMethod ):::PRIME -->
			id2("PrimeDeviceStart[***]DatalogTestMethod"):::PRIME -->
			id3(iCBkndTest):::EVG
		end
:::

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note** </span></font> :warning:
>For mDUT usage the HMI_TestPlanSetup instance must be executed; this HMI instance will flush all the datalog buffers for each DUT avoiding data interleaving.
>
>HMI_TestPlanSetup (OperationMode = TESTPLAN_START) instance must be before the PrimeDeviceStartSetupTestMethod instance.

# Exit Ports

The DeviceStartSetup test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |
| **N**         | ***Pass/Fail*** | Failing condition            |

# Additional Dependencies

More dependencies to consider for this TestMethod to well operate:
This TM instance is intended to be placed before Device datalog TMs:
- DeviceStartPackageDatalog
- DeviceStartSingleDieDatalog
- DeviceStartWaferDatalog
- DeviceEndDatalog
- DeviceEndFinalize

Failing to do so can result in unexpected errors and strange ituff output from the Device datalog TMs.
# Version tracking

| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| Mar 12th, 2022 | 1.0.0       | jabarran |              |
| Aug 12, 2022 | 1.0.1 | hramirez | Adding Flow order documentation |
| Mar 4th, 2023| 1.0.2 | htiow | Adding global sample rate decrement feature |

# Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
