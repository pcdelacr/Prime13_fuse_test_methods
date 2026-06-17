

[[_TOC_]]

# Prerequisites

<p style="font-size: 20px;"><span style="color: #cf6679;">This Service is using Aleph initialization. Please refer to Aleph documentation</span></p>

[Link to Aleph Documentation](/Learn/PRIME-Features/Aleph)

# Description

The VminForwarding Service provides a read/write infrastructure of resulted voltages, stored under a unique "domain-corner" identifier. Also, the service provides linear calculation support based on the stored values without modify them.
Export data capabilities are implemented at the IVminForwardingExport interface, as also a full history of all changes in a corner is saved and accessible by the user.

# Configuration File

Example file:
```json
{
  "powerDomains": [
    {
      "name": "IA",
      "instances": "IA59,IA58,IA57,IA56,IA55,IA54,IA53,IA52",
      "corners": [
        {
          "name": "F3",
          "frequencySource": {
            "type": "BinMatrix",
            "value": "CR_F3_FREQ"
          },
          "voltageSources": {
			"Dff" : [ "DffKey"] ,
            "cornersWithLinearAdjustment": [
              [ "IA@F1", "IA@F2" ],
              [ "IAX@F8", "IAX@F9" ]
            ],
            "sharedStorage": [ "VminValueSharedStorageKeyIAF3" ]
          },
          "exportStorageVariable": {
            "voltageVariablePrefix": "CURRENT_FAST_VMIN_VCCC_CR_",
            "frequencyVariablePrefix": "CURRENT_FAST_VMINFREQUENCY_CR_"
          }
        },
        {
          "name": "F2",
          "frequencySource": {
            "type": "Literal",
            "value": "CR_F2_FREQ"
          },
          "voltageSources": {
            "corner": [ "IA@F1" ],
            "sharedStorage": [ "VminValueSharedStorageKeyIAF2" ],
            "sharedStorageLimitCheck": "VminValueForCheckSharedStorageKeyIAF2",
            "frequencySwitchAdjustment": "IA@F1"
          },
          "exportStorageVariable": {
            "voltageVariablePrefix": "CURRENT_FAST_VMIN_VCCC_CR_",
            "frequencyVariablePrefix": "CURRENT_FAST_VMINFREQUENCY_CR_"
          }
        },
        {
          "name": "F1",
          "frequencySource": {
            "type": "SharedStorage",
            "value": "CR_F1_FREQ"
          },
          "voltageSources": {
            "sharedStorage": [ "VminValueSharedStorageKeyIAF1" ],
            "sharedStorageLimitCheck": "VminValueForCheckSharedStorageKeyIAF1",
            "frequencySwitchAdjustment": "IA@F2"
          },
          "exportStorageVariable": {
            "voltageVariablePrefix": "CURRENT_FAST_VMIN_VCCC_CR_",
            "frequencyVariablePrefix": "CURRENT_FAST_VMINFREQUENCY_CR_"
          }
        }
      ]
    },
    {
      "name": "IAX",
      "instances": "IAX1,IAX2,IAX3,IAX4,IAX5,IAX6,IAX7,IAX8",
      "corners": [
        {
          "name": "F9",
          "frequencySource": {
            "type": "UserVar",
            "value": "CR_F9_FREQ"
          },
          "voltageSources": {
            "corner": [ "IAX@F8" ],
            "sharedStorage": [ "VminValueSharedStorageKeyIAXF9" ],
            "sharedStorageLimitCheck": "VminValueForCheckSharedStorageKeyIAXF9",
            "frequencySwitchAdjustment": "IA@F8"
          },
          "exportStorageVariable": {
            "voltageVariablePrefix": "CURRENT_FAST_VMIN_VCCC_CR_",
            "frequencyVariablePrefix": "CURRENT_FAST_VMINFREQUENCY_CR_"
          }
        },
        {
          "name": "F8",
          "frequencySource": {
            "type": "BinMatrix",
            "value": "CR_F8_FREQ"
          },
          "voltageSources": {
            "sharedStorage": [ "VminValueSharedStorageKeyIAXF8" ],
            "sharedStorageLimitCheck": "VminValueForCheckSharedStorageKeyIAXF8"
          }
        }
      ]
    }
  ]
}

```

Example file with die notation:
```json
{
  "powerDomains": [
    {
      "name": "IA",
      "instances": "U1:IA[19-0],U2:IA[39-20],U3:IA[59-40]",
      "corners": [
        {
          "name": "F3",
          "frequencySource": {
            "type": "BinMatrix",
            "value": "CR_F3_FREQ"
          },
          "voltageSources": {
            "Dff" : [ "DffKey"] ,
            "cornersWithLinearAdjustment": [
              [ "IA@F1", "IA@F2" ],
              [ "IAX@F8", "IAX@F9" ]
            ],
            "sharedStorage": [ "VminValueSharedStorageKeyIAF3" ]
          },
          "exportStorageVariable": {
            "voltageVariablePrefix": "CURRENT_FAST_VMIN_VCCC_CR_",
            "frequencyVariablePrefix": "CURRENT_FAST_VMINFREQUENCY_CR_"
          }
        },
        {
          "name": "F2",
          "frequencySource": {
            "type": "Literal",
            "value": "CR_F2_FREQ"
          },
          "voltageSources": {
            "corner": [ "IA@F1" ],
            "sharedStorage": [ "VminValueSharedStorageKeyIAF2" ],
            "sharedStorageLimitCheck": "VminValueForCheckSharedStorageKeyIAF2",
            "frequencySwitchAdjustment": "IA@F1"
          },
          "exportStorageVariable": {
            "voltageVariablePrefix": "CURRENT_FAST_VMIN_VCCC_CR_",
            "frequencyVariablePrefix": "CURRENT_FAST_VMINFREQUENCY_CR_"
          }
        },
        {
          "name": "F1",
          "frequencySource": {
            "type": "SharedStorage",
            "value": "CR_F1_FREQ"
          },
          "voltageSources": {
            "sharedStorage": [ "VminValueSharedStorageKeyIAF1" ],
            "sharedStorageLimitCheck": "VminValueForCheckSharedStorageKeyIAF1",
            "frequencySwitchAdjustment": "IA@F2"
          },
          "exportStorageVariable": {
            "voltageVariablePrefix": "CURRENT_FAST_VMIN_VCCC_CR_",
            "frequencyVariablePrefix": "CURRENT_FAST_VMINFREQUENCY_CR_"
          }
        }
      ]
    },
    {
      "name": "IAX",
      "instances": "U1.U2:IAX[19-0],U2.U1:IAX[39-20]",
      "corners": [
        {
          "name": "F9",
          "frequencySource": {
            "type": "UserVar",
            "value": "CR_F9_FREQ"
          },
          "voltageSources": {
            "corner": [ "IAX@F8" ],
            "sharedStorage": [ "VminValueSharedStorageKeyIAXF9" ],
            "sharedStorageLimitCheck": "VminValueForCheckSharedStorageKeyIAXF9",
            "frequencySwitchAdjustment": "IA@F8"
          },
          "exportStorageVariable": {
            "voltageVariablePrefix": "CURRENT_FAST_VMIN_VCCC_CR_",
            "frequencyVariablePrefix": "CURRENT_FAST_VMINFREQUENCY_CR_"
          }
        },
        {
          "name": "F8",
          "frequencySource": {
            "type": "BinMatrix",
            "value": "CR_F8_FREQ"
          },
          "voltageSources": {
            "sharedStorage": [ "VminValueSharedStorageKeyIAXF8" ],
            "sharedStorageLimitCheck": "VminValueForCheckSharedStorageKeyIAXF8"
          }
        }
      ]
    }
  ]
}
```

## Field details
- **powerDomains**: a collection of powerDomain items.
    - **name**: domain name group.
    - **instances**: instances of the domain group. A comma separated list of instance names, the instances can be listed using a extended notation (Example: IA40,IA39,IA38...) or by using a range notation (Example: IA[19-0],IA[39-20]). The instance names must be different than the domain name. If the product requires per die data, the die-instance mapping must be indicated in this section (Example: U1:IA[19-0],U2:IA[39-20],U3:IA[59-40]). Only one type of notation is allowed per configuration file, meaning that all the domains in the file must be defined using die notation, or not using it at all.  
    - **corners**: corner configuration collection. All corner applied to each domain instance.
        - **name**: corner name identifier. Must be unique within all configuration file.
        - **frequencySource**: source object to get the frequency value. This object has two required fields: `type` and `value`. Flow specific value is supported by the index position of the `value`, which means that the value for flow=1 is the first frequency value when `value` field is resolved, and so on.
            - **type**: The source can be `BinMatrix`, `SharedStorage`, `UserVar` ,`Literal` and 'UserVarStringArray'.
            - **value**: The string's format value depends on the source type. For almost all types the value is the respective key or name, for instance, a shared storage key which would contain a comma-separated list of values. But if the type is Literal, the value format is the comma-separated list itself. The shared storage required context is `DUT` from the `string` table.
        - **voltageSources**: voltage sources object to get values for corner. (optional)
            - **corner**: collection of corner identifiers. The position index of current instance will be matched when reading the value. (optional)
            - **cornersWithLinearAdjustment**: collection of corner identifier pairs to get a value from a linear calculation algorithm. (optional)
            - **sharedStorage**: collection of shared storage keys for read voltage values. (optional) *
            - **sharedStorageLimitCheck**: shared storage string key for load check/limit values of voltages to apply in Store and Get operations. (optional) * 
			- **Dff** : dff string key for read voltages values. (optional) *
            - **frequencySwitchAdjustment**: same-instance corner source to calculate via Linear Interpolation/Extrapolation, a voltage value when no data is available for a flow. Used by the Interpolator with the Export Handler.
        - **exportStorageVariable**: shared storage tokens to write storage values of the corner. The corner's name provided by the user is appended to the end of each prefix.(optional)
            -  **voltageVariablePrefix**: shared storage key prefix for voltage value.
            -  **frequencyVariablePrefix**: shared storage key prefix for frequency value.


**\* Shared Storage voltage source values string format:**
1. Single value:
      `0.1`

2. Piped multi-value:       
```
Flow 1                         |             Flow N
0.1,0.1,0.1,0.1,0.1,0.1,0.1,0.1|0.1,0.1,0.1,0.1,0.1,0.1,0.1,0.1
```
        
3. Multi-value:             
```
             Flow
0.1,0.1,0.1,0.1,0.1,0.1,0.1,0.1
```       

4. Piped single-value:    
```
   Flow
0.1|0.1|0.1
```

---

**\* Dff voltage source values string format:**

1. single-value:
	`0.9`

2. Piped multi-value:       
```
Flow 1                         |             Flow N
0.1V0.1V0.1V0.1V0.1V0.1V0.1V0.1|0.1V0.1V0.1V0.1V0.1V0.1V0.1V0.1
```

3. Multi-value:             
```
             Flow
0.1V0.1V0.1V0.1V0.1V0.1V0.1V0.1
```       

4. Piped single-value:    
```
   Flow
0.1|0.1|0.1
```

---
# Data Structures
 
## VminForwardingCornerRecord
  Complete vmin data for a specific corner. 

### Members:
  - Key (string): key identifier for the corner, including the domain prefix and frequency (i.e. : IA45@F4).
  - ActiveCornerData (VminForwardingCornerData): the active data for the corner. Is the last valid storage voltage.
  - CornerData (List<VminForwardingCornerData>): complete history of data stored for the corner. Includes also the "disabled" (-9.999) store calls.

## VminForwardingCornerData
  Corner data saved at the Store call.

### Members:
  - Voltage (double): voltage value.
  - Frequency (double): frequency value.
  - InstanceName (string): instance name executing the store action.
  - Flow (int): flow number.

---
# Service Interfaces

## IVminForwardingService

The main service instance, for operations at service level. It loads the configuration file loading and create handler for feature support.

## Methods

### SetSettingFlag
Sets an operation mode flag value.

```csharp
void SetSettingFlag(OperationModeFlag setting, bool value)
```

***Parameters***
- **setting**: flag type to set.
- **value**: boolean value to set.

The types of ```OperationModeFlag``` flags are:
  - UseLimitCheckAsSource: When enabled the limit/check values are included as voltage sources at the GetSourceVoltages method.
  - UseVoltagesSources: When enabled the corner's voltage values are included in sources at the GetSourceVoltages method.
  - UseLimitCheck: When enabled, the limit/check values are used as limiters in the final results of the StoreVoltages method and GetSourceVoltage method. 
    - StoreVoltages: If a value is higher than the limiting value for any corner inside the current handler, the store operation for all the corners of the handler is not executed and a "false" result is returned.
    - GetSourceVoltages: If a value is higher than the limiting value for a corner inside the current handler, the value of the limiter is used for the corner instead.
  - StoreVoltages: When enabled, the input voltages are confirmed and stored at the StoreVoltages method.

### CreateHandler
Creates a new forwarding table handler with N corner support and 1 flow. There are two options for creating a forwarding handler:

1. Handler with dynamic operation mode flags which are refreshed before EVERY Store() and Get() operations. That method used for creating an handler or set of handlers that has the same synced behavior across the execution. In other words, using that kind handler creation allow users defining a global behavior by SetOperationModeFlags().

```csharp
IVminForwardingHandler CreateDynamicModeHandler(IReadOnlyList<string> cornerNames, int flow, bool includeFailsInGetSourceVoltage, ISessionContextProviderContainer sessionContext);
```

_**Parameters**_
  - **cornerNames**: List of full corner names. (i.e. { "IA54@F3", "IA54@F2" } )
  - **flow**: flow number to use.
  - **includeFailsInGetSourceVoltage**: indicate whether the handler should report failing voltage values (reported as -9.999) when ```GetSourceVoltages``` is called. Otherwise, don't report failing values.

2. Handler with static operation mode flags which are not synced with *SetOperationModeFlags()* changes. That method used for creating an handler with unique behavior. In other words, using that kind of handler creation allow users defining a unique behavior per handler across the execution.

```csharp
IVminForwardingHandler CreateStaticModeHandler(IReadOnlyList<string> cornerNames, int flow, bool includeFailsInGetSourceVoltage, VminForwardingOperationSettingsState operationModeSettings, ISessionContextProviderContainer sessionContext);
```

_**Parameters**_
  - **cornerNames**: List of full corner names. (i.e. { "IA54@F3", "IA54@F2" } )
  - **flow**: flow number to use.
  - **includeFailsInGetSourceVoltage**: indicate whether the handler should report failing voltage values (reported as -9.999) when ```GetSourceVoltages``` is called. Otherwise, don't report failing values.
  - **operationModeSettings** set of flags defining the behavior of the handler. Those flags will not be changed / synced with ```SetOpeartionModeFlags()``` changes.


### CreateExportHandler
Creates a handler for Export operations at service level:

```csharp
IVminForwardingExportHandler CreateExportHandler()
```

### CreateConfigurationHandler
Creates an handler for configuration settings querying.

```csharp
IVminForwardingConfigurationHandler CreateConfigurationHandler()
```

## IVminForwardingHandler
   The interface provides the main methods for store and get voltages for a corner. The order of  corner's values returned is the same order of definition when creating the handler.

## Methods

### StoreVoltages
Stores a list of voltages to the Shared Storage.

```csharp
bool StoreVoltages(IList<double> voltages);
```

_**Result**_ : True if all voltages stored successfully. False if at least one voltage fails a Limit/Check condition and no corner at all is updated.

_**Parameters**_
- **voltages**: a list with voltage values. 
  - The size of the List must be equal to 1 or equal to the quantity of corners loaded for the handler only. 
  - The voltages will be applied to the same index/position of the corner collection defined at the handler "CreateHandler" call.
  - If a negative value is set, an automatic value of -9.999 will be stored.

### GetSourceVoltages
Get the start voltage from the sources defined for the corner, selecting the maximum value of all.

```csharp
IList<double> GetSourceVoltages(IList<double> voltages);
```

_**Result**_ : List of source voltage for each corner.

_**Parameters**_
- **voltages**: a list with voltage values. 
  - The size of the List must be equal to 1 or to the quantity of corners loaded for the handler only. 
  - The voltages will be assigned/compared to the same index/position of the corner collection defined at the handler "CreateHandler" call.
  - The last valid value incremented for the corner will be returned. If a negative/fail value is set and the handler was created with the flag **includeFailsInGetSourceVoltage = true**, a value of -9.999 will be returned.

**Note:** This GetSourceVoltages API by default executes all the calculations to choose the maximum value each time it is called. But if it can be globally configured to do it only if no previous value is stored, if there is a previous value it would be returned without checking all other sources.
The service global configurations must be done through the GlobalSettingsFilePath parameter set up in PrimeInitializeLibraryTestMethod. The configuration keyword to use is Prime.VminForwardingService.GetSourceVoltageOnlyOnce, setting it to either 'Enabled' or 'Disabled'
example:
```
Prime.VminForwardingService.GetSourceVoltageOnlyOnce : Enabled
```

### GetAllCornerData
Get the complete forwarding data for the handler's corners. If one corner has no data, a null is returned at the corner's index position.

```csharp
IList<VminForwardingCornerRecord> GetAllCornerData();
```

_**Result**_ : List of forwarding data for the handler's corners.

### GetFrequencySourceValue
Gets the frequency value for a corner managed by the handler.
```csharp
double GetFrequencySourceValue(string corner);
```

_**Result**_ : List of forwarding data for the handler's corners.

_**Parameters**_
- **corner**: the corner identifier, ie: IA56@F1.
  - The corner must be managed by handler.

## IVminForwardingExportHandler
   The interface provides methods to export corner's data from the VminForwarding Service storage.
## Methods

### GetProcessedCornersData
Returns corner's data saved with StoreVoltages calls for all corners defined at the configuration file.

 ```csharp
 Dictionary<string, Dictionary<string, List<VminForwardingCornerRecord>>> GetProcessedCornersData()
 ```

_**Result**_ : The first key is the domain ("IA") and the second level is the instance or subdomain ("IA53") key. Next is the collection of all corners within the instance (of type VminForwardingCornerRecord).

**VminForwardingCornerRecord** class: stores all the processed information of a corner.
   - **CornerData** ActiveCornerData: last valid data reported for the corner.
   - List<**CornerData**> CornerData: list of all store calls to the corner, including all flows and disabling target calls. If the last element in this list is a 'disabled' target value (-9.999) the corner is considered disabled.

**CornerData** class:
  - double Voltage: voltage value.
  - double Frequency: frequency value.
  - string InstanceName: name of the instance.
  - int Flow: flow number.

### GetVoltagesForFrequencyChange
    Returns the source values for frequency changes for a instance/flow, based on a data snapshot passed as parameter, usually taken from the GetProcessedCornersData method.

 ```csharp
 Dictionary<string, Tuple<double, double>> GetVoltagesForFrequencyChange(string domainName, string instanceName, List<VminForwardingCornerRecord> baseValues, int flow)
 ```

     Remarks:
     - The target frequency must be between the two reference corners frequency values, decribed as: 
         - Current corner value: Reference 1 (Ref1)
         - frequencySwitchAdjustment corner value: Reference 2 (Ref2)
     
     - This condition must be meet to calculate a valid voltage value: Ref1 <= target <= Ref2 or Ref1 >= target >= Ref2. 
     - In case the current corner's value and the reference at frequencySwitchAdjustment does not comply with the rule, the algorithm will jump into the frequencySwitchAdjustment corner, setting it up as the new Reference 1 corner and its corresponding frequencySwitchAdjustment as the new Reference 2 corner, moving both references until the condition is meet, throwing an Exception otherwise.
     - If a corner with the same target frequency to calculate is in the baseValues collection and has a value greater than the calculated, that higher value will be used instead.

   
_**Result**_ : Key is full instance name, i.e. "IA54@F1".

_**Parameters**_
- **domainName** : name of the domain (i.e. "IA").
- **instanceName**: name of the instance/subdomain (i.e. "IA54").
- **baseValues**: instance base values to execute a Linear calculation. The result will be the linear calculation between the base values and the sources defined at frequencySwitchAdjustment parameter at configuration file for the corner/flow. If the corner's current flow is equal to the "flow" parameter, the corner's value is used without any adjustment.
  - If the voltage from the baseValues and the current corner's voltage are equal, the same voltage is used.
- **flow**: flow number to extract.

Warning: If the corner doesn't have a source for frequency change, the method raise an Exception, halting the execution.
   - If a corner with the same target frequency to calculate is inside the baseValues collection and has a value greater than the calculated, that higher value will be used instead.

   **flow**: flow number to extract.
   **allowEmptyCorners**: Enables empty corners (non-tested) to be returned as a disabled value (-9.999) instead.

### UpdateCornerValue
Updates the current voltage, flow and frequency values for a corner. Used solely for "adjustment" operations for further searches.

 ```csharp
 bool UpdateCornerValue(string cornerName, double frequency, double voltage, int flow)
 ```

   
_**Result**_ : Update operation result.

_**Parameters**_

- **cornerName** : full corner's name with instance identifier (i.e. "IA53@F1").
- **frequency**: frequency value.
- **voltage**: voltage value.
- **flow**: flow number.

## IVminForwardingConfigurationHandler
   The interface provides methods for query the service configuration loaded values.

## Methods

### GetSharedStorageLimitCheck
Return the key value for the corner's limit/check voltage source.

```csharp
string GetSharedStorageLimitCheck(string cornerName)
```

_**Result**_ : The shared storage key value loaded from the configuration file for the corner.

_**Parameters**_
- **cornerName**: the corner name without the instance. i.e.: "IA@F5"
  - If the key doesn't exists the method throws an ArgumentException.
  - The return value can be empty or null.
  
### GetDomainNames
Returns a list of all domain names loaded from the configuration file.
```csharp
List<string> GetDomainNames()
```

_**Result**_ : List of domain names.

### GetInstanceNames
Returns a list of all instance names loaded from the configuration file for a domain.

```csharp
List<string> GetInstanceNames(string domainName)
```

_**Result**_ : List of instance names.

_**Parameters**_
  - **domainName**: domain name. i.e.: "IA".
     - If the key doesn't exists the method throws an ArgumentException.
     - The return value can be an empty list.
     - The instance name includes the domain name. i.e.: "IA45".

### GetsCornerNames
Returns a list of all corner names loaded from the configuration file for an instance.

```csharp
List<string> GetsCornerNames(string instanceName)
```

_**Result**_ : List of corner names.

_**Parameters**_
- **instanceName**: Domain instance name. i.e.: "IA45".
   - If the key doesn't exists the method throws an ArgumentException.
   - The return value can be an empty list.
   - The corner name includes the instance name. i.e.: "IA45@F3".
---
# Main usage flow

Next is described the flow to create a service handler:
1. Get the service instance and load the service configuration from file.
2. Specify if check/limit operation modes are user defined.
3. Create the handlers for domain-corner collections.
4. Using the handlers to read/write values.

## 1. Call the main service handler.

The service instance is created by calling an "IVminForwardingService" singleton.

```csharp
IVminForwardingService service = Prime.Base.ServiceStore<IVminForwardingService>.Service;
```

## 2. Set the check/limits operation mode flags (optional).

User can set different operation modes for the StoreVoltages and GetVoltages methods for using the limit/check values:

```csharp
service.SetSettingFlag(OperationModeFlag.UseLimitCheckAsSource, false);

service.SetSettingFlag(OperationModeFlag.UseLimitCheck, false);

service.SetSettingFlag(OperationModeFlag.StoreVoltages, true);

service.SetSettingFlag(OperationModeFlag.UseVoltagesSources, true);
```

## 3. Creating a corner service handler
 Next, a service handler must be created with the corner identifiers and the flow number. This allows the management of the corner's data: 

 **This call should be at Instance's Verify level:**

```csharp
IVminForwardingHandler vminHandler = service.CreateHandler(new List<string>(3) { "IA59@F1", "IA59@F2", "IA59@F3" }, 1);
```

## 4. Calling GetSourceVoltages or StoreVoltages

 **This calls should be at Instance's Execute level:**

```csharp
var initialVoltagesToDoGet = new List<double>(3) { 0.1, 0.1, 0.1 }; 
List<double> cornersSourceVoltages = vminHandler.GetSourceVoltages(initialVoltagesToDoGet);
...
...
...
var voltagesToStore = new List<double>(3) { 0.3, 0.5, 0.6 }; 
bool storeVoltages = vminHandler.StoreVoltages(voltagesToStore);
```

# Export handler usage

## 1. Creating a handler:
```csharp
var forwardingExportHandler = Prime.Base.ServiceStore<IVminForwardingService>.Service.CreateExportHandler();
```

## 2. Get a "snapshot" of current service data.
```csharp
var dataSnapshot = forwardingExportHandler.GetProcessedCornersData();
```

## 3. Manually update a corner's data:
```csharp
var cornerName = "IA56@F7";
var frequency = 1200d;
var voltage = 0.7d;
var flow = 2;

var isCornerUpdated = forwardingExportHandler.UpdateCornerValue(cornerName, frequency, voltage, flow);
```

# Version tracking

| **Date**   | **Prime release** | **Author**    | **Comments** |
| ---------- | ----------- | ------------- | ------------ |
| May, 2021 | 5.01.00       | Javier Alpizar | Add VminForwardingConfigurationHandler.|
| June, 2021 | 5.02.00       | Javier Alpizar | VminForwarding service as a formal Prime Service using Aleph and mockable. |
| Nov 6, 2023 | 12.03.00 | CR Team | Remove old content + improvements to documentation. |
