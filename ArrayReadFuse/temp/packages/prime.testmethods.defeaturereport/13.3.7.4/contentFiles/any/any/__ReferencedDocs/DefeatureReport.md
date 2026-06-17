**Prime Test-Method Specification REP**

Revision 1.0.0

April 2022

[[_TOC_]]

 ## REP for DefeatureReport

 This **REP** is intended to describe the DefeatureReport Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Datalog output** – A detailed description of what is datalogged by his TestMethod

  - **Custom User Code hooks** – A list of functions available to the user code to override

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document

  ## Methodology

  The DefeatureReport test method provides the capability to write a report of defeatured elements from various data sources. The report can be written to iTuff and SharedStorage for further consumption.

  At Verify (or Init if configured) the report configuration file is read and stored into SharedStorage. If the same file is read before by different test instance and user didn't configure the instance to always read from file, the configuration is obtained from SharedStorage in order to save test time.

  At Execute, the report will be written according to the report configuration. The report can contain the vector of the structure elements in binary string format, the number of enabled element in the structure (represented by 0s in the vector), and the number of defeatured element in the structure (represented by 1s in the vector). The vector are the result of OR operations from multiple data inputs and the data inputs can be sourced from either DefeatureTracking service, SharedStorage or UserVars. If the data is sourced from either SharedStorage or UserVars, user is responsible to provide the key to the SharedStorage row or the UserVars collection and name.

  ## Test Instance Parameters

  The table below lists and describes the test instance parameters supported by the LotEnd test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ |
| InputFilePath      | Yes           | String   |            | The path to the config file. |
| SubFlow            | Yes           | String   |            | SubFlow to report as configured in the file. |
| ReadConfigFromFile | Yes           | String   | ONLY_ONCE, ALWAYS | ONLY_ONCE will check if the file is already stored in SharedStorage, obtain the configuration from SharedStorage if exists. ALWAYS will always read the configuration from file. |

## Datalog Output

If user configure the report to be written to datalog, the report will be in the following format:-

```python
2_tname_DEFEATURED_[subflow]_[socket]_[Ip]_VECTOR
2_strgval_[vector]
2_tname_DEFEATURED_[subflow]_[socket]_[IPtype]_GOODCOUNT
2_mrslt_[good element count]
2_tname_DEFEATURED_[subflow]_[socket]_[IPtype]_BADCOUNT
2_mrslt_[defeatured element count]
```

Example
```python
2_tname_DEFEATURED_FINAL_COLD_SLICE_VECTOR
2_strgval_0101
2_tname_DEFEATURED_FINAL_COLD_SLICE_GOODCOUNT
2_mrslt_2
2_tname_DEFEATURED_FINAL_COLD_SLICE_BADCOUNT
2_mrslt_2
```

## TPL Samples

Here is test instance examples using the DefeatureReport test method:

```python
Import PrimeDefeatureReportTestMethod.xml;
Test PrimeDefeatureReportTestMethod GoodReport_P1
{
	LogLevel = "PRIME_DEBUG";
	InputFilePath = ".\\Modules\\DEFEATUREREPORT\\DEFEATUREREPORT\\InputFiles\\Good.DefeatureReport.json";
	SubFlow = "SubFlow_1";
}
```

Here is the configuration setup JSON file:
```python
{
  "Subflows": [
    {
      "Name": "SubFlow_1",
      "IP": [
        {
          "Name": "IP_1",
          "Destinations": [ "Storage", "Datalog" ],
          "BitWidth": 4,
          "Sockets": [
            {
              "Name": "Socket1_Service",
              "ReportItems": [ "Vector", "GoodCount", "BadCount" ],
              "Inputs": [
                {
                  "Structure": "Struct1",
                  "BitMap": [ 3, 2, 1, 0 ],
                  "SourceType": "Service"
                },
                {
                  "Structure": "Struct2",
                  "BitMap": [ 0, 3, 1, 2 ],
                  "SourceType": "Service"
                }
              ]
            },
            {
              "Name": "Socket2_UserVar",
              "ReportItems": [ "Vector", "GoodCount", "BadCount" ],
              "Inputs": [
                {
                  "Structure": "Struct1",
                  "BitMap": [ 0, 1, 2, 3 ],
                  "SourceType": "UserVar",
                  "SourceName": "DefeatureVars.Struct1_Tracking"
                },
                {
                  "Structure": "Struct2",
                  "BitMap": [ 0, 3, 1, 2 ],
                  "SourceType": "UserVar",
                  "SourceName": "DefeatureVars.Struct2_Tracking"
                }
              ]
            },
            {
              "Name": "Socket3_Storage",
              "ReportItems": [ "Vector", "GoodCount", "BadCount" ],
              "Inputs": [
                {
                  "Structure": "Struct1",
                  "BitMap": [ 3, 2, 1, 0 ],
                  "SourceType": "Storage",
                  "SourceName": "Struct1_Tracking"
                },
                {
                  "Structure": "Struct2",
                  "BitMap": [ 0, 3, 1, 2 ],
                  "SourceType": "Storage",
                  "SourceName": "Struct2_Tracking"
                }
              ]
            }
          ]
        },
        {
          "Name": "IP_2",
          "Destinations": [ "Storage", "Datalog" ],
          "BitWidth": 4,
          "Sockets": [
            {
              "Name": "Socket1_Service",
              "ReportItems": [ "Vector", "GoodCount", "BadCount" ],
              "Inputs": [
                {
                  "Structure": "Struct1",
                  "BitMap": [ 3, 2, 1, 0 ],
                  "SourceType": "Service"
                },
                {
                  "Structure": "Struct2",
                  "BitMap": [ 0, 3, 1, 2 ],
                  "SourceType": "Service"
                }
              ]
            }
          ]
        }
      ]
    }
  ]
}
```

## Exit Ports

DefeatureReport test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                         |
| ------------- | ------------- | ------------------------------------------------------------------------|
| **-2**        | ***Alarm***   | Any alarm condition                                                     |
| **-1**        | ***Error***   | Any software condition error                                            |
| **0**         | ***Fail***    | Failure by either DatalogService or SharedStorageService.               |
| **1**         | ***Pass***    | No issues in writing reports.                                           |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System

## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**    |
| ------------------------- | ----------- | ----------------- | --------------- |
| April 27<sup>th</sup>, 2022 | 1.0.0       | Adam Malik      | Initial version |