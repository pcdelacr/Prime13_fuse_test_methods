<h1>Prime Test-Method Specification REP</h1>

[[_TOC_]]

## Methodology

The `PrimeSetRepairDffTestMethod` accesses decompressed repair data from the SharedStorage service using specified keys.
In this context, repair data refers to the set of data within an array that are defective and need to be repaired.
This data is subsequently compressed into the HEX format. After compression, the repair data is stored in the DFF
service, ensuring efficient data management and accessibility.

### Enhanced Registering Capabilities

The `PrimeSetRepairDffTestMethod` provides advanced options for registering data. These options are configured through 
the `RepairDffFile`:

- **Die ID Registering:** This method facilitates the registration of data by die IDs, enabling users to set token data
  in the DFF specific to individual dies within a multi-die setup. The input is validated to ensure that the number of
  die IDs matches the number of storage keys. Each storage key corresponds to a specific die ID, and they must align 
  with the same index of the die ID they belong to.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the `PrimeSetRepairDffTestMethod`.

| **Parameter Name** | **Required?** | **Type** | **Values**       | **Comments**                                                                                                                                  |
|--------------------|---------------|----------|------------------|-----------------------------------------------------------------------------------------------------------------------------------------------|
| RepairDffFile      | Yes           | File     | File path (JSON) | Specifies the path to a JSON configuration file that contains the parameters for processing and registering repair data into the DFF service. |

## Configuration file

The configuration file, specified using the RepairDffFile parameter, is a JSON document that defines the parameters for
transferring repair data from SharedStorage to the DFF service. It specifies the necessary tokens, keys, and optional
identifiers to ensure precise data registration and storage.

```json
{
  "StorageData": [
    {
      "StorageKey": "<shared storage key(s) to obtain the data from>",
      "DffToken": "<dff token to store the dff data>",
      "DieId": "<die ID(s) to register dff data>" // Optional
    }
  ]
}
```

Each attribute of the configuration file is explained in the summary below.

- _**StorageData:**_ Grouping field. This attribute consolidates all the necessary information to share repair data from
   SharedStorage to the DFF service.
    - _**StorageKey:**_ Required. Specifies the key used to retrieve repair data from SharedStorage. If die IDs are 
      provided, this can be a comma-separated list of keys, with each key corresponding to a specific die ID.
    - _**DffToken:**_ Required. Defines the token under which the compressed repair data is stored in the DFF service.
    - _**DieId:**_ Optional. Identifies the die ID(s) used to register the DFF data. If omitted, data will be registered
      for the current die ID. When specified, the number of storage keys must match the number of die IDs, ensuring each
      storage key position aligns with its corresponding die ID.

### Configuration File Samples

1. Gets repair data from SharedStorage service and stores it in DFF for current die ID.
```json
{
  "StorageData": [
    {
      "StorageKey": "storageKey1",
      "DffToken": "dffToken1"
    }
  ]
}
```
Current die ID = `dieId1`.

SharedStorage data retrieved:  
`storageKey1 = dataValueA1-decompressed`

DFF assignment:  

| **DffToken** | **DieId** | **DffData**            |
|--------------|-----------|------------------------|
| dffToken1    | dieId1    | dataValueA1-compressed |

2. Gets repair data from SharedStorage service and stores it in DFF for multiple die IDs.
```json
{
  "StorageData": [
    {
      "StorageKey": "storageKey1,storageKey2,storageKey3",
      "DffToken": "dffToken1",
      "DieId": "dieId1,dieId2,dieId3"
    }
  ]
}
```

SharedStorage data retrieved:
```
"storageKey1": "dataValueA1-decompressed"
"storageKey2": "dataValueB2-decompressed"
"storageKey3": "dataValueC3-decompressed"
```

DFF assignment:

| **DffToken** | **DieId** | **DffData**            |
|--------------|-----------|------------------------|
| dffToken1    | dieId1    | dataValueA1-compressed |
| dffToken1    | dieId2    | dataValueB2-compressed |
| dffToken1    | dieId3    | dataValueC3-compressed |

3. Failing case: `DieId` field has a value and the number of die IDs does not match the number of storage keys in
   `StorageKeys`.
```json
{
  "StorageList": [
    {
      "StorageKey": "storageKey1,storageKey2,storageKey3",
      "DffToken": "dffToken1",
      "DieId": "dieId1,dieId2"
    }
  ]
}
```

DFF assignment:  
No value will be registered for the DFF token.

Output:  Test method fails. Check `Exit Ports` section to see the exit port for this case.

## TPL Samples
```
CSharpTest PrimeSetRepairDffTestMethod SetRepairDFF_SharesValueFromSharedStorageToDFF_P1
{
  RepairDffFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Repair/Modules/Repair/InputFiles/SetRepairDff_Basic.json";
}
```

## Exit Ports

The `PrimeSetRepairDffTestMethod` supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                                                                                                  |
| ------------- |---------------|--------------------------------------------------------------------------------------------------------------------------------------------------|
| **1**         | ***Pass***    | The retrieval of the shared storage value(s) and its/their registration into the DFF data was successful.                                        |
| **-1**        | ***Error***   | Any software condition error. `DieId` field was specified, and the number of die IDs does not match the number of storage keys in `StorageKeys`. |

## Version tracking

| **Date**        | **Version** | **Author**            | **Comments**                                           |
|-----------------|-------------|-----------------------|--------------------------------------------------------|
| June 23rd, 2025 | 13.3.0      | Pinto Rosales, Raquel | Adding capability for registering DFF data by die IDs. |


