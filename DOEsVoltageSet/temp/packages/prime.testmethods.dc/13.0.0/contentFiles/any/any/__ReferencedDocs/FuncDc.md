**prime Test-Method Specification REP**

November 2023

[[_TOC_]]

## REP for FuncDc

This **REP** is intended to describe the FuncDc Prime TestMethod.

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

The FuncDc test method provides capability to perform plist execution followed by a raw Dc measurements (Voltage and Current).

It can be used to implement, for example, Leakage methodology where plist is executed as a precondition to a DC test This test has two levels TC parameters - one for functional execution and one for DC execution DC level defines setup and triggers the measurement.

This is the example of the DC level

![image.png](./.attachments/image-dfb339d8-8a8b-46b5-8ec5-5b3b34b2ef1d.png)

In the above example, each measured pin is being setup, measured (using *StartMeasurement* attribute) and then reset to 0V sequentially.

### Verify

  - Validate limits (numeric with units)
  - Validate test condition exist and valid
  - Validate pins
  - Match number of pins to number of limits
  - Validate plist exists

### Execute

  - Execute functional test (preconditions the DUT)
  - Apply DC Levels to HW (triggers DC measurement)
  - Gather the measured values
  - Match measured values against the provided limits
  - Print results to Datalog(ituff)

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Dc test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                                                                                         | **Comments**                                |
| ------------------ | ------------- | --------------- | -------------------------------------------------------------------------------------------------- | ------------------------------------------- |
| MeasurementTypes   | No            | String (choice) | Comma separated list of measurement types, Current(**default**) , Voltage or inital letter "C", "V" |if is used only one measurement type, that will be applied to all pins |
| DatalogLevel       | No            | String (choice) | FAIL_ONLY(**default**), ALL, COMPRESS, PINMAP_COMPRESS                                             | FAIL_ONLY - prints to Ituff only fail result.<br> ALL - prints both fail and pass results.<br> COMPRESS - print all result in ituff compress format.<br> PINMAP_COMPRESS - print all result in ituff compress format with pinMapId. |
| Patlist            | Yes           | Plist           | Plist name to be executed                                                                          |                                             |
| Pins               | Yes           | String          | Comma separated list of measured pins                                                              |                                             |
| LowLimits          | No            | String          | Comma separated list of real numbers with units representing measurement low limits                | Default value – empty string                |
| HighLimits         | No            | String          | Comma separated list of real numbers with units representing measurement high limits               | Default value – empty string                |
| DcLevels           | Yes           | LevelsCondition | Levels test condition to be applied after the plist execution to setup and trigger DC measurements |                                             |
| LevelsTc           | Yes           | LevelsCondition | Levels test condition to be applied prior to pattern execution                                     |                                             |
| TimingsTc          | Yes           | TimingCondition | Timing test condition to be applied prior to pattern execution                                     |                                             |
| MaskPins           | No            | String          | Comma separated list of pins for which the fail data capture will be skipped                       | Default value – empty string                |
| AlarmPortRedirect  | No            | String (choice) | DISABLED  (**default**)                                                                            | Default alarm port=[-2] behavior.           |
|                    |               |                 | ENABLED                                                                                            | Enable the alarm port redirect to port=[2]. |

## Console output (debug mode)

Measurement results can be printed to console in debug mode in form of
below table

Passing case results printout

![image.png](./.attachments/image-7691ef1b-1f70-4ede-b3e7-7deebaf7f594.png)

Failing case results printout

## Datalog output

Dc results are logged to Ituff in “composite” format as shown below
```
2_tname_Dc::PrimeFuncDcTest
2_category_pc
2_composite_0002028_0.001488
2_composite_0002028_0.001488
2_composite_0002020_0.002499
2_composite_0002020_0.002499
2_composite_0002004_0.003490
2_composite_0002004_0.003490
2_composite_0007051_0.000496
2_composite_0007051_0.000496
2_composite_0007107_0.000496
```

Example ituff with failing pin.
```
2_tname_Dc::Test_F0
2_category_fc
2_composite_0000002_0.0200000
2_tname_Dc::Test_F0
2_category_pc
2_composite_0000000_0.0020000
2_composite_0000001_0.0030000
```

**IMPORTANT**
The above datalog printout is executed by default from CustomPostProcessResults hook (see below )
In case user will override CustomPostProcessResults hook function there are few options for printing the results to datalog:
1. User will implement their own logic for results printout to Ituff as part of CustomPostProcessResults function implementation
2. To print the default datalog output in the above format (composite) user will call the IDcResult API PrintToDatalog in CustomPostProcessResults implementation
```python
        void IFuncDcExtensions.CustomPostProcessResults(FuncDcTestInstanceResults results)
        {
            var areDcResultsWithinLimits = results.DcResults.AreAllDcResultsWithinLimits(this.perPinDcSetup);
            results.ExitPort = (results.FunctionalResult && areDcResultsWithinLimits) ? TestMethodPassPort : TestMethodFailPort;
            this.DcResultsFormat.SetData(results.DcResults);
            Prime.Base.ServiceStore<IDatalogService>.Service.WriteToItuff(this.DcResultsFormat, this.SessionContext);

            if (results.FailPinsName.Count > 0)
            {
                var failPinsInfo = new List<FailPinInfo>();

                foreach (var pinName in results.FailPinsName.Distinct())
                {
                    failPinsInfo.Add(DcCommon.CreateDcFailPinInfo(pinName, string.Empty, this.InstanceName));
                }

                DcCommon.DatalogFailPinInfo(failPinsInfo, this.SessionContext);
                DcCommon.StoreFailPinInfoToSharedStorage(failPinsInfo, this.SessionContext);
            }
        }
```
3. To print the default datalog output in the above format (composite) user will call the base class implementation of CustomPostProcessResults and then add its own:
```python
        void IFuncDcExtensions.CustomPostProcessResults(FuncDcTestInstanceResults results)
        {
            base.CustomPostProcessResults(results)

            ///////////////////////////////////
            //  Rest of the user logic
            ///////////////////////////////////

        }
```

## Custom User Code Hooks

FuncDc test method supports the following extensions:

![image.png](./.attachments/image-58d37d36-45c5-4a3e-a47f-21eb8ecda08b.png)

- CreateTestInstanceResults - Creates test instance results object that holds all results and the exit port (to be done early in Execute).

- CustomPreExecute -  using this function user who extending this test method can perform some pre process before the Dc measurement

- CustomPostProcessResults - Allows user to post-process all results and override the exit port determined by the test method.  
Input is the test instance results object which includes functional test results, DC results, and the exit port.

- GetDynamicPinMask- allows user to populate mask pins after TP load

Example implementations of those functions can be seen in Prime's SampleTP , user code section - Prime\UserSDK\SampleTP\UserCode

## TPL Samples

Here are a few test instance examples using the FuncDc test method

```python
Import PrimeFuncTestMethod.xml;

Test PrimeFuncTestMethod PrimeLeakageDcTest
{
   Patlist = "array_pbist_llc_dat_lya_autoclkmux0_s0_list";
   TimingsTc = "BASE::cpu_func_sdr_univ_sta_univ_univ_b100_t100_d100";
   FuncLevels = "__main__::leakage_tc";
   DcLevels = "__main__::start_measurement_tc";
   Pins = "NOAB_00,NOAB_01,NOAB_02";
   MeasurementType = "Current";
   LowLimits = "0A,0A,0A";
   HighLimits = "0.05A,0.04A,0.03A";
   LogLevel = "PRIME_DEBUG";
}
```

## Exit Ports

The Dc test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                            |
| ------------- | ------------- | -------------------------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                                                        |
| **-1**        | ***Error***   | Any software condition error                                               |
| **0**         | ***Fail***    | Failing condition. Results are out of limits or plist execution had failed |
| **1**         | ***Pass***    | Passing condition – results are within the limits                          |
| **2**         | ***Alarm***   | Any alarm condition if the AlarmPortRedirect is enabled. |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language

## Version tracking

| **Date**                  | **Version**     | **Author**            | **Comments**                                                                    |
| ------------------------- | --------------- | --------------------- | ------------------------------------------------------------------------------- |
| Feb 23<sup>rd</sup>, 2020 | 1.0.0           | Slava Yablonovich     | Initial version                                                                 |
| May 18<sup>th</sup>, 2021 | 1.0.1           | Adam Malik            | Removed functional datalog printout documentation.                              |
| May 24<sup>th</sup>, 2021 | 1.0.2           | Didier Jimenez Retana | PR 2497: Allow the TriggeredDc test method to perform measurement type per Pin. |
| Jun  9<sup>th</sup>, 2021 | 1.0.3           | Kevin D. Krake        | Adding pin mask ability.                                                        |
| Nov 15<sup>th</sup>, 2021 | 7.1             | Gadeer Awaisy         | Allow user to override PrintToDatalog                                           |
| Apr  6<sup>th</sup>, 2022 | 9.0             | Kevin D Krake         | Adding new Results method for exit port handling                                |
| Sep 13<sup>th</sup>, 2023 | 12.02.02        | Teoh, Khai Jie        | include fail pin name and fail channel printing to ituff. #39827                |
| Nov 21<sup>th</sup>, 2023 | 12.03.00        | Teoh, Khai Jie        | Disable Witch Project '2_tname_failchannel' and '3_binpinfails' printing to ituff.<br> #46008 |