<h1>Prime Test-Method Specification REP</h1>

[[_TOC_]]

## Methodology

The `PrimeGetRepairDffTestMethod` retrieves compressed repair data from the DFF service using specified tokens. In this
context, repair data refers to the set of data within an array that are defective and need to be repaired. This data
is then decompressed using the HEX format. Once decompressed, the repair data is stored in the SharedStorage service, 
ensuring it is readily accessible for other test methods and processes.

### Enhanced Filtering Capabilities

The `PrimeGetRepairDffTestMethod` offers advanced filtering options to refine the data retrieval process. These options
are configured through the `RepairDffFile`:
- **Die ID Filtering:** This method supports filtering by die IDs, allowing users to retrieve token data from DFF
  specific to individual dies within a multi-die setup. The input is validated to ensure that the number of die IDs
  matches the number of storage keys. Each storage key corresponds to a specific die ID, and they must align with the
  same index of the die ID they belong to.
- **Operation Type Filtering:** The method enables filtering of DFF data by operation type, allowing users to focus on
  specific operations.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the `PrimeGetRepairDffTestMethod`.

| **Parameter Name** | **Required?** | **Type** | **Values**       | **Comments**                                                                                                                                 |
|--------------------|---------------|----------|------------------|----------------------------------------------------------------------------------------------------------------------------------------------|
| RepairDffFile      | Yes           | File     | File path (JSON) | Specifies the path to a JSON configuration file that contains the parameters for retrieving and processing repair data from the DFF service. |

## Configuration File

The configuration file, specified using the `RepairDffFile` parameter, is a JSON document that defines the parameters for
transferring repair data from the DFF service to SharedStorage. It specifies the necessary tokens, keys, and optional
filters to ensure precise data retrieval and storage.

```json
{
  "StorageData": [
    {
      "StorageKey": "<shared storage key(s) to store dff data>",
      "DffToken": "<dff token to obtain the data from>",
      "DieId": "<die ID(s) to filter dff data>", // Optional
      "OpType": "<operation type to filter dff data>" // Optional
    }
  ]
}
```

Each attribute of the configuration file is explained in the summary below.

- _**StorageData:**_ Grouping field. This attribute consolidates all the necessary information to share repair data from
  DFF service to the SharedStorage.
    - _**StorageKey:**_ Required. Specifies the key(s) under which the repair data will be stored in SharedStorage. If
      die IDs are provided, this can be a comma-separated list of keys, with each key corresponding to a specific die
      ID.
    - _**DffToken:**_ Required. Identifies the token used to retrieve repair data from the DFF service.
    - _**DieId:**_ Optional. Defines the die ID(s) used to filter the DFF data. If omitted, data from the current die ID
      will be accessed. When specified, the number of storage keys must match the number of die IDs, ensuring each
      storage key position aligns with its corresponding die ID.
    - _**OpType:**_ Optional. Indicates the operation type for filtering DFF data. If not specified, data related to the
      current operation type will be retrieved.

### Configuration File Samples

Consider the following data for the sample cases in this section:

| **DffToken** | **DieId** | **OpType** | **DffData**            |
|--------------|-----------|------------|------------------------|
| dffToken1    | dieId1    | opType0    | dataValueA0-compressed |
| dffToken1    | dieId1    | opType1    | dataValueA1-compressed |
| dffToken1    | dieId2    | opType0    | dataValueB0-compressed |
| dffToken1    | dieId2    | opType2    | dataValueB2-compressed |
| dffToken1    | dieId3    | opType0    | dataValueC0-compressed |
| dffToken1    | dieId3    | opType3    | dataValueC3-compressed |

1. Gets repair data from DFF service and stores it in SharedStorage for current die ID and operation type.
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
Current die ID = `dieId1`, current operation type = `opType1`.

DFF data retrieved:  
`dffToken1 = dataValueA1-compressed`

SharedStorage assignment:  
`storageKey1 = dataValueA1-decompressed`

2. Gets repair data from DFF service and stores it in SharedStorage for multiple die IDs and operation type.
```json
{
  "StorageData": [
    {
      "StorageKey": "storageKey1,storageKey2,storageKey3",
      "DffToken": "dffToken1",
      "DieId": "dieId1,dieId2,dieId3",
      "OpType": "opType0"
    }
  ]
}
```
DFF data retrieved:

| **DffToken** | **DieId** | **OpType** | **DffData**            |
|--------------|-----------|------------|------------------------|
| dffToken1    | dieId1    | opType0    | dataValueA0-compressed |
| dffToken1    | dieId2    | opType0    | dataValueB0-compressed |
| dffToken1    | dieId3    | opType0    | dataValueC0-compressed |

SharedStorage assignment:  
```
"storageKey1": "dataValueA0-decompressed"
"storageKey2": "dataValueB0-decompressed"
"storageKey3": "dataValueC0-decompressed"
```

3. Gets repair data from DFF service and stores it in SharedStorage filtered only by die ID.
```json
{
  "StorageData": [
    {
      "StorageKey": "storageKey2",
      "DffToken": "dffToken1",
      "DieId": "dieId2"
    }
  ]
}
```
Current operation type = `opType2`.

DFF data retrieved:  
`dffToken1 = dataValueB2-compressed`

SharedStorage assignment:  
`storageKey2 = dataValueB2-decompressed`

4. Gets repair data from DFF service and stores it in SharedStorage filtered only by operation type.
```json
{
  "StorageData": [
    {
      "StorageKey": "storageKey3",
      "DffToken": "dffToken1",
      "OpType": "opType3"
    }
  ]
}
```
Current die ID = `dieId3`.

DFF data retrieved:
`dffToken1 = dataValueC3-compressed`

SharedStorage assignment:  
`storageKey3 = dataValueC3-decompressed`

5. Failing case: `DieId` field has a value and the number of die IDs does not match the number of storage keys in
   `StorageKeys`.
```json
{
  "StorageList": [
    {
      "StorageKey": "storageKey1,storageKey2",
      "DffToken": "dffToken1",
      "DieId": "dieId1,dieId2,dieId3",
      "OpType": "opType0"
    }
  ]
}
```

SharedStorage assignment:  
No shared storage keys will be created.

Output:  Test method fails. Check `Exit Ports` section to see the exit port for this case.

## TPL Samples
```
CSharpTest PrimeGetRepairDffTestMethod GetRepairDFF_GetValueFromDffAndStoreToSharedStorage_P1
{
  RepairDffFile = GetEnvironmentVariable("~HDMT_TPL_DIR")+"/TestPrograms/Repair/Modules/Repair/InputFiles/GetRepairDff_Basic.json";
}
```

## Exit Ports

The `PrimeGetRepairDffTestMethod` supports the following exit ports:

| **Exit Port**  | **Condition** | **Description**                                                                                                                                  |
|----------------|---------------|--------------------------------------------------------------------------------------------------------------------------------------------------|
| **1**          | ***Pass***    | The retrieval of the DFF data and its registration into the shared storage was successful.                                                       |
| **-1**         | ***Error***   | Any software condition error. `DieId` field was specified, and the number of die IDs does not match the number of storage keys in `StorageKeys`. |

## Version tracking

| **Date**        | **Version** | **Author**            | **Comments**                                                            |
|-----------------|-------------|-----------------------|-------------------------------------------------------------------------|
| June 23rd, 2025 | 13.3.0      | Pinto Rosales, Raquel | Adding capability for filtering DFF data by die IDs and operation type. |
