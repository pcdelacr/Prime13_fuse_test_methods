**prime Test-Method Specification REP**

Prime Release 13.00.00

October 13<sup>th</sup>, 2023

[[_TOC_]]

## REP for Auxiliary

This **REP** is intended to describe the Auxiliary Prime TestMethod.

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
----  
## Introduction  
Auxiliary is a basic utility test method that allows user to evaluate mathematical expressions evaluation capability, supporting various kinds of functional and logic processing semantics. It can use SharedStorage, Uservars and/or DFF and store evaluate result into a new token, and/or assign an exit port based on a second expression.
## Methodology
<p><span style="font-weight: bold; color:gold">Disclaimer:</span> This template uses for its expression calculations the external dependent of the nugets CoreCLR-NCalc (2.2.113) and MathNet.Numerics (5.0.0).

**The template runs in 3 main stages:**

1)	Parse (during Verify) and evaluate (during Execute) expression value *(Result)*.
2)	Print Result to Datalog *(if feature  is enabled)*.
3)	Evaluate Fork expression *(if exist)* and set final Port. If no Fork expression, exit port 1 *(if no other error occurred)*.

The template supports having variables as part of the expression, to be evaluated at run-time.
The supported variables are: UserVar, DFF, SharedStorage tables.

<h1 style="font-size: 1.1rem; color: yellow">Note: PrimeAuxiliaryTestMethod use NCalc.Expression givin us flexibility to users but has a toll in time consumption. The recommendation for users looking to improve test time is to create and use alternative solutions to the use of Auxiliary.</h1>

### Capabilitys
1. Calculate a math expression.
2. Expression result can be stored with a token name in sharedStorage, userVars and Dff.
3. Support common math functions.
4. Support string operations (string compare, substr, string concatenation, string matching (like, ‘*’, ))
5. Expression is evaluated to a result. Use Result for:
    - Set UserVar, dff or sharedStorage.
    - Set final port(**fork**) per expression logic that contains the Result value.
    - Print Result to Datalog.
----  
## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the Auxiliary test method

| Parameter Name       | Required? | Type | Values |  Comments |  
| :-----------         | :----------- | :----------- | :----------- | :----------- |   
| Expression           | Yes | String |  | Expression to be evaluated. Uses NCalc third party as engine. Tokens should use '[]'. |  
| DataType             | Yes | Enum   |  | String, Double or Integer.|  
| StorageType          | No  | Enum   |  | Defines storage type for ResultToken. SharedStorage, Uservar or DFF.|  
| ResultToken          | No  | String |  | Token name where result will be stored. Uses Storage and DataType to define the variable to use but shared storge should still be the full format U.S.blah.|  
| ResultPort           | No  | String |  | Expression to determine exit port. Expected to be solved as int. Uses [R] as result token.|  
| Datalog              | No  | Enum   |  | Enabled or Disabled. Integer and string will be printed as strgval while double will be set as msrlt.|  

----  
## Console output (debug mode)

## Datalog output

Print when Datalog is enabled.

### If result is a string
```
2_tname_<Auxiliary instance name>_results
2_strgval_<expression result>
```
### If result is a double
```
2_tname_<Auxiliary instance name>_results
2_mrslt_<expression result>
```

----
## Custom User Code hooks
Auxiliary test method support the folowing extensions available to be overrided by user.
- CustomGetExpressionEngine - Create a expression engine with user setup to be used by Auxiliary, The users here is enabled to add more function to the engine.
- CustomGetPortExpressionEngine - Create an expression engine to set the final port, The users here is enabled to add more function to the engine.

----
## TPL Samples  

###  

```perl
Test PrimeAuxiliaryTestMethod ExampleAux_Variables  
{
    Expression = "[T.IP_CPU_BASE::FlowDomain.Default] + [_UserVars.IntToken1]+ToInt32([L.S.Token2])";  
    DataType = "Integer";  
    StorageType = "SharedStorage";  
    ResultToken = "U.I.ResultToken";  
    ResultPort = "[R]>0?2:1";  
}
```

```perl
Test PrimeAuxiliaryTestMethod ExampleAux_Function  
{
    Expression = "GetCurrentDataBin()";  
    DataType = "Integer";  
    StorageType = "DFF";  
    ResultToken = "OLB";  
}  
```

```perl
Test PrimeAuxiliaryTestMethod ExampleAux_PrimeCallback  
{
    Expression = "ExecCallback('SomeCallback', '--args1 A,B,C --args2 ' + [U.S.Token] + ' --args3 Blah')";  
    DataType = "String";  
    ResultPort = "[R]=='PASS'?1:2;  
}  
```

```perl
Test PrimeAuxiliaryTestMethod AuxiliaryTC_ReplaceEvgVisualIdCheck
{
    # String SC_VISUALID = "D2HN723401482";
    Expression = "Length([SCVars.SC_VISUALID]) == 13 and not IsMatch([SCVars.SC_VISUALID], '^0+$') and not IsMatch([SCVars.SC_VISUALID], '^F+$') and IsMatch([SCVars.SC_VISUALID], '^[a-zA-Z0-9]+$')";
    DataType = "String";
    ResultPort = "[R]?1:0";
    Datalog = "Enabled";
}
```

```perl
Test PrimeAuxiliaryTestMethod AuxiliaryTC_LinearFitRquared_P0
{
    # U.S.XValues = "1|3|5"
    # U.S.YValues = "22|27|29"
    # Result = 0.942 (exit port 0)
    Expression = "Round(GetDelimitedData(LinearFit([U.S.XValues], [U.S.YValues], '|'), '|', 2), 3)";
    DataType = "Double";
    ResultPort = "[R] > 0.975 ? 1 : 0";
    Datalog = "Enabled";
}
```
----  
## Exit Ports  

| Exit Port       | Condition   | Description |   
| -----------     | ----------- | ----------- |    
| -2  | Alarm | Any HW alarm condition |
| -1  | Error | Non-DUT related errors that are unexpected (e.g. API, tester, communication, user function, etc.).  |
| 0   | Fail  | User assigned. |
| 1-20| Pass  | User assigned. |

###

----  
## Additional Dependencies
### Expression Engine
This engine supports Ncalc as expression engine. https://github.com/ncalc/ncalc   
It is able to evaluate simple mathematical expression using SharedStorage, Uservar and/or DFF tokens, etc.  

All tokens must be specified using '[]'. Value will be evaluated in the following order:
- SharedStorage: Should be of the form _context_._type_.Token where _context_ is **L** for **Lot**, **U** for **DUT**/**Unit** or **I** for **IP**, and _type_ is **S** for *string*, **D** for *double* or **I** for *integer*3e.  
- UserVar: String, Double and Integer. Always using full _collection_._uservar_ format with the full IP/Module scoping on the collection.  
- DFF: Using token name. It will be read from the current optype and die id.  
- MultiTest Trial Variables: Use the form T._collection_._variable_ - Always use the full collection name with IP/Module scoping.  
- Environment Variables: Use the form E._variable_.  
- ResistanceOffset Variables: Use the form R.O._signal_.  
- VminForwarding Variables: Format 1: CoreDomain^CR0@F1, Format2: CR0@F1 when instance is setting FlowIndex.

Any literal string values should be enclosed in single quotes.  

#### Supported functions:
https://github.com/ncalc/ncalc/wiki/Functions

#### Additional functions:   
| Function                                            | Details                                                                                                                                                                                                                                                                                                                                                               |   
|:----------------------------------------------------|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|   
| AndBitWise(*value1*, *value2*)                      | Executes a bitwise AND between the two binary string values. It will fail if the two values are not the same size.                                                                                                                                                                                                                                                    |   
| ArrayAverage(*array*, *delimiter*)                  | Converts string array to doubles and calculates the average.                                                                                                                                                                                                                                                                                                          |   
| ArrayMax(*array*, *delimiter*)                      | Converts string array to doubles and calculates the max.                                                                                                                                                                                                                                                                                                              |   
| ArrayMedian(*array*, *delimiter*)                   | Converts string array to doubles and calculates the median.                                                                                                                                                                                                                                                                                                           |   
| ArrayMin(*array*, *delimiter*)                      | Converts string array to doubles and calculates the min.                                                                                                                                                                                                                                                                                                              |   
| ArraySum(*array*, *delimiter*)                      | Converts string array to doubles and calculates the sum.                                                                                                                                                                                                                                                                                                              |   
| Bin2Dec(*value*)                                    | Converts the binary string *value* into an integer. *value* must be less than 32 bits.                                                                                                                                                                                                                                                                                |   
| Bin2Hex(*value*)                                    | Converts the binary string *value* into a string hex. *value* can exceed 64bits. Result will be MSB first, no radix, just a raw hex string: Bin2Hex('01111') -> 0F                                                                                                                                                                                                    |   
| Contains(*input*, *partial*)                        | Check if input contains partial.                                                                                                                                                                                                                                                                                                                                      |   
| Dec2Bin(*value*, *bits*)                            | Converts the integer *value* into a binary string containing *bits* number of bits. Result will be MSB first, no radix, just a raw binary string: Dec2Bin(5, 6) -> 000101                                                                                                                                                                                             |   
| ExecCallback('*callback*', '*args'*)                | Execute the Prime callback with the given argument.                                                                                                                                                                                                                                                                                                                   |   
| GetCurrentDataBin()                                 | Returns current data bin.                                                                                                                                                                                                                                                                                                                                             |   
| GetCurrentHardBin()                                 | Returns current hard bin.                                                                                                                                                                                                                                                                                                                                             |   
| GetCurrentSoftBin()                                 | Returns current soft bin.                                                                                                                                                                                                                                                                                                                                             |   
| GetDelimitedData(*value*, *pattern*, *index*)       | Splits *value* into a list based on the regex *pattern* and returns the item at index *index*. *pattern* can be a single character like \| or , or a string or a regular expression.                                                                                                                                                                                  |   
| GetPatSymbolString(*value*, *size*)                 | Converts *value* into a binary string suitable for pattern-modify.  *value* must contain a 2 character prefix denoting format - 0x for hex, 0b for binary, 0d for decmial (up to 64 bits only). If *value* ends with 'r the returned binary value will be reversed.  Example formats -  0xFFF, 0b0101, 0d7, 0xFFF'r, 0b0101'r                                         |   
| Hex2Bin(*value*, *bits*)                            | Converts the string *value* into a binary string containing *bits* number of bits. Result will be MSB first, no radix, just a raw hex string: Hex2Bin('AF', 10) -> 0010101111                                                                                                                                                                                         |   
| IndexOf(*value*, *char**)                           | Gets the 0 based Index for character in a string.                                                                                                                                                                                                                                                                                                                     |   
| IQR(*array*, *delimiter*)                           | Converts string array to doubles and calculates the IQR (p75 - p25).                                                                                                                                                                                                                                                                                                  |   
| IsMatch(*input*, *regex*)                           | Runs a regular expression pattern match on *input* using the pattern *regex*. Returns true if it matches, false otherwise. See C# [Regex.IsMatch](https://learn.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex?view=netframework-4.6.2) for the regex details.                                                                                   |   
| LastIndexOf(*value*)                                | Gets the LAST 0 based Index for character in a string.                                                                                                                                                                                                                                                                                                                |   
| Length(*string*)                                    | Returns the length of the string argument.                                                                                                                                                                                                                                                                                                                            |   
| LinearFit(*xValues*, *yValues*, *delimiter*)        | Performs a simple linear regression on the data using [Math.Numerics Fit.Line and GoodnessOfFit.RSquared](https://numerics.mathdotnet.com/Regression.html). The *xValues* and *yValues* should be *delimiter* separated lists. It returns a *delimiter* separated list of the form "Intercept,Slope,Rsquared" (use GetDelimitedData to extract the individual values) |   
| LittleEndian(*value*)                               | Converts a 32-bit Hex value from Big Endian to Little Endian at the byte level. Example: LittleEndian('78ABCDEF') returns 'EFCDAB78'                                                                                                                                                                                                                                  |   
| Max(*value1*, *value2*, ... , *valueN*)             | Gets the maximum of a comma-separated list of values. This overwrites the built-in Max() which only accepts two arguments. If any of the arguments is a non-integer the result will be returned as a double, otherwise it will be an integer.                                                                                                                         |   
| Min(*value1*, *value2*, ... , *valueN*)             | Gets the minimum of a comma-separated list of values. This overwrites the built-in Min() which only accepts two arguments. If any of the arguments is a non-integer the result will be returned as a double, otherwise it will be an integer.                                                                                                                         |   
| NotBitWise(*value*)                                 | Executes a bitwise NOT on the binary string value (ie. it inverts all the bits).  NotBitWise('0011') -> '1100'                                                                                                                                                                                                                                                        |   
| OrBitWise(*value1*, *value2*)                       | Executes a bitwise OR between the two binary string values. It will fail if the two values are not the same size.                                                                                                                                                                                                                                                     |   
| Percentile(*array*, *p* *delimiter*)                | Converts string array to doubles and calculates the percentile p. p is an int.                                                                                                                                                                                                                                                                                        |   
| Random()                                            | Generates a random value between 0 and 1                                                                                                                                                                                                                                                                                                                              |   
| Replace(*input*, *searchValue*, *replaceValue*)     | A simple case-sensitive search-replace. All instances of the string *searchValue* will be replaced with the string *replaceValue* in *input*                                                                                                                                                                                                                          |   
| Reverse(*value*)                                    | Reverses the string *value*                                                                                                                                                                                                                                                                                                                                           |   
| ScalarMultiplyArray(*scalar*, *array*, *delimiter*) | Multiples each element of an array by the scalar value. Ex: ScalarMultiplyArray(1000, '1,2,3', ',')                                                                                                                                                                                                                                                                   |   
| SelectArrayElements(*value*, *range*, *delimiter*)  | Extracts a new array in a string using the entered indexes range. Ex: SelectArrayElements('1,2,3', '1-2', ',')                                                                                                                                                                                                                                                        |   
| Substring(*value*, *start*, *length*)               | Creates a substring of *value*, starting at index *start* (0 is first bit), and containing *length* bits                                                                                                                                                                                                                                                              |   
| SubtractArrays(*value1*, *value2*, *delimiter*)     | Subtract values from two arrays of same size. Ex: SubtractArrays('1,2,3', '1,0,0', ',')                                                                                                                                                                                                                                                                               |   
| ToDouble(*value*)                                   | Converts a string or int value to a double                                                                                                                                                                                                                                                                                                                            |   
| ToInt32(*value*)                                    | Converts a string or double value to an integer                                                                                                                                                                                                                                                                                                                       |   
| ToString(*value*)                                   | Converts any value to a string                                                                                                                                                                                                                                                                                                                                        |   
| XorBitWise(*value1*, *value2*) 
____
## Custom User Code Hooks
User is enabled to add funtion using the method extencion **AddFunctions**

### Examples of user codes extensions

```CS
namespace IntegrationTestUserCode.FlowExpressionHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NCalc;
    using Prime.BinCounterService;
    using Prime.CallbacksService;
    using Prime.ConsoleService;
    using Prime.DatalogService;
    using Prime.DffService;
    using Prime.MultiTrialTestService;
    using Prime.PhAttributes;
    using Prime.PlatformService;
    using Prime.SessionService;
    using Prime.SharedStorageService;
    using Prime.TestConditionService;
    using Prime.TestMethods.FlowExpressionHandling;
    using Prime.TestMethods.FlowExpressionHandling.ExpressionEngine;
    using Prime.TestProgramService;
    using Prime.UserVarService;
    using Prime.VminForwardingService;

    /// <inheritdoc />
    [PrimeTestMethod]
    public class AuxiliaryWithFunctionsExtensions : PrimeAuxiliaryTestMethod, IAuxiliaryExtensions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuxiliaryWithFunctionsExtensions"/> class.
        /// </summary>
        public AuxiliaryWithFunctionsExtensions()
            : base(
                Prime.Base.ServiceStore<IDffService>.Service,
                Prime.Base.ServiceStore<IConsoleService>.Service,
                Prime.Base.ServiceStore<ISharedStorageService>.Service,
                Prime.Base.ServiceStore<IUserVarService>.Service,
                Prime.Base.ServiceStore<IDatalogService>.Service,
                new ExpressionEngineServices(
                    Prime.Base.ServiceStore<IDffService>.Service,
                    Prime.Base.ServiceStore<IMultiTrialTestService>.Service,
                    Prime.Base.ServiceStore<IConsoleService>.Service,
                    Prime.Base.ServiceStore<IPlatformService>.Service,
                    Prime.Base.ServiceStore<ITestProgramService>.Service,
                    Prime.Base.ServiceStore<IVminForwardingService>.Service,
                    Prime.Base.ServiceStore<ITestConditionService>.Service,
                    Prime.Base.ServiceStore<ISharedStorageService>.Service,
                    Prime.Base.ServiceStore<IUserVarService>.Service),
                new FunctionsServices(
                    Prime.Base.ServiceStore<IConsoleService>.Service,
                    Prime.Base.ServiceStore<IBinCounterService>.Service,
                    Prime.Base.ServiceStore<ICallbacksService>.Service))
        {
        }

        /// <inheritdoc/>
        List<Action<FunctionArgs, ISessionContextProviderContainer>> IAuxiliaryExtensions.AddFunctions()
        {
            var functions = new List<Action<FunctionArgs, ISessionContextProviderContainer>>();
            functions.Add(MyPrintToItuff);
            functions.Add(MyTestInstancePort);
            return functions;
        }

        private static void MyPrintToItuff(FunctionArgs arguments, ISessionContextProviderContainer context)
        {
            var writer = Prime.Base.ServiceStore<IDatalogService>.Service.GetItuffStrgvalWriter();
            var data = System.Convert.ToString(arguments.Parameters.First().Evaluate());
            writer.SetData(data);
            Prime.Base.ServiceStore<IDatalogService>.Service.WriteToItuff(writer, context);
            arguments.Result = data;
        }

        private static void MyTestInstancePort(FunctionArgs arguments, ISessionContextProviderContainer context)
        {
            var expectedPortByInstanceName = Prime.Base.ServiceStore<IPlatformService>.Service.Utils
                .GetCurrentTestInstanceName().Split('_').Last().Trim('P');
            Prime.Base.ServiceStore<IConsoleService>.Service.PrintDebug(() => $"Expected port for current port=[{expectedPortByInstanceName}]", context);
            arguments.Result = int.Parse(expectedPortByInstanceName);
        }
    }
}
```
----
## Version tracking
| **Date**                  | **Prime Version** | **Author**      | **Comments**                                                                        |
| ------------------------- | ----------- | --------------------- | ------------------------------------------------------------------------------------|
| Oct 20<sup>th</sup>, 2023 | 13.0        | Didier Armando Jimenez Retana | Initial version, [Ticket](https://dev.azure.com/mit-us/PRIME/_workitems/edit/44462) |                                                                              |
____
## Acronyms
- **TPL**: Test Programming Language
- **CoreCLR**:  **Core** **C**ommon **L**anguage **R**untime
- **IQR**: The **I**nter**Q**uartile **R**ange (IQR) measures the spread of the middle half of your data.
- **MSB**: The **M**ost **S**ignificant **B**it