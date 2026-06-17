**prime Test-Method Specification REP**

Revision 1.1.0

April 2022

[[_TOC_]]

## Methodology

This test method intends to decode the CTVs captured in executing a plist in lot number, wafer number, and location X and Y of the Die ID.

The ULTDecoder test method can perform plist execution, capture CTV data, and decode them in Lot number, Wafer, X Location, and Y Location.

The way the data needs to be decoded is specific to each fab, so this test method relies on Prime's plugin infrastructure to provide a unified user interface where the user can provide an implementation of the `IUltDecoderPlugin` interface to extend this test method and specify a custom decoding.

Currently, Prime supports out-of-the-box decoding specifications for Intel's format with the `UltDecoderPlugin` and for TSMC with the `TsmcDecoderPlugin`.

This method do not catch the raw data, it catches in the shared storage from the BusinessLogic.FuseLogicCollection.UltHandle, the lot, wafer, and location under the die id name.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the UltDecoder test method

| **Parameter Name** | **Required?** | **Type**        | **Values**                                                                                                                                                                                                                                                     | **Comments** |
| ------------------ | ------------- | --------------- |----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------| ------------ |
| Patlist            | Yes           | Plist           | Plist name to be executed                                                                                                                                                                                                                                      |              |
| LevelsTc           | Yes           | LevelsCondition | Levels test condition required for plist execution                                                                                                                                                                                                             |              |
| TimingsTc          | Yes           | TimingCondition | Timing test condition required for plist execution                                                                                                                                                                                                             |              |
| MaskPins           | No            | String          | Comma separated list of pins for which the fail data capture will be skipped. Default is Empty String                                                                                                                                                          |              |
| CapturePins        | No            | String          | Comma separated list of pins for which CTV data should be captured                                                                                                                                                                                             |              |
| ExpectedBitCount   | Yes           | Integer         | Number of expected bits to capture. Currently, there are supported values 50, 56, or 64.                                                                                                                                                                       |              |
| DieIdNames         | Yes           | String          | Comma separated list of the die id associated with each of the pins. You must have one for each pin. Ex. U1,U2.U1,U3. Also accept uservar in replace of die id. Ex. UserVar.DieId1,U2.U1,UserVar.DieId2. However one uservar should only represent one die id. |              |
| Offset             | Yes           | Integer         | If the CTVs have an offset you can define it here. Default 0. If one data is given it applies for all pins, otherwise a value for each pin must be given separated by commas.                                                                                  |
| UserVarsList       | No            | String          | If the data are coming from UserVars or SharedStorage include the userVarName or the SharedStorage's keys separated by commas, one for each DieId. If using this parameter; PatList, LevelsTc, and TimingTc are not required.                                  |
| CTVs order         | Yes           | LSB/MSB         | This denotes whether the first bit is the least significant [LSB]  or the most significant [MSB]. Default is LSB                                                                                                                                               |
| UltDecoderPlugin | Yes | `Plugin<IUltDecoderPlugin>` | This parameter specifies the plugin to use to decode the data. Default=[`Prime.TestMethods.UltEncoderDecoder.UltDecoderPlugin`]                                                                                                                                                                                      |                                                                                                                                       

## Intel's decoding
It's implemented and exposed via the `UltDecoderPlugin`.

When capture bits count is 50, the code is done following this conversion:

![image.png](./.attachments/image-926e05fb-0c2b-4c03-a832-13c5d8c5bced.png)

When capture bits count is 56, the code is done following this conversion:

![image.png](./.attachments/image-0e9d3844-eeee-4d11-b038-1d47b5b66d8f.png)

When capture bits count is 64, the code is done following this conversion:

![Universal intel ULT decode.png](./.attachments/Universal%20intel%20ULT%20decode-e936fbde-838c-4a0c-9bdd-3e697d14afce.png)
Intel Universal decoding.

## TSMC's decoding
It's implemented and exposed via the `TsmcDecoderPlugin`.

![TSMC.png](./.attachments/TSMC-4ad66bd1-697d-4153-8d43-6529bc3bb74e.png)

## Console output

When LogLevel is TEST\_METHOD - test method shows the actual CTV data captured

![image.png](./.attachments/image-735976f4-e279-4785-8ebb-eb763b49c433.png)

## Datalog output
### Ituff

Functional test execution results are logged as shown below:

<u>Functional part passed printout</u>:

2\_tname\_Func::PrimeUltDecoderTestMethod
2\_strgval\_pass

<u>Functional part failed printout</u>:

2\_tname\_Func::PrimeUltDecoderTestMethod
2\_strgval\_fail

Could not execute functional test (abnormal error or alarm)

2\_tname\_Func::PrimeUltDecoderTestMethod
2\_strgval\_notexecuted

### Scan
Information is printed in scan datalog intended to identify the current DUT.
An example of the priting is as follows:

```
2_lotid_D1246360
2_wafid_709
2_xloc_-01
2_yloc_-04
```

### Ult Detail(example)
```
2_lsep
2_sstrlot_U2.U1_Y5106803
2_sstrwafer_U2.U1_330
2_sstrxloc_U2.U1_-10
2_sstryloc_U2.U1_+00
2_lsep
2_tname_FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1
2_tssid_U2.U1
2_strgalt_nsv_Y5106803_330_-10_+00
2_lsep
2_sstrlot_U4_Y5106803
2_sstrwafer_U4_330
2_sstrxloc_U4_-10
2_sstryloc_U4_+02
2_lsep
2_tname_FUSEREAD::ExecutePattern_x2_PostUltDecodeMask_1PreMask_2PostMask_Pass_P1
2_tssid_U4
2_strgalt_nsv_Y5106803_330_-10_+02
```
if the ultdecode fails
```
2_lsep
2_sstrlot_U2.U1_9999999
2_sstrwafer_U2.U1_999
2_sstrxloc_U2.U1_99
2_sstryloc_U2.U1_99
2_lsep
2_tname_FuseRead::ExecutePattern_x2_PostUltDecodeMask_1PreMask_UltDecode_Fail_F3
2_tssid_U2.U1
2_strgalt_nsv_9999999_999_99_99
```

## Custom User Code Hooks

Here is the list of functions available to the user code to override.

```python
    public interface IUltDecoderExtensions
    {
        /// <summary>
        /// Called right after successful execution to allow the user to post process exit port, CTV results, and functional results.
        /// </summary>
        /// <param name="results">The object for holding the exit port, CTV results, and functional results.</param>
        void CustomPostProcessResults(UltDecoderTestInstanceResults results);

        /// <summary>
        /// Called prior to any test in Execute to generate the test instance results object.
        /// </summary>
        /// <returns>The test instance results object.</returns>
        UltDecoderTestInstanceResults CreateTestInstanceResults();

        /// <summary>
        /// Gets the list of pins to mask execution after execution. The test method will merge this list with the ones from the test instance parameter.
        /// </summary>
        /// <returns>The list of mask pins.</returns>
        List<string> GetDynamicPinMask();
    }
```

- CreateTestInstanceResults - Creates test instance results object that holds all results and the exit port (to be done early in Execute).

- CustomPostProcessResults - Allows user to post-process all results and override the exit port determined by the test method.  
Input is the test instance results object which includes functional test results, CTV data, and the exit port.

- GetDynamicPinMask - Allows user to populate pinmask pins after TP load.

**Default Post Processing**
```python
        void IUltDecoderExtensions.CustomPostProcessResults(UltDecoderTestInstanceResults results)
        {
            if (results.FunctionalResult && (results.CtvResults != null))
            {
                results.ExitPort = this.ProcessCtvAndUltData(results.CtvResults);
            }
            else
            {
                Prime.Services.ConsoleService.PrintDebug("Functional test execution failed");
                results.ExitPort = TestMethodFailPort; // setting fail port
            }
        }
```
Note: UltDecoder main function has been moved to BusinessLogic FuseLogicCollection, the function can be called when the UltDecoderBasic.cs and Ult.cs are included to the project.


## TPL Samples

Here are a few test instance examples using the UltDecoder test method

```python
Import PrimeUltDecoderTestMethod.xml;

Test DecodeULT PrimeUltDecoderTestMethod
{
   Patlist = "array_pbist_llc_dat_lya_autoclkmux0_s0_list";
   TimingsTc = "BASE::cpu_func_sdr_univ_sta_univ_univ_b100_t100_d100";
   LevelsTc = "__main__::leakage_tc";
   CtvCapturePins = "NOAB_00";
   DieId = "U1";
   ExpectedBitCount = "56";
}

Test FromUserVar PrimeUltDecoderTestMethod
{
   UserVars = "FUSE.U1,FUSE.U2";
   DieId = "U1,U2";
   ExpectedBitCount = "50";
}
```

##Exit Ports

The UltDecoder test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Plist execution has failed   |
| **1**         | ***Pass***      | Plist execution has passed   |
| **2**         | ***Fail***      | Decode logic failed          |
| **N**         | ***Pass/Fail*** | Failing condition            |

## Version tracking

| **Date**      | **Version** | **Author**        | **Comments**                                     |
| ------------- | ----------- | ----------------- | ------------------------------------------------ |
| March 4, 2020 | 1.0.0       | Slava Yablonovich | Initial version                                  |
| Oct. 10, 2020 | 1.0.1       | Gilbert Figueroa  | Update for several verification                  |
| Oct. 15, 2020 | 1.0.2       | Gilbert Figueroa  | Adding Intel universal decoding                  |
| June  9, 2021 | 1.0.3       | Kevin D. Krake    | Adding pin masking ability                       |
| Nov. 24, 2021 | 1.0.4       | Brian Morera      | Printing ult info to scan datalog                |
| Apr. 11, 2022 | 1.10        | Kevin D Krake     | Adding new Results method for exit port handling |
| Jul. 29, 2022 | 1.11.1      | Lee, Yeong Jui    | Moving UltDecoder basic function to FuseLogicCollection |
| Dec. 02, 2022 | 1.11.2      | Javier Alpizar    | Updating Port 2 documentation                    |
| Feb. 28, 2023 | 12.0.0      | Lee, Yeong Jui    | DieIdNames vs PackageEfuse in PrimeFuseReadMaskUltDecodeTestMethod|
| Feb. 28, 2023 | 12.0.0      | Lee, Yeong Jui    | To Decode ULT even this ULT is not available at UBE File|

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High-Density Modular Tester
  - **TPL**: Test Programming Language
  - **CTV**: Capture This Vector
  - **ULT** Means Unit Level Traceability is a consolidated online database application that provides unit-based traceability across operations.