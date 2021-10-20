# BlueprintConfigurator&lt;T,TBuilder&gt;.Configure method

Commits the configuration changes to the blueprint.

```csharp
public T Configure()
```

## Remarks

After commiting the changes the blueprint is validated and any errors are logged as a warning.

Throws InvalidOperationException if called twice on the same configurator.

## See Also

* class [BlueprintConfigurator&lt;T,TBuilder&gt;](../BlueprintConfigurator-2.md)
* namespace [BlueprintCore.Blueprints](../../Blueprint-Core.md)

<!-- DO NOT EDIT: generated by xmldocmd for Blueprint-Core.dll -->