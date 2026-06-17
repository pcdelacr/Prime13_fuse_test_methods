**Prime Test-Method Specification REP**

[[_TOC_]]

## REP for SortTBin Test Method

This **REP** is intended to describe the behavior and features of the SortTBin Prime Test Method.

# Introduction

Total Bin (TBin) was born out of the need for a new binning strategy that did not impact wafer manufacturing indicators. TBin works by gathering the DUT bin information, separated by temperature and provided by the user, and combining it into a single output representative of the final, shippable bin.

# Prerequisites and Dependencies

The successful use of this Test Method depends on the prior execution of one PrimeSortSetBin test instance per Temperature/Flow that needs to be processed for TBin. Each of these SetBin instances will produce the corresponding Bin Input that will be fed to the TBin Test Method.

In order for the Test Method to properly apply any resulting TBin value into TOS, the user must declare all possible bin values in the Bin Definitions File of the Test Program. This includes 1-3 digit HardBins, 3-5 digit SoftBins and 7-9 digit SoftBins.

## TBin Format and Value Ranges

The SortTBin Test Method assumes 8-Digit Binning format is used throughout the Test Program. The main output of the Test Method is the "Total Bin" which is a 9-Digit Bin Number with the following format:

| 1st Digit | 2nd Digit | 3rd Digit | 4th Digit | 5th Digit | 6th Digit | 7th Digit | 8th Digit | 9th Digit |
|---------|---------|---------|---------|---------|---------|---------|---------|---------|
| ITB | ITB | ITB | | | | | | |
| FTB | FTB | FTB | FTB | FTB | | | | |
| DTB | DTB | DTB | DTB | DTB | DTB | DTB | DTB | DTB |

As can be seen the first 3 digits represent the Interface TBin (ITB), the first five digits represent the Functional TBin (FTB). All 9 digits make up the Data TBin (DTB). Each of these segments of the TBin format has the following value ranges:

| Segment Name | Total Range | Bad Die Range |
|---------|---------|---------|
| Interface TBin| 1 - 999 | > 6 |
| Functional TBin| 100 - 99999 | > 699 |
| Data TBin| 1000000 - 999999999 | > 6999999 |

# User Interface Parameters

The table below lists and describes the user interface parameters provided by the SortTBin Test Method.

| **Parameter Name** | **Mandatory?** | **Type** | **Description** |**Default Value** |
|--------------------|----------------|----------|-----------------|-----------------|
| BinInputs | Yes | Comma-Separated String | A list of the Shared Storage Keys which contain the integer-type Bin values from the Flows/Temperatures to process.| N/A |
| ExpectedNumberOfFlows | Yes | Integer | The number of Flows/Temperatures from the BinInputs parameter that are expected to hold a Bin value.| N/A |
| DesignKill | No | Multi-Choice (String) | Can be "On" or "Off". When turned on, the Test Method will force a good die to bin as ITB157 and be killed. Unit does not follow expected design parameters and is considered of little value to continue testing. This parameter has the first priority among Kill Overrides.| "Off" |
| CostKill | No | Multi-Choice (String) | Can be "On" or "Off". When turned on, the Test Method will force a good die to bin as ITB158 and be killed. Unit does not have significant market value and is considered of little value to continue testing. This parameter has the second priority among Kill Overrides.| "Off" |
| QualityKill | No | Multi-Choice (String) | Can be "On" or "Off". When turned on, the Test Method will force a good die to bin as ITB159 and be killed. Unit is at high risk of reliability issues occurring and is considered of little value to continue testing. This parameter has the third priority among Kill Overrides.| "Off" |

# SortTBin Test Method Exit Ports

The SortTBin Test Method supports the following exit ports.

| **Port Number** | **Exit Condition** | **Description** |
|-----------------|--------------------|-----------------|
|**-2**           |***Alarm***         |Any hardware alarm.|
|**-1**           |***Error***         |Any unexpected software error or software execution failure.|
|**0**            |***Fail***          |Verify or Execute software errors due to unexpected values or conditions|
|**1**            |***Pass***          |Passing Bin set by the Test Method.|
|**2**            |***Pass***          |Bad Bin set by the Test Method.|

# Implementation

## Verify

During Verify the Test Method will check that all the following criteria are met in order to pass.
- The BinInputs parameter must not be empty.
- The BinInputs parameter must not contain more than six inputs.
- The ExpectedNumberOfFlows parameter must be greater than 0 but not be greater than the size of the BinInputs parameter.

## Execute

The following figure depicts the overall flow of the Total Binning Algorithm during Execute.

<!--- This editable graphics file contains the relevant flowchart --->
<!--- and it should be edited with VS Code + the Draw.io Extension --->
<!--- since it's not a regular image. --->
<center>

![SortTBin Flow](./.attachments/TBinFlow.drawio.png)

</center>

The blue boxes represent the main sections of the flow: "IBin 98/99 Handling", "Bad Die Binning" and "Good Die Binning".

A couple of decision nodes help direct the flow of execution, for example the flow only goes to "IBin 98/99 Handling" when the IBin of the Current Tester Bin is 98 or 99 AND the user defined Bin Inputs do NOT contain Bin 98/99, otherwise flow continues. Similarly, if the user defined Bin Inputs do not contain the Current Tester Bin then the flow will set TBin to the Current Tester Bin and exit through port 2. The next flow decision happens after the TBin result has been determined. If either a Good Die or Bad Die Bin was set a value check is performed. If the selected DTBin is less than the First Bin Input Value the Test Method aborts further execution and exits through port 0.

### IBin 98/99 Handling

The following image depicts the handling of IBin 98/99. This subflow is only executed if the Current Bin has an IBin equal to 98 or 99.

<!--- This editable graphics file contains the relevant flowchart --->
<!--- and it should be edited with VS Code + the Draw.io Extension --->
<!--- since it's not a regular image. --->
<center>

![SortTBin Flow](./.attachments/TBinAlarmHandlingFlow.drawio.png)

</center>

As the diagram shows, the first action is to find the Failing Temperature Index (WhichFailingTempIndex) from the Bin Inputs given to the Test Method via its respective input parameter. The Failing Temperature will be the one for which there is no data in the Shared Storage or its value is set to zero. If all Bin Inputs exist in the Shared Storage then WhichFailingTempIndex will be 0. If the first Bin Input does not exist WhichFailingTempIndex will be 1 and so on.

If WhichFailingTempIndex is equal to 0 or 1 then the TBin will be set to the current bin. Otherwise the TBin will be set to a bin value where WhichFailingTempIndex is added as a prefix to the current bin. For example if WhichFailingTempIndex = 2 and CurrentBin = 98010001 then TBin will be set to 298010001.

### Bad Die Binning

The below image shows the overall flow for Bad Die Binning.

<!--- This editable graphics file contains the relevant flowchart --->
<!--- and it should be edited with VS Code + the Draw.io Extension --->
<!--- since it's not a regular image. --->
<center>

![SortTBin Flow](./.attachments/TBinBadDieBinningFlow.drawio.png)

</center>

The first step is to search for a Bad Bin within the Bin Input values input parameter. If no Bad Bin is found then the flow exits and continues to Good Die Binning.

If a Bad Bin is found then the index of the first failure will be used to determine which TBin value to set. If the first Bin Input contains a Bad Bin then the "WhichFailingTempIndex" will be 1, if the second Bin Input contains the first failure then the index will be 2, and so on. For the case of WhichFailingTempIndex equal to 1 the TBin will be set to the value of Bin Input 1. For all other cases the TBin will be set to the Bad Bin that was found with "WhichFailingTempIndex" as a prefix.

For all cases of a Bad Die value being set into TBin the Test Method will exit through port 2.

### Good Die Binning

The following diagram represents the overall flow of Good Die Binning.

<!--- This editable graphics file contains the relevant flowchart --->
<!--- and it should be edited with VS Code + the Draw.io Extension --->
<!--- since it's not a regular image. --->
<center>

![SortTBin Flow](./.attachments/TBinGoodDieBinningFlow.drawio.png)

</center>

The Good Die Binning algorithm starts by checking the Kill Override input parameters (DesignKill, CostKill, QualityKill). These parameters allow the user to override an otherwise good die and mark it as a failing part, removing it from the sort flow without impacting Sort RISO numbers. These three parameters are checked one by one in order (Design -> Cost -> Quality) and if any of them is "On" the appropriate TBin value is set and the Test Method exits through port 2. It's important to note that if multiple Kill Override parameters are turned on, the priority specified in the diagram (Design -> Cost -> Quality) is what will determine the final TBin.

Next, the "ExpectedNumberOfFlows" parameter is checked by comparing it to how many bin values exist in the "BinInputs" keys. If the number of values is equal flow continues, otherwise the Test Method exits through port 0 with an error message.

The last step is to compute an encoded Good Die TBin and set it into TOS. To do this, the Test Method gathers all of the IBins from the incoming BinInputs into an array of size 6. If a particular BinInput does not exist a value of zero is stored in its corresponding array index. The Max value of all of these IBin digits will also be used as the ITBin. Look at the following example:

____
BinInput1 = <span style="color:#996600">2</span>120001</br>
BinInput2 => <span style="color:#FF3300">Does Not Exist</span></br>
BinInput3 = <span style="color:#00AAFF">3</span>510001</br>
BinInput4 => <span style="color:#FF3300">Does Not Exist</span></br>
BinInput5 = <span style="color:#FF9900">6</span>840001</br>
BinInput6 = <span style="color:#9966FF">5</span>330001</br>

IncomingIbins = {<span style="color:#996600">2</span>,<span style="color:#FF3300">0</span>,<span style="color:#00AAFF">3</span>,<span style="color:#FF3300">0</span>,<span style="color:#FF9900">6</span>,<span style="color:#9966FF">5</span>}</br>
MaxValue = <span style="color:#FF9900">6</span>
____

Each of the values in the "IncomingIbins" array can be referenced with an index, where the first one is index 0 and the last one is index 5. Having gathered this information, the encoded TBin is computed using the following equations:

````
ITBin = MaxValue

FTBin = (ITBin * 100) + (IncomingIbins[0] * 10) + (IncomingIbins[1])

DTBin = (FTBin * 10000) + (IncomingIbins[2] * 1000) + (IncomingIbins[3] * 100) + (IncomingIbins[4] * 10) + (IncomingIbins[5] )
````

Continuing with the previous example, the encoded TBin would be computed as follows:

ITBin = <span style="color:#FF9900">6</span>

FTBin = (6 * 100) + (<span style="color:#996600">2</span> * 10) + (<span style="color:#FF3300">0</span>) = 620

DTBin = (620 * 10000) + (<span style="color:#00AAFF">3</span> * 1000) + (<span style="color:#FF3300">0</span> * 100) + (<span style="color:#FF9900">6</span> * 10) + (<span style="color:#9966FF">5</span> ) = 6203065

Finally the Test Method sets this TBin into TOS and exits through port 1.

# Ituff Output

The SortTBin Test Method logs final results into Ituff with the following format:

```xml
2_tname_<CurrentInstanceName>_debug
2_strgval_TBIN_<FinalBin>|<BinInput1Key>_<Value1>|<BinInput2Key>_<Value2>|<BinInput3Key>_<Value3>|<BinInput4Key>_<Value4>|<BinInput5Key>_<Value5>|<BinInput6Key>_<Value6>|IncomingTOSBin_<TOSBin>
```

The first line includes the Instance Name with a "debug" postfix, which indicates that the data should only be used for debug purposes.

The next and final line includes the Final TBin value as well as each incoming Bin Input Key and value and the incoming bin that was last set in TOS, where each of these data segments is separated by a pipe character. If a particular Bin Input Key does not exist in Shared Storage a value of "0" will be logged.

An additional "Error" section will appear after the final TBin value if the Test Method exits through port 0 due to an unexpected but identified error. The format is as follows:

```xml
2_tname_<CurrentInstanceName>_debug
2_strgval_TBIN_90949999|Error_<ErrorDescription>|<BinInput1Key>_<Value1>|<BinInput2Key>_<Value2>|<BinInput3Key>_<Value3>|<BinInput4Key>_<Value4>|<BinInput5Key>_<Value5>|<BinInput6Key>_<Value6>|IncomingTOSBin_<TOSBin>
```

Note that TBin will be hardcoded to 90949999 for all of these cases and a brief error description is given in the Error section of this ituff output. The following table details the error scenarios that are currently supported.

| Error Description | Cause |
|---------|---------|
| FlowCountMismatch| The number of existing Bin Input Flows is different than the number of expected flows provided by the "ExpectedNumberOfFlows" input parameter.  |

# Device End Datalog Data Export

In order for the "PrimeDeviceEndDatalogTestMethod" to properly log the final DTBin, FTBin and ITBin values, the SortTBin Test Method will store those values in the appropriate Shared Storage Keys. The following table lists the Shared Storage keys that the DeviceEndDatalog Test Method consumes for each of these values.

| TBin Value | Shared Storage Key |
|---------|---------|
| ITBin| NG_ITBIN |
| FTBin| NG_FTBIN |
| DTBin| NG_DTBIN |

# Acronyms

Definition of acronyms used in this document:

  - **DTB** or **DTBin**: Data Total Bin
  - **FTB** or **FTBin**: Functional Total Bin
  - **ITB** or **ITBin**: Interface Total Bin
  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TBin**: Total Bin