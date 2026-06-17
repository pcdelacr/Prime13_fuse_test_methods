**prime Test-Method Specification REP**

Feb 2022

[[_TOC_]]

# Introduction

VadtlOverallResultsToItuff is a test method intended to be used after all VADTL instances have been executed. The purpose of this Test Method is to provide overall results of the VADTL instances printing to ituff the maximums values for different cases.

# Methodology

The Test Method will use the information from previous VADTL instances to calculate the next results:

<table>
<thead>
<tr class="header">
<th><strong>Token</strong></th>
<th><strong>Description</strong></th>
<th><strong>Example</strong></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td>2_tname_HVQK_MBIT_SD</td>
<td>Max shift value of all previous singledomain instances.</td>
<td>2_tname_HVQK_MBIT_SD<br>
2_mrslt_0.0200</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MBIT_MD</td>
<td>Max shift value of all previous multidomain instances.</td>
<td>2_tname_HVQK_MBIT_MD<br>
2_mrslt_0.1800</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MBIT_MD_POSTRECOVERY</td>
<td>Max value between postrecoverys of multidomain instances.</td>
<td>2_tname_HVQK_MBIT_MD_POSTRECOVERY<br>
2_mrslt_0.0300</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MBIT</td>
<td>Max value between the HVQK_MBIT_SD and HVQK_MBIT_MD_POSTRECOVERY</td>
<td>2_tname_HVQK_MBIT<br>
2_mrslt_0.0300</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MD_VADTL_RECOVERY</td>
<td>Numeric token to identify hard xx26 vs VADTL recovery<br>
0: hard x26<br>
2699:  VADTL Recovery</td>
<td>2_tname_HVQK_MD_VADTL_RECOVERY<br>
2_mrslt_2699</td>
</tr>
</table>

###How to calculate HVQK_MBIT_MD_POSTRECOVERY

**MultiDomain Instance #1**
```json
MBIT_name = HVQK_MBIT_MD_CORE
Vadtl recovery key = ISVADTLL_RECOVERY_CORE

2_tname_HVQK_MBIT_MD_MAXSHIFT_HVQK_MBIT_MD_CORE
-8888_0.0900_0.0000_0.0000_0.0000_0.0000_0.0000_0.0000_0.0400_-9999
```
**MultiDomain Instance #2**
```json
MBIT_name = HVQK_MBIT_MD_MC
Vadtl recovery key = ISVADTLL_RECOVERY_MC

2_tname_HVQK_MBIT_MD_MAXSHIFT_HVQK_MBIT_MD_MC
0.0000_0.0000_0.0150_-9999
```

<table>
<tr class="odd">
<td>Incoming (NOT set by the Test Method) If 1 that means that the shift was defeature.<br> Shared Storage key: VADTL_Defeature_<b>MBit_name</b></td>
<td>VADTL_Defeature_HVQK_MBIT_MD_CORE<br> 0100000000</td>
<td>VADTL_Defeature_HVQK_MBIT_MD_MC<br> 0010</td>
</tr>
<tr class="odd">
<td>Postrecovery for each MBIT file. Note that any shift value that was defeature is excluded.</td>
<td>HVQK_MBIT_MD_CORE <br> 0.0000</td>
<td>HVQK_MBIT_MD_MC <br> 0.0400</td>
</tr>
</table>

The maximum postrecovery will be calculated between all previous postrecoverys. For this example will be

2_tname_HVQK_MBIT_MD_POSTRECOVERY
0.0400

# How to calculate Vadtl Recovery

This is a numeric token to identify hard xx26 vs VADTL recovery. The possible values are:

 - 0: hard x26
 - 2699:  VADTL Recovery.
 
The test method will count the amount of "1" between all was_vadtl_recovery and is_vadtl_recovery keys and if the count is 1 or more then the token will be 2699 otherwise will be 0.

2_tname_HVQK_MD_VADTL_RECOVERY
2_mrslt_2699



# Test instance parameters

This test method doesn't have any parameters. 

# Exit ports

The VADTL test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                                    |
| ------------- | ------------- | ---------------------------------------------------------------------------------- |
| **0**        | ***Error***   | Any software condition error                                                      
| **1**         | ***Pass***    | Passing condition. VADTL instances were executed before and all token were able to print to ituff.|

# Customer Contact
 
Next, you can find the list of experts in the usage of this Test Method, who provided the methodology details and validated the solution:

- Thwing, James
- Herndon, Steve


# Version tracking

| **Date**   | **Prime release** | **Author**    | **Comments** |
| ---------- | ----------- | ------------- | ------------ |
| Dic, 2022  | 11.00.00      | Viviana Villalobos| 1st version             