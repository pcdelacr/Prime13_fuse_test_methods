**prime Test-Method Specification REP**

Revision 1.0.0

Jan 2024

[[_TOC_]]

## REP for ScreenTest

This **REP** is intended to describe the ScreenTest Prime TestMethod.

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

Prime's ScreenTest provides a means of creating and acting on data sources (UserVars, SharedStorage, DFF), 
flow controller data (e.g., BMFC, Station Controller user variables), in order to aid test programs in routing DUT specific actions. 
This test method is capable of many operations, using a simple input language in which end-users can experiment 
and customize data flows.

<h1 style="font-size: 1.1rem; color: red">Note: The flexibility ScreenTests offers has a toll in time consumption. The recommendation for users looking to improve test time is to create and use alternative solutions to the use of ScreenTest. Code that can perform direct actions without the need of parsing and process instructions from an input file will be faster.</h1>

### ScreenTests File    

Users are able to define different “screens” to be performed in the program, and all “screens” can be defined within a single file (no test class limit exists on the number). Each “screen” is composed of a sequence of “test” definitions.  

In Table 1, these “tests” can be performed using the following types of data: DFF, Global or UserVar, SharedStorage and Literal. Each “test” is considered a different mode of operation and is further detailed in the following sections.  

Table 1: ScreenTests File Format   
| START_SCREEN | *ScreenSetName* |                                              |                                                             |                                                         |                                                    |                     |                |              |                    |                    |
| :----------- | :-------------- | :------------------------------------------- | :---------------------------------------------------------- | :------------------------------------------------------ | :------------------------------------------------- | :------------------ | :------------- | :----------- | :----------------- | :----------------- |
| **Run On**   | **Test Name**   | **Data Type**                                | **Param1 Type**                                             | **Param1**                                              | **Operation**                                      | **Param2 Type**     | **Param2**     | **TmpStore** | **On True**        | **On False**       |
| EXEC         | *TestName*      | INT<br>INTEGER<br>DOUBLE<br>STRING<br>BINARY | DFF<br> GLOBAL<br> SHAREDSTORAGE<br> LITERAL<br> EXPRESSION | *see below*                                             | <br>><br><=<br>>=<br>==<br>!=<br>SET<br>APPEND<br> | same as param1 type | same as param1 | _ignored_    | *TestName or Port* | *TestName or Port* |
|              |                 |                                              | DFF                                                         | \<SSID>.\<OpType>.\<Token>                              |                                                    |                     |                |              |                    |                    |
|              |                 |                                              | GLOBAL                                                      | \<collection>.\<UserVar>                                |                                                    |                     |                |              |                    |                    |
|              |                 |                                              | SHAREDSTORAGE                                               | <L,U,I>.<D,I,S>.\<Token><br> G.<L,U,I>.<D,I,S>.\<Token> |                                                    |                     |                |              |                    |                    |
|              |                 |                                              | LITERAL                                                     | <any string, double, integer>                           |                                                    |                     |                |              |                    |                    |
|              |                 |                                              | EXPRESSION                                                  | Please refer to [ExpressionEngine] Documentation.       |                                                    |                     |                |              |                    |                    |
| END_SCREEN   |                 |                                              |                                                             |                                                         |                                                    |                     |                |              |                    |                    |

Caveats:   
1. Screen test execution will always begin with the first test listed:     
    * This ‘first test’ is the one immediately following the ‘START_SCREEN’ definition block.   
2. The execution result of the first test, dictates subsequent execution order.   
3. Subsequent tests can appear (i.e., be registered) in any order.   
4. Test ‘names’ should be alpha-numeric, and need to be case-insensitive.   
    * There is no execution order or registration order required.   
    * It is recommended to use test names comprised of [a-zA-Z0-9_] characters.   
    * Test names will be internally normalized to upper-case. For example, the testName “condition_10” and “Condition_10”, within one START_SCREEN block, will fail to verify.   
5. Data Types and Operations:   
    * String type can use ==, !=, SET and APPEND only.   
    * Binary type can use ==, !=, and SET only.    
    * Integer and Double Types can use <, >, <=, >=, ==, !=, and SET.       

Table 2: Screen Tests File Example
ScreenTests File Example
```
START_SCREEN:HWRTREMAP_BIN80
#RunON : TestName : DataType : Param Type : Param1               : Operation : Param Type : Param2              : Tmp : OnTrue : OnFalse
EXEC   : Test1    : INT      : GLOBAL     : TPI_BASE_XXX.ORIBIN  : SET       : EXPRESSION : GetCurrentSoftBin() : -   : Test2  : 2
EXEC   : Test2    : INT      : EXPRESSION : GetCurrentHardBin()  : >         : LITERAL    : 999                 : -   : Test3  : 1
EXEC   : Test3    : STRING   : GLOBAL     : SCVars.SC_TEST_COUNT : ==        : LITERAL    : "2"                 : -   : 4      : Test4
EXEC   : Test4    : STRING   : GLOBAL     : SCVars.SC_TEST_COUNT : ==        : LITERAL    : "3"                 : -   : 4      : Test5
EXEC   : Test5    : STRING   : GLOBAL     : SCVars.SC_TEST_COUNT : ==        : LITERAL    : "4"                 : -   : 4      : 1
END_SCREEN

START_SCREEN:PostSetVCCIOVmin
#RunOn : TestName : DataType : Param Type : Param1                                           : Op  : Param Type    : Param2         : Tmp : OnTrue  : OnFalse
EXEC   : test1    : STRING   : GLOBAL     : _UserVars.PRIMARY_OPTYPE                         : ==  : LITERAL       : "PBIC_DAB"     : -   : test44  : 1
EXEC   : test44   : DOUBLE   : GLOBAL     : _UserVars.uv_vccio_spec_nom                      : ==  : SHAREDSTORAGE : U.D.VMIN_VCCIO : -   : test44A : test2
EXEC   ; test44A  ; DOUBLE   ; GLOBAL     ; IP_CPU::IP_CPU_BASE::_UserVars.uv_vccio_spec_nom ; ==  ; SHAREDSTORAGE ; U.D.VMIN_VCCIO ; -   ; test44B ; test2
EXEC   ; test44B  ; DOUBLE   ; GLOBAL     ; IP_PCH::IP_PCH_BASE::_UserVars.uv_vccio_spec_nom ; ==  ; SHAREDSTORAGE ; U.D.VMIN_VCCIO ; -   ; test3   ; test2
EXEC   : test2    : DOUBLE   : GLOBAL     : _UserVars.uv_vccio_spec_nom                      : SET : SHAREDSTORAGE : U.D.VMIN_VCCIO : -   : test2A  : 2
EXEC   ; test2A   ; DOUBLE   ; GLOBAL     ; IP_CPU::IP_CPU_BASE::_UserVars.uv_vccio_spec_nom ; SET ; SHAREDSTORAGE ; U.D.VMIN_VCCIO ; -   ; test2B  ; 2
EXEC   ; test2B   ; DOUBLE   ; GLOBAL     ; IP_PCH::IP_PCH_BASE::_UserVars.uv_vccio_spec_nom ; SET ; SHAREDSTORAGE ; U.D.VMIN_VCCIO ; -   ; test3   ; 2
EXEC   : test3    : DOUBLE   : DFF        : CURRENT.PBIC_DAB.VCCIO                           : SET : SHAREDSTORAGE : U.D.VMIN_VCCIO : -   : 1       : 2
END_SCREEN

START_SCREEN:HWRTREMAP_BIN80
#RunOn : TestName : Data Type : Param Type : Param1               : Op  : Param Type : Param2              : Tmp : OnTrue : OnFalse
EXEC   : Test1    : INT       : GLOBAL     : TPI_BASE_XXX.ORIBIN  : SET : EXPRESSION : GetCurrentSoftBin() : -   : Test2  : 2
EXEC   : Test2    : INT       : EXPRESSION : GetCurrentHardBin()  : >   : LITERAL    : 999                 : -   : Test3  : 1
EXEC   : Test3    : STRING    : GLOBAL     : SCVars.SC_TEST_COUNT : ==  : LITERAL    : "2"                 : -   : 4      : Test4
EXEC   : Test4    : STRING    : GLOBAL     : SCVars.SC_TEST_COUNT : ==  : LITERAL    : "3"                 : -   : 4      : Test5
EXEC   : Test5    : STRING    : GLOBAL     : SCVars.SC_TEST_COUNT : ==  : LITERAL    : "4"                 : -   : 4      : 1
END_SCREEN
```

----  

#### DFF Mode   

The objective of the DFF is to maximize factory responsiveness to meet immediate market demand and to contain lot mixing. It is designed as an operational database to store required data for a unit (DFF DB). This project will shift the final configuration of packaged CPU (Fuse and Mark) to the last/best point in the Intel supply line, and ensures that lot mixing will be eliminated, which will be primarily achieved by maintaining a database that contains unit-level information (ULT). The database keeps track of all units with their Visual ID, EfuseID, Sspec validation, Fuse trim, Lot number and other traceable attributes.   

| START_SCREEN | _ScreenSetName_ |              |             |                             |              |                |            |           |                    |                    |
|:-------------|:----------------|--------------|-------------|-----------------------------|--------------|----------------|------------|-----------|--------------------|--------------------|
| Run On       | Test Name       | Data Type    | Param1 Type | Param1                      | Operation    | Param2 Type    | Param2     | Tmp Store | On True            | On False           |
| EXEC         | _TestName_      | \<data type> | DFF         | \<DieId>.\<OpType>.\<Token> | \<operation> | \<Param Type2> | \<Param 2> | _ignored_ | _TestName or Port_ | _TestName or Port_ |
| EXEC         | ...             | ...          | ...         | ...                         | ...          | ...            | ...        | ...       | ...                | ...                |
| END_SCREEN   |                 |              |             |                             |              |                |            |           |                    |                    |

Notes:   
   * _DieId_ can be CURRENT, PKG, WFR or a die ID of the form U1, U1.U1 ... (can be any number - U1, U2, U3, ... - and any amount of dot-separated-dielets U1.U2, U1.U1.U1,...)  
   * _OpType_ can be the operation or CURRENT
   * _Token_ can be the DFF Token or the Token.Field
   * If the DFF does not exist then an error is thrown.
   * \{String} -> \{INTEGER, DOUBLE} data conversion problems will result in an ERROR PORT, and mini-flow execution will halt immediately.   
   * \<OpType> can have a special value of “CURRENT”, referring to reading DFF data from the current operation type.   
   * DFF “write” (e.g., “SET” operations) can only occur at the current operation. Example: if the program is executing at “PBIC_S1”, any set operation with “PKG. PBIC_S2.NAME” will always write into the “PBIC_S1” operation.    
    - To avoid confusion, end-users should use “CURRENT” (e.g., “PKG.CURRENT.NAME” instead of “PKG.PBIC_S2.NAME”) for a set operation.   
    - This convention is not enforced via any verification error.

ScreenTests File, Dff example:
```
START_SCREEN:SampleScreen
#RunOn : TestName : DataType : ParamType : Param1                    : Operation : ParamType     : Param2           : Tmp : OnTrue : OnFalse
EXEC   : Test1    : STRING   : GLOBAL    : AVIDVars.UFG_AVID_Ref_Vid : >         : DFF           : PKG.PBIC_S1.VOLT : -   : Test2  : 2
EXEC   : Test2    : STRING   : DFF       : PKG.PBIC_S1.FMAX          : ==        : LITERAL       : “CS”             : -   : Test3  : 2
EXEC   : Test3    : STRING   : DFF       : PKG.PBIC_S1.FMAX          : SET       : SHAREDSTORAGE : U.S.MASTER       : -   : 1      : 2
END_SCREEN
```

----  

#### GLOBAL Mode   

The Global mode capability allows the user to create tests using data residing in user variables. The user specifies the set of user variables, along with the collection name.
The keyword USERVAR can be used for the Parameter Type instead of GLOBAL.  

| START_SCREEN | _ScreenSetName_ |              |             |                           |              |                |            |           |                    |                    |
|:-------------|:----------------|--------------|-------------|---------------------------|--------------|----------------|------------|-----------|--------------------|--------------------|
| Run On       | Test Name       | Data Type    | Param1 Type | Param1                    | Operation    | Param2 Type    | Param2     | Tmp Store | On True            | On False           |
| EXEC         | _TestName_      | \<data type> | GLOBAL      | \<collection>.\<user var> | \<operation> | \<Param Type2> | \<Param 2> | _ignored_ | _TestName or Port_ | _TestName or Port_ |
| EXEC         | ...             | ...          | ...         | ...                       | ...          | ...            | ...        | ...       | ...                | ...                |
| END_SCREEN   |                 |              |             |                           |              |                |            |           |                    |                    |

Notes:   
1. The normal screen test delimiter is a colon ":", but UserVars with IP or Module scope use :: as their delimiter. If this is detected, the TM will switch to using a semi-colon ";" as a delimiter for that line/test only.

ScreenTests File, UserVar example
```
START_SCREEN:SampleScreen
#RunOn : TestName : DataType : ParamType     : Param1                : Operation : ParamType : Param2                       : TmpStore : OnTrue : OnFalse
EXEC   : Test1    : STRING   : GLOBAL        : CTSCVars.SC_TEST_FLOW : ==        : LITERAL   : “CLASS”                      : -        : Test2  : 1
EXEC   : Test2    : STRING   : GLOBAL        : CTSCVars.SC_SSPEC     : ==        : LITERAL   : “T1”                         : -        : 3      : Test3
EXEC   : Test3    : STRING   : SHAREDSTORAGE : G.U.S.TRAY            : SET       : GLOBAL    : _UserVars.UFG_FUSE_TME_TRAY1 : -        : 1      : 2
END_SCREEN

START_SCREEN:PostSetVCCIOVmin
#RunOn : TestName : DataType : ParamType : Param1                                           : Operation : ParamType     : Param2         : Tmp : OnTrue  : OnFalse
EXEC   : test1    : STRING   : GLOBAL    : _UserVars.PRIMARY_OPTYPE                         : ==        : LITERAL       : "PBIC_DAB"     : -   : test44  : 1
EXEC   : test44   : DOUBLE   : GLOBAL    : _UserVars.uv_vccio_spec_nom                      : ==        : SHAREDSTORAGE : U.D.VMIN_VCCIO : -   : test44A : test2
EXEC   ; test44A  ; DOUBLE   ; GLOBAL    ; IP_CPU::IP_CPU_BASE::_UserVars.uv_vccio_spec_nom ; ==        ; SHAREDSTORAGE ; U.D.VMIN_VCCIO ; -   ; test44B ; test2
EXEC   ; test44B  ; DOUBLE   ; GLOBAL    ; IP_PCH::IP_PCH_BASE::_UserVars.uv_vccio_spec_nom ; ==        ; SHAREDSTORAGE ; U.D.VMIN_VCCIO ; -   ; test3   ; test2
EXEC   : test2    : DOUBLE   : GLOBAL    : _UserVars.uv_vccio_spec_nom                      : SET       : SHAREDSTORAGE : U.D.VMIN_VCCIO : -   : test2A  : 2
EXEC   ; test2A   ; DOUBLE   ; GLOBAL    ; IP_CPU::IP_CPU_BASE::_UserVars.uv_vccio_spec_nom ; SET       ; SHAREDSTORAGE ; U.D.VMIN_VCCIO ; -   ; test2B  ; 2
EXEC   ; test2B   ; DOUBLE   ; GLOBAL    ; IP_PCH::IP_PCH_BASE::_UserVars.uv_vccio_spec_nom ; SET       ; SHAREDSTORAGE ; U.D.VMIN_VCCIO ; -   ; test3   ; 2
EXEC   : test3    : DOUBLE   : DFF       : CURRENT.CURRENT.VCCIO                            : SET       : SHAREDSTORAGE : U.D.VMIN_VCCIO : -   : 1       : 2
END_SCREEN
```
----  

#### SHAREDSTORAGE Mode   

The SharedStorage mode capability allows the user to create tests using data residing in SharedStorage. The user specifies the “Scope” (e.g., "U" for Unit, "L" for Lot and "I" for IP), data type (e.g., “I” = Integer, “D” = Double, “S” = String), and a token name.<br>
A "G." be added before the scope to match the naming convention used for expressions, but it is optional.

| START_SCREEN | _ScreenSetName_ |              |               |                                  |              |                |            |           |                    |                    |
|:-------------|:----------------|--------------|---------------|----------------------------------|--------------|----------------|------------|-----------|--------------------|--------------------|
| Run On       | Test Name       | Data Type    | Param1 Type   | Param1                           | Operation    | Param2 Type    | Param2     | Tmp Store | On True            | On False           |
| EXEC         | _TestName_      | \<data type> | SHAREDSTORAGE | \<scope>.\<data type>.\<token>   | \<operation> | \<Param Type2> | \<Param 2> | _ignored_ | _TestName or Port_ | _TestName or Port_ |
| EXEC         | _TestName_      | \<data type> | SHAREDSTORAGE | G.\<scope>.\<data type>.\<token> | \<operation> | \<Param Type2> | \<Param 2> | _ignored_ | _TestName or Port_ | _TestName or Port_ |
| EXEC         | ...             | ...          | ...           | ...                              | ...          | ...            | ...        | ...       | ...                | ...                |
| END_SCREEN   |                 |              |               |                                  |              |                |            |           |                    |                    |

Notes: 
* It is possible to directly assign a primitive value from a SharedStorage to another. For example, if U.S.Token has the string value "5", it can be directly assigned to U.I.Token2 which will be stored as an integer. An error will occur if the value can't be converted.
* If a SharedStorage value does not exist for a given \<Scope>.\<Data Type>.\<Token> an error will be thrown.

ScreenTests file, SharedStorage example:
```
START_SCREEN:SampleScreen
#RunOn : TestName : DataType : ParamType     : Param1        : Operation : ParamType     : Param2             : Tmp : OnTrue : OnFalse
EXEC   : Test1    : STRING   : SHAREDSTORAGE : U.S.TEST_FLOW : ==        : LITERAL       : "CLASS"            : -   : Test2  : 1
EXEC   : Test2    : STRING   : SHAREDSTORAGE : U.S.SSPEC     : ==        : LITERAL       : "T1"               : -   : 3      : Test3
EXEC   : Test3    : STRING   : SHAREDSTORAGE : G.U.S.BLAH    : SET       : SHAREDSTORAGE : U.S.FUSE_TME_TRAY1 : -   : 1      : 2
END_SCREEN
```

----  

#### LITERAL Mode   

Literal mode provides a means for the user to hardcode a literal string or numerical value. This is useful when a screen needs to test for a known value.   

Caveats:
  - String values don't need quotes unless they contain a colon ":", semi-colon ";", spaces or other quotes, if they do, then they should use double quotes ("")
  - Integer, Double, and Binary values do not require quotes.

| START_SCREEN | _ScreenSetName_ |              |             |                  |              |                |            |           |                    |                    |
|:-------------|:----------------|--------------|-------------|------------------|--------------|----------------|------------|-----------|--------------------|--------------------|
| RunOn        | Test Name       | Data Type    | Param1 Type | Param1           | Operation    | Param2 Type    | Param2     | Tmp Store | On True            | On False           |
| EXEC         | _TestName_      | \<data type> | LITERAL     | \<string value>  | \<operation> | \<Param Type2> | \<Param 2> | _ignored_ | _TestName or Port_ | _TestName or Port_ |
| EXEC         | _TestName_      | \<data type> | LITERAL     | \<integer value> | \<operation> | \<Param Type2> | \<Param 2> | _ignored_ | _TestName or Port_ | _TestName or Port_ |
| EXEC         | _TestName_      | \<data type> | LITERAL     | \<double value>  | \<operation> | \<Param Type2> | \<Param 2> | _ignored_ | _TestName or Port_ | _TestName or Port_ |
| EXEC         | _TestName_      | \<data type> | LITERAL     | \<binary value>  | \<operation> | \<Param Type2> | \<Param 2> | _ignored_ | _TestName or Port_ | _TestName or Port_ |
| EXEC         | ...             | ...          | ...         | ...              | ...          | ...            | ...        | ...       | ...                | ...                |
| END_SCREEN   |                 |              |             |                  |              |                |            |           |                    |                    |


ScreenTests file, Literal example:
```
START_SCREEN: TestSet
EXEC : T01 : STRING : GLOBAL : SCVars.SUV1 : SET    : LITERAL : CLASS    : - : T02 : 2
EXEC : T02 : DOUBLE : GLOBAL : SCVars.DUV1 : SET    : LITERAL : 4.5      : - : T03 : 2
EXEC : T03 : INT    : GLOBAL : SCVars.IUV1 : SET    : LITERAL : 124      : - : T04 : 2

# double quotes are not necessary in most cases and will be automatically removed
EXEC : T04 : STRING : GLOBAL : SCVars.SUV1 : ==     : LITERAL : CLASS    : - : T05 : 2
EXEC : T05 : STRING : GLOBAL : SCVars.SUV1 : APPEND : LITERAL : "HOT"    : - : T06 : 2
EXEC : T06 : STRING : GLOBAL : SCVars.SUV1 : ==     : LITERAL : CLASSHOT : - : T07 : 2
EXEC : T07 : STRING : GLOBAL : SCVars.SUV1 : !=     : LITERAL : CLASS    : - : T08 : 2

EXEC : T08 : DOUBLE : GLOBAL : SCVars.DUV1 : ==     : LITERAL : 4.5      : - : T09 : 2
EXEC : T09 : DOUBLE : GLOBAL : SCVars.DUV1 : >      : LITERAL : 4.1      : - : T10 : 2
EXEC : T10 : DOUBLE : GLOBAL : SCVars.DUV1 : <      : LITERAL : 4.9      : - : T11 : 2
EXEC : T11 : DOUBLE : GLOBAL : SCVars.DUV1 : >=     : LITERAL : 4.5      : - : T12 : 2
EXEC : T12 : DOUBLE : GLOBAL : SCVars.DUV1 : <=     : LITERAL : 100      : - : T13 : 2
EXEC : T13 : DOUBLE : GLOBAL : SCVars.DUV1 : !=     : LITERAL :   0      : - : T14 : 2

EXEC : T14 : INT    : GLOBAL : SCVars.IUV1 : ==     : LITERAL : 124      : - : T15 : 2
EXEC : T15 : INT    : GLOBAL : SCVars.IUV1 : >      : LITERAL : 24       : - : T16 : 2
EXEC : T16 : INT    : GLOBAL : SCVars.IUV1 : <      : LITERAL : 999      : - : T17 : 2
EXEC : T17 : INT    : GLOBAL : SCVars.IUV1 : >=     : LITERAL : 1        : - : T18 : 2
EXEC : T18 : INT    : GLOBAL : SCVars.IUV1 : <=     : LITERAL : 124      : - : T19 : 2
EXEC : T19 : INT    : GLOBAL : SCVars.IUV1 : !=     : LITERAL : 0        : - :   1 : 2
END_SCREEN
```

----  

#### EXPRESSION Mode   

The EXPRESSION mode capability allows the user to use expression that will be calculated to represent data. Please reference the [ExpressionEngine section](../Auxiliary/AuxiliaryTest.md#Expression_Engine) in [AuxiliaryTest documentation](../Auxiliary/AuxiliaryTest.md) for more details about the expression format.<br>

Notes:
* Its is possible to use an EXPRESSION as param1 with no operation, but it's return value will always be "true". Beyond that it is treated like any other parameter, its return value will be converted to a \<data type> and sent to the operation. If an expression returns true/false, a String datatype can be use to compare the return value to the Literal True or False.  
* Any Literal values in the expression (like callback names/arguments) should be enclosed in single quotes ('').
* The use of double quotes ("") are required only when the expression contains colons ":", semi-colons ";", spaces or other starting/ending quotes.

| START_SCREEN | _ScreenSetName_ |              |             |               |              |                |            |           |                    |                    |
|:-------------|:----------------|--------------|-------------|---------------|--------------|----------------|------------|-----------|--------------------|--------------------|
| Run On       | Test Name       | Data Type    | Param1 Type | Param1        | Operation    | Param2 Type    | Param2     | Tmp Store | On True            | On False           |
| EXEC         | _TestName_      | \<data type> | EXPRESSION  | \<expression> | \<operation> | \<Param Type2> | \<Param 2> | _ignored_ | _TestName or Port_ | _TestName or Port_ |
| EXEC         | ...             | ...          | ...         | ...           | ...          | ...            | ...        | ...       | ...                | ...                |
| END_SCREEN   |                 |              |             |               |              |                |            |           |                    |                    |

ScreenTests file, Expression examples:

- Execute a prime callback (ignores the return value).  
```
START_SCREEN:PrintBinToItuff_FINAL
EXEC : P1 : STRING : EXPRESSION : ExecCallback('PrintToItuff', '--body_type mrslt --precision 0 --body_data G.U.I.BIN_IB --tname_suf BIN_FINAL_G.U.I.BIN_IB') : - : - : - : - : P2 : 2
EXEC : P2 : STRING : EXPRESSION : ExecCallback('PrintToItuff', '--body_type mrslt --precision 0 --body_data G.U.I.BIN_FB --tname_suf BIN_FINAL_G.U.I.BIN_FB') : - : - : - : - : 1  : 2
END_SCREEN
```

- Execute a built-in function to get the binning information.   
```
#Run  : Test  : Data: Param      : Param1             : Oper : Param   : Param2 : Tmp  : On   : On
#On   : Name  : Type: Type       :                    :      : Type    :        : Store: True : False
EXEC  : Chk02 : INT : EXPRESSION : GetCurrentHardBin() :  >  : LITERAL : 999      : - : Chk03 : Chk04
EXEC  : Chk03 : INT : GLOBAL     : TP_KNOB.Bin69_FLG   : ==  : LITERAL : 2        : - : Chk04 : Chk05
EXEC  : Chk04 : INT : GLOBAL     : TP_KNOB.Bin69_FLG   : SET : LITERAL : 3        : - : 1     : 3
EXEC  : Chk05 : INT : GLOBAL     : TP_KNOB.Bin69_FLG   : SET : LITERAL : 3        : - : 2     : 3
END_SCREEN
```

- Simple math.   
```
START_SCREEN:SampleScreen
#RunOn : TestName : DataType : ParamType     : Param1                               : Operation : ParamType  : Param2                                       : Tmp : OnTrue : OnFalse
EXEC   : TEST01   : DOUBLE   : SHAREDSTORAGE : G.U.D.VMIN_ATOM_CACHE_RF_LSA_M0_PRE  : <         : LITERAL    : 0.54                                         : -   : TEST02 : TEST03
EXEC   : TEST02   : DOUBLE   : SHAREDSTORAGE : U.D.VMIN_ATOM_CACHE_RF_LSA_M0_PRE_GB : SET       : LITERAL    : 0.50                                         : -   : TEST04 : 2
EXEC   : TEST03   : DOUBLE   : SHAREDSTORAGE : U.D.VMIN_ATOM_CACHE_RF_LSA_M0_PRE_GB : SET       : EXPRESSION : [G.U.D.VMIN_ATOM_CACHE_RF_LSA_M0_PRE] - 0.04 : -   : TEST04 : 2
END_SCREEN

START_SCREEN:HCS_FDS_HEXTOBIN
#RunOn : TestName : DataType : ParamType     : Param1                       : Operation : ParamType  : Param2            : Tmp : OnTrue : OnFalse
EXEC   : fds      : STRING   : SHAREDSTORAGE : U.S.FUSE_EMB_VFDM_FDS_BINARY : SET       : EXPRESSION : HexToBin([SVFDS]) : -   : hcs    : 2
EXEC   : hcs      : STRING   : SHAREDSTORAGE : U.S.FUSE_EMB_VFDM_HCS_BINARY : SET       : EXPRESSION : HexToBin([SVHCS]) : -   : 1      : 2
END_SCREEN
```

----   

### Alternative JSON Format   
This TM also supports an alternative JSON file format.
All the same keywords/parameters/options are supported.
Columns are the same except the "RunOn" and "Tmp Store" columns are removed.  
This mode is automatically selected when the ScreenTestsFile Template parameter ends with .json.  

Json Top-Level Keywords:
  * **JsonScreenConfig** - Type List\<ScreenOption> - required

ScreenOption Definition:
  * **TestName** - Type String - required
    * The name of the Screen Set which will be referenced from the ScreenTestSet.
    * Equivalent to the "START_SCREEN:" line from the text file format.
  * **TestConfig** - Type List\<List\<String>> - required
    * 2-dimensional array containing the screen test.
    * Each row is a separate Test.
    * Each column matches a :-separated-column from the text file format, except for the "RunOn" and "TmpStore" columns which are excluded.  

**ScreenFile json format example:**
```json
{
    "JsonScreenConfig":
    [
        {
            "TestName" : "Screen_Test01",
            "TestConfig" :
            [
                ["ExecutionB", "Double", "SharedStorage", "G.U.D.TEN", "SET", "Literal", "10", "ExecutionC", "2"],
                ["ExecutionC", "Double", "GLOBAL", "Screen::Screen.FIVE", "SET", "Literal", "5", "ExecutionD", "2"],
                ["ExecutionD", "Double", "SharedStorage", "G.U.D.TEN", "<", "GLOBAL", "Screen::Screen.FIVE", "1", "2"]
            ]
        },
        {
            "TestName" : "Screen_Test02",
            "TestConfig" : [
                ["Test0","String","Global","SCVars.SC_LOTNAME","SET","Literal","PVxxxxMV1","Test1","2"],
                ["Test1","String","SharedStorage","U.S.RESULT","SET","Expression","Substring([SCVars.SC_LOTNAME], Length([SCVars.SC_LOTNAME]) - 3, 2)","Test2","2"],
                ["Test2","String","SharedStorage","U.S.RESULT","==","Literal","CR","Test3","2"],
                ["Test3","String","SharedStorage","U.S.RESULT","==","Literal","MV","1","2"]
            ]
        }
    ]
}
```

**Json schema**
```json
{
    "$schema": "http://json-schema.org/draft-04/schema#",
    "type": "object",
    "additionalProperties": false,
    "properties": {
        "JsonScreenConfig": {
            "type": "array",
            "items": [
                {
                    "type": "object",
                    "properties": {
                        "TestName": {
                            "type": "string"
                        },
                        "TestConfig": {
                            "type": "array"
                        }
                    },
                    "required": [
                        "TestName",
                        "TestConfig"
                    ]
                }
            ]
        }
    },
    "required": [
        "JsonScreenConfig"
    ]
}
```

----   

## Test Instance Parameters 

The table below lists and describes the user interface parameters supported by the ScreenTest test method

| **Parameter Name** | **Required?** | **Type** | **Values**                                      | **Comments** |  
|:-------------------|:--------------|:---------|:------------------------------------------------|:-------------|   
| ScreenTestsFile    | Yes           | File     | Path to file containing test executions.        | json or txt  |  
| ScreenTestSet      | Yes           | String   | Test name pre-existing in the screen test file. |              |  

----  

## Datalog output
Print when Datalog is enabled.

**When a Parameter exception occurs**. 
```
2_tname_<ScreenTest instance name>_EC
2_strgval_<Encoded Error>
```

Example: row not found in SharedStorage
```
2_tname_ScreenTest::ScreenTestSharedStorage_InvalidToken_txt_P2_T01_EC
2_strgval_DEFLATE32_WVJU23Y2GEIL2R7KP6MFGRJFGBUZWNANCKKSBO34FBIBBO7BWJNELRR3YAFGGU33S2HUW733NUQCCVCR2WUKV5XCDXN7HXU364OO2LHBSI24ZLCYUKRHAK3QIW4VMNMI43OGAFURGZ6INLENGTTLEQAIRXI2NK5GXIDEG3XYWKPLJWIDRPMPOQJ4N32LUQNDPEL2IEHPKZME7ISOMMKAMUJ4DA25UYIKXVOGCLPZSRBCXF3O6GZRH5HDY54NQ6YMXKRWVDEWQZDM6XCLLOGYIQKUXZKO325MSB4FN2VKKVA35R6LVL7WPNFFCQVJLSRXDCNHZCNMZEW3EZ2NS22TQ4PZIJRTYRZQPIBZXHHGWDAB2JHHGIZI2FUUE3AG23WJAITSA5ZFREHFHC6I4RVFNBVFCXNMQ7CSQYYRV2Z226X5RJP7ZJYFOSBKBTLBKFTEXAWMHMEYZBONZYZO2YFVJ5OJZUG36FWXL47N47V6YLF27NZJCCPURBC4QHIIU4CLSACA64OKI2HONHD3G2YPMRVQRN3ROAE62FA6VGK5CGNHKLUQAVHK6I2ZBDH4DMU6CZOPVWWEZ7CVT2U2W2GF5RQ6GKE5GUSDBC2HNTYM7BUV3BUDKI7SEJ7JZDV4R36DL7YHRNPYBL7B6YGK2CGRYHZX3TRYMC6LHLH5B6C752JGLQE65X74NS3HUSHTJV4ZROA5JG3QXMZIP4Y5VYZOSYGZH3HURTWC2PZDT6FFIM7ROLQIKUDHV2RM6KQPRFBGWDCMKAU4ZMBP3MP7AL7ZM7VHW7ASFFBINCYEFQZ52VJFG24JLSA3XYXS7FSO664Y53PYO77B74CBO3SRCSCODD7CAD3VNZ36AE======
```

----
## Custom User Code hooks

N/A

----

## TPL Samples  

```csharp
Test PrimeScreenTestTestMethod FUSE_X_SCREEN_K_INIT_X_X_X_X_SET_VF_SIZE_P1
{
    ScreenTestSet = "SetVFSize";
    ScreenTestsFile = "./Modules/TPI_FUSE/InputFiles/screentest_fuse_read.txt";
}

Test PrimeScreenTestTestMethod FUSE_X_SCREEN_K_START_X_X_X_X_PERDIE_STATE_INIT_P1
{
    ScreenTestSet = "PerDieValuesInit";
    ScreenTestsFile = "./Modules/TPI_FUSE/InputFiles/screentest_fuse_read.txt";
}
```

----  

## Exit Ports  

| Exit Port | Condition | Description            |   
|:---------:|:---------:|------------------------|    
|    -2     |   Alarm   | Any HW alarm condition |    
|    -1     |   Error   | Any software error     |   
|     0     |   Fail    | Default fail port.     |   
|     1     |   Pass    | Default pass port.     |   
|    2+     |   Pass    | User assigned.         |   

----  

## Additional Dependencies
* Expression Engine: Required only for the Expression mode, this engine supports Ncalc as expression engine. https://github.com/ncalc/ncalc   
It is able to evaluate simple mathematical expression using SharedStorage, UserVar and/or DFF tokens, etc.
For detailed information check the [ExpressionEngine section](../Auxiliary/AuxiliaryTest.md#Expression_Engine) in the AuxiliaryTest documentation.

----  

## Version tracking
| **Date**                  | **Version** | **Author**    | **Comments**             |
|---------------------------|-------------|---------------|--------------------------|
| Jan 26<sup>th</sup>, 2024 | 13.00.00    | Maria N Rojas | Initial document release |

## Acronyms
Definition of acronyms used in this document:
  - **BMFC**: Bin Matrix Flow Control.
  - **DUT**: Device Under Test.
  - **ITUFF**: Intel Tester Universal File Format.
  - **JSON**: JavaScript Object Notation.
  - **TM**: Test Method.

