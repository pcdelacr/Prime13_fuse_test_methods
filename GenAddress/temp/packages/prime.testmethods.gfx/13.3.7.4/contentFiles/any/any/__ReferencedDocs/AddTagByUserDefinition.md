# AddTagByUserDefinition

[[_TOC_]]

## REP for AddTagByUserDefinition

This REP is intended to describe the `GfxAddTagByUserDefinition` Prime test method.

Prior to reading this REP, it is recommend to read the `GfxAggregator` SDK to get an overview of the GFX methodology and the infrastructure for it in PRIME.

## Methodology

The `AddTagByUserDefinition` test method utilizes SKUs or recovery group strings that were either generated in prior executions of the `PrimeGfxScoreBoard` test method or defined by users. Its purpose is to add tags to **sharedStorage**. These tags reflect the statuses of passing and failing EIDs based on the specified SKU or recovery group.

This test method serves as a wrapper for invoking the `GfxAggregator` API's `AddTagByRecoveryGroupString` and `AddTagBySkuName` functions. Consequently, users have the flexibility to create custom code that wraps the call to the `GfxAggregator` API.

### Verify

This test method is designed to validate the input parameters and ensure they meet the specified requirements before proceeding with further operations. The method focuses on verifying the `UserInput` parameter based on the `UserInputType`, as well as checking the validity of optional parameters `HryPassSymbol` and `HryFailSymbol`.

#### Required Parameters

- **UserInput**:
  - The content of `UserInput` is validated according to the specified `UserInputType`.
  - **Example**: If `UserInputType` is set to `RecoveryString`, the following checks are performed:
    - **Length Check**: The length of the `RecoveryString` must match the number of area recovery groups.
    - **Content Check**: The `RecoveryString` must be a binary string, consisting only of '0' or '1'.

#### Optional Parameters

- **HryPassSymbol**:
  - This parameter must be a valid HRY symbol.
  - The status associated with `HryPassSymbol` must be `PASS`.
  - If the status is not `PASS`, an exception will be raised.

- **HryFailSymbol**:
  - This parameter must also be a valid HRY symbol.
  - The status associated with `HryFailSymbol` must be `FAIL`.
  - If the status is not `FAIL`, an exception will be raised.

### Execute

The `GfxAggregator` API finally called is determined by `UserInputType`.  `AddUserTagByRecoveryString` will be called when `UserInputType` is one of `RecoveryString` and `RecoveryStringSharedStorageKey`, and `AddUserTagBySkuNameDefaultRecovery` will be called if `UserInputType` is `SkuName` or `SkuNameSharedStorageKey`. `UserInput` requirements for different `UserInputType` could be found in the following table.

|          UserInputType           | Datatype of `UserInput`                        | Invoked `AfxAggregator` API          |
| :------------------------------: | :--------------------------------------------- | :----------------------------------- |
|            `SkuName`             | Target sku name.                               | `AddUserTagBySkuNameDefaultRecovery` |
|    `SkuNameSharedStorageKey`     | Shared storage key for target sku name.        | `AddUserTagBySkuNameDefaultRecovery` |
|         `RecoveryString`         | Recovery string used for determining user tag. | `AddUserTagByRecoveryString`         |
| `RecoveryStringSharedStorageKey` | Shared storage key for target recovery string. | `AddUserTagByRecoveryString`         |

The difference between `UserInputType` with `SharedStorageKey` and the ones without `SharedStorageKey` is that, when the `UserInputType` is the one with `SharedStorageKey`, then this test method will firstly get the `SkuName` or `RecoveryString` from **SharedStorage**, then **UserTag** could be added with the new fetched `SkuName` or `RecoveryString`.

## Test Instance Parameters

| Parameter Name | Required | Type    | Description                                                                                      | Default Value |
| :------------- | :------: | :------ | :----------------------------------------------------------------------------------------------- | :------------ |
| Area           |   Yes    | String  | Target Area name.                                                                                |               |
| Content        |   Yes    | String  | Target Content name.                                                                             |               |
| Tag            |   Yes    | String  | The tag used to mark the EID's status.                                                           |               |
| UserInputType  |   Yes    | Choices | One of: `SkuName`, `SkuNameSharedStorageKey`, `RecoveryString`, `RecoveryStringSharedStorageKey` |               |
| UserInput      |   Yes    | String  | Content according to `UserInputType`.                                                            |               |
| HryPassSymbol  |    No    | String  | Pass EID symbol.                                                                                 | "1"           |
| HryFailSymbol  |    No    | String  | Fail EID symbol.                                                                                 | "0"           |

## TPL Samples

### Example for `UserInputType` = "RecoveryString"

``` java
CSharpTest PrimeGfxAddTagByUserDefinitionTestMethod AddTagByRGString_GT_ARRAY_4X8XSS02S1_P1
{
    LogLevel = "Enabled";
    Area = "GT_EVAL";
    Content = "ARRAY";
    Tag = "TAG_ARRAY_RG1";
    HryPassSymbol = "1";
    HryFailSymbol = "0";
    UserInput= "0000000000110000000";
    UserInputType= "RecoveryString";
}
```

### Example for `UserInputType` = "RecoveryStringSharedStorageKey"

``` java
CSharpTest PrimeGfxAddTagByUserDefinitionTestMethod AddTagByRGString_GT_SCAN_4X8XSS12S1_P1
{
    Area = "GT_EVAL";
    Content = "SCAN";
    Tag = "TAG_SCAN_RG2";
    HryPassSymbol = "1";
    HryFailSymbol = "0";
    UserInput= "RG_1";  
    UserInputType= "RecoveryStringSharedStorageKey";
}

```

### Example for `UserInputType` = "SkuName"

``` java
CSharpTest PrimeGfxAddTagByUserDefinitionTestMethod AddTagBySkuName_GT_ARRAY_5X8XS0S1_P1
{
    Area = "GT_EVAL";
    Content = "ARRAY";
    Tag = "TAG1";
    UserInput = "5X8XS0S1";
    HryPassSymbol = "1";
    HryFailSymbol = "0";
    UserInputType= "SkuName";
}
```

### Example for `UserInputType` = "SkuNameSharedStorageKey"

``` java
CSharpTest PrimeGfxAddTagByUserDefinitionTestMethod AddTagBySkuName_GT_SCAN_4X8XSS12S1_P1
{
    Area = "GT_EVAL";
    Content = "SCAN";
    Tag = "GT_SCAN";
    UserInput = "SKU_KEY_1";
    HryPassSymbol = "1";
    HryFailSymbol = "0";
    UserInputType= "SkuNameSharedStorageKey";
}
```

## Exit Ports

| Exit Port | Condition | Description                                                                                   |
| :-------: | :-------: | :-------------------------------------------------------------------------------------------- |
|    -2     |   Alarm   | Any hardware alarm.                                                                           |
|    -1     |   Error   | Any test class error.                                                                         |
|     1     |   Pass    | The test method called the GfxAggregator API and the addition of the tag passed successfully. |

## Acronyms

Definition of acronyms used in this document

- REP Prime Test-Method Specification
- EID Engine Identifier
- TP Test Program
- HRY Human Readable Yield
- PID Partition Identifier
- TPL Test Programming Language
  
## Version tracking

|     Date     | Version |    Author     | Comments                                              |
| :----------: | :-----: | :-----------: | :---------------------------------------------------- |
| Sep 21, 2022 |  1.0.0  | Azriel, Yuval | Initial version.                                      |
| Jan 24, 2023 |  2.0.0  | Azriel, Yuval | Updated version.                                      |
| Feb 24, 2023 |  3.0.0  |  Wang, Quzhi  | Fix markdown table and update test method parameters. |
