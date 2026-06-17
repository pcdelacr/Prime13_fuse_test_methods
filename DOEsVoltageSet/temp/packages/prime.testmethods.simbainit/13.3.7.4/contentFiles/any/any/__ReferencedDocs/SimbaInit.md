<h1>Prime Test-Method Specification REP</h1>
Revision 1.1.0

August 2022

[[_TOC_]]

## Methodology
The SimbaInit test method supports the Unit Level Adaptive Test (ULAT) methodology to bypass modules and test instances at a lot or unit level.
The main activities are:

1. Take input arguments to setup Simba execution.
2. Store Simba data from ULAT file into Shared Storage for lookup during instance execution. 

The necessary data is read from the ULAT file during the SimbaInit test method execution.

## Test Instance Parameters
| **Parameter Name**       | **Required?** | **Type** | **Values**                        | **Comments**                  |
| ------------------------ | ------------- | -------- |-----------------------------------| ----------------------------- |
| UlatEnableMode           | Yes           | String   | Off/On                            | Replaces SC_ULAT_MODE uservar |
| UlatFileType             | Yes           | String   | Lot/Unit                          | Replaces SC_ULAT_TYPE uservar |
| UnitIdentificationMethod | Yes           | String   | Vid/Ult/VidAndUlt                 |                               |
| SsidKeyForUltData        | No            | String   | Empty string (default)            | Used to access ULT/eFuse data |
| UlatFilePath             | No            | String   | A path to a engineering Ulat file | This parameter has presedency over the SCVars.SC_ULAT_FILE, and only if it's empty Simba will use the SCVars.SC_ULAT_FILE as the path to the ULAT file. |

**Notes:**
- This test methods instance must be included towards the end of the INIT flow, after the Init test method instance.

- The input XML file will be parsed during the SimbaInit instance by querying the global SCVars.SC_ULAT_FILE, to allow edits from external scripts.

- NOTE that this input file will likely switch back to a JSON format at a later time.

- ULAT File Examples:
```python
LOT format

<?xml version="1.0" encoding="utf-8"?>
<Simba_ULAT xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="Simba.schema.xsd">
  <FileRevision>BXT_B1_00E00_01</FileRevision>
  <ConfigSets>
    <ConfigSet id="Config_1">
      <Bypass name="CACHSA::I1"/>
      <Bypass name="CACHSA::I2"/>
    </ConfigSet>
    <ConfigSet id="Config_2">
      <Bypass name="CACHSA::*"/>
    </ConfigSet>
  </ConfigSets>
  <ProcessTypes>
    <ProcessType name="CLASSHOT">
      <Lot ConfigSet="Config_1,Config_2" Mode="SKIP,TEST" />
    </ProcessType>
    <ProcessType name="CLASSCOLD">
      <Lot ConfigSet="NA" />
    </ProcessType>
  </ProcessTypes>
</Simba_ULAT>

UNIT format

{
<?xml version="1.0" encoding="utf-8"?>
<Simba_ULAT xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="Simba.schema.xsd">
  <FileRevision>BXT_B1_00E00_01</FileRevision>
  <ConfigSets>
    <ConfigSet id="Config_1">
      <Bypass name="CACHSA::I1"/>
      <Bypass name="CACHSA::I2"/>
    </ConfigSet>
    <ConfigSet id="Config_2">
      <Bypass name="CACHSA::*"/>
    </ConfigSet>
  </ConfigSets>
  <ProcessTypes>
    <ProcessType name="CLASSHOT">
      <Unit VID="75TC444601336" ULT="D425847C_496_+02_+00" ConfigSet="Config_1,Config_2" Mode="SKIP,TEST" />
      <Unit VID="75TC444601338" ULT="D425847C_496_-02_+02" ConfigSet="Config_2" Mode="SKIP" />
      <Unit VID="75TC444601340" ULT="D425847C_496_-02_+04" ConfigSet="NA" Mode="SKIP" />
    </ProcessType>
    <ProcessType name="CLASSCOLD">
      <Unit VID="75TC444601336" ULT="D425847C_496_+02_+00" ConfigSet="Config_1" />
      <Unit VID="75TC444601338" ULT="D425847C_496_-02_+02" ConfigSet="Config_2" />
      <Unit VID="75TC444601340" ULT="D425847C_496_-02_+04" ConfigSet="NA" />
    </ProcessType>
  </ProcessTypes>
</Simba_ULAT>
```

- ULAT File Details:
1. Either LOT or UNIT element are allowed in a single file.
2. Bypass tags hold the wildcard names of the wanted bypassed test instances.
3. ConfigSets holds these Bypass names under ID tags, which are selected per ProcessType. 
4. Under ProcessTypes, there must be an equal number of comma-separated ConfigSet and Mode items.
5. Mode values can be SKIP or TEST, and is optional (TEST is default).
6. If ConfigSet is "NA", no bypassing is done.
7. VID and ULT values must be included, and unique.
8. Any called ConfigSet or ProcessType names or IDs must be defined and unique.
9. Element Bypass can have two formats: `ModuleName::InstanceName` or `ModuleName::*`.

## Datalog output
While this testmethod does not directly output to ITUFF, below is an example of when an instance is bypassed by SIMBA.
```python
LOT format

4_tsattrs_ULAT_FILE_PATH,D:\PRIME_12_23_Simba\validation\regressionTps\hdmt\Modules\SIMBA\SIMBA\InputFiles\SimbaService_Lot_Regression.xml
4_tsattrs_ULAT_FILE_REV,TST_A1_00A00_01
4_tsattrs_ULAT_MODE,PROD
4_tsattrs_ULAT_TYPE,LOT
4_tsattrs_MAESTRO_CONFIGSET,Config_1,Config_2
4_tsattrs_MAESTRO_MODE,SKIP,TEST
3_lbeg
2_tname_Simba::ExecuteInstanceTwo_P1
2_comnt_SIMBA_BYPASSED

UNIT format

4_tsattrs_ULAT_FILE_PATH,D:\PRIME_12_23_Simba\validation\regressionTps\hdmt\Modules\SIMBA\SIMBA\InputFiles\SimbaService_Unit_Regression.xml
4_tsattrs_ULAT_FILE_REV,TST_A2_00A00_02
4_tsattrs_ULAT_MODE,PROD
4_tsattrs_ULAT_TYPE,UNIT
3_lbeg
2_lsep
2_tname_Simba::PrimeULTDecode_P1
2_tssid_U4
2_strgalt_nsv_Y5106803_330_-10_+02
2_tname_Simba::ExecuteInstanceOne_P1_MAESTRO_CONFIGSET
2_strgval_Config_1,Config_2
2_tname_Simba::ExecuteInstanceOne_P1_MAESTRO_MODE
2_strgval_SKIP,TEST
2_tname_Simba::ExecuteInstanceOne_P1
2_comnt_SIMBA_BYPASSED

```

## Custom User Code Hooks
N/A

## TPL Samples
```python
Import PrimeSimbaInitTestMethod.xml;

Test PrimeSimbaInitTestMethod SimbaInitLot_P1
{
    UlatEnableMode = "Enabled";
    UlatFileType = "Lot";
    UnitIdentificationMethod = "VidAndUlt";
}

Test PrimeSimbaInitTestMethod SimbaInitUnit_P1
{
    UlatEnableMode = "Enabled";
    UlatFileType = "Unit";
    UnitIdentificationMethod = "Vid";
    SsidKeyForUltData = "PKG";
}
```

## Exit Ports
The SimbaInit test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |
  
## Additional Dependencies
In Hybrid test programs, this PRIME method will ignore any EVG test instances listed in the ULAT file.  Same goes for the EVG method and PRIME instances.
Should there be both EVG and PRIME instances to be bypassed, **both** EVG and PRIME Simba methods must be included.

## Version tracking
| **Date**       | **Version** | **Author**     | **Comments**                                      |
| -------------- | ----------- | -------------- | ------------------------------------------------- |
| January 2022   | 1.0.0       | Kevin D. Krake | Initial release                                   |
| June 2022      | 1.1.0       | Kevin D. Krake | Switch from JSON to XML format                    |
| August 2022    | 1.1.1       | Kevin D. Krake | Switch from Aleph to global uservar for ULAT file |
|                |             |                |                                                   |

## Acronyms
Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **DFF**: Data Feed Forward