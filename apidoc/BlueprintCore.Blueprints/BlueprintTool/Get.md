# BlueprintTool.Get&lt;T&gt; method

Returns the blueprint with the specified Name or Guid.

```csharp
public static T Get<T>(string nameOrGuid)
    where T : SimpleBlueprint
```

| parameter | description |
| --- | --- |
| nameOrGuid | Use Name if you have registered it using [`AddGuidsByName`](./AddGuidsByName.md) or Guid otherwise. |

## See Also

* class [BlueprintTool](../BlueprintTool.md)
* namespace [BlueprintCore.Blueprints](../../Blueprint-Core.md)

<!-- DO NOT EDIT: generated by xmldocmd for Blueprint-Core.dll -->