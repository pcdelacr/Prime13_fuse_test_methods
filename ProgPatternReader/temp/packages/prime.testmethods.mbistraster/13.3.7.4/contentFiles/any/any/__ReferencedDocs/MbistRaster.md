[[_TOC_]]

## REP for MbistRaster

This **REP** is intended to describe the MbistRaster Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Datalog output** – A detailed description of what is datalogged by this TestMethod

  - **Custom User Code hooks** – A list of functions available to the user code to override

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document
  
## Methodology
> **Note:** <span style="color: yellow;">Under construction</span>


## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Dc test method

<table>
  <tr>
    <td rowspan="1" style="text-align: center; vertical-align: middle; font-weight: bold">&nbspParameter Name</td>
    <td colspan="1" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Required?</td>
    <td rowspan="1" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Type</td>
    <td rowspan="1" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Value</td>
    <td rowspan="1" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Comments</td>
  </tr>
  <tr>
    <td style="text-align: center; vertical-align: middle">&nbsp;Patlist</td>
    <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;Plist</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;Plist name to be executed.</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  </tr>
  <tr>
    <td style="text-align: center; vertical-align: middle">&nbsp;TimingsTc</td>
    <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;TimingCondition</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;Timing test condition required for plist execution</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  </tr>
  <tr>
    <td style="text-align: center; vertical-align: middle">&nbsp;LevelsTc</td>
    <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;LevelsCondition</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;Levels test condition required for plist execution</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;If anyone is a pinGroup and MeasureSequence selected is SERIAL the group will be splited by pins.</td>
  </tr>
  <tr>
    <td style="text-align: center; vertical-align: middle">&nbsp;CtvCapturePins</td>
    <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;CommaSeparatedString</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;Comma-separated list of pins to capture CTV data.</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  </tr>
  <tr>
    <td style="text-align: center; vertical-align: middle">&nbsp;RasterInputFile</td>
    <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;File</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;Path of the Raster JSON configuration file.</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  </tr>
  <tr>
    <td style="text-align: center; vertical-align: middle">&nbsp;RepairInputFile</td>
    <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;File</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;Path of the Repair JSON configuration file.</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  </tr>
  <tr>
    <td style="text-align: center; vertical-align: middle">&nbsp;EnableRepair</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;Boolean</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;EnableRepair flag.</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  </tr>
  <tr>
    <td style="text-align: center; vertical-align: middle">&nbsp;RasterOffHry</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;String</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;Raster off HRY.</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  </tr>
  <tr>
    <td style="text-align: center; vertical-align: middle">&nbsp;SkipPriority</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;String</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  </tr>
  <tr>
    <td style="text-align: center; vertical-align: middle">&nbsp;LYASharedStorage</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;String</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  </tr>
  <tr>
    <td style="text-align: center; vertical-align: middle">&nbsp;FuseFunctionality</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;String</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  </tr>
  <tr>
    <td style="text-align: center; vertical-align: middle">&nbsp;PlistLimit</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;Integer</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  </tr>
  <tr>
    <td style="text-align: center; vertical-align: middle">&nbsp;EnableFAFI</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;Boolean</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
    <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  </tr>
</table>

## Console output (debug mode)

> **Note:** <span style="color: yellow;">Under construction</span>

## Datalog output

> **Note:** <span style="color: yellow;">Under construction</span>

## Custom User Code Hooks

> **Note:** <span style="color: yellow;">Under construction</span>

## TPL Samples

> **Note:** <span style="color: yellow;">Under construction</span>

## Exit Ports

The Dc test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                          |
| ------------- | ------------- | -------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                                      |
| **-1**        | ***Error***   | Any software condition error                             |
| **0**         | ***Fail***    | Functional fail.                                         |
| **1**         | ***Pass***    | Test Pass with Reapaired result.                         |
| **2**         | ***Fail***    | Test failed with raster success result.                  |
| **3**         | ***Fail***    | Test failed with unrepairableter result.                 |
| **4**         | ***Fail***    | Fail with No Failures Detected result.                   |

## Acronyms

Definition of acronyms used in this document:

  > **Note:** <span style="color: yellow;">Under construction</span>

## Version tracking

| **Date**                  | **Prime Version** | **Author**                    | **Comments**                                                                        |
| ------------------------- | ----------------- | ----------------------------- | ----------------------------------------------------------------------------------- |
| Mar 3<sup>rd</sup>, 2025  | 13.2              | Didier Armando Jimenez Retana | Initial version                                                                     |