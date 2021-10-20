# ProgressionExtensions.DivideByThenAdd1 method

`Result = 1 + Max((BaseValue - Start) / Divisor, 0)` OR`Result = 0`, if `delayStart` is `true` and `BaseValue < Start`

```csharp
public static ContextRankConfig DivideByThenAdd1(this ContextRankConfig config, int divisor, 
    int start = 0, bool delayStart = false)
```

## Remarks

Implements StartPlusDivStep, DelayedStartPlusDivStep, and OnePlusDivStep

## See Also

* class [ProgressionExtensions](../ProgressionExtensions.md)
* namespace [BlueprintCore.Blueprints.Components](../../Blueprint-Core.md)

<!-- DO NOT EDIT: generated by xmldocmd for Blueprint-Core.dll -->