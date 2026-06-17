[[_TOC_]]

# **ALEPH** 

<p style="font-size: 20px;"><span style="color: #cf6679;">This Service is using Aleph initialization. Please refer to the Aleph documentation for more information.</span></p>

[Link to Aleph Documentation](https://dev.azure.com/mit-us/PrimeWiki/_wiki/wikis/PrimeWiki.wiki/28879/Special-ENV-Variables-in-Prime)

# Terminology
- **Packet**: A grouping of bits. Formed by concatenating strobed failure data.
- **Vector**: A collection of values strobed for a number of pins for a given cycle of a pattern.
- **Cycle**: A cycle of your pattern. Interchangable with the term "Vector".
- **CTV**: Vector data collected through the use of the "CTV" instruction within a pattern. Legacy name for this is "DMEM".
- **PerCycleFails**: Fail data obtained by querying TOS for "PerCycleFailures" (invalid strobes) for a given pattern/plist. Legacy name for this "DFM".

# Summary
PacketDecoder APIs provides tools that allow the processing of vectors into groupings of bits. To put simply, PacketDecoder APIs concatenate vectors into a binary strings based on your configuration.

The data contained in each packet is formed by using values strobed on the data pins defined in your configuration. As for how a packet is exactly formed, that depends on your configuration and the type of decoder you're going to use, but in general, all decoders:

1. Iterate through each cycle of the failing data (in other words, iterating over each vector that was strobed in your pattern).
2. Check if the vector is valid by using the values strobed on the valid pins defined in your configuration.
3. Assign the bits strobed from the data pins to a packet based on the values strobed from the id pins defined in your configuration.

# Configuration File
<p>To use PacketDecoder APIs, you must create a configuration file that is named <span style="color:Lime;  font-weight: bold">PacketDecoder.json</span> for ALEPH initialization. Any other names will prevent the ALEPH initialization needed to configure the PacketDecoder APIs.</p>

The following is an example setup of the PacketDecoder.json configuration:
```json
{
  "PacketConfigurations": [
    {
      "Name": "example",
      "DataPins": [ "P3", "P6", "P7" ],
      "IdPins": [ "P010" ],
      "ValidPins": [ "P008" ],
      "ValidValues": [ "1" ],
      "PacketSize": 1,
      "ErrorWhenPacketSizeMismatch": true
    },
    {
      "Name": "another_example",
      "DataPins": [ "P002" ],
      "IdPins": [ "P004" ],
      "ValidPins": [ "P008" ],
      "ValidValues": [ "0" ],
      "PacketSize": 24,
      "ErrorWhenPacketSizeMismatch": false
    },
    {
      "Name": "this_one_throws_when_packetsize_mismatches",
      "DataPins": [ "P002" ],
      "IdPins": [ "P004" ],
      "ValidPins": [ "P008" ],
      "ValidValues": [ "0" ],
      "PacketSize": 24,
      "ErrorWhenPacketSizeMismatch": true 
    },
    {
      "Name": "real-ish_looking_example",
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
      "Name": "redefining_pin_names_in_multiple_fields",
      "DataPins": [ "P002", "P004", "P006", "P008", "P010", "P012", "P014" ],
      "IdPins": [ "P002", "P004", "P016", "P018" ],
      "ValidPins": [ "P020" ],
      "ValidValues": [ "0" ],
      "PacketSize": 7,
      "ErrorWhenPacketSizeMismatch": false
    },
  ]
}
```

Field details:
- _**Packet fields**_: 
  - _**Name**_:  A string value that is mapped to your config's contents. You will provide this value to APIs within <span style="color:Violet;  font-weight: bold">DecoderBuilder</span> static class in order to build a decoder that adheres to your config's parameters
  - _**DataPins**_: A list of strings containing the names of pins whose bits will be used to build packets.
  - _**IdPins**_: A list of strings containing the names of pins that will represent the id of the vector. This is used to sort vector data into the appropriate packet depending on the Decoder being used.
  - _**ValidPins**_: A list of strings containing the names of pins that whose values will determine whether a given cycle will be used for packet creation.
  - _**ValidValues**_: A list of strings containing a single bit. Each bit corresponds to one of your defined valid pins. The incoming bits from the valid pins must match the values defined here in order for the cycle to be considered valid. The number of bits defined in this list must be the same length as the number of valid pins defined.
  - _**PacketSize**_: A integer value that defines the number of bits each packet must contain in order to be considered valid.
  - _**ErrorWhenPacketSizeMismatch**_: A bool value that determines if the decoder should error out if any packets containing valid data do not match to the packet size parameter.<span style="font-weight: bold"> Note:</span> If this parameter is <span style="color:red; font-weight: bold">false</span>, the incomplete packets will be available for querying by the 

<p><span style="font-weight: bold">Note:</span> If "<span style="color:lightblue; font-weight: bold">ErrorWhenPacketSizeMismatch</span>" is <span style="color:red; font-weight: bold">false</span> the incomplete packets are added to the list.</p>

# APIs summary
## <span style="color:Violet;  font-weight: bold">DecoderBuilder</span>
A static class containing methods used for assembling IPacketDecoders
### <span style="color:orange;  font-weight: bold">.BuildLinearDecoder()
Builds some object that contains the logic defined by ILinearDecoder. Uses the name of a configuration defined in your ALEPH file.
### <span style="color:orange;  font-weight: bold">.BuildWaveDecoder()</span>
Builds some object that contains the logic defined by IWaveDecoder. Uses the name of a configuration defined in your ALEPH file.
## <span style="color:yellowgreen;  font-weight: bold">ILinearDecoder</span>
Some object that's used for querying for packets that are assembled in a "Linear" fashion. This decoder will assemble packets by concatenating vectors that contain the same id in the order of their cycle. <span style="font-weight:bold; color: lightblue">Note:</span> For any APIs that return the "order" of the packets, the ordering of packets is determined by the cycle of the first vector used to assemble the given packet.
### <span style="color:orange;  font-weight: bold">.GetPacketsForId()</span>
Queries for all packets that match to the given id.
### <span style="color:orange;  font-weight: bold">.GetAllPackets()</span>
Returns a mapping of all id -> all packets that match to each id. 
### <span style="color:orange;  font-weight: bold">.GetAllPacketsWithOrder()</span>
Returns a mapping of id -> all packets that match to the id and the order that the packet arrived.
### <span style="color:orange;  font-weight: bold">.GetLayeredPacketsForIds()</span>
Given a list of id values, return a list of concatenated strings that are assembled by:  
1. Iterating through user provided list of id values
2. Iterating through each packet belonging to a id value
3. Concatenate the nth packet that belongs to a the id value to the nth string in the returning list.

<span style="font-weight:bold; color:lightblue">Note:</span> This API will concatenate complete AND incomplete packets.
## <span style="color:yellowgreen;  font-weight: bold">IWaveDecoder</span>
Some object that's used for querying for packets that are assembled in a "Wave" fashion. Specifically requested by array team. This decoder assembles packets by utilizing a stack and sequence values provided by the user. More details on this below...
### <span style="color:orange;  font-weight: bold">.GetWaveSequencedPackets()</span>
Creates packets by sequencing vector data using the "Wave" algorithm. Packets are assembled on a stack using vectors that follow a sequence of id values. The algorithm iterates over all vectors in the order of their cycle number. During a packet's assembly, if the id pins of the next incoming vector breaks the sequence defined by the user, the incoming vector is expected to be the head of a new packet, and pushed onto the stack. Packets that complete the sequence are popped from the stack. Packet assembly continues using the most recent uncompleted packet from the top of the stack.


# <span style="color:yellowgreen;  font-weight: bold">ILinearDecoder</span> Examples
## Example 1
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
To begin the processing of fail data into packets using obtained from CTV instructions...
```c
var myFuncTest = Prime.Base.ServiceStore<IFunctionalService>.Service.CreateCaptureCtvPerPinTest(/* params here */);
myFuncTest.Execute();
var ctvData = myFuncTest.GetCtvData();
ILinearDecoder myDecoder = DecoderBuilder.BuildLinearDecoder("example_1");
myDecoder.ProcessCtvData(ctvData);
```

...if you want to use per cycle fail data instead...
```c
var myFuncTest = Prime.Base.ServiceStore<IFunctionalService>.Service.CreateCaptureFailureTest(/* params here */);
myFuncTest.Execute();
var fails = myFuncTest.GetFailData();
ILinearDecoder myDecoder = DecoderBuilder.BuildLinearDecoder("example_1");
myDecoder.ProcessFailureData(fails);
```

Regardless of the type of failure data used, as long as you executed a functional test on the same pattern/plist, either one of the above examples should result in the same data that will be described in the following table (same for all of the following examples):

<table>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Vector Cycle</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P002</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P004</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P006</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P008</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P010</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;4</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;6</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;8</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;9</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
 </tr>
</table>

<p>
The ILinearDecoder is now going to process the vectors into the following table by using your defined configuration. Pin P010 is going to describe the Id of each cycle, while P008 determines if the given cycle is valid. The rest of the pins will contain data that will be used for packet assembly.
</p>

<table>
 <tr>
   <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Vector Cycle</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Id<br>P010</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Is Valid?<br/>P008</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P002</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P004</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P006</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: orange">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;4</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: Lime">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: red">&nbsp;&#xd7;</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;6</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Aqua">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: red">&nbsp;&#xd7;</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;8</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: red">&nbsp;&#xd7;</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; text-decoration: line-through">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;9</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: green">&nbsp;&#x2713;</td>
  <td style="text-align: center; vertical-align: middle; color: Fuchsia">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle; color: Fuchsia">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle; color: Fuchsia">&nbsp;0</td>
 </tr>
</table>

As a result, the following packets are generated:

<p>Packet with ID = 1: <span style="color:orange">000</span><span style="color:Lime">110</span><span style="color:Fuchsia">100</span></p>
<p>Packet with ID = 0: <span style="color:Aqua">000</span> <span style="color:red;font-weight:bold;">(incomplete)</span> <sup>Note: If ErrorWhenPacketSizeMismatch is set to true, PacketDecoder will fail by exception.</sup></p>
<p>Invalid vectors = <span style="text-decoration: line-through">000 000 001</span></p>

Here are the values that are expected to be returned when calling the following APIs for the above data...

<span style="color:orange">.GetPacketsForId()</span>
```c
// Possible string Inputs for GetPacketsForId() are "0" and "1".
var packetsForIdZero = myDecoder.GetPacket("0");
var packetsForIdOne = myDecoder.GetPacket("1");
```
```c
// what the decoder assigns to each variable...
var packetsForIdZero = new List<string>() { "000" };
var packetsForIdOne = new List<string>() { "000110100" };
```

<span style="color:orange">.GetAllPackets();</span>
```c
// C# object returned by GetPacket() Api.
new Dictionary<string, List<string>>() 
  {
    { "0", new List<string>() { "000" } },
    { "1", new List<string>() { "000110100" } }
  }
```

<span style="color:orange">.GetAllPacketsWithOrder();</span>
```c
// C# object returned by GetPacket() Api.
new Dictionary<string, Dictionary<uint, string>>() 
  {
    // packet "000" arrived second, therefore, assigned uint value 1
    { "0", new Dictionary<uint, string>() { 1, "000" } },
    // packet "000110100" arrived first, therefore, assigned uint value 0    
    { "1", new Dictionary<uint, string>() { 0, "000110100" } }
  }
```


<span style="color:orange">.GetLayeredPacketsForIds()</span>
```c
// Possible Inputs object for GetValidSequenceIds(inputValue).
var listOfKeys0 = new List<string>() { "0" };
var listOfKeys1 = new List<string>() { "1" };
var listOfAll   = new List<string>() { "0", "1" };
```
```c
//C# Get packets.
var listOfPackets0   = myDecoder.GetLayeredPacketsForIds(listOfKeys1)
var listOfPackets1   = myDecoder.GetLayeredPacketsForIds(listOfKeys2)
var listOfAllPackets = myDecoder.GetLayeredPacketsForIds(listOfAll)
```
```c
//C# object returned by GetValidSequenceIds(inputValue) Api.
listOfPackets0   = new List<string>() { "000" };
listOfPackets1   = new List<string>() { "000110100" };
listOfAllPackets = new List<string>() { "000110100000" };
```

<span style="color:orange">.GetInvalidPackets()</span>
```c
// C# object returned by GetInvalidPackets().
var listInvalidPackets = new List<string>() { "000000001" };
```
</br>

## Example 2
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
Code for processing this data...
```c
var myDecoder = DecoderBuilder.BuildLinearDecoder("example_2");
myDecoder.ProcessCtvData(ctvData);
```
The `ctvData` used in the example code above is visualized in the following table...
<table>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;Vector Cycle</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P002</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P004</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P006</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P008</td>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;P010</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;3</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;4</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;5</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;6</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;7</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;8</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;9</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;10</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;11</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;12</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;13</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
 <tr>
  <td style="text-align: center; vertical-align: middle; font-weight: bold">&nbsp;14</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;1</td>
  <td style="text-align: center; vertical-align: middle">&nbsp;0</td>
 </tr>
</table>

<p>
The ILinearDecoder is now going to process the vectors into the following table by using your defined configuration. Pins P004 and P006 are going to describe the Id of each cycle, while P008 and P010 determine if the given cycle is valid. The rest of the pins will contain data that will be used for packet assembly.
</p>

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

Here are the values that are returned with each API call.</br>

<span style="color:orange">.GetAllPackets()</span>
```c
// C# object returned by GetPacket Api.
new Dictionary<string, List<string>>() 
  {
    { "01", new List<string>() { "001", "001", "001" } },
    { "10", new List<string>() { "010", "010", "010" } },
    { "11", new List<string>() { "011", "011", "011" } },
  }
```
<span style="color:orange">.GetLayeredPacketsForIds()</span>
```c
// Get packets.
var listOfPackets0 = decoder.GetLayeredPacketsForIds("00", "01");
var listOfPackets1 = decoder.GetLayeredPacketsForIds("10", "11");
var listOfPackets2 = decoder.GetLayeredPacketsForIds("00", "01", "10");
var listOfPackets3 = decoder.GetLayeredPacketsForIds("01", "11", "10");
```
```c
//C# object returned by .GetLayeredPacketsForIds() Api.
listOfPackets0 = new List<string>() { "001", "001", "001" };                   
listOfPackets1 = new List<string>() { "010011", "010011", "010011" };           
listOfPackets2 = new List<string>() { "001010", "001010", "001010" };           
listOfPackets3 = new List<string>() { "001011010", "001011010", "001011010" };  
```
<span style="color:orange">.GetPackets()</span>
```c
//Possible string Inputs for GetPackets() are "01", "10" and "11".
var listOfPackets0 = decoder.GetPackets();
var listOfPackets1 = decoder.GetPackets();
var listOfPackets2 = decoder.GetPackets();
```
```c
// C# objects returned by each key.
var listOfPackets0 = new List<string>() { "001", "001", "001" };
var listOfPackets1 = new List<string>() { "010", "010", "010" };
var listOfPacktes2 = new List<string>() { "011", "011", "011" };
```
<span style="color:orange">.GetInvalidPackets()</span>
```c
// C# object returned by GetInvalidPackets().
var listInvalidPackets = new List<string>() { "011", "001", "001" };
```

# <span style="color:yellowgreen;  font-weight: bold">IWaveDecoder</span> Examples
## Example 1
Here's the configuration we're going to use for the following example...
```json
{
  "Name": "example_1", 
  "DataPins": [
      "pin_02",
      "pin_03",
      "pin_04"
  ],
  "IdPins": [ "pin_00", "pin_01" ],
  "ValidPins": [ "pin_05", "pin_06" ],
  "ValidValues": [ "1", "0" ],
  "PacketSize": 6,
  "ErrorWhenPacketSizeMismatch": true
}
```
To summon the following configuration in our code using the IWaveDecoder, we can use the following...
```c
var myDecoder = DecoderBuilder.BuildWaveDecoder("example_1");
```
Same as the ILinearDecoder, this will make it so that our decoder will adhere to the parameters defined in configuration "example_1". To start, we will give the decoder vector data by providing CTV fail data from our functional test...
```c
var myFuncTest = Prime.Base.ServiceStore<IFunctionalService>.Service.CreateCaptureCtvPerPinTest(/* params here */);
myFuncTest.Execute();
var ctvData = myFuncTest.GetCtvData();
var mySequence = new string[] { "11", "01", "10" }
ILinearDecoder myDecoder = DecoderBuilder.BuildLinearDecoder("example_1", mySequence);
myDecoder.ProcessCtvData(ctvData);
```
...if you want to use per cycle fail data instead...
```c
var myFuncTest = Prime.Base.ServiceStore<IFunctionalService>.Service.CreateCaptureFailureTest(/* params here */);
myFuncTest.Execute();
var fails = myFuncTest.GetFailData();
var mySequence = new string[] { "11", "01", "10" }
ILinearDecoder myDecoder = DecoderBuilder.BuildLinearDecoder("example_1", mySequence);
myDecoder.ProcessFailureData(fails);
```
This setup is very similar to the ILinearDecoder's. The only difference is the additional parameter required for sequencing the incoming vector data. The vector data fed into the decoder will be visualized in the table below...

| **Vector** | **pin_00** | **pin_01** | **pin_02** | **pin_03** | **pin_04** | **pin_05** | **pin_06** |
|------------|------------|------------|------------|------------|------------|------------|------------|
| 1          | 1          | 1          | 1          | 0          | 1          | 1          | 0          |
| 2          | 1          | 1          | 0          | 1          | 0          | 1          | 0          |
| 3          |            |            |            |            |            | 0          | 0          |
| 4          | 0          | 1          | 1          | 1          | 1          | 1          | 0          |
| 5          | 1          | 1          | 0          | 0          | 1          | 1          | 0          |
| 6          | 0          | 1          | 1          | 0          | 0          | 1          | 0          |
| 7          | 1          | 0          | 0          | 1          | 0          | 1          | 0          |
| 8          | 1          | 0          | 1          | 1          | 1          | 1          | 0          |
| 9          | 0          | 1          | 1          | 1          | 1          | 1          | 0          |
| 10         |            |            |            |            |            | 0          | 1          |
| 11         | 1          | 0          | 0          | 1          | 0          | 1          | 0          |
| 12         | 1          | 1          | 0          | 0          | 1          | 1          | 0          |
| 13         | 1          | 1          | 1          | 1          | 0          | 1          | 0          |
| 14         | 0          | 1          | 1          | 0          | 1          | 1          | 0          |
| 15         |            |            |            |            |            | 1          | 1          |
| 16         | 1          | 0          | 0          | 0          | 1          | 1          | 0          |
| 17         | 0          | 1          | 1          | 1          | 1          | 1          | 0          |
| 18         | 1          | 0          | 0          | 1          | 1          | 1          | 0          |

Note how some of the fields for the data pins are empty. If pins pin_05 and pin_06 did not match to the user defined "valid values" for a given vector, the cycle is ignored and its data will not be appended to any packet. The following table will describe how the vectors are interpreted by the wave decoder.


**Legend**:</br>
⚠️ - Start of new packet sequence</br>
 ❗  - Most recent packet has just recieved the final sequence value</br>
🟢 - A packet (different color for each unique packet)

**REMEMBER**: Our code example provided the sequence values [11, 01, 10]. A packet is completed when the most recent packet on the stack has recieved three cycles that progressed through the binary sequence values in the order provided by the user. So, a packet must recieve three cycles where the first cycle has a sequence value of 11, the second cycle's sequence value is 01, and the final cycle's sequence value is 10..</br>

| **Vector** | **Valid Cycle** | **Current Packet** | **Stack** | **Sequence Value** | **pin_02** | **pin_03** | **pin_04**
|------------|-----------|--------------|-----------|--------------------|------------|------------|-----------
| 1          |           | 🟢⚠️       | 🟢⚠️       | 11                 | 1          | 0          | 1           
| 2          |           | 🔴⚠️       | 🟢🔴⚠️     | 11                 | 0          | 1          | 0           
| 3          |   ❌      |              |           |                    |            |            |             
| 4          |           | 🔴         | 🟢🔴       |  01                 | 1          | 1          | 1           
| 5          |           | 🔵⚠️       | 🟢🔴🔵⚠️    | 11                 | 0          | 0          | 1            
| 6          |           | 🔵         | 🟢🔴🔵     | 01                 | 1          | 0          | 0           
| 7          |           | 🔵❗        | 🟢🔴🔵❗     | 10                 | 0          | 1          | 0           
| 8          |           | 🔴❗        | 🟢🔴❗       | 01                 | 1          | 1          | 1           
| 9          |           | 🟢         | 🟢         | 01                 | 1          | 1          | 1           
| 10         |   ❌     |               |            |                    |            |            |             
| 11         |           | 🟢❗        | 🟢❗         | 10                 | 0          | 1          | 0           
| 12         |           | 🟣⚠️       | 🟣⚠️        | 11                 | 0          | 0          | 1           
| 13         |           | 🟠⚠️       | 🟣🟠⚠️      | 11                 | 1          | 1          | 0           
| 14         |           | 🟠         | 🟣🟠       | 01                 | 1          | 0          | 1           
| 15         |   ❌     |               |           |                     |            |            |             
| 16         |           | 🟠❗        | 🟣🟠❗      | 10                 | 0          | 0          | 1           
| 17         |           | 🟣         | 🟣        | 01                 | 1          | 1          | 1           
| 18         |           | 🟣❗        | 🟣❗       | 10                 | 0          | 1          | 1           

Here are the final values for each packet when <span style="color:orange">.GetWaveSequencedPackets()</span> is called...

| Packet #   | Packet Data |
|------------|-------------|
| Packet 1 🔵| 001 100 010 |
| Packet 2 🔴| 010 111 111 |                               
| Packet 3 🟢| 101 111 010 |
| Packet 4 🟠| 110 101 001 |
| Packet 5 🟣| 001 111 011 |

```c
List<string> packets = myDecoder.GetWaveSequencedPackets();
string packet1 = packets[0]; // packet1 = "001100010"
string packet2 = packets[1]; // packet2 = "010111111"
string packet3 = packets[2]; // packet3 = "101111010"
string packet4 = packets[3]; // packet4 = "110101001"
string packet5 = packets[4]; // packet5 = "001111011"
```

# Version tracking

| **Date**   | **Prime release** | **Author**                    | **Comments** |
| ---------- | -----------       | -------------                 | ------------ |
| Mar, 2021  | 5.00.00           | Didier Armando Jiménez Retana | Add PacketDecoder.|
| Jul, 2021  | 6.00.00           | Didier Armando Jiménez Retana | Enable the PacketDecoder to build packets from failure.|
| May, 2022 | 10.00.00           | Andrea Gomez Montero          | Changing the PacketConfigurations .json file format.|
| Jun, 2022 | 11.00.00           | Didier Armando Jiménez Retana | Add GetPacketsWithReceivedSequenceId.|
| Jan, 2023 | 12.00.00           | Caio Fernandes                | Updating API names. Adding wave decoder APIs. Refactoring. Making documentation easier to understand.