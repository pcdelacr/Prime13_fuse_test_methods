[[_TOC_]]

## Functional Test Interface with Capture Mode information.
### For HDMT and HBI
| Functional Test Interface                    | Capture Mode (HDMT) | What is capture | When captures are triggered | Stop Patgen/Continue to End |
|----------------------------------------------|---------------------|-----------------|-----------------------------|-----------------------------|
| CreateNoCaptureTest                          | CaptureDisabled     | Nothing         | Never                       | Continue to end             |
| CreateCaptureFailureTest(without perPatternCaptureCount) | CaptureNFails | Miscompare/Fail | on fail | Stop Patgen |
| CreateNoCaptureTest | CaptureDisabled | Nothing | Never | Continue to end
| CreateCaptureFailureTest(without perPatternCaptureCount) | CaptureNFails | Miscompare/Fail | on fail | Stop Patgen
| CreateCaptureFailureTest(with perPatternCaptureCount) | CaptureNFailsPerPattern | Miscompare/Fail | on fail | Continue to end

### For HDMT Only
| Functional Test Interface                    | Capture Mode (HDMT) | What is capture | When captures are triggered | Stop Patgen/Continue to End |
|----------------------------------------------|---------------------|-----------------|-----------------------------|-----------------------------|
| CreateCaptureCtvPerPinTest (For HDMT), <br> CreateCaptureCtvPerCycleTest (For HDMT) | CaptureCTV | Raw Miscompare/Fail | on CTV | Continue to end |
| CreateCaptureFailureAndCtvPerPinTest (without perPatternCaptureCount) (For HDMT), <br> CreateCaptureFailureAndCtvPerCycleTest (without perPatternCaptureCount) (For HDMT)  | CaptureNFailsAndCTV | Raw Miscompare/Fail | on fail and CTV | Stop Patgen |
| CreateCaptureFailureAndCtvPerPinTest (with perPatternCaptureCount) (For HDMT), <br> CreateCaptureFailureAndCtvPerCycleTest (with perPatternCaptureCount) (For HDMT), <br> CreateCaptureFailureAndCtvPerCycleTestForFailingPatterns (with perPatternCaptureCount) (For HDMT), <br> CreateCaptureFailureAndCtvPerCycleTestForFailingPatterns (without perPatternCaptureCount) (For HDMT) | CaptureCTVNFailsPerPattern | Raw Miscompare/Fail | on fail and CTV | Continue to end |

### For HBI Only
| Functional Test Interface                    | Capture Mode (HDMT) | What is capture | When captures are triggered | Stop Patgen/Continue to End |
|----------------------------------------------|---------------------|-----------------|-----------------------------|-----------------------------|
| CreateCaptureCtvPerPinTest (For HBI), <br> CreateCaptureCtvPerCycleTest (For HBI) | CapturePinStateForCTV | PinState | on CTV | Stop Patgen
| CreateCaptureFailureAndCtvPerPinTest (with perPatternCaptureCount) (For HBI), <br> CreateCaptureFailureAndCtvPerCycleTest (with perPatternCaptureCount) (For HBI), <br> CreateCaptureFailureAndCtvPerCycleTestForFailingPatterns (with perPatternCaptureCount) (For HBI), <br> CreateCaptureFailureAndCtvPerCycleTestForFailingPatterns (without perPatternCaptureCount) (For HBI) | CapturePinStateForCTVNFailsPerPattern | PinState | on fail and CTV | Continue to end | 
| CreateCaptureFailureAndCtvPerPinTest (without perPatternCaptureCount) (For HBI), <br> CreateCaptureFailureAndCtvPerCyclenTest (without perPatternCaptureCount) (For HBI) | CapturePinStateforCTVNFails | PinState | on fail and CTV | Stop Patgen |

### Unused Capture Mode
| Functional Test Interface                    | Capture Mode (HDMT)          | What is capture     | When captures are triggered | Stop Patgen/Continue to End |
|----------------------------------------------|------------------------------|---------------------|-----------------------------|-----------------------------|
|                                              | CaptureFirstFail             | Miscompare/Fail     | on fail                     | Stop Patgen                 |
|                                              | CaptureFirstFailAndCTV       | Raw Miscompare/Fail | on fail and CTV             | Stop Patgen                 |
|                                              | CapturePinStateAndFailCycles | PinState            | on every cycle              | Continue to end             |