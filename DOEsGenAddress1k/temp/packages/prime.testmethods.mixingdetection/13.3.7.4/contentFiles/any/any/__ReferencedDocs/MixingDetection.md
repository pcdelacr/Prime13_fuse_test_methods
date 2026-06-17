[[_TOC_]]

## REP for Mixing Detection

This **REP** is intended to describe Mixing Detection Prime TestMethod

## Introduction 

Mixing Detection is a test method used to check and detect component mixing by comparing set of DFF data set with specified UserVar value sets. By using DFF data, mixing can be detected at a per unit level. 

A Chip-Set device’s DLCP (set up at a Wafer Sort operation) and a CPU device’s DLCP (set up at Wafer Sort operation) are paired together, properly, in package assembly. This test class, designed to run in a post-assembly operation (e.g., Class Test, Raw-Class, PBIC, etc) will ensure proper (CS, CPU) allowable die pairings have taken place.

The “Compare” value mapped to the DFF set will be used to check against the UserVar trimmed value. If these 2 values are matched and all tokens and values of DFF data set are matched, then it indicates no mixing. Otherwise, illegal mixing will be triggered.

## User interface parameter 

The table below lists and describes the user interface parameters supported by the Mixing Detection test method

| **Parameter Name** | **Required?** | **Type** | **Default** | **Description** | 
| ------------------ | :-------------: | -------- | ----------- | --------------- |
| ConfigurationFilePath      | YES        | STRING   |  | Path to the json configuration file. |
| LogLevel          |        NO           | STRING |    | Flag to indicates LogLevel. | 

## Test Method Exit Ports

The Mixing Detection test method supports the following exit ports:

| **Exit Condition** | **Detailed Description** | **Port Number** |
| :----------------: | ------------------------ | :-------------: |
| ***Alarm*** | Any hardware alarm | **-2** |
| ***Error*** | Any test class error, software errors, or software execution fails | **-1** |
| ***Fail***  | Malformed DFF token value or failed getting the value  | **0**  |
| ***Pass***  | No mixing detected | **1**  |
| ***Fail***  | Mixing detected | **2**  |


## Implementation 


### Verify 


* Mixing Detection will check for all required params. Refer to the list of parameters required in the [User Interface Parameters](#user-interface-parameter). If all required params are meet, the instance will validate that the configuration file meet all the requirements defined in the schema. If the TM instance detects any missing parameters or an error in the configuration file is found, the TM will fail.

* Once the configuration file has been validated, the instance will deserialize each property of the configuration file into its own object. If any of the exeption conditions are met the instance will fail.



### Execute 

* Firstly, for each DFF token contained in the DFFTokenList, its valued will be updated.

* Next, the TM will start comparing the information contained in Compare list.
    * If Compare list is empty, the TM will pass, otherwise 
    * the TM will will search for those Compare objects which its value match with the UserVar value.
    * If DuplicatedCompareValues does not allow duplicated Compare values, and more than one Compare objects are found, th TM will fails. 
    * Next, for each Compare object, its DFF data set will be validated,meaning, for each DDF, if all the token's value match with the value specified the TM will pass, otherwise, continues with the next Compare object. If there are no more Compare objets, the TM will fails. 



![image.png](./attachments/MixingDetection_ExecutionFlow.png)



## Configuration file 

The file below is a example of a configuration file used by Mixing Detection. This is a JSON file and its extension must be <>.mdet.json/

```json
{
  "UserVar": {
      "Name": "SCVars.SC_DEVICE",
      "CharPos": "2",
      "Count": "1"
  },
  "DuplicatedCompareValues": false,
  "DFFTokenList": [ "U2.U1:SORT.TOKA" ],
  "Compares": [
      {
          "Value": "A",
          "CatchAll": true,
          "DFF": [
              {
                  "Value": "2",
                  "Token": "U2.U1:SORT.TOKA"
              }
          ]
      },
      {
          "Value": "B",
          "DFF": [
              {
                  "Value": "2",
                  "Token": "U2.U1:SORT.TOKA"
              }
          ]
      },
  ]
}

```


* **UserVar**: Contains the UserVar name that is needed for a comparison.

    * **Name**: The UserVar name must be specified in the format of ```<Uservars group>.<UserVar Name>```. 

    * **CharPos**: This attribute indicates the position of the start character needed to be indexed from with the UserVar value string. It accepts an integer or an “ALL” string. If a positive integer is specified, the index position will be from left to right. If a negative integer is specified, the index position is from right to left: ```CharPos:"1”``` indicates the 1st character, while ```CharPos:"-1”``` indicates the last character. For example, if UserVar value equal to “4EHGAV”, ```CharPos:"2”``` would indicate “E” as the start character and ```CharPos:"-3”``` would indicate “G” as the start character. 

    * **Count**: The count attribute indicates the character count from the start character. Using the previous example, ```CharPos:"2”``` and  ```Count:"3```, the UserVar sub-string value would be “EHG”. If ```Count:"0”```, the UserVar sub-string value would be empty. If any char_pos or count or both is set to “ALL”, the UserVar sub-string value would be the full UserVar value (by default).


* **DuplicatedCompareValues**: By default, this value should be set to ```false``` to indicate that the Compare.Value element value needs to be unique and no duplicate are permitted. If set ```true```, duplication of Compare.Value values are permitted.

* **DFFTokenList**: Specifies the DFF token list. Each token needs to be exactly in the format of ```<DieId>:<DFF Optype>:<Token name>```.

  In case you need to use current optype, the DFF token must follow the next format ```<DieId>:CURRENT:<Token Name>```.


* **Compares**: This an array of objects which consists of Compare Value attribute and its DFF data sets. 
    * **Value**: The value that will be used to compare against the UserVar sub-string value.
    
    * **CatchAll**: By default, this value is false. If set ```true``` all comparation of the tokens and its values included in DFF data set will be ignored. It is an optional property. 

    * **DFF**: To specify the DFF token name and its value. The DFF token specified here must be one of the DFF token list specified in **DFFTokenList**. It is case sensitive, so must be the exactly the same as specified in **DFFTokenList**. The token value also case sensitive so must be exactly the same as what expected from the DFF/UBE data. Must specify all the DFF tokens which are listed the DFFTokenList, no more or less. DFF values expected can ONLY contain only upper-case alphabetic characters (A-Z) and integers (0-9). 

## DFF Tokens with WFR

For SORT, you’ll need to use the die id i.e. U2.U1 or U4 etc instead of using WFR. 

As the WFR is also part of the same ULT that’s tied to the same Die Id. So to pull data from SORT, you’ll need to use the associate die ID to it.


```txt
UNIT,000000000000Z5X00100000
PKG,PBIC_S2,SOT_T1=0.74,SOT_T0=-10,BINN=4002,
PKG,PBIC_DAB,SOT_T1=0.75,SOT_T0=-11,BINN=4003,
U1,PBIC_S2,SOT_T1=0.76,SOT_T0=-12,BINN=4004,
U1,PBIC_DAB,SOT_T1=0.77,SOT_T0=-13,BINN=4005,
U2,PBIC_S2,SOT_T1=0.78,SOT_T0=-14,BINN=4006,
U2,PBIC_DAB,SOT_T1=0.79,SOT_T0=-15,BINN=4007,
Y5106803_330_-10_+00:
U2.U1,PBIC_S2,TOKA=0,TOKB=0,TOKC=98,TOKD=1.444,TOKE=100,TOKF=0|0|0,TOKHEX=65686577,
U2.U1,PBIC_DAB,TOKA=1,TOKB=1,TOKC=98,TOKD=1.544,TOKE=101,TOKF=0,TOKHEX=75686577,
WFR,SORT,TOKA=2,TOKB=2,TOKC=98,TOKD=1.644,TOKE=102,TOKF=0|0|1,TOKHEX=85686577,
Y5106803_330_-10_+02:
U4,PBIC_S2,UBEID=U1.U3,TOKA=0,TOKB=0,TOKC=98,TOKD=1.444,TOKE=100,TOKF=0|0|0,TOKHEX=65686577,
U4,PBIC_DAB,UBEID=U1.U3,TOKA=0,TOKB=0,TOKC=98,TOKD=1.444,TOKE=100,TOKF=0|0|0,TOKHEX=65686577,
WFR,SORT,TOKA=0,TOKB=0,TOKC=98,TOKD=1.444,TOKE=100,TOKF=0|0|0,TOKHEX=65686577,
```

For example, if you need WFR:SORT:TOKA from Y5106803_330_-10_+00, you'll need to call it using U2.U1:SORT:TOKA.


## Acronyms

Definition of acronyms used in this document:

  - **DFF**: Data Feed Forward
  - **UBE**: Unit Based Execution
  - **JSON**: JavaScript Object Notation


## Version tracking

| **Date**                  | **Version** | **Author**        | **Comments**    |
| ------------------------- | ----------- | ----------------- | --------------- |
| Aug 17<sup>st</sup>, 2022 | 1.0.0       | Sebastian Mora      | Initial version |