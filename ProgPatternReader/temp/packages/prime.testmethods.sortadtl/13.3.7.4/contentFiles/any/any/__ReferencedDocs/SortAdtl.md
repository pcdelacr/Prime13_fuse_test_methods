## Rev 3.1

# ADTL Adaptive Test Limit

## Methodology

The ADTL template is template that can do an adaptive kill with incoming data. The template takes in various number of inputs from literals, DFF, SharedStorage, or UserVars.

## KILL 

1. Any invalid calculation: infinity or non-real numbers OR failures exit port 0.
1. Supports an ABOVE and BELOW kill line to detect ANY outlier. ABOVE is the traditional ADTL kill line.
1. Fail Flag Decode:
   1. 0 – Fail - The calculation fails (soft fail, GSDS token not found).
   1. 1 – Pass - The comparison condition is met. The exit will be a pass port.
   1. 2 – AboveFail - Traditional ADTL Kill: The comparison condition is not met in kill, Vmin Measured above ADTL line.
   1. 3 – BelowFail - The comparison condition is not met in kill, Vmin Measured below ADTL line.
   1. 4 – BothFail - Within a single test suite, both Above and Below fails have been encountered.
   1. 5 - EDCFailAbove - The comparison condition is not met in EDC for an Above Kill Line, but the test result port will always be a pass port in EDC mode.
   1. 6 - EDCFailBelow - The comparison condition is not met in EDC for a Below Kill Line, but the test result port will always be a pass port in EDC mode.
   1. 8 - Invalid - Invalid calculation encountered ie. NaN / Infinite.
   1. 9 - SkipTesting - Invalid / missing GSDS token values encountered. Will Exit Port 1.
1. KILL_LIMIT/VMIN Decode
   1. Double – Actual Value, Precision set to 4 ie: 0.6400
   1. Whole Numbers - Invalid Vmins. Will print as actual value with 0 precision ie: -5555, -6666, -9999, 10, etc.
      1. Values -5 > x > 5 are considered invalid and will automatically Skip Calculations.
1. ITUFF cannot be disabled. Depending on type of kill [Below, Above, Both] will print accordingly:

## PRINT OUTPUT
There are 2 cases for what may be printed, depending on if you are killing for an Above ADTL kill line or Below ADTL kill line.

<span style="color: red;">For printing only in the case of an Above Kill, it will be colored Red</span>.

<span style="color: green;">For printing only in the case of a Below Kill, it will be colored Green</span>.

If you are killing for BOTH [Comparison: BOTH], then both red and green text will be printed.

### DEBUG PRINT
<pre style="border: 1px solid gray; padding: 5px; background-color: ##05004f; font-family: monospace;"> 
[2024-Jul-24 11:11:28.916][DUT: 1]
Evaluating test: <TestName> with kill = &ltTrue or False based on Json&gt
<span style="color: red;">[2024-Jul-24 11:11:28.916][DUT: 1]        ABOVE KILL OFFSET: &ltValue of Offset defined in Json&gt
[2024-Jul-24 11:11:28.916][DUT: 1]        ABOVE KILL VALUE: &ltTab delimited kill value&gt </span>
<span style="color: green;">[2024-Jul-24 11:11:28.916][DUT: 1]        BELOW KILL OFFSET: &ltValue of Offset defined in Json&gt
[2024-Jul-24 11:11:28.916][DUT: 1]        BELOW KILL VALUE: &ltTab delimited kill value&gt</span>
[2024-Jul-24 11:11:28.916][DUT: 1]           MEASURED: &ltTab delimited measured Vmin values&gt 
[2024-Jul-24 11:11:28.916][DUT: 1]         COMPARISON: &ltTab Delimited Comparison type used [ABOVE, BELOW, BOTH]&gt
[2024-Jul-24 11:11:28.916][DUT: 1]               FLAG: &ltTab Delimited Flag for each vmin&gt
[2024-Jul-24 11:11:28.916][DUT: 1]             STATUS: &ltTab Delimited corresponding status for each Flag&gt
[2024-Jul-24 11:11:28.916][DUT: 1]             ADTL_OUTLIER_&ltAggregateToken&gt_&ltTestName&gt_SIGMA_&ltDelimited_Sigma_Values&gt
[2024-Jul-24 11:11:28.916][DUT: 1]             PORT: &ltPort output decided by this one test&gt
</pre>
### DEBUG PRINT EXAMPLE

<pre style="border: 1px solid gray; padding: 5px; background-color: ##05004f; font-family: monospace;"> 
[2024-Jul-24 11:11:28.916][DUT: 1]
Evaluating test: <TestName> with kill = True
<span style="color: red;">[2024-Jul-24 11:11:28.916][DUT: 1]        ABOVE KILL OFFSET: 0.12
[2024-Jul-24 11:11:28.916][DUT: 1]        ABOVE KILL VALUE: 0.8909		0.8909		0.8909 </span>
<span style="color: green;">[2024-Jul-24 11:11:28.916][DUT: 1]        BELOW KILL OFFSET: -0.14
[2024-Jul-24 11:11:28.916][DUT: 1]        BELOW KILL VALUE: 0.6309		0.6309		0.6309</span>
[2024-Jul-24 11:11:28.916][DUT: 1]           MEASURED: 0.7000		0.8910		0.6308
[2024-Jul-24 11:11:28.916][DUT: 1]         COMPARISON: BOTH			BOTH			BOTH
[2024-Jul-24 11:11:28.916][DUT: 1]               FLAG: 1			2			3
[2024-Jul-24 11:11:28.916][DUT: 1]             STATUS: Pass			AboveFail			BelowFail
[2024-Jul-24 11:11:28.916][DUT: 1]             ADTL_OUTLIER_ARR_Test1_MD_With_NonZero_SlopeIDV_DiffSS_SIGMA_-5_9_-10
[2024-Jul-24 11:11:28.916][DUT: 1]             PORT: BothFail
</pre>

### ITUFF PRINT
<pre style="border: 1px solid gray; padding: 5px; background-color: ##05004f; font-family: monospace;">
2_tname_&ltFlow_Instance&gt_ADTL_OUTLIER_&ltAggregateToken&gt_&ltTestName&gt_SIGMA
2_strgval_&ltDelimited_Sigmas_from_Best_fit&gt
<span style="color: red;">2_tname_&ltFlow_Instance&gt_&ltTestName&gt_ABOVE
2_strgval_&ltDelimited_IDV_Values&gt|&ltDelimited_Vmin_Values&gt|&ltDelimited_AboveKill_Vmin_Threshold&gt|&ltKillOrEDC&gt|&ltDelimited_PASS_FAIL_Status&gt</span>
<span style="color: green;">2_tname_&ltFlow_Instance&gt_&ltTestName&gt_BELOW
2_strgval_&ltDelimited_IDV_Values&gt|&ltDelimited_Vmin_Values&gt|&ltDelimited_BelowKill_Vmin_Threshold&gt|&ltKillOrEDC&gt|&ltDelimited_PASS_FAIL_Status&gt</span>
</pre>
### ITUFF PRINT EXAMPLE
<pre style="border: 1px solid gray; padding: 5px; background-color: ##05004f; font-family: monospace;">
2_tname_PVAL_COMMON::SPEXADTL_P4_ADTL_OUTLIER_ARR_MD_With_NonZero_SlopeIDV_DiffSS_SIGMA
2_strgval_-5_9_-10
<span style="color: red;">2_tname_PVAL_COMMON::SPEXADTL_P4_MD_With_NonZero_SlopeIDV_DiffSS_ABOVE
2_strgval_15000_15000_15000|0.7000_0.8910_0.6308|0.8909_0.8909_0.8909|KILL|PASS_FAIL_PASS</span>
<span style="color: green;">2_tname_PVAL_COMMON::SPEXADTL_P4_MD_With_NonZero_SlopeIDV_DiffSS_BELOW
2_strgval_15000_15000_15000|0.7000_0.8910_0.6308|0.6309_0.6309_0.6309|KILL|PASS_PASS_FAIL</span>
</pre>

### End of Template ITUFF PRINT
After all tests have completed running, Template will print this to ITUFF.

Number of Aggregate Token printing depends on how many Aggregate tokens are defined and calculated for in JSON.
<pre style="border: 1px solid gray; padding: 5px; background-color: ##05004f; font-family: monospace;">
2_tname_&ltFlow_Instance&gt_ADTL_OUTLIER_&ltAggregateToken&gt_MAX_SIGMA:
2_mrslt_&ltMax Sigma Value for this Aggregate Token&gt
[2024-Jul-24 11:11:28.924][DUT: 1]Printed to Ituff:
2_tname_&ltFlow_Instance&gt_ADTL_OUTLIER_&ltAggregateToken&gt_MIN_SIGMA:
2_mrslt_&ltMin Sigma Value for this Aggregate Token&gt
[2024-Jul-24 11:11:28.925][DUT: 1]Printed to Ituff:
2_tname_&ltFlow_Instance&gt_ADTL_OUTLIER_MAX:
2_mrslt_&ltAbsolute max Sigma Value experienced&gt
[2024-Jul-24 11:11:28.925][DUT: 1]Printed to Ituff:
2_tname_&ltFlow_Instance&gt_ADTL_OUTLIER_MIN:
2_mrslt_&ltAbsolute min Sigma Value experienced&gt
</pre>

### End of Template ITUFF PRINT EXAMPLE
<pre style="border: 1px solid gray; padding: 5px; background-color: ##05004f; font-family: monospace;">
2_tname_PVAL_COMMON::SPEXADTL_P4_ADTL_OUTLIER_ARR_MAX_SIGMA:
2_mrslt_12.0000
[2024-Jul-24 11:11:28.924][DUT: 1]Printed to Ituff:
2_tname_PVAL_COMMON::SPEXADTL_P4_ADTL_OUTLIER_ARR_MIN_SIGMA:
2_mrslt_-14.0000
[2024-Jul-24 11:11:28.925][DUT: 1]Printed to Ituff:
2_tname_PVAL_COMMON::SPEXADTL_P4_ADTL_OUTLIER_MAX:
2_mrslt_12.0000
[2024-Jul-24 11:11:28.925][DUT: 1]Printed to Ituff:
2_tname_PVAL_COMMON::SPEXADTL_P4_ADTL_OUTLIER_MIN:
2_mrslt_-14.0000
</pre>

## Exit Port Definitions

|Port|Definition|
| :---: | --- |
|1|Pass|
|2|ABOVE fail encountered|
|3|BELOW fail encountered|
|4|BOTH an ABOVE and BELOW fail encountered|
|5|EDC (Kill = 0) Fail (ABOVE, BELOW, or BOTH) encountered AND all other tests passed|

## Parameter

### Parameter Description

|Parameter Name|Required?|Default|Options|Description|
| :---: | :---: | :---: |---|---|
|ConfigFile|Y|Empty|JsonFile|File path to the configuration file with the setup of different predictive kill limit or upper/lower limits for tests.|
|Resolution|N|4|Any integer|Used to define the resolution of the final values.|
|LogLevel|N|ENABLED|ENABLED<br>DISABLED|Used to determine if Tester will Print Debug Messages.|
|YMinMax|N|"0,5"|"*min*,*max*"|Minimum and maximum range for validity on the Y-Component.|
|XMinMax|N|"0,200000"|"*min*,*max*"|Minimum and maximum range for validity on the X-Component.|
|PrintSigmas|N|"Enabled"|"Enabled"<br>"Disabled"|Enable/Disable Sigma-related Ituff printing to reduce Ituff content.|
|PrintPriorityGSDS|N|"Disabled"|"Enabled"<br>"Disabled"|Enables push to GSDS token of first encountered priority failed test name and priority value.*|

*PrintPriorityGSDS Enabled SSToken will have this format: \<InstanceName>_PRIORITYFAIL = "\<TestName>|\<PriorityValue>"<br>
Example using OTPL Instance name and Json test name: SPEXADTL_P4_PRIORITYFAIL = ALT_BOTH_INV|0

### OTPL Example

```
Version 1.0;

ProgramStyle = Modular;

TestPlan SortAdtl;

CSharpTest PrimeSortAdtlTestMethod UserVar_Below_Fail_P3
{
    ConfigFile = "InputFiles\\JsonFiles\\USERVAR_BELOW_FAIL.json";
    LogLevel = "Enabled";
}

FlowItem UserVar_Below_Fail_P3 UserVar_Below_Fail_P3
	{
		Result -2
        {
            Property PassFail = "Fail";
            Return 1;
        }
        Result -1
        {
            Property PassFail = "Fail";
            Return 1;
        }
        Result 0
        {
            Property PassFail = "Fail";
            Return 1;
        }
        Result 1
        {
            Property PassFail = "Pass";
            Return 1;
        }
        Result 2
        {
            Property PassFail = "Fail";
            Return 1;
        }
        Result 3
        {
            Property PassFail = "Fail";
            Return 1;
        }
        Result 4
        {
            Property PassFail = "Fail";
            Return 1;
        }
        Result 5
        {
            Property PassFail = "Fail";
            Return 1;
        }
	}
```
## Configuration File

### Config File Fields
|Parameter Name|Required?|Default|Input Source|Options|Description|
| :---: | :---: | :---: | :---: | :---: |---|
|Name|Y|N/A|LITERAL|A-Z, 0-9, or _|A simple single word descriptive Name. Use in printing to ITUFF and Console|
|TestName|N|Empty|LITERAL|A-Z, 0-9, or _|A simple single word descriptive Name.<br> Not used within the template but for better organization in JSON file.|
|AggToken|N|Empty|String|A-Z, 0-9, or _|A string for MO's to define how Sigmas are organized.
|Y1|Y|N/A|UserVar<br>DFF<br>SharedStorage|Comma-Separated Double or just a double|An input Vmin to compare against the ADTL formula and must be the same size as the VectorSize or single.<sup>1</sup>|
|Y1a|N|N/A|UserVar<br>DFF<br>SharedStorage|Comma-Separated Double or just a double|An alternative Vmin input in the event of missing / invalid Vmins from Y1.<sup>1</sup>|
|X1|Y|N/A|UserVar<br>DFF<br>SharedStorage|Comma-Separated Double or just a double|An IDV input to compute ADTL value. Must be the same size as the VectorSize or single.<sup>1</sup>|
|X1a|N|N/A|UserVar<br>DFF<br>SharedStorage|Comma-Separated Double or just a double|An alternative IDV input in the event of missing / invalid IDV from X1.<sup>1</sup>|
|CompareTerm|Y|N/A|LITERAL|ABOVE<br>BELOW<br>BOTH|What kind of comparison against ADTL value will be used. The Measured values that don't meet the condition will be killed and the exit port will be the fail port (if kill = true).|
|M|Y|N/A|LITERAL|Comma-Separated Double|The Vmin Cloud Best-Fit slope used for the ADTL Formula.|
|B|Y|N/A|LITERAL|Comma-Separated Double|The Vmin Cloud Best-Fit intercept used for the ADTL Formula.|
|AboveKillOffset|*If CompareTerm is set to BOTH or ABOVE*|N/A|LITERAL|Double|The offset from the best fit line that will be the ADTL kill value.|
|BelowKillOffset|*If CompareTerm is set to BOTH or BELOW*|N/A|LITERAL|Double|The offset from the best fit line that will be the ADTL kill value.|
|VectorSize|Y|1|LITERAL|1<br>Any Integer and equal to ExpandedVectorSize|The expected input array size.|
|ExpandedVectorSize|Y|1|LITERAL|Any Integer|Expands the output size to the value specified.|
|Kill|Y|N/A|LITERAL|true<br>false|Enables kill or edc to the equation of the ADTL equation. If it is in EDC, always exit by port 1.|
|MaxValue|N|double.MaxValue|LITERAL|Any double|If the result exceeds the MaxValue then the result sets the MaxValue.|
|MinValue|N|double.MinValue|LITERAL|Any double|If the result is less than the MinValue then the result sets the MinValue.|
|StepSize|N|0.02|LITERAL|Positive double|Used to define the step size. This is used to round the Calculated Sigma value to the nearest number (up in the case it's above the best fit and down in the case it's below the best fit) divisible by the step size.|
|Priority|N|int.MaxValue|LITERAL|Any int|Used to define priority value (Lower value = Higher priority).|

<sup>1</sup>Allowed formats for X / Y variables: "DUT." + "\<Token_Name\>". 
<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Shared Storage Token name may include the following characters: '_', '^', '.', '|'
# Json example in EDC 
```json
{
    "Tests": [
        {
            "NAME": "VMIN_ABOVE_EDC_ABOVE_FAIL",
            "AggToken": "ARR",
            "Y1": "DUT.VMIN_ABOVE_EDC_ABOVE_FAIL",
            "B": [ 0.46 ],
            "M": [ 0 ],
            "X1": "DUT.SMP_ABOVE_EDC_ABOVE_FAIL",
            "AboveKillOffset": 0.08,
            "StepSize": 0.02,
            "VectorSize": 1,
            "ExpandedVectorSize": 1,
            "CompareTerm": "ABOVE",
            "Kill": 0
        },
        {
            "NAME": "VMIN_BELOW_EDC_BELOW_FAIL",
            "TestName": "Test2"
            "AggToken": "SCN",
            "Y1": "DUT.VMIN_BELOW_EDC_BELOW_FAIL",
            "B": [ 0.54 ],
            "M": [ 0 ],
            "X1": "DUT.SMP_BELOW_EDC_BELOW_FAIL",
            "BelowKillOffset": -0.2,
            "StepSize": 0.02,
            "VectorSize": 1,
            "ExpandedVectorSize": 1,
            "CompareTerm": "BELOW",
            "Kill": 0
        }
    ]
}
```

# Json example in KILL
```json
# Sharestorage Example
{
    "Tests": [
        {
            "NAME": "BOTH_PASS",
            "AggToken": "ARR",
            "Y1": "DUT.VMIN_BOTH_PASS",
            "B": [ 0.48 ],
            "M": [ 0 ],
            "X1": "DUT.SMP_BOTH_PASS",
            "AboveKillOffset": 0.14,
            "BelowKillOffset": -0.08,
            "StepSize": 0.02,
            "VectorSize": 1,
            "ExpandedVectorSize": 1,
            "CompareTerm": "BOTH",
            "Kill": 1,
            "MaxValue": 1.12,
            "Priority": 1
        },
        {
            "NAME": "ALT_BOTH_INV",
            "AggToken": "ARR_SSA",
            "Y1": "DUT.INVALIDY_T4",
            "Y1a": "DUT.VALID_ALTY_T4",
            "B": [ 0.46 ],
            "M": [ 0 ],
            "X1": "DUT.INVALIDX_T4",
            "X1a": "DUT.VALID_ALTX_T4",
            "AboveKillOffset": 0.08,
            "StepSize": 0.02,
            "VectorSize": 1,
            "ExpandedVectorSize": 1,
            "CompareTerm": "ABOVE",
            "Kill": 1,
            "MinValue": 0.7,
            "Priority": 0
        }
    ]
}
# UserVar Example
{
    "Tests": [
      {
        "NAME": "NEAR_KILL",
        "AggToken": "ARR",
        "Y1": "SCVars.ADTLVmin",
        "B": [ 0.5 ],
        "M": [ 0 ],
        "X1": "SCVars.ADTLSmp",
        "AboveKillOffset": 0.1,
        "StepSize": 0.02,
        "VectorSize": 3,
        "ExpandedVectorSize": 3,
        "CompareTerm": "ABOVE",
        "Kill": 1
      }
    ]
}
```
