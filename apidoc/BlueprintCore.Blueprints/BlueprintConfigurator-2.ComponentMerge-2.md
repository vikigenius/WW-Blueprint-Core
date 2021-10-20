# BlueprintConfigurator&lt;T,TBuilder&gt;.ComponentMerge&lt;T,TBuilder&gt; enumeration

Describes how to resolve conflicts when multiple unique components are added to a blueprint.

```csharp
public enum ComponentMerge<T, TBuilder>
    where T : BlueprintScriptableObject
    where TBuilder : BlueprintConfigurator<T, TBuilder>
```

## Values

| name | value | description |
| --- | --- | --- |
| Fail | `0` | Default. Throws an exception. |
| Skip | `1` | Skips the new component. Useful for components without per-instance behavior. |
| Replace | `2` | The new component is used and the existing component is removed. |
| Merge | `3` | The two components are merged into one. Requires an Action to define merge behavior. |

## Remarks

When adding a component that is unique, the function accepts a [`ComponentMerge`](./BlueprintConfigurator-2.ComponentMerge-2.md) and Action argument to resolve the conflict. Whenever possible, a reasonable default behavior is provided. Usually this is in the form of concatenating two components that represent lists or combining flags.

## See Also

* class [BlueprintConfigurator&lt;T,TBuilder&gt;](./BlueprintConfigurator-2.md)
* namespace [BlueprintCore.Blueprints](../Blueprint-Core.md)

<!-- DO NOT EDIT: generated by xmldocmd for Blueprint-Core.dll -->