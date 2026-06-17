[[_TOC_]]

# Prerequisites



<p style="font-size: 20px;"><span style="color: #cf6679;">This is not a finished documentation, this wiki is currently under development</span></p>





<p style="font-size: 20px;"><span style="color: #cf6679;">This Service is using Aleph initialization. Please refer to the Init - Aleph documentation</span></p>

[Link to Aleph Documentation](https://dev.azure.com/mit-us/PrimeWiki/_wiki/wikis/PrimeWiki.wiki/28879/Special-ENV-Variables-in-Prime)

# DtsProcessingService
The DtsProcessingService is a C# service, provides de capability to process a CTV string into temperature values, log them or store them.

#Configuration File
<p>Aleph initialization only recognize files with the name "<span style="color:Lime;  font-weight: bold">DtsProcessingService.json</span>".</p>

As follows an example of the content of this file:
```json
{
  "Configurations": [
    {
      "Name": "SimpleDtsConfiguration",
      "Pins": [
        {
          "NumberOfSensors": 2,
          "PinName": "xxHPCC_DPIN_Dig_slcA_AA0",
          "SensorRegisterSize": 8
          "Slope": 0.5,
          "Offset": -64,
        }
      ],
      "DatalogValues": true
    },
    {
      "Name": "MultiPinDtsConfiguration",
      "Pins": [
        {
          "NumberOfSensors": 2,
          "PinName": "xxHPCC_DPIN_Dig_slcA_AA0",
          "SensorRegisterSize": 8
          "Slope": 0.5,
          "Offset": -64,
        },
        {
          "NumberOfSensors": 4,
          "PinName": "xxHPCC_DPIN_Dig_slcA_AA1",
          "SensorRegisterSize": 4
          "Slope": 0.5,
          "Offset": -64,
        }
      ],
      "DatalogValues": true
    },
    {
      "Name": "DtsConfigurationLimitCheck",
      "Pins": [
        {
          "SensorsList": [ "S1", "S2" ],
          "PinName": "xxHPCC_DPIN_Dig_slcB_AA0",
          "SensorRegisterSize": 12,
          "SetPoint": 100,
          "UpperTolerance": 7,
          "LowerTolerance": 7
          "Slope": 0.5,
          "Offset": -64,
        }
      ],
      "DatalogValues": false
    },
    {
      "Name": "DtsConfigurationFilterSensors",
      "DatalogValues": true,
      "CompressedDatalog": true,
      "Pins": [
        {
          "SensorsList": [ "S0", "S1", "S2", "S3" ],
          "PinName": "xxHPCC_DPIN_Dig_slcB_AA0",
          "SensorRegisterSize": 3,
          "SensorIndexesToLog": "0|2-3"
          "Slope": 0.5,
          "Offset": -64,
        }
      ]
    },
    {
      "Name": "MultiPinDtsConfigurationFilterSensors",
      "DatalogValues": true,
      "CompressedDatalog": true,
      "Pins": [
        {
          "SensorsList": [ "S0", "S1", "S3", "S5" ],
          "PinName": "xxHPCC_DPIN_Dig_slcB_AA0",
          "SensorRegisterSize": 3,
          "SensorIndexesToLog": "0|2",
          "SetPoint": 0,
          "UpperTolerance": 7,
          "LowerTolerance": 65
          "Slope": 0.5,
          "Offset": -64,
        },
        {
          "SensorsList": [ "S2", "S4" ],
          "PinName": "xxHPCC_DPIN_Dig_slcA_A0",
          "SensorRegisterSize": 4,
          "SensorIndexesToLog": "0"
          "Slope": 0.5,
          "Offset": -64,
        }
      ]
    },
    {
      "Name": "DtsConfigurationMaxSensor",
      "Pins": [
        {
          "SensorsList": [ "S1", "S2" ],
          "PinName": "xxHPCC_DPIN_Dig_slcA_AA0",
          "SensorRegisterSize": 8
          "Slope": 0.5,
          "Offset": -64,
        }
      ],
      "DatalogValues": true,
      "PrintMaxSensor": true
    }
  ]
}
```

 **Field details:**
- _**Configurations**_: 
  - _**ExportToSharedStorage**_: boolean to set if saving data to shared storage is required.
  - _**DatalogValues**_: boolean to set if printing to Ituff is required.
  - _**CompressedDatalog**_: boolean to set if printing to Ituff is required in compress mode.
  - _**Pins**_: list of Pin Objects. Fields described below.
    - _**PinName**_: name of the pin with CTV data to process.
    - _**SensorRegisterSize**_: number of bits needed to represent the temperature from one sensor.
    - _**NumberOfSensors**_: expected number of sensors per CTV chunk.
    - _**SensorsList**_(optional): List with the name of the sensors. Used to get the expected number of sensors and for logging purposes.
    - _**Slope**_: Slope for temperature calculation. If not value is provided, the default value (Slope = 0.5) is used.
    - _**Offset**_: Offset for temperature calculation. If not value is provided, the default value (Offset = -64) is used. 
    - _**PrintMaxSensor**_(optional): boolean to set if printing just the Maximu value across all sensors. Default is false.
    - _**SensorIndexesToLog**_(optional): indexes to filter sensor data (separaded by "|"). Example to log only sensor 0, 2 and 3 => "0|2-3"
    - _**CtvCaptureFromBits**_(optional): bits to take into account from the CTV input string (separaded by "|"). Example to take into account bits 6, 7, 8 and 16 => "6-8|16". If the parameter is not defined the whole input string will be used.
    - _**SetPoint**_(optional): Temperature value to be used as a reference for limit calculation.
    - _**UpperTolerance**_(optional): offset to be used for limit calculation. HighLimit = setPointTemperature + upperTolerance.
    - _**LowerTolerance**_(optional): offset to be used for limit calculation. LowLimit = setPointTemperature - lowerTolerance.

*Notes*
1. If ExportToSharedStorage is set to TRUE a new set of rows in the shared storage will be created with the keys:
```csharp
{testInstanceName}_DTS_{pinConfiguration.PinName}_{dtsIndex}
```
the stored value will be a "|" separated string with the processed temperatures.
The same information can be logged to the ituff if DatalogValues is set to TRUE

2. CtvCaptureFromBits example: 
```json
input string = "0100111"
CtvCaptureFromBits = "4-6"
Filtered resulting string will be "111"
```

#Internal Logic
The DTSProcessingService will divide the CTV data into chunks and each chunk will have the data for a set of measurements for each sensor.
The data of each sensor wil then be converted from binary to decimal using the equation below: 

**DTSValue = (sensorDecimalValue * slope) offset**


**Examples** <br>
Slope = 0.5<br>
Offset = -64<br>
SensorCount  = 2<br>
SensorRegisterLenght = 8 bits<br>
ChunkLenght = SensorCount * SensorRegisterLenght = 16<br>
ChunkNumber = TDO_DataLenght / chunkLenght  = 1<br>

For Pin TDO data [<span style="color:#2ECC71">**00000001**00001011</span>]<br>
<span style="color:#2ECC71">Chunk1</span>
- <span style="color:#2ECC71">**Sensor1**</span>: 00000001 , sensorDecimalValue = 128  (LSB First)   &rarr;   (128*Slope)+Offset = 0

- <span style="color:#2ECC71">Sensor2</span>: 00001011, sensorDecimalValue = 208  (LSB First)  &rarr;  (208*Slope)+Offset = 40

<br>

Slope = 0.5<br>
Offset = -64<br>
SensorCount  = 2<br>
SensorRegisterLenght = 8 bits<br>
chunkLenght = SensorCount * SensorRegisterLenght = 16<br>
ChunkNumber = TDO_DataLenght / chunkLenght  = 2<br>

For Pin TDO data [<span style="color:#A44AEB">**10100001**00000011</span><span style="color:#1DC1E9">**00001001**01001011</span>] <br>
<span style="color:#A44AEB">Chunk1</span><br>
- <span style="color:#A44AEB">**Sensor1**</span>: 01100001, sensorDecimalValue = 134 (LSB First)  &rarr;  (134*Slope)+Offset = 3<br>
- <span style="color:#A44AEB">Sensor2</span>: 00000011, sensorDecimalValue = 192 (LSB First)  &rarr;  (192*Slope)+Offset = 32<br>

<span style="color:#1DC1E9">Chunk2</span><br>
- <span style="color:#1DC1E9">**Sensor1**</span>: 00001001, sensorDecimalValue = 144 (LSB First) &rarr;   (128*Slope)+Offset = 8<br>
- <span style="color:#1DC1E9">Sensor2</span>: 01001011, sensorDecimalValue = 210 (LSB First) &rarr;  (210*Slope)+Offset = 41<br>

<br>
<br>
<br>
<br>

#How to use it with VminSearchTestMethod

- create a DtsHandler object using the DtaProcessingServie during CustomVerify extension. this method will read and set the Dts configuration.
```csharp
using Prime.Prime.DtsProcessingService;

    /// <summary>
    /// Vmin Search with basic usage of the DtsProcessing Service.
    /// </summary>
    [PrimeTestMethod]
    public class VminSearchWithDtsProcessing : PrimeVminSearchTestMethod, IVminSearchExtensions
    {
        private IDtsHandler dtsHandler;

        /// <summary>
        /// Gets or sets the configuration in case DTS processing is wanted.
        /// </summary>
        public TestMethodsParams.String DtsConfigurationName { get; set; } = string.Empty;

        /// <inheritdoc />
        public override void CustomVerify()
        {
            this.dtsHandler = Services.DtsProcessingService.CreateDtsHandler(this.DtsConfigurationName);
        }
```
- Once the DtsHandler was created you can retrive the CTV capture pin that was defined in the json configuration. For capturing CTVs create 
a "CreateCaptureCtvPerPinTest" object during the GetFunctionalTest extension and use the defined pin as capture pin, this way CTVs will be collected for the selected pin once the plist is executed.

```csharp
       public IFunctionalTest GetFunctionalTest(string patlist, string levelsTc, string timingsTc, string prePlist)
        {
            var capturePin = new List<string>() { (this.dtsHandler.GetPinName()) };
            return Prime.Services.FunctionalService.CreateCaptureCtvPerPinTest(patlist, levelsTc, timingsTc, capturePin, prePlist);
        }
```
- Inside the ProcessPlistResults Extension you will need to call the ProcessDtsMeasurementsFromCtvData method from DtsHandler, usnig the capture CTV string as input.
This will do the CTV conversion into temperature values, and according to the configuration set in the json file it will log to Ituff or export it to the shared Storage
```csharp
       public BitArray ProcessPlistResults(bool plistExecuteResult, IFunctionalTest functionalTest)
        {
            var bitResult = new BitArray(this.VoltageTargets.ToList().Count, true);

            if ((functionalTest is ICaptureCtvPerPinTest captureCtvPerPin))
            {
                this.dtsHandler.ProcessDtsMeasurementsFromCtvData(
                        captureCtvPerPin.GetCtvData(this.dtsHandler.GetPinName()));
                return bitResult;
            }

        }
```
- Before finishing the instance execution the DtsHandler should be reset to clear the internal service structures, preventing calculations made from the current unit to interfer with the next one.
```csharp
       this.dtsHandler.Reset();
```
# Version tracking

| **Date**   | **Prime release** | **Author**    | **Comments** |
| ---------- | ----------- | ------------- | ------------ |
| Dic, 2021  | 7.02.00     | Ariel Mata Vega | DtsProcessing Basic features.|
| May, 2022  | 10.00.00    | Maria Rojas Rojas | Removal of Slope and Offset from DtsProcessing configurationFile.|
| May, 2022  | 10.00.00    | Ariel Mata Vega | Reseting hander.|
| May, 2022  | 11.00.00    | Andrea Gomez | Adding support for multiple pins.|
| Sep, 2022  | 12.00.00    | Johnny Mata | Updating missing fields on configuration description.|
| Oct, 2022  | 12.00.00    | Johnny Mata | Adding Slope and Offset to the DtsProcessing configuration file.