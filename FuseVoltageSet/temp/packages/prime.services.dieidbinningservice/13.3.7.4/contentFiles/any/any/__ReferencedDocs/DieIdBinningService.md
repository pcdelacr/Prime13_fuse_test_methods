[[_TOC_]]
# Overview

The purpose of DieIdBinning Service is to keep tracking of the information required to report a bin number result per each active dieId key name and provide the utilities to handle this tracking, including:

* Setting the list of active die Ids, for which the service will do tracking and report final bin numbers.
* Setting current active die Id(s) to be used by the service.
* Saving of current instance information (flow item name, port(s), and dieId(s)) to the tracking data, used later compute of the final bins.
* Saving custom bins directly to the current or to an specific DieId and add that information to the tracking data.
* Getting bins per die Id based on the stored tracking information.

# Service details
The tracking information is saved in a list per dieId within two types of tracking data structures like the following example:

| dieId name  |            flowIten name            |    port     |
|:-----------:|:-----------------------------------:|:-----------:|
|     U1      | flowItem1<br>flowItem3<br>flowItem4 | 1<br>0<br>2 |
|    U1.1     |     flowItem2<br>flowItem5<br>      |   1<br>1    |
|     U2      |              flowitem8              |      3      |

| dieId name |            flowIten name            | directly assigned bin |
|:----------:|:-----------------------------------:|:---------------------:|
|   U1.U2    |              flowitem6              |          300          |

The data flow items saved for each dieId keep the order in which they are added in this tracking structure because later the bin is decided based on the last flow item that has a bin assigned for the corresponding dieId, either if that bin was directly assigned or obtained though the port.

An item will be added into the tracking structure only if the given dieId is part of the list of active die Ids that must be configured only once per TP load. 

This list will be extracted from the XML file, that must be provided from an UserVar, **__UserVars.DCF_XmlPath_**, and it is read during the execution of the PrimeInitTestMethod:
UserVar example:

```
	String DCF_XmlPath = "~HDMT_TPL_DIR/DieIDBinning/DieIDBinning.xml";
```

XML file content example:

```
	<DESIGNFILE ProductName="Ponte_Vecchio_2-Tile_128GB_Pkg_Assy" AssyNum="K86882" AssyRev="01" SPEEDNum="K42461" SPEEDRev="01" UnitOfMeasurement="UM" PLAVERSION="12.6.0.0">
		<DIE_LEVEL>
			<DIEINFO LEVEL="1" REFDES="U1" DESIGN_DIE_NAME="PONTE_VECCHIO_BASE_A0" ORIGIN_X="14364.5" ORIGIN_Y="0" STEP="A" REV="0" DIE_SIZE="28626,22554.2"/>
			<DIE_LEVEL>
				<DIEINFO LEVEL="2" REFDES="U2" DESIGN_DIE_NAME="FLATTOP_MOUNTAIN_TSD_31_A0_C4" ORIGIN_X="-9225.74" ORIGIN_Y="-9324" STEP="A" REV="0"/>
				<DIEINFO LEVEL="2" REFDES="U3" DESIGN_DIE_NAME="FLATTOP_MOUNTAIN_TSD_31_A0_C4" ORIGIN_X="-9225.74" ORIGIN_Y="9324" STEP="A" REV="0"/>
			</DIE_LEVEL>
		</DIE_LEVEL>
		<DIE_LEVEL>
			<DIEINFO LEVEL="1" REFDES="U10" DESIGN_DIE_NAME="SS_HBM2_HBM2E_SUPERSET_ATTD_DESIGNONLY_A0" ORIGIN_X="-5838.22" ORIGIN_Y="-16382.5" STEP="A" REV="0" DIE_SIZE="11010,10010"/>
		</DIE_LEVEL>
		<DIE_LEVEL>
			<DIEINFO LEVEL="1" REFDES="U2" DESIGN_DIE_NAME="PONTE_VECCHIO_BASE_A0" ORIGIN_X="-14364.5" ORIGIN_Y="0" STEP="A" REV="0" DIE_SIZE="28626,22554.2"/>
			<DIE_LEVEL>
				<DIEINFO LEVEL="2" REFDES="U2" DESIGN_DIE_NAME="FLATTOP_MOUNTAIN_TSD_31_A0_C4" ORIGIN_X="-9225.85" ORIGIN_Y="-9325" STEP="A" REV="0"/>
			</DIE_LEVEL>
		</DIE_LEVEL>
	</DESIGNFILE>
```

The list (comma-separated) example:

```
    "U1:SUBDEVICE:DRS:CPU,U2:SUBDEVICE:DRS:CS"
```

For each item in the comma-separated list, the element before the colon sign is the dieId added to the list of active Ids.

This list can be later overridden from user code using the following service API:
```csharp
    void SetActiveDieIds(IEnumerable<string> activeDieIds);
```


ITUFF printing example by PrimeLotStartDatalogTestMethod from XML file example above,
```
6_ssid_U1
6_sscat_SUBDEVICEGROUP
6_ssprdct_A0
6_ssdesc_PONTE_VECCHIO_BASE_A0
6_ssid_U2
6_sscat_SUBDEVICEGROUP
6_ssprdct_A0
6_ssdesc_PONTE_VECCHIO_BASE_A0
6_ssid_U1.U1
6_sscat_SUBDEVICE
6_ssprdct_A0
6_ssdesc_PONTE_VECCHIO_BASE_A0
6_ssid_U1.U2
6_sscat_SUBDEVICE
6_ssprdct_A0
6_ssdesc_FLATTOP_MOUNTAIN_TSD_31_A0_C4
6_ssid_U1.U3
6_sscat_SUBDEVICE
6_ssprdct_A0
6_ssdesc_FLATTOP_MOUNTAIN_TSD_31_A0_C4
6_ssid_U10
6_sscat_SUBDEVICE
6_ssprdct_A0
6_ssdesc_SS_HBM2_HBM2E_SUPERSET_ATTD_DESIGNONLY_A0
6_ssid_U2.U1
6_sscat_SUBDEVICE
6_ssprdct_A0
6_ssdesc_PONTE_VECCHIO_BASE_A0
6_ssid_U2.U2
6_sscat_SUBDEVICE
6_ssprdct_A0
6_ssdesc_FLATTOP_MOUNTAIN_TSD_31_A0_C4
```

ITUFF printing example by PrimeDeviceEndDatalogTestMethod from XML file example above,
```
3_sscurfbin_U1_9069
3_sscurfbin_U2_9069
3_sscurfbin_U1.U1_9069
3_sscurfbin_U1.U2_9069
3_sscurfbin_U1.U3_9069
3_sscurfbin_U10_9069
3_sscurfbin_U2.U1_9069
3_sscurfbin_U2.U2_9069
```

# Options to add items to the dieId binning tracking
The service and prime infrastructure provide two ways to add items to the tracking information:

## 1. Automatic post instance port store action
To activate this mode following variable must be set in the environment file:
```
POST_INSTANCE_EXECUTE_DIE_ID_BINNING = "ENABLED";
```
If this mode is active and one or more current dieIds are set, each test instance post execute will automatically add the corresponding information (current flow item name and output port for each current dieId) to the tracking structure.

For this mode, the current die Id must be set to one or more of the valid die Ids previously set as active. Following service API can be used to set this current die Id:
```csharp
    void SetCurrentDieId(string dieId);
```
Where dieId string argument can be a comma-separated list.

### integration with DFF currentDieId 
The current die Ids values to be used during the instance post execute operation can be the ones stored using the already mentioned service API (DieIdBinningService.SetCurrentDieId), but if the DFF service current die Id is also already set (to a value not empty and different than "PKG"), that value will be used instead.
DFF dieId can be set using the following API from DffService:
```csharp
    void SetCurrentDieId(string dieId);
```

There is <span style="color: red">limitation</span> from DFF service ```SetCurrentDieId``` where it only allow to set <b>one</b> current die id at a time. The workaround is to use ```SetAdditionalCurrentDieIds``` to set the additional current die ids, additional die ids information will be stored in <b>IP context</b>, and reset during <b>Device Start</b>.
```csharp
    void SetAdditionalCurrentDieIds(IEnumerable<string> dieIds, ISessionContextProviderContainer sessionContext);
```

## 2. Custom user code for port die Id assigning
Assigning the outpot port to the current dieIds, only works for the cases where the result of the instance can be enterely be assigned to those die Ids. For a more flexible assigning of any port (not only the output port) to any dieId, an API is provided to the user to be able to do any mapping of ports to dieIds according to their needs.
```csharp
    void SavePortInformation(string dieId, int port);
```
<DIV style="padding: 15px 35px 15px 15px;margin-bottom: 20px;border: 1px solid rgb(235, 204, 209);color: rgb(169, 68, 66);background-color: rgb(242,222,222);font-family: Verdana, sans-serif;font-size: 15px;font-style: normal;font-weight: 400;letter-spacing: normal;text-align: start;text-indent: 0px;text-transform: none;white-space: normal;word-spacing: 0px">
  <STRONG style="font-weight: bolder">Note!</STRONG><SPAN> </SPAN>
  When using this service API, the current die Id value (from both DFF and the service) must be empty or "PKG" otherwise an error is thrown.
</DIV>

## 3. Custom user code for directly bin dieId assigning
Assigning a custom bin number for a specific dieId.
```csharp
    void SaveBinInformation(string dieId, uint bin);
```    

Assigning a custom bin number for the current dieId.
```csharp
    void SaveBinInformationFromCurrentDieId(uint bin);
```

The custom bin number can be any unsigned integer value.

<DIV style="padding: 15px 35px 15px 15px;margin-bottom: 20px;border: 1px solid rgb(235, 204, 209);color: rgb(169, 68, 66);background-color: rgb(242,222,222);font-family: Verdana, sans-serif;font-size: 15px;font-style: normal;font-weight: 400;letter-spacing: normal;text-align: start;text-indent: 0px;text-transform: none;white-space: normal;word-spacing: 0px">
  <STRONG style="font-weight: bolder">Note!</STRONG><SPAN> </SPAN>
  When using this service APIs, the current dieId value (from both DFF and the service) must be empty or "PKG" otherwise an error is thrown.
</DIV>


# Getting final per dieId bin results
Following API returns a dictionary that contains the final bin for each active die Id based in the stored tracking information already described:
```csharp
    Dictionary<string, uint> GetBinsForAllActiveIds();
```

Using the same example table shown before, if the bin for each of the given ports is as follow:

| dieId name | flowIten name                       | port        | bin                         |
|------------|-------------------------------------|-------------|-----------------------------|
| U1         | flowItem1<br>flowItem3<br>flowItem4 | 1<br>0<br>2 | 100<br>90626235<br>90424253 |
| U1.1       | flowItem2<br>flowItem5<br>          | 1<br>1      | <br>100                     |
| U2         | flowitem8                           | 3           | 90444446                    |
| U1.U2      | flowitem6                           |             | 300                         |

Then, this API would return the following dictionary:
```csharp
    {U1, 90424253}, {U1.U1, 100}, {U2, 90444446}, {U1.U2, 300}
```

# Hybrid mode ituff printing
For hybrid mode (if there are active dieIs set) a callback was implemented to get a formatted string from prime (based on the dictionary returned from GetBinsForAllActiveIds) to be printed by iCBkgndTest at end of the device unit footer ituff print.
Using the same example table shown before and with following active die Ids set:
```csharp
    {U1, U1.U1, U1.U2, U2, U3}
```

Following is the ituff string returned by such cllabck and printed by iCBkgndTest:
```
3_sscurfbin_U1_4253
3_sscurfbin_U1.U1_100
3_sscurfbin_U1.U2_9069
3_sscurfbin_U2_4446
3_sscurfbin_U3_9069
```
Note that:
- If none instance/port has been saved for a given active dieId '9069' is the default bin value to be printed.
- If the final bin id for a given die Id has more than 4 digits, the first 4 digits will be removed from the printed value.
- When using SavePortInformationFromCurrentDieId or SaveBinInformationFromCurrentDieId, if previously current Die Id was not set correctly (die id is not part of Active Die Id, or invalid Die Id), it will fail this execution. This can be reset by setting a valid Die Id again using SetCurrentDieId.

# Version tracking
| Prime version | ww      | prime ticket reference |
|---------------|---------|------------------------|
| 8.2           | 12'2022 | #22575                 |
| 12.0          | 39'2022 | #27726                 |
| 12.0          | 53'2022 | #28911                 |
| 13.1          | 35'2024 | #40501                 |
| 13.3          | 28'2025 | [#60973](https://dev.azure.com/mit-us/PRIME/_workitems/edit/60973/)                |
