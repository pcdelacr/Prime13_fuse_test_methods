[[_TOC_]]

# Prerequisites

Don't apply.

# ScoreboardLogger
The ScoreboardLogger is a C# service, only provide the tools to process packets of bits.

#Configuration File
<p>Aleph initialization only recognize files with the name "<span style="color:Lime;  font-weight: bold">ScoreboardLogger.json</span>".</p>

As follows an example of the content of this file:
```json
{
  "PacketConfigurations": [
    {
      "Name": "one",
      "DataPins": [ "P3", "P6", "P7" ],
      "IdPins": [ "P010" ],
      "ValidPins": [ "P008" ],
      "ValidValues": [ "1" ],
      "PacketSize": 1,
      "ErrorWhenPacketSizeMismatch": true
    },
    {
      "Name": "BuilderConfiguration_1",
      "DataPins": [ "P002" ],
      "IdPins": [ "P004" ],
      "ValidPins": [ "P008" ],
      "ValidValues": [ "0" ],
      "PacketSize": 24,
      "ErrorWhenPacketSizeMismatch": false
    },
    {
      "Name": "BuilderConfiguration_2",
      "DataPins": [ "xxHPCC_DPIN_Dig_slcA_A0" ],
      "IdPins": [ "xxHPCC_DPIN_Dig_slcA_A1" ],
      "ValidPins": [ "xxHPCC_DPIN_Dig_slcA_A3" ],
      "ValidValues": [ "0" ],
      "PacketSize": 24,
      "ErrorWhenPacketSizeMismatch": false
    },
    {
      "Name": "BuilderConfiguration_3",
      "DataPins": [
        "xxHPCC_DPIN_Dig_slcA_A0",
        "xxHPCC_DPIN_Dig_slcA_A1",
        "xxHPCC_DPIN_Dig_slcA_A2",
        "xxHPCC_DPIN_Dig_slcA_A3",
        "xxHPCC_DPIN_Dig_slcA_A4",
        "xxHPCC_DPIN_Dig_slcA_A5",
        "xxHPCC_DPIN_Dig_slcA_A6"
      ],
      "IdPins": [ "xxHPCC_DPIN_Dig_slcA_A7", "xxHPCC_DPIN_Dig_slcA_AA0" ],
      "ValidPins": [ "xxHPCC_DPIN_Dig_slcA_AA1" ],
      "ValidValues": [ "0" ],
      "PacketSize": 7,
      "ErrorWhenPacketSizeMismatch": false
    },
    {
      "Name": "BuilderConfiguration_4",
      "DataPins": [
        "xxHPCC_DPIN_Dig_slcA_A0",
        "xxHPCC_DPIN_Dig_slcA_A1",
        "xxHPCC_DPIN_Dig_slcA_A2",
        "xxHPCC_DPIN_Dig_slcA_A3",
        "xxHPCC_DPIN_Dig_slcA_A4",
        "xxHPCC_DPIN_Dig_slcA_A5",
        "xxHPCC_DPIN_Dig_slcA_A6"
      ],
      "IdPins": [ "xxHPCC_DPIN_Dig_slcA_A7", "xxHPCC_DPIN_Dig_slcA_AA0" ],
      "ValidPins": [ "xxHPCC_DPIN_Dig_slcA_AA1", "xxHPCC_DPIN_Dig_slcA_AA2" ],
      "ValidValues": [ "0", "1" ],
      "PacketSize": 7,
      "ErrorWhenPacketSizeMismatch": false
    },
    {
      "Name": "BuilderConfiguration_5",
      "DataPins": [ "P002", "P004", "P006", "P008", "P010", "P012", "P014" ],
      "IdPins": [ "P016", "P018" ],
      "ValidPins": [ "P020" ],
      "ValidValues": [ "0" ],
      "PacketSize": 7,
      "ErrorWhenPacketSizeMismatch": false
    },
    {
      "Name": "BuilderConfiguration_7",
      "DataPins": [ "P002", "P004", "P006", "P008", "P010", "P012", "P014" ],
      "IdPins": [ "P016", "P018" ],
      "ValidPins": [ "P020", "P022" ],
      "ValidValues": [ "0", "1" ],
      "PacketSize": 7,
      "ErrorWhenPacketSizeMismatch": false
    },
    {
      "Name": "BuilderConfiguration_8",
      "DataPins": [ "xxHPCC_DPIN_Dig_slcA_A0", "xxHPCC_DPIN_Dig_slcA_A1", "xxHPCC_DPIN_Dig_slcA_A2" ],
      "IdPins": [ "xxHPCC_DPIN_Dig_slcA_A7", "xxHPCC_DPIN_Dig_slcA_AA0" ],
      "ValidPins": [ "xxHPCC_DPIN_Dig_slcA_AA1" ],
      "ValidValues": [ "0" ],
      "PacketSize": 6,
      "ErrorWhenPacketSizeMismatch": false
    }
  ]
}
```

Field details:
- _**Packet fields**_: 
  - _**Name**_:  The name used to call the header service with the configuration to use.
  - _**DataPins**_: names of Pins with data to process.
  - _**IdPins**_: names of Pins that represent the id of the data.
  - _**ValidPins**_: names of Pins that represent the valid value of each vector.
  - _**ValidValues**_: Value required in ValidPins to accept the vector as a valid to process.
  - _**PacketSize**_: number of bits per packet.
  - _**ErrorWhenPacketSizeMismatch**_: boolean to set if is required to fail if the data has incomplete packets.

<p><span style="font-weight: bold">Note:</span> If "<span style="color:lightblue; font-weight: bold">ErrorWhenPacketSizeMismatch</span>" is <span style="color:red; font-weight: bold">false</span> the incomplete packets are added to the list.</p>

# APIs description
## Get Handle APIs
In order to get the configuration from json file, the user must get a handle for the required configuration.
These APIs will return an object with the configuration to build the packets.

**- GetPacketHandler(ConfigurationName)**
This is the get handle, the user will give a configuration name and will get handle to this configuration.

## Process data APIs
**- ProcessCtvData(CtvData)**
This API belongs to the handle. This API let the user process the CTV data.
**- ProcessFailureData(failureList)**
This API belongs to the handle. This API let the user process the failure data.

## Get packets APIs
- ###GetPackets()
This is the basic get packets, after to process Ctvs or Failures the user can get all packets with the configuration defined in packetHander object.

- ###GetPacketsReceivedId()
Similar to previous one but including received sequense id.

- ###GetValidSequenceIds(listOfKeys)
After to process CTVs or Failures, the user can get filtered data with the specified keys values.

- ###GetPacket(Key)
Get packets with specific key.

- ###GetInvalidPackets()
Get all packets discarded by valid data.

- ###GetUnexpectedPinFailures()
This get packet only can be used with failures data, return the names of all pins with failures unexpected or undefined by configuration.

# Terminology
**- Row:** A group of fixed number of packets.
**- Packet:** A group of bit captured within the same pattern vector.

<table>
 <tr>
  <td colspan="3" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Row 0</td>
  <td colspan="3" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Row 1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle">&nbsp;Packet0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Packet1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Packet2</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Packet0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Packet1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;Packet2</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; color: lightblue">&nbsp;000</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;001</td>
  <td style="text-align: center; vertical-align: middle; color: Fuchsia">&nbsp;100</td>
  <td style="text-align: center; vertical-align: middle; color: red">&nbsp;000</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;110</td>
  <td style="text-align: center; vertical-align: middle; color: Olive">&nbsp;100</td>
 </tr>
</table>

# Examples
-  ##Example 1: simple case.
Packet configuration used for this example.
```json
{
  "example_1": {
    "DataPins": [ "P002", "P004", "P006" ],
    "IdPins": [ "P010" ],
    "ValidPins": [ "P008" ],
    "ValidValues": [ "1" ],
    "PacketSize": 9,
    "ErrorWhenPacketSizeMismatch": false
  }
}
```
User code implementation for this example.
```c
var packetHandler = this.GetPacketHandler("example_1");
packetHandler.ProcessCtvData(ctvData);
```
CtvData are visualized in the next table. 
<table>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Vector</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;CTV</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;MTV</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P002</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P004</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P006</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P008</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P010</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;4</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;6</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;8</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;9</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
 </tr>
</table>

<p>The categorizations formed from the previous pattern results look as follows. Notice that <span style="color:Aqua;font-weight:bold;">line-through</span> data is ignored because it is not valid.

<table>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;ID</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Is Valid?<br/>P008</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P002</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P004</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P006</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: red">&nbsp;&#xd7;</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: red">&nbsp;&#xd7;</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: red">&nbsp;&#xd7;</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: Fuchsia">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: Fuchsia">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Fuchsia">&nbsp;0</td>
 </tr>
</table>
Then,

<p>Packet with ID = 1: <span style="color:orange">000</span><span style="color:Lime">110</span><span style="color:Fuchsia">100</span></p>
<p>Packet with ID = 0: <span style="color:Aqua">000</span> <span style="color:red;font-weight:bold;">(incomplete)</span> <sup>Note: ErrorWhenPacketSizeMismatch should be false, if it is true ScoreboardLogger fail by exception.</sup></p>
<p>Invalid Packets = <span style="text-decoration: line-through">000 000 001</span></p>

-  ###Get packets APIs values returned.
1. GetPackets()
```c
// C# object returned by GetPacket() Api.
new Dictionar<string, List<string>>() 
    {
        { "0", new List<string>() { "000" } },
        { "1", new List<string>() { "000110100" } }
    }
```
2. GetValidSequenceIds(listOfKeys)
```c
// Possible Inputs object for GetValidSequenceIds(inputValue).
var listOfKeys1 = new List<string>() { "0" };
var listOfKeys2 = new List<string>() { "1" };
var listOfAll   = new List<string>() { "0", "1" };
```
```c
//C# Get packets.
var listOfPacktes1 = myHandler.GetValidSequenceIds(listOfKeys1)
var listOfPacktes2 = myHandler.GetValidSequenceIds(listOfKeys2 )
var listOfAllPacktes = myHandler.GetValidSequenceIds(listOfAll)
```
```c
//C# object returned by GetValidSequenceIds(inputValue) Api.
listOfPacktes1 = new List<string>() { "000" };
listOfPacktes2 = new List<string>() { "000110100" };
listOfAllPacktes = new List<string>() { "000", "000110100" };
```
3. GetPacket(Key)
```c
//Possible string Inputs for GetPacket(value) are "0" and "1".
var listOfPacktes1 = packetHandler.GetPacket("0");
var listOfPacktes2 = packetHandler.GetPacket("1");
```
```c
// C# objects returned by each key.
var listOfPacktes1 = new List<string>() { "000" };
var listOfPacktes2 = new List<string>() { "000110100" };
```
4. GetInvalidPackets()
```c
// C# object returned by GetInvalidPackets().
var listInvalidPackets = new List<string>() { "000000001" };
```
-  ##Example 2.
Packet configuration used for this example.
```json
{
  "example_2": {
    "DataPins": [ "P002", "P004", "P008" ],
    "IdPins": [ "P004", "P006" ],
    "ValidPins": [ "P008", "P010" ],
    "ValidValues": [ "1", "0" ],
    "PacketSize": 3,
    "ErrorWhenPacketSizeMismatch": true
  }
}
```
User code implementation for this example.
```c
var packetHandler = this.GetPacketHandler("example_2");
packetHandler.ProcessCtvData(ctvData);
```
CtvData are visualized in the next table. 
<table>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Vector</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;CTV</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;MTV</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P002</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P004</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P006</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P008</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P010</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;4</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;6</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;8</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;9</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;10</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;11</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;12</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;13</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;14</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
</table>

<p>The categorizations formed from the previous pattern results look as follows. Notice that <span style="color:Aqua;font-weight:bold;">line-through</span> data is ignored because it is not valid.

<table>
 <tr>
  <td colspan="2" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;ID</td>
  <td colspan="2" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Is Valid?</td>
  <td rowspan="2" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P002</td>
  <td rowspan="2" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P004</td>
  <td rowspan="2" style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P006</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P004</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P006</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P008</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P010</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;1 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;0 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: red">&nbsp;0 &#xd7;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;0 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;1 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;0 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: red">&nbsp;0 &#xd7;</td>
  <td style="text-align: center; vertical-align: middle; color: red">&nbsp;1 &#xd7;</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;1 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;0 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;1 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;0 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;1 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;0 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;1 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;0 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;1 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: red">&nbsp;1 &#xd7;</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;1 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;0 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;1 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;0 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;1 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;0 &#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;0</td>
 </tr>
</table>

Then,

<p>Packets with ID = 11: <span style="color:orange">011 011 011</span></p>
<p>Packets with ID = 01: <span style="color:Aqua">001 001 001</span></p>
<p>Packets with ID = 10: <span style="color:Lime">010 010 010</span></p>
<p>Invalid Packets = <span style="text-decoration: line-through">011 001 001</span></p>

-  ###Get packets APIs values returned.
1. GetPackets()
```c
// C# object returned by GetPacket Api.
new Dictionar<string, List<string>>() 
    {
        { "01", new List<string>() { "001", "001", "001" } },
        { "10", new List<string>() { "010", "010", "010" } },
        { "11", new List<string>() { "011", "011", "011" } },
    }
```
2. GetValidSequenceIds(listOfKeys)
```c
// Get packets.
var listOfPacktes1 = myHandler.GetValidSequenceIds("00", "01");
var listOfPacktes2 = myHandler.GetValidSequenceIds("10", "11");
var listOfPacktes3 = myHandler.GetValidSequenceIds("00", "01", "10");
var listOfPacktes4 = myHandler.GetValidSequenceIds("01", "11", "10");
```
```c
//C# object returned by GetValidSequenceIds(inputValue) Api.
listOfPacktes1 = new List<string>() { "001", "001", "001" };                    //myHandler.GetValidSequenceIds("00", "01");
listOfPacktes2 = new List<string>() { "010011", "010011", "010011" };           //myHandler.GetValidSequenceIds("10", "11");
listOfPacktes3 = new List<string>() { "001010", "001010", "001010" };           //myHandler.GetValidSequenceIds("00", "01", "10");
listOfPacktes4 = new List<string>() { "001011010", "001011010", "001011010" };  //myHandler.GetValidSequenceIds("01", "11", "10")
```
3. GetPacket(Key)
```c
//Possible string Inputs for GetPacket(value) are "01", "10" and "11".
var listOfPacktes1 = packetHandler.GetPacket("01");
var listOfPacktes2 = packetHandler.GetPacket("10");
var listOfPacktes3 = packetHandler.GetPacket("11");
```
```c
// C# objects returned by each key.
var listOfPacktes1 = new List<string>() { "001", "001", "001" };
var listOfPacktes2 = new List<string>() { "010", "010", "010" };
var listOfPacktes2 = new List<string>() { "011", "011", "011" };
```
4. GetInvalidPackets()
```c
// C# object returned by GetInvalidPackets().
var listInvalidPackets = new List<string>() { "011", "001", "001" };
```
-  ##Example 3: Proccesing Failure data.
Packet configuration used for this example.
```json
{
  "example_3": {
    "DataPins": [ "18", "17", "16", "15", "14", "13", "12", "11", "10", "09", "08", "07", "06", "05", "04", "03", "02", "01", "00" ],
    "IdPins": [ "22", "21", "20", "19" ],
    "ValidPins": [ "23" ],
    "ValidValues": [ "1" ],
    "PacketSize": 19,
    "ErrorWhenPacketSizeMismatch": true
  }
}
```
User code implementation for this example.
```c
var packetHandler = this.GetPacketHandler("example_3");
packetHandler.ProcessFailureData(FailurePerCycleData);
```
Failure data are visualized in the next table.

<table style="font-size: 0.8em;">
 <tr>
  <td colspan="2" style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold; font-size: 0.8em;">&nbsp;BITS</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;23</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;22</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;21</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;20</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;19</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;18</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;17</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;16</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;15</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;14</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;13</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;12</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;11</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;10</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;09</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;08</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;07</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;06</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;05</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;04</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;03</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;02</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;01</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse; font-weight: bold">&nbsp;00</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;Format</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;EID</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;DONE</td>
  <td colspan="4" style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;Pkt_Seq</td>
  <td colspan="19" style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;DATA</td>
 </tr>
 <tr style="color: orange">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: orange">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;2</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
 </tr>
 <tr style="color: orange">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: orange">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;4</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
 </tr>
 <tr style="color: orange">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: orange">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;6</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
 </tr>
 <tr style="color: orange">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: orange">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;8</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
 </tr>
 <tr style="color: orange">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;9</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: Aqua">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: Aqua">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;2</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
 </tr>
 <tr style="color: Aqua">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: Aqua">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;4</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
 </tr>
 <tr style="color: Aqua">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: Aqua">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;6</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
 </tr>
 <tr style="color: Aqua">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: Aqua">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;8</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
 </tr>
 <tr style="color: Aqua">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;9</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: Fuchsia">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: Fuchsia">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;2</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
 </tr>
 <tr style="color: Fuchsia">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: Fuchsia">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;4</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
 </tr>
 <tr style="color: Fuchsia">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: Fuchsia">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;6</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
 </tr>
 <tr style="color: Fuchsia">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
 <tr style="color: Fuchsia">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;8</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
 </tr>
 <tr style="color: Fuchsia">
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;9</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; padding:0; margin:0; border-collapse: collapse">&nbsp;1</td>
 </tr>
</table>

-  ###Get packets APIs values returned.
1. GetPackets()
```c
// C# object returned by GetPacket Api.
new Dictionar<string, List<string>>() 
    {
        { "0001", new List<string>() { "0000000000000000001", "0101010101010101001", "1111111111111110001" } },
        { "0010", new List<string>() { "0000000000000000010", "0101010101010101010", "1111111111111110010" } },
        { "0011", new List<string>() { "0000000000000000011", "0101010101010101011", "1111111111111110011" } },
        { "0100", new List<string>() { "0000000000000000100", "0101010101010101100", "1111111111111110100" } },
        { "0101", new List<string>() { "0000000000000000101", "0101010101010101101", "1111111111111110101" } },
        { "0110", new List<string>() { "0000000000000000110", "0101010101010101110", "1111111111111110110" } },
        { "0111", new List<string>() { "0000000000000000111", "0101010101010101111", "1111111111111110111" } },
        { "1000", new List<string>() { "0000000000000001000", "0101010101010101000", "1111111111111111000" } },
        { "1001", new List<string>() { "0000000000000001001", "0101010101010101001", "1111111111111111001" } },
    }
```

2. GetValidSequenceIds(listOfKeys)
```c
// Get packets.
var listOfPacktes1 = myHandler.GetValidSequenceIds(new List<string>() { "0001", "0111" });
var listOfPacktes2 = myHandler.GetValidSequenceIds(new List<string>() { "1001", "0100", "1000" });
```
```c
//C# object returned by GetValidSequenceIds(inputValue) Api.
listOfPacktes1 = new List<string>() { "00000000000000000010000000000000000111", "01010101010101010010101010101010101111", "11111111111111100011111111111111110111" };                                                          //myHandler.GetValidSequenceIds("0001", "0111");
listOfPacktes2 = new List<string>() { "000000000000000100100000000000000001000000000000000001000", "010101010101010100101010101010101011000101010101010101000", "111111111111111100111111111111111101001111111111111111000" }; //myHandler.GetValidSequenceIds("1001", "0100", "1000");
```
The categorizations formed from the previous pattern results look as follows.

# Version tracking

| **Date**   | **Prime release** | **Author**    | **Comments** |
| ---------- | ----------- | ------------- | ------------ |
| Mar, 2021 | 5.00.00       | Didier Armando Jiménez Retana | Add ScoreboardLogger.|
| Jul, 2021 | 6.00.00       | Didier Armando Jiménez Retana | Enable the ScoreboardLogger to build packets from failure.|
| May, 2022 | 10.00.00       | Andrea Gomez Montero | Changing the PacketConfigurations .json file format.|
| Jun, 2022 | 11.00.00       | Didier Armando Jiménez Retana | Add GetPacketsWithReceivedSequenceId.|