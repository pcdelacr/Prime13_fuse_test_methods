<h1>Prime Test-Method Specification REP</h1>

<DIV style="padding: 15px 35px 15px 15px;margin-bottom: 20px;border: 1px solid rgb(235, 204, 209);color: rgb(169, 68, 66);background-color: rgb(242,222,222);font-family: Verdana, sans-serif;font-size: 15px;font-style: normal;font-weight: 400;letter-spacing: normal;text-align: start;text-indent: 0px;text-transform: none;white-space: normal;word-spacing: 0px">
  <STRONG style="font-weight: bolder">/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////</STRONG><SPAN> </SPAN><br>
  <font color="red">**This is engineering version - Development of this test method is WIP - It must not be used in production.**</font><br>
  <STRONG style="font-weight: bolder">/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////</STRONG><SPAN> </SPAN><br>
</DIV>

[[_TOC_]]

# Stakeholder
| Area | Contact |
| ---- | ------- |
| Methodology | [Krishnamoorthy, Arun](mailto:arun.krishnamoorthy@intel.com) <br> [Shah, Rushit](mailto:rushit.shah@intel.com) |
| TestMethod Architect | [Murillo, Freddy](mailto:freddy.murillo@intel.com) |  

# Methodology
The PrimeDevicePowerScalerTestMethod is designed to validate and apply power scaling factors (PSF) to device test instances based on a user-supplied configuration file. It ensures that the configuration is valid, calculates the required PSF values, and registers the results for use in subsequent test flows.

# Verfiy
- **Parameter Validation**: Ensures `ConfigurationFile` is provided and not empty.
- **Schema Validation**: Uses `JsonSchemaValidator` to check the configuration file structure and content.
- **Configuration Parsing**: Loads and deserializes the configuration into a `ConfigurationHandle`.
- **Pin Name Validation**: Confirms all pin names in the configuration exist in the test environment.
- **Test Instance Mapping**: Uses regex to map test instances to pins as specified in the configuration.
- **Duplicate Check**: Ensures no test instance is mapped to multiple pins or appears more than once.

# Execute
- **PSF Calculation**:
  - Calculates the base PSF value from the configuration PSF equation.
  - Computes the PSF to apply for each test instance, `PSFToApply = PSFBase * PSFMultiplier`.
  - Validates that all calculated PSF values are within allowed ranges.
- **Test Application Construction**: Builds a list of TestToApply objects, each representing a test instance, pin, and its PSF.
- **Callback Registration**: Registers the constructed test application objects with the DevicePowerScalerCallBack for use in the test session.
- **Result**: Returns 1 (Pass) if all steps succeed, otherwise throws a TestMethodException (handled by the framework as a fail).

## DevicePowerScalerCallBack
This is an internal component of the Device Power Scaler test method. It is responsible for dynamically applying and restoring Power Scaling Factor (PSF) values to device pin during test execution, based on the configuration and test instance context.
For each test instance, the callback:
- Applies the calculated PSF and Datalogs the PSF value before the test runs.
- Restores the PSF to the default value (1.0) after the test completes.
You do not need to instantiate or call this class directly. It is managed by the PrimeDevicePowerScalerTestMethod, which constructs and registers the callback as part of its execution flow.

**How It Works**
- Registration:
The callback is registered with the Prime framework for both pre-execute and post-execute events using the RegisterCallBack method. This ensures the callback is invoked automatically before and after each test instance execution.
- Pre-Execute Event:
Before a test instance runs, the callback:
  1. Identifies the current test instance.
  2. Looks up the corresponding TestToApply object (which contains the pin name and PSF value).
  3. Applies the specified PSF to the pin using the Prime Thermal Service.
  4. Logs the applied PSF value to the debug console and datalogs it using the iTUFF format.

- Post-Execute Event:
After the test instance completes, the callback:
  1. Restores the pin’s PSF to the default value (1.0).
  2. Logs the restoration action to the debug console.

# Exit Port
| Port | Description |
| ---- | ----------- |
| 1    | Indicates that the DevicePowerScaler test method executed successfully without errors. |
| 0    | Indicates that the DevicePowerScaler test method encountered an error during execution. |

# Test Instance Parameters
| **Parameter Name**             | **Required?** | **Type**        | **Values**                                         | **Comments**                      |
| ------------------------------ | ------------- | --------------- | -------------------------------------------------- | --------------------------------- |
| ConfigurationFile              | Yes           | File            | Path to configuration file.                        | Able to use with "~HDMT_TPL_DIR". |

## Test Instance setup
```txt
Test PrimeDevicePowerScalerTestMethod MyTest_P1
{
    ConfigurationFile = "~HDMT_TPL_DIR/TestPrograms/DevicePowerScaler/Modules/DevicePowerScaler/InputFiles/DPS_ConfigurationFile_Demo.json";
    LogLevel = "Enabled";
}
```

## Datalog
Test instance of this testmethod will print the calculated PSFBase.
```txt
2_tname_DevicePowerScaler::Execute_P1_DPS_PSFBase
2_mrslt_0.6000
```

Before target test instance Execute, testmethod callback will set the specified Pin with the intended PSF value and print to ituff.
```
2_tname_IPA::DevicePowerScalerIP::PrimeFuncCaptureCtvFuncTestDtsProcessing_P1_DPS_PSF
2_mrslt_1.8000
```

# Configuration File Json Attribute explanation
## Download Sample File
<font color="green">**Download** </font>[DevicePowerScaler Configuration explanation](./.attachments/DPS_Json_explanation.pptx) <br>
<font color="green">**Download** </font>[DevicePowerScaler Configuration File](./.attachments/DPS_ConfigurationFile_Demo.json) <br>
- All properties are required unless marked otherwise.
- No additional properties are allowed at any level.
- The Regex field in each test is used to match test instance names dynamically.
- The PSFMultiplier must be a number between 0 and 10, inclusive.
- The HighLimit and LowLimit in the PSF object define the valid range for calculated PSF values.

# Version Tracking
| Prime Version | Prime ticket reference | Author         | Comments |
| ------------- | ---------------------- | -------------- | -------- |
| 13.03.00      | #59742                 | Teoh, Khai Jie | Initial version. |