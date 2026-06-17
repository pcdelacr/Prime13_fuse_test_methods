# REP for Prime's InitializeServices TestMethod

This **REP** is intended to describe the InitializeServices Prime TestMethod.

[[_TOC_]]

# Methodology

The InitializeServices test method provides several capabilities:

1)  Initialize Prime Services (every service will detail its specific initialization sequence)
2)  Validate Service's configuration JSON files against the Schema for structural checks.
3)  Validate against TorchRulesVar.AlephEnvVariable to support dynamic env var name for aleph files per bom group

InitializeServices has by default enabled aleph parsing optimization for all services. This optimization keeps tracks of already parsed files and skips subsequent calls to aleph for the given service if the files are exactly the same as before.

Methodology works as follows:

- First time all services are initialized with their files. The **filenames** and **timestamps** are stored.
- Second aleph call will compare found files, if they are the same and havent been edited the aleph will be skipped.
- If new or less files are found for a given service it will be initialized with **all** files found.
- If a service fails initializing (parsing error) future calls will keep retrying.

# Test Instance Parameters

The table below lists and describes the test instance parameters supported by the InitializeServices test method

| **Parameter Name**        | **Required?** | **Type**                         | **Values**           | **Comments**                               |
| ------------------------- | ------------- | -------------------------------- | -------------------- | ------------------------------------------ |
| ForceAleph   | No            | Choices                          | True, False(Default) | When True, file tracking and skipping optimization is disabled. |
| ForceValidateAlephFiles   | No            | Choices                          | True, False(Default) | When True, all JSON configuration files are validated against the Schema, even in Production runs. |

**Notes:**
If the `MemoryAndTimeProfiling` parameter is different than disabled, the InitializeServices test method will profile the services' verify in a comma-separated table so it can easely be post-processed:

All services' verify's example:
```
Services' Verify summary.
===========================================================================
Name,ElapsedTime(ms),PrivateBytesBefore(B),PrivateBytesAfter(B),PrivateBytesDelta(B)
FuseDBService,10,678408192,678408192,0
DefeatureTrackingService,19,678412288,678920192,507904
BinMatrixService,11,678952960,679018496,65536
DatalogService,10,681218048,681291776,73728
DffService,11,681365504,681336832,-28672
PatConfigService,11,681664512,681443328,-221184
PinService,10,681574400,681783296,208896
TestConditionService,10,681930752,681713664,-217088
TpSettingsService,11,681766912,681852928,86016
FuseReadService,10,681955328,682024960,69632
GfxService,10,682057728,682020864,-36864
PinMonitorService,10,682065920,682098688,32768
TestProgramService,20,682106880,682110976,4096
VoltageService,10,682119168,682123264,4096
DieIdBinningService,11,682123264,682135552,12288
DtsProcessingService,23,659607552,659927040,319488
PacketService,11,659927040,659931136,4096
ResistanceOffsetService,9,659931136,659935232,4096
ScanService,9,659935232,659935232,0
SimbaService,10,659935232,659939328,4096
ThermalControlSetService,9,659939328,659939328,0
VminForwardingService,9,659939328,659939328,0
===========================================================================

Service's Verify total execution time: 1209ms.
Service's Verify total bytes at beginning: 678326272B = 646MB.
Service's Verify total bytes at the end: 659968000B = 629MB.
Service's Verify total bytes difference: -18358272B = -17MB.
```

# Datalog output

This Test Method does not create a datalog

# Custom User Code Hooks

**NA**

# TPL Samples

Here are a few test instance examples using the InitializeServices test method

**TPL Sample1**

```python
Import PrimeInitializeServicesTestMethod.xml;

Test PrimeInitializeServicesTestMethod PrimeInitializeServices
{
   ForceValidateAlephFiles = "False";
}
```

**UserVars Sample**

```
UserVars TorchRulesVars
{
	String locationCode = "";
	String bom= "";
	String AlephEnvVariableName = ~HDMT_TPL_DIR/MODULES/PATTERN_MODIFY/PATTERN_MODIFY/inputFiles/patConfigTestMethod.json;
              ~HDMT_TPL_DIR/MODULES/PATTERN_MODIFY/PATTERN_MODIFY/inputFiles/PatMod.PatConfigSetPoints.json;
              ~HDMT_TPL_DIR/MODULES/PATTERN_MODIFY/PATTERN_MODIFY/inputFiles/PatModWithPlist.PatConfigSetPoints.json;
              ~HDMT_TPL_DIR/MODULES/FUSEDB/FUSEDB/inputFiles/test.fuseDataLabel.json;
              ~HDMT_TPL_DIR/MODULES/FUSEDB/FUSEDB/inputFiles/test.fuseDef.json;
              ~HDMT_TPL_DIR/MODULES/PATTERN_MODIFY/PATTERN_MODIFY/inputFiles/test.patmod.json;
              ~HDMT_TPL_DIR/MODULES/PATTERN_MODIFY/PATTERN_MODIFY/inputFiles/test.fuseDataLabel.json;
              ~HDMT_TPL_DIR/MODULES/PATTERN_MODIFY/PATTERN_MODIFY/inputFiles/test.fuseDef.json;
              ~HDMT_TPL_DIR/MODULES/PATTERN_MODIFY/PATTERN_MODIFY/inputFiles/test.fuseconfig.json";
}
```
# Test Program Flow order requirements

The image below ilustrates the order requiremetns for the Evergreen and Prime instances to correctly <font size="3"><span style="color:OrangeRed">Initialize</font> the libraries.

1. iCInitTest (Evergreen Instance)
1. PrimeInitializeLibraryTestMethod
1. PrimeInitializeServicesTestMethod (Aleph)
1. PrimeInitializeInstancesTestMethod


::: mermaid
graph LR
  subgraph Init Subflow
  direction LR
  id1(iCInitTest):::EVG --> id2(PrimeInitializeLibraryTestMethod):::PRIME
  id2 --> id3(PrimeInitializeServicesTestMethod):::PRIME
  id3 --> id4(PrimeInitializeInstancesTestMethod):::PRIME
  end
classDef EVG fill:#708541;
classDef PRIME fill:#005B85;
:::

# Exit Ports

The InitializeServices test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |
| **N**         | ***Pass/Fail*** | Failing condition            |

# Additional Dependencies

More dependencies to consider for this TestMethod to well operate:

  - **ALEPH\_FILES** – An ENV variable that needs to be defined in the ENV file

      - This [ENV variable](https://dev.azure.com/mit-us/PrimeWiki/_wiki/wikis/PrimeWiki.wiki/28879/Special-ENV-Variables-in-Prime) stores the list (semicolon-separated) of all the input files required for Prime Services initializations (every Service will detail the relevant files required for its
        initialization)
      - Every file should exist in the TP; otherwise, INIT will fail
        - **Example**: ALEPH\_FILES = "./testfile.patmod.json";

  - **AlephEnvVariableName** - An Optional UserVar at TorchPrimeVars group to support dynamic env name for aleph files per bom group, which specified in environment file .
    - If the uservar group **TorchPrimeVars** , or uservar **AlephEnvVariableName**  does not exists or the value is empty, use **ALEPH_FILES**.
	- Console printing to indicate which aleph files path is implemented from (initial configuration - ALEPH_FILES or TorchPrimeVars - AlephEnvVariableName).
		- **Example**:
		UserVars TorchPrimeVars
		{
			String AlephEnvVariableName = "ALEPH_FILES_UCCAP"; # ALEPH_FILES_UCCAP is specified in environment file
			# ( or FlowMatrixRule.BomGroupRule(“ALEPH_BG1” , ALEPH_BG2”); for multi bom groups programs )
		}

		- **Console printing**: **TorchPrimeVars.AlephEnvVariableName** = "AlephFile env variable name = [alephVarName]."
		- **Console printing**: **ALEPH_FILES** = "AlephFile env variable name = [ALEPH_FILES]."

  - **PRIME_SCHEMA_PATH** - An ENV variable with the schema files folder path. This folder contains XML and JSON schema files.
  - **PrintServiceFileToItuff** - An UserVar at the main collection containing a comma-separated list of the services' names to store the configuration files relative path (same as defined at the ENV file) at SharedStorage, for further ituff printing at LotStart with EVG (via Gateway Service) or Prime LotStart (not implemented).

     - Example:
       - Input => **PrintServiceFileToItuff** = "VminForwardingService, DefeatureTrackingService"
       - Output => **INIT_ServicesToPrintFilesToItuff** string value at LOT context: "VminForwardingService^.\Modules\VminSearch\InputFiles\VminForwardingConfiguration.json|DefeatureTrackingService^.\Modules\VminSearch\InputFiles\configuration.defeaturetracking.json"

     - Constraints:
       - Token separators:
          - Service: **|**
          - Service name and files: **^**
          - Multiple Files: **;**
       - If the service has more than one file, the files will be separated by a semicolon ( ; ).
       - The file path will be printed exactly as defined at the ENV file. The idea is to have these files always defined relative to the stpl file (as usual), but if the user defines these files with an absolute path, the same absolute path will be printed.


# Version tracking


| **Date**       | **Version** | **Author**     | **Comments**    |
| -------------- | ----------- | -------------- | --------------- |
| Apr 5th, 2020  | 1.0.0       | Zamir Zuriel   | Initial version |
| Aug 31st, 2021 | 1.1.0       | Javier Alpizar | Add service files path saving and JSON Schema Validation features |
| Feb 10th, 2022 | 8.1.0       | Lim, Xin Yan   | Enable TorchRulesVar.AlephEnvVariable to support dynamic env var name for aleph files |
| Apr 22nd, 2022 | 9.1.0       | Lim, Xin Yan   | Enable TorchPrimeVar.AlephEnvVariable to support dynamic env var name for aleph files |
| Apr 22nd, 2022 | 9.1.0       | Lim, Xin Yan   | Fix bug for supporting dynamic env var name for aleph files |
| Jul 11th, 2022 | 11.0.0      | lchavarr       | Split Init's test methods |
| Aug 12, 2022 | 11.0.1 | hramirez | Adding Flow order documentation |

# Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System