<h1>Prime CallbacksRegistrar Test-Method Specification REP</h1>

Revision 1.0.0

Oct 2020

[[_TOC_]]

## Methodology

The CallbacksRegistrar test method provides capability to register user callbacks to be used later in the code as described in the dedicated [Callbacks Wiki page](https://dev.azure.com/mit-us/PrimeWiki/_wiki/wikis/PrimeWiki.wiki/28912/Callbacks).

## Test Instance Parameters

The table below lists and describes the test instance parameters supported by the CallbacksRegistrar test method

| **Parameter Name** | **Required?** | **Type** | **Values** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ |

**Notes:**
  - This Test Method doesn't do anything when instantiating it by itself. It is expected that the user will extend it through ITME/IFE and implement the ICallbacksRegistrarExtension.RegisterCallbacks() function.
- The registration of the callbacks itself is being done during Verify stage of this Test Method. The Execute will do nothing except exit through port 1.

## Datalog output
None

## Custom User Code Hooks

Here is the list of functions available to the user code to override.

**RegisterCallbacks()** - Here the user will register his callbacks. See [Callbacks Wiki page](/Prime-Customer-Wiki/General/Frequent-Asked/User-Code/Prerequisite-[User-Code]/Callbacks) for details.


## TPL Samples

Here are a few test instance examples using the MyTestMethodName test method

**TPL Sample1:**

```python
Import MyCallbackRegistrarUserCode.xml;

Test MyCallbacksRegistrarUserCode MyCallbacksRegistrarUserCode
{
   
}
```
## Exit Ports

The CallbacksRegistrar test method supports the following exit ports:


| **Exit Port** | **Condition**   | **Description**              |
| ------------- | --------------- | ---------------------------- |
| **-2**        | ***Alarm***     | Any alarm condition          |
| **-1**        | ***Error***     | Any software condition error |
| **1**         | ***Pass***      | Passing condition            |

## Additional Dependencies

The Verify of the CallbacksRegistrar instance must be executed before any usage in the flow of the callbacks being registered by it.

## Version tracking


| **Date**       | **Version** | **Author**   | **Comments** |
| -------------- | ----------- | ------------ | ------------ |
| Oct 11th, 2020 | 1.0.0       | Eran Hirsh |  Initial doc            |
|                |             |              |              |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System