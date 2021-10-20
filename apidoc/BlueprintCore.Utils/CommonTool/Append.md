# CommonTool.Append&lt;T&gt; method

Returns an array with the given values appended.

```csharp
public static T[] Append<T>(T[] array, params T[] values)
```

## Remarks

Remember that this does not do an in-place modification of the array. Typical usage:

```csharp
myArray = myArray.AppendToArray(otherArray);
```

## See Also

* class [CommonTool](../CommonTool.md)
* namespace [BlueprintCore.Utils](../../Blueprint-Core.md)

<!-- DO NOT EDIT: generated by xmldocmd for Blueprint-Core.dll -->