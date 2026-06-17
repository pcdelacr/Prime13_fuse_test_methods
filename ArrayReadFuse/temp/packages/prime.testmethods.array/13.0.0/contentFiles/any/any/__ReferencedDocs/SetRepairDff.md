<h1>Prime Test-Method Specification REP</h1>
Revision 1.0.0

Jan 2024

[[_TOC_]]

## Methodology
SetRepairDff Test Method gets repair data from SharedStorage service and stores the new compress value into DFF service.

Compression is always done using HEX format.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the SetRepairDff test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Default Value** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ | -----|
| RepairDffFile | Yes | File | N/A | N/A | Path to the input file containing the information to be set to the DFF. |

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

- _**StorageData:**_ Grouping field. This attribute groups all the necessary information to share repair data from either SharedStorage.
    - _**StorageKey:**_ Required. This value indicates they key from which to retrieve the repair data either from SharedStorage. The obtained value is then compressed using HEX format and store in the DFF service.
    - _**DffToken:**_ Required. This token indicates the key under which to store the compressed value in the DFF service.

## TPL Samples
```
Test PrimeSetRepairDffTestMethod SetRepairDFF_SharesValueFromSharedStorageToDFF_P1
{
  RepairDffFile = "~HDMT_TPL_DIR/TestPrograms/Repair/Modules/Repair/InputFiles/SetRepairDff_Basic.json";
}
```

## Exit Ports

The SetRepairDff test method supports the following exit ports:

| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **1**         | ***Pass***      | Passing condition            |