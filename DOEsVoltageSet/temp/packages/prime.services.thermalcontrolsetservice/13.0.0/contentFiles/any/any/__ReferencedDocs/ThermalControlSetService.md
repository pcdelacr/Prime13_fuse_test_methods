[[_TOC_]]

# Prerequisites

<p style="font-size: 20px;"><span style="color: #cf6679;">This Service is using Aleph initialization. Please refer to Aleph documentation</span></p>

<p style="font-size: 20px;"><span style="color: #cf6679;">Configuration file format changed with respect to Prime 10.</span></p>

<p style="font-size: 20px;"><span style="color: #cf6679;">Multizone support is not fully validated yet.</span></p>

# Description

The ThermalControlSet Service reads the json configurationfile in Aleph to load the configurations set to later be used by the ThermalControlSet Test Method instances (Once the configurations are loaded the TempSet needs to be selected using the ThermalControlSetInit Test Method).
In order for the configuration file to be read it's name has to be "ControlSetConfiguration.json".

This service doesn't have any method available for extension.

# Configuration File

Example file:
```json
{
{
  "PinNames": [
    "UVS_100"
  ],
  "TCCTempSet": [
    {
      "name": "tgl_u42_ct_100C_TCC",
      "ControlSetMapping": {
        "SOAK": 1,
        "DIODESOT": 2,
        "TEST": 3,
        "DESOAK": 4,
        "IDLE": 5,
        "DISPENSE": 6,
        "PPR": 7
      }
    },
    {
      "name": "tgl_u42_ct_-50C_TCC",
      "ControlSetMapping": {
        "SOAK2": 1,
        "DIODESOT2": 2,
        "TEST2": 3,
        "TESTRJHRISE": 4,
        "IDLE2": 5,
        "DISPENSE2": 6
      }
    },
    {
      "name": "tgl_u42_ct_-10C_TCC",
      "ControlSetMapping": {
        "SOAK2": 1,
        "DIODESOT2": 2,
        "TEST2": 3,
        "TESTRJHRISE": 4,
        "IDLE2": 5,
        "DISPENSE2": 6
      }
    }
  ]
}

```

## Field details
- **PinNames**: List (at least one element) of UEI pins that are going to be used along TP execution.
example for Multizone definition:
```json
  "PinNames": [
    "IPA::UVS_100",
    "IPB::UVS_101",
    "IPC::UVS_102"
  ],
```
- **TCCTempSet**: Array of thermal controller configurations (at least one element).
    - **name**: Configuration name.
    - **ControlSetMapping**: A dictionary that maps each control set name with their corresponding recipe index.
         format: "ControlSetName": Index

**Example for the aleph files information**

```python
ALEPH_FILES = "~HDMT_TPL_DIR\Modules\THERMAL\THERMAL\InputFiles\ControlSetConfiguration.json";
```

# Version tracking

| **Date**   | **Prime release** | **Author**    | **Comments** |
| ---------- | ----------- | ------------- | ------------ |
| Feb, 2022 | 8.00.00       | Viviana Villalobos | Add Aleph initialization for the configuration file. |
| Jul, 2022 | 11.00.00       | Johnny Mata | Add Multiple UEI pin support. |