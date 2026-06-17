[[_TOC_]]

## REP for Dc

This **REP** is intended to describe the Shops Prime TestMethod.

In this document, you will find the below sections:

  - **Methodology** – A detailed description of this TestMethod intention and purpose

  - **Parameters** – A table describes each instance parameter (Name, Type, Default, Required?)

  - **Datalog output** – A detailed description of what is datalogged by this TestMethod

  - **Custom User Code hooks** – A list of functions available to the user code to override

  - **TPL Samples** – Examples of how to use this TestMethod in a TPL file

  - **Exit Ports** - A table describes each exit port

  - **Additional Dependencies** – More to consider for this TestMethod to operate

  - **Version tracking** – With author names, so you always have a name to address

  - **Acronyms** - Definition of acronyms used in this document
  
## Methodology

Shops test performs the shorts and opens testing as defined in the Intel Test Methodology handboock (ITMH), Charter 7, DC and AC Parametric Test Methods, section 7.2.1, Shorts / Opens Testing.

This test method provide DcTest using the prescribed force current measure voltage (ISVM) and force voltage measure current (VSIM) method with the funtional test method to be executed before each DcTest for Lower and Upper Diode. These tests rely on the input and output protection circuitry with characteristics representative of a typical diode. The requirements are primarily derived from the ITMH chapter on shorts and opens testing. 

### Verify

  - Validate limits (numeric with units)

  - Validate Levels block

  - Validate pins

  - Match number of pins to number of limits, is allow only one measure of limit for all pins.

### Execute

  - Apply Levels to HW (if Levels was provided by the user)

  - Gather the measured values

  - Match measured values against the provided limits

  - Print results to Datalog(ituff) if funtional fail or datalog mode for DC tests.

  ### Diagrams of Shops methodology
::: mermaid
graph TD;
start((Start Shops)) --> vf1
E4 --> End((Return port))
vdctEnd --> E1
    subgraph Verify
        vf1 -- NO --> vdct2
        fe --> vdct1
        subgraph Functional
            fe(( ));
            vf1{Has functional test?} -- YES --> vf2[Verify Timing Test Condition] --> vf_l
            vf2 --> vf_u
            vf_l2 --> fe
            vf_u2 --> fe
            vf_l -- NO --> fe
            vf_u -- NO --> fe
            subgraph LowerFunc[Lower Diode]
                vf_l{has lower diode?} -- YES -->vf_l1[Verify parameters:<br>- PatList<br>- LowerLevelTc] --> vf_l2[Build lower functional test]
            end
            subgraph UpperFunc[Upper Diode]
                vf_u{has upper diode?} -- YES -->vf_u1[Verify parameters:<br>- PatList<br>- UpperLevelTc] --> vf_u2[Build upper functional test]
            end
        end
        subgraph DC
            vdct1{has DcTest?} -- YES --> vdct2[Verify DcTestLevel with Initial condition] --> vdct3[Verify PrePause parameter] --> vdc_l
            vdct3 --> vdc_u
            vdc_l7 --> vdctEnd((End Dc verify))
            vdc_u7 --> vdctEnd
            vdct1 -- NO --> vdctEnd
            vdc_l -- NO --> vdctEnd
            vdc_u -- NO --> vdctEnd
            subgraph LowerDc[Lower Diode]
                vdc_l{has lower diode?} -- YES -->vdc_l1[Verify LowerDiodeForceAttributes parameter] --> vdc_l2[build configuration per pin] --> vdc_l3[Verify parameters:<br>- LowerDiodeLimitLow<br>- LowerDiodeLimitHigh]
                vdc_l3 --> vdc_l4{MeasureSequense} -- Parallel --> vdc_l5[Split pinGroups and comfigurations per pin] --> vdc_l6[Build lower testCondition for measure] --> vdc_l7[CustomGetDcTest]
                vdc_l4 -- Serial --> vdc_l6
            end
            subgraph UpperDc[Upper Diode]
                vdc_u{has upper diode?} -- YES -->vdc_u1[Verify UpperDiodeForceAttributes parameter] --> vdc_u2[build configuration per pin] --> vdc_u3[Verify parameters:<br>- UpperDiodeLimitLow<br>- UpperDiodeLimitHigh]
                vdc_u3 --> vdc_u4{MeasureSequense} -- Parallel --> vdc_u5[Split pinGroups and comfigurations per pin] --> vdc_u6[Build upper testCondition for measure] --> vdc_u7[CustomGetDcTest]
                vdc_u4 -- Serial --> vdc_u6
            end
        end
    end
    subgraph Execute
        E1[CustomPreExecute: If user defined, otherwise execute default empty] --> E2[CreateTestInstanceResults: Can be overwritten by user through extencion, Default return ShopsTestInstanceResults]
        E2 --> ELD1
        ELD3 -- NO --> EUD1
        ELD6 --> EUD1
        EUD6 --> E3[CustomPostProcessResults:<br>Default is a empty method] --> E4[CalculateFinalPort]
        EUD3 -- NO --> E3
        subgraph ELD[Lower Diode]
            ELD1{Has Functional test for lower diode?} -- YES --> ELD2[Execute Functional test for lower diode]
            ELD2 --> ELD3{Has lower DcTest?} -- YES --> ELD4[Apply DcLevelTc for HW initial conditions] --> ELD5[Execute lower DcTest]
            ELD1 -- NO --> ELD3
            ELD5 --> ELD6[DcProcess:<br>default only add results to ShopsTestInstanceResults object]
        end
        subgraph EUD[Upper Diode]
            EUD1{Has Functional test for upper diode?} -- YES --> EUD2[Execute Functional test for upper diode]
            EUD2 --> EUD3{Has upper DcTest?} -- YES --> EUD4[Apply DcLevelTc for HW initial conditions] --> EUD5[Execute upper DcTest]
            EUD1 -- NO --> EUD3
            EUD5 --> EUD6[DcProcess:<br> default only add results to ShopsTestInstanceResults object]
        end
    end

classDef Extentions stroke:RED,stroke-width:4px,color:White,stroke-dasharray: 6 3
classDef ExtectionDc Fill:Purple,stroke:RED,stroke-width:4px,color:White,stroke-dasharray: 6 3
classDef funtObj color:White, fill:darkBlue
classDef DcObj color:White, fill:Purple
classDef LowerObj color:White, fill:Green
classDef UpperObj color:White, fill:DarkCyan

class E1,E2,E3,vdc_l7,vdc_u7 Extentions
class ELD6,EUD6 ExtectionDc
class ELD1,ELD2,EUD1,EUD2 funtObj
class ELD3,ELD4,ELD5,EUD3,EUD4,EUD5 DcObj
class LowerFunc,LowerDc,ELD LowerObj
class UpperFunc,UpperDc,EUD UpperObj

style Verify Fond-size:20
style DC Fill:gray,stroke:Purple,stroke-width:4px,color:White,stroke-dasharray: 6 3
style Functional Fill:gray,stroke:Blue,stroke-width:4px,color:White,stroke-dasharray: 6 3
:::

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Dc test method

<table>
 <tr>
  <td rowspan="3" style="text-align: center; vertical-align: middle; font-weight: bold">&nbspParameter Name</td>
  <td colspan="4" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Required?</td>
  <td rowspan="3" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Type</td>
  <td rowspan="3" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Value</td>
  <td rowspan="3" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Comments</td>
 </tr>
 <tr>
  <td colspan="2" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Dc Test</td>
  <td colspan="2" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Func Test</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Lower</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Upper</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Lower</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Upper</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;TimingsTc</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;TimingCondition</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Levels test condition required for plist execution</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;Patlist</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Plist</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;DcLevelsTc</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;String</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Level TestCondition to set initial condition in hardware before execute DcTests.</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;The attributes defined in this test condition are used to restore the hardware condition of each pin after to take the measure in DcTest</td>
 </tr>
  <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;MeasureSequence</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;String (choice)</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;PARALLEL (<span style="color:blue;  font-weight: bold">default</span>), SERIAL</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;PARALLEL: Dc executed at the same time for all pins<br>SERIAL: Dc executed sequentially pin per pin.</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;Pins</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;CommaSeparatedString</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Comma separated list of measured pins</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;If anyone is a pinGroup and MeasureSequence selected is SERIAL the group will be splited by pins.</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;LowerLevelsTc</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;LevelsCondition</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;UpperLevelsTc</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;LevelsCondition</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;LowerDiodeForceAttributes</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;CommaSeparatedString</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;- Comma separated list of real numbers with units representing measurement of current to force in each pin.<br>- List of attributes and values.<br>- Path of json file with configuration.</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;For DC testing, shorts and lower diode opens test, specifies the current force value(s) applied to the corresponding pin/pingroup(s). A negative current value should be provided to draw current out of the device, and forward bias the lower diode. If the number of values is more of one It must match the number of pins being tested.</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;LowerDiodeLimitLow</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;CommaSeparatedString</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Comma separated list of real numbers with units representing measurement low limits.</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;For DC testing, shorts and lower diode opens test, specifies the lower voltage limit(s) considered a DUT failure. Since a negative current should be forced, a corresponding negative voltage value is expected. If the number of values is more of one It must match the number of pins being tested.</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;LowerDiodeLimitHigh</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;CommaSeparatedString</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Comma separated list of real numbers with units representing measurement high limits.</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;For DC testing, shorts and lower diode opens test, specifies the upper voltage limit(s) considered a DUT failure. Since a negative current should be forced, a corresponding negative voltage value is expected. If the number of values is more of one It must match the number of pins being tested.</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;UpperDiodeForceAttributes</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;CommaSeparatedString</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;- Comma separated list of real numbers with units representing measurement of current to force in each pin..<br>- List of attributes and values.<br>- Path of json file with configuration.</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;For DC testing, upper diode opens test, specifies the current force value(s) applied to the corresponding pin/pingroup(s). A positive current value should be provided to draw current into the device, and forward bias the upper diode. If the number of values is more of one It must match the number of pins being tested.</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;UpperDiodeLimitLow</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;CommaSeparatedString</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Comma separated list of real numbers with units representing measurement low limits.</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;For DC testing, upper diode opens test, specifies the lower voltage limit(s) considered a DUT failure. Since a positive current should be forced, a corresponding positive voltage value is expected. If the number of values is more of one It must match the number of pins being tested.</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;UpperDiodeLimitHigh</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;CommaSeparatedString</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Comma separated list of real numbers with units representing measurement high limits.</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;For DC testing, upper diode opens test, specifies the upper voltage limit(s) considered a DUT failure. Since a positive current should be forced, a corresponding positive voltage value is expected. If the number of values is more of one It must match the number of pins being tested.</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;ShopsTestType</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;String (choice)</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;ALL (<span style="color:blue;  font-weight: bold">default</span>), LOWER_SHORTS, LOWER_SHORTS_AND_OPENS, UPPER_SHORTS, UPPER_SHORTS_AND_OPENS</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;TemplateMode</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;String (choice)</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;DC_AND_FUNC (<span style="color:blue;  font-weight: bold">default</span>), DC_ONLY, FUNC_ONLY</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;EnableFlushSmartTc</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;String (choice)</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Enabled(<span style="color:blue;  font-weight: bold">default</span>), Disabled</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;SmartTC levels are not ignored otherwise SmartTC levels are ignored.</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;DatalogLevel</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;String (choice)</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;NONE (<span style="color:blue;  font-weight: bold">default</span>), FAIL_DATA, ALL_DATA, ALL_DATA_COMPRESS, ALL_DATA_PINMAP_COMPRESS</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;
    <span style="color:GREEN;  font-weight: bold">FAIL_ONLY</span> prints to Ituff only fail result.<br>
	<span style="color:GREEN;  font-weight: bold">ALL_DATA</span> prints both fail and pass results.<br>
	<span style="color:GREEN;  font-weight: bold">ALL_DATA_COMPRESS</span> print all result in ituff compress format.<br>
	<span style="color:GREEN;  font-weight: bold">ALL_DATA_PINMAP_COMPRESS</span> print all result in ituff compress format with pinMapId.</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;AlarmPortRedirect</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;String (choice)</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;DISABLED (<span style="color:blue;  font-weight: bold">default</span>), ENABLE</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Default alarm port=[-2] behavior, Enable the alarm port redirect to port=[7].</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;PrePause</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;String</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;</td>
 </tr>
</table>

## Console output (debug mode)

Measurement results can be printed to console in debug mode in form of below tables

Funtional and Dc test for upper diode pass console outPut:

```console-------
[DUT: 1]
=========================
Running Execute() for test instance=[Shops::PinLCUpperDiodeShort_AllPass_P1]
=========================
[DUT: 1]Applying test condition=[Shops::SampleShopsTestMethodTC] without SmartTc, Category=[NONE].
[DUT: 1]Running DC test for Upper diode.
[DUT: 1]Applying test condition=[Shops_Upper_Shops::PinLCUpperDiodeShort_AllPass_P1_SerialMeasurement] without SmartTc, Category=[NONE].
[DUT: 1]
***********************************************************************************************
Printed from C# instance [Shops::PinLCUpperDiodeShort_AllPass_P1_Upper] of DcTestCs test method
***********************************************************************************************

------------------------------------------------
| Pin                     |   Results | Status |
------------------------------------------------
| HDDPS_LC_nogang_5ohm1   | 0.78967 V | Pass   |
------------------------------------------------
| HDDPS_LC_nogang_12ohm1  |  1.0893 V | Pass   |
------------------------------------------------
| HDDPS_LC_nogang_250ohm1 |  0.9999 V | Pass   |
------------------------------------------------
| HDDPS_LC_nogang_250ohm2 |  0.6668 V | Pass   |
------------------------------------------------
| HDDPS_LC_nogang_12ohm2  |   0.898 V | Pass   |
------------------------------------------------
| HDDPS_LC_nogang_5ohm2   | 0.98389 V | Pass   |
------------------------------------------------
[DUT: 1]=== Processing results ===.
[DUT: 1]Printed to Ituff:
2_tname_Shops::PinLCUpperDiodeShort_AllPass_P1_Upper
2_category_pc
2_composite_0011018_0.7896700
2_composite_0000019_1.0893000
2_composite_0011020_0.9999000
2_composite_0000021_0.6668000
2_composite_0011022_0.8980000
2_composite_0000023_0.9838900
[DUT: 1]Test instance=[Shops::PinLCUpperDiodeShort_AllPass_P1] executed using 13.662800 ms
[DUT: 1]TestInstance=[Shops::PinLCUpperDiodeShort_AllPass_P1] exit port=[1].
[2023-Jul-16 21:04:55.760][A][TAL][DUT: 1] StopTest PrimeShopsTestMethod::Shops::PinLCUpperDiodeShort_AllPass_P1
[2023-Jul-16 21:04:55.760][A][TAL][DUT: 1] StartTest PrimeShopsTestMethod::Shops::PinVLCLowerDiodeShort_FailByLowerLimitButTestPassAndAllDataPinDetail_P1
[DUT: 1]
```
Funtional test pass, Dc test fail for upper diode console outPut:
```console
[DUT: 1]
=========================
Running Execute() for test instance=[Shops::PinLCUpperDiodeShort_FailByUpperShort_P4]
=========================
[DUT: 1]Applying test condition=[Shops::SampleShopsTestMethodTC] without SmartTc, Category=[NONE].
[DUT: 1]Running DC test for Upper diode.
[DUT: 1]Applying test condition=[Shops_Upper_Shops::PinLCUpperDiodeShort_FailByUpperShort_P4_SerialMeasurement] without SmartTc, Category=[NONE].
[DUT: 1]
********************************************************************************************************
Printed from C# instance [Shops::PinLCUpperDiodeShort_FailByUpperShort_P4_Upper] of DcTestCs test method
********************************************************************************************************

------------------------------------------------
| Pin                     |   Results | Status |
------------------------------------------------
| HDDPS_LC_nogang_5ohm1   | 0.78967 V | Pass   |
------------------------------------------------
| HDDPS_LC_nogang_12ohm1  |  1.0893 V | Pass   |
------------------------------------------------
| HDDPS_LC_nogang_250ohm1 |  0.9999 V | Pass   |
------------------------------------------------
| HDDPS_LC_nogang_250ohm2 |  0.6668 V | Pass   |
------------------------------------------------
| HDDPS_LC_nogang_12ohm2  | 0.23456 V | Fail   |
------------------------------------------------
| HDDPS_LC_nogang_5ohm2   | 0.98389 V | Pass   |
------------------------------------------------
[DUT: 1]=== Processing results ===.
[DUT: 1]Printed to Ituff:
2_tname_Shops::PinLCUpperDiodeShort_FailByUpperShort_P4_Upper
2_category_fc
2_composite_0011022_0.2345600
[DUT: 1]
=============================================================
Calculated final port = [PORT_FAIL_DC_SHORTS, Fail DC shorts]
=============================================================
-----------------------------------------------------------------------------------------------------------------------------------
| Port Description | Detail                                                          |               Pin Name | Values            |
-----------------------------------------------------------------------------------------------------------------------------------
|   Fail DC shorts | Pin=[HDDPS_LC_nogang_12ohm2] fail by short in Upper diode test. | HDDPS_LC_nogang_12ohm2 | 0.234559997916222 |
-----------------------------------------------------------------------------------------------------------------------------------
[DUT: 1]Test instance=[Shops::PinLCUpperDiodeShort_FailByUpperShort_P4] executed using 12.515200 ms
[DUT: 1]TestInstance=[Shops::PinLCUpperDiodeShort_FailByUpperShort_P4] exit port=[4].
[2023-Jul-16 21:04:55.833][A][TAL][DUT: 1] StopTest PrimeShopsTestMethod::Shops::PinLCUpperDiodeShort_FailByUpperShort_P4
[2023-Jul-16 21:04:55.833][A][TAL][DUT: 1] StartTest PrimeShopsTestMethod::Shops::PinLCUpperDiodeShort_UpperOpen_P6
[DUT: 1]
```

Funtional test fail, Dc test fail but promoted to pass console output:
```console
[DUT: 1]
=========================
Running Execute() for test instance=[Shops::PinLCUpperDiode_FuncFailDcPass_P2]
=========================
[DUT: 1]PowerUpTc name is empty.
[DUT: 1]PowerOnTC will not be applied as it is empty.
[DUT: 1]Applying test condition=[Shops::SampleShopsTestMethodTC] with SmartTC, Category=[LEVELS_SETUP].
[DUT: 1]Applying test condition=[Shops::basic_Shops_timing_10MHz_20MHz] with SmartTC, Category=[TIMING].
[DUT: 1]Functional Test settings:
- Plist name=[Shops1_Plist].
- Levels test condition=[Shops::SampleShopsTestMethodTC].
- Timings test condition=[Shops::basic_Shops_timing_10MHz_20MHz].
- No pin mask set.
- No edge counter pins set.
- No software trigger set.
- No trigger map set.
- Failure settings:
  --Total failure capture=[1].
  --Per pattern failure capture=[0].
[2023-Jul-16 21:04:56.354][A][HAL][DUT: 1] Starting burst group execution.
[2023-Jul-16 21:04:56.358][A][HAL][DUT: 1] Waiting 30000ms for execution to finish
[2023-Jul-16 21:04:56.358][A][HAL][DUT: 1] Time domain is stopped on fail.  Stopping other domains from executing
[DUT: 1]Captured pattern=[shops], domain=[DomainA_All_DPIN_Dig], parentPlist=[Shops1_Plist], patternId=[1], burstIndex=[0], vectorAddress=[1], cycle=[1101].
[DUT: 1]	Failing pins=[xxHPCC_DPIN_Dig_slcA_AA0,xxHPCC_DPIN_Dig_slcA_AA1], corresponding failing channels=[8032,8033].
[DUT: 1]Plist=[Shops1_Plist], has finished all burst with result=[FAIL].
[DUT: 1]Printed to Ituff:
2_tname_Shops::PinLCUpperDiode_FuncFailDcPass_P2
2_pttrn_DomainA_All_DPIN_Dig:shops:Shops1_Plist
2_fdpmv_1
2_fcpmv_-1
2_fsdmv_-1
2_vcont_1101
2_faildata_{8032,8033}
[DUT: 1]Applying test condition=[Shops::SampleShopsTestMethodTC] without SmartTc, Category=[NONE].
[DUT: 1]Running DC test for Upper diode.
[DUT: 1]Applying test condition=[Shops_Upper_Shops::PinLCUpperDiode_FuncFailDcPass_P2_SerialMeasurement] without SmartTc, Category=[NONE].
[DUT: 1]
*************************************************************************************************
Printed from C# instance [Shops::PinLCUpperDiode_FuncFailDcPass_P2_Upper] of DcTestCs test method
*************************************************************************************************

----------------------------------------------------
| Pin                     |       Results | Status |
----------------------------------------------------
| HDDPS_LC_nogang_5ohm1   |     0.78967 V | Pass   |
----------------------------------------------------
| HDDPS_LC_nogang_12ohm1  |      1.0893 V | Pass   |
----------------------------------------------------
| HDDPS_LC_nogang_250ohm1 |      0.9999 V | Pass   |
----------------------------------------------------
| HDDPS_LC_nogang_250ohm2 |      0.6668 V | Pass   |
----------------------------------------------------
| HDDPS_LC_nogang_12ohm2  |       0.898 V | Pass   |
----------------------------------------------------
| HDDPS_LC_nogang_5ohm2   | 983.8900146 V | Fail   |
----------------------------------------------------
[DUT: 1]=== Processing results ===.
[DUT: 1]
=====================================================================================
Calculated final port = [PORT_FAIL_FUNCTIONAL_PASS_DC, Functional fail, DC test pass]
=====================================================================================
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|                       Port Description | Detail                                                                                                                              |                Pin Name | Values            |
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
| Functional fail upper shorts and opens |                                                                                                                                     |                         |                   |
|          Functional fail, DC test pass | The pin=[HDDPS_LC_nogang_5ohm1](FAIL)[promoted to PASS] results from a [DC open fault] detected in this [DC short] test instance.   |   HDDPS_LC_nogang_5ohm1 | 0.789669990539551 |
|          Functional fail, DC test pass | The pin=[HDDPS_LC_nogang_12ohm1](FAIL)[promoted to PASS] results from a [DC open fault] detected in this [DC short] test instance.  |  HDDPS_LC_nogang_12ohm1 | 1.08930003643036  |
|          Functional fail, DC test pass | The pin=[HDDPS_LC_nogang_250ohm1](FAIL)[promoted to PASS] results from a [DC open fault] detected in this [DC short] test instance. | HDDPS_LC_nogang_250ohm1 | 0.999899983406067 |
|          Functional fail, DC test pass | The pin=[HDDPS_LC_nogang_250ohm2](FAIL)[promoted to PASS] results from a [DC open fault] detected in this [DC short] test instance. | HDDPS_LC_nogang_250ohm2 | 0.666800022125244 |
|          Functional fail, DC test pass | The pin=[HDDPS_LC_nogang_12ohm2](FAIL)[promoted to PASS] results from a [DC open fault] detected in this [DC short] test instance.  |  HDDPS_LC_nogang_12ohm2 | 0.898000001907349 |
|          Functional fail, DC test pass | The pin=[HDDPS_LC_nogang_5ohm2](FAIL)[promoted to PASS] results from a [DC open fault] detected in this [DC short] test instance.   |   HDDPS_LC_nogang_5ohm2 | 983.890014648438  |
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
[DUT: 1]Test instance=[Shops::PinLCUpperDiode_FuncFailDcPass_P2] executed using 22.273500 ms
[DUT: 1]TestInstance=[Shops::PinLCUpperDiode_FuncFailDcPass_P2] exit port=[2].
[2023-Jul-16 21:04:56.371][A][TAL][DUT: 1] StopTest PrimeShopsTestMethod::Shops::PinLCUpperDiode_FuncFailDcPass_P2
[2023-Jul-16 21:04:56.372][A][TAL][DUT: 1] StartTest PrimeShopsTestMethod::Shops::PinHCLowerDiode_FuncAndDcFail_P4
[DUT: 1]
```

## Datalog output

Dc results are logged to Ituff in “composite” format as shown below

2\_tname\_Dc::PrimeShopsDcTest

2\_category\_pc
2\_composite\_2028\_0.001488
2\_composite\_2028\_0.001488
2\_composite\_2020\_0.002499
2\_composite\_2020\_0.002499
2\_composite\_2004\_0.003490
2\_composite\_2004\_0.003490
2\_composite\_7051\_0.000496
2\_composite\_7051\_0.000496
2\_composite\_7107\_0.000496

## Custom User Code Hooks

Some methods in PrimeShops can be overwritten by extensions, the [diagram](#Diagrams-of-Shops-methodology) show the location of each extensible method into verify and execute flow highlighted by red stroke dasharray.
Next table has a light description of each extensible method:

| **Callback Names**                      | **Inputs**               | **Output**                      | **Description**                                                                                                    |
| --------------------------------------- | ------------------------ | ------------------------------- | ------------------------------------------------------------------------------------------------------------------ |
| **CustomGetDcTest**                     | Pins and Measuremenets   | DcTest object                   | This method is instended to set SuperPinGroup definition or modify the DCTest in any way in verify.                |
| **CustomPreExecute**                    | None                     | None                            | Called early during Execute to add some user-custom behavior before the test starts.                               |
| **CustomPostProcessResults**            | ShopsTestInstanceResults | None                            | Called right after successful execution to provide the user the ability to post process exit port, and DC results. |
| **CreateTestInstanceResults**           | None                     | ShopsTestInstanceResults object | Called prior to any test in Execute to generate the test instance results object.                                  |
| **DcProcess**                           | None                     | Results and DcResults           | Called intraloop to provide the user the ability to process DC results.                                            |

### Examples of user codes extensions

```CS
namespace IntegrationTestUserCode.Dc
{
    using System.Collections.Generic;
    using Prime.DatalogService;
    using Prime.DcService;
    using Prime.PhAttributes;
    using Prime.PlatformService;
    using Prime.TestMethods.Dc;

    /// <summary>
    /// This class is intended to overwrite the test method PrimeShopsTestMethod.
    /// </summary>
    [PrimeTestMethod]
    public class ShopsWithSuperPinGroup : PrimeShopsTestMethod, IShopsExtensions
    {
        /// <inheritdoc />
        IDcTest IShopsExtensions.CustomGetDcTest(List<string> pins, List<MeasurementType> measurements)
        {
            var dcTest = Prime.Base.ServiceStore<IDcService>.Service.GetDcTest(pins, measurements);

            var superPinGroupDic = new Dictionary<string, List<string>>()
            {
                { "MySuperPinGroup1", new List<string>() { "HDDPS_LC_nogang_5ohm1", "HDDPS_LC_nogang_12ohm1" } },
                { "MySuperPinGroup2", new List<string>() { "HDDPS_LC_nogang_250ohm1", "HDDPS_LC_nogang_250ohm2" } },
                { "MySuperPinGroup3", new List<string>() { "HDDPS_LC_nogang_12ohm2", "HDDPS_LC_nogang_5ohm2" } },
            };

            dcTest.SetSuperPinGroup(superPinGroupDic);

            return dcTest;
        }

        /// <inheritdoc />
        void IShopsExtensions.CustomPreExecute()
        {
            var writter = Prime.Base.ServiceStore<IDatalogService>.Service.GetItuffStrgvalWriter();
            writter.SetData($"Shop_Ext=[{nameof(IShopsExtensions.CustomPreExecute)}]");
            writter.SetTnamePostfix($"_Ext_{nameof(IShopsExtensions.CustomPreExecute)}");
            Prime.Base.ServiceStore<IDatalogService>.Service.WriteToItuff(writter);
        }

        /// <inheritdoc />
        ShopsTestInstanceResults IShopsExtensions.CreateTestInstanceResults()
        {
            var writter = Prime.Base.ServiceStore<IDatalogService>.Service.GetItuffStrgvalWriter();
            writter.SetData($"Shop_Ext=[{nameof(IShopsExtensions.CreateTestInstanceResults)}]");
            writter.SetTnamePostfix($"_Ext_{nameof(IShopsExtensions.CreateTestInstanceResults)}");
            Prime.Base.ServiceStore<IDatalogService>.Service.WriteToItuff(writter);

            return new ShopsTestInstanceResults();
        }

        /// <inheritdoc />
        void IShopsExtensions.CustomPostProcessResults(ShopsTestInstanceResults results)
        {
            var writter = Prime.Base.ServiceStore<IDatalogService>.Service.GetItuffStrgvalWriter();
            writter.SetData($"Shop_Ext=[{nameof(IShopsExtensions.CustomPostProcessResults)}]");
            writter.SetTnamePostfix($"_Ext_{nameof(IShopsExtensions.CustomPostProcessResults)}");
            Prime.Base.ServiceStore<IDatalogService>.Service.WriteToItuff(writter);
        }

        /// <inheritdoc />
        void IShopsExtensions.DcProcess(ShopsTestInstanceResults results, IDcResults perPinResults)
        {
            var writter = Prime.Base.ServiceStore<IDatalogService>.Service.GetItuffStrgvalWriter();
            writter.SetData($"Shop_Ext=[{nameof(IShopsExtensions.DcProcess)}]");
            writter.SetTnamePostfix($"_Ext_{nameof(IShopsExtensions.DcProcess)}");
            Prime.Base.ServiceStore<IDatalogService>.Service.WriteToItuff(writter);

            results.DcPinResults.Add(perPinResults);
        }
    }
}
```

#### Redirect port result by user algoritms in extensible method
Redirect the output port is enable in CustomPostProcessResults method, this method is called once all functionals and dcTests were executed success and when this method is called the user can read the result of all of them, in this point is enable to add any algorithms or post process criterial to set a final port, user has the responsibility of add a new result of type **USER_EXTENSION** with the new port output assigned, this result will have the highest priority.

##### User code redirect port example
```CS
namespace IntegrationTestUserCode.Dc
{
    using Prime.DcService;
    using Prime.PhAttributes;
    using Prime.TestMethods.Dc;

    /// <summary>
    /// This class is intended to overwrite the test method PrimeShopsTestMethod.
    /// </summary>
    [PrimeTestMethod]
    public class ShopsWithUserRedirectPort : PrimeShopsTestMethod, IShopsExtensions
    {
        /// <inheritdoc />
        void IShopsExtensions.CustomPostProcessResults(ShopsTestInstanceResults results)
        {
            // User here can write the algorithms to set the new port, also here the user has access to all results from functionals and DcTest.
            var userDetail = "User detail description only for debug"; // This field is intended to write a detail that will be printed in result table in console file when debug mode is enable,
            IPinDcResults userResults = null;                          // User can build a new results that can be printed in ituff, default null object.
            var userPort = 10;                                         // User can set here the new port value.
            var shopsTestResultType = ShopsTestResults.USER_EXTENSION; // This result type has the highest priority, if user add more of one with this type the first one will be selected to set the port.

            results.AddShopResult(shopsTestResultType, userDetail, userResults, userPort);
        }
    }
}
```

## TPL Samples

Here are a few test instance examples using the Shops test method.

### User set only IForce value

```python
Import PrimeShopsTestMethod.xml;
Test PrimeShopsTestMethod PinHCLowAndUpperDiode_FailLowerDiodeOnlyForOnePin_P4
{
    LogLevel = "PRIME_DEBUG";
    UpperLevelsTc = "SampleShopsTestMethodTC";
    LowerLevelsTc = "SampleShopsTestMethodTC";
    DcLevelsTc = "SampleShopsTestMethodTC";
    PatList = "Shops1_Plist";
    TimingsTc = "basic_Shops_timing_10MHz_20MHz";
    LowerDiodeForceAttributes = "200mA";
    LowerDiodeLimitLow = "545mV";
    LowerDiodeLimitHigh = "1.1V";
    UpperDiodeForceAttributes = "200mA";
    UpperDiodeLimitLow = "24mV";
    UpperDiodeLimitHigh = "456.78mV";
    Pins = "HDDPS_HC_nogang_5ohm1, HDDPS_HC_nogang_12ohm1, HDDPS_HC_nogang_250ohm1, HDDPS_HC_nogang_250ohm2, HDDPS_HC_nogang_12ohm2, HDDPS_HC_nogang_5ohm2";
    EnableFlushSmartTc = "Enabled";
    MeasureSequence = "SERIAL";
}
```

```python
Import PrimeShopsTestMethod.xml;
Test PrimeShopsTestMethod PinLCLowerDiodeOnlyDcTest_FailSerialSequence_P4
{
    LogLevel = "PRIME_DEBUG";
    UpperLevelsTc = "SampleShopsTestMethodTC";
    DcLevelsTc = "SampleShopsTestMethodTC";
    PatList = "Shops1_Plist";
    TimingsTc = "basic_Shops_timing_10MHz_20MHz";
    LowerDiodeForceAttributes = "200mA";
    LowerDiodeLimitLow = "34mV";
    LowerDiodeLimitHigh = "333mV";
    Pins = "HDDPS_LC_nogang_5ohm1, HDDPS_LC_nogang_12ohm1, HDDPS_LC_nogang_250ohm1, HDDPS_LC_nogang_250ohm2, HDDPS_LC_nogang_12ohm2,HDDPS_LC_nogang_5ohm2";
    EnableFlushSmartTc = "Enabled";
    ShopsTestType = "LOWER_SHORTS_AND_OPENS";
    TemplateMode = "DC_ONLY";
    MeasureSequence = "PARALLEL";
    DatalogLevel = "ALL";
}
```

### User set multiples attributes

```python
Import PrimeShopsTestMethod.xml;
Test PrimeShopsTestMethod PinLCUpperDiodeShort_TesterReturnOverflowValue_P6
{
    LogLevel = "Enabled";
    DcLevelsTc = "SampleShopsTestMethodTC";
    PatList = "Shops1_Plist";
    UpperDiodeForceAttributes = "OPModeCheck=ISVM|IForce=200mA|OverVoltageLimit=1.5V|UnderVoltageLimit=-1.5V";
    UpperDiodeLimitLow = "567mV";
    UpperDiodeLimitHigh = "1.2V";
    Pins = "HDDPS_LC_nogang_5ohm1, HDDPS_LC_nogang_12ohm1, HDDPS_LC_nogang_250ohm1, HDDPS_LC_nogang_250ohm2, HDDPS_LC_nogang_12ohm2,HDDPS_LC_nogang_5ohm2";
    TemplateMode = "DC_ONLY";
    ShopsTestType = "UPPER_SHORTS_AND_OPENS";
    DatalogLevel = "FAIL_ONLY";
    MeasureSequence = "SERIAL";
}
```

### User provide configuration file to be consum by PrimeShops
```python
Import PrimeShopsTestMethod.xml;
Test PrimeShopsTestMethod PinLCLowerDiodeOnlyDcTest_FailSerialSequenceUsingJsonFileConfig_P4
{
    LogLevel = "Enabled";
    UpperLevelsTc = "SampleShopsTestMethodTC";
    DcLevelsTc = "SampleShopsTestMethodTC";
    PatList = "Shops1_Plist";
    TimingsTc = "basic_Shops_timing_10MHz_20MHz";
    LowerDiodeForceAttributes = "~HDMT_TPL_DIR/TestPrograms/Shops/Modules/Shops/InputFiles/shopsConfigFile.json";
    LowerDiodeLimitLow = "34mV";
    LowerDiodeLimitHigh = "333mV";
    Pins = "HDDPS_LC_nogang_5ohm1, HDDPS_LC_nogang_12ohm1, HDDPS_LC_nogang_250ohm1, HDDPS_LC_nogang_250ohm2, HDDPS_LC_nogang_12ohm2,HDDPS_LC_nogang_5ohm2";
    EnableFlushSmartTc = "Enabled";
    ShopsTestType = "LOWER_SHORTS_AND_OPENS";
    TemplateMode = "DC_ONLY";
    MeasureSequence = "SERIAL";
    DatalogLevel = "ALL";
}
```
#### Example of JSON file with attributes per pin
This feature is intended to provide a way to execute shops a lot of pins in serial mode with a high level of customization for test time performance.

##### Description of attributes

| **Parameter Names**                     | **Is required?**         | **Type**                        | **Description**                                                        |
| --------------------------------------- | ------------------------ | ------------------------------- | ---------------------------------------------------------------------- |
| **Pins**                                | Yes                      | List<string>                    | List of pins to apply configuration in measurement.                    |
| **Attributes**                          | Yes                      | Dictionary<string, string>      | Dictionary of attributes to measure the pins.                          |
| **AfterMeasureSequenceBreak**           | No                       | string                          | SequenseBreak after of each pin measure, only for serial mode.        |
| **AfterCrackDownSequenceBreak**         | No                       | string                          | SequenseBreak after of each crack down per pin, only for serial mode. |

##### Json File
```JSON
[
  {
    "Pins": [
      "HDDPS_LC_nogang_5ohm1",
      "HDDPS_LC_nogang_250ohm2"
    ],
    "Attributes": {
      "IForce": "123mA",
      "OPModeCheck": "ISVM",
      "OverVoltageLimit": "1.5V",
      "UnderVoltageLimit": "-1.5V"
    },
    "AfterMeasureSequenceBreak": "17ms",
    "AfterCrackDownSequenceBreak": "47ms"
  },
  {
    "Pins": [
      "HDDPS_LC_nogang_12ohm1",
      "HDDPS_LC_nogang_12ohm2"
    ],
    "Attributes": {
      "IForce": "456mA",
      "OPModeCheck": "ISVM",
      "OverVoltageLimit": "1.5V",
      "UnderVoltageLimit": "-1.5V"
    },
    "AfterMeasureSequenceBreak": "12ms",
    "AfterCrackDownSequenceBreak": "52ms"
  },
  {
    "Pins": [
      "HDDPS_LC_nogang_250ohm1",
      "HDDPS_LC_nogang_5ohm2"
    ],
    "Attributes": {
      "IForce": "456mA",
      "OPModeCheck": "ISVM",
      "OverVoltageLimit": "1.5V",
      "UnderVoltageLimit": "-1.5V"
    },
    "AfterMeasureSequenceBreak": "26ms",
    "AfterCrackDownSequenceBreak": "62ms"
  }
]
```

## Exit Ports

The Dc test method supports the following exit ports:

| **Exit Port** | **Condition** | **Description**                                          |
| ------------- | ------------- | -------------------------------------------------------- |
| **-2**        | ***Alarm***   | Any alarm condition                                      |
| **-1**        | ***Error***   | Any software condition error                             |
| **0**         | ***Fail***    | Functional fail lower shorts and opens.                  |
| **1**         | ***Pass***    | Test Pass.                                               |
| **2**         | ***Fail***    | Functional fail, DC test pass.                           |
| **3**         | ***Fail***    | Functional fail upper shorts and opens.                  |
| **4**         | ***Fail***    | Fail DC shorts.                                          |
| **5**         | ***Fail***    | Fail DC lower opens.                                     |
| **6**         | ***Fail***    | Fail DC upper opens.                                     |
| **7**         | ***Fail***    | Port fail alarm port enabled.                            |
| **8-99**      | ***Pass***    | PORT TO BE USED BY USER WITH REDIRECT PORT               |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **SHOPS**: **Sh**orts and **Op**en**s** test methodology
  - **DC**: Direct current
  - **ITMH**: Intel Test Methodology handboock
  - **ISVM**: This is a HTM_DCParametric to force a current and measure the resultant voltage .

## Version tracking

| **Date**                  | **Prime Version** | **Author**      | **Comments**                                                                                   |
| ------------------------- | ----------- | --------------------- | ---------------------------------------------------------------------------------------------- |
| Jun 13<sup>th</sup>, 2022 | 11.0        | Didier Jimenez Retana | Initial version                                                                                |
| Nob 10<sup>th</sup>, 2022 | 12.0        | Didier Jimenez Retana | [Fix serial behavior with pinGroups.](https://dev.azure.com/mit-us/PRIME/_workitems/edit/34056)|
| Jan 18<sup>th</sup>, 2023 | 12.0        | Didier Jimenez Retana | [Fix FlushSmartTc behavior for sereal mode. Added ALL_DATA_PIN_DETAIL mode to print in ituff.](https://dev.azure.com/mit-us/PRIME/_workitems/edit/31052)|
| Jul 18<sup>th</sup>, 2023 | 12.3        | Didier Jimenez Retana | [[Shops] Define Powerdown/Crackdown level and set attributes for correct measurement with Parallel/Serial execution](https://dev.azure.com/mit-us/PRIME/_workitems/edit/39001)|
| May 16<sup>th</sup>, 2024 | 13.0        | Didier Jimenez Retana | Refactoring to enabling use this testMethod with TOS4 infrastructure.|
