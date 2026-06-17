<h1>Prime Test-Method Specification REP</h1>
Revision 1.0.0
This Test Method is inherit from MPE PGM PCH Team.

March 2025

[[_TOC_]]

## Methodology

The Prime HptpImpedance test method is to cater for RXRTERM/TXPD/TXPU characterization, it require measurement config file and patmod file.

2 supported dc measurement mode:
1. Dc measurement setup thru level test condition. (Recommended by TOS for better test time.)
2. Dc measurement setup thru direct pin attribute modification, attribute and value need to set in measurement config file (MeasConfigFile).

When ```DcLevels``` parameters defined, HptpImpedance test method will use dc measurement setup thru level test condition, else it will use direct pin attribute modification.

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the HptpImpedance test method

| **Parameter Name** | **Required?** | **Type** | **Default Value** | **Comments** |
| ------------------ | ------------- | -------- | ----------------- | -------------|
|MaskPins            | No | String | | Gets or sets comma separated pins for mask. |
|FuncLevel | Yes | String | | Specifies the level name for digital level setup. |
|DcLevels | No | String | | Specifies the level for pin measurement setup only. **Number of levels** must be equivalent to **number of settings** in MeasConfigFile as each dc level will assign to each setting respectively.|
|TimingsTc | Yes | String | | Specifies the timing name. |
|Patlist | Yes | String | | Specifies the plist name. |
|TargetResistance | Yes | Integer | | Specifies target resistance for the termination circuit. |
|EndSequenceLevel | No | String | | End sequence level test consition. |
|MeasConfigFile | Yes | String | | Specifies the json file for PMU measurement. |
|MeasConfigSet | Yes | Plist | | Specifies the config set name for RX PD PMU Calibration. |
|PatConfigName | Yes | TimingCondition | | Specifies the patmod config name for the termination field register. |

## Execute FlowChart
::: mermaid
flowchart TD
    A[Execute FuncLevel] --> B[CODE=0]
    B --> C{While CODE <= 63}
    C -->|Yes| D[Patmod 6 bits binary to the designated location of the termshift register]
    D --> E[Execute plist and apply attributes/level test condition to perform PMU measurement]
    E --> F[VSIM = 0.39V]
    F --> G[VSIM = 0.41V]
    G --> H[Compute RTERM]
    H --> I[CODE +=1]
    I --> J[Datalog ituff]
    J --> C
    C -->|No| K[End]
:::

## Datalog output

Ituff output,<br>
Will walk through code 0 to 63 with ituff format as such:<br>
	2_tname_<pinname>_CALCODE_\<code\><br>
	2_strgval_V0|\<inputVoltage0\>|I0|\<outputCurrent0\>|V1|\<inputVoltage1\>|I1|\<outputCurrent1\>|R|<TerminationResistance>|Rraw|\<TerminationResistanceWithoutRstray\>|MEAN|\<IfMeasIsBasedOnMeanValue\>

```
2_tname_DFXPHY_WRAP_XX_I_RX_0_CALCODE_0
2_strgval_V0|0.19|I0|0.0025|V1|0.21|I1|0.00275|R|79|RRaw|80|MEAN|False\
```

## Patmod file (.patmod.json)
```json
{
		"Configurations": [
		{
				 "Name": "RxTermCharPatmod",
				 "ConfigurationElement": [
				  {
							 "Type": "PINDATA",
							 "IsChannelLink": false,
							 "Pin": "DFXPHY_WRAP_XX_I_DFX_CONTROL",
							 "StartAddress": "91564",
							 "StartAddressOffset": 0,
							 "EndAddress": "93363",
							 "EndAddressOffset": 0,
							 "Ratio": 300,
							 "PatternsRegEx": [
										   ".*DFXPHY_RXTERM_CHAR"
							  ]
				   }
			  ]     
		 }
}
```
```RxTermCharPatmod``` will be insert to ```PatConfigName``` instance parameter.

## MeasConfigFile (.json)
```json
{
"ConfigurationSets": [
{
	  "ConfigurationName": "RXCHAR",
	  "Measurements": [
		{
		  "Settings": [
			{
			  "Pins": [
				"dfxphy_all_rx_dpins",
		              ],
			  "PinSetting": {
				"OPMode": "VSIM",
				"PreMeasurementDelay": 0.02,
				"ForceValue": 0.19,
				"LimitHigh": 0.0399,
				"LimitLow": -0.0399,
				"ClampHigh": 0.04,
				"ClampLow": -0.04,
				"IRange": "IR40mA",
				"SamplingCount": 2
			  }
			},
			{
			  "Pins": [
				"dfxphy_all_rx_dpins",
		  			  ],
			  "PinSetting": {
				"OPMode": "VSIM",
				"PreMeasurementDelay": 0.02,
				"ForceValue": 0.21,
				"LimitHigh": 0.0399,
				"LimitLow": -0.0399,
				"ClampHigh": 0.04,
				"ClampLow": -0.04,
				"IRange": "IR40mA",
				"SamplingCount": 2
			  }
			}
		  ]
		}
	  ]
	}
  ]
}
```

```RXCHAR``` will be insert to ```MeasConfigSet``` instance parameter. <br>
```Pins```, ```OPMode```, ```LimitHigh```, and ```LimitLow``` are **required** parameters.
```ForceValue```, ```ClampHigh```, ```ClampLow```, ```IRange```, and ```SamplingCount``` are **optional** parameters if using level test condition setup.

## Custom User Code Hooks
N/A

## TPL Samples
**TPL Sample 1:**

HptpImpedance Test Method(default configuration):
```
CSharpTest PrimeHptpImpedanceTestMethod Execute_VSIM_Characterization_P1
{
	LogLevel = "Enabled";
	FuncLevel = "HptpImpedance::basic_hptp_lvl_nom";
	TimingsTc = "HptpImpedance::basic_hptp_timing_0p667ns"; 
	Patlist = "passing_plist"; 
	TargetResistance = 50;
	MeasConfigFile = GetEnvironmentVariable("~HDMT_TPL_DIR") + "/TestPrograms/HptpImpedance/Modules/HptpImpedance/InputFiles/RXChar.json";
	MeasConfigSet = "RXCHAR";
	PatConfigName = "RxTermCharPatmod";
}
```

**TPL Sample 2:**

HptpImpedance Test Method(with DcLevels):
```
CSharpTest PrimeHptpImpedanceTestMethod Execute_VSIM_Characterization_P1
{
	LogLevel = "Enabled";
	FuncLevel = "HptpImpedance::basic_hptp_lvl_nom";
	DcLevels = "HptpImpedance::hptp_dc_impedance_lvl_1, HptpImpedance::hptp_dc_impedance_lvl_2";
	TimingsTc = "HptpImpedance::basic_hptp_timing_0p667ns"; 
	Patlist = "passing_plist"; 
	TargetResistance = 50;
	MeasConfigFile = GetEnvironmentVariable("~HDMT_TPL_DIR") + "/TestPrograms/HptpImpedance/Modules/HptpImpedance/InputFiles/RXChar.json";
	MeasConfigSet = "RXCHAR";
	PatConfigName = "RxTermCharPatmod";
}
```

## Exit Ports

The test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **0**         | ***Fail***      | General failure            |
| **1**         | ***Pass***      | Passing rterm characterization            |
| **2**         | ***Fail***      | Fail for pattern execution |
| **3**         | ***Fail***      | Fail for measurement limits |

  
## Additional Dependencies

Patmod configurations that is in patmod input file need to be load in as aleph file.

## Version tracking

| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| March, 2025 | 13.02.00       | Khor, Fang Hua |https://dev.azure.com/mit-us/PRIME/_workitems/edit/52775|

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System