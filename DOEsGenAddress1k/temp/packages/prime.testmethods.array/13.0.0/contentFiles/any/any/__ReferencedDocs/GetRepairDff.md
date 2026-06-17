<h1>Prime Test-Method Specification REP</h1>
Revision 1.0.0

Jan 2024

[[_TOC_]]

## Methodology
GetRepairDff Test Method gets repair compressed data from DFF services and then store its decompressed value into the SharedStorage service.

Decompression is always done using HEX format.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the GetRepairDff test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Default Value** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ | -----|
| RepairDffFile | Yes | File | N/A | N/A | Path to the input file containing the information to be obtained from the DFF. |

## Configuration file
```json
{
  "StorageData": [
    {
      "StorageKey": "LLDROW",
      "DffToken": "LLDROW_DFF"
    }
  ]
}
```

Each attribute of the configuration file is explained in the summary below

- _**StorageData:**_ Grouping field. This attribute groups all the necessary information to share repair data from DFF service to either SharedStorage.
    - _**StorageKey:**_ Required. This token indicates the key under which to store the repair data in either SharedStorage.
    - _**DffToken:**_ Required. This token indicates the key from which to retrieve the repair data from the DFF service.

## TPL Samples
```
Test PrimeGetRepairDffTestMethod GetRepairDFF_GetValueFromDffAndStoreToSharedStorage_P1
{
  RepairDffFile = "~HDMT_TPL_DIR/TestPrograms/Repair/Modules/Repair/InputFiles/GetRepairDff_Basic.json";
}
```

## Exit Ports

The GetRepairDff test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **1**         | ***Pass***      | Passing condition            |