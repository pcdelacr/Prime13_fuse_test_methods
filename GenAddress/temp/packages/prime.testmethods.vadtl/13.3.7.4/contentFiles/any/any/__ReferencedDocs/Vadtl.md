**prime Test-Method Specification REP**

Feb 2022

[[_TOC_]]

# Introduction

VADTL or VMIN Adaptive Test Limit is a methodology used in Sort around HVQK stress to identify a handful of quality and reliability defects that are not easily identified through traditional go/nogo type tests.  The quality/reliability defects manifest themselves through a degradation of VMIN as a result of stress.  The basic implementation of VADTL is to execute a VMIN search pre and post HVQK and measure the VMIN shift. 

# Methodology

## Single Domain

The json configuration file will be read in the Verify and the configuration information will be saved in the SharedStorage by the MBit name. 

The Test Method will calculate the delta between the pre and post vmin values defined in each shift. If the delta es bigger than the threshold defined for the IPDomain the ISVADTL key will be set to "1" and the Test Methos will exit at port 2.

### Json Configuration File 
Example file:
```json
{
    "MBIT_name": "HVQK_MBIT",
    "IPDomains": [
        {
            "name": "CACHE_CLM",
            "threshold": 0.1,
            "extended_datalog": true,
            "shift": [
                {
                    "name": "CLM_CACHE_CABIST_SFTSL2_SSA",
                    "pre": "CLM_CACHE_CABIST_SFTSL2_SSA_PRE",
                    "post": "CLM_CACHE_CABIST_SFTSL2_SSA_POST"
                },
                {
                    "name": "CLM_CACHE_CABIST_LLCDAT_CLM_SSA",
                    "pre": "CLM_CACHE_CABIST_LLCDAT_CLM_SSA_PRE",
                    "post": "CLM_CACHE_CABIST_LLCDAT_CLM_SSA_POST"
                },
		{
                    "name": "CLM_CACHE_CABIST_LLCTAG_CLM_SSA",
                    "pre": "CLM_CACHE_CABIST_LLCTAG_CLM_SSA_PRE",
                    "post": "CLM_CACHE_CABIST_LLCTAG_CLM_SSA_POST"
                },
                {
                    "name": "CLM_CACHE_CABIST_LLCSL2_SSA",
                    "pre": "CLM_CACHE_CABIST_LLCSL2_SSA_PRE",
                    "post": "CLM_CACHE_CABIST_LLCSL2_SSA_POST"
                }
            ]
        },
        {
            "name": "SBFT_CLM",
            "threshold": 0.4,
            "shift": [
                {
                    "name": "CLM_SBFT_MESH_80TH",
                    "pre": "CLM_SBFT_MESH_80TH_PRE",
                    "post": "CLM_SBFT_MESH_80TH_POST"
                }
            ]
        }
    ]
}
```
### Field details
- **MBIT_name**: name of the configuration file. If a second instance use the same MBIT_name then the second configuration file will be ignored and the previous loaded configuration will be used.
- **IPDomains**: Ipdomains collection that holds the parameters for it's shifts.
    - **name**: IPdomain name.
    - **threshold**: is used to define if ISVADTL key is set to 1 or 0. This is positive double number.
    - **extended_datalog**: [optional] if "true" pre and post values will be printed out in the ituff for each shift in the IPdomain. If false or skipped only the shift result will be printed out.
    - **shift**: is a collections of the shifts that will need to follow the threshold of the IPdomain.
        - **name**: shift name. This key will be used to print out the shift result as well as the pre and post values in the ituff.
        - **pre**: Double type Shared Storage key at DUT context for the pre HVQK VMIN value.
        - **post**: Double type Shared Storage key at DUT context for the post HVQK VMIN value.

### Ituff output

The table below lists and describes each token that will be used to print out in the ituff.

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
<td>2_tname_HVQK_VMIN_PRE_<b>shiftname</b></td>
<td>Value of the pre HVQK VIM for that shift.</td>
<td>2_tname_HVQK_VMIN_PRE_CLM_CACHE_CABIST_SFTSL2_SSA<br>
2_mrslt_0.7400</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_VMIN_POST_<b>shiftname</b></td>
<td>Value of the post HVQK VIM for that shift.</td>
<td>2_tname_HVQK_VMIN_POST_CLM_CACHE_CABIST_SFTSL2_SSA<br>
2_mrslt_0.7400</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_VMIN_SHIFT_<b>shiftname</b></td>
<td>Result of the delta between the pre and post HVQK VMIN for the shift.<br> If one or both VMINS are -5555 then the result will be -5555. <br>If one or both VMIN are -9999 then the result will be -9999</td>
<td>2_tname_HVQK_VMIN_SHIFT_CLM_CACHE_CABIST_SFTSL2_SSA<br>
2_mrslt_0.0000</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_SHIFT_<b>IPDomain_name</b></td>
<td>Largest delta of the IPDomain. This token will appear at the end of all the IPDomains and appears one per IPDomain.</td>
<td>2_tname_HVQK_SHIFT_CACHE_CLM<br>
2_mrslt_0.0100</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MBIT_SD_<b>MBIT_name</b></td>
<td>Largest delta of all the shifts in the configuration file.</td>
<td>2_tname_HVQK_MBIT_SD_HVQK_MBIT<br>
2_mrslt_0.0200</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MBIT_SD_NSEARCHES_VALID</td>
<td>Amount of valid delta results.</td>
<td>2_tname_HVQK_MBIT_SD_NSEARCHES_VALID<br>
2_mrslt_54</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MBIT_SD_NSEARCHES_INVALID</td>
<td>Amount of invalid delta results. (-9999) </td>
<td>2_tname_HVQK_MBIT_SD_NSEARCHES_INVALID<br>
2_mrslt_1</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MBIT_SD_NSEARCHES_SKIPPED</td>
<td>Amount of skipped delta results. (-5555) </td>
<td>2_tname_HVQK_MBIT_SD_NSEARCHES_SKIPPED<br>
2_mrslt_10</td>
</tr>
</table>

## Multi Domain

The json configuration file will be read in the Verify and the configuration information will be saved in the SharedStorage by the MBit name. 

The Test Method will calculate the delta between the pre and post vmin values defined in each shift. If the delta is bigger than the threshold defined for the IPDomain the ISVADTL key will be set to "1" and the Test Methos will exit at port 2.

If the json file has the _Multidomain_Settings_ setup then the instance will be considered as a multidomain instance.

### Json Configuration File 
Example file:
```json
{
    "MBIT_name": "HVQK_MBIT_MD",
    "Multidomain_Settings": {
        "domain_count": 45,
        "active_domains": "000000000000000001111111111111111111111111111",
        "vadtl_recovery": "ISVADTL_RECOVERY_45",
        "was_vadtl_recovery": "WASVADTL_RECOVERY_45",
        "max_allowed_shifts": 2,
        "grouping": "0|3|4,1|2|5,6|8|9,7|19|21,10|11|12,13|16|17,14|15|18,20|22|23,24|25,26|27"
    },
    "IPDomains": [
        {
            "name": "CACHE_CORE",
            "threshold": 0.05,
            "extended_datalog": true,
            "shift": [
                {
                    "name": "CORE_CACHE_PBIST_MLC_TAG_GALCOL_LSA",
                    "pre": "CORE_CACHE_PBIST_MLC_TAG_GALCOL_LSA_PRE",
                    "post": "CORE_CACHE_PBIST_MLC_TAG_GALCOL_LSA_POST"
                },
                {
                    "name": "CORE_CACHE_PBIST_CRFALL_GALCOL_LSA",
                    "pre": "CORE_CACHE_PBIST_CRFALL_GALCOL_LSA_PRE",
                    "post": "CORE_CACHE_PBIST_CRFALL_GALCOL_LSA_POST"
                },
                {
                    "name": "CORE_CACHE_PBIST_MLC_TAG_NOGALCOL_LSA",
                    "pre": "CORE_CACHE_PBIST_MLC_TAG_NOGALCOL_LSA_PRE",
                    "post": "CORE_CACHE_PBIST_MLC_TAG_NOGALCOL_LSA_POST",
                }
            ]
        },
        {
            "name": "SCAN_CORE",
            "threshold": 0.05,
            "extended_datalog": true,
            "shift": [
                {
                    "name": "CORE_SCAN_VCCCORE_ALL",
                    "pre": "CORE_SCAN_VCCCORE_ALL_PRE",
                    "post": "CORE_SCAN_VCCCORE_ALL_POST"
                },
                {
                    "name": "CORE_SCAN_VCCCORE_MLC",
                    "pre": "CORE_SCAN_VCCCORE_MLC_PRE",
                    "post": "CORE_SCAN_VCCCORE_MLC_POST"
                }
            ]
        }
    ]
}
```
### Field details
- **MBIT_name**: name of the configuration file. If a second instance use the same MBIT_name then the previous one will be used and the new one will be ignored.
- **Multidomain_Settings**: this field determinates if the instances is multidomain. Holds all the settings and configurations for the IPDomains.
    - **domain_count**: Amount of the domains that each shift will have. Has to be a number greater than 1. (>1)
    - **active_domains**: Mask of 1s and 0s that defines if a domain is active or not. Has to have a definition for each domain. The domains are input MSBF as well as this mask.
    - **vadtl_recovery**: SharedStorage key where the vadtl_recovery information will be set. At the beginning all the domains are set to "0". During the shift calculation, if the domain is active and its shift is higher than defined threshold the value of the bit representing that domain is set to "1". This value is arranged in MSBF.
    - **was_vadtl_recovery**: SharedStorage key whose value is a string in size of domain_count. This value has to be set by the user before the instance is executed. Using the same format as the vadtl_recovery key. This value is also MSBF.
    - **max_allowed_shifts**: Indicates the maximum number of shifts whose value is allowed to be higher than specified threshold and ISVADL flag will not be set to "1". Number greater than 0. (>0)
    - **grouping**: [optional] If set the domains that are in the same group will be counted as 1 for the final shift count to raise or not the ISVADTL flag. Each group has to have 2 or more domains and not all domains need to be in a group.
- **IPDomains**: Ipdomains collection that holds the parameters for it's shifts.
    - **name**: IPdomain name.
    - **threshold**: is used to define if ISVADTL key is set to 1 or 0.
    - **extended_datalog**: [optional] If "true" pre and post values will be printed out in the ituff for each shift in the IPdomain. If false or skipped only the shift result will be printed out.
    - **shift**: is a collections of the shifts that will need to follow the threshold of the IPdomain.
        - **name**: shift name. This key will be used to print out the shift result as well as the pre and post values in the ituff.
        - **pre**: SharedStorage key for the pre HVQK VMIN value. This need to be in the string table and its size is domain_count. The domains have to be arranged in MSBF.
        - **post**: SharedStorage key for the post HVQK VMIN value. This need to be in the string table and its size is domain_count. The domains have to be arranged in MSBF.

### Ituff output

The table below lists and describes each token that will be used to print out in the ituff.

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
<td>2_tname_HVQK_MDV_PRE_<b>shiftname</b></td>
<td>Value of the pre HVQK VMIN for that shift.</td>
<td>2_tname_HVQK_MDV_PRE_CORE_CACHE_PBIST_MLC_TAG_GALCOL_LSA<br>
2_strgval_-8888_-8888_0.5300_0.5300_0.5400_0.5400_0.5300</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MDV_POST_<b>shiftname</b></td>
<td>Value of the post HVQK VMIN for that shift.</td>
<td>2_tname_HVQK_MDV_POST_CORE_CACHE_PBIST_MLC_TAG_GALCOL_LSA<br>
2_strgval_-8888_-8888_0.5300_0.5300_0.5400_0.5400_0.5300</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MDV_SHIFT_<b>shiftname</b></td>
<td>Result of the delta between the pre and post HVQK VMIN for the shift.<br>If the domain is inactive the delta is not calculated and the results will be -7777 <br> If one or both VMINS are -5555 then the result will be -5555. <br>If one or both VMIN are -9999 then the result will be -9999 <br>If one or both VMIN are -8888 then the result will be -8888</td>
<td>2_tname_HVQK_MDV_SHIFT_CORE_CACHE_PBIST_MLC_TAG_GALCOL_LSA<br>
2_strgval_-7777_-7777_0.0000_0.0000_0.0000_0.0000_0.0000</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MDV_SHIFT_<b>IPDomain_name</b></td>
<td>Largest delta of the IPDomain. This token will appear at the end of all the IPDomains and appears one per IPDomain.</td>
<td>2_tname_HVQK_MDV_SHIFT_SCAN_CORE<br>
2_mrslt_0.0300</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MBIT_MD_<b>MBIT_name</b></td>
<td>Largest delta of all the shifts in the configuration file.</td>
<td>2_tname_HVQK_MBIT_MD_HVQK_MBIT_MD<br>
2_mrslt_0.1800</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MBIT_MD_MAXSHIFT_<b>MBIT_name</b></td>
<td>Max delta for each domain of all deltas calculated.</td>
<td>2_tname_HVQK_MBIT_MD_MAXSHIFT_HVQK_MBIT_MD<br>
2_strgval_-7777_-7777_0.0100_0.0300_0.0300_0.0300_0.0100</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MBIT_MD_MAXSHIFT_ALL_<b>MBIT_name</b></td>
<td>Max value of the maxshift.</td>
<td>2_tname_HVQK_MBIT_MD_MAXSHIFT_ALL_HVQK_MBIT_MD<br>
2_mrslt_0.0300</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MBIT_MD_NSEARCHES_VALID_<b>MBIT_name</b></td>
<td>Amount of valid delta results.</td>
<td>2_tname_HVQK_MBIT_MD_NSEARCHES_VALID_HVQK_MBIT_MD<br>
2_mrslt_54</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MBIT_MD_NSEARCHES_INVALID_<b>MBIT_name</b></td>
<td>Amount of invalid delta results. (-9999) </td>
<td>2_tname_HVQK_MBIT_MD_NSEARCHES_INVALID_HVQK_MBIT_MD<br>
2_mrslt_1</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_MBIT_MD_NSEARCHES_SKIPPED<b>MBIT_name</b></td>
<td>Amount of skipped delta results. (-5555 or -8888) </td>
<td>2_tname_HVQK_MBIT_MD_NSEARCHES_SKIPPED_HVQK_MBIT_MD<br>
2_mrslt_10</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_SHIFT_DEFEATURED_<b>was_vadtl_recovery</b></td>
<td>Value of the WAS_VADTL_RECOVERY key</td>
<td>2_tname_HVQK_SHIFT_DEFEATURED_WASVADTL_RECOVERY_45<br>
2_strgval_x0000110</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_SHIFT_DEFEATURED_<b>vadtl_recovery</b></td>
<td>Value of the VADTL_RECOVERY key</td>
<td>2_tname_HVQK_SHIFT_DEFEATURED_VADTL_RECOVERY_45<br>
2_strgval_x0000110</td>
</tr>
<tr class="odd">
<td>2_tname_HVQK_SHIFT_IP_CONTENT_<b>MBIT_name</b></td>
<td>List of domains that beat the threshold. This token has the domain number, shift name and the result.</td>
<td>2_tname_HVQK_SHIFT_IP_CONTENT_HVQK_MBIT_MD<br>
2_strgval_17:CORE_CACHE_PBIST_CRFALL_GALCOL_LSA:0.18|13:CORE_SBFT_VCCCORE_MLC_SIG:0.09|16:CORE_SBFT_VCCCORE_SLC_SIG:0.11</td>
</tr>
</table>

## NOTE
If need to get the overall Vadtl data, please couple PrimeVadtlTestMethod with PrimeVadtlOverallResultsToItuffTestMethod
::: mermaid
graph LR
    A[PrimeVadtlTestMethod]
    A --> B[PrimeVadtlOverallResultsToItuffTestMethod]
:::
Ituff Token that will be create by PrimeVadtlOverallResultsToItuffTestMethod for overall summary:
```
2_tname_HVQK_MBIT_SD
2_tname_HVQK_MBIT_MD
2_tname_HVQK_MBIT_MD_POSTRECOVERY
2_tname_HVQK_MBIT
```

###Example of the WAS VADTL RECOVERY use

The was_vadtl_recovery key is used to set the amount of shifts that passed the threshold alongside the vadtl_recovery key.

Shift result example
```json
Threshold = 0.01

HVQK_MDV_SHIFT_CACHE_MLD_VCORE
-8888_0.0900_0.0000_0.0000_0.0000_0.0000_0.0000_0.0000_0.0000_-9999
HVQK_MDV_SHIFT_ CACHE_MLT _VCORE
-8888_0.0600_0.0000_0.0000_0.0000_0.0000_0.0000_0.0000_0.0000_-9999

```

<table>
<tr class="odd">
<td>Incoming (NOT set by the Test Method) was_vadtl_recovery</td>
<td>0000000000</td>
<td>0000000010</td>
</tr>
<tr class="odd">
<td>Vadtl_recovery set according to the shift results.</td>
<td>0100000000</td>
<td>0100000000</td>
</tr>
<tr class="odd">
<td>Count of shifts that pass the threshold. </td>
<td>1</td>
<td>2</td>
</tr>
</table>
 


# Test instance parameters

The table below lists and describes the test instance parameters supported by the VADTL test method

<table>
<thead>
<tr class="header">
<th><strong>Parameter Name</strong></th>
<th><strong>Type</strong></th>
<th><strong>Description</strong></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td>ConfigurationFile</td>
<td>String</td>
<td>Path to the json configuration file.</td>
</tr>
</table>

# Exit ports

The VADTL test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                                                    |
| ------------- | ------------- | ---------------------------------------------------------------------------------- |
| **0**        | ***Error***   | Any software condition error                                                      
| **1**         | ***Pass***    | Passing condition. None of the domains pass the threshold or amount that passed the threshold wasn't bigger than the maximum. |
| **2**         | ***Fail***    | Fail condition. This happens if the ISVADTL flag is raised. |

# Customer Contact
 
Next, you can find the list of experts in the usage of this Test Method, who provided the methodology details and validated the solution:

- Thwing, James
- Herndon, Steve

# Version tracking

| **Date**   | **Prime release** | **Author**    | **Comments** |
| ---------- | ----------- | ------------- | ------------ |
| Dic, 2022  | 11.00.00      | Viviana Villalobos| 1st version             |