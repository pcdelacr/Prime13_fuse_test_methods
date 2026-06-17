**prime Test-Method Specification REP**

Revision 1.0.0

Jul 2020

[[_TOC_]]

## REP for ThermalUeiStream

This **REP** is intended to describe the ThermalUeiStream Prime TestMethod.

## Methodology

The ThermalUeiStream test method is not intended for HVM, this a feature to be used for product characterization only as it will do intensive data collection on thermal pins, consuming considerable time and tester resources.

This Test Method provides capability to enable thermal diode profiling through start and stop actions for DTC and TCC modes (tester dependent) using selected Uei Pins.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ThermalUeiStream test method

| **Parameter Name** | **Required** | **Type**        | **Values** | **Description**           | **Comments**                 |
|--------------------|--------------|-----------------|------------|---------------------------|------------------------------|
| UeiSlaveType       | Yes          | String (choice) | DTC        | Thermal controller to use |                              |
|                    |              |                 | TCC        |                           |                              |
| ActionType         | Yes          | String (choice) | Start      | Start or stop streaming   |                              |
|                    |              |                 | Stop       |                           |                              |
| CollectPins        | Yes          | String (list)   | Pin Names  |                           | Comma separated              |
| DtsMode            | No           | String (choice) | Disabled   | DTS streaming mode        | Default value: Disabled      |
|                    |              |                 | Average    |                           | Dts streaming average values |
|                    |              |                 | Max        |                           | Dts streaming max values     |

### Only for TOS4 and TCC slave type:

| **Parameter Name** | **Required** | **Type**        | **Values** | **Comments**                                                                   |
|--------------------|--------------| --------------- | ---------- |--------------------------------------------------------------------------------|
| PowerPins          | No           | String (list)   | Pin Names  | Comma separated. Only required for power multi zone thermal control using TCC. |


**Notes:**

  - **Pin name(s) are only required for TCC Start**

## Datalog output

This test method does not print any information to ituff nor datalog as it only enables continuous thermal measurements for product characterization

## Custom User Code Hooks

TBD

## TPL Samples

Here are a few test instance examples using the ThermalUeiStream test method

**TPL Sample 1: Tcc Start on 2 Tdau pins**

```python
Import PrimeThermalUeiStreamTestMethod.xml;

Test PrimeThermalUeiStreamTestMethod ThermalUeiStream_TccStart
{
   CollectPins = "TDAU_CH_PCH,TDAU_CH_CORE";
   UeiSlaveType = "TCC";
   ActionType = "Start";
}
```

**TOS4 - TPL Sample 1: Tcc Start on 2 Tdau pins**

```python
CSharpTest PrimeThermalUeiStreamTestMethod ThermalUeiStream_TccStart
{
   CollectPins = "TDAU_CH_PCH,TDAU_CH_CORE";
   UeiSlaveType = "TCC";
   ActionType = "Start";
   PowerPins = "TDAU_PW_PCH_ZONE0,TDAU_PW_CORE_ZONE1";
}
```

**TPL Sample 2: Perform a Dtc Stop**

```python
Import PrimeThermalUeiStreamTestMethod.xml;

Test PrimeThermalUeiStreamTestMethod ThermalUeiStream_DtcStop
{
   CollectPins = "";
   UeiSlaveType = "DTC";
   ActionType = "Stop";
}
```

## Exit Ports

The ThermalUeiStream test method supports the following exit ports:


| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition          |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |

## Additional Dependencies

There are no Test Method dependencies

## Version tracking


| **Date**       | **Version** | **Author**             | **Comments**                                     |
|----------------|-------------|------------------------|--------------------------------------------------|
| Mar 5th, 2020  | 1.0.0       | Alberto Suarez         | Initial document released                        |
| Feb 27th, 2024 | 1.1.0       | Jose Alberto Barrantes | Tos4 new parameter support for power multi zone. |

## Acronyms

Definition of acronyms used in this document:

  - **DTC**: Device Thermal Controller
  - **HDMT**: High Density Modular Tester
  - **HVM**: High Volume Manufacturing
  - **TCC**: Thermal Controller Card
  - **TDAU**: Thermal diode acquisition unit
  - **TPL**: Test Programming Language