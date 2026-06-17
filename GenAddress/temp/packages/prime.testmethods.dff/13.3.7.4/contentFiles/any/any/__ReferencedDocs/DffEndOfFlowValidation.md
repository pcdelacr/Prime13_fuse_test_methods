<h1>Prime Test-Method Specification REP</h1>
Revision 1.0.0

June 2021

[[_TOC_]]

## Methodology
The DFFEndOfFlowValidation test method is meant to do two things mainly:

1. Validate tokens that were not set during the mainflow execution.
1. Make sure that if there's a token that was fail to set during main flow execution but was 'forced flow' to continue execution, it will fail during this test method execution. 

The checking will be done based on the same MTL rules defined in the Test Program and the decision to exit port 0 or not is decided based on the tokens kill/non-kill mode defined during DFFRead.

For example, if a token was failed to set during main flow execution and the token was a non-kill, during the this test method execution, it will just log the token in ituff but it will still exit to port 1.

However, if it's a kill, it will exit to port 0. Same rule applied to the tokens that were not set during main flow execution. It will use the default as the value to check against the MTL rule and do kill/non-kill based on the DFFRead execution.

## Test Instance Parameters


| **Parameter Name** | **Required?** | **Type** | **Values** | **Default Value** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ | -----|
| EnableFailingPortForDefaultValue | Optional | String (Choice) |     ENABLED       | DISABLED| Default behavior to turn off port for MTL default value failures (DNS). When ENABLED, if there's a DNS failure will exit to port 2. |
||||DISABLED|||



**Notes:**
- The test method test instance strongly advise to locating it at the end of main flow.

- The EOF Validation is validating the unused DFF tokens and fields against MTL, and the summary of failed SetDff in Main Flow

- Information on Unused tokens Validations and summary of failed SetDff in Main Flow(DMFF & DMFFN) are datalog to console(when LogLevel=PRIME_DEBUG) and ITUFF stream.
 
- Console Example:
```python
DFF End of Flow Results:
     Failures Identified: <total number of fails> of Inline Validate of fails + <total number of fails> of EOF fails
	     Enabled Modules: <Enabled Module>
     Tokens Not Set During Flow:
	     [<Die ID>] = <Token Names>
	         *Will appear new line when more Die ID have unused token
Failing Validation Results:
	 Fail Type Description 			 = <TOKEN.FIELD>,<VALUE>,<TOKEN2>,<VALUE2>
	 ----------------------------------------------------------------------------
	 <Fail Type Description>		 = <TOKEN.FIELD>,<VALUE>,<TOKEN2>,<VALUE2>
```
## Datalog output
If the field is not found it will log as such in ITUFF:
```python
2_tname_<TestInstanceName>_DVO
2_mrslt_<total number of fails>
2_tname_<TestInstanceName>_DKM
2_strgval_<Enabled Module>
2_tname_<TestInstanceName>_<Fail Type>_<Die ID>
2_strgval_<Token Names>
  *Will appear new set of 2_tname and 2_strgval when there is multiple Fail Types or multiple Die IDs

2_tname_<TestInstanceName>_DMFF_<Die ID>
2_strgval_<Token Names>
  *Will appear when there is SetDff failure and multiple set of 2_tname and 2_strgval when there is multiple Die IDs

2_tname_<TestInstanceName>_DMFFN_<Die ID>
2_strgval_<Token Names>
  *Will appear when there is SetDff failure(NONKILL) and multiple set of 2_tname and 2_strgval when there is multiple Die IDs
```

Actual example:
```python
2_tname_Dff::EndOfFlowValidation_P1_DVO
2_mrslt_1
2_tname_Dff::EndOfFlowValidation_P1_DKM
2_strgval_!TPI_IDV
2_tname_Dff::EndOfFlowValidation_P1_DNS_PKG
2_strgval_SOT_T0,SOT_T1
2_tname_Dff::EndOfFlowValidation_P1_DNS_U1
2_strgval_SOT_T0,SOT_T1
2_tname_Dff::EndOfFlowValidation_P1_DNS_U2
2_strgval_SOT_T0,SOT_T1
2_tname_Dff::EndOfFlowValidation_P1_DNS_U2.U1
2_strgval_TOKC,TOKD,TOKE,TOKF.F1,TOKF.F2,TOKF.F3,TOKHEX
2_tname_Dff::EndOfFlowValidation_P1_DNS_U4
2_strgval_TOKC,TOKD,TOKE,TOKF.F1,TOKF.F2,TOKF.F3,TOKHEX
2_tname_Dff::EndOfFlowValidation_P1_GDNEF
2_strgval_U4NOMTL.F1
```

- Descriptions for datalogging above

    |Field|Description|
    |-|-|
    |TestInstanceName|Current Test Instance Name|
    |Total number of fails|total count of Inline Validate of fails and EOF validation fails|
    |Enabled Module|DFF Kill Mode from DFFRead enabled Modules Parameter|
    |Fail Type|show first encountered fail when validating according to priority defined as below|
    |Die ID|Die ID Used during Set DFF|
    |TokenNames|Tokens involved(if there is field name, <TOKEN.FIELD>), separated by comma|
    |Value|token current value|
- Fail Type and other involved Acronyms

    |Fail Type|Description|Valid when Kill Status is|
    |-|-|-|
    |DVO|Total Count of SetDff Failures|KILL & NON-KILL|
    |DKM|DFF Kill Mode|KILL & NON-KILL|
    |DNS|DFF tokens and fields that not touch in Main Flow|KILL & NON-KILL|
    |DAV|DFF token Access Violation|KILL & NON-KILL|
    |DNE|DFF token Not Exist|KILL & NON-KILL|
    |DNEF|DFF token Field Not Exist|KILL & NON-KILL|
    |GDNE|Get DFF token Not Exist|n/a|
    |GDNEF|Get DFF token Field Not Exist|n/a|
    |DDC|DFF token Delimiter Count Check|KILL & NON-KILL|
    |DINC|DFF token Invalid Character Check|KILL & NON-KILL|
    |DRO|DFF token Required/Optional Check|KILL Only|
    |DFC|DFF token Format Check|KILL Only|
    |DSL|DFF token String Length Check|KILL Only|
    |DRCAV/DRCDV|DFF token Range Check Allowed/Disallowed Values|KILL Only|
    |DMFF|DFF Main Flow Fail Flag|KILL & NON-KILL|
    |DMFFN|DFF Main Flow Fail Flag (NONKILL)|KILL Only|

## Custom User Code Hooks
N/A

## TPL Samples

Without DNS Port enabled:
```python
Import PrimeDffEndOfFlowValidation.xml;

Test PrimeDffEndOfFlowValidationTestMethod PrimeDffEndOfFlowValidation_P1
{
    LogLevel = "PRIME_DEBUG";
}
```

With DNS Port enabled:
```python
Import PrimeDffEndOfFlowValidation.xml;

Test PrimeDffEndOfFlowValidationTestMethod PrimeDffEndOfFlowValidation_P1
{
    LogLevel = "PRIME_DEBUG";
    EnableFailingPortForDefaultValue = "ENABLED";
}
```

## Exit Ports

The GetDff test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**                 |
| ------------- | --------------- | ----------------------------    |
| **0**         | ***Fail***      | Failing condition               |
| **1**         | ***Pass***      | Passing condition               |
| **2**         | ***Fail***      | Failing condition (DNS Failure) |

  
## Additional Dependencies

N/A

## Version tracking

| **Date**       | **Version** | **Author**         | **Comments** |
| -------------- | ----------- | ------------       | ------------ |
| June, 2021     | 1.0.0       | Mohd Faiz Mohd Asri| Initial release of test method.             |
| March, 2023    | 12.1.0      | Mohd Faiz Mohd Asri| #35074:   Added Port 2 for DNS failure (MTL default value)              |
|                |             |                    |              |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
- **DFF**: Data Feed Forward