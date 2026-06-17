# PrimeForkTestMethod User Guide
This doc is intended to help you get started with using the PrimeForkTestMethod, or otherwise known as ***Fork***.

In this document, you will find the below sections:

  - **Use Cases** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table that describes each instance parameter (Name, Required?, Type, Values)

  - **Modes** – Description of the different modes for ***Fork***.

  - **Required User Variables** - User variables required for ***Fork*** to function in "Config mode".

  - **Configuration file** - Defines the .json file used for ***Fork*** to function in "Config mode".

  - **TPL example** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document 

## Use Cases
***Fork*** is a simple test method that determines its exit port based on the location code of the running test program.

PDEs can use ***Fork*** to create flows that are dedicated for specific location codes.

## Parameters
The table below lists and describes the test instance parameters used by the ***Fork*** test method.

| **Name**              | **Required?**    | **Type**        | **Values**                                                       |
| --------------------- | ---------------- | --------------- | -----------------------------------------------------------------|
| Mode                  | Yes              | String          | "Parameter", "Config" |
| LocationCodeToMatch   | Parameter mode: **Yes**, Config mode: **No** | String | Any string representing a location code or RegEx matching to a location code |
| IsRegex         | No                 | Bool  | Indicates whether "LocationCodeToMatch" is a RegEx or not. |

## Modes
The table below describes each possible parameter for "Mode"

| **Mode**    | **Function**                                                                                     |
|-------------|--------------------------------------------------------------------------------------------------|
| Parameter   | Fork exits out of port 1 if the "LocationCodeToMatch" parameter matches the current location mode of the test program.|
| Config      | Fork exits out of a port defined in a user provided configuration that is mapped to the current location code. This mode determines the setup of all ***Fork*** instances across your test program. **NOTE**: Please check the User Variables to learn how to define where your configuration file is located.|

## User Variables
| **UserVarName** | **Collection** | **Required** | **Type** | **Value** |
|-----------------|----------------|--------------|----------|-----------|
| PRIMEFORKTESTMETHOD_CONFIG | UserVars | Parameter mode: **No**, Config mode: **Yes**| String | Path to the ***Fork*** .json configuration file. Ignored for "Parameter mode". |
| SC_LOCN | SCVars | Yes | String | A string representing the current location code of the test program. |

Here's an example of defining the above user variable within your .USVR file:
```
UserVars
{
  String PRIMEFORKTESTMETHOD_CONFIG = "~HDMT_TPL_DIR/TestPrograms/Fork/Modules/Fork/ForkConfig.json";
}
UserVars SCVars
{
   String SC_LOCN = "7721";
}
```

## Configuration file
***Fork***'s configuration file is a simple .json file that maps a string representing a location code to its designated port. This file is required if ***Fork*** is set to "Config mode".
```json
{
    "LocationCodeToPort": {
        "1000" : 1,
        "24823": 2,
        "9999" : 3,
        "42"   : 4,
        "01010": 99
    }
}
```

## TPL example
Here's an example of defining ***Fork*** tests in your test program.
```

# This test will exit out of port 1 if the current location code matches to "3642"
# Exits out of port 0 otherwise.
Test PrimeForkTestMethod ParameterMode_DirectMatch
{
   LogLevel = "Enabled";
   Mode = "Parameter";
   LocationCodeToMatchTo = "3642";
}

# This test will exit out of port 1 if it matches to the defined RegEx.
# Exits out of port 0 otherwise.
Test PrimeForkTestMethod ParameterMode_UsingRegex
{
   LogLevel = "Enabled";
   Mode = "Parameter";
   LocationCodeToMatchTo = ".*[1234][64]45$";
   IsRegex = "true"
}

# This instance will fetch the defined configuration file, and attempt
# to find the mapped port for the current location code.
# Exits out of port 0 of the current location code is not found in the configutation. 
Test PrimeForkTestMethod ConfigMode
{
   LogLevel = "Enabled";
   Mode = "Config";
}
```

## Exit Ports
The ***Fork*** test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Not matched to parameter (Parameter mode) or Location code not found in config (Config mode) |
| **1**         | ***Pass***      | Matched to parameter (Parameter mode) or defined for current location code (Config mode)     |
| **2-99**      | ***Pass***      | User defined port (Config mode)|

## Version tracking
| **Date**         | **Prime Version** | **Author** | **Comments**                        |
| ---------------- | -----------  | -------------- | ----------------------------------- |
| March 23th, 2023 | 12.0.0       | Caio Fernandes | Initial Version.                    |

## Acronyms

Definition of acronyms used in this document:
  - **TPL**: Test Programming Language
  - **.USVR**: User Variable file format