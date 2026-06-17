**prime Test-Method Specification REP**

[[_TOC_]]

## REP for SignalFile

This **REP** is intended to describe the SignalFile Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose -

  - **Signal File example** – A example of the contents of a Signal File  -

  - **Test Instance Parameters** – A table describes each instance parameter (Name, Required?, Desription) -
  
  - **Matching SCVars**  – A table describes each SCVar and its match criteria (Criteria, SCVar, Criteria Type) -

  - **Matching Samples** – A table describes the file name example, its match criteria, and the criteria type it belongs to (Product specific or Process generic)  -

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file -

  - **Exit Ports** - A table describes each exit port -

  - **Version tracking** – With author names, so you always have a name to address -

  - **Acronyms** - Definition of acronyms used in this document -
<div style="text-align: justify">

## Methodology

Signal File allows to change the TP behavior by changing the value of specific UserVars, based on rules set by two files: Product Specific Signal Files and Process Generic Signal Files.

The user provides this two folder paths for Signal Files, each of the two folders could contain multiple files and the Test Method should select a single one to read and apply. The selection is done via the following hierarchy of criterias, given from highest priority to lowest priority:

**Product Specific Criteria**  
Lot + Wafer + Operation  
Lot + Operation  
Lot + Wafer  
Lot  
```text 
# product file try get file sequence as below:
$"{currentLotName}.{currentWaferName}.{currentLocation}",
$"{currentLotName}.x.{currentLocation}",
$"{currentLotName}.{currentWaferName}.x",
$"{currentLotName}.x.x",
$"{currentLotName}.{currentWaferName}",// SPEXSignalFile requirements
$"{currentLotName}",// SPEXSignalFile requirements

```

**Process Generic Criteria**  
Device + Revision + Stepping + LotWildcard  
Device + LotWildcard  

The specific values that the Test Method will use to match for Lot, Wafer and/or Operation are the current values contained in the SCVars.

Once a match is found the Test Method selects that file and lower priority criteria are not evaluated.

The following are the steps which the Test Method will go through during execution of its main flow:

* Reset modified UserVars to their original value from the previous execution, if any.
* Read the SCVars containing the current Lot, Wafer and Operation values.
* If possible, find a matching Product Specific Signal File by following the Matching Hierarchy defined above(If Local is not exist, will try with Sysc, if Sysc service is down and local file also not exist, will fail verify). If a matching file is found skip Process Generic matching criteria.
* Read the SCVars containing the current Device, Revision and Stepping values.
* If possible, find a matching Process Generic Signal File by following the Matching Hierarchy defined above. If no matching file is found exit through port 1.
* Read the matched signal file and cache the current value of the specified UserVars.
* Try to modify the UserVars with the new values given by the Signal File.
* If value change is succesful exit through port 1, otherwise port 0.

## Signal File example: ##

A Signal File is a text file which provides the specific UserVar/Value pairs to set. The contents of the file should match the following syntax:

```text

  UserVarName Value
  UserVarName Value
  UserVarName Value

```

The user may specify as many UserVar/Value pairs as needed, Signal File supports modifications in uservars of type string and integer. 
 For example, a signal file could contain the following:

```text

  HVQK_Force_Stress_SDT 1
  HVQK_Force_Stress 1

```

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Signal File test method

| **Parameter Name**    | **Required?** | **Description**        | 
| --------------------- | ------------- | --------------- |
| ProductSpecificSignalFilesPath     | Yes           | The drive or network path in which to read signal files from          |
| ProcessGenericSignalFilesPath              | Yes           | The drive or network path in which to read signal files from.         | 

## Matching SCVars 

| **Criteria**    | **SCVar** | **Criteria Type** | 
| --------------------- | ------------- |------------- |
| Lot  | SC_LOTNAME      | Product Specific |
| Wafer           | 	SC_LOT_WAFER         | Product Specific |
| Operation | SC_LOCN        | Product Specific |
| Device          | 	SC_DEVICE     | 	Process Generic
| Revision   | SC_REV     | Process Generic |
| Stepping          | 	SC_STEP       | Process Generic |

## Matching Samples
| **Filename Example**    | **Match Criteria** | **Criteria Type** | 
| --------------------- | ------------- |------------- |
| L12345678.999.132110   | Will match by Lot (L12345678), Wafer (999) and Operation.         | Product Specific |
| L12345678.x.132350             | 	Will match by Lot (L12345678) and Operation only.          | Product Specific |
| L12345678.999.x   | Will match by Lot (L12345678) and Wafer (999) only.        | Product Specific |
| L12345678.999  | Will match by Lot (L12345678) and Wafer (999) only.        | Product Specific | (SPEX newly added requirements)
| L12345678.x.x            | 	Will match by Lot (L12345678) only.        | Product Specific
| L12345678         | 	Will match by Lot (L12345678) only.        | Product Specific (SPEX newly added requirements)
| 8PRPCV.A.S.Dx   | Will match by Device, Revision, Stepping and Lot's first character if the Lot name begins with the provided character. ('D' in this example)       | Process Generic |
| 8PRPx.x.x.Dx          | 	Will match by Device's first four characters (8PRPCV) and Lot's first character if the Lot name begins with the provided character. ('D' in this example)       | Process Generic |

## TPL Samples

```plaintext
Test PrimeSignalFileTestMethod SignalFile_Pass_P1
{
ProductSpecificSignalFilesPath= "~HDMT_TPL_DIR/TestPrograms/SignalFile/Modules/SignalFile/InputFiles/pass/";
ProcessGenericSignalFilesPath = "~HDMT_TPL_DIR/TestPrograms/SignalFile/Modules/SignalFile/InputFiles/";
LogLevel = "Enabled";
}
```

## Exit Ports

| **Exit Port** | **Value**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Execution Failure***      | Uservars Couldn't be modified, Uservar didn't exist or any other error           |
| **1**         | ***Pass***      | Passing condition            |

## Version tracking

| **Date**       | **Version** | **Author**      | **Comments** |
| -------------- | ----------- | --------------  | ------------ |
|Jul 10, 2024    |    13.1     |  Ashley Vasquez |  [Support override of integer type tokens](https://dev.azure.com/mit-us/PRIME/_workitems/edit/49927/)
  |
|May 19, 2025    |    13.3     |  Liu Tao |  [Adopt with SpexSignalFile](https://dev.azure.com/mit-us/PRIME/_workitems/edit/58035)
  |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **JSON**: JavaScript Object Notation
</div>