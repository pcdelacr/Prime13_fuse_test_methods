Prime Test-Method Specification REP

Revision 1.2.0

June 2022

[[_TOC_]]

## Methodology

It is a simple Test Method that takes 8 digit softbin and converts it to 4 digits and calls the TOS API to set it as the final bin for ODESE scripts.

**<span style="color: red">Important:</span>**
**Note that the 4-digit bin must exist in the bin definition file, otherwise the methodology will fail.**

The test method is extensible using ITME. The default implementation takes the indexes described on the uservas:
* RunTimeLibraryVars.iCGL_PrimeSimplifiedBinningIndexesForPassingBin and 
* RunTimeLibraryVars.iCGL_PrimeSimplifiedBinningIndexesForFailingBin 

to create a 4-digit bin. This logic can be enhanced by the user.

Examples:

| **8-Digit SoftBin**|**4-Digits Softbin**|**Uservar**|
|--------------------|------------------|-------|
| 90440012|4400|RunTimeLibraryVars.iCGL_PrimeSimplifiedBinningIndexesForFailingBin = "2,3,4,5"|
| 90620012|9200|RunTimeLibraryVars.iCGL_PrimeSimplifiedBinningIndexesForFailingBin = "0,3,4,5"|
| 90621212|6200|RunTimeLibraryVars.iCGL_PrimeSimplifiedBinningIndexesForFailingBin = "2,3,4,5"|
| 10010100| 101|RunTimeLibraryVars.iCGL_PrimeSimplifiedBinningIndexesForPassingBin = "2,3,4,5"|


<span style="color: #FF5662">Notes:</span>

[1] This is simpler version of the Evergreen WriteBmfcFbinIntoTssSoftBinForOdese.  
[2] This Index count is from 0 to 7.  
[3] User MUST specify 4 indexes on each uservar. 

## Test Instance Parameters

| **Parameter Name** | **Required?** | **Type** | **Values** | **Comments** |
| ------------------ | ------------- | -------- | ---------- | ------------ |

## Custom User Code Hooks

This test method supports the following extensions available to the user code to override:

```C#
    public interface IOdeseBinConverterExtensions
    {
        /// <summary>
        /// Called prior to any test in Execute to generate the test instance results object.
        /// </summary>
        /// <returns>The test instance results object.</returns>
        OdeseTestInstanceResults CreateTestInstanceResults();

        /// <summary>
        /// Provides the user the ability to post process exit port, and implement their logic for Softbin Calculation.
        /// </summary>
        /// <param name="results">The object for holding the exit port, and 8-digit softbin.</param>
        /// <returns>Converted 4-digit value.</returns>
        uint ConvertSoftBin(OdeseTestInstanceResults results);
    }
```

- CreateTestInstanceResults - Creates test instance results object that holds all results and the exit port (to be done early in Execute).

- ConvertSoftBin - Allows user to enhance the conversion logic and override the exit port determined by the test method.  
Input is the test instance results object which includes the 8-digit softbin value, and the exit port.

**Default Conversion Logic**
```C#
uint IOdeseBinConverterExtensions.ConvertSoftBin(OdeseTestInstanceResults results)
{
    var currentBin = results.CurrentSoftBin.ToString();
    if (currentBin.Length <= 4)
    {
        return results.CurrentSoftBin;
    }

    var simplifiedBinningIndexesForPassingBin = Prime.Services.UserVarService.GetStringValue("RunTimeLibraryVars", "iCGL_PrimeSimplifiedBinningIndexesForPassingBin").Split(',');
    var simplifiedBinningIndexesForFailingBin = Prime.Services.UserVarService.GetStringValue("RunTimeLibraryVars", "iCGL_PrimeSimplifiedBinningIndexesForFailingBin").Split(',');

    var newBin = currentBin[0] == '9' ?
        simplifiedBinningIndexesForFailingBin.Aggregate(string.Empty, (current, c) => current + currentBin[int.Parse(c)]) :
        simplifiedBinningIndexesForPassingBin.Aggregate(string.Empty, (current, c) => current + currentBin[int.Parse(c)]);

    var uIntBin = (uint)int.Parse(newBin);

    Prime.Services.ConsoleService.PrintDebug($"Converting Softbin=[{results.CurrentSoftBin}] to New Softbin=[{uIntBin}]");
    results.ExitPort = TestMethodPassPort;
    return uIntBin;
}
```

## OTPL Sample1:
``` Perl
Test PrimeOdeseBinConverterTestMethod OdeseRemap_P1
{
	LogLevel = "PRIME_DEBUG";
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
### Related Test Methods
* None


### Related Services

## Version tracking

| **Date**          | **Version** | **Author**    | **Comments**                                     |
| ----------------- | ----------- | ------------- | ------------------------------------------------ |
| September 7, 2021 | 1.0.0       | hramirez      | Initial doc                                      |     
| April 7, 2022     | 1.1.0       | Kevin D Krake | Adding new Results method for exit port handling |
| June 1, 2022     | 1.2.0       | hramirez | adding support of uservars for 4-digit index |  