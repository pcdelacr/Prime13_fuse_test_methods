[[_TOC_]]

## Prerequisites

<p style="font-size: 20px;"><span style="color: #cf6679;">This Service makes use of Aleph initialization for parsing and validation of the configuration file. Please refer to Aleph documentation for details.</span></p>

[Link to Aleph Documentation](/[Internal%2DOnly]-Prime-Developers-Wiki/Wiki-master/General/Special-ENV-Variables-in-Prime)

Starting **PRIME 7.1** it is **no longer supported** to user VDAC UF from EVG in combination with Prime Voltage Service. Test program must only use one of them.
#Voltage handler modes
The Voltage Service provides the methods to create objects capable of applying voltages to areas (domains) of the DUT (Device Under Test).

Depending on the product under test and its requirements, the voltage can be applied through different ways:
## DPS mode
The voltage is applied directly to a power pin (connected to a Device Power Supply (DPS) resource in the tester) intended to supply the voltage to a specific area of the unit.
##Test Condition mode
In this mode, the voltage service changes the VForce value in the specified test condition. This can be used in conjunction with an External Trigger in the pattern and a TriggerMap, managed by the FunctionalService, to effectively apply the desired voltage.
## FIVR mode
FIVR stands for "Fully Integrated Voltage Regulator". In this case, the power pins for each area of the unit are not exposed directly to the tester, but instead, the power is supplied as a whole to the unit, and there are built-in voltage regulators in the package, which internally set the voltage to each power area (domain).
For this scenario, as there is no direct control of the voltage being set to each power pin, the unit itself provides the logic and instructions required to feed the desired voltage setting to each of the integrated voltage regulators. In order to set these per-domain independent voltages, the adequate pattern instructions values at the beginning of any plist execution must be configured with the desired voltage value conditions. Therefore to control these settings dynamically during testing flows, a pattern modify is required.

# Configuration files
PRIME provides one configuration file format to define all the FIVR domains of the product under test together with the required values for the internal calculation to convert from input voltage values to the internal values required to be set to the corresponding patterns, and the modified configuration references for each of them. This file is also used to define all DPS domains of the product under test, in this case only the name of the pin for a given domain is required. 
There are three optional sections that allow dynamic control over the Digital Linear Voltage Regulator (DLVR) pin values, modifying timing attribute values for timing pins and running pattern modify configurations based dynamically on an expression. 

PRIME also provides a second configuration file that is used to declare FIVR conditions, this can be used to set several FIVR domains from the domain configuration file under the same condition.

The following sections detail the configuration fields (JSON format) for each of these two configuration files. All sections are required to be populated unless stated as optional, both input files are compared via their own JSON schema. These files are consumed by Aleph, see [ALEPH Files](https://dev.azure.com/mit-us/PRIME/_wiki/wikis/PRIME.wiki/181/Special-ENV-Variables-in-Prime?anchor=aleph_files) for more information on setup:

## Voltage domains
Format file name: {customer file name}*.voltageDomain.json*
Example file:
```json
{
    "DisabledDomains": {
        "type": "UserVar",
        "value": "Voltage::TestVars.DisabledDomains"
    },
    "Domains" :
    [{
        "name" : "IO1",
        "pattern_modify" : {
            "initial_voltage" : {
                "multiplier" : 256,
                "configuration" : "PatModConfiguration0",
                "pattern_regex": "fivr.*",
                "number_of_targets" : 1                      
            }
        },
        "default_value" : 0.45 
    },
    {
        "name" : "IO2",
        "pattern_modify" : {
            "initial_voltage" : {
                "multiplier" : 256,
                "configuration" : "PatModConfiguration2",
                "pattern_regex": "fivr.*",
                "number_of_targets" : 1                
            }
        },
        "default_value" : 0.45
    },
    {
        "name" : "CORE0",
        "pattern_modify" : {
            "initial_voltage" : {
                "multiplier" : 256,
                "configuration" : "PatModConfiguration1",
                "number_of_targets": 1               
            },
            "offset" : {
                "type" : "calibration",
                "magnitude_configuration" : "MagnitudeConfiguration1",
                "magnitude_pattern_regex": "fivr.*",
                "sign_configuration" : "SignConfiguration1",
                "sign_pattern_regex": "fivr.*"
            }
        },
        "dac_calibration" : {
            "slope" : "SlopeDACValue",
            "offset" : "OffsetDACValue"
        },
        "default_value": 0.45
    },
    {
        "name" : "CORE1",
        "pattern_modify" : {
            "initial_voltage" : {
                "multiplier" : 256,
                "configuration" : "PatModConfiguration1",
                "number_of_targets": 1                
            },
            "offset" : {
                "type" : "calibration",
                "magnitude_configuration" : "MagnitudeConfiguration2",
                "sign_configuration" : "SignConfiguration2"
            }
        },
        "dac_calibration" : {
            "slope" : "SlopeDACValue",
            "offset" : "OffsetDACValue"
        },
        "default_value": 0.45
    },
    {
        "name" : "DPS_DOMAIN",
        "dps_pin" : "HDDPS_VLC_16ohm1"
    }],
    "DomainGroups" :
        [{
            "name": "I012",
            "domains" : [
                "IO1",
                "IO2"
            ]
        },
        {
            "name": "CORE01",
            "domains": [
                "CORE0",
                "CORE1"
            ]
        },
        {
            "name" : "DPS_GROUP",
            "domains": [
                "DPS_DOMAIN"
            ]
        }],
    "DlvrPins": [
        {
            "pin_name" : "VCC_H",
            "voltage_expression" :[
				{
					"expression_name" : "First_Expression",
					"expression_value" : "max(SA,CORE)+0.5"
				},
								{
					"expression_name" : "Second_Expression",
					"expression_value" : "min(SA,CORE)+0.5"
				}	
			],			
            "min": 0.5,
            "max" : 2.0,
            "step_size" : 0.1
        }
    ],
    "PatternModify" : [
        {
            "pattern_modify_name" : "PatternModify0",
            "module" : "Module",
            "group" : "SWITCH",
            "selector" : "0.675 > max(SA, CORE)",
            "set_point_for_true" : "ON_CONFIG_SET",
            "set_point_for_false" : "OFF_CONFIG_SET"
        }],
    "TimingAttributes" : [
        {
            "pin_name" : "ClockPin",
            "domain" : "DDR_STF",
            "attributes" : [
                {
                    "name" : "TFall",
                    "expression" : "(SA/0.5)*PERIOD",
                    "step_size" : 0.0000000001
                }
            ],
            "required_attributes" : [
                {
                    "name" : "EdgeCounter",
                    "value" : "Off"
                }
            ]
        }]
}
```
Field details:
- _**DisabledDomains**_: Optional grouping field. Used to provide disabled domain configuration settings defined in the current file.
  - _**type**_: Field to provide the chosen method to provide the disabled domain list. There are two available options to be used as input: <CODE class="json" style="font-family: Menlo, Consolas, &amp;quot;font-size: inherit;background-color: transparent;color: var(--text-primary-color,rgba(0, 0, 0, 0.9));padding: 0px"><SPAN  class="hljs-string" style="color: rgba(var(--palette-accent1-dark,168, 0, 0),1)">"UserVar"</SPAN></CODE> or <CODE class="json" style="font-family: Menlo, Consolas, &amp;quot;font-size: inherit;background-color: transparent;color: var(--text-primary-color,rgba(0, 0, 0, 0.9));padding: 0px"><SPAN  class="hljs-string" style="color: rgba(var(--palette-accent1-dark,168, 0, 0),1)">"Literal"</SPAN></CODE>.
  - _**value**_: In the case of literal, a comma separated list of domains to be disabled or in the userVar case the userVar name where the list of disabled domains is stored. In any case the list should look like: <CODE class="json" style="font-family: Menlo, Consolas, &amp;quot;font-size: inherit;background-color: transparent;color: var(--text-primary-color,rgba(0, 0, 0, 0.9));padding: 0px"><SPAN  class="hljs-string" style="color: rgba(var(--palette-accent1-dark,168, 0, 0),1)">"CORE0,CORE1"</SPAN></CODE>.
- _**Domains**_: Grouping field. Used to provide a tuple of domain definitions. This section is optional.
  - _**name**_: Field to provide a name for each FIVR domain or DPS domain. This name will be used to match the inputs provided to the service.
  - _**dps_pin**_: Field to provide the DPS pin for the given DPS domain name. This is the pin that will be handled when the DPS domain name is used as input to the service. This section can only be used with DPS domains.
  - _**pattern_modify**_: Grouping field. The main purpose of FIVR voltage objects is to do pattern setup modifications. This field is used is to group all the required definitions of each pattern modification needed for a specific domain (for both, calculations for converting from voltage to internal units and the corresponding target pattern modify configurations). Depending on the product and the domain, there are up to three pattern modification targets (to set internal DUT FIVR configuration registers): one for an initial value (always required) and two additional (optional) configurations for offset magnitude and offset sign.
    - _**initial_voltage**_: Grouping field. Used for grouping pattern modification configuration settings of initial voltage value.
      - **_multiplier_**: Field to provide an integer value used for conversion calculations (explained in calculations section)
      - _**configuration**_: Field to provide the corresponding Pattern modify configuration reference name, which must be defined using Prime PatModify or FuseConfig infrastructures.
      - **_pattern_regex_**: This field is optional by default, but it is mandatory if the fuse configuration mode is set to `FLAME` and the `configuration` is a fuse configuration (see PatConfigService). This field is the way the voltage service has to provide the pattern's regular expression to the PatConfigService to filter the patterns to apply the initial voltage.
      - **_number_of_targets_**: Field to provide an integer value used for conversion from calculated decimal value to binary. This is because the pattern modify configuration could include more than one register target, where the same value must be set. So, the calculated decimal value is converted to the binary length of the total size of the configuration divided by the number of targets and then the resulting binary is concatenated that same number of times.
   - **_default_value_**: Field to provide a positive double value, which will be used instead of any negative number that is attempted to be applied to the current condition. This field is only used in FIVR domains.
    - **_offset_**: Grouping field. Used for grouping offset voltage value pattern modification configuration settings. This section is optional and only used in FIVR domains.
      - _**type**_: Field to specify methodology to be used for conversion calculation of required offset value (explained in calculations section). There are two available options: <CODE class="json" style="font-family: Menlo, Consolas, &amp;quot;font-size: inherit;background-color: transparent;color: var(--text-primary-color,rgba(0, 0, 0, 0.9));padding: 0px"><SPAN  class="hljs-string" style="color: rgba(var(--palette-accent1-dark,168, 0, 0),1)">"vid"</SPAN></CODE> or <CODE class="json" style="font-family: Menlo, Consolas, &amp;quot;font-size: inherit;background-color: transparent;color: var(--text-primary-color,rgba(0, 0, 0, 0.9));padding: 0px"><SPAN  class="hljs-string" style="color: rgba(var(--palette-accent1-dark,168, 0, 0),1)">"calibration"</SPAN></CODE>.
      - _**magnitude_configuration**_: Field to provide the corresponding Pattern modify configuration reference name, which must be defined using Prime PatModify or FuseConfig infrastructures.
      - _**magnitude_pattern_regex**_: This field is optional by default, but it is mandatory if the fuse configuration mode is set to `FLAME` and the `magnitude_configuration` is a fuse configuration (see PatConfigService). This field is the way the voltage service has to provide the pattern's regular expression to the PatConfigService to filter the patterns to apply the magnitude.
      - _**sign_configuration**_: Optional field to provide the corresponding Pattern modify configuration reference name, which must be defined using Prime PatModify or FuseConfig infrastructures.
      - _**sign_pattern_regex**_: This field is optional by default, but it is mandatory if the fuse configuration mode is set to `FLAME` and the `sign_configuration` is a fuse configuration  (see PatConfigService). This field is the way the voltage service has to provide the pattern's regular expression to the PatConfigService to filter the patterns to apply the sign.
      - _**invert_sign**_: Optional field to invert the current sign convention. If this field is set to <CODE class="json" style="font-family: Menlo, Consolas, &amp;quot;font-size: inherit;background-color: transparent;color: var(--text-primary-color,rgba(0, 0, 0, 0.9));padding: 0px"><SPAN  class="hljs-string" style="color: rgba(var(--palette-accent1-dark,168, 0, 0),1)">"TRUE"</SPAN></CODE> when the offset calculation is done the sign convertion wil be inverted. 
  - _**dac_calibration**_: Grouping field. Used for grouping of DAC calibration references. If any of the target internal registers to be set corresponds to a DAC units register, per DUT calibration inputs are required for conversion calculations. A FIVR calibration module must be responsible (early in DUT test flow) for measuring DAC register values for different voltage points, and to determine and store these linear calibration parameters. This section is optional and only used in FIVR domains.
      - _**slope**_: Keyword to access a double value in shared storage (LOT context). Used for conversion calculations (explained in calculations section)
      - _**offset**_: Keyword to access a double value in shared storage (LOT context). Used for conversion calculations (explained in calculations section)
- _**DomainGroups**_: Grouping field. Used to provide a tuple of domain group definitions, only one type of domain is supported per group, using DPS domains and FIVR domains in a group is not allowed. This section is optional.
  - _**name**_: Field to provide a name for each domain group. This name will be used to match the inputs provided to the service.
  - _**domains**_: Field to provide the domain names part of the domain group.
- _**DlvrPins**_: Grouping field. Used to provide a tuple of the DLVR pins to be used. This section is optional.
  - _**pin_name**_: Field to provide the name for each DLVR pin to be used. This name will be used to match inputs to the service. 
  - _**voltage_expression**_: Field to provide a voltage value based on the result of the input expression , few expression can be defined, the default one is the first defined in file (explained in RailSetups section).
  - _**min**_: Field to provide a minimum value to apply to DLVR pin. If the expression result is lower, this value will override the applied value to the DVLR pin.  
  - _**max**_: Field to provide a maximum value to apply to DLVR pin. If the expression result is higher, this value will override the applied value to the DLVR pin.
  - _**step_size**_: Field to provide a step size for the jumps in new values applied to DLVR pins (explained in RailSetups section).
- _**PatternModify**_: Grouping field. Used to provide a tuple of pattern modify configurations to use. This section is optional.
  - _**pattern_modify_name**_: Field to provide a name to identify the configuration. This name will be used to match inputs to the service. 
  - _**module**_: Field to provide the corresponding Pattern Modify configuration reference name which must be defined using Prime PatModify or FuseConfig infrastructures.
  - _**group**_: Field to provide the corresponding Pattern Modify configuration reference name which must be defined using Prime PatModify or FuseConfig infrastructures.
  - _**selector**_: Field to provide a boolean expression whose result will be used to determine which set point to use. 
  - _**set_point_for_true**_: Field to provide a set point when <CODE class="json" style="font-family: Menlo, Consolas, &amp;quot;font-size: inherit;background-color: transparent;color: var(--text-primary-color,rgba(0, 0, 0, 0.9));padding: 0px"><SPAN  class="hljs-string" style="color: rgba(var(--palette-accent1-dark,168, 0, 0),1)">"selector"</SPAN></CODE> evaluates to ```true```. Pattern Modify configuration reference name must be defined using Prime PatModify or FuseConfig infrastructures.
  - _**set_point_for_false**_: Field to provide a set point when <CODE class="json" style="font-family: Menlo, Consolas, &amp;quot;font-size: inherit;background-color: transparent;color: var(--text-primary-color,rgba(0, 0, 0, 0.9));padding: 0px"><SPAN  class="hljs-string" style="color: rgba(var(--palette-accent1-dark,168, 0, 0),1)">"selector"</SPAN></CODE> evaluates to ```false```. Pattern Modify configuration reference name must be defined using Prime PatModify or FuseConfig infrastructures.
- _**TimingAttributes**_: Grouping field. Used to provide a tuple of timing attributes to apply to different pins. This section is optional.
  - _**pin_name**_: Field to provide the name of the pin whose attributes will be modified. This name will be used to match inputs to the service. 
  - _**domain**_: Field to provide a domain name for each timing attribute. This domain name is used for calculating the ```PERIOD``` value in the expression (explained in the RailSetups section).
  - _**attributes**_: Grouping field. Used to provide a tuple of attributes to modify for the given pin.
    - _**name**_: Field to provide the name of the attribute to modify.
    - _**expression**_: Field to provide a value to apply to the attribute based on the expression result (explained in RailSetups section).
    - _**step_size**_: Field to provide a step size for the jumps in new values applied to the given attribute (explained in RailSetups section).
  - _**required_attributes**_: Grouping field. Used to provide a tuple of required attributes to modify for the given pin and timing attribute.
      - _**name**_: Field to provide the name of the attribute to modify.
      - _**expression**_: Field to provide a value to apply to the attribute.

## FIVR conditions
Format file name: {customer file name}*.fivrCondition.json*
Example file:
```json
{
  "Conditions": [
    {
      "name": "FIVR_VLOAD_CORE0123_LC",
      "domains": [
        {
          "name": "FIVR_VLOAD_CORE0_LC",
          "voltage": {
            "type": "Literal",
            "value": "0.75"
          }
        },
        {
          "name": "FIVR_VLOAD_CORE1_LC",
          "voltage": {
            "type": "SharedStorage",
            "value": "FIVR_VLOAD_CORE1_LC"
          },
          "guard_band": {
            "type": "Literal",
            "value": "0.0"
          }
        },
        {
          "name": "FIVR_VLOAD_CORE2_LC",
          "voltage": {
            "type": "Literal",
            "value": "0.85"
          },
          "guard_band": {
            "type": "SharedStorage",
            "value": "FIVR_VLOAD_CORE0_LC_GUARD_BAND"
          }
        },
        {
          "name": "FIVR_VLOAD_CORE3_LC",
          "voltage": {
            "type": "Literal",
            "value": "0.65"
          }
        }
      ]
    },
    {
      "name": "FIVR_VLOAD_CORE0123_Literal",
      "domains": [
        {
          "name": "FIVR_VLOAD_CORE0_LC",
          "voltage": {
            "type": "Literal",
            "value": "0.75"
          }
        },
        {
          "name": "FIVR_VLOAD_CORE1_LC",
          "voltage": {
            "type": "Literal",
            "value": "0.8"
          }
        },
        {
          "name": "FIVR_VLOAD_CORE2_LC",
          "voltage": {
            "type": "Literal",
            "value": "0.85"
          }
        },
        {
          "name": "FIVR_VLOAD_CORE3_LC",
          "voltage": {
            "type": "Literal",
            "value": "0.65"
          }
        }
      ]
    },
    {
      "name": "FIVR_GUARDBAND_VLOAD_CORE0_LC",
      "domains": [
        {
          "name": "FIVR_VLOAD_CORE0_LC",
          "voltage": {
            "type": "Literal",
            "value": "0.75"
          },
          "guard_band": {
            "type": "Literal",
            "value": "0.01"
          }
        }
      ]
    },
    {
      "name": "CONDITION_OF_GROUPS",
      "domains": [
        {
          "name": "DOMAIN_GROUP_CORE01",
          "voltage": {
            "type": "Literal",
            "value": "0.75"
          }
        },
        {
          "name": "DOMAIN_GROUP_CORE23",
          "voltage": {
            "type": "Literal",
            "value": "0.8"
          }
        }
      ]
    }
  ]
}
```
Field details:
- All defined FIVR conditions must be defined under the _**Conditions**_ tuple grouping.
  - _**name**_: Field to provide a name each FIVR available condition. This name will be used to match the single input provided to the service for condition input case.
  - _**domains**_: Grouping field. Used to group all domains to be set under the corresponding condition name.
    - _**name**_: Field to provide a name of a domain to be set. The given value must match an existing domain name defined in FIVR domains configuration file.
    - _**voltage**_: Field to provide the voltage value (static or dynamic) to be set for the corresponding domain if the service is used to apply corresponding FIVR condition. The value is going to be taken from the shared storage (specifically from the `Double` table and `DUT` context) or as a literal value as a Double.
       - type: "Literal" to take the value as a Double and "SharedStorage" to take the value as a shared storage key.
       - value: value to be applied / resolved from shared storage.
    - _**guard_band**_: Field to provide a guard band value (static or dynamic) which will be added to the `Value` field. The value is going to be taken from the shared storage (specifically from the `Double` table and `DUT` context) or as a literal value as a Double. This field is optional.
       - type: "Literal" to take the value as a Double and "SharedStorage" to take the value as a shared storage key.
       - value: value to be applied / resolved from shared storage.
#FIVR calculations

## Independent domains
In the ideal case there are no domain dependencies and with only one internal register per domain is enough to set the required voltage. There are two known common cases:
1. The register to set directly represents the required voltage value, in which case, the only conversion required is a multiplier to convert from the input voltage value to an integer value (number of steps that represent that voltage in the internal FIVR logic) required by the internal register (calibration adjustments for DAC would also be handled internally by internal FIVR logic).
2. The register to set must already consider the per DUT DAC calibration. So, the input voltage must be adjusted following a linear equation.

Both cases can be handled with one single formula:

$$initial\_voltage\_set\_value = input\_voltage * multiplier * dac\_calibration\_slope + dac\_calibration\_offset$$

Given that calibration values are optional, in this equation the default used values are:
- dac_calibration_slope = 1
- dac_calibration_offset = 0

With these default values the first case is addressed. For the second case it is expected that the user fills the calibration fields with appropriate references and sets the multiplier field to '1'.

## Dependent domains
In a less ideal case, some domains share the same internal register to set the required voltage, not allowing to use this register to set independent voltages for each of these domains. The workaround to be able to set independent voltages on such domains is through additional setting registers for each one, that allow to set an offset from the common value set to the shared register.
These dependent domains are identified directly from configuration files, grouping those that share a common pattern modify configuration under the 'initial_voltage' section.
If one of these dependent domains is the target for a voltage apply, all detected dependent domains would also be also applied, using the maximum value  (this choice could not be ideal and the common value to use could change in the future) from the grouped domains providing a target input voltage.

### Dependent domains with VID offset method

$$initial\_voltage\_set\_value = common\_voltage * multiplier$$

$$offset\_magnitud\_set\_value = \mid (input\_voltage - common\_voltage) * dac\_calibration\_slope \mid $$

$$
offset\_sign\_set\_value=
    \begin{cases}
      Positive, & \text{if}\ (input\_voltage - common\_voltage) \geq 0 \\
      Negative, & \text{otherwise}
    \end{cases}
$$

### Dependent domains with Calibration offset method

$$initial\_voltage\_set\_value = common\_voltage * multiplier$$

$$offset\_magnitud\_set\_value = \mid (input\_voltage - common\_voltage) * dac\_calibration\_slope + dac\_calibration\_offset \mid $$

$$
offset\_sign\_set\_value=
    \begin{cases}
      Positive, & \text{if}\ ((input\_voltage - common\_voltage) * dac\_calibration\_slope + dac\_calibration\_offset) \geq 0 \\
      Negative, & \text{otherwise}
    \end{cases}
$$


### Dependent domains without sign configuration fuse
Due to the _**sign configuration**_ field being optional the case where only a _**magnitude configuration**_ is provided can happen. In this case the previous methods of calculating apply, be it _**VID**_ or _**Calibration**_ still apply. Main difference is that if the pattern modify configuration size is _**N**_ bits only _**N-1**_ bits will be used for setting the offset magnitue. Meanwhile the sign value will be fit into **1 bit** and the **most significant bit** will contain the resulting sign value.

For example if you had a 10 bit magnitude configuration such as:

![image.png](./.attachments/image.png)

When the offset magnitude value is calculated it will be fit into bit **0** to bit **8**. Meanwhile the resulting offset sign will be set in the MSB bit **9**.

### Dependent domains with invert sign enabled

When invert sign is enabled the sign convention of both **Calibration** and **VID** is inverted when calculating the sign bit. This works for both cases where only a magnitude configuration is provided and where both magnitude and sign configurations are provided.

Current sign convertion works like this:
|Sign magnitue|Sign Bit Value|
|-|-|
|`Positive`|0|
|`Negative`|1|

When utilizing invert sign on an offset configuration the convention now becomes: 
|Sign magnitue|Sign Bit Value|
|-|-|
|`Positive`|1|
|`Negative`|0|

This needs to be configured **for each domain that requires this change**  when calculating sign offset value.

# Disabled Domains
Voltage Service supports an __optional__ feature where the user can provide a list of domains to disable. Disabling a domain means that the given domain information from the configuration file will not be loaded into memory. This causes all relevant operations that could be done to such a domain will be __ignored__. 

Imagine the case where there is a voltage object with 5 independant domains (CORE0-CORE5). Then for some reason we do not want to use CORE3 and CORE5 anymore. This means the disabled domain list would look like: <CODE class="json" style="font-family: Menlo, Consolas, &amp;quot;font-size: inherit;background-color: transparent;color: var(--text-primary-color,rgba(0, 0, 0, 0.9));padding: 0px"><SPAN  class="hljs-string" style="color: rgba(var(--palette-accent1-dark,168, 0, 0),1)">"CORE3,CORE5"</SPAN></CODE>. Lastly we have the following voltage object with some values to apply per domain as the following:
$$
Voltage\_targets =
\begin{cases}
      CORE0, & Enabled, & value\_to\_apply = 0.1V\\
      CORE1, & Enabled, & value\_to\_apply = 0.2V\\
      CORE2, & Enabled, & value\_to\_apply = 0.3V\\
      CORE3, & Disabled, & value\_to\_apply = 0.4V \\
      CORE4, & Enabled, & value\_to\_apply = 0.5V\\
      CORE5, & Disabled, & value\_to\_apply = 0.6V\\
\end{cases}
$$

When the voltage object is used to apply the domain values what will really happen is this:
$$
Voltage\_Values =
\begin{cases}
      CORE0, & 0.1V\\
      CORE1, & 0.2V\\
      CORE2, & 0.3V\\
      CORE3, & Ignored\\
      CORE4, & 0.5V\\
      CORE5, & Ignored\\
\end{cases} 
$$

 Instead of having to modify our voltage object it is possible to just provide a disabled domain list, be it via a literal value or a userVar. Note that the user still has to __provide enough voltage values__ for each voltage target, but the disabled domains will not be applied, simply ignored.

 __Note__ that disabling a domain that is part of a __domain group__ or part of a __FIVR condition__ will cause it to be ignored when using either of these.

# Negative values
Applying negative values to a Power Supply is not allowed, and because of that the customers need to consider the behavior when a negative value is sent to be applied to a pin or domain.

To begin with, the behavior between DPS and FIVR is different. When an IVoltage object is created to use a DPS pin, either as a DPS domain or DPS pin, the voltage object will perform a hardware measurement of the pin voltage value, and this value is saved to be applied in the case that a negative value is sent to be applied. In the case of FIVR, there is an optional field (with default value `0.0 V`) in the FIVR's definition, intended to set a safe value to be applied to the domain instead of the negative value.

When the negative value is sent to be applied to a domain group, in the case of the DPS domain group, the value to apply will be the saved voltage from the hardware measurement in the moment of the IVoltage object creation, for each DPS domains in the group. In the case of FIVR domain group, the value to applied will the `maximum` of the defaul values of the FIVR domains in the group.

## Negative values in repeated domains
A behavior to consider regarding the domains, is that the `repeated domain` feature is resolved during the creation of the IVoltage object, while `negative values` is resolved during the application of the voltages. To understand the interactions between these capabilities working together, let's understand these cases:

### Case Negative value to single domains
This case just override the negative value and no repeated management is required.

Following table shows incoming data:
|Target|Value|Default value|
|-|-|-|
|`SingleDomain`|-0.6|0.8|
|`OtherSingleDomain`|0.7|0.8|

Following table shows applied values:
|Target|Applied value|
|-|-|
|`SingleDomain`|0.8|
|`OtherSingleDomain`|0.7|

### Case 2: Same negative value to a repeated domain
This case presents a repeated domain where the value to apply is the same negative number. The resolution process is as follows: first, the maximun of the incoming values will be taken to apply because of the repetition domain, in this case `-0.6` is the maximum. Then, the is a resulted negative number that is going to be repleaced by the default value, so at the end `0.8` is applied once to the domain.

Following table shows incoming data:
|Target|Value|Default value|
|-|-|-|
|`SingleDomain`|-0.6|0.8|
|`SingleDomain`|-0.6|0.8|

Following table shows applied values:
|Target|Applied value|
|-|-|
|`SingleDomain`|0.8|

### Case 3: Different values to a repeated domain, one of them is negative
This case presents a repeated domain with two different values to be applied, one of them is negative. The resolution process is as follows: first, the maximun of the incoming values will be taken to apply because of the repetition domain, in this case `0.7` is the maximum. Then, because of the resulted number is positive, there is nothing else to be resolved, at the end `0.7` is applied.

Following table shows incoming data:
|Target|Value|Default value|
|-|-|-|
|`SingleDomain`|0.7|0.8|
|`SingleDomain`|-0.6|0.8|

Following table shows applied values:
|Target|Applied value|
|-|-|
|`SingleDomain`|0.7|

# Rail Configurations
Voltage Service calls ```Rail configurations``` to three kinds of configurations which are different to ```voltage domains```. Every rail, which are [DLVR Pins](#DLVR-Pins), [Pattern Modify](#Pattern-Modify) and [Timing-Attribute-Pins](#Timing-Attribute-Pins), has its own configuration section in [Domains configuration file](#FIVR-domains).

Voltage Service calls ```Rail configuration``` to this kind of data, because it is intended to execute them as a collection through the object returned by [CreateDomainObjectWithConditionAndRailSupport interface](#CreateDomainObjectWithConditionAndRailSupport-interface). It is possible to execute as many configurations as the customer needs.

## DLVR Pins
This rail configuration has the intention of applying a voltage value to a DPS pin where the value to apply could or couldn't depend on the applied voltage domain values.

The value to apply could be the result of the expression provided in the [configuration file](#FIVR-domains), but it could be overridden by the maximum and minimum values, which are values for protection. The expression result or minimum or maximum value, will be applied only if the difference between it and the previously applied value to the DLVR pin, is higher than the indicated step size. Also, the range defined by the minimum and the maximum protection value should be higher than the step size, otherwise, the service will raise an exception.

So, the minimum, maximum and step size must need to meet as follows:

* **minimun:** minimum <= maximun
* **maximum:** maximun >= minimun
* **step\_size:** step\_size >= (maximum - minimun)

So, the DPS value to apply is selected as follows:

$$
dps\_value\_to\_apply=
    \begin{cases}
      minimun, & value\_resolved\_by\_expression <= minimun \\
      maximum, & value\_resolved\_by\_expression >= maximun \\
      previous\_applied\_dps\_value, & step\_size >= abs(value\_resolved\_by\_expression - previous\_applied\_dps\_value) \\
      value\_resolved\_by\_expression, & \text{otherwise}
    \end{cases}
$$

In order to provide an example of the DLVR value expression, let's say that **SA**, **CORE0**, and **RING0** are well-defined domains, so some valid expressions are as follows:
 * Next expression will take the maximum value from SA and RING0 and will add it an offset: **0.5 + max(SA, RING0)**
 * Next expression will multiply a constant to SA and add the result to CORE0: **2.0*SA + CORE0**
 * Expression can be switched with other expression defined at the input file.

Allowed mathematical operations: ```min, max, avg, sum, abs, ceil, floor, round, roundn, exp, log, log10, logn, pow, root, sqrt, clamp, inrange, swap```.

Important to note that it is required to provide all pin attributes for every DLVR pin that the customer would like to use for a specific voltage object. For more details see [this api](#CreateDomainObjectWithConditionAndRailSupport-interface).

## Pattern Modify
This rail configuration has the intention of executing a patmod configuration set point, where the details must be defined in [domain configuration file](#FIVR-domains).

Every PatternModify defined, has a selector which works as a switch deciding between two set points which will be executed. This selector must be a boolean comparison expression and could or couldn't depend on previous domain applied values. The result of the selector expression will decide which setpoint will be executed for the **true** or **false** result.

In order to provide an example of the PatternModify selector expression, let's say that **SA**, **CORE0**, and **RING0** are well-defined domains, so some valid expressions are as follows:
 * Next expression will return true if SA is higher than CORE0, otherwise will return false: **SA > CORE0**
 * Next expression will return true if SA is equal to a constant, otherwise will return false: **SA == 0.75**

Allowed equalities and inequalities: ```=, ==, <>, !=, <, <=, >, >=```.

So, the setpoint to execute is as follows:

$$
setpoint\_to\_execute=
    \begin{cases}
      set\_point\_for\_true, & \text{boolean expression return TRUE} \\
      set\_point\_for\_false, & \text{boolean expression return FALSE}
    \end{cases}
$$

## Timing Attribute Pins
This rail configuration is intended to changing the values of the attributes of a specific timing pin. Every pin attribute value is the result of an expression that could or couldn't depend on previously applied domain values. The expression in this configuration has a protected keyword: ```PERIOD```, this keyword is used when the user wants to input expressions that use the tester period value for the given domain in this configuration, it is **optional**. Every timing attribute configuration must be defined in [domain configuration file](#FIVR-domains).

In order to provide an example of the attribute value expression, let's say that **SA**, **CORE0**, and **RING0** are well-defined domains and we configured the domain for the example pin as **D0**, so some valid expressions are as follows:
 * Next expression will take the maximum value from SA and RING0 and will add it an offset: **0.5 + max(SA, RING0)**
 * Next expression will multiply a constant to SA and add the result to CORE0: **2.0*SA + CORE0**
 * Next expression will multiply a constant to the period value given by the tester for the configured domain D0: **2.0*PERIOD**

Allowed mathematical operations: ```min, max, avg, sum, abs, ceil, floor, round, roundn, exp, log, log10, logn, pow, root, sqrt, clamp, inrange, swap```.

It's important to note that the resulted expression value will be applied only if the difference between the previously applied attribute value is higher than the step size,. As follows:

$$
attribute\_value\_to\_apply=
    \begin{cases}
      current\_attribute\_value, &  step\_size > abs(value\_resolved\_by\_expression - current\_attribute\_value)\\
      value\_resolved\_by\_expression, & step\_size <= abs(value\_resolved\_by\_expression - current\_attribute\_value)
    \end{cases}
$$

### Required Attributes 
Optional field for providing additional attributes that are required for successfully applying expression based attributes. None of the attributes defined here support expression syntax, the value provided will be sent directly to TOS when applying the timing attributes.

Mainly useful for cases when configuring attributes such as __Compare__, __EdgeCounter__ which are mandatory when configuring certain attributes (mainly applies to TOS4 and beyond). Example:
```json
"TimingAttributes" : [
    {
      "pin_name": "ClockPin",
      "domain": "DDR_STF",
      "attributes": [
        {
          "name": "TFall",
          "expression": "(SA/0.5)*PERIOD",
          "step_size": 0.0000000001
        }
      ]
    },
    {
      "pin_name": "ClockPin2",
      "domain": "DDR_STF",
      "attributes": [
        {
          "name": "TFall",
          "expression": "(SA/0.5)*PERIOD",
          "step_size": 0.0000000001
        }
      ],
      "required_attributes" : [
        {
          "name": "EdgeCounter",
          "value" : "Off"
        }
      ]
    }
  ]
```
# Service Interfaces
Voltage Service provides methods to create objects to apply voltage values to specific domains (which should be previously defined and loaded from corresponding configuration file) or a power supply pin. Managing to apply voltages values to five domains or dps pin, depends on the type of returned voltage object, which is selected using a specific create API.

Every provided API returns an [IVoltage](#IVoltage) object, where ```IVoltage``` is a common interface for all types of objects, but depending on what the customer is going do to, it's required to use a specific interface that implements the needed functionalities. 

All the availble interfaces are as follows:
- [IVoltage](#IVoltage)
- [IVForcePinAttribute](#IVForcePinAttribute)
- [IFivrDomains](#IFivrDomains)
- [IFivrCondition](#IFivrCondition)
- [IFivrDomainsAndCondition](#IFivrDomainsAndCondition)
- [IFivrDomainsAndConditionWithRails](#IFivrDomainsAndCondition)
- [IVForcePinAttributeWithRails](#IVForcePinAttributeWithRails)

### IVoltage
This interface is the common interface, this means all objects are IVoltage objects.
This interface is helpul in a API or method parameter level, no matter which specific object the customer sends, it is for sure an IVoltage object.

To understand how to use IVoltage interface, let's see following example:

``` csharp
    void ApplyConditionToVoltageObject(IVoltage voltageObject)
    {
        if (voltageObject is IFivrConditionVoltage voltage)
        {
            voltage.ApplyCondition();
        }
    }
```

### IVForcePinAttribute
This interface is to modify the ``VForce`` pin attribute. The modification could be done through ``CreateVForceForPinAttribute`` method applying the voltage value to the ``hardware`` or through ``CreateVForceForPinTestCondition`` modifying the voltage value to the ``level test condition``. This interface has a method called ``Restore()`` that allows restoring the original value of the ``VForce`` to hardware. In conjuction to the last method it also has a method called ``GetRestoreValues()`` that allows obtaining each restore value for each pin. 

### IFivrDomains
FIVR voltage objects provided by Voltage service can be created providing the domain names (which should be previously defined and loaded from the corresponding configuration file) to be used in order to apply the required voltage values providing them as inputs to the apply method of such object.

### IFivrCondition
This interface manages a condition, which is a collection of domains with the corresponding voltage value required for each of them in a second configuration file format. If this option is used the Apply method won't require any input as the required voltage values will be used as defined for the corresponding condition for each included domain.

### IFivrDomainsAndCondition
This interface could be considered as a combination of an [IFivrDomains](#IFivrDomains) object and [IFivrCondition](#IFivrCondition) object. This means that an IFivrDomainsAndCondition object can executes all methods from both previous objects.

### IFivrDomainsAndConditionWithRails
This interface can be considered a combination of [IFivrDomainsAndCondition](#IFivrDomainsAndCondition) object with support for [Rail Configurations](#Rail-Configurations). In this interface the user provides the different rail configuration names to execute and the DLVR pin attributes (if using a DLVR pin rail configuration) required in conjunction to the previous attributes of the IFivrDomainsAndCondition object.

### IVForcePinAttributeWithRails
This interface allows using DPS domains with support for [Rail Configurations](#Rail-Configurations). The object can be creating by providing the DPS domain names (which are defined in the corresponding configuration file) to be used in order to apply the required voltage (Vforce) values provided as inputs to the apply method of such object. In this interface the user provides the different rail configuration names to execute and the DLVR pin attributes (if using a DLVR pin rail configuration).

## CreateVForceForPinAttribute interface

|IVoltage CreateVForceForPinAttribute(List<string> pinNames, Dictionary<string, Dictionary<string, string>> attributes)|
|--|

The _**CreateVForceForPinAttribute**_ method returns an IVoltage test object interface which gives access to voltage operations that can be performed unto DPS pins.

### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| pinNames | List<string>| A list containing the names of the DPS pins. Repeated pin names are supported. |
| attributes | Dictionary<string, Dictionary<string, string>> | The attributes and its values per pin to apply with the application of the VForce. |

### Return Value
IVoltage test object interface for the DPS pins.

### Usage Example
The following C# code excerpt exemplifies the usage of the CreateVForceForPinAttribute:
``` csharp
using Prime.VoltageService;

(...)

private IVoltage dpsPinVoltage;

(...)

// Parameters are hard-coded only to simplify the example. This should be avoided.
List<string> pinNames = new List<string>{ "PinOne", "PinTwo"};
Dictionary<string, Dictionary<string, string>> attributes= new Dictionary<string, Dictionary<string, string>>{
   {"PinOne", { "AtributeNameOne", "AtributeValueOne" }, { "AtributeNameTwo", "AtributeValueTwo" }},
   {"PinTwo", { "AtributeNameOne", "AtributeValueOne" }, { "AtributeNameTwo", "AtributeValueTwo" }},

this.dpsPinVoltage= Prime.Services.VoltageService.CreateVForceForPinTestCondition(pinNames, attributes);
```
For correct use of this handler all required attributes to set a VForce value for the given pin must be present. 

## CreateVForceForPinTestCondition interface

|IVoltage CreateVForceForPinTestCondition(List<string> pinNames, string testConditionName)|
|--|

The _**CreateVForceForPinTestCondition**_ method returns an IVoltage test object interface which gives access to voltage operations that can be performed unto a Test Condition.

### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| pinNames | List<string>| A list containing the names of the pins to which the voltage must be applied to. |
| testConditionName | string  | The name of the test condition to modify. |

### Return Value
IVoltage test object interface for Test Conditions.

### Usage Example
The following C# code excerpt exemplifies the usage of the CreateVForceForPinTestConditionmethod:
``` csharp
using Prime.VoltageService;

(...)

private IVoltage testConditionVoltage;

(...)

// Parameters are hard-coded only to simplify the example. This should be avoided.
List<string> pinNames = new List<string>{ "PinOne", "PinTwo", "PinThree" };
string testCondition = "MyTestCondition";

this.testConditionVoltage = Prime.Services.VoltageService.CreateVForceForPinTestCondition(pinNames, testCondition);
```

For this voltage handler to properly work, a pattern must be configured with the "EXTTrigger" instruction along with the propper TriggerMap and levels configuration in the TP. Additionaly the Functional Service in charge of running the corresponding plist must be configured to handle the TriggerMap as exemplified in the following code snippet.

``` csharp
using Prime.FunctionalService;

(...)

private ICaptureFailureTest funcTest;

(...)

// Parameters are hard-coded only to simplify the example. This should be avoided.
string patlist = "MyPlist";
string levelsTc = "MyLevelsFile";
string levelsTc = "MyTimingsFile";
string triggerMapName = "MyTriggerMap";
this.funcTest = Prime.Services.FunctionalService.CreateCaptureFailureTest(patlist, levelsTc, timingsTc, 1);
this.funcTest.SetTriggerMap(triggerMapName);

```

## CreateFivrForDomains interface
| IFivrDomains CreateFivrForDomains(List<string> domainNames, string plist)|
|--|

The _**CreateFivrForDomains**_ method returns and IVoltage test object interface which gives access to voltage operations that can be performed unto a list of domains.

### Parameters 
| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| domainNames | List<string>| A list containing the names of the domains to which the voltage must be applied to. Repeated domains (not domain groups) are supported. |
| pilst | string | Instance's plist name. |

### Return value
IVoltage test object interface for FIVR domain object.

### Usage Example
The following C# code exceprt exemplifies the usage of the CreateFivrForDomains method:
``` csharp
using Prime.VoltageService;

(...)

private IVoltage fivrForDomains;

(...)

// Parameters are hard-coded only to simplify the example. This should be avoided.
List<string> domainNames = new List<string>{ "SA", "CORE0", "RING0" };
string plist = "PlistName";

this.fivrForDomains= Prime.Services.VoltageService.CreateFivrForDomains(domainNames, plist);
```

## CreateFivrForCondition interface
|IFivrCondition CreateFivrForCondition(string conditionName, string plist)|
|--|
The _**CreateFivrForCondition**_ method returns an IVoltage test object interface which gives access to voltage operations that can be performed unto a specific domain conditions.

### Parameters
| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| conditionName | string  | The name of the domain condition to modify. Repeated domains (not domain groups) are supported. |
| pilst | string | Instance's plist name. |

### Return value
IVoltage test object interface for the domain condition object.

### Usage Example
The folowing C# code excerpt exemplifies the usage of the CreateFivrForCondition method:
``` csharp
using Prime.VoltageService;

(...)

private IVoltage domainConditionObject;

(...)

// Parameters are hard-coded only to simplify the example. This should be avoided.
string plist = "PlistName";
string conditionName = "ALL_DOMAINS";

this.domainConditionObject= Prime.Services.VoltageService.CreateFivrForCondition(conditionName, plist);
```
## CreateFivrForDomainsAndCondition interface

|IFivrDomainsAndCondition CreateFivrForDomainsAndCondition(List<string> domainNames, string conditionName, string plist)|
|--|

The _**CreateFivrForDomainsAndCondition**_ method returns and IVoltage test object interface which gives access to voltage operations that can be performed unto a list of domains and a specific condition name.

### Parameters
| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| domainNames | List<string>| A list containing the names of the domains to which the voltage must be applied to. Repeated domains (not domain groups) are supported. |
| conditionName | string  | The name of the domain condition to modify. |
| pilst | string | Instance's plist name. |

### Return value
IVoltage test object interface for domain object with domain support and domain condition.
### Usage Example
The following C# code excerpt exemplifies the usage of the CreateFivrDomainsAndCondition method:
``` csharp
using Prime.VoltageService;

(...)

private IVoltage domainsAndConditionObject;

(...)

// Parameters are hard-coded only to simplify the example. This should be avoided.
List<string> domainNames = new List<string>{ "SA", "CORE0", "RING0" };
string plist = "PlistName";
string conditionName = "ALL_DOMAINS";

this.domainsAndConditionObject= Prime.Services.VoltageService.CreateFivrDomainsAndConditionWithRails(domainNames, conditionName, plist);
```

## CreateFivrDomainsAndConditionWithRails interface

|IVoltage CreateFivrDomainsAndConditionWithRails(List<string> domainNames, string conditionName, List<string> railConfigurationNames, Dictionary<string, Dictionary<string, string>> dlvrPinAttributes, string plist)|
|--|

The _**CreateFivrDomainsAndConditionWithRails**_ method returns an IVoltage test object interface which gives access to voltage operations that can be performed unto a list of domains, a specific domain condition and many rail configurations.

### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| domainNames | List<string>| A list containing the names of the domains to which the voltage must be applied to. Repeated domains (not domain groups) are supported. |
| conditionName | string  | The name of the domain condition to modify. |
| plist | string | Instance's plist name. |
| railConfigurations | List<string> | A list containing the names of the rail configurations to modify or apply. |
| dlvrPinAttributes | Dictionary<string, Dictionary<string, string>> | A list of pin attribute names and their values per dlvr pin names to apply as rail configuration. |


### Return Value
IVoltage test object interface for domain object with domain support, domain condition and rail configuration support.

### Usage Example 1
The following C# code excerpt exemplifies the usage of the CreateDomainObjectWithConditionAndRailSupport method:
``` csharp
using Prime.VoltageService;

(...)

private IVoltage domainObjectWithConditionAndRailSupport;

(...)

// Parameters are hard-coded only to simplify the example. This should be avoided.
List<string> domainNames = new List<string>{ "SA", "CORE0", "RING0" };
string plist = "PlistName";
string conditionName = "ALL_DOMAINS";
List<string> railConfigurationNames = new List<string>{"PinOne", "ConfigSetPointOne", "PinTwo"};
Dictionary<string, Dictionary<string, string>> dlvrPinAttributes = new Dictionary<string, Dictionary<string, string>>{
   {"PinOne", { "AtributeNameOne", "AtributeValueOne" }, { "AtributeNameTwo", "AtributeValueTwo" }},
   {"PinTwo", { "AtributeNameOne", "AtributeValueOne" }, { "AtributeNameTwo", "AtributeValueTwo" }},
};

this.domainObjectWithConditionAndRailSupport = Prime.Services.VoltageService.CreateFivrDomainsAndConditionWithRails(domainNames, conditionName, plist, railConfigurationNames, dlvrPinAttributes);
```
### Interface overload
This interface in particular has a second implementation which is made to be used when the rail configurations to be applied do not include DLVR pins, which in turn does not require DLVR pin attributes. 
|IVoltage CreateFivrDomainsAndConditionWithRails(List<string> domainNames, string conditionName, string plist, List<string> railConfigurationNames)|
|--|
### Usage Example 2
``` csharp
using Prime.VoltageService;

(...)

private IVoltage domainObjectWithConditionAndRailSupport;

(...)

// Parameters are hard-coded only to simplify the example. This should be avoided.
List<string> domainNames = new List<string>{ "SA", "CORE0", "RING0" };
string plist = "PlistName";
string conditionName = "ALL_DOMAINS";
List<string> railConfigurationNames = new List<string>{"ConfigSetPointOne", "TimingPin"};

this.domainObjectWithConditionAndRailSupport = Prime.Services.VoltageService.CreateFivrDomainsAndConditionWithRails(domainNames, conditionName, plist, railConfigurationNames);
```

## CreateVForcePinAttributeWithRails interface 
This method returns an IVoltage test object interface which gives access to voltage operations that can be performed unto a list of DPS domains and many rail configurations.

### Parameters

| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| domainNames | List<string>| A list containing the names of the domains to which the voltage must be applied to. Repeated domains (not domain groups) are supported. |
| domainPinAttributes | Dictionary<string, Dictionary<string, string>> | The list of attributes and values per pin to use. |
| railConfigurations | List<string> | A list containing the names of the rail configurations to modify or apply. |
| dlvrPinAttributes | Dictionary<string, Dictionary<string, string>> | A list of pin attribute names and their values per dlvr pin names to apply as rail configuration. |

### Return Value
IVoltage test object interface for DPS domain object with domain support, and rail configuration support.

### Usage Example 1
The following C# code excerpt exemplifies the usage of the CreateVForcePinAttributeWithRails method:
``` csharp
using Prime.VoltageService;

(...)

private IVoltage dpsDomainObjectWithRailSupport;

(...)

// Parameters are hard-coded only to simplify the example. This should be avoided.
List<string> domainNames = new List<string>{ "DPS1", "DPS2", "DPS3" };
List<string> railConfigurationNames = new List<string>{"PinOne", "ConfigSetPointOne", "PinTwo"};
Dictionary<string, Dictionary<string, string>> dlvrPinAttributes = new Dictionary<string, Dictionary<string, string>>{
   {"PinOne", { "AtributeNameOne", "AtributeValueOne" }, { "AtributeNameTwo", "AtributeValueTwo" }},
   {"PinTwo", { "AtributeNameOne", "AtributeValueOne" }, { "AtributeNameTwo", "AtributeValueTwo" }},
};
Dictionary<string, Dictionary<string, string>> domainPinAttributes = new Dictionary<string, Dictionary<string, string>>{
   {"DomainPin1", { "AtributeNameOne", "AtributeValueOne" }, { "AtributeNameTwo", "AtributeValueTwo" }},
   {"DomainPin2", { "AtributeNameOne", "AtributeValueOne" }, { "AtributeNameTwo", "AtributeValueTwo" }},
};

this.dpsDomainObjectWithRailSupport = Prime.Services.VoltageService.CreateVForcePinAttributeWithRails(domainNames, domainPinAttributes, railConfigurationNames, dlvrPinAttributes);
```

### Interface overload
This interface also has a second implementation which is made to be used when the rail configurations to be applied do not include DLVR pins, which in turn does not require DLVR pin attributes. 
|IVoltage CreateVForcePinAttributeWithRails(List<string> domainNames,Dictionary<string, Dictionary<string, string>> domainPinAttributes, List<string> railConfigurationNames)|
|--|

## OverrideExpression interface 
This method override the DLVR expression.
New expression must be defined at the .json file under DlvrPins section.

### Parameters
  
| _**Parameter Name**_ | _**Type**_  | _**Description**_  |
|-----------------------|---|---|
| RailHandlerType | railHandlerType| Enum that describe which rail handler expression is override.|
| expressionName | string | expression name to set, must be defined at the input file. |
| configName | string | dlvr Pin name.  |

### Return Value
No return value.


### Usage Example 1
The following C# code excerpt exemplifies the usage of the OverrideExpression method:
``` csharp
using Prime.VoltageService;

(...)

private IVoltage dpsDomainObjectWithRailSupport;

(...)

// Parameters are hard-coded only to simplify the example. This should be avoided.
List<string> domainNames = new List<string>{ "DPS1", "DPS2", "DPS3" };
List<string> railConfigurationNames = new List<string>{"PinOne", "ConfigSetPointOne", "PinTwo"};
Dictionary<string, Dictionary<string, string>> dlvrPinAttributes = new Dictionary<string, Dictionary<string, string>>{
   {"PinOne", { "AtributeNameOne", "AtributeValueOne" }, { "AtributeNameTwo", "AtributeValueTwo" }},
   {"PinTwo", { "AtributeNameOne", "AtributeValueOne" }, { "AtributeNameTwo", "AtributeValueTwo" }},
};
Dictionary<string, Dictionary<string, string>> domainPinAttributes = new Dictionary<string, Dictionary<string, string>>{
   {"DomainPin1", { "AtributeNameOne", "AtributeValueOne" }, { "AtributeNameTwo", "AtributeValueTwo" }},
   {"DomainPin2", { "AtributeNameOne", "AtributeValueOne" }, { "AtributeNameTwo", "AtributeValueTwo" }},
};

this.dpsDomainObjectWithRailSupport = Prime.Services.VoltageService.CreateVForcePinAttributeWithRails(domainNames, domainPinAttributes, railConfigurationNames, dlvrPinAttributes);
this.dpsDomainObjectWithRailSupport.OverrideExpression(railHandlerType.DLVR,"DPS1","ExpressionToOverride");
```


# Common Parameters
Some common parameters are provided from the Prime Kernel in order to enable further functionality in all Test Methods which run a plist, this without the need of adding additional code to interact with the Voltage Service. For information regarding general aspects of Common Parameters please refer to the [Common Parameters](https://dev.azure.com/mit-us/PRIME/_wiki/wikis/PRIME.wiki/1979/Common-Parameters) documentation.

## Fivr Condition common parameters
This set of common parameters allows the user to set a FivrCondition in any Test Method which runs a plist. The two parameters must be used in conjunction in order to effectively enable the feature. The provided parameters are as follows:
- **FivrConditionName**: specifies the name of the Fivr Condition to apply. The appropriate configuration files must exist as specified in previous sections.
- **FivrConditionPlistParamName**: specifies the name of the parameter which points to the plist to execute (**not** the plist name itself). This is required since User Code may exist where the parameter name is not standardized, the feature should still be available under these circumstances.

Please refer to the following example:

```c
Test PrimeFuncCaptureCtvTestMethod FivrConditionCommonParameter_P1
{
   Patlist = "MyPlist";
   FivrConditionName = "FIVR_CORE0_LC";
   FivrConditionPlistParamName = "Patlist";
   ...
}
```
Here, a Fivr Condition "FIVR_CORE0_LC" is applied in a PrimeFuncCaptureCtvTestMethod test instance, and "FivrConditionPlistParamName" is given the value of "Patlist" which is the name of the parameter that points to the plist name for this specific Test Method.

In order for this to work the configuration files described above for Fivr Conditions must be in place as well as the enabling of these common parameters for the "PrimeFuncCaptureCtvTestMethod". The latter is achieved by copying the "PrimeFuncCaptureCtvTestMethodCommonParams.xml" from Prime's resources\preheaders\CommonParams folder into the Test Program's supersedes\preheaders folder and modifying that xml file to import the Fivr Condition common parameters as follows:

```xml
<?xml version="1.0" encoding="utf-8"?>
<TestLibraryInterfaces xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xsi:schemaLocation="http://vtsm.intel.com/2009/TestLibraryInterfaces file:///C:/Intel/hdmt/hdmtOS_3.4.4.5_Release/TOSRelease/bin/release/TestLibraryInterfaces.xsd" xmlns="http://vtsm.intel.com/2009/TestLibraryInterfaces">
  <TestLibrary name="PrimeTestInstance">
    <TestClass name="PrimeFuncCaptureCtvTestMethodCommonParams" />
    <Imports>
      <FileName>FivrConditionCommonParam.xml</FileName>
    </Imports>
    <PublicBases/>
    <Parameters/>
    <ExitPorts/>
  </TestLibrary>
</TestLibraryInterfaces>

```

# Exceptions
Here are exposed all exceptions you could get while using Voltage Service:

| Object interface    | Exception message (example) | Root cause |
| ------------------- | -----------------------       | --------------------------------------------- |
| IVForcePinAttribute | Different quantity of values. Expected quantity of=[3], and not of=[5]. | Creating this object for DPS or test condition managing, is required to pass a list of pins, and then a list of voltage values to apply to the pins, the voltage list quantity must be the same quantity as pin list. This exception is raised when list quantities are mismatched. |
| IVForcePinAttribute | Pin list should not be empty. | Creating this object for DPS or test condition managing, is required to pass a list of pins. This exception is raised when pin list is empty. |
| IVForcePinAttribute | Pin=[DDR99999] does not exist. | Creating this object for DPS or test condition managing, is required to pass a list of pins. This exception is raised when a pin name provided in pin list does not exist. |
| IVForcePinAttribute | Test Condition=[TestCondition_0] does not exist. | Creating this object for test condition managing, is required to pass the test condition name. This exception is raised when an invalid test condition is provided. |
| IVForcePinAttribute | Pin=[DDR0] has no attributes indicated. | Creating this object for DPS or test condition, a list of pins must be provided and a list of pin attributes too. This exception is raised when is not provide the pin attributes of any pin provided in the list. |
| IFivrDomains, IFivrDomainsAndCondition, IFivrDomainsAndConditionWithRails | Domain list should not be empty. | Creating an object that manages a FIVR domain, a list of domains must be provided. This exception is raised when the domain list is empty. |
| IFivrDomains, IFivrDomainsAndCondition, IFivrDomainsAndConditionWithRails | Domain=[CORE0] is present more than one time in domain list. | Creating an object that manages a FIVR domain, a list of domains must be provided and some names could be a domain group. This exception is raised when the same domain is provided in the list and is part of an indicated domain group. |
| IFivrDomains, IFivrDomainsAndCondition, IFivrDomainsAndConditionWithRails | Domain=[CORE_0] is pointed by domain group=[DOMAIN_GROUP_1] and other domain group. | Creating an object that manages a FIVR domain, a list of domains must be provided and some names could be a domain group. This exception is raised when more than one domain group is provided and at least two domain groups are pointing to at least the same domain. |
| IFivrDomains, IFivrDomainsAndCondition, IFivrDomainsAndConditionWithRails | Name=[CORE_9999] is not a valid domain nor domain group. | Creating an object that manages a FIVR domain, a list of domains must be provided and some names could be a domain group. This exception is raised when a string in the domain list is not recognized as a valid domain nor a valid domain group. |
| IFivrCondition, IFivrDomainsAndCondition, IFivrDomainsAndConditionWithRails | Condition name should not be __*empty*__. | Creating an object that manages a FIVR condition with an empty value, will raise this exception. |
| IFivrCondition, IFivrDomainsAndCondition, IFivrDomainsAndConditionWithRails | The FIVR condition=[ConditionCore0123] has repeated domain names. | While creating an object that manages a FIVR condition, the five condition name must be provided. This exception is raised when the FIVR condition provided, __ConditionCore0123__ in this example, has a same domain name repeated. |
| IFivrCondition, IFivrDomainsAndCondition, IFivrDomainsAndConditionWithRails | Condition name=[FivrConditionDoesNotExist] does not exist in the Shared Storage. | Creating an object that manages a FIVR condition with an __*invalid*__ value, will raise this exception. |
| IFivrCondition, IFivrDomainsAndCondition, IFivrDomainsAndConditionWithRails | Domain=[CORE1000] does not exist in the Shared Storage. | Creating an object that manages a FIVR domain, a list of domains must be provided. This exception is raised when a string provided in the domain list is not recognized as a valid domain name. |
| IFivrDomainsAndConditionWithRails | DLVR pin=[DLVR_PIN_0] is not defined as a rail configuration=[DLVR_PIN_1,RAIL_CONFIG_1,RAIL_CONFIG_1] | This exception could be raised while creating a voltage object that support rail configurations. The exception is caused when it is indicated a DLVR pin in the pin attributes parameter, but not in the list of rail configurations. In this example, the __RAIL_CONFIG_1__ is the repeated value. |
| IFivrDomainsAndConditionWithRails | Fail creating IVoltage object due to at least a rail configuration is repeated=[RAIL_0,RAIL_1,RAIL_1,RAIL_2] | This exception could be raised while creating a voltage object that support rail configurations. The exception is caused when a rail name is repeated in rail list parameter, in this example the repeated name is __RAIL_1__. |
| IFivrDomainsAndConditionWithRails | There is missing pin attributes for pin name=[DDR0]. | Creating as voltage object that supports rail configurations, a list of pins must be provided and a list of pin attributes too. This exception is raised when is not provide the pin attributes of any pin provided in the list. |

# Version tracking

| **Date**   | **Prime release** | **Author**    | **Comments** |
| ---------- | ----------- | ------------- | ------------ |
| May 17<sup>th</sup>, 2022 | 10.00.00       | Andrea Gomez Montero | Changing the Fivr Condition .json file format.|