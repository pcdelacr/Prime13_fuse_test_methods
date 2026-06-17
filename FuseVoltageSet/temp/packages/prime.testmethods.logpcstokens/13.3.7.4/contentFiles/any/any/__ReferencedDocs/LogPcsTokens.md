<h1>Prime LogPcsTokens Specification REP</h1>
Revision 1.0.0

May 2021

[[_TOC_]]

## Methodology
The LogPcsTokens test method logs the PCS tokens (all EEPROM counters) and current EEPROM Socket serial number to ituff in the case of HBI.

Due to how PCS tokens are obtained (from EEPROM) when running this test method offline (engineering) nothing **will be logged**, this due to there being nothing to log. This test method will log the PCS tokens when tester is **online**.

## Test Instance Parameters

<table>
<thead>
<tr class="header">
<th><strong>Parameter Name</strong></th>
<th><strong>Required?</strong></th>
<th><strong>Type</strong></th>
<th><strong>Description</strong></th>
<th><strong>Comments</strong></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td>ForceAries</td>
<td>No</td>
<td>String</td>
<td>Forces ARIES output in datalog output. Even if MIDAS is enabled.</td>
<td>Should only be "true" or "false". By default this is true.</td>
</tr>
</tbody>
</table>

## Datalog output
The test method logs different information based on tester type. If chassis is HBI the test method will log:
```python
2_tname_PCS_SKTCN{dutId}
2_mrslt_2234
2_lsep
2_tname_PCS_SKTRP{dutId}
2_mrslt_2234
2_lsep
2_tname_PCS_SKTCL{dutId}
2_mrslt_2234
2_lsep
2_tname_PCS_SKT_SN
2_strgval_{{eepromSocketSerialNumber}}
2_tname_PCS_SKTMF{dutId}
2_mrslt_2234
2_lsep
2_tname_PCS_SKTES{dutId}
2_mrslt_2234
2_lsep
```

Meanwhile for HDMX output should look like:
```python
2_tname_PCS_BBTCN
2_mrslt_2234
2_lsep
2_tname_PCS_CTACN{dutId}
2_mrslt_2234
2_lsep
2_tname_PCS_SKTCN{dutId}
2_mrslt_2234
2_lsep
2_tname_PCS_SKTRP{dutId}
2_mrslt_2234
2_lsep
2_tname_PCS_SKTCL{dutId}
2_mrslt_2234
2_lsep
2_tname_PCS_SKTMF{dutId}
2_mrslt_2234
2_lsep
2_tname_PCS_SKTES{dutId}
2_mrslt_2234
2_lsep
```

When using test method parameter `ForceAries` as true the output will always be ituff level 2. Regardless if MIDAS (output level 0) is enabled.

## Custom User Code Hooks

This test method does not currently support user code hooks.

## TPL Samples

Here are a few test instance examples using the LogPcsTokens test method.

**TPL Sample1:**

```python
Import PrimeLogPcsTokensTestMethod.xml;


Test PrimeLogPcsTokensTestMethod PrimeLogPcsTokensTestMethod_P1
{
	ForceAries = "FALSE"
}

```
## Exit Ports

The LogPcsTokens test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Fail***      | Failing condition            |
| **1**         | ***Pass***      | Passing condition            |

  
## Additional Dependencies

More dependencies to consider for this TestMethod to well operate:

## Version tracking

| **Date**            | **Version** | **Author**             | **Comments** |
| ------------------- | ----------- | ---------------------- | ------------ |
| May 13th, 2021      | 1.0.0       | Lauren McDonald        |              |
| Wednesday 4th, 2021 | 2.0.0       | Jose Alberto Barrantes | Fixing https://dev.azure.com/mit-us/PRIME/_workitems/edit/25923/ and https://dev.azure.com/mit-us/PRIME/_workitems/edit/24826/ |
| July 11th, 2024     | 13.01.00    | Raquel Pinto           | Adding MetalFrame and ElastomerSheet serial numbers counters. <br> #50154 #44556 |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System