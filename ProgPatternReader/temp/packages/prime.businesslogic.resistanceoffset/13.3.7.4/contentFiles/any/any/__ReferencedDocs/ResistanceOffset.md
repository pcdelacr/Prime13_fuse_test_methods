# Prerequisites

<p style="font-size: 20px;"><span style="color: #cf6679;">This Service is using Aleph initialization. Please refer to Aleph documentation</span></p>

[Link to Aleph Documentation](https://dev.azure.com/mit-us/PrimeWiki/_wiki/wikis/PrimeWiki.wiki/28879/Special-ENV-Variables-in-Prime)

# ResistanceOffsetService
The ResistanceOffsetService provides offset Rstray measurements to aid the DC testmethods with measurement accuracy.

# Configuration Files
ResistanceOffsetService requires one configuration file - the [resistance file](#Resistance-file), which holds per-pin, per-TIU resistance values.

It's important to mention that this file is in **JSON** format.

## Resistance file
This file is parsed using the [Aleph mechanism](https://dev.azure.com/mit-us/PrimeWiki/_wiki/wikis/PrimeWiki.wiki/28879/Special-ENV-Variables-in-Prime), meaning in this case that the file name should match the following regex `.*ResistanceOffset.json`, for example `rstray.ResistanceOffset.json`.
Take note that **DutID** parameter is actually **Dut Index** with index start from **1** (one-based index). Dut index is depend on **DUT** sequence define in socket file.

An example of this file content follows:
```json
{
  "$schema": "ResistanceOffset.schema.json",
  "Loadboards": [
    {
      "TiuName": "Fake_TIU",
      "Duts": [
        {
          "DutID": "1",
          "Resistances": [
            {
              "PinName": "xxHPCC_DPIN_PMU_slcB_180Kohm",
              "ResistanceOffset": 8.4
            }
          ]
        },
        {
          "DutID": "2",
          "Resistances": [
            {
              "PinName": "xxHPCC_DPIN_PMU_slcB_180Kohm",
              "ResistanceOffset": 9.4
            }
          ]
        }
      ]
    },

    {
      "TiuName": "SPHT.*",
      "Duts": [
        {
          "DutID": "1",
          "Resistances": [
            {
              "PinName": "xxHPCC_DPIN_PMU_slcA_50ohm1",
              "ResistanceOffset": 11.12
            },
            {
              "PinName": "xxHPCC_DPIN_PMU_slcA_10Kohm",
              "ResistanceOffset": 11.12
            },
            {
              "PinName": "xxHPCC_DPIN_PMU_slcB_200ohm",
              "ResistanceOffset": 11.12
            }
          ]
        },
        {
          "DutID": "2",
          "Resistances": [
            {
              "PinName": "xxHPCC_DPIN_PMU_slcA_50ohm1",
              "ResistanceOffset": 10.12
            },
            {
              "PinName": "xxHPCC_DPIN_PMU_slcA_10Kohm",
              "ResistanceOffset": 10.12
            },
            {
              "PinName": "xxHPCC_DPIN_PMU_slcB_200ohm",
              "ResistanceOffset": 10.12
            }
          ]
        },
        {
          "DutID": "3",
          "Resistances": [
            {
              "PinName": "xxHPCC_DPIN_PMU_slcB_180Kohm",
              "ResistanceOffset": 10.4
            }
          ]
        }
      ]
    }
  ]
}
```

Field details:
- _**$schema**_:  Name of schema file used to check JSON file validity.
- _**Loadboards**_: Under this field, all the relevant TIUs will be defined. Each TIU specification contains the `TiuName` and `Duts` fields.
  - _**TiuName**_: Name of TIU.  (**Wildcards are allowed.**)
  - _**Duts**_: Under this field, all the relevant DUTs will be defined. Each DUT specification contains the `DutID` and `Resistance` fields.
    - _**DutID**_: DUT location number.  (**Must be unique within each Duts field.**)
    - _**Resistances**_: Under this field, all the relevant resistances will be defined. Each resistance specification contains the `PinName` and `ResistanceOffset` fields.
      - _**PinName**_: Name of DUT pin.  (**Pin groups NOT allowed.**)
      - _**ResistanceOffset**_: Double value of measured resistance offset.


# Usage
There are locations in the DC testmethods where the default behavior can be overridden or enhanced by the user.
The intended usage here is to include the ResistanceOffset calls in this PRIME user code.

For additional information, please consult the [User Code Wiki](https://dev.azure.com/mit-us/PrimeWiki/%5C_wiki/wikis/PrimeWiki.wiki/16682/User-Code).


# Example

*TriggeredDC*

By default, the **CustomPostProcessResults** extension checks functional & DC results against limits to choose the output port.
```
void ITriggeredDcExtensions.CustomPostProcessResults(TriggeredDcTestInstanceResults results)
{
    ///////////////////////////////////
    //  To be overridden by user code
    ///////////////////////////////////
    this.PrintToDatalog(results.DcResults);
    bool areDcResultsWithinLimits = results.DcResults.AreAllDcResultsWithinLimits(this.verifyDataStructure.PerPinDcSetup);
    results.ExitPort = (results.FunctionalResult && areDcResultsWithinLimits) ? TestMethodPassPort : TestMethodFailPort;
}
```

This results.DcResults object contains all of the DC results from the test, so one could write user code to ratchet through these results, calculate resistance, and call ResistanceOffsetService as needed.
This example adds in the relevant ResistanceOffset calls, and marks a location for doing the resistance calculation itself.

```
/// <summary>
/// This class is intended to overwrite the test method PrimeTriggeredDcTestMethod for resistance calculation.
/// </summary>
[PrimeTestMethod]
public class TriggeredDcWithDcResultsPostProcessing : PrimeTriggeredDcTestMethod, ITriggeredDcExtensions
{
    /// <summary>
    /// Sample of post-execute operation.
    /// </summary>
    /// <param name="results">Generic results.</param>
    void ITriggeredDcExtensions.CustomPostProcessResults(TriggeredDcTestInstanceResults results)
    {
        var resistanceOffsetHandler = Builder.Build();
        results.DcResults.PrintToConsole();

        var calcport = 1;  // preset to passing port
        var allPinGroupsResults = results.DcResults.GetAllPinGroupsDcResults();
        foreach (var singlePinGroupResult in allPinGroupsResults)
        {
            foreach (var singlePinResult in singlePinGroupResult.GetAllPinsDcResults())
            {
                string pinName = singlePinResult.GetPinName();

                // failure condition - replace this with whatever your failing criteria is
                if (!resistanceOffsetHandler.TryGetResistanceOffset(pinName, out _, this.SessionContext))
                {
                    calcport = 0;
                }

                var offset = resistanceOffsetHandler.GetResistanceOffset(pinName, this.SessionContext);
                // insert your resistance calculation here <pinResistance>
                var resistance = <pinResistance> - offset;
                Prime.Services.ConsoleService.PrintDebug($" Pin [{pinName}] with resistance [{resistance}] and offset [{offset}]");
            }
        }

        results.ExitPort = calcport;
    }
}
```

# Version tracking

| **Date**   | **Author**     | **Prime version** | **Comments**          |
| ---------- | -------------  | ----------------- | --------------------- |
| Feb, 2023  | Kevin D. Krake | - | Initial documentation.|
| May, 2025  | Chen Tat Khoh  | 13.2.1 | Clarify DutID parameter.|
