[[_TOC_]]

----  
# Introduction  
This template can be used to execute any Prime Callback function. The callback will print debug messages correctly based on the LogLevel parameter.

----
<h1 style="font-size: 1.1rem; color: yellow">Note: PrimeRunCallbackTestMethod use NCalc.Expression givin us flexibility to users but has a toll in time consumption. The recommendation for users looking to improve test time is to create and use alternative solutions to the use of RunCallback.</h1>
____
# Test Instance Parameters  

| Parameter Name       | Required? | Type | Values |  Comments |  
| :-----------         | :----------- | :----------- | :----------- | :----------- |   
| Callback             | Yes | String |  | Name of the callback to execute. |  
| Parameters           | No  | String |  | Parameters to send to the callback. |  
| ResultToken          | No  | String |  | GSDS (of the form G.[ULI].S.TokenName) to hold the return value from the callback. |  
| ResultPort           | No  | String |  | Expression to determine exit port. Expected to be solved as int. Uses [R] as the return value from the callback. See AuxiliaryTC for expression examples.|  

----  
# TPL Samples  

```perl
Test PrimeRunCallbackTestMethod BASEPRIM_X_FUNC_K_BEGIN_X_X_X_X_CORETOTRACKER_ATOM  
{
    Callback = "WriteTracker";  
    Parameters = "--tracker ATOM_M0,ATOM_M1 --value 00000000";  
}  

Test PrimeRunCallbackTestMethod SOMETESTNAME  
{  
    Callback = "ExecuteInstance";  
    Parameters = "--test SOMEOTHERTESTINSTANCE --save_exit_port G.U.S.ExitPort";  
    ResultPort = "ToInt32([G.U.S.ExitPort])==1?1:0"    
}  

```


----  
# Exit Ports  

| Exit Port       | Condition   | Description |   
| -----------     | ----------- | ----------- |    
| -2  | Alarm | Any HW alarm condition |    
| -1  | Error | Any software error |   
| 0   | Fail  | nothing causes this today. |   
| 1   | Pass  | If the callback was successfully executed. |   

----  
