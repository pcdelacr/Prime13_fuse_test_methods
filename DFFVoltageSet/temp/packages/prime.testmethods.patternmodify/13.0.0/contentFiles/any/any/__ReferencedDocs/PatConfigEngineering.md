
This Test Method was made to override patconfig configuration files from Aleph.

It is expected to provide a json file containing the configuration files.

The file will expect the following fields within the json file:

1. EngineeringConfiguration: provide this token as the indication of an engineering configuration override
2. Name: the override configuration name
3. Files: A list with the file paths to be loaded for override
4. Mode: Provides the engineering mode to be used from 2 possible:
   a. PRODUCTION_NORMAL: This is meant for production only, used only to reload the aleph files (updated)
   b. ENGINEERING_SAFE:  This is meant for engineering overrides, but not overriding any aleph configuration
   c. ENGINEERING_UNSAFE: This is meant to override any configuration, even aleph configurations.
   

__Json Example:__

```json
{
    "EngineeringConfiguration" :
    [
        {
            "Name" : "EngineeringOverride",
            "Files" : ["~HDMT_TPL_DIR/Modules/patternModify/PATTERNMODIFY/inputFiles/Engineering.test.patmod.json" ],
            "Mode" : "ENGINEERING_UNSAFE"
        }
    ]
}
```

The way the instance should be created is like this in the MTPL:

```
Test PrimePatConfigEngineeringTestMethod EngineeringOverride_P1
{
    EngineeringConfigurationFile = "~HDMT_TPL_DIR/Modules/patternModify/patternModify/inputFiles/engineeringFilesPass.json";
    LogLevel = "PRIME_DEBUG";
}
```


