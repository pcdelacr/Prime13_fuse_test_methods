## Prime's Out-of-the-box Functional Plugins

Revision 1.0.0

Oct 2024

[[_TOC_]]

This **REP** is intended to describe the Prime's out-of-the-box Plugins to extend functional tests.

Please note that this document will mention some services dependencies but it's not intended to explain in deep those
services.

Every plugin must inherit from `Prime.Base.ITypes.IPlugin` interface for Prime to recognize it as a plugin.

For a general overview of the plugin system, see the [Plugins documentation](https://dev.azure.com/mit-us/PrimeWiki/_wiki/wikis/PrimeWiki.wiki/103884/Plugins).

# `IDynamicMaskingPlugin` plugin interface
## Overview

The `IDynamicMaskingPlugin` interface defines a contract for plugins that returns list of pins to mask dynamically 
during the test instance `Execute` before the `IFunctionalTest.Execute`.

## Methods

### `Initialize(string userInput)`

Initializes the plugin with the specified `userInput`.

### `GetDynamicPinMask()`

Gets the list of dynamically stored pins to mask.

- **Returns:**
  - `List<string>`: Returns a list of pins to mask.

## Usage

Implement this interface to create a plugin that can handle the list of pins to mask dynamically during the 
test instance `Execute`. The plugin must provide concrete implementations for the `Initialize` and 
`GetDynamicPinMask` 
methods.

## Example

```csharp
public class DynamicMaskingProcessor : IDynamicMaskingPlugin
{
    public void Initialize(string userInput)
    {
        // Initialization logic here
    }

    public List<string> GetDynamicPinMask()
    {
        // Logic to get dynamic pin mask here
        return new List<string> { "Pin1", "Pin2" };
    }
}
```

## Implementations

### `SharedStorageToList` plugin
- Can be used to populate mask pins dynamically using the Shared Storage.
- The `userInput` parameter value for this plugin should be the shared storage a *
  *key**, which stores the comma separated pins for masking.
- Shared storage variable could have empty data, one pin, or multiple comma separated pins.

# `IFunctionalTestPlugin` plugin interface
## Overview

The `IFunctionalTestPlugin` interface defines a contract for plugins that modify the execution of a functional test by extending the pre and post execution phases.

## Methods

### `Initialize(string userInput)`

Initializes the plugin with the specified `userInput`.

### `PreExecuteFunctionalTest(IFunctionalTest functionalTest)`

Invoked before the execution of the functional test.

- **Parameters:**
  - `functionalTest`: An instance of `IFunctionalTest` representing the functional test to be executed.

### `PostExecuteFunctionalTest(IFunctionalTest functionalTest)`

Invoked after the execution of the functional test.

- **Parameters:**
  - `functionalTest`: An instance of `IFunctionalTest` representing the functional test that was executed.
- **Returns:**
  - `bool`: Returns `true` if the post-execution processing is successful, otherwise `false`.

## Usage

Implement this interface to create a plugin that can handle pre and post execution phases of a functional test. The plugin must provide concrete implementations for the `Initialize`, `PreExecuteFunctionalTest`, and `PostExecuteFunctionalTest` methods.

## Example

```csharp
public class FunctionalTestProcessor : IFunctionalTestPlugin
{
    public void Initialize(string userInput)
    {
        // Initialization logic here
    }

    public void PreExecuteFunctionalTest(IFunctionalTest functionalTest)
    {
        // Pre-execution logic here
    }

    public bool PostExecuteFunctionalTest(IFunctionalTest functionalTest)
    {
        // Post-execution logic here
        return true;
    }
}
```

## Implementations

### `FrequencyMeasurePlugin` plugin

- The EdgeCounter plugin is designed to work with reserved action tokens in a pattern. Users must insert EDGECOUNTERON
  and EDGECOUNTEROFF into the pattern to mark the start and stop points for edge count collection. The output pin within
  the edge counter window should be set to X in the pattern.
- Multiple EDGECOUNTERON/EDGECOUNTEROFF sequences are allowed within a pattern. The count will continue from the
  previous sequence, providing a single total output at the end of the pattern burst.
  Below is an example of the pattern configuration for enabling the edge counter:

<pre>
<code>
#File name: frequencyMeasure.pat
Version 1.0;
MainPattern
{
	Pxr "functional.pxr";
	Vectors
	{
		Domain DomainA_All_DPIN_Dig
		{
			{ IO { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=ZZZZZZZZZZ; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=XXXXXXXXXX;} }
Start:
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=LLLLLLLLLL;} }
RPT 1024
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=LLLLLLLLLL;} }
StartCtvArea:
RPT 20
CTV	MTV		{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=1010101010; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=LLLLLLLLLL;} }
RPT 10240
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=HHHHHHHHHH;} }
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=LLLLLLLLLL;} }
<span style="color:YellowGreen">EDGECOUNTERON</span>
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=<span style="color:YellowGreen">XXXXXXXXXX</span>;} }
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=<span style="color:YellowGreen">XXXXXXXXXX</span>;} }
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=<span style="color:YellowGreen">XXXXXXXXXX</span>;} }
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=<span style="color:YellowGreen">XXXXXXXXXX</span>;} }
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=<span style="color:YellowGreen">XXXXXXXXXX</span>;} }
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=<span style="color:YellowGreen">XXXXXXXXXX</span>;} }
<span style="color:YellowGreen">EDGECOUNTEROFF</span>
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=LLLLLLLLLL;} }
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=LLLLLLLLLL;} }
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=LLLLLLLLLL;} }
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=LLLLLLLLLL;} }
			{ V { HPCC_DPIN_Dig_slcA_by8Aand2B_grp=0000000000; HPCC_DPIN_Dig_slcA_by8AAand2BB_grp=LLLLLLLLLL;} }
			RET
		}
	}
}
</code>
</pre>

The **FrequencyMeasure** plugin is an implementation of `IFunctionalTestPlugin` used to measure frequency using the edge
counter. The frequency is calculated using the following equation:
$$
\text{Frequency} = \frac{\text{EdgeCount}}{i \times \text{CycleNumber} \times \text{TesterPeriod} \times 10^6}
$$

where:

- EdgeCount: The total number of edges counted during the measurement period.
- CycleNumber: The number of cycles over which the edges are counted.
- TesterPeriod: The period of the tester in seconds.
- i: A factor that depends on the type of edge counting:
  - ( i = 2 ) if both rising and falling edges are counted (BothEdges).
  - ( i = 1 ) if only one type of edge (either rising or falling) is counted.

This equation converts the edge count into a frequency value by considering the number of cycles, the tester period, and
the type of edge counting. The multiplication by ( 10^6 ) is used to convert the frequency into megahertz (MHz) for
easier interpretation.

#### MTPL FrequencyMeasure plugin example

The following example demonstrates how to configure a test method to use the **FrequencyMeasure** plugin in an MTPL.
This setup measures the frequency using the specified pattern list, timings, and levels.

```python
CSharpTest PrimeFunctionalTestMethod Test_45391_PrimeFunctionalTestMethod_FrequencyMeasurePluginBothEdgesMode_P1
{
    Patlist = "FrequencyMeasure_plist";                                                                                                         // Specifies the pattern list to be used for the test.
    TimingsTc = "Functional::frequencyMeasure_timing_10MHz_20MHz";                                                                              // Defines the timing conditions for the test, ranging from 10MHz to 20MHz.
    LevelsTc = "Functional::basic_func_lvl_nom";                                                                                                // Sets the level conditions for the test.
    LogLevel = "Enabled";                                                                                                                       // Enables debug console output for the test.
    FunctionalPlugin = "FrequencyMeasurePlugin";                                                                                           // Specifies the name of the edge counter plugin to be used.
    FunctionalPluginInput = "~HDMT_TPL_DIR\\TestPrograms\\Functional\\Modules\\Functional\\InputFiles.hdmt\\FrequencyMeasureConfig.json";  // Path to the JSON configuration file for the plugin.
}
```

#### JSON file example to configure FrequencyMeasure plugin.

The following JSON configuration file is used to set up the **FrequencyMeasure** plugin. It defines the pins to be
monitored and their corresponding output names, as well as additional configuration parameters.

```JSON
{
  "Pins": [
    {
      "PinName": "xxHPCC_DPIN_Dig_slcA_AA0",
      "OutputName": "MyInternalIp0"
    },
    {
      "PinName": "xxHPCC_DPIN_Dig_slcA_AA1",
      "OutputName": "MyInternalIp1"
    },
    {
      "PinName": "xxHPCC_DPIN_Dig_slcA_AA2",
      "OutputName": "MyInternalIp2"
    },
    {
      "PinName": "xxHPCC_DPIN_Dig_slcA_AA3",
      "OutputName": "MyInternalIp3"
    },
    {
      "PinName": "xxHPCC_DPIN_Dig_slcA_AA4",
      "OutputName": "MyInternalIp4"
    },
    {
      "PinName": "xxHPCC_DPIN_Dig_slcA_AA5",
      "OutputName": "MyInternalIp5"
    },
    {
      "PinName": "xxHPCC_DPIN_Dig_slcA_AA6",
      "OutputName": "MyInternalIp6"
    },
    {
      "PinName": "xxHPCC_DPIN_Dig_slcA_AA7",
      "OutputName": "MyInternalIp7"
    }
  ],
  "PreFixToken": "PROMISE_0",     // Token to be prefixed in ituff.
  "PostFixToken": "08200_FREQ",   // Token to be suffixed in ituff.
  "CycleNumber": 22.0             // Number of cycles over which the edges are counted.
}
```

#### ITUFF output

The ITUFF output provides the results of the frequency measurements for each pin configured in the FrequencyMeasure
plugin. Each entry in the output corresponds to a specific pin and includes the test name, measured result, and
measurement unit.

```python
2_tname_Functional::Test_45391_PrimeFunctionalTestMethod_FrequencyMeasurePluginBothEdgesMode_P1_PROMISE_0_MyInternalIp0_08200_FREQ
2_mrslt_1.1
2_msunit_MHz
2_tname_Functional::Test_45391_PrimeFunctionalTestMethod_FrequencyMeasurePluginBothEdgesMode_P1_PROMISE_0_MyInternalIp1_08200_FREQ
2_mrslt_1.26
2_msunit_MHz
...
2_tname_Functional::Test_45391_PrimeFunctionalTestMethod_FrequencyMeasurePluginBothEdgesMode_P1_PROMISE_0_MyInternalIp6_08200_FREQ
2_mrslt_56.00
2_msunit_MHz
2_tname_Functional::Test_45391_PrimeFunctionalTestMethod_FrequencyMeasurePluginBothEdgesMode_P1_PROMISE_0_MyInternalIp7_08200_FREQ
2_mrslt_50.0001
2_msunit_MHz
```

#### Explanation of FrequencyMeasure plugin ITUFF Output Fields

1. Test Name (2_tname_...):
- This field specifies the name of the test and includes the following components:
  - Functional::Test_45391_PrimeFunctionalTestMethod_FrequencyMeasurePluginBothEdgesMode_P1: The name of the test
    method.
  - PROMISE_0: The prefix token defined in the JSON configuration.
  - MyInternalIpX: The output name for the pin, where X ranges from 0 to 7.
  - 08200_FREQ: The postfix token defined in the JSON configuration.

2. Measured Result (2_mrslt_...):
- This field provides the measured frequency result for the corresponding pin.
- The value is given in megahertz (MHz).

3. Measurement Unit (2_msunit_...):
- This field indicates the unit of the measured result, which is megahertz (MHz).

# `IProcessCtvPerPinPlugin` plugin interface
## Overview

The `IProcessCtvPerPinPlugin` interface defines a contract for plugins that process CTV per pin data 
captured during a functional test, specifically, a `ICaptureCtvPerPinTest`.

## Methods

### `Initialize(string userInput)`

Initializes the plugin with the specified `userInput`.

### `ProcessCtvPerPin(ICaptureCtvPerPinTest captureCtvPerPinTest)`

Processes the CTV data captured per pin by the provided test.

- **Parameters:**
  - `captureCtvPerPinTest`: An instance of `ICaptureCtvPerPinTest` that captures the CTV data per pin.
- **Returns:**
  - `bool`: Returns `true` if the processing is successful, otherwise `false`.

## Usage

Implement this interface to create a plugin that can handle and process CTV data captured per pin. The plugin must provide concrete implementations for the `Initialize` and `ProcessCtvPerPin` methods.

## Example

```csharp
public class CtvPerPinProcessor : IProcessCtvPerPinPlugin
{
    public void Initialize(string userInput)
    {
        // Initialization logic here
    }

    public bool ProcessCtvPerPin(ICaptureCtvPerPinTest captureCtvPerPinTest)
    {
        // CTV processing logic here
        return true;
    }
}
```

## Implementations

### `CtvToSharedStorage` plugin

This plugin saves the CTV data per pin and store it in shared storage.
- ** `userInput` Parameters: **
    - `CtvPinDataTokens`: This input should be a comma-separated list of tokens that the plugin will use as SharedStorage's keys.
      An key per CTV pin is expected, and the plugin will fail the `Initialize` if there's no a 1:1 match.
    - `CtvPinNamesParameter`: The current instance's parameter name used for the comma-separated list of CTV pin names. Default=[`CtvCapturePins`].
- ** Examples **
  - This example shows how to use the `CtvToSharedStorage` plugin with the `CtvPinDataTokens` and
    `CtvPinNamesParameter` parameters relying on the default parameter to extract the list of CTV pins.
```python
Test PrimeFunctionalTestMethod BasicExample
{
   Patlist = "ExamplePatList";
   TimingsTc = "ATiming";
   FuncLevels = "ALevel";
   CtvCapturePins = "Pin1, Pin2";
   ProcessCtvPerPinPlugin = "CtvToSharedStorage";
   ProcessCtvPerPinPluginInput = "--CtvPinDataTokens SharedStorageKey1, SharedStorageKey2";
}
```
   - This example shows how to use the `CtvToSharedStorage` plugin with the `CtvPinDataTokens` and
`CtvPinNamesParameter` parameters relying on the default parameter to extract the list of CTV pins.
```python
Test CustomTM CtvPinInCustomParameterExample
{
   Patlist = "ExamplePatList";
   TimingsTc = "ATiming";
   FuncLevels = "ALevel";
   CustomParameter = "Pin1, Pin2";
   ProcessCtvPerPinPlugin = "CtvToSharedStorage";
   ProcessCtvPerPinPluginInput = "--CtvPinDataTokens SharedStorageKey1,SharedStorageKey2 --CtvPinNamesParameter 
   CustomParameter";
}
```

# `IProcessFailuresPlugin` plugin interface
## Overview

The `IProcessFailuresPlugin` interface defines a contract for plugins that process failures captured during a 
functional test, specifically, a `ICaptureFailureTest`.

## Methods

### `Initialize(string userInput)`

Initializes the plugin with the specified `userInput`.

### `ProcessFailures(ICaptureFailureTest captureFailureTest)`

Processes the failures captured by the provided `ICaptureFailureTest`.

- **Parameters:**
  - `captureFailureTest`: An instance of `ICaptureFailureTest` that captures the failures to be processed.
- **Returns:**
  - `bool`: Returns `true` if the processing is successful, otherwise `false`.

## Usage

Implement this interface to create a plugin that can handle and process test failures. The plugin must provide concrete implementations for the `Initialize` and `ProcessFailures` methods.

## Example

```csharp
public class FailureProcessor : IProcessFailuresPlugin
{
    public void Initialize(string userInput)
    {
        // Initialization logic here
    }

    public bool ProcessFailures(ICaptureFailureTest captureFailureTest)
    {
        // Failure processing logic here
        return true;
    }
}
```

## Implementations

### `FailuresToDatalog` plugin

This plugin processes failure data and logs it to a datalog.

- **`userInput` Parameters:**
  - `PrintPreviousLabel`: A boolean flag indicating whether to print the previous label into the datalog. Default=`false`.
  - `MaxFailuresToItuff`: The maximum number of failures to log to Ituff. If not provided, it will use the value from
    the current instance's parameters.
  - `MaxFailuresPerPatternToItuff`: The maximum number of failures per pattern to log to Ituff. If not provided, it will
    use the value from the current instance's parameters.

- **Examples:**
  - This example shows how to use the `FailuresToDatalog` plugin with the `PrintPreviousLabel`, `MaxFailuresToItuff`,
    and `MaxFailuresPerPatternToItuff` parameters.
```python
Test PrimeFunctionalTestMethod BasicExample
{
   Patlist = "ExamplePatList";
   TimingsTc = "ATiming";
   FuncLevels = "ALevel";
   ProcessFailuresPlugin = "FailuresToDatalog";
   ProcessFailuresPluginInput = "--PrintPreviousLabel true --MaxFailuresToItuff 10 --MaxFailuresPerPatternToItuff 5";
}
```

## Version tracking

| **Date**                  | **Version** | **Author**     | **Comments** |
|---------------------------| ----------- | -------------- | ------------ |
| Oct 31<sup>st</sup>, 2024 | 1.0.0       | lchavarr       |              |

## Acronyms

Definition of acronyms used in this document:

- **REP**: P**r**ime T**e**st-Method S**p**ecification
- **HDMT**: High Density Modular Tester
- **TPL**: Test Programming Language
- **TOS**: Test Operating System