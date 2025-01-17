﻿using BlueprintCore.Actions.Builder;
using BlueprintCore.Blueprints.Configurators;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.ElementsSystem;
using Kingmaker.Localization;
using Kingmaker.ResourceLinks;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BlueprintCoreGen.CodeGen
{

  /// <summary>
  /// Represents a field modified in a generated method.
  /// </summary>
  public interface IField
  {
    /// <summary>
    /// Name of the field in source.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Processed type name.
    /// </summary>
    string TypeName { get; }

    /// <summary>
    /// Name of the field's method parameter.
    /// </summary>
    /// 
    /// <remarks>If this is null it should be assigned last, as it may rely on other fields being configured.</remarks>
    string ParamName { get; }

    /// <summary>
    /// Name of the field for use in method name.
    /// </summary>
    string MethodName { get; }

    /// <summary>
    /// Optional parameter comment.
    /// </summary>
    string? Comment { get; }

    /// <summary>
    /// Optional default parameter value.
    /// </summary>
    string? DefaultValue { get; }

    /// <summary>
    /// Indicates if a field can be validated.
    /// </summary>
    bool ShouldValidate { get; }

    /// <summary>
    /// Returns a complete statement which sets the field's contents.
    /// </summary>
    /// <param name="objectName">The variable name of the object containing the field.</param>
    List<string> GetAssignment(string objectName);

    /// <summary>
    /// List of types to import.
    /// </summary>
    HashSet<Type> Imports { get; }
  }

  public interface IEnumerableField : IField
  {
    /// <summary>
    /// Processed type name of the field's enumerable type.
    /// </summary>
    string EnumerableTypeName { get; }

    /// <summary>
    /// Returns a complete statement which adds to the field's contents.
    /// </summary>
    /// <param name="objectName">The variable name of the object containing the field.</param>
    List<string> GetAddTo(string objectName);

    /// <summary>
    /// Returns a complete statement which removes from the field's contents.
    /// </summary>
    /// <param name="objectName">The variable name of the object containing the field.</param>
    List<string> GetRemoveFrom(string objectName);
  }

  public static class FieldFactory
  {
    public static IField? Create(FieldInfo info, Type sourceType)
    {
      return CreateInternal(info, sourceType);
      //IField field = CreateInternal(info, sourceType);
      //if (field is null) { return null; }
      //return FieldOverrides.GetFor(field, sourceType);
    }

    private static IField? CreateInternal(FieldInfo info, Type sourceType)
    {
      if (IsIgnoredField(info, sourceType)) { return null; }

      if (DefaultEmptyField.TypeToEmptyConstant.ContainsKey(info.FieldType))
      {
        return new DefaultEmptyField(info);
      }

      var enumerableType = GetEnumerableType(info.FieldType);
      if (enumerableType is not null && !IsIgnoredEnumerableType(info.FieldType))
      {
        if (enumerableType.IsSubclassOf(typeof(BlueprintReferenceBase)))
        {
          return new EnumerableBlueprintField(info, enumerableType);
        }
        return new EnumerableField(info, enumerableType);
      }
      if (info.FieldType.IsSubclassOf(typeof(BlueprintReferenceBase)))
      {
        return new BlueprintField(info, info.FieldType);
      }
      if (info.FieldType == typeof(ActionList))
      {
        return new BuilderField(info, BuilderField.BuilderType.Actions);
      }
      if (info.FieldType == typeof(ConditionsChecker))
      {
        return new BuilderField(info, BuilderField.BuilderType.Conditions);
      }
      if (sourceType.IsSubclassOf(typeof(Condition)) && info.Name == "Not")
      {
        return new NegateConditionField();
      }
      return new Field(info);
    }

    private static List<IField> UniqueComponentFields = new() { new MergeBehaviorField(), new MergeActionField() };
    public static List<IField> CreateUniqueComponentFields()
    {
      return UniqueComponentFields;
    }

    private static bool IsIgnoredField(FieldInfo field, Type sourceType)
    {
      foreach (var (type, fieldNames) in Overrides.IgnoredFieldNamesByType)
      {
        if (sourceType.IsSubclassOf(type) || sourceType == type)
        {
          if (fieldNames.Contains(field.Name)) { return true; }
        }
      }

      return field.Name.Contains("__BackingField") // Compiler generated field
          // Skip constant, static, and read-only
          || field.IsLiteral
          || field.IsStatic
          || field.IsInitOnly;
    }

    private static bool IsIgnoredEnumerableType(Type fieldType)
    {
      return fieldType == typeof(string)
          // For now just skip dictionaries
          || fieldType.GetInterfaces()
              .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDictionary<,>))
              .Any()
          // Maybe this type can be added later
          || fieldType.IsSubclassOf(typeof(TagListBase));
    }

    private static Type? GetEnumerableType(Type type)
    {
      return type.GetInterfaces()
          .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
          ?.GetGenericArguments()[0];
    }

    private class Field : IField
    {
      public string Name { get; private set; }
      public string TypeName { get; protected set; }
      public string MethodName { get; private set; }
      public string ParamName { get; private set; }
      public string? Comment { get; protected set; }
      public string? DefaultValue { get; protected set; }
      public bool ShouldValidate { get; protected set; }
      public HashSet<Type> Imports { get; private set; }

      public Field(FieldInfo info)
      {
        Name = info.Name;
        TypeName = IsNullable() ? $"{TypeTool.GetName(info.FieldType)}?" : TypeTool.GetName(info.FieldType);
        ParamName = GetFriendlyName(Name, lowercase: true);
        MethodName = GetFriendlyName(Name, lowercase: false);

        ShouldValidate = GetShouldValidate(info.FieldType);
        DefaultValue = GetDefaultValue(info.FieldType);

        Imports = new();
        Imports.Add(typeof(Constants));
        PopulateImports(info.FieldType, Imports);
      }

      public virtual List<string> GetAssignment(string objectName)
      {
        return new() { $"{objectName}.{Name} = {ParamName};" };
      }

      // Flags items with null default values to address nullable warnings. Not necessary for nullable value types.
      protected virtual bool IsNullable()
      {
        return false;
      }

      protected static bool GetShouldValidate(Type type)
      {
        if (type.IsPrimitive)
        {
          return false;
        }
        if (type == typeof(string))
        {
          return false;
        }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
          return false;
        }
        if (type.IsValueType)
        {
          return false;
        }
        return true;
      }

      private static void PopulateImports(Type type, HashSet<Type> imports)
      {
        imports.Add(type);
        if (!type.IsGenericType)
        {
          return;
        }
        type.GetGenericArguments().ToList().ForEach(t => PopulateImports(t, imports));
      }

      private static string? GetDefaultValue(Type type)
      {
        if (type.IsPrimitive || type.IsEnum) { return "default"; }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) { return "null"; }
        return null;
      }

      private static string GetFriendlyName(string fieldName, bool lowercase)
      {
        StringBuilder friendlyName = new(fieldName);
        if (friendlyName[0] == 'm' && friendlyName[1] == '_')
        {
          friendlyName.Remove(0, 2);
        }
        friendlyName[0] = lowercase ? char.ToLower(friendlyName[0]) : char.ToUpper(friendlyName[0]);
        
        var result = friendlyName.ToString();
        if (Overrides.FriendlyNameOverrides.ContainsKey(result)) { return Overrides.FriendlyNameOverrides[result]; }
        return result;
      }
    }

    private class DefaultEmptyField : Field
    {
      private readonly string EmptyConstant;

      public static readonly Dictionary<Type, string> TypeToEmptyConstant =
          new()
          {
            { typeof(PrefabLink), "Constants.Empty.PrefabLink" },
            { typeof(LocalizedString), "Constants.Empty.String" },
            { typeof(ContextValue), "ContextValues.Constant(0)" },
            { typeof(ContextDiceValue), "Constants.Empty.DiceValue" }
          };
      public DefaultEmptyField(FieldInfo info) : base(info)
      {
        DefaultValue = "null";
        EmptyConstant = TypeToEmptyConstant[info.FieldType];

        Imports.Add(typeof(Constants));
        Imports.Add(typeof(ContextValues));
      }

      public override List<string> GetAssignment(string objectName)
      {
        return new() { $"{objectName}.{Name} = {ParamName} ?? {EmptyConstant};" };
      }

      protected override bool IsNullable()
      {
        return true;
      }
    }

    private class NegateConditionField : IField
    {
      public string Name => "Not";
      public string TypeName => "bool";
      public string ParamName => "negate";
      public string MethodName => throw new NotImplementedException();
      public string? Comment => null;
      public string DefaultValue => "false";
      public bool ShouldValidate => false;
      public HashSet<Type> Imports => new();

      public List<string> GetAssignment(string objectName)
      {
        return new() { $"{objectName}.Not = negate;" };
      }
    }

    private class MergeBehaviorField : IField
    {
      public string Name => null!;
      public virtual string TypeName => "ComponentMerge";
      public virtual string ParamName => "mergeBehavior";
      public string MethodName => null!;
      public string? Comment => null;
      public virtual string DefaultValue => "ComponentMerge.Replace";
      public bool ShouldValidate => false;
      public HashSet<Type> Imports =>
          new() { typeof(BlueprintConfigurator<>), typeof(Action), typeof(BlueprintComponent) };

      public List<string> GetAssignment(string objectName)
      {
        return new();
      }
    }

    private class MergeActionField : MergeBehaviorField
    {
      public override string TypeName => "Action<BlueprintComponent, BlueprintComponent>?";
      public override string ParamName => "mergeAction";
      public override string DefaultValue => "null";
    }

    private class BuilderField : Field
    {
      public enum BuilderType
      {
        Actions,
        Conditions
      }
      private readonly BuilderType Type;

      public BuilderField(FieldInfo info, BuilderType builderType) : base(info)
      {
        Type = builderType;
        DefaultValue = "null";

        ShouldValidate = false;

        Imports.Add(typeof(Constants));
        switch (Type)
        {
          case BuilderType.Actions:
            TypeName = "ActionsBuilder?";
            Imports.Add(typeof(ActionsBuilder));
            break;
          case BuilderType.Conditions:
            TypeName = "ConditionsBuilder?";
            Imports.Add(typeof(ConditionsBuilder));
            break;
          default:
            throw new InvalidOperationException("Unsupported builder type: " + info.FieldType.Name);
        }
      }

      public override List<string> GetAssignment(string objectName)
      {
        return new() { $"{objectName}.{Name} = {ParamName}?.Build() ?? Constants.Empty.{Type};" };
      }
    }

    private class EnumerableField : Field, IEnumerableField
    {
      public string EnumerableTypeName { get; private set; }
      private readonly bool IsArray;
      private readonly string ToEnumerable;
      private readonly string InternalTypeName;

      public EnumerableField(FieldInfo info, Type enumerableType) : base(info)
      {
        InternalTypeName = TypeTool.GetName(info.FieldType);
        EnumerableTypeName = TypeTool.GetName(enumerableType);
        IsArray = info.FieldType.IsArray;
        ToEnumerable = IsArray ? "ToArray()" : "ToList()";

        DefaultValue = "null";

        ShouldValidate = GetShouldValidate(enumerableType);

        Imports.Add(typeof(Enumerable));
        Imports.Add(typeof(CommonTool));
      }

      protected override bool IsNullable()
      {
        return true;
      }

      public List<string> GetAddTo(string objectName)
      {
        return
            new()
            {
              IsArray
                  ? $"{objectName}.{Name} = CommonTool.Append({objectName}.{Name}, {ParamName} ?? new {EnumerableTypeName}[0]);"
                  : $"{objectName}.{Name}.AddRange({ParamName}.ToList() ?? new {InternalTypeName}());"
            };
      }

      public List<string> GetRemoveFrom(string objectName)
      {
        return
            new()
            {
              $"{objectName}.{Name} = {objectName}.{Name}.Where(item => !{ParamName}.Contains(item)).{ToEnumerable};"
            };
      }
    }

    private class BlueprintField : Field
    {
      protected readonly string ReferenceTypeName;

      public BlueprintField(FieldInfo info, Type referenceType) : base(info)
      {
        TypeName = "string?";
        ReferenceTypeName = TypeTool.GetName(referenceType);

        var blueprintType = GetBlueprintTypeFromReferenceType(referenceType);
        var blueprintTypeName = TypeTool.GetName(blueprintType);
        Comment = $"<param name=\"{ParamName}\"><see cref=\"{blueprintType.Namespace}.{blueprintTypeName}\"/></param>";

        DefaultValue = "null";

        ShouldValidate = false;
        Imports.Add(typeof(BlueprintTool));
      }

      public override List<string> GetAssignment(string objectName)
      {
        return new() { $"{objectName}.{Name} = BlueprintTool.GetRef<{ReferenceTypeName}>({ParamName});" };
      }

      private static Type GetBlueprintTypeFromReferenceType(Type referenceType)
      {
        var refType = referenceType;
        while (!(refType!.IsGenericType && refType.GetGenericTypeDefinition() == typeof(BlueprintReference<>)))
        {
          refType = refType.BaseType;
        }
        return refType.GenericTypeArguments[0];
      }
    }

    private class EnumerableBlueprintField : BlueprintField, IEnumerableField
    {
      public string EnumerableTypeName { get; private set; }
      private readonly bool IsArray;
      private readonly string ToEnumerable;
      private readonly string ReferenceConversion;

      public EnumerableBlueprintField(FieldInfo info, Type referenceType) : base(info, referenceType)
      {
        TypeName = "string[]?";
        EnumerableTypeName = "string";

        IsArray = info.FieldType.IsArray;
        ToEnumerable = IsArray ? "ToArray()" : "ToList()";
        ReferenceConversion = $"{ParamName}.Select(name => BlueprintTool.GetRef<{ReferenceTypeName}>(name))";

        Imports.Add(typeof(Enumerable));
        Imports.Add(typeof(CommonTool));
      }

      public override List<string> GetAssignment(string objectName)
      {
        return new() { $"{objectName}.{Name} = {ReferenceConversion}.{ToEnumerable};" };
      }

      public List<string> GetAddTo(string objectName)
      {
        return
            new()
            {
              IsArray
                  ? $"{objectName}.{Name} = CommonTool.Append({objectName}.{Name}, {ReferenceConversion}.ToArray());"
                  : $"{objectName}.{Name}.AddRange({ReferenceConversion});"
            };
      }

      public List<string> GetRemoveFrom(string objectName)
      {
        return
            new()
            {
              $"var excludeRefs = {ReferenceConversion};",
              $"{objectName}.{Name} =",
              $"    {objectName}.{Name}",
              $"        .Where(",
              $"            bpRef => !excludeRefs.ToList().Exists(exclude => bpRef.deserializedGuid == exclude.deserializedGuid))",
              $"        .{ToEnumerable};"
            };
      }
    }
  }
}
