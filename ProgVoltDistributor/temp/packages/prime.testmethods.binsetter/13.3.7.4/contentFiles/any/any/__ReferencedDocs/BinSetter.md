<h1>Prime Test-Method Specification REP</h1>
Revision 1.0.0

December 2022

[[_TOC_]]

## Methodology
The Prime BinSetter test method gets the BinSetterPath based on the TRACE program, and uses the data obtained via TOS Flow Trace to filter. Generally it filters the Exit port, and the SetBin of all run instances. Instead of getting the last BinSetterPath from TRACE, this test method is able to get multiple BinSetterPaths, either from the first occurance forwards or the last occurance backwards, and print them to ituff.

The test method prints a bin and the corresponding path to the instance to a strgval format. The “|” character will be printed in the case of more than one BinSetter. Other than that, the postfix of the tname will show what setting the current test method had. Details under datalog output.

The test method allows user to find the BinSetterPath from direction:
1. "First" of matching instances
2. "Last" of matching instances

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the BinSetter test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Default Value** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ | -----|
| ReadDirectionSetting| No | Choice |    First, Last       | Last|  |
| NumberOfFailingPath| No | Integer |     Any positive whole number       | 1| |

**Notes:**
- For console prints, LogLevel must be set to "Enabled"
- The test method will fail if PrimeLotStartDatalog runs with _UserVars.ENABLE_PRIME_BRITA = 'FALSE'.
- The test method will fail if PrimeDeviceStartSetup runs with _UserVars.ENABLE_PRIME_BRITA = 'FALSE'.
- If the instance cannot find the BinSetterPath but fit all the condition on exit port and Set Bin, it will flag out at console log and print only instance name in the ituff.
- All FatalExceptions will exit port 0.
- Test method will exit port 1 and pass if all the ran instances are exiting port 1, or exiting other ports without Setting any Bin.
## Datalog output

ituff output:
```
2_tname_BinSetterPath_< *ReadDirectionSetting* ><br>
2_strgval_< *Bin* >_< *BinSetterPath* >
```
if the NumberOfFailingPath is more than 1:
```
2_tname_BinSetterPath_<*ReadDirectionSetting* >< *NumberOfFailingPath* >
2_strgval_< *Bin* >_< *BinSetterPath* >
```
Example:
```
2_tname_BinSetterPath_Last
2_strgval_b6901_FAIL_ERROR_MAIN/BinSetter_TestMethods/BinSetter_MiniFlow/BinSetter_MiniFlow1/DirectExitPort_F0

2_tname_BinSetterPath_First3
2_strgval_b103_PASS_DeviceStartDirectExitPort_P3|b6901_FAIL_ERROR_MAIN/BinSetter_TestMethods/BinSetter_NoBrita_F0|b90989899_FAIL_SOFTWARE_ALARM_MAIN/BinSetter_TestMethods/DirectExitPort_FNeg1
```

## Custom User Code Hooks
N/A

## TPL Samples
**TPL Sample1:**

only 1 BinSetter from Last Direction (default configuration):
```python
Import PrimeBinSetterTestMethod.xml;

Test PrimeBinSetterTestMethod BinSetter_Last_P1
{
}
```

**TPL Sample2:**

expecting a custom number of BinSetter from First Direction:
```python
Import PrimeBinSetterTestMethod.xml;

Test PrimeBinSetterTestMethod BinSetter_First_20_P1
{
    NumberOfFailingPath = 20;
    ReadDirectionSetting = "First";
	  LogLevel = "Enabled";
}
```

## Exit Ports

The CurrentDieIdManager test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

  
## Additional Dependencies

N/A

## Version tracking

| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| December, 2022 | 1.0.0       | Lee, Yeong Jui|              |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System