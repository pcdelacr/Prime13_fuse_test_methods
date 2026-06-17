# REP for Prime's LotEndDatalog TestMethod

This **REP** is intended to describe the LotEndDatalog Prime TestMethod.

[[_TOC_]]

# <span id="Methodology" class="anchor"></span> **Methodology**

The LotEndDatalog Test Method is in charge of writting the relevant datalog information after a Lot has been run to ituff. This test method is intended to be used **before LotEndFinalize TM**. 

This test method will create the datalog Lot footer for ituff. There should only be **one** instance of this kind and it should be before the `HMI_Ituff` instance in charge of closing the streams.

## <p><span style="color:#ff0000"><strong>Ituff Sensitivity Warning</strong></span></p>
<p><span style="color:#ff0000">Do not enable <em>InstanceSummaryMode</em> for this test method instances. It will cause Ituff to be printed out-of-sequence causing failure down the line. A better solution is being worked on.</span></p>

## <span id="_Toc36332601" class="anchor"></span>**Execute**

:large_blue_diamond: Set Lot end date.  
:large_blue_diamond: Construct ituff footer.  
:large_blue_diamond: Write output to datalog(ituff).  

# <span id="Parameters" class="anchor"></span> **Test Instance Parameters**

The table below lists and describes the test instance parameters supported by the LotEndDatalog test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Comments**                           |
| ------------------ | ------------- | -------- | ---------- | -------------------------------------- |
| LogLevel     | Yes            | String   | “PRIME_DEBUG”, "DISABLED"    | Default value - DISABLED  |

# <span id="Output" class="anchor"></span> **Datalog output**

Here you will find explanation of each level logged by this TM. This includes each token that could appear, conditions on how it appears (if it applies) and any depedencies for how it works. 

Note that the following items are in order of appearance.

## <span style="color:DarkSeaGreen"> **Ituff level 3** </span>
---
### <span style="color:MediumSeaGreen   "> **Counters Datalog** </span>
All counters used in the lot will be printed if certain conditions are met. First the counter has a **value of more than 0**, second the counter name starts with `p` or `n`. 

If a counter starts with `p` its considered a **pass** counter. If it starts with `n` its considered a **fail** counter. The format for printing is: 

`3_comnt_{counterName}_{counterValue}`.

 Pass counters are datalogged first, then fail counters.

#### <span style="color:Cyan"> **Example:** </span>
```python
3_comnt_p99530045_pass_TPI_DFF::DFFX_X_CALC_K_FINAL_X_X_X_X_SET_SORTBIN_1_5
3_comnt_n90204099_fail_ARR_CORE_PBISTKS::LSA_CR_FAST_K_PREHVQK_X_VCCCORE_F1_X_ROMCAMKS_ALL_PB_PO_0_2
```

### <span style="color:MediumSeaGreen   "> **Hard Bins** </span>
All lot hard bins that were used in the TP will be dataloged. Similar to counters if a hard bin count is 0 it will **not** be logged. 

Format for printing:

`3_comnt_{hardBinName}_{hardBinCounter}`

#### <span style="color:Cyan"> **Example:** </span>

```python
3_comnt_HardBins.b1_PASS_CMTTRAY_1_2
3_comnt_HardBins.b3_PASS_CMTTRAY_3_3
3_comnt_HardBins.b8_FAIL_VCCCONTINUITY_1
3_comnt_HardBins.b19_FAIL_RESET_1
```
### <span style="color:MediumSeaGreen   "> **Level 3 end** </span>
Format `3_lend`.

#### <span style="color:Cyan"> **Example:** </span>
```python
3_lend
```

## <span style="color:DarkSeaGreen"> **Ituff level 4** </span>
---
### <span style="color:MediumSeaGreen   "> **Binning Counters** </span>

Tokens <span style="color:yellow"> **fbinctr**, **dbinctr**, **ibinctr**</span> : **All soft and hard bins are datalogged at this level as long as they have been used at least once**. Hence their count is **NOT** 0. This applies for all the following tokens.

First token printed is <span style="color:yellow">**ibinctr**</span>. These are essencially all hardbins that were used.

Format for printing:

`4_ibinctr_{ibinID}_{ibinCount}`

#### <span style="color:Cyan"> **Example:** </span>

```python
4_ibinctr_1_2
4_ibinctr_3_3
4_ibinctr_8_1
```

Second token printed is <span style="color:yellow"> **fbinctr**</span>. These are a combination of **soft** bins, first all soft bins that are **not** 8 digits long are printed. The remainer soft bins that are **8 digits long** are converted to 4 digit bins and dataloged as fbins. The conversion logic is the following:

- 8 digit soft bin: SoftBins.b90<span style="color: lightgreen">0857</span>14
- Converted to fbin: `SoftBins.b0857`

Please note the logging order is **ascending** and based on the `bin ID`.

Format for printing:

`4_fbinctr_{fbinID}_{fbinCount}`

#### <span style="color:Cyan"> **Example:** </span>

```python
4_fbinctr_100_2
4_fbinctr_327_3
4_fbinctr_805_1
4_fbinctr_1988_1
4_fbinctr_2620_1
4_fbinctr_4137_1
4_fbinctr_4208_1
4_fbinctr_4297_1
4_fbinctr_4307_1
4_fbinctr_6810_1
4_fbinctr_6818_1
```

Finally the last token is <span style="color:yellow">**dbinctr**</span>. These are all soft bins that are **8 digits long** and their respective count. 

Format for printing:

`4_dbinctr_{dbinID}_{dbinCount}`

#### <span style="color:Cyan"> **Example:** </span>

```python
4_dbinctr_90191988_1
4_dbinctr_90262620_1
4_dbinctr_90414137_1
4_dbinctr_90424208_1
4_dbinctr_90424297_1
4_dbinctr_90434307_1
4_dbinctr_90686810_1
4_dbinctr_90686818_1
```
### <span style="color:MediumSeaGreen   "> **Total unit count** </span>
Device TMs take care of counting the processed units per lot. Once the lot has been completed the total amount of units tested is datalogged.

Format for printing:

`4_total_{amountOfUnits}`

#### <span style="color:Cyan"> **Example:** </span>

```python
4_total_14
```
### <span style="color:MediumSeaGreen   "> **BRITA variables** </span>

At this point BRITA related information is logged. This only happens if PRIME BRITA (Flow Trace) is **enabled**. By default it's enabled, unless following uservar: `_UserVars.ENABLE_PRIME_BRITA` is defined with `FALSE` value.

Format for printing (if BRITA enabled):

`4_BRITA_CHECKSUM_{checksum}`

`4_BRITA_VERSION_{version}`

#### <span style="color:Cyan"> **Example:** </span>

```python
4_tsattrs_BRITA_CHECKSUM,97da4aa3eae4cca1523159ee80eb10ad
4_tsattrs_BRITA_VERSION,3p0
```

### <span style="color:MediumSeaGreen   "> **MultiTrialTest variables** </span>

MultiTrialTest variables: If MTT variables are under use in the TP they will be logged at this point as `tsattrs`. 

#### <span style="color:Cyan"> **Example:** </span>

```python
4_tsattrs_mttVars1,MyUserVarCo1.TrialVarValue["2","0","1"];MyUserVarCo1.VariableName1["Var1","Var1","Var1"];MyUserVarCo1.PrefixContinue["ActionContinue1","ActionContinue2","ActionContinue3"]
4_tsattrs_mttVars2,MyUserVarCo1.SomeReallyLongNameForAnUserVarForTestingPurposes1["SomeReallyLongValuesForAnUserVarForTestingPurposes1","SomeReallyLongValuesForAnUserVarForTestingPurposes2","SomeReallyLongValuesForAnUserVarForTestingPurposes3"]
```

### <span style="color:MediumSeaGreen   "> **Lot End Time** </span>

Last token logged is the time and date at which the lot ended. This is taken from the start of the execution of this TM. 

Format for printing:

`4_enddate_{Date}`

The date format is: `yyyyMMddHHmmss`

#### <span style="color:Cyan"> **Example:** </span>

```python
4_enddate_20211030011348
```

### <span style="color:MediumSeaGreen   "> **Level 4 end** </span>

Format `4_lend`.

#### <span style="color:Cyan"> **Example:** </span>
```python
4_lend
```
---

## <span style="color:DarkSeaGreen"> **Ituff level 5** </span>
 Format `5_lend`.

 #### <span style="color:Cyan"> **Example:** </span>
```python
5_lend
```

---
## <span style="color:DarkSeaGreen"> **Ituff level 6** </span>

Format `6_lend`.

#### <span style="color:Cyan"> **Example:** </span>
```python
6_lend
```

---
## <span style="color:DarkSeaGreen"> **Ituff level 7** </span>

This level is populated by TOS, which is why the LotEndDatalog Test Method does not provide an entry for this level.

---
**At this point nothing else is printed for the current lot, ituff datalogging is completed**.

Full example:
```python
3_comnt_p99530045_pass_TPI_DFF::DFFX_X_CALC_K_FINAL_X_X_X_X_SET_SORTBIN_1_5
3_comnt_n90204099_fail_ARR_CORE_PBISTKS::LSA_CR_FAST_K_PREHVQK_X_VCCCORE_F1_X_ROMCAMKS_ALL_PB_PO_0_2
3_comnt_HardBins.b1_PASS_CMTTRAY_1_2
3_comnt_HardBins.b3_PASS_CMTTRAY_3_3
3_comnt_HardBins.b8_FAIL_VCCCONTINUITY_1
3_comnt_HardBins.b19_FAIL_RESET_1
4_ibinctr_1_2
4_ibinctr_3_3
4_ibinctr_8_1
4_fbinctr_100_2
4_fbinctr_327_3
4_fbinctr_805_1
4_fbinctr_1988_1
4_fbinctr_2620_1
4_fbinctr_4137_1
4_fbinctr_4208_1
4_fbinctr_4297_1
4_fbinctr_4307_1
4_fbinctr_6810_1
4_fbinctr_6818_1
4_dbinctr_90191988_1
4_dbinctr_90262620_1
4_dbinctr_90414137_1
4_dbinctr_90424208_1
4_dbinctr_90424297_1
4_dbinctr_90434307_1
4_dbinctr_90686810_1
4_dbinctr_90686818_1
4_total_14
4_tsattrs_BRITA_CHECKSUM,97da4aa3eae4cca1523159ee80eb10ad
4_tsattrs_BRITA_VERSION,3p0
4_tsattrs_mttVars1,MyUserVarCo1.TrialVarValue["2","0","1"];MyUserVarCo1.VariableName1["Var1","Var1","Var1"];MyUserVarCo1.PrefixContinue["ActionContinue1","ActionContinue2","ActionContinue3"]
4_tsattrs_mttVars2,MyUserVarCo1.SomeReallyLongNameForAnUserVarForTestingPurposes1["SomeReallyLongValuesForAnUserVarForTestingPurposes1","SomeReallyLongValuesForAnUserVarForTestingPurposes2","SomeReallyLongValuesForAnUserVarForTestingPurposes3"]
4_enddate_20211030011348
4_lend
5_lend
6_lend
```

## Custom User Code Hooks
LotEndDatalog test method supports the following extensions:
```csharp
    /// <summary>
    /// Class to define extendable methods.
    /// </summary>
    public interface ILotEndDatalogExtensions
    {
        /// <summary>
        /// Called at execute once level 6 footer is built.
        /// Method to be implemented by user to modify level 6 footer values before they are printed out.
        /// This allows the user to modify this level as desired, for example adding new items or modifying
        /// existing ones. Default implementation outputs basic working level 6 footer format.
        /// </summary>
        /// <param name="footerData">Contains lot level 6 footer data.</param>
        /// <returns>Modified Footer.</returns>
        string ModifyLotLevel6Footer(string footerData);

        /// <summary>
        /// Called at execute once level 5 footer is built.
        /// Method to be implemented by user to modify level 5 footer values before they are printed out.
        /// This allows the user to modify this level as desired, for example adding new items or modifying
        /// existing ones. Default implementation outputs basic working level 5 footer format.
        /// </summary>
        /// <param name="footerData">Contains lot level 5 footer data.</param>
        /// <returns>Modified Footer.</returns>
        string ModifyLotLevel5Footer(string footerData);

        /// <summary>
        /// Called at execute once level 4 footer is built.
        /// Method to be implemented by user to modify level 4 footer values before they are printed out.
        /// This allows the user to modify this level as desired, for example adding new items or modifying
        /// existing ones. Default implementation outputs basic working level 4 footer format.
        /// </summary>
        /// <param name="footerData">Contains lot level 4 footer data.</param>
        /// <returns>Modified Footer.</returns>
        string ModifyLotLevel4Footer(string footerData);

        /// <summary>
        /// Called at execute once level 3 footer is built.
        /// Method to be implemented by user to modify level 3 footer values before they are printed out.
        /// This allows the user to modify this level as desired, for example adding new items or modifying
        /// existing ones. Default implementation outputs basic working level 3 footer format.
        /// </summary>
        /// <param name="footerData">Contains lot level 3 footer data.</param>
        /// <returns>Modified Footer.</returns>
        string ModifyLotLevel3Footer(string footerData);
    }
```

All extension methods essentially provide the exact same capability. Each method allows the user to utilize the **basic** generated footer for each level and modify it as the user pleases. The ``default`` implementation will return the already generated basic footer for each level. 

The user could make changes such as:
- Introducing a new ituff line.
- Modifying default ituff and deleting lines.

The idea for each extension method is to take in the whole string for each ituff level, the user can then use C# code to modify the string. Lastly return the resulting string for the footer level.

**Note:** If a user decides to modify the base ituff, it becomes **the users responsability to make sure any new line or modification is compatible with ituff format.**

# <span id="Hooks" class="anchor"></span> **TPL Samples**

```python
Import PrimeLotEndDatalog.xml;

Test PrimeLotEndDatalog DATALOG_LOTENDFLOW_INSTANCE
{
}
```

# <span id="FlowOrder" class="anchor"></span> **Test Program Flow order requirements**

The image below ilustrates the order requiremetns for Prime instances in order to generate correctly the ITUFF <font size="3"><span style="color:OrangeRed">Lot </font> testing session <font size="3"><span style="color:OrangeRed">Footer</font>.

1. PrimeLotEndDatalogTestMethod
1. PrimeLotEndFinalizeTestMethod
1. HMI_ituff (<font size="3"><span style="color:OrangeRed">TOS3 Only</font>)

::: mermaid
	graph LR  
	classDef PRIME fill:#005B85;
	classDef TOS fill:#B24501;
		subgraph Lot End Subflow
			direction LR		
			id2(PrimeLotEndDatalogTestMethod):::PRIME -->
			id3(PrimeLotEndFinalizeTestMethod):::PRIME--> 
			id4("HMI_ituff (TOS3 Only)"):::TOS
		end
:::

:warning: <font size="3"><span style="color:OrangeRed"> **Important Note for TOS3** </span></font> :warning:
>For HDMT the ITUFF and PUDL control is handled by TOS through the HMI Test Methods. Instances must exist on the Start and End Lot Flows to create and close the ITUFF streams.
>
>HMI_Ituff (OperationMode = LOTTEST_END) instance must be after the PrimeLotEndFinalizeTestMethod instance.  

# <span id="Hooks" class="anchor"></span> **Exit Ports**

The LotEndatalog test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**              |
| ------------- | ------------- | ---------------------------- |
| **-1**        | ***Error***   | Any software condition error |
| **0**         | ***Fail***    | Failing condition            |
| **1**         | ***Pass***    | Passing condition            |

# <span id="Hooks" class="anchor"></span> **Additional Dependencies**

This test method has a dependency with all Device TMs from PRIME. The use of LotEndDatalog without Device TMs is not a supported setup, **any attempt to not follow this rule can cause errors**. 

**It is also intended to be used before PRIME's LotEndFinalize TM.**

# <span id="VersionTracking" class="anchor"></span> **Version tracking**

| **Date**       | **Version** | **Author**     | **Comments** |
| -------------- | ----------- | -------------- | ------------ |
| Mar 10, 2022 | 1.0.0| jabarran| LotEndDatalog TM added. | 
| Aug 12, 2022 | 1.0.1 | hramirez | Adding Flow order documentation |
| Aug 13, 2024 | 13.01.00 | ckhoh    | Update Flow order documentation |

# <span id="Hooks" class="anchor"></span> **Acronyms**

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **HDMX**: Next Generation HDMT
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System
  - **TM**: Test Method
  - **SCVar**: Station Controller uservar.
  - **MTT**: Multi trial test.