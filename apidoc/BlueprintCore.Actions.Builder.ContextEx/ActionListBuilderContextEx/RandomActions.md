# ActionListBuilderContextEx.RandomActions method

Adds ContextActionRandomize

```csharp
public static ActionListBuilder RandomActions(this ActionListBuilder builder, 
    params (ActionListBuilder actions, int weight)[] weightedActions)
```

| parameter | description |
| --- | --- |
| weightedActions | Pair of [`ActionListBuilder`](../../BlueprintCore.Actions.Builder/ActionListBuilder.md) and an int representing the relative probability of that action compared to the rest of the entries. These map to ActionWrapper. |

## See Also

* class [ActionListBuilder](../../BlueprintCore.Actions.Builder/ActionListBuilder.md)
* class [ActionListBuilderContextEx](../ActionListBuilderContextEx.md)
* namespace [BlueprintCore.Actions.Builder.ContextEx](../../Blueprint-Core.md)

<!-- DO NOT EDIT: generated by xmldocmd for Blueprint-Core.dll -->