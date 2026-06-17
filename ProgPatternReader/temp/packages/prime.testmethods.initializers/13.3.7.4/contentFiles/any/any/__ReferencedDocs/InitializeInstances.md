# REP for Prime's InitializeInstances test method

This **REP** is intended to describe the InitializeInstances Prime TestMethod.

[[_TOC_]]

# Methodology

The InitializeInstances test method provides several capabilities:

1)  Verify Prime's test instances defined in the PriorityInstanceVerify parameter.
2)  Verify all Prime Instances for first init, and skipped subsequent Verify all if the conditions are meet. Refer to section InitOptimization for more information.

# Test Instance Parameters

The table below lists and describes the test instance parameters supported by the InitializeInstances test method

| **Parameter Name**        | **Required?** | **Type**                         | **Values**           | **Comments**                               |
| ------------------------- | ------------- | -------------------------------- | -------------------- | ------------------------------------------ |
| VerifyAll                 | No            | Choices                          | True(Default), False | When True, will Verify all Prime instances
| PriorityInstanceVerify    | No            | Comma-separated list of strings  | (Default)            | A list of test instance to run the `Verify` first, before the rest. |
| EngineeringDebug          | No            | Choices                          | False(Default), True | When True, will force reverify for every Prime instances. |
| PrintPatternMemory        | No            | Choices                          | False(Default), True | When True, will print all patterns memory vector count. |

**Notes:**
If the `MemoryAndTimeProfiling` parameter is different than disabled, the InitializeInstances test method will profile the all instances' `Verify` in a comma-separated table so it can easely be post-processed:

All instances' `Verify`'s example:
```
Test instances' Verify - Per instance summary.
===========================================================================
Name,ClassName,ElapsedTime(ms),PrivateBytesBefore(B),PrivateBytesAfter(B),PrivateBytesDelta(B)
Base::DeviceEnd_P1,PrimeDeviceEndDatalogTestMethod,1092,659976192,665731072,5754880
Base::DeviceStart_P1,PrimeDeviceStartTestMethod,755,665862144,684027904,18165760
Base::LotEnd_P1,PrimeLotEndTestMethod,513,684224512,703455232,19230720
Base::LotStart_P1,PrimeLotStartTestMethod,586,703586304,723169280,19582976
Base::SharedStorageDumpRestore_P1,SharedStorageDumpRestore,529,723300352,742854656,19554304
Functional::AlarmWithRedirectPortDisabled_FNEG2,ProcessFailureWithAlarm,1139,742985728,765313024,22327296
Functional::AlarmWithRedirectPortEnabled_P4,ProcessFailureWithAlarm,13,765444096,765771776,327680
Functional::CaptureCtvPerFailingPatternMultiplePatterns_P1,CaptureCtvPerFailingPattern,20,765902848,766119936,217088
Functional::CaptureCtvPerPatternMultiplePatterns_P1,CaptureCtvPerPattern,17,766251008,766521344,270336
Functional::CaptureCtvPerPattern_P1,CaptureCtvPerPattern,14,766652416,766849024,196608
Functional::CaptureFailures_FailuresToPrintSmallerThanFailuresToPrintPerPattern_FNEG1,PrimeFunctionalTestMethod,860,766980096,789921792,22941696
Functional::CaptureFailures_FiveFailuresButPrintFour_F0,PrimeFunctionalTestMethod,256,789921792,790712320,790528
Functional::CaptureFailures_FiveFailuresButPrintMaxFourWithOnePerPattern_FNEG1,PrimeFunctionalTestMethod,12,790712320,790716416,4096
Functional::CaptureFailures_FiveFailuresButPrintSix_FNEG1,PrimeFunctionalTestMethod,11,790716416,790716416,0
Functional::CaptureFailures_FiveFailuresPrintMaxFourWithTwoPerPattern_F0,PrimeFunctionalTestMethod,12,790716416,790720512,4096
Functional::CaptureFailures_FiveFailuresPrintTwoButMaxPerPatternThree_FNEG1,PrimeFunctionalTestMethod,14,790720512,790720512,0
Functional::CaptureFiveFailuresUpToSecondDomainOnFirstBurst_F0,FunctionalToItuff,19,790720512,790720512,0
Functional::CaptureMoreThanInjectionsAndCaptureEverything_F0,FunctionalToItuff,13,790720512,790720512,0

Functional::PrimeFuncCaptureScoreboardAndCtvPerCycleFuncTest_P1,CtvPerCycleLogger,16,834703360,834973696,270336
Functional::PrimeFuncCaptureScoreboardFuncTest_P1,PrimeFunctionalTestMethod,11,835104768,835366912,262144
Functional::PrimeFuncDcCtv_NoHighLimits_P1,PrimeFuncDcCtvTestMethod,1220,835497984,853098496,17600512
Functional::PrimeFuncDcCtv_NoLimits_P1,PrimeFuncDcCtvTestMethod,13,853229568,858972160,5742592
Functional::PrimeFuncDcCtv_NoLowLimits_P1,PrimeFuncDcCtvTestMethod,12,858972160,858972160,0
Functional::PrimeFuncDcCtv_P1,PrimeFuncDcCtvTestMethod,12,858972160,858972160,0
Functional::PrimeFuncNoCaptureFuncTest_P1,PrimeFunctionalTestMethod,13,858972160,858976256,4096
Functional::StartPatternTryToGetStartPatternWithOutFirstFailures_FNEG1,FunctionalWithStartPattern,21,858976256,859009024,32768
Functional::StartPattern_DisablePreviousStartPattern_F0,FunctionalWithStartPattern,12,859009024,859009024,0
Functional::StartPattern_FirstFailureAmblePatterns_F0,FunctionalWithStartPattern,12,859009024,859009024,0
Functional::StartPattern_FirstFailure_P1,FunctionalWithStartPattern,13,859009024,859009024,0
Functional::StartPattern_TryToGetStartPatternInDisableMode_FNEG1,FunctionalWithStartPattern,12,859009024,859009024,0
Functional::StartPattern_UserDefInition_P1,FunctionalWithStartPattern,12,859009024,859009024,0
SOFTWARETRIGGER::SoftwareTriggerCommonParam_CallbackNoFound_FNEG1,PrimeFunctionalTestMethod,35,859009024,859037696,28672
SOFTWARETRIGGER::SoftwareTriggerCommonParam_ParamIsEmpty_P1,PrimeFunctionalTestMethod,11,859037696,859037696,0
SOFTWARETRIGGER::SoftwareTriggerCommonParam_SetCallbackAndExecute_P1,PrimeFunctionalTestMethod,11,859037696,859037696,0
SOFTWARETRIGGER::SoftwareTriggerCommonParam_VerifyCommonParamFail_FNEG1,PrimeFunctionalTestMethod,14,859037696,859037696,0
===========================================================================

Test instances' Verify - Per test class summary.
===========================================================================
ClassName,AccumulatedElapsedTime(ms),AccumulatedPrivateBytes
PrimeDeviceEndDatalogTestMethod,1092,0
PrimeDeviceStartTestMethod,755,0
PrimeLotEndTestMethod,513,0
PrimeLotStartTestMethod,586,0
SharedStorageDumpRestore,529,0
ProcessFailureWithAlarm,1152,0
CaptureCtvPerFailingPattern,20,0
CaptureCtvPerPattern,31,0
PrimeFunctionalTestMethod,1556,0
FunctionalToItuff,70,0
FunctionalToItuffExtensionsFailing,18,0
EdgeCounter,21,0
FuncWithSoftwareTrigger,20,0
FunctionalServiceCallbacksRegistrar,20,0
FunctionalWithExitPort,27,0
FunctionalWithTail,30,0
CtvPerCycleLogger,46,0
PrimeFuncDcCtvTestMethod,1257,0
FunctionalWithStartPattern,82,0
===========================================================================

VerifyAllInstance total execution time: 10035ms.
VerifyAllInstance total bytes at beginning: 659968000B = 629MB.
VerifyAllInstance total bytes at the end: 859099136B = 819MB.
VerifyAllInstance total bytes difference: 199131136B = 189MB.
```

To enable pattern memory dump as shown below, `PrintPatternMemory` is required to set as True. The printout records per domain size and total pattern size in vectors which trigger during Verify.
```
======================================================================
Pattern Name	Domain	Size (IN VECTORS)
======================================================================
Pattern: fivr_all_pass	Domain: DomainA_All_DPIN_Dig   Size: 913        	Domain: DomainB_All_DPIN_Dig   Size: 1650
Pattern: prescreen_array_bpu_bme_pat	Domain: DomainA_All_DPIN_Dig   Size: 1622       	Domain: DomainB_All_DPIN_Dig   Size: 1622
Pattern: prescreen_array_bpu_bmo_pat	Domain: DomainA_All_DPIN_Dig   Size: 1622       	Domain: DomainB_All_DPIN_Dig   Size: 1622
Pattern: prescreen_array_bpu_glh_pat	Domain: DomainA_All_DPIN_Dig   Size: 1622       	Domain: DomainB_All_DPIN_Dig   Size: 1622
Pattern: raster_pat 	Domain: DomainA_All_DPIN_Dig   Size: 1083       	Domain: DomainB_All_DPIN_Dig   Size: 1083
===== Total memory used per domain ===================================
Total memory vectors=[6862] on domain=[DomainA_All_DPIN_Dig]
Total memory vectors=[7599] on domain=[DomainB_All_DPIN_Dig]
===== Total Memory Used ==============================================
Total memory vectors=[14461]
```

# Datalog output

This Test Method does not create a datalog

# Custom User Code Hooks

**NA**

# TPL Samples

Here are a few test instance examples using the InitializeInstances test method

**TPL Sample1**

```python
Import PrimeInitializeInstancesTestMethod.xml;

Test PrimeInitializeInstancesTestMethod PrimeInitializeInstances
{
   VerifyAll = "True";
}
```
# Test Program Flow order requirements

The image below ilustrates the order requiremetns for Prime instances to correctly <font size="3"><span style="color:OrangeRed">Initialize</font> the libraries.

1. PrimeInitializeLibraryTestMethod
1. PrimeInitializeServicesTestMethod (Aleph)
1. PrimeInitializeInstancesTestMethod

::: mermaid
graph LR
  subgraph Init Subflow
  direction LR
  id2(PrimeInitializeLibraryTestMethod):::PRIME -->
  id3(PrimeInitializeServicesTestMethod):::PRIME -->
  id4(PrimeInitializeInstancesTestMethod):::PRIME
  end
classDef PRIME fill:#005B85;
:::

# Init Optimization

Default callback function will be run to decide if the VerifyAll Prime instances will be skipped or not.
Users can overwrite the default callback function with their custom callback function by defining the uservar name="_UserVars.FORCE_REVERIFY_CALLBACK" with value format "<callback_name>,<callback_argument>".

For example:
UserVars
{
  String FORCE_REVERIFY_CALLBACK = "CustomForceReVerify,FALSE";
}

If the uservars is left empty or not exist in the test program, default callback function will be run to decide whether a forceReverify is needed.

Default callback function:
```python
        public static string PrimeIsForceReverifyNeeded(string args)
        {
            if ((Prime.Base.ServiceStore<IInitService>.Service.GetLastVerifyAllInstancesStatus() == false) ||
                (Prime.Base.ServiceStore<IInitService>.Service.GetLastSucessVerifyAllInstancesLocation() == string.Empty) ||
                (Prime.Base.ServiceStore<IInitService>.Service.GetLastSucessVerifyAllInstancesLocation() != Prime.Base.ServiceStore<IInitService>.Service.GetSavedLocation()) ||
                (Prime.Base.ServiceStore<IInitService>.Service.GetLastSucessVerifyAllInstancesProductName() == string.Empty) ||
                (Prime.Base.ServiceStore<IInitService>.Service.GetLastSucessVerifyAllInstancesProductName() != Prime.Base.ServiceStore<IInitService>.Service.GetSavedProductName()))
            {
                Prime.Base.ServiceStore<IConsoleService>.Service.PrintDebug($"Force ReVerify return TRUE, executing verifyAll instances.");

                return "TRUE";
            }

            Prime.Base.ServiceStore<IConsoleService>.Service.PrintDebug($"Force ReVerify return FALSE, skipping the verifyAll instances.");
            return "FALSE";
        }
```

# Exit Ports

The InitializeInstances test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |
| **N**         | ***Pass/Fail*** | Failing condition            |

# Additional Dependencies

**NA**

# Version tracking

| **Date**       | **Version** | **Author**     | **Comments**    |
| -------------- | ----------- | -------------- | --------------- |
| Apr 5th, 2020  | 1.0.0       | Zamir Zuriel   | Initial version |
| Aug 31st, 2021 | 1.1.0       | Javier Alpizar | Add service files path saving and JSON Schema Validation features |
| Feb 10th, 2022 | 8.1.0       | Lim, Xin Yan   | Enable TorchRulesVar.AlephEnvVariable to support dynamic env var name for aleph files |
| Apr 22nd, 2022 | 9.1.0       | Lim, Xin Yan   | Enable TorchPrimeVar.AlephEnvVariable to support dynamic env var name for aleph files |
| Apr 22nd, 2022 | 9.1.0       | Lim, Xin Yan   | Fix bug for supporting dynamic env var name for aleph files |
| Aug 12, 2022 | 9.1.1 | hramirez | Adding Flow order documentation |
| Oct 4, 2022 | 12.0.0 | Tiow, Hian Seng | Add pattern memory dump feature |
| Nov 1, 2022 | 12.0.0 | Ong, Ping Ping | Add init optimization feature |
| Dec 1, 2022 | 12.0.0 | Tiow, Hian Seng | Add PrintPatternMemory parameter |
| Aug 13, 2024 | 13.01.00 | ckhoh    | Update Flow order documentation |

# Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System