[[_TOC_]]

# Prerequisites

The data source properly setup.

# InputData
The InputData is a C# service, and provides the capability to use different data source for a given user input, for the basic types: `string`, `int` and `double`.
``` C#
using Prime.BusinessLogic.InputData;
var factory = FactoryBuilder.Build();
string input = ...;

var stringHandler = factory.CreateInputString(input);
string myActualStringValue = stringHandler.Get();

var intHandler = factory.CreateInputInteger(input);
int myActualIntValue = intHandler.Get();

var doubleHandler = factory.CreateInputDouble(input);
double myActualDoubleValue = doubleHandler.Get();
```
The `factory.CreateInput*(input)` resolves the source at during construction, while the `handler.Get()` resolves the actual value at running time.

# Input format description
The InputData business logic support three different data source: SharedStorage, UserVars, and Dff.
The input data is expected to follow the format `<Source>.<Source specific key>`.
If the input doesn't match that, the factory will treat them as raw data if none of them match.

- SharedStorage's format:
The SharedStorage's specific key follows the format `S.<Context>.<Key>`, where the `Context` is the context of the data (L=Lot, D=Dut, I=Ip), and the `Key` is the key in the respective table.
- UserVars' format:
The UserVars' specific key follows the format `U.<Module>::<Collection.<Name>`, where the `Module` and `Collection` being optional.
- Dff's format:
The Dff's specific specific key follow the format `D.<TargetId>.<OperationType>.<Token>`, where the `TargetId` and `OperationType` can be set to `Current` to use the current target id or current operation type.

# Version tracking

| **Date**  | **Prime release** | **Author** | **Comments**            |
|-----------|-------------------|------------|-------------------------|
| Oct, 2022 | 11.00.00          | lchavarr   | Adding initial version. |