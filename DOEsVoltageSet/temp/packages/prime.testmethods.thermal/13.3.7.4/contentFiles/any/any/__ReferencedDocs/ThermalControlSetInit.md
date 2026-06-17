**prime Test-Method Specification REP**

Revision 11.0.0

Jul 2022

[[_TOC_]]

## REP for ThermalControlSetInit

This **REP** is intended to describe the ThermalControlSetInit Prime TestMethod.

## Methodology

The ThermalControlSetInit test method sets the ControlSet configuration from the selected TemperatureSet. The mapping between control set names and indexes is stored in the internal Prime storage. It is required for the application of Control Sets later on the TP.
Take into consideration that the TemperatureSet Configurations are defined previously in the ThermalControlSetService by the InitializeServicesTestMethod.


**Notes**
1. Place the ControlSetInit test Instance after executing the InitializeServicesTestMethod and before executing InitializeInstancesTestMethod
## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ThermalControlSet test method

<table>
<thead>
<tr class="header">
<th><strong>Parameter Name</strong></th>
<th><strong>Required</strong></th>
<th><strong>Type</strong></th>
<th><strong>Values</strong></th>
<th><strong>Comments</strong></th>
</tr>
</thead>
<tbody>
<tr class="even">
<td>TemperatureSetName</td>
<td>Yes</td>
<td>String</td>
<td>Temperature Set<br />
Name</td>
<td>It has to be a configuration from the json file.</td>
</tr>
</tbody>
</table>

## Datalog output

This test method does not print any information to ituff nor datalog. It only prints relevant information to the console.

## Examples

**MTPL Sample1: Regular call to load an IndexTag named Dispense**

```python
Import PrimeThermalControlSetTestMethod.xml;

Test PrimeThermalControlSetInitTestMethod ControlSetInit
{
   TemperatureSetName = "tgl_u42_ct_100C_TCC";
}
```

## Exit Ports

The ThermalControlSetInit test method supports the following exit ports:

 
| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition          |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |


## Version tracking


| **Date**      | **Version** | **Author**     | **Comments**              |
| ------------- | ----------- | -------------- | ------------------------- |
| Mar 5th, 2020 | 1.0.0       | Alberto Suarez | Initial document released |
| Jan 7th, 2021 | 1.0.1       | Viviana Villalobos| Change to aleph and json files|
## Acronyms

Definition of acronyms used in this document:

  - **HDMT**: High Density Modular Tester
  - **TDAU**: Thermal diode acquisition unit
  - **TPL**: Test Programming Language