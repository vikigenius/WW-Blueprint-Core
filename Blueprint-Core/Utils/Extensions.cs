using System;

namespace BlueprintCore.Utils
{
  /** Otherwise uncategorized extension methods are contained here. */
  public static class ExtensionMethods
  {
    /**
     * Remember this does not modify the array in-place. Typical usage:
     * `array = array.AppendtoArray(otherArray)`
     */
    public static T[] AppendToArray<T>(this T[] array, params T[] values)
    {
      if (values == null) { return array; }
      var len = array.Length;
      var result = new T[len + values.Length];
      Array.Copy(array, result, len);
      Array.Copy(values, 0, result, len, values.Length);
      return result;
    }
  }
}