<h1>Prime CallbacksRegistrar Test-Method Specification REP</h1>

Revision 1.0.1

Oct 2024

[[_TOC_]]

## Methodology

The CallbacksRegistrar test method provides capability to register user callbacks to be used later in the code as described in the dedicated [Callbacks Wiki page](https://dev.azure.com/mit-us/PrimeWiki/_wiki/wikis/PrimeWiki.wiki/90037/Callbacks).

## Test Instance Parameters

- No instance parameters.

**Notes:**
  - This Test Method doesn't do anything when instantiating it by itself. It is expected that the user will extend it through ITME/IFE and implement the ICallbacksRegistrarExtension.RegisterCallbacks() function or use [CallbackMethod] attribute on required methods.

- The registration of the callbacks itself is being done during Verify stage of this Test Method. The Execute will do nothing except exit through port 1.

## Datalog output
None

## Custom User Code Hooks

Here is the list of functions/capabilities available to the user code to override.

**RegisterCallbacks()** - Here the user will register his callbacks. See [Callbacks Wiki page](https://dev.azure.com/mit-us/PrimeWiki/_wiki/wikis/PrimeWiki.wiki/90037/Callbacks) for details.

Note - this hook is allowed only for callbacks that return string and except string or return void and expect string.

**CallbackMethod Attribute** - Here the user will register his callbacks by adding the attribute [CallbackMethod] above the required method.

by using this attribute, the user can register generic callback which expects to get any type or receiving any type or amount of arguments.

## TPL Samples

```python
Import MyCallbackRegistrarUserCode.xml;

Test MyCallbacksRegistrarUserCode MyCallbacksRegistrarUserCode
{
   
}
```

## UserCode Example - register with RegisterCallbacks() hook:

```python
[PrimeTestMethod]
    public class RegisterGenericCallbacks : PrimeCallbacksRegistrarTestMethod
    {
        public static string SomeStringBasedCallback(string arg)
        {
            var session = ServiceStore<ISessionService>.Service.GetCurrentThreadSessionContextContainer();
            var result = $"string argument: {arg}";
            ServiceStore<IConsoleService>.Service.PrintDebug(() => result, session);
            return result;
        }

        public override void RegisterCallbacks()
        {
            this.RegisterCallback(SomeStringBasedCallback);
        }
    }
```

## UserCode Example - register with CallbackMethod attribute:

```python
[PrimeTestMethod]
    public class RegisterGenericCallbacks : PrimeCallbacksRegistrarTestMethod
    {
        [CallbackMethod]
        public static Dictionary<string, int> SomeGenericCallback(string key, int value)
        {
            var session = ServiceStore<ISessionService>.Service.GetCurrentThreadSessionContextContainer();
            var result = $"Key: {key}, Value: {value}";
            ServiceStore<IConsoleService>.Service.PrintDebug(() => result, session);
            var dictResult = new Dictionary<string, int>
            {
                { key, value },
            };
            return dictResult;
        }
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
| Oct 11th, 2020 | 1.0.0       | Eran Hirsh   |  Initial doc |
| Oct 6th, 2024  | 1.0.1       | Izhar Mishli |  Add callback registration attribute |

## Acronyms

Definition of acronyms used in this document:

  - **REP**: P**r**ime T**e**st-Method S**p**ecification
  - **HDMT**: High Density Modular Tester
  - **TPL**: Test Programming Language
  - **TOS**: Test Operating System