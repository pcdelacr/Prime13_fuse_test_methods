**prime Test-Method Specification REP**

[[_TOC_]]

## REP for PinProfiler

This **REP** is intended to describe the PinProfiler Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Datalog output** – A detailed description of what is data-logged by his TestMethod

  - **Custom User Code hooks** – A list of functions available to the user code to override

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document 

## Methodology

The PinProfiler test method provides the capability to start and end the profile of the given pins per test instance, for all the test instances between the Start and the Stop of the same tag.

This is a new feature enabled for Prime, where Prime will datalog the profile for the given pins at the end of every test instance.
The data type this test method will datalog depends on the pins being profiled. For example, if the pins are TDAU, the data type will be the temperature for those pins; while if the pins are DPS, the data will the Voltage and Current.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the ApplyTestCondition test method

| **Parameter Name**    | **Required?**    | **Type**        | **Values**                      |
| --------------------- | ---------------- | --------------- | ------------------------------- |
| Mode                  | Yes              | String (choice) | Start (**default**)             |
|                       |                  |                 | Stop                            |
| PinNames              | When Mode=Start  | String          | A comma-separated list of pins. |
| Tag                   | Yes              | String          | The tag of the current profile. |
| SamplingInterval      | When Mode=Start  | Integer         | The number of milliseconds between measurements (multiples of 4).          |

The user is able to trigger different profiles with different tags. These profiles can share the same pin as soon as they respect the constraints of the hdmtAPI, like having all the pins be of the same type.

## Datalog output

This test method, by default, will datalog the profile per pin at the end of every test instance executed between the PinProfiler test instance with the Start mode, and the one with the Stop mode, for the given tags.

## Custom User Code Hooks

By default, this test method will datalog the profile to the ituff.
It also offers the capability to inject a callback (for more information about how the callback works, refer to callback's documentation).
This callback receives a string with the tag of the data.
Then it has to request the proper data to the `PinService` using that tag.
Then the callback can handle the data as required by the user.
Example:
```csharp
public static void PostProcessData(string tag)
{
    var streamingDataCollection = Prime.Services.PinService.GetStreamingSensorData(tag);
    foreach (var perPinDataCollection in streamingDataCollection)
    {
        var pinName = perPinDataCollection.GetPinName();
        var dataType = perPinDataCollection.GetDataType();
        var pinData = perPinDataCollection.GetData();
        var strgvalWriter = Prime.Services.DatalogService.GetItuffStrgvalWriter();
        strgvalWriter.SetTnamePostfix("_PinProfiler_Callback_" + pinName + "_" + dataType);
        strgvalWriter.SetData($"min_{pinData.Min()}_max_{pinData.Max()}");
        Prime.Services.DatalogService.WriteToItuff(strgvalWriter);
    }
}
```

## TPL Samples

Here are a few test instance examples using the PinProfiler test method

TPL Sample1:  

```javascript
Import PrimePinProfilerTestMethod.xml;

Test PrimePinProfilerTestMethod StartCore_P1
{
   Mode = "Start";
   PinNames = "TDAU_CH_CORE";
   Tag = "CoreTag";
   SamplingInterval = 4;
}

Test PrimePinProfilerTestMethod StopCore_P1
{
   Mode = "Stop";
   Tag = "CoreTag";
}
```

## Exit Ports

The ApplyTestCondition test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

## Additional Dependencies

More dependencies to consider for this TestMethod to well operate:

  - All provided test condition names must be valid test condition names, just check they are the same as the defined test conditions     on the tester

## Version tracking


| **Date**       | **Version** | **Author**     | **Comments**    |
| -------------- | ----------- | -------------- | --------------- |
| Mar 18th, 2021 | 1.0.0       | lchavarr       | Initial Version |
|                |             |                |                 |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System