**Prime Test-Method Specification REP**

Revision 1.0.2

Sep 2020

[[_TOC_]]

## Introduction
The purpose of this test class is to provide sampling based on flows defined by the user. The flow is controlled via the output ports of these SampleRateTest instances. The test class provides sampling control at wafer and unit/die level over a single instance or a group of instances.

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

This methodology can be used in CLASS for DUT level sampling, or in SORT for wafer level sampling.In Wafer level sampling, There are two levels of sampling.The lower level is the DUT level sampling where the DUTs in a particular wafer are sampled,based on the “sample_rate". DUT level sampling will be done on the particular wafer.If a wafer is not to be sampled, then no DUT sampling will be done on the wafer either, and every DUT will exit to the port 2 for that wafer.
If sampling for the unit is to be done , test instance will exit from port 1 otherwise exit from port 2 instead.

Wafer sampling or DUT sampling for the current instance is decided based on the test instance parameters.

The test method supports 4 modes of sample rate, which user can select by setting the SampleOption instance parameter.

### DUT Sample Rate
This sample option is used in CLASS DUT sampling. In this usage mode, the DUT in the lot sampled based on the SampleRateValue instance parameter.If SampleRateValue is set to 3, every 3rd unit will be sampled and exit from default port 1 and unit 1,2 will exit out from default port2. SamplingRateValueparameter needs to be an integer value,and must be specified for this feature. Otherwise it will be an error. Below is the sample usage.


```
Test PrimeSampleRateTestMethod ClassSampleRate_P1
{
   SamplingRateValue = "1";
   WaferSampleList = "";
   SampleOption = "DUT_SAMPLING";
   PrintItuffExitPort = "True";
   LogLevel = "Enabled";
}
```
 

### Wafer Sample Rate
This sample option is used only in SORT (Wafer sampling). In this usage mode, the wafers in a lot are sampled based on the WaferSampleRateValue instance parameter. If WaferSampleRateValue is set to 3 then first 2 wafers will be skipped and all die/unit from the first 2 wafer exit out from default port 2. The 3rd wafer will be sampled according the SamplingRateValue parameter specified. For example, if the WaferSampleRateValue is 3 and  SamplingRateValue specified as 2 then, every 3rd wafer will be sampled and every 2nd DUT/die from the 3rd wafer will be sampled. Both SamplingRateValue and WaferSampleRateValue parameter must be specified for this feature. Otherwise it will be an error. SampleOption must be specified as WAFER_SAMPLING. Below is the sample usage. 


```
Test PrimeSampleRateTestMethod WaferSampleRate_P1
{
   SamplingRateValue = "2";
   SampleOption = "WAFER_SAMPLE_RATE";
   WaferSampleRateValue = "3";
   PrintItuffExitPort = "False";
   LogLevel = "Enabled";
}
```
  
      

### WaferList Sample Rate
This sample option is used only in SORT (Wafer sampling). In this mode, user can specify the desired wafer list to sampled in the WaferSampleList instance parameter. Multiple wafer lists can be specified as comma separated strings. if the current wafer name obtained from SC_LOT_WAFER matches the name specified in the WaferSampleList parameter then that wafer will be sampled according to the SamplingRateValue parameter. For example, if the SC_LOT_WAFER is "123" and Wafer SampleList has values "456","123" and the SamplingRateValue specified as 3 then every 3rd die/unit from the wafer name  "123" will be sampled.

If the user provides an empty value for the WaferSampleList parameter and specify SampleOption as "WAFER_LIST" then wafer list will be obtained from station controller variable SC_WAFER_LIST. if the current wafer name SC_LOT_WAFER matches any of the name from SC_WAFER_LIST then it will be sampled. If both "WaferSampleList" test instance parameter and station controller variable SC_WAFER_LIST is empty then it will be an error. SamplingRateValue parameter must be specified for this feature. SampleOption must be specified as WAFER_LIST. Example usage mode below.


```
Test PrimeSampleRateTestMethod WaferListSampling_P1
{
   SamplingRateValue = "1";
   WaferSampleList = "324,526,123,481";
   SampleOption = "WAFER_LIST";
   PrintItuffExitPort = "True";
   LogLevel = "Enabled";
}
```

### Sample lowest Sample Rate

This feature allow user to sample only the lowest numerical wafer/tray ID, specified by comma-seperated list of ID's, given in either the SampleLowestWaferTrayList parameter or the uservar SCVars.SC_WAFER_LIST (which is the default is if parameter is kept empty).

User input of SampleLowestWaferTrayList parameter are expected to follow such format:
1. [126,124,123,125] - Wafer ID example.
2. [T130,T122,T125,T119] - Tray ID example (note the prefix T).
3. [120T121,121T124,119T115] - Combined Wafer and Tray ID example. The value before T is wafer ID, after T is tray ID.

- Multiple commas, spaces, redundant IDs are allowed, for example [,,,123,124,123,130  ] will be resolve to [123,124,130] during Verify. The SampleLowestWaferTrayList parameter is able to accept strings value from uservar and sharedStorage.
- When SAMPLE_LOWEST option is chosen, either "WAFER_SAMPLING" or "TRAY_SAMPLING" shall be selected using parameter IsSampleLowestTrayType, default is set as false, which represent WAFER_SAMPLING for SAMPLE_LOWEST.
- Test method is expected to resolve respective wafer/tray ID from the input (SampleLowestWaferTrayList), as ID's are expected to be delimited by comma.

Test case explain:

Given SampleLowestValue = 2 and IsSampleLowestTrayType = False, the 2 lowest numerical wafers ID among 4 wafers above will be chosen for sample (eg: 123 & 124).

These 2 IDs will be use to compare against SC_LOT_WAFER (current Wafer ID) to determine if ID exist, and perform DUT sampling.
Otherwise, if ID does not exist in SC_LOT_WAFER, this will be skip and exit from port 2.
The same apply for tray sampling, except tray ID will be use to compare against SC_TRAY_ID.

In case the amount of wafers/trays ID's is less then SampleLowestValue, all the wafers/trays will be sampled. 
Otherwise, only the specified lowest X number of wafers/trays will be sampled.

If SampleLowestValue is set to 0 (zero), sampling is skipped and test method will exit from port 2.

```
Test PrimeSampleRateTestMethod SampleLowestWaferSampling_P2
{
   SamplingRateValue = "2";
   WaferSampleList = "";
   SampleOption = "SAMPLE_LOWEST";
   PrintItuffExitPort = "True";
   SampleLowestWaferTrayList = "123,,124, 127T444,,,  T100";
   SampleLowestValue = "2";
   IsSampleLowestTrayType = "False";
   LogLevel = "Enabled";
}
```

## Test Instance Parameters


| Parameter Name       | Required                    | Type   | Values                                    | Comment                                                                                                                                                             |
|----------------------|-----------------------------|--------|-------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| SamplingRateValue    | yes                         | String | Valid values > 0                          | Specifies the frequency of sampling on a wafer. Setting 1 is 100% sampling. Setting 2 is to sample every second DUT.Setting 3 is sample every 3rd DUT.User can specify the input value through shared storage table or uservar.|
| SampleOption         | No. Default is DUT_Sampling | String | DUT_SAMPLING,WAFER_SAMPLING,WAFER_LIST,SAMPLE_LOWEST | Based on this parameter, it will be decided whether wafer or DUT sampling is to be done.                                                                            |
| WaferSampleRateValue | No                          | String | Valid values >0                           | Specifies the rate at which wafers are to be sampled. Setting to 1: 100%. samplingSetting to 2: sample every second Wafer.Setting to 3: sample every third Wafer.User can specify the input value through shared storage table or uservar.|
| WaferSampleList      | No                          | String | comma separated strings                   | sample option should be WAFER_LIST. Wafername specified here will be sampled. If this field is empty then SC_WAFER_LIST uservar will be used to get the Wafer names. |
| PrintItuffExitPort   | No.Default is "False"       | String | "True" or "False"                         | if it is True exit port information is printed in the ituff.
| ShouldFirstBeSampled   | No.Default is "False"       | String | "True" or "False"                         | If it is True the first unit will be the first sampled. Otherwise, the first DUT to be sampled will be the one with the same number as the Sample Rate. Example, if false and the sample rate is 3, the 3rd unit will be sampled.|
| GlobalSampleRate     | No. Default is "False" | String | "True" or "False" | If true, sampling happen in test-program level, where sampleRate is centralized, else it is test-method level or so called local sampleRate. |
| SampleLowestWaferTrayList | No. | String   | String value comprised of tray or wafer ID. | Specifies list of wafers/trays in strings that use to perform sample lowest sampling. Uservars or sharedStorage value are accepted. If this field is empty, then SC_WAFER_LIST uservar will be used to get the wafer or tray names list. |
| SampleLowestValue | No. | String | Valid values >= 0 | Lowest number of wafers/trays to be sampled. UserVar or sharedStorage value are allowed. No sampling done if set to zero.|
| IsSampleLowestTrayType | No. Default is False. | String | "True" or "False" | Determine either "WAFER_SAMPLING" (False) or "TRAY_SAMPLING" (True) during SAMPLE_LOWEST option. |

**Sampling rate value through shared storage table or Uservar**

Input parameters "SamplingRateValue", "WaferSampleRateValue" and "SampleLowestValue" can take input value from shared storage table or uservar.
sample rate input value will be searched in the below order. if it is not specified in both places then value will be directly obtained from the test instance.
1) Shared storage table 
2) Uservar

In the below example it gets the sampling rate value from userVars collection. It can be defined in any uservar collection. It has to be defined like this in the uservar file.  

Note: 
- This value needs to defined as integer otherwise the value will not retrieved.
- If UserVar is defined in the module level, the uservar name needs to be preceded by the module scope i.e. "ModuleName::CollectionName.UserVarName"
- The user var will be read the first time the instance is executed only so its value must be set before this happens.

...
UserVars
{
	Integer DUTSamplingRateValue = "1";
}
...



Test PrimeSampleRateTestMethod DUTSamplingFromUserVar_P1
{
   SamplingRateValue = "_UserVars.DUTSamplingRateValue";
   SampleOption = "DUT_SAMPLING";
   ShouldFirstBeSampled = "False";
   PrintItuffExitPort = "False";
   LogLevel = "PRIME_DEBUG";
}

Below example show the SamplingRateValue defined in the shared storage table.  
"SamplingRateValueKey" must be defined as integer prior using the InsertRowAtTable Prime Service.
Prime.Services.SharedStorageService.InsertRowAtTable("SamplingRateValueKey", 1, Context.LOT);.

Test PrimeSampleRateTestMethod SamplingRateValueFromKey_P1
{
   SamplingRateValue = "SamplingRateValueKey";
   SampleOption = "DUT_SAMPLING";
   ShouldFirstBeSampled = "False";
   PrintItuffExitPort = "False";
   LogLevel = "PRIME_DEBUG";
}

**Global Sample Rate**

When global sampleRate is enabled, sampling is done when DeviceStartSetup is executed instead of SampleRateTestMethod.
Hence, all sampleRate instances within the test program will use this as central sampleRate value.

For example:

Consider a scenario where SampleRateTestMethod is setup with globalSampleRate = Enabled and sampleRate = 3. 
The sampleRate value(3) is then stored into sharedStorage. Unlike local sampleRate, sampling would not happen until start of device initiated.
In this case with two times start of device in between sampleRateTestMethod, unit will be sample.
- SampleRateTestMethod(3) -> start of device(2) -> SampleRateTestMethod(2) -> start of device(1) -> SampleRateTestMethod(1 - sample) 

Multiple sampleRate will not be allowed for different sample rate test instances if global sampleRate is enabled.

For example:
- SampleRateTestMethod_A (sampleRate value = 3) -> SampleRateTestMethod_B (sampleRate value = 7) -> SampleRateTestMethod_C (sampelRate value = 10)

SampleRate value 7 & 10 would not be taken into account, test program will execute sampleRate using sampleRate value = 3 in this case.

Global sampleRate value is being stored into sharedStorage and will only be cleared during test program reload. 

## Datalog output
If PrintItuffExitPort test instance parameter is "True" then Test instance will print the exit port information in ituff. Otherwise it will not print the information. Ituff information format is 
...
2_tname_<Test Instance Name>_EDC
2_mrslt_<Test Instance exit port>
..

Example 1: 
..
2_tname_SampleRate::ClassSampleRate_P1_EDC
2_mrslt_1.0000
..

Example 2
..
2_tname_SampleRate::WaferListSampling_P1_EDC
2_mrslt_2.0000
..

## TPL Samples

Example 1: Samplerate instance for CLASS. This instance sample all the units as the samplingRatevalue is 1. Print the exit port information in ituff.

```
Test PrimeSampleRateTestMethod ClassSampleRate_P1
{
   SamplingRateValue = "1";
   WaferSampleList = "";
   SampleOption = "DUT_SAMPLING";
   PrintItuffExitPort = "True";
   LogLevel = "Enabled";
}
```


Example 2: Samplerate instance for CLASS. This instance sample every 2nd unit. Print the exit port information in ituff.(ie) It will sample unit 2,4,6,8,etc and exit from port 1. DUT's 1,3,5,7, etc exit from port 2 hence it is not sampled.

```
Test PrimeSampleRateTestMethod ClassSampleRate_P1
{
   SamplingRateValue = "2";
   WaferSampleList = "";
   SampleOption = "DUT_SAMPLING";
   PrintItuffExitPort = "True";
   LogLevel = "Enabled";
}
```


Example 3: Sample rate instance for SORT(WAFER). If the current wafer name extracted from SC_LOT_WAFER is "324,526,123,481" then it will process the wafer and DUTs from it and exit to port 1 otherwise exit to port 2.

```
Test PrimeSampleRateTestMethod WaferListSampling_P1
{
   SamplingRateValue = "1";
   WaferSampleList = "324,526,123,481";
   SampleOption = "WAFER_LIST";
   PrintItuffExitPort = "True";
   LogLevel = "Enabled";
}
```


Example 4: Sample rate instance for SORT(WAFER). Here WaferSampleList is empty.So the wafer name will be obtained SC_WAFER_LIST uservar. If the current wafer name extracted from SC_LOT_WAFER is  matches any of the wafer name in SC_WAFER_LIST then it will process the wafer and DUTs from it and exit to port 1 otherwise exit to port 2.

```
Test PrimeSampleRateTestMethod WaferListSampling_P1
{
   SamplingRateValue = "1";
   WaferSampleList = "";
   SampleOption = "WAFER_LIST";
   PrintItuffExitPort = "True";
   LogLevel = "Enabled";
}
```


Example 5: Sample rate instance for SORT(WAFER). This instance will sample every 2nd wafer and every 3rd DUT in the wafer and exit from port 1. All the DUT from wafer 1 and wafer 3 etc exit from port 2.  DUT 1,3,5 etc from wafer 2,wafer 4 also exit from port 2.

```
Test PrimeSampleRateTestMethod WaferSampleRate_P2
{
   SamplingRateValue = "3";
   WaferSampleList = "324,526,123,481";
   SampleOption = "WAFER_SAMPLING";
   WaferSampleRateValue = "2";
   PrintItuffExitPort = "False";
   LogLevel = "Enabled";
}
```


Example 6: Sample rate instance for CLASS(DUT). This instance will sample every 3rd dut exit from port 2, with usercode example shown above. Without usercode, this instance will always exit from port 2.

```
Test PrimeSampleRateTestMethod GlobalClassSampleRate1st_P2
{
   SamplingRateValue = "3";
   SampleOption = "DUT_SAMPLING";
   PrintItuffExitPort = "True";
   LogLevel = "Enabled";
   GlobalSampleRate = "True"
}
```

Example 7: Sample rate instance for SORT(WAFER). This instance will sample all DUTs, with lowest 1 wafer among wafer listed in the SampleLowestWaferTrayList will be sample, this instance will exit port 1 for the first time, and exit port 2 when wafer change.

```
Test PrimeSampleRateTestMethod SampleLowestWaferSampling_P1P2
{
   SamplingRateValue = "1";
   WaferSampleList = "";
   SampleOption = "SAMPLE_LOWEST";
   PrintItuffExitPort = "True";
   SampleLowestWaferTrayList = "456,,460, 462T444,,, ";
   SampleLowestValue = "1";
   IsSampleLowestTrayType = "False";
   LogLevel = "Enabled";
}
```

Example 8: Sample rate instance for SORT(WAFER). This instance will sample all DUTs, with lowest 2 wafers of the list. As SampleLowestWaferTrayList is empty, and SC_WAFER_LIST is empty, this instance will exit port -1 for first time. Once it is defined, it will exit port 1.

```
Test PrimeSampleRateTestMethod SampleLowestWaferSampling_EmptyWaferTrayList_FNEG1P1
{
   SamplingRateValue = "1";
   WaferSampleList = "";
   SampleOption = "SAMPLE_LOWEST";
   PrintItuffExitPort = "True";
   SampleLowestWaferTrayList = "";
   SampleLowestValue = "2";
   IsSampleLowestTrayType = "False";
   LogLevel = "Enabled";
}
```
Example 9: Sample rate instance for SORT(WAFER). This instance will sample all DUTs, with lowest 1 wafers of the list. When sampleLowestValue is the same or larger than list of item in SampleLowestWaferTrayList, all the wafer will be sample, this instance will exit port 1.

```
Test PrimeSampleRateTestMethod SampleLowestWaferSampling_EqualList_P1
{
   SamplingRateValue = "1";
   WaferSampleList = "";
   SampleOption = "SAMPLE_LOWEST";
   PrintItuffExitPort = "True";
   SampleLowestWaferTrayList = "456";
   SampleLowestValue = "1";
   IsSampleLowestTrayType = "False";
   LogLevel = "Enabled";
}
```

Example 10: Sample rate instance for SORT(WAFER). This instance will sample all DUTs, with lowest 1 wafers of the list. If IDs listed in SampleLowestWaferTrayList doe snot exist in SC_LOT_WAFER, this instance will be skipped and exit port 2.

```
Test PrimeSampleRateTestMethod SampleLowestWaferSampling_SkipIdNotMatch_P2
{
   SamplingRateValue = "1";
   WaferSampleList = "";
   SampleOption = "SAMPLE_LOWEST";
   PrintItuffExitPort = "True";
   SampleLowestWaferTrayList = "198,199";
   SampleLowestValue = "1";
   IsSampleLowestTrayType = "False";
   LogLevel = "Enabled";
}
```

Example 11:  Sample rate instance for SORT(WAFER). This instance will sample all DUTs, with lowest 2 trays of the list. Tray id will be breakdown after letter 'T', hence in tray type only T100, T102 and T104 will be picked. Given the same ID exist in SC_TRAY_ID, this instance shall exit port 1 upon all executes.

```
Test PrimeSampleRateTestMethod SampleLowestTraySampling_P1P1
{
   SamplingRateValue = "1";
   WaferSampleList = "";
   SampleOption = "SAMPLE_LOWEST";
   PrintItuffExitPort = "True";
   SampleLowestWaferTrayList = " 462T102,,,  T100, 499T104";
   SampleLowestValue = "2";
   IsSampleLowestTrayType = "True";
   LogLevel = "Enabled";
}
```

## Exit Ports

The Sample Rate test method supports the following exit ports:


|****Exit** Port**|**Condition**  |**Description**  |
|--|--|--|
| -2 |Alarm  |Any Alarm Condition  |
|  -1|Error  |Any software error  |
| 0 | Fail | Invalid test parameters, test instance Error |
| 1 | Pass | DUT(No Skip) sampling to be done |
| 2 | Pass  |DUT(Skip) sampling not to be done  |

**Acronyms**
Definition of acronyms used in this document:

**REP**: Prime Test-Method Specification.
**DUT**: Device Under Test.
**TPL:** Test Programming Language

## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**    |
| ------------------------- | ----------- | ----------------- | --------------- |
| Seb 27<sup>st</sup>, 2020 | 1.0.0       | Feroz,Ahamed| Initial version |
| Mar 28<sup>th</sup>, 2023 | 1.0.1 | Tiow, Hian Seng | Added global sampleRate |
| Sep 4<sup>th</sup>, 2023 | 1.0.2 | Tiow, Hian Seng | Added SAMPLE_LOWEST mode |