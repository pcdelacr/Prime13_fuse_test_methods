[[_TOC_]]

## REP for Dc

This **REP** is intended to describe the Dc Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Datalog output** – A detailed description of what is datalogged by this TestMethod

  - **Custom User Code hooks** – A list of functions available to the user code to override

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document
  
## Methodology

The Dc test method provides capability to perform raw Dc measurements (Voltage and Current).
The Dc measurements setup is implemented in Levels block.
Please find below an example of serial Shops methodology Levels setup

![image.png](./.attachments/Dc/image-fc8353e4-0790-4d9a-a3d8-27f3f93f2a72.png)

In the above example, each measured pin is being setup, measured (using *StartMeasurement* attribute) and then reset to 0V sequentially.

Here is the example of serial DC measurement setup in level block :

```csharp
Levels serial_dc_shops
{
        ###### Pingroup 1  ######
    tpc_tx
    {
        OPMode = ISVM;
        IRange = IR1mA;
        IForce = 0.001A;
        VClampHi = 2V;
        VClampLo = -1.5V;
        StartMeasurement = True; # Attribute that triggers measurement on the supply.
        PreMeasurementDelay = 20us;
        SamplingCount = 1;
        SamplingRatio = 1; #Provides sampling control to the users.
        SamplingMode = Trace;
    }
    SequenceBreak 1mS;

    tpc_tx
    {
        OPMode = VSIM;
        IRange = IR1mA;
        VForce = 0V;
    }

    SequenceBreak 1mS;

    ###### Pin 1  ######
    NOAB_00
    {
        OPMode = ISVM;
        IRange = IR1mA;
        IForce = 0.001A;
        VClampHi = 2V;
        VClampLo = -1.5V;
        StartMeasurement = True; # Attribute that triggers measurement on the supply.
        PreMeasurementDelay = 20us;
        SamplingCount = 1;
        SamplingRatio = 1; #Provides sampling control to the users.
        SamplingMode = Trace;
    }
    SequenceBreak 1mS;

    NOAB_00
    {
        OPMode = VSIM;
        IRange = IR1mA;
        VForce = 0V;
    }

    SequenceBreak 1mS;
} #End of serial_dc_shops

```

### Verify

  - Validate limits (numeric with units)

  - Validate Levels block

  - Validate pins

  - Match number of pins to number of limits

### Execute

  - Apply Levels to HW (if Levels was provided by the user)

  - Gather the measured values

  - Match measured values against the provided limits

  - Print results to Datalog(ituff)

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Dc test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                                                                                          | **Comments**                                                                                                                                                                                                                                                                                                                             |
| ------------------ | ------------- | --------------- |-----------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| MeasurementTypes   | No            | String (choice) | Comma separated list of measurement types, Current(**default**) , Voltage or inital letter "C", "V" | if is used only one measurement type, that will be applied to all pins                                                                                                                                                                                                                                                                   |
| DatalogLevel       | No            | String (choice) | FAIL_ONLY(**default**), ALL, COMPRESS, PINMAP_COMPRESS, PIN_DETAIL                                  | FAIL_ONLY: Prints to Ituff only fail result.<br> ALL: Prints both fail and pass results.<br> COMPRESS: Prints all result in ituff compress format.<br> PINMAP_COMPRESS: Prints all result in ituff compress format with pinMapId.<br> PIN_DETAIL: Prints pin name and SuperPinGroup (if used), and Pin Channel as a comnt if pin failed.<br> **NOTE**: Refer to [DC Business Logic](../../../../BusinessLogic/Dc/Readme.md) Wiki for more info.|
| Pins               | Yes           | String          | Comma separated list of measured pins                                                               |                                                                                                                                                                                                                                                                                                                                          |
| LowLimits          | No            | String          | Comma separated list of real numbers with units representing measurement low limits                 | Default value – empty string                                                                                                                                                                                                                                                                                                             |
| HighLimits         | No            | String          | Comma separated list of real numbers with units representing measurement high limits                | Default value – empty string                                                                                                                                                                                                                                                                                                             |
| LevelsTc           | Yes           | LevelsCondition | Levels TC required to setup and perform the DC measurement                                          |                                                                                                                                                                                                                                                                                                                                          |
| AlarmPortRedirect  | No            | String (choice) | DISABLED  (**default**)<br>ENABLED                                                                  | Default alarm port=[-2] behavior.<br>Enable the alarm port redirect to port=[2].                                                                                                                                                                                                                                                         |
| EDMEnable          | No            | String          | DISABLED  (**default**)<br>ENABLED                                                                  | Enable the EDM process converaged from SPEXEDM. will print the pin's measurments to ituff and console, NOTE: this just support single pin as SPEXEMD expected, And if this is not enabled, the DFF config below will not take effect will go to normal DC Process                                                                        |
| DFFEnabled         | No            | String          | DISABLED  (**default**)<br>ENABLED                                                                  | Enabled will set assigned pin to be dff'd.                                                                                                                                                                                                                                                                                               |
| PinToDff           | No            | String          | default is string.Empty                                                                             | Assign pin name to overwrite default - if input is empty string, default is first index of Pins list.                                                                                                                                                                                                                                    |
| DFFNAME            | No            | String          | DFFNAME to write to - default is SCEDM.                                                             |                                                                                                                                                                                                                                                                                                                                          |

## Console output (debug mode)

Measurement results can be printed to console in debug mode in form of below table

Passing case results printout

![image.png](./.attachments/Dc/image-826e524e-fa01-4954-9a74-bd214a7a8c7f.png)

Failing case results printout

![image.png](./.attachments/Dc/image-72f9f8da-50dc-4e47-8d52-bd04ac86e429.png)

## Datalog output

Dc results are logged to Ituff in “composite” format as shown below
```
2_tname_Dc::PrimeShopsDcTest

2_category_pc
2_composite_0002028_0.001488
2_composite_0002028_0.001488
2_composite_0002020_0.002499
2_composite_0002020_0.002499
2_composite_0002004_0.003490
2_composite_0002004_0.003490
2_composite_7000051_0.000496
2_composite_0007051_0.000496
2_composite_0007107_0.000496
```

Example ituff with failing pin.
```
2_tname_Dc::PrimeContinuityDcTest_F0
2_category_fc
2_composite_0000002_0.0200000
2_tname_Dc::PrimeContinuityDcTest_F0
2_category_pc
2_composite_0000000_0.0020000
2_composite_0000001_0.0030000
```

**IMPORTANT**
The above datalog printout is executed by default from CustomPostProcessResults hook (see below )
In case user will override CustomPostProcessResults hook function there are few options for printing the results to datalog:
1. User will implement their own logic for results printout to Ituff as part of CustomPostProcessResults function implementation
2. To print the default datalog output in the above format (composite) user will call the IDcResult API  PrintToDatalog in CustomPostProcessResults implementation
```csharp
        void IDcExtensions.CustomPostProcessResults(DcTestInstanceResults results)
        {
            bool areDcResultsWithinLimits = results.DcResults.AreAllDcResultsWithinLimits(this.perPinDcSetup);
            results.ExitPort = areDcResultsWithinLimits ? TestMethodPassPort : TestMethodFailPort;
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

## Custom User Code Hooks

Dc test method supports the following extensions:

![image.png](./.attachments/Dc/image-aafac3dc-9851-495b-b45d-4b77cd74c3a1.png)

- CreateTestInstanceResults - Creates test instance results object that holds all results and the exit port (to be done early in Execute).

- CustomPreExecute - Allows user to perform some pre-processing before the DC measurements.

- CustomPostProcessResults - Allows user to post-process all results and override the exit port determined by the test method.  
Input is the test instance results object which includes both DC results, and the exit port.

Example implementations of those functions can be seen in Prime's SampleTP , user code section - Prime\UserSDK\SampleTP\UserCode

## TPL Samples

Here are a few test instance examples using the Dc test method
**Shops implementation:**

In this test the pins are digital signals and the level block defined shops implementation.

```csharp
Import PrimeDcTestMethod.xml;
Test PrimeDcTestMethod PrimeShopsDcTest
{
   LevelsTc = "__main__::serial_dc_shops_tc";
   Pins = "NOAB_00,NOAB_01,NOAB_02,tpc_tx";
   MeasurementType = "Voltage";
   LowLimits = "-5mV,1mV,2.5mV,0mV";
   HighLimits = "15mV,35mV,35mV,55mV";
   LogLevel = "PRIME_DEBUG";

}
```

**Continuity implementation**

In this test the pins are power supply pins and the level block defined continuity implementation.

```csharp
Import PrimeDcTestMethod.xml;

Test PrimeDcTestMethod PrimeContinuityDcTest
{
   LevelsTc = "__main__::serial_continuity_tc";
   Pins = "VCC1P8A_HC,VCCIN_HC,VCCDDQ_HC";
   MeasurementType = "Current";
   LowLimits = "1mA,1mA,1mA";
   HighLimits = "5mA,5mA,6mA";
   LogLevel = "PRIME_DEBUG";
}

```
## Note: the DFF and Print Measurements capability is based on requirements from SORT SPEXEDM code convergency, Diff with SPEXEDM is(to make sure no impact to original process)
 - add a parameter EDMEnabled to be Enabled to go to SEPXEDM logic, otherwise it will just go through Original logic, 
 - low limit fail port is 3
 - high limit fail is 4
 - Dff fail port is 5 

### DFF Enabled Example (VSIM Return resistance - default DFF token)
```csharp
Import PrimeDcTestMethod.xml;

Test PrimeDcTestMethod EDM_X_DC_K_START_X_X_2UA_X_OPENS
{
    LevelsTc = "BASE::PRIME_EDM_VSIM_lvl";
    Pins = "XXEDM_GCD";
    LowLimits = "-0.000001A";
    HighLimits = "0.000007A"; 
    DFFEnabled = "Enabled";
    SamplingCount = "1";
    MeasurementTypes = "Current";
    LogLevel = "PRIME_DEBUG";
    EDMEnabled = "Enabled";
}
```
### DFF Disabled Example (VSIM Return resistance)
```csharp
Import PrimeDcTestMethod.xml;

Test PrimeDcTestMethod EDM_X_DC_K_START_X_X_2UA_X_SHORTS
{
    LevelsTc = "BASE::PRIME_EDM_VSIM_lvl";
    Pins = "XXEDM_GCD";
    LowLimits = "-0.000001A";
    HighLimits = "0.000007A"; 
    DFFEnabled = "Disabled";
    SamplingCount = "1";
    MeasurementTypes = "Current";
    EDMEnabled = "Enabled";
}
```
### DFF Enabled Example (VSIM Return resistance - custom DFFTOKEN)
```csharp
Import PrimeDcTestMethod.xml;

Test PrimeDcTestMethod EDM_X_DC_K_START_X_X_2UA_X_OPENS
{
    LevelsTc = "BASE::PRIME_EDM_VSIM_lvl";
    Pins = "XXEDM_GCD";
    LowLimits = "-0.000001A";
    HighLimits = "0.000007A"; 
    DFFEnabled = "Enabled";
    DFFNAME = "SCAWESOMEDFFTOKEN"
    SamplingCount = "1";
    MeasurementTypes = "Current";
    EDMEnabled = "Enabled";
}
```
## Exit Ports

The Dc test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                          |
| ------------- | ------------- | -------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                                      |
| **-1**        | ***Error***   | Any software condition error                             |
| **0**         | ***Fail***    | Failing condition. Results are out of limits             |
| **1**         | ***Pass***    | Passing condition                                        |
| **2**         | ***Alarm***   | Any alarm condition if the AlarmPortRedirect is enabled. |
| **3**         | ***Fail***    | TestMethodFailPortLo - Low limit. (enable this port if PrintMeasurementsEnable is Enabled) |
| **4**         | ***Fail***    | TestMethodFailPortHi - High Limit (enable this port if PrintMeasurementsEnable is Enabled)|
| **5**         | ***Fail***    | DFF Assignment failed. (enable this port if DffEnabled is Enabled) |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SHOPS**: **Sh**orts and **Op**en**s** test methodology
  - **DC: direct current**

## Version tracking

| **Date**                  | **Version** | **Author**            | **Comments**                                                                    |
| ------------------------- | ----------- | --------------------- | ------------------------------------------------------------------------------- |
| Feb 23<sup>rd</sup>, 2020 | 1.0.0       | Slava Yablonovich     | Initial version                                                                 |
| May 24<sup>th</sup>, 2021 | 5.01.00     | Didier Jimenez Retana | PR 2497: Allow the TriggeredDc test method to perform measurement type per Pin. |
| Nov 15<sup>th</sup>, 2021 | 7.1         | Gadeer Awaisy         | Allow user to override PrintToDatalog                                           |
| Mar 25<sup>th</sup>, 2022 | 9.0         | Kevin D Krake         | Adding new Results method for exit port handling                                |
| Sep 13<sup>th</sup>, 2023 | 12.02.02    | Teoh, Khai Jie        | include fail pin name and fail channel printing to ituff.<br> #39827                |
| Nov 21<sup>th</sup>, 2023 | 12.03.00    | Teoh, Khai Jie        | Disable Witch Project '2_tname_failchannel' and '3_binpinfails' printing to ituff.<br> #46008 |
| Mar 20<sup>th</sup>, 2025 | 13.02.00    | Liu, Tao              | Combine the code from Spex to enable EDM test to print measurements in console and upload to DFF.<br> #57830 |