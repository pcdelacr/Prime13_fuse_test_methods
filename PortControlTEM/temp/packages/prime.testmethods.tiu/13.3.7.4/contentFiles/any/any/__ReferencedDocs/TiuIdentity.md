**Prime Test-Method Specification REP**

Nov 2022

[[_TOC_]]

# Introduction

The main purpose of this Test Method is to validate that the TIU currently installed is the correct TIU intended for the Test Program.  
In addition, it can optionally validate that the Metal Frame or the Elastomer Sheet currently installed is the correct one intended for the Test Program.

# Methodology

TiuIdentity Test Method will read the TIU name from the tester and match it against the list of Regex specified in **ValidTiuRegex** parameter. Once the match is found, the test method will load the TIU name to "SCVars.TP_TESTBOARD_ID" uservar.

If a list of Regex is specified in the **ValidMetalFrameRegex** or **ValidElastomerSheetRegex** parameters, 
the TiuIdentity Test Method will read the Metal Frame and/or Elastomer Sheet serial numbers from the tester accordingly and match them against the corresponding list.  
If a parameter is not specified, there will be no checking for the corresponding serial number.  

## Serial Number Configuration
The Metal Frame and Elastomer Sheet serial numbers are read from EEPROM index 0 for HDBI and index 1 for HDMT, as per customer standards and requirements (see ticket [#59864](https://dev.azure.com/mit-us/PRIME/_workitems/edit/59864/)).  

These serial numbers must be stored within the Auxiliary/Supplemental Data section. This section can be modified to register the serial numbers using the BLT Editor available in the SiteC Dashboard. Alternatively, the serial numbers can be configured directly within the `HwConfigFile.xml` file.   

### Example: HDMT BLT Editor
The image below illustrates the selection of EEPROM1 for writing, and the entry of Metal Frame and Elastomer Sheet serial numbers in the Supplemental Data section.

![image.png](../.attachments/TiuIdentity/BLTEditor_ElastomerSocketSerialNumbers.png) <br>

### Example: HDMT HwConfigFile.xml Section
Below is an example of how to structure the relevant section in the `HwConfigFile.xml`. The configuration requires specifying a DUTSocket with its corresponding index, 
along with MetalFrameSN and ElastomerSheetSN keys for the serial numbers, each matching the DUTSocket index to which they belong.  

In this example, the BoardName is set to TIUEEPROM1, which is the board designated to contain the Metal Frame and Elastomer Sheet serial numbers in HDMT.  
```xml
VendorID="16"  VendorName="RDAltanova"  BoardID="13"  BoardName="TIUEEPROM1"  PartNumberAsBuilt="640-0292-REV1"  PartNumberCurrent="640-0292-REV1"  SerialNumber="NVLSL11T0117"  ManufactureDate="041825">
<SupplementalData Name="DUTSocket0"  Value="11"/>
<SupplementalData Name="DUTSocketSerialNumber0"  Value="NVLS11T0117"/>
<SupplementalData Name="MetalFrameSN0"  Value="I01954L1020F0118"/>
<SupplementalData Name="ElastomerSheetSN0"  Value="I01954L1010E0001"/>
<SupplementalData Name="BottomElastomerSN0"  Value="I01954L1020E0001"/>
```

For additional information on Elastomer Socket Configuration, please refer to [this resource](https://wiki.ith.intel.com/display/ITSpdxtp/NVL+XIU+Check+Setup?preview=%2F3676835812%2F4084724839%2F20230106+Elastomer+Socket+Antimixing+TFS+%281%29.pptx). 

### Naming Convention
The "Name" field for serial numbers must adhere to the following convention to ensure uniqueness:
- **Metal Frame**: `MetalFrameSN{DUT index}` (e.g., `MetalFrameSN0` corresponds to `DUTSocket0`)
- **Elastomer Sheet**: `ElastomerSheetSN{DUT index}` (e.g., `ElastomerSheetSN1` corresponds to `DUTSocket1`)

# Test Instance Parameters

| **Parameter Name**       | **Required?**    | **Type**               | **Values**                      |
| ------------------------ | ---------------- | ---------------------- | ------------------------------- |
| ValidTiuRegex            | Yes              | String (Regex pattern) |  Comma separated list of regexes to match with the TIU name.            |
| ValidMetalFrameRegex     | No               | String (Regex pattern) |  Comma separated list of regexes to match with the Metal Frame serial number.            |
| ValidElastomerSheetRegex | No               | String (Regex pattern) |  Comma separated list of regexes to match with the Elastomer Sheet serial number.            |

# Console Log
Once validated, the TiuIdentity Test Method logs the TIU name and the serial numbers to the console when the LogLevel is set to "Enabled".  

The console log will display the following messages:
```txt
{tiuName} is valid.
Metal Frame Serial Number: [{metalFrameSerialNumber}] is valid.
Elastomer Sheet Serial Number: [{elastomerSheetSerialNumber}] is valid.
```

### Debug Prints
Additional debug information is available in the console log when LogLevel is set to "Enabled." 
These debug prints offer deeper insights into the data retrieval and validation process.  

#### Metal Frame Serial Number Debug Prints
```txt
-D- Reading TIU Auxiliary Data from EEPROM [{eepromIndex}] and TIU Index [{tiuIndex}].
-D- Auxiliary Data contents obtained from TIU Index [{tiuIndex}]: [ {Formatted Auxiliary Data} ]
-D- DUT Index: [{dutIndex}] obtained from DUT ID [{dutId}].
-D- Metal Frame Serial Number Key: [{metal frame key used to obtain the serial number value e.g. MetalFrameSN1}].
-D- Value for Metal Frame Serial Number: [{metalFrameSerialNumber}].
```

#### Elastomer Sheet Serial Number Debug Prints
```txt
-D- Reading TIU Auxiliary Data from EEPROM [{eepromIndex}] and TIU Index [{tiuIndex}].
-D- Auxiliary Data contents obtained from TIU Index [{tiuIndex}]: [ {Formatted Auxiliary Data} ]
-D- DUT Index: [{dutIndex}] obtained from DUT ID [{dutId}].
-D- Elastomer Sheet Serial Number Key: [{elastomer sheet key used to obtain the serial number value e.g. ElastomerSheetSN0}].
-D- Value for Elastomer Sheet Serial Number: [{elastomerSheetSerialNumber}].
```

# Custom User Code Hooks

Two user extendable hook is available for user to extend the functionality (e.g additional serial number validation).

```csharp
PreExecuteDecision ITiuIdentityExtension.PreExecuteEvaluation();
bool ITiuIdentityExtension.CustomValidation();
```

## PreExecuteEvaluation()

This method executes right before the Tiu Name is being checked against **ValidTiuRegex**. The method expected a return of `PreExecuteDecision` which the main code will use to decide whether to continue execution of the test method, or to stop execution and exit through port 0 or 1.

The return value:
```csharp
public enum PreExecuteDecision
{
   /// <summary>
   /// Continue the execution.
   /// </summary>
   Continue,

   /// <summary>
   /// Stop the execution and exit port 0.
   /// </summary>
   StopPort0,

   /// <summary>
   /// Stop the execution and exit port 1.
   /// </summary>
   StopPort1,
}
```

Example:
```csharp
PreExecuteDecision ITiuIdentityExtension.PreExecuteEvaluation()
{
   if (TesterIsOffline())
   {
      return PreExecuteDecision.StopPort1;
   }

   if (EepromValidationFailed())
   {
      return PreExecuteDecision.StopPort0;
   }

   return PreExecuteDecision.Continue;
}
```

This method is being used to check if the tester is offline and stop execution through exit port 1 if the tester is found offline as default implementation.

## CustomValidation()

This method is provided to user with the purpose of doing custom validation after TIU name is checked against **ValidTiuRegex** (e.g additional serial number check). It requires a return of boolean value (true or false). A false will cause the test method to fail and exit through port 0.

# TPL Samples

Here are a few test instance examples using the TiuIdentity test method

```javascript
Import PrimeTiuIdentityTestMethod.xml;

Test PrimeTiuIdentityTestMethod TiuNameCheck_P1
{
   ValidTiuRegex = "V[a-zA-Z0-9]*Tiu,D[a-zA-Z0-9]*Tiu";
}

Test PrimeTiuIdentityTestMethod RegexChecks_P1
{
    ValidTiuRegex = "SPHT[0-9]*";
    ValidMetalFrameRegex = "I02049B101AF[0-9]*";
    ValidElastomerSheetRegex = "I02049B101AE[0-9]*";
    LogLevel = "Enabled";
}

```
The `TiuNameCheck_P1` test instance above will match with TIU names:
- "ValidTiu"
- "Valid25Tiu
- "Dummy109Tiu"

# Exit Ports

The TiuIdentity test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                                    |
| ------------- | ------------- | ---------------------------------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                                                                |
| **-1**        | ***Error***   | Any software condition error                                                       |
| **0**         | ***Fail***    | **Failing condition.** TIU name doesn't match any of the regexes specified, OR `PreExecuteEvaluation()` returns `StopPort0`, OR `CustomValidation()` returned a `false`. <br> Metal Frame and/or Elastomer Sheet regexes were specified, but serial numbers don't match.          |
| **1**         | ***Pass***    | **Passing condition.** TIU name match found AND `CustomValidation()` returned a `true`, OR `PreExecuteEvaluation()` returns `StopPort1`. |

# Version tracking

| **Date**     | **Prime release** | **Author**            | **Comments**                                                                                       |
|--------------|-------------------| --------------------- |----------------------------------------------------------------------------------------------------|
| Dec 2nd 2021 | 7.02.00           | Yusof, Adam Malik     | 1st version                                                                                        |
| Nov 24 2022  | 12.00.00          | Yusof, Adam Malik     | Introduction of new extension method `PreExecuteEvaluation()`                                      |
| July 11 2024 | 13.01.00          | Pinto Rosales, Raquel | Adding optional checking for Metal Frame and Elastomer Sheet serial numbers. <br> #44556           |
| May 13 2025  | 13.03.00          | Pinto Rosales, Raquel | Changing EEPROM index to 1 for HDMT for reading Elastomer Sheet and Metal Frame serial numbers. <br> #59864 |

# Acronyms

Definition of acronyms used in this document:

  - **DUT**: Device Under Testing
  - **TIU**: Test Interface Unit
  - **HDMT**: High Density Modular Tester
  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **TPL**: Test Programming Language
  - **BLT Editor**: The BLT (Boot Loader Table) Editor is a feature within the hdmtOS environment accessible through the SiteC Dashboard.