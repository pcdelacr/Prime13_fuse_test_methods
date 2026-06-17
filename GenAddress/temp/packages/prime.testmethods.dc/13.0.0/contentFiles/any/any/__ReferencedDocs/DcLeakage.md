
[[_TOC_]]

# Prerequisites

<p style="font-size: 20px;"><span style="color: #cf6679;">This Service is using Aleph initialization. Please refer to Aleph documentation</span></p>

[Link to Aleph Documentation](/Prime-Customers-Wiki/General/Special-ENV-Variables-in-Prime)

# Description

This test class provides a framework to measure current leakage on high-impedance I/O pins to VCC, VSS and adjacent pins. This test class does not support testing DPS pins.
DcLeakage allows executing a leakage low (VCC) and/or leakage high (VSS) test for each specified pin/pingroup.
Each leakage test sequentially does the following:

1. Places the DUT into a tri-state or high-impedance mode by executing a pre-conditioning pattern
2. Forces a voltage on the specified pin/pingroup using the PMU by level Test Condition.
3. Measures the actual current flow
4. Checks the measured current against the user-supplied upper and lower limits
5. Records the results into ITUFF and console.

# Configuration File

Example file:
```json
{
  "ConfigurationSets": [
    {
      "ConfigurationName": "MyDcLkgConfiguration",
      "Settings": [
        {
          "LimitsSettings": {
            "VssLimitHigh": 1.0,
            "VssLimitLow": 1.0,
            "VccLimitHigh": 1.0,
            "VccLimitLow": 1.0
          },
          "LeakageElements": [
            "pinA",
            "pinB",
            "pinC"
          ]
        }
      ]
    }
  ]
}

```

## Field details
- **ConfigurationName**:  a unique name for leakage settings.
- **Settings** : Vss and Vcc high and low limits. 
- **LeakageElements** : LIst of measured Pins or groups, which should be included in the LevelsTC.


## Test Parameters:
|ParameterName  |Details|
|--|--|
|TimingsTc  | Timing for Pre Conditions Plists execution  |
|LevelsTc| Levels for Pre Conditions Plists execution|
|TestType|Enum, Leakage Test execution mode , [Both, Vss , Vcc]|
|LkgHiPlist| Pre condition plist for LkgHi Test|
|LkgHiLevel| Level TC contains the measurements requests for lkgHi|
|LkgLoPlist| Pre condition plist for lkgLo Test|
|LkgLoLevel| Level TC contains the measurements requests for lkgLo|
|ConfigurationFile|Path for dc lkg configuration .json|
|Configuration|Configuration name from the listed file|

## Test Sequence:

Dc Leakage test method not supporting anymore the conversion of .json or any other input to measurement requests using the SetPinAttribute.
In order to perform LKG measurement everything should be defined by dedicated levels Test condition.
Using direct measurement by LevelsTc are much more efficient and allow users edit the levels on tester while understanding the sequence and measurement requests.

Each Level TC should contains corresponded settings in the configuration file. 

Examples:
```csharp
Single Group Or Pin Measuremnet:

Current measurement of the pinGroup HPCC_DPIN_Dig_slcA_All.
Forcing a Voltage of 0.02V.
All the pins in the group will be measured in PARALLEL.

LevelsTestCondition ForceHiPinOrGroupExample_lvl
{
    HPCC_DPIN_Dig_slcA_All
    {
        StartMeasurement = True ;
        OPMode = "VSIM" ;
        PinModeSel = "PMU" ;
        SamplingCount = 1 ;
        SamplingRatio = 1 ;
        SamplingMode = "Average" ;
        VForce = 0.02V; 
        IRange = "IR40mA";
        IClampHi = 0.02A ;
        IClampLo = -0.02A ;
    }
}

The pinGroup DomainA_DPIN_PMU will be measured in parallel to the pin xxHPCC_DPIN_PMU_slcB_2Kohm since no SquenceBreak staement set between the blocks.

LevelsTestCondition  ParallelMeasurementExmaple_lvl
{
    DomainA_DPIN_PMU
    {
        StartMeasurement = True;
        OPMode = "VSIM" ;
        PinModeSel = "PMU" ;
        SamplingCount =  1 ;
        SamplingRatio = 1 ;
        SamplingMode = "Average" ;
        VForce = 0.02V; 
        IRange = "IR40mA";
        IClampHi = 0.02A ;
        IClampLo = -0.02A ;
    }
    xxHPCC_DPIN_PMU_slcB_2Kohm
    {
        StartMeasurement = True;
        OPMode = "VSIM" ;
        PinModeSel = "PMU" ;
        SamplingCount =  1 ;
        SamplingRatio = 1 ;
        SamplingMode = "Average" ;
        VForce = 0.02V; 
        IRange = "IR40mA";
        IClampHi = 0.02A ;
        IClampLo = -0.02A ;
    }
}

The pin xxHPCC_DPIN_PMU_slcB_2Kohm will be measured after DomainA_DPIN_PMU measurement done.

LevelsTestCondition  ParallelMeasurementExmaple_lvl
{
    DomainA_DPIN_PMU
    {
        StartMeasurement = True;
        OPMode = "VSIM" ;
        PinModeSel = "PMU" ;
        SamplingCount =  1 ;
        SamplingRatio = 1 ;
        SamplingMode = "Average" ;
        VForce = 0.02V; 
        IRange = "IR40mA";
        IClampHi = 0.02A ;
        IClampLo = -0.02A ;
    }
    SeaquenceBreak 0ms;
    xxHPCC_DPIN_PMU_slcB_2Kohm
    {
        StartMeasurement = True;
        OPMode = "VSIM" ;
        PinModeSel = "PMU" ;
        SamplingCount =  1 ;
        SamplingRatio = 1 ;
        SamplingMode = "Average" ;
        VForce = 0.02V; 
        IRange = "IR40mA";
        IClampHi = 0.02A ;
        IClampLo = -0.02A ;
    }
}
```
**Verify:**

1. Validate and Process test inputs.
2. Build LkgTest Object according to the given inputs.

LkgLevels and LkgPlists parameters are required!
Test Mode [Both] - Both LkgHi and LkgLo levelsTc and Plists should be supplied. 
Test Mode [VSS] -  LkgLo levelsTc and Plists should be supplied. 
Test Mode [VSS] - Both LkgHi levelsTc and Plists should be supplied. 

**Execute:**

1. Execute some setups before any measurement execution
2. Execute the required and available Lkg Tests.
3. Results Processing.
4. Port set.

## Test Method Extensions:

```csharp
// Callded at the beggin of the Execute()
// Every global setups for LKG measuremnts can be specified here.
void PreExecuteSetups();

/// Get a FunctionalTest to be executed before measring leakage.
/// The default implementation will return a CaptureFailureTest.
/// In addition, in cases where the test Mode is Both and the given Plist is the same for both
/// only one Functional Object will be running for optimizing the execution.
ICaptureFailureTest GetLeakagePreConditionTest(DcLeakageType dcLeakageType);

/// Generate a DcTest for Lkg measring based on the given inputs. 
/// The default implementation creats a DcTest with DatalogLevel = DcDatalogLevel.COMPRESS.
IDcTest GetLeakageDcTest(DcLeakageType dcLeakageType);

/// Called as a part of Execute.
/// Default implementation will print the data to console in table foramt in case of Debug mode.
/// In addition, data will be printed to ituff using datalog dc formater.
void CustomPostProcessResults(IDcResult lkglowResults, IDcResult lkgHiResults);

/// Called as a part of dataloging stage, that string will be printed to console in Debug mode.
string GetDebugPrint(IDcResult evaluatedResults);

///  Retrieve the current test method exit port.
int GetExitPort();
```

## Test Method Components:

```mermaid
classDiagram
	class DcLeakageTestMethod{
	}
	class LeakageTest{
	}
	class ConfigurationFileProcessor{
	}
	
	DcLeakageTestMethod --* LeakageTest
	DcLeakageTestMethod --* ConfigurationFileProcessor
	
```

