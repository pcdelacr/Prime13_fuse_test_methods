<h1>Prime Test-Method Specification REP</h1>

[[_TOC_]]

## Methodology

The purpose of the `PrimeDFFFooterPrintTestMethod` is to enable the capability to print a DFF token list from the MTL (Master Token List) to ITUFF in the footer format.  
It is useful in cases where the DFF data is required for TULIP, but the units fail before starting the execution of the `PrimeTimeUnderStressControlTestMethod` or the test plan end flow, which are processes that print the same DFF data.  

The `PrimeDFFFooterPrintTestMethod` has various features for printing the tokens:

1. Pulling the list of master tokens from the MTL file for a specific die name, filtered by one or more modules.
2. Generating a custom DFF token list where the user defines which tokens to be printed from the MTL file for a specific die name.
3. Combining the two previous features to filter by modules the custom DFF token list defined by the user.

The test method uses the die name to obtain the list of tokens. It also provides the option of specifying the datalog
user bin to be printed in the footer bin information.

### Execute flow

flowchart TD
    A[Start] --> B{Is 'Modules' parameter Specified?}
    B --> |Yes| C[Call dffService.GetAllMtlTokensByDieIdAndModule with DieName and every Module]
    B --> |No| D[Call dffService.GetAllMtlTokensByDieId with DieName]
    C --> E{Is TokensList empty?}
    D --> E
    E --> |Yes| F[TokensToPrint = MtlTokens]
    E --> |No| G[Call FilterByCustomTokens with TokensList]
    G --> F[TokensToPrint = MtlTokens]
    F --> H{Is TokensToPrint empty?}
    H --> |Yes| I[Fail Execute]
    H --> |No| J[Call dffService.PrintTokensToItuffFooter with TokensToPrint, DatalogUserBin and DieName]
    I --> K[End]
    J --> K[End]


## Test Instance Parameters

| **Parameter Name** | **Required?** | **Type** | **Values**                                                              | **Default Value**            | **Comments**                                                                                                                                                                    |
|--------------------|---------------|----------|-------------------------------------------------------------------------|------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| TokensList         | Optional      | String   | Comma-separated list with the name of the tokens to be printed.         | Defaults to an empty string. | Either `TokensList` or `Modules` must be specified. If a token name specified is not found in MTL either filtered by module or not, the test instance will fail through port 0. |
| Modules            | Optional      | String   | Comma-separated list with the module names of the tokens to be printed. | Defaults to an empty string. | Either `TokensList` or `Modules` must be specified. If the module name does not exist in MTL, test instance will fail through port 0.                                           |
| DieName            | Optional      | String   | Die ID for printing the DFF data.                                       | Default value is "PKG".      | The die name is always used to obtain the list of tokens.                                                                                                                       |
| DatalogUserBin     | Optional      | String   | User bin to be reported in the footer.                                  | Default value is "9850".     |                                                                                                                                                                                 |


## Datalog output
The footer will be logged as follows in ITUFF:

```python
3_commt_footer_begin
2_lsep
2_tname_<TName>
2_comnt_DFF_Data_<DieName>_<DffToken1>=<Token1Value>,<DffToken2>=<Token2Value>,…,<DffTokenN>=<TokenNValue>
2_lend 
3_dvtsteddt_<datetime>
3_binn_<dbin>
3_curfbin_<fbin> 
3_curibin_<ibin>
3_trslt_fail
3_commt_footer_end
```

Example:
```python
3_comnt_footer_begin
2_lsep
2_tname_Dff::PrintFooter_DefaultDieName_P1
2_comnt_DFF_Data_PKG,BINN=4002,TIME=1234,SOT_T1=0.74,SOT_T0=-10,GENCHKS=-999|-999,GENCHKI=0,GENCHKD=0.1,GENCHKB=0,GENCHKH=0,OLBTEST=-999
2_lend
3_dvtsteddt_20241213100201
3_binn_90989850
3_curfbin_9850
3_curibin_98
3_trslt_fail
3_comnt_footer_end
```

## Custom User Code Hooks
N/A


## TPL Samples

Printing all the DFF master tokens of two modules:
```python
CSharpTest PrimeDffFooterPrintTestMethod PrintFooterWithoutTokensList_DefaultDieName_P1
{
    Modules = "HBI,TPI";
    LogLevel = "Enabled";
}
```

Specifying tokens list and modules:
```python
CSharpTest PrimeDffFooterPrintTestMethod PrintFooterWithCustomDffTokensListAndModules_P1
{
    Modules = "HBI";
    DieName = "U4";
    DatalogUserBin = "9999";
    TokensList = "TOKB,TOKBIN,ENCBIN,TOKHEX";
    LogLevel = "Enabled";
}
```

## Exit Ports

The `PrimeDffFooterPrintTestMethod` supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                                                                                                                                      |
|---------------|---------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **-2**        | ***Alarm***   | Any alarm condition.                                                                                                                                                                 |
| **-1**        | ***Error***   | Any software condition error. The die name does not exist in MTL. `Modules` and `TokensList` were not specified.                                                                     |
| **0**         | ***Fail***    | There are no tokens to print and/or the specified modules do not exist in MTL for the die name. A custom token does not exist in MTL for the die name and specified modules, if any. |
| **1**         | ***Pass***    | All tokens printed successfully.                                                                                                                                                     |

  
## Additional Dependencies
This test method depends on the MTL file contents used by DFF.

## Version tracking

| **Date**       | **Version** | **Author**            | **Comments**                    |
|----------------|-------------|-----------------------|---------------------------------|
| Dec 13th, 2024 | 13.2.0      | Pinto Rosales, Raquel | Initial release of test method. |


## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **DFF**: Data Feed Forward
  - **TULIP**: Constructs DFF lines based on what is written in ITUFF.